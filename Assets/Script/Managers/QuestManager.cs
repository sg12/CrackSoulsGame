using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;
using RPGAE.CharacterController;

public class QuestManager : MonoBehaviour
{
    [Serializable]
    public class MainQuestSlots
    {
        public int slotNum = 0;
        public TextMeshProUGUI[] questName;
        public TextMeshProUGUI[] questLevel;
        public TextMeshProUGUI[] questDistance;
        public TextMeshProUGUI[] questLocationName;
        public Image[] highLight;
        public QuestData[] questData;
    }
    public static int numOfMainQuestSlot;
    [HideInInspector]
    public MainQuestSlots mainQuestSlots;
    [Serializable]
    public class SideQuestSlots
    {
        public int slotNum = 0;
        public TextMeshProUGUI[] questName;
        public TextMeshProUGUI[] questLevel;
        public TextMeshProUGUI[] questDistance;
        public TextMeshProUGUI[] questLocationName;
        public Image[] highLight;
        public QuestData[] questData;
    }
    public static int numOfSideQuestSlot;
    [HideInInspector]
    public SideQuestSlots sideQuestSlots;

    [Header("QUEST SETTINGS")]
    public Sprite miniMapLocationIcon;
    public Color miniMapIconColor = new Color(253, 255, 0, 230);
    public Color miniMapIconAreaColor = new Color(253, 255, 0, 230);
    [Header("Quest Slot HighLight")]
    public Color neutralColor = new Color32(0, 0, 0, 191);
    public Color highlightColor = new Color32(128, 128, 128, 191);
    public Color selectColor = new Color32(214, 214, 214, 191);
    public Color completeColor = new Color32(241, 213, 0, 191);

    [Header("Quest Added HUD")]
    public TextMeshProUGUI questAddedType;
    public TextMeshProUGUI questAddedName;
    public FadeUI questAddedfadeUI;
    public GameObject[] rewardItems;

    [Header("REFERENCES")]
    public GameObject createSlotPrefab;
    public GameObject questSlotOptions;
    public TextMeshProUGUI questName;
    public TextMeshProUGUI questLocation;
    public TextMeshProUGUI questLevel;
    public TextMeshProUGUI questDescription;
    public TextMeshProUGUI questObjectiveTitle;
    public TextMeshProUGUI questHint;

    //Private
    private GameObject newSlot;
    private ThirdPersonController cc;
    bool canClear;

    void Start()
    {
        cc = FindObjectOfType<ThirdPersonController>();
    }

