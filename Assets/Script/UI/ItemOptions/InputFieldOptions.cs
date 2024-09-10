using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using RPGAE.CharacterController;

public class InputFieldOptions : MonoBehaviour {

    public enum InputFieldType
    {
        Drop,
        Buy,
        Sell, 
        Null
    }
    public Image decrease;
    public Image increase;
    public FadeUI buttonGroupAlpha;

    [Header("AUDIO")]
    public RandomAudioPlayer itemPurchasedAS;
    public RandomAudioPlayer cancelAS;

    #region Private 

    private float inputValue;
    private BuyStoreManager buyM;
    private int parsedNum = 0;
    private string enteredNum;
    private Transform dropTarget;
    private InfoMessage infoMessage;
    private BuyStoreManager buyItemM;
    private TMP_InputField inputField;
    private ThirdPersonController cc;
    private InventoryManager inventoryM;
    [HideInInspector] public ItemDescription itemDescription;
    [HideInInspector] public FadeUI fadeUI;
    [HideInInspector] public GameObject dropItem;
    [HideInInspector] public InputFieldType inputFieldType;

    #endregion

    // Use this for initialization
    void Start () 
    {
        dropTarget = GameObject.Find("Character").transform;

        // Get Components
        buyM = FindObjectOfType<BuyStoreManager>();
        fadeUI = GetComponent<FadeUI>();
        buyItemM = FindObjectOfType<BuyStoreManager>();
        inputField = GetComponent<TMP_InputField>();
        infoMessage = FindObjectOfType<InfoMessage>();
        cc = FindObjectOfType<ThirdPersonController>();
        inventoryM = FindObjectOfType<InventoryManager>();
    }

    // Update is called once per frame
    void Update()
    { 
        if (inventoryM.systemM.blackScreenFUI.canvasGroup.alpha != 0) return;

        if (fadeUI.canvasGroup.alpha == 0)
        {
            Navigation none = new Navigation { mode = Navigation.Mode.None };
            inputField.navigation = none;
        }
        else if (fadeUI.canvasGroup.alpha == 1)
        {
            Navigation automatic = new Navigation { mode = Navigation.Mode.Automatic };
            inputField.navigation = automatic;

            ItemOptionsInput();
        }
    }

