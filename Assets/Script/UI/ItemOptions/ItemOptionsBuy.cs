using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ItemOptionsBuy : MonoBehaviour {

    public InputFieldOptions inputFieldOptions;
    readonly List<string> names = new List<string>() { "Choose:", "Buy", "Cancel"};

    [Header("AUDIO")]
    public RandomAudioPlayer itemPurchasedAS;
    public RandomAudioPlayer cancelAS;

    #region Private 

    private TMP_Dropdown dropDown;
    private TextMeshProUGUI selectedName;
    private GameObject dropDownObj;
    private InventoryManager inventoryM;
    private ItemData buyData;
    private BuyStoreManager buyItemM;
    private WeaponHolster WpnHolster;

    #endregion

    // Use this for initialization
    void Awake()
    {
        // Get Components
        inputFieldOptions = FindObjectOfType<InputFieldOptions>();
        buyItemM = FindObjectOfType<BuyStoreManager>();
        inventoryM = FindObjectOfType<InventoryManager>();
        dropDown = GetComponent<TMP_Dropdown>();
        selectedName = GameObject.Find("BuyLabel").GetComponent<TextMeshProUGUI>();
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
        buyData = buyItemM.buyItems.itemData[buyItemM.buyItems.buySlot];
        Image buyHL = buyItemM.buyItems.highLight[buyItemM.buyItems.buySlot];

        if (index == 1)
        {
            // Buy Item
            if (buyHL.color == buyItemM.selectColor)
            {
                if (buyData.stackable)
                {
                    inputFieldOptions.fadeUI.canvasGroup.alpha = 1;
                    inputFieldOptions.inputFieldType = InputFieldOptions.InputFieldType.Buy;
                }
                else
                {
                    inputFieldOptions.inputFieldType = InputFieldOptions.InputFieldType.Null;
                    switch (buyItemM.buyItems.itemData[buyItemM.buyItems.buySlot].itemType)
                    {
                        case ItemData.ItemType.Weapon:
                            inputFieldOptions.CalculateBuyItemNonStackle(ref inventoryM.weaponInv.image, ref buyItemM.buyItems.inStockCounter, ref buyItemM.buyItems.buySlot);
                            break;
                        case ItemData.ItemType.Bow:
                            inputFieldOptions.CalculateBuyItemNonStackle(ref inventoryM.bowAndArrowInv.image, ref buyItemM.buyItems.inStockCounter, ref buyItemM.buyItems.buySlot);
                            break;
                        case ItemData.ItemType.Arrow:
                            inputFieldOptions.CalculateBuyItemNonStackle(ref inventoryM.bowAndArrowInv.image, ref buyItemM.buyItems.inStockCounter, ref buyItemM.buyItems.buySlot);
                            break;
                        case ItemData.ItemType.Shield:
                            inputFieldOptions.CalculateBuyItemNonStackle(ref inventoryM.shieldInv.image, ref buyItemM.buyItems.inStockCounter, ref buyItemM.buyItems.buySlot);
                            break;
                        case ItemData.ItemType.Armor:
                            inputFieldOptions.CalculateBuyItemNonStackle(ref inventoryM.armorInv.image, ref buyItemM.buyItems.inStockCounter, ref buyItemM.buyItems.buySlot);
                            break;
                        case ItemData.ItemType.Material:
                            inputFieldOptions.CalculateBuyItemNonStackle(ref inventoryM.materialInv.image, ref buyItemM.buyItems.inStockCounter, ref buyItemM.buyItems.buySlot);
                            break;
                        case ItemData.ItemType.Healing:
                            inputFieldOptions.CalculateBuyItemNonStackle(ref inventoryM.healingInv.image, ref buyItemM.buyItems.inStockCounter, ref buyItemM.buyItems.buySlot);
                            break;
                        case ItemData.ItemType.Key:
                            inputFieldOptions.CalculateBuyItemNonStackle(ref inventoryM.keyInv.image, ref buyItemM.buyItems.inStockCounter, ref buyItemM.buyItems.buySlot);
                            break;
                    }
                    if (itemPurchasedAS)
                        itemPurchasedAS.PlayRandomClip();

                    buyHL.color = buyItemM.neutralColor;
                }
            }

            #region Check Inventory Stock

            switch (buyItemM.buyItems.itemData[buyItemM.buyItems.buySlot].itemType)
            {
                case ItemData.ItemType.Weapon:
                    buyItemM.CheckInventoryItems(ref inventoryM.weaponInv.itemData, ref inventoryM.weaponInv.counter, ref buyItemM.buyItems.itemData[buyItemM.buyItems.buySlot].itemName);
                    break;
                case ItemData.ItemType.Bow:
                    buyItemM.CheckInventoryItems(ref inventoryM.bowAndArrowInv.itemData, ref inventoryM.bowAndArrowInv.counter, ref buyItemM.buyItems.itemData[buyItemM.buyItems.buySlot].itemName);
                    break;
                case ItemData.ItemType.Arrow:
                    buyItemM.CheckInventoryItems(ref inventoryM.bowAndArrowInv.itemData, ref inventoryM.bowAndArrowInv.counter, ref buyItemM.buyItems.itemData[buyItemM.buyItems.buySlot].itemName);
                    break;
                case ItemData.ItemType.Shield:
                    buyItemM.CheckInventoryItems(ref inventoryM.shieldInv.itemData, ref inventoryM.shieldInv.counter, ref buyItemM.buyItems.itemData[buyItemM.buyItems.buySlot].itemName);
                    break;
                case ItemData.ItemType.Armor:
                    buyItemM.CheckInventoryItems(ref inventoryM.armorInv.itemData, ref inventoryM.armorInv.counter, ref buyItemM.buyItems.itemData[buyItemM.buyItems.buySlot].itemName);
                    break;
                case ItemData.ItemType.Material:
                    buyItemM.CheckInventoryItems(ref inventoryM.materialInv.itemData, ref inventoryM.materialInv.counter, ref buyItemM.buyItems.itemData[buyItemM.buyItems.buySlot].itemName);
                    break;
                case ItemData.ItemType.Healing:
                    buyItemM.CheckInventoryItems(ref inventoryM.healingInv.itemData, ref inventoryM.healingInv.counter, ref buyItemM.buyItems.itemData[buyItemM.buyItems.buySlot].itemName);
                    break;
                case ItemData.ItemType.Key:
                    buyItemM.CheckInventoryItems(ref inventoryM.keyInv.itemData, ref inventoryM.keyInv.counter, ref buyItemM.buyItems.itemData[buyItemM.buyItems.buySlot].itemName);
                    break;
            }

            #endregion
        }

        if (index == 2)
        {
            // Cancel Item
            if (buyHL.color == buyItemM.selectColor)
            {
                buyHL.color = buyItemM.neutralColor;

                if (cancelAS)
                    cancelAS.PlayRandomClip();
            }
        }

        dropDown.value = 0;
        DestroyImmediate(dropDownObj);
        gameObject.SetActive(false);
    }

    void PopulateList()
    {
        dropDown.AddOptions(names);
    }
}
