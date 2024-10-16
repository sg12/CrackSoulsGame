using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine;
using BLINK.Controller;

namespace RPGAE.CharacterController
{
    public class ThirdPersonMotor : ThirdPersonInput, DamageReceiver
    {
        
        #region General Settings 

        [Header("MOVEMENT SETTINGS")]
        public bool canMove;
        public float rotationSpeed = 18f;
        public float jumpForce = 8f;

        [Range(0f, 4f)]
        public float gravityMultiplier = 2f;

        public bool canJump;
        public bool isJumping;

        #endregion

        #region Combat Settings

        [Header("COMBAT SETTINGS")]
        public LayerMask targetLayer;
        public bool useStatSpeed;
        [HideInInspector] public bool slowDown;
        public float timeSpeed;
        public float attackPower;
        public float slowMotionDuration;
        [HideInInspector] public bool performingLightAttack;
        [HideInInspector] public bool performingHeavyAttack;
        [HideInInspector] public bool bowRushOneShot;
        [HideInInspector] public float slowMotionTimer;
        [HideInInspector] public Volume slowDownVolume;

        [HideInInspector] public Vector3 projectileSpawnPoint;
        [HideInInspector] public Quaternion aimRotationPoint;

        [HideInInspector] public Vector3 oldWeaponT;
        [HideInInspector] public Quaternion oldWeaponRot;
        [HideInInspector] public Vector3 finisherWeaponT;
        [HideInInspector] public Quaternion finisherWeaponRot;
        private float finisherDist;

        #endregion

        #region Climb Settings 

        [Header("CLIMB SETTINGS")]
        public Transform handTarget;
        public Vector3 handTargetPosition
        {
            get
            {
                return transform.TransformPoint(handTarget.localPosition.x, handTarget.localPosition.y, 0);
            }
        }
        public float ledgeStance;
        [HideInInspector] public Vector3 bracedTargetPosition;
        [HideInInspector] public Vector3 hangingTargetPosition;

        public float climbEnterMaxDistance = 1f;

        bool canCornerEdgeLerp;
        Vector3 cornerEdgePoint;
        Quaternion cornerEdgePointRot;

        public float oldInput;
        public float climbEnterSpeed = 5f;
        public float climbUpHeight = 2f;

        public float climbJumpDistance = 2f;
        public float climbJumpDepth = 2f;

        public float offsetHandTarget = -0.3f;
        public float lastPointDistanceH = 0.4f;
        public float lastPointDistanceVUp = 1.25f;
        public float lastPointDistanceVDown = 1.25f;

        [Tooltip("Min Wall thickeness to climbUp")]
        public float climbUpMinThickness = 0.9f;
        [Tooltip("Min space  to climbUp with obstruction")]
        public float climbUpMinSpace = 0.5f;
        public float climbJumpEdgeDistance = 3.5f;

        private bool inAlingClimb;
        private bool inClimbUp;
        [HideInInspector] public bool inClimbEnter;
        private bool inClimbEnterAir;
        [HideInInspector] public bool canClimbDown;
        [HideInInspector] public bool canJumpAcross;
        [HideInInspector] public bool isShortLedge;
        private bool canMoveClimb;
        [HideInInspector] public bool canClimbJump;
        [HideInInspector] public bool inClimbJump;
        [HideInInspector] public bool canJumpBackOnLedge;
        [HideInInspector] public bool canFreeJump;

        public LayerMask obstacle;
        public LayerMask climbableSurface;
        public LayerMask climbableLedge;

        [System.Serializable]
        public class ClimbData
        {
            public bool canClimb;
            public bool inPosition;

            public Vector3 position;
            public Vector3 targetNormal;
            public Transform targetTransform;
        }
        public ClimbData climbData;
        public ClimbData jumpClimbData;

        private RaycastHit climbHit;

        Vector3 fwdRayCastHitForSmallLedge;
        public GameObject m_closestSmallLedge;

        public enum ClimbState
        {
            NA,
            GroundedClimbDownToHang,
            GroundedJumpAcrossToHang
        }
        [HideInInspector] public ClimbState climbState;

        #endregion

        #region Swim Settings

        [Header("SWIMMING SETTINGS")]
        public bool canSwim = true;
        public bool isSwimming = false;
        public bool dived;

        public float diveSpeed = 4;
        public float waterDrag = 1.4f;
        public float offsetToSurf = 2.2f;
        public float distY = 0.0f;
        public float waterHeightLevel;
        public Vector3 liftVector;

        [Header("Water Particle System")]
        public GameObject waterbubblesPFX;
        [HideInInspector] public GameObject waterbubbles;
        public GameObject waterImpactPFX;
        [Tooltip("Check the Rigibody.Y of the character to trigger the ImpactEffect Particle")]
        public float velocityToImpact = -4f;
        public GameObject waterRipplesPFX;
        [Tooltip("Frequency to instantiate the WaterRing effect while standing still")]
        public float waterRingFrequencyIdle = 1.25f;
        [Tooltip("Frequency to instantiate the WaterRing effect while swimming")]
        public float waterRingFrequencySwim = 0.25f;
        public GameObject waterDripsPFX;
        [HideInInspector] public Transform waterDripPos;
        [Tooltip("Y Offset based at the capsule collider")]
        public float waterDropsYOffset = 1.6f;

        [HideInInspector] public float defaultRigidBodyMass;
        float curAngle = 0.0f;
        Vector3 diveVec;

        private float waterRingSpawnFrequency;
        private float canSwimOnSurfaceTimer;
        [HideInInspector] public Transform rootB = null;

        #endregion

        #region Ground Settings

        [Header("GROUND SETTINGS")]
        public bool grounded;
        public LayerMask groundLayer;
        public float groundDistance;
        [HideInInspector] public float colliderHeight;
        [HideInInspector] public Vector3 colliderCenter;
        [HideInInspector] public float colliderRadius;
        public float groundMinDistance = 0.25f;
        private float groundMaxDistance = 0.5f;
        public float groundCheckHeight = 0.5f;
        public float groundCheckRadius = 0.5f;
        public float groundCheckDistance = 0.3f;
        public RaycastHit groundHit;
        public RaycastHit currentGroundInfo;
        public float stepOffsetEnd = 0.45f;
        public float stepOffsetStart = 0.05f;
        public float stepSmooth = 4f;
        private float slopeLimit = 45f;
        [Tooltip("Velocity to slide when on a slope limit ramp")]
        [Range(0, 12)]
        public float slideVelocity = 12;

        #endregion

        #region Audio Settings

        [Header("AUDIO")]
        public RandomAudioPlayer jumpAS;
        public RandomAudioPlayer landAS;
        public RandomAudioPlayer waterSplashAS;
        public AudioSource underwaterAS;
        public RandomAudioPlayer slowMotionStartAS;
        public RandomAudioPlayer slowMotionEndAS;
        public RandomAudioPlayer drawAS;
        public RandomAudioPlayer putAwayAS;

        #endregion

        #region Components

        internal Rigidbody rigidBody;                                                
        internal PhysicsMaterial frictionPhysics, maxFrictionPhysics, slippyPhysics;    
        internal CapsuleCollider capCollider;
        HealthPoints hp;
        BloodEffect bloodEffect;

        #endregion

        #region Actions Bools

        [HideInInspector]
        public bool
        preventAtkInteruption,
        isAiming,
        isSliding,
        isSprinting,
        isStrafing,
        isDodging,
        isSheathing,
        isAttacking,
        slowMotion,
        isBlocking,
        isReloading,
        isCarrying,
        canShoot,
        isKnockedBack;

        #endregion

        #region Hide Variables

        [HideInInspector] public int actionState = 0;
        [HideInInspector] public float timer;
        [HideInInspector] public float velocity;              
        [HideInInspector] public float verticalVelocity;
        [HideInInspector] public float footStepCycle;
        float currentFinisherPosition;
        bool finisherInProcess;

        [HideInInspector] public Vector3 input;
        [HideInInspector] public Vector3 targetDirection;
        [HideInInspector] public Quaternion targetRotation;
        [HideInInspector] public Vector3 lookPosition;
        [HideInInspector] public Vector3 dodgeLastInput;
        [HideInInspector] public Vector3 climbLastInput;

        #endregion

        public void Init()
        {
            animator.updateMode = AnimatorUpdateMode.Fixed;

            // slides the character through walls and edges
            frictionPhysics = new PhysicsMaterial();
            frictionPhysics.name = "frictionPhysics";
            frictionPhysics.staticFriction = .25f;
            frictionPhysics.dynamicFriction = .25f;
            frictionPhysics.frictionCombine = PhysicsMaterialCombine.Multiply;

            // prevents the collider from slipping on ramps
            maxFrictionPhysics = new PhysicsMaterial();
            maxFrictionPhysics.name = "maxFrictionPhysics";
            maxFrictionPhysics.staticFriction = 1f;
            maxFrictionPhysics.dynamicFriction = 1f;
            maxFrictionPhysics.frictionCombine = PhysicsMaterialCombine.Maximum;

            // air physics 
            slippyPhysics = new PhysicsMaterial();
            slippyPhysics.name = "slippyPhysics";
            slippyPhysics.staticFriction = 0f;
            slippyPhysics.dynamicFriction = 0f;
            slippyPhysics.frictionCombine = PhysicsMaterialCombine.Minimum;

            // get components
            rigidBody = GetComponent<Rigidbody>();
            capCollider = GetComponent<CapsuleCollider>();
            hp = GetComponentInChildren<HealthPoints>();
            bloodEffect = GetComponentInChildren<BloodEffect>();
            slowDownVolume = GetComponentInChildren<Volume>();

            #region Swimming Init

            defaultRigidBodyMass = rigidBody.mass;

            rootB = animator.GetBoneTransform(HumanBodyBones.Hips).transform;
            waterbubbles = Instantiate(waterbubblesPFX, animator.GetBoneTransform(HumanBodyBones.Head).position,
            animator.GetBoneTransform(HumanBodyBones.Head).rotation, animator.GetBoneTransform(HumanBodyBones.Head));
            waterbubbles.SetActive(false);

            #endregion

            cc.lookWeight = 0;
            waterHeightLevel = 0;
            climbState = ClimbState.NA;
            grounded = true;
        }

        public virtual void UpdateMotor()
        {
            if (input.magnitude > 0 && !isDodging)
                dodgeLastInput = input.normalized;

            CheckGround();
            SwimBehavior();

            ExtraTurnRotation();

            HolsterBehaviour();

            if (!canSwim && !isSwimming)
                CheckForJump();

            CombatManagement();
            ClimbManagement();
        }

        void ExtraTurnRotation()
        {
            if (tpCam == null || cc.IsAnimatorTag("Intro") || animator.GetBool("Dead") == true) return;

            Vector3 forward = tpCam.transform.TransformDirection(Vector3.forward);
            forward.y = 0;
            Vector3 right = new Vector3(forward.z, 0, -forward.x);
            targetDirection = forward * input.z + right * input.x;
            targetDirection.Normalize();

            float rotateAngle = Vector3.Angle(transform.forward, targetDirection);

            bool cannotRotate = rotateAngle > 100f || cc.IsAnimatorTag("Idle") || cc.IsAnimatorTag("180") 
            || cc.IsAnimatorTag("Action") || cc.IsAnimatorTag("Heavy Attack") || 
            cc.IsAnimatorTag("Ability Attack") || isStrafing || isAiming || climbData.canClimb 
            || climbData.inPosition || isSwimming || inventoryM.inventoryHUD.isActive || !inventoryM.ConditionsToOpenMenu() ||
            systemM.blackScreenFUI.canvasGroup.alpha != 0 || systemM.loadingScreenFUI.canvasGroup.alpha != 0 || finisherInProcess;

            if (cannotRotate)
                return;

            if (targetDirection != Vector3.zero)
            {
                targetRotation = Quaternion.LookRotation(targetDirection);
                Quaternion newRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                transform.rotation = newRotation;
            }
        }

        #region Climb Behaviour

