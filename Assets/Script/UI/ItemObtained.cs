using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ItemObtained : MonoBehaviour
{
    [Header("AUDIO")]
    public AudioSource getCommonItem;
    public AudioSource getRareItem;

    [Header("REFERENCES")]
    public FadeUI fadeUI;
    public Animator animEffect;

    public Image itemImage;
    public TextMeshProUGUI itemName;

    #region Private

    private bool inProcess;
    [HideInInspector] public bool isActive;

    private InventoryManager inventoryM;
    [HideInInspector] public ItemData itemData;
    [HideInInspector] public ItemInteract itemInteract;

    [HideInInspector] public ItemType itemType;
    public enum ItemType
    {
        Weapon,
        Bow,
        Arrow,
        Shield,
        Armor,
        Material,
        Healing,
        Key,
        Null
    }

    [HideInInspector] public string sceneName;
    [HideInInspector] public int SpawnPointIDRef;
    [HideInInspector] public Color transitionScreenColor;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Get Components
        inventoryM = FindObjectOfType<InventoryManager>();
        fadeUI = GetComponent<FadeUI>();
        fadeUI.canvasGroup.alpha = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (itemData != null)
        {
            itemName.text = itemData.itemName;
            itemImage.sprite = itemData.itemSprite;

            if (!isActive)
                StartCoroutine(StartUI(0.9f));

            ObtainItem();
        }
    }

    public IEnumerator StartUI(float delay)
    {
        isActive = true;
        inProcess = true;
        yield return new WaitForSeconds(delay);
        isActive = true;
        animEffect.SetBool("Active", isActive);
        fadeUI.FadeTransition(1, 0, 0.2f);
        if (inventoryM.miniMap.fadeUI.canvasGroup.alpha == 1 && !inventoryM.isPauseMenuOn)
            inventoryM.miniMap.fadeUI.FadeTransition(0, 0, 0.2f);

        inventoryM.UIcanvas.renderMode = RenderMode.ScreenSpaceCamera;

        if (itemData.rankNum == 3)
        {
            if (getRareItem)
                getRareItem.Play();
        }
        else
        {
            if (getCommonItem)
                getCommonItem.Play();
        }
    }

    void ObtainItem()
    {
        if (fadeUI.canvasGroup.alpha == 1)
        {
            #region Add To Inventory

            GameObject itemDataConvert = itemData.gameObject;

            if (inProcess)
            {
                if (itemData.itemType == ItemData.ItemType.Weapon)
                    inventoryM.AddItemDataSlot(ref inventoryM.weaponInv.itemData, ref inventoryM.weaponInv.image, ref itemDataConvert);
                if (itemData.itemType == ItemData.ItemType.Bow || itemData.itemType == ItemData.ItemType.Arrow)
                    inventoryM.AddItemDataSlot(ref inventoryM.bowAndArrowInv.itemData, ref inventoryM.bowAndArrowInv.image, ref itemDataConvert);
                if (itemData.itemType == ItemData.ItemType.Shield)
                    inventoryM.AddItemDataSlot(ref inventoryM.shieldInv.itemData, ref inventoryM.shieldInv.image, ref itemDataConvert);
                if (itemData.itemType == ItemData.ItemType.Armor)
                    inventoryM.AddItemDataSlot(ref inventoryM.armorInv.itemData, ref inventoryM.armorInv.image, ref itemDataConvert);
                if (itemData.itemType == ItemData.ItemType.Material)
                    inventoryM.AddItemDataSlot(ref inventoryM.materialInv.itemData, ref inventoryM.materialInv.image, ref itemDataConvert);
                if (itemData.itemType == ItemData.ItemType.Healing)
                    inventoryM.AddItemDataSlot(ref inventoryM.healingInv.itemData, ref inventoryM.healingInv.image, ref itemDataConvert);
                if (itemData.itemType == ItemData.ItemType.Key)
                    inventoryM.AddItemDataSlot(ref inventoryM.keyInv.itemData, ref inventoryM.keyInv.image, ref itemDataConvert);
            }

            #endregion

            inProcess = false;
            fadeUI.FadeTransition(0, 3, 0.5f);
        }
        if (fadeUI.canvasGroup.alpha == 0 && !inProcess)
        {
            isActive = false;
            itemData = null;
            itemInteract = null;
            if (inventoryM.miniMap.fadeUI.canvasGroup.alpha == 0 && !inventoryM.isPauseMenuOn)
                inventoryM.miniMap.fadeUI.FadeTransition(1, 0, 0.5f);
            if (!inventoryM.isPauseMenuOn)
                inventoryM.UIcanvas.renderMode = RenderMode.ScreenSpaceOverlay;

            if (sceneName != "")
               inventoryM.systemM.canLoadScene = true;
        }
    }
}
