using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public enum CursorType
    {
        Default,
        Ground,
        Enemy,
        Bot,
        Environment,
        Item
    }
    
    [System.Serializable]
    public class CursorMapping
    {
        public CursorType type;
        public Texture2D texture;
        public Vector2 hotspot = Vector2.zero;
        public CursorMode cursorMode = CursorMode.Auto;
    }
    
    [Header("CURSORS")]
    [SerializeField] private List<CursorMapping> cursorMappings = new List<CursorMapping>();

    [Header("LAYERS")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask botLayer;

    private Texture2D currentCursor;
    private Dictionary<CursorType, CursorMapping> cursorDict;
  
    private void Awake()
    {
        cursorDict = new Dictionary<CursorType, CursorMapping>();
        foreach (var mapping in cursorMappings)
        {
            if (!cursorDict.ContainsKey(mapping.type))
            {
                //масштабируем текстуры
                float dpi = Screen.dpi;
                if (dpi == 0) dpi = 96;
                float scaleFactor = dpi / 96;
                scaleFactor = Mathf.Clamp(scaleFactor, 0.5f, 1.5f);

                //сохраняем отмасштабируемые
                mapping.texture = ScaleTexture(mapping.texture, scaleFactor);
                cursorDict.Add(mapping.type, mapping);
            }
            else
            {
                Debug.LogWarning($"Duplicate CursorType detected: {mapping.type}. Only the first occurrence will be used.");
            }
        }
    }

    private void Start()
    {
        SetCursor(CursorType.Default);
    }

    private void Update()
    {
        UpdateCursor();
    }

    private void UpdateCursor()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            SetCursor(CursorType.Default);
            return;
        }
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            CursorType newCursorType = CursorType.Default;
            GameObject hitObject = hit.collider.gameObject;
            int hitLayer = hitObject.layer;
            
            //проверка на врага
            if (((1 << hitLayer) & enemyLayer) != 0)
            {
                newCursorType = CursorType.Enemy;
            }
            //проверка на землю
            else if (((1 << hitLayer) & groundLayer) != 0)
            {
                newCursorType = CursorType.Ground;
            }
            //проверка на бота
            else if (((1 << hitLayer) & botLayer) != 0)
            {
                newCursorType = CursorType.Bot;
            }
            //проверка на окружение
            else if (hitObject.CompareTag("Environment"))
            {
                newCursorType = CursorType.Environment;
            }
            //проверка на предмет
            else if (hitObject.CompareTag("Item"))
            {
                newCursorType = CursorType.Item;
            }

            SetCursor(newCursorType);
        }
        else
        {
            SetCursor(CursorType.Default);
        }
        
    }
    
    private void SetCursor(CursorType type)
    {
        if (cursorDict.TryGetValue(type, out CursorMapping mapping))
        {
            if (currentCursor != mapping.texture)
            {
                Cursor.SetCursor(mapping.texture, mapping.hotspot, mapping.cursorMode);
                currentCursor = mapping.texture;
            }
        }
        else
        {
            Debug.LogWarning($"No cursor mapping found for CursorType: {type}. Using default cursor.");
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            currentCursor = null;
        }
    }


    //метод изменяет размеры текстур курсоров
    private Texture2D ScaleTexture(Texture2D source, float scaleFactor)
    {
        int width = Mathf.RoundToInt(source.width * scaleFactor);
        int height = Mathf.RoundToInt(source.height * scaleFactor);
    
        Texture2D result = new Texture2D(width, height, source.format, false);
    
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color newColor = source.GetPixelBilinear((float)x / width, (float)y / height);
                result.SetPixel(x, y, newColor);
            }
        }
    
        result.Apply();
        return result;
    }


}
