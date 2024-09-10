using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPGAE.CharacterController;

public class TriggerAction : MonoBehaviour, DamageReceiver
{
    [Header("ACTIVATION OPTIONS")]
    public int side;
    public bool isActive;
    public bool AutoTriggered;
    [Tooltip("Start count down active time if the value is higher than 0")]
    public float isActiveTimer;
    public InteractionType interactionType;
    public AIController[] requiredDeaths;
    public TriggerAction[] requiredTriggers;
    public RandomAudioPlayer activationSnd;
    public RandomAudioPlayer requiredTriggerSnd;
    public Color neutralColor;
    public Color lockedColor;
    public Color unlockedColor;
    public TriggerActionDependance triggerActionDependance;
    public TriggerAction[] thisGroupOfTriggerActions;

    [Header("TREASURE CHEST")]
    public GameObject item;

    [Header("CINEMATIC CAMERA")]
    public Transform newCamPos;
    public Transform newCamLookAt;
    [Tooltip("Any value higher than zero will trigger a count down")]
    public float cameraStayTime;
    public CameraState cameraState;

    [Header("QUEST SETTINGS")]
    [Tooltip("The name of the current quest you're progressing through.")]
    public string questSequenceNameReceipt;
    [Tooltip("The required phase number in order to progress to the next.")]
    public int questPhaseReceipt;
    [Tooltip("Once the required action is meet you will increment a phase.")]
    public int addQuestPhaseBy = 1;
    [Tooltip("Once the required action is meet you will increment a quantity amount.")]
    public int addQuestQuantityBy;

    [Header("REFERENCES")]
    public Animator animator;
    public Renderer material;
    public ParticleSystem particleS;
    public Breakable breakable;
    public Transformer transformer;
    public GameObject interactiveUI;

    [Header("TRIGGER ACTION OPTIONS")]
    [Tooltip("Automatically execute the action without the need to press a Button")]
    public bool autoAction;
    [Tooltip("Automatically execute the action without the need to press a Button")]
    public ButtonType buttonType;
    [Tooltip("Disable the the Capsule Collider Collision of the Player")]
    public bool disableCollision = true;
    [Tooltip("Disable the Rigibody Gravity of the Player")]
    public bool disableGravity = true;
    [Tooltip("Reset Player Gravity and Collision at the end of the animation")]
    public bool resetPlayerSettings = true;
    [Tooltip("Trigger an Animation - Use the exactly same name of the AnimationState you want to trigger")]
    public string playAnimation;
    [Tooltip("Check the Exit Time of your animation and insert here")]
    public float startTimeAnimation = 0;
    [Tooltip("Check the Exit Time of your animation and insert here")]
    public float endExitTimeAnimation = 0.8f;
    [Tooltip("Select the transform you want to use as reference to the Match Target")]
    public AvatarTarget avatarTarget;
    [Tooltip("Check what position XYZ you want the matchTarget to work")]
    public Vector3 matchTargetMask;
    [Tooltip("Use a transform to help the character climb any height, take a look at the Example Scene ClimbUp, StepUp, JumpOver objects.")]
    public Transform matchTarget;
    [Tooltip("Start the match target of the animation")]
    public float startMatchTarget;
    [Tooltip("End the match target of the animation")]
    public float endMatchTarget;
    [Tooltip("Use this to limit the trigger to active if forward of character is close to this forward")]
    public bool activeFromForward;
    [Tooltip("Rotate Character for this rotation when active")]
    public bool useTriggerRotation;
    [Tooltip("Destroy this TriggerAction after press the input or do the auto action")]
    public bool destroyAfter = false;
    public bool destroyInteractUI = false;
    [Tooltip("Delay to destroy the TriggerAction")]
    public float destroyDelay = 0f;
    [Tooltip("Delay to run the OnDoAction Event")]
    public float onDoActionDelay;

    #region Private

    private ThirdPersonCamera cam;
    private InventoryManager inventoryM;
    private ThirdPersonController cc;
    private QuestManager questM;

    [HideInInspector] public Transform originCamPos;

    public bool onTriggerEnterUI;
    private bool isActiveChecked;
    public int triggeredAmount;
    private bool triggerDone;
    private float triggerTimerStart;
    private bool triggerTimerActive;
    private bool questObjectiveDone;

    [HideInInspector] public bool isLocked;
    [HideInInspector] public bool wasLocked;
    [HideInInspector] public bool m_isActive;
    [HideInInspector] public bool wasInContact;

    public enum ButtonType
    {
        InteractButton,
        ActionButton,
        NA
    }

    public enum InteractionType
    {
        Normal,
        Triggered,
        Key,
        PressureStep,
        PressureObjectWeight,
        permanentlyLocked
    }

