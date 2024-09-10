using UnityEngine.UI;
using UnityEngine;
using TMPro;
using RPGAE.CharacterController;

public class UpgradeStoreManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject[] hideUI;
    public GameObject[] showUI;
    public GameObject[] normalizeShow;
    public GameObject[] normalizeHide;

    [Header("REFERENCES")]
    public GameObject[] requiredItems;
    public FadeUI fadeUI;
    public FadeUI weaponEquipmentAlpha;
    public FadeUI clothingEquipmentAlpha;
    public FadeUI storeItemPreviewAlpha;
    public Image itemMarketImage;
    public TextMeshProUGUI itemMarketName;
    public TextMeshProUGUI itemMarketDescription;
    public TextMeshProUGUI itemMarketPrice;
    public TextMeshProUGUI itemMarketInInventory;
    public TextMeshProUGUI coinQuantity;
    public TextMeshProUGUI maxed;
    public Animation storeItemPreviewAnim;
    public ParticleSystem sparkleParticle;

    [Header("UPGRADE UI")]
    public FadeUI upgradePreviewAlpha;
    public FadeUI requiredPreviewAlpha;
    [Header("Curent Stat")]
    public TextMeshProUGUI curStat1;
    public TextMeshProUGUI curStatValue1;
    public TextMeshProUGUI curStat2;
    public TextMeshProUGUI curStatValue2;
    public TextMeshProUGUI curStat3;
    public TextMeshProUGUI curStatValue3;
    public TextMeshProUGUI curStat4;
    public TextMeshProUGUI curStatValue4;
    public TextMeshProUGUI curStat5;
    public TextMeshProUGUI curStatValue5;
    public TextMeshProUGUI curStat6;
    public TextMeshProUGUI curStatValue6;
    [Header("New Stat")]
    public TextMeshProUGUI newStat1;
    public TextMeshProUGUI newStatValue1;
    public TextMeshProUGUI newStat2;
    public TextMeshProUGUI newStatValue2;
    public TextMeshProUGUI newStat3;
    public TextMeshProUGUI newStatValue3;
    public TextMeshProUGUI newStat4;
    public TextMeshProUGUI newStatValue4;
    public TextMeshProUGUI newStat5;
    public TextMeshProUGUI newStatValue5;
    public TextMeshProUGUI newStat6;
    public TextMeshProUGUI newStatValue6;

    #region Private 

    private MiniMap miniMap;
    private InfoMessage infoMessage;
    private ThirdPersonController cc;
    private InventoryManager inventoryM;
    private StoreManager storeM;

    private int nextNewStat1;
    private int nextNewStatValue1;
    private int nextNewStat2;
    private int nextNewStatValue2;
    private int nextNewStat3;
    private int nextNewStatValue3;
    private int nextNewStat4;
    private int nextNewStatValue4;
    private int nextNewStat5;
    private int nextNewStatValue5;
    private int nextNewStat6;
    private int nextNewStatValue6;

    [HideInInspector] public bool isActive;
    [HideInInspector] public bool oneShot;
    private bool previouslyActive;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        isActive = false;
        sparkleParticle.Stop();

        // Get Components
        miniMap = FindObjectOfType<MiniMap>();
        fadeUI = GetComponent<FadeUI>();
        storeM = FindObjectOfType<StoreManager>();
        infoMessage = FindObjectOfType<InfoMessage>();
        inventoryM = FindObjectOfType<InventoryManager>();
        cc = FindObjectOfType<ThirdPersonController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            foreach (GameObject hide in hideUI)
            {
                hide.SetActive(false);
            }
            foreach (GameObject show in showUI)
            {
                show.SetActive(true);
            }

            inventoryM.UpdateInventoryGrid();

            if (fadeUI.canvasGroup.alpha == 0)
            {
                weaponEquipmentAlpha.canvasGroup.alpha = 0;
                clothingEquipmentAlpha.canvasGroup.alpha = 0;
                inventoryM.InventorySectionIsActive(false);
                inventoryM.inventoryS = InventoryManager.InventorySection.Weapon;

                inventoryM.referencesObj.pauseMenu.GetComponent<FadeUI>().FadeTransition(1, 0.5f, 0.5f);
                inventoryM.referencesObj.inventorySection.GetComponent<FadeUI>().FadeTransition(1, 0.5f, 0.5f);

                miniMap.fadeUI.canvasGroup.alpha = 0;
                inventoryM.referencesObj.coinHUD.canvasGroup.alpha = 1;
                inventoryM.UIcanvas.renderMode = RenderMode.ScreenSpaceCamera;

                fadeUI.FadeTransition(1, 0, 0.5f);
                storeItemPreviewAlpha.FadeTransition(1, 0, 0.5f);
                storeItemPreviewAnim.Play();
                previouslyActive = true;
            }

            // If you're not fading exit 
            if (cc.rpgaeIM.PlayerControls.Start.triggered && !fadeUI.isFading)
            {
                ResetPreviewValues();
                isActive = false;
                inventoryM.storeM.isActive = false;
                cc.canMove = true;
                cc.tpCam.RemoveTargets(); 
                inventoryM.ItemOptionsIsActive(false);
                storeM.dialogueM.curDialogue.controller.dialogueActive = false;
                inventoryM.referencesObj.inventorySection.GetComponent<HideUI>().enabled = true;
            }
        }
        else
        {
            if (fadeUI.canvasGroup.alpha == 1)
            {
                inventoryM.referencesObj.pauseMenu.GetComponent<FadeUI>().FadeTransition(0, 0, 0.5f);

                inventoryM.UIcanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                if (sparkleParticle.isPlaying) sparkleParticle.Stop();
                inventoryM.referencesObj.coinHUD.canvasGroup.alpha = 0;

                fadeUI.FadeTransition(0, 0, 0.5f);
                storeItemPreviewAlpha.FadeTransition(0, 0, 0.5f);
                miniMap.fadeUI.FadeTransition(1, 0, 0.5f);
            }
            else if (fadeUI.canvasGroup.alpha == 0 && previouslyActive)
            {
                foreach (GameObject normalize in normalizeShow)
                {
                    normalize.SetActive(true);
                }
                foreach (GameObject normalize in normalizeHide)
                {
                    normalize.SetActive(false);
                }
                previouslyActive = false;
            }
        }
    }

    public bool Upgrade(string materialName, int requiredUpgradeAmount, ref ItemData itemID)
    {
        foreach (ItemData item in inventoryM.materialInv.itemData)
        {
            if (item.itemName == materialName)
            {
                if (item.quantity >= requiredUpgradeAmount)
                {
                    if (!oneShot)
                    {
                        itemID.rankNum++;
                        for (int i = 0; i < itemID.upgradeMaterial.Length; i++)
                        {
                            if (itemID.rankNum == 1)
                            {
                                RemoveMaterial(ref itemID.upgradeMaterial[i].matName, ref itemID.upgradeMaterial[i].matRequired);
                                itemID.upgradeMaterial[i].GetRank1Value();
                            }
                            if (itemID.rankNum == 2)
                            {
                                RemoveMaterial(ref itemID.upgradeMaterial[i].matName, ref itemID.upgradeMaterial[i].matRequired);
                                itemID.upgradeMaterial[i].GetRank2Value();
                            }
                            if (itemID.rankNum == 3)
                            {
                                RemoveMaterial(ref itemID.upgradeMaterial[i].matName, ref itemID.upgradeMaterial[i].matRequired);
                                itemID.upgradeMaterial[i].GetRank2Value();
                            }
                        }
                        if (itemID.rankNum == 1 || itemID.rankNum == 2 || itemID.rankNum == 3)
                        {
                            oneShot = true;
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    public bool RemoveMaterial(ref string materialName, ref int removeAmount)
    {
        foreach (ItemData item in inventoryM.materialInv.itemData)
        {
            if (materialName == item.itemName)
            {
                int Slot = -1;
                for (int i = 0; i < inventoryM.materialInv.itemData.Length; i++)
                {
                    if (item.itemName == inventoryM.materialInv.itemData[i].itemName)
                    {
                        Slot = i;
                    }
                }

                inventoryM.RemoveStackableItem(ref inventoryM.materialInv.itemData, ref inventoryM.materialInv.image,
                ref inventoryM.materialInv.highLight, ref inventoryM.materialInv.outLineBorder, ref inventoryM.materialInv.statValueBG, ref inventoryM.materialInv.quantity,
                ref inventoryM.materialInv.statValue, ref Slot, ref inventoryM.materialInv.counter, ref inventoryM.materialInv.statCounter,
                ref inventoryM.materialInv.removeNullSlots, removeAmount);

                return true;
            }
        }
        return false;
    }

    public bool CheckResources(ref string materialName, ref int currentAmount)
    {
        foreach (ItemData item in inventoryM.materialInv.itemData)
        {
            if (materialName == item.itemName)
            {
                int Slot = -1;
                for (int i = 0; i < inventoryM.materialInv.itemData.Length; i++)
                {
                    if (item.itemName == inventoryM.materialInv.itemData[i].itemName)
                    {
                        Slot = i;
                    }
                }
                if (inventoryM.materialInv.counter[Slot] < currentAmount)
                {
                    infoMessage.info.text = "You don't have the required materials!";
                    return true;
                }
                if (inventoryM.materialInv.itemData[Slot].buyPrice > inventoryM.unityCoins)
                {
                    infoMessage.info.text = "You don't have enough coins!";
                    return true;
                }
                else
                    return false;

            }
        }
        return false;
    }

    #region Item Preview 

    protected bool CurrentMaterial(ref string materialName, ref int currentAmount)
    {
        foreach (ItemData item in inventoryM.materialInv.itemData)
        {
            if (item.itemName == materialName)
            {
                int Slot = -1;
                for (int i = 0; i < inventoryM.materialInv.itemData.Length; i++)
                {
                    if (item.itemName == inventoryM.materialInv.itemData[i].itemName)
                    {
                        Slot = i;
                    }
                }
                currentAmount = inventoryM.materialInv.counter[Slot];
                return true;
            }
        }
        currentAmount = 0;
        return false;
    }

    public void CheckInventoryItems(ref ItemData[] itemData, ref int inventorySlot, ref int[] itemCounter)
    {
        itemMarketImage.enabled = true;
        itemMarketImage.sprite = itemData[inventorySlot].itemSprite;
        itemMarketName.text = itemData[inventorySlot].itemName;
        itemMarketDescription.text = itemData[inventorySlot].itemDescription;
        itemMarketPrice.text = "x" + itemData[inventorySlot].upgradePrice + " Upgrade Price";
        itemMarketInInventory.text = "x" + itemCounter[inventorySlot] + " In Inventory";

        switch (itemData[inventorySlot].itemType)
        {
            case ItemData.ItemType.Weapon:

                // CURRENT STATS

                curStat1.text = "Atk:";
                curStatValue1.text = itemData[inventorySlot].maxWpnAtk.ToString();

                curStat2.text = "Spd:";
                curStatValue2.text = itemData[inventorySlot].maxWpnSpd.ToString();

                curStat3.text = "Stun:";
                curStatValue3.text = itemData[inventorySlot].maxWpnStun.ToString();

                curStat4.gameObject.SetActive(true);
                curStat4.text = "Crit-Hit:";
                curStatValue4.text = itemData[inventorySlot].maxWpnCritHit.ToString();

                curStat5.gameObject.SetActive(true);
                curStat5.text = "Dura:";
                curStatValue5.text = itemData[inventorySlot].maxWpnDura.ToString();

                curStat6.gameObject.SetActive(false);

                // NEW  STATS

                if (itemData[inventorySlot].rankNum == 0)
                {
                    nextNewStat1 = itemData[inventorySlot].wpnAtk.value + itemData[inventorySlot].wpnAtk.rank1;
                    newStat1.text = curStat1.text;
                    newStatValue1.text = nextNewStat1.ToString();

                    nextNewStat2 = itemData[inventorySlot].wpnSpd.value + itemData[inventorySlot].wpnSpd.rank1;
                    newStat2.text = curStat2.text;
                    newStatValue2.text = nextNewStat2.ToString();

                    nextNewStat3 = itemData[inventorySlot].wpnStun.value + itemData[inventorySlot].wpnStun.rank1;
                    newStat3.text = curStat3.text;
                    newStatValue3.text = nextNewStat3.ToString();

                    newStat4.gameObject.SetActive(true);
                    nextNewStat4 = itemData[inventorySlot].wpnCritHit.value + itemData[inventorySlot].wpnCritHit.rank1;
                    newStat4.text = curStat4.text;
                    newStatValue4.text = nextNewStat4.ToString();

                    newStat5.gameObject.SetActive(true);
                    nextNewStat5 = itemData[inventorySlot].wpnDura.value + itemData[inventorySlot].wpnDura.rank1;
                    newStat5.text = curStat5.text;
                    newStatValue5.text = nextNewStat5.ToString();

                    newStat6.gameObject.SetActive(false);
                }

                if (itemData[inventorySlot].rankNum == 1)
                {
                    nextNewStat1 = itemData[inventorySlot].wpnAtk.value + itemData[inventorySlot].wpnAtk.rank2;
                    newStat1.text = curStat1.text;
                    newStatValue1.text = nextNewStat1.ToString();

                    nextNewStat2 = itemData[inventorySlot].wpnSpd.value + itemData[inventorySlot].wpnSpd.rank2;
                    newStat2.text = curStat2.text;
                    newStatValue2.text = nextNewStat2.ToString();

                    nextNewStat3 = itemData[inventorySlot].wpnStun.value + itemData[inventorySlot].wpnStun.rank2;
                    newStat3.text = curStat3.text;
                    newStatValue3.text = nextNewStat3.ToString();

                    newStat4.gameObject.SetActive(true);
                    nextNewStat4 = itemData[inventorySlot].wpnCritHit.value + itemData[inventorySlot].wpnCritHit.rank2;
                    newStat4.text = curStat4.text;
                    newStatValue4.text = nextNewStat4.ToString();

                    newStat5.gameObject.SetActive(true);
                    nextNewStat5 = itemData[inventorySlot].wpnDura.value + itemData[inventorySlot].wpnDura.rank2;
                    newStat5.text = curStat5.text;
                    newStatValue5.text = nextNewStat5.ToString();

                    newStat6.gameObject.SetActive(false);
                }

                if (itemData[inventorySlot].rankNum == 2)
                {
                    nextNewStat1 = itemData[inventorySlot].wpnAtk.value + itemData[inventorySlot].wpnAtk.rank3;
                    newStat1.text = curStat1.text;
                    newStatValue1.text = nextNewStat1.ToString();

                    nextNewStat2 = itemData[inventorySlot].wpnSpd.value + itemData[inventorySlot].wpnSpd.rank3;
                    newStat2.text = curStat2.text;
                    newStatValue2.text = nextNewStat2.ToString();

                    nextNewStat3 = itemData[inventorySlot].wpnStun.value + itemData[inventorySlot].wpnStun.rank3;
                    newStat3.text = curStat3.text;
                    newStatValue3.text = nextNewStat3.ToString();

                    newStat4.gameObject.SetActive(true);
                    nextNewStat4 = itemData[inventorySlot].wpnCritHit.value + itemData[inventorySlot].wpnCritHit.rank3;
                    newStat4.text = curStat4.text;
                    newStatValue4.text = nextNewStat4.ToString();

                    newStat5.gameObject.SetActive(true);
                    nextNewStat5 = itemData[inventorySlot].wpnDura.value + itemData[inventorySlot].wpnDura.rank3;
                    newStat5.text = curStat5.text;
                    newStatValue5.text = nextNewStat5.ToString();

                    newStat6.gameObject.SetActive(false);
                }

                if (itemData[inventorySlot].rankNum < 3)
                {
                    maxed.text = "";
                    requiredPreviewAlpha.canvasGroup.alpha = 1;
                }
                else
                {
                    maxed.text = "Maxed";
                    requiredPreviewAlpha.canvasGroup.alpha = 0;
                }

                break;
            case ItemData.ItemType.Bow:

                // CURRENT STATS

                curStat1.text = "Atk:";
                curStatValue1.text = itemData[inventorySlot].maxBowAtk.ToString();

                curStat2.text = "Spd:";
                curStatValue2.text = itemData[inventorySlot].maxBowSpd.ToString();

                curStat3.text = "Stun:";
                curStatValue3.text = itemData[inventorySlot].maxBowStun.ToString();

                curStat4.gameObject.SetActive(true);
                curStat4.text = "Crit-Hit:";
                curStatValue4.text = itemData[inventorySlot].maxBowCritHit.ToString();

                curStat5.gameObject.SetActive(true);
                curStat5.text = "Hd-Shot:";
                curStatValue5.text = itemData[inventorySlot].maxBowHdShot.ToString();

                curStat6.gameObject.SetActive(true);
                curStat6.text = "Dura:";
                curStatValue6.text = itemData[inventorySlot].maxBowDura.ToString();

                // NEW  STATS

                if (itemData[inventorySlot].rankNum == 0)
                {
                    nextNewStat1 = itemData[inventorySlot].bowAtk.value + itemData[inventorySlot].bowAtk.rank1;
                    newStat1.text = curStat1.text;
                    newStatValue1.text = nextNewStat1.ToString();

                    nextNewStat2 = itemData[inventorySlot].bowSpd.value + itemData[inventorySlot].bowSpd.rank1;
                    newStat2.text = curStat2.text;
                    newStatValue2.text = nextNewStat2.ToString();

                    nextNewStat3 = itemData[inventorySlot].bowStun.value + itemData[inventorySlot].bowStun.rank1;
                    newStat3.text = curStat3.text;
                    newStatValue3.text = nextNewStat3.ToString();

                    newStat4.gameObject.SetActive(true);
                    nextNewStat4 = itemData[inventorySlot].bowCritHit.value + itemData[inventorySlot].bowCritHit.rank1;
                    newStat4.text = curStat4.text;
                    newStatValue4.text = nextNewStat4.ToString();

                    newStat5.gameObject.SetActive(true);
                    nextNewStat5 = itemData[inventorySlot].bowHdShot.value + itemData[inventorySlot].bowHdShot.rank1;
                    newStat5.text = curStat5.text;
                    newStatValue5.text = nextNewStat5.ToString();

                    newStat6.gameObject.SetActive(true);
                    nextNewStat6 = itemData[inventorySlot].bowDura.value + itemData[inventorySlot].bowDura.rank1;
                    newStat6.text = curStat6.text;
                    newStatValue6.text = nextNewStat6.ToString();
                }

                if (itemData[inventorySlot].rankNum == 1)
                {
                    nextNewStat1 = itemData[inventorySlot].bowAtk.value + itemData[inventorySlot].bowAtk.rank2;
                    newStat1.text = curStat1.text;
                    newStatValue1.text = nextNewStat1.ToString();

                    nextNewStat2 = itemData[inventorySlot].bowSpd.value + itemData[inventorySlot].bowSpd.rank2;
                    newStat2.text = curStat2.text;
                    newStatValue2.text = nextNewStat2.ToString();

                    nextNewStat3 = itemData[inventorySlot].bowStun.value + itemData[inventorySlot].bowStun.rank2;
                    newStat3.text = curStat3.text;
                    newStatValue3.text = nextNewStat3.ToString();

                    newStat4.gameObject.SetActive(true);
                    nextNewStat4 = itemData[inventorySlot].bowCritHit.value + itemData[inventorySlot].bowCritHit.rank2;
                    newStat4.text = curStat4.text;
                    newStatValue4.text = nextNewStat4.ToString();

                    newStat5.gameObject.SetActive(true);
                    nextNewStat5 = itemData[inventorySlot].bowHdShot.value + itemData[inventorySlot].bowHdShot.rank2;
                    newStat5.text = curStat5.text;
                    newStatValue5.text = nextNewStat5.ToString();

                    newStat6.gameObject.SetActive(true);
                    nextNewStat6 = itemData[inventorySlot].bowDura.value + itemData[inventorySlot].bowDura.rank2;
                    newStat6.text = curStat6.text;
                    newStatValue6.text = nextNewStat6.ToString();
                }

                if (itemData[inventorySlot].rankNum == 2)
                {
                    nextNewStat1 = itemData[inventorySlot].bowAtk.value + itemData[inventorySlot].bowAtk.rank3;
                    newStat1.text = curStat1.text;
                    newStatValue1.text = nextNewStat1.ToString();

                    nextNewStat2 = itemData[inventorySlot].bowSpd.value + itemData[inventorySlot].bowSpd.rank3;
                    newStat2.text = curStat2.text;
                    newStatValue2.text = nextNewStat2.ToString();

                    nextNewStat3 = itemData[inventorySlot].bowStun.value + itemData[inventorySlot].bowStun.rank3;
                    newStat3.text = curStat3.text;
                    newStatValue3.text = nextNewStat3.ToString();

                    newStat4.gameObject.SetActive(true);
                    nextNewStat4 = itemData[inventorySlot].bowCritHit.value + itemData[inventorySlot].bowCritHit.rank3;
                    newStat4.text = curStat4.text;
                    newStatValue4.text = nextNewStat4.ToString();

                    newStat5.gameObject.SetActive(true);
                    nextNewStat5 = itemData[inventorySlot].bowHdShot.value + itemData[inventorySlot].bowHdShot.rank3;
                    newStat5.text = curStat5.text;
                    newStatValue5.text = nextNewStat5.ToString();

                    newStat6.gameObject.SetActive(true);
                    nextNewStat6 = itemData[inventorySlot].bowDura.value + itemData[inventorySlot].bowDura.rank3;
                    newStat6.text = curStat6.text;
                    newStatValue6.text = nextNewStat6.ToString();
                }

                if (itemData[inventorySlot].rankNum < 3)
                {
                    maxed.text = "";
                    requiredPreviewAlpha.canvasGroup.alpha = 1;
                }
                else
                {
                    maxed.text = "Maxed";
                    requiredPreviewAlpha.canvasGroup.alpha = 0;
                }

                break;
            case ItemData.ItemType.Shield:

                // CURRENT STATS

                curStat1.text = "Atk:";
                curStatValue1.text = itemData[inventorySlot].maxShdAtk.ToString();

                curStat2.text = "Spd:";
                curStatValue2.text = itemData[inventorySlot].maxShdSpd.ToString();

                curStat3.text = "Stun:";
                curStatValue3.text = itemData[inventorySlot].maxShdStun.ToString();

                curStat4.gameObject.SetActive(true);
                curStat4.text = "Crit-Hit:";
                curStatValue4.text = itemData[inventorySlot].maxShdCritHit.ToString();

                curStat5.gameObject.SetActive(true);
                curStat5.text = "Shield Block:";
                curStatValue5.text = itemData[inventorySlot].maxShdBlock.ToString();

                curStat6.gameObject.SetActive(true);
                curStat6.text = "Dura:";
                curStatValue6.text = itemData[inventorySlot].maxShdDura.ToString();

                // NEW  STATS

                if (itemData[inventorySlot].rankNum == 0)
                {
                    nextNewStat1 = itemData[inventorySlot].shdAtk.value + itemData[inventorySlot].shdAtk.rank1;
                    newStat1.text = curStat1.text;
                    newStatValue1.text = nextNewStat1.ToString();

                    nextNewStat2 = itemData[inventorySlot].shdSpd.value + itemData[inventorySlot].shdSpd.rank1;
                    newStat2.text = curStat2.text;
                    newStatValue2.text = nextNewStat2.ToString();

                    nextNewStat3 = itemData[inventorySlot].shdStun.value + itemData[inventorySlot].shdStun.rank1;
                    newStat3.text = curStat3.text;
                    newStatValue3.text = nextNewStat3.ToString();

                    newStat4.gameObject.SetActive(true);
                    nextNewStat4 = itemData[inventorySlot].shdCritHit.value + itemData[inventorySlot].shdCritHit.rank1;
                    newStat4.text = curStat4.text;
                    newStatValue4.text = nextNewStat4.ToString();

                    newStat5.gameObject.SetActive(true);
                    nextNewStat5 = itemData[inventorySlot].shdBlock.value + itemData[inventorySlot].shdBlock.rank1;
                    newStat5.text = curStat5.text;
                    newStatValue5.text = nextNewStat5.ToString();

                    newStat6.gameObject.SetActive(true);
                    nextNewStat6 = itemData[inventorySlot].shdDura.value + itemData[inventorySlot].shdDura.rank1;
                    newStat6.text = curStat6.text;
                    newStatValue6.text = nextNewStat6.ToString();
                }

                if (itemData[inventorySlot].rankNum == 1)
                {
                    nextNewStat1 = itemData[inventorySlot].shdAtk.value + itemData[inventorySlot].shdAtk.rank2;
                    newStat1.text = curStat1.text;
                    newStatValue1.text = nextNewStat1.ToString();

                    nextNewStat2 = itemData[inventorySlot].shdSpd.value + itemData[inventorySlot].shdSpd.rank2;
                    newStat2.text = curStat2.text;
                    newStatValue2.text = nextNewStat2.ToString();

                    nextNewStat3 = itemData[inventorySlot].shdStun.value + itemData[inventorySlot].shdStun.rank2;
                    newStat3.text = curStat3.text;
                    newStatValue3.text = nextNewStat3.ToString();

                    newStat4.gameObject.SetActive(true);
                    nextNewStat4 = itemData[inventorySlot].shdCritHit.value + itemData[inventorySlot].shdCritHit.rank2;
                    newStat4.text = curStat4.text;
                    newStatValue4.text = nextNewStat4.ToString();

                    newStat5.gameObject.SetActive(true);
                    nextNewStat5 = itemData[inventorySlot].shdBlock.value + itemData[inventorySlot].shdBlock.rank2;
                    newStat5.text = curStat5.text;
                    newStatValue5.text = nextNewStat5.ToString();

                    newStat6.gameObject.SetActive(true);
                    nextNewStat6 = itemData[inventorySlot].shdDura.value + itemData[inventorySlot].shdDura.rank2;
                    newStat6.text = curStat6.text;
                    newStatValue6.text = nextNewStat6.ToString();
                }

                if (itemData[inventorySlot].rankNum == 2)
                {
                    nextNewStat1 = itemData[inventorySlot].shdAtk.value + itemData[inventorySlot].shdAtk.rank3;
                    newStat1.text = curStat1.text;
                    newStatValue1.text = nextNewStat1.ToString();

                    nextNewStat2 = itemData[inventorySlot].shdSpd.value + itemData[inventorySlot].shdSpd.rank3;
                    newStat2.text = curStat2.text;
                    newStatValue2.text = nextNewStat2.ToString();

                    nextNewStat3 = itemData[inventorySlot].shdStun.value + itemData[inventorySlot].shdStun.rank3;
                    newStat3.text = curStat3.text;
                    newStatValue3.text = nextNewStat3.ToString();

                    newStat4.gameObject.SetActive(true);
                    nextNewStat4 = itemData[inventorySlot].shdCritHit.value + itemData[inventorySlot].shdCritHit.rank3;
                    newStat4.text = curStat4.text;
                    newStatValue4.text = nextNewStat4.ToString();

                    newStat5.gameObject.SetActive(true);
                    nextNewStat5 = itemData[inventorySlot].shdBlock.value + itemData[inventorySlot].shdBlock.rank3;
                    newStat5.text = curStat5.text;
                    newStatValue5.text = nextNewStat5.ToString();

                    newStat6.gameObject.SetActive(true);
                    nextNewStat6 = itemData[inventorySlot].shdDura.value + itemData[inventorySlot].shdDura.rank3;
                    newStat6.text = curStat6.text;
                    newStatValue6.text = nextNewStat6.ToString();
                }

                if (itemData[inventorySlot].rankNum < 3)
                {
                    maxed.text = "";
                    requiredPreviewAlpha.canvasGroup.alpha = 1;
                }
                else
                {
                    maxed.text = "Maxed";
                    requiredPreviewAlpha.canvasGroup.alpha = 0;
                }

                break;
            case ItemData.ItemType.Armor:

                // CURRENT STATS

                curStat1.text = "Arm:";
                curStatValue1.text = itemData[inventorySlot].maxArm.ToString();

                curStat2.text = "Arm L Res:";
                curStatValue2.text = itemData[inventorySlot].maxArmLRes.ToString();

                curStat3.text = "Arm H Res:";
                curStatValue3.text = itemData[inventorySlot].maxArmHRes.ToString();

                curStat4.gameObject.SetActive(false);
                curStat5.gameObject.SetActive(false);
                curStat6.gameObject.SetActive(false);

                // NEW  STATS

                if (itemData[inventorySlot].rankNum == 0)
                {
                    nextNewStat1 = itemData[inventorySlot].arm.value + itemData[inventorySlot].arm.rank1;
                    newStat1.text = curStat1.text;
                    newStatValue1.text = nextNewStat1.ToString();

                    nextNewStat2 = itemData[inventorySlot].armLRes.value + itemData[inventorySlot].armLRes.rank1;
                    newStat2.text = curStat2.text;
                    newStatValue2.text = nextNewStat2.ToString();

                    nextNewStat3 = itemData[inventorySlot].armHRes.value + itemData[inventorySlot].armHRes.rank1;
                    newStat3.text = curStat3.text;
                    newStatValue3.text = nextNewStat3.ToString();

                    newStat4.gameObject.SetActive(false);
                    newStat5.gameObject.SetActive(false);
                    newStat6.gameObject.SetActive(false);
                }

                if (itemData[inventorySlot].rankNum == 1)
                {
                    nextNewStat1 = itemData[inventorySlot].arm.value + itemData[inventorySlot].arm.rank2;
                    newStat1.text = curStat1.text;
                    newStatValue1.text = nextNewStat1.ToString();

                    nextNewStat2 = itemData[inventorySlot].armLRes.value + itemData[inventorySlot].armLRes.rank2;
                    newStat2.text = curStat2.text;
                    newStatValue2.text = nextNewStat2.ToString();

                    nextNewStat3 = itemData[inventorySlot].armHRes.value + itemData[inventorySlot].armHRes.rank2;
                    newStat3.text = curStat3.text;
                    newStatValue3.text = nextNewStat3.ToString();

                    newStat4.gameObject.SetActive(false);
                    newStat5.gameObject.SetActive(false);
                    newStat6.gameObject.SetActive(false);
                }

                if (itemData[inventorySlot].rankNum == 2)
                {
                    nextNewStat1 = itemData[inventorySlot].arm.value + itemData[inventorySlot].arm.rank3;
                    newStat1.text = curStat1.text;
                    newStatValue1.text = nextNewStat1.ToString();

                    nextNewStat2 = itemData[inventorySlot].armLRes.value + itemData[inventorySlot].armLRes.rank3;
                    newStat2.text = curStat2.text;
                    newStatValue2.text = nextNewStat2.ToString();

                    nextNewStat3 = itemData[inventorySlot].armHRes.value + itemData[inventorySlot].armHRes.rank3;
                    newStat3.text = curStat3.text;
                    newStatValue3.text = nextNewStat3.ToString();

                    newStat4.gameObject.SetActive(false);
                    newStat5.gameObject.SetActive(false);
                    newStat6.gameObject.SetActive(false);
                }

                if (itemData[inventorySlot].rankNum < 3)
                {
                    maxed.text = "";
                    requiredPreviewAlpha.canvasGroup.alpha = 1;
                }
                else
                {
                    maxed.text = "Maxed";
                    requiredPreviewAlpha.canvasGroup.alpha = 0;
                }

                break;
        }

        for (int i = 0; i < itemData[inventorySlot].upgradeMaterial.Length; i++)
        {
            foreach (GameObject items in inventoryM.itemIconData)
            {
                if (items.GetComponentInChildren<ItemData>().itemName == itemData[inventorySlot].upgradeMaterial[i].matName)
                {
                    requiredItems[i].GetComponent<RequiredItems>().image.sprite = items.GetComponentInChildren<ItemData>().itemSprite;
                }
            }

            requiredItems[i].GetComponent<RequiredItems>().requiredAmount = itemData[inventorySlot].upgradeMaterial[i].matRequired;
            CurrentMaterial(ref itemData[inventorySlot].upgradeMaterial[i].matName, ref requiredItems[i].GetComponent<RequiredItems>().inventoryAmount);
        }
        upgradePreviewAlpha.canvasGroup.alpha = 1;
    }

    public void ResetPreviewValues()
    {
        itemMarketImage.sprite = null;
        itemMarketImage.enabled = false;
        itemMarketName.text = null;
        itemMarketDescription.text = null;
        itemMarketPrice.text = null;
        itemMarketInInventory.text = null;
        sparkleParticle.Stop();
        upgradePreviewAlpha.canvasGroup.alpha = 0;
    }

    #endregion
}
