using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.AI;

namespace RPGAE.CharacterController
{
    public class ThirdPersonController : ThirdPersonAnimator
    {
        //public NavMeshAgent navAgent:

        public virtual void Sprint(bool sprintingButton)
        {
            isSprinting = sprintingButton;
        }

        public void Dodging()
        {
            bool inputToDodge = input.magnitude > 0f && cc.rpgaeIM.PlayerControls.Action.triggered;
            bool conditionsToDodge = grounded && isStrafing && !isDodging && !cc.baseLayerInfo.IsName("Dodge") && !animator.IsInTransition(0);
            if (inputToDodge && conditionsToDodge)
            {
                animator.SetFloat("InputVertical", 0);
                animator.SetFloat("InputHorizontal", 0);
                animator.CrossFadeInFixedTime("Dodge", 0.1f);
            }
        }

        public void LockOn()
        {
            tpCam.ToggleLockOn(!tpCam.lockedOn);
        }

        public void RayCastLookPosition()
        {
            if (tpCam == null) return;

            Ray ray = new Ray(tpCam.transform.position, tpCam.transform.forward);
            lookPosition = ray.GetPoint(10);
        }
    }
}