    public enum CameraState
    {
        LookAt, Tween, orbit
    }

    public enum ActionType
    {
        Common,
        Door,
        TreasureChest
    }

    public enum CollidedWith
    {
        Player,
        Breakable,
        Other
    }
    public CollidedWith collidedWith;

    public enum TriggerActionDependance
    {
        Single,
        Multiple
    }


    #endregion

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.tag = "Action";
        if (matchTarget != null)
        {
            GetComponent<Collider>().isTrigger = true;
        }
        this.gameObject.layer = LayerMask.NameToLayer("Triggers");

        if (requiredTriggers.Length > 0)
            interactionType = InteractionType.Triggered;

        if(triggerActionDependance == TriggerActionDependance.Multiple)
        {
            thisGroupOfTriggerActions = gameObject.transform.parent.gameObject.GetComponentsInChildren<TriggerAction>();
        }

        if (breakable == null)
            breakable = GetComponentInParent<Breakable>();

        if (interactiveUI != null)
        {
            if (interactiveUI.GetComponent<InteractWorldSpaceUI>() != null)
                interactiveUI.GetComponent<InteractWorldSpaceUI>().ToggleIcon(false);
            else if (interactiveUI.GetComponent<InteractOverlayUI>() != null)
                interactiveUI.GetComponent<InteractOverlayUI>().fadeUI.FadeTransition(0, 0, 0.5f);
        }

