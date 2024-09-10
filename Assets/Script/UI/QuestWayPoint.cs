using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using RPGAE.CharacterController;

public class QuestWayPoint : MonoBehaviour
{
    // Indicator icon
    public Image img;
    // The target (location, enemy, etc..)
    public Transform target;
    // UI Text to display the distance
    public TextMeshProUGUI meter;
    // To adjust the position of the icon
    public Vector3 offset;

    [Header("QUEST SETTINGS")]
    [Tooltip("The name of the current quest you're progressing through.")]
    public string questSequenceNameReceipt;
    [Tooltip("The required phase number in order to progress to the next.")]
    public int questPhaseReceipt;
    [Tooltip("Once the required action is meet you will increment a phase.")]
    public int addQuestPhaseBy = 1;
    [Tooltip("Once the required action is meet you will increment a quantity amount.")]
    public int addQuestQuantityBy;

    public FadeUI fadeUI;
    private MiniMapItem miniMapItem;
    private ThirdPersonController cc;
    private QuestManager questM;
    public bool collided;

    void Start()
    {
        fadeUI = GetComponentInParent<FadeUI>();
        miniMapItem = GetComponent<MiniMapItem>();
        cc = FindObjectOfType<ThirdPersonController>();
        questM = FindObjectOfType<QuestManager>();
    }

    // Update is called once per frame
    void Update()
    {
        IconVisbility();

        float minX = img.GetPixelAdjustedRect().width / 2;
        float maxX = Screen.width - minX;

        float minY = img.GetPixelAdjustedRect().height / 2;
        float maxY = Screen.height - minY;

        Vector2 pos = Camera.main.WorldToScreenPoint(target.position + offset);
        if (Vector3.Dot((target.position - Camera.main.transform.position), Camera.main.transform.forward) < 0)
        {
            if (pos.x < Screen.width / 2)
            {
                pos.x = maxX;
            }
            else
            {
                pos.x = minX;
            }
        }

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        img.transform.position = pos;
        meter.text = ((int)Vector3.Distance(target.position, cc.transform.position)).ToString() + "m";
    }

    void IconVisbility()
    {
        if (cc.inventoryM.isPauseMenuOn)
        {
            fadeUI.canvasGroup.alpha = 0;
            return;
        }

        if (miniMapItem.Size == 1 || collided)
        {
            fadeUI.canvasGroup.alpha = 0;
        }

        if (miniMapItem.Size > 1 && cc.systemM.blackScreenFUI.canvasGroup.alpha == 0 && !collided)
        {
            fadeUI.canvasGroup.alpha = 1;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && fadeUI.canvasGroup.alpha == 1)
        {
            for (int i = 0; i < questM.mainQuestSlots.questData.Length; i++)
            {
                if (questM.mainQuestSlots.questData[i].questName == questSequenceNameReceipt &&
                    questM.mainQuestSlots.questData[i].questObjective[questM.mainQuestSlots.questData[i].questObjectiveNum].curCollectableAmount >=
                    questM.mainQuestSlots.questData[i].questObjective[questM.mainQuestSlots.questData[i].questObjectiveNum].maxCollectableAmount)
                {
                    questM.AddQuestObjective(ref questSequenceNameReceipt, ref questPhaseReceipt, ref addQuestPhaseBy, ref addQuestQuantityBy);
                    miniMapItem.Size = 1;
                }
            }

            for (int i = 0; i < questM.sideQuestSlots.questData.Length; i++)
            {
                if (questM.sideQuestSlots.questData[i].questName == questSequenceNameReceipt &&
                    questM.sideQuestSlots.questData[i].questObjective[questM.sideQuestSlots.questData[i].questObjectiveNum].curCollectableAmount >=
                    questM.sideQuestSlots.questData[i].questObjective[questM.sideQuestSlots.questData[i].questObjectiveNum].maxCollectableAmount)
                {
                    questM.AddQuestObjective(ref questSequenceNameReceipt, ref questPhaseReceipt, ref addQuestPhaseBy, ref addQuestQuantityBy);
                    miniMapItem.Size = 1;
                }
            }

            collided = true;
        }
    }
}
