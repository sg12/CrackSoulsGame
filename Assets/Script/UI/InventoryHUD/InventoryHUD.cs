using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using RPGAE.CharacterController;
using TMPro;
using JoshH.UI;

public class InventoryHUD : MonoBehaviour
{
    [Header("SETTINGS")]
    [Tooltip("Set starting page index - starting from 0")]
    public int startingSlot = 0;
    [Tooltip("How fast will page lerp to target position")]
    public float decelerationRate = 10f;
    [Tooltip("Container with page images (optional)")]
    public Transform slotSelectionIcons;
    public Color unselectedColor;
    public Color selectedColor;

    [Header("REFERENCES")]
    public TextMeshProUGUI itemName;
    public GameObject inventorySlotHUDPrefab;
    public Animator outLineSelect;
    public FadeUI fadeUI;
    public FadeUI KeyboardAlpha;
    public FadeUI XboxAlpha;
    public FadeUI pSAlpha;

    private ScrollRect m_scrollRect;
    private RectTransform m_scrollRectTransform;
    private RectTransform m_content;

    private WeaponHolster wpnHolster;

    private bool m_horizontal;

    // number of pages in container
    public int m_slotCount;
    private int m_currentSlot;

    // whether lerping is in progress and target lerp position
    private bool m_lerp;
    private Vector2 mm_lerpTo;

    // target position of every page
    private List<Vector2> m_slotPositions = new List<Vector2>();

    // for showing small page icons
    private bool m_showSlotSelection;
    private int m_previousSlotSelectionIndex;
    // container with Image components - one Image for each page
    private List<Image> _slotSelectionImages;

    public InventorySlotHUD inventorySlotHUD;
    private ThirdPersonController cc;
    private InventoryManager inventoryM;
    private HUDManager hudManager;

    public bool oneShot = false;
    public int slotNum;


    public InventorySlotHUD[] slots;
    public bool isActive;

    // Start is called before the first frame update
    void Start()
    {
        m_scrollRect = GetComponent<ScrollRect>();
        m_scrollRectTransform = GetComponent<RectTransform>();
        cc = FindObjectOfType<ThirdPersonController>();
        inventoryM = FindObjectOfType<InventoryManager>();
        hudManager = FindObjectOfType<HUDManager>();
        wpnHolster = GameObject.Find("PlayerHolster").GetComponent<WeaponHolster>();
        fadeUI = GetComponent<FadeUI>();

        cc.rpgaeIM.PlayerControls.Movement.performed += ctx => {
            if (cc.rpgaeIM.PlayerControls.Movement.ReadValue<Vector2>().x < -0.1f && m_slotCount > 0)
                PreviousScreen();
            else if (cc.rpgaeIM.PlayerControls.Movement.ReadValue<Vector2>().x > 0.1f && m_slotCount > 0)
                NextScreen();
        };
    }

