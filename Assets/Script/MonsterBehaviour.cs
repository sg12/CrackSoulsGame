using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class MonsterBehaviour : MonoBehaviour
{
    [Header("SCANNER SETTINGS")]
    public TargetScanner targetScanner;
    public float followStoppingDistance; 
    public GameObject enemyTarget;
    public Transform followCompanion;
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
    public float maxDetectionRadius = 10f;
    private float distanceToStopTracking = 15f; // Дистанция, после которой персонаж прекратит отслеживание

    private AIController controller;
    private enum State
    {
        Patrol,
        Pursuit,
        CombatStance,
        WalkBackToBase,
        FollowCompanion,
        Stunned
    }

    // Добавленные переменные для новой логики обнаружения
    

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
            case State.Patrol:
                break;
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

    #region Find Target

    public void FindTarget()
    {
        // Обнаружение цели
        GameObject targetScanned = targetScanner.Detect(transform, enemyTarget == null);

        if (enemyTarget == null)
        {
            // Если цель не была найдена ранее, сохраняем её и увеличиваем радиус обнаружения
            if (targetScanned != null)
            {
                HealthPoints targetHealth = targetScanned.GetComponent<HealthPoints>();
                if (targetHealth != null && targetHealth.curHealthPoints > 0)
                {
                    enemyTarget = targetHealth.gameObject;
                    targetScanner.detectionRadius += targetScanner.detectionRadiusWhenSpotted;
                    state = State.Pursuit; // Переход в режим преследования
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
        controller.navmeshAgent.enabled = true; // Возвращаем персонажа в обычное состояние
        state = State.Patrol; // Возвращаем в режим патрулирования
    }

    #endregion

    #region Pursuit
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
    #endregion
    public void FollowCompanionState()
    {
        if (followCompanion == null || controller == null) return;

        distanceFromTarget = Vector3.Distance(followCompanion.position, transform.position);

        controller.animator.SetBool("Patrol", true);
        controller.navmeshAgent.stoppingDistance = followStoppingDistance;

        /*if (!controller.IsAnimatorTag("Heavy Charge") || !controller.IsAnimatorTag("Attack") || !controller.IsAnimatorTag("Hit"))
            EquipWeapon(false);*/

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
            controller.animator.SetBool("Attack", true); // Запускаем анимацию атаки
            isAttacking = true; // Устанавливаем флаг атаки
        }

        if (distanceFromTarget > maxAtkDistance)
        {
            // Если цель слишком далеко, переходим в режим преследования
            state = State.Pursuit;
        }
    }

    void AttackRecoveryTimer()
    {
        if (isAttacking)
        {
            atkRecoveryTimer += Time.deltaTime;

            if (atkRecoveryTimer >= atkRecoveryRate)
            {
                atkRecoveryTimer = 0f;
                isAttacking = false;
                controller.animator.SetBool("Attack", false);
            }
        }
    }
    
    //отрисовка сканнера на сцене
    private void OnDrawGizmosSelected()
    {
        targetScanner.EditorGizmo(transform);
    }
}
