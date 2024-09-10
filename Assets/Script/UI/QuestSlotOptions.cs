using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using RPGAE.CharacterController;

public class QuestSlotOptions : MonoBehaviour
{
    readonly List<string> names = new List<string>() { "Choose:", "Activate", "Deactivate",  "Cancel" };

    [Header("AUDIO")]
    public RandomAudioPlayer cancelAS;

    #region Private 

    private bool onButton;
    private TMP_Dropdown dropDown;
    private TextMeshProUGUI selectedName;
    private GameObject dropDownObj;
    private InventoryManager inventoryM;
    private QuestManager questM;
    private ThirdPersonController cc;
    [HideInInspector] public QuestData questData;

    #endregion

    // Use this for initialization
    void Awake()
    {
        dropDown = GetComponent<TMP_Dropdown>();
        inventoryM = FindObjectOfType<InventoryManager>();
        questM = FindObjectOfType<QuestManager>();
        selectedName = GameObject.Find("QuestSlotLabel").GetComponent<TextMeshProUGUI>();
        PopulateList();
    }

    // Update is called once per frame
    void Update()
    {
        dropDownObj = GameObject.Find("Dropdown List");

        if (onButton)
        {
            if (inventoryM.cc.rpgaeIM.PlayerControls.Action.triggered)
            {
                dropDown.Show();
                onButton = false;
            }
        }
    }

    public void DropDownSelect(int index)
    {
        selectedName.text = names[index];

        if (index == 1)
        {
            if (questData.questType == QuestData.QuestType.MainQuest)
            {
                questM.MainQuestSlotSelect(questM.mainQuestSlots.slotNum);
            }

            if (questData.questType == QuestData.QuestType.SideQuest)
            {
                questM.SideQuestSlotSelect(questM.sideQuestSlots.slotNum);
            }

            dropDown.value = 0;
            DestroyImmediate(dropDownObj);
            gameObject.SetActive(false);
        }

        if (index == 2)
        {
            if(questData.questType == QuestData.QuestType.MainQuest)
            {
                QuestData mainQuestData = questM.mainQuestSlots.questData[questM.mainQuestSlots.slotNum];
                Image mainQuestHighLight = questM.mainQuestSlots.highLight[questM.mainQuestSlots.slotNum];

                mainQuestData.isQuestActive = false;
                mainQuestHighLight.color = questM.neutralColor;
            }

            if (questData.questType == QuestData.QuestType.SideQuest)
            {
                QuestData sideQuestData = questM.sideQuestSlots.questData[questM.sideQuestSlots.slotNum];
                Image sideQuestHighLight = questM.sideQuestSlots.highLight[questM.sideQuestSlots.slotNum];

                sideQuestData.isQuestActive = false;
                sideQuestHighLight.color = questM.neutralColor;
            }

            cc.hudM.fadeUIBG.canvasGroup.alpha = 0;
            cc.hudM.fadeUIFadeGroup.canvasGroup.alpha = 0;
            cc.hudM.fadeUIQuestObjective.canvasGroup.alpha = 0;
            cc.hudM.fadeUIQuestObjective.isFading = false;
            cc.hudM.fadeUIBG.isFading = false;
            cc.hudM.fadeUIFadeGroup.isFading = false;

            dropDown.value = 0;
            DestroyImmediate(dropDownObj);
            gameObject.SetActive(false);
        }

        if (index == 3)
        {
            if (questData.questType == QuestData.QuestType.MainQuest)
            {
                QuestData mainQuestData = questM.mainQuestSlots.questData[questM.mainQuestSlots.slotNum];
                Image mainQuestHighLight = questM.mainQuestSlots.highLight[questM.mainQuestSlots.slotNum];

                if (mainQuestData.isQuestActive)
                    mainQuestHighLight.color = questM.selectColor;
                else
                    mainQuestHighLight.color = questM.neutralColor;
            }

            if (questData.questType == QuestData.QuestType.SideQuest)
            {
                QuestData sideQuestData = questM.sideQuestSlots.questData[questM.sideQuestSlots.slotNum];
                Image sideQuestHighLight = questM.sideQuestSlots.highLight[questM.sideQuestSlots.slotNum];

                if (sideQuestData.isQuestActive)
                    sideQuestHighLight.color = questM.selectColor;
                else
                    sideQuestHighLight.color = questM.neutralColor;
            }

            dropDown.value = 0;
            DestroyImmediate(dropDownObj);
            gameObject.SetActive(false);
        }
    }

    void PopulateList()
    {
        dropDown.AddOptions(names);
    }
}
