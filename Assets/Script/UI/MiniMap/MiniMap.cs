using UnityEngine;
using UnityEngine.UI;
using RPGAE.CharacterController;
using System.Collections.Generic;
using System.Collections;

public class MiniMap : MonoBehaviour
{
    [Header("General Settings")]
    // Target for the minimap.
    public GameObject target;
    public Camera MMCamera = null;
    public Color playerColor;
    public Sprite PlayerIconSprite;

    [Header("World Map Settings")]
    public GameObject cursorPrefab;
    public GameObject MapPointerPrefab;
    public bool isMarkerOnMap;
    public bool AllowMapMarks = true;
    public bool AllowMultipleMarks = false;
    private Transform mapCursor;
    private GameObject mapPointer;

    [Header("Height")]
    [Range(0.05f, 2)] public float IconMultiplier = 1;
    [Tooltip("How much should we move for each small movement on the mouse wheel?")]
    [Range(1, 10)] public int scrollSensitivity = 3;
    //Default height to view from, if you need have a static height, just edit this.
    [Range(5, 500)]
    public float DefaultHeight = 30;
    [Tooltip("Maximum heights that the camera can reach.")]
    public float MaxZoom = 80;
    [Tooltip("Minimum heights that the camera can reach.")]
    public float MinZoom = 20;
    [Range(1, 15)]
    [Tooltip("Smooth speed to height change.")]
    public float LerpHeight = 8;

    [Header("Rotation")]
    [Tooltip("Compass rotation for circle maps, rotate icons around pivot.")]
    public bool useCompassRotation = false;
    [Range(25, 500)]
    [Tooltip("Size of Compass rotation diameter.")]
    public float CompassSize = 175f;
    [Range(1, 15)]
    public float LerpRotation = 8;

    [Header("Map Rect")]
    [Tooltip("Position for World Map.")]
    public Vector3 FullMapPosition = Vector2.zero;
    [Tooltip("Size of World Map.")]
    public Vector2 FullMapSize = Vector2.zero;
    private Vector3 MiniMapPosition = Vector2.zero;
    private Vector3 MiniMapRotation = Vector3.zero;
    private Vector2 MiniMapSize = Vector2.zero;
    public Vector2 MaxOffSetPosition = new Vector2(1000, 1000);

    [Space(5)]
    [Tooltip("Smooth Speed for MiniMap World Map transition.")]
    [Range(1, 15)]
    public float LerpTransition = 7;

    [Header("Compass Settings")]
    public RectTransform CompassRoot;
    public RectTransform North;
    public RectTransform South;
    public RectTransform East;
    public RectTransform West;
    [HideInInspector] public int Grade;
    private int Rotation;

    [Header("UI")]
    public GameObject iconPrefab;
    public Canvas m_Canvas = null;
    public RectTransform MiniMapUIRoot = null;
    public RectTransform IconsParent;
    public Image PlayerIcon = null;
    public FadeUI fadeUI;
    public FadeUI fadeUILegendInfo;

    //Global variables
    public bool isFullScreen;
    public static Camera MiniMapCamera = null;

    private bool goingToTarget;

    #region Private 

    private Vector3 defualtMapRot = Vector3.zero;
    private bool DefaultRotationCircle = false;
    private ThirdPersonController cc;
    private InventoryManager inventoryM;
    private RectTransform PlayerIconTransform;
    private MaskHelper maskHelper;

    #endregion

    /// <summary>
    /// 
    /// </summary>
    void Awake()
    {
        GetMiniMapSize();
        MiniMapCamera = MMCamera;
        defualtMapRot = transform.eulerAngles;
        DefaultRotationCircle = useCompassRotation;
        PlayerIcon.sprite = PlayerIconSprite;
        PlayerIcon.color = playerColor;

        // Create world map cursor
        GameObject MMCursor = Instantiate(cursorPrefab) as GameObject;
        MMCursor.GetComponent<MiniMapItem>().Size = 20;
        MMCursor.transform.position = Vector3.zero;
        mapCursor = MMCursor.transform;
        mapCursor.transform.position = target.transform.position;

        // Get Components
        PlayerIconTransform = PlayerIcon.GetComponent<RectTransform>();
        maskHelper = FindObjectOfType<MaskHelper>();
        cc = FindObjectOfType<ThirdPersonController>();
        inventoryM = FindObjectOfType<InventoryManager>();

        if (fadeUI != null) { fadeUI.FadeTransition(1, 0, 1); }
    }

    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        if (target == null || MMCamera == null || inventoryM.referencesObj.systemSection.activeInHierarchy)
            return;

