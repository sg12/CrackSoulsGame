using UnityEngine.UI;
using UnityEngine;
using RPGAE.CharacterController;

public class StoreManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject[] hideUI;
    public GameObject[] showUI;
    public GameObject[] normalizeShow;
    public GameObject[] normalizeHide;

    [Header("REFERENCES")]
    public FadeUI fadeUI;
    public FadeUI storeItemPreviewAlpha;
    public CanvasGroup storeButtonsCanvas;
    public BuyStoreManager buyM;
    public SellStoreManager sellM;
    public RepairStoreManager repairM;
    public UpgradeStoreManager upgradeM;
    public SpecialStoreManager specialM;

    #region Private 

    [HideInInspector] public DialogueManager dialogueM;
    private ThirdPersonController cc;
    private InventoryManager inventoryM;
    private bool isDone;
    public bool isActive;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        storeItemPreviewAlpha.canvasGroup.alpha = 0;

        // Get Components
        fadeUI = GetComponent<FadeUI>();
        cc = FindObjectOfType<ThirdPersonController>();
        dialogueM = FindObjectOfType<DialogueManager>();
        inventoryM = FindObjectOfType<InventoryManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeUI.canvasGroup.alpha == 0)
        {
            if (isDone)
            {
                fadeUI.FadeTransition(0, 0, 0.5f);
                foreach (GameObject normalize in normalizeShow)
                {
                    normalize.SetActive(true);
                }
                foreach (GameObject normalize in normalizeHide)
                {
                    normalize.SetActive(false);
                }
                isDone = false;
            }

            storeButtonsCanvas.interactable = false;
            storeButtonsCanvas.blocksRaycasts = false;
        }
        else
        {
            // Exit Buy Menu
            if (cc.rpgaeIM.PlayerControls.Start.triggered)
            {
                isDone = true;
                isActive = false;
                cc.canMove = true;
                inventoryM.referencesObj.inventorySection.GetComponent<HideUI>().enabled = true;
                inventoryM.referencesObj.pauseMenu.GetComponent<FadeUI>().FadeTransition(0, 0, 0.5f);
                fadeUI.FadeTransition(0, 0, 0.5f);
                cc.tpCam.RemoveTargets();
                dialogueM.curDialogue.controller.dialogueActive = false;
            }

            if (!storeButtonsCanvas.interactable)
                isActive = true;

            storeButtonsCanvas.interactable = true;
            storeButtonsCanvas.blocksRaycasts = true;

            foreach (GameObject hide in hideUI)
            {
                hide.SetActive(false);
            }
            foreach (GameObject show in showUI)
            {
                show.SetActive(true);
            }
        }
    }
}
