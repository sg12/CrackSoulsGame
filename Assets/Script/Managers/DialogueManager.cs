using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using RPGAE.CharacterController;

public class DialogueManager : MonoBehaviour
{
    public Dialogue curDialogue;

    [Header("AUDIO")]
    public RandomAudioPlayer dialogueStart;
    public RandomAudioPlayer dialogueType;
    public RandomAudioPlayer dialogueNext;
    public RandomAudioPlayer dialogueEnd;
    public RandomAudioPlayer buttonSelect;

    [Header("REFERENCES")]
    public FadeUI fadeUI;
    public RawImage background;
    public TextMeshProUGUI npcName;
    public TextMeshProUGUI dialogue;
    public TextMeshProUGUI optionA;
    public TextMeshProUGUI optionB;
    public Slider timerQuestion;
    public GameObject optionButton, continueButton;
    public CanvasGroup[] fadeUIOnDialogue;

    protected int numOfLetters = 0;
    protected Queue<string> sentences;

    #region Private 

    public enum SentenceType
    {
        Default,
        DefaultAlt,
        OptionA,
        OptionAAlt,
        OptionB,
        OptionBAlt,
        CompleteQuest,
        CompletedQuest
    }
    private ThirdPersonController cc;
    private StoreManager storeM;
    [HideInInspector] public QuestManager questM;
    [HideInInspector] public QuestData currentQuest;
    [HideInInspector] public QuestData previousQuest;
    [HideInInspector] public InventoryManager inventoryM;
    [HideInInspector] public SentenceType sentenceType;

    #endregion

    void Start()
    {
        sentences = new Queue<string>();
        optionButton.SetActive(false);

        // Get Components
        questM = FindObjectOfType<QuestManager>();
        inventoryM = FindObjectOfType<InventoryManager>();
        cc = FindObjectOfType<ThirdPersonController>();
        storeM = FindObjectOfType<StoreManager>();
    }

    void Update()
    {
        QuestionTimer();
        NextSentenceInput();

        if(npcName.text != "")
            FadeUIOnDialogue(0);
    }

    public void FadeUIOnDialogue(int num)
    {
        for (int i = 0; i < fadeUIOnDialogue.Length; i++)
        {
            if (fadeUIOnDialogue[i] != null)
            {
                fadeUIOnDialogue[i].alpha = num;
            }
        }
    }

    void QuestionTimer()
    {
        if (!optionButton.activeInHierarchy || !curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].timedQuestion)
            return;

