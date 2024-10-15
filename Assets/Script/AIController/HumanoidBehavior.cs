using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using RPGAE.CharacterController;

public class HumanoidBehavior : MonoBehaviour, DamageReceiver
{
    [Header("HUMANOID SETTINGS")]
    public NPCPOSE npcPose;
    public Behaviour behaviour;
    [HideInInspector] public State state;
    public float idleWaitTime = 2;
    private float idleWaitTimer;

    [Header("ROAM SETTINGS")]
    [Range(0, 20)] public float roamRadius = 10;

    [Header("PATROL SETTINGS")]
    public Transform[] patrolPoints;
    private int destPoint = 0;

    [Header("FOLLOW COMPANION SETTINGS")]
    public Transform followCompanion;
    public float followStoppingDistance;

    [Header("COMBAT SETTINGS")]
    public GameObject equippedWeapon;
    public GameObject equippedArrow;
    public TargetScanner targetScanner;
    public float timeToStopPursuit = 6f;
    [Range(0, 100)]
    public float pursuitStoppingDistance;
    [Range(0, 100)]
    public float minAtkDistance = 0;
    [Range(0, 100)]
    public float maxAtkDistance = 0;
    public float shootOnAimTimer;
    public float atkRecoveryRate;
    private float atkRecoveryTimer;

    public float stunDeflectMaxTime = 3;
    public float stunFinisherMaxtime = 8;

    [Header("ON DEATH")]
    public int Exp = 0;
    public float dropItemTimerMax = 3;
    [Tooltip("Specfic items that would drop upon it's death")]
    public GameObject[] itemDrops;

    HealthPoints.DamageData tempData;

    #region Hide 

    private float lookWeight;

    private Transform leftHandIK;
    [HideInInspector] public float signedAngle;

    private Vector3 patrolPosition;

    private bool isAttacking;
    private bool isShooting;
    private bool primaryEquip;
    private bool secondaryEquip;

    private object dead;
    private float itemDropTimer;

    private NavMeshHit navHit;
    private Vector3 originalPosition;
    public GameObject enemyTarget;
    [HideInInspector] public AIController controller;

    private float distanceFromTarget;
    private float horizontalMovement;
    private bool isReloading;

    private float curStunnedRecoveryTime;
    private float m_TimerSinceLostTarget = 0.0f;
    private float weaponArmsID;
    private bool battleStanceDecisionMade;

    public enum State
    {
        Idle,
        FollowCompanion,
        RoamArea,
        PatrolPoints,
        Pursuit,
        CombatStance,
        WalkBackToBase,
        Stunned,
        Dead
    }

    public enum Behaviour
    {
        Idle,
        FollowCompanion,
        RoamArea,
        PatrolPoints
    }

    public enum NPCPOSE
    {
        NA,
        CleanerOne, 
        CleanerTwo,
        CoffeeWait,
        MagazineOne,
        MagazineTwo, 
        MagazineThree,
        MobileWait,
        ScreenMagazine, 
        SittingWaiting,
        StandingOvationOne,
        StandingOvationTwo,
        StandingOvationThree,
        StandingOvationFour,
        StandingOvationFive,
        StandingOvationSix,
        TalkingNPCOne,
        TalkingNPCTwo,
        TalkingNPCThree,
        TrampOne,
        TrampTwo,
        WorkingOnLaptop,
    }
    private Quaternion oldLookDirection;
    private InteractWorldSpaceUI interactUI;

    [HideInInspector] public HUDManager hudM;
    [HideInInspector] public DialogueManager dialogueM;
    [HideInInspector] public ThirdPersonController cc;
    [HideInInspector] public HealthPoints hp;
    [HideInInspector] public BloodEffect bloodEffect;
    [HideInInspector] public QuestManager questM;

    #endregion

    void Start()
    {
        controller = GetComponentInChildren<AIController>();

        hudM = FindObjectOfType<HUDManager>();
        cc = FindObjectOfType<ThirdPersonController>();
        dialogueM = FindObjectOfType<DialogueManager>();
        hp = GetComponentInChildren<HealthPoints>();
        bloodEffect = GetComponentInChildren<BloodEffect>();
        interactUI = GetComponentInChildren<InteractWorldSpaceUI>();
        questM = FindObjectOfType<QuestManager>();

        controller.animator.GetBoneTransform(HumanBodyBones.Head).tag = "HeadShot";

        SetWeapon();

        originalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (cc.inventoryM.isPauseMenuOn || cc.inventoryM.inventoryHUD.isActive) return;

        if(npcPose == NPCPOSE.NA)
        {
            switch (state)
            {
                case State.Idle:
                    IdleState();
                    break;
                case State.RoamArea:
                    RoamState();
                    break;
                case State.PatrolPoints:
                    PatrolPointsState();
                    break;
                case State.FollowCompanion:
                    FollowCompanionState();
                    break;
                case State.Pursuit:
                    PursuitState();
                    break;
                case State.CombatStance:
                    CombatStanceState();
                    break;
                case State.WalkBackToBase:
                    WalkBackToBase();
                    break;
                case State.Stunned:
                    StunnedState();
                    break;
                case State.Dead:
                    DeathDrop();
                    break;
            }
        }
        else
        {
            IdleState();
        }
        //Debug.Log(state);
        FindTarget();
        WeaponAim();
        SheathingWeapon();
        AttackRecoveryTimer();

        if (!controller.dialogue.isDeactivated)
            StartDialogueWithPlayer();

        controller.animator.SetBool("Stunned", controller.stunned);
        if (hp != null)
        {
            controller.animator.SetFloat("CurStunPoints", hp.curStunPoints);
        }
        controller.animator.SetBool("IsShooting", isShooting);
        controller.animator.SetFloat("AttackRecovery", atkRecoveryTimer);
        controller.animator.SetBool("Reloading", isReloading);
        controller.animator.SetBool("Grounded", controller.grounded);
        controller.animator.SetFloat("WalkPivotAngle", signedAngle);
    }

    void FixedUpdate()
    {
        NearOriginalPosition();
    }

