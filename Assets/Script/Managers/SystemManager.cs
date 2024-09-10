using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using RPGAE.CharacterController;
using JoshH.UI;

using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SystemManager : MonoBehaviour
{
    [Header("OPTIONS SETTINGS")]
    public bool invertCameraV;
    public bool invertCameraH;
    [Range(25f, 100)]
    public float cameraSensitivity = 25f;
    public bool SFX;
    public bool music;

    [Header("REFERENCES")]
    public GameObject itemDataPrefab;
    public GameObject questSlotPrefab;
    public GameObject questObjectivePosPrefab;
    public FadeUI rootCtrlLayout;
    public FadeUI KeyboardFadeUI;
    public FadeUI XboxFadeUI;
    public FadeUI PSFadeUI;
    public FadeUI blackScreenFUI;
    public FadeUI loadingScreenFUI;
    public TextMeshProUGUI loadingText;
    public TextMeshProUGUI loadingDataNameText;
    public FadeUI gameOverFUI;
    public FadeUI gameOverOptionFUI;

    #region private

    [HideInInspector]
    public HUDManager hudM;
    [HideInInspector]
    public MiniMap miniMap;
    [HideInInspector]
    public QuestManager questM;
    [HideInInspector]
    public ThirdPersonController cc;
    [HideInInspector]
    public InventoryManager inventoryM;
    [HideInInspector]
    public SkillsManager skillM;
    [HideInInspector]
    public ItemObtained itemObtained;
    [HideInInspector]
    public SkillSlot[] skillSlots;
    [HideInInspector]
    public QuestData[] questData;
    [HideInInspector]
    public QuestSlot[] questSlots;
    [HideInInspector]
    public InventorySlot[] inventorySlot;
    [HideInInspector] 
    public AIController[] dialogueData;
    [HideInInspector]
    public LocationCheckPoint[] locationCPData;
    [HideInInspector]
    public TriggerAction[] triggerActionData;
    [HideInInspector]
    public StartInventoryItems[] sitems;

    [HideInInspector] 
    public GameData globalData;
    [HideInInspector] 
    public GameData transitionData;
    [HideInInspector]
    public GameData sceneData;

    [HideInInspector] public bool isLoadingData;

    public bool canLoadScene = false;

    #endregion

    public List<string> scenes;

    // Start is called before the first frame update
    void Awake()
    {
        hudM = FindObjectOfType<HUDManager>();
        cc = FindObjectOfType<ThirdPersonController>();
        miniMap = FindObjectOfType<MiniMap>();
        questM = FindObjectOfType<QuestManager>();
        inventoryM = FindObjectOfType<InventoryManager>();
        itemObtained = FindObjectOfType<ItemObtained>();
        skillM = FindObjectOfType<SkillsManager>();

        InitializeGame();
    }

    void InitializeGame()
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < sceneCount; i++)
        {
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
            scenes.Add(sceneName);
        }

        inventoryM.referencesObj.pauseMenu.GetComponent<FadeUI>().canvasGroup.alpha = 0;
        loadingScreenFUI.canvasGroup.alpha = 1;

        loadingText.text = SceneManager.GetActiveScene().name;

        #region Menu Transition

        if (PlayerPrefs.GetInt("MenuInTransitionDataRef") == 1)
        {
            PlayerPrefs.SetInt("SceneInTransitionDataRef", 0);
            PlayerPrefs.SetInt("SpawnPointIDRef", 0);

            LoadData();

            PlayerPrefs.SetInt("MenuInTransitionDataRef", 0);
        }

        #endregion

        #region Scene Transition

        if (PlayerPrefs.GetInt("SceneInTransitionDataRef") == 1)
        {
            transitionData = SaveSystem.LoadTransitionData(PlayerPrefs.GetString("transitionSceneNameData"));
            for (int i = 0; i < scenes.Count; i++)
            {
                SaveSystem.LoadSceneData(scenes[i]);
            }

            StartCoroutine(LoadLocalSavedData());
        }
        else
        {
            hudM.levelFadeUI.canvasGroup.alpha = 0;
            inventoryM.referencesObj.coinHUD.canvasGroup.alpha = 0;
            loadingScreenFUI.FadeTransition(0, 3, 1);
        }

        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        dialogueData = FindObjectsOfType<AIController>();
        locationCPData = FindObjectsOfType<LocationCheckPoint>();
        triggerActionData = FindObjectsOfType<TriggerAction>();
        questData = FindObjectsOfType<QuestData>();
        skillSlots = FindObjectsOfType<SkillSlot>();
        inventoryM.initWpns = FindObjectsOfType<ItemInteract>();

        if (inventoryM.isPauseMenuOn)
        {
            ControllerLayout();
        }

        if (canLoadScene)
        {
            inventoryM.PauseMenuDataCollection();
            // Save current local room data and load the destionation room and his spawn position
            if (cc.sceneTD != null)
                SaveAndLoadTransitionScene(cc.sceneTD.sceneName, cc.sceneTD.SpawnPointIDRef);
            else
                SaveAndLoadTransitionScene(cc.itemObtained.sceneName, cc.itemObtained.SpawnPointIDRef);
        }
    }

    #region Saving Data

    public void SaveGlobalData()
    {
        PlayerPrefs.SetString("GlobalSaveSceneNameDataRef", SceneManager.GetActiveScene().name);

        SaveSystem.SaveSceneData(this);
        SaveSystem.SaveGlobalData(this);
    }

    public void SaveTransitionData()
    {
        PlayerPrefs.SetString("transitionSceneNameData", SceneManager.GetActiveScene().name);

        SaveSystem.SaveSceneData(this);
        SaveSystem.SaveTransitionData(this);
    }

    #endregion

    #region Loading Data

    public void LoadData()
    {
        globalData = SaveSystem.LoadGlobalData(this);

        // Don't load anything if data does not exist.
        if (globalData == null) return;

        for (int i = 0; i < scenes.Count; i++)
        {
            SaveSystem.LoadSceneData(scenes[i]);
        }

        QuestManager.numOfMainQuestSlot = 0;
        QuestManager.numOfSideQuestSlot = 0;

        if (gameOverFUI.gameObject.activeInHierarchy)
            gameOverFUI.canvasGroup.alpha = 0;
        if (gameOverOptionFUI.gameObject.activeInHierarchy)
            gameOverOptionFUI.canvasGroup.alpha = 0;

        loadingScreenFUI.FadeTransition(1, 0, 1);
        StartCoroutine(LoadGlobalSavedData());
    }

    public void SaveAndLoadTransitionScene(string sceneName, int _SpawnPointIDRef)
    {
        // This is called when the Player while in game triggers something 
        // that prompts the current scene to load to the next specific scene 

        // Save local Data
        if (blackScreenFUI.canvasGroup.alpha == 0)
        {
            // Save prefs for current room before load
            PlayerPrefs.SetInt("SceneInTransitionDataRef", 1);
            PlayerPrefs.SetInt("SpawnPointIDRef", _SpawnPointIDRef);

            SaveTransitionData();
            cc.canMove = false;
            blackScreenFUI.gameObject.SetActive(true);
            blackScreenFUI.FadeTransition(1, 0, 0.5f);
        }

        // Load local saved data
        if (blackScreenFUI.canvasGroup.alpha == 1)
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    #region  Loading Global Data Process

    protected IEnumerator LoadGlobalSavedData()
    {
        isLoadingData = true;

        foreach (HideUI obj in FindObjectsOfType<HideUI>())
        {
            obj.enabled = false;
        }

        // Disable Navmesh in order to reposition AI
        foreach (AIController npc in dialogueData)
        {
            if (npc.GetComponent<NavMeshAgent>() != null)
                npc.GetComponent<NavMeshAgent>().enabled = false;
        }

        // Deactivate Game Over UI before loading
        if (gameOverFUI.gameObject.activeInHierarchy)
        {
            gameOverFUI.canvasGroup.alpha = 0;
            gameOverFUI.gameObject.SetActive(false);
        }

        loadingDataNameText.text = "Loading...";
        yield return new WaitForSeconds(1);

        blackScreenFUI.canvasGroup.alpha = 0;

        // Set Inventory and Menu Sections Active during initialization.
        inventoryM.InventorySectionIsActive(true);
        inventoryM.PauseMenuSectionIsActive(true);

        #region Load Player Data

        cc.animator.SetBool("Dead", false);

        hudM.curLevel = globalData.level;
        hudM.curHealth = globalData.curhealth;
        hudM.maxHealth = globalData.maxhealth;
        hudM.curEnegry = globalData.curEnegry;
        hudM.maxEnegry = globalData.maxEnegry;
        hudM.curExp = globalData.curExp;
        hudM.expRequired = globalData.reqExp;
        hudM.curSkillPoint = globalData.skillPoint;

        // Load Player Position.
        Vector3 playerPosition = new Vector3(cc.transform.position.x,
        cc.transform.position.y, cc.transform.position.z);
        playerPosition.x = globalData.playerPosition[0];
        playerPosition.y = globalData.playerPosition[1];
        playerPosition.z = globalData.playerPosition[2];
        cc.transform.position = playerPosition;

        // Load Player Rotation.
        Quaternion playerRotation = Quaternion.Euler(cc.transform.rotation.x,
        cc.transform.rotation.y, cc.transform.rotation.z);
        playerRotation.x = globalData.playerRotation[0];
        playerRotation.y = globalData.playerRotation[1];
        playerRotation.z = globalData.playerRotation[2];
        cc.transform.rotation = playerRotation;

        // ---------------- NPC ---------------- //
        for (int i = 0; i < globalData.npcData.Length; i++)
        {
            dialogueData[i].state = (AIController.State)globalData.npcData[i].state;
            dialogueData[i].dialogue.dialogueType = (Dialogue.DialogueType)globalData.npcData[i].dialogueType;

            dialogueData[i].dialogue.dialogueSegmentNum = globalData.npcData[i].dialogueSegmentNum;
            dialogueData[i].dialogueAltered = globalData.npcData[i].dialogueAlteredPrimary;

            dialogueData[i].dialogue.questPhaseReceipt = globalData.npcData[i].questPhaseReceipt;

            if (dialogueData[i].dialogueAltered)
            {
                dialogueData[i].dialogue.dialogueState = Dialogue.DialogueState.OptionA;
                dialogueData[i].gameObject.transform.position = new Vector3(globalData.npcData[i].positionX, globalData.npcData[i].positionY, globalData.npcData[i].positionZ);
                dialogueData[i].gameObject.transform.rotation = Quaternion.Euler(globalData.npcData[i].rotationX, globalData.npcData[i].rotationY, globalData.npcData[i].rotationZ);
            }
        }

        #endregion

        loadingDataNameText.text = "Initializing Player Data...";
        yield return new WaitForSeconds(0.8f);

        #region Load MiniMap Data

        miniMap.isMarkerOnMap = globalData.isMarkerOnMap;

        if (miniMap.isMarkerOnMap)
        {
            GameObject mapMarkerPrefab = Instantiate(miniMap.MapPointerPrefab) as GameObject;

            mapMarkerPrefab.GetComponent<MapPointer>().isAudioActive = false;
            mapMarkerPrefab.transform.position = new Vector3(globalData.mapMarkerPosition[0], globalData.mapMarkerPosition[1], globalData.mapMarkerPosition[2]);
            mapMarkerPrefab.GetComponent<MapPointer>().SetColor(miniMap.playerColor);
        }

        #endregion

        loadingDataNameText.text = "Loading MiniMap Data...";
        yield return new WaitForSeconds(miniMap.isMarkerOnMap ? 1 : 0);

        #region Load Quest Data

        QuestManager.numOfMainQuestSlot = 0;
        QuestManager.numOfSideQuestSlot = 0;

        ResetQuest();

        questSlots = FindObjectsOfType<QuestSlot>();
        foreach (QuestSlot slots in questSlots)
        {
            Destroy(slots.gameObject);
        }

        #region Load Main Quest Data

        int mQInt = globalData.mainQuestData.Length;

        QuestData[] tempMQ0 = new QuestData[mQInt];
        TextMeshProUGUI[] tempMQ1 = new TextMeshProUGUI[mQInt];
        TextMeshProUGUI[] tempMQ2 = new TextMeshProUGUI[mQInt];
        TextMeshProUGUI[] tempMQ3 = new TextMeshProUGUI[mQInt];
        TextMeshProUGUI[] tempMQ4 = new TextMeshProUGUI[mQInt];
        Image[] tempMQ5 = new Image[mQInt];

        questM.mainQuestSlots.slotNum = globalData.mainQuestSlot;
        questM.mainQuestSlots.questData = new QuestData[mQInt];

        for (int i = 0; i < globalData.mainQuestData.Length; i++)
        {
            // Create and set Quest Type.
            GameObject questSlot = Instantiate(questSlotPrefab) as GameObject;
            questSlot.GetComponent<QuestSlot>().questType = QuestSlot.QuestType.MainQuest;

            QuestManager.numOfMainQuestSlot++;
            questSlot.GetComponent<QuestSlot>().slotNum = QuestManager.numOfMainQuestSlot - 1;

            tempMQ0[i] = questSlot.GetComponent<QuestData>();
            tempMQ1[i] = questSlot.GetComponent<QuestSlot>().questName;
            tempMQ2[i] = questSlot.GetComponent<QuestSlot>().questLocation;
            tempMQ3[i] = questSlot.GetComponent<QuestSlot>().questDistance;
            tempMQ4[i] = questSlot.GetComponent<QuestSlot>().questLevel;
            tempMQ5[i] = questSlot.GetComponent<QuestSlot>().highLight;

            QuestData[] tempMQ00 = new QuestData[mQInt];
            TextMeshProUGUI[] tempMQ11 = new TextMeshProUGUI[mQInt];
            TextMeshProUGUI[] tempMQ22 = new TextMeshProUGUI[mQInt];
            TextMeshProUGUI[] tempMQ33 = new TextMeshProUGUI[mQInt];
            TextMeshProUGUI[] tempMQ44 = new TextMeshProUGUI[mQInt];
            Image[] tempMQ55 = new Image[mQInt];

            if (questM.mainQuestSlots.questData[0] != null)
            {
                tempMQ00[i] = questM.mainQuestSlots.questData[i];
                tempMQ11[i] = questM.mainQuestSlots.questName[i];
                tempMQ22[i] = questM.mainQuestSlots.questLocationName[i];
                tempMQ33[i] = questM.mainQuestSlots.questDistance[i];
                tempMQ44[i] = questM.mainQuestSlots.questLevel[i];
                tempMQ55[i] = questM.mainQuestSlots.highLight[i];
            }

            questM.mainQuestSlots.questData = tempMQ00;
            questM.mainQuestSlots.questName = tempMQ11;
            questM.mainQuestSlots.questLocationName = tempMQ22;
            questM.mainQuestSlots.questDistance = tempMQ33;
            questM.mainQuestSlots.questLevel = tempMQ44;
            questM.mainQuestSlots.highLight = tempMQ55;

            questM.mainQuestSlots.questData = tempMQ0;
            questM.mainQuestSlots.questName = tempMQ1;
            questM.mainQuestSlots.questLocationName = tempMQ2;
            questM.mainQuestSlots.questDistance = tempMQ3;
            questM.mainQuestSlots.questLevel = tempMQ4;
            questM.mainQuestSlots.highLight = tempMQ5;

            questM.mainQuestSlots.questData[i].questType = (QuestData.QuestType)globalData.mainQuestData[i].questType;
            questM.mainQuestSlots.questData[i].questName = globalData.mainQuestData[i].questName;
            questM.mainQuestSlots.questData[i].questLevel = globalData.mainQuestData[i].questLevel;

            questM.mainQuestSlots.questData[i].EXPReward = globalData.mainQuestData[i].questEXPReward;
            questM.mainQuestSlots.questData[i].questDescription = globalData.mainQuestData[i].questDescription;
            questM.mainQuestSlots.questData[i].curObjectivePhase = globalData.mainQuestData[i].curObjectivePhase;
            questM.mainQuestSlots.questData[i].maxObjectivePhase = globalData.mainQuestData[i].maxObjectivePhase;

            #region New Main Quest Objective Data

            questM.mainQuestSlots.questData[i].questObjectiveNum = globalData.mainQuestData[i].questObjectiveNum;

            questM.mainQuestSlots.questData[i].questObjective = new QuestData.QuestObjective[globalData.mainQuestData[i].questObective.Length];

            for (int mQSize = 0; mQSize < globalData.mainQuestData[i].questObective.Length; mQSize++)
            {
                // Create New Objective Location.
                GameObject newMainQuestPosPrefab = Instantiate(questObjectivePosPrefab) as GameObject;
                newMainQuestPosPrefab.name = "MainQuestPositionPrefab";

                newMainQuestPosPrefab.GetComponent<SetQuestObjectiveLocation>().phaseNum = mQSize;

                questM.mainQuestSlots.questData[i].questObjective[mQSize].questLocation = newMainQuestPosPrefab.transform;

                for (int ki = 0; ki < newMainQuestPosPrefab.GetComponent<SetQuestObjectiveLocation>().waypoints.Length; ki++)
                {
                    if (questM.mainQuestSlots.questData[i].questName == newMainQuestPosPrefab.GetComponent<SetQuestObjectiveLocation>().waypoints[ki].questSequenceNameReceipt &&
                        mQSize == newMainQuestPosPrefab.GetComponent<SetQuestObjectiveLocation>().waypoints[ki].questPhaseReceipt)
                    {
                        newMainQuestPosPrefab.GetComponent<SetQuestObjectiveLocation>().waypoints[ki].GetComponent<MiniMapItem>().Size = 1;
                        newMainQuestPosPrefab.GetComponent<SetQuestObjectiveLocation>().waypoints[ki].GetComponent<MiniMapItem>().HideCircleArea();
                        questM.mainQuestSlots.questData[i].questObjective[mQSize].questLocation = newMainQuestPosPrefab.GetComponent<SetQuestObjectiveLocation>().waypoints[ki].transform;
                    }
                }

                questM.mainQuestSlots.questData[i].questObjective[mQSize].questLocation.position =
                new Vector3(globalData.mainQuestData[i].questObective[mQSize].questLocationX,
                globalData.mainQuestData[i].questObective[mQSize].questLocationY,
                globalData.mainQuestData[i].questObective[mQSize].questLocationZ);

                questM.mainQuestSlots.questData[i].questObjective[mQSize].questLocationName = globalData.mainQuestData[i].questObective[mQSize].questLocationName;
                questM.mainQuestSlots.questData[i].questObjective[mQSize].questObjectiveSubject = globalData.mainQuestData[i].questObective[mQSize].questObjectiveSubject;
                questM.mainQuestSlots.questData[i].questObjective[mQSize].questHint = globalData.mainQuestData[i].questObective[mQSize].questHint;

                questM.mainQuestSlots.questData[i].questObjective[mQSize].curCollectableAmount = globalData.mainQuestData[i].questObective[mQSize].curCollectableAmount;
                questM.mainQuestSlots.questData[i].questObjective[mQSize].maxCollectableAmount = globalData.mainQuestData[i].questObective[mQSize].maxCollectableAmount;
            }

            #endregion

            questM.mainQuestSlots.questData[i].inProgress = globalData.mainQuestData[i].questInProgress;
            if (questM.mainQuestSlots.questData[i].inProgress) questM.MainQuestSlotSelect(globalData.mainQuestSlot);
            questM.mainQuestSlots.questData[i].isComplete = globalData.mainQuestData[i].questIsComplete;

            questSlot.GetComponent<QuestSlot>().questName.text = questSlot.GetComponent<QuestData>().questName;
            questSlot.GetComponent<QuestSlot>().questLocation.text = questSlot.GetComponent<QuestData>().questObjective[questSlot.GetComponent<QuestData>().questObjectiveNum].questLocationName;
            questSlot.GetComponent<QuestSlot>().questLevel.text = questSlot.GetComponent<QuestData>().questLevel.ToString();
        }

        #endregion

        #region Load Side Quest Data

        int sQInt = globalData.sideQuestData.Length;

        QuestData[] tempSQ0 = new QuestData[sQInt];
        TextMeshProUGUI[] tempSQ1 = new TextMeshProUGUI[sQInt];
        TextMeshProUGUI[] tempSQ2 = new TextMeshProUGUI[sQInt];
        TextMeshProUGUI[] tempSQ3 = new TextMeshProUGUI[sQInt];
        TextMeshProUGUI[] tempSQ4 = new TextMeshProUGUI[sQInt];
        Image[] tempSQ5 = new Image[sQInt];

        questM.sideQuestSlots.slotNum = globalData.sideQuestSlot;
        questM.sideQuestSlots.questData = new QuestData[sQInt];

        for (int i = 0; i < globalData.sideQuestData.Length; i++)
        {
            // Create and set Quest Type.
            GameObject questSlot = Instantiate(questSlotPrefab) as GameObject;
            questSlot.GetComponent<QuestSlot>().questType = QuestSlot.QuestType.SideQuest;

            QuestManager.numOfSideQuestSlot++;
            questSlot.GetComponent<QuestSlot>().slotNum = QuestManager.numOfSideQuestSlot - 1;

            tempSQ0[i] = questSlot.GetComponent<QuestData>();
            tempSQ1[i] = questSlot.GetComponent<QuestSlot>().questName;
            tempSQ2[i] = questSlot.GetComponent<QuestSlot>().questLocation;
            tempSQ3[i] = questSlot.GetComponent<QuestSlot>().questDistance;
            tempSQ4[i] = questSlot.GetComponent<QuestSlot>().questLevel;
            tempSQ5[i] = questSlot.GetComponent<QuestSlot>().highLight;

            QuestData[] tempSQ00 = new QuestData[sQInt];
            TextMeshProUGUI[] tempSQ11 = new TextMeshProUGUI[sQInt];
            TextMeshProUGUI[] tempSQ22 = new TextMeshProUGUI[sQInt];
            TextMeshProUGUI[] tempSQ33 = new TextMeshProUGUI[sQInt];
            TextMeshProUGUI[] tempSQ44 = new TextMeshProUGUI[sQInt];
            Image[] tempSQ55 = new Image[sQInt];

            if (questM.sideQuestSlots.questData[0] != null)
            {
                tempSQ00[i] = questM.sideQuestSlots.questData[i];
                tempSQ11[i] = questM.sideQuestSlots.questName[i];
                tempSQ22[i] = questM.sideQuestSlots.questLocationName[i];
                tempSQ33[i] = questM.sideQuestSlots.questDistance[i];
                tempSQ44[i] = questM.sideQuestSlots.questLevel[i];
                tempSQ55[i] = questM.sideQuestSlots.highLight[i];
            }

            questM.sideQuestSlots.questData = tempSQ00;
            questM.sideQuestSlots.questName = tempSQ11;
            questM.sideQuestSlots.questLocationName = tempSQ22;
            questM.sideQuestSlots.questDistance = tempSQ33;
            questM.sideQuestSlots.questLevel = tempSQ44;
            questM.sideQuestSlots.highLight = tempSQ55;

            questM.sideQuestSlots.questData = tempSQ0;
            questM.sideQuestSlots.questName = tempSQ1;
            questM.sideQuestSlots.questLocationName = tempSQ2;
            questM.sideQuestSlots.questDistance = tempSQ3;
            questM.sideQuestSlots.questLevel = tempSQ4;
            questM.sideQuestSlots.highLight = tempSQ5;

            questM.sideQuestSlots.questData[i].questType = (QuestData.QuestType)globalData.sideQuestData[i].questType;
            questM.sideQuestSlots.questData[i].questName = globalData.sideQuestData[i].questName;
            questM.sideQuestSlots.questData[i].questLevel = globalData.sideQuestData[i].questLevel;

            questM.sideQuestSlots.questData[i].EXPReward = globalData.sideQuestData[i].questEXPReward;
            questM.sideQuestSlots.questData[i].questDescription = globalData.sideQuestData[i].questDescription;
            questM.sideQuestSlots.questData[i].curObjectivePhase = globalData.sideQuestData[i].curObjectivePhase;
            questM.sideQuestSlots.questData[i].maxObjectivePhase = globalData.sideQuestData[i].maxObjectivePhase;


            #region New Side Quest Objective Data

            questM.sideQuestSlots.questData[i].questObjectiveNum = globalData.sideQuestData[i].questObjectiveNum;

            questM.sideQuestSlots.questData[i].questObjective = new QuestData.QuestObjective[globalData.sideQuestData[i].questObective.Length];

            for (int sQSize = 0; sQSize < globalData.sideQuestData[i].questObective.Length; sQSize++)
            {
                // Create New Objective Location.
                GameObject newSideQuestPosPrefab = Instantiate(questObjectivePosPrefab) as GameObject;
                newSideQuestPosPrefab.name = "SideQuestPositionPrefab";

                newSideQuestPosPrefab.GetComponent<SetQuestObjectiveLocation>().phaseNum = sQSize;

                questM.sideQuestSlots.questData[i].questObjective[sQSize].questLocation = newSideQuestPosPrefab.transform;

                for (int ki = 0; ki < newSideQuestPosPrefab.GetComponent<SetQuestObjectiveLocation>().waypoints.Length; ki++)
                {
                    if (questM.sideQuestSlots.questData[i].questName == newSideQuestPosPrefab.GetComponent<SetQuestObjectiveLocation>().waypoints[ki].questSequenceNameReceipt &&
                        sQSize == newSideQuestPosPrefab.GetComponent<SetQuestObjectiveLocation>().waypoints[ki].questPhaseReceipt)
                    {
                        newSideQuestPosPrefab.GetComponent<SetQuestObjectiveLocation>().waypoints[ki].GetComponent<MiniMapItem>().Size = 1;
                        newSideQuestPosPrefab.GetComponent<SetQuestObjectiveLocation>().waypoints[ki].GetComponent<MiniMapItem>().HideCircleArea();
                        questM.sideQuestSlots.questData[i].questObjective[sQSize].questLocation = newSideQuestPosPrefab.GetComponent<SetQuestObjectiveLocation>().waypoints[ki].transform;
                    }
                }

                questM.sideQuestSlots.questData[i].questObjective[sQSize].questLocation.position =
                new Vector3(globalData.sideQuestData[i].questObective[sQSize].questLocationX,
                globalData.sideQuestData[i].questObective[sQSize].questLocationY,
                globalData.sideQuestData[i].questObective[sQSize].questLocationZ);

                questM.sideQuestSlots.questData[i].questObjective[sQSize].questLocationName = globalData.sideQuestData[i].questObective[sQSize].questLocationName;
                questM.sideQuestSlots.questData[i].questObjective[sQSize].questObjectiveSubject = globalData.sideQuestData[i].questObective[sQSize].questObjectiveSubject;
                questM.sideQuestSlots.questData[i].questObjective[sQSize].questHint = globalData.sideQuestData[i].questObective[sQSize].questHint;

                questM.sideQuestSlots.questData[i].questObjective[sQSize].curCollectableAmount = globalData.sideQuestData[i].questObective[sQSize].curCollectableAmount;
                questM.sideQuestSlots.questData[i].questObjective[sQSize].maxCollectableAmount = globalData.sideQuestData[i].questObective[sQSize].maxCollectableAmount;
            }

            #endregion

            questM.sideQuestSlots.questData[i].inProgress = globalData.sideQuestData[i].questInProgress;
            if (questM.sideQuestSlots.questData[i].inProgress) questM.SideQuestSlotSelect(globalData.sideQuestSlot);
            questM.sideQuestSlots.questData[i].isComplete = globalData.sideQuestData[i].questIsComplete;

            questSlot.GetComponent<QuestSlot>().questName.text = questSlot.GetComponent<QuestData>().questName;
            questSlot.GetComponent<QuestSlot>().questLocation.text = questSlot.GetComponent<QuestData>().questObjective[questSlot.GetComponent<QuestData>().questObjectiveNum].questLocationName;
            questSlot.GetComponent<QuestSlot>().questLevel.text = questSlot.GetComponent<QuestData>().questLevel.ToString();
        }

        #endregion

        #endregion

        loadingDataNameText.text = globalData.mainQuestData.Length > 0 || globalData.sideQuestData.Length > 0 ? "Loading Quest data..." : "";
        yield return new WaitForSeconds(globalData.mainQuestData.Length > 0 ? 0.4f : 0);
        yield return new WaitForSeconds(globalData.sideQuestData.Length > 0 ? 0.4f : 0);

        #region Load Inventory Data

        // Remove current slots.
        inventorySlot = FindObjectsOfType<InventorySlot>();
        foreach (InventorySlot items in inventorySlot)
        {
            Destroy(items.gameObject);
        }

        ResetInventory();

        inventoryM.unityCoins = globalData.unityCoins;

        inventoryM.weaponInv.weaponSlotActive = false;
        inventoryM.bowAndArrowInv.bowAndArrowSlotActive = false;
        inventoryM.shieldInv.shieldSlotActive = false;
        inventoryM.armorInv.armorSlotActive = false;
        inventoryM.materialInv.materialSlotActive = false;
        inventoryM.healingInv.healingSlotActive = false;
        inventoryM.keyInv.keySlotActive = false;

        inventoryM.weaponSlots = globalData.wpnSlots;
        inventoryM.bowAndArrowSlots = globalData.bowSlots;
        inventoryM.shieldSlots = globalData.shdSlots;
        inventoryM.armorSlots = globalData.armSlots;
        inventoryM.materialSlots = globalData.matSlots;
        inventoryM.healingSlots = globalData.healSlots;
        inventoryM.keySlots = globalData.keySlots;

        inventoryM.SetWeaponSlot(ref globalData.wpnSlots);
        inventoryM.SetBowAndArrowSlot(ref globalData.bowSlots);
        inventoryM.SetShieldSlot(ref globalData.shdSlots);
        inventoryM.SetArmorSlot(ref globalData.armSlots);
        inventoryM.SetMaterialSlot(ref globalData.matSlots);
        inventoryM.SetHealingSlot(ref globalData.healSlots);
        inventoryM.SetKeySlot(ref globalData.keySlots);

        inventoryM.inventoryS = (InventoryManager.InventorySection)globalData.inventorySection;

        inventoryM.preSwordEquipped = globalData.preSwordEquipped;
        inventoryM.pre2HSwordEquipped = globalData.pre2HSwordEquipped;
        inventoryM.preShieldEquipped = globalData.preshieldEquipped;
        inventoryM.preSpearEquipped = globalData.preSpearEquipped;
        inventoryM.preStaffEquipped = globalData.preStaffEquipped;
        inventoryM.preBowEquipped = globalData.preBowEquipped;

        loadingDataNameText.text = "Initializing Inventory data...";
        yield return new WaitForSeconds(0.8f);

        #region Load Weapon Inventory Data

        inventoryM.weaponInv.slotNum = globalData.wpnSlotNum;
        inventoryM.weaponInv.slotWeaponEquipped = globalData.wpnSlotEquipped;

        inventoryM.weaponInv.itemData = new ItemData[globalData.wpnData.Length];

        for (int i = 0; i < globalData.wpnData.Length; i++)
        {
            GameObject emptyitemData = Instantiate(itemDataPrefab) as GameObject;
            inventoryM.weaponInv.itemData[i] = emptyitemData.GetComponent<ItemData>();

            inventoryM.weaponInv.itemData[i].itemType = (ItemData.ItemType)globalData.wpnData[i].itemType;
            inventoryM.weaponInv.itemData[i].itemName = globalData.wpnData[i].itemName;
            inventoryM.weaponInv.itemData[i].itemDescription = globalData.wpnData[i].itemDescription;

            inventoryM.weaponInv.itemData[i].wpnAtk.value = globalData.wpnData[i].wpnAtkStat.value;
            inventoryM.weaponInv.itemData[i].wpnAtk.rank1 = globalData.wpnData[i].wpnAtkStat.rank1;
            inventoryM.weaponInv.itemData[i].wpnAtk.rank2 = globalData.wpnData[i].wpnAtkStat.rank2;
            inventoryM.weaponInv.itemData[i].wpnAtk.rank3 = globalData.wpnData[i].wpnAtkStat.rank3;

            inventoryM.weaponInv.itemData[i].wpnSpd.value = globalData.wpnData[i].wpnSpdStat.value;
            inventoryM.weaponInv.itemData[i].wpnSpd.rank1 = globalData.wpnData[i].wpnSpdStat.rank1;
            inventoryM.weaponInv.itemData[i].wpnSpd.rank2 = globalData.wpnData[i].wpnSpdStat.rank2;
            inventoryM.weaponInv.itemData[i].wpnSpd.rank3 = globalData.wpnData[i].wpnSpdStat.rank3;

            inventoryM.weaponInv.itemData[i].wpnStun.value = globalData.wpnData[i].wpnStunStat.value;
            inventoryM.weaponInv.itemData[i].wpnStun.rank1 = globalData.wpnData[i].wpnStunStat.rank1;
            inventoryM.weaponInv.itemData[i].wpnStun.rank2 = globalData.wpnData[i].wpnStunStat.rank2;
            inventoryM.weaponInv.itemData[i].wpnStun.rank3 = globalData.wpnData[i].wpnStunStat.rank3;

            inventoryM.weaponInv.itemData[i].wpnCritHit.value = globalData.wpnData[i].wpnCritHitStat.value;
            inventoryM.weaponInv.itemData[i].wpnCritHit.rank1 = globalData.wpnData[i].wpnCritHitStat.rank1;
            inventoryM.weaponInv.itemData[i].wpnCritHit.rank2 = globalData.wpnData[i].wpnCritHitStat.rank2;
            inventoryM.weaponInv.itemData[i].wpnCritHit.rank3 = globalData.wpnData[i].wpnCritHitStat.rank3;

            inventoryM.weaponInv.itemData[i].wpnDura.value = globalData.wpnData[i].wpnDuraStat.value;
            inventoryM.weaponInv.itemData[i].wpnDura.rank1 = globalData.wpnData[i].wpnDuraStat.rank1;
            inventoryM.weaponInv.itemData[i].wpnDura.rank2 = globalData.wpnData[i].wpnDuraStat.rank2;
            inventoryM.weaponInv.itemData[i].wpnDura.rank3 = globalData.wpnData[i].wpnDuraStat.rank3;

            inventoryM.weaponInv.itemData[i].throwDist.throwRange = (ItemData.ThrowRange)globalData.wpnData[i].throwDistance.throwRange;

            inventoryM.weaponInv.itemData[i].wpnSpecialStat1 = (ItemData.WeaponSpecialStat1)globalData.wpnData[i].wpnSpecialStat1;
            inventoryM.weaponInv.itemData[i].wpnSpecialStat2 = (ItemData.WeaponSpecialStat2)globalData.wpnData[i].wpnSpecialStat2;

            inventoryM.weaponInv.itemData[i].wpnWeight = globalData.wpnData[i].weight;
            inventoryM.weaponInv.itemData[i].weaponArmsID = globalData.wpnData[i].weaponArmsID;

            inventoryM.weaponInv.itemData[i].rankNum = globalData.wpnData[i].rankNum;

            inventoryM.weaponInv.itemData[i].upgradeMaterial = new ItemData.Material[globalData.wpnData[i].upgradeMaterial.Length];

            for (int matSize = 0; matSize < globalData.wpnData[i].upgradeMaterial.Length; matSize++)
            {
                inventoryM.weaponInv.itemData[i].upgradeMaterial[matSize].matName = globalData.wpnData[i].upgradeMaterial[matSize].matName;
                inventoryM.weaponInv.itemData[i].upgradeMaterial[matSize].matRequired = globalData.wpnData[i].upgradeMaterial[matSize].matRequired;
            }

            inventoryM.weaponInv.itemData[i].buyPrice = globalData.wpnData[i].buyPrice;
            inventoryM.weaponInv.itemData[i].sellPrice = globalData.wpnData[i].sellPrice;
            inventoryM.weaponInv.itemData[i].repairPrice = globalData.wpnData[i].repairPrice;
            inventoryM.weaponInv.itemData[i].upgradePrice = globalData.wpnData[i].upgradePrice;
            inventoryM.weaponInv.itemData[i].specialPrice = globalData.wpnData[i].specialPrice;
            inventoryM.weaponInv.itemData[i].inStockQuantity = globalData.wpnData[i].inStockQuantity;

            inventoryM.weaponInv.counter[i] = globalData.wpnData[i].quantity;
            inventoryM.weaponInv.itemData[i].stackable = globalData.wpnData[i].stackable;
            inventoryM.weaponInv.itemData[i].inInventory = globalData.wpnData[i].inInventory;
            inventoryM.weaponInv.itemData[i].equipped = globalData.wpnData[i].equipped;

            inventoryM.weaponInv.itemData[i].broken = globalData.wpnData[i].broken;
            inventoryM.weaponInv.itemData[i].rankTag = globalData.wpnData[i].rankTag;
            inventoryM.weaponInv.itemData[i].originItemName = globalData.wpnData[i].originItemName;
            inventoryM.weaponInv.itemData[i].numOfEngraving = globalData.wpnData[i].numOfEngraving;

            inventoryM.weaponInv.itemData[i].maxWpnAtk = globalData.wpnData[i].maxWpnAtk;
            inventoryM.weaponInv.itemData[i].maxWpnSpd = globalData.wpnData[i].maxWpnSpd;
            inventoryM.weaponInv.itemData[i].maxWpnStun = globalData.wpnData[i].maxWpnStun;
            inventoryM.weaponInv.itemData[i].maxWpnCritHit = globalData.wpnData[i].maxWpnCritHit;
            inventoryM.weaponInv.itemData[i].maxWpnDura = globalData.wpnData[i].maxWpnDura;

            inventoryM.weaponInv.image[i].type = Image.Type.Simple;
            inventoryM.weaponInv.image[i].preserveAspect = true;
            Navigation automatic = new Navigation { mode = Navigation.Mode.Automatic };
            inventoryM.weaponInv.image[i].GetComponent<Button>().navigation = automatic;
            inventoryM.weaponInv.image[i].enabled = true;

            #region Set Inventory BG Color

            switch (globalData.wpnData[i].rankNum)
            {
                case 0:
                    inventoryM.weaponInv.highLight[i].color = new Color(0, 0, 0, 80);
                    inventoryM.weaponInv.highLight[i].GetComponent<UIGradient>().enabled = false;
                    break;
                case 1:
                    inventoryM.weaponInv.highLight[i].GetComponent<UIGradient>().enabled = true;
                    inventoryM.weaponInv.highLight[i].GetComponent<UIGradient>().LinearColor1 = inventoryM.rank1Color1;
                    inventoryM.weaponInv.highLight[i].GetComponent<UIGradient>().LinearColor2 = inventoryM.rank1Color2;
                    break;
                case 2:
                    inventoryM.weaponInv.highLight[i].GetComponent<UIGradient>().enabled = true;
                    inventoryM.weaponInv.highLight[i].GetComponent<UIGradient>().LinearColor1 = inventoryM.rank2Color1;
                    inventoryM.weaponInv.highLight[i].GetComponent<UIGradient>().LinearColor2 = inventoryM.rank2Color2;
                    break;
                case 3:
                    inventoryM.weaponInv.highLight[i].GetComponent<UIGradient>().enabled = true;
                    inventoryM.weaponInv.highLight[i].GetComponent<UIGradient>().LinearColor1 = inventoryM.rank3Color1;
                    inventoryM.weaponInv.highLight[i].GetComponent<UIGradient>().LinearColor2 = inventoryM.rank3Color2;
                    break;
            }

            if (globalData.wpnData[i].broken)
            {
                inventoryM.weaponInv.highLight[i].GetComponent<UIGradient>().enabled = true;
                inventoryM.weaponInv.highLight[i].GetComponent<UIGradient>().LinearColor1 = inventoryM.brokenColor1;
                inventoryM.weaponInv.highLight[i].GetComponent<UIGradient>().LinearColor2 = inventoryM.brokenColor2;
            }

            #endregion

            if (inventoryM.weaponInv.itemData[i].equipped)
            {
                inventoryM.weaponInv.outLineBorder[i].enabled = true;
                inventoryM.weaponInv.outLineBorder[i].color = inventoryM.outLineSelected;
            }

            if (!inventoryM.weaponInv.itemData[i].stackable)
            {
                inventoryM.weaponInv.counter[i] = 1;
                inventoryM.weaponInv.image[i].enabled = true;
            }
            else
            {
                inventoryM.weaponInv.image[i].enabled = true;
                inventoryM.weaponInv.quantity[i].enabled = true;
                inventoryM.weaponInv.counter[i] = globalData.wpnData[i].quantity;
                inventoryM.weaponInv.quantity[i].text = "x" + inventoryM.weaponInv.counter[i].ToString();
            }
            SetLoadedSprite(ref inventoryM.weaponInv.itemData[i], ref inventoryM.weaponInv.image[i]);
        }

        #endregion

        loadingDataNameText.text = "Loading Weapon slots data...";
        yield return new WaitForSeconds(0.8f);

        #region Load Bow And Arrow Inventory Data

        inventoryM.bowAndArrowInv.slotNum = globalData.bowSlotNum;
        inventoryM.bowAndArrowInv.slotBowEquipped = globalData.bowSlotEquipped;
        inventoryM.bowAndArrowInv.slotArrowEquipped = globalData.arrowSlotEquipped;

        inventoryM.bowAndArrowInv.itemData = new ItemData[globalData.bowData.Length];
        for (int i = 0; i < globalData.bowData.Length; i++)
        {
            GameObject emptyitemData = Instantiate(itemDataPrefab) as GameObject;
            inventoryM.bowAndArrowInv.itemData[i] = emptyitemData.GetComponent<ItemData>();

            inventoryM.bowAndArrowInv.itemData[i].itemType = (ItemData.ItemType)globalData.bowData[i].itemType;
            inventoryM.bowAndArrowInv.itemData[i].itemName = globalData.bowData[i].itemName;
            inventoryM.bowAndArrowInv.itemData[i].itemDescription = globalData.bowData[i].itemDescription;

            inventoryM.bowAndArrowInv.itemData[i].bowAtk.value = globalData.bowData[i].bowAtkStat.value;
            inventoryM.bowAndArrowInv.itemData[i].bowAtk.rank1 = globalData.bowData[i].bowAtkStat.rank1;
            inventoryM.bowAndArrowInv.itemData[i].bowAtk.rank2 = globalData.bowData[i].bowAtkStat.rank2;
            inventoryM.bowAndArrowInv.itemData[i].bowAtk.rank3 = globalData.bowData[i].bowAtkStat.rank3;

            inventoryM.bowAndArrowInv.itemData[i].bowSpd.value = globalData.bowData[i].bowSpdStat.value;
            inventoryM.bowAndArrowInv.itemData[i].bowSpd.rank1 = globalData.bowData[i].bowSpdStat.rank1;
            inventoryM.bowAndArrowInv.itemData[i].bowSpd.rank2 = globalData.bowData[i].bowSpdStat.rank2;
            inventoryM.bowAndArrowInv.itemData[i].bowSpd.rank3 = globalData.bowData[i].bowSpdStat.rank3;

            inventoryM.bowAndArrowInv.itemData[i].bowStun.value = globalData.bowData[i].bowStunStat.value;
            inventoryM.bowAndArrowInv.itemData[i].bowStun.rank1 = globalData.bowData[i].bowStunStat.rank1;
            inventoryM.bowAndArrowInv.itemData[i].bowStun.rank2 = globalData.bowData[i].bowStunStat.rank2;
            inventoryM.bowAndArrowInv.itemData[i].bowStun.rank3 = globalData.bowData[i].bowStunStat.rank3;

            inventoryM.bowAndArrowInv.itemData[i].bowCritHit.value = globalData.bowData[i].bowCritHitStat.value;
            inventoryM.bowAndArrowInv.itemData[i].bowCritHit.rank1 = globalData.bowData[i].bowCritHitStat.rank1;
            inventoryM.bowAndArrowInv.itemData[i].bowCritHit.rank2 = globalData.bowData[i].bowCritHitStat.rank2;
            inventoryM.bowAndArrowInv.itemData[i].bowCritHit.rank3 = globalData.bowData[i].bowCritHitStat.rank3;

            inventoryM.bowAndArrowInv.itemData[i].bowHdShot.value = globalData.bowData[i].bowHdShotStat.value;
            inventoryM.bowAndArrowInv.itemData[i].bowHdShot.rank1 = globalData.bowData[i].bowHdShotStat.rank1;
            inventoryM.bowAndArrowInv.itemData[i].bowHdShot.rank2 = globalData.bowData[i].bowHdShotStat.rank2;
            inventoryM.bowAndArrowInv.itemData[i].bowHdShot.rank3 = globalData.bowData[i].bowHdShotStat.rank3;

            inventoryM.bowAndArrowInv.itemData[i].bowDura.value = globalData.bowData[i].bowDuraStat.value;
            inventoryM.bowAndArrowInv.itemData[i].bowDura.rank1 = globalData.bowData[i].bowDuraStat.rank1;
            inventoryM.bowAndArrowInv.itemData[i].bowDura.rank2 = globalData.bowData[i].bowDuraStat.rank2;
            inventoryM.bowAndArrowInv.itemData[i].bowDura.rank3 = globalData.bowData[i].bowDuraStat.rank3;

            inventoryM.bowAndArrowInv.itemData[i].bowRange.bowRangeType = (ItemData.BowRangeType)globalData.bowData[i].bowRange.bowRangeType;

            inventoryM.bowAndArrowInv.itemData[i].bowSpecialStat1 = (ItemData.BowSpecialStat1)globalData.bowData[i].bowSpecialStat1;
            inventoryM.bowAndArrowInv.itemData[i].bowSpecialStat2 = (ItemData.BowSpecialStat2)globalData.bowData[i].bowSpecialStat2;

            inventoryM.bowAndArrowInv.itemData[i].bowWeight = globalData.bowData[i].weight;
            inventoryM.bowAndArrowInv.itemData[i].weaponArmsID = globalData.bowData[i].weaponArmsID;

            inventoryM.bowAndArrowInv.itemData[i].rankNum = globalData.bowData[i].rankNum;

            inventoryM.bowAndArrowInv.itemData[i].upgradeMaterial = new ItemData.Material[globalData.bowData[i].upgradeMaterial.Length];
            for (int matSize = 0; matSize < globalData.bowData[i].upgradeMaterial.Length; matSize++)
            {
                inventoryM.bowAndArrowInv.itemData[i].upgradeMaterial[matSize].matName = globalData.bowData[i].upgradeMaterial[matSize].matName;
                inventoryM.bowAndArrowInv.itemData[i].upgradeMaterial[matSize].matRequired = globalData.bowData[i].upgradeMaterial[matSize].matRequired;
            }

            inventoryM.bowAndArrowInv.itemData[i].buyPrice = globalData.bowData[i].buyPrice;
            inventoryM.bowAndArrowInv.itemData[i].sellPrice = globalData.bowData[i].sellPrice;
            inventoryM.bowAndArrowInv.itemData[i].repairPrice = globalData.bowData[i].repairPrice;
            inventoryM.bowAndArrowInv.itemData[i].upgradePrice = globalData.bowData[i].upgradePrice;
            inventoryM.bowAndArrowInv.itemData[i].specialPrice = globalData.bowData[i].specialPrice;
            inventoryM.bowAndArrowInv.itemData[i].inStockQuantity = globalData.bowData[i].inStockQuantity;

            inventoryM.bowAndArrowInv.counter[i] = globalData.bowData[i].quantity;
            inventoryM.bowAndArrowInv.itemData[i].stackable = globalData.bowData[i].stackable;
            inventoryM.bowAndArrowInv.itemData[i].inInventory = globalData.bowData[i].inInventory;
            inventoryM.bowAndArrowInv.itemData[i].equipped = globalData.bowData[i].equipped;

            inventoryM.bowAndArrowInv.itemData[i].broken = globalData.bowData[i].broken;
            inventoryM.bowAndArrowInv.itemData[i].rankTag = globalData.bowData[i].rankTag;
            inventoryM.bowAndArrowInv.itemData[i].originItemName = globalData.bowData[i].originItemName;
            inventoryM.bowAndArrowInv.itemData[i].numOfEngraving = globalData.bowData[i].numOfEngraving;

            inventoryM.bowAndArrowInv.itemData[i].maxBowAtk = globalData.bowData[i].maxBowAtk;
            inventoryM.bowAndArrowInv.itemData[i].maxBowSpd = globalData.bowData[i].maxBowSpd;
            inventoryM.bowAndArrowInv.itemData[i].maxBowStun = globalData.bowData[i].maxBowStun;
            inventoryM.bowAndArrowInv.itemData[i].maxBowCritHit = globalData.bowData[i].maxBowCritHit;
            inventoryM.bowAndArrowInv.itemData[i].maxBowHdShot = globalData.bowData[i].maxBowHdShot;
            inventoryM.bowAndArrowInv.itemData[i].maxBowDura = globalData.bowData[i].maxBowDura;

            inventoryM.bowAndArrowInv.image[i].type = Image.Type.Simple;
            inventoryM.bowAndArrowInv.image[i].preserveAspect = true;
            Navigation automatic = new Navigation { mode = Navigation.Mode.Automatic };
            inventoryM.bowAndArrowInv.image[i].GetComponent<Button>().navigation = automatic;
            inventoryM.bowAndArrowInv.image[i].enabled = true;

            #region Set Inventory BG Color

            switch (globalData.bowData[i].rankNum)
            {
                case 0:
                    inventoryM.bowAndArrowInv.highLight[i].GetComponent<UIGradient>().enabled = false;
                    inventoryM.bowAndArrowInv.highLight[i].color = new Color(0, 0, 0, 80);
                    break;
                case 1:
                    inventoryM.bowAndArrowInv.highLight[i].GetComponent<UIGradient>().enabled = true;
                    inventoryM.bowAndArrowInv.highLight[i].GetComponent<UIGradient>().LinearColor1 = inventoryM.rank1Color1;
                    inventoryM.bowAndArrowInv.highLight[i].GetComponent<UIGradient>().LinearColor2 = inventoryM.rank1Color2;
                    break;
                case 2:
                    inventoryM.bowAndArrowInv.highLight[i].GetComponent<UIGradient>().enabled = true;
                    inventoryM.bowAndArrowInv.highLight[i].GetComponent<UIGradient>().LinearColor1 = inventoryM.rank2Color1;
                    inventoryM.bowAndArrowInv.highLight[i].GetComponent<UIGradient>().LinearColor2 = inventoryM.rank2Color2;
                    break;
                case 3:
                    inventoryM.bowAndArrowInv.highLight[i].GetComponent<UIGradient>().enabled = true;
                    inventoryM.bowAndArrowInv.highLight[i].GetComponent<UIGradient>().LinearColor1 = inventoryM.rank3Color1;
                    inventoryM.bowAndArrowInv.highLight[i].GetComponent<UIGradient>().LinearColor2 = inventoryM.rank3Color2;
                    break;
            }

            if (globalData.bowData[i].broken)
            {
                inventoryM.bowAndArrowInv.highLight[i].GetComponent<UIGradient>().enabled = true;
                inventoryM.bowAndArrowInv.highLight[i].GetComponent<UIGradient>().LinearColor1 = inventoryM.brokenColor1;
                inventoryM.bowAndArrowInv.highLight[i].GetComponent<UIGradient>().LinearColor2 = inventoryM.brokenColor2;
            }

            #endregion

            if (inventoryM.bowAndArrowInv.itemData[i].equipped)
            {
                inventoryM.bowAndArrowInv.outLineBorder[i].enabled = true;
                inventoryM.bowAndArrowInv.outLineBorder[i].color = inventoryM.outLineSelected;
            }

            if (!inventoryM.bowAndArrowInv.itemData[i].stackable)
            {
                inventoryM.bowAndArrowInv.counter[i] = 1;
                inventoryM.bowAndArrowInv.image[i].enabled = true;
            }
            else
            {
                inventoryM.bowAndArrowInv.image[i].enabled = true;
                inventoryM.bowAndArrowInv.quantity[i].enabled = true;
                inventoryM.bowAndArrowInv.counter[i] = globalData.bowData[i].quantity;
                inventoryM.bowAndArrowInv.quantity[i].text = "x" + inventoryM.bowAndArrowInv.counter[i].ToString();
            }
            SetLoadedSprite(ref inventoryM.bowAndArrowInv.itemData[i], ref inventoryM.bowAndArrowInv.image[i]);
        }

        #endregion

        loadingDataNameText.text = "Loading Bow and Arrow slots data...";
        yield return new WaitForSeconds(0.8f);

        #region Load Shield Inventory Data

        inventoryM.shieldInv.slotNum = globalData.shieldSlotNum;
        inventoryM.shieldInv.slotShieldEquipped = globalData.shieldSlotEquipped;

        inventoryM.shieldInv.itemData = new ItemData[globalData.shdData.Length];
        for (int i = 0; i < globalData.shdData.Length; i++)
        {
            GameObject emptyitemData = Instantiate(itemDataPrefab) as GameObject;
            inventoryM.shieldInv.itemData[i] = emptyitemData.GetComponent<ItemData>();

            inventoryM.shieldInv.itemData[i].itemType = (ItemData.ItemType)globalData.shdData[i].itemType;
            inventoryM.shieldInv.itemData[i].itemName = globalData.shdData[i].itemName;
            inventoryM.shieldInv.itemData[i].itemDescription = globalData.shdData[i].itemDescription;

            inventoryM.shieldInv.itemData[i].shdAtk.value = globalData.shdData[i].shdAtkStat.value;
            inventoryM.shieldInv.itemData[i].shdAtk.rank1 = globalData.shdData[i].shdAtkStat.rank1;
            inventoryM.shieldInv.itemData[i].shdAtk.rank2 = globalData.shdData[i].shdAtkStat.rank2;
            inventoryM.shieldInv.itemData[i].shdAtk.rank3 = globalData.shdData[i].shdAtkStat.rank3;

            inventoryM.shieldInv.itemData[i].shdSpd.value = globalData.shdData[i].shdSpdStat.value;
            inventoryM.shieldInv.itemData[i].shdSpd.rank1 = globalData.shdData[i].shdSpdStat.rank1;
            inventoryM.shieldInv.itemData[i].shdSpd.rank2 = globalData.shdData[i].shdSpdStat.rank2;
            inventoryM.shieldInv.itemData[i].shdSpd.rank3 = globalData.shdData[i].shdSpdStat.rank3;

            inventoryM.shieldInv.itemData[i].shdStun.value = globalData.shdData[i].shdStunStat.value;
            inventoryM.shieldInv.itemData[i].shdStun.rank1 = globalData.shdData[i].shdStunStat.rank1;
            inventoryM.shieldInv.itemData[i].shdStun.rank2 = globalData.shdData[i].shdStunStat.rank2;
            inventoryM.shieldInv.itemData[i].shdStun.rank3 = globalData.shdData[i].shdStunStat.rank3;

            inventoryM.shieldInv.itemData[i].shdCritHit.value = globalData.shdData[i].shdCritHitStat.value;
            inventoryM.shieldInv.itemData[i].shdCritHit.rank1 = globalData.shdData[i].shdCritHitStat.rank1;
            inventoryM.shieldInv.itemData[i].shdCritHit.rank2 = globalData.shdData[i].shdCritHitStat.rank2;
            inventoryM.shieldInv.itemData[i].shdCritHit.rank3 = globalData.shdData[i].shdCritHitStat.rank3;

            inventoryM.shieldInv.itemData[i].shdBlock.value = globalData.shdData[i].shdBlockStat.value;
            inventoryM.shieldInv.itemData[i].shdBlock.rank1 = globalData.shdData[i].shdBlockStat.rank1;
            inventoryM.shieldInv.itemData[i].shdBlock.rank2 = globalData.shdData[i].shdBlockStat.rank2;
            inventoryM.shieldInv.itemData[i].shdBlock.rank3 = globalData.shdData[i].shdBlockStat.rank3;

            inventoryM.shieldInv.itemData[i].shdDura.value = globalData.shdData[i].shdDuraStat.value;
            inventoryM.shieldInv.itemData[i].shdDura.rank1 = globalData.shdData[i].shdDuraStat.rank1;
            inventoryM.shieldInv.itemData[i].shdDura.rank2 = globalData.shdData[i].shdDuraStat.rank2;
            inventoryM.shieldInv.itemData[i].shdDura.rank3 = globalData.shdData[i].shdDuraStat.rank3;

            inventoryM.shieldInv.itemData[i].shdSpecialStat1 = (ItemData.ShieldSpecialStat1)globalData.shdData[i].shdSpecialStat1;
            inventoryM.shieldInv.itemData[i].shdSpecialStat2 = (ItemData.ShieldSpecialStat2)globalData.shdData[i].shdSpecialStat2;

            inventoryM.shieldInv.itemData[i].bowWeight = globalData.shdData[i].weight;
            inventoryM.shieldInv.itemData[i].weaponArmsID = globalData.shdData[i].weaponArmsID;

            inventoryM.shieldInv.itemData[i].rankNum = globalData.shdData[i].rankNum;

            inventoryM.shieldInv.itemData[i].upgradeMaterial = new ItemData.Material[globalData.shdData[i].upgradeMaterial.Length];
            for (int matSize = 0; matSize < globalData.shdData[i].upgradeMaterial.Length; matSize++)
            {
                inventoryM.shieldInv.itemData[i].upgradeMaterial[matSize].matName = globalData.shdData[i].upgradeMaterial[matSize].matName;
                inventoryM.shieldInv.itemData[i].upgradeMaterial[matSize].matRequired = globalData.shdData[i].upgradeMaterial[matSize].matRequired;
            }

            inventoryM.shieldInv.itemData[i].buyPrice = globalData.shdData[i].buyPrice;
            inventoryM.shieldInv.itemData[i].sellPrice = globalData.shdData[i].sellPrice;
            inventoryM.shieldInv.itemData[i].repairPrice = globalData.shdData[i].repairPrice;
            inventoryM.shieldInv.itemData[i].upgradePrice = globalData.shdData[i].upgradePrice;
            inventoryM.shieldInv.itemData[i].specialPrice = globalData.shdData[i].specialPrice;
            inventoryM.shieldInv.itemData[i].inStockQuantity = globalData.shdData[i].inStockQuantity;

            inventoryM.shieldInv.counter[i] = globalData.shdData[i].quantity;
            inventoryM.shieldInv.itemData[i].stackable = globalData.shdData[i].stackable;
            inventoryM.shieldInv.itemData[i].inInventory = globalData.shdData[i].inInventory;
            inventoryM.shieldInv.itemData[i].equipped = globalData.shdData[i].equipped;

            inventoryM.shieldInv.itemData[i].broken = globalData.shdData[i].broken;
            inventoryM.shieldInv.itemData[i].rankTag = globalData.shdData[i].rankTag;
            inventoryM.shieldInv.itemData[i].originItemName = globalData.shdData[i].originItemName;
            inventoryM.shieldInv.itemData[i].numOfEngraving = globalData.shdData[i].numOfEngraving;

            inventoryM.shieldInv.itemData[i].maxBowAtk = globalData.shdData[i].maxBowAtk;
            inventoryM.shieldInv.itemData[i].maxBowSpd = globalData.shdData[i].maxBowSpd;
            inventoryM.shieldInv.itemData[i].maxBowStun = globalData.shdData[i].maxBowStun;
            inventoryM.shieldInv.itemData[i].maxBowCritHit = globalData.shdData[i].maxBowCritHit;
            inventoryM.shieldInv.itemData[i].maxBowHdShot = globalData.shdData[i].maxBowHdShot;
            inventoryM.shieldInv.itemData[i].maxBowDura = globalData.shdData[i].maxBowDura;

            inventoryM.shieldInv.image[i].type = Image.Type.Simple;
            inventoryM.shieldInv.image[i].preserveAspect = true;
            Navigation automatic = new Navigation { mode = Navigation.Mode.Automatic };
            inventoryM.shieldInv.image[i].GetComponent<Button>().navigation = automatic;
            inventoryM.shieldInv.image[i].enabled = true;

            #region Set Inventory BG Color

            switch (globalData.shdData[i].rankNum)
            {
                case 0:
                    inventoryM.shieldInv.highLight[i].GetComponent<UIGradient>().enabled = false;
                    inventoryM.shieldInv.highLight[i].color = new Color(0, 0, 0, 80);
                    break;
                case 1:
                    inventoryM.shieldInv.highLight[i].GetComponent<UIGradient>().enabled = true;
                    inventoryM.shieldInv.highLight[i].GetComponent<UIGradient>().LinearColor1 = inventoryM.rank1Color1;
                    inventoryM.shieldInv.highLight[i].GetComponent<UIGradient>().LinearColor2 = inventoryM.rank1Color2;
                    break;
                case 2:
                    inventoryM.shieldInv.highLight[i].GetComponent<UIGradient>().enabled = true;
                    inventoryM.shieldInv.highLight[i].GetComponent<UIGradient>().LinearColor1 = inventoryM.rank2Color1;
                    inventoryM.shieldInv.highLight[i].GetComponent<UIGradient>().LinearColor2 = inventoryM.rank2Color2;
                    break;
                case 3:
                    inventoryM.shieldInv.highLight[i].GetComponent<UIGradient>().enabled = true;
                    inventoryM.shieldInv.highLight[i].GetComponent<UIGradient>().LinearColor1 = inventoryM.rank3Color1;
                    inventoryM.shieldInv.highLight[i].GetComponent<UIGradient>().LinearColor2 = inventoryM.rank3Color2;
                    break;
            }

            if (globalData.shdData[i].broken)
            {
                inventoryM.shieldInv.highLight[i].GetComponent<UIGradient>().enabled = true;
                inventoryM.shieldInv.highLight[i].GetComponent<UIGradient>().LinearColor1 = inventoryM.brokenColor1;
                inventoryM.shieldInv.highLight[i].GetComponent<UIGradient>().LinearColor2 = inventoryM.brokenColor2;
            }

            #endregion

            if (inventoryM.shieldInv.itemData[i].equipped)
            {
                inventoryM.shieldInv.outLineBorder[i].enabled = true;
                inventoryM.shieldInv.outLineBorder[i].color = inventoryM.outLineSelected;
            }

            if (!inventoryM.shieldInv.itemData[i].stackable)
            {
                inventoryM.shieldInv.counter[i] = 1;
                inventoryM.shieldInv.image[i].enabled = true;
            }
            else
            {
                inventoryM.shieldInv.image[i].enabled = true;
                inventoryM.shieldInv.quantity[i].enabled = true;
                inventoryM.shieldInv.counter[i] = globalData.shdData[i].quantity;
                inventoryM.shieldInv.quantity[i].text = "x" + inventoryM.shieldInv.counter[i].ToString();
            }
            SetLoadedSprite(ref inventoryM.shieldInv.itemData[i], ref inventoryM.shieldInv.image[i]);
        }

        #endregion

        loadingDataNameText.text = "Loading Shield slots Data...";
        yield return new WaitForSeconds(0.8f);

        #region Load Armor Inventory Data

        inventoryM.armorInv.slotNum = globalData.armSlotNum;
        inventoryM.armorInv.slotArmorHeadEquipped = globalData.armSlotHeadEquipped;
        inventoryM.armorInv.slotArmorChestEquipped = globalData.armSlotChestEquipped;
        inventoryM.armorInv.slotArmorAmuletEquipped = globalData.armSlotAmuletEquipped;

        inventoryM.armorInv.itemData = new ItemData[globalData.armData.Length];
        for (int i = 0; i < globalData.armData.Length; i++)
        {
            GameObject emptyitemData = Instantiate(itemDataPrefab) as GameObject;
            inventoryM.armorInv.itemData[i] = emptyitemData.GetComponent<ItemData>();

            inventoryM.armorInv.itemData[i].itemType = (ItemData.ItemType)globalData.armData[i].itemType;
            inventoryM.armorInv.itemData[i].itemName = globalData.armData[i].itemName;
            inventoryM.armorInv.itemData[i].itemDescription = globalData.armData[i].itemDescription;

            inventoryM.armorInv.itemData[i].arm.value = globalData.armData[i].armStat.value;
            inventoryM.armorInv.itemData[i].arm.rank1 = globalData.armData[i].armStat.rank1;
            inventoryM.armorInv.itemData[i].arm.rank2 = globalData.armData[i].armStat.rank2;
            inventoryM.armorInv.itemData[i].arm.rank3 = globalData.armData[i].armStat.rank3;

            inventoryM.armorInv.itemData[i].armLRes.value = globalData.armData[i].armLResStat.value;
            inventoryM.armorInv.itemData[i].armLRes.rank1 = globalData.armData[i].armLResStat.rank1;
            inventoryM.armorInv.itemData[i].armLRes.rank2 = globalData.armData[i].armLResStat.rank2;
            inventoryM.armorInv.itemData[i].armLRes.rank3 = globalData.armData[i].armLResStat.rank3;

            inventoryM.armorInv.itemData[i].armHRes.value = globalData.armData[i].armHResStat.value;
            inventoryM.armorInv.itemData[i].armHRes.rank1 = globalData.armData[i].armHResStat.rank1;
            inventoryM.armorInv.itemData[i].armHRes.rank2 = globalData.armData[i].armHResStat.rank2;
            inventoryM.armorInv.itemData[i].armHRes.rank3 = globalData.armData[i].armHResStat.rank3;

            inventoryM.armorInv.itemData[i].armSpecialStat1 = (ItemData.ArmorSpecialStat1)globalData.armData[i].armSpecialStat1;
            inventoryM.armorInv.itemData[i].armSpecialStat2 = (ItemData.ArmorSpecialStat2)globalData.armData[i].armSpecialStat2;

            inventoryM.armorInv.itemData[i].bowWeight = globalData.armData[i].weight;

            inventoryM.armorInv.itemData[i].rankNum = globalData.armData[i].rankNum;

            inventoryM.armorInv.itemData[i].upgradeMaterial = new ItemData.Material[globalData.armData[i].upgradeMaterial.Length];
            for (int matSize = 0; matSize < globalData.armData[i].upgradeMaterial.Length; matSize++)
            {
                inventoryM.armorInv.itemData[i].upgradeMaterial[matSize].matName = globalData.armData[i].upgradeMaterial[matSize].matName;
                inventoryM.armorInv.itemData[i].upgradeMaterial[matSize].matRequired = globalData.armData[i].upgradeMaterial[matSize].matRequired;
            }

            inventoryM.armorInv.itemData[i].buyPrice = globalData.armData[i].buyPrice;
            inventoryM.armorInv.itemData[i].sellPrice = globalData.armData[i].sellPrice;
            inventoryM.armorInv.itemData[i].repairPrice = globalData.armData[i].repairPrice;
            inventoryM.armorInv.itemData[i].upgradePrice = globalData.armData[i].upgradePrice;
            inventoryM.armorInv.itemData[i].specialPrice = globalData.armData[i].specialPrice;
            inventoryM.armorInv.itemData[i].inStockQuantity = globalData.armData[i].inStockQuantity;

            inventoryM.armorInv.counter[i] = globalData.armData[i].quantity;
            inventoryM.armorInv.itemData[i].stackable = globalData.armData[i].stackable;
            inventoryM.armorInv.itemData[i].inInventory = globalData.armData[i].inInventory;
            inventoryM.armorInv.itemData[i].equipped = globalData.armData[i].equipped;

            inventoryM.armorInv.itemData[i].broken = globalData.armData[i].broken;
            inventoryM.armorInv.itemData[i].rankTag = globalData.armData[i].rankTag;
            inventoryM.armorInv.itemData[i].originItemName = globalData.armData[i].originItemName;
            inventoryM.armorInv.itemData[i].numOfEngraving = globalData.armData[i].numOfEngraving;

            inventoryM.armorInv.itemData[i].maxArm = globalData.armData[i].maxArm;
            inventoryM.armorInv.itemData[i].maxArmLRes = globalData.armData[i].maxArmLRes;
            inventoryM.armorInv.itemData[i].maxArmHRes = globalData.armData[i].maxArmHRes;

            inventoryM.armorInv.image[i].type = Image.Type.Simple;
            inventoryM.armorInv.image[i].preserveAspect = true;
            Navigation automatic = new Navigation { mode = Navigation.Mode.Automatic };
            inventoryM.armorInv.image[i].GetComponent<Button>().navigation = automatic;
            inventoryM.armorInv.image[i].enabled = true;

            #region Set Inventory BG Color

            switch (globalData.armData[i].rankNum)
            {
                case 0:
                    inventoryM.armorInv.highLight[i].GetComponent<UIGradient>().enabled = false;
                    inventoryM.armorInv.highLight[i].color = new Color(0, 0, 0, 80);
                    break;
                case 1:
                    inventoryM.armorInv.highLight[i].GetComponent<UIGradient>().enabled = true;
                    inventoryM.armorInv.highLight[i].GetComponent<UIGradient>().LinearColor1 = inventoryM.rank1Color1;
                    inventoryM.armorInv.highLight[i].GetComponent<UIGradient>().LinearColor2 = inventoryM.rank1Color2;
                    break;
                case 2:
                    inventoryM.armorInv.highLight[i].GetComponent<UIGradient>().enabled = true;
                    inventoryM.armorInv.highLight[i].GetComponent<UIGradient>().LinearColor1 = inventoryM.rank2Color1;
                    inventoryM.armorInv.highLight[i].GetComponent<UIGradient>().LinearColor2 = inventoryM.rank2Color2;
                    break;
                case 3:
                    inventoryM.armorInv.highLight[i].GetComponent<UIGradient>().enabled = true;
                    inventoryM.armorInv.highLight[i].GetComponent<UIGradient>().LinearColor1 = inventoryM.rank3Color1;
                    inventoryM.armorInv.highLight[i].GetComponent<UIGradient>().LinearColor2 = inventoryM.rank3Color2;
                    break;
            }

            if (globalData.armData[i].broken)
            {
                inventoryM.armorInv.highLight[i].GetComponent<UIGradient>().enabled = true;
                inventoryM.armorInv.highLight[i].GetComponent<UIGradient>().LinearColor1 = inventoryM.brokenColor1;
                inventoryM.armorInv.highLight[i].GetComponent<UIGradient>().LinearColor2 = inventoryM.brokenColor2;
            }

            #endregion

            if (inventoryM.armorInv.itemData[i].equipped)
            {
                inventoryM.armorInv.outLineBorder[i].enabled = true;
                inventoryM.armorInv.outLineBorder[i].color = inventoryM.outLineSelected;
            }

            if (!inventoryM.armorInv.itemData[i].stackable)
            {
                inventoryM.armorInv.counter[i] = 1;
                inventoryM.armorInv.image[i].enabled = true;
            }
            else
            {
                inventoryM.armorInv.image[i].enabled = true;
                inventoryM.armorInv.quantity[i].enabled = true;
                inventoryM.armorInv.counter[i] = globalData.armData[i].quantity;
                inventoryM.armorInv.quantity[i].text = "x" + inventoryM.armorInv.counter[i].ToString();
            }
            SetLoadedSprite(ref inventoryM.armorInv.itemData[i], ref inventoryM.armorInv.image[i]);
        }

        #endregion

        loadingDataNameText.text = "Loading Armor slots data...";
        yield return new WaitForSeconds(0.8f);

        #region Load Material Inventory Data

        inventoryM.materialInv.slotNum = globalData.matSlotNum;

        inventoryM.materialInv.itemData = new ItemData[globalData.matData.Length];
        for (int i = 0; i < globalData.matData.Length; i++)
        {
            GameObject emptyitemData = Instantiate(itemDataPrefab) as GameObject;
            inventoryM.materialInv.itemData[i] = emptyitemData.GetComponent<ItemData>();

            inventoryM.materialInv.itemData[i].itemType = (ItemData.ItemType)globalData.matData[i].itemType;
            inventoryM.materialInv.itemData[i].itemName = globalData.matData[i].itemName;
            inventoryM.materialInv.itemData[i].itemDescription = globalData.matData[i].itemDescription;

            inventoryM.materialInv.itemData[i].buyPrice = globalData.matData[i].buyPrice;
            inventoryM.materialInv.itemData[i].sellPrice = globalData.matData[i].sellPrice;
            inventoryM.materialInv.itemData[i].repairPrice = globalData.matData[i].repairPrice;
            inventoryM.materialInv.itemData[i].upgradePrice = globalData.matData[i].upgradePrice;
            inventoryM.materialInv.itemData[i].specialPrice = globalData.matData[i].specialPrice;
            inventoryM.materialInv.itemData[i].inStockQuantity = globalData.matData[i].inStockQuantity;

            inventoryM.materialInv.counter[i] = globalData.matData[i].quantity;
            inventoryM.materialInv.itemData[i].stackable = globalData.matData[i].stackable;
            inventoryM.materialInv.itemData[i].inInventory = globalData.matData[i].inInventory;

            inventoryM.materialInv.image[i].type = Image.Type.Simple;
            inventoryM.materialInv.image[i].preserveAspect = true;
            Navigation automatic = new Navigation { mode = Navigation.Mode.Automatic };
            inventoryM.materialInv.image[i].GetComponent<Button>().navigation = automatic;
            inventoryM.materialInv.image[i].enabled = true;

            if (!inventoryM.materialInv.itemData[i].stackable)
            {
                inventoryM.materialInv.counter[i] = 1;
                inventoryM.materialInv.image[i].enabled = true;
            }
            else
            {
                inventoryM.materialInv.image[i].enabled = true;
                inventoryM.materialInv.quantity[i].enabled = true;
                inventoryM.materialInv.counter[i] = globalData.matData[i].quantity;
                inventoryM.materialInv.quantity[i].text = "x" + inventoryM.materialInv.counter[i].ToString();
            }
            SetLoadedSprite(ref inventoryM.materialInv.itemData[i], ref inventoryM.materialInv.image[i]);
        }

        #endregion

        loadingDataNameText.text = "Loading Material slots data...";
        yield return new WaitForSeconds(0.8f);

        #region Load Healing Inventory Data

        inventoryM.healingInv.slotNum = globalData.healSlotNum;

        inventoryM.healingInv.itemData = new ItemData[globalData.healData.Length];
        for (int i = 0; i < globalData.healData.Length; i++)
        {
            GameObject emptyitemData = Instantiate(itemDataPrefab) as GameObject;
            inventoryM.healingInv.itemData[i] = emptyitemData.GetComponent<ItemData>();

            inventoryM.healingInv.itemData[i].itemType = (ItemData.ItemType)globalData.healData[i].itemType;
            inventoryM.healingInv.itemData[i].itemName = globalData.healData[i].itemName;
            inventoryM.healingInv.itemData[i].itemDescription = globalData.healData[i].itemDescription;

            inventoryM.healingInv.itemData[i].buyPrice = globalData.healData[i].buyPrice;
            inventoryM.healingInv.itemData[i].sellPrice = globalData.healData[i].sellPrice;
            inventoryM.healingInv.itemData[i].repairPrice = globalData.healData[i].repairPrice;
            inventoryM.healingInv.itemData[i].upgradePrice = globalData.healData[i].upgradePrice;
            inventoryM.healingInv.itemData[i].specialPrice = globalData.healData[i].specialPrice;
            inventoryM.healingInv.itemData[i].inStockQuantity = globalData.healData[i].inStockQuantity;

            inventoryM.healingInv.counter[i] = globalData.healData[i].quantity;
            inventoryM.healingInv.itemData[i].stackable = globalData.healData[i].stackable;
            inventoryM.healingInv.itemData[i].inInventory = globalData.healData[i].inInventory;

            inventoryM.healingInv.image[i].type = Image.Type.Simple;
            inventoryM.healingInv.image[i].preserveAspect = true;
            Navigation automatic = new Navigation { mode = Navigation.Mode.Automatic };
            inventoryM.healingInv.image[i].GetComponent<Button>().navigation = automatic;
            inventoryM.healingInv.image[i].enabled = true;

            if (!inventoryM.healingInv.itemData[i].stackable)
            {
                inventoryM.healingInv.counter[i] = 1;
                inventoryM.healingInv.image[i].enabled = true;
            }
            else
            {
                inventoryM.healingInv.image[i].enabled = true;
                inventoryM.healingInv.quantity[i].enabled = true;
                inventoryM.healingInv.counter[i] = globalData.healData[i].quantity;
                inventoryM.healingInv.quantity[i].text = "x" + inventoryM.healingInv.counter[i].ToString();
            }
            SetLoadedSprite(ref inventoryM.healingInv.itemData[i], ref inventoryM.healingInv.image[i]);
        }

        #endregion

        loadingDataNameText.text = "Loading Healing slots data...";
        yield return new WaitForSeconds(0.8f);

        #region Load Key Inventory Data

        inventoryM.keyInv.slotNum = globalData.keySlotNum;

        inventoryM.keyInv.itemData = new ItemData[globalData.keyData.Length];
        for (int i = 0; i < globalData.keyData.Length; i++)
        {
            GameObject emptyitemData = Instantiate(itemDataPrefab) as GameObject;
            inventoryM.keyInv.itemData[i] = emptyitemData.GetComponent<ItemData>();

            inventoryM.keyInv.itemData[i].itemType = (ItemData.ItemType)globalData.keyData[i].itemType;

            inventoryM.keyInv.itemData[i].itemType = (ItemData.ItemType)globalData.keyData[i].itemType;
            inventoryM.keyInv.itemData[i].itemName = globalData.keyData[i].itemName;
            inventoryM.keyInv.itemData[i].itemDescription = globalData.keyData[i].itemDescription;

            inventoryM.keyInv.itemData[i].buyPrice = globalData.keyData[i].buyPrice;
            inventoryM.keyInv.itemData[i].sellPrice = globalData.keyData[i].sellPrice;
            inventoryM.keyInv.itemData[i].repairPrice = globalData.keyData[i].repairPrice;
            inventoryM.keyInv.itemData[i].upgradePrice = globalData.keyData[i].upgradePrice;
            inventoryM.keyInv.itemData[i].specialPrice = globalData.keyData[i].specialPrice;
            inventoryM.keyInv.itemData[i].inStockQuantity = globalData.keyData[i].inStockQuantity;

            inventoryM.keyInv.counter[i] = globalData.keyData[i].quantity;
            inventoryM.keyInv.itemData[i].stackable = globalData.keyData[i].stackable;
            inventoryM.keyInv.itemData[i].inInventory = globalData.keyData[i].inInventory;

            inventoryM.keyInv.image[i].type = Image.Type.Simple;
            inventoryM.keyInv.image[i].preserveAspect = true;
            Navigation automatic = new Navigation { mode = Navigation.Mode.Automatic };
            inventoryM.keyInv.image[i].GetComponent<Button>().navigation = automatic;
            inventoryM.keyInv.image[i].enabled = true;

            if (!inventoryM.keyInv.itemData[i].stackable)
            {
                inventoryM.keyInv.counter[i] = 1;
                inventoryM.keyInv.image[i].enabled = true;
            }
            else
            {
                inventoryM.keyInv.image[i].enabled = true;
                inventoryM.keyInv.quantity[i].enabled = true;
                inventoryM.keyInv.counter[i] = globalData.keyData[i].quantity;
                inventoryM.keyInv.quantity[i].text = "x" + inventoryM.keyInv.counter[i].ToString();
            }
            SetLoadedSprite(ref inventoryM.keyInv.itemData[i], ref inventoryM.keyInv.image[i]);
        }

        #endregion

        loadingDataNameText.text = "Loading Key slots data...";
        yield return new WaitForSeconds(0.8f);

        #endregion

        #region Load Skill Tree Data

        inventoryM.referencesObj.skillsSection.gameObject.SetActive(true);
        inventoryM.referencesObj.skillsSection.GetComponent<CanvasGroup>().alpha = 0;
        inventoryM.referencesObj.skillsSection.GetComponent<CanvasGroup>().interactable = false;
        inventoryM.referencesObj.skillsSection.GetComponent<CanvasGroup>().blocksRaycasts = false;

        skillM.topAbilityNameMenu = globalData.topAbilityName;
        skillM.leftAbilityNameMenu = globalData.leftAbilityName;
        skillM.rightAbilityNameMenu = globalData.rightAbilityName;
        skillM.bottomAbilityNameMenu = globalData.bottomAbilityName;

        for (int i = 0; i < skillSlots.Length; i++)
        {
            if (skillSlots.Length > 0)
            {
                if (skillSlots[i].purchasedSkillName == globalData.purchasedSkillName[i])
                {
                    skillSlots[i].m_isPurchased = true;
                }
            }
        }

        #endregion

        loadingDataNameText.text = "Loading Skill Tree data...";
        yield return new WaitForSeconds(1);

        #region Load Miscellaneous Data

        if (sceneData != null)
        {
            for (int i = 0; i < locationCPData.Length; i++)
            {
                if (locationCPData[i] != null)
                {
                    locationCPData[i].explored = sceneData.explored[i];
                }
            }

            for (int i = 0; i < sceneData.triggerAction.Length; i++)
            {
                if (triggerActionData.Length > 0)
                {
                    triggerActionData[i].isActive = sceneData.triggerAction[i].isActive;
                    triggerActionData[i].interactionType = (TriggerAction.InteractionType)sceneData.triggerAction[i].interactionType;
                    triggerActionData[i].destroyAfter = sceneData.triggerAction[i].destroyAfter;

                    // Treasure Chest
                    if (triggerActionData[i].isActive && triggerActionData[i].item != null)
                    {
                        if (triggerActionData[i].animator)
                        {
                            triggerActionData[i].animator.SetBool("Activate", true);
                        }
                    }

                    // Moving Platform 
                    if (triggerActionData[i].isActive && triggerActionData[i].name == "MovingPlatform" &&
                        triggerActionData[i].interactionType == TriggerAction.InteractionType.Normal)
                    {
                        if (triggerActionData[i].animator)
                        {
                            triggerActionData[i].animator.SetBool("Activate", false);
                        }
                    }

                    // Pressure Pad
                    if (triggerActionData[i].isActive && triggerActionData[i].item == null &&
                    (triggerActionData[i].interactionType == TriggerAction.InteractionType.PressureStep ||
                    triggerActionData[i].interactionType == TriggerAction.InteractionType.PressureObjectWeight))
                    {
                        if (triggerActionData[i].animator)
                        {
                            triggerActionData[i].animator.SetBool("Activate", true);
                        }
                    }

                    // Generic Trigger
                    if (triggerActionData[i].isActive && triggerActionData[i].item == null &&
                        triggerActionData[i].interactionType == TriggerAction.InteractionType.Normal)
                    {
                        if (triggerActionData[i].animator)
                        {
                            triggerActionData[i].animator.SetBool("Activate", false);
                            triggerActionData[i].animator.SetInteger("Side", triggerActionData[i].side);
                        }

                        if (triggerActionData[i].gameObject.name != "MovingPlatform" && triggerActionData[i].gameObject.name != "Sphere")
                            triggerActionData[i].isActive = false;
                    }
                }
            }

            // Interactable Items
            for (int i = 0; i < inventoryM.initWpns.Length; i++)
            {
                inventoryM.initWpns[i].GetComponent<ItemData>().itemActive = sceneData.itemActive[i];
            }
        }

        #endregion

        // When AI position/rotation is repositoned enabled NavMeshAgent.
        foreach (AIController npc in dialogueData)
        {
            if (npc.GetComponent<NavMeshAgent>() != null)
                npc.GetComponent<NavMeshAgent>().enabled = true;
        }

        loadingDataNameText.text = "Loading Scene...";
        yield return new WaitForSeconds(1.5f);

        loadingDataNameText.text = "";

        loadingScreenFUI.FadeTransition(0, 0, 1);

        // Deactivate Menu Section that was activate in order to load data.
        inventoryM.InventorySectionIsActive(false);
        inventoryM.PauseMenuSectionIsActive(false);

        PlayerPrefs.SetInt("SceneInTransitionDataRef", 0);

        inventoryM.referencesObj.inventorySection.GetComponent<HideUI>().enabled = true;
        inventoryM.referencesObj.questObjective.gameObject.SetActive(true);
        inventoryM.referencesObj.questObjective.gameObject.SetActive(true);

        isLoadingData = false;
    }

    #endregion

    #region Loading Local Data Process

    protected IEnumerator LoadLocalSavedData()
    {
        isLoadingData = true;

        foreach (HideUI obj in FindObjectsOfType<HideUI>())
        {
            obj.enabled = false;
        }

        // Disable Navmesh in order to reposition AI
        foreach (AIController npc in dialogueData)
        {
            if (npc.GetComponent<NavMeshAgent>() != null)
                npc.GetComponent<NavMeshAgent>().enabled = false;
        }

        // Deactivate Game Over UI before loading
        if (gameOverFUI.gameObject.activeInHierarchy)
        {
            gameOverFUI.canvasGroup.alpha = 0;
            gameOverFUI.gameObject.SetActive(false);
        }

        loadingDataNameText.text = "Loading...";
        yield return new WaitForSeconds(1);

        blackScreenFUI.canvasGroup.alpha = 0;

        // Set Inventory and Menu Sections Active during initialization.
        inventoryM.InventorySectionIsActive(true);
        inventoryM.PauseMenuSectionIsActive(true);

        #region Load Player Data

        cc.animator.SetBool("Dead", false);

        hudM.curLevel = transitionData.level;
        hudM.curHealth = transitionData.curhealth;
        hudM.maxHealth = transitionData.maxhealth;
        hudM.curEnegry = transitionData.curEnegry;
        hudM.maxEnegry = transitionData.maxEnegry;
        hudM.curExp = transitionData.curExp;
        hudM.expRequired = transitionData.reqExp;
        hudM.curSkillPoint = transitionData.skillPoint;

        #endregion

        loadingDataNameText.text = "Initializing Player data...";
        yield return new WaitForSeconds(0.8f);

        #region Load MiniMap Data

        if (sceneData != null)
        {
            miniMap.isMarkerOnMap = sceneData.isMarkerOnMap;

            if (miniMap.isMarkerOnMap)
            {
                GameObject mapMarkerPrefab = Instantiate(miniMap.MapPointerPrefab) as GameObject;

                mapMarkerPrefab.GetComponent<MapPointer>().isAudioActive = false;
                mapMarkerPrefab.transform.position = new Vector3(sceneData.mapMarkerPosition[0], sceneData.mapMarkerPosition[1], sceneData.mapMarkerPosition[2]);
                mapMarkerPrefab.GetComponent<MapPointer>().SetColor(miniMap.playerColor);
            }
        }

        #endregion

        loadingDataNameText.text = "Loading MiniMap Data...";
        yield return new WaitForSeconds(miniMap.isMarkerOnMap ? 1 : 0);

        #region Load Quest Data

        QuestManager.numOfMainQuestSlot = 0;
        QuestManager.numOfSideQuestSlot = 0;

        ResetQuest();

        questSlots = FindObjectsOfType<QuestSlot>();
        foreach (QuestSlot slots in questSlots)
        {
            Destroy(slots.gameObject);
        }

        #region Load Main Quest Data

        int mQInt = transitionData.mainQuestData.Length;

        QuestData[] tempMQ0 = new QuestData[mQInt];
        TextMeshProUGUI[] tempMQ1 = new TextMeshProUGUI[mQInt];
        TextMeshProUGUI[] tempMQ2 = new TextMeshProUGUI[mQInt];
        TextMeshProUGUI[] tempMQ3 = new TextMeshProUGUI[mQInt];
        TextMeshProUGUI[] tempMQ4 = new TextMeshProUGUI[mQInt];
        Image[] tempMQ5 = new Image[mQInt];

        questM.mainQuestSlots.slotNum = transitionData.mainQuestSlot;
        questM.mainQuestSlots.questData = new QuestData[mQInt];

        for (int i = 0; i < transitionData.mainQuestData.Length; i++)
        {
            // Create and set Quest Type.
            GameObject questSlot = Instantiate(questSlotPrefab) as GameObject;
            questSlot.GetComponent<QuestSlot>().questType = QuestSlot.QuestType.MainQuest;

            QuestManager.numOfMainQuestSlot++;
            questSlot.GetComponent<QuestSlot>().slotNum = QuestManager.numOfMainQuestSlot - 1;

            tempMQ0[i] = questSlot.GetComponent<QuestData>();
            tempMQ1[i] = questSlot.GetComponent<QuestSlot>().questName;
            tempMQ2[i] = questSlot.GetComponent<QuestSlot>().questLocation;
            tempMQ3[i] = questSlot.GetComponent<QuestSlot>().questDistance;
            tempMQ4[i] = questSlot.GetComponent<QuestSlot>().questLevel;
            tempMQ5[i] = questSlot.GetComponent<QuestSlot>().highLight;

            QuestData[] tempMQ00 = new QuestData[mQInt];
            TextMeshProUGUI[] tempMQ11 = new TextMeshProUGUI[mQInt];
            TextMeshProUGUI[] tempMQ22 = new TextMeshProUGUI[mQInt];
            TextMeshProUGUI[] tempMQ33 = new TextMeshProUGUI[mQInt];
            TextMeshProUGUI[] tempMQ44 = new TextMeshProUGUI[mQInt];
            Image[] tempMQ55 = new Image[mQInt];

            if (questM.mainQuestSlots.questData[0] != null)
            {
                tempMQ00[i] = questM.mainQuestSlots.questData[i];
                tempMQ11[i] = questM.mainQuestSlots.questName[i];
                tempMQ22[i] = questM.mainQuestSlots.questLocationName[i];
                tempMQ33[i] = questM.mainQuestSlots.questDistance[i];
                tempMQ44[i] = questM.mainQuestSlots.questLevel[i];
                tempMQ55[i] = questM.mainQuestSlots.highLight[i];
            }

            questM.mainQuestSlots.questData = tempMQ00;
            questM.mainQuestSlots.questName = tempMQ11;
            questM.mainQuestSlots.questLocationName = tempMQ22;
            questM.mainQuestSlots.questDistance = tempMQ33;
            questM.mainQuestSlots.questLevel = tempMQ44;
            questM.mainQuestSlots.highLight = tempMQ55;

            questM.mainQuestSlots.questData = tempMQ0;
            questM.mainQuestSlots.questName = tempMQ1;
            questM.mainQuestSlots.questLocationName = tempMQ2;
            questM.mainQuestSlots.questDistance = tempMQ3;
            questM.mainQuestSlots.questLevel = tempMQ4;
            questM.mainQuestSlots.highLight = tempMQ5;

            questM.mainQuestSlots.questData[i].questType = (QuestData.QuestType)transitionData.mainQuestData[i].questType;
            questM.mainQuestSlots.questData[i].questName = transitionData.mainQuestData[i].questName;
            questM.mainQuestSlots.questData[i].questLevel = transitionData.mainQuestData[i].questLevel;

            questM.mainQuestSlots.questData[i].EXPReward = transitionData.mainQuestData[i].questEXPReward;
            questM.mainQuestSlots.questData[i].questDescription = transitionData.mainQuestData[i].questDescription;
            questM.mainQuestSlots.questData[i].curObjectivePhase = transitionData.mainQuestData[i].curObjectivePhase;
            questM.mainQuestSlots.questData[i].maxObjectivePhase = transitionData.mainQuestData[i].maxObjectivePhase;

            #region New Main Quest Objective Data

            questM.mainQuestSlots.questData[i].questObjectiveNum = transitionData.mainQuestData[i].questObjectiveNum;

            questM.mainQuestSlots.questData[i].questObjective = new QuestData.QuestObjective[transitionData.mainQuestData[i].questObective.Length];

            for (int mQSize = 0; mQSize < transitionData.mainQuestData[i].questObective.Length; mQSize++)
            {
                // Create New Objective Location.
                GameObject newMainQuestPosPrefab = Instantiate(questObjectivePosPrefab) as GameObject;
                newMainQuestPosPrefab.name = "MainQuestPositionPrefab";

                newMainQuestPosPrefab.GetComponent<SetQuestObjectiveLocation>().phaseNum = mQSize;

                questM.mainQuestSlots.questData[i].questObjective[mQSize].questLocation = newMainQuestPosPrefab.transform;

                for (int ki = 0; ki < newMainQuestPosPrefab.GetComponent<SetQuestObjectiveLocation>().waypoints.Length; ki++)
                {
                    if (questM.mainQuestSlots.questData[i].questName == newMainQuestPosPrefab.GetComponent<SetQuestObjectiveLocation>().waypoints[ki].questSequenceNameReceipt &&
                        mQSize == newMainQuestPosPrefab.GetComponent<SetQuestObjectiveLocation>().waypoints[ki].questPhaseReceipt)
                    {
                        newMainQuestPosPrefab.GetComponent<SetQuestObjectiveLocation>().waypoints[ki].GetComponent<MiniMapItem>().Size = 1;
                        newMainQuestPosPrefab.GetComponent<SetQuestObjectiveLocation>().waypoints[ki].GetComponent<MiniMapItem>().HideCircleArea();
                        questM.mainQuestSlots.questData[i].questObjective[mQSize].questLocation = newMainQuestPosPrefab.GetComponent<SetQuestObjectiveLocation>().waypoints[ki].transform;
                    }
                }

                questM.mainQuestSlots.questData[i].questObjective[mQSize].questLocation.position =
                new Vector3(transitionData.mainQuestData[i].questObective[mQSize].questLocationX,
                transitionData.mainQuestData[i].questObective[mQSize].questLocationY,
                transitionData.mainQuestData[i].questObective[mQSize].questLocationZ);

                questM.mainQuestSlots.questData[i].questObjective[mQSize].questLocationName = transitionData.mainQuestData[i].questObective[mQSize].questLocationName;
                questM.mainQuestSlots.questData[i].questObjective[mQSize].questObjectiveSubject = transitionData.mainQuestData[i].questObective[mQSize].questObjectiveSubject;
                questM.mainQuestSlots.questData[i].questObjective[mQSize].questHint = transitionData.mainQuestData[i].questObective[mQSize].questHint;

                questM.mainQuestSlots.questData[i].questObjective[mQSize].curCollectableAmount = transitionData.mainQuestData[i].questObective[mQSize].curCollectableAmount;
                questM.mainQuestSlots.questData[i].questObjective[mQSize].maxCollectableAmount = transitionData.mainQuestData[i].questObective[mQSize].maxCollectableAmount;
            }

            #endregion

            questM.mainQuestSlots.questData[i].inProgress = transitionData.mainQuestData[i].questInProgress;
            if (questM.mainQuestSlots.questData[i].inProgress) questM.MainQuestSlotSelect(transitionData.mainQuestSlot);
            questM.mainQuestSlots.questData[i].isComplete = transitionData.mainQuestData[i].questIsComplete;

            questSlot.GetComponent<QuestSlot>().questName.text = questSlot.GetComponent<QuestData>().questName;
            questSlot.GetComponent<QuestSlot>().questLocation.text = questSlot.GetComponent<QuestData>().questObjective[questSlot.GetComponent<QuestData>().questObjectiveNum].questLocationName;
            questSlot.GetComponent<QuestSlot>().questLevel.text = questSlot.GetComponent<QuestData>().questLevel.ToString();
        }

        #endregion

        #region Load Side Quest Data

        int sQInt = transitionData.sideQuestData.Length;

        QuestData[] tempSQ0 = new QuestData[sQInt];
        TextMeshProUGUI[] tempSQ1 = new TextMeshProUGUI[sQInt];
        TextMeshProUGUI[] tempSQ2 = new TextMeshProUGUI[sQInt];
        TextMeshProUGUI[] tempSQ3 = new TextMeshProUGUI[sQInt];
        TextMeshProUGUI[] tempSQ4 = new TextMeshProUGUI[sQInt];
        Image[] tempSQ5 = new Image[sQInt];

        questM.sideQuestSlots.slotNum = transitionData.sideQuestSlot;
        questM.sideQuestSlots.questData = new QuestData[sQInt];

        for (int i = 0; i < transitionData.sideQuestData.Length; i++)
        {
            // Create and set Quest Type.
            GameObject questSlot = Instantiate(questSlotPrefab) as GameObject;
            questSlot.GetComponent<QuestSlot>().questType = QuestSlot.QuestType.SideQuest;

            QuestManager.numOfSideQuestSlot++;
            questSlot.GetComponent<QuestSlot>().slotNum = QuestManager.numOfSideQuestSlot - 1;

            tempSQ0[i] = questSlot.GetComponent<QuestData>();
            tempSQ1[i] = questSlot.GetComponent<QuestSlot>().questName;
            tempSQ2[i] = questSlot.GetComponent<QuestSlot>().questLocation;
            tempSQ3[i] = questSlot.GetComponent<QuestSlot>().questDistance;
            tempSQ4[i] = questSlot.GetComponent<QuestSlot>().questLevel;
            tempSQ5[i] = questSlot.GetComponent<QuestSlot>().highLight;

            QuestData[] tempSQ00 = new QuestData[sQInt];
            TextMeshProUGUI[] tempSQ11 = new TextMeshProUGUI[sQInt];
            TextMeshProUGUI[] tempSQ22 = new TextMeshProUGUI[sQInt];
            TextMeshProUGUI[] tempSQ33 = new TextMeshProUGUI[sQInt];
            TextMeshProUGUI[] tempSQ44 = new TextMeshProUGUI[sQInt];
            Image[] tempSQ55 = new Image[sQInt];

            if (questM.sideQuestSlots.questData[0] != null)
            {
                tempSQ00[i] = questM.sideQuestSlots.questData[i];
                tempSQ11[i] = questM.sideQuestSlots.questName[i];
                tempSQ22[i] = questM.sideQuestSlots.questLocationName[i];
                tempSQ33[i] = questM.sideQuestSlots.questDistance[i];
                tempSQ44[i] = questM.sideQuestSlots.questLevel[i];
                tempSQ55[i] = questM.sideQuestSlots.highLight[i];
            }

            questM.sideQuestSlots.questData = tempSQ00;
            questM.sideQuestSlots.questName = tempSQ11;
            questM.sideQuestSlots.questLocationName = tempSQ22;
            questM.sideQuestSlots.questDistance = tempSQ33;
            questM.sideQuestSlots.questLevel = tempSQ44;
            questM.sideQuestSlots.highLight = tempSQ55;

            questM.sideQuestSlots.questData = tempSQ0;
            questM.sideQuestSlots.questName = tempSQ1;
            questM.sideQuestSlots.questLocationName = tempSQ2;
            questM.sideQuestSlots.questDistance = tempSQ3;
            questM.sideQuestSlots.questLevel = tempSQ4;
            questM.sideQuestSlots.highLight = tempSQ5;

            questM.sideQuestSlots.questData[i].questType = (QuestData.QuestType)transitionData.sideQuestData[i].questType;
            questM.sideQuestSlots.questData[i].questName = transitionData.sideQuestData[i].questName;
            questM.sideQuestSlots.questData[i].questLevel = transitionData.sideQuestData[i].questLevel;

            questM.sideQuestSlots.questData[i].EXPReward = transitionData.sideQuestData[i].questEXPReward;
            questM.sideQuestSlots.questData[i].questDescription = transitionData.sideQuestData[i].questDescription;
            questM.sideQuestSlots.questData[i].curObjectivePhase = transitionData.sideQuestData[i].curObjectivePhase;
            questM.sideQuestSlots.questData[i].maxObjectivePhase = transitionData.sideQuestData[i].maxObjectivePhase;

            #region New Side Quest Objective Data

            questM.sideQuestSlots.questData[i].questObjectiveNum = transitionData.sideQuestData[i].questObjectiveNum;

            questM.sideQuestSlots.questData[i].questObjective = new QuestData.QuestObjective[transitionData.sideQuestData[i].questObective.Length];

            for (int sQSize = 0; sQSize < transitionData.sideQuestData[i].questObective.Length; sQSize++)
            {
                // Create New Objective Location.
                GameObject newSideQuestPosPrefab = Instantiate(questObjectivePosPrefab) as GameObject;
                newSideQuestPosPrefab.name = "SideQuestPositionPrefab";

                newSideQuestPosPrefab.GetComponent<SetQuestObjectiveLocation>().phaseNum = sQSize;

                questM.sideQuestSlots.questData[i].questObjective[sQSize].questLocation = newSideQuestPosPrefab.transform;

                for (int ki = 0; ki < newSideQuestPosPrefab.GetComponent<SetQuestObjectiveLocation>().waypoints.Length; ki++)
                {
                    if (questM.sideQuestSlots.questData[i].questName == newSideQuestPosPrefab.GetComponent<SetQuestObjectiveLocation>().waypoints[ki].questSequenceNameReceipt &&
                        sQSize == newSideQuestPosPrefab.GetComponent<SetQuestObjectiveLocation>().waypoints[ki].questPhaseReceipt)
                    {
                        newSideQuestPosPrefab.GetComponent<SetQuestObjectiveLocation>().waypoints[ki].GetComponent<MiniMapItem>().Size = 1;
                        newSideQuestPosPrefab.GetComponent<SetQuestObjectiveLocation>().waypoints[ki].GetComponent<MiniMapItem>().HideCircleArea();
                        questM.sideQuestSlots.questData[i].questObjective[sQSize].questLocation = newSideQuestPosPrefab.GetComponent<SetQuestObjectiveLocation>().waypoints[ki].transform;
                    }
                }

                questM.sideQuestSlots.questData[i].questObjective[sQSize].questLocation.position =
                new Vector3(transitionData.sideQuestData[i].questObective[sQSize].questLocationX,
                transitionData.sideQuestData[i].questObective[sQSize].questLocationY,
                transitionData.sideQuestData[i].questObective[sQSize].questLocationZ);

                questM.sideQuestSlots.questData[i].questObjective[sQSize].questLocationName = transitionData.sideQuestData[i].questObective[sQSize].questLocationName;
                questM.sideQuestSlots.questData[i].questObjective[sQSize].questObjectiveSubject = transitionData.sideQuestData[i].questObective[sQSize].questObjectiveSubject;
                questM.sideQuestSlots.questData[i].questObjective[sQSize].questHint = transitionData.sideQuestData[i].questObective[sQSize].questHint;

                questM.sideQuestSlots.questData[i].questObjective[sQSize].curCollectableAmount = transitionData.sideQuestData[i].questObective[sQSize].curCollectableAmount;
                questM.sideQuestSlots.questData[i].questObjective[sQSize].maxCollectableAmount = transitionData.sideQuestData[i].questObective[sQSize].maxCollectableAmount;
            }

            #endregion

            questM.sideQuestSlots.questData[i].inProgress = transitionData.sideQuestData[i].questInProgress;
            if (questM.sideQuestSlots.questData[i].inProgress) questM.SideQuestSlotSelect(transitionData.sideQuestSlot);
            questM.sideQuestSlots.questData[i].isComplete = transitionData.sideQuestData[i].questIsComplete;

            questSlot.GetComponent<QuestSlot>().questName.text = questSlot.GetComponent<QuestData>().questName;
            questSlot.GetComponent<QuestSlot>().questLocation.text = questSlot.GetComponent<QuestData>().questObjective[questSlot.GetComponent<QuestData>().questObjectiveNum].questLocationName;
            questSlot.GetComponent<QuestSlot>().questLevel.text = questSlot.GetComponent<QuestData>().questLevel.ToString();
        }

        #endregion

        #endregion

        loadingDataNameText.text = transitionData.mainQuestData.Length > 0 || transitionData.sideQuestData.Length > 0 ? "Loading Quest data..." : "";
        yield return new WaitForSeconds(transitionData.mainQuestData.Length > 0 ? 0.4f : 0);
        yield return new WaitForSeconds(transitionData.sideQuestData.Length > 0 ? 0.4f : 0);

        #region Load Inventory Data

        // Remove current slots.
        inventorySlot = FindObjectsOfType<InventorySlot>();
        foreach (InventorySlot items in inventorySlot)
        {
            Destroy(items.gameObject);
        }

        ResetInventory();

        inventoryM.unityCoins = transitionData.unityCoins;

        inventoryM.weaponInv.weaponSlotActive = false;
        inventoryM.bowAndArrowInv.bowAndArrowSlotActive = false;
        inventoryM.shieldInv.shieldSlotActive = false;
        inventoryM.armorInv.armorSlotActive = false;
        inventoryM.materialInv.materialSlotActive = false;
        inventoryM.healingInv.healingSlotActive = false;
        inventoryM.keyInv.keySlotActive = false;

        inventoryM.weaponSlots = transitionData.wpnSlots;
        inventoryM.bowAndArrowSlots = transitionData.bowSlots;
        inventoryM.shieldSlots = transitionData.shdSlots;
        inventoryM.armorSlots = transitionData.armSlots;
        inventoryM.materialSlots = transitionData.matSlots;
        inventoryM.healingSlots = transitionData.healSlots;
        inventoryM.keySlots = transitionData.keySlots;

        inventoryM.SetWeaponSlot(ref transitionData.wpnSlots);
        inventoryM.SetBowAndArrowSlot(ref transitionData.bowSlots);
        inventoryM.SetShieldSlot(ref transitionData.shdSlots);
        inventoryM.SetArmorSlot(ref transitionData.armSlots);
        inventoryM.SetMaterialSlot(ref transitionData.matSlots);
        inventoryM.SetHealingSlot(ref transitionData.healSlots);
        inventoryM.SetKeySlot(ref transitionData.keySlots);

        inventoryM.inventoryS = (InventoryManager.InventorySection)transitionData.inventorySection;

        inventoryM.preSwordEquipped = transitionData.preSwordEquipped;
        inventoryM.pre2HSwordEquipped = transitionData.pre2HSwordEquipped;
        inventoryM.preShieldEquipped = transitionData.preshieldEquipped;
        inventoryM.preSpearEquipped = transitionData.preSpearEquipped;
        inventoryM.preStaffEquipped = transitionData.preStaffEquipped;
        inventoryM.preBowEquipped = transitionData.preBowEquipped;

        loadingDataNameText.text = "Initializing Inventory data...";
        yield return new WaitForSeconds(0.8f);

        #region Load Weapon Inventory Data

        inventoryM.weaponInv.slotNum = transitionData.wpnSlotNum;
        inventoryM.weaponInv.slotWeaponEquipped = transitionData.wpnSlotEquipped;

        inventoryM.weaponInv.itemData = new ItemData[transitionData.wpnData.Length];

        for (int i = 0; i < transitionData.wpnData.Length; i++)
        {
            GameObject emptyitemData = Instantiate(itemDataPrefab) as GameObject;
            inventoryM.weaponInv.itemData[i] = emptyitemData.GetComponent<ItemData>();

            inventoryM.weaponInv.itemData[i].itemType = (ItemData.ItemType)transitionData.wpnData[i].itemType;
            inventoryM.weaponInv.itemData[i].itemName = transitionData.wpnData[i].itemName;
            inventoryM.weaponInv.itemData[i].itemDescription = transitionData.wpnData[i].itemDescription;

            inventoryM.weaponInv.itemData[i].wpnAtk.value = transitionData.wpnData[i].wpnAtkStat.value;
            inventoryM.weaponInv.itemData[i].wpnAtk.rank1 = transitionData.wpnData[i].wpnAtkStat.rank1;
            inventoryM.weaponInv.itemData[i].wpnAtk.rank2 = transitionData.wpnData[i].wpnAtkStat.rank2;
            inventoryM.weaponInv.itemData[i].wpnAtk.rank3 = transitionData.wpnData[i].wpnAtkStat.rank3;

            inventoryM.weaponInv.itemData[i].wpnSpd.value = transitionData.wpnData[i].wpnSpdStat.value;
            inventoryM.weaponInv.itemData[i].wpnSpd.rank1 = transitionData.wpnData[i].wpnSpdStat.rank1;
            inventoryM.weaponInv.itemData[i].wpnSpd.rank2 = transitionData.wpnData[i].wpnSpdStat.rank2;
            inventoryM.weaponInv.itemData[i].wpnSpd.rank3 = transitionData.wpnData[i].wpnSpdStat.rank3;

            inventoryM.weaponInv.itemData[i].wpnStun.value = transitionData.wpnData[i].wpnStunStat.value;
            inventoryM.weaponInv.itemData[i].wpnStun.rank1 = transitionData.wpnData[i].wpnStunStat.rank1;
            inventoryM.weaponInv.itemData[i].wpnStun.rank2 = transitionData.wpnData[i].wpnStunStat.rank2;
            inventoryM.weaponInv.itemData[i].wpnStun.rank3 = transitionData.wpnData[i].wpnStunStat.rank3;

            inventoryM.weaponInv.itemData[i].wpnCritHit.value = transitionData.wpnData[i].wpnCritHitStat.value;
            inventoryM.weaponInv.itemData[i].wpnCritHit.rank1 = transitionData.wpnData[i].wpnCritHitStat.rank1;
            inventoryM.weaponInv.itemData[i].wpnCritHit.rank2 = transitionData.wpnData[i].wpnCritHitStat.rank2;
            inventoryM.weaponInv.itemData[i].wpnCritHit.rank3 = transitionData.wpnData[i].wpnCritHitStat.rank3;

            inventoryM.weaponInv.itemData[i].wpnDura.value = transitionData.wpnData[i].wpnDuraStat.value;
            inventoryM.weaponInv.itemData[i].wpnDura.rank1 = transitionData.wpnData[i].wpnDuraStat.rank1;
            inventoryM.weaponInv.itemData[i].wpnDura.rank2 = transitionData.wpnData[i].wpnDuraStat.rank2;
            inventoryM.weaponInv.itemData[i].wpnDura.rank3 = transitionData.wpnData[i].wpnDuraStat.rank3;

            inventoryM.weaponInv.itemData[i].throwDist.throwRange = (ItemData.ThrowRange)transitionData.wpnData[i].throwDistance.throwRange;

            inventoryM.weaponInv.itemData[i].wpnSpecialStat1 = (ItemData.WeaponSpecialStat1)transitionData.wpnData[i].wpnSpecialStat1;
            inventoryM.weaponInv.itemData[i].wpnSpecialStat2 = (ItemData.WeaponSpecialStat2)transitionData.wpnData[i].wpnSpecialStat2;

            inventoryM.weaponInv.itemData[i].wpnWeight = transitionData.wpnData[i].weight;
            inventoryM.weaponInv.itemData[i].weaponArmsID = transitionData.wpnData[i].weaponArmsID;

            inventoryM.weaponInv.itemData[i].rankNum = transitionData.wpnData[i].rankNum;

            inventoryM.weaponInv.itemData[i].upgradeMaterial = new ItemData.Material[transitionData.wpnData[i].upgradeMaterial.Length];

            for (int matSize = 0; matSize < transitionData.wpnData[i].upgradeMaterial.Length; matSize++)
            {
                inventoryM.weaponInv.itemData[i].upgradeMaterial[matSize].matName = transitionData.wpnData[i].upgradeMaterial[matSize].matName;
                inventoryM.weaponInv.itemData[i].upgradeMaterial[matSize].matRequired = transitionData.wpnData[i].upgradeMaterial[matSize].matRequired;
            }

            inventoryM.weaponInv.itemData[i].buyPrice = transitionData.wpnData[i].buyPrice;
            inventoryM.weaponInv.itemData[i].sellPrice = transitionData.wpnData[i].sellPrice;
            inventoryM.weaponInv.itemData[i].repairPrice = transitionData.wpnData[i].repairPrice;
            inventoryM.weaponInv.itemData[i].upgradePrice = transitionData.wpnData[i].upgradePrice;
            inventoryM.weaponInv.itemData[i].specialPrice = transitionData.wpnData[i].specialPrice;
            inventoryM.weaponInv.itemData[i].inStockQuantity = transitionData.wpnData[i].inStockQuantity;

            inventoryM.weaponInv.counter[i] = transitionData.wpnData[i].quantity;
            inventoryM.weaponInv.itemData[i].stackable = transitionData.wpnData[i].stackable;
            inventoryM.weaponInv.itemData[i].inInventory = transitionData.wpnData[i].inInventory;
            inventoryM.weaponInv.itemData[i].equipped = transitionData.wpnData[i].equipped;

            inventoryM.weaponInv.itemData[i].broken = transitionData.wpnData[i].broken;
            inventoryM.weaponInv.itemData[i].rankTag = transitionData.wpnData[i].rankTag;
            inventoryM.weaponInv.itemData[i].originItemName = transitionData.wpnData[i].originItemName;
            inventoryM.weaponInv.itemData[i].numOfEngraving = transitionData.wpnData[i].numOfEngraving;

            inventoryM.weaponInv.itemData[i].maxWpnAtk = transitionData.wpnData[i].maxWpnAtk;
            inventoryM.weaponInv.itemData[i].maxWpnSpd = transitionData.wpnData[i].maxWpnSpd;
            inventoryM.weaponInv.itemData[i].maxWpnStun = transitionData.wpnData[i].maxWpnStun;
            inventoryM.weaponInv.itemData[i].maxWpnCritHit = transitionData.wpnData[i].maxWpnCritHit;
            inventoryM.weaponInv.itemData[i].maxWpnDura = transitionData.wpnData[i].maxWpnDura;

            inventoryM.weaponInv.image[i].type = Image.Type.Simple;
            inventoryM.weaponInv.image[i].preserveAspect = true;
            Navigation automatic = new Navigation { mode = Navigation.Mode.Automatic };
            inventoryM.weaponInv.image[i].GetComponent<Button>().navigation = automatic;
            inventoryM.weaponInv.image[i].enabled = true;

            #region Set Inventory BG Color

            switch (transitionData.wpnData[i].rankNum)
            {
                case 0:
                    inventoryM.weaponInv.highLight[i].GetComponent<UIGradient>().enabled = false;
                    inventoryM.weaponInv.highLight[i].color = new Color(0, 0, 0, 80);
                    break;
                case 1:
                    inventoryM.weaponInv.highLight[i].GetComponent<UIGradient>().enabled = true;
                    inventoryM.weaponInv.highLight[i].GetComponent<UIGradient>().LinearColor1 = inventoryM.rank1Color1;
                    inventoryM.weaponInv.highLight[i].GetComponent<UIGradient>().LinearColor2 = inventoryM.rank1Color2;
                    break;
                case 2:
                    inventoryM.weaponInv.highLight[i].GetComponent<UIGradient>().enabled = true;
                    inventoryM.weaponInv.highLight[i].GetComponent<UIGradient>().LinearColor1 = inventoryM.rank2Color1;
                    inventoryM.weaponInv.highLight[i].GetComponent<UIGradient>().LinearColor2 = inventoryM.rank2Color2;
                    break;
                case 3:
                    inventoryM.weaponInv.highLight[i].GetComponent<UIGradient>().enabled = true;
                    inventoryM.weaponInv.highLight[i].GetComponent<UIGradient>().LinearColor1 = inventoryM.rank3Color1;
                    inventoryM.weaponInv.highLight[i].GetComponent<UIGradient>().LinearColor2 = inventoryM.rank3Color2;
                    break;
            }

            if (transitionData.wpnData[i].broken)
            {
                inventoryM.weaponInv.highLight[i].GetComponent<UIGradient>().enabled = true;
                inventoryM.weaponInv.highLight[i].GetComponent<UIGradient>().LinearColor1 = inventoryM.brokenColor1;
                inventoryM.weaponInv.highLight[i].GetComponent<UIGradient>().LinearColor2 = inventoryM.brokenColor2;
            }

            #endregion

            if (inventoryM.weaponInv.itemData[i].equipped)
            {
                inventoryM.weaponInv.outLineBorder[i].enabled = true;
                inventoryM.weaponInv.outLineBorder[i].color = inventoryM.outLineSelected;
            }

            if (!inventoryM.weaponInv.itemData[i].stackable)
            {
                inventoryM.weaponInv.counter[i] = 1;
                inventoryM.weaponInv.image[i].enabled = true;
            }
            else
            {
                inventoryM.weaponInv.image[i].enabled = true;
                inventoryM.weaponInv.quantity[i].enabled = true;
                inventoryM.weaponInv.counter[i] = transitionData.wpnData[i].quantity;
                inventoryM.weaponInv.quantity[i].text = "x" + inventoryM.weaponInv.counter[i].ToString();
            }
            SetLoadedSprite(ref inventoryM.weaponInv.itemData[i], ref inventoryM.weaponInv.image[i]);
        }

        #endregion

        loadingDataNameText.text = "Loading Weapon slots data...";
        yield return new WaitForSeconds(0.8f);

        #region Load Bow And Arrow Inventory Data

        inventoryM.bowAndArrowInv.slotNum = transitionData.bowSlotNum;
        inventoryM.bowAndArrowInv.slotBowEquipped = transitionData.bowSlotEquipped;
        inventoryM.bowAndArrowInv.slotArrowEquipped = transitionData.arrowSlotEquipped;

        inventoryM.bowAndArrowInv.itemData = new ItemData[transitionData.bowData.Length];
        for (int i = 0; i < transitionData.bowData.Length; i++)
        {
            GameObject emptyitemData = Instantiate(itemDataPrefab) as GameObject;
            inventoryM.bowAndArrowInv.itemData[i] = emptyitemData.GetComponent<ItemData>();

            inventoryM.bowAndArrowInv.itemData[i].itemType = (ItemData.ItemType)transitionData.bowData[i].itemType;
            inventoryM.bowAndArrowInv.itemData[i].itemName = transitionData.bowData[i].itemName;
            inventoryM.bowAndArrowInv.itemData[i].itemDescription = transitionData.bowData[i].itemDescription;

            inventoryM.bowAndArrowInv.itemData[i].bowAtk.value = transitionData.bowData[i].bowAtkStat.value;
            inventoryM.bowAndArrowInv.itemData[i].bowAtk.rank1 = transitionData.bowData[i].bowAtkStat.rank1;
            inventoryM.bowAndArrowInv.itemData[i].bowAtk.rank2 = transitionData.bowData[i].bowAtkStat.rank2;
            inventoryM.bowAndArrowInv.itemData[i].bowAtk.rank3 = transitionData.bowData[i].bowAtkStat.rank3;

            inventoryM.bowAndArrowInv.itemData[i].bowSpd.value = transitionData.bowData[i].bowSpdStat.value;
            inventoryM.bowAndArrowInv.itemData[i].bowSpd.rank1 = transitionData.bowData[i].bowSpdStat.rank1;
            inventoryM.bowAndArrowInv.itemData[i].bowSpd.rank2 = transitionData.bowData[i].bowSpdStat.rank2;
            inventoryM.bowAndArrowInv.itemData[i].bowSpd.rank3 = transitionData.bowData[i].bowSpdStat.rank3;

            inventoryM.bowAndArrowInv.itemData[i].bowStun.value = transitionData.bowData[i].bowStunStat.value;
            inventoryM.bowAndArrowInv.itemData[i].bowStun.rank1 = transitionData.bowData[i].bowStunStat.rank1;
            inventoryM.bowAndArrowInv.itemData[i].bowStun.rank2 = transitionData.bowData[i].bowStunStat.rank2;
            inventoryM.bowAndArrowInv.itemData[i].bowStun.rank3 = transitionData.bowData[i].bowStunStat.rank3;

            inventoryM.bowAndArrowInv.itemData[i].bowCritHit.value = transitionData.bowData[i].bowCritHitStat.value;
            inventoryM.bowAndArrowInv.itemData[i].bowCritHit.rank1 = transitionData.bowData[i].bowCritHitStat.rank1;
            inventoryM.bowAndArrowInv.itemData[i].bowCritHit.rank2 = transitionData.bowData[i].bowCritHitStat.rank2;
            inventoryM.bowAndArrowInv.itemData[i].bowCritHit.rank3 = transitionData.bowData[i].bowCritHitStat.rank3;

            inventoryM.bowAndArrowInv.itemData[i].bowHdShot.value = transitionData.bowData[i].bowHdShotStat.value;
            inventoryM.bowAndArrowInv.itemData[i].bowHdShot.rank1 = transitionData.bowData[i].bowHdShotStat.rank1;
            inventoryM.bowAndArrowInv.itemData[i].bowHdShot.rank2 = transitionData.bowData[i].bowHdShotStat.rank2;
            inventoryM.bowAndArrowInv.itemData[i].bowHdShot.rank3 = transitionData.bowData[i].bowHdShotStat.rank3;

            inventoryM.bowAndArrowInv.itemData[i].bowDura.value = transitionData.bowData[i].bowDuraStat.value;
            inventoryM.bowAndArrowInv.itemData[i].bowDura.rank1 = transitionData.bowData[i].bowDuraStat.rank1;
            inventoryM.bowAndArrowInv.itemData[i].bowDura.rank2 = transitionData.bowData[i].bowDuraStat.rank2;
            inventoryM.bowAndArrowInv.itemData[i].bowDura.rank3 = transitionData.bowData[i].bowDuraStat.rank3;

            inventoryM.bowAndArrowInv.itemData[i].bowRange.bowRangeType = (ItemData.BowRangeType)transitionData.bowData[i].bowRange.bowRangeType;

            inventoryM.bowAndArrowInv.itemData[i].bowSpecialStat1 = (ItemData.BowSpecialStat1)transitionData.bowData[i].bowSpecialStat1;
            inventoryM.bowAndArrowInv.itemData[i].bowSpecialStat2 = (ItemData.BowSpecialStat2)transitionData.bowData[i].bowSpecialStat2;

            inventoryM.bowAndArrowInv.itemData[i].bowWeight = transitionData.bowData[i].weight;
            inventoryM.bowAndArrowInv.itemData[i].weaponArmsID = transitionData.bowData[i].weaponArmsID;

            inventoryM.bowAndArrowInv.itemData[i].rankNum = transitionData.bowData[i].rankNum;

            inventoryM.bowAndArrowInv.itemData[i].upgradeMaterial = new ItemData.Material[transitionData.bowData[i].upgradeMaterial.Length];
            for (int matSize = 0; matSize < transitionData.bowData[i].upgradeMaterial.Length; matSize++)
            {
                inventoryM.bowAndArrowInv.itemData[i].upgradeMaterial[matSize].matName = transitionData.bowData[i].upgradeMaterial[matSize].matName;
                inventoryM.bowAndArrowInv.itemData[i].upgradeMaterial[matSize].matRequired = transitionData.bowData[i].upgradeMaterial[matSize].matRequired;
            }

            inventoryM.bowAndArrowInv.itemData[i].buyPrice = transitionData.bowData[i].buyPrice;
            inventoryM.bowAndArrowInv.itemData[i].sellPrice = transitionData.bowData[i].sellPrice;
            inventoryM.bowAndArrowInv.itemData[i].repairPrice = transitionData.bowData[i].repairPrice;
            inventoryM.bowAndArrowInv.itemData[i].upgradePrice = transitionData.bowData[i].upgradePrice;
            inventoryM.bowAndArrowInv.itemData[i].specialPrice = transitionData.bowData[i].specialPrice;
            inventoryM.bowAndArrowInv.itemData[i].inStockQuantity = transitionData.bowData[i].inStockQuantity;

            inventoryM.bowAndArrowInv.counter[i] = transitionData.bowData[i].quantity;
            inventoryM.bowAndArrowInv.itemData[i].stackable = transitionData.bowData[i].stackable;
            inventoryM.bowAndArrowInv.itemData[i].inInventory = transitionData.bowData[i].inInventory;
            inventoryM.bowAndArrowInv.itemData[i].equipped = transitionData.bowData[i].equipped;

            inventoryM.bowAndArrowInv.itemData[i].broken = transitionData.bowData[i].broken;
            inventoryM.bowAndArrowInv.itemData[i].rankTag = transitionData.bowData[i].rankTag;
            inventoryM.bowAndArrowInv.itemData[i].originItemName = transitionData.bowData[i].originItemName;
            inventoryM.bowAndArrowInv.itemData[i].numOfEngraving = transitionData.bowData[i].numOfEngraving;

            inventoryM.bowAndArrowInv.itemData[i].maxBowAtk = transitionData.bowData[i].maxBowAtk;
            inventoryM.bowAndArrowInv.itemData[i].maxBowSpd = transitionData.bowData[i].maxBowSpd;
            inventoryM.bowAndArrowInv.itemData[i].maxBowStun = transitionData.bowData[i].maxBowStun;
            inventoryM.bowAndArrowInv.itemData[i].maxBowCritHit = transitionData.bowData[i].maxBowCritHit;
            inventoryM.bowAndArrowInv.itemData[i].maxBowHdShot = transitionData.bowData[i].maxBowHdShot;
            inventoryM.bowAndArrowInv.itemData[i].maxBowDura = transitionData.bowData[i].maxBowDura;

            inventoryM.bowAndArrowInv.image[i].type = Image.Type.Simple;
            inventoryM.bowAndArrowInv.image[i].preserveAspect = true;
            Navigation automatic = new Navigation { mode = Navigation.Mode.Automatic };
            inventoryM.bowAndArrowInv.image[i].GetComponent<Button>().navigation = automatic;
            inventoryM.bowAndArrowInv.image[i].enabled = true;

            #region Set Inventory BG Color

            switch (transitionData.bowData[i].rankNum)
            {
                case 0:
                    inventoryM.bowAndArrowInv.highLight[i].GetComponent<UIGradient>().enabled = false;
                    inventoryM.bowAndArrowInv.highLight[i].color = new Color(0, 0, 0, 80);
                    break;
                case 1:
                    inventoryM.bowAndArrowInv.highLight[i].GetComponent<UIGradient>().enabled = true;
                    inventoryM.bowAndArrowInv.highLight[i].GetComponent<UIGradient>().LinearColor1 = inventoryM.rank1Color1;
                    inventoryM.bowAndArrowInv.highLight[i].GetComponent<UIGradient>().LinearColor2 = inventoryM.rank1Color2;
                    break;
                case 2:
                    inventoryM.bowAndArrowInv.highLight[i].GetComponent<UIGradient>().enabled = true;
                    inventoryM.bowAndArrowInv.highLight[i].GetComponent<UIGradient>().LinearColor1 = inventoryM.rank2Color1;
                    inventoryM.bowAndArrowInv.highLight[i].GetComponent<UIGradient>().LinearColor2 = inventoryM.rank2Color2;
                    break;
                case 3:
                    inventoryM.bowAndArrowInv.highLight[i].GetComponent<UIGradient>().enabled = true;
                    inventoryM.bowAndArrowInv.highLight[i].GetComponent<UIGradient>().LinearColor1 = inventoryM.rank3Color1;
                    inventoryM.bowAndArrowInv.highLight[i].GetComponent<UIGradient>().LinearColor2 = inventoryM.rank3Color2;
                    break;
            }

            if (transitionData.bowData[i].broken)
            {
                inventoryM.bowAndArrowInv.highLight[i].GetComponent<UIGradient>().enabled = true;
                inventoryM.bowAndArrowInv.highLight[i].GetComponent<UIGradient>().LinearColor1 = inventoryM.brokenColor1;
                inventoryM.bowAndArrowInv.highLight[i].GetComponent<UIGradient>().LinearColor2 = inventoryM.brokenColor2;
            }

            #endregion

            if (inventoryM.bowAndArrowInv.itemData[i].equipped)
            {
                inventoryM.bowAndArrowInv.outLineBorder[i].enabled = true;
                inventoryM.bowAndArrowInv.outLineBorder[i].color = inventoryM.outLineSelected;
            }

            if (!inventoryM.bowAndArrowInv.itemData[i].stackable)
            {
                inventoryM.bowAndArrowInv.counter[i] = 1;
                inventoryM.bowAndArrowInv.image[i].enabled = true;
            }
            else
            {
                inventoryM.bowAndArrowInv.image[i].enabled = true;
                inventoryM.bowAndArrowInv.quantity[i].enabled = true;
                inventoryM.bowAndArrowInv.counter[i] = transitionData.bowData[i].quantity;
                inventoryM.bowAndArrowInv.quantity[i].text = "x" + inventoryM.bowAndArrowInv.counter[i].ToString();
            }
            SetLoadedSprite(ref inventoryM.bowAndArrowInv.itemData[i], ref inventoryM.bowAndArrowInv.image[i]);
        }

        #endregion

        loadingDataNameText.text = "Loading Bow and Arrow slots data...";
        yield return new WaitForSeconds(0.8f);

        #region Load Shield Inventory Data

        inventoryM.shieldInv.slotNum = transitionData.shieldSlotNum;
        inventoryM.shieldInv.slotShieldEquipped = transitionData.shieldSlotEquipped;

        inventoryM.shieldInv.itemData = new ItemData[transitionData.shdData.Length];
        for (int i = 0; i < transitionData.shdData.Length; i++)
        {
            GameObject emptyitemData = Instantiate(itemDataPrefab) as GameObject;
            inventoryM.shieldInv.itemData[i] = emptyitemData.GetComponent<ItemData>();

            inventoryM.shieldInv.itemData[i].itemType = (ItemData.ItemType)transitionData.shdData[i].itemType;
            inventoryM.shieldInv.itemData[i].itemName = transitionData.shdData[i].itemName;
            inventoryM.shieldInv.itemData[i].itemDescription = transitionData.shdData[i].itemDescription;

            inventoryM.shieldInv.itemData[i].shdAtk.value = transitionData.shdData[i].shdAtkStat.value;
            inventoryM.shieldInv.itemData[i].shdAtk.rank1 = transitionData.shdData[i].shdAtkStat.rank1;
            inventoryM.shieldInv.itemData[i].shdAtk.rank2 = transitionData.shdData[i].shdAtkStat.rank2;
            inventoryM.shieldInv.itemData[i].shdAtk.rank3 = transitionData.shdData[i].shdAtkStat.rank3;

            inventoryM.shieldInv.itemData[i].shdSpd.value = transitionData.shdData[i].shdSpdStat.value;
            inventoryM.shieldInv.itemData[i].shdSpd.rank1 = transitionData.shdData[i].shdSpdStat.rank1;
            inventoryM.shieldInv.itemData[i].shdSpd.rank2 = transitionData.shdData[i].shdSpdStat.rank2;
            inventoryM.shieldInv.itemData[i].shdSpd.rank3 = transitionData.shdData[i].shdSpdStat.rank3;

            inventoryM.shieldInv.itemData[i].shdStun.value = transitionData.shdData[i].shdStunStat.value;
            inventoryM.shieldInv.itemData[i].shdStun.rank1 = transitionData.shdData[i].shdStunStat.rank1;
            inventoryM.shieldInv.itemData[i].shdStun.rank2 = transitionData.shdData[i].shdStunStat.rank2;
            inventoryM.shieldInv.itemData[i].shdStun.rank3 = transitionData.shdData[i].shdStunStat.rank3;

            inventoryM.shieldInv.itemData[i].shdCritHit.value = transitionData.shdData[i].shdCritHitStat.value;
            inventoryM.shieldInv.itemData[i].shdCritHit.rank1 = transitionData.shdData[i].shdCritHitStat.rank1;
            inventoryM.shieldInv.itemData[i].shdCritHit.rank2 = transitionData.shdData[i].shdCritHitStat.rank2;
            inventoryM.shieldInv.itemData[i].shdCritHit.rank3 = transitionData.shdData[i].shdCritHitStat.rank3;

            inventoryM.shieldInv.itemData[i].shdBlock.value = transitionData.shdData[i].shdBlockStat.value;
            inventoryM.shieldInv.itemData[i].shdBlock.rank1 = transitionData.shdData[i].shdBlockStat.rank1;
            inventoryM.shieldInv.itemData[i].shdBlock.rank2 = transitionData.shdData[i].shdBlockStat.rank2;
            inventoryM.shieldInv.itemData[i].shdBlock.rank3 = transitionData.shdData[i].shdBlockStat.rank3;

            inventoryM.shieldInv.itemData[i].shdDura.value = transitionData.shdData[i].shdDuraStat.value;
            inventoryM.shieldInv.itemData[i].shdDura.rank1 = transitionData.shdData[i].shdDuraStat.rank1;
            inventoryM.shieldInv.itemData[i].shdDura.rank2 = transitionData.shdData[i].shdDuraStat.rank2;
            inventoryM.shieldInv.itemData[i].shdDura.rank3 = transitionData.shdData[i].shdDuraStat.rank3;

            inventoryM.shieldInv.itemData[i].shdSpecialStat1 = (ItemData.ShieldSpecialStat1)transitionData.shdData[i].shdSpecialStat1;
            inventoryM.shieldInv.itemData[i].shdSpecialStat2 = (ItemData.ShieldSpecialStat2)transitionData.shdData[i].shdSpecialStat2;

            inventoryM.shieldInv.itemData[i].bowWeight = transitionData.shdData[i].weight;
            inventoryM.shieldInv.itemData[i].weaponArmsID = transitionData.shdData[i].weaponArmsID;

            inventoryM.shieldInv.itemData[i].rankNum = transitionData.shdData[i].rankNum;

            inventoryM.shieldInv.itemData[i].upgradeMaterial = new ItemData.Material[transitionData.shdData[i].upgradeMaterial.Length];
            for (int matSize = 0; matSize < transitionData.shdData[i].upgradeMaterial.Length; matSize++)
            {
                inventoryM.shieldInv.itemData[i].upgradeMaterial[matSize].matName = transitionData.shdData[i].upgradeMaterial[matSize].matName;
                inventoryM.shieldInv.itemData[i].upgradeMaterial[matSize].matRequired = transitionData.shdData[i].upgradeMaterial[matSize].matRequired;
            }

            inventoryM.shieldInv.itemData[i].buyPrice = transitionData.shdData[i].buyPrice;
            inventoryM.shieldInv.itemData[i].sellPrice = transitionData.shdData[i].sellPrice;
            inventoryM.shieldInv.itemData[i].repairPrice = transitionData.shdData[i].repairPrice;
            inventoryM.shieldInv.itemData[i].upgradePrice = transitionData.shdData[i].upgradePrice;
            inventoryM.shieldInv.itemData[i].specialPrice = transitionData.shdData[i].specialPrice;
            inventoryM.shieldInv.itemData[i].inStockQuantity = transitionData.shdData[i].inStockQuantity;

            inventoryM.shieldInv.counter[i] = transitionData.shdData[i].quantity;
            inventoryM.shieldInv.itemData[i].stackable = transitionData.shdData[i].stackable;
            inventoryM.shieldInv.itemData[i].inInventory = transitionData.shdData[i].inInventory;
            inventoryM.shieldInv.itemData[i].equipped = transitionData.shdData[i].equipped;

            inventoryM.shieldInv.itemData[i].broken = transitionData.shdData[i].broken;
            inventoryM.shieldInv.itemData[i].rankTag = transitionData.shdData[i].rankTag;
            inventoryM.shieldInv.itemData[i].originItemName = transitionData.shdData[i].originItemName;
            inventoryM.shieldInv.itemData[i].numOfEngraving = transitionData.shdData[i].numOfEngraving;

            inventoryM.shieldInv.itemData[i].maxBowAtk = transitionData.shdData[i].maxBowAtk;
            inventoryM.shieldInv.itemData[i].maxBowSpd = transitionData.shdData[i].maxBowSpd;
            inventoryM.shieldInv.itemData[i].maxBowStun = transitionData.shdData[i].maxBowStun;
            inventoryM.shieldInv.itemData[i].maxBowCritHit = transitionData.shdData[i].maxBowCritHit;
            inventoryM.shieldInv.itemData[i].maxBowHdShot = transitionData.shdData[i].maxBowHdShot;
            inventoryM.shieldInv.itemData[i].maxBowDura = transitionData.shdData[i].maxBowDura;

            inventoryM.shieldInv.image[i].type = Image.Type.Simple;
            inventoryM.shieldInv.image[i].preserveAspect = true;
            Navigation automatic = new Navigation { mode = Navigation.Mode.Automatic };
            inventoryM.shieldInv.image[i].GetComponent<Button>().navigation = automatic;
            inventoryM.shieldInv.image[i].enabled = true;

            #region Set Inventory BG Color

            switch (transitionData.shdData[i].rankNum)
            {
                case 0:
                    inventoryM.shieldInv.highLight[i].GetComponent<UIGradient>().enabled = false;
                    inventoryM.shieldInv.highLight[i].color = new Color(0, 0, 0, 80);
                    break;
                case 1:
                    inventoryM.shieldInv.highLight[i].GetComponent<UIGradient>().enabled = true;
                    inventoryM.shieldInv.highLight[i].GetComponent<UIGradient>().LinearColor1 = inventoryM.rank1Color1;
                    inventoryM.shieldInv.highLight[i].GetComponent<UIGradient>().LinearColor2 = inventoryM.rank1Color2;
                    break;
                case 2:
                    inventoryM.shieldInv.highLight[i].GetComponent<UIGradient>().enabled = true;
                    inventoryM.shieldInv.highLight[i].GetComponent<UIGradient>().LinearColor1 = inventoryM.rank2Color1;
                    inventoryM.shieldInv.highLight[i].GetComponent<UIGradient>().LinearColor2 = inventoryM.rank2Color2;
                    break;
                case 3:
                    inventoryM.shieldInv.highLight[i].GetComponent<UIGradient>().enabled = true;
                    inventoryM.shieldInv.highLight[i].GetComponent<UIGradient>().LinearColor1 = inventoryM.rank3Color1;
                    inventoryM.shieldInv.highLight[i].GetComponent<UIGradient>().LinearColor2 = inventoryM.rank3Color2;
                    break;
            }

            if (transitionData.shdData[i].broken)
            {
                inventoryM.shieldInv.highLight[i].GetComponent<UIGradient>().enabled = true;
                inventoryM.shieldInv.highLight[i].GetComponent<UIGradient>().LinearColor1 = inventoryM.brokenColor1;
                inventoryM.shieldInv.highLight[i].GetComponent<UIGradient>().LinearColor2 = inventoryM.brokenColor2;
            }

            #endregion

            if (inventoryM.shieldInv.itemData[i].equipped)
            {
                inventoryM.shieldInv.outLineBorder[i].enabled = true;
                inventoryM.shieldInv.outLineBorder[i].color = inventoryM.outLineSelected;
            }

            if (!inventoryM.shieldInv.itemData[i].stackable)
            {
                inventoryM.shieldInv.counter[i] = 1;
                inventoryM.shieldInv.image[i].enabled = true;
            }
            else
            {
                inventoryM.shieldInv.image[i].enabled = true;
                inventoryM.shieldInv.quantity[i].enabled = true;
                inventoryM.shieldInv.counter[i] = transitionData.shdData[i].quantity;
                inventoryM.shieldInv.quantity[i].text = "x" + inventoryM.shieldInv.counter[i].ToString();
            }
            SetLoadedSprite(ref inventoryM.shieldInv.itemData[i], ref inventoryM.shieldInv.image[i]);
        }

        #endregion

        loadingDataNameText.text = "Loading Shield slots Data...";
        yield return new WaitForSeconds(0.8f);

        #region Load Armor Inventory Data

        inventoryM.armorInv.slotNum = transitionData.armSlotNum;
        inventoryM.armorInv.slotArmorHeadEquipped = transitionData.armSlotHeadEquipped;
        inventoryM.armorInv.slotArmorChestEquipped = transitionData.armSlotChestEquipped;
        inventoryM.armorInv.slotArmorAmuletEquipped = transitionData.armSlotAmuletEquipped;

        inventoryM.armorInv.itemData = new ItemData[transitionData.armData.Length];
        for (int i = 0; i < transitionData.armData.Length; i++)
        {
            GameObject emptyitemData = Instantiate(itemDataPrefab) as GameObject;
            inventoryM.armorInv.itemData[i] = emptyitemData.GetComponent<ItemData>();

            inventoryM.armorInv.itemData[i].itemType = (ItemData.ItemType)transitionData.armData[i].itemType;
            inventoryM.armorInv.itemData[i].itemName = transitionData.armData[i].itemName;
            inventoryM.armorInv.itemData[i].itemDescription = transitionData.armData[i].itemDescription;

            inventoryM.armorInv.itemData[i].arm.value = transitionData.armData[i].armStat.value;
            inventoryM.armorInv.itemData[i].arm.rank1 = transitionData.armData[i].armStat.rank1;
            inventoryM.armorInv.itemData[i].arm.rank2 = transitionData.armData[i].armStat.rank2;
            inventoryM.armorInv.itemData[i].arm.rank3 = transitionData.armData[i].armStat.rank3;

            inventoryM.armorInv.itemData[i].armLRes.value = transitionData.armData[i].armLResStat.value;
            inventoryM.armorInv.itemData[i].armLRes.rank1 = transitionData.armData[i].armLResStat.rank1;
            inventoryM.armorInv.itemData[i].armLRes.rank2 = transitionData.armData[i].armLResStat.rank2;
            inventoryM.armorInv.itemData[i].armLRes.rank3 = transitionData.armData[i].armLResStat.rank3;

            inventoryM.armorInv.itemData[i].armHRes.value = transitionData.armData[i].armHResStat.value;
            inventoryM.armorInv.itemData[i].armHRes.rank1 = transitionData.armData[i].armHResStat.rank1;
            inventoryM.armorInv.itemData[i].armHRes.rank2 = transitionData.armData[i].armHResStat.rank2;
            inventoryM.armorInv.itemData[i].armHRes.rank3 = transitionData.armData[i].armHResStat.rank3;

            inventoryM.armorInv.itemData[i].armSpecialStat1 = (ItemData.ArmorSpecialStat1)transitionData.armData[i].armSpecialStat1;
            inventoryM.armorInv.itemData[i].armSpecialStat2 = (ItemData.ArmorSpecialStat2)transitionData.armData[i].armSpecialStat2;

            inventoryM.armorInv.itemData[i].bowWeight = transitionData.armData[i].weight;

            inventoryM.armorInv.itemData[i].rankNum = transitionData.armData[i].rankNum;

            inventoryM.armorInv.itemData[i].upgradeMaterial = new ItemData.Material[transitionData.armData[i].upgradeMaterial.Length];
            for (int matSize = 0; matSize < transitionData.armData[i].upgradeMaterial.Length; matSize++)
            {
                inventoryM.armorInv.itemData[i].upgradeMaterial[matSize].matName = transitionData.armData[i].upgradeMaterial[matSize].matName;
                inventoryM.armorInv.itemData[i].upgradeMaterial[matSize].matRequired = transitionData.armData[i].upgradeMaterial[matSize].matRequired;
            }

            inventoryM.armorInv.itemData[i].buyPrice = transitionData.armData[i].buyPrice;
            inventoryM.armorInv.itemData[i].sellPrice = transitionData.armData[i].sellPrice;
            inventoryM.armorInv.itemData[i].repairPrice = transitionData.armData[i].repairPrice;
            inventoryM.armorInv.itemData[i].upgradePrice = transitionData.armData[i].upgradePrice;
            inventoryM.armorInv.itemData[i].specialPrice = transitionData.armData[i].specialPrice;
            inventoryM.armorInv.itemData[i].inStockQuantity = transitionData.armData[i].inStockQuantity;

            inventoryM.armorInv.counter[i] = transitionData.armData[i].quantity;
            inventoryM.armorInv.itemData[i].stackable = transitionData.armData[i].stackable;
            inventoryM.armorInv.itemData[i].inInventory = transitionData.armData[i].inInventory;
            inventoryM.armorInv.itemData[i].equipped = transitionData.armData[i].equipped;

            inventoryM.armorInv.itemData[i].broken = transitionData.armData[i].broken;
            inventoryM.armorInv.itemData[i].rankTag = transitionData.armData[i].rankTag;
            inventoryM.armorInv.itemData[i].originItemName = transitionData.armData[i].originItemName;
            inventoryM.armorInv.itemData[i].numOfEngraving = transitionData.armData[i].numOfEngraving;

            inventoryM.armorInv.itemData[i].maxArm = transitionData.armData[i].maxArm;
            inventoryM.armorInv.itemData[i].maxArmLRes = transitionData.armData[i].maxArmLRes;
            inventoryM.armorInv.itemData[i].maxArmHRes = transitionData.armData[i].maxArmHRes;

            inventoryM.armorInv.image[i].type = Image.Type.Simple;
            inventoryM.armorInv.image[i].preserveAspect = true;
            Navigation automatic = new Navigation { mode = Navigation.Mode.Automatic };
            inventoryM.armorInv.image[i].GetComponent<Button>().navigation = automatic;
            inventoryM.armorInv.image[i].enabled = true;

            #region Set Inventory BG Color

            switch (transitionData.armData[i].rankNum)
            {
                case 0:
                    inventoryM.armorInv.highLight[i].GetComponent<UIGradient>().enabled = false;
                    inventoryM.armorInv.highLight[i].color = new Color(0, 0, 0, 80);
                    break;
                case 1:
                    inventoryM.armorInv.highLight[i].GetComponent<UIGradient>().enabled = true;
                    inventoryM.armorInv.highLight[i].GetComponent<UIGradient>().LinearColor1 = inventoryM.rank1Color1;
                    inventoryM.armorInv.highLight[i].GetComponent<UIGradient>().LinearColor2 = inventoryM.rank1Color2;
                    break;
                case 2:
                    inventoryM.armorInv.highLight[i].GetComponent<UIGradient>().enabled = true;
                    inventoryM.armorInv.highLight[i].GetComponent<UIGradient>().LinearColor1 = inventoryM.rank2Color1;
                    inventoryM.armorInv.highLight[i].GetComponent<UIGradient>().LinearColor2 = inventoryM.rank2Color2;
                    break;
                case 3:
                    inventoryM.armorInv.highLight[i].GetComponent<UIGradient>().enabled = true;
                    inventoryM.armorInv.highLight[i].GetComponent<UIGradient>().LinearColor1 = inventoryM.rank3Color1;
                    inventoryM.armorInv.highLight[i].GetComponent<UIGradient>().LinearColor2 = inventoryM.rank3Color2;
                    break;
            }

            if (transitionData.armData[i].broken)
            {
                inventoryM.armorInv.highLight[i].GetComponent<UIGradient>().enabled = true;
                inventoryM.armorInv.highLight[i].GetComponent<UIGradient>().LinearColor1 = inventoryM.brokenColor1;
                inventoryM.armorInv.highLight[i].GetComponent<UIGradient>().LinearColor2 = inventoryM.brokenColor2;
            }

            #endregion

            if (inventoryM.armorInv.itemData[i].equipped)
            {
                inventoryM.armorInv.outLineBorder[i].enabled = true;
                inventoryM.armorInv.outLineBorder[i].color = inventoryM.outLineSelected;
            }

            if (!inventoryM.armorInv.itemData[i].stackable)
            {
                inventoryM.armorInv.counter[i] = 1;
                inventoryM.armorInv.image[i].enabled = true;
            }
            else
            {
                inventoryM.armorInv.image[i].enabled = true;
                inventoryM.armorInv.quantity[i].enabled = true;
                inventoryM.armorInv.counter[i] = transitionData.armData[i].quantity;
                inventoryM.armorInv.quantity[i].text = "x" + inventoryM.armorInv.counter[i].ToString();
            }
            SetLoadedSprite(ref inventoryM.armorInv.itemData[i], ref inventoryM.armorInv.image[i]);
        }

        #endregion

        loadingDataNameText.text = "Loading Armor slots data...";
        yield return new WaitForSeconds(0.8f);

        #region Load Material Inventory Data

        inventoryM.materialInv.slotNum = transitionData.matSlotNum;

        inventoryM.materialInv.itemData = new ItemData[transitionData.matData.Length];
        for (int i = 0; i < transitionData.matData.Length; i++)
        {
            GameObject emptyitemData = Instantiate(itemDataPrefab) as GameObject;
            inventoryM.materialInv.itemData[i] = emptyitemData.GetComponent<ItemData>();

            inventoryM.materialInv.itemData[i].itemType = (ItemData.ItemType)transitionData.matData[i].itemType;
            inventoryM.materialInv.itemData[i].itemName = transitionData.matData[i].itemName;
            inventoryM.materialInv.itemData[i].itemDescription = transitionData.matData[i].itemDescription;

            inventoryM.materialInv.itemData[i].buyPrice = transitionData.matData[i].buyPrice;
            inventoryM.materialInv.itemData[i].sellPrice = transitionData.matData[i].sellPrice;
            inventoryM.materialInv.itemData[i].repairPrice = transitionData.matData[i].repairPrice;
            inventoryM.materialInv.itemData[i].upgradePrice = transitionData.matData[i].upgradePrice;
            inventoryM.materialInv.itemData[i].specialPrice = transitionData.matData[i].specialPrice;
            inventoryM.materialInv.itemData[i].inStockQuantity = transitionData.matData[i].inStockQuantity;

            inventoryM.materialInv.counter[i] = transitionData.matData[i].quantity;
            inventoryM.materialInv.itemData[i].stackable = transitionData.matData[i].stackable;
            inventoryM.materialInv.itemData[i].inInventory = transitionData.matData[i].inInventory;

            inventoryM.materialInv.image[i].type = Image.Type.Simple;
            inventoryM.materialInv.image[i].preserveAspect = true;
            Navigation automatic = new Navigation { mode = Navigation.Mode.Automatic };
            inventoryM.materialInv.image[i].GetComponent<Button>().navigation = automatic;
            inventoryM.materialInv.image[i].enabled = true;

            if (!inventoryM.materialInv.itemData[i].stackable)
            {
                inventoryM.materialInv.counter[i] = 1;
                inventoryM.materialInv.image[i].enabled = true;
            }
            else
            {
                inventoryM.materialInv.image[i].enabled = true;
                inventoryM.materialInv.quantity[i].enabled = true;
                inventoryM.materialInv.counter[i] = transitionData.matData[i].quantity;
                inventoryM.materialInv.quantity[i].text = "x" + inventoryM.materialInv.counter[i].ToString();
            }
            SetLoadedSprite(ref inventoryM.materialInv.itemData[i], ref inventoryM.materialInv.image[i]);
        }

        #endregion

        loadingDataNameText.text = "Loading Material slots data...";
        yield return new WaitForSeconds(0.8f);

        #region Load Healing Inventory Data

        inventoryM.healingInv.slotNum = transitionData.healSlotNum;

        inventoryM.healingInv.itemData = new ItemData[transitionData.healData.Length];
        for (int i = 0; i < transitionData.healData.Length; i++)
        {
            GameObject emptyitemData = Instantiate(itemDataPrefab) as GameObject;
            inventoryM.healingInv.itemData[i] = emptyitemData.GetComponent<ItemData>();

            inventoryM.healingInv.itemData[i].itemType = (ItemData.ItemType)transitionData.healData[i].itemType;
            inventoryM.healingInv.itemData[i].itemName = transitionData.healData[i].itemName;
            inventoryM.healingInv.itemData[i].itemDescription = transitionData.healData[i].itemDescription;

            inventoryM.healingInv.itemData[i].buyPrice = transitionData.healData[i].buyPrice;
            inventoryM.healingInv.itemData[i].sellPrice = transitionData.healData[i].sellPrice;
            inventoryM.healingInv.itemData[i].repairPrice = transitionData.healData[i].repairPrice;
            inventoryM.healingInv.itemData[i].upgradePrice = transitionData.healData[i].upgradePrice;
            inventoryM.healingInv.itemData[i].specialPrice = transitionData.healData[i].specialPrice;
            inventoryM.healingInv.itemData[i].inStockQuantity = transitionData.healData[i].inStockQuantity;

            inventoryM.healingInv.counter[i] = transitionData.healData[i].quantity;
            inventoryM.healingInv.itemData[i].stackable = transitionData.healData[i].stackable;
            inventoryM.healingInv.itemData[i].inInventory = transitionData.healData[i].inInventory;

            inventoryM.healingInv.image[i].type = Image.Type.Simple;
            inventoryM.healingInv.image[i].preserveAspect = true;
            Navigation automatic = new Navigation { mode = Navigation.Mode.Automatic };
            inventoryM.healingInv.image[i].GetComponent<Button>().navigation = automatic;
            inventoryM.healingInv.image[i].enabled = true;

            if (!inventoryM.healingInv.itemData[i].stackable)
            {
                inventoryM.healingInv.counter[i] = 1;
                inventoryM.healingInv.image[i].enabled = true;
            }
            else
            {
                inventoryM.healingInv.image[i].enabled = true;
                inventoryM.healingInv.quantity[i].enabled = true;
                inventoryM.healingInv.counter[i] = transitionData.healData[i].quantity;
                inventoryM.healingInv.quantity[i].text = "x" + inventoryM.healingInv.counter[i].ToString();
            }
            SetLoadedSprite(ref inventoryM.healingInv.itemData[i], ref inventoryM.healingInv.image[i]);
        }

        #endregion

        loadingDataNameText.text = "Loading Healing slots data...";
        yield return new WaitForSeconds(0.8f);

        #region Load Key Inventory Data

        inventoryM.keyInv.slotNum = transitionData.keySlotNum;

        inventoryM.keyInv.itemData = new ItemData[transitionData.keyData.Length];
        for (int i = 0; i < transitionData.keyData.Length; i++)
        {
            GameObject emptyitemData = Instantiate(itemDataPrefab) as GameObject;
            inventoryM.keyInv.itemData[i] = emptyitemData.GetComponent<ItemData>();

            inventoryM.keyInv.itemData[i].itemType = (ItemData.ItemType)transitionData.keyData[i].itemType;

            inventoryM.keyInv.itemData[i].itemType = (ItemData.ItemType)transitionData.keyData[i].itemType;
            inventoryM.keyInv.itemData[i].itemName = transitionData.keyData[i].itemName;
            inventoryM.keyInv.itemData[i].itemDescription = transitionData.keyData[i].itemDescription;

            inventoryM.keyInv.itemData[i].buyPrice = transitionData.keyData[i].buyPrice;
            inventoryM.keyInv.itemData[i].sellPrice = transitionData.keyData[i].sellPrice;
            inventoryM.keyInv.itemData[i].repairPrice = transitionData.keyData[i].repairPrice;
            inventoryM.keyInv.itemData[i].upgradePrice = transitionData.keyData[i].upgradePrice;
            inventoryM.keyInv.itemData[i].specialPrice = transitionData.keyData[i].specialPrice;
            inventoryM.keyInv.itemData[i].inStockQuantity = transitionData.keyData[i].inStockQuantity;

            inventoryM.keyInv.counter[i] = transitionData.keyData[i].quantity;
            inventoryM.keyInv.itemData[i].stackable = transitionData.keyData[i].stackable;
            inventoryM.keyInv.itemData[i].inInventory = transitionData.keyData[i].inInventory;

            inventoryM.keyInv.image[i].type = Image.Type.Simple;
            inventoryM.keyInv.image[i].preserveAspect = true;
            Navigation automatic = new Navigation { mode = Navigation.Mode.Automatic };
            inventoryM.keyInv.image[i].GetComponent<Button>().navigation = automatic;
            inventoryM.keyInv.image[i].enabled = true;

            if (!inventoryM.keyInv.itemData[i].stackable)
            {
                inventoryM.keyInv.counter[i] = 1;
                inventoryM.keyInv.image[i].enabled = true;
            }
            else
            {
                inventoryM.keyInv.image[i].enabled = true;
                inventoryM.keyInv.quantity[i].enabled = true;
                inventoryM.keyInv.counter[i] = transitionData.keyData[i].quantity;
                inventoryM.keyInv.quantity[i].text = "x" + inventoryM.keyInv.counter[i].ToString();
            }
            SetLoadedSprite(ref inventoryM.keyInv.itemData[i], ref inventoryM.keyInv.image[i]);
        }

        #endregion

        loadingDataNameText.text = "Loading Key slots data...";
        yield return new WaitForSeconds(0.8f);

        #endregion

        #region Load Skill Tree Data

        skillM.topAbilityNameMenu = transitionData.topAbilityName;
        skillM.leftAbilityNameMenu = transitionData.leftAbilityName;
        skillM.rightAbilityNameMenu = transitionData.rightAbilityName;
        skillM.bottomAbilityNameMenu = transitionData.bottomAbilityName;

        for (int i = 0; i < skillSlots.Length; i++)
        {
            if(skillSlots[i] != null)
            {
                if (skillSlots[i].purchasedSkillName == transitionData.purchasedSkillName[i])
                {
                    skillSlots[i].m_isPurchased = true;
                }
            }
        }

        #endregion

        loadingDataNameText.text = "Loading Skill Tree data...";
        yield return new WaitForSeconds(1);

        #region Load Miscellaneous Data

        if (sceneData != null)
        {
            for (int i = 0; i < locationCPData.Length; i++)
            {
                if (locationCPData[i] != null)
                {
                    locationCPData[i].explored = sceneData.explored[i];
                }
            }

            for (int i = 0; i < sceneData.triggerAction.Length; i++)
            {
                if (triggerActionData.Length > 0)
                {
                    triggerActionData[i].isActive = sceneData.triggerAction[i].isActive;
                    triggerActionData[i].interactionType = (TriggerAction.InteractionType)sceneData.triggerAction[i].interactionType;
                    triggerActionData[i].destroyAfter = sceneData.triggerAction[i].destroyAfter;

                    // Treasure Chest
                    if (triggerActionData[i].isActive && triggerActionData[i].item != null)
                    {
                        if (triggerActionData[i].animator)
                        {
                            triggerActionData[i].animator.SetBool("Activate", true);
                        }
                    }

                    // Moving Platform 
                    if (triggerActionData[i].isActive && triggerActionData[i].name == "MovingPlatform" &&
                        triggerActionData[i].interactionType == TriggerAction.InteractionType.Normal)
                    {
                        if (triggerActionData[i].animator)
                        {
                            triggerActionData[i].animator.SetBool("Activate", false);
                        }
                    }

                    // Pressure Pad
                    if (triggerActionData[i].isActive && triggerActionData[i].item == null && 
                    (triggerActionData[i].interactionType == TriggerAction.InteractionType.PressureStep || 
                    triggerActionData[i].interactionType == TriggerAction.InteractionType.PressureObjectWeight))
                    {
                        if (triggerActionData[i].animator)
                        {
                            triggerActionData[i].animator.SetBool("Activate", true);
                        }
                    }

                    // Generic Trigger
                    if (triggerActionData[i].isActive && triggerActionData[i].item == null &&
                        triggerActionData[i].interactionType == TriggerAction.InteractionType.Normal)
                    {
                        if (triggerActionData[i].animator)
                        {
                            triggerActionData[i].animator.SetBool("Activate", false);
                            triggerActionData[i].animator.SetInteger("Side", triggerActionData[i].side);
                        }

                        if (triggerActionData[i].gameObject.name != "MovingPlatform" && triggerActionData[i].gameObject.name != "Sphere")
                            triggerActionData[i].isActive = false;
                    }
                }
            }

            // Interactable Items
            for (int i = 0; i < inventoryM.initWpns.Length; i++)
            {
                inventoryM.initWpns[i].GetComponent<ItemData>().itemActive = sceneData.itemActive[i];
            }
        }

        #endregion

        // When AI position/rotation is repositoned enabled NavMeshAgent.
        foreach (AIController npc in FindObjectsOfType<AIController>())
        {
            if (npc.GetComponent<NavMeshAgent>() != null)
                npc.GetComponent<NavMeshAgent>().enabled = true;
        }

        loadingDataNameText.text = "Loading Scene...";
        yield return new WaitForSeconds(1.5f);

        loadingDataNameText.text = "";

        loadingScreenFUI.FadeTransition(0, 0, 1);

        // Deactivate Menu Section that was activate in order to load data.
        inventoryM.InventorySectionIsActive(false);
        inventoryM.PauseMenuSectionIsActive(false);

        PlayerPrefs.SetInt("SceneInTransitionDataRef", 0);
        PlayerPrefs.SetInt("SpawnPointIDRef", -1);

        inventoryM.referencesObj.inventorySection.GetComponent<HideUI>().enabled = true;

        isLoadingData = false;
    }

    #endregion

    #endregion

    #region Set Loaded Item Sprite Data

    public void SetLoadedSprite(ref ItemData itemData, ref Image sprite)
    {
        if (itemData.itemName == "Windscar" + itemData.rankTag)
        {
            for (int i = 0; i < inventoryM.itemIconData.Length; i++)
            {
                if (itemData.originItemName == inventoryM.itemIconData[i].GetComponentInChildren<ItemData>().originItemName)
                    sprite.sprite = inventoryM.itemIconData[i].GetComponentInChildren<ItemData>().itemSprite;

                inventoryM.referencesObj.primaryItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                inventoryM.referencesObj.primaryItemImage.enabled = true;
                inventoryM.referencesObj.primaryItemImage.sprite = sprite.sprite;
                inventoryM.referencesObj.primaryValueBG.enabled = true;
                inventoryM.referencesObj.primaryValueBG.GetComponentInChildren<TextMeshProUGUI>().text = itemData.maxWpnAtk.ToString();
            }
            if (inventoryM.weaponInv.itemData[inventoryM.weaponInv.slotWeaponEquipped].itemName == itemData.itemName
            && inventoryM.weaponInv.itemData[inventoryM.weaponInv.slotWeaponEquipped].equipped)
            {
                inventoryM.DeactiveWeapons();
                inventoryM.DeactiveWeaponsHP();
                cc.wpnHolster.windScarNoParticlesH.SetActive(true);
                cc.wpnHolster.windScarNoParticlesEP.SetActive(true);
                cc.wpnHolster.swordHP = cc.wpnHolster.windScarNoParticlesHP;
                cc.wpnHolster.swordEP = cc.wpnHolster.windScarNoParticlesEP;
                cc.wpnHolster.primaryH = cc.wpnHolster.windScarNoParticlesH;
                cc.wpnHolster.primaryE = cc.wpnHolster.windScarNoParticleE;
                cc.wpnHolster.primaryD = cc.wpnHolster.windScarNoParticlePrefab;
                cc.wpnHolster.alteredPrimaryE = cc.wpnHolster.windScarE;
                cc.wpnHolster.alteredPrimaryEP = cc.wpnHolster.windScarEP;
                cc.wpnHolster.alteredPrimaryHP = cc.wpnHolster.windScarHP;
                cc.wpnHolster.SetItemData(ref cc.wpnHolster.windScarE, ref inventoryM.weaponInv.itemData[inventoryM.weaponInv.slotWeaponEquipped]);
            }
        }

        if (itemData.itemName == "The Tuning fork" + itemData.rankTag)
        {
            for (int i = 0; i < inventoryM.itemIconData.Length; i++)
            {
                if (itemData.originItemName == inventoryM.itemIconData[i].GetComponentInChildren<ItemData>().originItemName)
                {
                    sprite.sprite = inventoryM.itemIconData[i].GetComponentInChildren<ItemData>().itemSprite;

                    inventoryM.referencesObj.primaryItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                    inventoryM.referencesObj.primaryItemImage.enabled = true;
                    inventoryM.referencesObj.primaryItemImage.sprite = sprite.sprite;
                    inventoryM.referencesObj.primaryValueBG.enabled = true;
                    inventoryM.referencesObj.primaryValueBG.GetComponentInChildren<TextMeshProUGUI>().text = itemData.maxWpnAtk.ToString();
                }
            }
            if (inventoryM.weaponInv.itemData[inventoryM.weaponInv.slotWeaponEquipped].itemName == itemData.itemName
            && inventoryM.weaponInv.itemData[inventoryM.weaponInv.slotWeaponEquipped].equipped)
            {
                inventoryM.DeactiveWeapons();
                inventoryM.DeactiveWeaponsHP();
                cc.wpnHolster.theTuningForkH.SetActive(true);
                cc.wpnHolster.theTuningForkEP.SetActive(true);
                cc.wpnHolster.dSwordHP = cc.wpnHolster.theTuningForkHP;
                cc.wpnHolster.dSwordEP = cc.wpnHolster.theTuningForkEP;
                cc.wpnHolster.primaryEP = cc.wpnHolster.theTuningForkEP;
                cc.wpnHolster.primaryH = cc.wpnHolster.theTuningForkH;
                cc.wpnHolster.primaryE = cc.wpnHolster.theTuningForkE;
                cc.wpnHolster.primaryD = cc.wpnHolster.theTuningForkPrefab;
                cc.wpnHolster.SetItemData(ref cc.wpnHolster.theTuningForkE, ref inventoryM.weaponInv.itemData[inventoryM.weaponInv.slotWeaponEquipped]);
            }
        }

        if (itemData.itemName == "Assassin Dagger" + itemData.rankTag)
        {
            for (int i = 0; i < inventoryM.itemIconData.Length; i++)
            {
                if (itemData.originItemName == inventoryM.itemIconData[i].GetComponentInChildren<ItemData>().originItemName)
                {
                    sprite.sprite = inventoryM.itemIconData[i].GetComponentInChildren<ItemData>().itemSprite;

                    inventoryM.referencesObj.primaryItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                    inventoryM.referencesObj.primaryItemImage.enabled = true;
                    inventoryM.referencesObj.primaryItemImage.sprite = sprite.sprite;
                    inventoryM.referencesObj.primaryValueBG.enabled = true;
                    inventoryM.referencesObj.primaryValueBG.GetComponentInChildren<TextMeshProUGUI>().text = itemData.maxWpnAtk.ToString();
                }
            }
            if (inventoryM.weaponInv.itemData[inventoryM.weaponInv.slotWeaponEquipped].itemName == itemData.itemName
            && inventoryM.weaponInv.itemData[inventoryM.weaponInv.slotWeaponEquipped].equipped)
            {
                inventoryM.DeactiveWeapons();
                inventoryM.DeactiveWeaponsHP();
                cc.wpnHolster.assasinsDaggerH.SetActive(true);
                cc.wpnHolster.assasinsDaggerEP.SetActive(true);
                cc.wpnHolster.swordHP = cc.wpnHolster.assasinsDaggerHP;
                cc.wpnHolster.swordEP = cc.wpnHolster.assasinsDaggerEP;
                cc.wpnHolster.primaryH = cc.wpnHolster.assasinsDaggerH;
                cc.wpnHolster.primaryE = cc.wpnHolster.assasinsDaggerE;
                cc.wpnHolster.primaryD = cc.wpnHolster.assasinsDaggerPrefab;
                cc.wpnHolster.SetItemData(ref cc.wpnHolster.assasinsDaggerE, ref inventoryM.weaponInv.itemData[inventoryM.weaponInv.slotWeaponEquipped]);
            }
        }

        if (itemData.itemName == "Obsidian Fury" + itemData.rankTag)
        {
            for (int i = 0; i < inventoryM.itemIconData.Length; i++)
            {
                if (itemData.originItemName == inventoryM.itemIconData[i].GetComponentInChildren<ItemData>().originItemName)
                {
                    sprite.sprite = inventoryM.itemIconData[i].GetComponentInChildren<ItemData>().itemSprite;

                    inventoryM.referencesObj.primaryItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                    inventoryM.referencesObj.primaryItemImage.enabled = true;
                    inventoryM.referencesObj.primaryItemImage.sprite = sprite.sprite;
                    inventoryM.referencesObj.primaryValueBG.enabled = true;
                    inventoryM.referencesObj.primaryValueBG.GetComponentInChildren<TextMeshProUGUI>().text = itemData.maxWpnAtk.ToString();
                }
            }
            if (inventoryM.weaponInv.itemData[inventoryM.weaponInv.slotWeaponEquipped].itemName == itemData.itemName
                && inventoryM.weaponInv.itemData[inventoryM.weaponInv.slotWeaponEquipped].equipped)
            {
                inventoryM.DeactiveWeapons();
                inventoryM.DeactiveWeaponsHP();
                cc.wpnHolster.obsidianFuryH.SetActive(true);
                cc.wpnHolster.obsidianFuryEP.SetActive(true);
                cc.wpnHolster.primaryEP = cc.wpnHolster.obsidianFuryEP;
                cc.wpnHolster.hammerHP = cc.wpnHolster.obsidianFuryHP;
                cc.wpnHolster.hammerEP = cc.wpnHolster.obsidianFuryEP;
                cc.wpnHolster.primaryH = cc.wpnHolster.obsidianFuryH;
                cc.wpnHolster.primaryE = cc.wpnHolster.obsidianFuryE;
                cc.wpnHolster.primaryD = cc.wpnHolster.obsidianFuryPrefab;
                cc.wpnHolster.SetItemData(ref cc.wpnHolster.obsidianFuryE, ref inventoryM.weaponInv.itemData[inventoryM.weaponInv.slotWeaponEquipped]);
            }
        }

        if (itemData.itemName == "Glaive" + itemData.rankTag)
        {
            for (int i = 0; i < inventoryM.itemIconData.Length; i++)
            {
                if (itemData.originItemName == inventoryM.itemIconData[i].GetComponentInChildren<ItemData>().originItemName)
                {
                    sprite.sprite = inventoryM.itemIconData[i].GetComponentInChildren<ItemData>().itemSprite;

                    inventoryM.referencesObj.primaryItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                    inventoryM.referencesObj.primaryItemImage.enabled = true;
                    inventoryM.referencesObj.primaryItemImage.sprite = sprite.sprite;
                    inventoryM.referencesObj.primaryValueBG.enabled = true;
                    inventoryM.referencesObj.primaryValueBG.GetComponentInChildren<TextMeshProUGUI>().text = itemData.maxWpnAtk.ToString();
                }
            }
            if (inventoryM.weaponInv.itemData[inventoryM.weaponInv.slotWeaponEquipped].itemName == itemData.itemName
            && inventoryM.weaponInv.itemData[inventoryM.weaponInv.slotWeaponEquipped].equipped)
            {
                inventoryM.DeactiveWeapons();
                inventoryM.DeactiveWeaponsHP();
                cc.wpnHolster.glaiveH.SetActive(true);
                cc.wpnHolster.glaiveEP.SetActive(true);
                cc.wpnHolster.primaryEP = cc.wpnHolster.glaiveEP;
                cc.wpnHolster.spearHP = cc.wpnHolster.glaiveHP;
                cc.wpnHolster.spearEP = cc.wpnHolster.glaiveEP;
                cc.wpnHolster.primaryH = cc.wpnHolster.glaiveH;
                cc.wpnHolster.primaryE = cc.wpnHolster.glaiveE;
                cc.wpnHolster.primaryD = cc.wpnHolster.glaivePrefab;
                cc.wpnHolster.SetItemData(ref cc.wpnHolster.glaiveE, ref inventoryM.weaponInv.itemData[inventoryM.weaponInv.slotWeaponEquipped]);
            }
        }

        if (itemData.itemName == "Cleric's Staff" + itemData.rankTag)
        {
            for (int i = 0; i < inventoryM.itemIconData.Length; i++)
            {
                if (itemData.originItemName == inventoryM.itemIconData[i].GetComponentInChildren<ItemData>().originItemName)
                {
                    sprite.sprite = inventoryM.itemIconData[i].GetComponentInChildren<ItemData>().itemSprite;

                    inventoryM.referencesObj.primaryItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                    inventoryM.referencesObj.primaryItemImage.enabled = true;
                    inventoryM.referencesObj.primaryItemImage.sprite = sprite.sprite;
                    inventoryM.referencesObj.primaryValueBG.enabled = true;
                    inventoryM.referencesObj.primaryValueBG.GetComponentInChildren<TextMeshProUGUI>().text = itemData.maxWpnAtk.ToString();
                }
            }
            if (inventoryM.weaponInv.itemData[inventoryM.weaponInv.slotWeaponEquipped].itemName == itemData.itemName
            && inventoryM.weaponInv.itemData[inventoryM.weaponInv.slotWeaponEquipped].equipped)
            {
                inventoryM.DeactiveWeapons();
                inventoryM.DeactiveWeaponsHP();
                cc.wpnHolster.clericsStaffH.SetActive(true);
                cc.wpnHolster.clericsStaffEP.SetActive(true);
                cc.wpnHolster.staffHP = cc.wpnHolster.clericsStaffHP;
                cc.wpnHolster.staffEP = cc.wpnHolster.clericsStaffEP;
                cc.wpnHolster.primaryH = cc.wpnHolster.clericsStaffH;
                cc.wpnHolster.primaryE = cc.wpnHolster.clericsStaffPrefab;
                cc.wpnHolster.primaryD = inventoryM.weaponInv.itemData[inventoryM.weaponInv.slotWeaponEquipped].gameObject;
                cc.wpnHolster.SetItemData(ref cc.wpnHolster.clericsStaffE, ref inventoryM.weaponInv.itemData[inventoryM.weaponInv.slotWeaponEquipped]);
            }
        }

        if (itemData.itemName == "Circle Shield" + itemData.rankTag)
        {
            for (int i = 0; i < inventoryM.itemIconData.Length; i++)
            {
                if (itemData.originItemName == inventoryM.itemIconData[i].GetComponentInChildren<ItemData>().originItemName)
                {
                    sprite.sprite = inventoryM.itemIconData[i].GetComponentInChildren<ItemData>().itemSprite;

                    inventoryM.referencesObj.shieldItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                    inventoryM.referencesObj.shieldItemImage.enabled = true;
                    inventoryM.referencesObj.shieldItemImage.sprite = sprite.sprite;
                    inventoryM.referencesObj.shieldValueBG.enabled = true;
                    inventoryM.referencesObj.shieldValueBG.GetComponentInChildren<TextMeshProUGUI>().text = itemData.maxShdAtk.ToString();
                }
            }
            if (inventoryM.shieldInv.itemData[inventoryM.shieldInv.slotShieldEquipped].itemName == itemData.itemName
            && inventoryM.shieldInv.itemData[inventoryM.shieldInv.slotShieldEquipped].equipped)
            {
                inventoryM.DeactiveShield();
                inventoryM.DeactiveShieldHP();
                cc.wpnHolster.circleShieldH.SetActive(true);
                cc.wpnHolster.circleShieldEP.SetActive(true);
                cc.wpnHolster.shieldHP = cc.wpnHolster.circleShieldHP;
                cc.wpnHolster.shieldP = cc.wpnHolster.circleShieldEP;
                cc.wpnHolster.shieldH = cc.wpnHolster.circleShieldH;
                cc.wpnHolster.shieldE = cc.wpnHolster.circleShieldE;
                cc.wpnHolster.shieldD = cc.wpnHolster.circleShieldPrefab;
                cc.wpnHolster.SetItemData(ref cc.wpnHolster.circleShieldE, ref inventoryM.shieldInv.itemData[inventoryM.shieldInv.slotShieldEquipped]);
            }
        }

        if (itemData.itemName == "Warbow" + itemData.rankTag)
        {
            for (int i = 0; i < inventoryM.itemIconData.Length; i++)
            {
                if (itemData.originItemName == inventoryM.itemIconData[i].GetComponentInChildren<ItemData>().originItemName)
                {
                    sprite.sprite = inventoryM.itemIconData[i].GetComponentInChildren<ItemData>().itemSprite;

                    inventoryM.referencesObj.secondaryItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                    inventoryM.referencesObj.secondaryItemImage.enabled = true;
                    inventoryM.referencesObj.secondaryItemImage.sprite = sprite.sprite;
                    inventoryM.referencesObj.secondaryValueBG.enabled = true;
                    inventoryM.referencesObj.secondaryValueBG.GetComponentInChildren<TextMeshProUGUI>().text = itemData.maxBowAtk.ToString();
                }
            }
            if (inventoryM.bowAndArrowInv.itemData[inventoryM.bowAndArrowInv.slotBowEquipped].itemName == itemData.itemName
            && inventoryM.bowAndArrowInv.itemData[inventoryM.bowAndArrowInv.slotBowEquipped].equipped)
            {
                inventoryM.DeactiveBows();
                inventoryM.DeactiveBowsHP();
                cc.wpnHolster.warbowH.SetActive(true);
                cc.wpnHolster.warbowEP.SetActive(true);
                cc.wpnHolster.quiverHP.SetActive(true);
                cc.wpnHolster.quiverH.SetActive(true);
                cc.wpnHolster.bowHP = cc.wpnHolster.warbowHP;
                cc.wpnHolster.bowEP = cc.wpnHolster.warbowEP;
                cc.wpnHolster.bowString = cc.wpnHolster.warbowString;
                cc.wpnHolster.arrowPrefabSpot = cc.wpnHolster.warbowPrefabSpot;
                cc.wpnHolster.secondaryH = cc.wpnHolster.warbowH;
                cc.wpnHolster.secondaryE = cc.wpnHolster.warbowE;
                cc.bowAnim = cc.wpnHolster.secondaryE.GetComponent<Animator>();
                cc.wpnHolster.secondaryD = cc.wpnHolster.warbowPrefab;
                cc.wpnHolster.SetItemData(ref cc.wpnHolster.warbowE, ref inventoryM.bowAndArrowInv.itemData[inventoryM.bowAndArrowInv.slotBowEquipped]);
            }
        }

        if (itemData.itemName == "Lu Rifle" + itemData.rankTag)
        {
            for (int i = 0; i < inventoryM.itemIconData.Length; i++)
            {
                if (itemData.originItemName == inventoryM.itemIconData[i].GetComponentInChildren<ItemData>().originItemName)
                {
                    sprite.sprite = inventoryM.itemIconData[i].GetComponentInChildren<ItemData>().itemSprite;

                    inventoryM.referencesObj.secondaryItemImage.GetComponentInParent<EquippedItemSlot>().m_numOfEngraving = itemData.numOfEngraving;
                    inventoryM.referencesObj.secondaryItemImage.enabled = true;
                    inventoryM.referencesObj.secondaryItemImage.sprite = sprite.sprite;
                    inventoryM.referencesObj.secondaryValueBG.enabled = true;
                    inventoryM.referencesObj.secondaryValueBG.GetComponentInChildren<TextMeshProUGUI>().text = itemData.maxBowAtk.ToString();
                }
            }
            if (inventoryM.bowAndArrowInv.itemData[inventoryM.bowAndArrowInv.slotBowEquipped].itemName == itemData.itemName
            && inventoryM.bowAndArrowInv.itemData[inventoryM.bowAndArrowInv.slotBowEquipped].equipped)
            {
                inventoryM.DeactiveBows();
                inventoryM.DeactiveBowsHP();
                cc.wpnHolster.luRifleH.SetActive(true);
                cc.wpnHolster.luRifleEP.SetActive(true);
                cc.wpnHolster.luRifleHP = cc.wpnHolster.warbowHP;
                cc.wpnHolster.luRifleEP = cc.wpnHolster.warbowEP;
                cc.wpnHolster.arrowPrefabSpot = cc.wpnHolster.luRiflePrefabSpot;
                cc.wpnHolster.secondaryH = cc.wpnHolster.luRifleH;
                cc.wpnHolster.secondaryE = cc.wpnHolster.luRifleE;
                cc.wpnHolster.secondaryD = cc.wpnHolster.luRiflePrefab;
                cc.wpnHolster.SetItemData(ref cc.wpnHolster.luRifleE, ref inventoryM.bowAndArrowInv.itemData[inventoryM.bowAndArrowInv.slotBowEquipped]);
            }
        }

        if (itemData.itemName == "Common Arrow" + itemData.rankTag)
        {
            for (int i = 0; i < inventoryM.itemIconData.Length; i++)
            {
                if (itemData.originItemName == inventoryM.itemIconData[i].GetComponentInChildren<ItemData>().originItemName)
                {
                    sprite.sprite = inventoryM.itemIconData[i].GetComponentInChildren<ItemData>().itemSprite;

                    inventoryM.referencesObj.arrowItemImage.enabled = true;
                    inventoryM.referencesObj.arrowItemImage.sprite = sprite.sprite;
                    inventoryM.referencesObj.arrowValueBG.enabled = true;
                    inventoryM.referencesObj.arrowValueBG.GetComponentInChildren<TextMeshProUGUI>().text = inventoryM.bowAndArrowInv.counter[inventoryM.bowAndArrowInv.slotArrowEquipped].ToString();
                }
            }
            if (inventoryM.bowAndArrowInv.itemData[inventoryM.bowAndArrowInv.slotArrowEquipped].itemName == itemData.itemName
            && inventoryM.bowAndArrowInv.itemData[inventoryM.bowAndArrowInv.slotArrowEquipped].equipped)
            {
                inventoryM.DeactiveArrows();
                inventoryM.DeactiveArrowsHP();
                cc.wpnHolster.commonArrowH.SetActive(true);
                cc.wpnHolster.commonArrowHP.SetActive(true);
                cc.wpnHolster.commonArrowEP.SetActive(true);
                cc.wpnHolster.arrowHP = cc.wpnHolster.commonArrowHP;
                cc.wpnHolster.arrowEP = cc.wpnHolster.commonArrowEP;
                cc.wpnHolster.arrowH = cc.wpnHolster.commonArrowH;
                cc.wpnHolster.arrowE = cc.wpnHolster.commonArrowE;
                cc.wpnHolster.arrowD = cc.wpnHolster.commonArrowPrefab;
                cc.wpnHolster.arrowString = cc.wpnHolster.commonArrowString;
                cc.wpnHolster.bowStrings.transform.SetParent(cc.wpnHolster.bowString.transform);
                cc.wpnHolster.quiverArrows = cc.wpnHolster.commonArrowH.GetComponentsInChildren<QuiverArrows>();
                cc.wpnHolster.SetActiveQuiverArrows();
                cc.wpnHolster.SetItemData(ref cc.wpnHolster.commonArrowE, ref inventoryM.bowAndArrowInv.itemData[inventoryM.bowAndArrowInv.slotArrowEquipped]);
            }
        }

        if (itemData.itemName == "Particle Arrow" + itemData.rankTag)
        {
            for (int i = 0; i < inventoryM.itemIconData.Length; i++)
            {
                if (itemData.originItemName == inventoryM.itemIconData[i].GetComponentInChildren<ItemData>().originItemName)
                {
                    sprite.sprite = inventoryM.itemIconData[i].GetComponentInChildren<ItemData>().itemSprite;

                    inventoryM.referencesObj.arrowItemImage.enabled = true;
                    inventoryM.referencesObj.arrowItemImage.sprite = sprite.sprite;
                    inventoryM.referencesObj.arrowValueBG.enabled = true;
                    inventoryM.referencesObj.arrowValueBG.GetComponentInChildren<TextMeshProUGUI>().text = inventoryM.bowAndArrowInv.counter[inventoryM.bowAndArrowInv.slotArrowEquipped].ToString();
                }
            }
            if (inventoryM.bowAndArrowInv.itemData[inventoryM.bowAndArrowInv.slotArrowEquipped].itemName == itemData.itemName
            && inventoryM.bowAndArrowInv.itemData[inventoryM.bowAndArrowInv.slotArrowEquipped].equipped)
            {
                inventoryM.DeactiveArrows();
                inventoryM.DeactiveArrowsHP();
                cc.wpnHolster.particleArrowH.SetActive(true);
                cc.wpnHolster.particleArrowHP.SetActive(true);
                cc.wpnHolster.particleArrowEP.SetActive(true);
                cc.wpnHolster.arrowHP = cc.wpnHolster.particleArrowHP;
                cc.wpnHolster.arrowEP = cc.wpnHolster.particleArrowEP;
                cc.wpnHolster.arrowH = cc.wpnHolster.particleArrowH;
                cc.wpnHolster.arrowE = cc.wpnHolster.particleArrowE;
                cc.wpnHolster.arrowD = cc.wpnHolster.particleArrowPrefab;
                cc.wpnHolster.arrowString = cc.wpnHolster.particleArrowString;
                cc.wpnHolster.bowStrings.transform.SetParent(cc.wpnHolster.bowString.transform);
                cc.wpnHolster.quiverArrows = cc.wpnHolster.particleArrowH.GetComponentsInChildren<QuiverArrows>();
                cc.wpnHolster.SetActiveQuiverArrows();
                cc.wpnHolster.SetItemData(ref cc.wpnHolster.particleArrowE, ref inventoryM.bowAndArrowInv.itemData[inventoryM.bowAndArrowInv.slotArrowEquipped]);
            }
        }

        if (itemData.itemName == "7.62mm" + itemData.rankTag)
        {
            for (int i = 0; i < inventoryM.itemIconData.Length; i++)
            {
                if (itemData.originItemName == inventoryM.itemIconData[i].GetComponentInChildren<ItemData>().originItemName)
                {
                    sprite.sprite = inventoryM.itemIconData[i].GetComponentInChildren<ItemData>().itemSprite;

                    inventoryM.referencesObj.arrowItemImage.enabled = true;
                    inventoryM.referencesObj.arrowItemImage.sprite = sprite.sprite;
                    inventoryM.referencesObj.arrowValueBG.enabled = true;
                    inventoryM.referencesObj.arrowValueBG.GetComponentInChildren<TextMeshProUGUI>().text = inventoryM.bowAndArrowInv.counter[inventoryM.bowAndArrowInv.slotArrowEquipped].ToString();
                }
            }
            if (inventoryM.bowAndArrowInv.itemData[inventoryM.bowAndArrowInv.slotArrowEquipped].itemName == itemData.itemName
            && inventoryM.bowAndArrowInv.itemData[inventoryM.bowAndArrowInv.slotArrowEquipped].equipped)
            {
                inventoryM.DeactiveArrows();
                inventoryM.DeactiveArrowsHP();
                cc.wpnHolster.arrowE = cc.wpnHolster.luRifleE;
                cc.wpnHolster.arrowD = cc.wpnHolster.SevenSixTwoAmmoPrefab;
                cc.wpnHolster.SetItemData(ref cc.wpnHolster.particleArrowE, ref inventoryM.bowAndArrowInv.itemData[inventoryM.bowAndArrowInv.slotArrowEquipped]);
            }
        }

        switch (itemData.itemName)
        {
            case "Silver Bar":
                for (int i = 0; i < inventoryM.itemIconData.Length; i++)
                {
                    if (itemData.originItemName == inventoryM.itemIconData[i].GetComponentInChildren<ItemData>().originItemName)
                        sprite.sprite = inventoryM.itemIconData[i].GetComponentInChildren<ItemData>().itemSprite;
                }
                break;
            case "Gold Bar":
                for (int i = 0; i < inventoryM.itemIconData.Length; i++)
                {
                    if (itemData.originItemName == inventoryM.itemIconData[i].GetComponentInChildren<ItemData>().originItemName)
                        sprite.sprite = inventoryM.itemIconData[i].GetComponentInChildren<ItemData>().itemSprite;
                }
                break;
            case "Ruby":
                for (int i = 0; i < inventoryM.itemIconData.Length; i++)
                {
                    if (itemData.originItemName == inventoryM.itemIconData[i].GetComponentInChildren<ItemData>().originItemName)
                        sprite.sprite = inventoryM.itemIconData[i].GetComponentInChildren<ItemData>().itemSprite;
                }
                break;
            case "Emerald":
                for (int i = 0; i < inventoryM.itemIconData.Length; i++)
                {
                    if (itemData.originItemName == inventoryM.itemIconData[i].GetComponentInChildren<ItemData>().originItemName)
                        sprite.sprite = inventoryM.itemIconData[i].GetComponentInChildren<ItemData>().itemSprite;
                }
                break;
            case "Elixir of Health":
                for (int i = 0; i < inventoryM.itemIconData.Length; i++)
                {
                    if (itemData.originItemName == inventoryM.itemIconData[i].GetComponentInChildren<ItemData>().originItemName)
                        sprite.sprite = inventoryM.itemIconData[i].GetComponentInChildren<ItemData>().itemSprite;
                }
                break;
            case "Elixir of Enegry":
                for (int i = 0; i < inventoryM.itemIconData.Length; i++)
                {
                    if (itemData.originItemName == inventoryM.itemIconData[i].GetComponentInChildren<ItemData>().originItemName)
                        sprite.sprite = inventoryM.itemIconData[i].GetComponentInChildren<ItemData>().itemSprite;
                }
                break;
            case "Key":
                for (int i = 0; i < inventoryM.itemIconData.Length; i++)
                {
                    if (itemData.originItemName == inventoryM.itemIconData[i].GetComponentInChildren<ItemData>().originItemName)
                        sprite.sprite = inventoryM.itemIconData[i].GetComponentInChildren<ItemData>().itemSprite;
                }
                break;
        }
    }

    #endregion

    #region Reset Menu Slots

    void ResetQuest()
    {
        ResetQuestSlots(ref questM.mainQuestSlots.questName, ref questM.mainQuestSlots.questLevel, ref questM.mainQuestSlots.questDistance,
        ref questM.mainQuestSlots.questLocationName, ref questM.mainQuestSlots.highLight, ref questM.mainQuestSlots.questData);

        ResetQuestSlots(ref questM.sideQuestSlots.questName, ref questM.sideQuestSlots.questLevel, ref questM.sideQuestSlots.questDistance,
        ref questM.sideQuestSlots.questLocationName, ref questM.sideQuestSlots.highLight, ref questM.sideQuestSlots.questData);
    }

    protected void ResetQuestSlots(ref TextMeshProUGUI[] questName, ref TextMeshProUGUI[] questLevel, ref TextMeshProUGUI[] questDistance,
    ref TextMeshProUGUI[] questLocationName, ref Image[] highLight, ref QuestData[] questData)
    {
        questName = new TextMeshProUGUI[0];
        questLevel = new TextMeshProUGUI[0];
        questDistance = new TextMeshProUGUI[0];
        questLocationName = new TextMeshProUGUI[0];
        highLight = new Image[0];
        questData = new QuestData[0];
    }

    public void ResetInventory()
    {
        ResetInventorySlots(ref inventoryM.weaponInv.image, ref inventoryM.weaponInv.highLight,
        ref inventoryM.weaponInv.outLineBorder, ref inventoryM.weaponInv.counter, ref inventoryM.weaponInv.quantity,
        ref inventoryM.weaponInv.itemData);

        ResetInventorySlots(ref inventoryM.bowAndArrowInv.image, ref inventoryM.bowAndArrowInv.highLight,
        ref inventoryM.bowAndArrowInv.outLineBorder, ref inventoryM.bowAndArrowInv.counter, ref inventoryM.bowAndArrowInv.quantity,
        ref inventoryM.bowAndArrowInv.itemData);

        ResetInventorySlots(ref inventoryM.shieldInv.image, ref inventoryM.shieldInv.highLight,
        ref inventoryM.shieldInv.outLineBorder, ref inventoryM.shieldInv.counter, ref inventoryM.shieldInv.quantity,
        ref inventoryM.shieldInv.itemData);

        ResetInventorySlots(ref inventoryM.armorInv.image, ref inventoryM.armorInv.highLight,
        ref inventoryM.armorInv.outLineBorder, ref inventoryM.armorInv.counter, ref inventoryM.armorInv.quantity,
        ref inventoryM.armorInv.itemData);

        ResetInventorySlots(ref inventoryM.materialInv.image, ref inventoryM.materialInv.highLight,
        ref inventoryM.materialInv.outLineBorder, ref inventoryM.materialInv.counter, ref inventoryM.materialInv.quantity,
        ref inventoryM.materialInv.itemData);

        ResetInventorySlots(ref inventoryM.healingInv.image, ref inventoryM.healingInv.highLight,
        ref inventoryM.healingInv.outLineBorder, ref inventoryM.healingInv.counter, ref inventoryM.healingInv.quantity,
        ref inventoryM.healingInv.itemData);

        ResetInventorySlots(ref inventoryM.keyInv.image, ref inventoryM.keyInv.highLight,
        ref inventoryM.keyInv.outLineBorder, ref inventoryM.keyInv.counter, ref inventoryM.keyInv.quantity,
        ref inventoryM.keyInv.itemData);
    }

    protected void ResetInventorySlots(ref Image[] image, ref Image[] highLight, ref Image[] outLineBorder, ref int[] counter, ref TextMeshProUGUI[] quantity, ref ItemData[] itemData)
    {
        image = new Image[0];
        counter = new int[0];
        quantity = new TextMeshProUGUI[0];
        highLight = new Image[0];
        outLineBorder = new Image[0];
        itemData = new ItemData[0];
    }

    #endregion

    void ControllerLayout()
    {
        if (inventoryM.pauseMenuNavigation == 4)
        {
            switch (cc.controllerType)
            {
                case ThirdPersonInput.ControllerType.MOUSEKEYBOARD:
                    KeyboardFadeUI.canvasGroup.alpha = 1;
                    XboxFadeUI.canvasGroup.alpha = 0;
                    PSFadeUI.canvasGroup.alpha = 0;
                    break;
                case ThirdPersonInput.ControllerType.XBOX:
                    KeyboardFadeUI.canvasGroup.alpha = 0;
                    XboxFadeUI.canvasGroup.alpha = 1;
                    PSFadeUI.canvasGroup.alpha = 0;
                    break;
                case ThirdPersonInput.ControllerType.PLAYSTATION:
                    KeyboardFadeUI.canvasGroup.alpha = 0;
                    XboxFadeUI.canvasGroup.alpha = 0;
                    PSFadeUI.canvasGroup.alpha = 1;
                    break;
            }
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
