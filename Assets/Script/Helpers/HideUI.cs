using UnityEngine;
using TMPro;

public class HideUI : MonoBehaviour
{
    [Tooltip("Prevent (NormalizedShow UI from appearing)")]
    public bool isHidden;
    [Header("UI")]
    public GameObject[] hideUI;
    public GameObject[] showUI;
    public GameObject[] normalizeShow;
    public GameObject[] normalizeHide;
    public TMP_Dropdown[] ButtonContent;
    public FadeUI parentUI;
    public GameObject[] DontHideIfActive;

    void Update()
    {
        if (parentUI)
        {
            if (parentUI.canvasGroup.alpha == 0)
                isHidden = false;
            else
                isHidden = true;
        }

        foreach(GameObject item in DontHideIfActive)
        {
            if (item.activeInHierarchy && (item.GetComponent<FadeUI>() != null && item.GetComponent<FadeUI>().canvasGroup.alpha != 0 
                || item.GetComponent<CanvasGroup>().alpha != 0))
            {
                return;
            }
        }

        if (!isHidden) return;

        if (hideUI != null)
        {
            foreach (GameObject hide in hideUI)
            {
                hide.SetActive(false);
            }
        }

        if (showUI != null)
        {
            foreach (GameObject show in showUI)
            {
                show.SetActive(true);
            }
        }

        if (isHidden) return;

        if (normalizeShow != null)
        {
            foreach (GameObject show in normalizeShow)
            {
                show.SetActive(true);
            }
        }
    }

    void OnDisable()
    {
        if (normalizeShow != null)
        {
            foreach (GameObject show in normalizeShow)
            {
                show.SetActive(true);
            }
        }
        if (normalizeHide != null)
        {
            foreach (GameObject hide in normalizeHide)
            {
                hide.SetActive(false);
            }
        }
    }

}