    void Update()
    {
        if (questSlotOptions == null) questSlotOptions = GameObject.Find("QuestSlotOptions").gameObject;

        if (questAddedfadeUI.canvasGroup.alpha == 1 && !questAddedfadeUI.isFading)
        {
            questAddedfadeUI.FadeTransition(0, 2, 0.5f);
        }
        if (cc.inventoryM.isPauseMenuOn)
        {
            questAddedfadeUI.canvasGroup.alpha = 0;
        }

        #region Update Wapoint Icon

        if (cc.hudM.fadeUIFadeGroup.canvasGroup.alpha == 1)
        {
            if (cc.hudM.fadeUIQuestCompleteBG.canvasGroup.alpha == 0 && mainQuestSlots.questData.Length > 0 && mainQuestSlots.questData[mainQuestSlots.slotNum].isQuestActive)
            {
                Transform mainQuestLocation = mainQuestSlots.questData[mainQuestSlots.slotNum].questObjective[mainQuestSlots.questData[mainQuestSlots.slotNum].questObjectiveNum].questLocation;

                if(mainQuestLocation.GetComponentInChildren<MiniMapItem>() != null)
                {
                    mainQuestLocation.GetComponentInChildren<MiniMapItem>().Size = 20;

                    if (mainQuestLocation.GetComponentInChildren<MiniMapItem>().ShowCircleArea)
                    {
                        mainQuestLocation.GetComponentInChildren<MiniMapItem>().CircleAreaRadius = 15;
                        mainQuestLocation.GetComponentInChildren<MiniMapItem>().
                        SetCircleArea(mainQuestLocation.GetComponentInChildren<MiniMapItem>().CircleAreaRadius,
                        mainQuestLocation.GetComponentInChildren<MiniMapItem>().CircleAreaColor);
                    }
                }
            }

            if (cc.hudM.fadeUIQuestCompleteBG.canvasGroup.alpha == 0 && sideQuestSlots.questData.Length > 0 && sideQuestSlots.questData[sideQuestSlots.slotNum].isQuestActive)
            {
                Transform sideQuestLocation = sideQuestSlots.questData[sideQuestSlots.slotNum].questObjective[sideQuestSlots.questData[sideQuestSlots.slotNum].questObjectiveNum].questLocation;

                if (sideQuestLocation.GetComponentInChildren<MiniMapItem>() != null)
                {
                    sideQuestLocation.GetComponentInChildren<MiniMapItem>().Size = 20;

                    if (sideQuestLocation.GetComponentInChildren<MiniMapItem>().ShowCircleArea)
                    {
                        sideQuestLocation.GetComponentInChildren<MiniMapItem>().CircleAreaRadius = 15;
                        sideQuestLocation.GetComponentInChildren<MiniMapItem>().
                        SetCircleArea(sideQuestLocation.GetComponentInChildren<MiniMapItem>().CircleAreaRadius,
                        sideQuestLocation.GetComponentInChildren<MiniMapItem>().CircleAreaColor);
                    }
                }
            }
        }

        if (mainQuestSlots.questData.Length > 0)
        {
            if (cc.hudM.fadeUIQuestCompleteBG.canvasGroup.alpha != 0 || !mainQuestSlots.questData[mainQuestSlots.slotNum].isQuestActive)
            {
                if (mainQuestSlots.questData[mainQuestSlots.slotNum].questObjective[mainQuestSlots.questData[mainQuestSlots.slotNum].questObjectiveNum].questLocation.GetComponentInChildren<MiniMapItem>() != null)
                {
                    mainQuestSlots.questData[mainQuestSlots.slotNum].questObjective[mainQuestSlots.questData[mainQuestSlots.slotNum].questObjectiveNum].questLocation.GetComponentInChildren<MiniMapItem>().Size = 1;
                    mainQuestSlots.questData[mainQuestSlots.slotNum].questObjective[mainQuestSlots.questData[mainQuestSlots.slotNum].questObjectiveNum].questLocation.GetComponentInChildren<MiniMapItem>().HideCircleArea();
                }
            }
        }

        if (sideQuestSlots.questData.Length > 0)
        {
            if (cc.hudM.fadeUIQuestCompleteBG.canvasGroup.alpha != 0 || !sideQuestSlots.questData[sideQuestSlots.slotNum].isQuestActive)
            {
                if (sideQuestSlots.questData[sideQuestSlots.slotNum].questObjective[sideQuestSlots.questData[sideQuestSlots.slotNum].questObjectiveNum].questLocation.GetComponentInChildren<MiniMapItem>() != null)
                {
                    sideQuestSlots.questData[sideQuestSlots.slotNum].questObjective[sideQuestSlots.questData[sideQuestSlots.slotNum].questObjectiveNum].questLocation.GetComponentInChildren<MiniMapItem>().Size = 1;
                    sideQuestSlots.questData[sideQuestSlots.slotNum].questObjective[sideQuestSlots.questData[sideQuestSlots.slotNum].questObjectiveNum].questLocation.GetComponentInChildren<MiniMapItem>().HideCircleArea();
                }
            }
        }

        #endregion
    }

    public void AddMainQuest(QuestData questToAdd)
    {
        newSlot = Instantiate(createSlotPrefab);
        QuestSlot questData = newSlot.GetComponent<QuestSlot>();
        questData.questType = QuestSlot.QuestType.MainQuest;
        questData.slotNum = numOfMainQuestSlot - 1;

        QuestData[]   temp0 = new QuestData[mainQuestSlots.questData.Length + 1];
        TextMeshProUGUI[] temp1 = new TextMeshProUGUI[mainQuestSlots.questName.Length + 1];
        TextMeshProUGUI[] temp2 = new TextMeshProUGUI[mainQuestSlots.questLocationName.Length + 1];
        TextMeshProUGUI[] temp3 = new TextMeshProUGUI[mainQuestSlots.questDistance.Length + 1];
        TextMeshProUGUI[] temp4 = new TextMeshProUGUI[mainQuestSlots.questLevel.Length + 1];
        Image[]           temp5 = new Image[mainQuestSlots.highLight.Length + 1];

        for (int i = 0; i < mainQuestSlots.questData.Length; i++)
        {
            if (mainQuestSlots.questData[0] != null)
            {
                temp0[i] = mainQuestSlots.questData[i];
                temp1[i] = mainQuestSlots.questName[i];
                temp2[i] = mainQuestSlots.questLocationName[i];
                temp3[i] = mainQuestSlots.questDistance[i];
                temp4[i] = mainQuestSlots.questLevel[i];
                temp5[i] = mainQuestSlots.highLight[i];
            }
        }

        mainQuestSlots.questData = temp0;
        mainQuestSlots.questName = temp1;
        mainQuestSlots.questLocationName = temp2;
        mainQuestSlots.questDistance = temp3;
        mainQuestSlots.questLevel = temp4;
        mainQuestSlots.highLight = temp5;
        
        mainQuestSlots.questData[numOfMainQuestSlot - 1] = questToAdd;
        mainQuestSlots.questName[numOfMainQuestSlot - 1] = questData.questName;
        mainQuestSlots.questLocationName[numOfMainQuestSlot - 1] = questData.questLocation;
        mainQuestSlots.questDistance[numOfMainQuestSlot - 1] = questData.questDistance;
        mainQuestSlots.questLevel[numOfMainQuestSlot - 1] = questData.questLevel;
        mainQuestSlots.highLight[numOfMainQuestSlot - 1] = questData.highLight;

        questData.questName.text = questToAdd.questName;
        questData.questLocation.text = questToAdd.questObjective[questToAdd.questObjectiveNum].questLocationName;
        questData.questLevel.text = questToAdd.questLevel.ToString();

        // Quest has been added UI
        questAddedType.text = "Main Quest";
        questAddedName.text = questToAdd.questName;
        if (questAddedfadeUI.canvasGroup.alpha == 0) 
            questAddedfadeUI.FadeTransition(1, 0, 0.5f);
    }