    void ItemOptionsInput()
    {
        #region Gamepad Input

        if (inventoryM.cc.controllerType != ThirdPersonInput.ControllerType.MOUSEKEYBOARD)
        {
            buttonGroupAlpha.canvasGroup.alpha = 1;
            inputField.text = inputValue.ToString();
            if (cc.rpgaeIM.PlayerControls.Movement.ReadValue<Vector2>().x > 0.1f)
            {
                inputValue++;
                increase.color = Color.red;
            }
            else if (cc.rpgaeIM.PlayerControls.Movement.ReadValue<Vector2>().x < -0.1f)
            {
                inputValue--;
                decrease.color = Color.red;
            }
            else
            {
                decrease.color = Color.white;
                increase.color = Color.white;
            }
        }
        else
            buttonGroupAlpha.canvasGroup.alpha = 0;

        #endregion

        if (cc.rpgaeIM.PlayerControls.Action.triggered && fadeUI.canvasGroup.alpha == 1)
        {
            enteredNum = inputField.text;

            if (int.TryParse(enteredNum, out parsedNum))
            {
                int intNumber = parsedNum;
            }
            
            #region Calculate Buy

            if (inputFieldType == InputFieldType.Buy)
            {
                switch (buyItemM.buyItems.itemData[buyItemM.buyItems.buySlot].itemType)
                {
                    case ItemData.ItemType.Weapon:
                        CalculateBuyItem(ref inventoryM.weaponInv.image, ref buyItemM.buyItems.inStockCounter, ref buyItemM.buyItems.buySlot);
                        break;
                    case ItemData.ItemType.Bow:
                        CalculateBuyItem(ref inventoryM.bowAndArrowInv.image, ref buyItemM.buyItems.inStockCounter, ref buyItemM.buyItems.buySlot);
                        break;
                    case ItemData.ItemType.Arrow:
                        CalculateBuyItem(ref inventoryM.bowAndArrowInv.image, ref buyItemM.buyItems.inStockCounter, ref buyItemM.buyItems.buySlot);
                        break;
                    case ItemData.ItemType.Shield:
                        CalculateBuyItem(ref inventoryM.shieldInv.image, ref buyItemM.buyItems.inStockCounter, ref buyItemM.buyItems.buySlot);
                        break;
                    case ItemData.ItemType.Armor:
                        CalculateBuyItem(ref inventoryM.armorInv.image, ref buyItemM.buyItems.inStockCounter, ref buyItemM.buyItems.buySlot);
                        break;
                    case ItemData.ItemType.Material:
                        CalculateBuyItem(ref inventoryM.materialInv.image, ref buyItemM.buyItems.inStockCounter, ref buyItemM.buyItems.buySlot);
                        break;
                    case ItemData.ItemType.Healing:
                        CalculateBuyItem(ref inventoryM.healingInv.image, ref buyItemM.buyItems.inStockCounter, ref buyItemM.buyItems.buySlot);
                        break;
                    case ItemData.ItemType.Key:
                        CalculateBuyItem(ref inventoryM.keyInv.image, ref buyItemM.buyItems.inStockCounter, ref buyItemM.buyItems.buySlot);
                        break;
                }
            }

            #endregion

            #region Calculate Sell

            if (inputFieldType == InputFieldType.Sell)
            {
                switch (inventoryM.inventoryS)
                {
                    case InventoryManager.InventorySection.Weapon:
                        CalculatSellItem(ref inventoryM.weaponInv.itemData, ref inventoryM.weaponInv.image, ref inventoryM.weaponInv.highLight, ref inventoryM.weaponInv.outLineBorder, ref inventoryM.weaponInv.statValueBG, ref inventoryM.weaponInv.statValue, 
                        ref inventoryM.weaponInv.quantity, ref inventoryM.weaponInv.slotNum, ref inventoryM.weaponInv.counter, ref inventoryM.weaponInv.statCounter, ref inventoryM.weaponInv.removeNullSlots);
                        break;
                    case InventoryManager.InventorySection.BowAndArrow:
                        CalculatSellItem(ref inventoryM.bowAndArrowInv.itemData, ref inventoryM.bowAndArrowInv.image, ref inventoryM.bowAndArrowInv.highLight, ref inventoryM.bowAndArrowInv.outLineBorder, ref inventoryM.bowAndArrowInv.statValueBG, ref inventoryM.bowAndArrowInv.statValue,
                        ref inventoryM.bowAndArrowInv.quantity, ref inventoryM.bowAndArrowInv.slotNum, ref inventoryM.bowAndArrowInv.counter, ref inventoryM.bowAndArrowInv.statCounter, ref inventoryM.bowAndArrowInv.removeNullSlots);
                        break;
                    case InventoryManager.InventorySection.Shield:
                        CalculatSellItem(ref inventoryM.shieldInv.itemData, ref inventoryM.shieldInv.image, ref inventoryM.shieldInv.highLight, ref inventoryM.shieldInv.outLineBorder, ref inventoryM.shieldInv.statValueBG, ref inventoryM.shieldInv.statValue,
                        ref inventoryM.shieldInv.quantity, ref inventoryM.shieldInv.slotNum, ref inventoryM.shieldInv.counter, ref inventoryM.shieldInv.statCounter, ref inventoryM.shieldInv.removeNullSlots);
                        break;
                    case InventoryManager.InventorySection.Armor:
                        CalculatSellItem(ref inventoryM.armorInv.itemData, ref inventoryM.armorInv.image, ref inventoryM.armorInv.highLight, ref inventoryM.armorInv.outLineBorder, ref inventoryM.armorInv.statValueBG, ref inventoryM.armorInv.statValue,
                        ref inventoryM.armorInv.quantity, ref inventoryM.armorInv.slotNum, ref inventoryM.armorInv.counter, ref inventoryM.armorInv.statCounter, ref inventoryM.armorInv.removeNullSlots);
                        break;
                    case InventoryManager.InventorySection.Material:
                        CalculatSellItem(ref inventoryM.materialInv.itemData, ref inventoryM.materialInv.image, ref inventoryM.materialInv.highLight, ref inventoryM.materialInv.outLineBorder, ref inventoryM.materialInv.statValueBG, ref inventoryM.materialInv.statValue,
                        ref inventoryM.materialInv.quantity, ref inventoryM.materialInv.slotNum, ref inventoryM.materialInv.counter, ref inventoryM.materialInv.statCounter, ref inventoryM.materialInv.removeNullSlots);
                        break;
                    case InventoryManager.InventorySection.Healing:
                        CalculatSellItem(ref inventoryM.healingInv.itemData, ref inventoryM.materialInv.image, ref inventoryM.healingInv.highLight, ref inventoryM.healingInv.outLineBorder, ref inventoryM.healingInv.statValueBG, ref inventoryM.healingInv.statValue,
                        ref inventoryM.healingInv.quantity, ref inventoryM.healingInv.slotNum, ref inventoryM.materialInv.counter, ref inventoryM.healingInv.statCounter, ref inventoryM.healingInv.removeNullSlots);
                        break;
                    default:
                        break;
                }
            }

            #endregion

            #region Calculate Drop 

            if (inputFieldType == InputFieldType.Drop)
            {
                switch (inventoryM.inventoryS)
                {
                    case InventoryManager.InventorySection.Weapon:
                        CalculateDropItem(ref inventoryM.weaponInv.removeNullSlots, ref inventoryM.weaponInv.counter, ref inventoryM.weaponInv.statCounter, ref inventoryM.weaponInv.slotNum, ref inventoryM.weaponInv.quantity, 
                        ref inventoryM.weaponInv.statValue, ref inventoryM.weaponInv.highLight, ref inventoryM.weaponInv.outLineBorder, ref inventoryM.weaponInv.statValueBG, ref inventoryM.weaponInv.image, ref inventoryM.weaponInv.itemData);
                        break;
                    case InventoryManager.InventorySection.BowAndArrow:
                        CalculateDropItem(ref inventoryM.bowAndArrowInv.removeNullSlots, ref inventoryM.bowAndArrowInv.counter, ref inventoryM.bowAndArrowInv.statCounter, ref inventoryM.bowAndArrowInv.slotNum, ref inventoryM.bowAndArrowInv.quantity,
                        ref inventoryM.bowAndArrowInv.statValue, ref inventoryM.bowAndArrowInv.highLight, ref inventoryM.bowAndArrowInv.outLineBorder, ref inventoryM.bowAndArrowInv.statValueBG, ref inventoryM.bowAndArrowInv.image, ref inventoryM.bowAndArrowInv.itemData);
                        break;
                    case InventoryManager.InventorySection.Shield:
                        CalculateDropItem(ref inventoryM.shieldInv.removeNullSlots, ref inventoryM.shieldInv.counter, ref inventoryM.shieldInv.statCounter, ref inventoryM.shieldInv.slotNum, ref inventoryM.shieldInv.quantity,
                        ref inventoryM.shieldInv.statValue, ref inventoryM.shieldInv.highLight, ref inventoryM.shieldInv.outLineBorder, ref inventoryM.shieldInv.statValueBG, ref inventoryM.shieldInv.image, ref inventoryM.shieldInv.itemData);
                        break;
                    case InventoryManager.InventorySection.Material:
                        CalculateDropItem(ref inventoryM.materialInv.removeNullSlots, ref inventoryM.shieldInv.counter, ref inventoryM.materialInv.statCounter, ref inventoryM.materialInv.slotNum, ref inventoryM.materialInv.quantity,
                        ref inventoryM.materialInv.statValue, ref inventoryM.materialInv.highLight, ref inventoryM.materialInv.outLineBorder, ref inventoryM.shieldInv.statValueBG, ref inventoryM.materialInv.image, ref inventoryM.materialInv.itemData);
                        break;
                    case InventoryManager.InventorySection.Healing:
                        CalculateDropItem(ref inventoryM.healingInv.removeNullSlots, ref inventoryM.healingInv.counter, ref inventoryM.healingInv.statCounter, ref inventoryM.healingInv.slotNum, ref inventoryM.healingInv.quantity,
                        ref inventoryM.healingInv.statValue, ref inventoryM.healingInv.highLight, ref inventoryM.healingInv.outLineBorder, ref inventoryM.healingInv.statValueBG, ref inventoryM.healingInv.image, ref inventoryM.healingInv.itemData);
                        break;
                    default:
                        break;
                }
            }

            #endregion

            if(cc.controllerType != ThirdPersonInput.ControllerType.MOUSEKEYBOARD)
            {
                buyM.buyItems.buySlot = 0;
            }

            inputValue = 0;
            inputFieldType = InputFieldType.Null;
            fadeUI.canvasGroup.alpha = 0;
        }
    }

