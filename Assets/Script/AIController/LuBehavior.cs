using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine;
using RPGAE.CharacterController;

public class LuBehavior : MonoBehaviour, DamageReceiver
{
    [Header("COMBAT SETTINGS")]
    public GameObject rifle;
    public GameObject bullets;
    public Transform bulletPrefabSpot;
    public ParticleSystem muzzleFlash;
    public GameObject grenade;
    public GameObject grenadeInHand;
    public Transform grenadePrefabSpot;
    public TargetScanner targetScanner;
    public float timeToStopPursuit = 6;
    [Range(2, 100)]
    public float pursuitStoppingDistance = 0.5f;
    [Range(0, 100)]
    public float minAtkDistance;
    [Range(2, 100)]
    public float maxAtkDistance;
    public float shootOnAimTimer;
    public float atkRecoveryRate;
    private float atkRecoveryTimer;
    public float stunDeflectMaxTime = 3;
    public float stunFinisherMaxTime = 8;

    [Header("AUDIO")]
    public RandomAudioPlayer rifleShotAudio;
    public RandomAudioPlayer rifleReloadAudio;

    [Header("ON DEATH")]
    public int Exp = 0;
    public float dropItemTimerMax = 3;
    [Tooltip("Specfic items that would drop upon it's death")]
    public GameObject[] itemDrops;

    float oldPosX, oldPosZ;
    private float itemDropTimer;

    [HideInInspector] public bool isAttacking;
    [HideInInspector] public float signedAngle;

    private Transform leftHandIK;
    private bool isDashing;

    #region Hide

    private State state;
    private float lookWeight;
    private bool dialogueOneShot;

    private QuestData questData;
    private Quaternion oldLookDirection;

    private bool stunned;
    private float curStunnedRecoveryTime;
    private float m_TimerSinceLostTarget = 0.0f;
    private bool battleStanceDecisionMade;

    private float distanceFromTarget;
    private float horizontalMovement;
    private bool isReloading;

    public enum State
    {
        Pursuit,
        CombatStance,
        Stunned,
        Dead
    }

    [HideInInspector] public bool isShooting;
    [HideInInspector] public float idleWaitTimer;

    [HideInInspector] public NavMeshHit navHit;
    [HideInInspector] public Vector3 originalPosition;
    [HideInInspector] public GameObject enemyTarget;
    [HideInInspector] public AIController controller;

    [HideInInspector] public HUDManager hudM;
    [HideInInspector] public DialogueManager dialogueM;
    [HideInInspector] public ThirdPersonController cc;
    [HideInInspector] public HealthPoints hp;
    [HideInInspector] public AIHealthGuage healthGuage;
    [HideInInspector] public BloodEffect bloodEffect;
    [HideInInspector] public BossHealthGuage bossHP;
    [HideInInspector] public QuestManager questM;

    #endregion

    void Start()
    {
        atkRecoveryTimer = atkRecoveryRate;
        controller = GetComponentInChildren<AIController>();

        questData = GetComponentInChildren<QuestData>();

        hudM = FindObjectOfType<HUDManager>();
        cc = FindObjectOfType<ThirdPersonController>();
        dialogueM = FindObjectOfType<DialogueManager>();
        hp = GetComponentInChildren<HealthPoints>();
        healthGuage = GetComponentInChildren<AIHealthGuage>();
        bloodEffect = GetComponentInChildren<BloodEffect>();
        bossHP = FindObjectOfType<BossHealthGuage>();
        questM = FindObjectOfType<QuestManager>();

        controller.animator.GetBoneTransform(HumanBodyBones.Head).tag = "HeadShot";

        originalPosition = transform.position;
        oldPosX = transform.position.x;
        oldPosZ = transform.position.z;

        controller.fullBodyWeight = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (cc.inventoryM.isPauseMenuOn || cc.inventoryM.inventoryHUD.isActive) return;

        switch (state)
        {
            case State.Pursuit:
                PursuitState();
                break;
            case State.CombatStance:
                CombatStanceState();
                break;
            case State.Stunned:
                StunnedState();
                break;
            case State.Dead:
                DeathDrop();
                break;
        }

        BossHP();
        FindTarget();
        WeaponAim();
        WeaponArmsWeight();
        AttackRecoveryTimer();

        controller.animator.SetBool("Stunned", stunned);
        controller.animator.SetFloat("CurStunPoints", hp.curStunPoints);
        controller.animator.SetBool("IsShooting", isShooting);
        controller.animator.SetFloat("AttackRecovery", atkRecoveryTimer);
        controller.animator.SetBool("Reloading", isReloading);
        controller.animator.SetBool("Grounded", controller.grounded);
        controller.animator.SetFloat("WalkPivotAngle", signedAngle);
    }

