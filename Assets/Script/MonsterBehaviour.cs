using System;
using UnityEngine;

public class MonsterBehaviour : MonoBehaviour
{
    [Header("SCANNER SETTINGS")]
    public TargetScanner targetScanner;
    public float followStoppingDistance; // Расстояние, при котором персонаж перестает преследовать цель
    public GameObject enemyTarget;
    public Transform followCompanion; // Компаньон, за которым будет следовать NPC
    public float timeToStopPursuit = 6f;
    
    private float m_TimerSinceLostTarget = 0.0f;
    private Vector3 originalPosition;
    private float distanceFromTarget;
    private float pursuitStoppingDistance = 3f;
    private float maxAtkDistance = 5f;
    private float minAtkDistance = 1f;
    private float atkRecoveryTimer;
    private float atkRecoveryRate = 2f;
    private bool isAttacking;
    private bool battleStanceDecisionMade = false;
    private float signedAngle; // Угол для управления поворотом NPC
    private State state;
    private AIController controller;

    private enum State
    {
        Pursuit,
        CombatStance,
        WalkBackToBase,
        FollowCompanion,
        Stunned
    }

    void Start()
    {
        originalPosition = transform.position;
        controller = GetComponent<AIController>();

        if (controller == null)
        {
            Debug.LogError("AIController не найден на объекте.");
        }
    }

    private void Update()
    {
        if (controller == null)
        {
            Debug.LogError("AIController не установлен.");
            return;
        }

        FindTarget();

        switch (state)
        {
            case State.Pursuit:
                PursuitState();
                break;
            case State.CombatStance:
                CombatStanceState();
                break;
            case State.WalkBackToBase:
                WalkBackToBaseState();
                break;
            case State.FollowCompanion:
                FollowCompanionState();
                break;
            case State.Stunned:
                // Логика оглушения
                break;
        }

        AttackRecoveryTimer();
    }

    /*public void FindTarget()
    {
        GameObject targetScanned = targetScanner.Detect(transform, enemyTarget == null);

        if (enemyTarget == null && targetScanned != null)
        {
            HealthPoints distributor = targetScanned.GetComponent<HealthPoints>();
            if (distributor != null && distributor.curHealthPoints > 0)
            {
                enemyTarget = distributor.gameObject;
                targetScanner.detectionRadius += targetScanner.detectionRadiusWhenSpotted;
                state = State.Pursuit;
            }
        }
        else if (enemyTarget != null)
        {
            PursuitState();
        }
    }*/
    
     public void FindTarget()
    {
        // we ignore height difference if the target was already seen
        GameObject targetScanned = targetScanner.Detect(transform, enemyTarget == null);

        if (enemyTarget == null)
        {
            // we just saw the player for the first time, pick an empty spot to target around them
            if (targetScanned != null)
            {
                HealthPoints distributor = targetScanned.GetComponent<HealthPoints>();
                if (distributor != null && distributor.curHealthPoints > 0)
                {
                    enemyTarget = distributor.gameObject;
                    targetScanner.detectionRadius += targetScanner.detectionRadiusWhenSpotted;
                }
            }
        }
        else
        {
            if (enemyTarget.GetComponent<HealthPoints>().curHealthPoints <= 0)
            {
                controller.animator.SetBool("Patrol", true);
                controller.animator.SetBool("Pursuit", false);

                if (followCompanion != null)
                {
                    state = State.FollowCompanion;

                    enemyTarget = null;
                    targetScanner.detectionRadius -= targetScanner.detectionRadiusWhenSpotted;
                }
                else
                {
                    Vector3 toOriginPosition = originalPosition - transform.position;
                    toOriginPosition.y = 0;
                    if (toOriginPosition.sqrMagnitude > 1)
                        state = State.WalkBackToBase;

                    enemyTarget = null;
                    targetScanner.detectionRadius -= targetScanner.detectionRadiusWhenSpotted;
                }
                return;
            }

            if (state != State.Pursuit) 
                return;

            m_TimerSinceLostTarget += Time.deltaTime;
            if (m_TimerSinceLostTarget >= timeToStopPursuit)
            {
                Vector3 toTarget = enemyTarget.transform.position - transform.position;

                if (toTarget.sqrMagnitude > targetScanner.detectionRadius * targetScanner.detectionRadius || 
                    enemyTarget.GetComponent<HealthPoints>().curHealthPoints <= 0)
                {
                    // the target move out of range, reset the target
                    enemyTarget = null;
                    targetScanner.detectionRadius -= targetScanner.detectionRadiusWhenSpotted;
                }
            }
        }
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    

    public void PursuitState()
    {
        if (enemyTarget == null)
        {
            state = State.WalkBackToBase;
            return;
        }

        distanceFromTarget = Vector3.Distance(enemyTarget.transform.position, transform.position);

        // Проверка, чтобы прекратить преследование, если достигнуто расстояние остановки
        if (distanceFromTarget <= followStoppingDistance)
        {
            enemyTarget = null;
            state = State.WalkBackToBase; // Возвращаемся на базу, если цель слишком близко
            return;
        }

        ChaseTarget();
    }

    public void ChaseTarget()
    {
        if (controller == null || enemyTarget == null) return;

        distanceFromTarget = Vector3.Distance(enemyTarget.transform.position, transform.position);
        controller.SetDestination(enemyTarget.transform.position);
        controller.RotateWithNavMeshAgent();

        if (!controller.animator.GetBool("Pursuit"))
        {
            controller.animator.SetFloat("Vertical", 1);
            controller.animator.SetBool("Pursuit", true);
        }

        if (distanceFromTarget > pursuitStoppingDistance)
        {
            controller.animator.SetBool("BattleStance", false);
            controller.animator.SetFloat("Vertical", 1, 0.2f, Time.deltaTime);
        }
        else if (distanceFromTarget <= pursuitStoppingDistance)
        {
            controller.animator.SetBool("BattleStance", true);
            controller.animator.SetFloat("Vertical", 0, 0.2f, Time.deltaTime);
            if (controller.baseLayerInfo.IsName("Combat Stance"))
                state = State.CombatStance;
        }
    }

    public void FollowCompanionState()
    {
        if (followCompanion == null || controller == null) return;

        distanceFromTarget = Vector3.Distance(followCompanion.position, transform.position);

        controller.animator.SetBool("Patrol", true);
        controller.navmeshAgent.stoppingDistance = followStoppingDistance;

        if (!controller.IsAnimatorTag("Heavy Charge") || !controller.IsAnimatorTag("Attack") || !controller.IsAnimatorTag("Hit"))
            EquipWeapon(false);

        if (enemyTarget != null)
        {
            controller.animator.SetBool("Patrol", false);
            state = State.Pursuit;
            return;
        }

        if (distanceFromTarget <= followStoppingDistance)
        {
            GetPivotAngle(followCompanion.position);
            controller.animator.SetBool("IsMoving", false);
        }
        else
        {
            if (distanceFromTarget > followStoppingDistance * 1.9f)
                controller.animator.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);
            else
                controller.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);

            if (!controller.baseLayerInfo.IsTag("Pivot"))
            {
                Vector3 targetDirection = followCompanion.position - transform.position;
                targetDirection.y = 0;
                targetDirection.Normalize();
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 2 * Time.deltaTime);
            }

