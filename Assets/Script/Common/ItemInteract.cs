using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using RPGAE.CharacterController;
using TMPro;

public class ItemInteract : MonoBehaviour
{
    [Header("ITEM SETTINGS")]
    public float pickUpDistance = 2.5f;
    public RaycastHit currentGroundInfo;

    [Header("SCENE TRANSITION SETTINGS")]
    public string sceneName;
    public int SpawnPointIDRef;

    [Header("QUEST SETTINGS")]
    [Tooltip("The name of the current quest you're progressing through.")]
    public string questSequenceNameReceipt;
    [Tooltip("The required phase number in order to progress to the next.")]
    public int questPhaseReceipt;
    [Tooltip("Once the required action is meet you will increment a phase.")]
    public int addQuestPhaseBy = 1;
    [Tooltip("Once the required action is meet you will increment a quantity amount.")]
    public int addQuestQuantityBy;

    [Header("REFERENCES")]
    public ParticleSystem itemOnGroundParticle;
    public InteractWorldSpaceUI interactUI;

    #region Private 

    [HideInInspector] public bool grounded;
    [HideInInspector] public Rigidbody rigidBody;
    [HideInInspector] public Collider[] colliders;
    [HideInInspector] public ItemData itemData;
    [HideInInspector] public ThirdPersonController cc;
    [HideInInspector] public InventoryManager inventoryM;
    [HideInInspector] public QuestManager questM;
    [HideInInspector] public ItemObtained itemObtained;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        cc = FindObjectOfType<ThirdPersonController>();
        inventoryM = FindObjectOfType<InventoryManager>();
        questM = FindObjectOfType<QuestManager>();
        itemData = GetComponent<ItemData>();
        rigidBody = GetComponent<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();

        if (interactUI == null)
        {
            interactUI = GetComponent<ItemData>().transform.parent.GetComponentInChildren<InteractWorldSpaceUI>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(itemData.itemActive);
        float dist = Vector3.Distance(transform.position, cc.transform.position);
        if(itemData.itemActive && dist < pickUpDistance)
        {
            if(itemData.GetComponent<HitBox>() == null)
            {
                GetItemBehaviour();
                if (interactUI != null)
                {
                    interactUI.ToggleIcon(true);
                }
            }
            else
            {
                if (!itemData.GetComponent<HitBox>().isProjectile)
                {
                    GetItemBehaviour();
                    if (interactUI != null)
                    {
                        interactUI.ToggleIcon(true);
                    }
                }
            }
        }
        else
        {
            if (interactUI != null)
            {
                interactUI.fadeUI.canvasGroup.alpha = 0; 
            }
        }
        //CheckGround(); // To Do: Add drop sound effects?
        ItemOnGroundParticle();
    }

    void ItemOnGroundParticle()
    {
        if (!inventoryM.isPauseMenuOn)
        {
            if (itemData.itemActive && !itemOnGroundParticle.isPlaying)
                itemOnGroundParticle.Play();
            else
                itemOnGroundParticle.Stop();
        }
        else
        {
            if (itemOnGroundParticle.isPlaying)
                itemOnGroundParticle.Stop();
        }
    }

