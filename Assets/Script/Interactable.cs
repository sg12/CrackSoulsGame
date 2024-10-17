using UnityEngine;
public enum InteractableType { Enemy, Item }

public class Interactable : MonoBehaviour
{
    public InteractableType interactionType;

    private void Awake() 
    {
        if(interactionType == InteractableType.Enemy)
        { 
            // Дополнительная инициализация для врага
        }
    }

    public void InteractWithItem()
    {
        // Логика взаимодействия с предметом
        Destroy(gameObject);
    }
}