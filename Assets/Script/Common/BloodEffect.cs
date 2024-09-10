using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPGAE.CharacterController;

public class BloodEffect : MonoBehaviour
{
    [System.Serializable]
    public class Blood
    {
        public string bloodName;
        public GameObject bloodPrefab;
    }
    public Blood[] bloodList;

    public void CreateBloodEffect(Vector3 damageSource, string bloodEffectName)
    {
        // Position
        Vector3 positionHeight = new Vector3(transform.position.x, damageSource.y, transform.position.z);
        // Rotation
        Vector3 lookPosition = damageSource - transform.position;
        Quaternion lookDirection = Quaternion.LookRotation(lookPosition);
        // Create Blood
        foreach (Blood blood in bloodList)
        {
            if (blood.bloodName == bloodEffectName)
            {
                Instantiate(blood.bloodPrefab, positionHeight, lookDirection);
            }
        }
    }
}
