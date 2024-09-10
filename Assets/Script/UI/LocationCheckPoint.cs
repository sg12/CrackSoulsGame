using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using RPGAE.CharacterController;

public class LocationCheckPoint : MonoBehaviour
{
    public Size size;
    public string title;
    public bool explored;
    public float earnedEXP;

    #region Private 

    [HideInInspector]
    public float curEXP;
    [HideInInspector]
    public bool done;

    [HideInInspector]
    public FadeUI fadeUI;
    [HideInInspector]
    public FadeUI expfadeUI;
    [HideInInspector]
    public RectTransform rootRect;
    [HideInInspector]
    public TextMeshProUGUI areaDiscovered;
    [HideInInspector]
    public TextMeshProUGUI titleDiscovered;
    [HideInInspector]
    public TextMeshProUGUI miniMapLocation;
    [HideInInspector]
    public TextMeshProUGUI discoveredEXP;

    private HUDManager hudM;
    private InventoryManager inventoryM;
    private ItemObtained itemOb;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Get Components
        hudM = FindObjectOfType<HUDManager>();
        inventoryM = FindObjectOfType<InventoryManager>();
        itemOb = FindObjectOfType<ItemObtained>();
        fadeUI = GameObject.Find("DiscoveredLocation").GetComponent<FadeUI>();
        expfadeUI = GameObject.Find("DiscoveredEXP").GetComponent<FadeUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeUI && (inventoryM.referencesObj.pauseMenu.GetComponent<FadeUI>().canvasGroup.alpha != 0 || itemOb.fadeUI.canvasGroup.alpha == 1))
            fadeUI.canvasGroup.alpha = 0;

        FadeTransition();
    }

    void FadeTransition()
    {
        if (fadeUI && fadeUI.canvasGroup.alpha == 1)
        {
            if (expfadeUI.canvasGroup.alpha == 0)
                expfadeUI.FadeTransition(1, 2, 0.5f);
            if (expfadeUI.canvasGroup.alpha == 1)
            {
                if (curEXP < earnedEXP)
                    curEXP += 60 * Time.deltaTime;
                else
                {
                    if (!done)
                    {
                        curEXP = earnedEXP;
                        hudM.IncrementExperience(earnedEXP);
                        fadeUI.FadeTransition(0, 4, 0.5f);
                        done = true;
                    }
                }
                if (discoveredEXP)
                    discoveredEXP.text = Mathf.Round(curEXP).ToString() + " EXP";
            }
        }

        if(done && fadeUI && fadeUI.canvasGroup.alpha == 0)
        {
            miniMapLocation.text = "";
            titleDiscovered.text = "";
            areaDiscovered.text = "";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ThirdPersonController character = other.GetComponent<ThirdPersonController>();

        if (!character) 
            return;

        if (!explored)
        {
            rootRect = GameObject.Find("DiscoveredLocation").GetComponent<RectTransform>();
            fadeUI = GameObject.Find("DiscoveredLocation").GetComponent<FadeUI>();
            expfadeUI = GameObject.Find("DiscoveredEXP").GetComponent<FadeUI>();
            discoveredEXP = GameObject.Find("DiscoveredEXP").GetComponent<TextMeshProUGUI>();
            miniMapLocation = GameObject.Find("MiniMapLocation").GetComponent<TextMeshProUGUI>();
            titleDiscovered = GameObject.Find("TitleDiscovered").GetComponent<TextMeshProUGUI>();
            areaDiscovered = GameObject.Find("AreaDiscovered").GetComponent<TextMeshProUGUI>();

            curEXP = 0;
            expfadeUI.canvasGroup.alpha = 0;
            fadeUI.canvasGroup.alpha = 0;
            switch (size)
            {
                case Size.Area:
                    areaDiscovered.text = "NEW AREA DISCOVERED";
                    rootRect.localScale = new Vector3(1.2f, 1.2f, 1.2f);
                    break;
                case Size.Location:
                    rootRect.localScale = new Vector3(1.4f, 1.4f, 1.4f);
                    areaDiscovered.text = "NEW LOCATION DISCOVERED";
                    break;
            }
            titleDiscovered.text = title;
            miniMapLocation.text = title;
            fadeUI.FadeTransition(1, 0, 0.5f);
            explored = true;
        }
        else
        {
            miniMapLocation = GameObject.Find("MiniMapLocation").GetComponent<TextMeshProUGUI>();
            miniMapLocation.text = title;
        }
    }

    public enum Size
    {
        Area,
        Location
    }
}
