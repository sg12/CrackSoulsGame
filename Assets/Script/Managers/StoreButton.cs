using UnityEngine.UI;
using UnityEngine;
using RPGAE.CharacterController;

public class StoreButton : MonoBehaviour
{
    [Header("SETTINGS")]
    public Color neutralColor = new Color32(0, 0, 0, 191);
    public Color highLightColor = new Color32(171, 171, 171, 191);
    public Color selectedColor = new Color32(255, 0, 144, 191);

    [Header("AUDIO")]
    public RandomAudioPlayer menuMoveAS;

    // Private
    private bool itemsStocked;
    private ThirdPersonController cc;
    [HideInInspector] public StoreManager storeM;
    [HideInInspector] public BuyStoreManager buyM;
    [HideInInspector] public SellStoreManager sellM;
    [HideInInspector] public RepairStoreManager repairM;
    [HideInInspector] public UpgradeStoreManager upgradeM;
    [HideInInspector] public SpecialStoreManager specialM;
    [HideInInspector] public InventoryManager inventoryM;
    [HideInInspector] public DialogueManager dialogueM;

    // Start is called before the first frame update
    void Start()
    {
        itemsStocked = false;

        // Get Components
        storeM = FindObjectOfType<StoreManager>();
        buyM = FindObjectOfType<BuyStoreManager>();
        sellM = FindObjectOfType<SellStoreManager>();
        repairM = FindObjectOfType<RepairStoreManager>();
        upgradeM = FindObjectOfType<UpgradeStoreManager>();
        specialM = FindObjectOfType<SpecialStoreManager>();
        inventoryM = FindObjectOfType<InventoryManager>();
        dialogueM = FindObjectOfType<DialogueManager>();
        cc = FindObjectOfType<ThirdPersonController>();
    }

    public void ButtonSelect()
    {
        if (storeM.fadeUI.canvasGroup.alpha != 1) return;

        Image buttonBG = GetComponent<Image>();

        if (buttonBG.color == neutralColor) return;

        if (cc.rpgaeIM.PlayerControls.Attack.triggered || cc.rpgaeIM.PlayerControls.Action.triggered)
        {
            storeM.fadeUI.FadeTransition(0, 0, 0.5f);
            if (buttonBG.name == "BuyButton")
            {
                inventoryM.referencesObj.buyStoreM.SetActive(true);
                storeM.fadeUI.canvasGroup.alpha = 0;
                storeM.gameObject.SetActive(false);
                if (!itemsStocked)
                {
                    for (int i = 0; i < dialogueM.curDialogue.storeItemPrefab.Length; i++)
                    {
                        buyM.AddItem(dialogueM.curDialogue.storeItemPrefab[i].GetComponentInChildren<ItemData>());
                    }
                    itemsStocked = true;
                }
                buyM.isActive = true;
                buttonBG.color = neutralColor;
            }
            else if (buttonBG.name == "SellButton")
            {
                storeM.fadeUI.canvasGroup.alpha = 0;
                storeM.gameObject.SetActive(false);
                inventoryM.referencesObj.sellStoreM.SetActive(true);
                inventoryM.referencesObj.inventorySection.SetActive(true);
                inventoryM.referencesObj.inventorySection.GetComponent<FadeUI>().canvasGroup.alpha = 0;
                sellM.isActive = true;
                buttonBG.color = neutralColor;
            }
            else if (buttonBG.name == "RepairButton")
            {
                storeM.fadeUI.canvasGroup.alpha = 0;
                storeM.gameObject.SetActive(false);
                inventoryM.referencesObj.repairStoreM.SetActive(true);
                inventoryM.referencesObj.inventorySection.SetActive(true);
                inventoryM.referencesObj.inventorySection.GetComponent<FadeUI>().canvasGroup.alpha = 0;
                repairM.isActive = true;
                buttonBG.color = neutralColor;
            }
            else if (buttonBG.name == "UpgradeButton")
            {
                storeM.fadeUI.canvasGroup.alpha = 0;
                storeM.gameObject.SetActive(false);
                inventoryM.referencesObj.upgradeStoreM.SetActive(true);
                inventoryM.referencesObj.inventorySection.SetActive(true);
                inventoryM.referencesObj.inventorySection.GetComponent<FadeUI>().canvasGroup.alpha = 0;
                upgradeM.isActive = true;
                buttonBG.color = neutralColor;
            }
            else if (buttonBG.name == "SpecialButton")
            {
                storeM.fadeUI.canvasGroup.alpha = 0;
                storeM.gameObject.SetActive(false);
                inventoryM.referencesObj.specialStoreM.SetActive(true);
                inventoryM.referencesObj.inventorySection.SetActive(true);
                inventoryM.referencesObj.inventorySection.GetComponent<FadeUI>().canvasGroup.alpha = 0;
                specialM.isActive = true;
                buttonBG.color = neutralColor;
            }
        }
    }

    public void EnterButton()
    {
        if (menuMoveAS)
            menuMoveAS.PlayRandomClip();

        Image buttonBG = GetComponent<Image>();
        buttonBG.color = highLightColor;
    }

    public void ExitButton()
    {
        Image buttonBG = GetComponent<Image>();
        buttonBG.color = neutralColor;
    }
}
