using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using RPGAE.CharacterController;

public class ItemOptionsRemove : MonoBehaviour {

    public InputFieldOptions inputFieldOptions;
    readonly List<string> names = new List<string>() { "Choose:", "Remove", "Drop", "Cancel"};

    [Header("AUDIO")]
    public RandomAudioPlayer removeAS;
    public RandomAudioPlayer cancelAS;

    #region Private 

    private bool onButton;
    private TMP_Dropdown dropDown;
    private TextMeshProUGUI selectedName;
    private Transform dropTarget;
    private GameObject dropDownObj;
    private InfoMessage infoMessage;
    private ThirdPersonController cc;
    private InventoryManager inventoryM;
    private WeaponHolster wpnHolster;

    #endregion

    // Use this for initialization
    void Awake()
    {
        dropTarget = GameObject.Find("Character").transform;

        // Get Components
        inputFieldOptions = FindObjectOfType<InputFieldOptions>();
        wpnHolster = GameObject.Find("PlayerHolster").GetComponent<WeaponHolster>();
        dropDown = GetComponent<TMP_Dropdown>();
        infoMessage = FindObjectOfType<InfoMessage>();
        cc = FindObjectOfType<ThirdPersonController>();
        inventoryM = FindObjectOfType<InventoryManager>();
        selectedName = GameObject.Find("RemoveLabel").GetComponent<TextMeshProUGUI>();
        PopulateList();
    }

    // Update is called once per frame
    void Update ()
    {
        dropDownObj = GameObject.Find("Dropdown List");

        if (onButton)
        {
            if (cc.rpgaeIM.PlayerControls.Action.triggered)
            {
                dropDown.Show();
                onButton = false;
            }
        }
    }

    public void DropDownSelect(int index)
    {
        selectedName.text = names[index];
        if (index == 1)
        {
            if (inventoryM.inventoryS == InventoryManager.InventorySection.Weapon)
            {
                ItemData itemData = inventoryM.weaponInv.itemData[inventoryM.weaponInv.slotNum];
                Image outLine = inventoryM.weaponInv.outLineBorder[inventoryM.weaponInv.slotNum];

                itemData.equipped = false;

                outLine.enabled = false;
                outLine.color = inventoryM.outLineHover;

                DeactiveWeapons();
                DeactiveWeaponsHP();
            }

            if (inventoryM.inventoryS == InventoryManager.InventorySection.BowAndArrow)
            {
                ItemData itemData = inventoryM.bowAndArrowInv.itemData[inventoryM.bowAndArrowInv.slotNum];
                Image outLine = inventoryM.weaponInv.outLineBorder[inventoryM.bowAndArrowInv.slotNum];

                itemData.equipped = false;

                outLine.enabled = false;
                outLine.color = inventoryM.outLineHover;

                if (itemData.itemType == ItemData.ItemType.Bow)
                {
                    DeactiveBows();
                    DeactiveBowsHP();
                }

                if (itemData.itemType == ItemData.ItemType.Arrow)
                {
                    DeactiveArrows();
                    DeactiveArrowsHP();
                }
            }

            if (inventoryM.inventoryS == InventoryManager.InventorySection.Shield)
            {
                ItemData itemData = inventoryM.shieldInv.itemData[inventoryM.shieldInv.slotNum];
                Image outLine = inventoryM.shieldInv.outLineBorder[inventoryM.shieldInv.slotNum];

                itemData.equipped = false;

                outLine.enabled = false;
                outLine.color = inventoryM.outLineHover;

                DeactiveShield();
                DeactiveShieldHP();
            }

            if (inventoryM.inventoryS == InventoryManager.InventorySection.Armor)
            {
                ItemData itemData = inventoryM.armorInv.itemData[inventoryM.armorInv.slotNum];
                Image outLine = inventoryM.armorInv.outLineBorder[inventoryM.armorInv.slotNum];

                itemData.equipped = false;

                outLine.enabled = false;
                outLine.color = inventoryM.outLineHover;
            }
            if (removeAS)
                removeAS.PlayRandomClip();

            inventoryM.itemDescription.inventoryD = ItemDescription.InventoryDescription.Null;
        }

        if (index == 2)
        {
            // Drop Item
            wpnHolster.itemBeingDropped = true;
            if (inventoryM.inventoryS == InventoryManager.InventorySection.Weapon)
            {
                ItemData itemData = inventoryM.weaponInv.itemData[inventoryM.weaponInv.slotNum];
                Image outline = inventoryM.weaponInv.outLineBorder[inventoryM.weaponInv.slotNum];

                outline.enabled = false;
                outline.color = inventoryM.outLineHover;

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
                Image outline = inventoryM.bowAndArrowInv.outLineBorder[inventoryM.bowAndArrowInv.slotNum];

                outline.enabled = false;
                outline.color = inventoryM.outLineHover;

                wpnHolster.SetDropItemData(ref itemData);

                if (itemData.itemType == ItemData.ItemType.Bow)
                {
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
                Image outline = inventoryM.shieldInv.outLineBorder[inventoryM.shieldInv.slotNum];

                outline.enabled = false;
                outline.color = inventoryM.outLineHover;

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
                    ref inventoryM.shieldInv.statValue, ref inventoryM.shieldInv.slotNum, ref inventoryM.shieldInv.removeNullSlots, ref inventoryM.shieldInv.counter, ref inventoryM.shieldInv.statCounter);
                }
                else
                {
                    inputFieldOptions.fadeUI.canvasGroup.alpha = 1;
                    inputFieldOptions.dropItem = wpnHolster.shieldD;
                }
            }

            if (inventoryM.inventoryS == InventoryManager.InventorySection.Armor)
            {
                infoMessage.info.text = "This item cannot be dropped.";
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

    void CreateDropItem(GameObject itemDrop)
    {
        GameObject drop = Instantiate(itemDrop, dropTarget.transform.position + new Vector3(Random.Range(-0.8f, 0.8f), 1f,
        Random.Range(-0.8f, 0.8f)), Quaternion.Euler(0, 0, 90));
        drop.gameObject.SetActive(true);
        drop.GetComponentInChildren<ItemData>().equipped = false;
        drop.GetComponentInChildren<ItemData>().inInventory = false;
        drop.GetComponentInChildren<ItemData>().itemActive = true;
    }

    void PopulateList()
    {
        dropDown.AddOptions(names);
    }
}