        public void ClimbManagement()
        {
            Debug.DrawRay(animator.GetBoneTransform(HumanBodyBones.LeftFoot).transform.position + ((transform.right * -0.25f) - (-transform.forward * 0.5f)), transform.forward * 1, Color.blue);

            bool conditionsToClimb = !cc.IsAnimatorTag("Carry");
            //Debug.DrawRay(transform.position + (transform.up * 0.5f), -transform.up * 10, Color.blue);
            if (conditionsToClimb && Physics.Raycast(handTargetPosition, transform.forward, out climbHit, Mathf.Infinity, climbableSurface) && !climbData.inPosition)
            {
                Vector3 wallSurfaceHit = climbHit.point;
                if(climbHit.distance < 1.5f)
                {
                    #region Climb Methods 

                    if (grounded)
                        fwdRayCastHitForSmallLedge = climbHit.point;

                    if (grounded)
                    {
                        Debug.DrawRay(wallSurfaceHit, transform.up * 3f, Color.red);
                        if (Physics.Raycast(wallSurfaceHit, transform.up, out climbHit, 5, climbableLedge))
                        {
                            hudM.ShowInteractiveIcon("Move", "Climb Jump");

                            climbData.targetTransform = climbHit.transform;

                            if (climbData.targetTransform != null && climbData.targetTransform.gameObject.name == "LongLedge" ||
                            climbData.targetTransform.gameObject.name == "LongLedgeTop")
                            {
                                climbData.targetTransform = climbHit.transform;
                                if (input.magnitude > 0.1f && cc.rpgaeIM.PlayerControls.Action.triggered && Time.time > (oldInput + 0.5f))
                                {
                                    climbData.position = climbHit.point;
                                    climbData.targetTransform = climbHit.transform;
                                    climbData.canClimb = true;
                                    EnterClimb();
                                }
                            }
                        }

                        if (input.magnitude > 0.1f && cc.rpgaeIM.PlayerControls.Action.triggered)
                        {
                            FindClosestSmallClimbSpot();
                            if (climbData.targetTransform != null && climbData.targetTransform.name == "SmallLedge")
                                EnterClimb();
                        }
                    }
                    else if (!grounded)
                    {
                        Debug.DrawRay(wallSurfaceHit, -transform.up * 10, Color.blue);
                        if (Physics.Raycast(wallSurfaceHit, -transform.up, out climbHit, Mathf.Infinity, climbableLedge) && !isSwimming)
                        {
                            hudM.ShowInteractiveIcon("Move", "Climb Jump");

                            Vector3 ledgeFromTopHit = climbHit.point;
                            Debug.DrawRay(ledgeFromTopHit, -transform.forward * 0.5f, Color.blue);
                            Debug.DrawRay(ledgeFromTopHit + (-transform.forward * 0.5f), -transform.up * 0.5f, Color.blue);
                            Debug.DrawRay(ledgeFromTopHit + ((-transform.forward * 0.5f) - (transform.up * 0.5f)), transform.forward * 10, Color.blue);
                            if (climbHit.distance < 2)
                            {
                                if (Physics.Raycast(ledgeFromTopHit + ((-transform.forward * 0.5f) - (transform.up * 0.5f)), transform.forward, out climbHit, Mathf.Infinity, climbableSurface))
                                {
                                    if (Physics.Raycast(wallSurfaceHit, transform.up, out climbHit, Mathf.Infinity, climbableLedge))
                                    {
                                        if (input.magnitude > 0 && cc.rpgaeIM.PlayerControls.Action.triggered)
                                        {
                                            inClimbEnterAir = true;
                                            climbData.position = climbHit.point;
                                            climbData.targetTransform = climbHit.transform;
                                            climbData.canClimb = true;
                                            EnterClimb();
                                        }
                                    }
                                }
                            }
                        }
                        else if (Physics.Raycast(wallSurfaceHit, transform.up, out climbHit, 5, climbableLedge) && !dived && isSwimming)
                        {
                            hudM.ShowInteractiveIcon("Move", "Climb Jump");

                            Debug.DrawRay(wallSurfaceHit, transform.up * 3f, Color.red);
                            climbData.targetTransform = climbHit.transform;
                            if (climbData.targetTransform.gameObject.name == "LongLedge" ||
                            climbData.targetTransform.gameObject.name == "LongLedgeTop")
                            {
                                if (input.magnitude > 0.1f && cc.rpgaeIM.PlayerControls.Action.triggered)
                                {
                                    inClimbEnterAir = true;
                                    climbData.position = climbHit.point;
                                    climbData.targetTransform = climbHit.transform;
                                    climbData.canClimb = true;
                                    EnterClimb();
                                }
                            }
                        }
                    }

                    #endregion
                }
            }
            else
            {
                climbData.canClimb = false;
            }

            // Exit Climb input
            if (conditionsToClimb && climbData.inPosition && cc.rpgaeIM.PlayerControls.Interact.triggered && Time.time > (oldInput + 0.5f))
                ExitClimb();

            ClimbMovement();
            ClimbUpToGround();
            LedgeJumpBackwards();
            LedgeJumpBackUpwards();
            GroundedClimbDownToHang();
            GroundedJumpAcrossToHang();
        }

        public void EnterClimb()
        {
            oldInput = Time.time;
            rigidBody.isKinematic = true;
            cc.triggerAction = null;

            if (jumpAS)
                jumpAS.PlayRandomClip();

            animator.SetBool("Grounded", true);
            if (!canClimbDown && !canJumpAcross)
                animator.CrossFadeInFixedTime(grounded ? "EnterClimbGrounded" : "EnterClimbAir", 0.2f);
            else if (grounded && climbState == ClimbState.GroundedClimbDownToHang)
                animator.CrossFadeInFixedTime("DropToHang", 0.2f);
            else if (grounded && climbState == ClimbState.GroundedJumpAcrossToHang)
                animator.CrossFadeInFixedTime("EnterClimbGrounded", 0.2f);

            if (!grounded)
                StartCoroutine(EnterClimbAlignment());
            else
                climbData.inPosition = true;

            isShortLedge = false;
        }

        public void EnterSmallLedgeClimb()
        {
            oldInput = Time.time;
            rigidBody.isKinematic = true;
            cc.triggerAction = null;

            if (jumpAS)
                jumpAS.PlayRandomClip();

            animator.SetBool("Grounded", true);
            if (!canClimbDown && !canJumpAcross)
                animator.CrossFadeInFixedTime(grounded ? "EnterClimbGrounded" : "EnterClimbAir", 0.2f);
            else if (grounded && climbState == ClimbState.GroundedClimbDownToHang)
                animator.CrossFadeInFixedTime("DropToHang", 0.2f);
            else if (grounded && climbState == ClimbState.GroundedJumpAcrossToHang)
                animator.CrossFadeInFixedTime("EnterClimbGrounded", 0.2f);

            if (!grounded)
                StartCoroutine(EnterClimbAlignment());
            else
                climbData.inPosition = true;

            isShortLedge = false;
        }

        protected virtual void ExitClimb()
        {
            oldInput = Time.time;

            climbData.inPosition = false;
            climbData.canClimb = false;

            canClimbDown = false;
            canJumpAcross = false;

            inClimbJump = false;
            rigidBody.useGravity = true;
            rigidBody.isKinematic = false;

            hudM.ShowInteractiveIcon("", "");

            if (!inClimbUp)
            {
               animator.CrossFadeInFixedTime("ExitAir", 0.2f);
            }
            else
            {
                verticalVelocity = 0;
                animator.SetFloat("GroundDistance", 0);
            }

            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            inClimbUp = false;

            climbData.position = Vector3.zero;
            climbData.targetTransform = null;
        }

        void ClimbMovement()
        {
            if (!climbData.inPosition || inClimbEnter) return;

            hudM.ShowInteractiveIcon("Climb Jump", "Let Go");

            if (!canCornerEdgeLerp)
            {
                //Vector3 jumpAimPosition = handTargetPosition + (transform.up * 0.3f) - (transform.forward * capCollider.radius);
                Debug.DrawRay(handTargetPosition + (transform.up * 0.3f) - (transform.forward * (capCollider.radius + 0.5f)), Vector3.forward, Color.red);


                canMoveClimb = CheckClimbableLedges();
                if (!rigidBody.useGravity)
                    canClimbJump = CheckLedgeJump();
            }
            else
                StartCoroutine(CornerEdgeLerp());

            climbData.canClimb = canMoveClimb;

            if (canMoveClimb)
            {
                animator.SetFloat("InputHorizontal", input.x, 0.2f, Time.deltaTime);
                animator.SetFloat("InputVertical", input.z, 0.2f, Time.deltaTime);

                if (input.x >= 0.8f && !cc.baseLayerInfo.IsName("LookBack"))
                {
                    float edgeSpeed = animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 < 0.5f ? 0.9f : 0.3f;
                    transform.Translate(1 * Time.deltaTime * edgeSpeed, 0, 0, climbData.targetTransform);
                }
                else if (input.x <= -0.8f &&  !cc.baseLayerInfo.IsName("LookBack"))
                {
                    float edgeSpeed = animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 < 0.5f ? 0.3f : 0.9f;
                    transform.Translate(-1 * Time.deltaTime * edgeSpeed, 0, 0, climbData.targetTransform);
                }
                ShimmyHandOffSet();
            }
            else
            {
                animator.SetFloat("InputHorizontal", 0, 0.2f, Time.deltaTime);
                animator.SetFloat("InputVertical", 0, 0.2f, Time.deltaTime);
            }
        }

        void ShimmyHandOffSet()
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("ClimbJump")) return;

