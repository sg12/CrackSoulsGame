using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestPhaseTrigger : MonoBehaviour
{
    [Header("QUEST SETTINGS")]
    [Tooltip("The name of the current quest you're progressing through.")]
    public string questSequenceNameReceipt;
    [Tooltip("The required phase number in order to progress to the next.")]
    public int questPhaseReceipt;
    [Tooltip("Once the required action is meet you will increment a phase.")]
    public int addQuestPhaseBy = 1;
    [Tooltip("Once the required action is meet you will increment a quantity amount.")]
    public int addQuestQuantityBy;

    private QuestManager questM;

    // Start is called before the first frame update
    void Start()
    {
        questM = FindObjectOfType<QuestManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            for (int i = 0; i < questM.mainQuestSlots.questData.Length; i++)
            {
                if (questM.mainQuestSlots.questData[i].questName == questSequenceNameReceipt)
                {
                    // Current Objective UI
                    questM.AddQuestObjective(ref questSequenceNameReceipt, ref questPhaseReceipt, ref addQuestPhaseBy, ref addQuestQuantityBy);
                }
            }

            for (int i = 0; i < questM.sideQuestSlots.questData.Length; i++)
            {
                if (questM.sideQuestSlots.questData[i].questName == questSequenceNameReceipt)
                {
                    // Current Objective UI
                    questM.AddQuestObjective(ref questSequenceNameReceipt, ref questPhaseReceipt, ref addQuestPhaseBy, ref addQuestQuantityBy);
                }
            }
        }
    }
}
