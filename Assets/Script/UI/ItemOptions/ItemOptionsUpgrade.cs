using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using JoshH.UI;

public class ItemOptionsUpgrade : MonoBehaviour
{
    readonly List<string> names = new List<string>() { "Choose:", "Upgrade", "Cancel" };

    [Header("AUDIO")]
    public RandomAudioPlayer upgradeAS;
    public RandomAudioPlayer cancelAS;

    #region Private 

    private WeaponHolster wpnHolster;
    private TMP_Dropdown dropDown;
    private TextMeshProUGUI selectedName;
    private GameObject dropDownObj;
    private InfoMessage infoMessage;
    private InventoryManager inventoryM;
    private UpgradeStoreManager upgradeM;

    #endregion

    // Use this for initialization
    void Awake()
    {
        // Get Components
        wpnHolster = GameObject.Find("PlayerHolster").GetComponent<WeaponHolster>();
        dropDown = GetComponent<TMP_Dropdown>();
        infoMessage = FindObjectOfType<InfoMessage>();
        inventoryM = FindObjectOfType<InventoryManager>();
        upgradeM = FindObjectOfType<UpgradeStoreManager>();
        selectedName = GameObject.Find("UpgradeLabel").GetComponent<TextMeshProUGUI>();
        dropDown.AddOptions(names);
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
            if (inventoryM.inventoryS == InventoryManager.InventorySection.Weapon)
            {
                int slotNum = inventoryM.weaponInv.slotNum;
                wpnHolster.slotWeapon = slotNum;

                ItemData itemData = inventoryM.weaponInv.itemData[inventoryM.weaponInv.slotNum];
                Image itemHL = inventoryM.weaponInv.highLight[inventoryM.weaponInv.slotNum];
                Image outLineBorder = inventoryM.weaponInv.outLineBorder[inventoryM.weaponInv.slotNum];
                TextMeshProUGUI statValue = inventoryM.weaponInv.statValue[inventoryM.weaponInv.slotNum];
                int statCount = inventoryM.weaponInv.statCounter[inventoryM.weaponInv.slotNum];
                
                if (inventoryM.materialInv.image.Length == 0 || inventoryM.materialInv.image[0] == null)
                {
                    inventoryM.cc.infoMessage.info.text = "You don't have enough inventory space.";
                    outLineBorder.enabled = false;
                    dropDown.value = 0;
                    DestroyImmediate(dropDownObj);
                    gameObject.SetActive(false);
                    return;
                }
                
                if (inventoryM.weaponInv.itemData[slotNum].rankNum == 3)
                {
                    infoMessage.info.text = "This weapon is fully upgraded.";
                    outLineBorder.enabled = false;
                    dropDown.value = 0;
                    DestroyImmediate(dropDownObj);
                    gameObject.SetActive(false);
                    return;
                }

                for (int i = 0; i < inventoryM.weaponInv.itemData[slotNum].upgradeMaterial.Length; i++)
                {
                    if (inventoryM.storeM.upgradeM.CheckResources(ref itemData.upgradeMaterial[i].matName, ref itemData.upgradeMaterial[i].matRequired))
                    {
                        outLineBorder.enabled = false;
                        dropDown.value = 0;
                        DestroyImmediate(dropDownObj);
                        gameObject.SetActive(false);
                        return;
                    }
                }

                for (int i = 0; i < inventoryM.weaponInv.itemData[slotNum].upgradeMaterial.Length; i++)
                {
                    if (upgradeM.Upgrade(itemData.upgradeMaterial[i].matName, itemData.upgradeMaterial[i].matRequired, ref itemData)
                    && inventoryM.unityCoins >= itemData.upgradePrice)
                    {
                        inventoryM.unityCoins -= itemData.upgradePrice;

                        if (itemData.rankNum == 1)
                        {
                            itemData.originItemName = itemData.itemName;
                            itemData.itemName +=  "+";
                            itemData.maxWpnAtk = itemData.wpnAtk.GetRank1Value();
                            itemData.maxWpnSpd = itemData.wpnSpd.GetRank1Value();
                            itemData.maxWpnStun = itemData.wpnStun.GetRank1Value();
                            itemData.maxWpnCritHit = itemData.wpnCritHit.GetRank1Value();
                            itemData.maxWpnDura = itemData.wpnDura.GetRank1Value();
                            statCount = itemData.maxWpnAtk;
                            statValue.text = statCount.ToString();
                            itemHL.GetComponent<UIGradient>().enabled = true;
                            itemHL.GetComponent<UIGradient>().LinearColor1 = inventoryM.rank1Color1;
                            itemHL.GetComponent<UIGradient>().LinearColor2 = inventoryM.rank1Color2;
                            itemData.rankTag = "+";
                        }

                        if (itemData.rankNum == 2)
                        {
                            itemData.itemName += "+";
                            itemData.maxWpnAtk = itemData.wpnAtk.GetRank2Value();
                            itemData.maxWpnSpd = itemData.wpnSpd.GetRank2Value();
                            itemData.maxWpnStun = itemData.wpnStun.GetRank2Value();
                            itemData.maxWpnCritHit = itemData.wpnCritHit.GetRank2Value();
                            itemData.maxWpnDura = itemData.wpnDura.GetRank2Value();
                            statCount = itemData.maxWpnAtk;
                            statValue.text = statCount.ToString();
                            itemHL.GetComponent<UIGradient>().enabled = true;
                            itemHL.GetComponent<UIGradient>().LinearColor1 = inventoryM.rank2Color1;
                            itemHL.GetComponent<UIGradient>().LinearColor2 = inventoryM.rank2Color2;
                            itemData.rankTag = "++";
                        }

                        if (itemData.rankNum == 3)
                        {
                            itemData.itemName += "+";
                            itemData.maxWpnAtk = itemData.wpnAtk.GetRank3Value();
                            itemData.maxWpnSpd = itemData.wpnSpd.GetRank3Value();
                            itemData.maxWpnStun = itemData.wpnStun.GetRank3Value();
                            itemData.maxWpnCritHit = itemData.wpnCritHit.GetRank3Value();
                            itemData.maxWpnDura = itemData.wpnDura.GetRank3Value();
                            statCount = itemData.maxWpnAtk;
                            statValue.text = statCount.ToString();
                            itemHL.GetComponent<UIGradient>().enabled = true;
                            itemHL.GetComponent<UIGradient>().LinearColor1 = inventoryM.rank3Color1;
                            itemHL.GetComponent<UIGradient>().LinearColor2 = inventoryM.rank3Color2;
                            itemData.rankTag = "+++";
                        }

                        for (int ki = 0; ki < inventoryM.weaponInv.itemData.Length; ki++)
                        {
                            inventoryM.referencesObj.primaryItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                            inventoryM.referencesObj.primaryItemImage.enabled = true;
                            inventoryM.referencesObj.primaryItemImage.sprite = itemData.itemSprite;
                            inventoryM.referencesObj.primaryValueBG.enabled = true;
                            inventoryM.referencesObj.primaryValueBG.GetComponentInChildren<TextMeshProUGUI>().text = itemData.maxWpnAtk.ToString();

                            inventoryM.weaponInv.statValue[ki].text = itemData.maxWpnAtk.ToString();
                        }

                        inventoryM.storeM.upgradeM.CheckInventoryItems(ref inventoryM.weaponInv.itemData,
                        ref inventoryM.weaponInv.slotNum, ref inventoryM.weaponInv.counter);
                    }
                }
            }

            if (inventoryM.inventoryS == InventoryManager.InventorySection.BowAndArrow)
            {
                int slotNum = inventoryM.bowAndArrowInv.slotNum;
                inventoryM.bowAndArrowInv.slotBowEquipped = slotNum;
                wpnHolster.slotWeapon = slotNum;

                ItemData itemData = inventoryM.bowAndArrowInv.itemData[inventoryM.bowAndArrowInv.slotNum];
                Image itemHL = inventoryM.bowAndArrowInv.highLight[inventoryM.bowAndArrowInv.slotNum];
                Image outLineBorder = inventoryM.bowAndArrowInv.outLineBorder[inventoryM.bowAndArrowInv.slotNum];
                TextMeshProUGUI statValue = inventoryM.bowAndArrowInv.statValue[inventoryM.bowAndArrowInv.slotNum];
                int statCount = inventoryM.bowAndArrowInv.statCounter[inventoryM.bowAndArrowInv.slotNum];

                if (inventoryM.materialInv.image.Length == 0 || inventoryM.materialInv.image[0] == null)
                {
                    inventoryM.cc.infoMessage.info.text = "You don't have enough inventory space.";
                    outLineBorder.enabled = false;
                    dropDown.value = 0;
                    DestroyImmediate(dropDownObj);
                    gameObject.SetActive(false);
                    return;
                }

                if (inventoryM.bowAndArrowInv.itemData[slotNum].rankNum == 3)
                {
                    infoMessage.info.text = "This weapon is fully upgraded.";
                    outLineBorder.enabled = false;
                    dropDown.value = 0;
                    DestroyImmediate(dropDownObj);
                    gameObject.SetActive(false);
                    return;
                }

                for (int i = 0; i < inventoryM.bowAndArrowInv.itemData[slotNum].upgradeMaterial.Length; i++)
                {
                    if (inventoryM.storeM.upgradeM.CheckResources(ref itemData.upgradeMaterial[i].matName, ref itemData.upgradeMaterial[i].matRequired))
                    {
                        outLineBorder.enabled = false;
                        dropDown.value = 0;
                        DestroyImmediate(dropDownObj);
                        gameObject.SetActive(false);
                        return;
                    }
                }

                for (int i = 0; i < inventoryM.bowAndArrowInv.itemData[slotNum].upgradeMaterial.Length; i++)
                {
                    if (upgradeM.Upgrade(itemData.upgradeMaterial[i].matName, itemData.upgradeMaterial[i].matRequired, ref itemData)
                    && inventoryM.unityCoins >= itemData.upgradePrice)
                    {
                        inventoryM.unityCoins -= itemData.upgradePrice;

                        if (itemData.rankNum == 1)
                        {
                            itemData.originItemName = itemData.itemName;
                            itemData.itemName += "+";
                            itemData.maxBowAtk = itemData.bowAtk.GetRank3Value();
                            itemData.maxBowSpd = itemData.bowSpd.GetRank3Value();
                            itemData.maxBowStun = itemData.bowStun.GetRank3Value();
                            itemData.maxBowCritHit = itemData.bowCritHit.GetRank3Value();
                            itemData.maxBowDura = itemData.bowDura.GetRank3Value();
                            itemData.maxBowHdShot = itemData.bowHdShot.GetRank3Value();
                            statCount = itemData.maxBowAtk;
                            statValue.text = statCount.ToString();
                            itemHL.GetComponent<UIGradient>().enabled = true;
                            itemHL.GetComponent<UIGradient>().LinearColor1 = inventoryM.rank1Color1;
                            itemHL.GetComponent<UIGradient>().LinearColor2 = inventoryM.rank1Color2;
                            itemData.rankTag = "+";
                        }

                        if (itemData.rankNum == 2)
                        {
                            itemData.itemName += "+";
                            itemData.maxBowAtk = itemData.bowAtk.GetRank3Value();
                            itemData.maxBowSpd = itemData.bowSpd.GetRank3Value();
                            itemData.maxBowStun = itemData.bowStun.GetRank3Value();
                            itemData.maxBowCritHit = itemData.bowCritHit.GetRank3Value();
                            itemData.maxBowDura = itemData.bowDura.GetRank3Value();
                            itemData.maxBowHdShot = itemData.bowHdShot.GetRank3Value();
                            statCount = itemData.maxBowAtk;
                            statValue.text = statCount.ToString();
                            itemHL.GetComponent<UIGradient>().enabled = true;
                            itemHL.GetComponent<UIGradient>().LinearColor1 = inventoryM.rank3Color1;
                            itemHL.GetComponent<UIGradient>().LinearColor2 = inventoryM.rank3Color2;
                            itemData.rankTag = "++";
                        }

                        if (itemData.rankNum == 3)
                        {
                            itemData.itemName += "+";
                            itemData.maxBowAtk = itemData.bowAtk.GetRank3Value();
                            itemData.maxBowSpd = itemData.bowSpd.GetRank3Value();
                            itemData.maxBowStun = itemData.bowStun.GetRank3Value();
                            itemData.maxBowCritHit = itemData.bowCritHit.GetRank3Value();
                            itemData.maxBowDura = itemData.bowDura.GetRank3Value();
                            itemData.maxBowHdShot = itemData.bowHdShot.GetRank3Value();
                            statCount = itemData.maxBowAtk;
                            statValue.text = statCount.ToString();
                            itemHL.GetComponent<UIGradient>().enabled = true;
                            itemHL.GetComponent<UIGradient>().LinearColor1 = inventoryM.rank3Color1;
                            itemHL.GetComponent<UIGradient>().LinearColor2 = inventoryM.rank3Color2;
                            itemData.rankTag = "+++";
                        }

                        for (int ki = 0; ki < inventoryM.bowAndArrowInv.itemData.Length; ki++)
                        {
                            inventoryM.referencesObj.secondaryItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                            inventoryM.referencesObj.secondaryItemImage.enabled = true;
                            inventoryM.referencesObj.secondaryItemImage.sprite = itemData.itemSprite;
                            inventoryM.referencesObj.secondaryValueBG.enabled = true;
                            inventoryM.referencesObj.secondaryValueBG.GetComponentInChildren<TextMeshProUGUI>().text = itemData.maxWpnAtk.ToString();

                            inventoryM.bowAndArrowInv.statValue[ki].text = itemData.maxWpnAtk.ToString();
                        }

                        inventoryM.storeM.upgradeM.CheckInventoryItems(ref inventoryM.bowAndArrowInv.itemData,
                        ref inventoryM.bowAndArrowInv.slotNum, ref inventoryM.bowAndArrowInv.counter);
                    }
                }
            }

            if (inventoryM.inventoryS == InventoryManager.InventorySection.Shield)
            {
                int slotNum = inventoryM.shieldInv.slotNum;
                inventoryM.shieldInv.slotShieldEquipped = slotNum;
                wpnHolster.slotWeapon = slotNum;

                ItemData itemData = inventoryM.shieldInv.itemData[inventoryM.shieldInv.slotNum];
                Image itemHL = inventoryM.shieldInv.highLight[inventoryM.shieldInv.slotNum];
                Image outLineBorder = inventoryM.shieldInv.outLineBorder[inventoryM.shieldInv.slotNum];
                TextMeshProUGUI statValue = inventoryM.shieldInv.statValue[inventoryM.shieldInv.slotNum];
                int statCount = inventoryM.shieldInv.statCounter[inventoryM.shieldInv.slotNum];

                if (inventoryM.materialInv.image.Length == 0 || inventoryM.materialInv.image[0] == null)
                {
                    inventoryM.cc.infoMessage.info.text = "You don't have enough inventory space.";
                    outLineBorder.enabled = false;
                    dropDown.value = 0;
                    DestroyImmediate(dropDownObj);
                    gameObject.SetActive(false);
                    return;
                }

                if (inventoryM.shieldInv.itemData[slotNum].rankNum == 3)
                {
                    infoMessage.info.text = "This weapon is fully upgraded.";
                    outLineBorder.enabled = false;
                    dropDown.value = 0;
                    DestroyImmediate(dropDownObj);
                    gameObject.SetActive(false);
                    return;
                }

                for (int i = 0; i < inventoryM.shieldInv.itemData[slotNum].upgradeMaterial.Length; i++)
                {
                    if (inventoryM.storeM.upgradeM.CheckResources(ref itemData.upgradeMaterial[i].matName, ref itemData.upgradeMaterial[i].matRequired))
                    {
                        outLineBorder.enabled = false;
                        dropDown.value = 0;
                        DestroyImmediate(dropDownObj);
                        gameObject.SetActive(false);
                        return;
                    }
                }

                for (int i = 0; i < inventoryM.shieldInv.itemData[slotNum].upgradeMaterial.Length; i++)
                {
                    if (upgradeM.Upgrade(itemData.upgradeMaterial[i].matName, itemData.upgradeMaterial[i].matRequired, ref itemData)
                    && inventoryM.unityCoins >= itemData.upgradePrice)
                    {
                        inventoryM.unityCoins -= itemData.upgradePrice;

                        if (itemData.rankNum == 1)
                        {
                            itemData.originItemName = itemData.itemName;
                            itemData.itemName += "+";
                            itemData.maxShdAtk = itemData.shdAtk.GetRank1Value();
                            itemData.maxShdSpd = itemData.shdSpd.GetRank1Value();
                            itemData.maxShdBlock = itemData.shdBlock.GetRank1Value();
                            itemData.maxShdStun = itemData.shdStun.GetRank1Value();
                            itemData.maxShdCritHit = itemData.shdCritHit.GetRank1Value();
                            itemData.maxShdDura = itemData.shdDura.GetRank1Value();
                            statCount = itemData.maxShdAtk;
                            statValue.text = statCount.ToString();
                            itemHL.GetComponent<UIGradient>().enabled = true;
                            itemHL.GetComponent<UIGradient>().LinearColor1 = inventoryM.rank1Color1;
                            itemHL.GetComponent<UIGradient>().LinearColor2 = inventoryM.rank1Color2;
                            itemData.rankTag = "+";
                        }

                        if (itemData.rankNum == 2)
                        {
                            itemData.itemName += "+";
                            itemData.maxShdAtk = itemData.shdAtk.GetRank2Value();
                            itemData.maxShdSpd = itemData.shdSpd.GetRank2Value();
                            itemData.maxShdBlock = itemData.shdBlock.GetRank2Value();
                            itemData.maxShdStun = itemData.shdStun.GetRank2Value();
                            itemData.maxShdCritHit = itemData.shdCritHit.GetRank2Value();
                            itemData.maxShdDura = itemData.shdDura.GetRank2Value();
                            statCount = itemData.maxShdAtk;
                            statValue.text = statCount.ToString();
                            itemHL.GetComponent<UIGradient>().enabled = true;
                            itemHL.GetComponent<UIGradient>().LinearColor1 = inventoryM.rank2Color1;
                            itemHL.GetComponent<UIGradient>().LinearColor2 = inventoryM.rank2Color2;
                            itemData.rankTag = "++";
                        }

                        if (itemData.rankNum == 3)
                        {
                            itemData.itemName += "+";
                            itemData.maxShdAtk = itemData.shdAtk.GetRank3Value();
                            itemData.maxShdSpd = itemData.shdSpd.GetRank3Value();
                            itemData.maxShdBlock = itemData.shdBlock.GetRank3Value();
                            itemData.maxShdStun = itemData.shdStun.GetRank3Value();
                            itemData.maxShdCritHit = itemData.shdCritHit.GetRank3Value();
                            itemData.maxShdDura = itemData.shdDura.GetRank3Value();
                            statCount = itemData.maxShdAtk;
                            statValue.text = statCount.ToString();
                            itemHL.GetComponent<UIGradient>().enabled = true;
                            itemHL.GetComponent<UIGradient>().LinearColor1 = inventoryM.rank3Color1;
                            itemHL.GetComponent<UIGradient>().LinearColor2 = inventoryM.rank3Color2;
                            itemData.rankTag = "+++";
                        }

                        for (int ki = 0; ki < inventoryM.shieldInv.itemData.Length; ki++)
                        {
                            inventoryM.referencesObj.shieldItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                            inventoryM.referencesObj.shieldItemImage.enabled = true;
                            inventoryM.referencesObj.shieldItemImage.sprite = itemData.itemSprite;
                            inventoryM.referencesObj.shieldValueBG.enabled = true;
                            inventoryM.referencesObj.shieldValueBG.GetComponentInChildren<TextMeshProUGUI>().text = itemData.maxShdAtk.ToString();

                            inventoryM.shieldInv.statValue[ki].text = itemData.maxShdAtk.ToString();
                        }

                        inventoryM.storeM.upgradeM.CheckInventoryItems(ref inventoryM.shieldInv.itemData,
                        ref inventoryM.shieldInv.slotNum, ref inventoryM.shieldInv.counter);
                    }
                }
            }

            if (inventoryM.inventoryS == InventoryManager.InventorySection.Armor)
            {
                int slotNum = inventoryM.armorInv.slotNum;
                inventoryM.armorInv.slotNum = slotNum;
                wpnHolster.slotWeapon = slotNum;

                ItemData itemData = inventoryM.armorInv.itemData[inventoryM.armorInv.slotNum];
                Image itemHL = inventoryM.armorInv.highLight[inventoryM.armorInv.slotNum];
                Image outLineBorder = inventoryM.armorInv.outLineBorder[inventoryM.armorInv.slotNum];
                TextMeshProUGUI statValue = inventoryM.armorInv.statValue[inventoryM.armorInv.slotNum];
                int statCount = inventoryM.armorInv.statCounter[inventoryM.armorInv.slotNum];

                if (inventoryM.materialInv.image.Length == 0 || inventoryM.materialInv.image[0] == null)
                {
                    inventoryM.cc.infoMessage.info.text = "You don't have enough inventory space.";
                    outLineBorder.enabled = false;
                    dropDown.value = 0;
                    DestroyImmediate(dropDownObj);
                    gameObject.SetActive(false);
                    return;
                }

                if (inventoryM.armorInv.itemData[slotNum].rankNum == 3)
                {
                    infoMessage.info.text = "This weapon is fully upgraded.";
                    outLineBorder.enabled = false;
                    dropDown.value = 0;
                    DestroyImmediate(dropDownObj);
                    gameObject.SetActive(false);
                    return;
                }

                for (int i = 0; i < inventoryM.armorInv.itemData[slotNum].upgradeMaterial.Length; i++)
                {
                    if (inventoryM.storeM.upgradeM.CheckResources(ref itemData.upgradeMaterial[i].matName, ref itemData.upgradeMaterial[i].matRequired))
                    {
                        outLineBorder.enabled = false;
                        dropDown.value = 0;
                        DestroyImmediate(dropDownObj);
                        gameObject.SetActive(false);
                        return;
                    }
                }

                for (int i = 0; i < inventoryM.armorInv.itemData[slotNum].upgradeMaterial.Length; i++)
                {
                    if (upgradeM.Upgrade(itemData.upgradeMaterial[i].matName, itemData.upgradeMaterial[i].matRequired, ref itemData)
                    && inventoryM.unityCoins >= itemData.upgradePrice)
                    {
                        inventoryM.unityCoins -= itemData.upgradePrice;

                        if (itemData.rankNum == 1)
                        {
                            itemData.originItemName = itemData.itemName;
                            itemData.itemName += "+";
                            itemData.maxArm = itemData.arm.GetRank1Value();
                            itemData.maxArmHRes = itemData.armHRes.GetRank1Value();
                            itemData.maxArmLRes = itemData.armLRes.GetRank1Value();
                            statCount = itemData.maxArm;
                            statValue.text = statCount.ToString();
                            itemHL.GetComponent<UIGradient>().enabled = true;
                            itemHL.GetComponent<UIGradient>().LinearColor1 = inventoryM.rank1Color1;
                            itemHL.GetComponent<UIGradient>().LinearColor2 = inventoryM.rank1Color2;
                            itemData.rankTag = "+";
                        }

                        if (itemData.rankNum == 2)
                        {
                            itemData.itemName += "+";
                            itemData.maxArm = itemData.arm.GetRank2Value();
                            itemData.maxArmHRes = itemData.armHRes.GetRank2Value();
                            itemData.maxArmLRes = itemData.armLRes.GetRank2Value();
                            statCount = itemData.maxArm;
                            statValue.text = statCount.ToString();
                            itemHL.GetComponent<UIGradient>().enabled = true;
                            itemHL.GetComponent<UIGradient>().LinearColor1 = inventoryM.rank2Color1;
                            itemHL.GetComponent<UIGradient>().LinearColor2 = inventoryM.rank2Color2;
                            itemData.rankTag = "++";
                        }

                        if (itemData.rankNum == 3)
                        {
                            itemData.itemName += "+";
                            itemData.maxArm = itemData.arm.GetRank3Value();
                            itemData.maxArmHRes = itemData.armHRes.GetRank3Value();
                            itemData.maxArmLRes = itemData.armLRes.GetRank3Value();
                            statCount = itemData.maxArm;
                            statValue.text = statCount.ToString();
                            itemHL.GetComponent<UIGradient>().enabled = true;
                            itemHL.GetComponent<UIGradient>().LinearColor1 = inventoryM.rank3Color1;
                            itemHL.GetComponent<UIGradient>().LinearColor2 = inventoryM.rank3Color2;
                            itemData.rankTag = "+++";
                        }

                        for (int ki = 0; ki < inventoryM.armorInv.itemData.Length; ki++)
                        {
                            if (inventoryM.armorInv.itemData[ki].arm.armorType == ItemData.ArmorType.Head)
                            {
                                inventoryM.referencesObj.headItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                                inventoryM.referencesObj.headItemImage.enabled = true;
                                inventoryM.referencesObj.headItemImage.sprite = itemData.itemSprite;
                                inventoryM.referencesObj.headValueBG.enabled = true;
                                inventoryM.referencesObj.headValueBG.GetComponentInChildren<TextMeshProUGUI>().text = itemData.maxArm.ToString();

                                inventoryM.armorInv.statValue[ki].text = itemData.maxArm.ToString();
                            }

                            if (inventoryM.armorInv.itemData[ki].arm.armorType == ItemData.ArmorType.Chest)
                            {
                                inventoryM.referencesObj.chestItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                                inventoryM.referencesObj.chestItemImage.enabled = true;
                                inventoryM.referencesObj.chestItemImage.sprite = itemData.itemSprite;
                                inventoryM.referencesObj.chestValueBG.enabled = true;
                                inventoryM.referencesObj.chestValueBG.GetComponentInChildren<TextMeshProUGUI>().text = itemData.maxArm.ToString();

                                inventoryM.armorInv.statValue[ki].text = itemData.maxArm.ToString();
                            }

                            if (inventoryM.armorInv.itemData[ki].arm.armorType == ItemData.ArmorType.Legs)
                            {
                                inventoryM.referencesObj.legItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                                inventoryM.referencesObj.legItemImage.enabled = true;
                                inventoryM.referencesObj.legItemImage.sprite = itemData.itemSprite;
                                inventoryM.referencesObj.legValueBG.enabled = true;
                                inventoryM.referencesObj.legValueBG.GetComponentInChildren<TextMeshProUGUI>().text = itemData.maxArm.ToString();

                                inventoryM.armorInv.statValue[ki].text = itemData.maxArm.ToString();
                            }
                        }

                        inventoryM.storeM.upgradeM.CheckInventoryItems(ref inventoryM.armorInv.itemData,
                        ref inventoryM.armorInv.slotNum, ref inventoryM.armorInv.counter);
                    }
                }
            }
            if (upgradeAS)
                upgradeAS.PlayRandomClip();

            upgradeM.oneShot = false;

            dropDown.value = 0;
            DestroyImmediate(dropDownObj);
            gameObject.SetActive(false);
        }

        if (index == 2)
        {
            if (inventoryM.inventoryS == InventoryManager.InventorySection.Weapon)
            {
                Image outLineBorder = inventoryM.weaponInv.outLineBorder[inventoryM.weaponInv.slotNum];

                outLineBorder.enabled = false;
            }

            if (inventoryM.inventoryS == InventoryManager.InventorySection.BowAndArrow)
            {
                Image outLineBorder = inventoryM.bowAndArrowInv.outLineBorder[inventoryM.bowAndArrowInv.slotNum];

                outLineBorder.enabled = false;
            }

            if (inventoryM.inventoryS == InventoryManager.InventorySection.Shield)
            {
                Image outLineBorder = inventoryM.shieldInv.outLineBorder[inventoryM.shieldInv.slotNum];

                outLineBorder.enabled = false;
            }

            if (inventoryM.inventoryS == InventoryManager.InventorySection.Armor)
            {
                Image outLineBorder = inventoryM.armorInv.outLineBorder[inventoryM.armorInv.slotNum];

                outLineBorder.enabled = false;
            }

            if (cancelAS)
                cancelAS.PlayRandomClip();

            dropDown.value = 0;
            DestroyImmediate(dropDownObj);
            gameObject.SetActive(false);
        }
    }
}
