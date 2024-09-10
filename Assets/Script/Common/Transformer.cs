using UnityEngine;

public class Transformer : MonoBehaviour
{
    [System.Serializable]
    public class TransformerPoint
    {
        public Transform transform;
        public bool useDefaultStayTime = true;
        //[vHideInInspector("useDefaultstayTime", true)]
        public float stayTime;
        public bool useDefaultSpeed = true;
        //[vHideInInspector("useDefaultSpeed", true)]
        public float speedToNextPoint = 1f;
    }

    public enum TransformType
    {
        ToEndPoint,
        ToEndPointAndBack,
        EndlessLoop,
    }

    public enum InteractionType
    {
        MovingPlatorm,
        SmallDoor,
        HugeDoor
    }

    public InteractionType interactionType;
    public TransformType transformType;
    public TransformerPoint[] points;
    [Tooltip("Movement speed between points")]
    public float defaultSpeed = 1f;
    [Tooltip("Time to stay in current point")]
    public float defaultStayTime = 2f;
    [Tooltip("Index to Starting point")]
    public int startIndex;

    public bool pause;

    #region Private 

    Vector3 oldEuler;
    int index = 0;
    public bool invert;
    float currentTime;
    float currentSpeed;
    float dist, currentDist;
    Transform targetTransform;

    public bool again;

    #endregion

    public void SetPause(bool value)
    {
        pause = value;
    }

    void Start()
    {
        if (points.Length == 0 || startIndex >= points.Length) return;
        if (points.Length < 2) return;
        transform.position = points[startIndex].transform.position;
        transform.eulerAngles = points[startIndex].transform.eulerAngles;
        oldEuler = transform.eulerAngles;
        var targetIndex = startIndex;

        if (startIndex + 1 < points.Length) targetIndex++;
        else if (startIndex - 1 > 0)
        {
            targetIndex--; invert = true;
        }

        dist = Vector3.Distance(transform.position, points[targetIndex].transform.position);
        targetTransform = points[targetIndex].transform;
        currentTime = points[startIndex].useDefaultStayTime ? defaultStayTime : points[index].stayTime;
        currentSpeed = points[startIndex].useDefaultSpeed ? defaultSpeed : points[index].speedToNextPoint;
        index = targetIndex;

        if (interactionType == InteractionType.SmallDoor || interactionType == InteractionType.HugeDoor)
        {
            SetPause(true);
        }
    }

    void FixedUpdate()
    {
        if (points.Length == 0) return;

        if (pause) return;

        currentDist = Vector3.Distance(transform.position, targetTransform.position);

        if (currentTime <= 0)
        {
            var distFactor = (float)Mathf.Clamp((100f - ((float)(100f * currentDist) / dist)) * 0.01f, 0, 1f);
            //distFactor = (float)System.Math.Round(distFactor, 6);

            transform.position = Vector3.MoveTowards(transform.position, targetTransform.position, currentSpeed * Time.deltaTime);
            if (!float.IsNaN(distFactor) && !float.IsInfinity(distFactor) && oldEuler != (oldEuler + (((targetTransform.eulerAngles) - oldEuler))))
            {
                transform.eulerAngles = Vector3.Lerp(oldEuler, targetTransform.eulerAngles, distFactor);
            }
        }
        else
            currentTime -= Time.fixedDeltaTime;

        if (currentDist < 0.02f)
        {
            currentSpeed = points[index].useDefaultSpeed ? defaultSpeed : points[index].speedToNextPoint;
            currentTime = points[index].useDefaultStayTime ? defaultStayTime : points[index].stayTime;

            if(transformType == TransformType.ToEndPoint)
            {
                if (!invert)
                {
                    if (index + 1 < points.Length) index++;
                    else
                    {
                        SetPause(true);
                    }
                }
            }

            if(transformType == TransformType.ToEndPointAndBack)
            {
                if (!invert)
                {
                    if (index + 1 < points.Length) index++;
                    else
                    {
                        invert = true;
                    }
                }
                else
                {
                    if (index - 1 >= 0) index--;
                    else
                    {
                        TriggerAction triggerAction = gameObject.transform.parent.gameObject.GetComponentInChildren<TriggerAction>();
                        for (int i = 0; i < triggerAction.thisGroupOfTriggerActions.Length; i++)
                        {
                            triggerAction.thisGroupOfTriggerActions[i].isActive = false;
                        }
                        SetPause(true);
                    }
                }
            }

            if(transformType == TransformType.EndlessLoop)
            {
                if (!invert)
                {
                    if (index + 1 < points.Length) index++;
                    else
                    {
                        invert = true;
                    }
                }
                else
                {
                    if (index - 1 >= 0) index--;
                    else
                    {
                        invert = false;
                    }
                }
            }

            dist = Vector3.Distance(targetTransform.position, points[index].transform.position);
            targetTransform = points[index].transform;
            oldEuler = transform.eulerAngles;
        }
    }

    void OnDrawGizmos()
    {
        if (points == null || points.Length == 0 || startIndex >= points.Length) return;
        Transform oldT = points[0].transform;
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        if (!Application.isPlaying)
        {
            transform.position = points[startIndex].transform.position;
            transform.eulerAngles = points[startIndex].transform.eulerAngles;
        }

        foreach (TransformerPoint t in points)
        {
            if (t.transform != null && t.transform != oldT)
            {
                Gizmos.DrawLine(oldT.position, t.transform.position);
                oldT = t.transform;
            }
        }

        foreach (TransformerPoint t in points)
        {
            if (t.transform)
            {
                Matrix4x4 rotationMatrix = Matrix4x4.TRS(t.transform.position, t.transform.rotation, transform.lossyScale);
                Gizmos.matrix = rotationMatrix;
                Gizmos.DrawCube(Vector3.zero, Vector3.one);
            }
        }
    }
}