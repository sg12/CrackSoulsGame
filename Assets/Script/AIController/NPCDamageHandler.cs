using UnityEngine;
using RPGAE.CharacterController;

public class NPCDamageHandler : MonoBehaviour, DamageReceiver
{
    private HealthPoints healthPoints;

    void Start()
    {
        // Получаем компонент здоровья
        healthPoints = GetComponent<HealthPoints>();
        if (healthPoints == null)
        {
            Debug.LogError("На NPC отсутствует компонент HealthPoints");
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
        Debug.Log("NPC умер.");
        // Логика при смерти (например, анимация смерти, удаление объекта)
    }
}