        timerQuestion.value -= 0.2f * Time.deltaTime;
        timerQuestion.GetComponent<FadeUI>().canvasGroup.alpha = 1;
        if (timerQuestion.value == 0)
        {
            OptionBButton();
            timerQuestion.value = timerQuestion.maxValue;
            timerQuestion.GetComponent<FadeUI>().canvasGroup.alpha = 0;
        }
    }

    void NextSentenceInput()
    {
        //If Continue button is active display the next sentence.
        if (continueButton.activeInHierarchy && (cc.rpgaeIM.PlayerControls.Attack.triggered
            || cc.rpgaeIM.PlayerControls.Action.triggered || cc.rpgaeIM.PlayerControls.Start.triggered))
        {
            DisplayNextSentence();
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        curDialogue = dialogue;
        curDialogue.questData = dialogue.questData;
        curDialogue.controller = dialogue.controller;

        npcName.text = curDialogue.npcName;
        background.texture = curDialogue.background;

        optionA.text = curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].optionAAnswer;
        optionB.text = curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].optionBAnswer;

        if (dialogueStart)
            dialogueStart.PlayRandomClip();

        // Prevents the dialogue from leading into a question if there isn't any dialogue for it.
        for (int i = 0; i < curDialogue.dialogueSegment.Length; i++)
        {
            if (curDialogue.dialogueSegment[i].optionASentence.Length == 0 ||
                curDialogue.dialogueSegment[i].optionBSentence.Length == 0)
            {
                curDialogue.dialogueSegment[i].triggerOption = false;
            }
        }

        foreach (string sentence in curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].defaultSentence)
        {
            sentences.Enqueue(sentence);
        }

        // If you have quests in progress, check if objectives were completed or not.
        if (questM.mainQuestSlots.questData.Length > 0)
            CheckForActiveQuest(ref questM.mainQuestSlots.questData, ref questM.mainQuestSlots.highLight, ref questM.mainQuestSlots.questLocationName, 1);
        if (questM.sideQuestSlots.questData.Length > 0)
            CheckForActiveQuest(ref questM.sideQuestSlots.questData, ref questM.sideQuestSlots.highLight, ref questM.sideQuestSlots.questLocationName, 1);

        DisplayNextSentence();
    }

    public void OptionAButton()
    {
        sentences.Clear();
        foreach (string sentence in curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].optionASentence)
        {
            sentences.Enqueue(sentence);
        }

        switch (curDialogue.dialogueType)
        {
            case Dialogue.DialogueType.QuestGiver:

                #region Quest Type Switch

                switch (curDialogue.questData.questType)
                {
                    case QuestData.QuestType.MainQuest:
                        if (currentQuest == null)
                        {
                            Debug.LogWarning("Please attach (Quest Data script) as child to " + curDialogue.controller.gameObject.name);
                        }
                        else
                        {
                            // Increment the Quest slot size by 1
                            QuestManager.numOfMainQuestSlot += 1;

                            // Add Quest slot
                            questM.AddMainQuest(currentQuest);
                            curDialogue.controller.state = (AIController.State)curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].newAIState;
                        }
                        break;
                    case QuestData.QuestType.SideQuest:
                        if (currentQuest == null)
                        {
                            Debug.LogWarning("Please attach (Quest Data script) as child to " + curDialogue.controller.gameObject.name);
                        }
                        else
                        {
                            // Increment the Quest slot size by 1
                            QuestManager.numOfSideQuestSlot += 1;

                            // Add Quest slot
                            questM.AddSideQuest(currentQuest);
                            curDialogue.controller.state = (AIController.State)curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].newAIState;
                        }
                        break;
                }
                #endregion

                break;
            case Dialogue.DialogueType.FollowAsCompanion:
                curDialogue.controller.GetComponent<HumanoidBehavior>().state = (HumanoidBehavior.State)curDialogue.controller.state;
                break;
        }

        if(curDialogue.dialogueType == Dialogue.DialogueType.FollowAsCompanion)
        {
            curDialogue.isDeactivated = true;
            curDialogue.controller.state = AIController.State.FollowCompanion;
            curDialogue.controller.GetComponent<HumanoidBehavior>().state = (HumanoidBehavior.State)curDialogue.controller.state;
        }

        if (buttonSelect)
            buttonSelect.PlayRandomClip();

        curDialogue.dialogueState = Dialogue.DialogueState.OptionA;

        DisplayNextSentence();
        timerQuestion.value = timerQuestion.maxValue;
        optionButton.SetActive(false);
    }

    public void OptionBButton()
    {
        sentences.Clear();
        foreach (string sentence in curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].optionBSentence)
        {
            sentences.Enqueue(sentence);
        }
        switch (curDialogue.dialogueType)
        {
            case Dialogue.DialogueType.FollowAsCompanion:
                // If you're already following change to new state.
                if (curDialogue.controller.dialogue.dialogueAltered)
                {
                    // Cast NPC AI state to dialogue AI state.
                    curDialogue.controller.state = (AIController.State)curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].newAIState;
                    curDialogue.controller.dialogue.dialogueAltered = false;
                }
                break;
        }
        if (buttonSelect)
            buttonSelect.PlayRandomClip();

        curDialogue.dialogueState = Dialogue.DialogueState.OptionB;

        DisplayNextSentence();
        optionButton.SetActive(false);
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        TalkAudio();
        TalkAnimation();
        SetCameraLookAt();

        // Reset letter counter.
        numOfLetters = 0;

        // Deactivate continue button.
        continueButton.SetActive(false);
        inventoryM.cursorVirtual.isActive = false;

        if (dialogueNext)
            dialogueNext.PlayRandomClip();

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence, 0));
    }

    IEnumerator TypeSentence(string sentence, float delay)
    {
        yield return new WaitForSeconds(delay);
        dialogue.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            numOfLetters++;
            yield return null;
            dialogue.text += letter;

            if (dialogueType && !dialogueType.playing)
                dialogueType.PlayRandomClip();

            if (numOfLetters == sentence.ToCharArray().Length)
                continueButton.SetActive(true);

            if (numOfLetters == sentence.ToCharArray().Length && sentences.Count == 0 && curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].triggerOption
            && (curDialogue.dialogueState != Dialogue.DialogueState.OptionA && curDialogue.dialogueState != Dialogue.DialogueState.OptionB))
            {
                inventoryM.cursorVirtual.isActive = true;
                optionButton.SetActive(true);
                continueButton.SetActive(false);
            }
        }
    }

    void EndDialogue()
    {
        npcName.text = "";
        dialogue.text = "";
        FadeUIOnDialogue(1);
        fadeUI.FadeTransition(0, 0, 0.2f);
        curDialogue.defaultSegment = 0;
        numOfLetters = 0;
        continueButton.SetActive(false);
        cc.tpCam.RemoveTargets();
        if (dialogueEnd)
            dialogueEnd.PlayRandomClip();

        OpenStoreOptions();

        // When the dialogue ends you're given items from NPC.
        ReceivedItems();

        if (curDialogue.dialogueType == Dialogue.DialogueType.QuestGiver)
        {
            if (curDialogue.dialogueState == Dialogue.DialogueState.Default || curDialogue.dialogueState == Dialogue.DialogueState.OptionA)
            {
                if (curDialogue.questData.questType == QuestData.QuestType.MainQuest)
                    questM.MainQuestSlotSelect(questM.mainQuestSlots.slotNum);
                if (curDialogue.questData.questType == QuestData.QuestType.SideQuest)
                    questM.SideQuestSlotSelect(questM.sideQuestSlots.slotNum);
            }
        }

        // If option A is selected, change the dialogue to alternative sentences.
        if (curDialogue.dialogueState == Dialogue.DialogueState.OptionA)
        {
            if (curDialogue.dialogueSegmentNum < curDialogue.dialogueSegment.Length - 1)
                curDialogue.dialogueSegmentNum++;

            curDialogue.dialogueAltered = true;
            curDialogue.dialogueState = Dialogue.DialogueState.Default;
        }

        if (curDialogue.dialogueAltered && curDialogue.dialogueType == Dialogue.DialogueType.QuestGiver)
            curDialogue.controller.dialogue.dialogueType = Dialogue.DialogueType.QuestInProgress;

        // If option B is selected, resets and you can ask the questiion again.
        if (curDialogue.dialogueState == Dialogue.DialogueState.OptionB)
        {
            curDialogue.dialogueAltered = false;
            curDialogue.dialogueState = Dialogue.DialogueState.Default;
        }

        // Adds progression token on the selected quest.
        if (curDialogue.dialogueType == Dialogue.DialogueType.QuestInProgress && sentenceType != SentenceType.CompleteQuest)
        {
            questM.AddQuestObjective(ref curDialogue.questSequenceNameReceipt, ref curDialogue.questPhaseReceipt,
            ref curDialogue.addQuestPhaseBy, ref curDialogue.addQuestQuantityBy);

            #region Update Dialogue Phases Relating To The Quest 

            foreach (AIController npc in FindObjectsOfType<AIController>())
            {
                for (int i = 0; i < questM.mainQuestSlots.questData.Length; i++)
                {
                    if (questM.mainQuestSlots.questData[i].questName == npc.dialogue.questSequenceNameReceipt)
                    {
                      npc.dialogue.questPhaseReceipt++;
                    }
                }

                for (int i = 0; i < questM.sideQuestSlots.questData.Length; i++)
                {
                    if (questM.sideQuestSlots.questData[i].questName == npc.dialogue.questSequenceNameReceipt)
                    {
                      npc.dialogue.questPhaseReceipt++;
                    }
                }
            }

            #endregion
        }

        curDialogue.controller.animator.SetFloat("TalkID", 0);

        if(curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].newCamPos.Length > 0 &&
        curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].newCamLookAt.Length > 0)
        {
            if (curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].newCamPos[curDialogue.defaultSegment]
            && curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].newCamLookAt[curDialogue.defaultSegment])
            {
                cc.tpCam.FadeUIOnCinematicCamera(1);
                FindObjectOfType<ThirdPersonCamera>().newCamPos = null;
                FindObjectOfType<ThirdPersonCamera>().newCamLookAt = null;
                FindObjectOfType<ThirdPersonCamera>().cameraState = ThirdPersonCamera.CameraState.Orbit;
            }
        }

        if (curDialogue.controller.navmeshAgent && curDialogue.controller.grounded)
            curDialogue.controller.navmeshAgent.isStopped = false;

        GetComponent<AudioSource>().clip = null;
        curDialogue.controller.animator.SetBool("InDialogue", false);

        if (curDialogue.dialogueType != Dialogue.DialogueType.Store)
        {
            curDialogue.controller.dialogueActive = false;
            curDialogue = null;
        }
        if (curDialogue == null)
            cc.canMove = true;
    }

    void CheckForActiveQuest(ref QuestData[] questData, ref Image[] questHighLight, ref TextMeshProUGUI[] questLocationName, int phaseIncrement)
    {
        if (curDialogue.dialogueType == Dialogue.DialogueType.QuestInProgress)
        {
            curDialogue.controller.dialogue.dialogueSegment[curDialogue.dialogueSegmentNum].triggerOption = false;
            for (int i = 0; i < questData.Length; i++)
            {
                #region Check Quest Completion (Phase & Collectables)

                if (questData[i] != null)
                {
                    if (!questData[i].isComplete)
                    {
                        if (questData[i].questName == curDialogue.questSequenceNameReceipt)
                        {
                            if (questData[i].curObjectivePhase == questData[i].maxObjectivePhase || questData[i].questObjective[questData[i].questObjectiveNum].curCollectableAmount == questData[i].questObjective[questData[i].questObjectiveNum].maxCollectableAmount)
                            {
                                sentences.Clear();
                                foreach (string sentence in curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].completedRequired)
                                {
                                    sentences.Enqueue(sentence);
                                }
                                RemoveItemFromInventory();
                                sentenceType = SentenceType.CompletedQuest;
                            }
                            else
                            {
                                sentences.Clear();
                                foreach (string sentence in curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].completeRequired)
                                {
                                    sentences.Enqueue(sentence);
                                }
                                sentenceType = SentenceType.CompleteQuest;
                            }
                        }
                        else
                        {
                            sentences.Clear();
                            foreach (string sentence in curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].defaultSentence)
                            {
                                sentences.Enqueue(sentence);
                            }
                        }
                    }
                    else if (questHighLight[i].color == questM.neutralColor)
                    {
                        if (curDialogue.questSequenceNameReceipt == questData[i].questName)
                        {
                            sentences.Clear();
                            foreach (string sentence in curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].completeRequired)
                            {
                                sentences.Enqueue(sentence);
                            }
                            sentenceType = SentenceType.CompleteQuest;
                        }
                        else
                        {
                            sentences.Clear();
                            foreach (string sentence in curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].defaultSentence)
                            {
                                sentences.Enqueue(sentence);
                            }
                            sentenceType = SentenceType.Default;
                        }
                    }
                }

                #endregion

                #region Check Quest Change State (Quest Phase)

                if (questData[i] != null)
                {
                    if (!questData[i].isComplete)
                    {
                        if (questData[i].questName == curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].questSequenceNameReceipt)
                        {
                            if (questData[i].curObjectivePhase == curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].questPhaseReceipt)
                            {
                                sentences.Clear();
                                foreach (string sentence in curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].completedRequired)
                                {
                                    sentences.Enqueue(sentence);
                                }
                                sentenceType = SentenceType.CompletedQuest;
                                curDialogue.controller.dialogue.dialogueSegment[curDialogue.dialogueSegmentNum].hasReceived = true;
                                curDialogue.controller.state = (AIController.State)curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].questNewState;
                                questM.ChangeCurrentObjective(ref questData, ref questHighLight, ref questLocationName, ref phaseIncrement);
                            }
                            else
                            {
                                sentences.Clear();
                                foreach (string sentence in curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].completeRequired)
                                {
                                    sentences.Enqueue(sentence);
                                }
                                sentenceType = SentenceType.CompleteQuest;
                            }
                        }
                        else
                        {
                            sentences.Clear();
                            foreach (string sentence in curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].defaultSentence)
                            {
                                sentences.Enqueue(sentence);
                            }
                        }
                    }
                    else if (questHighLight[i].color == questM.neutralColor)
                    {
                        if (curDialogue.questSequenceNameReceipt == questData[i].questName)
                        {
                            sentences.Clear();
                            foreach (string sentence in curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].completeRequired)
                            {
                                sentences.Enqueue(sentence);
                            }
                            sentenceType = SentenceType.CompleteQuest;
                        }
                        else
                        {
                            sentences.Clear();
                            foreach (string sentence in curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].defaultSentence)
                            {
                                sentences.Enqueue(sentence);
                            }
                            sentenceType = SentenceType.Default;
                        }
                    }
                }

                #endregion
            }
        }
    }

    void RemoveItemFromInventory()
    {
        for (int i = 0; i < inventoryM.materialInv.itemData.Length; i++)
        {
            if (curDialogue.itemName == inventoryM.materialInv.itemData[i].itemName
               && curDialogue.removeQuantity >= inventoryM.materialInv.itemData[i].quantity)
            {
                inventoryM.RemoveStackableItem(ref inventoryM.materialInv.itemData, ref inventoryM.materialInv.image,
                ref inventoryM.materialInv.highLight, ref inventoryM.materialInv.outLineBorder, ref inventoryM.materialInv.statValueBG, ref inventoryM.materialInv.quantity,
                ref inventoryM.materialInv.statValue, ref inventoryM.materialInv.slotNum, ref inventoryM.materialInv.counter,
                ref inventoryM.materialInv.statCounter, ref inventoryM.materialInv.removeNullSlots, curDialogue.removeQuantity);
            }
        }

        for (int i = 0; i < inventoryM.healingInv.itemData.Length; i++)
        {
            if (curDialogue.itemName == inventoryM.healingInv.itemData[i].itemName
               && curDialogue.removeQuantity >= inventoryM.healingInv.itemData[i].quantity)
            {
                inventoryM.RemoveStackableItem(ref inventoryM.healingInv.itemData, ref inventoryM.healingInv.image,
                ref inventoryM.healingInv.highLight, ref inventoryM.healingInv.outLineBorder, ref inventoryM.healingInv.statValueBG, ref inventoryM.healingInv.quantity,
                ref inventoryM.healingInv.statValue, ref inventoryM.healingInv.slotNum, ref inventoryM.healingInv.counter,
                ref inventoryM.healingInv.statCounter, ref inventoryM.healingInv.removeNullSlots, curDialogue.removeQuantity);
            }
        }
    }

    void ReceivedItems()
    {
        if (curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].givenItemPrefab != null
            && curDialogue.dialogueState == curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].dialogueItemTrigger
            && !curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].hasReceived)
        {
            for (int i = 0; i < curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].givenItemPrefab.Length; i++)
            {
                GameObject givenItemPrefab = Instantiate(curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].givenItemPrefab[i]) as GameObject;
                ItemData givenItemPrefabConvert = givenItemPrefab.GetComponentInChildren<ItemData>();
                givenItemPrefab.gameObject.SetActive(false);

                switch (givenItemPrefabConvert.itemType)
                {
                    case ItemData.ItemType.Weapon:
                        inventoryM.AddItemDataSlot(ref inventoryM.weaponInv.itemData, ref inventoryM.weaponInv.image, ref givenItemPrefab);
                        break;
                    case ItemData.ItemType.Bow:
                        inventoryM.AddItemDataSlot(ref inventoryM.bowAndArrowInv.itemData, ref inventoryM.bowAndArrowInv.image, ref givenItemPrefab);
                        break;
                    case ItemData.ItemType.Arrow:
                        inventoryM.AddItemDataSlot(ref inventoryM.bowAndArrowInv.itemData, ref inventoryM.bowAndArrowInv.image, ref givenItemPrefab);
                        break;
                    case ItemData.ItemType.Shield:
                        inventoryM.AddItemDataSlot(ref inventoryM.shieldInv.itemData, ref inventoryM.shieldInv.image, ref givenItemPrefab);
                        break;
                    case ItemData.ItemType.Armor:
                        inventoryM.AddItemDataSlot(ref inventoryM.armorInv.itemData, ref inventoryM.armorInv.image, ref givenItemPrefab);
                        break;
                    case ItemData.ItemType.Material:
                        inventoryM.AddItemDataSlot(ref inventoryM.materialInv.itemData, ref inventoryM.materialInv.image, ref givenItemPrefab);
                        break;
                    case ItemData.ItemType.Healing:
                        inventoryM.AddItemDataSlot(ref inventoryM.healingInv.itemData, ref inventoryM.healingInv.image, ref givenItemPrefab);
                        break;
                    case ItemData.ItemType.Key:
                        inventoryM.AddItemDataSlot(ref inventoryM.keyInv.itemData, ref inventoryM.keyInv.image, ref givenItemPrefab);
                        break;
                    case ItemData.ItemType.General:
                        if (givenItemPrefabConvert.itemName == "Coin" || givenItemPrefabConvert.itemName == "Coin bag")
                        {
                            inventoryM.unityCoins += givenItemPrefabConvert.quantity;
                            inventoryM.unityCoinsText.text = "x" + inventoryM.unityCoins.ToString();
                        }
                        inventoryM.AddedItemUI(ref givenItemPrefab);
                        break;
                }
            }
            bool conditionToIncrement = !curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].triggerOption;
            if (curDialogue.dialogueSegmentNum < curDialogue.dialogueSegment.Length - 1 && conditionToIncrement)
                curDialogue.dialogueSegmentNum++;

            curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].hasReceived = true;
        }
    }

    void OpenStoreOptions()
    {
        if (curDialogue.dialogueType == Dialogue.DialogueType.Store
        && curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].triggerOption && curDialogue.dialogueState == Dialogue.DialogueState.OptionA)
        {
            storeM.gameObject.SetActive(true);
            storeM.fadeUI.isFading = false;
            storeM.fadeUI.FadeTransition(1, 0, 0.5f);

            inventoryM.miniMap.fadeUI.FadeTransition(0, 0, 0.5f);
            inventoryM.referencesObj.inventorySection.GetComponent<HideUI>().enabled = false;
            inventoryM.referencesObj.inventorySection.SetActive(false);
        }
        else if (curDialogue.dialogueType == Dialogue.DialogueType.Store
        && curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].triggerOption && curDialogue.dialogueState == Dialogue.DialogueState.OptionB)
        {
            cc.canMove = true;
            cc.tpCam.RemoveTargets(); 
            inventoryM.ItemOptionsIsActive(false);
            storeM.dialogueM.curDialogue.controller.dialogueActive = false;
        }
        else if (curDialogue.dialogueType == Dialogue.DialogueType.Store && curDialogue.dialogueState == Dialogue.DialogueState.Default)
        {
            storeM.gameObject.SetActive(true);
            storeM.fadeUI.isFading = false;
            storeM.fadeUI.FadeTransition(1, 0, 0.5f);

            inventoryM.miniMap.fadeUI.FadeTransition(0, 0, 0.5f);
            inventoryM.referencesObj.inventorySection.GetComponent<HideUI>().enabled = false;
            inventoryM.referencesObj.inventorySection.SetActive(false);
        }
    }

    void SetCameraLookAt()
    {
        if (curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].newCamPos.Length < (curDialogue.defaultSegment + 1)
        && curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].newCamLookAt.Length < (curDialogue.defaultSegment + 1))
        {
            cc.tpCam.FadeUIOnCinematicCamera(1);
            FindObjectOfType<ThirdPersonCamera>().newCamPos = null;
            FindObjectOfType<ThirdPersonCamera>().newCamLookAt = null;
            FindObjectOfType<ThirdPersonCamera>().cameraState = ThirdPersonCamera.CameraState.Orbit;
            return;
        }

        if (curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].newCamPos[curDialogue.defaultSegment]
        && curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].newCamLookAt[curDialogue.defaultSegment])
        {
            FindObjectOfType<ThirdPersonCamera>().SetCinematicCamera((ThirdPersonCamera.CameraState)curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].cameraState, 
            curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].newCamPos[curDialogue.defaultSegment], 
            curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].newCamLookAt[curDialogue.defaultSegment], 
            curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].cameraStayTime);
        }
        else
        {
            cc.tpCam.FadeUIOnCinematicCamera(1);
            FindObjectOfType<ThirdPersonCamera>().newCamPos = null;
            FindObjectOfType<ThirdPersonCamera>().newCamLookAt = null;
            FindObjectOfType<ThirdPersonCamera>().cameraState = ThirdPersonCamera.CameraState.Orbit;
        }

        if (curDialogue.defaultSegment < curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].defaultSentence.Length - 1)
            curDialogue.defaultSegment++;
    }

    void TalkAudio()
    {
        if (curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].talkAudio != null && GetComponent<AudioSource>().clip == null)
        {
            GetComponent<AudioSource>().clip = curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].talkAudio;
            GetComponent<AudioSource>().Play();
        }
    }

    void TalkAnimation()
    {
        switch (curDialogue.dialogueSegment[curDialogue.dialogueSegmentNum].talkAnim)
        {
            case Dialogue.TalkAnim.None:
                curDialogue.controller.animator.SetFloat("TalkID", 0);
                break;
            case Dialogue.TalkAnim.Talk1:
                curDialogue.controller.animator.SetFloat("TalkID", 1);
            break;
            case Dialogue.TalkAnim.Talk2:
                curDialogue.controller.animator.SetFloat("TalkID", 2);
                break;
            case Dialogue.TalkAnim.Talk3:
                curDialogue.controller.animator.SetFloat("TalkID", 3);
                break;
            case Dialogue.TalkAnim.Talk4:
                curDialogue.controller.animator.SetFloat("TalkID", 4);
                break;
            case Dialogue.TalkAnim.Talk5:
                curDialogue.controller.animator.SetFloat("TalkID", 5);
                break;
            case Dialogue.TalkAnim.Talk6:
                curDialogue.controller.animator.SetFloat("TalkID", 6);
                break;
            case Dialogue.TalkAnim.Talk7:
                curDialogue.controller.animator.SetFloat("TalkID", 7);
                break;
            case Dialogue.TalkAnim.Talk8:
                curDialogue.controller.animator.SetFloat("TalkID", 8);
                break;
        }
    }
}
