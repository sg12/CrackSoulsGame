using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using RPGAE.CharacterController;

public class HUDManager : MonoBehaviour 
{
    [Header("PLAYER STATS")]
    public int strength;
    public int defence;
    public int agility;
    public int faith;

    [Header("LEVEL")]
    public float curLevel = 1;
    public float maxLevel = 50;
    public float curExp;
    public float expRequired;
    private float remainingExp;
    public float curSkillPoint;
    [HideInInspector] 
    public float newSkillPoint;

    [Header("LEVEL UI")]
    public Slider levelSlider;
    public FadeUI levelFadeUI;
    public TextMeshProUGUI curLevelText;
    public TextMeshProUGUI nextLevelText;
    public TextMeshProUGUI expCounterText;

    [Header("SKILL POINT UI")]
    public FadeUI fadeUISkillPoint;
    public TextMeshProUGUI newSkillPointText;

    [Header("PLAYER STATS UI")]
    public Slider hpSliderStat;
    public TextMeshProUGUI hpCounterText;
    public Slider epSliderStat;
    public TextMeshProUGUI epCounterText;
    public Slider mpSliderStat;
    public TextMeshProUGUI mpCounterText;
    public TextMeshProUGUI strengthText;
    public TextMeshProUGUI defenceText;
    public TextMeshProUGUI agilityText;
    public TextMeshProUGUI faithText;


    [HideInInspector] public float curHealth;
    [HideInInspector] public float maxHealth = 100f;
    public float curEnegry;
    public float maxEnegry = 100f;
    public float curMana;
    public float maxMana = 100f;

    [Header("HEALTH UI/ENEGRY UI")]
    public Slider healthSlider;
    public Slider healthSliderGhost;
    public Image healthGlow;
    public Slider enegrySlider;
    public Slider enegrySliderGhost;
    public Image enegryGlow;
    public Slider manaSlider;
    public Slider manaSliderGhost;
    public Image manaGlow;
    public bool canCharge;
    public Color minGlow = new Color32(255, 255, 255, 102);
    public Color maxGlow = new Color32(255, 255, 255, 204);

    [Header("INVENTORY HUD UI")]
    public FadeUI keyboardHK;
    public FadeUI xboxHK;
    public FadeUI pSHK;
    public FadeUI hudKBButton1;
    public FadeUI hudKBButton2;
    public Image hudKBButton3;
    public Image hudKBButton4;
    public FadeUI hudXBButton1;
    public FadeUI hudXBButton2;
    public Image hudXBButton3;
    public Image hudXBButton4;
    public FadeUI hudPSButton1;
    public FadeUI hudPSButton2;
    public Image hudPSButton3;
    public Image hudPSButton4;
    public Sprite weaponHUDIcon;
    public Sprite bowHUDIcon;
    public Sprite shieldHUDIcon;
    public Sprite arrowHUDIcon;
    [HideInInspector] public bool swordHUD;
    [HideInInspector] public bool shieldHUD;
    [HideInInspector] public bool bowHUD;
    [HideInInspector] public bool arrowHUD;

    [Header("Crosshair Icon")]
    public FadeUI aimCrossHairAlpha;
    public TextMeshProUGUI aimAmmoText;
    public FadeUI lockOnCrossHairAlpha;

    [Header("Interactive UI Icon Overlay")]
    public InteractOverlayUI interactiveOverlayIcon1;
    public InteractOverlayUI interactiveOverlayIcon2;

    [Header("Current Objective UI")]
    [Header("QUEST HUD / MENU")]
    public TextMeshProUGUI questName;
    public TextMeshProUGUI questCurrentObjective;
    public TextMeshProUGUI questHint;
    public FadeUI fadeUIFadeGroup;
    public FadeUI fadeUIBG;
    public FadeUI fadeUIQuestObjective;
    public Animator currentObjectiveBG;

