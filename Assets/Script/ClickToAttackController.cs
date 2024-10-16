using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace BLINK.Controller
{
    public class ClickToAttackController : MonoBehaviour
    {
        public static bool enemyClicked = false;

        
        private NavMeshAgent agent;
        private Transform attackTarget;
        
        private float defaultStoppingDistance = 1.0f; 
        private float enemyStoppingDistance = 2.0f;
        private bool isAttacking = false;

        private void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            if (agent == null)
            {
                Debug.LogError("ClickToAttackController: NavMeshAgent не найден на персонаже.");
            }
            else
            {
                // установка стандартного stopping distance
                agent.stoppingDistance = defaultStoppingDistance;
            }
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandleAttackClick();
            }

            // проверка достижения цели для атаки
            if (isAttacking && attackTarget != null)
            {
                if (!agent.pathPending)
                {
                    if (agent.remainingDistance <= agent.stoppingDistance)
                    {
                        // остановить движение
                        agent.ResetPath();
                        isAttacking = false;
                        Debug.Log($"Персонаж достиг цели для атаки: {attackTarget.name}");

                        // установка стандартного stopping distance
                        agent.stoppingDistance = defaultStoppingDistance;
                        
                        // место для доп логики
                        
                        attackTarget = null;
                    }
                }
            }
        }
        
        private void LateUpdate()
        {
            // сбрасывание флага в конце кадра
            enemyClicked = false;
        }
        
        private void HandleAttackClick()
        {
            // проверка на нажатие по UI
            if (IsPointerOverUIObject())
            {
                Debug.Log("ЛКМ нажата по UI элементу.");
                return;
            }
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, 100f))
            {
                // Получение компонента Interactable, который висит на врагах
                Interactable interactable = hit.transform.GetComponent<Interactable>() ?? hit.transform.GetComponentInParent<Interactable>();
                if (interactable != null && interactable.interactionType == InteractableType.Enemy)
                {
                    // устанавливаем флаг, что клик был по врагу
                    enemyClicked = true;

                    Debug.Log($"Враг кликнут: {interactable.gameObject.name}");
                    attackTarget = interactable.transform;

                    // установка stopping distance для атаки
                    agent.stoppingDistance = enemyStoppingDistance;

                    // установка цели с учётом attackDistance
                    SetDestinationWithAttackDistance(attackTarget.position);
                    isAttacking = true;
                }
                else
                {
                    Debug.Log("Кликнутый объект не имеет компонента Interactable типа Enemy.");

                    // если клик не по врагу, то устанавливается стандартная дистанция
                    agent.stoppingDistance = defaultStoppingDistance;
                }
            }
            else
            {
                Debug.Log("ЛКМ кликнула по недостижимой позиции или не по врагу.");

                // если клик не по врагу, то устанавливается стандартная дистанция
                agent.stoppingDistance = defaultStoppingDistance;
            }
        }
        
        private void SetDestinationWithAttackDistance(Vector3 targetPosition)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            Vector3 destination = targetPosition - direction * enemyStoppingDistance;

            // проверка на нахождение точки в NavMesh
            NavMeshHit navHit;
            if (NavMesh.SamplePosition(destination, out navHit, enemyStoppingDistance, NavMesh.AllAreas))
            {
                agent.SetDestination(navHit.position);
                Debug.Log($"Установлена точка назначения: {navHit.position}");
            }
            else
            {
                // Если ближайшая допустимая точка не найдена, устанавливаем точку прямо к врагу
                agent.SetDestination(targetPosition);
                Debug.LogWarning("Не удалось найти допустимую точку назначения на заданном расстоянии. Устанавливается позиция врага.");
            }
        }

       // проверяет, находится ли указатель мыши над UI элементом.
        private bool IsPointerOverUIObject()
        {
            if (EventSystem.current == null) return false;

            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }
    }
    
}
