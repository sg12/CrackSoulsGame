using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface TargetableInterface
{
    public bool Targetable { get; }
    public Transform targetTransform { get; }
}
