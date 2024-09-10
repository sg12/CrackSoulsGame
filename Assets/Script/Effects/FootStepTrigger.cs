using System.Collections;
using System.Collections.Generic;
using RPGAE.CharacterController;
using UnityEngine;

public class FootStepTrigger : MonoBehaviour
{
    Collider thisCollider;
    FootStepSystem footStepS;
    ThirdPersonController cc;
    InventoryManager inventoryM;

    // Start is called before the first frame update
    void Start()
    {
        footStepS = GetComponentInParent<FootStepSystem>();
        thisCollider = GetComponentInParent<Collider>();
        cc = GetComponentInParent<ThirdPersonController>();
        inventoryM = FindObjectOfType<InventoryManager>();

        if (thisCollider)
        {
            thisCollider.isTrigger = true;
            SetCollisions();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (footStepS)
        {
            footStepS.TryPlayFootstep();
        }
        bool conditionsToParticle = !inventoryM.isPauseMenuOn;
        if (footStepS != null && footStepS.dustParticle && !other.transform.CompareTag("Ledge") && conditionsToParticle)
        {
            GameObject clone = Instantiate(footStepS.dustParticle, thisCollider.transform.position, thisCollider.transform.rotation);
            clone.transform.localScale = new Vector3(footStepS.dustSize, footStepS.dustSize, footStepS.dustSize);
        }
    }

    void SetCollisions()
    {
        if (!footStepS) 
            return;

        Collider[] allColliders = footStepS.GetComponentsInChildren<Collider>();
        foreach (var collider in allColliders)
        {
            if (collider != GetComponent<Collider>())
            {
                Physics.IgnoreCollision(thisCollider, collider);
            }
        }
    }
}
