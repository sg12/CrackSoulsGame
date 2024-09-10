using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MiniMapItem : MonoBehaviour
{
    [Header("TARGET")]
    [Tooltip("Transform to UI Icon will be follow")]
    public Transform Target = null;
    [Tooltip("Custom Position from target position")]
    public Vector3 OffSet = Vector3.zero;

    [Header("ICON")]
    AnimatorClipInfo[] m_CurrentClipInfo;
    public Sprite Icon = null;
    public Sprite DeathIcon = null;
    public Color IconColor = new Color(1, 1, 1, 0.9f);
    [Range(1, 100)] public float Size = 20;

    [Header("CIRCLE AREA")]
    public bool ShowCircleArea = false;
    [Range(1, 100)] public float CircleAreaRadius = 10;
    public Color CircleAreaColor = new Color(1, 1, 1, 0.9f);

    [Header("SETTINGS")]
    [Tooltip("Can Icon show when is off screen?")]
    public bool showOnlyOnWorldMap;
    public bool OffScreen = true;
    public bool DestroyWithObject = true;
    [Range(0, 5)] public float BorderOffScreen = 0.01f;
    [Range(1, 50)] public float OffScreenSize = 10;
    [Tooltip("Time before render/show item in minimap after instance")]
    [Range(0, 3)] public float RenderDelay = 0.3f;
    public ItemEffect m_Effect = ItemEffect.None;
    public GameObject iconPrefab;

    #region Private 

    public Image Graphic = null;
    private RectTransform GraphicRect;
    private RectTransform RectRoot;
    private GameObject cacheItem = null;
    private RectTransform CircleAreaRect = null;
    private Vector3 position;
    private MiniMap MiniMap;
    Vector3 screenPos = Vector3.zero;

    #endregion

    /// <summary>
    /// Get all required component in start
    /// </summary>
    void Start()
    {
        MiniMap = FindObjectOfType<MiniMap>();
        if (MiniMap != null)
        {
            CreateIcon();
        }
        else { Debug.Log("You need a MiniMap in scene for use MiniMap Items."); }

        if (GetComponent<QuestWayPoint>() != null)
            Size = 1;
    }

    /// <summary>
    /// 
    /// </summary>
    void CreateIcon()
    {
        if (!this.enabled) return;
        //Instantiate UI in canvas
        cacheItem = Instantiate(iconPrefab) as GameObject;
        RectRoot = OffScreen ? MiniMap.MiniMapUIRoot : MiniMap.IconsParent;
        // SetUp Icon UI
        Graphic = cacheItem.GetComponent<Image>();
        GraphicRect = Graphic.GetComponent<RectTransform>();
        if (Icon != null) { Graphic.sprite = Icon; Graphic.color = IconColor; }
        cacheItem.transform.SetParent(RectRoot.transform, false);
        GraphicRect.anchoredPosition = Vector2.zero;
        if (Target == null) { Target = this.GetComponent<Transform>(); }
        StartEffect();
        IconItem ii = cacheItem.GetComponent<IconItem>();
        ii.DelayStart(RenderDelay);
        ii.showOnlyOnWorldMap = showOnlyOnWorldMap;
        if (ShowCircleArea)
        {
            CircleAreaRect = ii.SetCircleArea(CircleAreaRadius, CircleAreaColor);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        //If a component missing, return for avoid bugs.
        if (Target == null)
            return;
        if (Graphic == null)
            return;

        //Setting the modify position
        Vector3 CorrectPosition = TargetPosition + OffSet;
        Vector2 fullSize = RectRoot.rect.size;
        Vector2 size = RectRoot.rect.size * 0.5f;
        //Convert the position of target in ViewPortPoint
        Vector2 wvp = MiniMap.MiniMapCamera.WorldToViewportPoint(CorrectPosition);
        //Calculate the position of target and convert into position of screen
        position = new Vector2((wvp.x * fullSize.x) - size.x, (wvp.y * fullSize.y) - size.y);

        Vector2 UnClampPosition = position;
        //if show off screen
        if (OffScreen)
        {
            //Calculate the max and min distance to move the UI
            //this clamp in the RectRoot sizeDela for border
            position.x = Mathf.Clamp(position.x, -(size.x - BorderOffScreen), (size.x - BorderOffScreen));
            position.y = Mathf.Clamp(position.y, -size.y - BorderOffScreen, (size.y - BorderOffScreen));
        }

        //calculate the position of UI again, determine if off screen
        //if off screen reduce the size
        float Iconsize = Size;
        //Use this (useCompassRotation when have a circle miniMap)
        if (m_miniMap.useCompassRotation)
        {
            //Calculate difference
            Vector3 forward = Target.position - m_miniMap.target.transform.position;
            //Position of target from camera
            Vector3 cameraRelativeDir = MiniMap.MiniMapCamera.transform.InverseTransformDirection(forward);
            //normalize values for screen fix
            cameraRelativeDir.z = 0;
            cameraRelativeDir = cameraRelativeDir.normalized * 0.5f;
            //Convert values to positive for calculate area OnScreen and OffScreen.
            float posPositiveX = Mathf.Abs(position.x);
            float relativePositiveX = Mathf.Abs((0.5f + (cameraRelativeDir.x * m_miniMap.CompassSize)));
            //when target if offScreen clamp position in circle area.
            if (posPositiveX >= relativePositiveX)
            {
                screenPos.x = 0.5f + (cameraRelativeDir.x * m_miniMap.CompassSize)/*/ Camera.main.aspect*/;
                screenPos.y = 0.5f + (cameraRelativeDir.y * m_miniMap.CompassSize);
                position = screenPos;
                Iconsize = OffScreenSize;
            }
            else
            {
                Iconsize = Size;
            }
        }
        else
        {
            if (position.x == size.x - BorderOffScreen || position.y == size.y - BorderOffScreen ||
                position.x == -size.x - BorderOffScreen || -position.y == size.y - BorderOffScreen)
            {
                Iconsize = OffScreenSize;
            }
            else
            {
                Iconsize = Size;
            }
        }

        //Apply position to the UI (for follow)
        GraphicRect.anchoredPosition = position;
        if (CircleAreaRect != null) { CircleAreaRect.anchoredPosition = UnClampPosition; }
        //Change size with smooth transition
        float CorrectSize = Iconsize * MiniMap.IconMultiplier;
        GraphicRect.sizeDelta = Vector2.Lerp(GraphicRect.sizeDelta, new Vector2(CorrectSize, CorrectSize), Time.deltaTime * 8);

        //with this the icon rotation always will be the same (for front)
        Quaternion r = Quaternion.identity;
        r.x = Target.rotation.x;
        GraphicRect.localRotation = r;
    }

    /// <summary>
    /// 
    /// </summary>
    void StartEffect()
    {
        Animator a = Graphic.GetComponent<Animator>();
        if (m_Effect == ItemEffect.Pulsing)
        {
            a.SetInteger("Type", 2);
        }
        else if (m_Effect == ItemEffect.Fade)
        {
            a.SetInteger("Type", 1);
        }
    }

    /// <summary>
    /// When player or the target die,desactive,remove,etc..
    /// call this for remove the item UI from Map
    /// for change to other icon and desactive in certain time
    /// or destroy immediate
    /// </summary>
    /// <param name="inmediate"></param>
    public void DestroyItem(bool inmediate)
    {
        if (Graphic == null)
        {
            return;
        }

        if (DeathIcon == null || inmediate)
        {
            Graphic.GetComponent<IconItem>().DestroyIcon(inmediate);
        }
        else

        {
            Graphic.GetComponent<IconItem>().DestroyIcon(inmediate, DeathIcon);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ico"></param>
    public void SetIcon(Sprite ico)
    {
        if (cacheItem == null)
        {
            Debug.LogWarning("You can't set a icon before create the item.");
            return;
        }

        cacheItem.GetComponent<IconItem>().SetIcon(ico);
    }

    /// <summary>
    /// Show a visible circle area in the minimap with this
    /// item as center
    /// </summary>
    public void SetCircleArea(float radius, Color AreaColor)
    {
        CircleAreaRect = cacheItem.GetComponent<IconItem>().SetCircleArea(radius, AreaColor);
    }

    /// <summary>
    /// 
    /// </summary>
    public void HideCircleArea()
    {
        if (cacheItem)
            cacheItem.GetComponent<IconItem>().HideCircleArea();

        CircleAreaRect = null;
    }

    /// <summary>
    /// Call this for hide item in miniMap
    /// For show again just call "ShowItem()"
    /// NOTE: For destroy item call "DestroyItem(bool immediate)" instant this.
    /// </summary>
    public void HideItem()
    {
        if (cacheItem != null)
        {
            cacheItem.SetActive(false);
        }
        else
        {
            Debug.Log("There is no item to disable.");
        }
    }

    /// <summary>
    /// Call this for show again the item in miniMap when is hide
    /// </summary>
    public void ShowItem()
    {
        if (cacheItem != null)
        {
            cacheItem.SetActive(true);
            cacheItem.GetComponent<IconItem>().SetVisibleAlpha();
        }
        else
        {
            Debug.Log("There is no item to active.");
        }
    }

    /// <summary>
    /// If you need destroy icon when this gameObject is destroy.
    /// </summary>
    void OnDestroy()
    {
        if (DestroyWithObject)
        {
            DestroyItem(true);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public Vector3 TargetPosition
    {
        get
        {
            if (Target == null)
            {
                return Vector3.zero;
            }

            return new Vector3(Target.position.x, 0, Target.position.z);
        }
    }

    private MiniMap _minimap = null;
    private MiniMap m_miniMap
    {
        get
        {
            if (_minimap == null)
            {
                _minimap = this.cacheItem.transform.root.GetComponentInChildren<MiniMap>();
            }
            return _minimap;
        }
    }
}