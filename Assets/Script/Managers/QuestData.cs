using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; 
using UnityEngine;
using System;

public class QuestData : MonoBehaviour
{
    public enum QuestType
    {
        MainQuest,
        SideQuest
    }
    [Header("QUEST SETTINGS")]
    public QuestType questType;
    public string questName;
    public int questLevel;
    [TextArea(3, 10)]
    public string questDescription;
    [Space(2)]
    [Header("CURRENT OBJECTIVES")]
    public int curObjectivePhase;
    public int maxObjectivePhase;
    [System.Serializable]
    public struct QuestObjective
    {
        public Transform questLocation;
        public string questLocationName;
        public string questObjectiveSubject;
        [Space(8)]
        [TextArea(3, 10)]
        public string questHint;
        public int curCollectableAmount;
        public int maxCollectableAmount;
        public int thisObjectiveNum;
    }
    public QuestObjective[] questObjective;
    [Header("REWARDS")]
    public int EXPReward;
    public GameObject[] rewardItems;
    [Header("OTHER SETTINGS")]
    public bool inProgress;
    public bool isComplete;
    [HideInInspector] public bool isQuestActive;

    private QuestData[] activeQuests;
    public int questObjectiveNum;

    void Start()
    {
        QuestDebug();
    }

    void QuestDebug()
    {
        for (int i = 0; i < questObjective.Length; i++)
        {
            if (questObjective[i].questLocation == null)
                Debug.LogWarning("Please Add NewObject Location Transform to Quest");
        }
    }

    void Update()
    {
        SetQuestRewards();
    }

    void SetQuestRewards()
    {
        activeQuests = FindObjectsOfType<QuestData>();
        foreach (QuestData quests in activeQuests)
        {
            if (rewardItems.Length == 0 && questName == quests.questName)
            {
                rewardItems = quests.rewardItems;
            }
        }
    }
}
