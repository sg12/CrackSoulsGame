using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ItemDescription : MonoBehaviour 
{
    [Header("REFERENCES")]
    public FadeUI fadeUI;

    public TextMeshProUGUI
    itemName,
    stat1Name, stat2Name, stat3Name, stat4Name, stat5Name, stat6Name,
    stat7Name, statSpecial1Name, statSpecial2Name, 
    statSpecial1Info, statSpecial2Info,
    stat1Value, stat2Value, stat3Value, stat4Value, stat5Value, 
    stat6Value, stat7Value, itemDescription;

    #region Private

    public bool refreshDescription;
    private InventoryManager inventoryM;
    public InventoryDescription inventoryD;

    private Vector2 description1Spot = new Vector2(-79.084f, 58.5f);
    private Vector2 description2Spot = new Vector2(-79.084f, -13.6f);
    private Vector2 description3Spot = new Vector2(-79.084f, -78.13712f);

    #endregion

    // Use this for initialization
    void Awake () 
    {
        // Get Components
        inventoryM = FindObjectOfType<InventoryManager>();
        fadeUI = GetComponent<FadeUI>();
        fadeUI.canvasGroup.alpha = 0;
	}
	
	// Update is called once per frame
	void Update () 
    {
        switch (inventoryD)
        {
            case InventoryDescription.Weapon:
                WeaponDescriptionSettings();
                break;
            case InventoryDescription.Bow:
                BowDescriptionSettings();
                break;
            case InventoryDescription.Arrow:
                ArrowDescriptionSettings();
                break;
            case InventoryDescription.Shield:
                ShieldDescriptionSettings();
                break;
            case InventoryDescription.Armor:
                ArmorDescriptionSettings();
                break;
            case InventoryDescription.Material:
                MaterialDescriptionSettings();
                break;
            case InventoryDescription.Healing:
                HealingDescriptionSettings();
                break;
            case InventoryDescription.Key:
                KeyDescriptionSettings();
                break;
            case InventoryDescription.Null:
                if (fadeUI.canvasGroup.alpha == 1)
                    fadeUI.FadeTransition(0, 0, 0.2f);
                break;
        }
    }

    void WeaponDescriptionSettings()
    {
        if (!inventoryM.weaponInv.image[inventoryM.weaponInv.slotNum].enabled)
            return;

        fadeUI.FadeTransition(1, 0, 0.2f);

        ItemData itemData = inventoryM.weaponInv.itemData[inventoryM.weaponInv.slotNum];

        itemName.text = itemData.itemName;

        stat1Name.text = "Atk:";
        stat1Value.text = itemData.maxWpnAtk.ToString();

        stat2Name.text = "Spd:";
        stat2Value.text = itemData.maxWpnSpd.ToString();

        stat3Name.text = "Stun:";
        stat3Value.text = itemData.maxWpnStun.ToString();

        stat4Name.text = "Crit-Hit:";
        stat4Value.text = itemData.maxWpnCritHit.ToString();

        stat5Name.text = "Dura:";
        stat5Value.text = itemData.maxWpnDura.ToString();

        stat6Name.text = "Wgt:";
        stat6Value.text = itemData.wpnWeight.ToString();

        stat7Name.GetComponent<TextMeshProUGUI>().gameObject.SetActive(false);

        if (itemData.wpnSpecialStat1 == ItemData.WeaponSpecialStat1.None)
        {
            statSpecial1Name.GetComponent<TextMeshProUGUI>().text = "";
            statSpecial1Info.GetComponent<TextMeshProUGUI>().text = "";
        }
        else
        {
            statSpecial1Name.GetComponent<TextMeshProUGUI>().text = itemData.wpnSpecialStatName1;
            statSpecial1Info.GetComponent<TextMeshProUGUI>().text = itemData.wpnSpecialStatInfo1;
        }

        if (itemData.wpnSpecialStat2 == ItemData.WeaponSpecialStat2.None)
        {
            statSpecial2Name.GetComponent<TextMeshProUGUI>().text = "";
            statSpecial2Info.GetComponent<TextMeshProUGUI>().text = "";
        }
        else
        {
            statSpecial2Name.GetComponent<TextMeshProUGUI>().text = itemData.wpnSpecialStatName2;
            statSpecial2Info.GetComponent<TextMeshProUGUI>().text = itemData.wpnSpecialStatInfo2;
        }

        if (itemData.wpnSpecialStat1 == ItemData.WeaponSpecialStat1.None && itemData.wpnSpecialStat2 == ItemData.WeaponSpecialStat2.None)
            itemDescription.GetComponent<RectTransform>().anchoredPosition = description2Spot;
        else if (itemData.wpnSpecialStat1 != ItemData.WeaponSpecialStat1.None && itemData.wpnSpecialStat2 != ItemData.WeaponSpecialStat2.None)
            itemDescription.GetComponent<RectTransform>().anchoredPosition = description3Spot;
        else
            itemDescription.GetComponent<RectTransform>().anchoredPosition = description3Spot;

        itemDescription.text = itemData.itemDescription;

        if (!itemData.equipped)
            inventoryM.itemEquippedCheck.color = inventoryM.outLineHover;
        else
            inventoryM.itemEquippedCheck.color = inventoryM.outLineSelected;
    }

    void BowDescriptionSettings()
    {
        if (!inventoryM.bowAndArrowInv.image[inventoryM.bowAndArrowInv.slotNum].enabled)
            return;

        fadeUI.FadeTransition(1, 0, 0.2f);

        ItemData itemData = inventoryM.bowAndArrowInv.itemData[inventoryM.bowAndArrowInv.slotNum];

        itemName.text = itemData.itemName;

        stat1Name.text = "Atk:";
        stat1Value.text = itemData.maxBowAtk.ToString();

        stat2Name.text = "Spd:";
        stat2Value.text = itemData.maxBowSpd.ToString();

        stat3Name.text = "Stun:";
        stat3Value.text = itemData.maxBowStun.ToString();

        stat4Name.text = "Crit-Hit:";
        stat4Value.text = itemData.maxBowCritHit.ToString();

        stat5Name.text = "Dura:";
        stat5Value.text = itemData.maxBowDura.ToString();

        stat6Name.text = "Wgt:";
        stat6Value.text = itemData.bowWeight.ToString();

        stat7Name.GetComponent<TextMeshProUGUI>().gameObject.SetActive(false);

        if (itemData.bowSpecialStat1 == ItemData.BowSpecialStat1.None)
        {
            statSpecial1Name.GetComponent<TextMeshProUGUI>().text = "";
            statSpecial1Info.GetComponent<TextMeshProUGUI>().text = "";
        }
        else
        {
            statSpecial1Name.GetComponent<TextMeshProUGUI>().text = itemData.bowSpecialStatName1;
            statSpecial1Info.GetComponent<TextMeshProUGUI>().text = itemData.bowSpecialStatInfo1;
        }

        if (itemData.bowSpecialStat2 == ItemData.BowSpecialStat2.None)
        {
            statSpecial2Name.GetComponent<TextMeshProUGUI>().text = "";
            statSpecial2Info.GetComponent<TextMeshProUGUI>().text = "";
        }
        else
        {
            statSpecial2Name.GetComponent<TextMeshProUGUI>().text = itemData.bowSpecialStatName2;
            statSpecial2Info.GetComponent<TextMeshProUGUI>().text = itemData.bowSpecialStatInfo2;
        }

        if (itemData.bowSpecialStat1 == ItemData.BowSpecialStat1.None && itemData.bowSpecialStat2 == ItemData.BowSpecialStat2.None)
            itemDescription.GetComponent<RectTransform>().anchoredPosition = description2Spot;
        else if (itemData.bowSpecialStat1 != ItemData.BowSpecialStat1.None && itemData.bowSpecialStat2 != ItemData.BowSpecialStat2.None)
            itemDescription.GetComponent<RectTransform>().anchoredPosition = description3Spot;
        else
            itemDescription.GetComponent<RectTransform>().anchoredPosition = description3Spot;

        itemDescription.text = itemData.itemDescription;

        if (!itemData.equipped)
            inventoryM.itemEquippedCheck.color = inventoryM.outLineHover;
        else
            inventoryM.itemEquippedCheck.color = inventoryM.outLineSelected;
    }

    void ArrowDescriptionSettings()
    {
        if (!inventoryM.bowAndArrowInv.image[inventoryM.bowAndArrowInv.slotNum].enabled)
            return;

        fadeUI.FadeTransition(1, 0, 0.2f);

        ItemData itemData = inventoryM.bowAndArrowInv.itemData[inventoryM.bowAndArrowInv.slotNum];

        itemName.text = itemData.itemName;

        stat1Name.text = "";
        stat1Value.text = "";

        stat2Name.text = "";
        stat2Value.text = "";

        stat3Name.text = "";
        stat3Value.text = "";

        stat4Name.text = "";
        stat4Value.text = "";

        stat5Name.text = "";
        stat5Value.text = "";

        stat6Name.text = "";
        stat6Value.text = "";

        stat7Name.GetComponent<TextMeshProUGUI>().gameObject.SetActive(false);

        statSpecial1Name.GetComponent<TextMeshProUGUI>().text = "";
        statSpecial1Info.GetComponent<TextMeshProUGUI>().text = "";

        statSpecial2Name.GetComponent<TextMeshProUGUI>().text = "";
        statSpecial2Info.GetComponent<TextMeshProUGUI>().text = "";

        itemDescription.GetComponent<RectTransform>().anchoredPosition = description1Spot;

        itemDescription.text = itemData.itemDescription;

        if (!itemData.equipped)
            inventoryM.itemEquippedCheck.color = inventoryM.outLineHover;
        else
            inventoryM.itemEquippedCheck.color = inventoryM.outLineSelected;
    }

    void ShieldDescriptionSettings()
    {
        if (!inventoryM.shieldInv.image[inventoryM.shieldInv.slotNum].enabled)
            return;

        fadeUI.FadeTransition(1, 0, 0.2f);

        ItemData itemData = inventoryM.shieldInv.itemData[inventoryM.shieldInv.slotNum];

        itemName.text = itemData.itemName;

        stat1Name.text = "Atk:";
        stat1Value.text = itemData.maxShdAtk.ToString();

        stat2Name.text = "Spd:";
        stat2Value.text = itemData.maxShdSpd.ToString();

        stat3Name.text = "Block:";
        stat3Value.text = itemData.maxShdBlock.ToString();

        stat4Name.text = "Stun:";
        stat3Value.text = itemData.maxShdStun.ToString();

        stat5Name.text = "Crit-Hit:";
        stat4Value.text = itemData.maxShdCritHit.ToString();

        stat6Name.text = "Dura:";
        stat6Value.text = itemData.maxShdDura.ToString();

        stat7Name.GetComponent<TextMeshProUGUI>().gameObject.SetActive(true);
        stat7Name.text = "Wgt:";
        stat7Value.text = itemData.shdWeight.ToString();

        if (itemData.shdSpecialStat1 == ItemData.ShieldSpecialStat1.None)
        {
            statSpecial1Name.GetComponent<TextMeshProUGUI>().text = "";
            statSpecial1Info.GetComponent<TextMeshProUGUI>().text = "";
        }
        else
        {
            statSpecial1Name.GetComponent<TextMeshProUGUI>().text = itemData.shdSpecialStatName1;
            statSpecial1Info.GetComponent<TextMeshProUGUI>().text = itemData.shdSpecialStatInfo1;
        }

        if (itemData.shdSpecialStat2 == ItemData.ShieldSpecialStat2.None)
        {
            statSpecial2Name.GetComponent<TextMeshProUGUI>().text = "";
            statSpecial2Info.GetComponent<TextMeshProUGUI>().text = "";
        }
        else
        {
            statSpecial2Name.GetComponent<TextMeshProUGUI>().text = itemData.shdSpecialStatName2;
            statSpecial2Info.GetComponent<TextMeshProUGUI>().text = itemData.shdSpecialStatInfo2;
        }

        if (itemData.shdSpecialStat1 == ItemData.ShieldSpecialStat1.None && itemData.shdSpecialStat2 == ItemData.ShieldSpecialStat2.None)
            itemDescription.GetComponent<RectTransform>().anchoredPosition = description2Spot;
        else if (itemData.shdSpecialStat1 != ItemData.ShieldSpecialStat1.None && itemData.shdSpecialStat2 != ItemData.ShieldSpecialStat2.None)
            itemDescription.GetComponent<RectTransform>().anchoredPosition = description3Spot;
        else
            itemDescription.GetComponent<RectTransform>().anchoredPosition = description3Spot;

        itemDescription.text = itemData.itemDescription;

        if (!itemData.equipped)
            inventoryM.itemEquippedCheck.color = inventoryM.outLineHover;
        else
            inventoryM.itemEquippedCheck.color = inventoryM.outLineSelected;
    }

    void ArmorDescriptionSettings()
    {
        if (!inventoryM.armorInv.image[inventoryM.armorInv.slotNum].enabled)
            return;

        fadeUI.FadeTransition(1, 0, 0.2f);

        ItemData itemData = inventoryM.armorInv.itemData[inventoryM.armorInv.slotNum];

        itemName.text = itemData.itemName;

        stat1Name.text = "Arm:";
        stat1Value.text = itemData.maxArm.ToString();

        stat2Name.text = "L-Res:";
        stat2Value.text = itemData.maxArmLRes.ToString();

        stat3Name.text = "H-Res:";
        stat3Value.text = itemData.maxArmHRes.ToString();

        stat4Name.text = "Wgt:";
        stat4Value.text = itemData.armWeight.ToString();

        stat5Name.GetComponent<TextMeshProUGUI>().gameObject.SetActive(false);
        stat6Name.GetComponent<TextMeshProUGUI>().gameObject.SetActive(false);
        stat7Name.GetComponent<TextMeshProUGUI>().gameObject.SetActive(false);

        if (itemData.armSpecialStat1 == ItemData.ArmorSpecialStat1.None)
        {
            statSpecial1Name.GetComponent<TextMeshProUGUI>().text = "";
            statSpecial1Info.GetComponent<TextMeshProUGUI>().text = "";
        }
        else
        {
            statSpecial1Name.GetComponent<TextMeshProUGUI>().text = itemData.armSpecialStatName1;
            statSpecial1Info.GetComponent<TextMeshProUGUI>().text = itemData.armSpecialStatInfo1;
        }

        if (itemData.armSpecialStat2 == ItemData.ArmorSpecialStat2.None)
        {
            statSpecial2Name.GetComponent<TextMeshProUGUI>().text = "";
            statSpecial2Info.GetComponent<TextMeshProUGUI>().text = "";
        }
        else
        {
            statSpecial2Name.GetComponent<TextMeshProUGUI>().text = itemData.armSpecialStatName2;
            statSpecial2Info.GetComponent<TextMeshProUGUI>().text = itemData.armSpecialStatInfo2;
        }

        if (itemData.armSpecialStat1 == ItemData.ArmorSpecialStat1.None && itemData.armSpecialStat2 == ItemData.ArmorSpecialStat2.None)
            itemDescription.GetComponent<RectTransform>().anchoredPosition = description2Spot;
        else if (itemData.armSpecialStat1 != ItemData.ArmorSpecialStat1.None && itemData.armSpecialStat2 != ItemData.ArmorSpecialStat2.None)
            itemDescription.GetComponent<RectTransform>().anchoredPosition = description3Spot;
        else
            itemDescription.GetComponent<RectTransform>().anchoredPosition = description3Spot;

        itemDescription.text = itemData.itemDescription;

        if (!itemData.equipped)
            inventoryM.itemEquippedCheck.color = inventoryM.outLineHover;
        else
            inventoryM.itemEquippedCheck.color = inventoryM.outLineSelected;
    }

    void MaterialDescriptionSettings()
    {
        if (!inventoryM.materialInv.image[inventoryM.materialInv.slotNum].enabled)
            return;

        fadeUI.FadeTransition(1, 0, 0.2f);

        ItemData itemData = inventoryM.materialInv.itemData[inventoryM.materialInv.slotNum];

        itemName.text = itemData.itemName;

        stat1Name.text = "";
        stat1Value.text = "";

        stat2Name.text = "";
        stat2Value.text = "";

        stat3Name.text = "";
        stat3Value.text = "";

        stat4Name.text = "";
        stat4Value.text = "";

        stat5Name.text = "";
        stat5Value.text = "";

        stat6Name.text = "";
        stat6Value.text = "";

        stat7Name.GetComponent<TextMeshProUGUI>().gameObject.SetActive(false);

        statSpecial1Name.GetComponent<TextMeshProUGUI>().text = "";
        statSpecial1Info.GetComponent<TextMeshProUGUI>().text = "";

        statSpecial2Name.GetComponent<TextMeshProUGUI>().text = "";
        statSpecial2Info.GetComponent<TextMeshProUGUI>().text = "";

        itemDescription.GetComponent<RectTransform>().anchoredPosition = description1Spot;

        itemDescription.text = itemData.itemDescription;
    }

    void HealingDescriptionSettings()
    {
        if (!inventoryM.healingInv.image[inventoryM.healingInv.slotNum].enabled)
            return;

        fadeUI.FadeTransition(1, 0, 0.2f);

        ItemData itemData = inventoryM.healingInv.itemData[inventoryM.healingInv.slotNum];

        itemName.text = itemData.itemName;

        stat1Name.text = "";
        stat1Value.text = "";

        stat2Name.text = "";
        stat2Value.text = "";

        stat3Name.text = "";
        stat3Value.text = "";

        stat4Name.text = "";
        stat4Value.text = "";

        stat5Name.text = "";
        stat5Value.text = "";

        stat6Name.text = "";
        stat6Value.text = "";

        stat7Name.GetComponent<TextMeshProUGUI>().gameObject.SetActive(false);

        statSpecial1Name.GetComponent<TextMeshProUGUI>().text = "";
        statSpecial1Info.GetComponent<TextMeshProUGUI>().text = "";

        statSpecial2Name.GetComponent<TextMeshProUGUI>().text = "";
        statSpecial2Info.GetComponent<TextMeshProUGUI>().text = "";

        itemDescription.GetComponent<RectTransform>().anchoredPosition = description1Spot;

        itemDescription.text = itemData.itemDescription;
    }

    void KeyDescriptionSettings()
    {
        if (!inventoryM.keyInv.image[inventoryM.keyInv.slotNum].enabled)
            return;

        fadeUI.FadeTransition(1, 0, 0.2f);

        ItemData itemData = inventoryM.keyInv.itemData[inventoryM.keyInv.slotNum];

        itemName.text = itemData.itemName;

        stat1Name.text = "";
        stat1Value.text = "";

        stat2Name.text = "";
        stat2Value.text = "";

        stat3Name.text = "";
        stat3Value.text = "";

        stat4Name.text = "";
        stat4Value.text = "";

        stat5Name.text = "";
        stat5Value.text = "";

        stat6Name.text = "";
        stat6Value.text = "";

        stat7Name.GetComponent<TextMeshProUGUI>().gameObject.SetActive(false);

        statSpecial1Name.GetComponent<TextMeshProUGUI>().text = "";
        statSpecial1Info.GetComponent<TextMeshProUGUI>().text = "";

        statSpecial2Name.GetComponent<TextMeshProUGUI>().text = "";
        statSpecial2Info.GetComponent<TextMeshProUGUI>().text = "";

        itemDescription.GetComponent<RectTransform>().anchoredPosition = description1Spot;

        itemDescription.text = itemData.itemDescription;
    }

    public enum InventoryDescription
    {
        Null,
        Weapon,
        Bow,
        Arrow,
        Shield,
        Armor,
        Material,
        Healing,
        Key,
    }
}
