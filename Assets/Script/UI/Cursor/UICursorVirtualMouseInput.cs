using System;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine;
using RPGAE.CharacterController;

public class UICursorVirtualMouseInput : MonoBehaviour
{
    [SerializeField] public CursorController m_cursor;
    [SerializeField] public float m_ScrollSpeed = 45;

    public bool isActive;

    private float m_ScreenWidth;
    private float m_ScreenHeight;
    private float m_referenceResolutionWidth = 800f;
    private Vector3 m_lastMousePosition = Vector3.zero;

    private bool m_FirstEnabled = false;
    private bool m_UpdateCursor = true;

    private RPGAEInputManager rpgaeIM;
    private InputAction m_Point;
    private InputAction m_Scroll;
    private InputAction m_Click;

    private bool m_SetPosition = false;
    private Vector2 m_GoToPosition = Vector2.zero;
    private Vector3 m_currentMousePosition;
    private bool m_isSystemMouseMoving = false;
    private GameObject m_lastGameObjectOver;
    private Action buttonTriggerListener;
    private Mouse m_VirtualMouse;
    private Mouse m_SystemMouse;
    private MouseState mouseState;

    // Start is called before the first frame update
    void Awake()
    {
        //Cursor.visible = false;
        rpgaeIM = new RPGAEInputManager();
        m_cursor = FindObjectOfType<CursorController>();
    }

    private void Update()
    {
        if (isActive)
            m_cursor.fadeUI.canvasGroup.alpha = 1;
        else
            m_cursor.fadeUI.canvasGroup.alpha = 0;

        if (isActive)
        {
            buttonTriggerListener = OnAfterInputUpdate;
            InputSystem.onAfterUpdate += buttonTriggerListener;
        }
    }

    internal void SetCursorPosition(Vector2 position)
    {
        m_SetPosition = true;
        m_GoToPosition = position;
    }

    protected void OnEnable()
    {
        TryGetHardwareCursor();

        m_FirstEnabled = true;
        m_ScreenWidth = Screen.width;
        m_ScreenHeight = Screen.height;

        // Add mouse device.
        if (m_VirtualMouse == null)
            m_VirtualMouse = (Mouse)InputSystem.AddDevice("VirtualMouse");
        else if (!m_VirtualMouse.added)
            InputSystem.AddDevice(m_VirtualMouse);

        m_Point = rpgaeIM.UICursor.Point;
        m_Point.Enable();

        m_Click = rpgaeIM.UICursor.Click;
        m_Click.performed += SendEventAction;
        m_Click.Enable();

        m_Scroll = rpgaeIM.UICursor.Scroll;
        m_Scroll.Enable();
    }

    protected void OnDisable()
    {
        m_FirstEnabled = false;

        rpgaeIM.Disable();

        // Remove mouse device.
        if (m_VirtualMouse != null && m_VirtualMouse.added)
            InputSystem.RemoveDevice(m_VirtualMouse);

        m_Point = rpgaeIM.UICursor.Point;
        m_Point.Disable();

        m_Click = rpgaeIM.UICursor.Click;
        m_Click.Disable();

        m_Scroll = rpgaeIM.UICursor.Scroll;
        m_Scroll.Disable();
    }

    private void TryGetHardwareCursor()
    {
        var devices = InputSystem.devices;
        for (var i = 0; i < devices.Count; ++i)
        {
            var device = devices[i];
            if (device.native && device is Mouse mouse)
            {
                m_SystemMouse = mouse;
                break;
            }
        }
    }

    public void SetCursorToCenter()
    {
        if (m_cursor.RectTransform != null)
        {
            m_cursor.RectTransform.position = (new Vector2(Screen.width / 2, Screen.height / 2));
        }
    }

    private void OnAfterInputUpdate()
    {
        if (isActive && EventSystem.current != null && EventSystem.current.sendNavigationEvents == false)
        {
            if (m_UpdateCursor)
            {
                UpdateMotion();
            }
        }
    }

