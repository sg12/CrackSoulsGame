using UnityEngine;

public class NPCDamageHandler : MonoBehaviour, DamageReceiver
{
    private HealthPoints healthPoints;
    private AIController controller;
    private Animator animator; 
    private bool isDead = false; 
    
    private void Start()
    {
        healthPoints = GetComponent<HealthPoints>();
        if (healthPoints == null)
        {
            Debug.LogError("На NPC отсутствует компонент HealthPoints");
        }
        
        animator = GetComponent<Animator>();
        
        if (animator == null)
        {
            Debug.LogError("На NPC отсутствует компонент Animator");
        }
    }
    
    #region Message Handling
    
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
    #endregion

    #region Damage and Death Handling
    private void HandleDamage(HealthPoints.DamageData damageData)
    {
        if (healthPoints != null && healthPoints.curHealthPoints > 0)
        {
            Debug.Log($"NPC получил урон: {damageData.wpnAtk}");

            PlayHitAnimation();
        }
    }

    private void HandleDeath(HealthPoints.DamageData damageData)
    {
        if (isDead) return; // Если NPC уже мертв, пропускаем

        Debug.Log("NPC умер.");
        isDead = true;

        PlayDeathAnimation();
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
    #endregion
    
    #region Animations
    private void PlayHitAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }
    }
    
    private void PlayDeathAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Death"); 
        }
    }
    
    #endregion
}
