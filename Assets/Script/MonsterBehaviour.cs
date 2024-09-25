using UnityEngine;
public class MonsterBehaviour : MonoBehaviour
{
    [Header("SCANNER SETTINGS")]
    public TargetScanner targetScanner;
    public float followStoppingDistance; 
    public GameObject enemyTarget;
    public float timeToStopPursuit = 6f;

    private float distanceFromTarget;
    private float maxAtkDistance = 5f;
    private float atkRecoveryTimer;
    private float atkRecoveryRate = 2f;
    private bool isAttacking;
   
    private State state;
    public float maxDetectionRadius = 10f;
    private float distanceToStopTracking = 15f; // Дистанция, после которой персонаж прекратит отслеживание
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
                    state = State.Pursuit; // Переход в режим преследования
                    controller.animator.SetTrigger("Run"); // Включаем анимацию бега
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

    void StopTrackingTarget()
    {
        // Прекращаем отслеживание цели
        enemyTarget = null;
        targetScanner.detectionRadius -= targetScanner.detectionRadiusWhenSpotted;
        state = State.Idle; // Возвращаем в режим ожидания
        controller.animator.SetTrigger("Idle"); // Включаем анимацию стояния на месте
    }

    #endregion

    #region Idle
    private void IdleState()
    {
        controller.animator.SetFloat("Vertical", 0); // Останавливаем движение в Idle
    }
    #endregion

    #region Pursuit
    public void PursuitState()
    {
        if (enemyTarget == null)
        {
            state = State.Idle; // Если цели нет, возвращаемся в состояние ожидания
            controller.animator.SetTrigger("Idle");
            return;
        }

        distanceFromTarget = Vector3.Distance(enemyTarget.transform.position, transform.position);

        if (distanceFromTarget <= followStoppingDistance)
        {
            state = State.Attack; // Переход в состояние атаки
            controller.animator.SetTrigger("Attack"); // Включаем анимацию атаки
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

        if (!controller.animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
        {
            controller.animator.SetTrigger("Run"); // Включаем анимацию бега
        }
    }
    #endregion

    #region Attack
    private void AttackState()
    {
        if (enemyTarget == null) return;

        distanceFromTarget = Vector3.Distance(enemyTarget.transform.position, transform.position);

        if (distanceFromTarget > maxAtkDistance)
        {
            state = State.Pursuit; // Если цель далеко, продолжаем преследование
            controller.animator.SetTrigger("Run");
        }
        else
        {
            if (!isAttacking)
            {
                controller.animator.SetTrigger("Attack"); // Включаем анимацию атаки
                isAttacking = true;
            }
        }
    }
    #endregion

    #region Death
    public void DeathState()
    {
        controller.animator.SetTrigger("Death"); // Включаем анимацию смерти
        controller.navmeshAgent.enabled = false; // Отключаем навигацию
        // Дополнительная логика, если нужна (например, уничтожение объекта)
    }
    #endregion

    #region Attack Recovery
    void AttackRecoveryTimer()
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

    //отрисовка сканнера на сцене
    private void OnDrawGizmosSelected()
    {
        targetScanner.EditorGizmo(transform);
    }
}
