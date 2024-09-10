using UnityEngine;

public class SetQuestObjectiveLocation : MonoBehaviour
{
    public MiniMapItem miniItem;

    public QuestWayPoint[] waypoints;
    public int phaseNum;

    private void Awake()
    {
        waypoints = FindObjectsOfType<QuestWayPoint>();
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i].fadeUI.canvasGroup.alpha = 0;
        }
    }

    void FixedUpdate()
    {
        SetPos();
    }

    void SetPos()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, -Vector3.up);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, Physics.AllLayers))
        {
            if (hit.collider.GetComponent<MiniMapItem>() && miniItem == null)
            {
               // miniItem = hit.collider.GetComponent<MiniMapItem>();
              //  transform.SetParent(hit.transform);
            }
        }
    }
}
