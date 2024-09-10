using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class RequiredItems : MonoBehaviour
{
    public int inventoryAmount;
    public int requiredAmount;

    public Image image;
    public FadeUI fadeUI;
    public TextMeshProUGUI amountText;

    // Start is called before the first frame update
    void Start()
    {
        fadeUI = GetComponent<FadeUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (image.sprite == null)
        {
            fadeUI.canvasGroup.alpha = 0;
        }
        else
        {
            fadeUI.canvasGroup.alpha = 1;
            amountText.text = inventoryAmount + "/" + requiredAmount;
        }
    }
}