        // Input controls for mini map/world map
        Inputs();
        // Cursor for world map.
        CursorControl();
        // Controlled that minimap follow the target
        PositionControl();
        // Apply rotation settings.
        RotationControl();
        // For minimap and world map control.
        MapSize();
        // Compaus cordinates on minimap.
        UpdateCompassRotation();
    }

    void CursorControl()
    {
        if (isFullScreen && inventoryM.referencesObj.mapSection && inventoryM.referencesObj.mapSection.activeInHierarchy)
        {
            if (goingToTarget) return;

            // Stop Player Movement.
            cc.canMove = false;

            // Make cursor noticeable on World Map.
            mapCursor.GetComponent<MiniMapItem>().Size = 30;

            // Cursor Input
            Vector3 cursorMovement = new Vector3(cc.rpgaeIM.PlayerControls.Movement.ReadValue<Vector2>().x, 0, cc.rpgaeIM.PlayerControls.Movement.ReadValue<Vector2>().y);
            mapCursor.transform.position += cursorMovement * 60 * Time.deltaTime;

            // Clamp cursor movements.
            mapCursor.transform.position = new Vector3(Mathf.Clamp(mapCursor.transform.position.x, -MaxOffSetPosition.x, MaxOffSetPosition.x),
            0, Mathf.Clamp(mapCursor.transform.position.z, -MaxOffSetPosition.y, MaxOffSetPosition.y));

            // Set Marker on map.
            if (cc.rpgaeIM.PlayerControls.Attack.triggered && fadeUILegendInfo.canvasGroup.alpha == 0)
            {
                SetPointMark(mapCursor.transform.position);
            }

            // Reset cursor position on player.
            if (cc.rpgaeIM.PlayerControls.LockOn.triggered)
                GoToTarget();
        }
        else
        {
            mapCursor.GetComponent<MiniMapItem>().Size = 0;
            mapCursor.transform.position = target.transform.position;
        }
    }

    /// <summary>
    /// Minimap follow the target.
    /// </summary>
    void PositionControl()
    {
        if (target == null)
            return;

        // Update the transformation of the camera as the target's position.
        Vector3 camPosition = mapCursor.transform.position;
        camPosition.x = mapCursor.transform.position.x;
        camPosition.z = mapCursor.transform.position.z;

        // Calculate player position
        if (target != null)
        {
            Vector3 pp = MMCamera.WorldToViewportPoint(target.transform.position);
            PlayerIconTransform.anchoredPosition = CalculateMiniMapPosition(pp, MiniMapUIRoot);
        }

        // For this, we add the predefined (but variable, see below) height var.
        camPosition.y = (MaxZoom + MinZoom * 0.5f) + (target.transform.position.y * 2);

        // Camera follow the target
        transform.position = Vector3.Lerp(transform.position, camPosition, Time.deltaTime * 10);
    }

    /// </summary>
    /// <param name="viewPoint"></param>
    /// <param name="maxAnchor"></param>
    /// <returns></returns>
    public static Vector3 CalculateMiniMapPosition(Vector3 viewPoint, RectTransform maxAnchor)
    {
        viewPoint = new Vector2((viewPoint.x * maxAnchor.sizeDelta.x) - (maxAnchor.sizeDelta.x * 0.5f),
            (viewPoint.y * maxAnchor.sizeDelta.y) - (maxAnchor.sizeDelta.y * 0.5f));

        return viewPoint;
    }

    /// <summary>
    /// 
    /// </summary>
    void RotationControl()
    {
        // If the minimap should rotate as the target does, the rotateWithTarget var should be true.
        // An extra catch because rotation with the full screen map is a bit weird.
        RectTransform rt = PlayerIcon.GetComponent<RectTransform>();

        transform.eulerAngles = defualtMapRot;
        //When map rotation is static, only rotate the player icon
        Vector3 e = Vector3.zero;
        //get and fix the correct angle rotation of target
        e.z = -target.transform.eulerAngles.y;
        rt.eulerAngles = e;
    }

    /// <summary>
    /// 
    /// </summary>
    void Inputs()
    {
        if (!isFullScreen) return;

        if (cc.rpgaeIM.PlayerControls.Action.triggered && inventoryM.referencesObj.mapSection.activeInHierarchy)
        {
            if (fadeUILegendInfo.canvasGroup.alpha == 0)
                fadeUILegendInfo.FadeTransition(1, 0, 0.25f);
            if (fadeUILegendInfo.canvasGroup.alpha == 1)
                fadeUILegendInfo.FadeTransition(0, 0, 0.25f);
        }

        // zoom in on world map
        if (cc.zoomValue < 0.1f && DefaultHeight < MaxZoom)
        {
            ChangeHeight(true);
        }
        // zoom out on world map
        if (cc.zoomValue > -0.1f && DefaultHeight > MinZoom)
        {
            ChangeHeight(false);
        }
    }

    /// <summary>
    /// Map FullScreen or MiniMap
    /// Lerp all transition for smooth effect.
    /// </summary>
    void MapSize()
    {
        float delta = Time.deltaTime;
        if (isFullScreen)
        {
            MiniMapUIRoot.sizeDelta = FullMapSize;
            MiniMapUIRoot.anchoredPosition = FullMapPosition;
        }
        else
        {
            MiniMapUIRoot.sizeDelta = MiniMapSize;
            MiniMapUIRoot.anchoredPosition = MiniMapPosition;
            MiniMapUIRoot.localEulerAngles = MiniMapRotation;
        }
        MMCamera.orthographicSize = Mathf.Lerp(MMCamera.orthographicSize, DefaultHeight, delta * LerpHeight);
    }

    /// <summary>
    /// This is called when you press the start button
    /// </summary>
    public void ToggleSize()
    {
        isFullScreen = !isFullScreen;
        if (isFullScreen)
        {
            fadeUI.canvasGroup.alpha = 0;
            //when change to full screen, the height is the max
            DefaultHeight = MaxZoom;

            useCompassRotation = false;
            if (maskHelper) { maskHelper.OnChange(true); }
        }
        else
        {
            if(fadeUILegendInfo.canvasGroup.alpha != 0)
            {
                fadeUILegendInfo.canvasGroup.alpha = 0;
                fadeUILegendInfo.isFading = false;
            }
            if (fadeUI.canvasGroup.alpha == 0) fadeUI.FadeTransition(1, 0, 1.3f);
            //when return of full screen, return to current height
            DefaultHeight = 30;

            if (useCompassRotation != DefaultRotationCircle) { useCompassRotation = DefaultRotationCircle; }
            if (maskHelper) { maskHelper.OnChange(); }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Position">world map position</param>
    /// /*
    public void SetPointMark(Vector3 Position)
    {
        if (!AllowMapMarks)
            return;

        if (!AllowMultipleMarks)
        {
            Destroy(mapPointer);
            isMarkerOnMap = false;
        }

        mapPointer = Instantiate(MapPointerPrefab, Position, Quaternion.identity) as GameObject;
        mapPointer.GetComponent<MapPointer>().isAudioActive = true;
        mapPointer.GetComponent<MapPointer>().SetColor(playerColor);
        isMarkerOnMap = true;
    }

    /// <summary>
    /// 
    /// </summary>
    public void GoToTarget()
    {
        StopCoroutine("ResetOffset");
        StartCoroutine("ResetOffset");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator ResetOffset()
    {
        goingToTarget = true;
        while (Vector3.Distance(mapCursor.transform.position, target.transform.position) > 1)
        {
            mapCursor.transform.position = Vector3.Lerp(mapCursor.transform.position, target.transform.position, Time.deltaTime * LerpTransition);
            yield return null;
        }
        mapCursor.transform.position = target.transform.position;
        goingToTarget = false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="b"></param>
    public void ChangeHeight(bool b)
    {
        if (b)
        {
            if (DefaultHeight + scrollSensitivity <= MaxZoom)
            {
                DefaultHeight += scrollSensitivity;
            }
            else
            {
                DefaultHeight = MaxZoom;
            }
        }
        else
        {
            if (DefaultHeight - scrollSensitivity >= MinZoom)
            {
                DefaultHeight -= scrollSensitivity;
            }
            else
            {
                DefaultHeight = MinZoom;
            }
        }
    }

    /// <summary>
    /// Reset this transform rotation helper.
    /// </summary>
    void ResetMapRotation()
    {
        transform.eulerAngles = new Vector3(90, 0, 0);
    }

    /// <summary>
    /// 
    /// </summary>
    void GetMiniMapSize()
    {
        MiniMapSize = MiniMapUIRoot.sizeDelta;
        MiniMapPosition = MiniMapUIRoot.anchoredPosition;
        MiniMapRotation = MiniMapUIRoot.eulerAngles;
    }

    void UpdateCompassRotation()
    {
        //return always positive
        if (target != null)
        {
            Rotation = (int)Mathf.Abs(target.transform.eulerAngles.y);
        }
        else
        {
            Rotation = (int)Mathf.Abs(transform.eulerAngles.y);
        }
        Rotation = Rotation % 360;//return to 0 

        Grade = Rotation;
        //opposite angle
        if (Grade > 180)
        {
            Grade = Grade - 360;
        }
        float cm = CompassRoot.sizeDelta.x * 0.5f;
        if (useCompassRotation)
        {
            Vector3 north = Vector3.forward * 1000;
            Vector3 tar = Camera.main.transform.forward;
            tar.y = 0;

            float n = angle3602(north, tar, target.transform.right);
            Vector3 rot = CompassRoot.eulerAngles;
            rot.z = -n;
            CompassRoot.eulerAngles = rot;
        }
    }

    float angle3602(Vector3 from, Vector3 to, Vector3 right)
    {
        float angle = Vector3.Angle(from, to);
        Vector3 cross = Vector3.Cross(from, to);
        if (cross.y < 0) { angle = -angle; }
        return angle;
    }

    public float Angle360(Vector2 p1, Vector2 p2, Vector2 o = default(Vector2))
    {
        Vector2 v1, v2;
        if (o == default(Vector2))
        {
            v1 = p1.normalized;
            v2 = p2.normalized;
        }
        else
        {
            v1 = (p1 - o).normalized;
            v2 = (p2 - o).normalized;
        }
        float angle = Vector2.Angle(v1, v2);
        return Mathf.Sign(Vector3.Cross(v1, v2).z) < 0 ? (360 - angle) % 360 : angle;
    }
}