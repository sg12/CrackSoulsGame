using UnityEngine;
using RPGAE.CharacterController;

public class NPCDamageHandler : MonoBehaviour, DamageReceiver
{
    private HealthPoints healthPoints;
    private AIController controller;
    private Animator animator; // Добавляем ссылку на аниматор
    private bool isDead = false; // Флаг для предотвращения повторного вызова смерти

    void Start()
    {
        // Получаем компонент здоровья
        healthPoints = GetComponent<HealthPoints>();
        if (healthPoints == null)
        {
            Debug.LogError("На NPC отсутствует компонент HealthPoints");
        }

        // Получаем компонент аниматора
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("На NPC отсутствует компонент Animator");
        }
    }

    // Реализация интерфейса DamageReceiver
    public void OnReceiveMessage(MsgType type, object sender, object msg)
    {
        // Обработка сообщения в зависимости от типа
        switch (type)
        {
            case MsgType.DEAD:
                HandleDeath((HealthPoints.DamageData)msg);
                break;

            case MsgType.DAMAGED:
                HandleDamage((HealthPoints.DamageData)msg);
                break;
        }
    }

    private void HandleDamage(HealthPoints.DamageData damageData)
    {
        if (healthPoints != null && healthPoints.curHealthPoints > 0)
        {
            Debug.Log($"NPC получил урон: {damageData.wpnAtk}");
            // Дополнительная логика при получении урона
        }
    }

    private void HandleDeath(HealthPoints.DamageData damageData)
    {
        if (isDead) return; // Если NPC уже мертв, пропускаем

        Debug.Log("NPC умер.");
        isDead = true; // Ставим флаг смерти

        // Проверяем, есть ли аниматор, и включаем анимацию смерти
        if (animator != null)
        {
            animator.SetTrigger("Death"); // Включаем триггер "Death", чтобы воспроизвести анимацию смерти
        }

        // Дополнительная логика при смерти (например, отключение коллайдеров)
        DisableNPC();
    }

    private void DisableNPC()
    {
        // Отключаем коллайдеры или логику движения NPC после смерти
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (var col in colliders)
        {
            col.enabled = false;
        }

        // Можно добавить логику удаления объекта через определенное время:
        // Destroy(gameObject, 5f); // Уничтожение объекта через 5 секунд после смерти
    }
}
