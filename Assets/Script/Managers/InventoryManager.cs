using System.Collections;
using System.Collections.Generic;
using RPGAE.CharacterController;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;
using JoshH.UI;

public class InventoryManager : MonoBehaviour
{
    #region Inventory Sections

    [System.Serializable]
    public struct Weapons
    {
        public int slotNum;
        public int slotWeaponEquipped;
        public int removeNullSlots;
        public bool weaponSlotActive;
        public int[] counter;
        public int[] statCounter;
        public Image[] image;
        public Image[] highLight;
        public Image[] outLineBorder;
        public Image[] statValueBG;
        public TextMeshProUGUI[] quantity;
        public TextMeshProUGUI[] statValue;
        public ItemData[] itemData;
    }
    [System.Serializable]
    public struct BowAndArrow
    {
        public int slotNum;
        public int slotBowEquipped;
        public int slotArrowEquipped;
        public int removeNullSlots;
        public bool bowAndArrowSlotActive;
        public int[] counter;
        public int[] statCounter;
        public Image[] image;
        public Image[] highLight;
        public Image[] outLineBorder;
        public Image[] statValueBG;
        public TextMeshProUGUI[] quantity;
        public TextMeshProUGUI[] statValue;
        public ItemData[] itemData;
    }
    [System.Serializable]
    public struct Shield
    {
        public int slotNum;
        public int slotShieldEquipped;
        public int removeNullSlots;
        public bool shieldSlotActive;
        public int[] counter;
        public int[] statCounter;
        public Image[] image;
        public Image[] highLight;
        public Image[] outLineBorder;
        public Image[] statValueBG;
        public TextMeshProUGUI[] quantity;
        public TextMeshProUGUI[] statValue;
        public ItemData[] itemData;
    }
    [System.Serializable]
    public struct Armor
    {
        public int slotNum;
        public int slotArmorHeadEquipped;
        public int slotArmorChestEquipped;
        public int slotArmorLegEquipped;
        public int slotArmorAmuletEquipped;
        public int removeNullSlots;
        public bool armorSlotActive;
        public int[] counter;
        public int[] statCounter;
        public Image[] image;
        public Image[] highLight;
        public Image[] outLineBorder;
        public Image[] statValueBG;
        public TextMeshProUGUI[] quantity;
        public TextMeshProUGUI[] statValue;
        public ItemData[] itemData;
    }
    [System.Serializable]
    public struct Material
    {
        public int slotNum;
        public int removeNullSlots;
        public bool materialSlotActive;
        public int[] counter;
        public int[] statCounter;
        public Image[] image;
        public Image[] highLight;
        public Image[] outLineBorder;
        public Image[] statValueBG;
        public TextMeshProUGUI[] quantity;
        public TextMeshProUGUI[] statValue;
        public ItemData[] itemData;
    }
    [System.Serializable]
    public struct Healing
    {
        public int slotNum;
        public int removeNullSlots;
        public bool healingSlotActive;
        public int[] counter;
        public int[] statCounter;
        public Image[] image;
        public Image[] highLight;
        public Image[] outLineBorder;
        public Image[] statValueBG;
        public TextMeshProUGUI[] quantity;
        public TextMeshProUGUI[] statValue;
        public ItemData[] itemData;
    }
    [System.Serializable]
    public struct Key
    {
        public int slotNum;
        public int removeNullSlots;
        public bool keySlotActive;
        public int[] counter;
        public int[] statCounter;
        public Image[] image;
        public Image[] highLight;
        public Image[] outLineBorder;
        public Image[] statValueBG;
        public TextMeshProUGUI[] quantity;
        public TextMeshProUGUI[] statValue;
        public ItemData[] itemData;
    }

    public Weapons weaponInv;
    public BowAndArrow bowAndArrowInv;
    [HideInInspector] public Shield shieldInv;
    [HideInInspector] public Armor armorInv;
    [HideInInspector] public Material materialInv;
    [HideInInspector] public Healing healingInv;
    [HideInInspector] public Key keyInv;

    #endregion

    [Header("INVENTORY SLOTS")]
    public int weaponSlots;
    public int bowAndArrowSlots;
    public int shieldSlots;
    public int armorSlots;
    public int materialSlots;
    public int healingSlots;
    public int keySlots;

    [Tooltip("Can destroyed items be repaired? (If not, it will be removed from inventory.)")]
    public bool itemRepair;

    [Tooltip("The game currency used to purchase items.")]
    public int unityCoins;

    // Inventory slot colors.
    [Header("HIGHLIGHT COLORS")]
    public Color rank1Color1;
    public Color rank1Color2;
    public Color rank2Color1;
    public Color rank2Color2;
    public Color rank3Color1;
    public Color rank3Color2;
    public Color brokenColor1;
    public Color brokenColor2;
    public Color outLineHover;
    public Color outLineSelected;

    [Header("REFERENCE")]
    public Canvas UIcanvas;
    public GameObject[] itemIconData;
    public ReferencesObj referencesObj;
    public GameObject createSlotPrefab;
    public GameObject addedItemPrefab;
    public TextMeshProUGUI unityCoinsText;
    public Image itemEquippedCheck;
    public RenderPipelineAsset defaultRenderPipelineAsset;

    #region Private 

    public bool isPauseMenuOn;
    [HideInInspector]
    public bool
    preSwordEquipped,
    preDaggerEquipped,
    pre2HSwordEquipped,
    preSpearEquipped,
    preStaffEquipped,
    preBowEquipped,
    preGunEquipped,
    preShieldEquipped;
    [HideInInspector] public int pauseMenuNavigation = 0;
    [HideInInspector] public WeaponHolster wpnHolster;
    [HideInInspector] public ItemObtained itemObtained;
    [HideInInspector] public ThirdPersonController cc;
    [HideInInspector] public MiniMap miniMap;
    [HideInInspector] public StoreManager storeM;
    [HideInInspector] public DialogueManager dialogueM;
    [HideInInspector] public SystemManager systemM;
    [HideInInspector] public ItemDescription itemDescription;
    [HideInInspector] public StartInventoryItems[] startItems;
    [HideInInspector] public AddedItemListManager addedItemList;
    [HideInInspector] public UICursorVirtualMouseInput cursorVirtual;
    [HideInInspector] public GameObject addedItemPanel, prefabHolder;
    public ItemInteract[] initWpns;
    [HideInInspector] public InventoryHUD inventoryHUD;

    [HideInInspector] public PauseMenuSection pauseMenuS;
    public enum PauseMenuSection
    {
        Inventory,
        Quest,
        Map,
        Skills,
        System
    }
    [HideInInspector]
    public InventorySection inventoryS;
    public enum InventorySection
    {
        Weapon,
        BowAndArrow,
        Shield,
        Armor,
        Material,
        Healing,
        Key
    }

    [Serializable]
    public class ReferencesObj
    {
        [Header("PAUSE MENU")]
        public GameObject UIBorder;
        public GameObject InventoryTitleBG;
        public GameObject menuParticleSystem;
        public GameObject pauseMenu;
        public GameObject mapSection;
        public GameObject questSection;
        public GameObject inventorySection;
        public GameObject skillsSection;
        public GameObject systemSection;

        public GameObject weaponGrid;
        public GameObject bowAndArrowGrid;
        public GameObject shieldGrid;
        public GameObject armorGrid;
        public GameObject materialGrid;
        public GameObject healingGrid;
        public GameObject keyGrid;
        [Space(10)]
        public GameObject abilityAssignMenu;

        [Header("STORE UI")]
        public GameObject storeM;
        public GameObject buyStoreM;
        public GameObject sellStoreM;
        public GameObject repairStoreM;
        public GameObject upgradeStoreM;
        public GameObject specialStoreM;

        [Header("ITEM OPTION")]
        public GameObject inputFieldOptions;
        public GameObject itemOptionsGroup;
        public GameObject itemOptionsEquipObj;
        public GameObject itemOptionsRemoveObj;
        public GameObject itemOptionsDropObj;
        public GameObject itemOptionsPreviewObj;
        public GameObject itemOptionsConsumeObj;
        public GameObject itemOptionsBuyObj;
        public GameObject itemOptionsSellObj;
        public GameObject itemOptionsRepairObj;
        public GameObject itemOptionsUpgradeObj;
        public GameObject itemOptionsWeaponSpecialObj;
        public GameObject itemOptionsBowSpecialObj;
        public GameObject itemOptionsShieldSpecialObj;
        public GameObject itemOptionsArmorSpecialObj;

        [Header("EQUIPPED ICON")]
        public Image primaryItemImage;
        public Image primaryValueBG;
        public Image shieldItemImage;
        public Image shieldValueBG;
        public Image secondaryItemImage;
        public Image secondaryValueBG;
        public Image arrowItemImage;
        public Image arrowValueBG;
        public Image headItemImage;
        public Image headValueBG;
        public Image chestItemImage;
        public Image chestValueBG;
        public Image legItemImage;
        public Image legValueBG;
        public Image amuletItemImage;
        public Image amuletValueBG;

        [Header("HUD")]
        public FadeUI playerGuage;
        public FadeUI levelGuage;
        public FadeUI questObjective;
        public FadeUI inventoryHUD;
        public FadeUI abilityHUD;
        public FadeUI buttonsHUD;
        public FadeUI mainCursor;
        public FadeUI coinHUD;

        [HideInInspector] 
        public GameObject mapCursorObj;
    }

    #endregion

    private MeshRenderer[] allMesh;
    private SkinnedMeshRenderer[] allSkinMesh;
    private HumanoidBehavior[] humanoidB;
    private AIController[] aiController;
    private AIHealthGuage[] aiHp;
    private InteractWorldSpaceUI[] uiWorldSpace;

    // Start is called before the first frame update
    void Awake()
    {
        cc = FindObjectOfType<ThirdPersonController>();
        miniMap = FindObjectOfType<MiniMap>();
        storeM = FindObjectOfType<StoreManager>();
        dialogueM = FindObjectOfType<DialogueManager>();
        cursorVirtual = FindObjectOfType<UICursorVirtualMouseInput>();
        wpnHolster = GameObject.Find("PlayerHolster").GetComponent<WeaponHolster>();
        addedItemList = FindObjectOfType<AddedItemListManager>();
        itemDescription = FindObjectOfType<ItemDescription>();
        itemObtained = FindObjectOfType<ItemObtained>();
        inventoryHUD = FindObjectOfType<InventoryHUD>();
        systemM = FindObjectOfType<SystemManager>();

        pauseMenuNavigation = 0;
        pauseMenuS = PauseMenuSection.Inventory;

        UIcanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        UIcanvas.worldCamera = GameObject.Find("TPCamera").GetComponent<Camera>();
        UIcanvas.planeDistance = 1;

        //GraphicsSettings.defaultRenderPipeline = defaultRenderPipelineAsset;
        //QualitySettings.renderPipeline = defaultRenderPipelineAsset;

        allMesh = FindObjectsOfType<MeshRenderer>();
        allSkinMesh = FindObjectsOfType<SkinnedMeshRenderer>();
        humanoidB = FindObjectsOfType<HumanoidBehavior>();
        aiController = FindObjectsOfType<AIController>();
        aiHp = FindObjectsOfType<AIHealthGuage>();
        uiWorldSpace = FindObjectsOfType<InteractWorldSpaceUI>();

        inventoryS = InventorySection.Weapon;
        pauseMenuS = PauseMenuSection.Inventory;

        SetWeaponSlot(ref weaponSlots);
        SetBowAndArrowSlot(ref bowAndArrowSlots);
        SetShieldSlot(ref shieldSlots);
        SetArmorSlot(ref armorSlots);
        SetMaterialSlot(ref materialSlots);
        SetHealingSlot(ref healingSlots);
        SetKeySlot(ref keySlots);

    }

