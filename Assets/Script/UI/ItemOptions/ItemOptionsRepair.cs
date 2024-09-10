using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using JoshH.UI;

public class ItemOptionsRepair : MonoBehaviour
{
    readonly List<string> names = new List<string>() { "Choose:", "Repair", "Cancel" };

    [Header("AUDIO")]
    public RandomAudioPlayer repairAS;
    public RandomAudioPlayer cancelAS;

    #region Private 

    private TMP_Dropdown dropDown;
    private TextMeshProUGUI selectedName;
    private GameObject dropDownObj;
    private InfoMessage infoMessage;
    private InventoryManager inventoryM;
    private WeaponHolster wpnHolster;

    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        // Get Components
        wpnHolster = GameObject.Find("PlayerHolster").GetComponent<WeaponHolster>();
        dropDown = GetComponent<TMP_Dropdown>();
        infoMessage = FindObjectOfType<InfoMessage>();
        inventoryM = FindObjectOfType<InventoryManager>();
        selectedName = GameObject.Find("RepairLabel").GetComponent<TextMeshProUGUI>();
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
            if (inventoryM.inventoryS == InventoryManager.InventorySection.Weapon)
            {
                int slotNum = inventoryM.weaponInv.slotNum;
                inventoryM.weaponInv.slotWeaponEquipped = slotNum;
                wpnHolster.slotWeapon = slotNum;

                ItemData itemData = inventoryM.weaponInv.itemData[inventoryM.weaponInv.slotNum];
                Image itemHL = inventoryM.weaponInv.highLight[inventoryM.weaponInv.slotNum];

                if (itemData.broken)
                {
                    if (inventoryM.unityCoins >= itemData.repairPrice)
                    {
                        inventoryM.unityCoins -= itemData.repairPrice;
                        itemData.broken = false;

                        #region Set Inventory BG Color

                        switch (itemData.rankNum)
                        {
                            case 0:
                                itemHL.GetComponent<UIGradient>().enabled = false;
                                itemHL.color = new Color(0, 0, 0, 80);
                                break;
                            case 1:
                                itemHL.GetComponent<UIGradient>().enabled = true;
                                itemHL.GetComponent<UIGradient>().LinearColor1 = inventoryM.rank1Color1;
                                itemHL.GetComponent<UIGradient>().LinearColor2 = inventoryM.rank1Color2;
                                break;
                            case 2:
                                itemHL.GetComponent<UIGradient>().enabled = true;
                                itemHL.GetComponent<UIGradient>().LinearColor1 = inventoryM.rank2Color1;
                                itemHL.GetComponent<UIGradient>().LinearColor2 = inventoryM.rank2Color2;
                                break;
                            case 3:
                                itemHL.GetComponent<UIGradient>().enabled = true;
                                itemHL.GetComponent<UIGradient>().LinearColor1 = inventoryM.rank3Color1;
                                itemHL.GetComponent<UIGradient>().LinearColor2 = inventoryM.rank3Color2;
                                break;
                        }

                        #endregion

                        itemData.GetComponent<HealthPoints>().curHealthPoints = itemData.GetComponent<HealthPoints>().maxHealthPoints;
                    }
                    else
                    {
                        infoMessage.info.text = "You don't have enough coins.";
                    }
                }
                else
                    infoMessage.info.text = "This weapon is not broken.";
            }

