using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine.AI;
using UnityEngine;
using RPGAE.CharacterController;

public class AIController : MonoBehaviour
{
    [HideInInspector] public bool isDead;
    [HideInInspector] public bool stunned;
    [HideInInspector] public bool canPerfectDodge = false;
    [HideInInspector] public bool finisherInProcess;
    [HideInInspector] public bool subjectedToSlowDown;

    [Header("DIALOGUE SETTINGS")]
    public Dialogue dialogue;
    public bool dialogueActive;
    public bool dialogueAltered;
    public bool rotateOnY;

    [Header("QUEST SETTINGS")]
    [Tooltip("The name of the current quest you're progressing through.")]
    public string questSequenceNameReceipt;
    [Tooltip("The required phase number in order to progress to the next.")]
    public int questPhaseReceipt;
    [Tooltip("Once the required action is meet you will increment a phase.")]
    public int addQuestPhaseBy = 1;
    [Tooltip("Once the required action is meet you will increment a quantity amount.")]
    public int addQuestQuantityBy;

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
    [HideInInspector] public State state;

    [HideInInspector]
    public float
    upperBodyID,
    faceLayerWeight,
    rightArmLayerWeight,
    leftArmLayerWeight,
    upperBodyWeight,
    fullBodyWeight;

    [HideInInspector]
    public AnimatorStateInfo
    baseLayerInfo,
    faceLayerInfo,
    rightArmdInfo,
    leftArmdInfo,
    onlyArmsInfo,
    upperBodyInfo,
    fullBodyInfo;

    [HideInInspector] public Animator animator;
    [HideInInspector] public Rigidbody rigidBody;
    [HideInInspector] public CapsuleCollider capsuleC;
    [HideInInspector] public NavMeshAgent navmeshAgent;
    [HideInInspector] public ThirdPersonController cc;
    [HideInInspector] public WeaponHolster wpnHolster;

    [HideInInspector]
    public bool grounded;
    [HideInInspector]
    public float k_GroundedRayDistance = 0.8f;
    [HideInInspector]
    public bool m_UnderExternalForce;
    [HideInInspector]
    float defaultRadius;


    void Awake()
    {
        navmeshAgent = GetComponent<NavMeshAgent>();

        capsuleC = GetComponent<CapsuleCollider>();
        defaultRadius = capsuleC.radius;

        dialogue.controller = GetComponent<AIController>();
        dialogue.questData = GetComponentInChildren<QuestData>();

        wpnHolster = GetComponentInChildren<WeaponHolster>();

        animator = GetComponent<Animator>();

        animator.updateMode = AnimatorUpdateMode.Fixed;
        if (navmeshAgent)
            navmeshAgent.updatePosition = false;

        rigidBody = GetComponentInChildren<Rigidbody>();
        if (rigidBody == null)
            rigidBody = gameObject.AddComponent<Rigidbody>();

        cc = FindObjectOfType<ThirdPersonController>();

        rigidBody.isKinematic = true;
        rigidBody.useGravity = true;
        rigidBody.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Update()
    {
        LayerInfo();
    }

    void FixedUpdate()
    {
        if (cc.slowDown) return;

        CheckGrounded();
        LockOnColliderSize();
        IgnoreColllisionOnForce();

        if (grounded && navmeshAgent && !m_UnderExternalForce)
        {
            transform.position = navmeshAgent.nextPosition;
            navmeshAgent.velocity = (animator.deltaPosition / Time.deltaTime);
        }
    }


    #region Animator

    void LayerInfo()
    {
        animator.SetLayerWeight(1, faceLayerWeight);
        animator.SetLayerWeight(2, leftArmLayerWeight);
        animator.SetLayerWeight(3, rightArmLayerWeight);
        animator.SetLayerWeight(4, upperBodyWeight);
        animator.SetLayerWeight(5, fullBodyWeight);
        baseLayerInfo = animator.GetCurrentAnimatorStateInfo(0);
        faceLayerInfo = animator.GetCurrentAnimatorStateInfo(1);
        leftArmdInfo = animator.GetCurrentAnimatorStateInfo(2);
        rightArmdInfo = animator.GetCurrentAnimatorStateInfo(3);
        upperBodyInfo = animator.GetCurrentAnimatorStateInfo(4);
        fullBodyInfo = animator.GetCurrentAnimatorStateInfo(5);
    }

    public bool IsAnimatorTag(string tag)
    {
        if (animator == null) return false;
        if (baseLayerInfo.IsTag(tag)) return true;
        if (faceLayerInfo.IsTag(tag)) return true;
        if (rightArmdInfo.IsTag(tag)) return true;
        if (leftArmdInfo.IsTag(tag)) return true;
        if (upperBodyInfo.IsTag(tag)) return true;
        if (fullBodyInfo.IsTag(tag)) return true;
        return false;
    }

    #endregion

    void LockOnColliderSize()
    {
        if (cc.tpCam.lockedOn)
        {
            capsuleC.radius = 1;
        }
        else
            capsuleC.radius = defaultRadius;
    }

    public void AddForce(Vector3 force, ForceMode forceMode)
    {
        rigidBody.useGravity = true;
        rigidBody.isKinematic = false;
        navmeshAgent.enabled = false;
        m_UnderExternalForce = true;

        rigidBody.AddForce(force, forceMode);
    }

    void IgnoreColllisionOnForce()
    {
        Collider[] allThisColliders = GetComponentsInChildren<Collider>();
        foreach (var thisCollider in allThisColliders)
        {
            if (GetComponent<HumanoidBehavior>() != null && GetComponent<HumanoidBehavior>().enemyTarget != null)
            {
                Collider[] allCollidersEnemy = GetComponent<HumanoidBehavior>().enemyTarget.GetComponentsInChildren<Collider>();
                foreach (var enemyCollider in allCollidersEnemy)
                {
                    Physics.IgnoreCollision(enemyCollider, thisCollider, m_UnderExternalForce);
                }
            }
        }
        foreach (var thisCollider in allThisColliders)
        {
            if (GetComponent<LuBehavior>() != null && GetComponent<LuBehavior>().enemyTarget != null)
            {
                Collider[] allCollidersEnemy = GetComponent<LuBehavior>().enemyTarget.GetComponentsInChildren<Collider>();
                foreach (var enemyCollider in allCollidersEnemy)
                {
                    Physics.IgnoreCollision(enemyCollider, thisCollider, m_UnderExternalForce);
                }
            }
        }
    }

    public void ClearForce()
    {
        m_UnderExternalForce = false;
        rigidBody.isKinematic = true;
        navmeshAgent.enabled = true;
    }

    public bool SetDestination(Vector3 position)
    {
        if (!navmeshAgent.enabled) return false;

        return navmeshAgent.SetDestination(position);
    }

    public void RotateWithNavMeshAgent()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, navmeshAgent.transform.rotation,
            navmeshAgent.angularSpeed * Time.deltaTime);
    }

    public void CheckGrounded()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position + Vector3.up * k_GroundedRayDistance * 0.5f, -Vector3.up);
        grounded = Physics.Raycast(ray, out hit, k_GroundedRayDistance, Physics.AllLayers,
        QueryTriggerInteraction.Ignore);

        if (navmeshAgent)
            navmeshAgent.enabled = grounded;
        if (rigidBody)
            rigidBody.isKinematic = grounded;
    }
}
