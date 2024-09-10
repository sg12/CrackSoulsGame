using UnityEngine.UI;
using UnityEngine;
using TMPro;
using RPGAE.CharacterController;

public class RepairStoreManager : MonoBehaviour
{
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
    public TextMeshProUGUI coinQuantity;
    public Animation storeItemPreviewAnim;
    public ParticleSystem sparkleParticle;

    #region Private

    // Private
    private MiniMap miniMap;
    private StoreManager storeM;
    private ThirdPersonController cc;
    private InventoryManager inventoryM;
    public bool isActive;
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

            // If you're not fading exit.
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
            else if(fadeUI.canvasGroup.alpha == 0 && previouslyActive)
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

    public void CheckInventoryItems(ref ItemData[] itemData, ref int inventorySlot, ref int[] itemCounter)
    {
        itemMarketImage.enabled = true;
        itemMarketImage.sprite = itemData[inventorySlot].itemSprite;
        itemMarketName.text = itemData[inventorySlot].itemName;
        itemMarketDescription.text = itemData[inventorySlot].itemDescription;
        itemMarketPrice.text = "x" + itemData[inventorySlot].repairPrice + " Upgrade Price";
        itemMarketInInventory.text = "x" + itemCounter[inventorySlot] + " In Inventory";
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
