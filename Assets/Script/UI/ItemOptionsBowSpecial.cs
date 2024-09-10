using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using RPGAE.CharacterController;

public class ItemOptionsBowSpecial : MonoBehaviour
{
    readonly List<string> names = new List<string>() { "Choose:", "Atk Bonus", "Spd Bonus", "Stun Bonus", "Crit-Hit Bonus", "Dura Bonus", "Hd-Shot Bonus", 
        "Med Range Bonus", "Long Range Bonus", "Endless Range Bonus", "Quick-Shot", "Three-Shot Burst", "Five-Shot Burst", "Cancel" };

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
        int slotNum = inventoryM.bowAndArrowInv.slotNum;
        originPrice = inventoryM.bowAndArrowInv.itemData[slotNum].specialPrice;

        // Get Components
        wpnHolster = GameObject.Find("PlayerHolster").GetComponent<WeaponHolster>();
        dropDown = GetComponent<TMP_Dropdown>();
        infoMessage = FindObjectOfType<InfoMessage>();
        cc = FindObjectOfType<ThirdPersonController>();
        inventoryM = FindObjectOfType<InventoryManager>();
        selectedName = GameObject.Find("BowSpecialLabel").GetComponent<TextMeshProUGUI>();
        dropDown.AddOptions(names);
        dropDownSpecial = GameObject.Find("DropdownSpecial").GetComponent<TMP_Dropdown>();
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
            int slotNum = inventoryM.bowAndArrowInv.slotNum;
            inventoryM.bowAndArrowInv.slotBowEquipped = slotNum;
            wpnHolster.slotWeapon = slotNum;

            ItemData itemData = inventoryM.bowAndArrowInv.itemData[inventoryM.bowAndArrowInv.slotNum];