    // Update is called once per frame
    void Update()
    {
        PauseMenuToggle();

        StartInventoryItems[] sitems = FindObjectsOfType<StartInventoryItems>();
        foreach (StartInventoryItems stitems in sitems)
        {
           stitems.AddItemsIntoInventory();
        }
    }

    void PauseMenuToggle()
    {
        if (cc.systemM.loadingScreenFUI.canvasGroup.alpha != 0 || cc.systemM.blackScreenFUI.canvasGroup.alpha != 0 || 
            cc.GetComponent<HealthPoints>().curHealthPoints <= 0 || dialogueM.curDialogue != null && dialogueM.curDialogue.npcName != "") return;

        AutoSetPauseMenuState();

        unityCoinsText.text = unityCoins.ToString();
        if (!isPauseMenuOn && cc.rpgaeIM.PlayerControls.Start.triggered)
        {
            if (referencesObj.pauseMenu.GetComponent<FadeUI>().canvasGroup.alpha == 0 &&
            systemM.blackScreenFUI.GetComponent<FadeUI>().canvasGroup.alpha == 0 && 
            referencesObj.inventoryHUD.GetComponent<FadeUI>().canvasGroup.alpha == 0 &&
            miniMap.fadeUI.canvasGroup.alpha == 1 && ConditionsToOpenMenu())
            {
                isPauseMenuOn = true;
                StartCoroutine(PauseMenuFade());
            }
        }
        if (isPauseMenuOn && cc.rpgaeIM.PlayerControls.Start.triggered)
        {
            if (referencesObj.pauseMenu.GetComponent<FadeUI>().canvasGroup.alpha == 1 &&
            systemM.blackScreenFUI.GetComponent<FadeUI>().canvasGroup.alpha == 0 && 
            referencesObj.inventoryHUD.GetComponent<FadeUI>().canvasGroup.alpha == 0 && ConditionsToOpenMenu())
            {
                isPauseMenuOn = false;
                StartCoroutine(PauseMenuFade());
            }
        }
        if (isPauseMenuOn && referencesObj.pauseMenu.GetComponent<FadeUI>().canvasGroup.alpha == 1)
        {
            UpdatePauseMenu();
        }
    }

    void AutoSetPauseMenuState()
    {
        if (referencesObj.pauseMenu.GetComponent<FadeUI>().canvasGroup.alpha == 0 && cc.systemM.blackScreenFUI.canvasGroup.alpha == 0)
            isPauseMenuOn = false;
        else
            isPauseMenuOn = true;
    }

    public bool ConditionsToOpenMenu()
    {
        if (storeM.isActive || storeM.buyM.isActive ||
            storeM.sellM.isActive || storeM.repairM.isActive ||
            storeM.upgradeM.isActive || storeM.specialM.isActive ||
            storeM.fadeUI.canvasGroup.alpha != 0 || storeM.buyM.fadeUI.canvasGroup.alpha != 0 ||
            storeM.sellM.fadeUI.canvasGroup.alpha != 0 || storeM.repairM.fadeUI.canvasGroup.alpha != 0 ||
            storeM.upgradeM.fadeUI.canvasGroup.alpha != 0 || storeM.specialM.fadeUI.canvasGroup.alpha != 0 ||
            cc.tpCam.cameraState != ThirdPersonCamera.CameraState.Orbit)
            return false;

        return true;
    }

    IEnumerator PauseMenuFade()
    {
        systemM.blackScreenFUI.isFading = true;
        systemM.blackScreenFUI.canvasGroup.alpha = 0;

        #region Initialization

        PauseMenuSectionIsActive(false);
        InventorySectionIsActive(false);

        #endregion

        while (!Mathf.Approximately(systemM.blackScreenFUI.canvasGroup.alpha, 1))
        {
            systemM.blackScreenFUI.canvasGroup.alpha = Mathf.MoveTowards(systemM.blackScreenFUI.canvasGroup.alpha, 1,
                2 * Time.deltaTime);
            yield return null;
        }

        #region On Black Screen

        if (isPauseMenuOn && systemM.blackScreenFUI.canvasGroup.alpha == 1)
        {
            FreezeWorld(true);
            miniMap.ToggleSize();
            IsWorldSpaceActive(false);
            referencesObj.menuParticleSystem.SetActive(true);
            cursorVirtual.SetCursorToCenter();
            referencesObj.pauseMenu.GetComponent<FadeUI>().canvasGroup.alpha = 1;
            SwitchOverrideRenderPipeline();
            cursorVirtual.isActive = true;
            WorldSpaceUILayer(true);

            if (pauseMenuS == PauseMenuSection.Inventory || pauseMenuS == PauseMenuSection.Skills)
                referencesObj.levelGuage.canvasGroup.alpha = 1;

            referencesObj.playerGuage.canvasGroup.alpha = 0;
            referencesObj.abilityHUD.canvasGroup.alpha = 0;
            referencesObj.buttonsHUD.canvasGroup.alpha = 0;

            if (FindObjectOfType<BossHealthGuage>() != null)
                FindObjectOfType<BossHealthGuage>().fadeUI.canvasGroup.alpha = 0;

            referencesObj.InventoryTitleBG.GetComponent<FadeUI>().canvasGroup.alpha = 0;

            cc.hudM.fadeUIQuestObjective.canvasGroup.alpha = 0;
            cc.hudM.fadeUIBG.canvasGroup.alpha = 0;
            cc.hudM.fadeUIFadeGroup.canvasGroup.alpha = 0;
            cc.hudM.currentObjectiveBG.SetFloat("Blend", 0);

            referencesObj.primaryItemImage.GetComponentInParent<FadeUI>().canvasGroup.alpha = 1;
            referencesObj.headItemImage.GetComponentInParent<FadeUI>().canvasGroup.alpha = 1;
        }
        else if (!isPauseMenuOn && systemM.blackScreenFUI.canvasGroup.alpha == 1)
        {
            FreezeWorld(false);
            miniMap.ToggleSize();
            IsWorldSpaceActive(true);
            UpdateRendererEnable(true);
            referencesObj.menuParticleSystem.SetActive(false);
            cursorVirtual.SetCursorToCenter();
            referencesObj.pauseMenu.GetComponent<FadeUI>().canvasGroup.alpha = 0;
            if (pauseMenuS != PauseMenuSection.Map)
                SwitchOverrideRenderPipeline();

            cursorVirtual.isActive = false;
            WorldSpaceUILayer(false);

            UIcanvas.renderMode = RenderMode.ScreenSpaceOverlay;

            ShowCurrentObjectiveUI(ref systemM.questM.mainQuestSlots.questData, ref systemM.questM.mainQuestSlots.highLight);
            ShowCurrentObjectiveUI(ref systemM.questM.sideQuestSlots.questData, ref systemM.questM.sideQuestSlots.highLight);

            cc.hudM.fadeUIQuestObjective.canvasGroup.alpha = 0;
            referencesObj.playerGuage.canvasGroup.alpha = 1;
            referencesObj.levelGuage.canvasGroup.alpha = 0;
            referencesObj.abilityHUD.canvasGroup.alpha = 1;
            referencesObj.buttonsHUD.canvasGroup.alpha = 1;

            referencesObj.coinHUD.canvasGroup.alpha = 0;
        }

        #endregion

        yield return new WaitForSeconds(0.25f);

        while (!Mathf.Approximately(systemM.blackScreenFUI.canvasGroup.alpha, 0))
        {
            systemM.blackScreenFUI.canvasGroup.alpha = Mathf.MoveTowards(systemM.blackScreenFUI.canvasGroup.alpha, 0,
            2 * Time.deltaTime);
            yield return null;
        }

        ShowCurrentObjectiveUI(ref systemM.questM.mainQuestSlots.questData, ref systemM.questM.mainQuestSlots.highLight);
        ShowCurrentObjectiveUI(ref systemM.questM.sideQuestSlots.questData, ref systemM.questM.sideQuestSlots.highLight);

        systemM.blackScreenFUI.canvasGroup.alpha = 0;
        systemM.blackScreenFUI.isFading = false;
    }

    public void FreezeWorld(bool isOn)
    {
        if (isOn)
        {
            cc.animator.speed = 0;
            cc.canMove = false;
        }
        else
        {
            cc.animator.speed = 1;
            if (!cc.canMove)
                cc.canMove = true;
        }
        foreach (AIController ai in aiController)
        {
            if (isOn)
            {
                if (ai != null)
                {
                    ai.animator.speed = 0;
                }
            }
            else
            {
                if (ai != null)
                {
                    ai.animator.speed = 1;
                }
            }
        }
    }

    // Switch the override render pipeline between null,
    // and the render pipeline defined in overrideRenderPipelineAsset
    void SwitchOverrideRenderPipeline()
    {
        //if (QualitySettings.renderPipeline == defaultRenderPipelineAsset)
        //{
        //    //GraphicsSettings.defaultRenderPipeline = null;
        //    //QualitySettings.renderPipeline = null;
        //}
        //else
        //{
        //    GraphicsSettings.defaultRenderPipeline = defaultRenderPipelineAsset;
        //    QualitySettings.renderPipeline = defaultRenderPipelineAsset;
        //}
    }

    #region Add, Remove Inventory Items 

    public void AddItemDataSlot(ref ItemData[] currentInventory, ref Image[] itemImages, ref GameObject addedItem)
    {
        // You need inventory slots!
        if (itemImages.Length == 0 || itemImages[0] == null)
        {
            if (dialogueM.curDialogue != null && dialogueM.curDialogue.npcName != "")
            {
                dialogueM.curDialogue.dialogueState = Dialogue.DialogueState.Default;
            }

            cc.infoMessage.info.text = "You don't have enough inventory space.";
            return;
        }

        // The inventory is full!
        int size = itemImages.Length - 1;
        if (itemImages[size].enabled)
        {
            if (dialogueM.curDialogue != null && dialogueM.curDialogue.npcName != "")
            {
                dialogueM.curDialogue.dialogueState = Dialogue.DialogueState.Default;
            }

            cc.infoMessage.info.text = "You don't have enough inventory space.";
            return;
        }

        ItemData[] temp0 = new ItemData[currentInventory.Length + 1];
        for (int ii = 0; ii < currentInventory.Length; ii++)
        {
            if (currentInventory[0] != null)
            {
                temp0[ii] = currentInventory[ii];
            }
        }
        currentInventory = temp0;

        GetItem(addedItem);
        AddedItemUI(ref addedItem);
    }

