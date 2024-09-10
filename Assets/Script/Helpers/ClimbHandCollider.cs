using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPGAE.CharacterController;

public class ClimbHandCollider : MonoBehaviour
{

    FootStepSystem footStepS;
    ThirdPersonController cc;

    // Start is called before the first frame update
    void Start()
    {
        footStepS = GetComponentInParent<FootStepSystem>();
        cc = GetComponentInParent<ThirdPersonController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7 && cc.climbData.inPosition)
        {
            other.gameObject.tag = "Untagged";
            other.gameObject.layer = 2;
        }

        if (footStepS.dustParticle && cc.climbData.inPosition && other.transform.name == "SmallLedge" ||
        other.transform.name == "LongLedge" || other.transform.name == "LongLedgeTop")
        {
            GameObject clone;
            clone = Instantiate(footStepS.dustParticle, transform.position, transform.rotation);
            clone.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 2 || other.gameObject.layer == 11 && cc.climbData.inPosition)
        {
            other.gameObject.tag = "Ledge";
            other.gameObject.layer = 7;
        }
    }
}