    [Header("Quest Complete UI")]
    public FadeUI fadeUIQuestCompleteBG;
    public FadeUI fadeUIQuestCompleteTitle;
    public FadeUI fadeUIQuestCompleteEXP;
    public TextMeshProUGUI completedQuestName;
    public TextMeshProUGUI expReward;
    public float curExpReward;

    [Header("System Options")]
    public Toggle invertCameraVertical;
    public Toggle invertCameraHorizontal;
    public Slider cameraSensivitySlider;
    public Toggle sFXToggle;
    public Toggle musicToggle;

    #region Private 

    private ThirdPersonController cc;
    private InventoryManager inventoryM;
    private QuestManager questM;
    private SystemManager systemM; 

    #endregion

    // Use this for initialization
    void Awake () 
    {
        curHealth = maxHealth;
        curEnegry = maxEnegry;
        curMana = maxMana;
        expRequired += 140 * curLevel * 2;

        // Get Components
        questM = FindObjectOfType<QuestManager>();
        inventoryM = FindObjectOfType<InventoryManager>();
        cc = FindObjectOfType<ThirdPersonController>();
        systemM = FindObjectOfType<SystemManager>();

        maxHealth = cc.GetComponent<HealthPoints>().maxHealthPoints;
    }
	
	// Update is called once per frame
	void Update ()
    {

        // Set curHealth to player health points.
        if (!systemM.isLoadingData)
            curHealth = cc.GetComponent<HealthPoints>().curHealthPoints;
        else
            cc.GetComponent<HealthPoints>().curHealthPoints = (int)curHealth;

        GuageSliders();
        LockOnCrossHair();
        Experience();
        SetHUDIcon();
        OptionSettings(); 
        CurrentQuestObjectiveUI();
    }

    void SetHUDIcon()
    {
        if(cc.controllerType == ThirdPersonInput.ControllerType.MOUSEKEYBOARD)
        {
            keyboardHK.canvasGroup.alpha = 1;
            xboxHK.canvasGroup.alpha = 0;
            pSHK.canvasGroup.alpha = 0;
            SetHUDButtonIcon1(ref hudKBButton1, ref hudKBButton2);
            SetHUDButtonIcon2(ref hudKBButton3, ref hudKBButton4);
        }
        else if (cc.controllerType == ThirdPersonInput.ControllerType.XBOX)
        {
            keyboardHK.canvasGroup.alpha = 0;
            xboxHK.canvasGroup.alpha = 1;
            pSHK.canvasGroup.alpha = 0;
            SetHUDButtonIcon1(ref hudXBButton1, ref hudXBButton2);
            SetHUDButtonIcon2(ref hudXBButton3, ref hudXBButton4);
        }
        else if (cc.controllerType == ThirdPersonInput.ControllerType.PLAYSTATION)
        {
            keyboardHK.canvasGroup.alpha = 0;
            xboxHK.canvasGroup.alpha = 0;
            pSHK.canvasGroup.alpha = 1;
            SetHUDButtonIcon1(ref hudPSButton1, ref hudPSButton2);
            SetHUDButtonIcon2(ref hudPSButton3, ref hudPSButton4);
        }

        EnableHUDButtonIcon(ref hudKBButton3);
        EnableHUDButtonIcon(ref hudKBButton4);
        EnableHUDButtonIcon(ref hudXBButton3);
        EnableHUDButtonIcon(ref hudXBButton4);
        EnableHUDButtonIcon(ref hudPSButton3);
        EnableHUDButtonIcon(ref hudPSButton4);
    }

    void SetHUDButtonIcon1(ref FadeUI fadeButton1, ref FadeUI fadeButton2)
    {
        if (cc.wpnHolster.PrimaryWeaponActive())
        {
            fadeButton1.canvasGroup.alpha = 1;
            fadeButton2.canvasGroup.alpha = 1;
        }
        else
        {
            fadeButton1.canvasGroup.alpha = 0;
            fadeButton2.canvasGroup.alpha = 0;
        }
    }

