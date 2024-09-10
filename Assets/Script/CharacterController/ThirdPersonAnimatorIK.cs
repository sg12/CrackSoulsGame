using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGAE.CharacterController
{
    public class ThirdPersonAnimatorIK : ThirdPersonAction
    {
        [Header("ANIMATOR IK")]
        // General Hands IK
        public Transform rightHandIKTarget;
        [HideInInspector] public float rightHandIKWeight;
        public Transform leftHandIKTarget;
        [HideInInspector] public float leftHandIKWeight;
        // Carry Hands IK
        public Transform rightHandIKCarry;
        public Transform leftHandIKCarry;
        // Climb Hands IK
        [HideInInspector]
        public Transform rightHandClimbPos;
        [HideInInspector]
        public Transform leftHandClimbPos;
        // Weapon IK
        [HideInInspector]
        public float drawPower;
        private Transform weaponAimIK;
        private Transform rightShoulder;

        [HideInInspector] public Transform aimReference;
        public float lookWeight;
        [HideInInspector]
        public float rightHandWeight;

        public LayerMask FootIkLayer;

        public void AnimatorIKStart()
        {
            weaponAimIK = GameObject.Find("WeaponAimIK").transform;
            aimReference = new GameObject().transform;
            aimReference.gameObject.name = "AimReference";
            rightShoulder = animator.GetBoneTransform(HumanBodyBones.RightShoulder);
        }

        private void OnAnimatorIK(int layerIndex)
        {
            ClimbHandsIK();
            CarryHandPositionIK();
            WeaponHandleIK();
            LeftHandWeaponHandleIK();

            animator.SetLookAtWeight(lookWeight, lookWeight);
            animator.SetLookAtPosition(lookPosition);

            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, animator.GetFloat("IKLeftFootWeight"));
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, animator.GetFloat("IKLeftFootWeight"));
            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, animator.GetFloat("IKRightFootWeight"));
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, animator.GetFloat("IKRightFootWeight"));
        }

        void LeftHandWeaponHandleIK()
        {
            #region Weapon During Gameplay

            if (wpnHolster.PrimaryWeaponActive())
            {
                if (cc.weaponArmsID == 3 || cc.weaponArmsID == 4)
                {
                    foreach(Transform t in wpnHolster.primaryE.GetComponentsInChildren<Transform>())
                    {
                        if(t.name == "LWeaponHandleIK")
                        {
                            leftHandIKTarget = t.transform;
                        }
                    }

                    if (cc.lhandLayerWeight == 1)
                    {
                        if(cc.fullBodyInfo.IsTag("Light Attack") || cc.fullBodyInfo.IsTag("Heavy Attack") ||
                        cc.fullBodyInfo.IsTag("Ability Attack") || cc.fullBodyInfo.IsTag("Aerial Attack") || 
                        cc.upperBodyInfo.IsTag("Aiming") || cc.fullBodyInfo.IsTag("Hurt"))
                        {
                            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
                        }
                        else
                        {
                            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                        }
                        animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandIKTarget.position);
                        animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandIKTarget.rotation);
                    }
                }
            }

            #endregion
        }

        void WeaponHandleIK()
        {
            if (wpnHolster.SecondaryActive() && cc.weaponArmsID == 6 && isAiming)
            {
                leftHandIKTarget = GameObject.Find("WeaponAimLeftHandIK").gameObject.transform;

                cc.animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, lookWeight);
                cc.animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandIKTarget.position);
                cc.animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, lookWeight);
                cc.animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandIKTarget.rotation);

                cc.animator.SetIKPositionWeight(AvatarIKGoal.RightHand, rightHandWeight);
                cc.animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandIKTarget.position);
            }
            else if (wpnHolster.SecondaryActive() && cc.weaponArmsID == 6 && !isAiming)
            {
                leftHandIKTarget = GameObject.Find("WeaponAimLeftHandIK").gameObject.transform;

                cc.animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                cc.animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandIKTarget.position);
                cc.animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
                cc.animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandIKTarget.rotation);

                cc.animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                cc.animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandIKTarget.position);
            }
        }

        void CarryHandPositionIK()
        {
            if (wpnHolster.PrimaryWeaponActive() || wpnHolster.SecondaryActive() || cc.isSwimming)
                return;

            if (isCarrying || (breakbleAction && breakbleAction.breakable && breakbleAction.breakable.lifted) && cc.upperBodyInfo.IsName("Carry"))
            {
                leftHandIKCarry.SetParent(animator.GetBoneTransform(HumanBodyBones.Spine).transform);
                rightHandIKCarry.SetParent(animator.GetBoneTransform(HumanBodyBones.Spine).transform);

                leftHandIKCarry.localPosition = new Vector3(-0.029f, -0.364f, 0.318f);
                leftHandIKCarry.localRotation = Quaternion.Euler(0, 20, -103.9f);
                rightHandIKCarry.localPosition = new Vector3(-0.163f, 0.07f, 0.433f);
                rightHandIKCarry.localRotation = Quaternion.Euler(0, 0, 10);

                cc.animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                cc.animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                cc.animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandIKCarry.position);
                cc.animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandIKCarry.rotation);

                cc.animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                cc.animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                cc.animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandIKCarry.position);
                cc.animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandIKCarry.rotation);
            }
        }

        void ClimbHandsIK()
        {
            if (climbData.inPosition)
            {
                animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandClimbPos.transform.position);
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, animator.GetFloat("HandClimbWeight"));
                animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandClimbPos.transform.position);
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, animator.GetFloat("HandClimbWeight"));
            }
        }

        public bool CanAttachLeftHandIK()
        {
            if (cc.IsAnimatorTag("Sheath R") || cc.IsAnimatorTag("Unsheathe R")
                || cc.IsAnimatorTag("Sheath L") || cc.IsAnimatorTag("Unsheathe L"))
                return false;

            return true;
        }

        public void WeaponAimIK()
        {
            if (!wpnHolster.secondaryE) return;

            if (rightShoulder) weaponAimIK.position = rightShoulder.position;

            if (isAiming)
            {
                // AIM REFERENCE TO CAMERA FORWARD
                aimReference.position = Vector3.Lerp(aimReference.position, lookPosition, 100 * Time.deltaTime);

                weaponAimIK.LookAt(aimReference.position);

                if (cc.weaponArmsID == 6)
                    rightHandIKTarget.position = wpnHolster.bowString.transform.position;

                // PLAYER ROTATION TOWARDS AIM REFERENCE
                Vector3 targetDirection = tpCam.transform.TransformDirection(Vector3.forward);
                targetDirection.y = 0;
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 60 * Time.deltaTime);
            }
            else
            {
                lookWeight = 0;
            }

            // DEBUG AIM DIRECTION
            Debug.DrawRay(wpnHolster.arrowPrefabSpot.transform.position, tpCam.transform.forward * 10, Color.red);
        }

        public void DrawingBowAnimation()
        {
            if (attackButton && isAiming && !isReloading)
            {
                rightHandWeight = drawPower;
                drawPower += 2 * Time.deltaTime;
            }
            if (!attackButton && isAiming)
            {
                rightHandWeight = 0;
                if (cc.upperBodyInfo.IsName("Shoot"))
                {
                    drawPower -= 4 * Time.deltaTime;
                }
            }
            lookWeight = drawPower;
            drawPower = Mathf.Clamp(drawPower, 0, 1);

            // PLAYER PULLING BOW ANIMATION
            animator.SetBool("Draw", attackButton);
            animator.SetFloat("DrawPower", drawPower);

            animator.SetBool("Reloading", cc.isReloading);
            animator.SetFloat("UpperBodyID", cc.weaponArmsID);

            // BOW ANIMATION
            if (!isReloading)
            {
                wpnHolster.secondaryE.GetComponent<Animator>().SetBool("Draw", attackButton);
                wpnHolster.secondaryE.GetComponent<Animator>().SetFloat("DrawPower", drawPower * GetBowStats());
            }
        }

        private float GetBowStats()
        {
            if (wpnHolster.secondaryE.GetComponent<ItemData>().maxBowSpd > 0)
            {
                return hudM.agility + wpnHolster.secondaryE.GetComponent<ItemData>().maxBowSpd;
            }
            else
                return hudM.agility + 1;
        }
    }
}