    public void AddSideQuest(QuestData questToAdd)
    {
        newSlot = Instantiate(createSlotPrefab);
        QuestSlot questData = newSlot.GetComponent<QuestSlot>();
        questData.questType = QuestSlot.QuestType.SideQuest;
        questData.slotNum = numOfSideQuestSlot - 1;

        QuestData[] temp0 = new QuestData[sideQuestSlots.questData.Length + 1];
        TextMeshProUGUI[] temp1 = new TextMeshProUGUI[sideQuestSlots.questName.Length + 1];
        TextMeshProUGUI[] temp2 = new TextMeshProUGUI[sideQuestSlots.questLocationName.Length + 1];
        TextMeshProUGUI[] temp3 = new TextMeshProUGUI[sideQuestSlots.questDistance.Length + 1];
        TextMeshProUGUI[] temp4 = new TextMeshProUGUI[sideQuestSlots.questLevel.Length + 1];
        Image[] temp5 = new Image[sideQuestSlots.highLight.Length + 1];

        for (int i = 0; i < sideQuestSlots.questData.Length; i++)
        {
            if (sideQuestSlots.questData[0] != null)
            {
                temp0[i] = sideQuestSlots.questData[i];
                temp1[i] = sideQuestSlots.questName[i];
                temp2[i] = sideQuestSlots.questLocationName[i];
                temp3[i] = sideQuestSlots.questDistance[i];
                temp4[i] = sideQuestSlots.questLevel[i];
                temp5[i] = sideQuestSlots.highLight[i];
            }
        }

        sideQuestSlots.questData = temp0;
        sideQuestSlots.questName = temp1;
        sideQuestSlots.questLocationName = temp2;
        sideQuestSlots.questDistance = temp3;
        sideQuestSlots.questLevel = temp4;
        sideQuestSlots.highLight = temp5;

        sideQuestSlots.questData[numOfSideQuestSlot - 1] = questToAdd;
        sideQuestSlots.questName[numOfSideQuestSlot - 1] = questData.questName;
        sideQuestSlots.questLocationName[numOfSideQuestSlot - 1] = questData.questLocation;
        sideQuestSlots.questDistance[numOfSideQuestSlot - 1] = questData.questDistance;
        sideQuestSlots.questLevel[numOfSideQuestSlot - 1] = questData.questLevel;
        sideQuestSlots.highLight[numOfSideQuestSlot - 1] = questData.highLight;

        questData.questName.text = questToAdd.questName;
        questData.questLocation.text = questToAdd.questObjective[questToAdd.questObjectiveNum].questLocationName;
        questData.questLevel.text = questToAdd.questLevel.ToString();

        // Quest has been added UI
        questAddedType.text = "Side Quest";
        questAddedName.text = questToAdd.questName;
        if (questAddedfadeUI.canvasGroup.alpha == 0)
            questAddedfadeUI.FadeTransition(1, 0, 0.5f);
    }