    void SetHUDButtonIcon2(ref Image hudButton1, ref Image hudButton2)
    {
        for (int i = 0; i < inventoryM.weaponInv.itemData.Length; i++)
        {
            if (!cc.wpnHolster.SecondaryActive() && inventoryM.weaponInv.itemData[i].inInventory)
            {
                bowHUD = false;
                hudButton1.sprite = weaponHUDIcon;
                swordHUD = true;
            }
        }

        for (int i = 0; i < inventoryM.shieldInv.itemData.Length; i++)
        {
            if (!cc.wpnHolster.SecondaryActive() && inventoryM.shieldInv.itemData[i].inInventory)
            {
                bowHUD = false;
                hudButton2.sprite = shieldHUDIcon;
                shieldHUD = true;
            }
        }

        if (cc.wpnHolster.SecondaryActive())
        {
            swordHUD = false;
            shieldHUD = false;
            hudButton1.sprite = bowHUDIcon;
            bowHUD = true;
        }

        if (cc.wpnHolster.ArrowActive() || cc.wpnHolster.ArrowOnStringActive())
        {
            swordHUD = false;
            shieldHUD = false;
            hudButton2.sprite = arrowHUDIcon;
            arrowHUD = true;
        }
        if (!shieldHUD && !arrowHUD)
            hudButton2.sprite = null;
    }

    void EnableHUDButtonIcon(ref Image hotkeyIcon)
    {
        if (hotkeyIcon.sprite == null)
            hotkeyIcon.enabled = false;
        else
            hotkeyIcon.enabled = true;
    }

    #region Level/Experience

    void Experience()
    {
        // Player Level shown on HUD
        curLevelText.text = curLevel.ToString();
        int nextLevel = (int)curLevel + 1;
        nextLevelText.text = nextLevel.ToString();
        expCounterText.text = curExp + "/" + expRequired;

        if (!inventoryM.isPauseMenuOn)
        {
            if (curSkillPoint > 0 && fadeUISkillPoint.canvasGroup.alpha == 0)
            {
                fadeUISkillPoint.FadeTransition(1, 0, 0.5f);
            }
        }
        if (curSkillPoint > 0)
            newSkillPointText.text = curSkillPoint.ToString() + " skill points available";
        else
        {
            newSkillPointText.text = "";
            fadeUISkillPoint.canvasGroup.alpha = 0;
        }

        // Player Stats
        hpCounterText.text = curHealth + "/" + maxHealth;
        epCounterText.text = curEnegry + "/" + maxEnegry;
        mpCounterText.text = curMana + "/" + maxMana;
        strengthText.text = "Сила: " + strength;
        defenceText.text = "Броня: " + defence;
        agilityText.text = "Ловкость: " + agility;
        faithText.text = "Вера: " + faith;

        if (curExp >= expRequired)
        {
            if (fadeUIQuestCompleteBG.canvasGroup.alpha == 0)
            {
                remainingExp = expRequired - (curExp + curExpReward);
                curExpReward = 0;
                LevelUp();
            }
        }

        // Quest Completed
        QuestComplete(ref questM.mainQuestSlots.questData, ref questM.mainQuestSlots.highLight);
        QuestComplete(ref questM.sideQuestSlots.questData, ref questM.sideQuestSlots.highLight);
    }

    void LevelUp()
    {
        // Increase levels. 
        curLevel += 1;
        cc.GetComponent<HealthPoints>().curHealthPoints = (int)maxHealth;
        curEnegry = maxEnegry;
        newSkillPoint += 1;
        curSkillPoint = newSkillPoint;

        // Reset parameters.
        curExp = 0;
        curExpReward = 0;
        levelSlider.value = 0;

        // Remaining experience.
        curExp += Mathf.Abs(remainingExp);
        expRequired += 140 * curLevel * 2;

        // Skill points available
        if (fadeUISkillPoint.canvasGroup.alpha == 0 && !inventoryM.isPauseMenuOn) 
            fadeUISkillPoint.FadeTransition(1, 0, 0.5f);
    }

    #endregion