            if (index == 1)
            {
                if (inventoryM.unityCoins >= itemData.specialPrice)
                {
                    if (dropDownSpecial.value == 1)
                    {
                        if (specialAS) specialAS.PlayRandomClip();
                        inventoryM.unityCoins -= itemData.specialPrice;
                        itemData.bowSpecialStat1 = ItemData.BowSpecialStat1.AtkBonus;
                        itemData.maxBowAtk += itemData.bowSpecialStatValue1;
                        itemData.bowSpecialStatName1 = "Attack Bonus + " + itemData.bowSpecialStatValue1;
                        itemData.bowSpecialStatInfo1 = "Additional attack points makes your bow more powerful.";
                        itemData.numOfEngraving++;
                    }
                    else if (dropDownSpecial.value == 2)
                    {
                        if (specialAS) specialAS.PlayRandomClip();
                        inventoryM.unityCoins -= itemData.specialPrice;
                        itemData.bowSpecialStat2 = ItemData.BowSpecialStat2.AtkBonus;
                        itemData.maxBowAtk += itemData.bowSpecialStatValue2;
                        itemData.bowSpecialStatName2 = "Attack Bonus + " + itemData.bowSpecialStatValue2;
                        itemData.bowSpecialStatInfo2 = "Additional attack points makes your bow more powerful.";
                        itemData.numOfEngraving++;
                    }
                    else if (dropDownSpecial.value == 0)
                    {
                        infoMessage.info.text = "Use the drop Dropdown option to add special engravings.";
                    }

                    for (int i = 0; i < inventoryM.bowAndArrowInv.itemData.Length; i++)
                    {
                        inventoryM.referencesObj.secondaryItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                        inventoryM.referencesObj.secondaryItemImage.enabled = true;
                        inventoryM.referencesObj.secondaryItemImage.sprite = itemData.itemSprite;
                        inventoryM.referencesObj.secondaryValueBG.enabled = true;
                        inventoryM.referencesObj.secondaryValueBG.GetComponentInChildren<TextMeshProUGUI>().text = itemData.maxBowAtk.ToString();

                        inventoryM.bowAndArrowInv.statValue[i].text = itemData.maxBowAtk.ToString();
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
                        itemData.bowSpecialStat1 = ItemData.BowSpecialStat1.SpdBonus;
                        itemData.maxBowSpd += itemData.bowSpecialStatValue1;
                        itemData.bowSpecialStatName1 = "Speed Bonus + " + itemData.bowSpecialStatValue1;
                        itemData.bowSpecialStatInfo1 = "Additional speed points makes your bow draw quicker.";
                        itemData.numOfEngraving++;
                    }
                    else if (dropDownSpecial.value == 2)
                    {
                        if (specialAS) specialAS.PlayRandomClip();
                        inventoryM.unityCoins -= itemData.specialPrice;
                        itemData.bowSpecialStat2 = ItemData.BowSpecialStat2.SpdBonus;
                        itemData.maxBowSpd += itemData.bowSpecialStatValue2;
                        itemData.bowSpecialStatName2 = "Speed Bonus + " + itemData.bowSpecialStatValue2;
                        itemData.bowSpecialStatInfo2 = "Additional speed points makes your bow draw quicker.";
                        itemData.numOfEngraving++;
                    }
                    else if (dropDownSpecial.value == 0)
                    {
                        infoMessage.info.text = "Use the drop Dropdown option to add special engravings.";
                    }

                    inventoryM.referencesObj.secondaryItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                    inventoryM.referencesObj.secondaryItemImage.enabled = true;
                    inventoryM.referencesObj.secondaryItemImage.sprite = itemData.itemSprite;
                    inventoryM.referencesObj.secondaryValueBG.enabled = true;
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
                        itemData.bowSpecialStat1 = ItemData.BowSpecialStat1.StunBonus;
                        itemData.maxBowStun += itemData.bowSpecialStatValue1;
                        itemData.bowSpecialStatName1 = "Stun Bonus + " + itemData.bowSpecialStatValue1;
                        itemData.bowSpecialStatInfo1 = "Additional stun points deals more stun damage.";
                        itemData.numOfEngraving++;
                    }
                    else if (dropDownSpecial.value == 2)
                    {
                        if (specialAS) specialAS.PlayRandomClip();
                        inventoryM.unityCoins -= itemData.specialPrice;
                        itemData.bowSpecialStat2 = ItemData.BowSpecialStat2.StunBonus;
                        itemData.maxBowStun += itemData.bowSpecialStatValue2;
                        itemData.bowSpecialStatName2 = "Stun Bonus + " + itemData.bowSpecialStatValue2;
                        itemData.bowSpecialStatInfo2 = "Additional stun points deals more stun damage.";
                        itemData.numOfEngraving++;
                    }
                    else if (dropDownSpecial.value == 0)
                    {
                        infoMessage.info.text = "Use the drop Dropdown option to add special engravings.";
                    }

                    inventoryM.referencesObj.secondaryItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                    inventoryM.referencesObj.secondaryItemImage.enabled = true;
                    inventoryM.referencesObj.secondaryItemImage.sprite = itemData.itemSprite;
                    inventoryM.referencesObj.secondaryValueBG.enabled = true;
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
                        itemData.bowSpecialStat1 = ItemData.BowSpecialStat1.CritHitBonus;
                        itemData.maxBowCritHit += itemData.bowSpecialStatValue1;
                        itemData.bowSpecialStatName1 = "Critical Hit Bonus + " + itemData.bowSpecialStatValue1;
                        itemData.bowSpecialStatInfo1 = "Additional Critical Hit points increases the chance of dealing more damage.";
                        itemData.numOfEngraving++;
                    }
                    else if (dropDownSpecial.value == 2)
                    {
                        if (specialAS) specialAS.PlayRandomClip();
                        inventoryM.unityCoins -= itemData.specialPrice;
                        itemData.bowSpecialStat2 = ItemData.BowSpecialStat2.CritHitBonus;
                        itemData.maxBowCritHit += itemData.bowSpecialStatValue2;
                        itemData.bowSpecialStatName2 = "Critical Hit Bonus + " + itemData.bowSpecialStatValue2;
                        itemData.bowSpecialStatInfo2 = "Additional Critical Hit points increases the chance of dealing more damage.";
                        itemData.numOfEngraving++;
                    }
                    else if (dropDownSpecial.value == 0)
                    {
                        infoMessage.info.text = "Use the drop Dropdown option to add special engravings.";
                    }

