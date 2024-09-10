using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using RPGAE.CharacterController;

public class ItemOptionsEquip : MonoBehaviour
{
    public InputFieldOptions inputFieldOptions;
    readonly List<string> names = new List<string>() {"Choose:", "Equip", "Drop", "Cancel"};

    [Header("AUDIO")]
    public RandomAudioPlayer equippedAS;
    public RandomAudioPlayer cancelAS;

    #region Private 

    public bool onButton;
    private TMP_Dropdown dropDown;
    private TextMeshProUGUI selectedName;
    private Transform dropTarget;
    private GameObject dropDownObj;
    private InfoMessage infoMessage;
    private ThirdPersonController cc;
    private InventoryManager inventoryM;
    public WeaponHolster wpnHolster;

    #endregion

    // Use this for initialization
    void Awake ()
    {
        selectedName = GameObject.Find("EquipLabel").GetComponent<TextMeshProUGUI>();
        dropTarget = GameObject.Find("Character").transform;

        // Get Componets.
        inputFieldOptions = FindObjectOfType<InputFieldOptions>();
        wpnHolster = GameObject.Find("PlayerHolster").GetComponent<WeaponHolster>();
        dropDown = GetComponent<TMP_Dropdown>();
        infoMessage = FindObjectOfType<InfoMessage>();
        cc = FindObjectOfType<ThirdPersonController>();
        inventoryM = FindObjectOfType<InventoryManager>();
        PopulateList ();
    }

    void Update()
    {
        dropDownObj = GameObject.Find("Dropdown List");
    }

    public void DropDownSelect(int index)
    {
        selectedName.text = names[index];

        if (index == 1)
        {
            if (inventoryM.inventoryS == InventoryManager.InventorySection.Weapon)
            {
                int slotNum = inventoryM.weaponInv.slotNum;
                inventoryM.weaponInv.slotWeaponEquipped = slotNum;
                wpnHolster.slotWeapon = slotNum;

                ItemData itemData = inventoryM.weaponInv.itemData[inventoryM.weaponInv.slotNum];

                #region Check If Broken

                if (itemData.broken && inventoryM.itemRepair)
                {
                    infoMessage.info.text = "You cannot equip a broken weapon.";
                    dropDown.value = 0;
                    DestroyImmediate(dropDownObj);
                    gameObject.SetActive(false);
                    return;
                }

                #endregion

                #region Set as equippped

                for (int i = 0; i < inventoryM.weaponInv.itemData.Length; i++)
                {
                    itemData.equipped = false;
                }
                itemData.equipped = true;

                #endregion

                #region Outline

                foreach (Image outline in inventoryM.weaponInv.outLineBorder)
                {
                    outline.enabled = false;
                    outline.color = inventoryM.outLineHover;
                }
                Image outLine = inventoryM.weaponInv.outLineBorder[inventoryM.weaponInv.slotWeaponEquipped];
                outLine.enabled = true;
                outLine.color = inventoryM.outLineSelected;

                #endregion

                #region Set button navigation to auto

                Image itemHL = inventoryM.weaponInv.highLight[inventoryM.weaponInv.slotNum];
                Navigation auto = new Navigation { mode = Navigation.Mode.Automatic };
                itemHL.GetComponentInChildren<Button>().navigation = auto;

                #endregion

                #region Equipped Icon

                inventoryM.referencesObj.primaryItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                inventoryM.referencesObj.primaryItemImage.enabled = true;
                inventoryM.referencesObj.primaryItemImage.sprite = itemData.itemSprite;
                inventoryM.referencesObj.primaryValueBG.enabled = true;
                inventoryM.referencesObj.primaryValueBG.GetComponentInChildren<TextMeshProUGUI>().text = itemData.maxWpnAtk.ToString();

                #endregion

                if (itemData.itemName == "Windscar" + itemData.rankTag)
                {
                    DeactiveWeapons();
                    DeactiveWeaponsHP();
                    wpnHolster.windScarNoParticlesH.SetActive(true);
                    wpnHolster.windScarNoParticlesEP.SetActive(true);
                    wpnHolster.swordHP = wpnHolster.windScarNoParticlesHP;
                    wpnHolster.swordEP = wpnHolster.windScarNoParticlesEP;
                    wpnHolster.primaryH = wpnHolster.windScarNoParticlesH;
                    wpnHolster.primaryE = wpnHolster.windScarNoParticleE;
                    wpnHolster.primaryD = wpnHolster.windScarNoParticlePrefab;
                    wpnHolster.alteredPrimaryE = wpnHolster.windScarE;
                    wpnHolster.alteredPrimaryEP = wpnHolster.windScarEP;
                    wpnHolster.alteredPrimaryHP = wpnHolster.windScarHP;
                    wpnHolster.SetItemData(ref wpnHolster.primaryE, ref itemData);
                    inventoryM.preSwordEquipped = true;
                }

                if (itemData.itemName == "The Tuning fork" + itemData.rankTag)
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
                    wpnHolster.SetItemData(ref wpnHolster.primaryE, ref itemData);
                    inventoryM.pre2HSwordEquipped = true;
                }

                if (itemData.itemName == "Assassin Dagger" + itemData.rankTag)
                {
                    DeactiveWeapons();
                    DeactiveWeaponsHP();
                    wpnHolster.assasinsDaggerH.SetActive(true);
                    wpnHolster.assasinsDaggerEP.SetActive(true);
                    wpnHolster.swordHP = wpnHolster.assasinsDaggerHP;
                    wpnHolster.swordEP = wpnHolster.assasinsDaggerEP;
                    wpnHolster.primaryH = wpnHolster.assasinsDaggerH;
                    wpnHolster.primaryE = wpnHolster.assasinsDaggerE;
                    wpnHolster.primaryD = wpnHolster.assasinsDaggerPrefab;
                    wpnHolster.SetItemData(ref wpnHolster.primaryE, ref itemData);
                    inventoryM.preDaggerEquipped = true;
                }

                if (itemData.itemName == "Cleric's Staff" + itemData.rankTag)
                {
                    DeactiveWeapons();
                    DeactiveWeaponsHP();
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
                    wpnHolster.SetItemData(ref wpnHolster.primaryE, ref itemData);
                    inventoryM.preSpearEquipped = true;
                }

                if (itemData.itemName == "Obsidian Fury" + itemData.rankTag)
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
                    wpnHolster.SetItemData(ref wpnHolster.primaryE, ref itemData);
                    inventoryM.pre2HSwordEquipped = true;
                }

                inventoryM.RefreshWeaponsInventory();
            }