    #region Quest HUD

    /// <summary>
    /// show Quest Complete UI on HUD
    /// </summary>
    public void QuestCompleteUI()
    {
        HideCurrentObjectiveUI(ref questM.mainQuestSlots.questData, ref questM.mainQuestSlots.highLight);

        HideCurrentObjectiveUI(ref questM.sideQuestSlots.questData, ref questM.sideQuestSlots.highLight);

        bool conditions = inventoryM.isPauseMenuOn || inventoryM.storeM.fadeUI.canvasGroup.alpha != 0;
        if (conditions) return;

        // Fade Quest Comelete UI.
        fadeUIQuestCompleteTitle.FadeTransition(1, 1, 1);
        fadeUIQuestCompleteEXP.FadeTransition(1, 2, 1);
        if (fadeUIQuestCompleteBG.canvasGroup.alpha == 0)
            fadeUIQuestCompleteBG.FadeTransition(1, 0, 1);
    }

    protected void QuestReward(ref GameObject[] rewardID)
    {
        for (int i = 0; i < rewardID.Length; i++)
        {
            if (rewardID[i].GetComponentInChildren<ItemData>().itemType == ItemData.ItemType.General)
            {
                if (rewardID[i].GetComponentInChildren<ItemData>().itemName == "Coin" 
                    || rewardID[i].GetComponentInChildren<ItemData>().itemName == "Coin bag")
                {
                    inventoryM.unityCoins += rewardID[i].GetComponent<ItemData>().quantity;
                    inventoryM.unityCoinsText.text = "x" + inventoryM.unityCoins.ToString();
                }
                inventoryM.AddedItemUI(ref rewardID[i]);
                return;
            }
            switch (inventoryM.inventoryS)
            {
                case InventoryManager.InventorySection.Weapon:
                    inventoryM.AddItemDataSlot(ref inventoryM.weaponInv.itemData, ref inventoryM.weaponInv.image, ref rewardID[i]);
                    break;
                case InventoryManager.InventorySection.BowAndArrow:
                    inventoryM.AddItemDataSlot(ref inventoryM.bowAndArrowInv.itemData, ref inventoryM.bowAndArrowInv.image, ref rewardID[i]);
                    break;
                case InventoryManager.InventorySection.Shield:
                    inventoryM.AddItemDataSlot(ref inventoryM.shieldInv.itemData, ref inventoryM.shieldInv.image, ref rewardID[i]);
                    break;
                case InventoryManager.InventorySection.Armor:
                    inventoryM.AddItemDataSlot(ref inventoryM.armorInv.itemData, ref inventoryM.armorInv.image, ref rewardID[i]);
                    break;
                case InventoryManager.InventorySection.Healing:
                    inventoryM.AddItemDataSlot(ref inventoryM.healingInv.itemData, ref inventoryM.materialInv.image, ref rewardID[i]);
                    break;
                case InventoryManager.InventorySection.Material:
                    inventoryM.AddItemDataSlot(ref inventoryM.materialInv.itemData, ref inventoryM.healingInv.image, ref rewardID[i]);
                    break;
                case InventoryManager.InventorySection.Key:
                    inventoryM.AddItemDataSlot(ref inventoryM.keyInv.itemData, ref inventoryM.keyInv.image, ref rewardID[i]);
                    break;
            }
        }
    }

