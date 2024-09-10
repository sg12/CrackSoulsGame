using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ItemOptionsDrop : MonoBehaviour {

    public InputFieldOptions inputFieldOptions;
    readonly List<string> names = new List<string>() { "Choose:", "Drop", "Cancel" };

    [Header("AUDIO")]
    public RandomAudioPlayer cancelAS;

    #region Private 

    private TMP_Dropdown dropDown;
    private TextMeshProUGUI selectedName;
    private Transform dropTarget;
    private GameObject dropDownObj;
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
        inventoryM = FindObjectOfType<InventoryManager>();
        selectedName = GameObject.Find("DropLabel").GetComponent<TextMeshProUGUI>();
        PopulateList();
    }

    // Update is called once per frame
    void Update()
    {
        dropDownObj = GameObject.Find("Dropdown List");
    }

    public void DropDownSelect(int index)
    {
        selectedName.text = names[index];

        if (index == 1)
        {
            // Dropping Item
            wpnHolster.itemBeingDropped = true;
            if (inventoryM.inventoryS == InventoryManager.InventorySection.Material)
            {
                ItemData itemData = inventoryM.materialInv.itemData[inventoryM.materialInv.slotNum];
                Image outline = inventoryM.materialInv.outLineBorder[inventoryM.materialInv.slotNum];

                outline.enabled = false;
                outline.color = inventoryM.outLineHover;

                wpnHolster.SetDropItemData(ref itemData);

                if (!itemData.stackable)
                {
                    CreateDropItem(wpnHolster.commonItemD);
                    inventoryM.RemoveNonStackableItem(ref inventoryM.materialInv.itemData, ref inventoryM.materialInv.image, ref inventoryM.materialInv.highLight, ref inventoryM.materialInv.outLineBorder, ref inventoryM.healingInv.statValueBG, ref inventoryM.materialInv.quantity,
                    ref inventoryM.materialInv.statValue, ref inventoryM.materialInv.slotNum, ref inventoryM.materialInv.removeNullSlots, ref inventoryM.materialInv.counter, ref inventoryM.healingInv.statCounter);
                }
                else
                {
                    inputFieldOptions.inputFieldType = InputFieldOptions.InputFieldType.Drop;
                    inputFieldOptions.fadeUI.canvasGroup.alpha = 1;
                    inputFieldOptions.dropItem = wpnHolster.commonItemD;
                }
            }

            if (inventoryM.inventoryS == InventoryManager.InventorySection.Healing)
            {
                ItemData itemData = inventoryM.healingInv.itemData[inventoryM.healingInv.slotNum];
                Image outline = inventoryM.healingInv.outLineBorder[inventoryM.healingInv.slotNum];

                outline.enabled = false;
                outline.color = inventoryM.outLineHover;

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

        if (index == 2)
        {
            // Cancel Item
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
            if (cancelAS)
                cancelAS.PlayRandomClip();
        }

        dropDown.value = 0;
        DestroyImmediate(dropDownObj);
        gameObject.SetActive(false);
    }

    void CreateDropItem(GameObject itemDrop)
    {
        GameObject drop = Instantiate(itemDrop, dropTarget.transform.position + new Vector3(Random.Range(-0.8f, 0.8f), 1f,
        Random.Range(-0.8f, 0.8f)), Quaternion.Euler(0, 0, 90));
        drop.gameObject.SetActive(true);
        drop.GetComponent<ItemData>().equipped = false;
        drop.GetComponent<ItemData>().inInventory = false;
        drop.GetComponent<ItemData>().itemActive = true;
    }

    void PopulateList()
    {
        dropDown.AddOptions(names);
    }
}