    /// <summary>
    /// We get the item and set them into inventory 
    /// </summary>
    public void GetItem(GameObject itemToAdd)
    {
        ItemData itemToAddConvert = itemToAdd.GetComponentInChildren<ItemData>();
        switch (itemToAddConvert.itemType)
        {
            case ItemData.ItemType.Weapon:
                SetItem(itemToAddConvert, ref weaponInv.slotNum, ref weaponInv.itemData, ref weaponInv.quantity, ref weaponInv.statValue, ref weaponInv.counter, ref weaponInv.statCounter, ref weaponInv.image, ref weaponInv.highLight, ref weaponInv.statValueBG);
                break;
            case ItemData.ItemType.Bow:
                SetItem(itemToAddConvert, ref bowAndArrowInv.slotNum, ref bowAndArrowInv.itemData, ref bowAndArrowInv.quantity, ref bowAndArrowInv.statValue, ref bowAndArrowInv.counter, ref bowAndArrowInv.statCounter, ref bowAndArrowInv.image, ref bowAndArrowInv.highLight, ref bowAndArrowInv.statValueBG);
                break;
            case ItemData.ItemType.Arrow:
                SetItem(itemToAddConvert, ref bowAndArrowInv.slotNum, ref bowAndArrowInv.itemData, ref bowAndArrowInv.quantity, ref bowAndArrowInv.statValue, ref bowAndArrowInv.counter, ref bowAndArrowInv.statCounter, ref bowAndArrowInv.image, ref bowAndArrowInv.highLight,  ref bowAndArrowInv.statValueBG);
                break;
            case ItemData.ItemType.Shield:
                SetItem(itemToAddConvert, ref shieldInv.slotNum, ref shieldInv.itemData, ref shieldInv.quantity, ref shieldInv.statValue, ref shieldInv.counter, ref shieldInv.statCounter, ref shieldInv.image, ref shieldInv.highLight, ref shieldInv.statValueBG);
                break;
            case ItemData.ItemType.Armor:
                SetItem(itemToAddConvert, ref armorInv.slotNum, ref armorInv.itemData, ref armorInv.quantity, ref armorInv.statValue, ref armorInv.counter, ref armorInv.statCounter, ref armorInv.image, ref armorInv.highLight, ref armorInv.statValueBG);
                break;
            case ItemData.ItemType.Material:
                SetItem(itemToAddConvert, ref materialInv.slotNum, ref materialInv.itemData, ref materialInv.quantity, ref materialInv.statValue, ref materialInv.counter, ref materialInv.statCounter, ref materialInv.image, ref materialInv.highLight, ref materialInv.statValueBG);
                break;
            case ItemData.ItemType.Healing:
                SetItem(itemToAddConvert, ref healingInv.slotNum, ref healingInv.itemData, ref healingInv.quantity, ref healingInv.statValue, ref healingInv.counter, ref healingInv.statCounter, ref healingInv.image, ref healingInv.highLight, ref healingInv.statValueBG);
                break;
            case ItemData.ItemType.Key:
                SetItem(itemToAddConvert, ref keyInv.slotNum, ref keyInv.itemData, ref keyInv.quantity, ref keyInv.statValue, ref keyInv.counter, ref keyInv.statCounter, ref keyInv.image, ref keyInv.highLight, ref keyInv.statValueBG);
                break;
        }
    }

