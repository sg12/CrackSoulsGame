using System;
using System.Collections;
using System.Collections.Generic;
using RPGAE.CharacterController;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace BLINK.Controller
{
    public class ValidCoordinate
    {
        public bool Valid;
        public Vector3 ValidPoint;
    }

    public class MoveInputType
    {
        public bool Valid;
        public bool Held;
    }

    [RequireComponent(typeof(NavMeshAgent), typeof(ThirdPersonInput))]
    public class TopDownClickToMoveController : MonoBehaviour
    {
        // REFERENCES
        public AudioSource cameraAudio;
        public Animator anim;
        private static readonly int IsMoving = Animator.StringToHash("isMoving");
        public NavMeshAgent agent;
        public LayerMask enemyLayer;
        
        // CAMERA
        public Camera playerCamera;
        public bool cameraEnabled = true;
        public bool initCameraOnSpawn = true;
        public string cameraName = "Main Camera";
        public Vector3 cameraPositionOffset = new Vector3(0, 10, 1);
        public Vector3 cameraRotationOffset = new Vector3(45, 0, 0);

        public float minCameraHeight = 2,
            maxCameraHeight = 15,
            minCameraVertical = 1.5f,
            maxCameraVertical = 14.5f,
            cameraZoomSpeed = 15,
            cameraZoomPower = 15,
            cameraRotateSpeed = 150f;

        private float currentCameraHeight, cameraHeightTarget, currentCameraVertical, cameraVerticalTarget;

        // NAVIGATION
        public bool movementEnabled = true;
        public bool stunned;
        public float destinationTreshold = 0.25f;
        public LayerMask groundLayers;
        public float maxGroundRaycastDistance = 100;
        public float minimumPathDistance = 0.5f;
        public float samplePositionDistanceMax = 5f;

        // INPUT SETTINGS
        public KeyCode moveKey = KeyCode.Mouse1,
            standKey = KeyCode.LeftShift,
            camRotateKeyLeft = KeyCode.LeftArrow,
            camRotateKeyRight = KeyCode.RightArrow;

        public bool allowHoldKey = true;
        public float holdMoveCd = 0.1f, nextHoldMove;
        public bool charLookAtCursorWhileStanding = true, canRotateCamera = true;

        // INPUT FEEDBACK
        public bool alwaysTriggerGroundPathFeedback;
        public GameObject validGroundPathPrefab, rectifiedGroundPathPrefab;
        public AudioClip validGroundPathAudio, rectifiedGroundPathAudio;
        public float groundMarkerDuration = 2;
        public Vector3 markerPositionOffset = new Vector3(0, 0.1f, 0);

        // STATES
        public enum CharacterState
        {
            Idle,
            Moving,
            Standing,
            Stunned
        }

        public CharacterState currentCharacterState;
        private static readonly int Standing = Animator.StringToHash("IsStanding");
        private static readonly int IsStunned = Animator.StringToHash("IsStunned");

        private ThirdPersonInput controlP;

        private void Awake()
        {
            InitCameraValues();
            InitCamera();
            InitAudio();
            controlP = GetComponent<ThirdPersonAnimator>();
        }

        private void Update()
        {
            if (NeedBlockAnyAction())
                    return;

            StandingLogic();
            MovementLogic();
            CameraLogic();
            EnemyClickMovement();
        }

        private bool NeedBlockAnyAction()
        {
            return controlP.inventoryM.dialogueM.fadeUI.canvasGroup.alpha != 0;
        }

        private void LateUpdate()
        {
            HandleCamera();
            CharacterStateLogic();
        }

        #region INIT

        private void InitCameraValues()
        {
            currentCameraHeight = cameraPositionOffset.y;
            cameraHeightTarget = currentCameraHeight;
            currentCameraVertical = cameraPositionOffset.z;
            cameraVerticalTarget = currentCameraVertical;
        }

        private void InitCamera()
        {
            if (!initCameraOnSpawn && playerCamera != null) return;
            Camera cam = GameObject.Find(cameraName).GetComponent<Camera>();
            if (cam == null)
            {
                cam = Camera.main;
                if (cam == null)
                {
                    Debug.LogError("TOPDOWN_CLICK_CONTROLLER: NO CAMERA FOUND! MAKE SURE TO EITHER DRAG AND DROP ONE, OR ENABLE INIT CAMERA AND TYPE A VALID CAMERA NAME OR MAIN CAMERA TAG");
                }
                else
                {
                    playerCamera = cam;
                }
            }
            else
            {
                playerCamera = cam;
            }

            if (playerCamera == null) return;
            playerCamera.transform.eulerAngles = cameraRotationOffset;
            InstantCameraUpdate();
        }

        private void InitAudio()
        {
            if (cameraAudio == null) InitAudioSource();
        }

        #endregion

        #region LOGIC

        private void MovementLogic()
        {
            if (!movementEnabled || stunned || IsPointerOverUIObject()) return;
            if (IsStanding()) return;
            MoveInputType moveInputType = MovingInput();
            if (!moveInputType.Valid) return;
            if (!Physics.Raycast(playerCamera.ScreenPointToRay(Input.mousePosition), out var hit,
                maxGroundRaycastDistance, groundLayers)) return;
            var destination = hit.point;
            bool validClick = true;
            if (IsPathTooClose(destination)) return;
            if (!IsPathAllowed(destination))
            {
                ValidCoordinate newResult = closestAllowedDestination(destination);
                if (newResult.Valid)
                {
                    destination = newResult.ValidPoint;
                    validClick = false;
                }
                else
                {
                    return;
                }
            }

            TriggerNewDestination(destination);

            if (!alwaysTriggerGroundPathFeedback && moveInputType.Held) return;
            SpawnGroundPathMarker(destination, validClick);
            PlayGroundPathAudio(validClick);
        }

        private void EnemyClickMovement()
        {
            if (Input.GetKeyDown(moveKey))
            {
                if (IsPointerOverUIObject()) return; 

                Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, maxGroundRaycastDistance))
                {
                    GameObject hitObject = hit.collider.gameObject;
                    int hitLayer = hitObject.layer;

                    //слой врага
                    if (((1 << hitLayer) & enemyLayer) != 0)
                    {
                        Vector3 destination = hit.point;

                        if (IsPathAllowed(destination))
                        {
                            TriggerNewDestination(destination);
                        }
                        else
                        {
                            ValidCoordinate newResult = closestAllowedDestination(destination);
                            if (newResult.Valid)
                            {
                                destination = newResult.ValidPoint;
                                TriggerNewDestination(destination);
                            }
                            else
                            {
                                // Точка недостижима
                                return;
                            }
                        }

                        // доп логика
                    }
                }
            }
        }

        
        
        private void CameraLogic()
        {
            if (!cameraEnabled) return;
            CameraInputs();
            LerpCameraHeight();
        }

        private void StandingLogic()
        {
            if (Input.GetKeyDown(standKey)) InitStanding();
            if (IsStanding())
            {
                HandleStanding();
            }
            else if (IsEndStanding())
            {
                ResetStanding();
            }
        }


        private void InitStanding()
        {
            ResetAgentActions();
            StartCoroutine(SetCharacterState(CharacterState.Standing));
        }

        private void ResetStanding()
        {
            StartCoroutine(SetCharacterState(CharacterState.Idle));
        }

        public void ResetAgentActions()
        {
            agent.ResetPath();
            StartCoroutine(SetCharacterState(CharacterState.Idle));
        }

        private void HandleStanding()
        {
            if (!charLookAtCursorWhileStanding) return;
            ValidCoordinate validCoordinate = GetGroundRayPoint();
            if (!validCoordinate.Valid) return;
            var targetRotation = Quaternion.LookRotation(validCoordinate.ValidPoint - transform.position);
            targetRotation.x = 0;
            targetRotation.z = 0;
            transform.rotation = targetRotation;
        }


        private ValidCoordinate GetGroundRayPoint()
        {
            var playerPlane = new Plane(Vector3.up, transform.position);
            var ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            ValidCoordinate validCoordinate = new ValidCoordinate();
            if (!playerPlane.Raycast(ray, out var hitDist)) return validCoordinate;
            validCoordinate.Valid = true;
            validCoordinate.ValidPoint = ray.GetPoint(hitDist);
            return validCoordinate;
        }

        public bool IsStanding()
        {
            return Input.GetKey(standKey);
        }

        public bool IsEndStanding()
        {
            return Input.GetKeyUp(standKey);
        }

        private void CharacterStateLogic()
        {
            switch (currentCharacterState)
            {
                case CharacterState.Idle:
                    break;

                case CharacterState.Moving:
                    if (IsDestinationReached()) ResetAgentActions();
                    break;
            }
        }

        public void InitStun()
        {
            stunned = true;
            ResetAgentActions();
            StartCoroutine(SetCharacterState(CharacterState.Stunned));
        }

        public void ResetStun()
        {
            stunned = false;
            StartCoroutine(SetCharacterState(CharacterState.Idle));
        }

        public IEnumerator SetCharacterState(CharacterState state)
        {
            yield return new WaitForEndOfFrame();
            currentCharacterState = state;
            StartAnimation(state);
        }
        
        #endregion

        #region CAMERA

        void InstantCameraUpdate()
        {
            Vector3 targetPos = transform.position - (playerCamera.transform.forward * currentCameraHeight);
            targetPos.z -= currentCameraVertical;
            playerCamera.transform.position = targetPos;
        }
        
        private void CameraInputs()
        {
            HandleCameraZoom();
        }

        private void HandleCameraZoom()
        {
            if (Input.mouseScrollDelta.y == 0) return;
            float heightDifference = Input.mouseScrollDelta.y < 0f ? cameraZoomPower : -cameraZoomPower;
            cameraHeightTarget = currentCameraHeight + heightDifference;
            cameraVerticalTarget = currentCameraVertical + heightDifference;
            if (cameraHeightTarget > maxCameraHeight) cameraHeightTarget = maxCameraHeight;
            else if (cameraHeightTarget < minCameraHeight) cameraHeightTarget = minCameraHeight;
            if (cameraVerticalTarget > maxCameraVertical) cameraVerticalTarget = maxCameraVertical;
            else if (cameraVerticalTarget < minCameraVertical) cameraVerticalTarget = minCameraVertical;
        }

        private void HandleCamera()
        {
            if (!cameraEnabled) return;
            if (canRotateCamera)
            {
                Vector3 eulerAngles = playerCamera.transform.rotation.eulerAngles;
                if (Input.GetKey(camRotateKeyLeft))
                {
                    playerCamera.transform.rotation = Quaternion.Euler(eulerAngles.x,
                        eulerAngles.y - cameraRotateSpeed * Time.deltaTime, eulerAngles.z);

                }
                else if (Input.GetKey(camRotateKeyRight))
                {
                    playerCamera.transform.rotation = Quaternion.Euler(eulerAngles.x,
                        eulerAngles.y + cameraRotateSpeed * Time.deltaTime, eulerAngles.z);
                }
            }
            
            playerCamera.transform.position = (transform.position + Vector3.up * 0.8f) - (playerCamera.transform.forward * currentCameraHeight);
        }

        private void LerpCameraHeight()
        {
            currentCameraHeight = Mathf.Lerp(currentCameraHeight, cameraHeightTarget, Time.deltaTime * cameraZoomSpeed);
            currentCameraVertical = Mathf.Lerp(currentCameraVertical, cameraVerticalTarget, Time.deltaTime * cameraZoomSpeed);
        }

        #endregion

        #region NAVIGATION


        private bool IsPathAllowed(Vector3 point)
        {
            NavMeshPath path = new NavMeshPath();
            return NavMesh.CalculatePath(transform.position, point, NavMesh.AllAreas, path);
        }

        private ValidCoordinate closestAllowedDestination(Vector3 point)
        {
            ValidCoordinate newResult = new ValidCoordinate();
            if (!NavMesh.SamplePosition(point, out var hit, samplePositionDistanceMax, NavMesh.AllAreas))
                return newResult;
            newResult.Valid = true;
            newResult.ValidPoint = hit.position;
            return newResult;
        }

        private MoveInputType MovingInput()
        {
            MoveInputType moveInputType = new MoveInputType();
            if (Input.GetKeyDown(moveKey))
            {
                moveInputType.Valid = true;
                return moveInputType;
            }

            if (!(Time.time >= nextHoldMove))
            {
                moveInputType.Valid = false;
                return moveInputType;
            }

            if (!allowHoldKey || !Input.GetKey(moveKey)) return moveInputType;
            nextHoldMove = Time.time + holdMoveCd;
            moveInputType.Valid = true;
            moveInputType.Held = true;
            return moveInputType;
        }

        private void TriggerNewDestination(Vector3 location)
        {
            //ThirdPersonAnimator controlP = GameObject.FindAnyObjectByType<ThirdPersonAnimator>();
            //if (controlP.inventoryM.dialogueM.fadeUI.canvasGroup.alpha != 0)
            //    return;
            agent.SetDestination(location);
            StartCoroutine(SetCharacterState(CharacterState.Moving));
        }

        private bool IsDestinationReached()
        {
            return !agent.hasPath || agent.remainingDistance <= (agent.stoppingDistance + destinationTreshold);
        }

        private bool IsPathTooClose(Vector3 point)
        {
            return Vector3.Distance(transform.position, point) < minimumPathDistance;
        }

        #endregion

        #region FEEDBACK

        private void SpawnGroundPathMarker(Vector3 point, bool rectified)
        {
            GameObject prefab = rectified ? validGroundPathPrefab : rectifiedGroundPathPrefab;
            if (prefab == null) return;
            GameObject marker = Instantiate(prefab,
                new Vector3(point.x + markerPositionOffset.x, point.y + markerPositionOffset.y,
                    point.z + markerPositionOffset.z), prefab.transform.rotation);
            Destroy(marker, groundMarkerDuration);
        }

        private void PlayGroundPathAudio(bool rectified)
        {
            AudioClip audio = rectified ? validGroundPathAudio : rectifiedGroundPathAudio;
            if (audio == null) return;
            if (cameraAudio == null) InitAudioSource();
            cameraAudio.PlayOneShot(audio);
        }

        #endregion

        #region OTHER

        private void InitAudioSource()
        {
            AudioSource ASource = playerCamera.GetComponent<AudioSource>();
            if (ASource == null)
            {
                ASource = playerCamera.gameObject.AddComponent<AudioSource>();
            }

            cameraAudio = ASource;
        }

        private void StartAnimation(CharacterState state)
        {
            if (anim == null) return;
            ResetStateAnimations();
            switch (state)
            {
                case CharacterState.Idle:
                    break;
                case CharacterState.Moving:
                    anim.SetBool(IsMoving, true);
                    break;
                case CharacterState.Standing:
                    anim.SetBool(Standing, true);
                    break;
                case CharacterState.Stunned:
                    anim.SetBool(IsStunned, true);
                    break;
            }
        }

        private void ResetStateAnimations()
        {
            anim.SetBool(IsMoving, false);
            anim.SetBool(Standing, false);
            anim.SetBool(IsStunned, false);
        }

        public bool IsPointerOverUIObject()
        {
            var eventDataCurrentPosition = new PointerEventData(EventSystem.current)
            {
                position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
            };
            var results = new List<RaycastResult>();
            if (EventSystem.current == null) return false;
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }
        
        #endregion

    }
}
