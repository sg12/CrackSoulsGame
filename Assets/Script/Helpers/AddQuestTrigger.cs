using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddQuestTrigger : MonoBehaviour
{
    private BoxCollider boxC;
    private QuestData questData;
    private QuestManager questM;
    private InventoryManager inventoryM;

    private bool oneShot;

    // Start is called before the first frame update
    void Start()
    {
        boxC = GetComponent<BoxCollider>();
        boxC.enabled = false;
        questData = GetComponent<QuestData>();
        questM = FindObjectOfType<QuestManager>();
        inventoryM = FindObjectOfType<InventoryManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (inventoryM.systemM.loadingScreenFUI.canvasGroup.alpha == 0)
            boxC.enabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !oneShot && PlayerPrefs.GetInt("SceneInTransitionDataRef") == 0)
        {
            if (questData.questType == QuestData.QuestType.MainQuest)
            {
                // Increment the Quest slot size by 1
                QuestManager.numOfMainQuestSlot += 1;
                // Add Quest slot
                questM.AddMainQuest(questData);
                questM.MainQuestSlotSelect(questM.mainQuestSlots.slotNum);
            }

            if (questData.questType == QuestData.QuestType.SideQuest)
            {
                // Increment the Quest slot size by 1
                QuestManager.numOfSideQuestSlot += 1;
                // Add Quest slot
                questM.AddMainQuest(questData);
                questM.SideQuestSlotSelect(questM.sideQuestSlots.slotNum);
            }
            oneShot = true;
        }
    }
}