    void BossHP()
    {
        if (bossHP == null) return;

        bossHP.healthPoints = GetComponent<HealthPoints>();
        if (hp.curHealthPoints <= 0 && bossHP.fadeUI.canvasGroup.alpha == 1)
        {
            bossHP.fadeUI.FadeTransition(0, 1, 0.5f);
        }
    }

    void WeaponArmsWeight()
    {
        if (controller.baseLayerInfo.IsTag("Attack"))
        {
            if (controller.baseLayerInfo.normalizedTime < 0.8f)
            {
                controller.leftArmLayerWeight = 0;
                controller.rightArmLayerWeight = 0;
            }
            else
            {
                controller.leftArmLayerWeight = Mathf.Lerp(controller.leftArmLayerWeight, 1, 12 * Time.deltaTime);
                controller.rightArmLayerWeight = Mathf.Lerp(controller.rightArmLayerWeight, 1, 12 * Time.deltaTime);
            }
        }

        if (controller.baseLayerInfo.IsName("Heavy Attack"))
        {
            if(controller.baseLayerInfo.normalizedTime < 0.63f)
            {
                controller.leftArmLayerWeight = 0;
                controller.rightArmLayerWeight = 0;
            }
            else
            {
                controller.leftArmLayerWeight = Mathf.Lerp(controller.leftArmLayerWeight, 1, 12 * Time.deltaTime);
                controller.rightArmLayerWeight = Mathf.Lerp(controller.rightArmLayerWeight, 1, 12 * Time.deltaTime);
            }
        }

        if (controller.baseLayerInfo.IsName("Heavy Charge") || controller.baseLayerInfo.IsTag("Aim") || controller.animator.GetFloat("TalkID") > 0)
        {
            controller.leftArmLayerWeight = Mathf.Lerp(controller.leftArmLayerWeight, 0, 12 * Time.deltaTime);
            controller.rightArmLayerWeight = Mathf.Lerp(controller.rightArmLayerWeight, 0, 12 * Time.deltaTime);
        }
    }

    public void GetPivotAngle(Vector3 position)
    {
        Vector3 direction = position - transform.position;

        float forwardWeight = Vector3.Dot(direction, transform.forward);
        float rightWeight = Vector3.Dot(direction, transform.right);

        float forwardMag = Mathf.Abs(forwardWeight);
        float rightMag = Mathf.Abs(rightWeight);

        if (forwardMag >= rightMag)
        {
            if (forwardWeight > 0.0f)
                signedAngle = 0f;
            else
                signedAngle = -180f;
        }
        else if (rightMag >= forwardMag)
        {
            if (rightWeight > 0.0f)
                signedAngle = 90f;
            else
                signedAngle = -90f;
        }
        battleStanceDecisionMade = false;
    }

    #region Dialogue

    void StartDialogueWithPlayer()
    {
        controller.dialogueAltered = controller.dialogue.dialogueAltered;

        if (controller.dialogue.npcName == "")
            return;

        if (!controller.dialogueActive)
            DefaultIdleDirection();

        if (controller.dialogueActive)
        {
            // look at NPC
            LookAtTargetOnY(transform, cc.transform);
            // look at player
            LookAtTargetOnY(cc.transform, transform);
        }

        if (!controller.dialogueActive)
        {
            switch (controller.dialogue.dialogueType)
            {
                case Dialogue.DialogueType.QuestInProgress:
                    if (dialogueM.currentQuest != null)
                    {
                        dialogueM.previousQuest = dialogueM.currentQuest;
                        dialogueM.currentQuest = questData;
                    }
                    else
                        dialogueM.currentQuest = questData;
                    break;
            }

            cc.canMove = false;
            cc.targetDirection = Vector3.zero;
            cc.input = Vector3.zero;
            cc.animator.SetFloat("InputMagnitude", 0);
            cc.animator.SetFloat("InputVertical", 0);
            cc.animator.SetFloat("InputVertical", 0);
            cc.animator.SetFloat("InputHorizontal", 0);
            dialogueM.StartDialogue(controller.dialogue);
            dialogueM.fadeUI.FadeTransition(1, 0, 0.5f);

            controller.dialogueActive = true;
        }
    }

