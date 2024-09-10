using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ItemOptionsSell : MonoBehaviour 
{
    public InputFieldOptions inputFieldOptions;
    readonly List<string> names = new List<string>() { "Choose:", "Sell", "Cancel" };

    [Header("AUDIO")]
    public RandomAudioPlayer sellAS;
    public RandomAudioPlayer cancelAS;

    #region Private 

    private TMP_Dropdown dropDown;
    private TextMeshProUGUI selectedName;
    private GameObject dropDownObj;
    private InventoryManager inventoryM;

    #endregion

    // Use this for initialization
    void Start()
    {
        // Get Components
        inputFieldOptions = FindObjectOfType<InputFieldOptions>();
        dropDown = GetComponent<TMP_Dropdown>();
        inventoryM = FindObjectOfType<InventoryManager>();
        selectedName = GameObject.Find("SellLabel").GetComponent<TextMeshProUGUI>();
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
            // Sell Item
            switch (inventoryM.inventoryS)
            {
                case InventoryManager.InventorySection.Weapon:
                    Sell(ref inventoryM.weaponInv.itemData, ref inventoryM.weaponInv.outLineBorder, ref inventoryM.weaponInv.slotNum, ref inventoryM.weaponInv.removeNullSlots, ref inventoryM.weaponInv.counter, ref inventoryM.weaponInv.statCounter,
                    ref inventoryM.weaponInv.image, ref inventoryM.weaponInv.highLight, ref inventoryM.weaponInv.highLight, ref inventoryM.weaponInv.statValueBG, ref inventoryM.weaponInv.quantity, ref inventoryM.weaponInv.statValue);
                    break;
                case InventoryManager.InventorySection.BowAndArrow:
                    Sell(ref inventoryM.bowAndArrowInv.itemData, ref inventoryM.bowAndArrowInv.outLineBorder, ref inventoryM.bowAndArrowInv.slotNum, ref inventoryM.bowAndArrowInv.removeNullSlots, ref inventoryM.bowAndArrowInv.counter, ref inventoryM.bowAndArrowInv.statCounter,
                    ref inventoryM.bowAndArrowInv.image, ref inventoryM.bowAndArrowInv.outLineBorder, ref inventoryM.bowAndArrowInv.highLight, ref inventoryM.bowAndArrowInv.statValueBG, ref inventoryM.bowAndArrowInv.quantity, ref inventoryM.bowAndArrowInv.statValue);
                    break;
                case InventoryManager.InventorySection.Shield:
                    Sell(ref inventoryM.shieldInv.itemData, ref inventoryM.shieldInv.outLineBorder, ref inventoryM.shieldInv.slotNum, ref inventoryM.shieldInv.removeNullSlots, ref inventoryM.shieldInv.counter, ref inventoryM.shieldInv.statCounter,
                    ref inventoryM.shieldInv.image, ref inventoryM.shieldInv.highLight, ref inventoryM.shieldInv.outLineBorder, ref inventoryM.shieldInv.statValueBG, ref inventoryM.shieldInv.quantity, ref inventoryM.shieldInv.statValue);
                    break;
                case InventoryManager.InventorySection.Armor:
                    Sell(ref inventoryM.armorInv.itemData, ref inventoryM.armorInv.outLineBorder, ref inventoryM.armorInv.slotNum, ref inventoryM.armorInv.removeNullSlots, ref inventoryM.armorInv.counter, ref inventoryM.armorInv.statCounter,
                    ref inventoryM.armorInv.image, ref inventoryM.armorInv.highLight, ref inventoryM.armorInv.outLineBorder, ref inventoryM.armorInv.statValueBG, ref inventoryM.armorInv.quantity, ref inventoryM.armorInv.statValue);
                    break;
                case InventoryManager.InventorySection.Material:
                    Sell(ref inventoryM.materialInv.itemData, ref inventoryM.materialInv.outLineBorder, ref inventoryM.materialInv.slotNum, ref inventoryM.materialInv.removeNullSlots, ref inventoryM.materialInv.counter, ref inventoryM.materialInv.statCounter,
                    ref inventoryM.materialInv.image, ref inventoryM.materialInv.highLight, ref inventoryM.materialInv.outLineBorder, ref inventoryM.materialInv.statValueBG, ref inventoryM.materialInv.quantity, ref inventoryM.materialInv.statValue);
                    break;
                case InventoryManager.InventorySection.Healing:
                    Sell(ref inventoryM.healingInv.itemData, ref inventoryM.healingInv.outLineBorder, ref inventoryM.healingInv.slotNum, ref inventoryM.healingInv.removeNullSlots, ref inventoryM.healingInv.counter, ref inventoryM.healingInv.statCounter,
                    ref inventoryM.healingInv.image, ref inventoryM.healingInv.highLight, ref inventoryM.healingInv.outLineBorder, ref inventoryM.healingInv.statValueBG, ref inventoryM.healingInv.quantity, ref inventoryM.healingInv.statValue);
                    break;
                default:
                    break;
            }
            if (sellAS)
                sellAS.PlayRandomClip();
        }

        if (index == 2)
        {
            switch (inventoryM.inventoryS)
            {
                case InventoryManager.InventorySection.Weapon:
                    Image outLineBorder = inventoryM.weaponInv.outLineBorder[inventoryM.weaponInv.slotNum];
                    outLineBorder.enabled = false;
                    break;
                case InventoryManager.InventorySection.BowAndArrow:
                    Image outLineBorder1 = inventoryM.bowAndArrowInv.outLineBorder[inventoryM.bowAndArrowInv.slotNum];
                    outLineBorder1.enabled = false;
                    break;
                case InventoryManager.InventorySection.Shield:
                    Image outLineBorder2 = inventoryM.shieldInv.outLineBorder[inventoryM.shieldInv.slotNum];

                    outLineBorder2.enabled = false;
                    outLineBorder2.color = inventoryM.outLineHover;
                    break;
                case InventoryManager.InventorySection.Armor:
                    Image outLineBorder3 = inventoryM.armorInv.outLineBorder[inventoryM.armorInv.slotNum];

                    outLineBorder3.enabled = false;
                    outLineBorder3.color = inventoryM.outLineHover;
                    break;
                case InventoryManager.InventorySection.Material:
                    Image outLineBorder4 = inventoryM.materialInv.outLineBorder[inventoryM.materialInv.slotNum];

                    outLineBorder4.enabled = false;
                    outLineBorder4.color = inventoryM.outLineHover;
                    break;
                case InventoryManager.InventorySection.Healing:
                    Image outLineBorder5 = inventoryM.healingInv.outLineBorder[inventoryM.healingInv.slotNum];

                    outLineBorder5.enabled = false;
                    outLineBorder5.color = inventoryM.outLineHover;
                    break;
                case InventoryManager.InventorySection.Key:
                    Image outLineBorder6 = inventoryM.keyInv.outLineBorder[inventoryM.keyInv.slotNum];

                    outLineBorder6.enabled = false;
                    outLineBorder6.color = inventoryM.outLineHover;
                    break;
                default:
                    break;
            }
            if (cancelAS)
                cancelAS.PlayRandomClip();
        }

        dropDown.value = 0;
        DestroyImmediate(dropDownObj);
        gameObject.SetActive(false);
    }

    protected void Sell (ref ItemData[] itemData, ref Image[] outLineBorder, ref int InventorySlots, ref int removeNullSlot, ref int[] InventoryCounter, ref int[] statCounter, ref Image[] itemIcon, ref Image[] highLight, ref Image[] outline, ref Image[] statBG, ref TextMeshProUGUI[] itemQuantity, ref TextMeshProUGUI[] statValue)
    {
        if (itemData[InventorySlots].stackable)
        {
            inputFieldOptions.inputFieldType = InputFieldOptions.InputFieldType.Sell;
            inputFieldOptions.fadeUI.canvasGroup.alpha = 1;
        }
        else
        {
            inventoryM.unityCoins += itemData[InventorySlots].sellPrice;
            inventoryM.RemoveNonStackableItem(ref itemData, ref itemIcon, ref highLight, ref outline, ref statBG, ref itemQuantity, ref statValue,
            ref InventorySlots, ref removeNullSlot, ref InventoryCounter, ref statCounter);
        }
        outLineBorder[InventorySlots].enabled = false;
    }

    void PopulateList()
    {
        dropDown.AddOptions(names);
    }
}