        cam = FindObjectOfType<ThirdPersonCamera>();
        inventoryM = FindObjectOfType<InventoryManager>();
        cc = FindObjectOfType<ThirdPersonController>();
        questM = FindObjectOfType<QuestManager>();
    }

    void Update()
    {
        InteractUI();
        UnlockUpdate();
        TriggerTimerUpdate();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            wasInContact = true;
            onTriggerEnterUI = true;
        }

        if (!isActive && other.CompareTag("Player") && interactionType == InteractionType.PressureStep)
        {
            collidedWith = CollidedWith.Player;
            triggerTimerActive = true;
        }

        if (!isActive && other.CompareTag("Player") && interactionType == InteractionType.PressureObjectWeight)
            cc.infoMessage.info.text = "Carry and place an object on this pressure pad";

        if (!isActive && other.GetComponent<Breakable>() && interactionType == InteractionType.PressureObjectWeight)
        {
            collidedWith = CollidedWith.Breakable;
            triggerTimerActive = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onTriggerEnterUI = false;
        }

        if (!isActive && collidedWith == CollidedWith.Player && interactionType == InteractionType.PressureStep)
        {
            triggerTimerActive = true;
            triggerTimerStart = 0;
        }

        if (!isActive && collidedWith == CollidedWith.Breakable && interactionType == InteractionType.PressureObjectWeight)
        {
            triggerTimerActive = false;
            triggerTimerStart = 0;
        }
    }

    public void UnlockUpdate()
    {
        if (interactionType == InteractionType.Key
        || interactionType == InteractionType.Triggered
        || interactionType == InteractionType.permanentlyLocked)
        {
            isLocked = true;
            wasLocked = true;
            if (material)
                SetMaterialColor(lockedColor);
        }
        else if (interactionType == InteractionType.Normal && wasLocked)
        {
            if (material)
                SetMaterialColor(unlockedColor);

            isLocked = false;
            wasLocked = false;
        }

        RequiredTriggersMethod();
        RequiredDeathsMethod();

        if (isActive)
        {
            #region Quest Progression

            for (int i = 0; i < questM.mainQuestSlots.questData.Length; i++)
            {
                if (questM.mainQuestSlots.questData[i].questName == questSequenceNameReceipt &&
                questM.mainQuestSlots.questData[i].inProgress && !questObjectiveDone)
                {
                    questM.AddQuestObjective(ref questSequenceNameReceipt, ref questPhaseReceipt, ref addQuestPhaseBy, ref addQuestQuantityBy);
                    questObjectiveDone = true;
                }
            }

            for (int i = 0; i < questM.sideQuestSlots.questData.Length; i++)
            {
                if (questM.sideQuestSlots.questData[i].questName == questSequenceNameReceipt &&
                questM.sideQuestSlots.questData[i].inProgress && !questObjectiveDone)
                {
                    questM.AddQuestObjective(ref questSequenceNameReceipt, ref questPhaseReceipt, ref addQuestPhaseBy, ref addQuestQuantityBy);
                    questObjectiveDone = true;
                }
            }

            #endregion

            if (AutoTriggered)
                Activate();
            if (material)
                SetMaterialColor(unlockedColor);
        }
        if (isActive == false)
            m_isActive = false;
    }

    void Activate()
    {
        if (!m_isActive)
        {
            if (animator)
            {
                animator.SetBool("Activate", true);
                animator.SetInteger("Side", side);
            }

            if (isActiveTimer > 0)
                StartCoroutine(ActiveTimer(isActiveTimer));

            if (transformer)
            {
                for (int i = 0; i < thisGroupOfTriggerActions.Length; i++)
                {
                    thisGroupOfTriggerActions[i].isActive = true;
                }
                if (transformer.transformType == Transformer.TransformType.ToEndPointAndBack)
                {
                    if (transformer.invert == true)
                        transformer.invert = false;
                }
                transformer.pause = false;
            }

            // Play particle system
            if (particleS)
            {
                particleS.gameObject.SetActive(false);
                particleS.gameObject.SetActive(true);
            }

            // Play activation sound
            if (activationSnd && !item)
                activationSnd.PlayRandomClip();

            m_isActive = true;
        }
    }

    public virtual IEnumerator ActiveTimer(float timer)
    {
        // Let the trigger know it's been activated on one side,
        // so the UI buttons associated with it 'deactivate' as well 
        for (int i = 0; i < thisGroupOfTriggerActions.Length; i++)
        {
            thisGroupOfTriggerActions[i].isActive = true;
        }
        yield return new WaitForSeconds(timer);
        for (int i = 0; i < thisGroupOfTriggerActions.Length; i++)
        {
            thisGroupOfTriggerActions[i].isActive = false;
        }
        if (animator)
            animator.SetBool("Activate", false);

        isActive = false;
        m_isActive = false;
    }

    void RequiredTriggersMethod()
    {
        foreach (TriggerAction action in requiredTriggers)
        {
            if (action.isActive && !action.isActiveChecked)
            {
                triggeredAmount++;
                action.isActiveChecked = true;
            }
        }
        if (requiredTriggers.Length > 0 && triggeredAmount == requiredTriggers.Length && !triggerDone)
        {
            if(newCamPos != null && newCamLookAt != null)
            {
                cc.tpCam.SetCinematicCamera((ThirdPersonCamera.CameraState)cameraState, newCamPos, newCamLookAt, cameraStayTime);

                if ((cam.transform.position - cam.newCamPos.position).magnitude < 0.1f)
                {
                    isLocked = false;
                    wasLocked = false;
                    interactionType = InteractionType.Normal;

                    if (AutoTriggered)
                        isActive = true;

                    if (requiredTriggerSnd)
                        requiredTriggerSnd.PlayRandomClip();
                    if (material)
                        SetMaterialColor(unlockedColor);

                    triggerDone = true;
                }
            }
            else
            {
                isLocked = false;
                wasLocked = false;
                interactionType = InteractionType.Normal;

                if (AutoTriggered)
                    isActive = true;

                if (requiredTriggerSnd)
                    requiredTriggerSnd.PlayRandomClip();
                if (material)
                    SetMaterialColor(unlockedColor);

                triggerDone = true;
            }
        }
    }

    void RequiredDeathsMethod()
    {
        foreach (AIController controller in requiredDeaths)
        {
            if (!controller.isDead && controller.GetComponent<HealthPoints>().curHealthPoints <= 0)
            {
                triggeredAmount++;
                controller.isDead = true;
            }
        }

        if (requiredDeaths.Length > 0 && triggeredAmount == requiredDeaths.Length && !triggerDone)
        {
            if (newCamPos != null && newCamLookAt != null)
            {
                cc.tpCam.SetCinematicCamera((ThirdPersonCamera.CameraState)cameraState, newCamPos, newCamLookAt, cameraStayTime);

                if ((cam.transform.position - cam.newCamPos.position).magnitude < 0.1f)
                {
                    isLocked = false;
                    wasLocked = false;
                    interactionType = InteractionType.Normal;

                    if (AutoTriggered)
                        isActive = true;
                    if (requiredTriggerSnd)
                        requiredTriggerSnd.PlayRandomClip();
                    if (material)
                        SetMaterialColor(unlockedColor);

                    triggerDone = true;
                }
            }
            else
            {
                isLocked = false;
                wasLocked = false;
                interactionType = InteractionType.Normal;

                if (AutoTriggered)
                    isActive = true;

                if (requiredTriggerSnd)
                    requiredTriggerSnd.PlayRandomClip();
                if (material)
                    SetMaterialColor(unlockedColor);

                triggerDone = true;
            }
        }
    }

    void TriggerTimerUpdate()
    {
        if (triggerTimerActive && collidedWith == CollidedWith.Player && interactionType == InteractionType.PressureStep)
        {
            triggerTimerStart += Time.deltaTime;
            if (triggerTimerStart > 0.85f)
            {
                if (activationSnd && !item)
                    activationSnd.PlayRandomClip();
                if (material)
                    SetMaterialColor(unlockedColor);
                if (animator && playAnimation != "OpenChest")
                    animator.SetBool("Activate", true);

                triggerTimerStart = 0;
                triggerTimerActive = false;
                isActive = true;
            }
        }

        if (triggerTimerActive && collidedWith == CollidedWith.Breakable && interactionType == InteractionType.PressureObjectWeight)
        {
            triggerTimerStart += Time.deltaTime;
            if (triggerTimerStart > 0.85f)
            {
                if (activationSnd && !item)
                    activationSnd.PlayRandomClip();
                if (material)
                    SetMaterialColor(unlockedColor);
                if (animator && playAnimation != "OpenChest")
                    animator.SetBool("Activate", true);

                triggerTimerStart = 0;
                triggerTimerActive = false;
                isActive = true;
            }
        }
    }

    void SetMaterialColor(Color color)
    {
        material.material.SetColor("_BaseColor", color);
    }

    void InteractUI()
    {
        if (interactionType == InteractionType.PressureStep || interactionType == InteractionType.PressureObjectWeight)
            return;
        if (interactiveUI == null)
            return;

        bool conditions = cc.isUsingLadder;

        if (onTriggerEnterUI && !isActive && !inventoryM.isPauseMenuOn && !conditions)
        {
            if (interactiveUI.GetComponent<InteractWorldSpaceUI>() && interactiveUI.GetComponent<InteractWorldSpaceUI>().fadeUI.canvasGroup.alpha == 0)
                interactiveUI.GetComponent<InteractWorldSpaceUI>().ToggleIcon(true);
            else if (interactiveUI.GetComponent<InteractOverlayUI>() && interactiveUI.GetComponent<InteractOverlayUI>().fadeUI.canvasGroup.alpha == 0)
                interactiveUI.GetComponent<InteractOverlayUI>().fadeUI.FadeTransition(1, 0, 0.5f);
        }

        if (!onTriggerEnterUI && wasInContact && interactiveUI != null || buttonType == ButtonType.InteractButton && cc.rpgaeIM.PlayerControls.Interact.triggered || 
        buttonType == ButtonType.ActionButton && cc.rpgaeIM.PlayerControls.Action.triggered)
        {
            if (interactiveUI.GetComponent<InteractWorldSpaceUI>() != null && interactiveUI.GetComponent<InteractWorldSpaceUI>().fadeUI.canvasGroup.alpha != 0)
            {
                interactiveUI.GetComponent<InteractWorldSpaceUI>().fadeUI.FadeTransition(0, 0, 0.4f);
            }
            else if (interactiveUI.GetComponent<InteractOverlayUI>() != null && interactiveUI.GetComponent<InteractOverlayUI>().fadeUI.canvasGroup.alpha != 0)
            {
                interactiveUI.GetComponent<InteractOverlayUI>().fadeUI.FadeTransition(0, 0, 0.4f);
            }
        }
    }

    public void OnReceiveMessage(MsgType type, object sender, object data)
    {
        switch (type)
        {
            case MsgType.DAMAGED:
                {
                    HealthPoints.DamageData damageData = (HealthPoints.DamageData)data;
                    Damaged(damageData);
                }
                break;
            case MsgType.DEAD:
                {
                    HealthPoints.DamageData damageData = (HealthPoints.DamageData)data;
                    Die(damageData);
                }
                break;
            default:
                break;
        }
    }

    void Damaged(HealthPoints.DamageData data)
    {
        data.damager.GetComponent<HitBox>().PlayRandomSound("HitObstacleAS", false);
        for (int i = 0; i < data.damager.GetComponent<HitBox>().attackPoints.Length; i++)
        {
            data.damager.GetComponent<HitBox>().CreateParticle(data.damager.GetComponent<HitBox>().effects.obstacleEffectHit,
            data.damager.GetComponent<HitBox>().attackPoints[i].attackRoot.transform.position);
        }
    }

    public void Die(HealthPoints.DamageData data)
    {
        isActive = true;
        data.damager.GetComponent<HitBox>().PlayRandomSound("HitObstacleAS", false);
        for (int i = 0; i < data.damager.GetComponent<HitBox>().attackPoints.Length; i++)
        {
            data.damager.GetComponent<HitBox>().CreateParticle(data.damager.GetComponent<HitBox>().effects.obstacleEffectHit,
            data.damager.GetComponent<HitBox>().attackPoints[i].attackRoot.transform.position);
        }
    }
}
