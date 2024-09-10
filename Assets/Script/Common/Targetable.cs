using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targetable : MonoBehaviour, TargetableInterface
{
    [Header("Targetable")]
    [SerializeField] private bool m_targetable = true;
    [SerializeField] private Transform m_targetableTransform;

    bool TargetableInterface.Targetable { get => m_targetable; }
    Transform TargetableInterface.targetTransform { get => m_targetableTransform; }
}