    public void MainQuestSlotSelect(int slotNumber)
    {
        mainQuestSlots.slotNum = slotNumber;
        if (mainQuestSlots.questData.Length == 0 || mainQuestSlots.questData[slotNumber] == null)
        {
            cc.infoMessage.info.text = "Please mark (trigger question bool) in " + cc.inventoryM.dialogueM.curDialogue.controller.name + "'s" + " dialogue and add sentences in (option A Sentence/Option B Sentence) to receive quests.";
            return;
        }

        QuestData mq = mainQuestSlots.questData[slotNumber] as QuestData;

        mainQuestSlots.questData[slotNumber].isQuestActive = true;
        mainQuestSlots.highLight[slotNumber].color = selectColor;

        questName.text = mq.questName;
        questLocation.text = mq.questObjective[mq.questObjectiveNum].questLocationName;
        questLevel.text = "The Required Level: " + mq.questLevel;
        questDescription.text = mq.questDescription;

        if (mq.questObjective[mq.questObjectiveNum].maxCollectableAmount > 0)
            questObjectiveTitle.text = mq.questObjective[mq.questObjectiveNum].questObjectiveSubject + " " + mq.questObjective[mq.questObjectiveNum].curCollectableAmount + "/" + mq.questObjective[mq.questObjectiveNum].maxCollectableAmount;
        else
            questObjectiveTitle.text = mq.questObjective[mq.questObjectiveNum].questObjectiveSubject;

        questHint.text = mq.questObjective[mq.questObjectiveNum].questHint;

        mq.inProgress = true;
        if (sideQuestSlots.questData.Length > 0) sideQuestSlots.questData[sideQuestSlots.slotNum].inProgress = false;

        for (int i = 0; i < mq.rewardItems.Length; i++)
        {
            if (mainQuestSlots.questData[slotNumber].rewardItems[i] != null)
                rewardItems[i].GetComponent<QuestRewardIcon>().reward = mainQuestSlots.questData[slotNumber].rewardItems[i].GetComponentInChildren<ItemData>();
        }
    }

    public void SideQuestSlotSelect(int slotNumber)
    {
        sideQuestSlots.slotNum = slotNumber;
        if (sideQuestSlots.questData.Length == 0 || sideQuestSlots.questData[slotNumber] == null)
        {
            cc.infoMessage.info.text = "Please mark (trigger question bool) in " + cc.inventoryM.dialogueM.curDialogue.controller.name + "'s" + " dialogue and add sentences in (option A Sentence/Option B Sentence) to receive quests.";
            return;
        }

        QuestData sq = sideQuestSlots.questData[slotNumber] as QuestData;

        sideQuestSlots.questData[slotNumber].isQuestActive = true;
        sideQuestSlots.highLight[slotNumber].color = selectColor;

        questName.text = sq.questName;
        questLocation.text = sq.questObjective[sq.questObjectiveNum].questLocationName;
        questLevel.text = "The Required Level: " + sq.questLevel;
        questDescription.text = sq.questDescription;

        if (sq.questObjective[sq.questObjectiveNum].maxCollectableAmount > 0)
            questObjectiveTitle.text = sq.questObjective[sq.questObjectiveNum].questObjectiveSubject + " " + sq.questObjective[sq.questObjectiveNum].curCollectableAmount + "/" + sq.questObjective[sq.questObjectiveNum].maxCollectableAmount;
        else
            questObjectiveTitle.text = sq.questObjective[sq.questObjectiveNum].questObjectiveSubject;

        questHint.text = sq.questObjective[sq.questObjectiveNum].questHint;

        sq.inProgress = true;
        if(mainQuestSlots.questData.Length > 0) mainQuestSlots.questData[mainQuestSlots.slotNum].inProgress = false;

        for (int i = 0; i < sq.rewardItems.Length; i++)
        {
            if (sideQuestSlots.questData[slotNumber].rewardItems[i] != null)
                rewardItems[i].GetComponent<QuestRewardIcon>().reward = sideQuestSlots.questData[slotNumber].rewardItems[i].GetComponentInChildren<ItemData>();
        }
    }

    #region Quest Progresson 

    public void AddQuestObjective(ref string questSequenceNameReceipt, ref int questPhaseReceipt, ref int addQuestPhase, ref int addQuestQuantity)
    {
        for (int i = 0; i < mainQuestSlots.questData.Length; i++)
        {
            if (mainQuestSlots.questData[i].questName == questSequenceNameReceipt &&
                mainQuestSlots.questData[i].curObjectivePhase == questPhaseReceipt)
            {
                AddObjectiveQuantity(ref mainQuestSlots.questData, ref mainQuestSlots.highLight, ref mainQuestSlots.questLocationName, 
                ref mainQuestSlots.questData[i].questObjectiveNum, ref questSequenceNameReceipt, ref questPhaseReceipt, ref addQuestPhase, ref addQuestQuantity);
            }
        }

        for (int i = 0; i < sideQuestSlots.questData.Length; i++)
        {
            if (sideQuestSlots.questData[i].questName == questSequenceNameReceipt &&
                sideQuestSlots.questData[i].curObjectivePhase == questPhaseReceipt)
            {
                AddObjectiveQuantity(ref sideQuestSlots.questData, ref sideQuestSlots.highLight, ref sideQuestSlots.questLocationName, 
                ref sideQuestSlots.questData[i].questObjectiveNum,  ref questSequenceNameReceipt, ref questPhaseReceipt, ref addQuestPhase, ref addQuestQuantity);
            }
        }
    }