                    inventoryM.referencesObj.secondaryItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                    inventoryM.referencesObj.secondaryItemImage.enabled = true;
                    inventoryM.referencesObj.secondaryItemImage.sprite = itemData.itemSprite;
                    inventoryM.referencesObj.secondaryValueBG.enabled = true;
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
                        itemData.bowSpecialStat1 = ItemData.BowSpecialStat1.DuraBonus;
                        itemData.maxBowDura += itemData.bowSpecialStatValue1;
                        itemData.bowSpecialStatName1 = "Durability Bonus + " + itemData.bowSpecialStatValue1;
                        itemData.bowSpecialStatInfo1 = "Additional Durability points increases the use of bow shots before it breaks.";
                        itemData.numOfEngraving++;
                    }
                    else if (dropDownSpecial.value == 2)
                    {
                        if (specialAS) specialAS.PlayRandomClip();
                        inventoryM.unityCoins -= itemData.specialPrice;
                        itemData.bowSpecialStat2 = ItemData.BowSpecialStat2.DuraBonus;
                        itemData.maxBowDura += itemData.bowSpecialStatValue2;
                        itemData.bowSpecialStatName2 = "Durability Bonus +" + itemData.bowSpecialStatValue2;
                        itemData.bowSpecialStatInfo2 = "Additional Durability points increases the use of bow shots before it breaks.";
                        itemData.numOfEngraving++;
                    }
                    else if (dropDownSpecial.value == 0)
                    {
                        infoMessage.info.text = "Use the drop Dropdown option to add special engravings.";
                    }

                    inventoryM.referencesObj.secondaryItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                    inventoryM.referencesObj.secondaryItemImage.enabled = true;
                    inventoryM.referencesObj.secondaryItemImage.sprite = itemData.itemSprite;
                    inventoryM.referencesObj.secondaryValueBG.enabled = true;
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
                        itemData.bowSpecialStat1 = ItemData.BowSpecialStat1.HeadShotBonus;
                        itemData.maxBowHdShot += itemData.bowSpecialStatValue1;
                        itemData.bowSpecialStatName1 = "Head-Shot Bonus + " + itemData.bowSpecialStatValue1;
                        itemData.bowSpecialStatInfo1 = "additional head shot points deals more damage than before.";
                        itemData.numOfEngraving++;
                    }
                    else if (dropDownSpecial.value == 2)
                    {
                        if (specialAS) specialAS.PlayRandomClip();
                        inventoryM.unityCoins -= itemData.specialPrice;
                        itemData.bowSpecialStat2 = ItemData.BowSpecialStat2.HeadShotBonus;
                        itemData.maxBowHdShot += itemData.bowSpecialStatValue2;
                        itemData.bowSpecialStatName2 = "Head-Shot Bonus + " + itemData.bowSpecialStatValue2;
                        itemData.bowSpecialStatInfo2 = "additional head shot points deals more damage than before.";
                        itemData.numOfEngraving++;
                    }
                    else if (dropDownSpecial.value == 0)
                    {
                        infoMessage.info.text = "Use the drop Dropdown option to add special engravings.";
                    }

                    inventoryM.referencesObj.secondaryItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                    inventoryM.referencesObj.secondaryItemImage.enabled = true;
                    inventoryM.referencesObj.secondaryItemImage.sprite = itemData.itemSprite;
                    inventoryM.referencesObj.secondaryValueBG.enabled = true;
                }
                else
                    infoMessage.info.text = "You don't have enough coins.";
            }

            if (index == 7)
            {
                if (inventoryM.unityCoins >= itemData.specialPrice)
                {
                    if (dropDownSpecial.value == 1)
                    {
                        if (specialAS) specialAS.PlayRandomClip();
                        inventoryM.unityCoins -= itemData.specialPrice;
                        itemData.bowSpecialStat1 = ItemData.BowSpecialStat1.MedRangeBonus;
                        itemData.bowSpecialStatName1 = "Range++";
                        itemData.bowSpecialStatInfo1 = "Weapon can be thrown within a medium range area.";
                        itemData.numOfEngraving++;
                    }
                    else if (dropDownSpecial.value == 2)
                    {
                        if (specialAS) specialAS.PlayRandomClip();
                        inventoryM.unityCoins -= itemData.specialPrice;
                        itemData.bowSpecialStat2 = ItemData.BowSpecialStat2.MedRangeBonus;
                        itemData.bowSpecialStatName2 = "Range++";
                        itemData.bowSpecialStatInfo2 = "Weapon can be thrown within a medium range area.";
                        itemData.numOfEngraving++;
                    }
                    else if (dropDownSpecial.value == 0)
                    {
                        infoMessage.info.text = "Use the drop Dropdown option to add special engravings.";
                    }

                    inventoryM.referencesObj.secondaryItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                    inventoryM.referencesObj.secondaryItemImage.enabled = true;
                    inventoryM.referencesObj.secondaryItemImage.sprite = itemData.itemSprite;
                    inventoryM.referencesObj.secondaryValueBG.enabled = true;
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
                        itemData.bowSpecialStat1 = ItemData.BowSpecialStat1.LongRangeBonus;
                        itemData.bowAtk.value += itemData.bowSpecialStatValue1;
                        itemData.bowSpecialStatName1 = "Range+++";
                        itemData.bowSpecialStatInfo1 = "Weapon can be thrown within a long range area.";
                        itemData.numOfEngraving++;
                    }
                    else if (dropDownSpecial.value == 2)
                    {
                        if (specialAS) specialAS.PlayRandomClip();
                        inventoryM.unityCoins -= itemData.specialPrice;
                        itemData.bowSpecialStat2 = ItemData.BowSpecialStat2.LongRangeBonus;
                        itemData.bowAtk.value += itemData.bowSpecialStatValue2;
                        itemData.bowSpecialStatName2 = "Range+++";
                        itemData.bowSpecialStatInfo2 = "Weapon can be thrown within a long range area.";
                        itemData.numOfEngraving++;
                    }
                    else if (dropDownSpecial.value == 0)
                    {
                        infoMessage.info.text = "Use the drop Dropdown option to add special engravings.";
                    }

                    inventoryM.referencesObj.secondaryItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                    inventoryM.referencesObj.secondaryItemImage.enabled = true;
                    inventoryM.referencesObj.secondaryItemImage.sprite = itemData.itemSprite;
                    inventoryM.referencesObj.secondaryValueBG.enabled = true;
                }
                else
                    infoMessage.info.text = "You don't have enough coins.";
            }

            if (index == 9)
            {
                if (inventoryM.unityCoins >= itemData.specialPrice)
                {
                    if (dropDownSpecial.value == 1)
                    {
                        if (specialAS) specialAS.PlayRandomClip();
                        inventoryM.unityCoins -= itemData.specialPrice;
                        itemData.bowSpecialStat1 = ItemData.BowSpecialStat1.EndlessRangeBonus;
                        itemData.bowSpecialStatName1 = "Range∞";
                        itemData.bowSpecialStatInfo1 = "arrow shot from bow soars in the air endlessly until contact.";
                        itemData.numOfEngraving++;
                    }
                    else if (dropDownSpecial.value == 2)
                    {
                        if (specialAS) specialAS.PlayRandomClip();
                        inventoryM.unityCoins -= itemData.specialPrice;
                        itemData.bowSpecialStat2 = ItemData.BowSpecialStat2.EndlessRangeBonus;
                        itemData.bowSpecialStatName2 = "Range∞";
                        itemData.bowSpecialStatInfo2 = "arrow shot from bow soars in the air endlessly until contact.";
                        itemData.numOfEngraving++;
                    }
                    else if (dropDownSpecial.value == 0)
                    {
                        infoMessage.info.text = "Use the drop Dropdown option to add special engravings.";
                    }

                    inventoryM.referencesObj.secondaryItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                    inventoryM.referencesObj.secondaryItemImage.enabled = true;
                    inventoryM.referencesObj.secondaryItemImage.sprite = itemData.itemSprite;
                    inventoryM.referencesObj.secondaryValueBG.enabled = true;
                }
                else
                    infoMessage.info.text = "You don't have enough coins.";
            }

            if (index == 10)
            {
                if (inventoryM.unityCoins >= itemData.specialPrice)
                {
                    if (dropDownSpecial.value == 1)
                    {
                        if (specialAS) specialAS.PlayRandomClip();
                        inventoryM.unityCoins -= itemData.specialPrice;
                        itemData.bowSpecialStat1 = ItemData.BowSpecialStat1.QuickShot;
                        itemData.bowSpecialStatName1 = "Quick-Shot";
                        itemData.bowSpecialStatInfo1 = "Bow draws arrows instantly.";
                        itemData.numOfEngraving++;
                    }
                    else if (dropDownSpecial.value == 2)
                    {
                        if (specialAS) specialAS.PlayRandomClip();
                        inventoryM.unityCoins -= itemData.specialPrice;
                        itemData.bowSpecialStat2 = ItemData.BowSpecialStat2.QuickShot;
                        itemData.bowSpecialStatName2 = "Quick-Shot";
                        itemData.bowSpecialStatInfo2 = "Bow draws arrows instantly.";
                        itemData.numOfEngraving++;
                    }
                    else if (dropDownSpecial.value == 0)
                    {
                        infoMessage.info.text = "Use the drop Dropdown option to add special engravings.";
                    }

                    inventoryM.referencesObj.secondaryItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                    inventoryM.referencesObj.secondaryItemImage.enabled = true;
                    inventoryM.referencesObj.secondaryItemImage.sprite = itemData.itemSprite;
                    inventoryM.referencesObj.secondaryValueBG.enabled = true;
                }
                else
                    infoMessage.info.text = "You don't have enough coins.";
            }

            if (index == 11)
            {
                if (inventoryM.unityCoins >= itemData.specialPrice)
                {
                    if (dropDownSpecial.value == 1)
                    {
                        if (specialAS) specialAS.PlayRandomClip();
                        inventoryM.unityCoins -= itemData.specialPrice;
                        itemData.bowSpecialStat1 = ItemData.BowSpecialStat1.ThreeShotBurst;
                        itemData.bowRange.shotAmount = 3;
                        itemData.bowSpecialStatName1 = "Three-Shot Burst";
                        itemData.bowSpecialStatInfo1 = "Bow draws and fires three arrows at oneShot.";
                        itemData.numOfEngraving++;
                    }
                    else if (dropDownSpecial.value == 2)
                    {
                        if (specialAS) specialAS.PlayRandomClip();
                        inventoryM.unityCoins -= itemData.specialPrice;
                        itemData.bowSpecialStat2 = ItemData.BowSpecialStat2.ThreeShotBurst;
                        itemData.bowRange.shotAmount = 3;
                        itemData.bowSpecialStatName2 = "Three-Shot Burst";
                        itemData.bowSpecialStatInfo2 = "Bow draws and fires three arrows at oneShot.";
                        itemData.numOfEngraving++;
                    }
                    else if (dropDownSpecial.value == 0)
                    {
                        infoMessage.info.text = "Use the drop Dropdown option to add special engravings.";
                    }

                    inventoryM.referencesObj.secondaryItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                    inventoryM.referencesObj.secondaryItemImage.enabled = true;
                    inventoryM.referencesObj.secondaryItemImage.sprite = itemData.itemSprite;
                    inventoryM.referencesObj.secondaryValueBG.enabled = true;
                }
                else
                    infoMessage.info.text = "You don't have enough coins.";
            }

            if (index == 12)
            {
                if (inventoryM.unityCoins >= itemData.specialPrice)
                {
                    if (dropDownSpecial.value == 1)
                    {
                        if (specialAS) specialAS.PlayRandomClip();
                        inventoryM.unityCoins -= itemData.specialPrice;
                        itemData.bowSpecialStat1 = ItemData.BowSpecialStat1.FiveShotBurst;
                        itemData.bowRange.shotAmount = 5;
                        itemData.bowSpecialStatName1 = "Five-Shot Burst";
                        itemData.bowSpecialStatInfo1 = "arrow shot from bow soars in the air endlessly until contact.";
                        itemData.numOfEngraving++;
                    }
                    else if (dropDownSpecial.value == 2)
                    {
                        if (specialAS) specialAS.PlayRandomClip();
                        inventoryM.unityCoins -= itemData.specialPrice;
                        itemData.bowSpecialStat2 = ItemData.BowSpecialStat2.FiveShotBurst;
                        itemData.bowRange.shotAmount = 5;
                        itemData.bowSpecialStatName2 = "Five-Shot Burst";
                        itemData.bowSpecialStatInfo2 = "arrow shot from bow soars in the air endlessly until contact.";
                        itemData.numOfEngraving++;
                    }
                    else if (dropDownSpecial.value == 0)
                    {
                        infoMessage.info.text = "Use the drop Dropdown option to add special engravings.";
                    }

                    inventoryM.referencesObj.secondaryItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                    inventoryM.referencesObj.secondaryItemImage.enabled = true;
                    inventoryM.referencesObj.secondaryItemImage.sprite = itemData.itemSprite;
                    inventoryM.referencesObj.secondaryValueBG.enabled = true;
                }
                else
                    infoMessage.info.text = "You don't have enough coins.";
            }

            if (index == 6)
            {
                if (cancelAS)
                    cancelAS.PlayRandomClip();
            }

            inventoryM.storeM.specialM.CheckInventoryItems(ref inventoryM.bowAndArrowInv.itemData, ref inventoryM.bowAndArrowInv.slotNum, ref inventoryM.bowAndArrowInv.counter);

            dropDown.value = 0;
            DestroyImmediate(dropDownObj);
            gameObject.SetActive(false);
        }
    }


    public void PriceCheck(Transform select)
    {
        int slotNum = inventoryM.bowAndArrowInv.slotNum;
        inventoryM.bowAndArrowInv.slotBowEquipped = slotNum;
        wpnHolster.slotWeapon = slotNum;

        ItemData itemData = inventoryM.bowAndArrowInv.itemData[slotNum];
        Image itemHL = inventoryM.bowAndArrowInv.highLight[slotNum];

        itemData.specialPrice = originPrice;

        if (select.name == "Item 1: Atk Bonus")
            itemData.specialPrice += 1200;

        if (select.name == "Item 2: Spd Bonus")
            itemData.specialPrice += 1500;

        if (select.name == "Item 3: Stun Bonus")
            itemData.specialPrice += 1900;

        if (select.name == "Item 4: Crit-Hit Bonus")
            itemData.specialPrice += 1400;

        if (select.name == "Item 5: Dura Bonus")
            itemData.specialPrice += 1500;

        if (select.name == "Item 6: Hd-Shot Bonus")
            itemData.specialPrice += 1500;

        if (select.name == "Item 7: Med Range Bonus")
            itemData.specialPrice += 2100;

        if (select.name == "Item 8: Long Range Bonus")
            itemData.specialPrice += 3210;

        if (select.name == "Item 9: Endless Range Bonus")
            itemData.specialPrice += 4400;

        if (select.name == "Item 10: Quick-Shot")
            itemData.specialPrice += 3000;

        if (select.name == "Item 11: Three-Shot Burst")
            itemData.specialPrice += 4500;

        if (select.name == "Item 12: Five-Shot Burst")
            itemData.specialPrice += 5000;

        inventoryM.storeM.specialM.CheckInventoryItems(ref inventoryM.bowAndArrowInv.itemData, 
            ref inventoryM.bowAndArrowInv.slotNum, ref inventoryM.bowAndArrowInv.counter);
    }
}
