using UnityEngine;
public class MonsterBehaviour : MonoBehaviour
{
    [Header("SCANNER SETTINGS")]
    public TargetScanner targetScanner;
    public float followStoppingDistance; 
    public GameObject enemyTarget;
    //public float timeToStopPursuit = 6f;

    private float distanceFromTarget;
    private float maxAtkDistance = 5f;
    private float atkRecoveryTimer;
    private float atkRecoveryRate = 2f;
    private bool isAttacking;
   
    private State state; 
    // public float maxDetectionRadius = 10f;
    private float distanceToStopTracking = 15f;
    private AIController controller;
    
    private enum State
    {
        Idle,
        Pursuit,
        Attack,
        Death
    }

    private void Start()
    {
        controller = GetComponent<AIController>();

        if (controller == null)
        {
            Debug.LogError("AIController не найден на объекте.");
        }

        state = State.Idle; 
    }

    private void Update()
    {
        if (controller == null)
        {
            Debug.LogError("AIController не установлен.");
            return;
        }

        if (state != State.Death) 
        {
            FindTarget();
        }

        switch (state)
        {
            case State.Idle:
                IdleState();
                break;
            case State.Pursuit:
                PursuitState();
                break;
            case State.Attack:
                AttackState();
                break;
            case State.Death:
                DeathState();
                break;
        }

        AttackRecoveryTimer();
    }

    #region Find Target

    public void FindTarget()
    {
        // Обнаружение цели
        GameObject targetScanned = targetScanner.Detect(transform, enemyTarget == null);

        if (enemyTarget == null)
        {
            if (targetScanned != null)
            {
                HealthPoints targetHealth = targetScanned.GetComponent<HealthPoints>();
                if (targetHealth != null && targetHealth.curHealthPoints > 0)
                {
                    enemyTarget = targetHealth.gameObject;
                    targetScanner.detectionRadius += targetScanner.detectionRadiusWhenSpotted;
                    state = State.Pursuit;
                    controller.animator.SetTrigger("Run"); 
                }
            }
        }
        else
        {
            // Если цель была найдена ранее, проверяем её состояние
            if (enemyTarget.GetComponent<HealthPoints>().curHealthPoints <= 0)
            {
                StopTrackingTarget();
            }
            else
            {
                float distanceToTarget = Vector3.Distance(enemyTarget.transform.position, transform.position);

                // Прекращаем отслеживание, если цель слишком далеко
                if (distanceToTarget > distanceToStopTracking)
                {
                    StopTrackingTarget();
                }
            }
        }
    }

    private void StopTrackingTarget()
    {
        Debug.Log("StopTrackingTarget");
        enemyTarget = null;
        targetScanner.detectionRadius -= targetScanner.detectionRadiusWhenSpotted;
        state = State.Idle;
        controller.animator.SetTrigger("Idle");
    }

    #endregion

    #region Idle
    private void IdleState()
    {
        //controller.animator.SetFloat("Vertical", 0);
        controller.animator.SetTrigger("Idle");
    }
    #endregion

    #region Pursuit
    public void PursuitState()
    {
        if (enemyTarget == null)
        {
            state = State.Idle;
            controller.animator.SetTrigger("Idle");
            return;
        }

        distanceFromTarget = Vector3.Distance(enemyTarget.transform.position, transform.position);

        if (distanceFromTarget <= followStoppingDistance)
        {
            Debug.Log("Attack");
            state = State.Attack; 
            controller.animator.SetTrigger("Attack");
            controller.LookAtTarget(enemyTarget.transform);
        }
        else
        {
            ChaseTarget();
        }
    }

    public void ChaseTarget()
    {
        if (controller == null || enemyTarget == null) return;

        distanceFromTarget = Vector3.Distance(enemyTarget.transform.position, transform.position);
        controller.SetDestination(enemyTarget.transform.position);
        controller.LookAtTarget(enemyTarget.transform);
        if (!controller.animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
        {
            controller.animator.SetTrigger("Run");
        }
    }
    #endregion

    #region Attack
    private void AttackState()
    {
        if (enemyTarget == null) return;

        distanceFromTarget = Vector3.Distance(enemyTarget.transform.position, transform.position);
        
        // Проверяем, жив ли NPC
        HealthPoints targetHealth = enemyTarget.GetComponent<HealthPoints>();
        if (targetHealth != null && targetHealth.curHealthPoints <= 0)
        {
            StopTrackingTarget(); // Останавливаем атаку и отслеживание, если NPC мертв
            return;
        }

        if (distanceFromTarget > maxAtkDistance)
        {
            state = State.Pursuit;
            controller.animator.SetTrigger("Run");
            controller.LookAtTarget(enemyTarget.transform);
        }
        else
        {
            if (!isAttacking)
            {
                controller.animator.SetTrigger("Attack");
                ApplyDamageToTarget(); // Применяем урон
                isAttacking = true;
                controller.LookAtTarget(enemyTarget.transform);
            }
        }
    }

    private void ApplyDamageToTarget()
    {
        if (enemyTarget != null)
        {
            HealthPoints targetHealth = enemyTarget.GetComponent<HealthPoints>();

            // Проверяем, жив ли NPC перед нанесением урона
            if (targetHealth != null && targetHealth.curHealthPoints > 0)
            {
                HealthPoints.DamageData damageData = new HealthPoints.DamageData
                {
                    damager = this,
                    damageSource = transform.position,
                    wpnAtk = 20
                };
                targetHealth.ApplyDamage(damageData);
                Debug.Log("Нанесен урон NPC.");
            }
            else
            {
                // Если NPC мертв, прекращаем атаку и переходим в Idle
                StopTrackingTarget();
            }
        }
    }
    
    #endregion

    #region Death
    public void DeathState()
    {
        controller.animator.SetTrigger("Death"); 
        controller.navmeshAgent.enabled = false; // Отключаем навигацию
    }
    #endregion

    #region Attack Recovery
    private void AttackRecoveryTimer()
    {
        if (isAttacking)
        {
            atkRecoveryTimer += Time.deltaTime;

            if (atkRecoveryTimer >= atkRecoveryRate)
            {
                atkRecoveryTimer = 0f;
                isAttacking = false;
                state = State.Pursuit; // Возвращаемся в преследование после атаки
            }
        }
    }
    #endregion

    // Отрисовываем сканнер на сцене
    private void OnDrawGizmosSelected()
    {
        targetScanner.EditorGizmo(transform);
    }
}