            controller.animator.SetBool("IsMoving", true);
            if (controller.navmeshAgent.enabled)
                controller.navmeshAgent.SetDestination(followCompanion.position);
        }
    }

    // Метод для расчета угла поворота относительно позиции цели
    public void GetPivotAngle(Vector3 position)
    {
        Vector3 direction = position - transform.position;

        float forwardWeight = Vector3.Dot(direction, transform.forward);
        float rightWeight = Vector3.Dot(direction, transform.right);

        float forwardMag = Mathf.Abs(forwardWeight);
        float rightMag = Mathf.Abs(rightWeight);

        if (forwardMag >= rightMag)
        {
            if (forwardWeight > 0.0f)
                signedAngle = 0f;
            else
                signedAngle = -180f;
        }
        else if (rightMag >= forwardMag)
        {
            if (rightWeight > 0.0f)
                signedAngle = 90f;
            else
                signedAngle = -90f;
        }
        battleStanceDecisionMade = false;
    }

    void WalkBackToBaseState()
    {
        distanceFromTarget = Vector3.Distance(originalPosition, transform.position);

        if (distanceFromTarget > 0.1f)
        {
            controller.SetDestination(originalPosition);
            controller.animator.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);
        }
        else
        {
            controller.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
            state = State.FollowCompanion; // Переход к следующему состоянию, например, следованию за компаньоном
        }
    }

    void CombatStanceState()
    {
        if (enemyTarget == null) return;

        distanceFromTarget = Vector3.Distance(enemyTarget.transform.position, transform.position);
        Vector3 targetDirection = enemyTarget.transform.position - transform.position;

        if (controller.stunned)
        {
            state = State.Stunned;
            return;
        }

        if (!controller.IsAnimatorTag("Attack") && !controller.IsAnimatorTag("Hit"))
        {
            controller.SetDestination(enemyTarget.transform.position);
            targetDirection.y = 0;
            targetDirection.Normalize();
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 2 * Time.deltaTime);
        }

        if (distanceFromTarget > maxAtkDistance)
        {
            controller.animator.SetBool("BattleStance", false);
            controller.animator.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);
            state = State.Pursuit;
        }
        else if (distanceFromTarget < 2)
        {
            controller.animator.SetFloat("Vertical", -1, 0.1f, Time.deltaTime);
        }
        else
        {
            controller.animator.SetBool("BattleStance", true);
            controller.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
        }

        if (!isAttacking && distanceFromTarget <= maxAtkDistance && atkRecoveryTimer <= 0)
        {
            Attack();
        }
    }

    void Attack()
    {
        if (distanceFromTarget <= maxAtkDistance && distanceFromTarget >= minAtkDistance)
        {
            isAttacking = true;
            controller.animator.SetTrigger("Attack");
            atkRecoveryTimer = atkRecoveryRate;
            isAttacking = false;
        }
    }

    void AttackRecoveryTimer()
    {
        if (atkRecoveryTimer > 0)
        {
            atkRecoveryTimer -= Time.deltaTime;
        }
    }

    private void EquipWeapon(bool equip)
    {
        // Логика экипировки оружия
    }

    private void OnDrawGizmosSelected()
    {
        targetScanner.EditorGizmo(transform);
    }
}
