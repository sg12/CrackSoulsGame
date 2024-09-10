using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class AddItemsToList : MonoBehaviour {

    public float timer = 5f;

    [Header("REFERENCES")]
    public FadeUI fadeUI;
    public Image itemImage;
    public Image lvlArrow;
    public RectTransform lvlArrowRect;
    public Animator lvlArrowAnim;
    public Animator addedItemIConAnim;
    public RectTransform rectTransform;
    public TextMeshProUGUI itemNameText;

    #region Private 

    // Private 
    private int numberInList;
    private GameObject panel;
    private InventoryManager inventoryM;
    private AddedItemListManager addedItemListM;
    [HideInInspector] public ItemData tempItemData;
    [HideInInspector] public string itemName;
    [HideInInspector] public Sprite itemSprite;

    #endregion

    // Use this for initialization
    void Start ()
    {
        panel = GameObject.Find("AddedItemIconPanel");
        transform.SetParent(panel.transform);

        // Get Components
        fadeUI = GetComponent<FadeUI>();
        addedItemIConAnim = GetComponentInChildren<Animator>();
        addedItemListM = FindObjectOfType<AddedItemListManager>();
        inventoryM = FindObjectOfType<InventoryManager>();
        rectTransform.localScale = new Vector3(1, 1, 1);

        InitInventoryItems();
    }

    // Update is called once per frame
    void Update ()
    {
        TextSize();
        FadeTimer();
    }

    void InitInventoryItems()
    {
        switch (addedItemListM.itemAdded.Count)
        {
            case 1:
                numberInList = 1;
                break;
            case 2:
                numberInList = 2;
                break;
            case 3:
                numberInList = 3;
                break;
            default:
                numberInList = 3;
                break;
        }

        switch (tempItemData.itemType)
        {
            case ItemData.ItemType.Weapon:
                CheckInventoryItems(ref inventoryM.weaponInv.itemData);
                break;
            case ItemData.ItemType.Bow:
                CheckInventoryItems(ref inventoryM.bowAndArrowInv.itemData);
                break;
            case ItemData.ItemType.Shield:
                CheckInventoryItems(ref inventoryM.shieldInv.itemData);
                break;
            case ItemData.ItemType.Armor:
                CheckInventoryItems(ref inventoryM.armorInv.itemData);
                break;
            case ItemData.ItemType.Material:
                CheckInventoryItems(ref inventoryM.materialInv.itemData);
                break;
            case ItemData.ItemType.Healing:
                CheckInventoryItems(ref inventoryM.healingInv.itemData);
                break;
            case ItemData.ItemType.Key:
                CheckInventoryItems(ref inventoryM.keyInv.itemData);
                break;
        }
    }

    void TextSize()
    {
        if (gameObject.name == itemName + " AddedToList")
        {
            itemImage.sprite = itemSprite;
            if (tempItemData.quantity > 0)
            {
                itemNameText.text = itemName + " x" + tempItemData.quantity;

                if (itemName.Length > 15)
                    itemNameText.fontSize = 16;
                else
                    itemNameText.fontSize = 18;
            }
            else
            {
                itemNameText.text = itemName;
            }
        }
    }

    void FadeTimer()
    {
        if (timer > 0f) timer -= Time.deltaTime;
        if (timer < 0f) timer = 0f;

        if (inventoryM.referencesObj.pauseMenu.GetComponent<FadeUI>().canvasGroup.alpha == 1)
            fadeUI.canvasGroup.alpha = 0;
        else if (fadeUI.canvasGroup.alpha == 0 && timer > 0.4f)
            fadeUI.canvasGroup.alpha = 1;

        if (timer < 0.4f)
        {
            fadeUI.FadeTransition(0, 0, 0.2f);
            if (timer == 0f)
            {
                addedItemListM.itemAdded.Remove("Item");
                DestroyImmediate(this.gameObject);
            }
        }

        if (addedItemListM.itemAdded.Count > 3)
        {
            if (numberInList == 3)
            {
                fadeUI.FadeTransition(0, 0, 0.2f);
                addedItemListM.itemAdded.Remove("Item");
                Destroy(gameObject, 0.1f);
            }
        }
    }

    void CheckInventoryItems(ref ItemData[] itemData)
    {
        for (int i = 0; i < itemData.Length; i++)
        {
            if (itemData[i] != null)
            {
                if (tempItemData.itemType == ItemData.ItemType.Weapon)
                {
                    if (itemData.Length > 1)
                    {
                        if (tempItemData.maxWpnAtk >= itemData[i].maxWpnAtk)
                        {
                            lvlArrow.enabled = true;
                            lvlArrow.color = Color.green;
                            lvlArrowRect.localRotation = Quaternion.Euler(0, 0, -180);
                        }
                        else if (tempItemData.maxWpnAtk < itemData[i].maxWpnAtk)
                        {
                            lvlArrow.enabled = true;
                            lvlArrow.color = Color.red;
                            lvlArrowRect.localRotation = Quaternion.Euler(0, 0, 0);
                        }
                    }
                    else
                        lvlArrow.enabled = false;
                }
                if (tempItemData.itemType == ItemData.ItemType.Bow)
                {
                    if (itemData.Length > 1)
                    {
                        if (tempItemData.maxBowAtk >= itemData[i].maxBowAtk)
                        {
                            lvlArrow.enabled = true;
                            lvlArrow.color = Color.green;
                            lvlArrowRect.localRotation = Quaternion.Euler(0, 0, -180);
                            return;
                        }
                        else if (tempItemData.maxBowAtk < itemData[i].maxBowAtk)
                        {
                            lvlArrow.color = Color.red;
                            lvlArrowRect.localRotation = Quaternion.Euler(0, 0, 0);
                            return;
                        }
                    }
                    else
                        lvlArrow.enabled = false;
                }
                if (tempItemData.itemType == ItemData.ItemType.Arrow)
                {
                    lvlArrow.enabled = false;
                }
                if (tempItemData.itemType == ItemData.ItemType.Shield)
                {
                    if (itemData.Length > 1)
                    {
                        if (tempItemData.maxShdAtk >= itemData[i].maxShdAtk)
                        {
                            lvlArrow.enabled = true;
                            lvlArrow.color = Color.green;
                            lvlArrowRect.localRotation = Quaternion.Euler(0, 0, -180);
                            return;
                        }
                        else if (tempItemData.maxShdAtk < itemData[i].maxShdAtk)
                        {
                            lvlArrow.enabled = true;
                            lvlArrow.color = Color.red;
                            lvlArrowRect.localRotation = Quaternion.Euler(0, 0, 0);
                            return;
                        }
                        else
                            lvlArrow.enabled = false;
                    }
                    else
                        lvlArrow.enabled = false;
                }
                if (tempItemData.itemType == ItemData.ItemType.Material)
                {
                    lvlArrow.enabled = false;
                }
                if (tempItemData.itemType == ItemData.ItemType.Healing)
                {
                    lvlArrow.enabled = false;
                }
                if (tempItemData.itemType == ItemData.ItemType.General)
                {
                    lvlArrow.enabled = false;
                }
                if (tempItemData.itemType == ItemData.ItemType.Key)
                {
                    lvlArrow.enabled = false;
                }
            }
        }
    }
}