    /// <summary>
    /// show current selected quest details
    /// </summary>
    public void CurrentQuestObjectiveUI()
    {
        if (questM.questAddedfadeUI.canvasGroup.alpha != 0 || fadeUIQuestCompleteBG.canvasGroup.alpha != 0 || systemM.blackScreenFUI.canvasGroup.alpha != 0
            || systemM.loadingScreenFUI.canvasGroup.alpha != 0) 
            return;

        if (!inventoryM.isPauseMenuOn)
        {
            // ---------- Show UI ---------- //
            ShowCurrentObjectiveUI(ref questM.mainQuestSlots.questData);
            ShowCurrentObjectiveUI(ref questM.sideQuestSlots.questData);
        }
        else
        {
            questM.questAddedfadeUI.canvasGroup.alpha = 0;
        }

        questName.text = questM.questName.text;
        CurrentObjectiveSubject(ref questM.mainQuestSlots.questData, ref questM.mainQuestSlots.highLight, ref questM.mainQuestSlots.slotNum);
        CurrentObjectiveSubject(ref questM.sideQuestSlots.questData, ref questM.sideQuestSlots.highLight, ref questM.sideQuestSlots.slotNum);
        questHint.text = questM.questHint.text;
        if (!inventoryM.isPauseMenuOn)
            currentObjectiveBG.SetFloat("Blend", fadeUIQuestObjective.canvasGroup.alpha);
        else
            currentObjectiveBG.SetFloat("Blend", 1);
    }

    protected void CurrentObjectiveSubject(ref QuestData[] questData, ref Image[] highLight, ref int questSlot)
    {
        for (int i = 0; i < questData.Length; i++)
        {
            if (questData[i] != null)
            {
                if (questData[i].isQuestActive == true)
                {
                    if(questData[questSlot].questObjective[questData[i].questObjectiveNum].curCollectableAmount != questData[questSlot].questObjective[questData[i].questObjectiveNum].maxCollectableAmount 
                        && questData[questSlot].questObjective[questData[i].questObjectiveNum].maxCollectableAmount > 0)
                    {
                        questCurrentObjective.text = questData[questSlot].questObjective[questData[i].questObjectiveNum].questObjectiveSubject + " " +
                        questData[questSlot].questObjective[questData[i].questObjectiveNum].curCollectableAmount + "/" +
                        questData[questSlot].questObjective[questData[i].questObjectiveNum].maxCollectableAmount;
                    }
                    else if (questData[questSlot].questObjective[questData[i].questObjectiveNum].curCollectableAmount >= questData[questSlot].questObjective[questData[i].questObjectiveNum].maxCollectableAmount)
                        questCurrentObjective.text = questData[questSlot].questObjective[questData[i].questObjectiveNum].questObjectiveSubject;
                    else if (questData[questSlot].questObjective[questData[i].questObjectiveNum].maxCollectableAmount == 0)
                        questCurrentObjective.text = questData[questSlot].questObjective[questData[i].questObjectiveNum].questObjectiveSubject;

                    if (questData[questSlot].questObjective[questData[i].questObjectiveNum].questObjectiveSubject.Length > 25)
                        questCurrentObjective.fontSize = 20;
                    else
                        questCurrentObjective.fontSize = 21;
                }
            }
        }
    }

    #endregion

    public void ShowInteractiveIcon(string buttonName1, string buttonName2)
    {
        interactiveOverlayIcon1.buttonActionName = buttonName1;
        interactiveOverlayIcon2.buttonActionName = buttonName2;
    }

    public void IncrementExperience(float CurExp)
    {
        curExp += CurExp;
    }

    public void IncrementSkillPoint(float _skillPoint)
    {
        curSkillPoint += _skillPoint;
    }

    public void IncrementHealth(int value, bool smooth)
    {
        cc.GetComponent<HealthPoints>().curHealthPoints += smooth ? value * Time.deltaTime : value;
        cc.GetComponent<HealthPoints>().curHealthPoints = Mathf.Clamp(cc.GetComponent<HealthPoints>().curHealthPoints, 0, maxHealth);
    }

    public void IncrementMaxHealth(int value)
    {
        cc.GetComponent<HealthPoints>().maxHealthPoints += value;
    }

    public void ReceivedDamage(float damage)
    {
        cc.GetComponent<HealthPoints>().curHealthPoints -= damage;
    }

    public void SetmaxEnegry(int value)
    {
        maxEnegry = value;
    }

    public void IncrementEnegry(float value, bool smooth)
    {
        curEnegry += smooth ?  value * Time.deltaTime : value;
        curEnegry = Mathf.Clamp(curEnegry, curEnegry, maxEnegry);
    }