    #region Buy Option

    public void CalculateBuyItemNonStackle(ref Image[] inventoryImages, ref int[] inStockCounter, ref int inventoryBuySlots)
    {
        // You need inventory slots!
        if (inventoryImages.Length == 0 || inventoryImages[0] == null)
        {
            cc.infoMessage.info.text = "You don't have enough inventory space.";
            return;
        }

        // The inventory is full!
        int size = inventoryImages.Length - 1;
        if (inventoryImages[size].enabled)
        {
            cc.infoMessage.info.text = "You don't have enough inventory space.";
            return;
        }

        GameObject buyItem = Instantiate(buyItemM.buyItems.itemData[inventoryBuySlots].gameObject) as GameObject;
        buyItem.gameObject.SetActive(true);

        if (buyItem.GetComponentInChildren<ItemData>().buyPrice > inventoryM.unityCoins)
        {
            infoMessage.info.text = "You don't have enough coins.";
            Destroy(buyItem.gameObject);
            return;
        }

        inventoryM.unityCoins -= buyItem.GetComponentInChildren<ItemData>().buyPrice;
        inStockCounter[inventoryBuySlots] -= 1;

        switch (buyItem.GetComponentInChildren<ItemData>().itemType)
        {
            case ItemData.ItemType.Weapon:
                inventoryM.AddItemDataSlot(ref inventoryM.weaponInv.itemData, ref inventoryM.weaponInv.image, ref buyItem);
                break;
            case ItemData.ItemType.Bow:
                inventoryM.AddItemDataSlot(ref inventoryM.bowAndArrowInv.itemData, ref inventoryM.bowAndArrowInv.image, ref buyItem);
                break;
            case ItemData.ItemType.Arrow:
                inventoryM.AddItemDataSlot(ref inventoryM.bowAndArrowInv.itemData, ref inventoryM.bowAndArrowInv.image, ref buyItem);
                break;
            case ItemData.ItemType.Shield:
                inventoryM.AddItemDataSlot(ref inventoryM.shieldInv.itemData, ref inventoryM.shieldInv.image, ref buyItem);
                break;
            case ItemData.ItemType.Armor:
                inventoryM.AddItemDataSlot(ref inventoryM.armorInv.itemData, ref inventoryM.armorInv.image, ref buyItem);
                break;
            case ItemData.ItemType.Material:
                inventoryM.AddItemDataSlot(ref inventoryM.materialInv.itemData, ref inventoryM.materialInv.image, ref buyItem);
                break;
            case ItemData.ItemType.Healing:
                inventoryM.AddItemDataSlot(ref inventoryM.healingInv.itemData, ref inventoryM.healingInv.image, ref buyItem);
                break;
            case ItemData.ItemType.Key:
                inventoryM.AddItemDataSlot(ref inventoryM.keyInv.itemData, ref inventoryM.keyInv.image, ref buyItem);
                break;
        }
        buyItem.GetComponentInChildren<ItemData>().inInventory = true;

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

        if (inStockCounter[inventoryBuySlots] <= 0)
        {
            buyItemM.itemMarketImage.sprite = null;
            buyItemM.itemMarketImage.enabled = false;
            buyItemM.itemMarketName.text = null;
            buyItemM.itemMarketDescription.text = null;
            buyItemM.itemMarketPrice.text = null;
            buyItemM.itemMarketInInventory.text = null;
            buyItemM.sparkleParticle.Stop();
        }
    }