    /// <summary>
    /// We set stackable and non-stackable items
    /// </summary>
    void SetItem(ItemData itemToAdd, ref int inventorySlot, ref ItemData[] itemData, ref TextMeshProUGUI[] itemquantity, ref TextMeshProUGUI[] statValue, ref int[] ItemCounter, ref int[] statCounter, ref Image[] ItemImage, ref Image[] highlight, ref Image[] statValueBG)
    {
        // We increment current stackable item
        for (inventorySlot = 0; inventorySlot < itemData.Length; inventorySlot++)
        {
            if (itemData[inventorySlot] != null)
            {
                if (itemData[inventorySlot].stackable && itemData[inventorySlot].itemName == itemToAdd.itemName)
                {
                    ItemCounter[inventorySlot] += itemToAdd.quantity;
                    itemquantity[inventorySlot].text = "x" + ItemCounter[inventorySlot].ToString();

                    ItemData[] temp0 = new ItemData[itemData.Length - 1];
                    for (int ii = 0; ii < itemData.Length; ii++)
                    {
                        if (itemData[ii] != null)
                        {
                            temp0[ii] = itemData[ii];
                        }
                    }
                    itemData = temp0;
                    return;
                }
            }

            // We set default item icon into inventory slot
            if (itemData[inventorySlot] == null)
            {
                itemData[inventorySlot] = itemToAdd;
                ItemImage[inventorySlot].sprite = itemToAdd.itemSprite;
                ItemImage[inventorySlot].type = Image.Type.Simple;
                ItemImage[inventorySlot].preserveAspect = true;
                Navigation automatic = new Navigation { mode = Navigation.Mode.Automatic };
                ItemImage[inventorySlot].GetComponent<Button>().navigation = automatic;

                itemToAdd.inInventory = true;

                if (itemData[inventorySlot].itemName == itemToAdd.itemName)
                {
                    if (!itemData[inventorySlot].stackable)
                    {
                        ItemCounter[inventorySlot] = 1;
                        ItemImage[inventorySlot].enabled = true;
                        statValue[inventorySlot].enabled = true;
                        if (itemData[inventorySlot].itemType != ItemData.ItemType.Material || itemData[inventorySlot].itemType != ItemData.ItemType.Material || itemData[inventorySlot].itemType != ItemData.ItemType.Key)
                            statValueBG[inventorySlot].enabled = true;

                        #region Set Inventory BG Color

                        switch (itemData[inventorySlot].rankNum)
                        {
                            case 0:
                                highlight[inventorySlot].color = new Color(0, 0, 0, 80);
                                highlight[inventorySlot].enabled = true;
                                highlight[inventorySlot].GetComponent<UIGradient>().enabled = false;
                                break;
                            case 1:
                                highlight[inventorySlot].GetComponent<UIGradient>().enabled = true;
                                highlight[inventorySlot].GetComponent<UIGradient>().LinearColor1 = rank1Color1;
                                highlight[inventorySlot].GetComponent<UIGradient>().LinearColor2 = rank1Color2;
                                break;
                            case 2:
                                highlight[inventorySlot].GetComponent<UIGradient>().enabled = true;
                                highlight[inventorySlot].GetComponent<UIGradient>().LinearColor1 = rank2Color1;
                                highlight[inventorySlot].GetComponent<UIGradient>().LinearColor2 = rank2Color2;
                                break;
                            case 3:
                                highlight[inventorySlot].GetComponent<UIGradient>().enabled = true;
                                highlight[inventorySlot].GetComponent<UIGradient>().LinearColor1 = rank3Color1;
                                highlight[inventorySlot].GetComponent<UIGradient>().LinearColor2 = rank3Color2;
                                break;
                        }

                        if (itemData[inventorySlot].broken)
                        {
                            highlight[inventorySlot].GetComponent<UIGradient>().enabled = true;
                            highlight[inventorySlot].GetComponent<UIGradient>().LinearColor1 = brokenColor1;
                            highlight[inventorySlot].GetComponent<UIGradient>().LinearColor2 = brokenColor2;
                        }

                        #endregion

                        #region Max Stat Value

                        switch (itemData[inventorySlot].itemType)
                        {
                            case ItemData.ItemType.Weapon:
                                statCounter[inventorySlot] = itemToAdd.maxWpnAtk;
                                break;
                            case ItemData.ItemType.Bow:
                                statCounter[inventorySlot] = itemToAdd.maxBowAtk;
                                break;
                            case ItemData.ItemType.Shield:
                                statCounter[inventorySlot] = itemToAdd.maxShdAtk;
                                break;
                            case ItemData.ItemType.Armor:
                                statCounter[inventorySlot] = itemToAdd.maxArm;
                                break;
                        }

                        statValue[inventorySlot].text = statCounter[inventorySlot].ToString();
                        #endregion

                        return;
                    }
                    else
                    {
                        ItemImage[inventorySlot].enabled = true;
                        itemquantity[inventorySlot].enabled = true;
                        ItemCounter[inventorySlot] = itemToAdd.quantity;
                        itemquantity[inventorySlot].text = "x" + ItemCounter[inventorySlot].ToString();

                        #region Set Inventory BG Color

                        switch (itemData[inventorySlot].rankNum)
                        {
                            case 0:
                                highlight[inventorySlot].color = new Color(0, 0, 0, 80);
                                highlight[inventorySlot].enabled = true;
                                break;
                            case 1:
                                highlight[inventorySlot].GetComponent<UIGradient>().enabled = true;
                                highlight[inventorySlot].GetComponent<UIGradient>().LinearColor1 = rank1Color1;
                                highlight[inventorySlot].GetComponent<UIGradient>().LinearColor2 = rank1Color2;
                                break;
                            case 2:
                                highlight[inventorySlot].GetComponent<UIGradient>().enabled = true;
                                highlight[inventorySlot].GetComponent<UIGradient>().LinearColor1 = rank2Color1;
                                highlight[inventorySlot].GetComponent<UIGradient>().LinearColor2 = rank2Color2;
                                break;
                            case 3:
                                highlight[inventorySlot].GetComponent<UIGradient>().enabled = true;
                                highlight[inventorySlot].GetComponent<UIGradient>().LinearColor1 = rank3Color1;
                                highlight[inventorySlot].GetComponent<UIGradient>().LinearColor2 = rank3Color2;
                                break;
                        }

                        if (itemData[inventorySlot].broken)
                        {
                            highlight[inventorySlot].GetComponent<UIGradient>().enabled = true;
                            highlight[inventorySlot].GetComponent<UIGradient>().LinearColor1 = brokenColor1;
                            highlight[inventorySlot].GetComponent<UIGradient>().LinearColor2 = brokenColor2;
                        }

                        #endregion

                        return;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Remove an item from a single inventory slot
    /// </summary>
    public void RemoveNonStackableItem(ref ItemData[] itemData, ref Image[] itemImage, ref Image[] highLight, ref Image[] outline, ref Image[] statBG,
    ref TextMeshProUGUI[] itemQuantity, ref TextMeshProUGUI[] statValue, ref int inventorySlot, ref int nullSlots, ref int[] itemCounter, ref int[] statCounter)
    {
        #region Remove Equipment Icon

        switch (itemData[inventorySlot].itemType)
        {
            case ItemData.ItemType.Weapon:
                referencesObj.primaryItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = 0;
                referencesObj.primaryItemImage.enabled = false;
                referencesObj.primaryItemImage.sprite = null;
                referencesObj.primaryValueBG.enabled = false;
                referencesObj.primaryValueBG.GetComponentInChildren<TextMeshProUGUI>().text = "";
                break;
            case ItemData.ItemType.Bow:
                referencesObj.secondaryItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = 0;
                referencesObj.secondaryItemImage.enabled = false;
                referencesObj.secondaryItemImage.sprite = null;
                referencesObj.secondaryValueBG.enabled = false;
                referencesObj.secondaryValueBG.GetComponentInChildren<TextMeshProUGUI>().text = "";
                break;
            case ItemData.ItemType.Shield:
                referencesObj.shieldItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = 0;
                referencesObj.shieldItemImage.enabled = false;
                referencesObj.shieldItemImage.sprite = null;
                referencesObj.shieldValueBG.enabled = false;
                referencesObj.shieldValueBG.GetComponentInChildren<TextMeshProUGUI>().text = "";
                break;
            case ItemData.ItemType.Armor:
                if (itemData[inventorySlot].arm.armorType == ItemData.ArmorType.Head)
                {
                    referencesObj.headItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = 0;
                    referencesObj.headItemImage.enabled = true;
                    referencesObj.headItemImage.sprite = null;
                    referencesObj.headValueBG.enabled = true;
                    referencesObj.headValueBG.GetComponentInChildren<TextMeshProUGUI>().text = "";
                }
                else if (itemData[inventorySlot].arm.armorType == ItemData.ArmorType.Chest)
                {
                    referencesObj.chestItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = 0;
                    referencesObj.chestItemImage.enabled = true;
                    referencesObj.chestItemImage.sprite = null;
                    referencesObj.chestValueBG.enabled = true;
                    referencesObj.chestValueBG.GetComponentInChildren<TextMeshProUGUI>().text = "";
                }
                else if (itemData[inventorySlot].arm.armorType == ItemData.ArmorType.Legs)
                {
                    referencesObj.legItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = 0;
                    referencesObj.legItemImage.enabled = true;
                    referencesObj.legItemImage.sprite = null;
                    referencesObj.legValueBG.enabled = true;
                    referencesObj.legValueBG.GetComponentInChildren<TextMeshProUGUI>().text = "";
                }
                else if (itemData[inventorySlot].arm.armorType == ItemData.ArmorType.Amulet)
                {
                    referencesObj.amuletItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = 0;
                    referencesObj.amuletItemImage.enabled = true;
                    referencesObj.amuletItemImage.sprite = null;
                    referencesObj.amuletValueBG.enabled = true;
                    referencesObj.amuletValueBG.GetComponentInChildren<TextMeshProUGUI>().text = "";
                }
                break;
        }

        #endregion

        nullSlots++;
        itemData[inventorySlot] = null;
        itemImage[inventorySlot].sprite = null;
        itemImage[inventorySlot].enabled = false;
        statBG[inventorySlot].enabled = false;
        itemQuantity[inventorySlot].enabled = false;
        outline[inventorySlot].enabled = false;
        statValue[inventorySlot].enabled = false;
        highLight[inventorySlot].enabled = true;

        Navigation none = new Navigation { mode = Navigation.Mode.None };
        itemImage[inventorySlot].GetComponent<Button>().navigation = none;
        Array.Sort(itemData, delegate (ItemData x, ItemData y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            return x.buyPrice.CompareTo(y.buyPrice);
        });
        Array.Reverse(itemData);
        ItemData[] temp = new ItemData[itemData.Length];
        for (int i = 0; i < itemData.Length; i++)
        {
            temp[i] = itemData[i];

            itemData[i] = null;
            itemImage[i].sprite = null;
            highLight[i].color = new Color(0, 0, 0, 80);
            itemCounter[i] = 0;
            statCounter[i] = 0;
            itemQuantity[i].text = "";
            statValue[i].text = "";
            statBG[i].enabled = false;
        }
        itemData = new ItemData[itemData.Length - 1];
        for (int i = 0; i < itemData.Length; i++)
        {
            nullSlots = 0;
            itemData[i] = temp[i];
            itemImage[i].sprite = temp[i].itemSprite;

            #region Set Inventory BG Color

            switch (itemData[i].rankNum)
            {
                case 0:
                    highLight[i].color = new Color(0, 0, 0, 80);
                    highLight[i].enabled = true;
                    highLight[i].GetComponent<UIGradient>().enabled = false;
                    break;
                case 1:
                    highLight[i].GetComponent<UIGradient>().enabled = true;
                    highLight[i].GetComponent<UIGradient>().LinearColor1 = rank1Color1;
                    highLight[i].GetComponent<UIGradient>().LinearColor2 = rank1Color2;
                    break;
                case 2:
                    highLight[i].GetComponent<UIGradient>().enabled = true;
                    highLight[i].GetComponent<UIGradient>().LinearColor1 = rank2Color1;
                    highLight[i].GetComponent<UIGradient>().LinearColor2 = rank2Color2;
                    break;
                case 3:
                    highLight[i].GetComponent<UIGradient>().enabled = true;
                    highLight[i].GetComponent<UIGradient>().LinearColor1 = rank3Color1;
                    highLight[i].GetComponent<UIGradient>().LinearColor2 = rank3Color2;
                    break;
            }

            if (itemData[i].broken)
            {
                highLight[i].GetComponent<UIGradient>().enabled = true;
                highLight[i].GetComponent<UIGradient>().LinearColor1 = brokenColor1;
                highLight[i].GetComponent<UIGradient>().LinearColor2 = brokenColor2;
            }

            #endregion

            itemCounter[i] = temp[i].quantity;

            Navigation automatic = new Navigation { mode = Navigation.Mode.Automatic };
            itemImage[i].GetComponent<Button>().navigation = automatic;

            #region Set Stat Value

            switch (itemData[i].itemType)
            {
                case ItemData.ItemType.Weapon:
                    statValue[i].enabled = true;
                    statCounter[i] = itemData[i].maxWpnAtk;
                    statValue[i].text = statCounter[i].ToString();
                    break;
                case ItemData.ItemType.Bow:
                    statValue[i].enabled = true;
                    statCounter[i] = itemData[i].maxBowAtk;
                    statValue[i].text = statCounter[i].ToString();
                    break;
                case ItemData.ItemType.Shield:
                    statValue[i].enabled = true;
                    statCounter[i] = itemData[i].maxShdAtk;
                    statValue[i].text = statCounter[i].ToString();
                    break;
                case ItemData.ItemType.Armor:
                    statValue[i].enabled = true;
                    statCounter[i] = itemData[i].maxArm;
                    statValue[i].text = statCounter[i].ToString();
                    break;
            }

            #endregion

            if (itemData[i].stackable)
            {
                itemQuantity[i].enabled = true;
                itemQuantity[i].text = "x" + itemCounter[i].ToString();
            }
            else
            {
                itemQuantity[i].text = "";
                itemQuantity[i].enabled = false;
            }

            foreach (Image imgs in itemImage)
                if (imgs.sprite != null)
                    imgs.enabled = true;
                else
                    imgs.enabled = false;
            if (statCounter[i] > 0)
                statBG[i].enabled = true;
            else
                statBG[i].enabled = false;
            if (itemCounter[i] > 0)
                itemQuantity[i].enabled = true;
            else
                itemQuantity[i].enabled = false;
        }
    }

    /// <summary>
    /// Remove a stackable item from a single inventory slot
    /// </summary>
    public void RemoveStackableItem(ref ItemData[] itemData, ref Image[] itemImage, ref Image[] highLight, ref Image[] outline, ref Image[] statBG,
    ref TextMeshProUGUI[] itemQuantity, ref TextMeshProUGUI[] statValue, ref int inventorySlot, ref int[] itemCounter, ref int[] statCounter, ref int nullSlots, int quantity)
    {
        itemCounter[inventorySlot] -= quantity;

        itemQuantity[inventorySlot].text = "x" + itemCounter[inventorySlot].ToString();
        if (itemCounter[inventorySlot] < 1)
        {
            #region Remove Equipment Icon

            switch (itemData[inventorySlot].itemType)
            {
                case ItemData.ItemType.Arrow:
                    referencesObj.arrowItemImage.enabled = false;
                    referencesObj.arrowItemImage.sprite = null;
                    referencesObj.arrowValueBG.enabled = false;
                    referencesObj.arrowValueBG.GetComponentInChildren<TextMeshProUGUI>().text = "";
                    break;
            }

            #endregion

            switch (itemData[inventorySlot].itemType)
            {
                case ItemData.ItemType.Arrow:
                    DeactiveArrows();
                    break;
            }

            nullSlots++;
            itemData[inventorySlot] = null;
            itemImage[inventorySlot].sprite = null;
            itemImage[inventorySlot].enabled = false;
            statBG[inventorySlot].enabled = false;
            itemQuantity[inventorySlot].enabled = false;
            outline[inventorySlot].enabled = false;
            statValue[inventorySlot].enabled = false;
            highLight[inventorySlot].enabled = true;

            Navigation none = new Navigation { mode = Navigation.Mode.None };
            itemImage[inventorySlot].GetComponent<Button>().navigation = none;

            Array.Sort(itemData, delegate (ItemData x, ItemData y)
            {
                if (x == null && y == null) return 0;
                if (x == null) return -1;
                if (y == null) return 1;
                return x.buyPrice.CompareTo(y.buyPrice);
            });
            Array.Reverse(itemData);

            ItemData[] temp = new ItemData[itemData.Length];
            for (int i = 0; i < itemData.Length; i++)
            {
                temp[i] = itemData[i];

                itemData[i] = null;
                itemImage[i].sprite = null;
                highLight[i].color = new Color(0, 0, 0, 80);
                itemCounter[i] = 0;
                statCounter[i] = 0;
                itemQuantity[i].text = "";
                statValue[i].text = "";
                statBG[i].enabled = false;
            }

            itemData = new ItemData[itemData.Length - 1];
            for (int i = 0; i < itemData.Length; i++)
            {
                nullSlots = 0;
                itemData[i] = temp[i];
                itemImage[i].sprite = temp[i].itemSprite;

                itemCounter[i] = temp[i].quantity;
                Navigation automatic = new Navigation { mode = Navigation.Mode.Automatic };
                itemImage[i].GetComponent<Button>().navigation = automatic;
                
                #region Set Stat Value

                switch (itemData[i].itemType)
                {
                    case ItemData.ItemType.Weapon:
                        statValue[i].enabled = true;
                        statCounter[i] = itemData[i].maxWpnAtk;
                        statValue[i].text = statCounter[i].ToString();
                        break;
                    case ItemData.ItemType.Bow:
                        statValue[i].enabled = true;
                        statCounter[i] = itemData[i].maxBowAtk;
                        statValue[i].text = statCounter[i].ToString();
                        break;
                    case ItemData.ItemType.Shield:
                        statValue[i].enabled = true;
                        statCounter[i] = itemData[i].maxShdAtk;
                        statValue[i].text = statCounter[i].ToString();
                        break;
                    case ItemData.ItemType.Armor:
                        statValue[i].enabled = true;
                        statCounter[i] = itemData[i].maxArm;
                        statValue[i].text = statCounter[i].ToString();
                        break;
                }

                #endregion

                if (itemData[i].stackable)
                {
                    itemQuantity[i].enabled = true;
                    itemQuantity[i].text = "x" + itemCounter[i].ToString();
                }
                else
                {
                    itemQuantity[i].text = "";
                    itemQuantity[i].enabled = false;
                }

                foreach (Image imgs in itemImage)
                    if (imgs.sprite != null)
                        imgs.enabled = true;
                    else
                        imgs.enabled = false;
                if (statCounter[i] > 0)
                    statBG[i].enabled = true;
                else
                    statBG[i].enabled = false;
                if (itemCounter[i] > 0)
                    itemQuantity[i].enabled = true;
                else
                    itemQuantity[i].enabled = false;
            }
        }
    }

    public void AddedItemUI(ref GameObject itemData)
    {
        ItemData itemDataConvert = itemData.GetComponentInChildren<ItemData>();
        if (addedItemList.itemAdded.Count <= 3 && cc.infoMessage.info.text != "You don't have enough inventory space.")
        {
            addedItemList.itemAdded.Add("Item");

            prefabHolder = Instantiate(addedItemPrefab) as GameObject;
            prefabHolder.GetComponent<AddItemsToList>().tempItemData = itemDataConvert;

            prefabHolder.gameObject.name = itemDataConvert.itemName + " AddedToList";
            prefabHolder.GetComponent<AddItemsToList>().itemName = itemDataConvert.itemName;
            prefabHolder.GetComponent<AddItemsToList>().itemSprite = itemDataConvert.itemSprite;
        }
    }

    #endregion

    #region Create Inventory Slots

    public void SetWeaponSlot(ref int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            CreateInventorySlot(ref i, InventorySlot.SlotType.Weapon, ref weaponInv.counter, ref weaponInv.statCounter,
            ref weaponInv.itemData, ref weaponInv.image, ref weaponInv.highLight, ref weaponInv.outLineBorder, ref weaponInv.statValueBG,
            ref weaponInv.statValue, ref weaponInv.quantity);

            weaponInv.weaponSlotActive = true;
        }
    }

    public void SetBowAndArrowSlot(ref int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            CreateInventorySlot(ref i, InventorySlot.SlotType.BowAndArrow, ref bowAndArrowInv.counter, ref bowAndArrowInv.statCounter,
            ref bowAndArrowInv.itemData, ref bowAndArrowInv.image, ref bowAndArrowInv.highLight, ref bowAndArrowInv.outLineBorder, ref bowAndArrowInv.statValueBG,
            ref bowAndArrowInv.statValue, ref bowAndArrowInv.quantity);

            bowAndArrowInv.bowAndArrowSlotActive = true;
        }
    }

    public void SetShieldSlot(ref int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            CreateInventorySlot(ref i, InventorySlot.SlotType.Shield, ref shieldInv.counter, ref shieldInv.statCounter,
            ref shieldInv.itemData, ref shieldInv.image, ref shieldInv.highLight, ref shieldInv.outLineBorder, ref shieldInv.statValueBG,
            ref shieldInv.statValue, ref shieldInv.quantity);

            shieldInv.shieldSlotActive = true;
        }
    }

    public void SetArmorSlot(ref int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            CreateInventorySlot(ref i, InventorySlot.SlotType.Armor, ref armorInv.counter, ref armorInv.statCounter,
            ref armorInv.itemData, ref armorInv.image, ref armorInv.highLight, ref armorInv.outLineBorder, ref armorInv.statValueBG,
            ref armorInv.statValue, ref armorInv.quantity);

            armorInv.armorSlotActive = true;
        }
    }

    public void SetMaterialSlot(ref int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            CreateInventorySlot(ref i, InventorySlot.SlotType.Material, ref materialInv.counter, ref materialInv.statCounter,
            ref materialInv.itemData, ref materialInv.image, ref materialInv.highLight, ref materialInv.outLineBorder, ref materialInv.statValueBG,
            ref materialInv.statValue, ref materialInv.quantity);

            materialInv.materialSlotActive = true;
        }
    }

