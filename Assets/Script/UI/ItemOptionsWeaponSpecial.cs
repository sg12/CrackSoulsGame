using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using RPGAE.CharacterController;

public class ItemOptionsWeaponSpecial : MonoBehaviour
{
    readonly List<string> names = new List<string>() { "Choose:", "Atk Bonus", "Spd Bonus", "Stun Bonus", "Crit-Hit Bonus", "Dura Bonus", "Throw Med Bonus", "Throw Long Bonus", "Throw Endless Bonus", "Cancel" };

    [Header("AUDIO")]
    public RandomAudioPlayer specialAS;
    public RandomAudioPlayer cancelAS;

    #region Private 

    private ThirdPersonController cc;
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
        cc = FindObjectOfType<ThirdPersonController>();
        selectedName = GameObject.Find("WeaponSpecialLabel").GetComponent<TextMeshProUGUI>();
        dropDown.AddOptions(names);
        dropDownSpecial = GameObject.Find("DropdownSpecial").GetComponent<TMP_Dropdown>();

        int slotNum = inventoryM.weaponInv.slotNum;
        originPrice = inventoryM.weaponInv.itemData[slotNum].specialPrice;
    }

    // Update is called oneShot per frame
    void Update()
    {
        dropDownObj = GameObject.Find("Dropdown List");
    }

    public void DropDownSelect(int index)
    {
        selectedName.text = names[index];

        int slotNum = inventoryM.weaponInv.slotNum;
        inventoryM.weaponInv.slotWeaponEquipped = slotNum;
        wpnHolster.slotWeapon = slotNum;

        ItemData itemData = inventoryM.weaponInv.itemData[slotNum];

        if (index == 1)
        {
            if (inventoryM.unityCoins >= itemData.specialPrice)
            {
                if(dropDownSpecial.value == 1)
                {
                    if (specialAS) specialAS.PlayRandomClip();
                    inventoryM.unityCoins -= itemData.specialPrice;
                    itemData.wpnSpecialStat1 = ItemData.WeaponSpecialStat1.AtkBonus;
                    itemData.maxWpnAtk += itemData.wpnSpecialStatValue1;
                    itemData.wpnSpecialStatName1 = "Attack Bonus + " + itemData.wpnSpecialStatValue1;
                    itemData.wpnSpecialStatInfo1 = "Additional attack points makes your weapon more powerful than before.";
                    itemData.numOfEngraving++;
                }
                else if (dropDownSpecial.value == 2)
                {
                    if (specialAS) specialAS.PlayRandomClip();
                    inventoryM.unityCoins -= itemData.specialPrice;
                    itemData.wpnSpecialStat2 = ItemData.WeaponSpecialStat2.AtkBonus;
                    itemData.maxWpnAtk += itemData.wpnSpecialStatValue2;
                    itemData.wpnSpecialStatName2 = "Attack Bonus + " + itemData.wpnSpecialStatValue2;
                    itemData.wpnSpecialStatInfo2 = "Additional attack points makes your weapon more powerful than before.";
                    itemData.numOfEngraving++;
                }

                inventoryM.referencesObj.primaryItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                inventoryM.referencesObj.primaryItemImage.enabled = true;
                inventoryM.referencesObj.primaryItemImage.sprite = itemData.itemSprite;
                inventoryM.referencesObj.primaryValueBG.enabled = true;
                inventoryM.referencesObj.primaryValueBG.GetComponentInChildren<TextMeshProUGUI>().text = itemData.maxWpnAtk.ToString();

                for (int i = 0; i < inventoryM.weaponInv.itemData.Length; i++)
                {
                    inventoryM.weaponInv.statValue[i].text = itemData.maxWpnAtk.ToString();
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
                    itemData.wpnSpecialStat1 = ItemData.WeaponSpecialStat1.SpdBonus;
                    itemData.maxWpnSpd += itemData.wpnSpecialStatValue1;
                    itemData.wpnSpecialStatName1 = "Speed Bonus + " + itemData.wpnSpecialStatValue1;
                    itemData.wpnSpecialStatInfo1 = "Additional speed points makes your weapon swing a lot more quicker.";
                    itemData.numOfEngraving++;
                }
                else if (dropDownSpecial.value == 2)
                {
                    if (specialAS) specialAS.PlayRandomClip();
                    inventoryM.unityCoins -= itemData.specialPrice;
                    itemData.wpnSpecialStat2 = ItemData.WeaponSpecialStat2.SpdBonus;
                    itemData.maxWpnSpd += itemData.wpnSpecialStatValue2;
                    itemData.wpnSpecialStatName2 = "Speed Bonus + " + itemData.wpnSpecialStatValue2;
                    itemData.wpnSpecialStatInfo2 = "Additional speed points makes your weapon swing a lot more quicker.";
                    itemData.numOfEngraving++;
                }
                else if (dropDownSpecial.value == 0)
                {
                    infoMessage.info.text = "Use the drop Dropdown option to add special engravings.";
                }

                inventoryM.referencesObj.primaryItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                inventoryM.referencesObj.primaryItemImage.enabled = true;
                inventoryM.referencesObj.primaryItemImage.sprite = itemData.itemSprite;
                inventoryM.referencesObj.primaryValueBG.enabled = true;
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
                    itemData.wpnSpecialStat1 = ItemData.WeaponSpecialStat1.StunBonus;
                    itemData.maxWpnStun += itemData.wpnSpecialStatValue1;
                    itemData.wpnSpecialStatName1 = "Stun Bonus + " + itemData.wpnSpecialStatValue1;
                    itemData.wpnSpecialStatInfo1 = "Additional stun points staggers it's target at a faster pace.";
                    itemData.numOfEngraving++;
                }
                else if (dropDownSpecial.value == 2)
                {
                    if (specialAS) specialAS.PlayRandomClip();
                    inventoryM.unityCoins -= itemData.specialPrice;
                    itemData.wpnSpecialStat2 = ItemData.WeaponSpecialStat2.StunBonus;
                    itemData.maxWpnStun += itemData.wpnSpecialStatValue2;
                    itemData.wpnSpecialStatName2 = "Stun Bonus + " + itemData.wpnSpecialStatValue2;
                    itemData.wpnSpecialStatInfo2 = "Additional stun points staggers it's target at a faster pace.";
                    itemData.numOfEngraving++;
                }
                else if (dropDownSpecial.value == 0)
                {
                    infoMessage.info.text = "Use the drop Dropdown option to add special engravings.";
                }

                inventoryM.referencesObj.primaryItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                inventoryM.referencesObj.primaryItemImage.enabled = true;
                inventoryM.referencesObj.primaryItemImage.sprite = itemData.itemSprite;
                inventoryM.referencesObj.primaryValueBG.enabled = true;
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
                    itemData.wpnSpecialStat1 = ItemData.WeaponSpecialStat1.CritHitBonus;
                    itemData.maxWpnCritHit += itemData.wpnSpecialStatValue1;
                    itemData.wpnSpecialStatName1 = "Critical Hit Bonus + " + itemData.wpnSpecialStatValue1;
                    itemData.wpnSpecialStatInfo1 = "Additional Critical Hit points increases the chance of dealing more damage.";
                    itemData.numOfEngraving++;
                }
                else if (dropDownSpecial.value == 2)
                {
                    if (specialAS) specialAS.PlayRandomClip();
                    inventoryM.unityCoins -= itemData.specialPrice;
                    itemData.wpnSpecialStat2 = ItemData.WeaponSpecialStat2.CritHitBonus;
                    itemData.maxWpnCritHit += itemData.wpnSpecialStatValue2;
                    itemData.wpnSpecialStatName2 = "Critical Hit Bonus + " + itemData.wpnSpecialStatValue2;
                    itemData.wpnSpecialStatInfo2 = "Additional Critical Hit points increases the chance of dealing more damage.";
                    itemData.numOfEngraving++;
                }
                else if (dropDownSpecial.value == 0)
                {
                    infoMessage.info.text = "Use the drop Dropdown option to add special engravings.";
                }

                inventoryM.referencesObj.primaryItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                inventoryM.referencesObj.primaryItemImage.enabled = true;
                inventoryM.referencesObj.primaryItemImage.sprite = itemData.itemSprite;
                inventoryM.referencesObj.primaryValueBG.enabled = true;
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
                    itemData.wpnSpecialStat1 = ItemData.WeaponSpecialStat1.DuraBonus;
                    itemData.maxWpnDura += itemData.wpnSpecialStatValue1;
                    itemData.wpnSpecialStatName1 = "Durability Bonus + " + itemData.wpnSpecialStatValue1;
                    itemData.wpnSpecialStatInfo1 = "Additional Durability points increases amount of hits before it breaks.";
                    itemData.numOfEngraving++;
                }
                else if (dropDownSpecial.value == 2)
                {
                    if (specialAS) specialAS.PlayRandomClip();
                    inventoryM.unityCoins -= itemData.specialPrice;
                    itemData.wpnSpecialStat2 = ItemData.WeaponSpecialStat2.DuraBonus;
                    itemData.maxWpnDura += itemData.wpnSpecialStatValue2;
                    itemData.wpnSpecialStatName2 = "Durability Bonus + " + itemData.wpnSpecialStatValue2;
                    itemData.wpnSpecialStatInfo2 = "Additional Durability points increases amount of hits before it breaks.";
                    itemData.numOfEngraving++;
                }
                else if (dropDownSpecial.value == 0)
                {
                    infoMessage.info.text = "Use the drop Dropdown option to add special engravings.";
                }

                inventoryM.referencesObj.primaryItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                inventoryM.referencesObj.primaryItemImage.enabled = true;
                inventoryM.referencesObj.primaryItemImage.sprite = itemData.itemSprite;
                inventoryM.referencesObj.primaryValueBG.enabled = true;
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
                    itemData.wpnSpecialStat1 = ItemData.WeaponSpecialStat1.ThrowMedBonus;
                    itemData.wpnSpecialStatName1 = "Throw++";
                    itemData.wpnSpecialStatInfo1 = "Weapon can be thrown within a medium range area.";
                    itemData.numOfEngraving++;
                }
                else if (dropDownSpecial.value == 2)
                {
                    if (specialAS) specialAS.PlayRandomClip();
                    inventoryM.unityCoins -= itemData.specialPrice;
                    itemData.wpnSpecialStat2 = ItemData.WeaponSpecialStat2.ThrowMedBonus;
                    itemData.wpnSpecialStatName2 = "Throw++";
                    itemData.wpnSpecialStatInfo2 = "Weapon can be thrown within a medium range area.";
                    itemData.numOfEngraving++;
                }
                else if (dropDownSpecial.value == 0)
                {
                    infoMessage.info.text = "Use the drop Dropdown option to add special engravings.";
                }

                inventoryM.referencesObj.primaryItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                inventoryM.referencesObj.primaryItemImage.enabled = true;
                inventoryM.referencesObj.primaryItemImage.sprite = itemData.itemSprite;
                inventoryM.referencesObj.primaryValueBG.enabled = true;
            }
            else
                infoMessage.info.text = "You don't have enough coins.";
        }

        if (index == 7)
        {
            if (itemData.rankNum == 2) infoMessage.info.text = "This item cannot be specialized anymore.";

            if (inventoryM.unityCoins >= itemData.specialPrice)
            {
                if (dropDownSpecial.value == 1)
                {
                    if (specialAS) specialAS.PlayRandomClip();
                    inventoryM.unityCoins -= itemData.specialPrice;
                    itemData.wpnSpecialStat1 = ItemData.WeaponSpecialStat1.ThrowLongBonus;
                    itemData.wpnSpecialStatName1 = "Throw+++";
                    itemData.wpnSpecialStatInfo1 = "Weapon can be thrown within a long range area.";
                    itemData.numOfEngraving++;
                }
                else if (dropDownSpecial.value == 2)
                {
                    if (specialAS) specialAS.PlayRandomClip();
                    inventoryM.unityCoins -= itemData.specialPrice;
                    itemData.wpnSpecialStat2 = ItemData.WeaponSpecialStat2.ThrowLongBonus;
                    itemData.wpnSpecialStatName2 = "Throw+++";
                    itemData.wpnSpecialStatInfo2 = "Weapon can be thrown within a long range area.";
                    itemData.numOfEngraving++;
                }
                else if (dropDownSpecial.value == 0)
                {
                    infoMessage.info.text = "Use the drop Dropdown option to add special engravings.";
                }

                inventoryM.referencesObj.primaryItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                inventoryM.referencesObj.primaryItemImage.enabled = true;
                inventoryM.referencesObj.primaryItemImage.sprite = itemData.itemSprite;
                inventoryM.referencesObj.primaryValueBG.enabled = true;
            }
            else
                infoMessage.info.text = "You don't have enough coins.";
        }

        if (index == 8)
        {
            if (inventoryM.unityCoins >= itemData.specialPrice)
            {
                if (dropDownSpecial.value == 1)
                {
                    if (specialAS) specialAS.PlayRandomClip();
                    inventoryM.unityCoins -= itemData.specialPrice;
                    itemData.wpnSpecialStat1 = ItemData.WeaponSpecialStat1.ThrowEndlessBonus;
                    itemData.wpnSpecialStatName1 = "Throw∞";
                    itemData.wpnSpecialStatInfo1 = "Weapon thrown soars in the air endlessly until contact.";
                    itemData.numOfEngraving++;
                }
                else if (dropDownSpecial.value == 2)
                {
                    if (specialAS) specialAS.PlayRandomClip();
                    inventoryM.unityCoins -= itemData.specialPrice;
                    itemData.wpnSpecialStat2 = ItemData.WeaponSpecialStat2.ThrowEndlessBonus;
                    itemData.wpnSpecialStatName2 = "Throw∞";
                    itemData.wpnSpecialStatInfo2 = "Weapon thrown soars in the air endlessly until contact.";
                    itemData.numOfEngraving++;
                }
                else if (dropDownSpecial.value == 0)
                {
                    infoMessage.info.text = "Use the drop Dropdown option to add special engravings.";
                }

                inventoryM.referencesObj.primaryItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                inventoryM.referencesObj.primaryItemImage.enabled = true;
                inventoryM.referencesObj.primaryItemImage.sprite = itemData.itemSprite;
                inventoryM.referencesObj.primaryValueBG.enabled = true;
            }
            else
                infoMessage.info.text = "You don't have enough coins.";
        }

        if (index == 9)
        {
            if (cancelAS)
                cancelAS.PlayRandomClip();
        }

        inventoryM.storeM.specialM.CheckInventoryItems(ref inventoryM.weaponInv.itemData, ref inventoryM.weaponInv.slotNum, ref inventoryM.weaponInv.counter);

        dropDown.value = 0;
        dropDownSpecial.value = 0;
        DestroyImmediate(dropDownObj);
        gameObject.SetActive(false);
    }

    public void PriceCheck(Transform select)
    {
        int slotNum = inventoryM.weaponInv.slotNum;
        inventoryM.weaponInv.slotWeaponEquipped = slotNum;
        wpnHolster.slotWeapon = slotNum;

        ItemData itemData = inventoryM.weaponInv.itemData[slotNum];

        itemData.specialPrice = originPrice;

        if (select.name == "Item 1: Atk Bonus")
            itemData.specialPrice += 1600;

        if (select.name == "Item 2: Spd Bonus")
            itemData.specialPrice += 1500;

        if (select.name == "Item 3: Stun Bonus")
            itemData.specialPrice += 1100;

        if (select.name == "Item 4: Crit-Hit Bonus")
            itemData.specialPrice += 1250;

        if (select.name == "Item 5: Dura Bonus")
            itemData.specialPrice += 1350;

        if (select.name == "Item 6: Throw Med Bonus")
            itemData.specialPrice += 1600;

        if (select.name == "Item 7: Throw Long Bonus")
            itemData.specialPrice += 2100;

        if (select.name == "Item 8: Throw Endless Bonus")
            itemData.specialPrice += 3000;

        inventoryM.storeM.specialM.CheckInventoryItems(ref inventoryM.weaponInv.itemData, ref inventoryM.weaponInv.slotNum, ref inventoryM.weaponInv.counter);
    }
}