            if (inventoryM.inventoryS == InventoryManager.InventorySection.BowAndArrow)
            {
                ItemData itemData = inventoryM.bowAndArrowInv.itemData[inventoryM.bowAndArrowInv.slotNum];

                if (itemData.itemType == ItemData.ItemType.Bow)
                {
                    int slotNum = inventoryM.bowAndArrowInv.slotNum;
                    inventoryM.bowAndArrowInv.slotBowEquipped = slotNum;

                    #region Check If Broken

                    if (itemData.broken && inventoryM.itemRepair)
                    {
                        infoMessage.info.text = "You cannot equip a broken weapon.";
                        dropDown.value = 0;
                        DestroyImmediate(dropDownObj);
                        gameObject.SetActive(false);
                        return;
                    }

                    #endregion

                    #region Set as equippped

                    for (int i = 0; i < inventoryM.bowAndArrowInv.itemData.Length; i++)
                    {
                        if (inventoryM.bowAndArrowInv.itemData[i].itemType == ItemData.ItemType.Bow)
                        {
                            itemData.equipped = false;
                        }
                    }
                    itemData.equipped = true;

                    #endregion

                    #region Outline

                    for (int i = 0; i < inventoryM.bowAndArrowInv.itemData.Length; i++)
                    {
                        if (inventoryM.bowAndArrowInv.itemData[i].itemType == ItemData.ItemType.Bow)
                        {
                            inventoryM.bowAndArrowInv.outLineBorder[i].enabled = false;
                            inventoryM.bowAndArrowInv.outLineBorder[i].color = inventoryM.outLineHover;
                        }
                    }
                    Image outLine = inventoryM.bowAndArrowInv.outLineBorder[inventoryM.bowAndArrowInv.slotBowEquipped];
                    outLine.enabled = true;
                    outLine.color = inventoryM.outLineSelected;

                    #endregion

                    #region Equipped Icon

                    inventoryM.referencesObj.secondaryItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                    inventoryM.referencesObj.secondaryItemImage.enabled = true;
                    inventoryM.referencesObj.secondaryItemImage.sprite = itemData.itemSprite;
                    inventoryM.referencesObj.secondaryValueBG.enabled = true;
                    inventoryM.referencesObj.secondaryValueBG.GetComponentInChildren<TextMeshProUGUI>().text = itemData.maxBowAtk.ToString();

                    #endregion

                    #region Set button navigation to auto

                    Image itemHL = inventoryM.bowAndArrowInv.highLight[inventoryM.bowAndArrowInv.slotNum];
                    Navigation auto = new Navigation { mode = Navigation.Mode.Automatic };
                    itemHL.GetComponentInChildren<Button>().navigation = auto;

                    #endregion

                    if (itemData.itemName == "Warbow" + itemData.rankTag)
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
                        wpnHolster.SetItemData(ref wpnHolster.secondaryE, ref itemData);
                        inventoryM.preBowEquipped = true;
                    }

                    if (itemData.itemName == "Lu Rifle" + itemData.rankTag)
                    {
                        DeactiveBows();
                        DeactiveBowsHP();
                        wpnHolster.luRifleH.SetActive(true);
                        wpnHolster.luRifleEP.SetActive(true);
                        wpnHolster.quiverHP.SetActive(false);
                        wpnHolster.quiverH.SetActive(false);
                        wpnHolster.bowHP = wpnHolster.luRifleHP;
                        wpnHolster.bowEP = wpnHolster.luRifleEP;
                        wpnHolster.arrowPrefabSpot = wpnHolster.luRiflePrefabSpot;
                        wpnHolster.secondaryH = wpnHolster.luRifleH;
                        wpnHolster.secondaryE = wpnHolster.luRifleE;
                        wpnHolster.secondaryD = wpnHolster.warbowPrefab;
                        wpnHolster.SetItemData(ref wpnHolster.secondaryE, ref itemData);
                        inventoryM.preGunEquipped = true;
                    }
                }