    void NearOriginalPosition()
    {
        Vector3 toOriginPosition = originalPosition - transform.position;
        toOriginPosition.y = 0;
        controller.animator.SetBool("NearOriginPosition", toOriginPosition.sqrMagnitude < 0.1 * 0.1f);
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

    #region General State

    void IdleState()
    {
        if (enemyTarget != null)
        {
            controller.animator.SetBool("Patrol", false);
            state = State.Pursuit;
            return;
        }
        #region NPC Pose 

        switch (npcPose)
        {
            case NPCPOSE.CleanerOne:
                controller.animator.SetFloat("NPCPose", 1);
                break;
            case NPCPOSE.CleanerTwo:
                controller.animator.SetFloat("NPCPose", 2);
                break;
            case NPCPOSE.CoffeeWait:
                controller.animator.SetFloat("NPCPose", 3);
                break;
            case NPCPOSE.MagazineOne:
                controller.animator.SetFloat("NPCPose", 4);
                break;
            case NPCPOSE.MagazineTwo:
                controller.animator.SetFloat("NPCPose", 5);
                break;
            case NPCPOSE.MagazineThree:
                controller.animator.SetFloat("NPCPose", 6);
                break;
            case NPCPOSE.MobileWait:
                controller.animator.SetFloat("NPCPose", 7);
                break;
            case NPCPOSE.ScreenMagazine:
                controller.animator.SetFloat("NPCPose", 8);
                break;
            case NPCPOSE.SittingWaiting:
                controller.animator.SetFloat("NPCPose", 9);
                break;
            case NPCPOSE.StandingOvationOne:
                controller.animator.SetFloat("NPCPose", 10);
                break;
            case NPCPOSE.StandingOvationTwo:
                controller.animator.SetFloat("NPCPose", 11);
                break;
            case NPCPOSE.StandingOvationThree:
                controller.animator.SetFloat("NPCPose", 12);
                break;
            case NPCPOSE.StandingOvationFour:
                controller.animator.SetFloat("NPCPose", 13);
                break;
            case NPCPOSE.StandingOvationFive:
                controller.animator.SetFloat("NPCPose", 14);
                break;
            case NPCPOSE.TalkingNPCOne:
                controller.animator.SetFloat("NPCPose", 15);
                break;
            case NPCPOSE.TalkingNPCTwo:
                controller.animator.SetFloat("NPCPose", 16);
                break;
            case NPCPOSE.TalkingNPCThree:
                controller.animator.SetFloat("NPCPose", 17);
                break;
            case NPCPOSE.TrampOne:
                controller.animator.SetFloat("NPCPose", 18);
                break;
            case NPCPOSE.TrampTwo:
                controller.animator.SetFloat("NPCPose", 19);
                break;
            case NPCPOSE.WorkingOnLaptop:
                controller.animator.SetFloat("NPCPose", 20);
                break;
        }

        #endregion
    }

    void RoamState()
    {
        //if (!controller.grounded || !controller.navmeshAgent.enabled) return;

        //if (enemyTarget != null)
        //{
        //    controller.animator.SetBool("Patrol", false);
        //    state = State.Pursuit;
        //    return;
        //}
        //Debug.Log("----RoamState");
        //controller.animator.SetBool("Patrol", true);
        //controller.navmeshAgent.stoppingDistance = 0.5f;
        //controller.animator.SetBool("IsMoving", controller.navmeshAgent.hasPath);

        //if (!controller.navmeshAgent.pathPending)
        //{
        //    if (controller.navmeshAgent.remainingDistance <= controller.navmeshAgent.stoppingDistance)
        //    {
        //        if (!controller.navmeshAgent.hasPath || controller.navmeshAgent.velocity.sqrMagnitude == 0)
        //        {
        //            idleWaitTimer += Time.deltaTime;
        //            if (idleWaitTimer > idleWaitTime)
        //            {
        //                Vector3 randomPosition = Random.insideUnitSphere * roamRadius;
        //                NavMesh.SamplePosition(originalPosition + randomPosition, out navHit, 20f, NavMesh.AllAreas);

        //                patrolPosition = navHit.position;

        //                GetPivotAngle(patrolPosition);
        //                controller.navmeshAgent.SetDestination(patrolPosition);

        //                idleWaitTimer = 0;
        //            }
        //        }
        //    }
        //}
    }

    public void PatrolPointsState()
    {
        //if (!controller.grounded || !controller.navmeshAgent.enabled) return;

        //if (enemyTarget != null)
        //{
        //    controller.animator.SetBool("Patrol", false);
        //    state = State.Pursuit;
        //    return;
        //}

        //controller.animator.SetBool("Patrol", true);
        //controller.navmeshAgent.stoppingDistance = 0.5f;
        //controller.animator.SetBool("IsMoving", controller.navmeshAgent.hasPath);

        //if (!controller.navmeshAgent.pathPending)
        //{
        //    if (controller.navmeshAgent.remainingDistance <= controller.navmeshAgent.stoppingDistance)
        //    {
        //        if (!controller.navmeshAgent.hasPath || controller.navmeshAgent.velocity.sqrMagnitude == 0)
        //        {
        //            idleWaitTimer += Time.deltaTime;
        //            if (idleWaitTimer > idleWaitTime)
        //            {
        //                patrolPosition = patrolPoints[destPoint].position;

        //                GetPivotAngle(patrolPosition);
        //                GotoNextPoint(patrolPosition);

        //                idleWaitTimer = 0;
        //            }
        //        }
        //    }
        //}
    }

    void GotoNextPoint(Vector3 pos)
    {
        //if (patrolPoints.Length == 0)
        //    return;

        //controller.navmeshAgent.destination = pos;
        //destPoint = (destPoint + 1) % patrolPoints.Length;
    }

    public void FollowCompanionState()
    {
        //controller.animator.SetBool("Patrol", true);
        //controller.navmeshAgent.stoppingDistance = followStoppingDistance;
        //distanceFromTarget = Vector3.Distance(followCompanion.position, transform.position);

        //if (!controller.IsAnimatorTag("Heavy Charge") || !controller.IsAnimatorTag("Attack") || !controller.IsAnimatorTag("Hit"))
        //    EquipWeapon(false);

        //if (enemyTarget != null)
        //{
        //    controller.animator.SetBool("Patrol", false);
        //    state = State.Pursuit;
        //    return;
        //}

        //if (distanceFromTarget <= followStoppingDistance)
        //{
        //    GetPivotAngle(followCompanion.position);
        //    controller.animator.SetBool("IsMoving", false);
        //}
        //else
        //{
        //    if (distanceFromTarget > followStoppingDistance * 1.9f)
        //        controller.animator.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);
        //    else
        //        controller.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);


        //    if (!controller.baseLayerInfo.IsTag("Pivot"))
        //    {
        //        Vector3 targetDirection = followCompanion.position - transform.position;

        //        targetDirection.y = 0;
        //        targetDirection.Normalize();
        //        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        //        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 2 * Time.deltaTime);
        //    }

        //    controller.animator.SetBool("IsMoving", true);
        //    if (controller.navmeshAgent.enabled)
        //        controller.navmeshAgent.SetDestination(followCompanion.position);
        //}
    }

    #endregion

    #region Pursuit State

    public void PursuitState()
    {
        if (!enemyTarget)
        {
            #region State Change

            if (followCompanion != null)
            {
                state = State.FollowCompanion;
            }
            else
            {
                Vector3 toOriginPosition = originalPosition - transform.position;
                toOriginPosition.y = 0;
                if (toOriginPosition.sqrMagnitude > 1)
                    state = State.WalkBackToBase;
            }

            #endregion
        }
        else
        {
            ChaseTarget();
        }
    }

    public void ChaseTarget()
    {
        distanceFromTarget = Vector3.Distance(enemyTarget.transform.position, transform.position);

        controller.SetDestination(enemyTarget.transform.position);

        //controller.RotateWithNavMeshAgent();
        controller.LookAtTarget(enemyTarget.transform);

        //Vector3 targetDirection = enemyTarget.transform.position - transform.position;

        //targetDirection.y = 0;
        //targetDirection.Normalize();
        //Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 2 * Time.deltaTime);

        EquipWeapon(true);

        #region Get Pivot Angle 

        if (!controller.animator.GetBool("Pursuit"))
        {
            controller.animator.SetFloat("Vertical", 1);
            //GetPivotAngle(enemyTarget.transform.position);
            controller.animator.SetBool("Pursuit", true);
        }

        #endregion

        if (distanceFromTarget > pursuitStoppingDistance)
        {
            controller.animator.SetBool("BattleStance", false);

            #region Movement Speed 

            controller.animator.SetFloat("Vertical", 1, 0.2f, Time.deltaTime);

            #endregion
        }
        else if (distanceFromTarget <= pursuitStoppingDistance)
        {
            controller.animator.SetBool("BattleStance", true);

            #region Movement Speed

            controller.animator.SetFloat("Vertical", 0, 0.2f, Time.deltaTime);

            #endregion

            if (controller.baseLayerInfo.IsName("Combat Stance"))
            {
                controller.navmeshAgent.enabled = false;
                state = State.CombatStance;
                controller.navmeshAgent.enabled = true;
            }
        }
        //Debug.Log("ChaseTarget");
    }

    void WalkBackToBase()
    {
        if (controller.navmeshAgent == null || !controller.navmeshAgent.enabled 
            || followCompanion != null) return;

        controller.animator.SetFloat("Vertical", 0);
        controller.animator.SetBool("Pursuit", false);
        controller.animator.SetBool("BattleStance", false);

        if (!controller.IsAnimatorTag("Heavy Charge") || !controller.IsAnimatorTag("Attack") || !controller.IsAnimatorTag("Hit"))
            EquipWeapon(false);

        // Pivot towards destination 
        if (!controller.baseLayerInfo.IsTag("Pivot"))
        {
            //GetPivotAngle(originalPosition);
            controller.navmeshAgent.enabled = false;
            //Debug.Log("controller.navmeshAgent: " + originalPosition);
            controller.LookAtPoint(originalPosition);
            controller.navmeshAgent.enabled = true;
            controller.navmeshAgent.SetDestination(originalPosition);
        }
        //Debug.Log("enemyTarget: " + enemyTarget);
        //// Target is in your reach, the chase continues
        //if (enemyTarget != null)
        //{
        //    Debug.Log("enemyTarget controller.navmeshAgent: " + originalPosition);
        //    battleStanceDecisionMade = false;
        //    controller.navmeshAgent.nextPosition = transform.position;
        //    state = State.Pursuit;
        //}

        // Once you reach your orignal position change current state to previous
        Vector3 toOriginPosition = originalPosition - transform.position;
        toOriginPosition.y = 0;
        if (toOriginPosition.sqrMagnitude < 0.2f)
        {
            state = (State)behaviour;
            controller.navmeshAgent.enabled = false;
        }
    }

    void CombatStanceState()
    {
        if(enemyTarget == null)
        {
            return;
        }
        distanceFromTarget = Vector3.Distance(enemyTarget.transform.position, transform.position);
        //Vector3 targetDirection = enemyTarget.transform.position - transform.position;

        if (controller.stunned == true)
            state = State.Stunned;

        #region Get Pivot Angle

        //if (!controller.IsAnimatorTag("Pivot") && (controller.IsAnimatorTag("Attack") || controller.IsAnimatorTag("Aim")))
        //    GetPivotAngle(enemyTarget.transform.position);

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
            //targetDirection.y = 0;
            //targetDirection.Normalize();
            //Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 2 * Time.deltaTime);
        }
        else
        {
            if (controller.navmeshAgent.enabled)
                controller.navmeshAgent.isStopped = true;

            controller.animator.SetFloat("Vertical", 0);
        }

        if (controller.baseLayerInfo.IsName("Pivot Combat Stance") || controller.baseLayerInfo.IsName("Combat Stance") 
        || controller.baseLayerInfo.IsTag("Aim"))
        {
            if (controller.navmeshAgent.enabled)
                controller.navmeshAgent.isStopped = true;

            controller.animator.SetFloat("Vertical", 0);
        }

        // Constantly rotating for agent's target direction
        //controller.RotateWithNavMeshAgent();
        controller.LookAtTarget(enemyTarget.transform);

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
                controller.navmeshAgent.enabled = false;
                state = State.Pursuit;
                controller.navmeshAgent.enabled = false;
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
        if (!isAttacking && conditionsToAttack && !cc.slowDown && !isShooting && atkRecoveryTimer <= 0 && distanceFromTarget <= maxAtkDistance)
        {
            Attack();
        }
    }

