using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;
using RPGAE.CharacterController;

public class BuyStoreManager : MonoBehaviour
{
    [Serializable]
    public class BuyItems
    {
        public int buySlot = 0;
        public bool[] selectSlot;
        public int[] inStockCounter = new int[0];
        public Image[] itemImage = new Image[0];
        public Image[] highLight = new Image[0];
        public TextMeshProUGUI[] itemName = new TextMeshProUGUI[0];
        public TextMeshProUGUI[] inStock = new TextMeshProUGUI[0];
        public TextMeshProUGUI[] prices = new TextMeshProUGUI[0];
        public ItemData[] itemData = new ItemData[0];
    }
    [Header("BUY SETTINGS")]
    public GameObject buySlotPrefab;
    public Color neutralColor = new Color32(0, 0, 0, 191);
    public Color selectColor = new Color32(171, 171, 171, 191);

    [Header("UI")]
    public GameObject[] hideUI;
    public GameObject[] showUI;
    public GameObject[] normalizeShow;
    public GameObject[] normalizeHide; 

    [Header("REFERENCES")]
    public FadeUI fadeUI;
    public FadeUI storeItemPreviewAlpha;
    public Image itemMarketImage;
    public TextMeshProUGUI itemMarketName;
    public TextMeshProUGUI itemMarketDescription;
    public TextMeshProUGUI itemMarketPrice;
    public TextMeshProUGUI itemMarketInInventory;
    public Animation storeItemPreviewAnim;
    public ParticleSystem sparkleParticle;

    #region Private

    [HideInInspector] public BuyItems buyItems;
    private MiniMap miniMap;
    private StoreManager storeM;
    private ThirdPersonController cc;
    private InventoryManager inventoryM;
    private bool isDone;
    public static int numBuySlots;
    [HideInInspector] public int selectedBuySlot;
    public bool isActive;
    private bool previouslyActive;

    #endregion

    // Use this for initialization
    void Start()
    {
        isActive = false;
        sparkleParticle.Stop();

        // Get Components
        fadeUI = GetComponent<FadeUI>();
        miniMap = FindObjectOfType<MiniMap>();
        inventoryM = FindObjectOfType<InventoryManager>();
        cc = FindObjectOfType<ThirdPersonController>();
        storeM = FindObjectOfType<StoreManager>();

        if(sparkleParticle.isPlaying) 
            sparkleParticle.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            // Update instock value
            if (buyItems.itemData.Length > 0)
                buyItems.inStock[buyItems.buySlot].text = "x" + buyItems.inStockCounter[buyItems.buySlot].ToString();
            foreach (GameObject hide in hideUI)
            {
                hide.SetActive(false);
            }
            foreach (GameObject show in showUI)
            {
                show.SetActive(true);
            }

            if (fadeUI.canvasGroup.alpha == 0)
            {
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
                isActive = false;
                inventoryM.storeM.isActive = false;
                cc.canMove = true;
                ResetPreviewValues();
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
                isActive = false;
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

    public void AddItem(ItemData itemToAdd)
    {
        GameObject buySlot = Instantiate(buySlotPrefab) as GameObject;
        BuySlot buyData = buySlot.GetComponent<BuySlot>();
        numBuySlots += 1;
        buyData.SlotNum = numBuySlots - 1;

        int[] temp0 = new int[buyItems.itemData.Length + 1];
        Image[] temp1 = new Image[buyItems.itemData.Length + 1];
        Image[] temp2 = new Image[buyItems.itemData.Length + 1];
        TextMeshProUGUI[] temp3 = new TextMeshProUGUI[buyItems.itemData.Length + 1];
        TextMeshProUGUI[] temp4 = new TextMeshProUGUI[buyItems.itemData.Length + 1];
        TextMeshProUGUI[] temp5 = new TextMeshProUGUI[buyItems.itemData.Length + 1];
        ItemData[] temp6 = new ItemData[buyItems.itemData.Length + 1];
        bool[] temp7 = new bool[buyItems.itemData.Length + 1];

        for (int i = 0; i < buyItems.itemData.Length; i++)
        {
            if (buyItems.itemData[0] != null)
            {
                temp0[i] = buyItems.inStockCounter[i];
                temp1[i] = buyItems.itemImage[i];
                temp2[i] = buyItems.highLight[i];
                temp3[i] = buyItems.itemName[i];
                temp4[i] = buyItems.inStock[i];
                temp5[i] = buyItems.prices[i];
                temp6[i] = buyItems.itemData[i];
                temp7[i] = buyItems.selectSlot[i];
            }
        }

        buyItems.inStockCounter = temp0;
        buyItems.itemImage = temp1;
        buyItems.highLight = temp2;
        buyItems.itemName = temp3;
        buyItems.inStock = temp4;
        buyItems.prices = temp5;
        buyItems.itemData = temp6;
        buyItems.selectSlot = temp7;

        buyItems.inStockCounter[numBuySlots - 1] = itemToAdd.inStockQuantity;
        buyItems.itemImage[numBuySlots - 1] = buyData.itemImage;
        buyItems.highLight[numBuySlots - 1] = buyData.highLight;
        buyItems.itemName[numBuySlots - 1] = buyData.itemName;
        buyItems.inStock[numBuySlots - 1] = buyData.itemStock;
        buyItems.prices[numBuySlots - 1] = buyData.itemPrice;
        buyItems.itemData[numBuySlots - 1] = itemToAdd;

        buyData.itemName.text = itemToAdd.itemName;
        buyData.itemImage.sprite = itemToAdd.itemSprite;
        buyData.itemStock.text = "x" + itemToAdd.inStockQuantity.ToString();
        buyData.itemPrice.text = "x" + itemToAdd.buyPrice.ToString(); 
    }

    public void CheckInventoryItems(ref ItemData[] itemID, ref int[] itemCounter, ref string _itemName)
    {
        for (int i = 0; i < itemID.Length; i++)
        {
            if (itemID[i] != null)
            {
                if(itemID[i].itemName == _itemName)
                {
                    itemMarketInInventory.text = "x" + itemCounter[i] + " In Inventory";
                }
                else if (itemID[i].itemName != _itemName)
                {
                    itemMarketInInventory.text = "x" + 0 + " In Inventory";
                }
            }
        }
        if (itemID.Length == 0)
            itemMarketInInventory.text = "x" + 0 + " In Inventory";
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
    }
}