                if (itemData.itemType == ItemData.ItemType.Arrow)
                {
                    if (wpnHolster.bowEP == null)
                    {
                        infoMessage.info.text = "You must equip a bow before you can use arrows.";
                        dropDown.value = 0;
                        DestroyImmediate(dropDownObj);
                        gameObject.SetActive(false);
                        return;
                    }

                    int slotNum = inventoryM.bowAndArrowInv.slotNum;
                    inventoryM.bowAndArrowInv.slotArrowEquipped = slotNum;

                    #region Set as equippped

                    for (int i = 0; i < inventoryM.bowAndArrowInv.itemData.Length; i++)
                    {
                        if (inventoryM.bowAndArrowInv.itemData[i].itemType == ItemData.ItemType.Arrow)
                        {
                            itemData.equipped = false;
                        }
                    }
                    itemData.equipped = true;

                    #endregion

                    #region Outline

                    for (int i = 0; i < inventoryM.bowAndArrowInv.itemData.Length; i++)
                    {
                        if (inventoryM.bowAndArrowInv.itemData[i].itemType == ItemData.ItemType.Arrow)
                        {
                            inventoryM.bowAndArrowInv.outLineBorder[i].enabled = false;
                            inventoryM.bowAndArrowInv.outLineBorder[i].color = inventoryM.outLineHover;
                        }
                    }
                    Image outLine = inventoryM.bowAndArrowInv.outLineBorder[inventoryM.bowAndArrowInv.slotArrowEquipped];
                    outLine.enabled = true;
                    outLine.color = inventoryM.outLineSelected;

                    #endregion

                    #region Equipped Icon

                    inventoryM.referencesObj.arrowItemImage.enabled = true;
                    inventoryM.referencesObj.arrowItemImage.sprite = itemData.itemSprite;
                    inventoryM.referencesObj.arrowValueBG.enabled = true;
                    inventoryM.referencesObj.arrowValueBG.GetComponentInChildren<TextMeshProUGUI>().text = inventoryM.bowAndArrowInv.counter[inventoryM.bowAndArrowInv.slotArrowEquipped].ToString();

                    #endregion

                    #region Set button navigation to auto

                    Image itemHL = inventoryM.bowAndArrowInv.highLight[inventoryM.bowAndArrowInv.slotNum];
                    Navigation auto = new Navigation { mode = Navigation.Mode.Automatic };
                    itemHL.GetComponentInChildren<Button>().navigation = auto;

                    #endregion

                    if (itemData.itemName == "Common Arrow")
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
                        wpnHolster.SetItemData(ref wpnHolster.arrowE, ref itemData);
                        wpnHolster.quiverArrows = wpnHolster.commonArrowH.GetComponentsInChildren<QuiverArrows>();
                        wpnHolster.SetActiveQuiverArrows();
                    }

