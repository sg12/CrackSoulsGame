using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;

namespace RPGAE.CharacterController
{
    public class ThirdPersonInput : MonoBehaviour
    {
        #region Button Bools

        [HideInInspector]
        public float zoomValue;
        [HideInInspector]
        public bool attackButton;
        [HideInInspector]
        public bool sprintButton;
        [HideInInspector]
        public bool cursorButton;
        [HideInInspector]
        public float buttonTimer;

        #endregion

        #region Components 

        [HideInInspector]
        public Animator
        animator,
        bowAnim;

        [HideInInspector]
        public InfoMessage infoMessage;
        [HideInInspector]
        public WeaponHolster wpnHolster;
        [HideInInspector]
        public ThirdPersonController cc;
        [HideInInspector]
        public ThirdPersonCamera tpCam;
        [HideInInspector]
        public SceneTransitionDestination sceneTD;
        [HideInInspector]
        public SystemManager systemM;
        [HideInInspector]
        public HUDManager hudM;
        [HideInInspector]
        public InventoryManager inventoryM;
        [HideInInspector]
        public ItemObtained itemObtained;

        #endregion

        public RPGAEInputManager rpgaeIM;

        public enum ControllerType { XBOX, PLAYSTATION, MOUSEKEYBOARD };
        [HideInInspector] public ControllerType controllerType = ControllerType.MOUSEKEYBOARD;

        private void Awake()
        {
            rpgaeIM = new RPGAEInputManager();

            rpgaeIM.PlayerControls.Zoom.performed += x => zoomValue = x.ReadValue<float>();

            hudM = FindObjectOfType<HUDManager>();
            cc = GetComponent<ThirdPersonController>();
            tpCam = FindObjectOfType<ThirdPersonCamera>();
            infoMessage = FindObjectOfType<InfoMessage>();
            systemM = FindObjectOfType<SystemManager>();
            inventoryM = FindObjectOfType<InventoryManager>();
            animator = GetComponentInChildren<Animator>();
            itemObtained = FindObjectOfType<ItemObtained>();

            rpgaeIM.PlayerControls.Sprint.started += context => sprintButton = true;
            rpgaeIM.PlayerControls.Sprint.performed += context => sprintButton = true;
            rpgaeIM.PlayerControls.Sprint.canceled += context => sprintButton = false;

            rpgaeIM.PlayerControls.Attack.started += context => attackButton = true;
            rpgaeIM.PlayerControls.Attack.performed += context => attackButton = true;
            rpgaeIM.PlayerControls.Attack.canceled += context => attackButton = false;

            rpgaeIM.PlayerControls.Strafe.started += context => cc.isStrafing = true;
            rpgaeIM.PlayerControls.Strafe.performed += context => cc.isStrafing = true;
            rpgaeIM.PlayerControls.Strafe.canceled += context => cc.isStrafing = false;

            //rpgaeIM.PlayerControls.RightClick.started += context => cc.isRightClicking = true;
            //rpgaeIM.PlayerControls.RightClick.performed += context => cc.isRightClicking = true;
            //rpgaeIM.PlayerControls.RightClick.canceled += context => cc.isRightClicking = false;

            wpnHolster = GameObject.Find("PlayerHolster").GetComponent<WeaponHolster>();

            if (cc != null)
            {
                cc.Init();
                if (SceneManager.GetActiveScene().name == "Level2")
                {
                    animator.SetBool("Intro", true);
                }
                cc.AnimatorIKStart();
            }

            PlayerInput input = GetComponent<PlayerInput>();
            UpdateControllerType(input.currentControlScheme);
        }

        private void OnEnable()
        {
            rpgaeIM.Enable();

            rpgaeIM.PlayerControls.Movement.Enable();
            rpgaeIM.PlayerControls.CameraLook.Enable();

            rpgaeIM.PlayerControls.Action.Enable();
            rpgaeIM.PlayerControls.Interact.Enable();
            rpgaeIM.PlayerControls.RightBumber.Enable();
            rpgaeIM.PlayerControls.Attack.Enable();
            rpgaeIM.PlayerControls.InventoryHUD1.Enable();
            rpgaeIM.PlayerControls.InventoryHUD2.Enable();
            rpgaeIM.PlayerControls.Strafe.Enable();
            rpgaeIM.PlayerControls.Sprint.Enable();
            rpgaeIM.PlayerControls.LockOn.Enable();

            rpgaeIM.PlayerControls.Throw.Enable();
            rpgaeIM.PlayerControls.Projectile.Enable();

            rpgaeIM.PlayerControls.DPadUp.Enable();
            rpgaeIM.PlayerControls.DPadLeft.Enable();
            rpgaeIM.PlayerControls.DPadRight.Enable();
            rpgaeIM.PlayerControls.DPadDown.Enable();

            rpgaeIM.PlayerControls.Movement.Enable();
            rpgaeIM.PlayerControls.CameraLook.Enable();
            rpgaeIM.UICursor.Point.Enable();

            rpgaeIM.PlayerControls.Start.Enable();

            rpgaeIM.UICursor.Click.performed += CursorButtonAction;

            InputUser.onChange += onInputDeviceChange;
        }

