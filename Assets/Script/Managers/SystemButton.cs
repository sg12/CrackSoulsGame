using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SystemButton : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public string buttonName;

    [Header("AUDIO")]
    public RandomAudioPlayer menuMoveAS;
    public RandomAudioPlayer menuSelectAS;

    [Header("REFERENCES")]
    public FadeUI rootOptionSettings;

    private InventoryManager inventoryM;
    private SystemManager systemM;

    // Start is called before the first frame update
    void Start()
    {
        systemM = FindObjectOfType<SystemManager>();
        inventoryM = FindObjectOfType<InventoryManager>();
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        OnEnter();
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        OnSelected();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnExit();
    }

    void OnEnter()
    {
        if (menuMoveAS)
            menuMoveAS.PlayRandomClip();

        Image slotImage = GetComponent<Image>();
        slotImage.color = GetComponent<Button>().colors.selectedColor;

        NormalizeBGSystem();
    }

    void OnSelected()
    {
        if (menuSelectAS)
            menuSelectAS.PlayRandomClip();

        if (buttonName == "New Game")
        {
            inventoryM.pauseMenuNavigation = 2;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else if (buttonName == "Save")
        {
            systemM.SaveGlobalData();
        }
        else if (buttonName == "Load")
        {
            systemM.LoadData();
        }
        else if (buttonName == "Options")
        {
            if (rootOptionSettings.canvasGroup.alpha == 0) rootOptionSettings.FadeTransition(1, 0, 0.1f);
            if (systemM.rootCtrlLayout.canvasGroup.alpha == 1) systemM.rootCtrlLayout.FadeTransition(0, 0, 0.1f);
        }
        else if (buttonName == "Quit")
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }

    void OnExit()
    {
        Image slotImage = GetComponent<Image>();
        slotImage.color = GetComponent<Button>().colors.normalColor;

        NormalizeBGSystem();
    }

    void NormalizeBGSystem()
    {
        if (buttonName != "Options")
        {
            if (rootOptionSettings.canvasGroup.alpha == 1)
                rootOptionSettings.FadeTransition(0, 0, 0.1f);
            if (systemM.rootCtrlLayout.canvasGroup.alpha == 0)
                systemM.rootCtrlLayout.FadeTransition(1, 0, 0.1f);
        }
    }
}