                    if (itemData.itemName == "Particle Arrow")
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
                        wpnHolster.SetItemData(ref wpnHolster.arrowE, ref itemData);
                        wpnHolster.quiverArrows = wpnHolster.particleArrowH.GetComponentsInChildren<QuiverArrows>();
                        wpnHolster.SetActiveQuiverArrows();
                    }

                    if (itemData.itemName == "7.62mm")
                    {
                        DeactiveArrows();
                        DeactiveArrowsHP();
                        wpnHolster.arrowD = wpnHolster.SevenSixTwoAmmoPrefab;
                    }
                }

                inventoryM.RefreshBowAndArrowInventory();
            }

            if (inventoryM.inventoryS == InventoryManager.InventorySection.Shield)
            {
                int slotNum = inventoryM.shieldInv.slotNum;
                inventoryM.shieldInv.slotShieldEquipped = slotNum;
                wpnHolster.slotWeapon = slotNum;

                ItemData itemData = inventoryM.shieldInv.itemData[inventoryM.shieldInv.slotNum];

                #region Check If Broken

                if (itemData.broken && inventoryM.itemRepair)
                {
                    infoMessage.info.text = "You cannot equip a broken weapon.";
                    dropDown.value = 0;
                    DestroyImmediate(dropDownObj);
                    gameObject.SetActive(false);
                    return;
                }

                #endregion

                #region Set as equippped

                for (int i = 0; i < inventoryM.shieldInv.itemData.Length; i++)
                {
                    itemData.equipped = false;
                }
                itemData.equipped = true;

                #endregion

                #region Outline

                foreach (Image outline in inventoryM.shieldInv.outLineBorder)
                {
                    outline.enabled = false;
                    outline.color = inventoryM.outLineHover;
                }
                Image outLine = inventoryM.shieldInv.outLineBorder[inventoryM.shieldInv.slotShieldEquipped];
                outLine.enabled = true;
                outLine.color = inventoryM.outLineSelected;

                #endregion

                #region Set button navigation to auto

                Image itemHL = inventoryM.shieldInv.highLight[inventoryM.shieldInv.slotNum];
                Navigation auto = new Navigation { mode = Navigation.Mode.Automatic };
                itemHL.GetComponentInChildren<Button>().navigation = auto;

                #endregion

                #region Equipped Icon

                inventoryM.referencesObj.shieldItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                inventoryM.referencesObj.shieldItemImage.enabled = true;
                inventoryM.referencesObj.shieldItemImage.sprite = itemData.itemSprite;
                inventoryM.referencesObj.shieldValueBG.enabled = true;
                inventoryM.referencesObj.shieldValueBG.GetComponentInChildren<TextMeshProUGUI>().text = itemData.maxWpnAtk.ToString();

                #endregion

                if (itemData.itemName == "Circle Shield" + itemData.rankTag)
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
                    wpnHolster.SetItemData(ref wpnHolster.shieldE, ref itemData);
                    inventoryM.preShieldEquipped = true;
                }

                inventoryM.RefreshShieldInventory();
            }

            if (inventoryM.inventoryS == InventoryManager.InventorySection.Armor)
            {
                ItemData itemData = inventoryM.armorInv.itemData[inventoryM.armorInv.slotNum];

                if (itemData.arm.armorType == ItemData.ArmorType.Head)
                {
                    int slotNum = inventoryM.armorInv.slotNum;
                    inventoryM.armorInv.slotArmorHeadEquipped = slotNum;

                    #region Set as equippped

                    for (int i = 0; i < inventoryM.armorInv.itemData.Length; i++)
                    {
                        if (inventoryM.armorInv.itemData[i].arm.armorType == ItemData.ArmorType.Head)
                        {
                            itemData.equipped = false;
                        }
                    }
                    itemData.equipped = true;

                    #endregion

                    #region Outline

                    for (int i = 0; i < inventoryM.armorInv.itemData.Length; i++)
                    {
                        if (inventoryM.armorInv.itemData[i].arm.armorType == ItemData.ArmorType.Head)
                        {
                            inventoryM.armorInv.outLineBorder[i].enabled = false;
                            inventoryM.armorInv.outLineBorder[i].color = inventoryM.outLineHover;
                        }
                    }
                    Image outLine = inventoryM.armorInv.outLineBorder[inventoryM.armorInv.slotArmorHeadEquipped];
                    outLine.enabled = true;
                    outLine.color = inventoryM.outLineSelected;

                    #endregion

                    #region Set button navigation to auto

                    Image itemHL = inventoryM.armorInv.highLight[inventoryM.armorInv.slotNum];
                    Navigation auto = new Navigation { mode = Navigation.Mode.Automatic };
                    itemHL.GetComponentInChildren<Button>().navigation = auto;

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
                    int slotNum = inventoryM.armorInv.slotNum;
                    inventoryM.armorInv.slotArmorChestEquipped = slotNum;

                    #region Set as equippped

                    for (int i = 0; i < inventoryM.armorInv.itemData.Length; i++)
                    {
                        if (inventoryM.armorInv.itemData[i].arm.armorType == ItemData.ArmorType.Chest)
                        {
                            itemData.equipped = false;
                        }
                    }
                    itemData.equipped = true;

                    #endregion

                    #region Outline

                    for (int i = 0; i < inventoryM.armorInv.itemData.Length; i++)
                    {
                        if (inventoryM.armorInv.itemData[i].arm.armorType == ItemData.ArmorType.Chest)
                        {
                            inventoryM.armorInv.outLineBorder[i].enabled = false;
                            inventoryM.armorInv.outLineBorder[i].color = inventoryM.outLineHover;
                        }
                    }
                    Image outLine = inventoryM.armorInv.outLineBorder[inventoryM.armorInv.slotArmorChestEquipped];
                    outLine.enabled = true;
                    outLine.color = inventoryM.outLineSelected;

                    #endregion

                    #region Set button navigation to auto

                    Image itemHL = inventoryM.armorInv.highLight[inventoryM.armorInv.slotNum];
                    Navigation auto = new Navigation { mode = Navigation.Mode.Automatic };
                    itemHL.GetComponentInChildren<Button>().navigation = auto;

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
                    int slotNum = inventoryM.armorInv.slotNum;
                    inventoryM.armorInv.slotArmorLegEquipped = slotNum;

                    #region Set as equippped

                    for (int i = 0; i < inventoryM.armorInv.itemData.Length; i++)
                    {
                        if (inventoryM.armorInv.itemData[i].arm.armorType == ItemData.ArmorType.Legs)
                        {
                            itemData.equipped = false;
                        }
                    }
                    itemData.equipped = true;

                    #endregion

                    #region Outline

                    for (int i = 0; i < inventoryM.armorInv.itemData.Length; i++)
                    {
                        if (inventoryM.armorInv.itemData[i].arm.armorType == ItemData.ArmorType.Chest)
                        {
                            inventoryM.armorInv.outLineBorder[i].enabled = false;
                            inventoryM.armorInv.outLineBorder[i].color = inventoryM.outLineHover;
                        }
                    }
                    Image outLine = inventoryM.armorInv.outLineBorder[inventoryM.armorInv.slotArmorLegEquipped];
                    outLine.enabled = true;
                    outLine.color = inventoryM.outLineSelected;

                    #endregion

                    #region Set button navigation to auto

                    Image itemHL = inventoryM.armorInv.highLight[inventoryM.armorInv.slotNum];
                    Navigation auto = new Navigation { mode = Navigation.Mode.Automatic };
                    itemHL.GetComponentInChildren<Button>().navigation = auto;

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
                    int slotNum = inventoryM.armorInv.slotNum;
                    inventoryM.armorInv.slotArmorAmuletEquipped = slotNum;

                    #region Set as equippped

                    for (int i = 0; i < inventoryM.armorInv.itemData.Length; i++)
                    {
                        if (inventoryM.armorInv.itemData[i].arm.armorType == ItemData.ArmorType.Amulet)
                        {
                            itemData.equipped = false;
                        }
                    }
                    itemData.equipped = true;

                    #endregion

                    #region Outline

                    for (int i = 0; i < inventoryM.armorInv.itemData.Length; i++)
                    {
                        if (inventoryM.armorInv.itemData[i].arm.armorType == ItemData.ArmorType.Chest)
                        {
                            inventoryM.armorInv.outLineBorder[i].enabled = false;
                            inventoryM.armorInv.outLineBorder[i].color = inventoryM.outLineHover;
                        }
                    }
                    Image outLine = inventoryM.armorInv.outLineBorder[inventoryM.armorInv.slotArmorAmuletEquipped];
                    outLine.enabled = true;
                    outLine.color = inventoryM.outLineSelected;

                    #endregion

                    #region Set button navigation to auto

                    Image itemHL = inventoryM.armorInv.highLight[inventoryM.armorInv.slotNum];
                    Navigation auto = new Navigation { mode = Navigation.Mode.Automatic };
                    itemHL.GetComponentInChildren<Button>().navigation = auto;

                    #endregion

                    #region Equipped Icon

                    inventoryM.referencesObj.amuletItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                    inventoryM.referencesObj.amuletItemImage.enabled = true;
                    inventoryM.referencesObj.amuletItemImage.sprite = itemData.itemSprite;
                    inventoryM.referencesObj.amuletValueBG.enabled = true;
                    inventoryM.referencesObj.amuletValueBG.GetComponentInChildren<TextMeshProUGUI>().text = itemData.arm.ToString();

                    #endregion
                }
                inventoryM.RefreshArmorInventory();
            }
            if (equippedAS)
                equippedAS.PlayRandomClip();
        }

        if (index == 2)
        {
            // Dropping Item
            wpnHolster.itemBeingDropped = true;
            if (inventoryM.inventoryS == InventoryManager.InventorySection.Weapon)
            {
                ItemData itemData = inventoryM.weaponInv.itemData[inventoryM.weaponInv.slotNum];
                Image outLine = inventoryM.weaponInv.outLineBorder[inventoryM.weaponInv.slotNum];

                outLine.enabled = false;
                outLine.color = inventoryM.outLineHover;

                inputFieldOptions.inputFieldType = InputFieldOptions.InputFieldType.Drop;

                wpnHolster.SetDropItemData(ref itemData);

                if (!itemData.stackable)
                {
                    if (itemData.equipped)
                    {
                        CreateDropItem(wpnHolster.primaryD);
                        DeactiveWeapons();
                        DeactiveWeaponsHP();
                    }
                    else
                    {
                        CreateDropItem(wpnHolster.primaryD);
                    }

                    inventoryM.RemoveNonStackableItem(ref inventoryM.weaponInv.itemData, ref inventoryM.weaponInv.image, ref inventoryM.weaponInv.highLight, ref inventoryM.weaponInv.outLineBorder, ref inventoryM.weaponInv.statValueBG, ref inventoryM.weaponInv.quantity,
                    ref inventoryM.weaponInv.statValue, ref inventoryM.weaponInv.slotNum, ref inventoryM.weaponInv.removeNullSlots, ref inventoryM.weaponInv.counter, ref inventoryM.weaponInv.statCounter);
                }
                else
                {
                    inputFieldOptions.fadeUI.canvasGroup.alpha = 1;
                    inputFieldOptions.dropItem = wpnHolster.primaryD;
                }
            }

            if (inventoryM.inventoryS == InventoryManager.InventorySection.BowAndArrow)
            {
                ItemData itemData = inventoryM.bowAndArrowInv.itemData[inventoryM.bowAndArrowInv.slotNum];
                Image outLine = inventoryM.bowAndArrowInv.outLineBorder[inventoryM.bowAndArrowInv.slotNum];

                outLine.enabled = false;
                outLine.color = inventoryM.outLineHover;

                inputFieldOptions.inputFieldType = InputFieldOptions.InputFieldType.Drop;

                if (itemData.itemType == ItemData.ItemType.Bow)
                {
                    wpnHolster.SetDropItemData(ref itemData);

                    if (!itemData.stackable)
                    {
                        if (itemData.equipped)
                        {
                            CreateDropItem(wpnHolster.secondaryD);
                            DeactiveBows();
                            DeactiveBowsHP();
                        }
                        else
                        {
                            CreateDropItem(wpnHolster.secondaryD);
                        }

                        inventoryM.RemoveNonStackableItem(ref inventoryM.bowAndArrowInv.itemData, ref inventoryM.bowAndArrowInv.image, ref inventoryM.bowAndArrowInv.highLight, ref inventoryM.bowAndArrowInv.outLineBorder, ref inventoryM.bowAndArrowInv.statValueBG, ref inventoryM.bowAndArrowInv.quantity,
                        ref inventoryM.bowAndArrowInv.statValue, ref inventoryM.bowAndArrowInv.slotNum, ref inventoryM.bowAndArrowInv.removeNullSlots, ref inventoryM.bowAndArrowInv.counter, ref inventoryM.bowAndArrowInv.statCounter);
                    }
                    else
                    {
                        inputFieldOptions.fadeUI.canvasGroup.alpha = 1;
                        inputFieldOptions.dropItem = wpnHolster.secondaryD;
                    }
                }

                if (itemData.itemType == ItemData.ItemType.Arrow)
                {
                    wpnHolster.SetDropItemData(ref itemData);

                    if (!itemData.stackable)
                    {
                        if (itemData.equipped)
                        {
                            CreateDropItem(wpnHolster.arrowD);
                            DeactiveArrows();
                            DeactiveArrowsHP();
                        }
                        else
                        {
                            CreateDropItem(wpnHolster.arrowD);
                        }

                        inventoryM.RemoveNonStackableItem(ref inventoryM.bowAndArrowInv.itemData, ref inventoryM.bowAndArrowInv.image, ref inventoryM.bowAndArrowInv.highLight, ref inventoryM.bowAndArrowInv.outLineBorder, ref inventoryM.bowAndArrowInv.statValueBG, ref inventoryM.bowAndArrowInv.quantity,
                        ref inventoryM.bowAndArrowInv.statValue, ref inventoryM.bowAndArrowInv.slotNum, ref inventoryM.bowAndArrowInv.removeNullSlots, ref inventoryM.bowAndArrowInv.counter, ref inventoryM.bowAndArrowInv.statCounter);
                    }
                    else
                    {
                        inputFieldOptions.fadeUI.canvasGroup.alpha = 1;
                        inputFieldOptions.dropItem = wpnHolster.arrowD;
                    }
                }
            }

            if (inventoryM.inventoryS == InventoryManager.InventorySection.Shield)
            {
                ItemData itemData = inventoryM.shieldInv.itemData[inventoryM.shieldInv.slotNum];
                Image outLine = inventoryM.shieldInv.outLineBorder[inventoryM.shieldInv.slotNum];

                outLine.enabled = false;
                outLine.color = inventoryM.outLineHover;

                wpnHolster.SetDropItemData(ref itemData);

                if (!itemData.stackable)
                {
                    if (itemData.equipped)
                    {
                        CreateDropItem(wpnHolster.shieldD);
                        DeactiveShield();
                        DeactiveShieldHP();
                    }
                    else
                    {
                        CreateDropItem(wpnHolster.shieldD);
                    }

                    inventoryM.RemoveNonStackableItem(ref inventoryM.shieldInv.itemData, ref inventoryM.shieldInv.image, ref inventoryM.shieldInv.highLight, ref inventoryM.shieldInv.outLineBorder, ref inventoryM.shieldInv.statValueBG, ref inventoryM.shieldInv.quantity,
                    ref inventoryM.shieldInv.statValue, ref inventoryM.shieldInv.slotNum, ref inventoryM.bowAndArrowInv.removeNullSlots, ref inventoryM.shieldInv.counter, ref inventoryM.shieldInv.statCounter);
                }
                else
                {
                    inputFieldOptions.fadeUI.canvasGroup.alpha = 1;
                    inputFieldOptions.dropItem = wpnHolster.shieldD;
                }
            }

            if (inventoryM.inventoryS == InventoryManager.InventorySection.Armor)
            {
                infoMessage.info.text = "This item cannot be dropped...";
            }

            if (inventoryM.inventoryS == InventoryManager.InventorySection.Material)
            {
                ItemData itemData = inventoryM.materialInv.itemData[inventoryM.materialInv.slotNum];
                Image outLine = inventoryM.materialInv.outLineBorder[inventoryM.materialInv.slotNum];

                outLine.enabled = false;
                outLine.color = inventoryM.outLineHover;

                wpnHolster.SetDropItemData(ref itemData);

                if (!itemData.stackable)
                {
                    CreateDropItem(wpnHolster.commonItemD);
                    inventoryM.RemoveNonStackableItem(ref inventoryM.materialInv.itemData, ref inventoryM.materialInv.image, ref inventoryM.materialInv.highLight, ref inventoryM.materialInv.outLineBorder, ref inventoryM.healingInv.statValueBG, ref inventoryM.materialInv.quantity,
                    ref inventoryM.materialInv.statValue, ref inventoryM.materialInv.slotNum, ref inventoryM.materialInv.removeNullSlots, ref inventoryM.materialInv.counter, ref inventoryM.healingInv.statCounter);
                }
                else
                {
                    inputFieldOptions.fadeUI.canvasGroup.alpha = 1;
                    inputFieldOptions.dropItem = wpnHolster.commonItemD;
                }
            }

            if (inventoryM.inventoryS == InventoryManager.InventorySection.Healing)
            {
                ItemData itemData = inventoryM.healingInv.itemData[inventoryM.healingInv.slotNum];
                Image outLine = inventoryM.healingInv.outLineBorder[inventoryM.healingInv.slotNum];

                outLine.enabled = false;
                outLine.color = inventoryM.outLineHover;

                wpnHolster.SetDropItemData(ref itemData);

                if (!itemData.stackable)
                {
                    CreateDropItem(wpnHolster.commonItemD);
                    inventoryM.RemoveNonStackableItem(ref inventoryM.healingInv.itemData, ref inventoryM.healingInv.image, ref inventoryM.healingInv.highLight, ref inventoryM.healingInv.outLineBorder, ref inventoryM.healingInv.statValueBG, ref inventoryM.healingInv.quantity,
                    ref inventoryM.healingInv.statValue, ref inventoryM.healingInv.slotNum, ref inventoryM.healingInv.removeNullSlots, ref inventoryM.healingInv.counter, ref inventoryM.healingInv.statCounter);
                }
                else
                {
                    inputFieldOptions.fadeUI.canvasGroup.alpha = 1;
                    inputFieldOptions.dropItem = wpnHolster.commonItemD;
                }
            }
            inventoryM.itemDescription.inventoryD = ItemDescription.InventoryDescription.Null;
        }

        if (index == 3)
        {
            // Cancel Item
            if (inventoryM.inventoryS == InventoryManager.InventorySection.Weapon)
            {
                ItemData itemData = inventoryM.weaponInv.itemData[inventoryM.weaponInv.slotNum];
                Image outLine = inventoryM.weaponInv.outLineBorder[inventoryM.weaponInv.slotNum];

                if (itemData.equipped)
                {
                    outLine.enabled = true;
                    outLine.color = inventoryM.outLineSelected;
                }
                else
                {
                    outLine.enabled = false;
                    outLine.color = inventoryM.outLineHover;
                }
            }

            if (inventoryM.inventoryS == InventoryManager.InventorySection.BowAndArrow)
            {
                ItemData itemData = inventoryM.bowAndArrowInv.itemData[inventoryM.bowAndArrowInv.slotNum];
                Image outLine = inventoryM.bowAndArrowInv.outLineBorder[inventoryM.bowAndArrowInv.slotNum];

                if (itemData.equipped)
                {
                    outLine.enabled = true;
                    outLine.color = inventoryM.outLineSelected;
                }
                else
                {
                    outLine.enabled = false;
                    outLine.color = inventoryM.outLineHover;
                }
            }

            if (inventoryM.inventoryS == InventoryManager.InventorySection.Shield)
            {
                ItemData itemData = inventoryM.shieldInv.itemData[inventoryM.shieldInv.slotNum];
                Image outLine = inventoryM.shieldInv.outLineBorder[inventoryM.shieldInv.slotNum];

                if (itemData.equipped)
                {
                    outLine.enabled = true;
                    outLine.color = inventoryM.outLineSelected;
                }
                else
                {
                    outLine.enabled = false;
                    outLine.color = inventoryM.outLineHover;
                }
            }

            if (inventoryM.inventoryS == InventoryManager.InventorySection.Armor)
            {
                ItemData itemData = inventoryM.armorInv.itemData[inventoryM.armorInv.slotNum];
                Image outLine = inventoryM.armorInv.outLineBorder[inventoryM.armorInv.slotNum];

                if (itemData.equipped)
                {
                    outLine.enabled = true;
                    outLine.color = inventoryM.outLineSelected;
                }
                else
                {
                    outLine.enabled = false;
                    outLine.color = inventoryM.outLineHover;
                }
            }

            if (inventoryM.inventoryS == InventoryManager.InventorySection.Material)
            {
                ItemData itemData = inventoryM.materialInv.itemData[inventoryM.materialInv.slotNum];
                Image outLine = inventoryM.materialInv.outLineBorder[inventoryM.materialInv.slotNum];

                if (itemData.equipped)
                {
                    outLine.enabled = true;
                    outLine.color = inventoryM.outLineSelected;
                }
                else
                {
                    outLine.enabled = false;
                    outLine.color = inventoryM.outLineHover;
                }
            }

            if (inventoryM.inventoryS == InventoryManager.InventorySection.Healing)
            {
                ItemData itemData = inventoryM.healingInv.itemData[inventoryM.healingInv.slotNum];
                Image outLine = inventoryM.healingInv.outLineBorder[inventoryM.healingInv.slotNum];

                if (itemData.equipped)
                {
                    outLine.enabled = true;
                    outLine.color = inventoryM.outLineSelected;
                }
                else
                {
                    outLine.enabled = false;
                    outLine.color = inventoryM.outLineHover;
                }
            }

            if (inventoryM.inventoryS == InventoryManager.InventorySection.Key)
            {
                ItemData itemData = inventoryM.keyInv.itemData[inventoryM.keyInv.slotNum];
                Image outLine = inventoryM.keyInv.outLineBorder[inventoryM.keyInv.slotNum];

                if (itemData.equipped)
                {
                    outLine.enabled = true;
                    outLine.color = inventoryM.outLineSelected;
                }
                else
                {
                    outLine.enabled = false;
                    outLine.color = inventoryM.outLineHover;
                }
            }
            if (cancelAS)
                cancelAS.PlayRandomClip();
        }

        dropDown.value = 0;
        DestroyImmediate(dropDownObj);
        gameObject.SetActive(false);
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

    void CreateDropItem(GameObject itemDrop)
    {
        GameObject drop = Instantiate(itemDrop, dropTarget.transform.position + new Vector3(Random.Range(-0.8f, 0.8f), 1f,
        Random.Range(-0.8f, 0.8f)), Quaternion.Euler(0, 0, 90));
        drop.gameObject.SetActive(true);
        drop.GetComponentInChildren<ItemData>().equipped = false;
        drop.GetComponentInChildren<ItemData>().inInventory = false;
        drop.GetComponentInChildren<ItemData>().itemActive = true;
    }

    void PopulateList() { 
		dropDown.AddOptions (names);
	}
}