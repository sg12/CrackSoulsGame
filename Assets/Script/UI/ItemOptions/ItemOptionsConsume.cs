using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ItemOptionsConsume : MonoBehaviour 
{
    public InputFieldOptions inputFieldOptions;
    readonly List<string> names = new List<string>() { "Choose:", "Consume", "Drop", "Cancel" };

    [Header("AUDIO")]
    public RandomAudioPlayer cancelAS;

    #region Private

    private TMP_Dropdown dropDown;
    private TextMeshProUGUI selectedName;
    private Transform dropTarget;
    private GameObject dropDownObj;
    private HUDManager hudM;
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
        hudM = FindObjectOfType<HUDManager>();
        inventoryM = FindObjectOfType<InventoryManager>();
        selectedName = GameObject.Find("ConsumeLabel").GetComponent<TextMeshProUGUI>();
        PopulateList();
    }

    // Update is called once per frame
    void Update () 
    {
        dropDownObj = GameObject.Find("Dropdown List");
    }

    public void DropDownSelect(int index)
    {
        selectedName.text = names[index];
        if (index == 1)
        {
            if (inventoryM.inventoryS == InventoryManager.InventorySection.Healing)
            {
                ItemData itemData = inventoryM.healingInv.itemData[inventoryM.healingInv.slotNum];
                Image outLineBorder = inventoryM.healingInv.outLineBorder[inventoryM.healingInv.slotNum];

                outLineBorder.enabled = false;
                //inventoryM.itemEquippedCheck.color = inventoryM.highLightColor;

                if (itemData.itemName == "Elixir of Health")
                {
                    hudM.IncrementHealth(100, false);
                }

                if (itemData.itemName == "Elixir of Energy")
                {
                    hudM.IncrementEnegry(100, false);
                }

                inventoryM.RemoveStackableItem(ref inventoryM.healingInv.itemData, ref inventoryM.healingInv.image, ref inventoryM.healingInv.highLight, ref inventoryM.healingInv.outLineBorder, ref inventoryM.healingInv.statValueBG,
                ref inventoryM.healingInv.quantity, ref inventoryM.healingInv.statValue, ref inventoryM.healingInv.slotNum, ref inventoryM.healingInv.counter,
                ref inventoryM.healingInv.statCounter, ref inventoryM.healingInv.removeNullSlots, 1);
            }

            inventoryM.itemDescription.inventoryD = ItemDescription.InventoryDescription.Null;
        }

        if (index == 2)
        {
            // Dropping Item
            wpnHolster.itemBeingDropped = true;
            if (inventoryM.inventoryS == InventoryManager.InventorySection.Healing)
            {
                ItemData itemData = inventoryM.healingInv.itemData[inventoryM.healingInv.slotNum];
                Image outLineBorder = inventoryM.healingInv.outLineBorder[inventoryM.healingInv.slotNum];

                outLineBorder.enabled = false;
                //inventoryM.itemEquippedCheck.color = inventoryM.highLightColor;

                wpnHolster.SetDropItemData(ref itemData);

                if (!itemData.stackable)
                {
                    CreateDropItem(wpnHolster.commonItemD);
                }
                else
                {
                    inputFieldOptions.inputFieldType = InputFieldOptions.InputFieldType.Drop;
                    inputFieldOptions.fadeUI.canvasGroup.alpha = 1;
                    inputFieldOptions.dropItem = wpnHolster.commonItemD;
                }
            }

            inventoryM.itemDescription.inventoryD = ItemDescription.InventoryDescription.Null;
        }

        if (index == 3)
        {
            // Cancel Item

            if (inventoryM.inventoryS == InventoryManager.InventorySection.Healing)
            {
                ItemData itemData = inventoryM.healingInv.itemData[inventoryM.healingInv.slotNum];
                Image outLineBorder = inventoryM.healingInv.outLineBorder[inventoryM.healingInv.slotNum];

                outLineBorder.enabled = false;
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
