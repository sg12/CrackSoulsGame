using System;
using UnityEngine;

public class MonsterBehaviour : MonoBehaviour
{
    [Header("SCANNER SETTINGS")]
    public TargetScanner targetScanner;

    public GameObject enemyTarget;

    private Vector3 originalPosition;
    private float distanceFromTarget;
    private float pursuitStoppingDistance = 3f;
    private float maxAtkDistance = 5f;
    private float minAtkDistance = 1f;
    private float atkRecoveryTimer;
    private float atkRecoveryRate = 2f;
    private float shootOnAimTimer;
    private bool isAttacking;
    private bool isShooting;
    private bool battleStanceDecisionMade = false;
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
    }

    private void Update()
    {
        FindTarget();
    }

    public void FindTarget()
    {
        // Игнорируем разницу по высоте, если цель уже обнаружена
        GameObject targetScanned = targetScanner.Detect(transform, enemyTarget == null);

        if (enemyTarget == null)
        {
            if (targetScanned != null)
            {
                HealthPoints distributor = targetScanned.GetComponent<HealthPoints>();
                if (distributor != null && distributor.curHealthPoints > 0)
                {
                    enemyTarget = distributor.gameObject;
                    targetScanner.detectionRadius += targetScanner.detectionRadiusWhenSpotted;
                    state = State.Pursuit;  // Переход к преследованию при обнаружении цели
                }
            }
        }
        else
        {
            PursuitState();
        }
    }

    public void PursuitState()
    {
        if (!enemyTarget)
        {
            // Изменение состояния, если враг потерян
            if (state != State.WalkBackToBase)
            {
                Vector3 toOriginPosition = originalPosition - transform.position;
                toOriginPosition.y = 0;
                if (toOriginPosition.sqrMagnitude > 1)
                    state = State.WalkBackToBase;
            }
        }
        else
        {
            ChaseTarget();  // Логика преследования цели
        }
    }

    public void ChaseTarget()
    {
        distanceFromTarget = Vector3.Distance(enemyTarget.transform.position, transform.position);

        // Движение к цели
        controller.SetDestination(enemyTarget.transform.position);
        controller.RotateWithNavMeshAgent();

        Vector3 targetDirection = enemyTarget.transform.position - transform.position;
        targetDirection.y = 0;
        targetDirection.Normalize();
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 2 * Time.deltaTime);

        EquipWeapon(true);

        if (!controller.animator.GetBool("Pursuit"))
        {
            controller.animator.SetFloat("Vertical", 1);
            controller.animator.SetBool("Pursuit", true);
        }

        // Проверка дистанции до цели для переключения между преследованием и боевой стойкой
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
                state = State.CombatStance;  // Переход в боевую стойку
        }
    }

    // Логика боевой стойки
    void CombatStanceState()
    {
        if (enemyTarget == null)
        {
            return;
        }
        
        distanceFromTarget = Vector3.Distance(enemyTarget.transform.position, transform.position);
        Vector3 targetDirection = enemyTarget.transform.position - transform.position;

        if (controller.stunned)
        {
            state = State.Stunned;
            return;
        }

        // Поворот к цели
        if (!controller.IsAnimatorTag("Attack") && !controller.IsAnimatorTag("Hit"))
        {
            controller.SetDestination(enemyTarget.transform.position);
            targetDirection.y = 0;
            targetDirection.Normalize();
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 2 * Time.deltaTime);
        }

        // Если цель слишком близко или далеко — соответственно, подходить или отступать
        if (distanceFromTarget > maxAtkDistance)
        {
            controller.animator.SetBool("BattleStance", false);
            controller.animator.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);
            state = State.Pursuit;  // Возвращаемся к преследованию
        }
        else if (distanceFromTarget < 2)
        {
            controller.animator.SetFloat("Vertical", -1, 0.1f, Time.deltaTime);  // Отступаем
        }
        else
        {
            controller.animator.SetBool("BattleStance", true);
            controller.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);  // Останавливаемся
        }

        // Атака
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
            atkRecoveryTimer = atkRecoveryRate;  // Восстановление после атаки
            isAttacking = false;
        }
    }

    // Таймер восстановления после атаки
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
        targetScanner.EditorGizmo(transform);  // Отображение радиуса сканирования
    }
}