    protected void CalculateBuyItem (ref Image[] inventoryImages, ref int[] inStockCounter, ref int inventoryBuySlots)
    {
        if (inputFieldType == InputFieldType.Buy)
        {
            // You need inventory slots!
            if (inventoryImages.Length == 0 || inventoryImages[0] == null)
            {
                cc.infoMessage.info.text = "You don't have enough inventory space.";
                return;
            }
            // The inventory is full!
            int size = inventoryImages.Length - 1;
            if (inventoryImages[size].enabled)
            {
                cc.infoMessage.info.text = "You don't have enough inventory space.";
                return;
            }

            GameObject buyItem = Instantiate(buyItemM.buyItems.itemData[inventoryBuySlots].gameObject) as GameObject;
            buyItem.gameObject.SetActive(true);

            if (parsedNum >= inStockCounter[inventoryBuySlots])
                parsedNum = inStockCounter[inventoryBuySlots];

            int parsedDoubled = parsedNum;
            parsedDoubled *= buyItem.GetComponentInChildren<ItemData>().buyPrice;

            if (parsedDoubled > inventoryM.unityCoins)
            {
                infoMessage.info.text = "You don't have enough coins.";
                Destroy(buyItem.gameObject);
                return;
            }

            if (itemPurchasedAS)
                itemPurchasedAS.PlayRandomClip();

            inventoryM.unityCoins -= parsedDoubled;
            inStockCounter[inventoryBuySlots] -= parsedNum;
            buyItem.GetComponentInChildren<ItemData>().quantity *= parsedNum;
            inputField.text = "";

            switch (buyItem.GetComponentInChildren<ItemData>().itemType)
            {
                case ItemData.ItemType.Weapon:
                    inventoryM.AddItemDataSlot(ref inventoryM.weaponInv.itemData, ref inventoryM.weaponInv.image, ref buyItem);
                    break;
                case ItemData.ItemType.Bow:
                    inventoryM.AddItemDataSlot(ref inventoryM.bowAndArrowInv.itemData, ref inventoryM.weaponInv.image, ref buyItem);
                    break;
                case ItemData.ItemType.Arrow:
                    inventoryM.AddItemDataSlot(ref inventoryM.bowAndArrowInv.itemData, ref inventoryM.bowAndArrowInv.image, ref buyItem);
                    break;
                case ItemData.ItemType.Shield:
                    inventoryM.AddItemDataSlot(ref inventoryM.shieldInv.itemData, ref inventoryM.shieldInv.image, ref buyItem);
                    break;
                case ItemData.ItemType.Armor:
                    inventoryM.AddItemDataSlot(ref inventoryM.armorInv.itemData, ref inventoryM.armorInv.image, ref buyItem);
                    break;
                case ItemData.ItemType.Material:
                    inventoryM.AddItemDataSlot(ref inventoryM.materialInv.itemData, ref inventoryM.materialInv.image, ref buyItem);
                    break;
                case ItemData.ItemType.Healing:
                    inventoryM.AddItemDataSlot(ref inventoryM.healingInv.itemData, ref inventoryM.healingInv.image, ref buyItem);
                    break;
                case ItemData.ItemType.Key:
                    inventoryM.AddItemDataSlot(ref inventoryM.keyInv.itemData, ref inventoryM.keyInv.image, ref buyItem);
                    break;
            }
            buyItem.GetComponentInChildren<ItemData>().inInventory = true;

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

            if (inStockCounter[inventoryBuySlots] <= 0)
            {
                buyItemM.itemMarketImage.sprite = null;
                buyItemM.itemMarketImage.enabled = false;
                buyItemM.itemMarketName.text = null;
                buyItemM.itemMarketDescription.text = null;
                buyItemM.itemMarketPrice.text = null;
                buyItemM.itemMarketInInventory.text = null;
                buyItemM.sparkleParticle.Stop();
            }
        }
    }

