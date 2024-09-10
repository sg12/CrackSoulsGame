using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using RPGAE.CharacterController;

public class InventorySlot : MonoBehaviour
{
    public enum SlotType
    {
        Weapon,
        BowAndArrow,
        Shield,
        Armor,
        Material,
        Healing,
        Key,
        NA
    }
    public int slotNum = 0;
    public SlotType slotType;

    [Header("AUDIO")]
    public RandomAudioPlayer menuMoveAS;

    [Header("REFERENCES")]
    public Animator outLineBorderAnim;
    public RectTransform rT;
    public Image outlineBorder;
    public Image highLight;
    public Image itemImage;
    public Image statValueBG;
    public TextMeshProUGUI statValue;
    public TextMeshProUGUI itemQuantity;
    public Button button;

    #region Private 

    private bool isDone;
    private GameObject contentPanel;
    private static Color prevHighlight;
    private Color oldHighLightColor;
    private Color highLightClickedColor;
    private InventoryManager inventoryM;
    private InputFieldOptions inputField;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        rT = GetComponent<RectTransform>();
        inventoryM = FindObjectOfType<InventoryManager>();
        inputField = FindObjectOfType<InputFieldOptions>();
        button = GetComponentInChildren<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        SetPersistentListener();
    }

    void SetPersistentListener()
    {
        if (!isDone)
        {
            switch (slotType)
            {
                case SlotType.Weapon:
                    contentPanel = GameObject.Find("WeaponContent");
                    isDone = true;
                    break;
                case SlotType.BowAndArrow:
                    contentPanel = GameObject.Find("BowAndArrowContent");
                    isDone = true;
                    break;
                case SlotType.Shield:
                    contentPanel = GameObject.Find("ShieldContent");
                    isDone = true;
                    break;
                case SlotType.Armor:
                    contentPanel = GameObject.Find("ArmorContent");
                    isDone = true;
                    break;
                case SlotType.Material:
                    contentPanel = GameObject.Find("MaterialContent");
                    isDone = true;
                    break;
                case SlotType.Healing:
                    contentPanel = GameObject.Find("HealingContent");
                    isDone = true;
                    break;
                case SlotType.Key:
                    contentPanel = GameObject.Find("KeyContent");
                    isDone = true;
                    break;
            }
        }
        else if (slotType != SlotType.NA && isDone)
            SetDefaultSettings();
    }

    public void OnButtonEnter(Transform slot)
    {
        if (inputField.fadeUI.canvasGroup.alpha != 0) return;

        switch (slotType)
        {
            case SlotType.Weapon:
                if (inventoryM.weaponInv.image[slotNum].enabled)
                {
                    outlineBorder.GetComponent<Image>().enabled = true;
                    outlineBorder.color = inventoryM.outLineHover;
                    inventoryM.weaponInv.slotNum = slotNum;
                    if (inventoryM.pauseMenuS == InventoryManager.PauseMenuSection.Inventory)
                        inventoryM.SetItemDescriptionSettings();
                }
                break;
            case SlotType.BowAndArrow:
                if (inventoryM.bowAndArrowInv.image[slotNum].enabled)
                {
                    outlineBorder.GetComponent<Image>().enabled = true;
                    outlineBorder.color = inventoryM.outLineHover;
                    inventoryM.bowAndArrowInv.slotNum = slotNum;
                    if (inventoryM.pauseMenuS == InventoryManager.PauseMenuSection.Inventory)
                        inventoryM.SetItemDescriptionSettings();
                }
                break;
            case SlotType.Shield:
                if (inventoryM.shieldInv.image[slotNum].enabled)
                {
                    outlineBorder.GetComponent<Image>().enabled = true;
                    outlineBorder.color = inventoryM.outLineHover;
                    inventoryM.shieldInv.slotNum = slotNum;
                    if (inventoryM.pauseMenuS == InventoryManager.PauseMenuSection.Inventory)
                        inventoryM.SetItemDescriptionSettings();
                }
                break;
            case SlotType.Armor:
                if (inventoryM.armorInv.image[slotNum].enabled)
                {
                    outlineBorder.GetComponent<Image>().enabled = true;
                    outlineBorder.color = inventoryM.outLineHover;
                    inventoryM.armorInv.slotNum = slotNum;
                    if (inventoryM.pauseMenuS == InventoryManager.PauseMenuSection.Inventory)
                        inventoryM.SetItemDescriptionSettings();
                }
                break;
            case SlotType.Material:
                if (inventoryM.materialInv.image[slotNum].enabled)
                {
                    outlineBorder.GetComponent<Image>().enabled = true;
                    outlineBorder.color = inventoryM.outLineHover;
                    inventoryM.materialInv.slotNum = slotNum;
                    if (inventoryM.pauseMenuS == InventoryManager.PauseMenuSection.Inventory)
                        inventoryM.SetItemDescriptionSettings();
                }
                break;
            case SlotType.Healing:
                if (inventoryM.healingInv.image[slotNum].enabled)
                {
                    outlineBorder.GetComponent<Image>().enabled = true;
                    outlineBorder.color = inventoryM.outLineHover;
                    inventoryM.healingInv.slotNum = slotNum;
                    if (inventoryM.pauseMenuS == InventoryManager.PauseMenuSection.Inventory)
                        inventoryM.SetItemDescriptionSettings();
                }
                break;
            case SlotType.Key:
                if (inventoryM.keyInv.image[slotNum].enabled)
                {
                    outlineBorder.GetComponent<Image>().enabled = true;
                    outlineBorder.color = inventoryM.outLineHover;
                    inventoryM.keyInv.slotNum = slotNum;
                    if (inventoryM.pauseMenuS == InventoryManager.PauseMenuSection.Inventory)
                        inventoryM.SetItemDescriptionSettings();
                }
                break;
        }
    }

    public void OnButtonSelect(Transform slot)
    {
        inventoryM.ItemSelect(slotNum);
    }

    public void OnButtonExit(Transform slot)
    {
        if (inventoryM.referencesObj.itemOptionsEquipObj && inventoryM.referencesObj.itemOptionsEquipObj.activeInHierarchy 
            || inventoryM.referencesObj.itemOptionsRemoveObj && inventoryM.referencesObj.itemOptionsRemoveObj.activeInHierarchy
            || inventoryM.referencesObj.itemOptionsDropObj && inventoryM.referencesObj.itemOptionsDropObj.activeInHierarchy
            || inventoryM.referencesObj.itemOptionsConsumeObj && inventoryM.referencesObj.itemOptionsConsumeObj.activeInHierarchy
            || inventoryM.referencesObj.itemOptionsPreviewObj && inventoryM.referencesObj.itemOptionsPreviewObj.activeInHierarchy
            || inventoryM.referencesObj.itemOptionsBuyObj && inventoryM.referencesObj.itemOptionsBuyObj.activeInHierarchy
            || inventoryM.referencesObj.itemOptionsSellObj && inventoryM.referencesObj.itemOptionsSellObj.activeInHierarchy
            || inventoryM.referencesObj.itemOptionsRepairObj && inventoryM.referencesObj.itemOptionsRepairObj.activeInHierarchy
            || inventoryM.referencesObj.itemOptionsUpgradeObj && inventoryM.referencesObj.itemOptionsUpgradeObj.activeInHierarchy)
            return;

        inventoryM.itemDescription.inventoryD = ItemDescription.InventoryDescription.Null;

        switch (slotType)
        {
            case SlotType.Weapon:
                if (inventoryM.weaponInv.image[slotNum].enabled)
                {
                    if (inventoryM.weaponInv.itemData[slotNum].equipped)
                    {
                        outlineBorder.GetComponent<Image>().enabled = true;
                        outlineBorder.color = inventoryM.outLineSelected;
                    }
                    else
                    {
                        outlineBorder.color = inventoryM.outLineHover;
                        outlineBorder.GetComponent<Image>().enabled = false;
                    }
                }
                break;
            case SlotType.BowAndArrow:
                if (inventoryM.bowAndArrowInv.image[slotNum].enabled)
                {
                    if (inventoryM.bowAndArrowInv.itemData[slotNum].equipped)
                    {
                        outlineBorder.GetComponent<Image>().enabled = true;
                        outlineBorder.color = inventoryM.outLineSelected;
                    }
                    else
                    {
                        outlineBorder.color = inventoryM.outLineHover;
                        outlineBorder.GetComponent<Image>().enabled = false;
                    }
                }
                break;
            case SlotType.Shield:
                if (inventoryM.shieldInv.image[slotNum].enabled)
                {
                    if (inventoryM.shieldInv.itemData[slotNum].equipped)
                    {
                        outlineBorder.GetComponent<Image>().enabled = true;
                        outlineBorder.color = inventoryM.outLineSelected;
                    }
                    else
                    {
                        outlineBorder.color = inventoryM.outLineHover;
                        outlineBorder.GetComponent<Image>().enabled = false;
                    }
                }
                break;
            case SlotType.Armor:
                if (inventoryM.armorInv.image[slotNum].enabled)
                {
                    outlineBorder.GetComponent<Image>().enabled = true;
                    outlineBorder.color = inventoryM.outLineSelected;
                }
                else
                {
                    outlineBorder.color = inventoryM.outLineHover;
                    outlineBorder.GetComponent<Image>().enabled = false;
                }
                break;
            case SlotType.Material:
                if (inventoryM.materialInv.image[slotNum].enabled)
                {
                    outlineBorder.color = inventoryM.outLineHover;
                    outlineBorder.GetComponent<Image>().enabled = false;
                }
                break;
            case SlotType.Healing:
                if (inventoryM.healingInv.image[slotNum].enabled)
                {
                    outlineBorder.color = inventoryM.outLineHover;
                    outlineBorder.GetComponent<Image>().enabled = false;
                }
                break;
            case SlotType.Key:
                if (inventoryM.keyInv.image[slotNum].enabled)
                {
                    outlineBorder.color = inventoryM.outLineHover;
                    outlineBorder.GetComponent<Image>().enabled = false;
                }
                break;
        }
    }

    void SetDefaultSettings()
    {
        rT.SetParent(contentPanel.transform);
        rT.localScale = new Vector3(1, 1, 1);
    }
}
