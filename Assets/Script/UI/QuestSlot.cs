using UnityEngine.UI;
using UnityEngine;
using TMPro;
using RPGAE.CharacterController;

public class QuestSlot : MonoBehaviour
{
    public int slotNum;
    public QuestType questType;

    [Header("AUDIO")]
    public RandomAudioPlayer menuMoveAS;
    public RandomAudioPlayer menuSelectAS;

    [Header("REFERENCES")]
    public FadeUI fadeUI;
    public RectTransform rectTransform;
    public TextMeshProUGUI questName;
    public TextMeshProUGUI questLocation;
    public TextMeshProUGUI questLevel;
    public TextMeshProUGUI questDistance;
    public Image highLight;

    #region Private 

    private bool isDone;
    private bool setPosition;
    private MiniMap miniMap;
    private QuestManager questM;
    private ThirdPersonController cc;
    private HUDManager hudM;
    public GameObject panel;
    [HideInInspector] public Transform player;
    [HideInInspector] public Color oldHighLight;

    private bool oneShot;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Character").transform;

        // Get Components
        fadeUI = GetComponent<FadeUI>();
        miniMap = FindObjectOfType<MiniMap>();
        questM = FindObjectOfType<QuestManager>();
        cc = FindObjectOfType<ThirdPersonController>();
        hudM = FindObjectOfType<HUDManager>();
    }

    // Update is called once per frame
    void Update()
    {
        SetPersistentListener();
    }

    void SetPersistentListener()
    {
        if (!isDone)
        {
            switch (questType)
            {
                case QuestType.MainQuest:
                    isDone = true;
                    break;
                case QuestType.SideQuest:
                    isDone = true;
                    break;
            }
        }
        else
        {
            SetDefaultSettings();
        }
    }

    public void OnButtonEnter(Transform slot)
    {
        if (questM.questSlotOptions && questM.questSlotOptions.activeInHierarchy)
            return;

        if (menuMoveAS)
            menuMoveAS.PlayRandomClip();

        switch (questType)
        {
            case QuestType.MainQuest:
                questM.mainQuestSlots.slotNum = slotNum;
                oldHighLight = questM.mainQuestSlots.highLight[slotNum].color;
                questM.mainQuestSlots.highLight[slotNum].color = questM.highlightColor;
                break;
            case QuestType.SideQuest:
                questM.sideQuestSlots.slotNum = slotNum;
                oldHighLight = questM.sideQuestSlots.highLight[slotNum].color;
                questM.sideQuestSlots.highLight[slotNum].color = questM.highlightColor;
                break;
        }
    }


    public void OnButtonSelect(Transform slot)
    {
        foreach (Image hl in questM.mainQuestSlots.highLight)
            if (hl.color != questM.neutralColor)
                hl.color = questM.neutralColor;

        foreach (Image hl in questM.sideQuestSlots.highLight)
            if (hl.color != questM.neutralColor)
                hl.color = questM.neutralColor;

        switch (questType)
        {
            case QuestType.MainQuest:
                if (!questM.mainQuestSlots.questData[slotNum].isComplete)
                {
                    highLight.color = questM.highlightColor;
                    questM.questSlotOptions.SetActive(true);
                    questM.questSlotOptions.GetComponent<QuestSlotOptions>().questData = questM.mainQuestSlots.questData[questM.mainQuestSlots.slotNum];
                    questM.questSlotOptions.transform.position = questM.mainQuestSlots.highLight[questM.mainQuestSlots.slotNum].transform.position;
                }
                break;
            case QuestType.SideQuest:
                if (!questM.sideQuestSlots.questData[slotNum].isComplete)
                {
                    highLight.color = questM.highlightColor;
                    questM.questSlotOptions.SetActive(true);
                    questM.questSlotOptions.GetComponent<QuestSlotOptions>().questData = questM.sideQuestSlots.questData[questM.sideQuestSlots.slotNum];
                    questM.questSlotOptions.transform.position = questM.sideQuestSlots.highLight[questM.sideQuestSlots.slotNum].transform.position;
                }
                break;
        }
    }

    public void OnButtonExit(Transform slot)
    {
        if (questM.questSlotOptions && questM.questSlotOptions.activeInHierarchy)
            return;

        switch (questType)
        {
            case QuestType.MainQuest:
                questM.mainQuestSlots.highLight[slotNum].color = oldHighLight;
                break;
            case QuestType.SideQuest:
                questM.sideQuestSlots.highLight[slotNum].color = oldHighLight;
                break;
        }
    }

    void SetDefaultSettings()
    {
        fadeUI.canvasGroup.alpha = 1;

        switch (questType)
        {
            case QuestType.MainQuest:

                Transform mainQuestLocation = questM.mainQuestSlots.questData[slotNum].questObjective[questM.mainQuestSlots.questData[slotNum].questObjectiveNum].questLocation;
                TextMeshProUGUI mainQuestDistance = questM.mainQuestSlots.questDistance[slotNum];

                if (mainQuestLocation == null)
                {
                    cc.infoMessage.info.text = "Please add (objective location transform) in QuestData and try this again.";
                    return;
                }

                #region Update MiniMap Distance

                float mainQuestDist = Mathf.Round(Vector3.Distance(mainQuestLocation.position, player.position));
                mainQuestDistance.text = mainQuestDist.ToString() + "m";

                #endregion

                break;
            case QuestType.SideQuest:

                Transform sideQuestLocation = questM.sideQuestSlots.questData[slotNum].questObjective[questM.sideQuestSlots.questData[slotNum].questObjectiveNum].questLocation;
                TextMeshProUGUI sideQuestDistance = questM.sideQuestSlots.questDistance[slotNum];

                if (sideQuestLocation == null)
                {
                    cc.infoMessage.info.text = "Please add (objective location transform) in QuestData and try this again.";
                    return;
                }

                #region Update MiniMap Distance

                float sideQuestDist = Mathf.Round(Vector3.Distance(sideQuestLocation.position, player.position));
                sideQuestDistance.text = sideQuestDist.ToString() + "m";

                #endregion

                break;
        }

        if (!setPosition && cc.inventoryM.referencesObj.questSection.activeInHierarchy)
        {
            if (questType == QuestType.MainQuest)
                panel = GameObject.Find("MainQuestContent");
            if (questType == QuestType.SideQuest)
                panel = GameObject.Find("SideQuestContent");

            rectTransform.SetParent(panel.GetComponent<RectTransform>().transform);

            rectTransform.localPosition = new Vector3(0, 0, 0);
            rectTransform.localRotation = Quaternion.Euler(0, 0, 0);
            rectTransform.localScale = new Vector3(1, 1, 1);
            setPosition = true;
        }
    }

    public enum QuestType
    {
        MainQuest,
        SideQuest,
        NA
    }
}