    #endregion

    #region Sell Option

    public void CalculatSellItem(ref ItemData[] itemID, ref Image[] itemImage, ref Image[] highLight, ref Image[] outline, ref Image[] statBG, ref TextMeshProUGUI[] statValue, ref TextMeshProUGUI[] itemQuantity,
        ref int inventorySlot, ref int[] itemCounter, ref int[] statCounter, ref int removeNullSlots)
    {
        if (inputFieldType == InputFieldType.Sell)
        {
            if (parsedNum >= itemCounter[inventorySlot])
                parsedNum = itemCounter[inventorySlot];

            int parsedDoubled = parsedNum;
            parsedDoubled *= itemID[inventorySlot].sellPrice;
            inventoryM.unityCoins += parsedDoubled;
            inputField.text = "";

            inventoryM.RemoveStackableItem(ref itemID, ref itemImage,
            ref highLight, ref outline, ref statBG, ref itemQuantity, ref statValue, ref inventorySlot, ref itemCounter,
            ref statCounter, ref removeNullSlots, parsedNum);
        }
    }

    #endregion

    #region Drop Option

    protected void CalculateDropItem(ref int removeNull, ref int[] inventoryCounter, ref int[] statCounter, ref int inventorySlot, ref TextMeshProUGUI[] inventoryQuantity, ref TextMeshProUGUI[] statValue, ref Image[] lightImages, ref Image[] outline, ref Image[] statBG, ref Image[] itemImages, ref ItemData[] itemID)
    {
        if (inputFieldType == InputFieldType.Drop)
        {
            if (parsedNum >= inventoryCounter[inventorySlot])
                parsedNum = inventoryCounter[inventorySlot];

            GameObject drop = Instantiate(dropItem, dropTarget.transform.position + new Vector3(Random.Range(-0.8f, 0.8f), 1f,
            Random.Range(-0.8f, 0.8f)), Quaternion.Euler(0, 0, 90)) as GameObject;
            drop.GetComponentInChildren<ItemData>().quantity = parsedNum;
            drop.GetComponentInChildren<ItemData>().inInventory = false;
            drop.GetComponentInChildren<ItemData>().itemActive = true;
            drop.SetActive(true);

            inventoryM.RemoveStackableItem(ref itemID, ref itemImages,
            ref lightImages, ref outline, ref statBG, ref inventoryQuantity, ref statValue, ref inventorySlot, ref inventoryCounter,
            ref statCounter, ref removeNull, parsedNum);
            return;
        }
    }

    #endregion
}