    /// <summary>
    /// The total number of healing inventory slots
    /// </summary>
    /// <param name="amount">How many slots?</param>
    public void SetHealingSlot(ref int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            CreateInventorySlot(ref i, InventorySlot.SlotType.Healing, ref healingInv.counter, ref healingInv.statCounter,
            ref healingInv.itemData, ref healingInv.image, ref healingInv.highLight, ref healingInv.outLineBorder, ref healingInv.statValueBG,
            ref healingInv.statValue, ref healingInv.quantity);

            healingInv.healingSlotActive = true;
        }
    }

    /// <summary>
    /// The total number of key inventory slots
    /// </summary>
    /// <param name="amount">How many slots?</param>
    public void SetKeySlot(ref int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            CreateInventorySlot(ref i, InventorySlot.SlotType.Key, ref keyInv.counter, ref keyInv.statCounter,
            ref keyInv.itemData, ref keyInv.image, ref keyInv.highLight, ref keyInv.outLineBorder, ref keyInv.statValueBG,
            ref keyInv.statValue, ref keyInv.quantity);

            keyInv.keySlotActive = true;
        }
    }

    /// <summary>
    /// create desired inventory slot 
    /// </summary>
    void CreateInventorySlot(ref int numOfSlot, InventorySlot.SlotType slotType, ref int[] inventoryCounter, ref int[] statCounter, ref ItemData[] itemData, ref Image[] itemImage, ref Image[] highLight, ref Image[] outLineBorder, ref Image[] statValueBG, ref TextMeshProUGUI[] statValue, ref TextMeshProUGUI[] quantity)
    {
        GameObject newSlot = Instantiate(createSlotPrefab);
        InventorySlot slotData = newSlot.GetComponent<InventorySlot>();
        slotData.slotType = slotType;

        Image[] temp1 = new Image[itemImage.Length + 1];
        Image[] temp2 = new Image[highLight.Length + 1];
        Image[] temp3 = new Image[outLineBorder.Length + 1];
        Image[] temp4 = new Image[statValueBG.Length + 1];
        TextMeshProUGUI[] temp5 = new TextMeshProUGUI[quantity.Length + 1];
        TextMeshProUGUI[] temp6 = new TextMeshProUGUI[statValue.Length + 1];
        int[] temp7 = new int[inventoryCounter.Length + 1];
        int[] temp8 = new int[statCounter.Length + 1];

        for (int i = 0; i < itemImage.Length; i++)
        {
            if (itemImage[0] != null)
            {
                temp1[i] = itemImage[i];
                temp2[i] = highLight[i];
                temp3[i] = outLineBorder[i];
                temp4[i] = statValueBG[i];
                temp5[i] = quantity[i];
                temp6[i] = statValue[i];
                temp7[i] = inventoryCounter[i];
                temp8[i] = statCounter[i];
            }
        }

        itemImage = temp1;
        highLight = temp2;
        outLineBorder = temp3;
        statValueBG = temp4;
        quantity = temp5;
        statValue = temp6;
        inventoryCounter = temp7;
        statCounter = temp8;

        itemImage[numOfSlot] = slotData.itemImage;
        highLight[numOfSlot] = slotData.highLight;
        outLineBorder[numOfSlot] = slotData.outlineBorder;
        statValueBG[numOfSlot] = slotData.statValueBG;
        quantity[numOfSlot] = slotData.itemQuantity;
        statValue[numOfSlot] = slotData.statValue;

        slotData.slotNum = numOfSlot;
    }

    #endregion

    #region Inventory Item Select

    public void ItemSelect(int slotNumber)
    {
        switch (inventoryS)
        {
            case InventorySection.Weapon:
                weaponInv.slotNum = slotNumber;
                OnButtonSelect(ref weaponInv.itemData, ref weaponInv.image, ref weaponInv.highLight, ref weaponInv.slotNum);
                break;
            case InventorySection.BowAndArrow:
                bowAndArrowInv.slotNum = slotNumber;
                OnButtonSelect(ref bowAndArrowInv.itemData, ref bowAndArrowInv.image, ref bowAndArrowInv.highLight, ref bowAndArrowInv.slotNum);
                break;
            case InventorySection.Shield:
                bowAndArrowInv.slotNum = slotNumber;
                OnButtonSelect(ref shieldInv.itemData, ref shieldInv.image, ref shieldInv.highLight, ref shieldInv.slotNum);
                break;
            case InventorySection.Armor:
                armorInv.slotNum = slotNumber;
                OnButtonSelect(ref armorInv.itemData, ref armorInv.image, ref armorInv.highLight, ref armorInv.slotNum);
                break;
            case InventorySection.Material:
                materialInv.slotNum = slotNumber;
                OnButtonSelect(ref materialInv.itemData, ref materialInv.image, ref materialInv.highLight, ref materialInv.slotNum);
                break;
            case InventorySection.Healing:
                healingInv.slotNum = slotNumber;
                OnButtonSelect(ref healingInv.itemData, ref healingInv.image, ref healingInv.highLight, ref healingInv.slotNum);
                break;
            case InventorySection.Key:
                keyInv.slotNum = slotNumber;
                OnButtonSelect(ref keyInv.itemData, ref keyInv.image, ref keyInv.highLight, ref keyInv.slotNum);
                break;
        }
    }

    void OnButtonSelect(ref ItemData[] itemData, ref Image[] itemImage, ref Image[] highLight, ref int inventorySlot)
    {
        // we prevent other buttons from being accessed once selected
        if (storeM.storeItemPreviewAlpha.canvasGroup.alpha == 0)
        {
            if (inventoryS == InventorySection.Material) // Drop Options
            {
                referencesObj.itemOptionsDropObj.SetActive(true);
                referencesObj.itemOptionsDropObj.transform.position = highLight[inventorySlot].transform.position;
            }
            else if (inventoryS == InventorySection.Healing) // Consume Options
            {
                referencesObj.itemOptionsConsumeObj.SetActive(true);
                referencesObj.itemOptionsConsumeObj.transform.position = highLight[inventorySlot].transform.position;
            }
            else if (inventoryS == InventorySection.Key) // Preview Options
            {
                referencesObj.itemOptionsPreviewObj.SetActive(true);
                referencesObj.itemOptionsPreviewObj.transform.position = highLight[inventorySlot].transform.position;
            }
            else
            {
                if (itemImage[inventorySlot].enabled)
                {
                    if (!itemData[inventorySlot].equipped) // Equip Options
                    {
                        referencesObj.itemOptionsEquipObj.SetActive(true);
                        referencesObj.itemOptionsEquipObj.transform.position = highLight[inventorySlot].transform.position;
                    }
                    else                                   // Remove Options
                    {
                        referencesObj.itemOptionsRemoveObj.SetActive(true);
                        referencesObj.itemOptionsRemoveObj.transform.position = highLight[inventorySlot].transform.position;
                    }
                }
            }
        }
        // Sell Options
        else if (storeM.sellM.isActive)
        {
            switch (inventoryS)
            {
                case InventorySection.Weapon:
                    storeM.sellM.CheckInventoryItems(ref weaponInv.itemData, ref weaponInv.slotNum, ref weaponInv.counter);
                    break;
                case InventorySection.BowAndArrow:
                    if (bowAndArrowInv.itemData[bowAndArrowInv.slotNum].itemType == ItemData.ItemType.Bow)
                        storeM.sellM.CheckInventoryItems(ref bowAndArrowInv.itemData, ref bowAndArrowInv.slotNum, ref bowAndArrowInv.counter);
                    if (bowAndArrowInv.itemData[bowAndArrowInv.slotNum].itemType == ItemData.ItemType.Arrow)
                        storeM.sellM.CheckInventoryItems(ref bowAndArrowInv.itemData, ref bowAndArrowInv.slotNum, ref bowAndArrowInv.counter);
                    break;
                case InventorySection.Shield:
                    storeM.sellM.CheckInventoryItems(ref shieldInv.itemData, ref shieldInv.slotNum, ref shieldInv.counter);
                    break;
                case InventorySection.Armor:
                    storeM.sellM.CheckInventoryItems(ref armorInv.itemData, ref armorInv.slotNum, ref armorInv.counter);
                    break;
                case InventorySection.Material:
                    storeM.sellM.CheckInventoryItems(ref materialInv.itemData, ref materialInv.slotNum, ref materialInv.counter);
                    break;
                case InventorySection.Healing:
                    storeM.sellM.CheckInventoryItems(ref healingInv.itemData, ref healingInv.slotNum, ref healingInv.counter);
                    break;
            }
            storeM.sellM.sparkleParticle.Play();
            referencesObj.itemOptionsSellObj.SetActive(true);
            referencesObj.itemOptionsSellObj.transform.position = highLight[inventorySlot].transform.position;
        }

        // Repair Options
        else if (storeM.repairM.isActive)
        {
            switch (inventoryS)
            {
                case InventorySection.Weapon:
                    storeM.repairM.CheckInventoryItems(ref weaponInv.itemData, ref weaponInv.slotNum, ref weaponInv.counter);
                    break;
                case InventorySection.BowAndArrow:
                    if (bowAndArrowInv.itemData[bowAndArrowInv.slotNum].itemType == ItemData.ItemType.Bow)
                        storeM.repairM.CheckInventoryItems(ref bowAndArrowInv.itemData, ref bowAndArrowInv.slotNum, ref bowAndArrowInv.counter);
                    break;
                case InventorySection.Shield:
                    storeM.repairM.CheckInventoryItems(ref shieldInv.itemData, ref shieldInv.slotNum, ref shieldInv.counter);
                    break;
            }
            storeM.repairM.sparkleParticle.Play();
            referencesObj.itemOptionsRepairObj.SetActive(true);
            referencesObj.itemOptionsRepairObj.transform.position = highLight[inventorySlot].transform.position;
        }

        // Upgrade Options
        else if (storeM.upgradeM.isActive)
        {
            switch (inventoryS)
            {
                case InventorySection.Weapon:
                    storeM.upgradeM.CheckInventoryItems(ref weaponInv.itemData, ref weaponInv.slotNum, ref weaponInv.counter);
                    break;
                case InventorySection.BowAndArrow:
                    if (bowAndArrowInv.itemData[bowAndArrowInv.slotNum].itemType == ItemData.ItemType.Bow)
                        storeM.upgradeM.CheckInventoryItems(ref bowAndArrowInv.itemData, ref bowAndArrowInv.slotNum, ref bowAndArrowInv.counter);
                    break;
                case InventorySection.Shield:
                    storeM.upgradeM.CheckInventoryItems(ref shieldInv.itemData, ref shieldInv.slotNum, ref shieldInv.counter);
                    break;
                case InventorySection.Armor:
                    storeM.upgradeM.CheckInventoryItems(ref armorInv.itemData, ref armorInv.slotNum, ref armorInv.counter);
                    break;
            }
            storeM.upgradeM.sparkleParticle.Play();
            referencesObj.itemOptionsUpgradeObj.SetActive(true);
            referencesObj.itemOptionsUpgradeObj.transform.position = highLight[inventorySlot].transform.position;
        }

        // Special Options
        else if (storeM.specialM.isActive)
        {
            switch (inventoryS)
            {
                case InventorySection.Weapon:
                    storeM.specialM.CheckInventoryItems(ref weaponInv.itemData, ref weaponInv.slotNum, ref weaponInv.counter);
                    referencesObj.itemOptionsWeaponSpecialObj.SetActive(true);
                    referencesObj.itemOptionsWeaponSpecialObj.transform.position = highLight[inventorySlot].transform.position;
                    break;
                case InventorySection.BowAndArrow:
                    if (bowAndArrowInv.itemData[bowAndArrowInv.slotNum].itemType == ItemData.ItemType.Bow)
                    {
                        storeM.specialM.CheckInventoryItems(ref bowAndArrowInv.itemData, ref bowAndArrowInv.slotNum, ref bowAndArrowInv.counter);
                        referencesObj.itemOptionsBowSpecialObj.SetActive(true);
                        referencesObj.itemOptionsBowSpecialObj.transform.position = highLight[inventorySlot].transform.position;
                    }
                    break;
                case InventorySection.Shield:
                    storeM.specialM.CheckInventoryItems(ref shieldInv.itemData, ref shieldInv.slotNum, ref shieldInv.counter);
                    referencesObj.itemOptionsShieldSpecialObj.SetActive(true);
                    referencesObj.itemOptionsShieldSpecialObj.transform.position = highLight[inventorySlot].transform.position;
                    break;
                case InventorySection.Armor:
                    storeM.specialM.CheckInventoryItems(ref armorInv.itemData, ref armorInv.slotNum, ref armorInv.counter);
                    referencesObj.itemOptionsArmorSpecialObj.SetActive(true);
                    referencesObj.itemOptionsArmorSpecialObj.transform.position = highLight[inventorySlot].transform.position;
                    break;
            }
            storeM.specialM.sparkleParticle.Play();
        }
    }

    public void SetItemDescriptionSettings()
    {
        if (itemDescription == null)
            return;

        itemDescription.inventoryD = ItemDescription.InventoryDescription.Null;

        switch (inventoryS)
        {
            case InventorySection.Weapon:
                itemDescription.inventoryD = ItemDescription.InventoryDescription.Weapon;
                break;
            case InventorySection.BowAndArrow:
                if (bowAndArrowInv.itemData != null)
                {
                    if (bowAndArrowInv.itemData[bowAndArrowInv.slotNum].itemType == ItemData.ItemType.Bow)
                        itemDescription.inventoryD = ItemDescription.InventoryDescription.Bow;
                    if (bowAndArrowInv.itemData[bowAndArrowInv.slotNum].itemType == ItemData.ItemType.Arrow)
                        itemDescription.inventoryD = ItemDescription.InventoryDescription.Arrow;
                }
                break;
            case InventorySection.Shield:
                itemDescription.inventoryD = ItemDescription.InventoryDescription.Shield;
                break;
            case InventorySection.Armor:
                itemDescription.inventoryD = ItemDescription.InventoryDescription.Armor;
                break;
            case InventorySection.Material:
                itemDescription.inventoryD = ItemDescription.InventoryDescription.Material;
                break;
            case InventorySection.Healing:
                itemDescription.inventoryD = ItemDescription.InventoryDescription.Healing;
                break;
            case InventorySection.Key:
                itemDescription.inventoryD = ItemDescription.InventoryDescription.Key;
                break;
        }
    }

    #endregion

    public void NavLeftSection()
    {
        ItemOptionsIsActive(false);
        PauseMenuSectionIsActive(false);
        pauseMenuNavigation--;
    }

    public void NavRightSection()
    {
        ItemOptionsIsActive(false);
        PauseMenuSectionIsActive(false);
        pauseMenuNavigation++;
    }

    public void UpdatePauseMenu()
    {
        bool conditionToNavigate = referencesObj.inputFieldOptions.GetComponent<FadeUI>().canvasGroup.alpha == 0 &&
        referencesObj.abilityAssignMenu.GetComponent<FadeUI>().canvasGroup.alpha == 0;

        if (cc.cc.rpgaeIM.PlayerControls.Secondary.triggered && conditionToNavigate)
        {
            if (pauseMenuNavigation > 0)
                NavLeftSection();
        }
        if (cc.rpgaeIM.PlayerControls.RightBumber.triggered && conditionToNavigate)
        {
            if (pauseMenuNavigation < 4)
                NavRightSection();
        }

        if (referencesObj.pauseMenu.GetComponent<FadeUI>().canvasGroup.alpha == 1)
        {
            UpdateRendererEnable(false);

            // CURSOR 
            if (pauseMenuS == PauseMenuSection.Skills || pauseMenuS == PauseMenuSection.Map)
            {
                cursorVirtual.isActive = false;
                cursorVirtual.SetCursorToCenter();
            }
            else
            {
                cursorVirtual.isActive = true;
            }

            // WOLRD MAP
            if (pauseMenuS != PauseMenuSection.Map)
            {
                IsWorldSpaceActive(false);
                //GraphicsSettings.defaultRenderPipeline = null;
                //QualitySettings.renderPipeline = null;

                miniMap.fadeUI.canvasGroup.alpha = 0;
                UIcanvas.renderMode = RenderMode.ScreenSpaceCamera;

                cc.hudM.fadeUIQuestObjective.canvasGroup.alpha = 0;
                cc.hudM.fadeUIBG.canvasGroup.alpha = 0;
                cc.hudM.fadeUIFadeGroup.canvasGroup.alpha = 0;
            }
            else
            {
                if (isPauseMenuOn)
                {
                    IsWorldSpaceActive(true);
                    //GraphicsSettings.defaultRenderPipeline = defaultRenderPipelineAsset;
                    //QualitySettings.renderPipeline = defaultRenderPipelineAsset;

                    miniMap.fadeUI.canvasGroup.alpha = 1;
                    UIcanvas.renderMode = RenderMode.ScreenSpaceOverlay;

                    ShowCurrentObjectiveUI(ref systemM.questM.mainQuestSlots.questData, ref systemM.questM.mainQuestSlots.highLight);
                    ShowCurrentObjectiveUI(ref systemM.questM.sideQuestSlots.questData, ref systemM.questM.sideQuestSlots.highLight);

                    referencesObj.InventoryTitleBG.GetComponent<FadeUI>().canvasGroup.alpha = 1;
                }
            }

            // COIN 
            if(pauseMenuS != PauseMenuSection.Inventory)
            {
                referencesObj.coinHUD.canvasGroup.alpha = 0;
            }
            else
            {
                referencesObj.coinHUD.canvasGroup.alpha = 1;
                referencesObj.inventorySection.GetComponent<FadeUI>().canvasGroup.alpha = 1;
            }

            // LEVEL
            if (pauseMenuS == PauseMenuSection.Inventory || pauseMenuS == PauseMenuSection.Quest || pauseMenuS == PauseMenuSection.Skills)
                referencesObj.levelGuage.canvasGroup.alpha = 1;
            else
                referencesObj.levelGuage.canvasGroup.alpha = 0;

            // SYSTEM
            if (pauseMenuS == PauseMenuSection.System)
            {
               PauseMenuDataCollection();
            }
        }

        UpdateInventoryGrid();
        UpdatePauseMenuSection();
    }

    void ShowCurrentObjectiveUI(ref QuestData[] questData, ref Image[] questHighLight)
    {
        for (int i = 0; i < questData.Length; i++)
        {
            if (questData[i] != null)
            {
                if (questData[i].isQuestActive)
                {
                    if (!questData[i].isComplete)
                    {
                        if (!isPauseMenuOn)
                        {
                            if (cc.hudM.fadeUIQuestObjective.isFading)
                                cc.hudM.fadeUIQuestObjective.isFading = false;
                            if (cc.hudM.fadeUIBG.isFading)
                                cc.hudM.fadeUIBG.isFading = false;
                            if (cc.hudM.fadeUIFadeGroup.isFading)
                                cc.hudM.fadeUIFadeGroup.isFading = false;

                            cc.hudM.fadeUIQuestObjective.FadeTransition(1, 0, 0.5f);
                            cc.hudM.fadeUIBG.FadeTransition(1, 0, 0.5f);
                            cc.hudM.fadeUIFadeGroup.FadeTransition(1, 2, 0.5f);
                        }
                        else
                        {
                            if (miniMap.fadeUILegendInfo.canvasGroup.alpha == 0)
                            {
                                cc.hudM.fadeUIQuestObjective.canvasGroup.alpha = 1;
                                cc.hudM.fadeUIBG.canvasGroup.alpha = 1;
                                cc.hudM.fadeUIFadeGroup.canvasGroup.alpha = 1;
                            }
                            else
                            {
                                cc.hudM.fadeUIQuestObjective.canvasGroup.alpha = 0;
                                cc.hudM.fadeUIBG.canvasGroup.alpha = 0;
                                cc.hudM.fadeUIFadeGroup.canvasGroup.alpha = 0;
                            }
                        }
                    }
                }
            }
        }
    }

    public void PauseMenuDataCollection()
    {
        referencesObj.inventorySection.SetActive(true);
        referencesObj.inventorySection.GetComponent<CanvasGroup>().alpha = 0;
        referencesObj.inventorySection.GetComponent<CanvasGroup>().interactable = false;
        referencesObj.inventorySection.GetComponent<CanvasGroup>().blocksRaycasts = false;

        referencesObj.questSection.SetActive(true);
        referencesObj.questSection.GetComponent<CanvasGroup>().alpha = 0;
        referencesObj.questSection.GetComponent<CanvasGroup>().interactable = false;
        referencesObj.questSection.GetComponent<CanvasGroup>().blocksRaycasts = false;

        referencesObj.mapSection.SetActive(true);
        referencesObj.mapSection.GetComponent<CanvasGroup>().alpha = 0;
        referencesObj.mapSection.GetComponent<CanvasGroup>().interactable = false;
        referencesObj.mapSection.GetComponent<CanvasGroup>().blocksRaycasts = false;

        referencesObj.skillsSection.SetActive(true);
        referencesObj.skillsSection.GetComponent<CanvasGroup>().alpha = 0;
        referencesObj.skillsSection.GetComponent<CanvasGroup>().interactable = false;
        referencesObj.skillsSection.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    void UpdateRendererEnable(bool active)
    {
        foreach (MeshRenderer mesh in FindObjectsOfType<MeshRenderer>())
        {
            if (mesh.gameObject.layer == LayerMask.NameToLayer("Player"))
                mesh.enabled = active;
        }
        foreach (SkinnedMeshRenderer smesh in FindObjectsOfType<SkinnedMeshRenderer>())
        {
            if (smesh.gameObject.layer == LayerMask.NameToLayer("Player"))
                smesh.enabled = active;
        }
        foreach (ParticleSystem ps in FindObjectsOfType<ParticleSystem>())
        {
            if (ps.gameObject.layer != LayerMask.NameToLayer("UI"))
                ps.Clear();
        }
    }

    public void IsWorldSpaceActive(bool active)
    {
        foreach (MeshRenderer mesh in allMesh)
        {
            if (mesh.gameObject.layer != 5 && mesh.tag != "Item")
            {
                mesh.enabled = active;
            }
        }
        foreach (SkinnedMeshRenderer smesh in allSkinMesh)
        {
            if (smesh.gameObject.layer != 5 && smesh.tag != "Item")
            {
                smesh.enabled = active;
            }
        }
    }

    void WorldSpaceUILayer(bool active)
    {
        if (!active)
        {
            foreach (AIHealthGuage hp in aiHp)
            {
                if (hp.GetComponent<Canvas>().sortingOrder == -2)
                {
                    hp.GetComponent<Canvas>().sortingOrder = 2;
                }
            }

            foreach (InteractWorldSpaceUI ui in uiWorldSpace)
            {
                if (ui.GetComponent<Canvas>() != null &&
                    ui.GetComponent<Canvas>().sortingOrder == -2)
                {
                    ui.GetComponent<Canvas>().sortingOrder = 2;
                }
            }
        }
        else
        {
            foreach (AIHealthGuage hp in aiHp)
            {
                if (hp.GetComponent<Canvas>().sortingOrder == 2)
                {
                    hp.GetComponent<Canvas>().sortingOrder = -2;
                }
            }

            foreach (InteractWorldSpaceUI ui in uiWorldSpace)
            {
                if (ui.GetComponent<Canvas>() != null && 
                    ui.GetComponent<Canvas>().sortingOrder == 2)
                {
                    ui.GetComponent<Canvas>().sortingOrder = -2;
                }
            }
        }
    }

    public void PauseMenuSectionIsActive(bool isActive)
    {
        referencesObj.mapSection.SetActive(isActive);
        referencesObj.questSection.SetActive(isActive);
        referencesObj.inventorySection.SetActive(isActive);
        referencesObj.skillsSection.SetActive(isActive);
        referencesObj.systemSection.SetActive(isActive);
    }

    public void InventorySectionIsActive(bool isActive)
    {
        referencesObj.weaponGrid.SetActive(isActive);
        referencesObj.bowAndArrowGrid.SetActive(isActive);
        referencesObj.shieldGrid.SetActive(isActive);
        referencesObj.armorGrid.SetActive(isActive);
        referencesObj.materialGrid.SetActive(isActive);
        referencesObj.healingGrid.SetActive(isActive);
        referencesObj.keyGrid.SetActive(isActive);
    }

    public void ItemOptionsIsActive(bool isActive)
    {
        referencesObj.itemOptionsEquipObj.SetActive(isActive);
        referencesObj.itemOptionsRemoveObj.SetActive(isActive);
        referencesObj.itemOptionsDropObj.SetActive(isActive);
        referencesObj.itemOptionsPreviewObj.SetActive(isActive);
        referencesObj.itemOptionsConsumeObj.SetActive(isActive);
        referencesObj.itemOptionsBuyObj.SetActive(isActive);
        referencesObj.itemOptionsSellObj.SetActive(isActive);
        referencesObj.itemOptionsRepairObj.SetActive(isActive);
        referencesObj.itemOptionsUpgradeObj.SetActive(isActive);
    }

    public void UpdateInventoryGrid()
    {
        switch (inventoryS)
        {
            case InventorySection.Weapon:
                referencesObj.weaponGrid.SetActive(true);
                break;
            case InventorySection.BowAndArrow:
                referencesObj.bowAndArrowGrid.SetActive(true);
                break;
            case InventorySection.Shield:
                referencesObj.shieldGrid.SetActive(true);
                break;
            case InventorySection.Armor:
                referencesObj.armorGrid.SetActive(true);
                break;
            case InventorySection.Material:
                referencesObj.materialGrid.SetActive(true);
                break;
            case InventorySection.Healing:
                referencesObj.healingGrid.SetActive(true);
                break;
            case InventorySection.Key:
                referencesObj.keyGrid.SetActive(true);
                break;
        }
    }

    public void UpdatePauseMenuSection()
    {
        switch (pauseMenuNavigation)
        {
            case 0:
                pauseMenuS = PauseMenuSection.Inventory;
                referencesObj.inventorySection.SetActive(true);
                referencesObj.inventorySection.GetComponent<CanvasGroup>().alpha = 1;
                referencesObj.inventorySection.GetComponent<CanvasGroup>().interactable = true;
                referencesObj.inventorySection.GetComponent<CanvasGroup>().blocksRaycasts = true;
                break;
            case 1:
                pauseMenuS = PauseMenuSection.Quest;
                referencesObj.questSection.SetActive(true);
                referencesObj.questSection.GetComponent<CanvasGroup>().alpha = 1;
                referencesObj.questSection.GetComponent<CanvasGroup>().interactable = true;
                referencesObj.questSection.GetComponent<CanvasGroup>().blocksRaycasts = true;
                break;
            case 2:
                pauseMenuS = PauseMenuSection.Map;
                referencesObj.mapSection.SetActive(true);
                referencesObj.mapSection.GetComponent<CanvasGroup>().alpha = 1;
                referencesObj.mapSection.GetComponent<CanvasGroup>().interactable = true;
                referencesObj.mapSection.GetComponent<CanvasGroup>().blocksRaycasts = true;
                break;
            case 3:
                pauseMenuS = PauseMenuSection.Skills;
                referencesObj.skillsSection.SetActive(true);
                referencesObj.skillsSection.GetComponent<CanvasGroup>().alpha = 1;
                referencesObj.skillsSection.GetComponent<CanvasGroup>().interactable = true;
                referencesObj.skillsSection.GetComponent<CanvasGroup>().blocksRaycasts = true;
                break;
            case 4:
                pauseMenuS = PauseMenuSection.System;
                referencesObj.systemSection.SetActive(true);
                referencesObj.systemSection.GetComponent<CanvasGroup>().alpha = 1;
                referencesObj.systemSection.GetComponent<CanvasGroup>().interactable = true;
                referencesObj.systemSection.GetComponent<CanvasGroup>().blocksRaycasts = true;
                break;
        }
    }

    public void RefreshWeaponsInventory()
    {
        // Set Weapon ID
        if (preSwordEquipped)
            cc.preWeaponArmsID = 2;
        else if (preDaggerEquipped)
            cc.preWeaponArmsID = 2;
        else if (pre2HSwordEquipped)
            cc.preWeaponArmsID = 3;
        else if (preSpearEquipped)
            cc.preWeaponArmsID = 4;
        else if (preStaffEquipped)
            cc.preWeaponArmsID = 5;
        else
            cc.preWeaponArmsID = 0;

        // Deactivate
        if (wpnHolster.bowHP != null) wpnHolster.bowHP.SetActive(true);
        if (wpnHolster.bowEP != null) wpnHolster.bowEP.SetActive(false);
        if (wpnHolster.arrowHP != null) wpnHolster.arrowHP.SetActive(true);
        if (wpnHolster.arrowEP != null) wpnHolster.arrowEP.SetActive(false);
        if (wpnHolster.alteredPrimaryHP != null) wpnHolster.alteredPrimaryHP.SetActive(false);
        if (wpnHolster.alteredPrimaryEP != null) wpnHolster.alteredPrimaryEP.SetActive(false);

        // Set Active
        if (wpnHolster.swordEP != null)
        {
            wpnHolster.swordEP.SetActive(true);
            if (wpnHolster.shieldHP != null) wpnHolster.shieldHP.SetActive(false);
            if (wpnHolster.shieldP != null) wpnHolster.shieldP.SetActive(true);
        }
        else if (wpnHolster.dSwordEP != null)
        {
            wpnHolster.dSwordEP.SetActive(true);
            if (wpnHolster.shieldHP != null) wpnHolster.shieldHP.SetActive(true);
            if (wpnHolster.shieldP != null) wpnHolster.shieldP.SetActive(false);
        }
        else if (wpnHolster.spearEP != null)
        {
            wpnHolster.spearEP.SetActive(true);
            if (wpnHolster.shieldHP != null) wpnHolster.shieldHP.SetActive(true);
            if (wpnHolster.shieldP != null) wpnHolster.shieldP.SetActive(false);
        }
        else if (wpnHolster.staffEP != null)
        {
            wpnHolster.staffEP.SetActive(true);
            if (wpnHolster.shieldHP != null) wpnHolster.shieldHP.SetActive(true);
            if (wpnHolster.shieldP != null) wpnHolster.shieldP.SetActive(false);
        }
        else if (wpnHolster.hammerEP != null)
        {
            wpnHolster.hammerEP.SetActive(true);
            if (wpnHolster.shieldHP != null) wpnHolster.shieldHP.SetActive(true);
            if (wpnHolster.shieldP != null) wpnHolster.shieldP.SetActive(false);
        }
        else
        {
            if (wpnHolster.shieldHP != null) wpnHolster.shieldHP.SetActive(false);
            if (wpnHolster.shieldP != null) wpnHolster.shieldP.SetActive(true);
        }

        ItemOptionsIsActive(false);
        InventorySectionIsActive(false);
        referencesObj.weaponGrid.SetActive(true);
        inventoryS = InventorySection.Weapon;
        itemDescription.inventoryD = ItemDescription.InventoryDescription.Null;
    }

    public void RefreshBowAndArrowInventory()
    {
        // Set Weapon ID
        if (preBowEquipped)
            cc.preWeaponArmsID = 6;
        else if (preGunEquipped)
            cc.preWeaponArmsID = 7;
        else
            cc.preWeaponArmsID = 0;

        // Deactivate
        if (wpnHolster.swordHP != null) wpnHolster.swordHP.SetActive(true);
        if (wpnHolster.swordEP != null) wpnHolster.swordEP.SetActive(false);
        if (wpnHolster.dSwordHP != null) wpnHolster.dSwordHP.SetActive(true);
        if (wpnHolster.dSwordEP != null) wpnHolster.dSwordEP.SetActive(false);
        if (wpnHolster.hammerHP != null) wpnHolster.hammerHP.SetActive(true);
        if (wpnHolster.hammerEP != null) wpnHolster.hammerEP.SetActive(false);
        if (wpnHolster.spearHP != null) wpnHolster.spearHP.SetActive(true);
        if (wpnHolster.spearEP != null) wpnHolster.spearEP.SetActive(false);
        if (wpnHolster.staffHP != null) wpnHolster.staffHP.SetActive(true);
        if (wpnHolster.staffEP != null) wpnHolster.staffEP.SetActive(false);
        if (wpnHolster.shieldHP != null) wpnHolster.shieldHP.SetActive(true);
        if (wpnHolster.shieldP != null) wpnHolster.shieldP.SetActive(false);

        // SetActive
        if (wpnHolster.bowHP != null) wpnHolster.bowHP.SetActive(false);
        if (wpnHolster.bowEP != null) wpnHolster.bowEP.SetActive(true);
        if (wpnHolster.arrowHP != null) wpnHolster.arrowHP.SetActive(true);
        if (wpnHolster.arrowEP != null) wpnHolster.arrowEP.SetActive(true);

        ItemOptionsIsActive(false);
        InventorySectionIsActive(false);
        referencesObj.bowAndArrowGrid.SetActive(true);
        inventoryS = InventorySection.BowAndArrow;
        itemDescription.inventoryD = ItemDescription.InventoryDescription.Null;

        referencesObj.itemOptionsEquipObj.SetActive(false);
        referencesObj.itemOptionsRemoveObj.SetActive(false);
        referencesObj.itemOptionsConsumeObj.SetActive(false);
        referencesObj.itemOptionsPreviewObj.SetActive(false);
        referencesObj.itemOptionsSellObj.SetActive(false);
    }

    public void RefreshShieldInventory()
    {
        // Set Weapon ID
        if (preShieldEquipped)
            cc.preWeaponArmsID = 2;
        else if (preSwordEquipped)
            cc.preWeaponArmsID = 2;
        else if (preDaggerEquipped)
            cc.preWeaponArmsID = 2;
        else
            cc.preWeaponArmsID = 0;

        // Deactivate
        if (wpnHolster.dSwordHP != null) wpnHolster.dSwordHP.SetActive(true);
        if (wpnHolster.dSwordEP != null) wpnHolster.dSwordEP.SetActive(false);
        if (wpnHolster.hammerHP != null) wpnHolster.hammerHP.SetActive(true);
        if (wpnHolster.hammerEP != null) wpnHolster.hammerEP.SetActive(false);
        if (wpnHolster.spearHP != null) wpnHolster.spearHP.SetActive(true);
        if (wpnHolster.spearEP != null) wpnHolster.spearEP.SetActive(false);
        if (wpnHolster.staffHP != null) wpnHolster.staffHP.SetActive(true);
        if (wpnHolster.staffEP != null) wpnHolster.staffEP.SetActive(false);
        if (wpnHolster.shieldHP != null) wpnHolster.shieldHP.SetActive(true);
        if (wpnHolster.shieldP != null) wpnHolster.shieldP.SetActive(false);
        if (wpnHolster.bowHP != null) wpnHolster.bowHP.SetActive(true);
        if (wpnHolster.bowEP != null) wpnHolster.bowEP.SetActive(false);
        if (wpnHolster.arrowHP != null) wpnHolster.arrowHP.SetActive(true);
        if (wpnHolster.arrowEP != null) wpnHolster.arrowEP.SetActive(false);

        // SetActive
        if (wpnHolster.shieldHP != null) wpnHolster.shieldHP.SetActive(false);
        if (wpnHolster.shieldP != null) wpnHolster.shieldP.SetActive(true);
        if (wpnHolster.swordHP != null) wpnHolster.swordHP.SetActive(false);
        if (wpnHolster.swordEP != null) wpnHolster.swordEP.SetActive(true);

        ItemOptionsIsActive(false);
        InventorySectionIsActive(false);
        inventoryS = InventorySection.Shield;
        itemDescription.inventoryD = ItemDescription.InventoryDescription.Null;

        referencesObj.itemOptionsEquipObj.SetActive(false);
        referencesObj.itemOptionsRemoveObj.SetActive(false);
        referencesObj.itemOptionsConsumeObj.SetActive(false);
        referencesObj.itemOptionsPreviewObj.SetActive(false);
        referencesObj.itemOptionsSellObj.SetActive(false);
    }


    public void RefreshArmorInventory()
    {
        ItemOptionsIsActive(false);
        InventorySectionIsActive(false);
        inventoryS = InventorySection.Armor;
        itemDescription.inventoryD = ItemDescription.InventoryDescription.Null;

        referencesObj.itemOptionsEquipObj.SetActive(false);
        referencesObj.itemOptionsRemoveObj.SetActive(false);
        referencesObj.itemOptionsConsumeObj.SetActive(false);
        referencesObj.itemOptionsPreviewObj.SetActive(false);
        referencesObj.itemOptionsSellObj.SetActive(false);
    }

    public void RefreshMaterialInventory()
    {
        ItemOptionsIsActive(false);
        InventorySectionIsActive(false);
        inventoryS = InventorySection.Material;
        itemDescription.inventoryD = ItemDescription.InventoryDescription.Null;

        referencesObj.itemOptionsEquipObj.SetActive(false);
        referencesObj.itemOptionsRemoveObj.SetActive(false);
        referencesObj.itemOptionsConsumeObj.SetActive(false);
        referencesObj.itemOptionsPreviewObj.SetActive(false);
        referencesObj.itemOptionsSellObj.SetActive(false);
    }

    public void RefreshHealingInventory()
    {
        ItemOptionsIsActive(false);
        InventorySectionIsActive(false);
        inventoryS = InventorySection.Healing;
        itemDescription.inventoryD = ItemDescription.InventoryDescription.Null;

        referencesObj.itemOptionsEquipObj.SetActive(false);
        referencesObj.itemOptionsRemoveObj.SetActive(false);
        referencesObj.itemOptionsConsumeObj.SetActive(false);
        referencesObj.itemOptionsPreviewObj.SetActive(false);
        referencesObj.itemOptionsSellObj.SetActive(false);
    }

    public void RefreshKeyInventory()
    {
        ItemOptionsIsActive(false);
        InventorySectionIsActive(false);
        inventoryS = InventorySection.Key;
        itemDescription.inventoryD = ItemDescription.InventoryDescription.Null;

        referencesObj.itemOptionsEquipObj.SetActive(false);
        referencesObj.itemOptionsRemoveObj.SetActive(false);
        referencesObj.itemOptionsConsumeObj.SetActive(false);
        referencesObj.itemOptionsPreviewObj.SetActive(false);
        referencesObj.itemOptionsSellObj.SetActive(false);
    }

    #region Deactivate Weapons 

    public void DeactiveWeapons()
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
        preSwordEquipped = false;
        preDaggerEquipped = false;
        pre2HSwordEquipped = false;
        preSpearEquipped = false;
        preStaffEquipped = false;

        if (!wpnHolster.ShieldActive())
        {
            cc.weaponArmsID = 0;
            cc.preWeaponArmsID = 0;
        }
    }

    public void DeactiveBows()
    {
        DeactiveArrows();
        if (wpnHolster.secondaryH != null) wpnHolster.secondaryH.SetActive(false);
        if (wpnHolster.secondaryE != null) wpnHolster.secondaryE.SetActive(false);
        if (wpnHolster.secondaryD != null) wpnHolster.secondaryD.SetActive(false);
        if (wpnHolster.bowHP != null) wpnHolster.bowHP.SetActive(false);
        if (wpnHolster.bowEP != null) wpnHolster.bowEP.SetActive(false);
        if (wpnHolster.gunHP != null) wpnHolster.gunHP.SetActive(false);
        if (wpnHolster.gunEP != null) wpnHolster.gunEP.SetActive(false);
        if (wpnHolster.quiverHP != null) wpnHolster.quiverHP.SetActive(false);
        if (wpnHolster.quiverH != null) wpnHolster.quiverH.SetActive(false);
        if (wpnHolster.bowHP != null) wpnHolster.bowHP = null;
        if (wpnHolster.bowEP != null) wpnHolster.bowEP = null;
        if (wpnHolster.gunHP != null) wpnHolster.gunHP = null;
        if (wpnHolster.bowEP != null) wpnHolster.bowEP = null;
        if (wpnHolster.secondaryH != null) wpnHolster.secondaryH = null;
        if (wpnHolster.secondaryE != null) wpnHolster.secondaryE = null;
        if (wpnHolster.secondaryD != null) wpnHolster.secondaryD = null;

        preBowEquipped = false;

       cc.weaponArmsID = 0;
        cc.preWeaponArmsID = 0;
    }

    public void DeactiveShield()
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

        preShieldEquipped = false;

        if (!wpnHolster.PrimaryWeaponActive())
        {
            cc.weaponArmsID = 0;
            cc.preWeaponArmsID = 0;
        }
    }

    public void DeactiveArrows()
    {
        if (wpnHolster.arrowH != null) wpnHolster.arrowH.SetActive(false);
        if (wpnHolster.arrowE != null) wpnHolster.arrowE.SetActive(false);
        if (wpnHolster.arrowD != null) wpnHolster.arrowD.SetActive(false);
        if (wpnHolster.arrowString != null) wpnHolster.arrowString.SetActive(false);
        if (wpnHolster.arrowHP != null) wpnHolster.arrowHP.SetActive(false);
        if (wpnHolster.arrowEP != null) wpnHolster.arrowEP.SetActive(false);

        if (wpnHolster.arrowHP != null) wpnHolster.arrowHP = null;
        if (wpnHolster.arrowEP != null) wpnHolster.arrowEP = null;
        if (wpnHolster.arrowH != null) wpnHolster.arrowH = null;
        if (wpnHolster.arrowE != null) wpnHolster.arrowE = null;
        if (wpnHolster.arrowD != null) wpnHolster.arrowD = null;
        if (wpnHolster.arrowString != null) wpnHolster.arrowString = null;
    }

    public void DeactiveWeaponsHP()
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

    public void DeactiveBowsHP()
    {
        if (wpnHolster.bowHP != null) wpnHolster.bowHP.SetActive(false);
        if (wpnHolster.bowHP != null) wpnHolster.bowHP = null;
        if (wpnHolster.gunHP != null) wpnHolster.gunHP.SetActive(false);
        if (wpnHolster.gunHP != null) wpnHolster.gunHP = null;
    }

    public void DeactiveShieldHP()
    {
        if (wpnHolster.shieldHP != null) wpnHolster.shieldHP.SetActive(false);
        if (wpnHolster.shieldHP != null) wpnHolster.shieldHP = null;
    }

    public void DeactiveArrowsHP()
    {
        if (wpnHolster.arrowHP != null) wpnHolster.arrowHP.SetActive(false);
        if (wpnHolster.arrowHP != null) wpnHolster.arrowHP = null;
    }

    #endregion
}
