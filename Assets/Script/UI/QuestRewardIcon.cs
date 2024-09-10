using UnityEngine.UI;
using UnityEngine;

public class QuestRewardIcon : MonoBehaviour 
{
    public FadeUI fadeUI;
    [HideInInspector] 
    public Image rewardSprite;
    public ItemData reward;

    // Use this for initialization
    void Awake () 
    {
        // Get Components
        fadeUI = GetComponent<FadeUI>();
        rewardSprite = GetComponent<Image>();
    }
	
	// Update is called once per frame
	void Update () 
    {
        if (rewardSprite.sprite == null)
        {
            fadeUI.canvasGroup.alpha = 0;
            if (reward != null)
                rewardSprite.sprite = reward.itemSprite;
        }
        else
        {
            fadeUI.canvasGroup.alpha = 1;
        }
    }
}