    void DefaultIdleDirection()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, oldLookDirection, 6 * Time.fixedDeltaTime);
    }

    void LookAtTargetOnY(Transform thisTarget, Transform targetToFace)
    {
        Quaternion rot = Quaternion.LookRotation(targetToFace.position - thisTarget.position);
        Quaternion newPos = Quaternion.Euler(transform.eulerAngles.x, rot.eulerAngles.y, transform.eulerAngles.z);
        thisTarget.rotation = Quaternion.Slerp(thisTarget.rotation, newPos, 6 * Time.fixedDeltaTime);
    }

    #endregion

    #region Pursuit State

    public void PursuitState()
    {
        if (enemyTarget)
        {
            bossHP.gameObject.SetActive(true);

            foreach (var item in FindObjectsOfType<QuestWayPoint>())
            {
                if (item.questPhaseReceipt == 15 || item.questPhaseReceipt == 16)
                {
                    item.collided = true;
                }
            }

            if (!dialogueOneShot)
            {
                if (FindObjectOfType<SoundTrack>() != null)
                    FindObjectOfType<SoundTrack>().Stop();

                StartDialogueWithPlayer();
                controller.dialogueAltered = controller.dialogue.dialogueAltered;

                dialogueOneShot = true;
            }

            if (dialogueM.curDialogue == null && dialogueM.fadeUI.canvasGroup.alpha == 0)
            {
                bossHP.healthPoints = hp;
                if (hp.curHealthPoints > 0 && bossHP.fadeUI.canvasGroup.alpha == 0)
                {
                    bossHP.fadeUI.FadeTransition(1, 1, 0.5f);
                }

                if (FindObjectOfType<SoundTrack>() != null)
                {
                    FindObjectOfType<SoundTrack>().initialVolume = 0.7f;
                    FindObjectOfType<SoundTrack>().soundTrackVolume = 0.7f;
                    FindObjectOfType<SoundTrack>().PushTrack("BossBattle");
                }
                ChaseTarget();
            }
        }
    }

    public void ChaseTarget()
    {
        distanceFromTarget = Vector3.Distance(enemyTarget.transform.position, transform.position);

        if (!isDashing && !isAttacking && distanceFromTarget > pursuitStoppingDistance + pursuitStoppingDistance + 2)
        {
            controller.animator.SetFloat("AttackID", 4);
            controller.animator.SetTrigger("Attack");
        }

        if (controller.baseLayerInfo.IsName("Pivot Combat Stance") || controller.baseLayerInfo.IsName("Combat Stance"))
        {
            controller.animator.SetFloat("Vertical", 0);
            if (controller.navmeshAgent.enabled)
                controller.navmeshAgent.isStopped = true;
        }

        bool conditonToRotate = !controller.IsAnimatorTag("Attack") && !controller.IsAnimatorTag("Get Up") && !controller.IsAnimatorTag("Hit");
        if (conditonToRotate)
        {
            Vector3 targetDirection = enemyTarget.transform.position - transform.position;

            targetDirection.y = 0;
            targetDirection.Normalize();
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 2 * Time.deltaTime);

            controller.SetDestination(enemyTarget.transform.position);
            controller.RotateWithNavMeshAgent();
        }

        #region Get Pivot Angle 

        if (!controller.animator.GetBool("Pursuit"))
        {
            controller.animator.SetFloat("Vertical", 0);
            GetPivotAngle(enemyTarget.transform.position);
            controller.animator.SetBool("Pursuit", true);
        }

        #endregion

        // Dash towards
        if (!controller.IsAnimatorTag("Attack") && distanceFromTarget > pursuitStoppingDistance + pursuitStoppingDistance + 2)
        {
            isDashing = true;
            controller.animator.SetBool("BattleStance", false);

            #region Movement Speed 

            controller.animator.SetFloat("Vertical", 1, 0.2f, Time.deltaTime);

            #endregion
        }
        // Stalk
        else if (!isDashing && distanceFromTarget > pursuitStoppingDistance && distanceFromTarget < pursuitStoppingDistance + pursuitStoppingDistance)
        {
            controller.animator.SetBool("BattleStance", false);

            #region Movement Speed 

             controller.animator.SetFloat("Vertical", 0, 0.2f, Time.deltaTime);

            #endregion
        }
        // Battle Stance
        else if (distanceFromTarget <= pursuitStoppingDistance)
        {
            isDashing = false;
            controller.animator.SetBool("BattleStance", true);

            #region Movement Speed

            controller.animator.SetFloat("Vertical", 0, 0.2f, Time.deltaTime);

            #endregion

            if (controller.baseLayerInfo.IsName("Combat Stance"))
                state = State.CombatStance;
        }
    }

    void CombatStanceState()
    {
        distanceFromTarget = Vector3.Distance(enemyTarget.transform.position, transform.position);
        Vector3 targetDirection = enemyTarget.transform.position - transform.position;

        if (stunned == true && hp.curHealthPoints >= 0)
            state = State.Stunned;

        #region Get Pivot Angle

        if (!controller.IsAnimatorTag("Pivot") && (controller.IsAnimatorTag("Attack") || controller.IsAnimatorTag("Aim")))
            GetPivotAngle(enemyTarget.transform.position);

        #endregion

        // Set look direction
        bool conditonToRotate = !controller.IsAnimatorTag("Attack") && !controller.IsAnimatorTag("Get Up") && !controller.IsAnimatorTag("Hit");
        if (conditonToRotate)
        {
            // Last rotation position
            if (controller.navmeshAgent.enabled)
                controller.navmeshAgent.isStopped = false;

            controller.SetDestination(enemyTarget.transform.position);

            // Extra rotation for strafe
            targetDirection.y = 0;
            targetDirection.Normalize();
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 2 * Time.deltaTime);
        }
        else
        {
            if (controller.navmeshAgent.enabled)
                controller.navmeshAgent.isStopped = true;

            controller.animator.SetFloat("Vertical", 0);
        }
 
        if (controller.baseLayerInfo.IsName("Pivot Combat Stance"))
        {
            if (controller.navmeshAgent.enabled)
                controller.navmeshAgent.isStopped = true;

            controller.animator.SetFloat("Vertical", 0);
        }

        // Constantly rotating for agent's target direction
        controller.RotateWithNavMeshAgent();

        // Leave the combat stance and chase your target
        bool conditonToPursuit = !controller.IsAnimatorTag("Attack") && !controller.baseLayerInfo.IsName("Aim")
        && !controller.IsAnimatorTag("Get Up") && !controller.IsAnimatorTag("Hit");
        if (distanceFromTarget > maxAtkDistance)
        {
            controller.animator.SetBool("BattleStance", false);
            horizontalMovement = Mathf.Lerp(horizontalMovement, 0.0f, 80 * Time.deltaTime);
            controller.animator.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);
            if (conditonToPursuit && horizontalMovement == 0)
            {
                battleStanceDecisionMade = false;
                state = State.Pursuit;
            }
        }
        
        // Too close back up
        else if (distanceFromTarget > 0 && distanceFromTarget < 2)
        {
            controller.animator.SetFloat("Vertical", -1, 0.1f, Time.deltaTime);
        }
        // Stop when your not too far or to close
        else
        {
            controller.animator.SetBool("BattleStance", true);
            controller.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
        }

        horizontalMovement = Mathf.Clamp(horizontalMovement, -1, 1);
        controller.animator.SetFloat("Horizontal", horizontalMovement, 0.2f, Time.deltaTime);

        // Decide strafe direction
        if (!battleStanceDecisionMade && conditonToPursuit)
        {
            if (controller.baseLayerInfo.IsName("Combat Stance"))
            {
                RandomCircleAction();
                battleStanceDecisionMade = true;
            }
        }

        // Attack your target 
        bool conditionsToAttack = controller.baseLayerInfo.IsName("Combat Stance") && !controller.IsAnimatorTag("Hit")
        && !controller.IsAnimatorTag("Get Up");
        if (!isAttacking && conditionsToAttack && !isShooting && atkRecoveryTimer <= 0 && distanceFromTarget <= maxAtkDistance)
        {
            Attack();
        }
    }

    void RandomCircleAction()
    {
        horizontalMovement = Mathf.Lerp(horizontalMovement, 0.0f, 80 * Time.deltaTime);
        // Strafe direction
        int randomNumber = Random.Range(-100, 100);
        if (randomNumber > 0)
            horizontalMovement = 1;
        else if (randomNumber < 0)
            horizontalMovement = -1;
        else if (randomNumber == 0)
            horizontalMovement = 1;
    }

    void Attack()
    {
        if (distanceFromTarget <= maxAtkDistance && distanceFromTarget >= minAtkDistance)
        {
            controller.animator.SetTrigger("Attack");
            isAttacking = true;
        }
    }

    void WeaponAim()
    {
        if (controller.baseLayerInfo.IsName("Aim"))
        {
            shootOnAimTimer += Time.deltaTime;
            if (!isShooting && shootOnAimTimer > 1)
            {
                isShooting = true;
                shootOnAimTimer = 0;
                atkRecoveryTimer = atkRecoveryRate;
            }
        }
        else
            isShooting = false;
    }

    void AttackRecoveryTimer()
    {
        if (controller.IsAnimatorTag("Attack") || controller.IsAnimatorTag("Hit"))
            atkRecoveryTimer = atkRecoveryRate;

        bool conditionsToCountDown = !controller.baseLayerInfo.IsName("Aim")
        && !controller.IsAnimatorTag("Attack") && !controller.IsAnimatorTag("Sheath R")
        && !controller.IsAnimatorTag("Get Up");
        if (atkRecoveryTimer > 0 && conditionsToCountDown)
            atkRecoveryTimer -= Time.deltaTime;

        if (isAttacking)
        {
            if (atkRecoveryTimer <= 0)
            {
                controller.animator.SetFloat("AttackID", Random.Range(Mathf.RoundToInt(0), Mathf.RoundToInt(4)));
                controller.animator.SetFloat("HeavyAttackID", Random.Range(Mathf.RoundToInt(0), Mathf.RoundToInt(3)));

                atkRecoveryTimer = 0;
                isAttacking = false;
            }
        }
    }

    #endregion

    #region Damage

    public void OnReceiveMessage(MsgType type, object sender, object msg)
    {
        switch (type)
        {
            case MsgType.DEAD:
                Death((HealthPoints.DamageData)msg);
                break;
            case MsgType.DAMAGED:
                Damaged((HealthPoints.DamageData)msg);
                break;
            default:
                break;
        }
    }

    public void Death(HealthPoints.DamageData data)
    {
        if (controller.subjectedToSlowDown)
        {
            cc.StopSlowDown();
            cc.slowDownVolume.weight = 0;
        }
        if (state == State.Dead) return;

        FindObjectOfType<SoundTrack>().Stop();
        for (int i = 0; i < questM.mainQuestSlots.questData.Length; i++)
        {
            if (questM.mainQuestSlots.questData[i].questName == controller.questSequenceNameReceipt &&
            questM.mainQuestSlots.questData[i].inProgress)
            {
                questM.AddQuestObjective(ref controller.questSequenceNameReceipt, ref controller.questPhaseReceipt, ref controller.addQuestPhaseBy, ref controller.addQuestQuantityBy);
            }
        }

        for (int i = 0; i < questM.sideQuestSlots.questData.Length; i++)
        {
            if (questM.sideQuestSlots.questData[i].questName == controller.questSequenceNameReceipt &&
            questM.sideQuestSlots.questData[i].inProgress)
            {
                questM.AddQuestObjective(ref controller.questSequenceNameReceipt, ref controller.questPhaseReceipt, ref controller.addQuestPhaseBy, ref controller.addQuestQuantityBy);
            }
        }

        if (cc.tpCam.lockedOn)
        {
            cc.tpCam.ToggleLockOn(!cc.tpCam.lockedOn);
            cc.isStrafing = false;
        }

        if (rifle)
            rifle.SetActive(false);
        if (grenadeInHand)
            grenadeInHand.SetActive(false);

        if (GetComponent<Targetable>() != null)
            Destroy(GetComponent<Targetable>());

        controller.animator.SetBool("Dead", true);
        state = State.Dead;
    }

    public void Damaged(HealthPoints.DamageData data)
    {
        if (state == State.Dead) return;

        if (data.damager.GetComponentInChildren<ItemData>() != null)
            data.damager.GetComponent<HealthPoints>().curHealthPoints -= 1;

        if (hp.curStunPoints <= 0)
        {
            stunned = true;
            hp.curStunPoints = 0;
            controller.animator.speed = 1;

            if (bloodEffect != null)
                bloodEffect.CreateBloodEffect(data.damageSource, data.damager.GetComponent<HitBox>().bloodEffectName);
            if (data.damager.GetComponent<HitBox>().isAttacking)
                data.damager.GetComponent<HitBox>().isAttacking = false;

            return;
        }

        #region Blood Effect

        if (bloodEffect != null)
            bloodEffect.CreateBloodEffect(data.damageSource, data.damager.GetComponent<HitBox>().bloodEffectName);

        #endregion

        #region Particle Effect

        if (data.damager.GetComponent<HitBox>().effects.stickOnContactEffectHit != null)
        {
            GameObject stickEffect = Instantiate(data.damager.GetComponent<HitBox>().effects.stickOnContactEffectHit.gameObject,
            controller.animator.GetBoneTransform(HumanBodyBones.Hips).transform.position, transform.rotation) as GameObject;

            stickEffect.transform.parent = transform;
            GetComponent<HealthPoints>().stickOnEffectTimer = 5;
        }

        if (data.damager.GetComponent<ItemData>() != null &&
            data.damager.GetComponent<ItemData>().itemType == ItemData.ItemType.Arrow)
        {
            data.damager.GetComponent<HitBox>().isProjectile = false;
            Destroy(data.damager.GetComponent<HitBox>().gameObject);
        }

        #endregion

        #region Hurt Angle

        if (data.damager.GetComponent<HitBox>().hurtID == 1)
        {
            controller.animator.SetTrigger("Hurt");

            Vector3 forward = data.damageSource - transform.position;
            forward.y = 0f;

            Vector3 localHurt = transform.InverseTransformDirection(forward);
            var _angle = (int)(Mathf.Atan2(localHurt.x, localHurt.z) * Mathf.Rad2Deg);

            if (_angle <= 45 && _angle >= -45)
                _angle = 0;
            else if (_angle > 45 && _angle < 135)
                _angle = 90;
            else if (_angle >= 135 || _angle <= -135)
                _angle = 180;
            else if (_angle < -45 && _angle > -135)
                _angle = -90;

            controller.animator.SetInteger("HurtAngle", _angle);
        }
        else if (data.damager.GetComponent<HitBox>().hurtID == 2)
        {
            controller.animator.SetFloat("KnockBackID", 0);
            controller.animator.SetTrigger("KnockBack");
        }
        else if (data.damager.GetComponent<HitBox>().hurtID == 3)
        {
            controller.AddForce(Vector3.up.normalized * 10, ForceMode.VelocityChange);
            controller.animator.SetFloat("KnockBackID", 1);
            controller.animator.SetTrigger("KnockBack");
        }

        controller.animator.speed = 1;

        #endregion

        data.damager.GetComponent<HitBox>().isAttacking = false;
    }

    void DeathDrop()
    {
        itemDropTimer += Time.deltaTime;
        if (itemDropTimer > dropItemTimerMax)
        {
            for (int i = 0; i < itemDrops.Length; i++)
            {
                if (itemDrops[i] != null)
                {
                    GameObject items = Instantiate(itemDrops[i], transform.position + new Vector3(Random.Range(-1.2f, 1.2f),
                    1.2f, Random.Range(-1.2f, 1.2f)), Quaternion.Euler(0, 0, 90)) as GameObject;
                    items.GetComponentInChildren<ItemData>().itemActive = true;
                    items.SetActive(true);
                }
            }
            cc.infoMessage.info.text = "Pick up (Lu's Rifle) to transition to the next level.";
            hudM.IncrementExperience(Exp);

            GetComponent<LuBehavior>().enabled = false;
        }
    }

    void StunnedState()
    {
        distanceFromTarget = Vector3.Distance(enemyTarget.transform.position, transform.position);
        curStunnedRecoveryTime += 0.5f * Time.deltaTime;

        #region Stun Deflect

        if (hp.curStunPoints > 0)
        {
            if (curStunnedRecoveryTime > stunDeflectMaxTime)
            {
                curStunnedRecoveryTime = 0;
                stunned = false;
                state = State.Pursuit;
            }

            if (controller.IsAnimatorTag("Attack"))
            {
                Vector3 targetDirection = enemyTarget.transform.position - transform.position;

                targetDirection.y = 0;
                targetDirection.Normalize();
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 4000);
            }
        }

        #endregion

        #region State Transition

        #region Get Pivot Angle

        if (!controller.IsAnimatorTag("Pivot"))
            GetPivotAngle(enemyTarget.transform.position);

        #endregion

        if (!stunned)
        {
            controller.navmeshAgent.enabled = true;
        }
        else
            controller.navmeshAgent.enabled = false;

        #endregion

        #region Stun Finisher

        if (hp.curStunPoints <= 0)
        {
            if(hp.curHealthPoints <= 0)
            {
                hp.curHealthPoints = 0;
                state = State.Dead;
                return;
            }

            if (curStunnedRecoveryTime > stunFinisherMaxTime)
            {
                hp.curStunPoints = hp.maxStunPoints;
                curStunnedRecoveryTime = 0;
                stunned = false;
                state = State.Pursuit;
            }
        }

        Finisher();

        #endregion
    }

    void Finisher()
    {
        if (controller.fullBodyInfo.IsName("Finisher"))
        {
            if (state != State.Dead && controller.fullBodyInfo.normalizedTime > 0.9f)
            {
                hp.curHealthPoints = 0;
                state = State.Dead;
            }
        }
    }

    #endregion

    #region Find Target

    public void FindTarget()
    {
        // we ignore height difference if the target was already seen
        GameObject targetScanned = targetScanner.Detect(transform, enemyTarget == null);

        if (enemyTarget == null)
        {
            // we just saw the player for the first time, pick an empty spot to target around them
            if (targetScanned != null)
            {
                HealthPoints distributor = targetScanned.GetComponent<HealthPoints>();
                if (distributor != null)
                {
                    enemyTarget = distributor.gameObject;
                    targetScanner.detectionRadius = 50;
                }
            }
        }
        else
        {
            if (state != State.Pursuit)
                return;

            m_TimerSinceLostTarget += Time.deltaTime;
            if (m_TimerSinceLostTarget >= timeToStopPursuit)
            {
                Vector3 toTarget = enemyTarget.transform.position - transform.position;

                if (toTarget.sqrMagnitude > targetScanner.detectionRadius * targetScanner.detectionRadius)
                {
                    // the target move out of range, reset the target
                    enemyTarget = null;
                }
            }
        }
    }

    #endregion

    #region AnimatorIK

    void OnAnimatorIK(int layerIndex)
    {
        if (cc.slowDown || state == State.Dead)
        {
            lookWeight = 0;
            return;
        }

        if (controller.baseLayerInfo.IsTag("Aim"))
        {
            lookWeight = 1;
        }
        else
        {
            lookWeight = 0;
            muzzleFlash.gameObject.SetActive(false);
        }

        if (enemyTarget == null) return;

        lookWeight = Mathf.Clamp(lookWeight, 0, 1);

        // target position
        Vector3 targetPosition = enemyTarget.transform.position +
        (enemyTarget.transform.up * enemyTarget.GetComponent<CapsuleCollider>().height / 2);

        bool conditions = state == State.CombatStance;
        controller.animator.SetLookAtWeight(1, lookWeight, conditions ? 0 : 1);
        controller.animator.SetLookAtPosition(targetPosition);
    }

    #endregion

    #region Animation Event

    public void LastFootStepEvent(int footStep)
    {
    }

    public void AttackPhaseContextEvent(string context)
    {
        if (GetComponent<LuBehavior>())
        {
            if (context == "Start")
            {
                controller.animator.speed = 0.4f;
            }
            if (context == "End")
            {
                controller.animator.speed = 1;
            }
        }
    }

    public void PerfectDodgeEvent(string context)
    {
        if (context == "Start")
        {
            controller.canPerfectDodge = true;
        }
        if (context == "End")
        {
            controller.canPerfectDodge = false;
        }
    }

    public void ArmedAttackEvent(string context)
    {
        if(context == "Light Attack")
        {
            GetComponentInChildren<HitBox>().hurtID = 1;
            GetComponentInChildren<HitBox>().BeginAttack("Small Blood Hit");
        }

        if (context == "Heavy Attack")
        {
            GetComponentInChildren<HitBox>().hurtID = 2;
            GetComponentInChildren<HitBox>().BeginAttack("Medium Blood Hit");
        }

        if (context == "End")
        {
            GetComponentInChildren<HitBox>().trailActive = false;
            GetComponentInChildren<HitBox>().EndAttack();
        }
    }

    public void DamageEvent(string context)
    {
        if (controller.m_UnderExternalForce)
            controller.ClearForce();
    }

    public void WeaponProjectileEvent()
    {

    }

    public void WeaponThrowEvent(string context)
    {
        if(context == "Start")
        {
            Vector3 targetPosition = enemyTarget.transform.position +
            (enemyTarget.transform.up * enemyTarget.GetComponent<CapsuleCollider>().height / 2);

            Vector3 newDir = targetPosition - grenadePrefabSpot.transform.position;
            newDir.Normalize();

            GameObject _grenade = Instantiate(grenade, grenadePrefabSpot.transform.position,
            Quaternion.LookRotation(newDir)) as GameObject;

            _grenade.SetActive(true);

            _grenade.GetComponent<Rigidbody>().AddForce((transform.forward * 5) + (transform.up * 2), ForceMode.Impulse);

            isReloading = true;
        }

        if (context == "End")
        {
            grenadeInHand.SetActive(false);
        }
    }

    public void ShootEvent(string context)
    {
        if(context == "Fire")
        {
            Vector3 targetPosition = enemyTarget.transform.position +
            (enemyTarget.transform.up * enemyTarget.GetComponent<CapsuleCollider>().height / 2);

            Vector3 newDir = targetPosition - bulletPrefabSpot.transform.position;
            newDir.Normalize();

            GameObject _bullet = Instantiate(bullets, bulletPrefabSpot.transform.position,
            Quaternion.LookRotation(newDir)) as GameObject;

            _bullet.GetComponentInChildren<HitBox>().projectileVel = new Vector3(0, 8, 90);
            _bullet.GetComponentInChildren<HitBox>().targetLayers = targetScanner.targetLayer;
            _bullet.GetComponentInChildren<HitBox>().isAttacking = true;
            _bullet.GetComponentInChildren<HitBox>().isProjectile = true;
            _bullet.GetComponentInChildren<HitBox>().weaponTrail = true;
            _bullet.GetComponentInChildren<HitBox>().BeginAttack("Small Blood Hit");
            _bullet.SetActive(true);

            if (rifleShotAudio)
                rifleShotAudio.PlayRandomClip();

            muzzleFlash.gameObject.SetActive(true);
            muzzleFlash.Play();

            isReloading = true;
        }

        if (context == "End")
        {
            muzzleFlash.Stop();
        }
    }

    public void WeaponSwitch()
    {
        grenadeInHand.SetActive(true);
    }

    public void ReloadEvent()
    {
        if (rifleReloadAudio)
            rifleReloadAudio.PlayRandomClip();
    }


    #endregion
}