    void InitSlots()
    {
        if (oneShot)
        {
            m_content = m_scrollRect.content;
            m_slotCount = m_content.childCount;

            // is it horizontal or vertical scrollrect
            if (m_scrollRect.horizontal && !m_scrollRect.vertical)
            {
                m_horizontal = true;
            }
            else if (!m_scrollRect.horizontal && m_scrollRect.vertical)
            {
                m_horizontal = false;
            }
            else
            {
                Debug.LogWarning("Confusing setting of horizontal/vertical direction. Default set to horizontal.");
                m_horizontal = true;
            }

            m_lerp = false;

            // init
            SetPagePositions();
            SetPage(startingSlot);
            InitPageSelection();
            SetPageSelection(startingSlot);
            oneShot = false;
        }

        if (m_slotCount > 0)
        {
            // if moving to target position
            if (m_lerp)
            {
                // prevent overshooting with values greater than 1
                float decelerate = Mathf.Min(decelerationRate * Time.deltaTime, 1f);
                m_content.anchoredPosition = Vector2.Lerp(m_content.anchoredPosition, mm_lerpTo, decelerate);
                // time to stop lerping?
                if (Vector2.SqrMagnitude(m_content.anchoredPosition - mm_lerpTo) < 0.25f)
                {
                    // snap to target and stop lerping
                    m_content.anchoredPosition = mm_lerpTo;
                    m_lerp = false;
                    // clear also any scrollrect move that may interfere with our lerping
                    m_scrollRect.velocity = Vector2.zero;
                }

                // switches selection icon exactly to correct page
                if (m_showSlotSelection)
                {
                    SetPageSelection(GetNearestPage());
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        InitSlots();
        EquipItem();
        ControllerButtonIcon();
        SetInventoryHUDSlots();
        ClearInventoryHUDSlotsOnClose();
    }

    void SetInventoryHUDSlots()
    {
        if (inventoryM.isPauseMenuOn) return;

        if (cc.rpgaeIM.PlayerControls.InventoryHUD1.triggered && fadeUI.canvasGroup.alpha == 0)
        {
            if (hudManager.swordHUD)
            {
                isActive = true;
                inventoryM.FreezeWorld(true);
                cc.tpCam.cameraLocked = true;
                fadeUI.FadeTransition(1, 0, 0.25f);
                CreateInventorySlotHUD(ref inventoryM.weaponInv.itemData, ItemData.ItemType.Weapon, ref inventoryM.weaponInv.image,
                ref inventoryM.weaponInv.highLight, ref inventoryM.weaponInv.statValueBG, ref inventoryM.weaponInv.statValue,
                ref inventoryM.weaponInv.quantity, ref inventoryM.weaponInv.counter);
                oneShot = true;
            }
            if (hudManager.bowHUD)
            {
                isActive = true;
                inventoryM.FreezeWorld(true);
                cc.tpCam.cameraLocked = true;
                fadeUI.FadeTransition(1, 0, 0.25f);
                CreateInventorySlotHUD(ref inventoryM.bowAndArrowInv.itemData, ItemData.ItemType.Bow, ref inventoryM.bowAndArrowInv.image,
                ref inventoryM.bowAndArrowInv.highLight, ref inventoryM.bowAndArrowInv.statValueBG, ref inventoryM.bowAndArrowInv.statValue,
                ref inventoryM.bowAndArrowInv.quantity, ref inventoryM.bowAndArrowInv.counter);
                oneShot = true;
            }
        }
        else if (cc.rpgaeIM.PlayerControls.InventoryHUD2.triggered && fadeUI.canvasGroup.alpha == 0)
        {
            if (hudManager.shieldHUD)
            {
                isActive = true;
                inventoryM.FreezeWorld(true);
                cc.tpCam.cameraLocked = true;
                fadeUI.FadeTransition(1, 0, 0.25f);
                CreateInventorySlotHUD(ref inventoryM.shieldInv.itemData, ItemData.ItemType.Shield, ref inventoryM.shieldInv.image, 
                ref inventoryM.shieldInv.highLight, ref inventoryM.shieldInv.statValueBG, ref inventoryM.shieldInv.statValue, 
                ref inventoryM.shieldInv.quantity, ref inventoryM.shieldInv.counter);
                oneShot = true;
            }
            else if (hudManager.arrowHUD)
            {
                isActive = true;
                inventoryM.FreezeWorld(true);
                cc.tpCam.cameraLocked = true;
                fadeUI.FadeTransition(1, 0, 0.25f);
                CreateInventorySlotHUD(ref inventoryM.bowAndArrowInv.itemData, ItemData.ItemType.Arrow, ref inventoryM.bowAndArrowInv.image,
                ref inventoryM.bowAndArrowInv.highLight, ref inventoryM.bowAndArrowInv.statValueBG, ref inventoryM.bowAndArrowInv.statValue,
                ref inventoryM.bowAndArrowInv.quantity, ref inventoryM.bowAndArrowInv.counter);
                oneShot = true;
            }
        }

        if (cc.rpgaeIM.PlayerControls.Start.triggered && fadeUI.canvasGroup.alpha == 1)
        {
            isActive = false;
            inventoryM.FreezeWorld(false);
            cc.tpCam.cameraLocked = false;
            fadeUI.FadeTransition(0, 0, 0.25f);
        }
    }

    void EquipItem()
    {
        if (inventoryM.isPauseMenuOn) return;

        if (cc.rpgaeIM.PlayerControls.Action.triggered && fadeUI.canvasGroup.alpha == 1 && inventorySlotHUD.itemData)
        {
            if(inventorySlotHUD.itemData.itemType == ItemData.ItemType.Weapon)
            {
                #region Check If Broken

                if (inventorySlotHUD.itemData.broken && inventoryM.itemRepair)
                {
                    cc.infoMessage.info.text = "You cannot equip a broken weapon.";
                    return;
                }

                #endregion

                ClearAndSet(ref inventoryM.weaponInv.itemData, ref inventoryM.weaponInv.outLineBorder);

                if (inventorySlotHUD.itemData.itemName == "Windscar" + inventorySlotHUD.itemData.rankTag)
                {
                    DeactiveWeapons();
                    DeactiveWeaponsHP();
                    wpnHolster.windScarNoParticlesH.SetActive(true);
                    wpnHolster.windScarNoParticlesEP.SetActive(true);
                    wpnHolster.primaryEP = wpnHolster.windScarNoParticlesEP;
                    wpnHolster.swordHP = wpnHolster.windScarNoParticlesHP;
                    wpnHolster.swordEP = wpnHolster.windScarNoParticlesEP;
                    wpnHolster.primaryH = wpnHolster.windScarNoParticlesH;
                    wpnHolster.primaryE = wpnHolster.windScarNoParticleE;
                    wpnHolster.primaryD = wpnHolster.windScarNoParticlePrefab;
                    wpnHolster.alteredPrimaryE = wpnHolster.windScarE;
                    wpnHolster.alteredPrimaryEP = wpnHolster.windScarEP;
                    wpnHolster.alteredPrimaryHP = wpnHolster.windScarHP;
                    wpnHolster.SetItemData(ref wpnHolster.primaryE, ref inventorySlotHUD.itemData);
                    inventoryM.preSwordEquipped = true;
                }

                if (inventorySlotHUD.itemData.itemName == "The Tuning fork" + inventorySlotHUD.itemData.rankTag)
                {
                    DeactiveWeapons();
                    DeactiveWeaponsHP();
                    wpnHolster.theTuningForkH.SetActive(true);
                    wpnHolster.theTuningForkEP.SetActive(true);
                    wpnHolster.primaryEP = wpnHolster.theTuningForkEP;
                    wpnHolster.dSwordHP = wpnHolster.theTuningForkHP;
                    wpnHolster.dSwordEP = wpnHolster.theTuningForkEP;
                    wpnHolster.primaryH = wpnHolster.theTuningForkH;
                    wpnHolster.primaryE = wpnHolster.theTuningForkE;
                    wpnHolster.primaryD = wpnHolster.theTuningForkPrefab;
                    wpnHolster.SetItemData(ref wpnHolster.primaryE, ref inventorySlotHUD.itemData);
                    inventoryM.pre2HSwordEquipped = true;
                }

                if (inventorySlotHUD.itemData.itemName == "Assassin Dagger" + inventorySlotHUD.itemData.rankTag)
                {
                    DeactiveWeapons();
                    DeactiveWeaponsHP();
                    wpnHolster.assasinsDaggerH.SetActive(true);
                    wpnHolster.assasinsDaggerEP.SetActive(true);
                    wpnHolster.primaryEP = wpnHolster.assasinsDaggerEP;
                    wpnHolster.swordHP = wpnHolster.assasinsDaggerHP;
                    wpnHolster.swordEP = wpnHolster.assasinsDaggerEP;
                    wpnHolster.primaryH = wpnHolster.assasinsDaggerH;
                    wpnHolster.primaryE = wpnHolster.assasinsDaggerE;
                    wpnHolster.primaryD = wpnHolster.assasinsDaggerPrefab;
                    wpnHolster.SetItemData(ref wpnHolster.primaryE, ref inventorySlotHUD.itemData);
                    inventoryM.preDaggerEquipped = true;
                }

                if (inventorySlotHUD.itemData.itemName == "Cleric's Staff" + inventorySlotHUD.itemData.rankTag)
                {
                    DeactiveWeapons();
                    DeactiveWeaponsHP();
                    wpnHolster.clericsStaffH.SetActive(true);
                    wpnHolster.clericsStaffEP.SetActive(true);
                    wpnHolster.primaryEP = wpnHolster.clericsStaffEP;
                    wpnHolster.staffHP = wpnHolster.clericsStaffHP;
                    wpnHolster.staffEP = wpnHolster.clericsStaffEP;
                    wpnHolster.primaryH = wpnHolster.clericsStaffH;
                    wpnHolster.primaryE = wpnHolster.clericsStaffE;
                    wpnHolster.primaryD = wpnHolster.clericsStaffPrefab;
                    wpnHolster.SetItemData(ref wpnHolster.primaryE, ref inventorySlotHUD.itemData);
                    inventoryM.preStaffEquipped = true;
                }

                if (inventorySlotHUD.itemData.itemName == "Glaive" + inventorySlotHUD.itemData.rankTag)
                {
                    DeactiveWeapons();
                    DeactiveWeaponsHP();
                    wpnHolster.glaiveH.SetActive(true);
                    wpnHolster.glaiveEP.SetActive(true);
                    wpnHolster.primaryEP = wpnHolster.glaiveEP;
                    wpnHolster.spearHP = wpnHolster.glaiveHP;
                    wpnHolster.spearEP = wpnHolster.glaiveEP;
                    wpnHolster.primaryH = wpnHolster.glaiveH;
                    wpnHolster.primaryE = wpnHolster.glaiveE;
                    wpnHolster.primaryD = wpnHolster.glaivePrefab;
                    wpnHolster.SetItemData(ref wpnHolster.primaryE, ref inventorySlotHUD.itemData);
                    inventoryM.preSpearEquipped = true;
                }

                if (inventorySlotHUD.itemData.itemName == "Obsidian Fury" + inventorySlotHUD.itemData.rankTag)
                {
                    DeactiveWeapons();
                    DeactiveWeaponsHP();
                    wpnHolster.obsidianFuryH.SetActive(true);
                    wpnHolster.obsidianFuryEP.SetActive(true);
                    wpnHolster.primaryEP = wpnHolster.obsidianFuryEP;
                    wpnHolster.hammerHP = wpnHolster.obsidianFuryHP;
                    wpnHolster.hammerEP = wpnHolster.obsidianFuryEP;
                    wpnHolster.primaryH = wpnHolster.obsidianFuryH;
                    wpnHolster.primaryE = wpnHolster.obsidianFuryE;
                    wpnHolster.primaryD = wpnHolster.obsidianFuryPrefab;
                    wpnHolster.SetItemData(ref wpnHolster.primaryE, ref inventorySlotHUD.itemData);
                    inventoryM.pre2HSwordEquipped = true;
                }
                wpnHolster.primaryE.SetActive(false);
            }

            if (inventorySlotHUD.itemData.itemType == ItemData.ItemType.Bow)
            {
                CheckEquippedItems(ref inventoryM.bowAndArrowInv.itemData, ref inventoryM.bowAndArrowInv.outLineBorder);

                if (inventorySlotHUD.itemData.itemName == "Warbow" + inventorySlotHUD.itemData.rankTag)
                {
                    DeactiveBows();
                    DeactiveBowsHP();
                    wpnHolster.warbowH.SetActive(true);
                    wpnHolster.warbowEP.SetActive(true);
                    wpnHolster.quiverHP.SetActive(true);
                    wpnHolster.quiverH.SetActive(true);
                    wpnHolster.bowHP = wpnHolster.warbowHP;
                    wpnHolster.bowEP = wpnHolster.warbowEP;
                    wpnHolster.bowString = wpnHolster.warbowString;
                    wpnHolster.arrowPrefabSpot = wpnHolster.warbowPrefabSpot;
                    wpnHolster.secondaryH = wpnHolster.warbowH;
                    wpnHolster.secondaryE = wpnHolster.warbowE;
                    cc.bowAnim = wpnHolster.secondaryE.GetComponent<Animator>();
                    wpnHolster.secondaryD = wpnHolster.warbowPrefab;
                    wpnHolster.SetItemData(ref wpnHolster.secondaryE, ref inventorySlotHUD.itemData);
                    inventoryM.preBowEquipped = true;
                }
                wpnHolster.secondaryE.SetActive(false);
            }

            if (inventorySlotHUD.itemData.itemType == ItemData.ItemType.Arrow)
            {
                CheckEquippedItems(ref inventoryM.bowAndArrowInv.itemData, ref inventoryM.bowAndArrowInv.outLineBorder);

                if (inventorySlotHUD.itemData.itemName == "Common Arrow")
                {
                    DeactiveArrows();
                    DeactiveArrowsHP();
                    wpnHolster.commonArrowH.SetActive(true);
                    wpnHolster.commonArrowHP.SetActive(true);
                    wpnHolster.commonArrowEP.SetActive(true);
                    wpnHolster.arrowHP = wpnHolster.commonArrowHP;
                    wpnHolster.arrowEP = wpnHolster.commonArrowEP;
                    wpnHolster.arrowH = wpnHolster.commonArrowH;
                    wpnHolster.arrowE = wpnHolster.commonArrowE;
                    wpnHolster.arrowD = wpnHolster.commonArrowPrefab;
                    wpnHolster.arrowString = wpnHolster.commonArrowString;
                    wpnHolster.bowStrings.transform.SetParent(wpnHolster.bowString.transform);
                    wpnHolster.SetItemData(ref wpnHolster.arrowE, ref inventorySlotHUD.itemData);
                    wpnHolster.quiverArrows = wpnHolster.commonArrowH.GetComponentsInChildren<QuiverArrows>();
                    wpnHolster.SetActiveQuiverArrows();
                }

                if (inventorySlotHUD.itemData.itemName == "Particle Arrow")
                {
                    DeactiveArrows();
                    DeactiveArrowsHP();
                    wpnHolster.particleArrowH.SetActive(true);
                    wpnHolster.particleArrowHP.SetActive(true);
                    wpnHolster.particleArrowEP.SetActive(true);
                    wpnHolster.arrowHP = wpnHolster.particleArrowHP;
                    wpnHolster.arrowEP = wpnHolster.particleArrowEP;
                    wpnHolster.arrowH = wpnHolster.particleArrowH;
                    wpnHolster.arrowE = wpnHolster.particleArrowE;
                    wpnHolster.arrowD = wpnHolster.particleArrowPrefab;
                    wpnHolster.arrowString = wpnHolster.particleArrowString;
                    wpnHolster.bowStrings.transform.SetParent(wpnHolster.bowString.transform);
                    wpnHolster.SetItemData(ref wpnHolster.arrowE, ref inventorySlotHUD.itemData);
                    wpnHolster.quiverArrows = wpnHolster.particleArrowH.GetComponentsInChildren<QuiverArrows>();
                    wpnHolster.SetActiveQuiverArrows();
                }
                wpnHolster.arrowE.SetActive(false);
                wpnHolster.arrowString.SetActive(false);
            }

            if (inventorySlotHUD.itemData.itemType == ItemData.ItemType.Shield)
            {
                ClearAndSet(ref inventoryM.shieldInv.itemData, ref inventoryM.shieldInv.outLineBorder);

                if (inventorySlotHUD.itemData.itemName == "Circle Shield" + inventorySlotHUD.itemData.rankTag)
                {
                    DeactiveShield();
                    DeactiveShieldHP();
                    wpnHolster.circleShieldH.SetActive(true);
                    wpnHolster.circleShieldEP.SetActive(true);
                    wpnHolster.shieldHP = wpnHolster.circleShieldHP;
                    wpnHolster.shieldP = wpnHolster.circleShieldEP;
                    wpnHolster.shieldH = wpnHolster.circleShieldH;
                    wpnHolster.shieldE = wpnHolster.circleShieldE;
                    wpnHolster.shieldD = wpnHolster.circleShieldPrefab;
                    wpnHolster.SetItemData(ref wpnHolster.shieldE, ref inventorySlotHUD.itemData);
                    inventoryM.preShieldEquipped = true;
                }
                wpnHolster.shieldE.SetActive(false);
            }
        }
    }

    void ClearAndSet(ref ItemData[] inventoryItemData, ref Image[] inventoryOutLine)
    {
        InventorySlotHUD[] slotHUD = FindObjectsOfType<InventorySlotHUD>();
        for (int i = 0; i < slotHUD.Length; i++)
        {
            slotHUD[i].outLineEquipped.enabled = false;
        }
        inventorySlotHUD.outLineEquipped.color = inventoryM.outLineSelected;
        inventorySlotHUD.outLineEquipped.enabled = true;

        foreach (ItemData data in inventoryItemData)
            if (data)
            {
                data.equipped = false;
            }
        inventorySlotHUD.itemData.equipped = true;

        foreach (Image outline in inventoryOutLine)
            if (outline)
            {
                outline.color = inventoryM.outLineHover;
                outline.enabled = false;
            }
        cc.lhandLayerWeight = 0;
        cc.rhandLayerWeight = 0;
        Image outLine = inventoryOutLine[slotNum];
        outLine.color = inventoryM.outLineSelected;
        outLine.enabled = true;
    }

    void CheckEquippedItems(ref ItemData[] itemData, ref Image[] outLine)
    {
        if (itemData[inventoryM.bowAndArrowInv.slotNum])
        {
            if (itemData[inventorySlotHUD.slotNum].itemType == ItemData.ItemType.Bow)
            {
                InventorySlotHUD[] slotHUD = FindObjectsOfType<InventorySlotHUD>();
                for (int i = 0; i < slotHUD.Length; i++)
                {
                    slotHUD[i].outLineEquipped.enabled = false;
                }
                inventorySlotHUD.outLineEquipped.enabled = true;

                foreach (ItemData data in inventoryM.bowAndArrowInv.itemData)
                    if (data && data.itemType == ItemData.ItemType.Bow)
                    {
                        data.equipped = false;
                    }
                itemData[inventorySlotHUD.slotNum].equipped = true;
                outLine[inventorySlotHUD.slotNum].color = inventoryM.outLineSelected;
                return;
            }
            else if (itemData[inventorySlotHUD.slotNum].itemType == ItemData.ItemType.Arrow)
            {
                InventorySlotHUD[] slotHUD = FindObjectsOfType<InventorySlotHUD>();
                for (int i = 0; i < slotHUD.Length; i++)
                {
                    slotHUD[i].outLineEquipped.enabled = false;
                }
                inventorySlotHUD.outLineEquipped.enabled = true;

                foreach (ItemData data in inventoryM.bowAndArrowInv.itemData)
                    if (data && data.itemType == ItemData.ItemType.Arrow)
                    {
                        data.equipped = false;
                    }
                itemData[inventorySlotHUD.slotNum].equipped = true;
                outLine[inventorySlotHUD.slotNum].color = inventoryM.outLineSelected;
                return;
            }
        }
    }

    void CreateInventorySlotHUD(ref ItemData[] itemData, ItemData.ItemType itemType, ref Image[] itemImage, ref Image[] highLight, ref Image[] statValueBG, ref TextMeshProUGUI[] statValue, ref TextMeshProUGUI[] quantity, ref int[] itemCounter)
    {
        for (int i = 0; i < itemImage.Length; i++)
        {
            if (itemImage[i].enabled)
            {
                if(itemData[i].itemType == itemType)
                {
                    GameObject newSlot = Instantiate(inventorySlotHUDPrefab);

                    InventorySlotHUD slotData = newSlot.GetComponent<InventorySlotHUD>();
                    slotData.slotNum = i;

                    slotData.itemData = itemData[i];

                    if (slotData.itemData)
                    {
                        slotData.itemImage.enabled = true;
                        slotData.itemImage.sprite = itemImage[i].sprite;

                        if (!slotData.itemData.stackable)
                        {
                            slotData.statValue.enabled = true;
                            slotData.statValueBG.enabled = true;
                        }
                        else
                        {
                            slotData.itemImage.enabled = true;
                            slotData.itemQuantity.enabled = true;
                            slotData.itemQuantity.text = "x" + itemCounter[i].ToString();
                        }

                        if (slotData.itemData.equipped)
                            slotData.outLineEquipped.enabled = true;

                        #region Set Inventory BG Color

                        switch (itemData[i].rankNum)
                        {
                            case 0:
                                slotData.highLight.GetComponent<UIGradient>().enabled = false;
                                slotData.highLight.color = new Color(0, 0, 0, 80);
                                break;
                            case 1:
                                slotData.highLight.GetComponent<UIGradient>().enabled = true;
                                slotData.highLight.GetComponent<UIGradient>().LinearColor1 = inventoryM.rank1Color1;
                                slotData.highLight.GetComponent<UIGradient>().LinearColor2 = inventoryM.rank1Color2;
                                break;
                            case 2:
                                slotData.highLight.GetComponent<UIGradient>().enabled = true;
                                slotData.highLight.GetComponent<UIGradient>().LinearColor1 = inventoryM.rank2Color1;
                                slotData.highLight.GetComponent<UIGradient>().LinearColor2 = inventoryM.rank2Color2;
                                break;
                            case 3:
                                slotData.highLight.GetComponent<UIGradient>().enabled = true;
                                slotData.highLight.GetComponent<UIGradient>().LinearColor1 = inventoryM.rank3Color1;
                                slotData.highLight.GetComponent<UIGradient>().LinearColor2 = inventoryM.rank3Color2;
                                break;
                        }

                        if (itemData[i].broken)
                        {
                            slotData.highLight.GetComponent<UIGradient>().enabled = true;
                            slotData.highLight.GetComponent<UIGradient>().LinearColor1 = inventoryM.brokenColor1;
                            slotData.highLight.GetComponent<UIGradient>().LinearColor2 = inventoryM.brokenColor2;
                        }

                        #endregion

                        #region Max Stat Value

                        switch (slotData.itemData.itemType)
                        {
                            case ItemData.ItemType.Weapon:
                                slotData.statValue.text = slotData.itemData.maxWpnAtk.ToString();
                                break;
                            case ItemData.ItemType.Bow:
                                slotData.statValue.text = slotData.itemData.maxBowAtk.ToString();
                                break;
                            case ItemData.ItemType.Shield:
                                slotData.statValue.text = slotData.itemData.maxShdAtk.ToString();
                                break;
                        }
                        #endregion
                    }
                }
            }
        }
    }

    void ClearInventoryHUDSlotsOnClose()
    {
        if (fadeUI.canvasGroup.alpha == 0 && !isActive)
        {
            slots = FindObjectsOfType<InventorySlotHUD>();
            if (slots != null)
            {
                for (int i = 0; i < slots.Length; i++)
                {
                    Destroy(slots[i].gameObject);
                }
            }
        }
    }

    private void SetPagePositions()
    {
        int width = 0;
        int height = 0;
        int offsetX = 0;
        int offsetY = 0;
        int containerWidth = 0;
        int containerHeight = 0;

        if (m_horizontal)
        {
            // screen width in pixels of scrollrect window
            width = (int)m_scrollRectTransform.rect.width;
            // center position of all pages
            offsetX = width / 2;
            // total width
            containerWidth = width * m_slotCount;
        }
        else
        {
            height = (int)m_scrollRectTransform.rect.height;
            offsetY = height / 2;
            containerHeight = height * m_slotCount;
        }

        // set width of container
        Vector2 newSize = new Vector2(containerWidth, containerHeight);
        m_content.sizeDelta = newSize;
        Vector2 newPosition = new Vector2(containerWidth / 2, containerHeight / 2);
        m_content.anchoredPosition = newPosition;

        // delete any previous settings
        m_slotPositions.Clear();

        // iterate through all container childern and set their positions
        for (int i = 0; i < m_slotCount; i++)
        {
            RectTransform child = m_content.GetChild(i).GetComponent<RectTransform>();
            Vector2 childPosition;
            if (m_horizontal)
            {
                childPosition = new Vector2(i * width - containerWidth / 2 + offsetX, 0f);
            }
            else
            {
                childPosition = new Vector2(0f, -(i * height - containerHeight / 2 + offsetY));
            }
            child.anchoredPosition = childPosition;
            m_slotPositions.Add(-childPosition);
        }
    }

    private void SetPage(int aPageIndex)
    {
        aPageIndex = Mathf.Clamp(aPageIndex, 0, m_slotCount - 1);
        m_content.anchoredPosition = m_slotPositions[aPageIndex];
        m_currentSlot = aPageIndex;
    }

    private void LerpToPage(int aPageIndex)
    {
        aPageIndex = Mathf.Clamp(aPageIndex, 0, m_slotCount - 1);
        mm_lerpTo = m_slotPositions[aPageIndex];
        m_lerp = true;
        m_currentSlot = aPageIndex;
    }

    private void InitPageSelection()
    {
        // page selection - only if defined sprites for selection icons
        m_showSlotSelection = slotSelectionIcons != null;
        if (m_showSlotSelection)
        {
            // also container with selection images must be defined and must have exatly the same amount of items as pages container
            if (slotSelectionIcons == null || slotSelectionIcons.childCount != m_slotCount)
            {
                Debug.LogWarning("Different count of pages and selection icons - will not show page selection");
                m_showSlotSelection = false;
            }
            else
            {
                m_previousSlotSelectionIndex = -1;
                _slotSelectionImages = new List<Image>();

                // cache all Image components into list
                for (int i = 0; i < slotSelectionIcons.childCount; i++)
                {
                    Image image = slotSelectionIcons.GetChild(i).GetComponent<Image>();
                    if (image == null)
                    {
                        Debug.LogWarning("Page selection icon at position " + i + " is missing Image component");
                    }
                    image.color = unselectedColor;
                    _slotSelectionImages.Add(image);
                }
            }
        }
    }

    private void SetPageSelection(int aPageIndex)
    {
        // nothing to change
        if (m_previousSlotSelectionIndex == aPageIndex)
        {
            return;
        }

        // unselect old
        if (m_previousSlotSelectionIndex >= 0)
            _slotSelectionImages[m_previousSlotSelectionIndex].color = unselectedColor;

        // select new
        _slotSelectionImages[aPageIndex].color = selectedColor;

        m_previousSlotSelectionIndex = aPageIndex;
    }

    private void NextScreen()
    {
        LerpToPage(m_currentSlot + 1);
    }

    private void PreviousScreen()
    {
        LerpToPage(m_currentSlot - 1);
    }

    private int GetNearestPage()
    {
        // based on distance from current position, find nearest page
        Vector2 currentPosition = m_content.anchoredPosition;

        float distance = float.MaxValue;
        int nearestPage = m_currentSlot;

        for (int i = 0; i < m_slotPositions.Count; i++)
        {
            float testDist = Vector2.SqrMagnitude(currentPosition - m_slotPositions[i]);
            if (testDist < distance)
            {
                distance = testDist;
                nearestPage = i;
            }
        }

        return nearestPage;
    }

    void ControllerButtonIcon()
    {
        switch (cc.controllerType)
        {
            case ThirdPersonInput.ControllerType.MOUSEKEYBOARD:
                KeyboardAlpha.canvasGroup.alpha = 1;
                XboxAlpha.canvasGroup.alpha = 0;
                pSAlpha.canvasGroup.alpha = 0;
                break;
            case ThirdPersonInput.ControllerType.XBOX:
                KeyboardAlpha.canvasGroup.alpha = 0;
                XboxAlpha.canvasGroup.alpha = 1;
                pSAlpha.canvasGroup.alpha = 0;
                break;
            case ThirdPersonInput.ControllerType.PLAYSTATION:
                KeyboardAlpha.canvasGroup.alpha = 0;
                XboxAlpha.canvasGroup.alpha = 0;
                pSAlpha.canvasGroup.alpha = 1;
                break;
        }
    }

    #region Deactivate Weapons 

    void DeactiveWeapons()
    {
        if (wpnHolster.swordHP != null) wpnHolster.swordHP.SetActive(false);
        if (wpnHolster.alteredPrimaryHP != null) wpnHolster.alteredPrimaryHP.SetActive(false);
        if (wpnHolster.alteredPrimaryEP != null) wpnHolster.alteredPrimaryEP.SetActive(false);
        if (wpnHolster.swordEP != null) wpnHolster.swordEP.SetActive(false);
        if (wpnHolster.dSwordHP != null) wpnHolster.dSwordHP.SetActive(false);
        if (wpnHolster.dSwordEP != null) wpnHolster.dSwordEP.SetActive(false);
        if (wpnHolster.spearHP != null) wpnHolster.spearHP.SetActive(false);
        if (wpnHolster.spearEP != null) wpnHolster.spearEP.SetActive(false);
        if (wpnHolster.hammerHP != null) wpnHolster.hammerHP.SetActive(false);
        if (wpnHolster.hammerEP != null) wpnHolster.hammerEP.SetActive(false);
        if (wpnHolster.staffHP != null) wpnHolster.staffHP.SetActive(false);
        if (wpnHolster.staffEP != null) wpnHolster.staffEP.SetActive(false);
        if (wpnHolster.primaryH != null) wpnHolster.primaryH.SetActive(false);
        if (wpnHolster.primaryE != null) wpnHolster.primaryE.SetActive(false);
        if (wpnHolster.primaryD != null) wpnHolster.primaryD.SetActive(false);
        if (wpnHolster.alteredPrimaryE != null) wpnHolster.alteredPrimaryE.SetActive(false);
        if (wpnHolster.alteredPrimaryH != null) wpnHolster.alteredPrimaryH.SetActive(false);
        if (wpnHolster.swordHP != null) wpnHolster.swordHP = null;
        if (wpnHolster.swordEP != null) wpnHolster.swordEP = null;
        if (wpnHolster.dSwordHP != null) wpnHolster.dSwordHP = null;
        if (wpnHolster.dSwordEP != null) wpnHolster.dSwordEP = null;
        if (wpnHolster.spearHP != null) wpnHolster.spearHP = null;
        if (wpnHolster.spearEP != null) wpnHolster.spearEP = null;
        if (wpnHolster.hammerHP != null) wpnHolster.hammerHP = null;
        if (wpnHolster.hammerEP != null) wpnHolster.hammerEP = null;
        if (wpnHolster.staffHP != null) wpnHolster.staffHP = null;
        if (wpnHolster.staffEP != null) wpnHolster.staffEP = null;
        if (wpnHolster.primaryH != null) wpnHolster.primaryH = null;
        if (wpnHolster.primaryD != null) wpnHolster.primaryD = null;
        if (wpnHolster.primaryE != null) wpnHolster.primaryE = null;
        if (wpnHolster.alteredPrimaryE != null) wpnHolster.alteredPrimaryE = null;
        inventoryM.preSwordEquipped = false;
        inventoryM.preDaggerEquipped = false;
        inventoryM.pre2HSwordEquipped = false;
        inventoryM.preSpearEquipped = false;
        inventoryM.preStaffEquipped = false;

        cc.weaponArmsID = 0;
        cc.preWeaponArmsID = 0;
    }

    void DeactiveBows()
    {
        DeactiveArrows();
        if (wpnHolster.secondaryH != null) wpnHolster.secondaryH.SetActive(false);
        if (wpnHolster.secondaryE != null) wpnHolster.secondaryE.SetActive(false);
        if (wpnHolster.secondaryD != null) wpnHolster.secondaryD.SetActive(false);
        if (wpnHolster.bowHP != null) wpnHolster.bowHP.SetActive(false);
        if (wpnHolster.bowEP != null) wpnHolster.bowEP.SetActive(false);
        if (wpnHolster.quiverHP != null) wpnHolster.quiverHP.SetActive(false);
        if (wpnHolster.quiverH != null) wpnHolster.quiverH.SetActive(false);
        if (wpnHolster.bowHP != null) wpnHolster.bowHP = null;
        if (wpnHolster.bowEP != null) wpnHolster.bowEP = null;
        if (wpnHolster.secondaryH != null) wpnHolster.secondaryH = null;
        if (wpnHolster.secondaryE != null) wpnHolster.secondaryE = null;
        if (wpnHolster.secondaryD != null) wpnHolster.secondaryD = null;

        inventoryM.preBowEquipped = false;

        cc.weaponArmsID = 0;
        cc.preWeaponArmsID = 0;
    }

    void DeactiveShield()
    {
        if (wpnHolster.shieldH != null) wpnHolster.shieldH.SetActive(false);
        if (wpnHolster.shieldE != null) wpnHolster.shieldE.SetActive(false);
        if (wpnHolster.shieldD != null) wpnHolster.shieldD.SetActive(false);
        if (wpnHolster.shieldHP != null) wpnHolster.shieldHP.SetActive(false);
        if (wpnHolster.shieldP != null) wpnHolster.shieldP.SetActive(false);
        if (wpnHolster.shieldHP != null) wpnHolster.shieldHP = null;
        if (wpnHolster.shieldP != null) wpnHolster.shieldP = null;
        if (wpnHolster.shieldH != null) wpnHolster.shieldH = null;
        if (wpnHolster.shieldE != null) wpnHolster.shieldE = null;
        if (wpnHolster.shieldD != null) wpnHolster.shieldD = null;

        inventoryM.preShieldEquipped = false;

       cc.weaponArmsID = 0;
        cc.preWeaponArmsID = 0;
    }

    void DeactiveArrows()
    {
        if (wpnHolster.arrowH != null) wpnHolster.arrowH.SetActive(false);
        if (wpnHolster.arrowE != null) wpnHolster.arrowE.SetActive(false);
        if (wpnHolster.arrowD != null) wpnHolster.arrowD.SetActive(false);
        if (wpnHolster.arrowHP != null) wpnHolster.arrowHP.SetActive(false);
        if (wpnHolster.arrowEP != null) wpnHolster.arrowEP.SetActive(false);
        if (wpnHolster.arrowHP != null) wpnHolster.arrowHP = null;
        if (wpnHolster.arrowEP != null) wpnHolster.arrowEP = null;
        if (wpnHolster.arrowH != null) wpnHolster.arrowH = null;
        if (wpnHolster.arrowE != null) wpnHolster.arrowE = null;
        if (wpnHolster.arrowD != null) wpnHolster.arrowD = null;
    }

    void DeactiveWeaponsHP()
    {
        if (wpnHolster.swordHP != null) wpnHolster.swordHP.SetActive(false);
        if (wpnHolster.dSwordHP != null) wpnHolster.dSwordHP.SetActive(false);
        if (wpnHolster.spearHP != null) wpnHolster.spearHP.SetActive(false);
        if (wpnHolster.hammerHP != null) wpnHolster.hammerHP.SetActive(false);
        if (wpnHolster.staffHP != null) wpnHolster.staffHP.SetActive(false);
        if (wpnHolster.swordHP != null) wpnHolster.swordHP = null;
        if (wpnHolster.dSwordHP != null) wpnHolster.dSwordHP = null;
        if (wpnHolster.spearHP != null) wpnHolster.spearHP = null;
        if (wpnHolster.hammerHP != null) wpnHolster.hammerHP = null;
        if (wpnHolster.staffHP != null) wpnHolster.staffHP = null;
    }

    void DeactiveBowsHP()
    {
        if (wpnHolster.bowHP != null) wpnHolster.bowHP.SetActive(false);
        if (wpnHolster.bowHP != null) wpnHolster.bowHP = null;
    }

    void DeactiveShieldHP()
    {
        if (wpnHolster.shieldHP != null) wpnHolster.shieldHP.SetActive(false);
        if (wpnHolster.shieldHP != null) wpnHolster.shieldHP = null;
    }

    void DeactiveArrowsHP()
    {
        if (wpnHolster.arrowHP != null) wpnHolster.arrowHP.SetActive(false);
        if (wpnHolster.arrowHP != null) wpnHolster.arrowHP = null;
    }

    #endregion
}