    void GetItemBehaviour()
    {
        if (cc.cc.rpgaeIM.PlayerControls.Interact.triggered)
        {
            if (itemData.itemType == ItemData.ItemType.Weapon)
            {
                CheckInventoryItems(ref inventoryM.weaponInv.itemData, ref inventoryM.weaponInv.image);
            }
            else if (itemData.itemType == ItemData.ItemType.Bow || itemData.itemType == ItemData.ItemType.Arrow)
            {
                CheckInventoryItems(ref inventoryM.bowAndArrowInv.itemData, ref inventoryM.bowAndArrowInv.image);
            }
            else if (itemData.itemType == ItemData.ItemType.Shield)
            {
                CheckInventoryItems(ref inventoryM.shieldInv.itemData, ref inventoryM.shieldInv.image);
            }
            else if (itemData.itemType == ItemData.ItemType.Armor)
            {
                CheckInventoryItems(ref inventoryM.armorInv.itemData, ref inventoryM.armorInv.image);
            }
            else if (itemData.itemType == ItemData.ItemType.Material)
            {
                CheckInventoryItems(ref inventoryM.materialInv.itemData, ref inventoryM.materialInv.image);
            }
            else if (itemData.itemType == ItemData.ItemType.Healing)
            {
                CheckInventoryItems(ref inventoryM.healingInv.itemData, ref inventoryM.healingInv.image);
            }
            else if (itemData.itemType == ItemData.ItemType.Key)
            {
                CheckInventoryItems(ref inventoryM.keyInv.itemData, ref inventoryM.keyInv.image);
            }
            else if (itemData.itemType == ItemData.ItemType.General)
            {
                if (itemData.itemName == "Coin" || itemData.itemName == "Coin Bag")
                {
                    inventoryM.unityCoins += gameObject.GetComponent<ItemData>().quantity;
                    itemData.itemActive = false;
                }
            }

            #region Quest Event

            for (int i = 0; i < questM.mainQuestSlots.questData.Length; i++)
            {
                if (questM.mainQuestSlots.questData[i].questName == questSequenceNameReceipt
                && questM.mainQuestSlots.questData[i].questObjective[questM.mainQuestSlots.questData[i].questObjectiveNum].curCollectableAmount < questM.mainQuestSlots.questData[i].questObjective[questM.mainQuestSlots.questData[i].questObjectiveNum].maxCollectableAmount)
                {
                    questM.AddQuestObjective(ref questSequenceNameReceipt, ref questPhaseReceipt, ref addQuestPhaseBy, ref addQuestQuantityBy);
                }
            }

            for (int i = 0; i < questM.sideQuestSlots.questData.Length; i++)
            {
                if (questM.sideQuestSlots.questData[i].questName == questSequenceNameReceipt
                && questM.sideQuestSlots.questData[i].questObjective[questM.sideQuestSlots.questData[i].questObjectiveNum].curCollectableAmount < questM.sideQuestSlots.questData[i].questObjective[questM.sideQuestSlots.questData[i].questObjectiveNum].maxCollectableAmount)
                {
                    questM.AddQuestObjective(ref questSequenceNameReceipt, ref questPhaseReceipt, ref addQuestPhaseBy, ref addQuestQuantityBy);
                }
            }

            #endregion
        }
    }

    public void CheckInventoryItems(ref ItemData[] currentInventory, ref Image[] itemImage)
    {
        Debug.Log("CheckInventoryItems");
        if (cc.infoMessage.info.text == "You don't have enough inventory space.")
            return;

        if (!itemObtained)
            itemObtained = FindObjectOfType<ItemObtained>();

        GameObject itemDataConvert = itemData.gameObject;
        if (itemDataConvert.GetComponentInChildren<ItemData>().itemObtainedPickUp)
        {
            itemObtained.itemData = itemData;
            itemObtained.itemInteract = this;

            itemObtained.sceneName = sceneName;
            itemObtained.SpawnPointIDRef = SpawnPointIDRef;
        }
        else
        {
            inventoryM.AddItemDataSlot(ref currentInventory,
            ref itemImage, ref itemDataConvert);
        }
        itemData.itemActive = false;

        AddItemsIntoInventory();
    }

