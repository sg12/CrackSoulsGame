using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransitionDestination : MonoBehaviour
{
    public string sceneName;
    public int SpawnPointIDRef;
    public Color transitionScreenColor = new Color32(0, 0, 0, 255);
    public Transform[] spawnReposition;
    private MeshRenderer meshR;

    private void OnEnable()
    {
        meshR = GetComponent<MeshRenderer>();
        if (meshR != null)
            meshR.enabled = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);

        foreach (Transform t in spawnReposition)
        {
            Gizmos.DrawLine(transform.position, t.position);
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(t.position, t.rotation, transform.lossyScale);
            Gizmos.matrix = rotationMatrix;
            Gizmos.DrawCube(Vector3.zero, Vector3.one);
        }
    }
}