    public void ReduceEnegry(float value, bool smooth)
    {
        curEnegry -= smooth ? value * Time.deltaTime : value;
        if (curEnegry < 0)
        {
            // You are exhuasted.
            curEnegry = 0;
        }
        curEnegry = Mathf.Clamp(curEnegry, 0, maxEnegry);
    }

    /// <summary>
    /// Hide the current objective from HUD
    /// </summary>
    /// <param name="questData">The main Quest or Side Quest Identity</param>
    /// <param name="questHighLight">The quest type BG color</param>
    public void HideCurrentObjectiveUI(ref QuestData[] questData, ref Image[] questHighLight)
    {
        for (int i = 0; i < questData.Length; i++)
        {
            if (questData[i] != null)
            {
                if (questHighLight[i].color == questM.selectColor || questHighLight[i].color == questM.completeColor)
                {
                    fadeUIQuestObjective.canvasGroup.alpha = 0;
                    fadeUIBG.canvasGroup.alpha = 0;
                    fadeUIFadeGroup.canvasGroup.alpha = 0;
                    currentObjectiveBG.SetFloat("Blend", 0);
                    fadeUIQuestObjective.isFading = false;
                    fadeUIBG.isFading = false;
                    fadeUIFadeGroup.isFading = false;
                }
            }
        }
    }

