using UnityEngine.EventSystems;
using UnityEngine;

public class CursorManager : MonoBehaviour
{   
    [Header("CURSORS")]
    [SerializeField] private Texture2D _defaultCursor;
    [SerializeField] private Texture2D _groundCursor;
    [SerializeField] private Texture2D _enemyCursor;
    [SerializeField] private Texture2D _itemSelectionCursor;
    [Header("ENVIRONMENT FIELD FOR ANY OBJECTS")]
    [SerializeField] private Texture2D _environmentCursor;  //необходимо продумать какой курсор показывать
                                                            //при наведении на какой-либо предмет
                                                            //окружения(дом, деревья и тд)
    [Header("LAYERS")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _enemyLayer; 
  
    private Texture2D _currentCursor;

    private void Start()
    {
        _currentCursor = _defaultCursor;
        Cursor.SetCursor(_currentCursor, Vector2.zero, CursorMode.Auto);
    }

    private void Update()
    {
        Texture2D newCursor = _defaultCursor;
        
        // Проверка для установления дефолтного курсора на Canvas
        if (EventSystem.current.IsPointerOverGameObject())
        {
            newCursor = _defaultCursor;
        }
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            { 
                GameObject hitObject = hit.collider.gameObject;
                int hitLayer = hitObject.layer;

                // Проверка на слой врага
                if (((1 << hitLayer) & _enemyLayer) != 0)
                {
                    newCursor = _enemyCursor;
                }
                // Проверка на слой земли
                else if (((1 << hitLayer) & _groundLayer) != 0)
                {
                    newCursor = _groundCursor;
                }
                // Проверка на окружение
                else if (hitObject.CompareTag("Environment"))
                {
                    newCursor = _environmentCursor;
                }
                // Проверка на предметы
                else if (hitObject.CompareTag("Item"))
                {
                    newCursor = _itemSelectionCursor;
                }
                else
                {
                    newCursor = _defaultCursor;
                }
            }
            else
            {
                newCursor = _defaultCursor;
            }
        }

        // Обновляем курсор только если он изменился
        if (_currentCursor != newCursor)
        {
            Cursor.SetCursor(newCursor, Vector2.zero, CursorMode.Auto);
            _currentCursor = newCursor;
        }
    }
}