    void CheckGround()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.SphereCast(ray, 3, out currentGroundInfo, transform.position.magnitude, ~0, QueryTriggerInteraction.Ignore))
        {
            grounded = false;
        }
        else
        {
            grounded = true;
        }
    }

    void AddItemsIntoInventory()
    {
        //itemData.gameObject.SetActive(true);
        WeaponHolster wpnHolster = inventoryM.wpnHolster;
        itemData.itemActive = false;
        Debug.Log("AddItemsIntoInventory: " + itemData.itemType);
        switch (itemData.itemType)
        {
            case ItemData.ItemType.Weapon:
                for (int i = 0; i < inventoryM.weaponInv.itemData.Length; i++)
                {
                    if (itemData.itemName == "Windscar" + itemData.rankTag)
                    {
                        wpnHolster.windScarNoParticlesH.SetActive(true);
                        wpnHolster.windScarNoParticlesEP.SetActive(true);
                        wpnHolster.swordHP = wpnHolster.windScarNoParticlesHP;
                        wpnHolster.swordEP = wpnHolster.windScarNoParticlesEP;
                        wpnHolster.primaryEP = wpnHolster.windScarNoParticlesEP;
                        wpnHolster.primaryH = wpnHolster.windScarNoParticlesH;
                        wpnHolster.primaryE = wpnHolster.windScarNoParticleE;
                        wpnHolster.primaryD = wpnHolster.windScarNoParticlePrefab;
                        wpnHolster.alteredPrimaryE = wpnHolster.windScarE;
                        wpnHolster.SetItemData(ref wpnHolster.primaryE, ref itemData);
                        inventoryM.preSwordEquipped = true;
                    }

                    if (itemData.itemName == "The Tuning fork" + itemData.rankTag)
                    {
                        wpnHolster.theTuningForkH.SetActive(true);
                        wpnHolster.theTuningForkEP.SetActive(true);
                        wpnHolster.dSwordHP = wpnHolster.theTuningForkHP;
                        wpnHolster.dSwordEP = wpnHolster.theTuningForkEP;
                        wpnHolster.primaryEP = wpnHolster.theTuningForkEP;
                        wpnHolster.primaryH = wpnHolster.theTuningForkH;
                        wpnHolster.primaryE = wpnHolster.theTuningForkE;
                        wpnHolster.primaryD = wpnHolster.theTuningForkPrefab;
                        wpnHolster.SetItemData(ref wpnHolster.primaryE, ref itemData);
                        inventoryM.pre2HSwordEquipped = true;
                    }

                    if (itemData.itemName == "Assassin Dagger" + itemData.rankTag)
                    {
                        wpnHolster.assasinsDaggerH.SetActive(true);
                        wpnHolster.assasinsDaggerEP.SetActive(true);
                        wpnHolster.swordHP = wpnHolster.assasinsDaggerHP;
                        wpnHolster.swordEP = wpnHolster.assasinsDaggerEP;
                        wpnHolster.primaryEP = wpnHolster.assasinsDaggerEP;
                        wpnHolster.primaryH = wpnHolster.assasinsDaggerH;
                        wpnHolster.primaryE = wpnHolster.assasinsDaggerE;
                        wpnHolster.primaryD = wpnHolster.assasinsDaggerPrefab;
                        wpnHolster.SetItemData(ref wpnHolster.primaryE, ref itemData);
                        inventoryM.preSwordEquipped = true;
                    }

                    if (itemData.itemName == "Cleric's Staff" + itemData.rankTag)
                    {
                        wpnHolster.clericsStaffH.SetActive(true);
                        wpnHolster.clericsStaffEP.SetActive(true);
                        wpnHolster.staffHP = wpnHolster.clericsStaffHP;
                        wpnHolster.staffEP = wpnHolster.clericsStaffEP;
                        wpnHolster.primaryH = wpnHolster.clericsStaffH;
                        wpnHolster.primaryE = wpnHolster.clericsStaffE;
                        wpnHolster.primaryD = wpnHolster.clericsStaffPrefab;
                        wpnHolster.SetItemData(ref wpnHolster.primaryE, ref itemData);
                        inventoryM.preStaffEquipped = true;
                    }

                    if (itemData.itemName == "Glaive" + itemData.rankTag)
                    {
                        wpnHolster.glaiveH.SetActive(true);
                        wpnHolster.glaiveEP.SetActive(true);
                        wpnHolster.spearHP = wpnHolster.glaiveHP;
                        wpnHolster.spearEP = wpnHolster.glaiveEP;
                        wpnHolster.primaryH = wpnHolster.glaiveH;
                        wpnHolster.primaryE = wpnHolster.glaiveE;
                        wpnHolster.primaryD = wpnHolster.glaivePrefab;
                        wpnHolster.SetItemData(ref wpnHolster.primaryE, ref itemData);
                        inventoryM.preSpearEquipped = true;
                    }

                    if (itemData.itemName == "Obsidian Fury" + itemData.rankTag)
                    {
                        wpnHolster.obsidianFuryH.SetActive(true);
                        wpnHolster.obsidianFuryEP.SetActive(true);
                        wpnHolster.hammerHP = wpnHolster.obsidianFuryHP;
                        wpnHolster.hammerEP = wpnHolster.obsidianFuryEP;
                        wpnHolster.primaryH = wpnHolster.obsidianFuryH;
                        wpnHolster.primaryE = wpnHolster.obsidianFuryE;
                        wpnHolster.primaryD = wpnHolster.obsidianFuryPrefab;
                        wpnHolster.SetItemData(ref wpnHolster.primaryE, ref itemData);
                        inventoryM.pre2HSwordEquipped = true;
                    }

                    #region Outline

                    inventoryM.weaponInv.slotWeaponEquipped = i;
                    inventoryM.weaponInv.itemData[inventoryM.weaponInv.slotWeaponEquipped].equipped = true;
                    inventoryM.weaponInv.outLineBorder[inventoryM.weaponInv.slotWeaponEquipped].enabled = true;
                    inventoryM.weaponInv.outLineBorder[inventoryM.weaponInv.slotWeaponEquipped].color = inventoryM.outLineSelected;

                    #endregion

                    #region Equipped Icon

                    //inventoryM.referencesObj.primaryItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = inventoryM.weaponInv.itemData[i].numOfEngraving;
                    //inventoryM.referencesObj.primaryItemImage.enabled = true;
                    //inventoryM.referencesObj.primaryItemImage.sprite = inventoryM.weaponInv.itemData[i].itemSprite;
                    //inventoryM.referencesObj.primaryValueBG.enabled = true;
                    //inventoryM.referencesObj.primaryValueBG.GetComponentInChildren<TextMeshProUGUI>().text = inventoryM.weaponInv.itemData[i].maxWpnAtk.ToString();

                    #endregion
                }

                if (inventoryM.preSwordEquipped)
                    cc.preWeaponArmsID = 1;
                else if (inventoryM.preDaggerEquipped)
                    cc.preWeaponArmsID = 2;
                else if (inventoryM.pre2HSwordEquipped)
                    cc.preWeaponArmsID = 3;
                else if (inventoryM.preSpearEquipped)
                    cc.preWeaponArmsID = 4;
                else if (inventoryM.preStaffEquipped)
                    cc.preWeaponArmsID = 5;

                //oneShot = true;

                break;
            case ItemData.ItemType.Bow:

                //inventoryM.AddItemDataSlot(ref inventoryM.bowAndArrowInv.itemData,
                //ref inventoryM.bowAndArrowInv.image, ref itemDataPrefab);

                for (int i = 0; i < inventoryM.bowAndArrowInv.itemData.Length; i++)
                {
                    if (itemData.itemName == "Warbow" + itemData.rankTag)
                    {
                        inventoryM.DeactiveBows();
                        inventoryM.DeactiveBowsHP();
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
                        wpnHolster.SetItemData(ref wpnHolster.secondaryE, ref itemData);
                        inventoryM.preBowEquipped = true;
                    }

                    if (inventoryM.bowAndArrowInv.itemData[i].itemType == ItemData.ItemType.Bow)
                    {
                        #region Outline

                        inventoryM.bowAndArrowInv.slotBowEquipped = i;
                        inventoryM.bowAndArrowInv.itemData[inventoryM.bowAndArrowInv.slotBowEquipped].equipped = true;
                        inventoryM.bowAndArrowInv.outLineBorder[inventoryM.bowAndArrowInv.slotBowEquipped].enabled = true;
                        inventoryM.bowAndArrowInv.outLineBorder[inventoryM.bowAndArrowInv.slotBowEquipped].color = inventoryM.outLineSelected;

                        #endregion

                        #region Equipped Icon

                        //inventoryM.referencesObj.secondaryItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                        //inventoryM.referencesObj.secondaryItemImage.enabled = true;
                        //inventoryM.referencesObj.secondaryItemImage.sprite = itemData.itemSprite;
                        //inventoryM.referencesObj.secondaryValueBG.enabled = true;
                        //inventoryM.referencesObj.secondaryValueBG.GetComponentInChildren<TextMeshProUGUI>().text = itemData.maxBowAtk.ToString();

                        #endregion
                    }

                    if (inventoryM.bowAndArrowInv.itemData[i].itemType == ItemData.ItemType.Arrow)
                    {
                        #region Outline

                        inventoryM.bowAndArrowInv.slotArrowEquipped = i;
                        inventoryM.bowAndArrowInv.itemData[inventoryM.bowAndArrowInv.slotArrowEquipped].equipped = true;
                        inventoryM.bowAndArrowInv.outLineBorder[inventoryM.bowAndArrowInv.slotArrowEquipped].enabled = true;
                        inventoryM.bowAndArrowInv.outLineBorder[inventoryM.bowAndArrowInv.slotArrowEquipped].color = inventoryM.outLineSelected;

                        #endregion

                        #region Equipped Icon

                        inventoryM.referencesObj.arrowItemImage.enabled = true;
                        inventoryM.referencesObj.arrowItemImage.sprite = itemData.itemSprite;
                        inventoryM.referencesObj.arrowValueBG.enabled = true;
                        inventoryM.referencesObj.arrowValueBG.GetComponentInChildren<TextMeshProUGUI>().text = inventoryM.bowAndArrowInv.counter[inventoryM.bowAndArrowInv.slotArrowEquipped].ToString();

                        #endregion
                    }
                }


                //oneShot = true;
                break;
            case ItemData.ItemType.Arrow:

                for (int i = 0; i < inventoryM.bowAndArrowInv.itemData.Length; i++)
                {
                    if (itemData.itemName == "Common Arrow")
                    {
                        inventoryM.DeactiveArrows();
                        inventoryM.DeactiveArrowsHP();
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
                        wpnHolster.SetItemData(ref wpnHolster.arrowE, ref itemData);
                        wpnHolster.quiverArrows = wpnHolster.commonArrowH.GetComponentsInChildren<QuiverArrows>();
                        wpnHolster.SetActiveQuiverArrows();
                    }

                    if (itemData.itemName == "Particle Arrow")
                    {
                        inventoryM.DeactiveArrows();
                        inventoryM.DeactiveArrowsHP();
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
                        wpnHolster.SetItemData(ref wpnHolster.arrowE, ref itemData);
                        wpnHolster.quiverArrows = wpnHolster.particleArrowH.GetComponentsInChildren<QuiverArrows>();
                        wpnHolster.SetActiveQuiverArrows();
                    }

                    if (inventoryM.bowAndArrowInv.itemData[i].itemType == ItemData.ItemType.Bow)
                    {
                        #region Outline

                        inventoryM.bowAndArrowInv.slotBowEquipped = i;
                        inventoryM.bowAndArrowInv.itemData[inventoryM.bowAndArrowInv.slotBowEquipped].equipped = true;
                        inventoryM.bowAndArrowInv.outLineBorder[inventoryM.bowAndArrowInv.slotBowEquipped].enabled = true;
                        inventoryM.bowAndArrowInv.outLineBorder[inventoryM.bowAndArrowInv.slotBowEquipped].color = inventoryM.outLineSelected;

                        #endregion
                    }

                    if (inventoryM.bowAndArrowInv.itemData[i].itemType == ItemData.ItemType.Arrow)
                    {
                        #region Outline

                        inventoryM.bowAndArrowInv.slotArrowEquipped = i;
                        inventoryM.bowAndArrowInv.itemData[inventoryM.bowAndArrowInv.slotArrowEquipped].equipped = true;
                        inventoryM.bowAndArrowInv.outLineBorder[inventoryM.bowAndArrowInv.slotArrowEquipped].enabled = true;
                        inventoryM.bowAndArrowInv.outLineBorder[inventoryM.bowAndArrowInv.slotArrowEquipped].color = inventoryM.outLineSelected;

                        #endregion

                        #region Equipped Icon

                        inventoryM.referencesObj.arrowItemImage.enabled = true;
                        inventoryM.referencesObj.arrowItemImage.sprite = itemData.itemSprite;
                        inventoryM.referencesObj.arrowValueBG.enabled = true;
                        inventoryM.referencesObj.arrowValueBG.GetComponentInChildren<TextMeshProUGUI>().text = inventoryM.bowAndArrowInv.counter[inventoryM.bowAndArrowInv.slotArrowEquipped].ToString();

                        #endregion
                    }
                }

                if (inventoryM.preBowEquipped)
                    cc.preWeaponArmsID = 6;
                else if (inventoryM.preGunEquipped)
                    cc.preWeaponArmsID = 7;
                else
                    cc.preWeaponArmsID = 0;

                //oneShot = true;
                break;
            case ItemData.ItemType.Shield:

                //inventoryM.AddItemDataSlot(ref inventoryM.shieldInv.itemData,
                //ref inventoryM.shieldInv.image, ref itemDataPrefab);

                for (int i = 0; i < inventoryM.shieldInv.itemData.Length; i++)
                {
                    if (!inventoryM.shieldInv.itemData[i].equipped)
                    {
                        inventoryM.shieldInv.slotShieldEquipped = i;
                        inventoryM.shieldInv.itemData[inventoryM.shieldInv.slotShieldEquipped].equipped = true;
                        inventoryM.shieldInv.highLight[inventoryM.shieldInv.slotShieldEquipped].color = inventoryM.outLineSelected;
                    }

                    if (itemData.itemName == "Circle Shield" + itemData.rankTag)
                    {
                        inventoryM.DeactiveShield();
                        inventoryM.DeactiveShieldHP();
                        wpnHolster.circleShieldH.SetActive(true);
                        wpnHolster.circleShieldEP.SetActive(true);
                        wpnHolster.shieldHP = wpnHolster.circleShieldHP;
                        wpnHolster.shieldP = wpnHolster.circleShieldEP;
                        wpnHolster.shieldH = wpnHolster.circleShieldH;
                        wpnHolster.shieldE = wpnHolster.circleShieldE;
                        wpnHolster.shieldD = wpnHolster.circleShieldPrefab;
                        wpnHolster.SetItemData(ref wpnHolster.shieldE, ref itemData);
                        inventoryM.preShieldEquipped = true;
                    }

                    #region Outline

                    inventoryM.shieldInv.slotShieldEquipped = i;
                    inventoryM.shieldInv.itemData[inventoryM.shieldInv.slotShieldEquipped].equipped = true;
                    inventoryM.shieldInv.outLineBorder[inventoryM.shieldInv.slotShieldEquipped].enabled = true;
                    inventoryM.shieldInv.outLineBorder[inventoryM.shieldInv.slotShieldEquipped].color = inventoryM.outLineSelected;

                    #endregion

                    #region Equipped Icon

                    inventoryM.referencesObj.shieldItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                    inventoryM.referencesObj.shieldItemImage.enabled = true;
                    inventoryM.referencesObj.shieldItemImage.sprite = itemData.itemSprite;
                    inventoryM.referencesObj.shieldValueBG.enabled = true;
                    inventoryM.referencesObj.shieldValueBG.GetComponentInChildren<TextMeshProUGUI>().text = itemData.maxShdAtk.ToString();

                    #endregion
                }

                if (inventoryM.preShieldEquipped)
                    cc.preWeaponArmsID = 2;
                else if (inventoryM.preSwordEquipped)
                    cc.preWeaponArmsID = 2;
                else if (inventoryM.preDaggerEquipped)
                    cc.preWeaponArmsID = 2;
                else
                    cc.preWeaponArmsID = 0;

                //oneShot = true;
                break;
            case ItemData.ItemType.Armor:

                //inventoryM.AddItemDataSlot(ref inventoryM.armorInv.itemData,
                //ref inventoryM.armorInv.image, ref itemDataPrefab);

                for (int i = 0; i < inventoryM.armorInv.itemData.Length; i++)
                {
                    if (itemData.arm.armorType == ItemData.ArmorType.Head)
                    {
                        #region Outline

                        inventoryM.armorInv.slotArmorHeadEquipped = i;
                        inventoryM.armorInv.itemData[inventoryM.armorInv.slotArmorHeadEquipped].equipped = true;
                        inventoryM.armorInv.outLineBorder[inventoryM.armorInv.slotArmorHeadEquipped].enabled = true;
                        inventoryM.armorInv.outLineBorder[inventoryM.armorInv.slotArmorHeadEquipped].color = inventoryM.outLineSelected;

                        #endregion

                        #region Equipped Icon

                        inventoryM.referencesObj.headItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                        inventoryM.referencesObj.headItemImage.enabled = true;
                        inventoryM.referencesObj.headItemImage.sprite = itemData.itemSprite;
                        inventoryM.referencesObj.headValueBG.enabled = true;
                        inventoryM.referencesObj.headValueBG.GetComponentInChildren<TextMeshProUGUI>().text = itemData.maxWpnAtk.ToString();

                        #endregion
                    }
                    if (itemData.arm.armorType == ItemData.ArmorType.Chest)
                    {
                        #region Outline

                        inventoryM.armorInv.slotArmorChestEquipped = i;
                        inventoryM.armorInv.itemData[inventoryM.armorInv.slotArmorChestEquipped].equipped = true;
                        inventoryM.armorInv.outLineBorder[inventoryM.armorInv.slotArmorChestEquipped].enabled = true;
                        inventoryM.armorInv.outLineBorder[inventoryM.armorInv.slotArmorChestEquipped].color = inventoryM.outLineSelected;

                        #endregion

                        #region Equipped Icon

                        inventoryM.referencesObj.chestItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                        inventoryM.referencesObj.chestItemImage.enabled = true;
                        inventoryM.referencesObj.chestItemImage.sprite = itemData.itemSprite;
                        inventoryM.referencesObj.chestValueBG.enabled = true;
                        inventoryM.referencesObj.chestValueBG.GetComponentInChildren<TextMeshProUGUI>().text = itemData.maxWpnAtk.ToString();

                        #endregion
                    }

                    if (itemData.arm.armorType == ItemData.ArmorType.Legs)
                    {
                        #region Outline

                        inventoryM.armorInv.slotArmorLegEquipped = i;
                        inventoryM.armorInv.itemData[inventoryM.armorInv.slotArmorLegEquipped].equipped = true;
                        inventoryM.armorInv.outLineBorder[inventoryM.armorInv.slotArmorLegEquipped].enabled = true;
                        inventoryM.armorInv.outLineBorder[inventoryM.armorInv.slotArmorLegEquipped].color = inventoryM.outLineSelected;

                        #endregion

                        #region Equipped Icon

                        inventoryM.referencesObj.legItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                        inventoryM.referencesObj.legItemImage.enabled = true;
                        inventoryM.referencesObj.legItemImage.sprite = itemData.itemSprite;
                        inventoryM.referencesObj.legValueBG.enabled = true;
                        inventoryM.referencesObj.legValueBG.GetComponentInChildren<TextMeshProUGUI>().text = itemData.arm.ToString();

                        #endregion
                    }

                    if (itemData.arm.armorType == ItemData.ArmorType.Amulet)
                    {
                        #region Outline

                        inventoryM.armorInv.slotArmorAmuletEquipped = i;
                        inventoryM.armorInv.itemData[inventoryM.armorInv.slotArmorAmuletEquipped].equipped = true;
                        inventoryM.armorInv.outLineBorder[inventoryM.armorInv.slotArmorAmuletEquipped].enabled = true;
                        inventoryM.armorInv.outLineBorder[inventoryM.armorInv.slotArmorAmuletEquipped].color = inventoryM.outLineSelected;

                        #endregion

                        #region Equipped Icon

                        inventoryM.referencesObj.amuletItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                        inventoryM.referencesObj.amuletItemImage.enabled = true;
                        inventoryM.referencesObj.amuletItemImage.sprite = itemData.itemSprite;
                        inventoryM.referencesObj.amuletValueBG.enabled = true;
                        inventoryM.referencesObj.amuletValueBG.GetComponentInChildren<TextMeshProUGUI>().text = itemData.arm.ToString();

                        #endregion
                    }
                }

                //oneShot = true;
                break;
            case ItemData.ItemType.Material:

                //inventoryM.AddItemDataSlot(ref inventoryM.materialInv.itemData,
                //ref inventoryM.materialInv.image, ref itemDataPrefab);

                //oneShot = true;
                break;
            case ItemData.ItemType.Healing:

                //inventoryM.AddItemDataSlot(ref inventoryM.healingInv.itemData,
                //ref inventoryM.healingInv.image, ref itemDataPrefab);

                //oneShot = true;
                break;
            case ItemData.ItemType.Key:

                //inventoryM.AddItemDataSlot(ref inventoryM.keyInv.itemData,
                //ref inventoryM.keyInv.image, ref itemDataPrefab);

                //oneShot = true;
                break;
        }

        cc.SetActiveWeaponInHand();
    }
}
