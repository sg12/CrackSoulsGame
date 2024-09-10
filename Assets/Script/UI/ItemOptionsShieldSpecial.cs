using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ItemOptionsShieldSpecial : MonoBehaviour
{
    readonly List<string> names = new List<string>() { "Choose:", "Atk Bonus", "Spd Bonus", "Stun Bonus", "Crit-Hit Bonus", "Shield Block Bonus", "Dura Bonus", "Cancel" };

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
        wpnHolster = GameObject.Find("PlayerHolster").GetComponent<WeaponHolster>();
        dropDown = GetComponent<TMP_Dropdown>();
        infoMessage = FindObjectOfType<InfoMessage>();
        inventoryM = FindObjectOfType<InventoryManager>();
        selectedName = GameObject.Find("ShieldSpecialLabel").GetComponent<TextMeshProUGUI>();
        dropDown.AddOptions(names);
        dropDownSpecial = GameObject.Find("DropdownSpecial").GetComponent<TMP_Dropdown>();

        int slotNum = inventoryM.shieldInv.slotNum;
        originPrice = inventoryM.shieldInv.itemData[slotNum].specialPrice;
    }

    // Update is called oneShot per frame
    void Update()
    {
        dropDownObj = GameObject.Find("Dropdown List");
    }

    public void DropDownSelect(int index)
    {
        selectedName.text = names[index];

        if (index == 1)
        {
            int slotNum = inventoryM.shieldInv.slotNum;
            inventoryM.shieldInv.slotShieldEquipped = slotNum;
            wpnHolster.slotWeapon = slotNum;

            ItemData itemData = inventoryM.shieldInv.itemData[inventoryM.shieldInv.slotNum];

            if (index == 1)
            {
                if (inventoryM.unityCoins >= itemData.specialPrice)
                {
                    if (dropDownSpecial.value == 1)
                    {
                        if (specialAS) specialAS.PlayRandomClip();
                        inventoryM.unityCoins -= itemData.specialPrice;
                        itemData.shdSpecialStat1 = ItemData.ShieldSpecialStat1.AtkBonus;
                        itemData.maxShdAtk += itemData.shdSpecialStatValue1;
                        itemData.shdSpecialStatName1 = "Attack Bonus + " + itemData.shdSpecialStatValue1;
                        itemData.shdSpecialStatInfo1 = "Additional attack points makes your shield bash more powerful than before.";
                        itemData.numOfEngraving++;
                    }
                    else if (dropDownSpecial.value == 2)
                    {
                        if (specialAS) specialAS.PlayRandomClip();
                        inventoryM.unityCoins -= itemData.specialPrice;
                        itemData.shdSpecialStat2 = ItemData.ShieldSpecialStat2.AtkBonus;
                        itemData.maxShdAtk += itemData.shdSpecialStatValue2;
                        itemData.shdSpecialStatName2 = "Attack Bonus + " + itemData.shdSpecialStatValue2;
                        itemData.shdSpecialStatInfo2 = "Additional attack points makes your shield bash more powerful than before.";
                        itemData.numOfEngraving++;
                    }
                    else if (dropDownSpecial.value == 0)
                    {
                        infoMessage.info.text = "Use the drop Dropdown option to add special engravings.";
                    }

                    for (int i = 0; i < inventoryM.shieldInv.itemData.Length; i++)
                    {
                        inventoryM.referencesObj.shieldItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                        inventoryM.referencesObj.shieldItemImage.enabled = true;
                        inventoryM.referencesObj.shieldItemImage.sprite = itemData.itemSprite;
                        inventoryM.referencesObj.shieldValueBG.enabled = true;
                        inventoryM.referencesObj.shieldValueBG.GetComponentInChildren<TextMeshProUGUI>().text = itemData.maxShdAtk.ToString();

                        inventoryM.shieldInv.statValue[i].text = itemData.maxShdAtk.ToString();
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
                        itemData.shdSpecialStat1 = ItemData.ShieldSpecialStat1.SpdBonus;
                        itemData.maxShdSpd += itemData.shdSpecialStatValue1;
                        itemData.shdSpecialStatName1 = "Speed Bonus + " + itemData.shdSpecialStatValue1;
                        itemData.shdSpecialStatInfo1 = "Additional speed points makes your shield a lot more quicker.";
                        itemData.numOfEngraving++;
                    }
                    else if (dropDownSpecial.value == 2)
                    {
                        if (specialAS) specialAS.PlayRandomClip();
                        inventoryM.unityCoins -= itemData.specialPrice;
                        itemData.shdSpecialStat2 = ItemData.ShieldSpecialStat2.SpdBonus;
                        itemData.maxShdSpd += itemData.shdSpecialStatValue2;
                        itemData.shdSpecialStatName2 = "Speed Bonus + " + itemData.shdSpecialStatValue2;
                        itemData.shdSpecialStatInfo2 = "Additional speed points makes your shield a lot more quicker.";
                        itemData.numOfEngraving++;
                    }
                    else if (dropDownSpecial.value == 0)
                    {
                        infoMessage.info.text = "Use the drop Dropdown option to add special engravings.";
                    }

                    inventoryM.referencesObj.shieldItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                    inventoryM.referencesObj.shieldItemImage.enabled = true;
                    inventoryM.referencesObj.shieldItemImage.sprite = itemData.itemSprite;
                    inventoryM.referencesObj.shieldValueBG.enabled = true;
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
                        itemData.shdSpecialStat1 = ItemData.ShieldSpecialStat1.StunBonus;
                        itemData.maxShdSpd += itemData.shdSpecialStatValue1;
                        itemData.shdSpecialStatName1 = "Stun Bonus + " + itemData.shdSpecialStatValue1;
                        itemData.shdSpecialStatInfo1 = "Additional stun points staggers it's target at a faster pace.";
                        itemData.numOfEngraving++;
                    }
                    else if (dropDownSpecial.value == 2)
                    {
                        if (specialAS) specialAS.PlayRandomClip();
                        inventoryM.unityCoins -= itemData.specialPrice;
                        itemData.shdSpecialStat2 = ItemData.ShieldSpecialStat2.StunBonus;
                        itemData.maxShdSpd += itemData.shdSpecialStatValue2;
                        itemData.shdSpecialStatName2 = "Stun Bonus + " + itemData.shdSpecialStatValue2;
                        itemData.shdSpecialStatInfo2 = "Additional stun points staggers it's target at a faster pace.";
                        itemData.numOfEngraving++;
                    }
                    else if (dropDownSpecial.value == 0)
                    {
                        infoMessage.info.text = "Use the drop Dropdown option to add special engravings.";
                    }

                    inventoryM.referencesObj.shieldItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                    inventoryM.referencesObj.shieldItemImage.enabled = true;
                    inventoryM.referencesObj.shieldItemImage.sprite = itemData.itemSprite;
                    inventoryM.referencesObj.shieldValueBG.enabled = true;
                }
                else
                    infoMessage.info.text = "You don't have enough coins.";
            }

            if (index == 4)
            {
                if (inventoryM.unityCoins >= itemData.specialPrice)
                {
                    if (dropDownSpecial.value == 1)
                    {
                        if (specialAS) specialAS.PlayRandomClip();
                        inventoryM.unityCoins -= itemData.specialPrice;
                        itemData.shdSpecialStat1 = ItemData.ShieldSpecialStat1.ShieldBlockBonus;
                        itemData.maxShdSpd += itemData.shdSpecialStatValue1;
                        itemData.shdSpecialStatName1 = "Shield Block Bonus + " + itemData.shdSpecialStatValue1;
                        itemData.shdSpecialStatInfo1 = "Additional shield block points increases the chance of dealing more damage.";
                        itemData.numOfEngraving++;
                    }
                    else if (dropDownSpecial.value == 2)
                    {
                        if (specialAS) specialAS.PlayRandomClip();
                        inventoryM.unityCoins -= itemData.specialPrice;
                        itemData.shdSpecialStat2 = ItemData.ShieldSpecialStat2.ShieldBlockBonus;
                        itemData.maxShdSpd += itemData.shdSpecialStatValue2;
                        itemData.shdSpecialStatName2 = "Shield Block Bonus + " + itemData.shdSpecialStatValue2;
                        itemData.shdSpecialStatInfo2 = "Additional shield block points increases the chance of dealing more damage.";
                        itemData.numOfEngraving++;
                    }
                    else if (dropDownSpecial.value == 0)
                    {
                        infoMessage.info.text = "You already have these special engravings.";
                    }

                    inventoryM.referencesObj.shieldItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                    inventoryM.referencesObj.shieldItemImage.enabled = true;
                    inventoryM.referencesObj.shieldItemImage.sprite = itemData.itemSprite;
                    inventoryM.referencesObj.shieldValueBG.enabled = true;
                }
                else
                    infoMessage.info.text = "You don't have enough coins.";
            }

            if (index == 5)
            {
                if (inventoryM.unityCoins >= itemData.specialPrice)
                {
                    if (dropDownSpecial.value == 1)
                    {
                        if (specialAS) specialAS.PlayRandomClip();
                        inventoryM.unityCoins -= itemData.specialPrice;
                        itemData.shdSpecialStat1 = ItemData.ShieldSpecialStat1.CritHitBonus;
                        itemData.maxShdSpd += itemData.shdSpecialStatValue1;
                        itemData.shdSpecialStatName1 = "Critical Hit Bonus + " + itemData.shdSpecialStatValue1;
                        itemData.shdSpecialStatInfo1 = "Additional Critical Hit points increases the chance of dealing more damage.";
                        itemData.numOfEngraving++;
                    }
                    else if (dropDownSpecial.value == 2)
                    {
                        if (specialAS) specialAS.PlayRandomClip();
                        inventoryM.unityCoins -= itemData.specialPrice;
                        itemData.shdSpecialStat2 = ItemData.ShieldSpecialStat2.CritHitBonus;
                        itemData.maxShdSpd += itemData.shdSpecialStatValue2;
                        itemData.shdSpecialStatName2 = "Critical Hit Bonus + " + itemData.shdSpecialStatValue2;
                        itemData.shdSpecialStatInfo2 = "Additional Critical Hit points increases the chance of dealing more damage.";
                        itemData.numOfEngraving++;
                    }
                    else if (dropDownSpecial.value == 0)
                    {
                        infoMessage.info.text = "Use the drop Dropdown option to add special engravings.";
                    }

                    inventoryM.referencesObj.shieldItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                    inventoryM.referencesObj.shieldItemImage.enabled = true;
                    inventoryM.referencesObj.shieldItemImage.sprite = itemData.itemSprite;
                    inventoryM.referencesObj.shieldValueBG.enabled = true;
                }
                else
                    infoMessage.info.text = "You don't have enough coins.";
            }

            if (index == 6)
            {
                if (inventoryM.unityCoins >= itemData.specialPrice)
                {
                    if (dropDownSpecial.value == 1)
                    {
                        if (specialAS) specialAS.PlayRandomClip();
                        inventoryM.unityCoins -= itemData.specialPrice;
                        itemData.shdSpecialStat1 = ItemData.ShieldSpecialStat1.DuraBonus;
                        itemData.maxShdSpd += itemData.shdSpecialStatValue1;
                        itemData.shdSpecialStatName1 = "Durability Bonus + " + itemData.shdSpecialStatValue1;
                        itemData.shdSpecialStatInfo1 = "Additional Durability points increases the use of shield blocks before it breaks.";
                        itemData.numOfEngraving++;
                    }
                    else if (dropDownSpecial.value == 2)
                    {
                        if (specialAS) specialAS.PlayRandomClip();
                        inventoryM.unityCoins -= itemData.specialPrice;
                        itemData.shdSpecialStat2 = ItemData.ShieldSpecialStat2.DuraBonus;
                        itemData.maxShdSpd += itemData.shdSpecialStatValue2;
                        itemData.shdSpecialStatName2 = "Durability Bonus + " + itemData.shdSpecialStatValue2;
                        itemData.shdSpecialStatInfo2 = "Additional Durability points increases the use of shield blocks before it breaks.";
                        itemData.numOfEngraving++;
                    }
                    else if (dropDownSpecial.value == 0)
                    {
                        infoMessage.info.text = "Use the drop Dropdown option to add special engravings.";
                    }

                    inventoryM.referencesObj.shieldItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                    inventoryM.referencesObj.shieldItemImage.enabled = true;
                    inventoryM.referencesObj.shieldItemImage.sprite = itemData.itemSprite;
                    inventoryM.referencesObj.shieldValueBG.enabled = true;
                }
                else
                    infoMessage.info.text = "You don't have enough coins.";
            }

            if (index == 5)
            {
                if (cancelAS)
                    cancelAS.PlayRandomClip();
            }

            inventoryM.storeM.specialM.CheckInventoryItems(ref inventoryM.shieldInv.itemData, ref inventoryM.shieldInv.slotNum, ref inventoryM.shieldInv.counter);

            dropDown.value = 0;
            DestroyImmediate(dropDownObj);
            gameObject.SetActive(false);
        }
    }

    public void PriceCheck(Transform select)
    {
        int slotNum = inventoryM.shieldInv.slotNum;
        inventoryM.shieldInv.slotShieldEquipped = slotNum;
        wpnHolster.slotWeapon = slotNum;

        ItemData itemData = inventoryM.shieldInv.itemData[slotNum];

        itemData.specialPrice = originPrice;

        if (select.name == "Item 1: Atk Bonus")
            itemData.specialPrice += 500;

        if (select.name == "Item 2: Spd Bonus")
            itemData.specialPrice += 1000;

        if (select.name == "Item 3: Stun Bonus")
            itemData.specialPrice += 1400;

        if (select.name == "Item 4: Shield Block Bonus")
            itemData.specialPrice += 1300;

        if (select.name == "Item 5: Dura Bonus")
            itemData.specialPrice += 2000;

        inventoryM.storeM.specialM.CheckInventoryItems(ref inventoryM.shieldInv.itemData, ref inventoryM.shieldInv.slotNum, ref inventoryM.shieldInv.counter);
    }
}
