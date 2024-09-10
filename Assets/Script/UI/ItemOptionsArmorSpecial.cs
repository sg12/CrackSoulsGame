using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ItemOptionsArmorSpecial : MonoBehaviour
{
    readonly List<string> names = new List<string>() { "Choose:", "Arm Bonus", "Arm L Res Bonus", "Arm H Res Bonus", "Cancel" };

    [Header("AUDIO")]
    public RandomAudioPlayer specialAS;
    public RandomAudioPlayer cancelAS;

    #region Private 

    private TMP_Dropdown dropDown;
    private TextMeshProUGUI selectedName;
    private GameObject dropDownObj;
    private InfoMessage infoMessage;
    private InventoryManager inventoryM;
    private int originPrice;
    private WeaponHolster wpnHolster;
    private TMP_Dropdown dropDownSpecial;

    #endregion

    // Use this for initialization
    void Awake()
    {
        // Get Components
        wpnHolster = GameObject.Find("PlayerHolster").GetComponent<WeaponHolster>();
        dropDown = GetComponent<TMP_Dropdown>();
        infoMessage = FindObjectOfType<InfoMessage>();
        inventoryM = FindObjectOfType<InventoryManager>();
        selectedName = GameObject.Find("ArmorSpecialLabel").GetComponent<TextMeshProUGUI>();
        dropDown.AddOptions(names);
        dropDownSpecial = GameObject.Find("DropdownSpecial").GetComponent<TMP_Dropdown>();

        int slotNum = inventoryM.armorInv.slotNum;
        originPrice = inventoryM.armorInv.itemData[slotNum].specialPrice;
    }

    // Update is called oneShot per frame
    void Update()
    {
        dropDownObj = GameObject.Find("Dropdown List");
    }

    public void DropDownSelect(int index)
    {
        selectedName.text = names[index];

        int slotNum = inventoryM.armorInv.slotNum;
        inventoryM.armorInv.slotNum = slotNum;
        wpnHolster.slotWeapon = slotNum;

        ItemData itemData = inventoryM.armorInv.itemData[slotNum];

        if (index == 1)
        {
            if (inventoryM.unityCoins >= itemData.specialPrice)
            {
                if (dropDownSpecial.value == 1)
                {
                    if (specialAS) specialAS.PlayRandomClip();
                    inventoryM.unityCoins -= itemData.specialPrice;
                    itemData.armSpecialStat1 = ItemData.ArmorSpecialStat1.ArmBonus;
                    itemData.maxArm += itemData.armSpecialStatValue1;
                    itemData.armSpecialStatName1 = "Armor Bonus + " + itemData.armSpecialStatValue1;
                    itemData.armSpecialStatInfo1 = "Additional armor points adds resistance to phyiscal damage.";
                    itemData.numOfEngraving++;
                }
                else if (dropDownSpecial.value == 2)
                {
                    if (specialAS) specialAS.PlayRandomClip();
                    inventoryM.unityCoins -= itemData.specialPrice;
                    itemData.armSpecialStat2 = ItemData.ArmorSpecialStat2.ArmBonus;
                    itemData.maxArm += itemData.armSpecialStatValue2;
                    itemData.armSpecialStatName2 = "Armor Bonus + " + itemData.armSpecialStatValue2;
                    itemData.armSpecialStatInfo2 = "Additional armor points adds resistance to phyiscal damage.";
                    itemData.numOfEngraving++;
                }
                else if (dropDownSpecial.value == 0)
                {
                    infoMessage.info.text = "Use the drop Dropdown option to add special engravings.";
                }

                for (int i = 0; i < inventoryM.armorInv.itemData.Length; i++)
                {
                    if(inventoryM.armorInv.itemData[i].arm.armorType == ItemData.ArmorType.Head)
                    {
                        inventoryM.referencesObj.headItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                        inventoryM.referencesObj.headItemImage.enabled = true;
                        inventoryM.referencesObj.headItemImage.sprite = itemData.itemSprite;
                        inventoryM.referencesObj.headValueBG.enabled = true;
                        inventoryM.referencesObj.headValueBG.GetComponentInChildren<TextMeshProUGUI>().text = itemData.maxArm.ToString();

                        inventoryM.armorInv.statValue[i].text = itemData.maxArm.ToString();
                    }

                    if (inventoryM.armorInv.itemData[i].arm.armorType == ItemData.ArmorType.Chest)
                    {
                        inventoryM.referencesObj.chestItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                        inventoryM.referencesObj.chestItemImage.enabled = true;
                        inventoryM.referencesObj.chestItemImage.sprite = itemData.itemSprite;
                        inventoryM.referencesObj.chestValueBG.enabled = true;
                        inventoryM.referencesObj.chestValueBG.GetComponentInChildren<TextMeshProUGUI>().text = itemData.maxArm.ToString();

                        inventoryM.armorInv.statValue[i].text = itemData.maxArm.ToString();
                    }

                    if (inventoryM.armorInv.itemData[i].arm.armorType == ItemData.ArmorType.Legs)
                    {
                        inventoryM.referencesObj.legItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                        inventoryM.referencesObj.legItemImage.enabled = true;
                        inventoryM.referencesObj.legItemImage.sprite = itemData.itemSprite;
                        inventoryM.referencesObj.legValueBG.enabled = true;
                        inventoryM.referencesObj.legValueBG.GetComponentInChildren<TextMeshProUGUI>().text = itemData.maxArm.ToString();

                        inventoryM.armorInv.statValue[i].text = itemData.maxArm.ToString();
                    }
                }
            }
            else
                infoMessage.info.text = "You don't have enough coins.";
        }

        if (index == 2)
        {
            if (inventoryM.unityCoins >= itemData.specialPrice)
            {
                if (dropDownSpecial.value == 1)
                {
                    if (specialAS) specialAS.PlayRandomClip();
                    inventoryM.unityCoins -= itemData.specialPrice;
                    itemData.armSpecialStat1 = ItemData.ArmorSpecialStat1.ArmLResBonus;
                    itemData.maxArmLRes += itemData.armSpecialStatValue1;
                    itemData.armSpecialStatName1 = "Armor Light Resistance Bonus + " + itemData.armSpecialStatValue1;
                    itemData.armSpecialStatInfo1 = "Additional armor light attack points adds resistance to light phyiscal damage.";
                    itemData.numOfEngraving++;
                }
                else if (dropDownSpecial.value == 2)
                {
                    if (specialAS) specialAS.PlayRandomClip();
                    inventoryM.unityCoins -= itemData.specialPrice;
                    itemData.armSpecialStat2 = ItemData.ArmorSpecialStat2.ArmLResBonus;
                    itemData.maxArmLRes += itemData.armSpecialStatValue2;
                    itemData.armSpecialStatName2 = "Armor Light Resistance Bonus + " + itemData.armSpecialStatValue2;
                    itemData.armSpecialStatInfo2 = "Additional armor light attack points adds resistance to light phyiscal damage.";
                    itemData.numOfEngraving++;
                }
                else if (dropDownSpecial.value == 0)
                {
                    infoMessage.info.text = "Use the drop Dropdown option to add special engravings.";
                }

                for (int i = 0; i < inventoryM.armorInv.itemData.Length; i++)
                {
                    if (inventoryM.armorInv.itemData[i].arm.armorType == ItemData.ArmorType.Head)
                    {
                        inventoryM.referencesObj.headItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                        inventoryM.referencesObj.headItemImage.enabled = true;
                        inventoryM.referencesObj.headItemImage.sprite = itemData.itemSprite;
                        inventoryM.referencesObj.headValueBG.enabled = true;
                    }

                    if (inventoryM.armorInv.itemData[i].arm.armorType == ItemData.ArmorType.Chest)
                    {
                        inventoryM.referencesObj.chestItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                        inventoryM.referencesObj.chestItemImage.enabled = true;
                        inventoryM.referencesObj.chestItemImage.sprite = itemData.itemSprite;
                        inventoryM.referencesObj.chestValueBG.enabled = true;
                    }

                    if (inventoryM.armorInv.itemData[i].arm.armorType == ItemData.ArmorType.Legs)
                    {
                        inventoryM.referencesObj.legItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                        inventoryM.referencesObj.legItemImage.enabled = true;
                        inventoryM.referencesObj.legItemImage.sprite = itemData.itemSprite;
                        inventoryM.referencesObj.legValueBG.enabled = true;
                    }
                }
            }
            else
                infoMessage.info.text = "You don't have enough coins.";
        }

        if (index == 3)
        {
            if (inventoryM.unityCoins >= itemData.specialPrice)
            {
                if (dropDownSpecial.value == 1)
                {
                    if (specialAS) specialAS.PlayRandomClip();
                    inventoryM.unityCoins -= itemData.specialPrice;
                    itemData.armSpecialStat1 = ItemData.ArmorSpecialStat1.ArmHResBonus;
                    itemData.maxArmHRes += itemData.armSpecialStatValue1;
                    itemData.armSpecialStatName1 = "Armor Heavy Resistance Bonus + " + itemData.armSpecialStatValue1;
                    itemData.armSpecialStatInfo1 = "Additional armor heavy attack points adds resistance to heavy phyiscal damage.";
                    itemData.numOfEngraving++;
                }
                else if (dropDownSpecial.value == 2)
                {
                    if (specialAS) specialAS.PlayRandomClip();
                    inventoryM.unityCoins -= itemData.specialPrice;
                    itemData.armSpecialStat2 = ItemData.ArmorSpecialStat2.ArmHResBonus;
                    itemData.maxArmHRes += itemData.armSpecialStatValue2;
                    itemData.armSpecialStatName2 = "Armor Heavy Resistance Bonus + " + itemData.armSpecialStatValue2;
                    itemData.armSpecialStatInfo2 = "Additional armor heavy attack points adds resistance to heavy phyiscal damage.";
                    itemData.numOfEngraving++;
                }
                else if (dropDownSpecial.value == 0)
                {
                    infoMessage.info.text = "Use the drop Dropdown option to add special engravings.";
                }

                for (int i = 0; i < inventoryM.armorInv.itemData.Length; i++)
                {
                    if (inventoryM.armorInv.itemData[i].arm.armorType == ItemData.ArmorType.Head)
                    {
                        inventoryM.referencesObj.headItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                        inventoryM.referencesObj.headItemImage.enabled = true;
                        inventoryM.referencesObj.headItemImage.sprite = itemData.itemSprite;
                        inventoryM.referencesObj.headValueBG.enabled = true;
                    }

                    if (inventoryM.armorInv.itemData[i].arm.armorType == ItemData.ArmorType.Chest)
                    {
                        inventoryM.referencesObj.chestItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                        inventoryM.referencesObj.chestItemImage.enabled = true;
                        inventoryM.referencesObj.chestItemImage.sprite = itemData.itemSprite;
                        inventoryM.referencesObj.chestValueBG.enabled = true;
                    }

                    if (inventoryM.armorInv.itemData[i].arm.armorType == ItemData.ArmorType.Legs)
                    {
                        inventoryM.referencesObj.legItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                        inventoryM.referencesObj.legItemImage.enabled = true;
                        inventoryM.referencesObj.legItemImage.sprite = itemData.itemSprite;
                        inventoryM.referencesObj.legValueBG.enabled = true;
                    }
                }
            }
            else
                infoMessage.info.text = "You don't have enough coins.";
        }

        if (index == 3)
        {
            if (cancelAS)
                cancelAS.PlayRandomClip();
        }

        inventoryM.storeM.specialM.CheckInventoryItems(ref inventoryM.armorInv.itemData, ref inventoryM.armorInv.slotNum, ref inventoryM.armorInv.counter);

        dropDown.value = 0;
        DestroyImmediate(dropDownObj);
        gameObject.SetActive(false);
    }

    public void PriceCheck(Transform select)
    {
        int slotNum = inventoryM.armorInv.slotNum;
        inventoryM.armorInv.slotNum = slotNum;
        wpnHolster.slotWeapon = slotNum;

        ItemData itemData = inventoryM.armorInv.itemData[slotNum];

        itemData.specialPrice = originPrice;

        if (select.name == "Item 1: Arm Bonus")
            itemData.specialPrice += 1500;

        if (select.name == "Item 2: Arm L Res Bonus")
            itemData.specialPrice += 1800;

        if (select.name == "Item 3: Arm H Res Bonus")
            itemData.specialPrice += 2200;

        inventoryM.storeM.specialM.CheckInventoryItems(ref inventoryM.armorInv.itemData, ref inventoryM.armorInv.slotNum, ref inventoryM.armorInv.counter);
    }
}