    void RandomCircleAction()
    {
        //horizontalMovement = Mathf.Lerp(horizontalMovement, 0.0f, 80 * Time.deltaTime);
        //// Strafe direction
        //int randomNumber = Random.Range(-100, 100);
        //if (randomNumber > 0)
        //    horizontalMovement = 1;
        //else if (randomNumber < 0)
        //    horizontalMovement = -1;
        //else if (randomNumber == 0)
        //    horizontalMovement = 1;
    }

    void Attack()
    {
        if (distanceFromTarget <= maxAtkDistance && distanceFromTarget >= minAtkDistance)
        {
            isAttacking = true;
        }
    }

    void WeaponAim()
    {
        if (enemyTarget == null) return;

        float rangeDistance = Vector3.Distance(enemyTarget.transform.position, transform.position);
        controller.animator.SetFloat("RangeDistance", rangeDistance);

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
        if (controller.IsAnimatorTag("Attack") || controller.IsAnimatorTag("Aim") || controller.IsAnimatorTag("Hit"))
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
                controller.animator.SetTrigger("Attack");
                controller.animator.SetFloat("AttackID", Random.Range(Mathf.RoundToInt(0), Mathf.RoundToInt(4)));
                atkRecoveryTimer = 0;
                isAttacking = false;
            }
        }
    }

    #endregion

    #region Holster

    void SetWeapon()
    {
        if (equippedWeapon != null && equippedWeapon.GetComponentInChildren<ItemData>().itemType == ItemData.ItemType.Weapon)
        {
            controller.wpnHolster.SetAIWeapon(equippedWeapon.GetComponentInChildren<ItemData>());
            controller.wpnHolster.primaryH.gameObject.SetActive(true);
        }
        if (equippedWeapon != null && equippedWeapon.GetComponentInChildren<ItemData>().itemType == ItemData.ItemType.Bow)
        {
            controller.wpnHolster.SetAIWeapon(equippedWeapon.GetComponentInChildren<ItemData>());
            controller.wpnHolster.secondaryH.gameObject.SetActive(true);
            if (equippedArrow != null && equippedArrow.GetComponentInChildren<ItemData>().itemType == ItemData.ItemType.Arrow)
            {
                controller.wpnHolster.SetAIWeapon(equippedArrow.GetComponentInChildren<ItemData>());
                if(equippedWeapon.GetComponentInChildren<ItemData>().weaponArmsID == 6)
                {
                    controller.wpnHolster.quiverH.gameObject.SetActive(true);
                    controller.wpnHolster.arrowH.gameObject.SetActive(true);
                }
            }
        }
    }

    public void EquipWeapon(bool equip)
    {
        if (equippedWeapon == null) return;

        if (equip)
        {
            if(equippedWeapon.GetComponentInChildren<ItemData>() != null)
            {
                #region Primary Weapon

                if (controller.animator.GetInteger("EquipRightHandID") == 0 &&
                equippedWeapon.GetComponentInChildren<ItemData>().itemType == ItemData.ItemType.Weapon)
                {
                    controller.animator.SetTrigger("EquipRightHand");
                    controller.animator.SetInteger("EquipRightHandID", 1);

                    weaponArmsID = equippedWeapon.GetComponentInChildren<ItemData>().weaponArmsID;

                    controller.wpnHolster.SetAIWeapon(equippedWeapon.GetComponentInChildren<ItemData>());
                }

                #endregion

                #region Secondary Weapon

                #region Bow

                if (controller.animator.GetInteger("EquipLeftHandID") == 0 &&
                equippedWeapon.GetComponentInChildren<ItemData>().itemType == ItemData.ItemType.Bow &&
                equippedWeapon.GetComponentInChildren<ItemData>().weaponArmsID == 6)
                {
                    controller.animator.SetTrigger("EquipLeftHand");
                    controller.animator.SetInteger("EquipLeftHandID", 1);
                    controller.animator.SetTrigger("EquipRightHand");
                    controller.animator.SetInteger("EquipRightHandID", 1);

                    isReloading = false;

                    weaponArmsID = equippedWeapon.GetComponentInChildren<ItemData>().weaponArmsID;

                    controller.wpnHolster.SetAIWeapon(equippedWeapon.GetComponentInChildren<ItemData>());
                    controller.wpnHolster.SetAIWeapon(equippedArrow.GetComponentInChildren<ItemData>());
                }

                #endregion

                #region Gun 

                if (controller.animator.GetInteger("EquipRightHandID") == 0 &&
                equippedWeapon.GetComponentInChildren<ItemData>().itemType == ItemData.ItemType.Bow &&
                equippedWeapon.GetComponentInChildren<ItemData>().weaponArmsID == 7)
                {
                    controller.animator.SetTrigger("EquipRightHand");
                    controller.animator.SetInteger("EquipRightHandID", 1);

                    isReloading = false;

                    weaponArmsID = equippedWeapon.GetComponentInChildren<ItemData>().weaponArmsID;

                    controller.wpnHolster.SetAIWeapon(equippedWeapon.GetComponentInChildren<ItemData>());
                    controller.wpnHolster.SetAIWeapon(equippedArrow.GetComponentInChildren<ItemData>());
                }

                #endregion

                #endregion
            }
        }
        else if (!equip)
        {
            if (equippedWeapon.GetComponentInChildren<ItemData>() != null)
            {
                if (controller.animator.GetInteger("EquipRightHandID") == 1 &&
                equippedWeapon.GetComponentInChildren<ItemData>().itemType == ItemData.ItemType.Weapon)
                {
                    controller.animator.SetTrigger("EquipRightHand");
                    controller.animator.SetInteger("EquipRightHandID", 0);

                    weaponArmsID = 0;
                }
                if (controller.animator.GetInteger("EquipLeftHandID") == 1 &&
                equippedWeapon.GetComponentInChildren<ItemData>().itemType == ItemData.ItemType.Bow)
                {
                    controller.animator.SetTrigger("EquipLeftHand");
                    controller.animator.SetInteger("EquipLeftHandID", 0);

                    controller.animator.SetTrigger("EquipRightHand");
                    controller.animator.SetInteger("EquipRightHandID", 0);

                    // Remove arrow on string and activate arrowE before sheathing
                    if (controller.wpnHolster.arrowString)
                        controller.wpnHolster.arrowString.gameObject.SetActive(false);
                    if (controller.wpnHolster.arrowE)
                        controller.wpnHolster.arrowE.gameObject.SetActive(true);

                    weaponArmsID = 0;
                }
            }
        }
    }

    void SheathingWeapon()
    {
        if (controller.wpnHolster == null) return;

        controller.animator.SetFloat("WeaponArmsID", weaponArmsID);

        #region Weapon arms IK weight

        if (weaponArmsID == 0 || weaponArmsID == 1 || weaponArmsID == 2 || weaponArmsID == 3 || weaponArmsID == 4 || weaponArmsID == 5)
        {
            if (!controller.IsAnimatorTag("Attack") && !controller.baseLayerInfo.IsName("Heavy Charge") && 
                !controller.IsAnimatorTag("Hit") && !controller.IsAnimatorTag("Aim"))
            {
                controller.leftArmLayerWeight += 4 * Time.deltaTime;
                controller.rightArmLayerWeight += 4 * Time.deltaTime;
            }
            else
            {
                controller.leftArmLayerWeight = 0;
                controller.rightArmLayerWeight = 0;
            }
        }

        if (weaponArmsID == 6 || weaponArmsID == 7)
        {
            if (!controller.IsAnimatorTag("Aim") && !isShooting)
            {
                controller.rightArmLayerWeight += 2 * Time.deltaTime;
                controller.leftArmLayerWeight += 2 * Time.deltaTime;
            }
            else
            {
                controller.rightArmLayerWeight -= 2 * Time.deltaTime;
                controller.leftArmLayerWeight -= 2 * Time.deltaTime;
            }
        }

        // Bow
        if (controller.wpnHolster.SecondaryActive())
        {
            if (controller.baseLayerInfo.IsName("Reload") && weaponArmsID == 6)
            {
                if(controller.baseLayerInfo.normalizedTime > 0.6f)
                {
                    controller.wpnHolster.arrowE.gameObject.SetActive(true);
                }
            }

            if (controller.baseLayerInfo.IsName("Aim"))
            {
                controller.wpnHolster.arrowE.gameObject.SetActive(false);
                controller.wpnHolster.arrowString.gameObject.SetActive(true);
                controller.wpnHolster.secondaryE.GetComponent<Animator>().SetBool("Draw", true);
                controller.wpnHolster.secondaryE.GetComponent<Animator>().SetFloat("DrawPower", 1);
            }
        }

        controller.leftArmLayerWeight = Mathf.Clamp(controller.leftArmLayerWeight, 0, 1);
        controller.rightArmLayerWeight = Mathf.Clamp(controller.rightArmLayerWeight, 0, 1);

        #endregion

        if (controller.rightArmdInfo.IsTag("Sheath R"))
        {
            if (controller.rightArmdInfo.normalizedTime >= 0.45f)
            {
                // EQUIP
                #region Equip primary weapon

                if (controller.wpnHolster.PrimaryWeaponHActive() && !controller.wpnHolster.PrimaryWeaponActive())
                {
                    controller.wpnHolster.primaryE.gameObject.SetActive(true);
                    controller.wpnHolster.primaryH.gameObject.SetActive(false);
                }
                if (controller.wpnHolster.ArrowHActive() && !controller.wpnHolster.ArrowActive())
                {
                    controller.wpnHolster.arrowE.gameObject.SetActive(true);
                }

                #endregion

                #region Equip Rifle

                if (controller.wpnHolster.SecondaryHActive() && !controller.wpnHolster.SecondaryActive())
                {
                    controller.wpnHolster.secondaryE.gameObject.SetActive(true);
                    controller.wpnHolster.secondaryH.gameObject.SetActive(false);
                }

                #endregion
            }

            // ensures that reloading animation is finished before you can attack again
            if (controller.rightArmdInfo.normalizedTime > 0.8f)
            {
                isReloading = false;
            }
        }

        if (controller.rightArmdInfo.IsTag("Unsheathe R"))
        {
            if (controller.rightArmdInfo.normalizedTime >= 0.45f)
            {
                // REMOVE
                #region Remove primary Weapon

                if (controller.wpnHolster.PrimaryWeaponActive())
                {
                    controller.wpnHolster.primaryH.gameObject.SetActive(true);
                    controller.wpnHolster.primaryE.gameObject.SetActive(false);
                }

                #endregion

                #region Remove arrowE 

                if (controller.wpnHolster.ArrowActive())
                {
                    controller.wpnHolster.arrowE.gameObject.SetActive(false);
                }

                #endregion
            }
        }

        if (controller.leftArmdInfo.IsTag("Sheath L"))
        {
            if (controller.leftArmdInfo.normalizedTime >= 0.45f)
            {
                // EQUIP
                #region Equip secondary weapon

                if (controller.wpnHolster.SecondaryHActive() && !controller.wpnHolster.SecondaryActive())
                {
                    controller.wpnHolster.secondaryE.gameObject.SetActive(true);
                    controller.wpnHolster.secondaryH.gameObject.SetActive(false);
                }

                #endregion 
            }
        }

        if (controller.leftArmdInfo.IsTag("Unsheathe L"))
        {
            if (controller.leftArmdInfo.normalizedTime > 0.45f)
            {
                // REMOVE 
                #region Remove secondary weapon

                if (controller.wpnHolster.SecondaryActive() && !controller.wpnHolster.SecondaryHActive())
                {
                    controller.wpnHolster.secondaryH.gameObject.SetActive(true);
                    controller.wpnHolster.secondaryE.gameObject.SetActive(false);
                }

                #endregion
            }
        }
    }

    bool CanSheath()
    {
        if (controller.IsAnimatorTag("Sheath R") || controller.IsAnimatorTag("Unsheathe R")
            || controller.IsAnimatorTag("Sheath L") || controller.IsAnimatorTag("Unsheathe L"))
            return false;

        return true;
    }

    #endregion

    
    
    #region Dialogue

    void StartDialogueWithPlayer()
    {
        controller.dialogueAltered = controller.dialogue.dialogueAltered;

        if (controller.dialogue.npcName == "" || controller.dialogue.isDeactivated)
            return;

        float dist = Vector3.Distance(transform.position, cc.transform.position);
        if (dist < 2.5f)
        {
            if (interactUI)
            {
                if (!controller.dialogueActive && interactUI.fadeUI.canvasGroup.alpha == 0)
                {
                    if (interactUI.openSnd)
                        interactUI.openSnd.PlayRandomClip();

                    interactUI.buttonActionName = "Talk";
                    interactUI.fadeUI.FadeTransition(1, 0, 0.4f);
                }
                if (cc.rpgaeIM.PlayerControls.Interact.triggered || dialogueM.fadeUI.canvasGroup.alpha > 0)
                    interactUI.fadeUI.FadeTransition(0, 0, 0.25f);
            }
        }
        else
        {
            if (interactUI.fadeUI.canvasGroup.alpha == 1 && interactUI)
                interactUI.fadeUI.FadeTransition(0, 0, 0.4f);
        }

        if (!controller.dialogueActive)
            DefaultIdleDirection();

        if (dist < 2.5f)
        {
            if (controller.dialogueActive)
            {
                // NPC look at Player
                if (controller.rotateOnY)
                    LookAtTargetOnY(transform, cc.transform);

                // Player look at npc
                LookAtTargetOnY(cc.transform, transform);

                controller.animator.SetBool("InDialogue", true);
                if (controller.navmeshAgent)
                    controller.navmeshAgent.isStopped = true;
            }

            if (!controller.dialogueActive && cc.input == Vector3.zero && cc.cc.rpgaeIM.PlayerControls.Interact.triggered)
            {
                switch (controller.dialogue.dialogueType)
                {
                    case Dialogue.DialogueType.QuestGiver:
                        dialogueM.previousQuest = null;
                        dialogueM.currentQuest = controller.dialogue.questData;
                        break;
                    case Dialogue.DialogueType.QuestInProgress:
                        if (dialogueM.currentQuest != null)
                        {
                            dialogueM.previousQuest = dialogueM.currentQuest;
                            dialogueM.currentQuest = controller.dialogue.questData;
                        }
                        else
                            dialogueM.currentQuest = controller.dialogue.questData;
                        break;
                }

                cc.canMove = false;

                dialogueM.StartDialogue(controller.dialogue);
                dialogueM.fadeUI.FadeTransition(1, 0, 0.5f);
                controller.dialogue.controller = GetComponent<AIController>();

                cc.tpCam.AddTarget(this.gameObject.transform); 
                controller.dialogueActive = true;
            }
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

        if (controller.wpnHolster.PrimaryWeaponActive())
            controller.wpnHolster.primaryE.SetActive(false);
        if (controller.wpnHolster.SecondaryActive())
            controller.wpnHolster.secondaryE.SetActive(false);
        if (controller.wpnHolster.ArrowActive())
            controller.wpnHolster.arrowE.SetActive(false);

        if (GetComponent<Targetable>() != null)
            Destroy(GetComponent<Targetable>());

        controller.animator.SetBool("Dead", true);
        state = State.Dead;
    }

    public void Damaged(HealthPoints.DamageData data)
    {
        if (state == State.Dead) return;

        tempData = data;

        if (data.damager.GetComponent<HealthPoints>() != null && hp.curStunPoints > 0 && !controller.stunned)
            data.damager.GetComponent<HealthPoints>().curHealthPoints -= 1;

        if (hp.curStunPoints <= 0 && hp.curHealthPoints >= 0)
            {
            controller.stunned = true;
            hp.curStunPoints = 0;
            controller.animator.speed = 1;

            if (bloodEffect != null)
                bloodEffect.CreateBloodEffect(data.damageSource, data.damager.GetComponent<HitBox>().bloodEffectName);

            if (data.damager.GetComponent<HitBox>().isAttacking)
                data.damager.GetComponent<HitBox>().isAttacking = false;

            return;
        }

        // we ignore height difference if the target was already seen
        GameObject targetScanned = targetScanner.Detect(transform, enemyTarget == null);

        // we just saw the player for the first time, pick an empty spot to target around them
        if (targetScanned != null)
        {
            HealthPoints distributor = targetScanned.GetComponent<HealthPoints>();
            if (distributor != null && distributor.curHealthPoints > 0)
            {
                enemyTarget = distributor.gameObject;
            }
        }

        #region Blood Effect

        if (bloodEffect != null)
            bloodEffect.CreateBloodEffect(data.damageSource, data.damager.GetComponentInChildren<HitBox>().bloodEffectName);

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
            controller.animator.SetTrigger("KnockBack");
            controller.animator.SetInteger("KnockBackID", 1);
        }
        else if (data.damager.GetComponent<HitBox>().hurtID == 3)
        {
            controller.animator.SetTrigger("KnockBack");
            controller.animator.SetInteger("KnockBackID", 2);
            controller.AddForce(Vector3.up.normalized * 10, ForceMode.VelocityChange);
        }
        else if (data.damager.GetComponent<HitBox>().hurtID == 4)
        {
            controller.animator.SetTrigger("ShockHurt");
            controller.animator.SetInteger("ShockID", 1);
        }
        else if (data.damager.GetComponent<HitBox>().hurtID == 5)
        {
            controller.animator.SetTrigger("ShockHurt");
            controller.animator.SetInteger("ShockID", 2);
        }

        controller.animator.speed = 1;

        #endregion

        #region Particle Effect

        if (data.damager.GetComponent<HitBox>() != null && 
            data.damager.GetComponent<HitBox>().effects.stickOnContactEffectHit != null)
        {
            if (!GetComponent<HealthPoints>().stickOnActive)
            {
                GameObject stickEffect = Instantiate(data.damager.GetComponent<HitBox>().effects.stickOnContactEffectHit.gameObject,
                controller.animator.GetBoneTransform(HumanBodyBones.Hips).transform.position, transform.rotation) as GameObject;

                stickEffect.transform.parent = transform;
                GetComponent<HealthPoints>().stickOnActive = true;
            }
            GetComponent<HealthPoints>().stickOnEffectTimer = 5;
        }

        #endregion

        if (data.damager.GetComponent<ItemData>() != null &&
        data.damager.GetComponent<ItemData>().itemType == ItemData.ItemType.Arrow)
        {
            data.damager.GetComponent<HitBox>().DeactivateProjectile();
        }

        data.damager.GetComponent<HitBox>().isAttacking = false;
    }

    void DeathDrop()
    {
        hp.curHealthPoints = 0;
        itemDropTimer += Time.deltaTime;
        if (itemDropTimer > dropItemTimerMax)
        {
            for (int i = 0; i < itemDrops.Length; i++)
            {
                if(itemDrops[i] != null)
                {
                    GameObject items = Instantiate(itemDrops[i], transform.position + new Vector3(Random.Range(-1.2f, 1.2f),
                    1.2f, Random.Range(-1.2f, 1.2f)), Quaternion.Euler(0, 0, 90)) as GameObject;
                    items.GetComponentInChildren<ItemData>().itemActive = true;
                    items.SetActive(true);
                }
            }
            hudM.IncrementExperience(Exp);

            if (GetComponentInChildren<MiniMapItem>())
                Destroy(GetComponentInChildren<MiniMapItem>());

            Destroy(GetComponent<HumanoidBehavior>());
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
                controller.stunned = false;
            }

            if (controller.IsAnimatorTag("Attack"))
            {
                Vector3 targetDirection = enemyTarget.transform.position - transform.position;

                targetDirection.y = 0;
                targetDirection.Normalize();
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 4000);

                Debug.Log("-------- StunnedState");
            }
        }

        #endregion

        #region State Transition

        #region Get Pivot Angle

        if (!controller.IsAnimatorTag("Pivot"))
            GetPivotAngle(enemyTarget.transform.position);

        #endregion

        if (!controller.stunned)
        {
            controller.navmeshAgent.enabled = true;

            state = State.Pursuit;
        }
        else
            controller.navmeshAgent.enabled = false;

        #endregion

        #region Stun Finisher

        if (hp.curStunPoints <= 0)
        {
            if (hp.curHealthPoints <= 0)
            {
                hp.curHealthPoints = 0;
                state = State.Dead;
                return;
            }

            if (curStunnedRecoveryTime > stunFinisherMaxtime)
            {
                if (interactUI.fadeUI.canvasGroup.alpha == 1)
                {
                    interactUI.fadeUI.FadeTransition(0, 0, 0.2f);
                }

                hp.curStunPoints = hp.maxStunPoints;
                curStunnedRecoveryTime = 0;
                controller.stunned = false;
            }

            if (controller.fullBodyInfo.IsName("Stunned Loop"))
            {
                if (interactUI.fadeUI.canvasGroup.alpha == 0)
                {
                    interactUI.buttonActionName = "Finisher";
                    interactUI.fadeUI.FadeTransition(1, 0, 0.2f);

                    if (bloodEffect != null)
                    {
                        bloodEffect.CreateBloodEffect(controller.animator.GetBoneTransform(HumanBodyBones.Head).transform.position + (transform.up * 0.28f) , "Small Bleedout Hit");
                    }
                }
            }
        }

        Finisher();

        #endregion
    }

    void Finisher()
    {
        if (controller.fullBodyInfo.IsName("Finisher"))
        {
            if (controller.fullBodyInfo.normalizedTime > 0.9f)
            {
                if (cc.wpnHolster.PrimaryWeaponActive())
                    cc.wpnHolster.primaryE.GetComponent<HitBox>().trailActive = false;

                if (GetComponent<Targetable>() != null)
                    Destroy(GetComponent<Targetable>());
                if (cc.tpCam.lockedOn)
                    cc.tpCam.ToggleLockOn(!cc.tpCam.lockedOn);

                cc.isStrafing = false;
                controller.finisherInProcess = false;
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
                if (distributor != null && distributor.curHealthPoints > 0)
                {
                    enemyTarget = distributor.gameObject;
                    targetScanner.detectionRadius += targetScanner.detectionRadiusWhenSpotted;
                }
            }
        }
        else
        {
            if (enemyTarget.GetComponent<HealthPoints>().curHealthPoints <= 0)
            {
                controller.animator.SetBool("Patrol", true);
                controller.animator.SetBool("Pursuit", false);

                if (followCompanion != null)
                {
                    state = State.FollowCompanion;

                    enemyTarget = null;
                    targetScanner.detectionRadius -= targetScanner.detectionRadiusWhenSpotted;
                }
                else
                {
                    Vector3 toOriginPosition = originalPosition - transform.position;
                    toOriginPosition.y = 0;
                    if (toOriginPosition.sqrMagnitude > 1)
                        state = State.WalkBackToBase;

                    enemyTarget = null;
                    targetScanner.detectionRadius -= targetScanner.detectionRadiusWhenSpotted;
                }
                return;
            }

            if (state != State.Pursuit) 
                return;

            m_TimerSinceLostTarget += Time.deltaTime;
            if (m_TimerSinceLostTarget >= timeToStopPursuit)
            {
                Vector3 toTarget = enemyTarget.transform.position - transform.position;

                if (toTarget.sqrMagnitude > targetScanner.detectionRadius * targetScanner.detectionRadius || 
                    enemyTarget.GetComponent<HealthPoints>().curHealthPoints <= 0)
                {
                    // the target move out of range, reset the target
                    enemyTarget = null;
                    targetScanner.detectionRadius -= targetScanner.detectionRadiusWhenSpotted;
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
            lookWeight = 1;
        else
            lookWeight = 0;

        if (enemyTarget == null) return;

        lookWeight = Mathf.Clamp(lookWeight, 0, 1);

        // target position
        Vector3 targetPosition = enemyTarget.transform.position +
        (enemyTarget.transform.up * enemyTarget.GetComponent<CapsuleCollider>().height / 2);

        bool conditions = state == State.CombatStance;
        controller.animator.SetLookAtWeight(1, lookWeight, conditions ? 0 : 1);
        controller.animator.SetLookAtPosition(targetPosition);

        LeftHandWeaponHandleIK();
    }

    void LeftHandWeaponHandleIK()
    {
        if (controller.wpnHolster.PrimaryWeaponActive())
        {
            if (weaponArmsID == 3 || weaponArmsID == 4)
            {
                foreach (Transform eachChild in controller.wpnHolster.primaryE.transform)
                {
                    if (eachChild.name == "LWeaponHandleIK")
                    {
                        leftHandIK = eachChild;
                    }
                }

                if (leftHandIK != null && controller.leftArmLayerWeight == 1)
                {
                    if (controller.baseLayerInfo.IsName("Heavy Charge") || controller.IsAnimatorTag("Hit") || 
                        controller.IsAnimatorTag("Attack") || controller.IsAnimatorTag("Get up"))
                    {
                        controller.animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                        controller.animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
                    }
                    else
                    {
                        controller.animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                        controller.animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                    }
                    controller.animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandIK.position);
                    controller.animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandIK.rotation);
                }
            }
        }
    }

    #endregion

    #region Animation Event

    public void LastFootStepEvent(int footStep)
    {
    }

    public void AttackPhaseContextEvent(string context)
    {
        if (GetComponent<HumanoidBehavior>())
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

    public void UnarmedAttackEvent(string context)
    {
        #region Context

        if (context == "Left Punch")
        {
            controller.animator.GetBoneTransform(HumanBodyBones.LeftHand).GetComponent<HitBox>().hurtID = 1;
            controller.animator.GetBoneTransform(HumanBodyBones.LeftHand).GetComponent<HitBox>().BeginAttack("Small Blood Hit");
        }

        if (context == "Right Punch")
        {
            controller.animator.GetBoneTransform(HumanBodyBones.RightHand).GetComponent<HitBox>().hurtID = 1;
            controller.animator.GetBoneTransform(HumanBodyBones.RightHand).GetComponent<HitBox>().BeginAttack("Small Blood Hit");
        }

        if (context == "Right Kick")
        {
            controller.animator.GetBoneTransform(HumanBodyBones.RightFoot).GetComponent<HitBox>().hurtID = 1;
            controller.animator.GetBoneTransform(HumanBodyBones.RightFoot).GetComponent<HitBox>().BeginAttack("Small Blood Hit");
        }

        if (context == "Heavy Attack")
        {
            controller.animator.GetBoneTransform(HumanBodyBones.RightHand).GetComponent<HitBox>().hurtID = 2;
            controller.animator.GetBoneTransform(HumanBodyBones.RightHand).GetComponent<HitBox>().BeginAttack("Medium Blood Hit");
        }

        if (context == "End")
        {
            controller.animator.GetBoneTransform(HumanBodyBones.LeftHand).GetComponent<HitBox>().EndAttack();

            controller.animator.GetBoneTransform(HumanBodyBones.RightHand).GetComponent<HitBox>().EndAttack();

            controller.animator.GetBoneTransform(HumanBodyBones.LeftFoot).GetComponent<HitBox>().EndAttack();

            controller.animator.GetBoneTransform(HumanBodyBones.RightFoot).GetComponent<HitBox>().EndAttack();
        }

        #endregion
    }

    public void ArmedAttackEvent(string context)
    {
        if(context == "Light Attack")
        {
            controller.wpnHolster.primaryE.GetComponent<HitBox>().hurtID = 1;
            controller.wpnHolster.primaryE.GetComponent<HitBox>().BeginAttack("Small Blood Hit");
        }

        if (context == "Heavy Attack")
        {
            controller.wpnHolster.primaryE.GetComponent<HitBox>().hurtID = 2;
            controller.wpnHolster.primaryE.GetComponent<HitBox>().BeginAttack("Medium Blood Hit");
        }

        if (context == "End")
        {
            controller.wpnHolster.primaryE.GetComponent<HitBox>().EndAttack();
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

    public void WeaponThrowEvent()
    {

    }

    public void ShootEvent(string context)
    {
        if (context == "Start" && weaponArmsID == 6)
        {
            ItemData arrowData = controller.wpnHolster.secondaryE.GetComponentInChildren<ItemData>();

            Vector3 targetPosition = enemyTarget.transform.position +
            (enemyTarget.transform.up * enemyTarget.GetComponent<CapsuleCollider>().height / 2);

            Vector3 newDir = targetPosition - controller.wpnHolster.arrowPrefabSpot.transform.position;
            newDir.Normalize();

            GameObject _arrow = Instantiate(equippedArrow.gameObject, controller.wpnHolster.arrowPrefabSpot.transform.position,
            Quaternion.LookRotation(newDir)) as GameObject;

            _arrow.GetComponentInChildren<HitBox>().projectileVel = new Vector3(0, 8, 100);
            _arrow.SetActive(true);
            _arrow.GetComponentInChildren<HitBox>().targetLayers = targetScanner.targetLayer;
            _arrow.GetComponentInChildren<HitBox>().isAttacking = true;
            _arrow.GetComponentInChildren<HitBox>().isProjectile = true;
            _arrow.GetComponentInChildren<HitBox>().weaponTrail = true;
            _arrow.GetComponentInChildren<ItemData>().itemActive = true;
            _arrow.GetComponentInChildren<ItemData>().quantity = 1;
            _arrow.GetComponentInChildren<HitBox>().BeginAttack("Small Blood Hit");

            controller.wpnHolster.secondaryE.GetComponent<HitBox>().PlayRandomSound("BowShotAS", false);

            controller.wpnHolster.secondaryE.GetComponentInChildren<Animator>().SetBool("Draw", false);
            controller.wpnHolster.secondaryE.GetComponentInChildren<Animator>().SetFloat("DrawPower", 0);

            controller.wpnHolster.SetArrowDamage(ref _arrow, ref arrowData);
            controller.wpnHolster.arrowE.gameObject.SetActive(false);
            controller.wpnHolster.arrowString.gameObject.SetActive(false);

            isReloading = true;
        }

        if (context == "Start" && weaponArmsID == 7)
        {
            Vector3 targetPosition = enemyTarget.transform.position +
            (enemyTarget.transform.up * enemyTarget.GetComponent<CapsuleCollider>().height / 2);

            Vector3 newDir = targetPosition - controller.wpnHolster.arrowPrefabSpot.transform.position;
            newDir.Normalize();

            GameObject _bullet = Instantiate(equippedArrow.gameObject, controller.wpnHolster.arrowPrefabSpot.transform.position,
            Quaternion.LookRotation(newDir)) as GameObject;

            _bullet.GetComponentInChildren<HitBox>().projectileVel = new Vector3(0, 8, 90);
            _bullet.GetComponentInChildren<HitBox>().targetLayers = targetScanner.targetLayer;
            _bullet.GetComponentInChildren<HitBox>().isAttacking = true;
            _bullet.GetComponentInChildren<HitBox>().isProjectile = true;
            _bullet.GetComponentInChildren<HitBox>().weaponTrail = true;
            _bullet.GetComponentInChildren<HitBox>().BeginAttack("Small Blood Hit");
            _bullet.SetActive(true);

            controller.wpnHolster.secondaryE.GetComponent<HitBox>().PlayRandomSound("RifleShotAS", false);
            controller.wpnHolster.secondaryE.GetComponent<HitBox>().effects.signatureEffect.gameObject.SetActive(true);
            controller.wpnHolster.secondaryE.GetComponent<HitBox>().effects.signatureEffect.Play();

            isReloading = true;
        }

        if(context == "End")
        {
            controller.wpnHolster.secondaryE.GetComponent<HitBox>().effects.signatureEffect.Stop();
            controller.wpnHolster.secondaryE.GetComponent<HitBox>().effects.signatureEffect.gameObject.SetActive(false);
        }
    }

    public void ReloadEvent()
    {
        controller.wpnHolster.secondaryE.GetComponent<HitBox>().PlayRandomSound("RifleReloadAS", false);
    }

    #endregion

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        targetScanner.EditorGizmo(transform);
    }

    void OnDrawGizmos()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;
        Transform oldT = patrolPoints[0].transform;
        Gizmos.color = new Color(0, 1, 0, 0.5f);

        if (!Application.isPlaying)
        {
            transform.position = patrolPoints[destPoint].transform.position;
            transform.eulerAngles = patrolPoints[destPoint].transform.eulerAngles;
        }

        foreach (Transform t in patrolPoints)
        {
            if (t.transform != null && t.transform != oldT)
            {
                Gizmos.DrawLine(oldT.position, t.transform.position);
                oldT = t.transform;
            }
        }

        foreach (Transform t in patrolPoints)
        {
            if (t.transform)
            {
                Matrix4x4 rotationMatrix = Matrix4x4.TRS(t.transform.position, t.transform.rotation, transform.lossyScale);
                Gizmos.matrix = rotationMatrix;
                Gizmos.DrawCube(Vector3.zero, Vector3.one);
            }
        }
    }

#endif

}