            Debug.DrawRay(transform.position + (transform.up * 1.55f) - (transform.forward * 1), transform.forward * 5, Color.green);
            if (climbData.canClimb && climbData.inPosition && Physics.Raycast(transform.position + (transform.up * 1.55f) - (transform.forward * 1), transform.forward, out climbHit, 5, climbableSurface))
            {
                Vector3 wallSurfaceHit = climbHit.point;
                if (climbData.canClimb && Physics.Raycast(wallSurfaceHit, transform.up, out climbHit, 3, climbableLedge))
                {
                    climbData.targetTransform = climbHit.transform;
                    if (climbData.targetTransform.gameObject.name == "LongLedge" || climbData.targetTransform.gameObject.name == "LongLedgeTop")
                    {
                        climbData.position = climbHit.point;
                        // Braced
                        bracedTargetPosition = climbData.position - transform.rotation * handTarget.localPosition + ((transform.up * -0.20f) - (transform.forward * 0.08f));
                        // Hanged
                        hangingTargetPosition = climbData.position - transform.rotation * handTarget.localPosition + ((transform.up * -0.1f) - (transform.forward * -0.35f));
                        transform.position = Vector3.Lerp(transform.position, ledgeStance == 0 ? bracedTargetPosition : hangingTargetPosition, 8 * Time.deltaTime);
                    }
                }
            }
        }

        bool CheckClimbableLedges()
        {
            if (input.magnitude > 0.1f)
                climbLastInput = input;

            if (inClimbJump || inClimbUp) return false;

            float horizontalInput = climbLastInput.x > 0 ? 1 * lastPointDistanceH : climbLastInput.x < 0 ? -1 * lastPointDistanceH : 0;
            Vector3 centerCharacter = transform.position + (transform.up * 1.8f);
            Vector3 targetPosNormalized = centerCharacter + (transform.right * horizontalInput);

            // Directional linecast part 1
            Vector3 p1 = centerCharacter;
            Vector3 p2 = targetPosNormalized;
            Debug.DrawLine(p1, p2, Color.red);
            if (Physics.Linecast(p1, p2, out climbHit, climbableLedge))
            {
                if (climbData.targetTransform.gameObject.name == "LongLedge" ||
                    climbData.targetTransform.gameObject.name == "LongLedgeTop")
                {
                    climbData.targetNormal = climbHit.normal;
                    RotateTo(-climbData.targetNormal, climbData.position);
                }
            }

            // Forward linecast of direction part 1
            p1 = p2;
            p2 = p1 + transform.forward;
            Debug.DrawLine(p1, p2, Color.yellow);
            if (Physics.Linecast(p1, p2, out climbHit, climbableLedge))
            {
                if (climbData.targetTransform.gameObject.name == "LongLedge" ||
                    climbData.targetTransform.gameObject.name == "LongLedgeTop")
                {
                    climbData.targetNormal = climbHit.normal;
                    RotateTo(-climbData.targetNormal, climbData.position);
                }
            }

            float horizontalInput2 = climbLastInput.x > 0 ? 1 * lastPointDistanceH : climbLastInput.x < 0 ? -1 * lastPointDistanceH : 0;
            Vector3 centerCharacter2 = transform.position + (transform.up * 1.8f) - (transform.forward * capCollider.radius);
            Vector3 targetPosNormalized2 = centerCharacter2 + (transform.right * horizontalInput2);

            // Directional linecast part 2
            Vector3 p12 = centerCharacter2;
            Vector3 p22 = targetPosNormalized2;
            Debug.DrawLine(p12, p22, Color.green);
            if (Physics.Linecast(p12, p22, out climbHit, climbableLedge))
            {
                if (climbData.targetTransform.gameObject.name == "LongLedge" ||
                    climbData.targetTransform.gameObject.name == "LongLedgeTop")
                {
                    return true;
                }
            }

            // Forward linecast of direction part 2
            p12 = p22;
            p22 = p12 + transform.forward;
            Debug.DrawLine(p12, p22, Color.yellow);
            if (Physics.Linecast(p12, p22, out climbHit, climbableLedge))
            {
                if (climbData.targetTransform.gameObject.name == "LongLedge" ||
                    climbData.targetTransform.gameObject.name == "LongLedgeTop")
                {
                    return true;
                }
            }

            p1 += transform.forward * capCollider.radius * 2.65f;
            p2 += (transform.right * (capCollider.radius + lastPointDistanceH) * -input.x);
            Debug.DrawLine(p1, p2, Color.red);
            if (Physics.Linecast(p1, p2, out climbHit, climbableLedge))
            {
                climbData.targetNormal = climbHit.normal;

                Vector3 wallPos = climbHit.point;
                Debug.DrawLine(wallPos + (transform.up * -0.5f), climbHit.transform.forward, Color.blue);
                if (Physics.Raycast(wallPos + (transform.up * -0.5f), transform.up, out climbHit, 10, climbableLedge))
                {
                    if (climbData.targetTransform.gameObject.name == "LongLedge")
                    {
                        climbData.position = climbHit.point;
                        climbData.targetTransform = climbHit.transform;

                        canCornerEdgeLerp = true;
                        cornerEdgePoint = climbHit.point;
                        cornerEdgePointRot = Quaternion.LookRotation(-climbData.targetNormal);
                        return true;
                    }
                }
            }
            return false;
        }

        IEnumerator CornerEdgeLerp()
        {
            // Braced
            bracedTargetPosition = cornerEdgePoint - transform.rotation * handTarget.localPosition + ((transform.up * -0.20f) - (transform.forward * -0.08f * -1));
            // Hanged
            hangingTargetPosition = cornerEdgePoint - transform.rotation * handTarget.localPosition + ((transform.up * -0.1f) - (transform.forward * -0.35f * -1));

            transform.position = Vector3.Lerp(transform.position, ledgeStance == 0 ? bracedTargetPosition : hangingTargetPosition, Time.deltaTime * 0.7f * 11);
            transform.rotation = Quaternion.Lerp(transform.rotation, cornerEdgePointRot, Time.deltaTime * 4);

            yield return new WaitForSeconds(0.5f);

            canCornerEdgeLerp = false;
        }

        protected virtual void ClimbUpToGround()
        {
            if (inClimbJump || !animator || !climbData.inPosition) return;

            if (inClimbUp && !inAlingClimb)
            {
                if (cc.baseLayerInfo.IsName("ClimbUpWall"))
                {
                    if (cc.baseLayerInfo.normalizedTime >= 0.74f)
                    {
                        ExitClimb();
                    }
                }
                return;
            }

            if (climbData.canClimb && climbData.inPosition && Physics.Raycast(transform.position + (transform.up * 1.55f) - (transform.forward * 1), transform.forward, out climbHit, 5, climbableSurface))
            {
                Vector3 wallSurfaceHit = climbHit.point;
                if (climbData.canClimb && Physics.Raycast(wallSurfaceHit, transform.up, out climbHit, 3, climbableLedge))
                {
                    climbData.targetTransform = climbHit.transform;
                    if (climbData.targetTransform.gameObject.name == "LongLedgeTop")
                    {
                        if (!inClimbUp && !inClimbEnter && climbData.inPosition && (input.z > 0f)
                        && cc.rpgaeIM.PlayerControls.Action.triggered && Time.time > (oldInput + 0.5f))
                        {
                            ClimbUp();
                        }
                    }
                }
            }
        }

        void ClimbUp()
        {
            StartCoroutine(AlignClimb());
            inClimbUp = true;
        }

        IEnumerator AlignClimb()
        {
            inAlingClimb = true;
            animator.applyRootMotion = true;

            var transition = 0f;
            var dir = transform.forward;
            dir.y = 0;
            var angle = Vector3.Angle(Vector3.up, transform.forward);

            var targetRotation = Quaternion.LookRotation(-climbData.targetNormal);
            var targetPosition = ((climbData.position + dir * -capCollider.radius + Vector3.up * 0.1f) - transform.rotation * handTarget.localPosition);

            animator.SetFloat("InputVertical", 1f);
            while (transition < 1 && Vector3.Distance(targetRotation.eulerAngles, transform.rotation.eulerAngles) > 0.2f && angle < 60)
            {
                animator.SetFloat("InputVertical", 1f);
                transition += Time.deltaTime * 0.5f;
                targetPosition = ((climbData.position + dir * -capCollider.radius) - transform.rotation * handTarget.localPosition);
                transform.position = Vector3.Slerp(transform.position, targetPosition, transition);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, transition);
                yield return null;
            }
            animator.CrossFadeInFixedTime("ClimbUpWall", 0.1f);
            inAlingClimb = false;
        }

        bool CheckLedgeJump()
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("EnterClimbGrounded") || grounded || !climbData.inPosition || 
                inClimbJump || inClimbEnter || canSwim || isSwimming)
                return false;

            if (input.magnitude > 0 && cc.rpgaeIM.PlayerControls.Action.triggered && !animator.GetCurrentAnimatorStateInfo(0).IsName("ClimbJump"))
            {
                float x = cc.rpgaeIM.PlayerControls.Movement.ReadValue<Vector2>().x * 2 * Time.deltaTime;
                float y = cc.rpgaeIM.PlayerControls.Movement.ReadValue<Vector2>().y * 2 * Time.deltaTime;

                Vector3 inputDirection = (transform.right * x) + (transform.up * y);
                inputDirection.Normalize();

                #region Side Ledge Jump

                Vector3 jumpAimPosition = handTargetPosition + (transform.up * -0.3f) - (transform.forward * capCollider.radius);
                Vector3 targetPositionSideJump = jumpAimPosition + (inputDirection * 3.3f);

                Vector3 p1SideJump = jumpAimPosition;
                Vector3 p2SideJump = targetPositionSideJump;
                Debug.DrawLine(p1SideJump, p2SideJump, Color.green);
                if (Physics.Linecast(p1SideJump, p2SideJump, out climbHit, climbableSurface) && ledgeStance == 0)
                {
                    Vector3 hitSideSurfaceWall = climbHit.point;
                    Debug.DrawRay(hitSideSurfaceWall, transform.up * 3, Color.red);
                    if (Physics.Raycast(hitSideSurfaceWall, transform.up, out climbHit, 4, climbableLedge))
                    {
                        jumpClimbData.targetTransform = climbHit.transform;

                        if (jumpClimbData.targetTransform != null && 
                        jumpClimbData.targetTransform.gameObject.name == "LongLedge" ||
                        jumpClimbData.targetTransform.gameObject.name == "LongLedgeTop")
                        {
                            jumpClimbData.position = climbHit.point;
                            jumpClimbData.targetTransform = climbHit.transform;
                            ClimbJump();
                            return true;
                        }
                    }
                    else if (Physics.Raycast(hitSideSurfaceWall, -transform.up, out climbHit, 4, climbableLedge))
                    {
                        Vector3 ledgeFromTopHit = climbHit.point;
                        Debug.DrawRay(ledgeFromTopHit, -transform.forward * 0.5f, Color.blue);
                        Debug.DrawRay(ledgeFromTopHit + (-transform.forward * 0.5f), -transform.up * 0.5f, Color.blue);
                        Debug.DrawRay(ledgeFromTopHit + ((-transform.forward * 0.5f) - (transform.up * 0.5f)), transform.forward * 10, Color.blue);
                        if (Physics.Raycast(ledgeFromTopHit + ((-transform.forward * 0.5f) - (transform.up * 0.5f)), transform.forward, out climbHit, 10, climbableSurface))
                        {
                            Vector3 wallSurface2Hit = climbHit.point;
                            if (Physics.Raycast(wallSurface2Hit, transform.up, out climbHit, Mathf.Infinity, climbableLedge))
                            {
                                jumpClimbData.targetTransform = climbHit.transform;

                                if (jumpClimbData.targetTransform != null &&
                                jumpClimbData.targetTransform.gameObject.name == "LongLedge" ||
                                jumpClimbData.targetTransform.gameObject.name == "LongLedgeTop")
                                {
                                    jumpClimbData.position = climbHit.point;
                                    jumpClimbData.targetTransform = climbHit.transform;
                                    ClimbJump();
                                    return true;
                                }
                            }
                        }
                    }
                }

                #endregion

                #region Directional Surface Wall Jump

                if (Physics.Raycast(handTargetPosition + (transform.up * 0.3f), transform.forward, out climbHit, 1, climbableSurface))
                {
                    Vector3 surfaceWall = climbHit.point;
                    Vector3 targetPositionWallSufrace = surfaceWall + (inputDirection * 3.3f);

                    Vector3 p1WallSurface = surfaceWall;
                    Vector3 p2WallSurface = targetPositionWallSufrace;
                    Debug.DrawLine(p1WallSurface, p2WallSurface, Color.green);
                    if (Physics.Linecast(p1WallSurface, p2WallSurface, out climbHit, climbableLedge))
                    {
                        if (input.z > 0)
                        {
                            jumpClimbData.targetTransform = climbHit.transform;
                            if (jumpClimbData.targetTransform.name == "LongLedge" ||
                                jumpClimbData.targetTransform.name == "LongLedgeTop")
                            {
                                jumpClimbData.position = climbHit.point;
                                jumpClimbData.targetTransform = climbHit.transform;
                                ClimbJump();
                                return true;
                            }
                        }
                        else if (input.z < 0)
                        {
                            Vector3 ledgeFromTopHit = climbHit.point;
                            Debug.DrawRay(ledgeFromTopHit, -transform.forward * 0.5f, Color.blue);
                            Debug.DrawRay(ledgeFromTopHit + (-transform.forward * 0.5f), -transform.up * 0.5f, Color.blue);
                            Debug.DrawRay(ledgeFromTopHit + ((-transform.forward * 0.5f) - (transform.up * 0.5f)), transform.forward * 10, Color.blue);
                            if (Physics.Raycast(ledgeFromTopHit + ((-transform.forward * 0.5f) - (transform.up * 0.5f)), transform.forward, out climbHit, 10, climbableSurface))
                            {
                                Vector3 wallSurface2Hit = climbHit.point;
                                if (Physics.Raycast(wallSurface2Hit, transform.up, out climbHit, Mathf.Infinity, climbableLedge))
                                {
                                    jumpClimbData.targetTransform = climbHit.transform;
                                    if (jumpClimbData.targetTransform.name == "LongLedge" ||
                                    jumpClimbData.targetTransform.name == "LongLedgeTop")
                                    {
                                        jumpClimbData.position = climbHit.point;
                                        jumpClimbData.targetTransform = climbHit.transform;
                                        ClimbJump();
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }

                #endregion

                #region Small Ledge Jump

                float horizontalInput = climbLastInput.x > 0 ? 1 * 1.25f : climbLastInput.x < 0 ? -1 * 1.25f : 0;
                float verticalInput = climbLastInput.z > 0 ? 1 * 1.25f : climbLastInput.z < 0 ? -1 * 1.25f : 0;
                Vector3 centerCharacter = transform.position + (transform.up * 1.5f);
                Vector3 targetPosNormalized = centerCharacter + (transform.right * horizontalInput) + (transform.up * verticalInput);

                Vector3 p1 = centerCharacter;
                Vector3 p2 = targetPosNormalized;
                p1 = p2;
                p2 = p1 + transform.forward;
                Debug.DrawLine(p1, p2, Color.green);
                if (Physics.Linecast(p1, p2, out climbHit, climbableSurface))
                {
                    Debug.DrawRay(climbHit.point, -transform.forward * 10, Color.red);

                    fwdRayCastHitForSmallLedge = climbHit.point;
                    FindClosestSmallClimbSpot();

                    if (jumpClimbData.targetTransform && jumpClimbData.targetTransform.gameObject.name == "SmallLedge"
                    && jumpClimbData.targetTransform.gameObject.layer == 7)
                    {
                        ClimbJump();
                        return true;
                    }
                }

                #endregion

                #region Jump Across From Side

                Vector3 centerCharacter1 = handTargetPosition + (transform.up * -0.3f) - (transform.forward * capCollider.radius);
                Vector3 inputDirection2 = (transform.right * x) + (transform.up * y);
                inputDirection2.Normalize();

                bool ledgeSideJumpAcross = input.x < 0 || input.x > 0 && input.z == 0;
                Vector3 targetPosNormalized2 = ledgeSideJumpAcross ? centerCharacter1 + (inputDirection2 * 4) : centerCharacter1 + (inputDirection2 * 3);

                centerCharacter1 = targetPosNormalized2;
                targetPosNormalized2 = centerCharacter1 + transform.forward;
                Debug.DrawLine(centerCharacter1, targetPosNormalized2, Color.yellow);
                if (Physics.Linecast(centerCharacter1, targetPosNormalized2, out climbHit, climbableSurface) && input.x < 0 || input.x > 0 && input.z == 0 && ledgeStance == 0)
                {
                    fwdRayCastHitForSmallLedge = climbHit.point;
                    Debug.DrawRay(fwdRayCastHitForSmallLedge, transform.up * 10, Color.red);
                    if (Physics.Raycast(fwdRayCastHitForSmallLedge, transform.up, out climbHit, 4, climbableLedge))
                    {
                        if (jumpClimbData.targetTransform.name == "LongLedge" ||
                        jumpClimbData.targetTransform.name == "LongLedgeTop")
                        {
                            jumpClimbData.position = climbHit.point;
                            jumpClimbData.targetTransform = climbHit.transform;
                            ClimbJump();
                            return true;
                        }
                    }
                }

                #endregion

            }
            return false;
        }

        protected virtual void ClimbJump()
        {
            buttonTimer = 0;
            inClimbJump = true;
            canJumpBackOnLedge = false;
            animator.applyRootMotion = false;
            animator.SetFloat("InputHorizontal", cc.rpgaeIM.PlayerControls.Movement.ReadValue<Vector2>().x);
            animator.SetFloat("InputVertical", cc.rpgaeIM.PlayerControls.Movement.ReadValue<Vector2>().y);
            animator.CrossFadeInFixedTime("ClimbJump", 0.2f);

            if (jumpAS)
                jumpAS.PlayRandomClip();

            climbData.position = jumpClimbData.position;
            climbData.targetTransform = jumpClimbData.targetTransform;
        }


        void FindClosestSmallClimbSpot()
        {
            GameObject[] gos;
            gos = GameObject.FindGameObjectsWithTag("Ledge");
            GameObject closest = null;

            float distance = 5.5f;

            if(inClimbEnterAir)
                distance = 1.5f;
            else
                distance = 5.5f;

            foreach (GameObject go in gos)
            {
                Vector3 diff = go.transform.position - fwdRayCastHitForSmallLedge;
                float curDistance = diff.sqrMagnitude;

                if (curDistance < distance)
                {
                    closest = go;
                    distance = curDistance;

                    if (closest)
                        climbData.canClimb = true;

                    if (closest.name == "SmallLedge")
                    {
                        jumpClimbData.targetTransform = closest.transform;
                        jumpClimbData.position = closest.transform.position + ((transform.up * -0.1f) - (transform.forward * -0.09f));

                        climbData.targetTransform = jumpClimbData.targetTransform;
                        climbData.position = jumpClimbData.position;
                        m_closestSmallLedge = closest;
                    }
                }
            }
        }

        IEnumerator EnterClimbAlignment()
        {
            inClimbEnterAir = true;
            climbData.inPosition = true;
            var _position = transform.position;
            var _rotation = transform.rotation;

            var _targetRotation = Quaternion.LookRotation(-climbData.targetNormal);
            bracedTargetPosition = climbData.position - transform.rotation * handTarget.localPosition + ((transform.up * -0.20f) - (transform.forward * 0.08f));
            hangingTargetPosition = climbData.position - transform.rotation * handTarget.localPosition + ((transform.up * -0.1f) - (transform.forward * -0.35f));

            var _transition = 0f;
            Debug.DrawLine(handTargetPosition, climbData.position, Color.red, 60);
            Debug.DrawLine(transform.position, climbData.position, Color.red, 60);
            while (_transition < 1)
            {
                _transition += Time.deltaTime *5;
                transform.rotation = Quaternion.Lerp(_rotation, _targetRotation, _transition);
                transform.position = Vector3.Lerp(transform.position, ledgeStance == 0 ? bracedTargetPosition : hangingTargetPosition, _transition);
                yield return null;
            }
            rigidBody.useGravity = false;
            animator.applyRootMotion = true;
            transform.position = ledgeStance == 0 ? bracedTargetPosition : hangingTargetPosition;
            inClimbEnterAir = false;
        }

        void RotateTo(Vector3 direction, Vector3 point)
        {
            if (input.magnitude < 0.1f) return;
            Vector3 referenceDirection = point - (climbData.position);
            Debug.DrawLine(point, (climbData.position), Color.blue, .1f);
            Vector3 resultDirection = Quaternion.AngleAxis(-90, transform.right) * referenceDirection;
            float eulerX = Quaternion.LookRotation(resultDirection).eulerAngles.x;
            Quaternion baseRotation = Quaternion.LookRotation(direction);
            Quaternion resultRotation = Quaternion.Euler(eulerX, baseRotation.eulerAngles.y, transform.eulerAngles.z);
            transform.rotation = Quaternion.Lerp(transform.rotation, resultRotation, (animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1) * 0.2f);
            //transform.rotation = resultRotation;
        }

        void LedgeJumpBackwards()
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("EnterClimbGrounded")
            || grounded || !climbData.inPosition || inClimbJump || inClimbEnter || ledgeStance == 1)
                return;

            Debug.DrawRay(transform.position + ((transform.up * 0.1f) - (transform.forward * 0.5f)), -transform.forward * 3, Color.blue);

            if (Physics.Raycast(transform.position + ((transform.up * 0.1f) - (transform.forward * 0.5f)), -transform.forward, out climbHit, 3f, climbableSurface))
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("LookBack"))
                    return;

                if (input.z < -0.1f && cc.rpgaeIM.PlayerControls.Action.triggered && !animator.GetCurrentAnimatorStateInfo(0).IsName("ClimbJump"))
                {
                    fwdRayCastHitForSmallLedge = climbHit.point;

                    Vector3 backSurfaceWallHit = climbHit.point;
                    Debug.DrawRay(backSurfaceWallHit, transform.up * 0.5f, Color.red);
                    Debug.DrawRay(backSurfaceWallHit, -transform.up * 0.5f, Color.red);
                    if (Physics.Raycast(backSurfaceWallHit, transform.up, out climbHit, 3, climbableLedge))
                    {
                        if (climbHit.transform.name == "LongLedge" || climbHit.transform.name == "LongLedgeTop")
                        {
                            jumpClimbData.position = climbHit.point;
                            jumpClimbData.targetTransform = climbHit.transform;
                            ClimbJump();
                        }
                    }
                    else if (Physics.Raycast(backSurfaceWallHit, -transform.up, out climbHit, 3, climbableLedge))
                    {
                        Vector3 ledgeFromTopHit = climbHit.point;
                        Debug.DrawRay(ledgeFromTopHit + (transform.forward * 0.5f), -transform.forward * 0.5f, Color.red);
                        Debug.DrawRay(ledgeFromTopHit + (transform.forward * 0.5f), -transform.up * 0.5f, Color.red);
                        Debug.DrawRay(ledgeFromTopHit + ((transform.forward * 0.5f) - (transform.up * 0.5f)), -transform.forward * 10, Color.red);
                        if (Physics.Raycast(ledgeFromTopHit + ((transform.forward * 0.5f) - (transform.up * 0.5f)), -transform.forward, out climbHit, 10, climbableSurface))
                        {
                            Vector3 backSurfaceWall2Hit = climbHit.point;
                            if (Physics.Raycast(backSurfaceWall2Hit, transform.up, out climbHit, Mathf.Infinity, climbableLedge))
                            {
                                if (climbHit.transform.name == "LongLedge" || climbHit.transform.name == "LongLedgeTop")
                                {
                                    jumpClimbData.position = climbHit.point;
                                    jumpClimbData.targetTransform = climbHit.transform;
                                    ClimbJump();
                                }
                            }
                        }
                    }
                }
            }
        }

        void LedgeJumpBackUpwards()
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("ClimbJump")) return;

            if (Physics.Raycast(animator.GetBoneTransform(HumanBodyBones.LeftFoot).transform.position + ((transform.right * -0.2f) - (-transform.forward * 0.5f)), transform.forward, out climbHit, 1, climbableSurface) &&
            Physics.Raycast(animator.GetBoneTransform(HumanBodyBones.RightFoot).transform.position + ((transform.right * 0.2f) - (-transform.forward * 0.5f)), transform.forward, out climbHit, 1, climbableSurface) 
            && climbData.inPosition)
            {
                if (!canCornerEdgeLerp)
                    ledgeStance = 1;
            }
            else
            {
                if (!canCornerEdgeLerp)
                    ledgeStance = 0;
            }
            animator.SetFloat("LedgeStance", ledgeStance, 0.1f, Time.deltaTime);

            Debug.DrawRay(transform.position + (transform.up * 0.5f), transform.up * 4, Color.red);
            Debug.DrawRay(handTargetPosition + (transform.up * 0.3f), transform.forward * 1, Color.red);
            if (Physics.Raycast(handTargetPosition + (transform.up * 0.3f), transform.forward, out climbHit, 1, climbableSurface))
            {
                Vector3 wallSurfaceHit = climbHit.point;

                // check if a ledge is above if so you cant jump 
                Debug.DrawRay(wallSurfaceHit, transform.up * 1, Color.red);
                if (!Physics.Raycast(wallSurfaceHit, transform.up, out climbHit, 1, climbableLedge))
                {
                    if (Physics.Raycast(transform.position + (transform.up * 0.5f), transform.up, out climbHit, 4, climbableSurface) && climbData.inPosition)
                    {
                        Vector3 ceilingHit = climbHit.point;
                        // Cast back from ceiling hit
                        Debug.DrawRay(ceilingHit, -transform.forward * 2, Color.red);
                        // Cast up
                        Debug.DrawRay(ceilingHit + (-transform.forward * 2), transform.up * 0.1f, Color.red);
                        // Cast forward
                        Debug.DrawRay(ceilingHit + (-transform.forward * 2) - (-transform.up * 0.1f), transform.forward * 3, Color.red);
                        if (Physics.Raycast(ceilingHit + (-transform.forward * 2) - (-transform.up * 0.1f), transform.forward, out climbHit, 3, climbableSurface))
                        {
                            Vector3 wallSurfaceHit2 = climbHit.point;
                            // Cast Up to ledge
                            Debug.DrawRay(wallSurfaceHit2, transform.up * 2, Color.red);
                            if (input.z > 0.1f && cc.rpgaeIM.PlayerControls.Action.triggered && !animator.GetCurrentAnimatorStateInfo(0).IsName("ClimbJump"))
                            {
                                if (Physics.Raycast(wallSurfaceHit2, transform.up, out climbHit, 2, climbableLedge))
                                {
                                    if (climbHit.transform.name == "LongLedge" || climbHit.transform.name == "LongLedgeTop")
                                    {
                                        jumpClimbData.position = climbHit.point;
                                        jumpClimbData.targetTransform = climbHit.transform;
                                        ClimbJump();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        void GroundedClimbDownToHang()
        {
            if (hudM.interactiveOverlayIcon1.buttonActionName == "Climb Down" && !canClimbDown && !climbData.inPosition 
                || hudM.interactiveOverlayIcon1.buttonActionName == "Climb Down" && isJumping)
                hudM.ShowInteractiveIcon("", "");

            if (climbState == ClimbState.GroundedJumpAcrossToHang)
                return;

            if (!Physics.Raycast(transform.position + (transform.up * -1), transform.forward, out climbHit, 1) && grounded)
            {
                Debug.DrawRay(transform.position + (transform.up * 1), transform.forward * 1, Color.red);
                if (!Physics.Raycast(transform.position + ((transform.forward * 1) - (transform.up * -1)), -transform.up, out climbHit, 2))
                {
                    Debug.DrawRay(transform.position + ((transform.forward * 1) - (transform.up * -1)), transform.up * -2, Color.red);
                    if (Physics.Raycast(transform.position + ((transform.forward * 1) - (transform.up * 1)), -transform.forward, out climbHit, 2, climbableSurface))
                    {
                        Vector3 wallSurfaceHit = climbHit.point;
                        Debug.DrawRay(transform.position + ((transform.forward * 1) - (transform.up * 1)), transform.forward * -2, Color.red);
                        if (Physics.Raycast(wallSurfaceHit, transform.up, out climbHit, 3, climbableLedge))
                        {
                            canClimbDown = true;
                            if (!canJumpAcross)
                                hudM.ShowInteractiveIcon("Climb Down", "");
                            if (cc.rpgaeIM.PlayerControls.Interact.triggered)
                            {
                                climbState = ClimbState.GroundedClimbDownToHang;
                                climbData.targetTransform = climbHit.transform;
                                climbData.targetNormal = climbHit.normal;
                                climbData.position = climbHit.point;
                                climbData.canClimb = true;
                                EnterClimb();
                            }
                        }
                        else
                            canClimbDown = false;
                    }
                }
                else
                    canClimbDown = false;
            }
        }

        void GroundedJumpAcrossToHang()
        {
            if (climbState == ClimbState.GroundedClimbDownToHang)
                return;

            if (Physics.Raycast(transform.position + (transform.up * 1), transform.forward, out climbHit, Mathf.Infinity, climbableSurface) && grounded)
            {
                Vector3 wallSurfaceHit = climbHit.point;

                if (grounded)
                    fwdRayCastHitForSmallLedge = climbHit.point;

                if (climbHit.distance <= 5)
                {
                    Debug.DrawRay(transform.position + (transform.up * 1), transform.forward * 10, Color.blue);
                    if (Physics.Raycast(transform.position + ((transform.forward * 0.8f) - (transform.up * -1)), -transform.up, out climbHit, Mathf.Infinity))
                    {
                        if (climbHit.distance > 2.5f)
                        {
                            Debug.DrawRay(transform.position + ((transform.forward * 0.8f) - (transform.up * -1)), -transform.up * 10, Color.blue);

                            // Upwards from wall
                            if (Physics.Raycast(wallSurfaceHit, transform.up, out climbHit, 3, climbableLedge))
                            {
                                if (climbHit.transform.name == "LongLedge" ||
                                climbHit.transform.name == "LongLedgeTop")
                                {
                                    canJumpAcross = true;
                                    hudM.ShowInteractiveIcon("Move", "Climb Jump");
                                    Debug.DrawRay(wallSurfaceHit, transform.up * 3, Color.blue);
                                    if (cc.rpgaeIM.PlayerControls.Action.triggered)
                                    {
                                        climbState = ClimbState.GroundedJumpAcrossToHang;
                                        climbData.targetTransform = climbHit.transform;
                                        climbData.position = climbHit.point;
                                        climbData.canClimb = true;
                                        EnterClimb();
                                    }
                                }
                            }

                            // Downwards from wall 
                            else if (Physics.Raycast(wallSurfaceHit, -transform.up, out climbHit, 3, climbableLedge))
                            {
                                Vector3 ledgeFromTopHit = climbHit.point;
                                Debug.DrawRay(ledgeFromTopHit, -transform.forward * 0.5f, Color.blue);
                                Debug.DrawRay(ledgeFromTopHit + (-transform.forward * 0.5f), -transform.up * 0.5f, Color.blue);
                                Debug.DrawRay(ledgeFromTopHit + ((-transform.forward * 0.5f) - (transform.up * 0.5f)), transform.forward * 10, Color.blue);
                                if (Physics.Raycast(ledgeFromTopHit + ((-transform.forward * 0.5f) - (transform.up * 0.5f)), transform.forward, out climbHit, 10, climbableSurface))
                                {
                                    Vector3 wallSurface2Hit = climbHit.point;
                                    if (Physics.Raycast(wallSurface2Hit, transform.up, out climbHit, Mathf.Infinity, climbableLedge))
                                    {

                                        if (climbHit.transform.name == "LongLedge" ||
                                        climbHit.transform.name == "LongLedgeTop")
                                        {
                                            canJumpAcross = true;
                                            hudM.ShowInteractiveIcon("Move", "Climb Jump");
                                            Debug.DrawRay(wallSurfaceHit, transform.up * 3, Color.blue);
                                            if (cc.rpgaeIM.PlayerControls.Action.triggered)
                                            {
                                                climbState = ClimbState.GroundedJumpAcrossToHang;
                                                climbData.targetTransform = climbHit.transform;
                                                climbData.position = climbHit.point;
                                                climbData.canClimb = true;
                                                EnterClimb();
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                FindClosestSmallClimbSpot();

                                if (cc.rpgaeIM.PlayerControls.Action.triggered)
                                {
                                    climbState = ClimbState.GroundedJumpAcrossToHang;
                                    canJumpAcross = true;
                                    if (climbData.targetTransform != null && climbData.targetTransform.name == "SmallLedge")
                                        EnterClimb();
                                }

                                if (m_closestSmallLedge != null)
                                {
                                    if (Vector3.Distance(fwdRayCastHitForSmallLedge, m_closestSmallLedge.transform.position) < 4)
                                    {
                                        canJumpAcross = true;
                                        hudM.ShowInteractiveIcon("Move", "Climb Jump");
                                    }
                                    else
                                    {
                                        canJumpAcross = false;
                                        if (hudM.interactiveOverlayIcon1.buttonActionName == "Move" && !climbData.inPosition)
                                            hudM.ShowInteractiveIcon("", "");
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    canJumpAcross = false;
                    if (hudM.interactiveOverlayIcon1.buttonActionName == "Move" && !climbData.inPosition)
                        hudM.ShowInteractiveIcon("", "");
                }
            }
            else
            {
                if (hudM.interactiveOverlayIcon1.buttonActionName == "Move" && !climbData.inPosition)
                    hudM.ShowInteractiveIcon("", "");
            }
        }

        public void CheckForJump()
        {
            if (GroundAngle() > 10f)
            {
                canMove = true;
                canFreeJump = false;
                return;
            }

            if (canSwim || isSwimming)
            {
                canMove = true;
                canFreeJump = false;
                return;
            }

            if (!inventoryM.ConditionsToOpenMenu())
            {
                canMove = false;
                attackPower = 0;
                return;
            }

            if (!grounded || isJumping || !inventoryM.ConditionsToOpenMenu() || cc.IsAnimatorTag("Action"))
            {
                attackPower = 0;
                return;
            }

            if (systemM.blackScreenFUI.canvasGroup.alpha != 0 || systemM.loadingScreenFUI.canvasGroup.alpha != 0)
                return;

            RaycastHit topHit;
            bool checkTopDistance = Physics.Raycast(transform.position + ((transform.forward * 0.8f) - (transform.up * -1)), transform.up, out topHit, 100, groundLayer);

            RaycastHit jumpMinHeightHit;
            bool checkJumpMinHeight = Physics.Raycast(transform.position + ((transform.forward * 0.8f) - (transform.up * -1)), -transform.up, out jumpMinHeightHit, 100, groundLayer);

            RaycastHit jumpMaxHeightRightHit;
            bool checkJumpMaxHeightRight = Physics.Raycast(transform.position + ((transform.forward * 0.8f) - (transform.up * -0.5f) - (transform.right * -0.45f)), -transform.up, out jumpMaxHeightRightHit, 100, groundLayer);

            RaycastHit jumpHeightMaxCenterHit;
            bool checkJumpMaxHeightCenter = Physics.Raycast(transform.position + ((transform.forward * 0.8f) - (transform.up * -0.5f)), -transform.up, out jumpHeightMaxCenterHit, 100, groundLayer);

            RaycastHit jumpMaxHeightLeftHit;
            bool checkJumpMaxHeightLeft = Physics.Raycast(transform.position + ((transform.forward * 0.8f) - (transform.up * -0.5f) - (transform.right * 0.45f)), -transform.up, out jumpMaxHeightLeftHit, 100, groundLayer);

            RaycastHit obstacleHit;
            bool checkForObstacleAhead = Physics.Raycast(transform.position + (transform.up * 0.1f), transform.forward, out obstacleHit, 1);

            #region Edge Stop

            if (checkJumpMaxHeightRight)
            {
                Debug.DrawRay(transform.position + ((transform.forward * 0.8f) - (transform.up * -0.5f) - (transform.right * -0.45f)), -transform.up * 10, Color.red);
                if (!isJumping && jumpMaxHeightRightHit.distance > 2.5f)
                {
                    if (groundDistance < 0.2f)
                        canMove = false;

                    canFreeJump = true;
                    if (targetDirection != Vector3.zero)
                    {
                        targetRotation = Quaternion.LookRotation(targetDirection);
                        Quaternion newRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                        transform.rotation = newRotation;
                    }
                }
                else
                {
                    if (isJumping || jumpMaxHeightLeftHit.distance < 2.5f && jumpHeightMaxCenterHit.distance < 2.5f && jumpMaxHeightRightHit.distance < 2.5f)
                    {
                        canMove = true;
                        canFreeJump = false;
                    }
                }
            }

            if (checkJumpMaxHeightCenter)
            {
                Debug.DrawRay(transform.position + ((transform.forward * 0.8f) - (transform.up * -0.5f)), -transform.up * 10, Color.red);
                if (!isJumping && jumpHeightMaxCenterHit.distance > 2.5f)
                {
                    if (groundDistance < 0.2f)
                        canMove = false;

                    canFreeJump = true;
                    if (targetDirection != Vector3.zero)
                    {
                        targetRotation = Quaternion.LookRotation(targetDirection);
                        Quaternion newRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                        transform.rotation = newRotation;
                    }
                }
                else
                {
                    if (isJumping || jumpMaxHeightLeftHit.distance < 2.5f && jumpHeightMaxCenterHit.distance < 2.5f && jumpMaxHeightRightHit.distance < 2.5f)
                    {
                        canMove = true;
                        canFreeJump = false;
                    }
                }
            }

            if (checkJumpMaxHeightLeft)
            {
                Debug.DrawRay(transform.position + ((transform.forward * 0.8f) - (transform.up * -0.5f) - (transform.right * 0.45f)), -transform.up * 10, Color.red);
                if (!isJumping && jumpMaxHeightLeftHit.distance > 2.5f)
                {
                    if (groundDistance < 0.2f)
                        canMove = false;

                    canFreeJump = true;
                    if (targetDirection != Vector3.zero)
                    {
                        targetRotation = Quaternion.LookRotation(targetDirection);
                        Quaternion newRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                        transform.rotation = newRotation;
                    }
                }
                else
                {
                    if (isJumping || jumpMaxHeightLeftHit.distance < 2.5f && jumpHeightMaxCenterHit.distance < 2.5f && jumpMaxHeightRightHit.distance < 2.5f)
                    {
                        canMove = true;
                        canFreeJump = false;
                    }
                }
            }

            #endregion

            #region Check For Obstacles

            if (checkJumpMinHeight)
            {
                if (jumpMinHeightHit.distance < 2.5f)
                {
                    // Cant Jump Floor is to close
                    return;
                }
            }

            if (checkTopDistance)
            {
                if (topHit.distance < 1.5f)
                {
                    // Cant Jump something is above you
                    return;
                }
            }

            if (canJumpAcross)
            {
                // Can't jump there is climbable ledge infront of you
                return;
            }

            if (checkForObstacleAhead)
            {
                return;
            }

            #endregion

            bool conditionsToJump = !cc.baseLayerInfo.IsTag("Jump") && !cc.animator.IsInTransition(0);
            if(!isJumping && input.magnitude > 0 && cc.rpgaeIM.PlayerControls.Action.triggered && conditionsToJump)
            {
                TriggerJumpAnimation();
            }
        }

        void TriggerJumpAnimation()
        {
            isJumping = true;
            animator.applyRootMotion = false;

            if (jumpAS)
                jumpAS.PlayRandomClip();

            bool runningConditions = input.magnitude > 0.75f;
            if (!runningConditions)
            {
                if (footStepCycle == -1)
                    cc.animator.CrossFadeInFixedTime("JumpWalkStart_LU", 0.2f);
                if (footStepCycle == 1)
                    cc.animator.CrossFadeInFixedTime("JumpWalkStart_RU", 0.2f);
            }
            else
            {
                if (footStepCycle == -1)
                    cc.animator.CrossFadeInFixedTime("JumpRunStart_LU", 0.2f);
                if (footStepCycle == 1)
                    cc.animator.CrossFadeInFixedTime("JumpRunStart_RU", 0.2f);
            }
        }

        #endregion

        #region Swim Behavior

        void SwimBehavior()
        {
            animator.SetBool("Underwater", dived);
            animator.SetFloat("WaterLevel", waterHeightLevel);
            if (cc.baseLayerInfo.IsName("DiveUp"))
            {
                if (waterbubblesPFX)
                    waterbubbles.SetActive(false);
                if (cc.baseLayerInfo.normalizedTime > 0.8f)
                    dived = false;
            }

            CheckWaterHeightLevel();

            if (!isSwimming || climbData.inPosition)
                return;

            animator.SetFloat("InputMagnitude", input.magnitude, 0.2f, Time.deltaTime);

            rigidBody.linearDamping = waterDrag;
            animator.applyRootMotion = true;

            Vector3 positionInWaterSurface = transform.position;
            positionInWaterSurface.y = distY - (colliderHeight * 0.5f + 2.25f);

            // Lerp to water surface level
            if (!dived || cc.baseLayerInfo.IsName("DiveUp"))
            {
                if (underwaterAS.isPlaying)
                {
                    underwaterAS.Stop();
                }

                transform.position = Vector3.Lerp(transform.position, positionInWaterSurface, 2 * Time.deltaTime);
                if (waterHeightLevel >= offsetToSurf)
                    WaterRingEffect();
            }

            // When you reach the surface while underwater
            if (dived)
            {
                if(tpCam.transform.position.y < distY)
                {
                    if (!underwaterAS.isPlaying)
                        underwaterAS.Play();
                }
                else
                {
                    if (underwaterAS.isPlaying)
                        underwaterAS.Stop();
                }

                canSwimOnSurfaceTimer += Time.deltaTime;
                if (waterHeightLevel <= offsetToSurf)
                {
                    if (canSwimOnSurfaceTimer > 0.1f)
                    {
                        dived = false;
                    }
                    canSwimOnSurfaceTimer = 0;
                }
            }

            // Swimming Input
            Vector3 forward = tpCam.transform.TransformDirection(Vector3.forward);
            forward.y = 0;
            Vector3 right = new Vector3(forward.z, 0, -forward.x);
            targetDirection = forward * input.z + right * input.x;
            targetDirection.Normalize();

            // Swimming Rotation
            if (targetDirection != Vector3.zero)
            {
                targetRotation = Quaternion.LookRotation(targetDirection);
                Quaternion newRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                transform.rotation = newRotation;
            }

            // Dive velocity
            if (dived && !cc.IsAnimatorTag("Action") && input.magnitude > 0.1f)
            {
                float diveAngle = Mathf.Round(Vector3.Angle(tpCam.transform.forward, transform.up));
                float speed = diveAngle > 150 ? diveSpeed : diveSpeed;

                // Dive if camera to player angle is bigger as 
                if (diveAngle > 140)
                {
                    diveVec = -liftVector * waterDrag * 2;
                }
                else if (diveAngle <= 40)
                {
                    diveVec = liftVector * waterDrag * 2;
                }

                // Apply dive force
                rigidBody.AddForce(diveVec * speed, ForceMode.Acceleration);
            }


            // Damp velocity "water effect"
            if (rigidBody.linearVelocity.sqrMagnitude > 6f)
            {
               rigidBody.linearVelocity = Vector3.Lerp(rigidBody.linearVelocity, rigidBody.linearVelocity.normalized,  12 * Time.deltaTime);
            }
        }

        public void DiveRotation()
        {
            if (!isSwimming || climbData.inPosition)
                return;

            if (cc.rpgaeIM.PlayerControls.Interact.triggered && dived && !cc.IsAnimatorTag("Action"))
            {
                oldInput = Time.time;
                cc.animator.CrossFadeInFixedTime("DiveUp", 0.2f);
            }

            if (cc.rpgaeIM.PlayerControls.Interact.triggered && !dived && !cc.IsAnimatorTag("Action"))
            {
                if (waterbubblesPFX)
                    waterbubbles.SetActive(true);

                cc.animator.CrossFadeInFixedTime("DiveDown", 0.2f);
                rigidBody.AddForce(-liftVector * 4, ForceMode.VelocityChange);
                dived = true;
            }
            
            float targetAngle = Mathf.Abs(Vector3.Angle(tpCam.transform.forward, transform.up));

            // stay upwards till targetAngle is in scope
            if (targetAngle < 150.0f)
            {
                targetAngle -= 90f;
            }
            else if (targetAngle >= 150.0f)
            {
                targetAngle = 90f;
            }

            curAngle = Mathf.Lerp(curAngle, targetAngle, rotationSpeed * Time.deltaTime);

            // Update our current rotation
            if (dived && input.magnitude > 0)
            {
                // Apply rotation
                rootB.RotateAround(rootB.position, transform.right, curAngle);
            }
        }

        void WaterRingEffect()
        {
            if (!waterRipplesPFX)
                return;

            // switch between waterRingFrequency for idle and swimming
            if (input != Vector3.zero) waterRingSpawnFrequency = waterRingFrequencySwim;
            else waterRingSpawnFrequency = waterRingFrequencyIdle;

            // counter to instantiate the waterRingEffects using the current frequency
            timer += Time.deltaTime;
            if (timer >= waterRingSpawnFrequency)
            {
                var newPos = new Vector3(transform.position.x, distY - 0.4f, transform.position.z);
                Instantiate(waterRipplesPFX, newPos, waterRipplesPFX.transform.rotation);
                timer = 0f;
            }
        }

        void CheckWaterHeightLevel()
        {
            waterHeightLevel = Mathf.Abs(distY - transform.position.y);

            if (isSwimming)
                animator.updateMode = AnimatorUpdateMode.Normal;
            else
                animator.updateMode = AnimatorUpdateMode.Fixed;

            if (waterDripPos)
            {
                waterDripPos.transform.position = animator.GetBoneTransform(HumanBodyBones.Head).transform.position;
            }

            if (canSwim && waterHeightLevel < offsetToSurf)
            {
                WaterRingEffect();
            }
            if (isSwimming && !dived && waterHeightLevel > offsetToSurf)
                hudM.ShowInteractiveIcon("Dive", "");
            if (!isSwimming && !dived && waterHeightLevel > offsetToSurf && hudM.interactiveOverlayIcon1.buttonActionName == "Dive")
                hudM.ShowInteractiveIcon("", "");
            if (dived)
                hudM.ShowInteractiveIcon("Rise", "");

            if (canSwim && !isSwimming && waterHeightLevel > (offsetToSurf + 0.09f))
            {
                rigidBody.useGravity = false;
                animator.applyRootMotion = true;

                actionState = 1;
                animator.CrossFadeInFixedTime("Swimming", 0.2f);

                if (waterDripPos)
                    waterDripPos.gameObject.SetActive(false);

                if (verticalVelocity <= velocityToImpact)
                {
                    if (waterSplashAS)
                        waterSplashAS.PlayRandomClip();

                    //rigidBody.velocity = Vector3.down * 5; // Dive in fast
                    var newPos = new Vector3(transform.position.x, distY, transform.position.z);
                    Instantiate(waterImpactPFX, newPos, transform.rotation);
                }
                isSwimming = true;
            }
        }

        #endregion

        #region Combat Behaviour

        void CombatManagement()
        {
            //проверка, был ли клик по врагу
            if (!ClickToAttackController.enemyClicked)
            {
                attackPower = 0;
                return;
            }

            if (attackButton == false)
                preventAtkInteruption = false;
            if (inventoryM.isPauseMenuOn || !inventoryM.ConditionsToOpenMenu() || isSwimming || cc.IsAnimatorTag("Carry") || 
                preventAtkInteruption || cc.fullBodyInfo.IsTag("Intro") || climbData.inPosition || inventoryM.dialogueM.fadeUI.canvasGroup.alpha != 0)
            {
                attackPower = 0;
                return;
            }

            if (grounded && attackButton)
            {
                bool conditionsToUnarmedAttack = !wpnHolster.PrimaryWeaponHActive()
                                                 && !wpnHolster.PrimaryWeaponActive() && !wpnHolster.SecondaryActive() 
                                                 && !wpnHolster.ShieldHActive() && !wpnHolster.ShieldActive();
                if (conditionsToUnarmedAttack)
                    attackPower += Time.deltaTime;

                bool conditionsToArmedAttack = wpnHolster.PrimaryWeaponActive() || wpnHolster.SecondaryActive();
                if (conditionsToArmedAttack)
                    attackPower += Time.deltaTime;
            }
            else
            {
                attackPower -= Time.deltaTime;
            }
            attackPower = Mathf.Clamp(attackPower, 0.0f, 1);

            LightAttack();
            HeavyAttack();
            AerialAttack();
            ShieldAttack();
            AdrenalineRush();
            WeaponProjectile();
            FinisherAttack();
            BowAndArrowAttack();
            GunAttack();
        }


        void LightAttack()
        {
            if (!attackButton && !performingLightAttack && !isAttacking && attackPower > 0.0f && 
                wpnHolster.LightAttackStaminaConditions() && !preventAtkInteruption)
            {
                animator.SetTrigger("Light Attack");
                performingLightAttack = true;
            }

            // Start combo chain attack after intial light attack
            bool conditions = !performingHeavyAttack && !cc.fullBodyInfo.IsTag("Heavy Attack");
            if (cc.rpgaeIM.PlayerControls.Attack.triggered && isAttacking && conditions &&
                wpnHolster.LightAttackStaminaConditions() && !preventAtkInteruption)
                animator.SetTrigger("Light Attack");
        }

        void HeavyAttack()
        {
            if (slowDown) return;

            if (!performingHeavyAttack && !isAttacking && attackPower > 0.2f && 
                wpnHolster.HeavyAttackStaminaConditions() && !preventAtkInteruption)
            {
                animator.SetTrigger("Heavy Attack");
                performingHeavyAttack = true;
            }

            // Activate Altered Primary Weapon 
            if (hudM.curEnegry >= 10 && attackPower >= 1 && wpnHolster.PrimaryWeaponActive() && wpnHolster.primaryE.GetComponentInChildren<SkinnedMeshRenderer>().enabled)
            {
                if (wpnHolster.alteredPrimaryE != null)
                {
                    wpnHolster.alteredPrimaryE.SetActive(true);
                    wpnHolster.primaryE.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
                    wpnHolster.primaryE.GetComponent<HitBox>().PlayRandomSound("ChargeAS", false);
                }
            }
        }

        void AerialAttack()
        {
            bool conditionsToNotTo = (!slowDown && !wpnHolster.SecondaryActive());
            bool conditionsToAerialAttack = wpnHolster.PrimaryWeaponActive();
            if (!grounded && cc.rpgaeIM.PlayerControls.Attack.triggered && conditionsToNotTo && conditionsToAerialAttack)
            {
                if (!isAttacking && !cc.animator.IsInTransition(5))
                {
                    animator.SetTrigger("Aerial Attack");

                    Vector3 upwardVel = transform.up * (5);
                    Vector3 forwardVel = targetDirection * (5);
                    rigidBody.linearVelocity = upwardVel + forwardVel;
                }
            }
        }

        void ShieldAttack()
        {
            if (isAiming) return;

            if (wpnHolster.ShieldActive())
            {
                if (isStrafing)
                {
                    isBlocking = true;

                    wpnHolster.shieldE.transform.localRotation = Quaternion.Euler(0, 90, 90);

                    // Shield can recieve damage
                    wpnHolster.shieldE.GetComponent<HealthPoints>().isInvulnerable = false;

                    bool conditionsToDisableCollider = cc.IsAnimatorTag("Light Attack") || cc.fullBodyInfo.IsName("Heavy Charge")
                    || cc.IsAnimatorTag("Heavy Attack");
                    if (isDodging && conditionsToDisableCollider)
                        wpnHolster.shieldE.GetComponent<Collider>().enabled = false;
                    else
                        wpnHolster.shieldE.GetComponent<Collider>().enabled = true;

                    // Shield bash if you're not already
                    bool conditionsToShieldAttack = !cc.fullBodyInfo.IsName("Shield Attack");
                    if (cc.rpgaeIM.PlayerControls.Interact.triggered && conditionsToShieldAttack)
                    {
                        animator.SetTrigger("Shield Attack");
                    }
                }
                else
                {
                    isBlocking = false;

                    wpnHolster.shieldE.transform.localRotation = Quaternion.Euler(-12, 90, -243.4f);

                    // Shield cannot recieve damage
                    wpnHolster.shieldE.GetComponent<HealthPoints>().isInvulnerable = true;
                    wpnHolster.shieldE.GetComponent<Collider>().enabled = false;
                }
            }
        }

        void BowAndArrowAttack()
        {
            if (cc.weaponArmsID != 6) return;

            if (isStrafing && cc.rightArmdInfo.IsTag("WeaponArms"))
            {
                if (wpnHolster.SecondaryActive() && cc.weaponArmsID == 6)
                {
                    ItemData itemData = wpnHolster.secondaryE.GetComponent<ItemData>();

                    cc.upperBodyID = cc.weaponArmsID;

                    Vector3 aimDirection = cc.aimReference.position - cc.rightHandIKTarget.position;
                    Quaternion aimLookPoint = Quaternion.LookRotation(aimDirection.normalized);
                    Vector3 arrowSpawnPoint = wpnHolster.arrowPrefabSpot.transform.position;

                    if (cc.rpgaeIM.PlayerControls.Attack.triggered &&
                    inventoryM.bowAndArrowInv.counter[inventoryM.bowAndArrowInv.slotArrowEquipped] > 0)
                        isAiming = true;
                    else if (inventoryM.bowAndArrowInv.counter[inventoryM.bowAndArrowInv.slotArrowEquipped] < 1)
                    {
                        wpnHolster.secondaryE.GetComponentInChildren<Animator>().SetBool("Draw", false);
                        wpnHolster.secondaryE.GetComponentInChildren<Animator>().SetFloat("DrawPower", 0);
                        isAiming = false;
                    }

                    cc.DrawingBowAnimation();

                    if (wpnHolster.ArrowOnStringActive() && !wpnHolster.ArrowActive() && isAiming)
                    {
                        if (!cc.upperBodyInfo.IsName("Reload") && !isReloading && !attackButton && cc.drawPower > 0)
                        {
                            for (int i = 0; i < itemData.bowRange.shotAmount; i++)
                            {
                                GameObject arrow = Instantiate(wpnHolster.arrowD, arrowSpawnPoint, aimLookPoint) as GameObject;
                                arrow.SetActive(true);
                                wpnHolster.SetArrowDamage(ref arrow, ref itemData);
                                arrow.GetComponentInChildren<HitBox>().tag = "Player";
                                arrow.GetComponentInChildren<HitBox>().targetLayers = targetLayer;
                                arrow.GetComponentInChildren<HitBox>().isAttacking = true;
                                arrow.GetComponentInChildren<HitBox>().isProjectile = true;
                                arrow.GetComponentInChildren<HitBox>().weaponTrail = true;
                                arrow.GetComponentInChildren<ItemData>().itemActive = true;
                                arrow.GetComponentInChildren<ItemData>().quantity = 1;
                                arrow.GetComponentInChildren<HitBox>().BeginAttack("Small Blood Hit");
                            }
                            wpnHolster.secondaryE.GetComponent<HealthPoints>().curHealthPoints -= 1;
                            wpnHolster.secondaryE.GetComponent<HitBox>().PlayRandomSound("BowShotAS", false);

                            inventoryM.RemoveStackableItem(ref inventoryM.bowAndArrowInv.itemData, ref inventoryM.bowAndArrowInv.image,
                            ref inventoryM.bowAndArrowInv.highLight, ref inventoryM.bowAndArrowInv.outLineBorder, ref inventoryM.bowAndArrowInv.statValueBG, ref inventoryM.bowAndArrowInv.quantity,
                            ref inventoryM.bowAndArrowInv.statValue, ref inventoryM.bowAndArrowInv.slotArrowEquipped, ref inventoryM.bowAndArrowInv.counter,
                            ref inventoryM.bowAndArrowInv.statCounter, ref inventoryM.bowAndArrowInv.removeNullSlots, 1);

                            isReloading = true;
                            if (wpnHolster.arrowString)
                                wpnHolster.arrowString.SetActive(false);
                        }
                    }
                }
            }
            else
            {
                isAiming = false;
                wpnHolster.secondaryE.GetComponentInChildren<Animator>().SetBool("Draw", false);
                wpnHolster.secondaryE.GetComponentInChildren<Animator>().SetFloat("DrawPower", 0);
            }
        }

        void GunAttack()
        {
            if (cc.weaponArmsID != 7) return;

            if (isStrafing)
            {
                if (wpnHolster.SecondaryActive() && cc.weaponArmsID == 7)
                {
                    ItemData itemData = wpnHolster.secondaryE.GetComponent<ItemData>();

                    cc.upperBodyID = cc.weaponArmsID;

                    Vector3 aimDirection = cc.aimReference.position - cc.rightHandIKTarget.position;
                    Quaternion aimLookPoint = Quaternion.LookRotation(aimDirection.normalized);
                    Vector3 arrowSpawnPoint = wpnHolster.arrowPrefabSpot.transform.position;

                    animator.SetBool("Reloading", cc.isReloading);
                    animator.SetFloat("UpperBodyID", cc.weaponArmsID);

                    if (inventoryM.bowAndArrowInv.counter[inventoryM.bowAndArrowInv.slotArrowEquipped] > 0)
                    {
                        isAiming = true;
                        cc.lookWeight = 1;
                        animator.SetBool("Draw", attackButton);
                        if (!cc.upperBodyInfo.IsName("Reload") && !isReloading && attackButton)
                        {
                            wpnHolster.secondaryE.GetComponent<HitBox>().effects.signatureEffect.gameObject.SetActive(true);
                            wpnHolster.secondaryE.GetComponent<HitBox>().effects.signatureEffect.Play();
                            for (int i = 0; i < itemData.bowRange.shotAmount; i++)
                            {
                                GameObject _bullet = Instantiate(wpnHolster.arrowD, arrowSpawnPoint, aimLookPoint) as GameObject;
                                _bullet.SetActive(true);
                                wpnHolster.SetArrowDamage(ref _bullet, ref itemData);
                                _bullet.GetComponentInChildren<HitBox>().tag = "Player";
                                _bullet.GetComponentInChildren<HitBox>().targetLayers = targetLayer;
                                _bullet.GetComponentInChildren<HitBox>().isAttacking = true;
                                _bullet.GetComponentInChildren<HitBox>().isProjectile = true;
                                _bullet.GetComponentInChildren<HitBox>().weaponTrail = true;
                                _bullet.GetComponentInChildren<ItemData>().itemActive = true;
                                _bullet.GetComponentInChildren<HitBox>().BeginAttack("Small Blood Hit");

                                wpnHolster.secondaryE.GetComponent<HitBox>().PlayRandomSound("RifleShotAS", false);
                            }
                            wpnHolster.secondaryE.GetComponent<HealthPoints>().curHealthPoints -= 1;

                            inventoryM.RemoveStackableItem(ref inventoryM.bowAndArrowInv.itemData, ref inventoryM.bowAndArrowInv.image,
                            ref inventoryM.bowAndArrowInv.highLight, ref inventoryM.bowAndArrowInv.outLineBorder, ref inventoryM.bowAndArrowInv.statValueBG, ref inventoryM.bowAndArrowInv.quantity,
                            ref inventoryM.bowAndArrowInv.statValue, ref inventoryM.bowAndArrowInv.slotArrowEquipped, ref inventoryM.bowAndArrowInv.counter,
                            ref inventoryM.bowAndArrowInv.statCounter, ref inventoryM.bowAndArrowInv.removeNullSlots, 1);

                            isReloading = true;
                        }
                    }
                    else
                    {
                        isAiming = false;
                        isStrafing = false;
                        isReloading = false;
                        wpnHolster.secondaryE.GetComponent<HitBox>().effects.signatureEffect.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                isAiming = false;
                isStrafing = false;
                isReloading = false;
                wpnHolster.secondaryE.GetComponent<HitBox>().effects.signatureEffect.gameObject.SetActive(false);
            }
        }

        void WeaponProjectile()
        {
            if (!isStrafing && isAiming)
            {
                actionState = 0;
                isAiming = false;
                if (!wpnHolster.ShieldActive())
                    isStrafing = false;

                return;
            }

            Vector3 aimDirection = cc.aimReference.position - cc.rightHandIKTarget.position;
            aimRotationPoint = Quaternion.LookRotation(aimDirection.normalized);
            projectileSpawnPoint = cc.rightHandIKTarget.position;

            if (isAiming)
            {
                cc.leftHandIKWeight = 0;
                cc.aimReference.position = Vector3.Lerp(cc.aimReference.position, lookPosition, 100 * Time.deltaTime);

                Vector3 toAimReferece = cc.aimReference.position - transform.position;
                Vector3 newDirection = Vector3.ProjectOnPlane(toAimReferece, Vector3.up);
                targetRotation = Quaternion.LookRotation(newDirection);

                Quaternion newRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                transform.rotation = newRotation;
            }
            else
                actionState = 0;

            // Throw Weapon Aim Stance
            if (cc.rpgaeIM.PlayerControls.Throw.triggered)
            {
                isAiming = true;
                canShoot = true;
                actionState = 3;
                cc.upperBodyID = 1;
            }

            // Throw weapon 
            if (attackButton && cc.upperBodyID == 1 && canShoot)
            {
                animator.SetTrigger("Throw");
                preventAtkInteruption = true;
                isStrafing = false;
            }

            // Projectile Aim Stance
            if (rpgaeIM.PlayerControls.Projectile.triggered && wpnHolster.PrimaryWeaponActive() &&
            wpnHolster.primaryE.GetComponentInChildren<ItemData>().throwDist.projectile != null)
            {
                isAiming = true;
                canShoot = true;
                cc.upperBodyID = 0;
            }

            // Projectile Attack
            if (attackButton && wpnHolster.PrimaryWeaponActive() &&
            wpnHolster.primaryE.GetComponentInChildren<ItemData>().throwDist.projectile != null)
            {
                cc.upperBodyID = 0;
            }
        }

        public float CalculateHypotenuse(Vector3 position)
        {
            float screenCenterX = tpCam.cam.pixelWidth / 2;
            float screenCenterY = tpCam.cam.pixelHeight / 2;

            Vector3 screenPosition = tpCam.cam.WorldToScreenPoint(position);
            float xDelta = screenCenterX - screenPosition.x;
            float yDelta = screenCenterY - screenPosition.y;
            float hypotenuse = Mathf.Sqrt(Mathf.Pow(xDelta, 2) + Mathf.Pow(yDelta, 2));

            return hypotenuse;
        }

        void AdrenalineRush()
        {
            #region Find Closest Enemy

            List<AIController> targetables = new List<AIController>();
            Collider[] m_colliders = Physics.OverlapSphere(transform.position, 15);
            foreach (Collider m_collder in m_colliders)
            {
                AIController targetable = m_collder.GetComponent<AIController>();
                if (targetable != null)
                {
                    targetables.Add(targetable);
                }
            }

            float hypotenuse;
            float smallesthypotenuse = Mathf.Infinity;
            AIController closestTargetable = null;
            foreach (AIController targetable in targetables)
            {
                hypotenuse = CalculateHypotenuse(targetable.transform.position);
                if (smallesthypotenuse > hypotenuse)
                {
                    closestTargetable = targetable;
                    smallesthypotenuse = hypotenuse;
                }
            }

            #endregion

            if (closestTargetable == null) return;

            float posDist = Vector3.Distance(transform.position, closestTargetable.transform.position);
            float posFwdAngle = Vector3.Distance(transform.forward, -closestTargetable.transform.forward);

            #region Perfect Dodge

            if (!cc.IsAnimatorTag("Hurt") && posDist <= 3)
            {
                if(posFwdAngle <= 0.8f)
                {
                    if (!slowDown && closestTargetable.canPerfectDodge && input.magnitude > 0 && cc.rpgaeIM.PlayerControls.Action.triggered)
                    {
                        closestTargetable.subjectedToSlowDown = true;

                        slowMotionTimer = slowMotionDuration;
                        if (slowMotionStartAS)
                            slowMotionStartAS.PlayRandomClip();

                        slowDown = true;
                        hp.invulnerabiltyTime = Mathf.Infinity;
                        hp.isInvulnerable = true;
                    }
                }
            }

            #endregion

            #region Bow Attack 

            if (wpnHolster.SecondaryActive())
            {
                if (!grounded)
                {
                    if (!slowDown && !bowRushOneShot && cc.rpgaeIM.PlayerControls.Attack.triggered)
                    {
                        actionState = 1;
                        slowMotionTimer = slowMotionDuration + 20;
                        if (slowMotionStartAS)
                            slowMotionStartAS.PlayRandomClip();

                        rigidBody.linearDamping = 6;
                        hp.invulnerabiltyTime = Mathf.Infinity;
                        hp.isInvulnerable = true;
                        bowRushOneShot = true;
                        slowDown = true;
                    }

                    if (!isStrafing)
                    {
                        StopSlowDown();
                    }
                }
                else
                {
                    StopSlowDown();
                    bowRushOneShot = false;
                }
            }

            #endregion

            if (slowDown)
            {
                hudM.curEnegry = Mathf.Lerp(hudM.curEnegry, hudM.maxEnegry, 30 * Time.deltaTime);
                slowDownVolume.weight = Mathf.Lerp(slowDownVolume.weight, 1, 6 * Time.deltaTime);

                slowMotionTimer -= 5 * Time.deltaTime;
                if(slowMotionTimer <= 0 || closestTargetable.GetComponent<HealthPoints>().curHealthPoints <= 0 || 
                closestTargetable.GetComponent<HealthPoints>().curStunPoints <= 0)
                {
                    closestTargetable.subjectedToSlowDown = false;
                    StopSlowDown();
                }

                // Slow down time by 10x
                Time.timeScale = Mathf.Lerp(Time.timeScale, 0.1f, 2 * Time.fixedDeltaTime);
                timeSpeed = Time.timeScale;
                Time.fixedDeltaTime = Time.timeScale * 0.02f;

                // Normalize animator speed
                if (!wpnHolster.SecondaryActive() && cc.rpgaeIM.PlayerControls.Attack.triggered && Time.timeScale < 0.1f + 0.2f)
                {
                    animator.speed = 10f;
                }
                if (wpnHolster.SecondaryActive() && Time.timeScale < 0.1f + 0.25f)
                {
                    animator.speed = 10f;
                    wpnHolster.secondaryE.GetComponent<Animator>().speed = 10;
                }

                // Move player towards enemy
                if (!wpnHolster.SecondaryActive() && animator.speed == 10f && posDist > 2.5f)
                {
                    rigidBody.linearVelocity = new Vector3(0, 0, 0);
                    transform.position = Vector3.Lerp(transform.position, closestTargetable.transform.position, 10 * Time.deltaTime);
                }
            }
            else
            {
                Time.timeScale = 1;
                slowDownVolume.weight = Mathf.Lerp(slowDownVolume.weight, 0, 6 * Time.deltaTime);
            }
        }

        public void StopSlowDown()
        {
            if (slowDown)
            {
                actionState = 0;
                Time.timeScale = 1;
                rigidBody.linearDamping = 1;
                animator.speed = 1;
                hp.invulnerabiltyTime = 0;
                if (wpnHolster.SecondaryActive())
                {
                    cc.rightHandWeight = 0;
                    wpnHolster.secondaryE.GetComponent<Animator>().speed = 1;
                    wpnHolster.secondaryE.GetComponent<Animator>().SetTrigger("SlowMotionEnd");
                    wpnHolster.secondaryE.GetComponent<Animator>().SetBool("Draw", false);
                }
                if (slowMotionEndAS)
                    slowMotionEndAS.PlayRandomClip();

                slowMotionTimer = 0;
                isAiming = false;
                isStrafing = false;
                slowDown = false;
            }
        }

        void FinisherAttack()
        {
            if (!wpnHolster.PrimaryWeaponActive()) return;

            #region Find Closest Enemy

            List<AIController> targetables = new List<AIController>();
            Collider[] m_colliders = Physics.OverlapSphere(transform.position, 15);
            foreach (Collider m_collder in m_colliders)
            {
                AIController targetable = m_collder.GetComponent<AIController>();
                if (targetable != null && targetable.stunned &&
                    targetable.GetComponent<HealthPoints>().curStunPoints <= 0)
                {
                    targetables.Add(targetable);
                }
            }

            float hypotenuse;
            float smallesthypotenuse = Mathf.Infinity;
            AIController closestTargetable = null;
            foreach (AIController targetable in targetables)
            {
                hypotenuse = CalculateHypotenuse(targetable.transform.position);
                if (smallesthypotenuse > hypotenuse)
                {
                    closestTargetable = targetable;
                    smallesthypotenuse = hypotenuse;
                }
            }

            #endregion

            if (closestTargetable == null) return;

            float posDist = Vector3.Distance(transform.position, closestTargetable.transform.position);
            float posFwdAngle = Vector3.Distance(transform.forward, -closestTargetable.transform.forward);

            if (posDist <= 4)
            {
                if (posFwdAngle <= 0.8f)
                {
                    bool conditionsToUseFinisher = closestTargetable.stunned && closestTargetable.fullBodyInfo.IsName("Stunned Loop");

                    if (conditionsToUseFinisher && cc.rpgaeIM.PlayerControls.LockOn.triggered && !finisherInProcess)
                    {
                        closestTargetable.navmeshAgent.enabled = false;
                        foreach (Collider col in GetComponentsInChildren<Collider>())
                        {
                            if (!col.GetComponent<CapsuleCollider>())
                            {
                               col.enabled = false; // To do: After finisher is performed will AI collide with anything?
                            }

                            input = Vector2.zero;
                            targetDirection = Vector3.zero;
                            rigidBody.linearVelocity = Vector3.zero;
                            canMove = false;

                            rigidBody.useGravity = false;
                            capCollider.enabled = false;
                            rigidBody.linearVelocity = Vector3.zero;
                            grounded = true;
                            animator.SetBool("Grounded", true);
                            actionState = 3;
                        }

                        tpCam.enabled = false;
                        tpCam.transform.SetParent(animator.GetBoneTransform(HumanBodyBones.Hips).transform);

                        oldWeaponT = wpnHolster.primaryE.transform.localPosition;
                        oldWeaponRot = wpnHolster.primaryE.transform.localRotation;

                        #region Finisher Target Position

                        if (cc.weaponArmsID == 1)
                        {
                            finisherDist = 2.7f;
                            finisherWeaponT = new Vector3(-0.3909996f, -0.00300026f, 0.01399986f);
                            finisherWeaponRot = Quaternion.Euler(0, -48.95f, 91.41f);
                        }
                        else if (cc.weaponArmsID == 2)
                        {
                            finisherDist = 2.48f;
                            finisherWeaponT = new Vector3(-0.39f, -0.007f, -0.025f);
                            finisherWeaponRot = Quaternion.Euler(-1.9f, -43.75f, 77.92f);
                        }
                        else if (cc.weaponArmsID == 3)
                        {
                            finisherDist = 2.2f;
                            finisherWeaponT = new Vector3(-0.356f, 0.027f, 0.057f);
                            finisherWeaponRot = Quaternion.Euler(-350.9f, -143.62f, -54.1f);
                        }
                        else if (cc.weaponArmsID == 4)
                        {
                            finisherDist = 3.2f;
                            finisherWeaponT = new Vector3(-0.4275f, 0.0243f, -0.0265f);
                            finisherWeaponRot = Quaternion.Euler(-380.6f, -235.8f, -61.4f);
                        }
                        else if (cc.weaponArmsID == 5)
                        {
                            finisherDist = 1.5f;
                            finisherWeaponT = new Vector3(-0.846f, -0.062f, -0.47f);
                            finisherWeaponRot = Quaternion.Euler(-402.9f, -222.1f, -96.7f);
                        }

                        #endregion

                        if (conditionsToUseFinisher && closestTargetable.GetComponentInChildren<AIHealthGuage>() != null)
                            closestTargetable.GetComponentInChildren<AIHealthGuage>().gameObject.SetActive(false);

                        finisherInProcess = true;
                        closestTargetable.finisherInProcess = true;
                    }
                }
            }

            if (finisherInProcess && currentFinisherPosition >= 0.2f)
            {
                if (cc.tpCam.lockedOn && cc.weaponArmsID == 2)
                    cc.tpCam.ToggleLockOn(!cc.tpCam.lockedOn);

                animator.SetTrigger("Finisher");
                closestTargetable.animator.SetTrigger("Finisher");
                closestTargetable.animator.SetFloat("FinisherID", cc.weaponArmsID);

                wpnHolster.primaryE.transform.localPosition = finisherWeaponT;
                wpnHolster.primaryE.transform.localRotation = finisherWeaponRot;

                finisherInProcess = false;
            }

            if (finisherInProcess)
            {
                currentFinisherPosition = Mathf.MoveTowards(currentFinisherPosition, 1, 0.5f * Time.deltaTime);
                transform.position = Vector3.Lerp(transform.position, closestTargetable.transform.position + (closestTargetable.transform.forward * finisherDist), currentFinisherPosition);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(-closestTargetable.transform.forward), currentFinisherPosition);
            }
        }

        #endregion

        #region Holster Behavior 

        void HolsterBehaviour()
        {
            if (inventoryM.isPauseMenuOn || isStrafing || isAttacking || isAiming
            || inventoryM.dialogueM.fadeUI.canvasGroup.alpha != 0 || cc.isUsingLadder ||
            cc.isPlayingAnimation || climbData.inPosition || cc.systemM.loadingScreenFUI.canvasGroup.alpha != 0 || 
            cc.systemM.blackScreenFUI.canvasGroup.alpha != 0 || cc.fullBodyInfo.IsTag("Intro") || !inventoryM.ConditionsToOpenMenu()) return;

            // EQUIP-------------------------------------------------------------------------------------- 
            if (cc.rpgaeIM.PlayerControls.Attack.triggered && !isSheathing && CanSheath())
            {
                if (wpnHolster.PrimaryWeaponHActive())
                {
                    if (cc.weaponArmsID == 6)
                    {
                        animator.SetInteger("EquipLeftHandID", 0);
                        animator.SetTrigger("EquipLeftHand");
                    }

                    if (wpnHolster.SecondaryActive() && wpnHolster.ArrowActive() || wpnHolster.ArrowOnStringActive())
                    {
                        if (wpnHolster.arrowE)
                            wpnHolster.arrowE.SetActive(true);
                        if (wpnHolster.arrowString)
                            wpnHolster.arrowString.SetActive(false);
                    }

                    SetWeaponArms(wpnHolster.primaryE);
                    animator.SetInteger("EquipRightHandID", 1);
                    animator.SetTrigger("EquipRightHand");
                    isSheathing = true;
                }

                if (wpnHolster.ShieldHActive())
                {
                    if (wpnHolster.primaryE != null &&
                    wpnHolster.primaryE.GetComponent<ItemData>().weaponArmsID > 1)
                    {
                        return;
                    }

                    #region Switch 

                    if (wpnHolster.SecondaryActive() && wpnHolster.ArrowActive() || wpnHolster.ArrowOnStringActive() &&
                      wpnHolster.primaryE == null)
                    {
                        wpnHolster.arrowE.SetActive(true);
                        wpnHolster.arrowString.SetActive(false);
                        animator.SetInteger("EquipRightHandID", 0);
                        animator.SetTrigger("EquipRightHand");
                    }

                    if (wpnHolster.PrimaryWeaponHActive())
                    {
                        animator.SetInteger("EquipRightHandID", 1);
                        animator.SetTrigger("EquipRightHand");
                    }

                    #endregion

                    SetWeaponArms(wpnHolster.shieldE);
                    animator.SetInteger("EquipLeftHandID", 2);
                    animator.SetTrigger("EquipLeftHand");
                    isSheathing = true;
                }
            }

            if (cc.rpgaeIM.PlayerControls.Secondary.triggered && !isSheathing && CanSheath())
            {
                if (wpnHolster.SecondaryHActive())
                {
                    if (!wpnHolster.ArrowOnStringActive() && !wpnHolster.ArrowActive())
                    {
                        animator.SetInteger("EquipRightHandID", 2);
                        animator.SetTrigger("EquipRightHand");
                    }

                    SetWeaponArms(wpnHolster.secondaryE);
                    animator.SetInteger("EquipLeftHandID", 1);
                    animator.SetTrigger("EquipLeftHand");
                    isSheathing = true;
                }
            }

            // PUT AWAY--------------------------------------------------------------------------------------
            if (cc.rpgaeIM.PlayerControls.Interact.triggered && !isSheathing && CanSheath())
            {
                if (wpnHolster.PrimaryWeaponActive())
                {
                    animator.SetInteger("EquipRightHandID", 0);
                    animator.SetTrigger("EquipRightHand");
                    isSheathing = true;
                }

                if (wpnHolster.SecondaryActive())
                {
                    if (wpnHolster.ArrowActive() || wpnHolster.ArrowOnStringActive())
                    {
                        if (wpnHolster.arrowString != null)
                            wpnHolster.arrowString.SetActive(false);
                        if (wpnHolster.arrowE != null)
                            wpnHolster.arrowE.SetActive(true);

                        animator.SetInteger("EquipRightHandID", 0);
                        animator.SetTrigger("EquipRightHand");
                    }

                    if(cc.weaponArmsID == 6)
                    {
                        isReloading = false;
                        cc.weaponArmsID = 0;
                        animator.SetInteger("EquipLeftHandID", 0);
                        animator.SetTrigger("EquipLeftHand");
                    }
                    if (cc.weaponArmsID == 7)
                    {
                        isReloading = false;
                        animator.SetInteger("EquipRightHandID", 0);
                        animator.SetTrigger("EquipRightHand");
                    }

                    isSheathing = true;
                }

                if (wpnHolster.ShieldActive())
                {
                    animator.SetInteger("EquipLeftHandID", 0);
                    animator.SetTrigger("EquipLeftHand");
                    isSheathing = true;
                }
            }
        }

        void SetWeaponArms(GameObject activeWeapon)
        {
            if (activeWeapon.GetComponent<ItemData>().weaponArmsID > 0)
                cc.weaponArmsID = activeWeapon.GetComponent<ItemData>().weaponArmsID;
            else
                cc.infoMessage.info.text = "WeaponArmsID has to be a higher value than zero, " +
                    "please set weapon arms ID according to its weapon style.";
        }

        public bool CanSheath()
        {
            if (cc.IsAnimatorTag("Sheath R") || cc.IsAnimatorTag("Unsheathe R")
                || cc.IsAnimatorTag("Sheath L") || cc.IsAnimatorTag("Unsheathe L"))
                return false;

            return true;
        }

        #endregion

        #region HitBox 

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
            #region Blood Effect

            if (bloodEffect != null)
                bloodEffect.CreateBloodEffect(data.damageSource, data.damager.GetComponentInChildren<HitBox>().bloodEffectName);

            #endregion

            #region Particle Effect

            if (data.damager.GetComponentInChildren<HitBox>().effects.stickOnContactEffectHit != null)
            {
                GameObject stickEffect = Instantiate(data.damager.GetComponentInChildren<HitBox>().effects.stickOnContactEffectHit.gameObject, 
                animator.GetBoneTransform(HumanBodyBones.Hips).transform.position, transform.rotation) as GameObject;

                stickEffect.transform.parent = transform;
                GetComponent<HealthPoints>().stickOnEffectTimer = 5;
            }

            if (data.damager.GetComponent<ItemData>() != null &&
                data.damager.GetComponent<ItemData>().itemType == ItemData.ItemType.Arrow)
            {
                Destroy(data.damager.GetComponent<HitBox>().gameObject);
            }

            if (wpnHolster.PrimaryWeaponActive())
                wpnHolster.primaryE.GetComponent<HitBox>().trailActive = false;

            #endregion

            #region Hurt Angle

            if (!isKnockedBack && data.damager.GetComponentInChildren<HitBox>().hurtID == 1)
            {
                animator.SetTrigger("Hurt");

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

                animator.SetInteger("HurtAngle", _angle);
            }
            else if (data.damager.GetComponentInChildren<HitBox>().hurtID == 2)
            {
                animator.SetTrigger("KnockBack");
            }

            performingLightAttack = false;
            performingHeavyAttack = false;

            #endregion
        }

        public void Die(HealthPoints.DamageData data)
        {
            animator.SetBool("Dead", true);

            systemM.gameOverFUI.gameObject.SetActive(true);
            systemM.gameOverFUI.FadeTransition(1, 2, 1);

            systemM.blackScreenFUI.gameObject.SetActive(true);
            systemM.blackScreenFUI.FadeTransition(1, 4, 0.5f);

            systemM.gameOverOptionFUI.gameObject.SetActive(true);
            systemM.gameOverOptionFUI.FadeTransition(1, 6, 1);
        }

        #endregion

        #region Colliders Check

        /// <summary>
        /// Disables rigibody gravity, turn the capsule collider trigger and reset all input from the animator.
        /// </summary>
        public void DisableGravityAndCollision()
        {
            animator.SetFloat("InputHorizontal", 0f);
            animator.SetFloat("InputVertical", 0f);
            animator.SetFloat("VerticalVelocity", 0f);
            rigidBody.useGravity = false;
            capCollider.isTrigger = true;
        }

        /// <summary>
        /// Turn rigidbody gravity on the uncheck the capsulle collider as Trigger when the animation has finish playing
        /// </summary>
        /// <param name="normalizedTime">Check the value of your animation Exit Time and insert here</param>
        public void EnableGravityAndCollision(float normalizedTime)
        {
            if (canSwim || isSwimming) return;

            // enable collider and gravity at the end of the animation
            if (cc.baseLayerInfo.normalizedTime >= normalizedTime)
            {
                capCollider.isTrigger = false;
                rigidBody.useGravity = true;
            }
        }

        #endregion

        #region Ground Check   

        void ExtraGravityForceWhileGrounded()
        {
            if (cc.IsAnimatorTag("Action") || isSwimming || actionState == 3 || finisherInProcess ||
                !capCollider.enabled || slowDown)
                return;

            Vector3 extraGravity = (Physics.gravity * gravityMultiplier) - Physics.gravity;
            rigidBody.AddForce(extraGravity);
        }

        protected virtual void CheckGround()
        {
            CheckGroundDistance();
            ControlMaterialPhysics();

            // we don't want to stick the character grounded if one of these bools is true
            bool checkGroundConditions = !isSwimming;

            if (checkGroundConditions)
            {
                if (groundDistance <= 0.05f)
                {
                    grounded = true;
                    animator.applyRootMotion = true;
                    Sliding();
                }
                else
                {
                    ExtraGravityForceWhileGrounded();
                    if (groundDistance >= groundCheckDistance)
                    {
                        grounded = false;
                        verticalVelocity = rigidBody.linearVelocity.y;
                        animator.SetFloat("InputMagnitude", input.magnitude, 0.2f, Time.deltaTime);
                        if (!cc.IsAnimatorTag("Action") && !climbData.inPosition)
                        {
                            animator.applyRootMotion = false;
                        }
                    }
                }
            }
        }

        protected virtual void ControlMaterialPhysics()
        {
            // change the physics material to very slip when not grounded
            capCollider.material = (grounded && GroundAngle() <= slopeLimit + 1) ? frictionPhysics : slippyPhysics;

            if (grounded && input == Vector3.zero)
                capCollider.material = maxFrictionPhysics;
            else if (grounded && input != Vector3.zero)
                capCollider.material = frictionPhysics;
            else if (cc.IsAnimatorTag("Falling") || cc.IsAnimatorTag("WalkJump") || cc.IsAnimatorTag("RunJump"))
                capCollider.material = slippyPhysics;
            else
                capCollider.material = slippyPhysics;
        }

        protected virtual void CheckGroundDistance()
        {
            if (capCollider != null)
            {
                // radius of the SphereCast
                float radius = capCollider.radius * 0.9f;
                var dist = 10f;
                // ray for RayCast
                Ray ray2 = new Ray(transform.position + new Vector3(0, colliderHeight / 2, 0), Vector3.down);
                // raycast for check the ground distance
                if (Physics.Raycast(ray2, out groundHit, (colliderHeight / 2) + dist, groundLayer) && !groundHit.collider.isTrigger)
                    dist = transform.position.y - groundHit.point.y;
 
                // sphere cast around the base of the capsule to check the ground distance
                if (dist >= groundMinDistance)
                {
                    Vector3 pos = transform.position + Vector3.up * (capCollider.radius);
                    Ray ray = new Ray(pos, -Vector3.up);
                    if (Physics.SphereCast(ray, radius, out groundHit, capCollider.radius + groundMaxDistance, groundLayer) && !groundHit.collider.isTrigger)
                    {
                        Physics.Linecast(groundHit.point + (Vector3.up * 0.1f), groundHit.point + Vector3.down * 0.15f, out groundHit, groundLayer);
                        float newDist = transform.position.y - groundHit.point.y;
                        if (dist > newDist) dist = newDist;
                    }
                }
                groundDistance = (float)System.Math.Round(dist, 2);
            }
        }

        public virtual float GroundAngle()
        {
            var groundAngle = Vector3.Angle(groundHit.normal, Vector3.up);
            return groundAngle;
        }

        public virtual float GroundAngleFromDirection()
        {
            var dir = isStrafing && input.magnitude > 0 ? (transform.right * input.x + transform.forward * input.z).normalized : transform.forward;
            var movementAngle = Vector3.Angle(dir, groundHit.normal) - 90;
            return movementAngle;
        }

        void Sliding()
        {
            var onStep = StepOffset();

            if (GroundAngle() > slopeLimit && GroundAngle() <= 85 && groundDistance <= 0.05f && !onStep)
            {
                isSliding = true;

                if (cc.IsAnimatorTag("Falling") || cc.IsAnimatorTag("WalkJump") || cc.IsAnimatorTag("RunJump"))
                    grounded = true;
                else
                    grounded = false;

                var _slideVelocity = slideVelocity + (GroundAngle() - slopeLimit);
                _slideVelocity = Mathf.Clamp(_slideVelocity, slideVelocity, 10);

                rigidBody.linearVelocity = new Vector3(rigidBody.linearVelocity.x, -slideVelocity, rigidBody.linearVelocity.z);
            }
            else
            {
                isSliding = false;
                grounded = true;
            }
        }

        bool StepOffset()
        {
            if (input.sqrMagnitude < 0.1 || !grounded || canMove) return false;

            var _hit = new RaycastHit();
            var _movementDirection = isStrafing && input.magnitude > 0 ? (transform.right * input.x + transform.forward * input.y).normalized : transform.forward;
            Ray rayStep = new Ray((transform.position + new Vector3(0, stepOffsetEnd, 0) + _movementDirection * ((capCollider).radius + 0.05f)), Vector3.down);

            if (Physics.Raycast(rayStep, out _hit, stepOffsetEnd - stepOffsetStart, groundLayer) && !_hit.collider.isTrigger)
            {
                if (_hit.point.y >= (transform.position.y) && _hit.point.y <= (transform.position.y + stepOffsetEnd))
                {
                    var _speed = Mathf.Clamp(input.magnitude, 0, 1f);
                    var velocityDirection = (_hit.point - transform.position);
                    var vel = rigidBody.linearVelocity;
                    vel.y = (velocityDirection * stepSmooth * (_speed * (velocity > 1 ? velocity : 1))).y;
                    rigidBody.linearVelocity = vel;
                    return true;
                }
            }
            return false;
        }

        #endregion
    }
}
