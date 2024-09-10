using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPGAE.CharacterController;

public class CharacterWeaponPreview : MonoBehaviour
{
    Animator animator;
    public Transform leftHand;
    ThirdPersonController cc;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        cc = FindObjectOfType<ThirdPersonController>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("WeaponArmsID", cc.preWeaponArmsID);
    }


    void OnAnimatorIK(int layerIndex)
    {
        if (cc.wpnHolster.primaryEP)
        {
            if (cc.preWeaponArmsID == 3 || cc.preWeaponArmsID == 4)
            {
                foreach (Transform t in cc.wpnHolster.primaryEP.GetComponentsInChildren<Transform>())
                {
                    if (t.name == "LWeaponHandlePIK")
                    {
                        leftHand = t.transform;
                    }
                }
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHand.position);
                animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHand.rotation);
            }
        }
    }
}
