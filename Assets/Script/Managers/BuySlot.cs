using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class BuySlot : MonoBehaviour {

    public int SlotNum;

    [Header("AUDIO")]
    public RandomAudioPlayer menuMoveAS;

    [Header("REFERENCES")]
    public RectTransform rectTransform;
    public Image itemImage;
    public Image highLight;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemStock;
    public TextMeshProUGUI itemPrice;

    #region Private 

    private BuyStoreManager buyM;
    private InventoryManager inventoryM;
    private InputFieldOptions inputFieldOptions;
    private GameObject buyContentPannel;
    [HideInInspector] public int itemStockCounter;

    #endregion

    // Use this for initialization
    void Start ()
    {
        itemImage.preserveAspect = true;
        rectTransform = GetComponent<RectTransform>();
        inventoryM = FindObjectOfType<InventoryManager>();
        buyM = FindObjectOfType<BuyStoreManager>();

        buyContentPannel = GameObject.Find("BuyContent");
        SetDefaultSettings();
    }

    public void OnButtonEnter(Transform slot)
    {
        BuySlot slotInfo = slot.GetComponentInParent<BuySlot>();
        Image highLight = slot.GetComponent<Image>();
        InputFieldOptions inputField = FindObjectOfType<InputFieldOptions>();

        if (inventoryM.referencesObj.itemOptionsBuyObj.activeInHierarchy 
        || inputField.GetComponent<FadeUI>().canvasGroup.alpha == 1)
            return;

        buyM.buyItems.buySlot = slotInfo.SlotNum;

        #region Effects

        if (menuMoveAS)
            menuMoveAS.PlayRandomClip();
        if (buyM.sparkleParticle)
            buyM.sparkleParticle.Play();

        #endregion

        #region HighLight

        foreach (Image h in buyM.buyItems.highLight)
        {
            h.color = buyM.neutralColor;
        }
        highLight.color = buyM.selectColor;

        #endregion

        #region Check Inventory

        if (buyM.buyItems.itemData[buyM.buyItems.buySlot].itemName != null)
        {
            buyM.itemMarketImage.sprite = buyM.buyItems.itemImage[buyM.buyItems.buySlot].sprite;
            buyM.itemMarketImage.preserveAspect = true;
            buyM.itemMarketImage.enabled = true;
            buyM.itemMarketName.text = buyM.buyItems.itemName[buyM.buyItems.buySlot].text;
            buyM.itemMarketDescription.text = buyM.buyItems.itemData[buyM.buyItems.buySlot].itemDescription;
            buyM.itemMarketPrice.text = buyM.buyItems.prices[buyM.buyItems.buySlot].text + " Buy Price";

            switch (buyM.buyItems.itemData[buyM.buyItems.buySlot].itemType)
            {
                case ItemData.ItemType.Weapon:
                    buyM.CheckInventoryItems(ref inventoryM.weaponInv.itemData, ref inventoryM.weaponInv.counter, ref buyM.buyItems.itemData[buyM.buyItems.buySlot].itemName);
                    break;
                case ItemData.ItemType.Bow:
                    buyM.CheckInventoryItems(ref inventoryM.bowAndArrowInv.itemData, ref inventoryM.bowAndArrowInv.counter, ref buyM.buyItems.itemData[buyM.buyItems.buySlot].itemName);
                    break;
                case ItemData.ItemType.Arrow:
                    buyM.CheckInventoryItems(ref inventoryM.bowAndArrowInv.itemData, ref inventoryM.bowAndArrowInv.counter, ref buyM.buyItems.itemData[buyM.buyItems.buySlot].itemName);
                    break;
                case ItemData.ItemType.Shield:
                    buyM.CheckInventoryItems(ref inventoryM.shieldInv.itemData, ref inventoryM.shieldInv.counter, ref buyM.buyItems.itemData[buyM.buyItems.buySlot].itemName);
                    break;
                case ItemData.ItemType.Armor:
                    buyM.CheckInventoryItems(ref inventoryM.armorInv.itemData, ref inventoryM.armorInv.counter, ref buyM.buyItems.itemData[buyM.buyItems.buySlot].itemName);
                    break;
                case ItemData.ItemType.Material:
                    buyM.CheckInventoryItems(ref inventoryM.materialInv.itemData, ref inventoryM.materialInv.counter, ref buyM.buyItems.itemData[buyM.buyItems.buySlot].itemName);
                    break;
                case ItemData.ItemType.Healing:
                    buyM.CheckInventoryItems(ref inventoryM.healingInv.itemData, ref inventoryM.healingInv.counter, ref buyM.buyItems.itemData[buyM.buyItems.buySlot].itemName);
                    break;
                case ItemData.ItemType.Key:
                    buyM.CheckInventoryItems(ref inventoryM.keyInv.itemData, ref inventoryM.keyInv.counter, ref buyM.buyItems.itemData[buyM.buyItems.buySlot].itemName);
                    break;
            }
        }

        #endregion
    }

    public void ButtonSelect(Transform slot)
    {
        if (buyM.fadeUI.canvasGroup.alpha != 1) return;

        #region Buy Option

        if (buyM.buyItems.inStockCounter[buyM.buyItems.buySlot] > 0)
        {
            inventoryM.referencesObj.itemOptionsBuyObj.SetActive(true);
            inventoryM.referencesObj.itemOptionsBuyObj.transform.position = buyM.buyItems.highLight[buyM.buyItems.buySlot].transform.position;
        }
        else
            inventoryM.cc.infoMessage.info.text = "Sold out";

        #endregion

        #region Check Inventory

        if (buyM.buyItems.itemData[buyM.buyItems.buySlot].itemName != null)
        {
            buyM.itemMarketImage.sprite = buyM.buyItems.itemImage[buyM.buyItems.buySlot].sprite;
            buyM.itemMarketImage.preserveAspect = true;
            buyM.itemMarketImage.enabled = true;
            buyM.itemMarketName.text = buyM.buyItems.itemName[buyM.buyItems.buySlot].text;
            buyM.itemMarketDescription.text = buyM.buyItems.itemData[buyM.buyItems.buySlot].itemDescription;
            buyM.itemMarketPrice.text = buyM.buyItems.prices[buyM.buyItems.buySlot].text + " Buy Price";

            switch (buyM.buyItems.itemData[buyM.buyItems.buySlot].itemType)
            {
                case ItemData.ItemType.Weapon:
                    buyM.CheckInventoryItems(ref inventoryM.weaponInv.itemData, ref inventoryM.weaponInv.counter, ref buyM.buyItems.itemData[buyM.buyItems.buySlot].itemName);
                    break;
                case ItemData.ItemType.Bow:
                    buyM.CheckInventoryItems(ref inventoryM.bowAndArrowInv.itemData, ref inventoryM.bowAndArrowInv.counter, ref buyM.buyItems.itemData[buyM.buyItems.buySlot].itemName);
                    break;
                case ItemData.ItemType.Arrow:
                    buyM.CheckInventoryItems(ref inventoryM.bowAndArrowInv.itemData, ref inventoryM.bowAndArrowInv.counter, ref buyM.buyItems.itemData[buyM.buyItems.buySlot].itemName);
                    break;
                case ItemData.ItemType.Shield:
                    buyM.CheckInventoryItems(ref inventoryM.shieldInv.itemData, ref inventoryM.shieldInv.counter, ref buyM.buyItems.itemData[buyM.buyItems.buySlot].itemName);
                    break;
                case ItemData.ItemType.Armor:
                    buyM.CheckInventoryItems(ref inventoryM.armorInv.itemData, ref inventoryM.armorInv.counter, ref buyM.buyItems.itemData[buyM.buyItems.buySlot].itemName);
                    break;
                case ItemData.ItemType.Material:
                    buyM.CheckInventoryItems(ref inventoryM.materialInv.itemData, ref inventoryM.materialInv.counter, ref buyM.buyItems.itemData[buyM.buyItems.buySlot].itemName);
                    break;
                case ItemData.ItemType.Healing:
                    buyM.CheckInventoryItems(ref inventoryM.healingInv.itemData, ref inventoryM.healingInv.counter, ref buyM.buyItems.itemData[buyM.buyItems.buySlot].itemName);
                    break;
                case ItemData.ItemType.Key:
                    buyM.CheckInventoryItems(ref inventoryM.keyInv.itemData, ref inventoryM.keyInv.counter, ref buyM.buyItems.itemData[buyM.buyItems.buySlot].itemName);
                    break;
            }
        }

        #endregion
    }

    public void OnButtonExit(Transform slot)
    {
        Image highLight = slot.GetComponent<Image>();
        InputFieldOptions inputField = FindObjectOfType<InputFieldOptions>();

        if (inventoryM.referencesObj.itemOptionsBuyObj.activeInHierarchy 
        || inputField.GetComponent<FadeUI>().canvasGroup.alpha == 1)
            return;

        #region HighLight

        highLight.color = buyM.neutralColor;

        #endregion

        #region Check Inventory

        if (buyM.buyItems.itemData[buyM.buyItems.buySlot].itemName != null)
        {
            buyM.itemMarketImage.sprite = buyM.buyItems.itemImage[buyM.buyItems.buySlot].sprite;
            buyM.itemMarketImage.preserveAspect = true;
            buyM.itemMarketImage.enabled = true;
            buyM.itemMarketName.text = buyM.buyItems.itemName[buyM.buyItems.buySlot].text;
            buyM.itemMarketDescription.text = buyM.buyItems.itemData[buyM.buyItems.buySlot].itemDescription;
            buyM.itemMarketPrice.text = buyM.buyItems.prices[buyM.buyItems.buySlot].text + " Buy Price";

            switch (buyM.buyItems.itemData[buyM.buyItems.buySlot].itemType)
            {
                case ItemData.ItemType.Weapon:
                    buyM.CheckInventoryItems(ref inventoryM.weaponInv.itemData, ref inventoryM.weaponInv.counter, ref buyM.buyItems.itemData[buyM.buyItems.buySlot].itemName);
                    break;
                case ItemData.ItemType.Bow:
                    buyM.CheckInventoryItems(ref inventoryM.bowAndArrowInv.itemData, ref inventoryM.bowAndArrowInv.counter, ref buyM.buyItems.itemData[buyM.buyItems.buySlot].itemName);
                    break;
                case ItemData.ItemType.Arrow:
                    buyM.CheckInventoryItems(ref inventoryM.bowAndArrowInv.itemData, ref inventoryM.bowAndArrowInv.counter, ref buyM.buyItems.itemData[buyM.buyItems.buySlot].itemName);
                    break;
                case ItemData.ItemType.Shield:
                    buyM.CheckInventoryItems(ref inventoryM.shieldInv.itemData, ref inventoryM.shieldInv.counter, ref buyM.buyItems.itemData[buyM.buyItems.buySlot].itemName);
                    break;
                case ItemData.ItemType.Armor:
                    buyM.CheckInventoryItems(ref inventoryM.armorInv.itemData, ref inventoryM.armorInv.counter, ref buyM.buyItems.itemData[buyM.buyItems.buySlot].itemName);
                    break;
                case ItemData.ItemType.Material:
                    buyM.CheckInventoryItems(ref inventoryM.materialInv.itemData, ref inventoryM.materialInv.counter, ref buyM.buyItems.itemData[buyM.buyItems.buySlot].itemName);
                    break;
                case ItemData.ItemType.Healing:
                    buyM.CheckInventoryItems(ref inventoryM.healingInv.itemData, ref inventoryM.healingInv.counter, ref buyM.buyItems.itemData[buyM.buyItems.buySlot].itemName);
                    break;
                case ItemData.ItemType.Key:
                    buyM.CheckInventoryItems(ref inventoryM.keyInv.itemData, ref inventoryM.keyInv.counter, ref buyM.buyItems.itemData[buyM.buyItems.buySlot].itemName);
                    break;
            }
        }

        #endregion
    }

    void SetDefaultSettings()
    {
        transform.SetParent(buyContentPannel.transform);

        rectTransform.localPosition = new Vector3(0, 0, 0);
        rectTransform.localRotation = Quaternion.Euler(0, 0, 0);
        rectTransform.localScale = new Vector3(1, 1, 1);

        gameObject.name = "BuySlot" + SlotNum;
    }
}