            if (inventoryM.inventoryS == InventoryManager.InventorySection.BowAndArrow)
            {
                int slotNum = inventoryM.bowAndArrowInv.slotNum;
                inventoryM.bowAndArrowInv.slotBowEquipped = slotNum;
                wpnHolster.slotWeapon = slotNum;

                ItemData itemData = inventoryM.bowAndArrowInv.itemData[inventoryM.bowAndArrowInv.slotNum];
                Image itemHL = inventoryM.bowAndArrowInv.highLight[inventoryM.bowAndArrowInv.slotNum];

                if (itemData.broken)
                {
                    if (inventoryM.unityCoins >= itemData.repairPrice)
                    {
                        inventoryM.unityCoins -= itemData.repairPrice;
                        itemData.broken = false;

                        #region Set Inventory BG Color

                        switch (itemData.rankNum)
                        {
                            case 0:
                                itemHL.GetComponent<UIGradient>().enabled = false;
                                itemHL.color = new Color(0, 0, 0, 80);
                                break;
                            case 1:
                                itemHL.GetComponent<UIGradient>().enabled = true;
                                itemHL.GetComponent<UIGradient>().LinearColor1 = inventoryM.rank1Color1;
                                itemHL.GetComponent<UIGradient>().LinearColor2 = inventoryM.rank1Color2;
                                break;
                            case 2:
                                itemHL.GetComponent<UIGradient>().enabled = true;
                                itemHL.GetComponent<UIGradient>().LinearColor1 = inventoryM.rank2Color1;
                                itemHL.GetComponent<UIGradient>().LinearColor2 = inventoryM.rank2Color2;
                                break;
                            case 3:
                                itemHL.GetComponent<UIGradient>().enabled = true;
                                itemHL.GetComponent<UIGradient>().LinearColor1 = inventoryM.rank3Color1;
                                itemHL.GetComponent<UIGradient>().LinearColor2 = inventoryM.rank3Color2;
                                break;
                        }

                        #endregion

                        itemData.GetComponent<HealthPoints>().curHealthPoints = itemData.GetComponent<HealthPoints>().maxHealthPoints;
                    }
                    else
                    {
                        infoMessage.info.text = "You don't have enough coins.";
                    }
                }
                else
                    infoMessage.info.text = "This weapon is not broken.";
            }

            if (inventoryM.inventoryS == InventoryManager.InventorySection.Shield)
            {
                int slotNum = inventoryM.shieldInv.slotNum;
                inventoryM.shieldInv.slotShieldEquipped = slotNum;
                wpnHolster.slotWeapon = slotNum;

                ItemData itemData = inventoryM.shieldInv.itemData[inventoryM.shieldInv.slotNum];
                Image itemHL = inventoryM.shieldInv.highLight[inventoryM.shieldInv.slotNum];

                if (itemData.broken)
                {
                    if (inventoryM.unityCoins >= itemData.repairPrice)
                    {
                        inventoryM.unityCoins -= itemData.repairPrice;
                        itemData.broken = false;

                        #region Set Inventory BG Color

                        switch (itemData.rankNum)
                        {
                            case 0:
                                itemHL.GetComponent<UIGradient>().enabled = false;
                                itemHL.color = new Color(0, 0, 0, 80);
                                break;
                            case 1:
                                itemHL.GetComponent<UIGradient>().enabled = true;
                                itemHL.GetComponent<UIGradient>().LinearColor1 = inventoryM.rank1Color1;
                                itemHL.GetComponent<UIGradient>().LinearColor2 = inventoryM.rank1Color2;
                                break;
                            case 2:
                                itemHL.GetComponent<UIGradient>().enabled = true;
                                itemHL.GetComponent<UIGradient>().LinearColor1 = inventoryM.rank2Color1;
                                itemHL.GetComponent<UIGradient>().LinearColor2 = inventoryM.rank2Color2;
                                break;
                            case 3:
                                itemHL.GetComponent<UIGradient>().enabled = true;
                                itemHL.GetComponent<UIGradient>().LinearColor1 = inventoryM.rank3Color1;
                                itemHL.GetComponent<UIGradient>().LinearColor2 = inventoryM.rank3Color2;
                                break;
                        }

                        #endregion

                        itemData.GetComponent<HealthPoints>().curHealthPoints = itemData.GetComponent<HealthPoints>().maxHealthPoints;
                    }
                    else
                    {
                        infoMessage.info.text = "You don't have enough coins.";
                    }
                }
                else
                    infoMessage.info.text = "This weapon is not broken.";
            }
            if (repairAS)
                repairAS.PlayRandomClip();
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
            if (cancelAS)
                cancelAS.PlayRandomClip();
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