    private void UpdateMotion()
    {
        if (m_VirtualMouse == null || !isActive)
            return;

        m_currentMousePosition = Vector3.zero;
        m_isSystemMouseMoving = false;

        if (m_SystemMouse != null)
        {
            m_currentMousePosition = m_SystemMouse.position.ReadValue();
            var currentMouseDelta = m_SystemMouse.delta.ReadValue();
            m_isSystemMouseMoving = currentMouseDelta.magnitude > 0.25f;
        }

        // Set initial cursor position.
        if (m_FirstEnabled == true)
        {
            m_FirstEnabled = false;
            var position = m_cursor.RectTransform.position;
            InputState.Change(m_VirtualMouse.position, position);
        }

        // Read current stick value.
        var stickAction = m_Point;
        if (stickAction == null)
            return;
        var stickValue = stickAction.ReadValue<Vector2>();
        bool motionStop = Mathf.Approximately(0, stickValue.x) && Mathf.Approximately(0, stickValue.y);
        if ((motionStop && m_isSystemMouseMoving) || m_SystemMouse == null)
        {
            motionStop = false;
        }

        if (!motionStop || m_SetPosition)
        {
            var currentTime = InputState.currentTime;

            float speedThisFrame = (m_ScreenWidth * m_cursor.m_Speed) / m_referenceResolutionWidth;

            var delta = (speedThisFrame * stickValue) * Time.unscaledDeltaTime;

            //var currentPosition = m_VirtualMouse.position.ReadValue();
            var currentPosition = (Vector2)m_cursor.RectTransform.position;

            if (m_SystemMouse != null && m_isSystemMouseMoving)
            {
                delta = m_currentMousePosition - ((Vector3)currentPosition);
            }

            if (m_SetPosition)
            {
                delta = m_GoToPosition - currentPosition;
                m_SetPosition = false;
            }

            // Update position.
            var newPosition = currentPosition + delta;

            newPosition.x = Mathf.Clamp(newPosition.x, 0, m_ScreenWidth);
            newPosition.y = Mathf.Clamp(newPosition.y, 0, m_ScreenHeight);

            ////REVIEW: the fact we have no events on these means that actions won't have an event ID to go by; problem?
            InputState.Change(m_VirtualMouse.position, newPosition);
            InputState.Change(m_VirtualMouse.delta, delta);

            ProcessOverGameObject(UICursorOverRaycaster.LastPointerEventData);

            // Update software cursor transform, if any.
            if (m_cursor != null && isActive)
                m_cursor.RectTransform.position = newPosition;
        }

        // Update scroll wheel.
        var scrollAction = m_Scroll;
        if (scrollAction != null)
        {
            var scrollValue = scrollAction.ReadValue<Vector2>();
            scrollValue.x *= m_ScrollSpeed;
            scrollValue.y *= m_ScrollSpeed;

            InputState.Change(m_VirtualMouse.scroll, scrollValue);
        }
    }

    private void ProcessOverGameObject(PointerEventData pointerEvent)
    {
        //Debug.Log(PointerEventData.pointerCurrentRaycast);
        if (pointerEvent != null && EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            if (m_lastGameObjectOver != pointerEvent.pointerCurrentRaycast.gameObject)
            {

                var clickHandler = ExecuteEvents.GetEventHandler<IPointerDownHandler>(pointerEvent.pointerCurrentRaycast.gameObject);
                if (clickHandler == null)
                {
                    clickHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(pointerEvent.pointerCurrentRaycast.gameObject);
                }
                if (clickHandler != null)
                {
                    m_cursor.ClickableElement = pointerEvent.pointerCurrentRaycast.gameObject;
                }
                else
                {
                    m_cursor.ClickableElement = null;
                }
                m_cursor.OnClickableElementChanged();
                m_lastGameObjectOver = pointerEvent.pointerCurrentRaycast.gameObject;
            }
        }
        else
        {
            if (m_lastGameObjectOver != null)
            {
                m_cursor.ClickableElement = null;
                m_cursor.OnClickableElementChanged();
            }
            m_lastGameObjectOver = null;
        }
    }

    private void SendEventAction(InputAction.CallbackContext context)
    {
        if (m_VirtualMouse == null || !isActive)
            return;

        var action = context.action;

        MouseButton? button = null;
        if (action == m_Click)
        {
            button = MouseButton.Left;
        }

        if (button != null)
        {
            var isPressed = context.control.IsPressed();
            if (m_VirtualMouse.leftButton.isPressed != isPressed || context.canceled)
            {
                m_VirtualMouse.CopyState<MouseState>(out mouseState);
                mouseState.WithButton(button.Value, isPressed);

                InputState.Change(m_VirtualMouse, mouseState);

                //released
                if (m_cursor != null && !isPressed)
                {
                    m_cursor.OnClick();
                    //InputState.Change(m_VirtualMouse.position, cursorBehaviour.RectTransform.position);
                    //ProcessOverGameObject(UICursorOverRaycaster.LastPointerEventData);
                }
            }
        }
    }
}