        private void OnDisable()
        {
            rpgaeIM.Disable();

            rpgaeIM.PlayerControls.Movement.Disable();
            rpgaeIM.PlayerControls.CameraLook.Disable();

            rpgaeIM.PlayerControls.Action.Disable();
            rpgaeIM.PlayerControls.Interact.Disable();
            rpgaeIM.PlayerControls.RightBumber.Disable();
            rpgaeIM.PlayerControls.Attack.Disable();
            rpgaeIM.PlayerControls.InventoryHUD1.Disable();
            rpgaeIM.PlayerControls.InventoryHUD2.Disable();
            rpgaeIM.PlayerControls.Strafe.Disable();
            rpgaeIM.PlayerControls.Sprint.Disable();
            rpgaeIM.PlayerControls.LockOn.Disable();

            rpgaeIM.PlayerControls.Throw.Disable();
            rpgaeIM.PlayerControls.Projectile.Disable();

            rpgaeIM.PlayerControls.DPadUp.Disable();
            rpgaeIM.PlayerControls.DPadLeft.Disable();
            rpgaeIM.PlayerControls.DPadRight.Disable();
            rpgaeIM.PlayerControls.DPadDown.Disable();

            rpgaeIM.PlayerControls.Movement.Disable();
            rpgaeIM.PlayerControls.CameraLook.Disable();
            rpgaeIM.UICursor.Point.Disable();

            rpgaeIM.PlayerControls.Start.Disable();

            InputUser.onChange -= onInputDeviceChange;
        }

        void onInputDeviceChange(InputUser user, InputUserChange change, InputDevice device)
        {
            if (change == InputUserChange.ControlSchemeChanged)
            {
                //Debug.Log(user.controlScheme.Value.name);
                UpdateControllerType(user.controlScheme.Value.name);
            }
        }

        void UpdateControllerType(string schemeName)
        {
            if (schemeName.Equals("Keyboard"))
            {
                controllerType = ControllerType.MOUSEKEYBOARD;
            }
            else if (schemeName.Equals("Xbox"))
            {
                controllerType = ControllerType.XBOX;
            }
            else if (schemeName.Equals("Playstation"))
            {
                controllerType = ControllerType.PLAYSTATION;
            }
        }

        void PreventOverlappingAxis()
        {
            if (cc.inventoryM.cursorVirtual.isActive)
            {
                rpgaeIM.PlayerControls.Movement.Disable();
                rpgaeIM.PlayerControls.CameraLook.Disable();
                rpgaeIM.UICursor.Point.Enable();
            }
            else
            {
                rpgaeIM.PlayerControls.Movement.Enable();
                rpgaeIM.PlayerControls.CameraLook.Enable();
                rpgaeIM.UICursor.Point.Disable();
            }
        }

        void InputHandle()
        {
            MoveInput();
            ShiftInput();
            //SprintInput();
            CameraInput();
            cc.Dodging();
            LockOnInput();
        }

        void LockOnInput()
        {
            if (rpgaeIM.PlayerControls.LockOn.triggered)
            {
                cc.LockOn();
            }
        }

        void MoveInput()
        {
           if (inventoryM.isPauseMenuOn || inventoryM.inventoryHUD.isActive || 
           inventoryM.dialogueM.fadeUI.canvasGroup.alpha != 0 || 
           systemM.blackScreenFUI.canvasGroup.alpha != 0 ||
           systemM.loadingScreenFUI.canvasGroup.alpha != 0) return;

            //cc.input.x = rpgaeIM.PlayerControls.Movement.ReadValue<Vector2>().x;
            //cc.input.z = rpgaeIM.PlayerControls.Movement.ReadValue<Vector2>().y;
            //cc.input.Normalize();
        }

        protected void ShiftInput()
        {
            bool conditiions = !inventoryM.isPauseMenuOn;

            if (sprintButton && cc.input.magnitude > 0.1f && conditiions)
                cc.Sprint(true);
            else
                cc.Sprint(false);
        }

        protected void SprintInput()
        {
            bool conditiions = !inventoryM.isPauseMenuOn;

            if (sprintButton && cc.input.magnitude > 0.1f && conditiions)
                cc.Sprint(true);
            else
                cc.Sprint(false);
        }

        void CameraInput()
        {
            if (tpCam == null || animator.GetBool("Dead") || !tpCam.enabled || inventoryM.inventoryHUD.isActive || 
            inventoryM.dialogueM.optionButton.activeInHierarchy || inventoryM.referencesObj.pauseMenu.GetComponent<FadeUI>().canvasGroup.alpha == 1 ||
            !inventoryM.ConditionsToOpenMenu()) return;

            var X = rpgaeIM.PlayerControls.CameraLook.ReadValue<Vector2>().x;
            var Y = rpgaeIM.PlayerControls.CameraLook.ReadValue<Vector2>().y;
            tpCam.CameraMovement(X, Y);
        }

        private void CursorButtonAction(InputAction.CallbackContext context)
        {
            cursorButton = true;
            if (!context.control.IsPressed())
            {
                cursorButton = false;
            }
        }

        void Update()
        {
            //Debug.Log("attackButton: " + attackButton);
            //Debug.Log("isAiming: " + cc.isAiming);
            //Debug.Log("isStrafing: " + cc.isStrafing);
            //Debug.Log("isStrafing: " + cc.isSheathing);
            InputHandle();      
            cc.ActionUpdate();
            cc.UpdateMotor();             
            cc.UpdateAnimator();
            PreventOverlappingAxis();
        }

        void FixedUpdate()
        {
           cc.WeaponAimIK();
           cc.RayCastLookPosition();
        }

        private void LateUpdate()
        {
           cc.DiveRotation();
        }
    }
}
