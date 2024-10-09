using UnityEngine.EventSystems;
using UnityEngine;

public class CursorManager : MonoBehaviour
{   
    [Header("CURSORS")]
    [SerializeField] private Texture2D _defaultCursor;
    [SerializeField] private Texture2D _groundCursor;
    [SerializeField] private Texture2D _enemyCursor;
    [SerializeField] private Texture2D _environmentCursor;
    [SerializeField] private Texture2D _itemSelectionCursor;
   
    [Header("LAYERS")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _enemyLayer; 
  
    //[SerializeField] private LayerMask _envoirnmentLayer;

    private void Start()
    {
        Cursor.SetCursor(_defaultCursor, Vector2.zero, CursorMode.Auto);
    }

    private void Update()
    {
        //проверка для установления дефолтного курсора на Canvas
        if (EventSystem.current.IsPointerOverGameObject())
        {
            Cursor.SetCursor(_defaultCursor, Vector2.zero, CursorMode.Auto);
            return;
        }
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        { 
            //проверка на слой врага
            if (((1 << hit.collider.gameObject.layer) & _enemyLayer) != 0)
            {
                Cursor.SetCursor(_enemyCursor, Vector2.zero, CursorMode.Auto);
            }
            //проверка на слой земли
            else if (((1 << hit.collider.gameObject.layer) & _groundLayer) != 0)
            {
                Cursor.SetCursor(_groundCursor, Vector2.zero, CursorMode.Auto);
            }
            
            //проверка на окружение
            else if (hit.collider.gameObject.CompareTag("Environment"))
            {
                Cursor.SetCursor(_environmentCursor, Vector2.zero, CursorMode.Auto);
            }
            //проверка на предметы
            else if (hit.collider.gameObject.CompareTag("Item"))
            {
                Cursor.SetCursor(_itemSelectionCursor, Vector2.zero, CursorMode.Auto);
            }
           
        }
        else
        {
            Cursor.SetCursor(_defaultCursor, Vector2.zero, CursorMode.Auto);
        }
        

    }
}