    /// <summary>
    /// Show the current objective on HUD
    /// </summary>
    /// <param name="questData">The main Quest or Side Quest Identity</param>
    /// <param name="questHighLight">The quest type BG color</param>
    void ShowCurrentObjectiveUI(ref QuestData[] questData)
    {
        for (int i = 0; i < questData.Length; i++)
        {
            if (questData[i] != null)
            {
                if (questData[i].isQuestActive)
                {
                    if (!questData[i].isComplete)
                    {
                        if (questM.questAddedfadeUI.canvasGroup.alpha == 0)
                        {
                            if (fadeUIQuestObjective.canvasGroup.alpha == 0)
                                fadeUIQuestObjective.FadeTransition(1, 0, 0.5f);
                            if (fadeUIBG.canvasGroup.alpha == 0)
                                fadeUIBG.FadeTransition(1, 0, 0.5f);

                            if (currentObjectiveBG.GetFloat("Blend") == 1)
                                fadeUIFadeGroup.FadeTransition(1, 2, 0.5f);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// This quest has meet all it's requirements.
    /// </summary>
    /// <param name="questData">The main Quest or Side Quest Identity</param>
    /// <param name="questHighLight">The quest type BG color</param>
    void QuestComplete(ref QuestData[] questData, ref Image[] questHighLight)
    {
        for (int i = 0; i < questData.Length; i++)
        {
            if (questData[i] != null)
            {
                if (questHighLight[i].color == questM.neutralColor || questHighLight[i].color == questM.selectColor)
                {
                    if (questData[i].curObjectivePhase >= questData[i].maxObjectivePhase &&
                        questData[i].questObjective[questData[i].questObjectiveNum].curCollectableAmount >= questData[i].questObjective[questData[i].questObjectiveNum].maxCollectableAmount)
                    {
                        questData[i].isQuestActive = false;

                        HideCurrentObjectiveUI(ref questM.mainQuestSlots.questData, ref questM.mainQuestSlots.highLight);

                        HideCurrentObjectiveUI(ref questM.sideQuestSlots.questData, ref questM.sideQuestSlots.highLight);

                        QuestCompleteUI();

                        completedQuestName.text = questData[i].questName;

                        if (curExpReward >= questData[i].EXPReward)
                        {
                            curExpReward = questData[i].EXPReward;
                            IncrementExperience(curExpReward);
                            fadeUIQuestCompleteBG.FadeTransition(0, 2, 1);

                            // Add quest rewards into inventory.
                            for (int r = 0; r < questM.mainQuestSlots.questData.Length; r++)
                            {
                                QuestReward(ref questM.mainQuestSlots.questData[r].rewardItems);
                            }
                            for (int r = 0; r < questM.sideQuestSlots.questData.Length; r++)
                            {
                                QuestReward(ref questM.sideQuestSlots.questData[r].rewardItems);
                            }

                            fadeUIBG.canvasGroup.alpha = 0;
                            fadeUIFadeGroup.canvasGroup.alpha = 0;
                            fadeUIQuestObjective.canvasGroup.alpha = 0;
                            fadeUIQuestObjective.isFading = false;
                            fadeUIBG.isFading = false;
                            fadeUIFadeGroup.isFading = false;

                            questData[i].isComplete = true;
                            curExpReward = questData[i].EXPReward;
                            questHighLight[i].color = questM.completeColor;
                        }
                        else
                        {
                            expReward.text = curExpReward.ToString() + " XP";
                            if (fadeUIQuestCompleteEXP.canvasGroup.alpha == 1) curExpReward += 0.5f * 60;
                        }
                    }
                }
            }
        }
    }

    void GuageSliders()
    {
        // PLAYER GUAGE GLOW
        if (Mathf.Abs(healthSlider.value - maxHealth) < 0.35) { healthGlow.color = Color.Lerp(minGlow, maxGlow, Mathf.PingPong(Time.time, 1)); }
        else { healthGlow.color = minGlow; }
        if (Mathf.Abs(enegrySlider.value - maxEnegry) < 0.35) { enegryGlow.color = Color.Lerp(minGlow, maxGlow, Mathf.PingPong(Time.time, 1)); }
        else { enegryGlow.color = minGlow; }
        if (Mathf.Abs(manaSlider.value - maxMana) < 0.35) { manaGlow.color = Color.Lerp(minGlow, maxGlow, Mathf.PingPong(Time.time, 1)); }
        else { enegryGlow.color = minGlow; }

        // MAX VALUE SLIDER
        if (levelSlider.maxValue != expRequired)
            levelSlider.maxValue = expRequired;
        if (healthSlider.maxValue != maxHealth)
            healthSlider.maxValue = maxHealth;
        if (healthSliderGhost.maxValue != maxHealth)
            healthSliderGhost.maxValue = maxHealth;
        if (enegrySlider.maxValue != maxEnegry)
            enegrySlider.maxValue = maxEnegry;
        if (enegrySliderGhost.maxValue != maxEnegry)
            enegrySliderGhost.maxValue = maxEnegry;

        // GHOST SLIDER
        if (Mathf.Abs(healthSlider.value - curHealth) < 0.35f)
            healthSliderGhost.value = Mathf.Lerp(healthSliderGhost.value, curHealth, 15 * Time.fixedDeltaTime);
        if (Mathf.Abs(enegrySlider.value - curEnegry) < 0.35f)
            enegrySliderGhost.value = Mathf.Lerp(enegrySliderGhost.value, curEnegry, 15 * Time.fixedDeltaTime);
        if (Mathf.Abs(manaSlider.value - curMana) < 0.35f)
            manaSliderGhost.value = Mathf.Lerp(manaSliderGhost.value, curMana, 15 * Time.fixedDeltaTime);

        // LEVEL SLIDER
        levelSlider.value = Mathf.Lerp(levelSlider.value, curExp, 15 * Time.fixedDeltaTime);

        if (levelFadeUI.canvasGroup.alpha == 1 && !inventoryM.isPauseMenuOn) 
            levelFadeUI.FadeTransition(0, 3, 0.5f);

        if (Mathf.Abs(levelSlider.value - curExp) > 0.35f && systemM.loadingScreenFUI.canvasGroup.alpha == 0)
            levelFadeUI.FadeTransition(1, 0, 0.5f);

        // PLAYER GUAGE SLIDER
        healthSlider.value = Mathf.Lerp(healthSlider.value, curHealth, 15 * Time.fixedDeltaTime);
        enegrySlider.value = Mathf.Lerp(enegrySlider.value, curEnegry, 15 * Time.fixedDeltaTime);

        // PLAYER STATS SLIDER
        hpSliderStat.value = Mathf.Lerp(hpSliderStat.value, curHealth, 15 * Time.fixedDeltaTime);
        epSliderStat.value = Mathf.Lerp(epSliderStat.value, curEnegry, 15 * Time.fixedDeltaTime);
        mpSliderStat.value = Mathf.Lerp(mpSliderStat.value, curMana, 15 * Time.fixedDeltaTime);


        bool conditionsToCharge = cc.IsAnimatorTag("Light Attack") || cc.IsAnimatorTag("Heavy Attack") 
            || cc.IsAnimatorTag("Aerial Attack") || cc.IsAnimatorTag("Ability Attack");
        if (Mathf.Abs(enegrySliderGhost.value - curEnegry) < 1 && !conditionsToCharge)
        {
            IncrementEnegry(15, true);
        }
        else
        {
            canCharge = false;
        }
    }

    void LockOnCrossHair()
    {
        if (cc.tpCam == null) return;

        if (cc.tpCam.lockedOn)
        {
            if (lockOnCrossHairAlpha.canvasGroup.alpha == 0)
                lockOnCrossHairAlpha.FadeTransition(1, 0, 0.2f);

            cc.tpCam.lastCrossHairPosition = cc.tpCam.lockedOnTarget.targetTransform;

            lockOnCrossHairAlpha.gameObject.transform.position =
            cc.tpCam.cam.WorldToScreenPoint(cc.tpCam.lockedOnTarget.targetTransform.position);
        }
        else
        {
            if (cc.isAiming)
            {
                if (cc.wpnHolster.SecondaryActive() && cc.wpnHolster.arrowE != null)
                    aimAmmoText.text = inventoryM.bowAndArrowInv.counter[inventoryM.bowAndArrowInv.slotArrowEquipped].ToString();
                else
                    aimAmmoText.text = "";

                if (aimCrossHairAlpha.canvasGroup.alpha == 0)
                    aimCrossHairAlpha.FadeTransition(1, 0, 0.2f);
            }
            else
            {
                if (aimCrossHairAlpha.canvasGroup.alpha == 1)
                    aimCrossHairAlpha.FadeTransition(0, 0, 0.2f);
                if (lockOnCrossHairAlpha.canvasGroup.alpha == 1)
                    lockOnCrossHairAlpha.FadeTransition(0, 0, 0.2f);

                if (cc.tpCam.lastCrossHairPosition != null)
                {
                    lockOnCrossHairAlpha.gameObject.transform.position =
                    cc.tpCam.cam.WorldToScreenPoint(cc.tpCam.lastCrossHairPosition.position);
                }
            }
        }
    }

    void OptionSettings()
    {
        if (inventoryM.referencesObj.systemSection.activeInHierarchy)
        {
            systemM.invertCameraV = invertCameraVertical.isOn;
            systemM.invertCameraH = invertCameraHorizontal.isOn;
            systemM.cameraSensitivity = cameraSensivitySlider.value;
            systemM.SFX = sFXToggle.isOn;
            systemM.music = musicToggle.isOn;
        }
        else
        {
            invertCameraVertical.isOn = systemM.invertCameraV;
            invertCameraHorizontal.isOn = systemM.invertCameraH;
            cameraSensivitySlider.value = systemM.cameraSensitivity;
            sFXToggle.isOn = systemM.SFX;
            musicToggle.isOn = systemM.music;
        }

        if (!systemM.music)
        {
            if (FindObjectOfType<SoundTrack>() != null)
                FindObjectOfType<SoundTrack>().enabled = false;
        }
        else if (systemM.music && systemM.loadingScreenFUI.canvasGroup.alpha < 1 && systemM.blackScreenFUI.canvasGroup.alpha < 1)
        {
            if (FindObjectOfType<SoundTrack>() != null)
                FindObjectOfType<SoundTrack>().enabled = true;
        }
    }
}