    public void ChangeCurrentObjective(ref QuestData[] questData, ref Image[] highLight, ref TextMeshProUGUI[] questLocationName, ref int phaseIncrement)
    {
        for (int i = 0; i < questData.Length; i++)
        {
            if (questData[i].isQuestActive)
            {
                #region Hide Previous Location Icon

                if (questData[i].questObjective[questData[i].questObjectiveNum].questLocation.GetComponent<MiniMapItem>() != null)
                {
                    questData[i].questObjective[questData[i].questObjectiveNum].questLocation.GetComponent<MiniMapItem>().Size = 1;
                    questData[i].questObjective[questData[i].questObjectiveNum].questLocation.GetComponent<MiniMapItem>().HideCircleArea();
                }

                #endregion

                if (questData[i].questObjectiveNum < (questData[i].questObjective.Length - 1))
                {
                    if (questData[i].questObjective[questData[i].questObjectiveNum].curCollectableAmount >=
                    questData[i].questObjective[questData[i].questObjectiveNum].maxCollectableAmount)
                    {
                        questData[i].questObjectiveNum++;
                    }
                }

                if (questData[i].questObjective[questData[i].questObjectiveNum].questLocation != null)
                {
                    questLocationName[i].text = questData[i].questObjective[questData[i].questObjectiveNum].questLocationName;

                    if (questData[i].questObjective[questData[i].questObjectiveNum].maxCollectableAmount > 0)
                    {
                        questObjectiveTitle.text = questData[i].questObjective[questData[i].questObjectiveNum].questObjectiveSubject + " " + questData[i].questObjective[questData[i].questObjectiveNum].curCollectableAmount + "/" + questData[i].questObjective[questData[i].questObjectiveNum].maxCollectableAmount;
                    }
                    else
                    {
                        questObjectiveTitle.text = questData[i].questObjective[questData[i].questObjectiveNum].questObjectiveSubject;
                    }
                    questData[i].questObjective[questData[i].questObjectiveNum].questHint = questData[i].questObjective[questData[i].questObjectiveNum].questHint;

                    questLocation.text = questData[i].questObjective[questData[i].questObjectiveNum].questLocationName;
                    questHint.text = questData[i].questObjective[questData[i].questObjectiveNum].questHint;
                }
            }
        }
    }

    protected void AddObjectiveQuantity(ref QuestData[] questData, ref Image[] highLight, ref TextMeshProUGUI[] questLocationName, ref int questObjectiveNum, ref string questSequenceNameReceipt, ref int questPhaseRecipt, ref int phaseIncrement, ref int collectableIncrement)
    {
        for (int i = 0; i < questData.Length; i++)
        {
            if (questData[i].questName == questSequenceNameReceipt 
            && questData[i].curObjectivePhase == questPhaseRecipt)
            {
                questData[i].curObjectivePhase += phaseIncrement;

                if (questData[i].questObjective[questData[i].questObjectiveNum].maxCollectableAmount > 0)
                {
                    questData[i].questObjective[questData[i].questObjectiveNum].curCollectableAmount += collectableIncrement;
                }

                if (questData[i].questObjective[questData[i].questObjectiveNum].curCollectableAmount >= questData[i].questObjective[questData[i].questObjectiveNum].maxCollectableAmount)
                {
                    if (questData[i].questObjective[questData[i].questObjectiveNum].maxCollectableAmount > 0)
                        questData[i].curObjectivePhase++;

                    #region Update Dialogue Phases Relating To The Quest 

                    foreach (AIController npc in FindObjectsOfType<AIController>())
                    {
                        if (npc.dialogue.questSequenceNameReceipt == questSequenceNameReceipt && 
                            npc.dialogue.globalUpdatePhase)
                        {
                            npc.dialogue.questPhaseReceipt++;
                        }
                    }

                    #endregion

                    ChangeCurrentObjective(ref questData, ref highLight, ref questLocationName, ref phaseIncrement);
                }
            }
        }
    }

    #endregion
}
