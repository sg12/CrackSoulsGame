using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGAE.CharacterController
{
    public class ThirdPersonAnimator : ThirdPersonAnimatorIK
    {
        [HideInInspector]
        public float
        jumpToFall,
        idleVariationTimer,
        weaponArmsID,
        preWeaponArmsID,
        upperBodyID,
        lhandLayerWeight,
        rhandLayerWeight,
        upperBodyWeight,
        fullBodyWeight;
        
        public AnimatorStateInfo
        baseLayerInfo,
        faceInfo,
        rightArmdInfo,
        leftArmdInfo,
        onlyArmsInfo,
        upperBodyInfo,
        fullBodyInfo;

        #region Input Angles 

        [HideInInspector]
        public GameObject characterPositionCompass;
        [HideInInspector]
        public GameObject lookAtInputDirection;

        [HideInInspector]
        public float walkStartAngle;
        [HideInInspector]
        public float inputAngle;

        [Tooltip("The shortest Angle. unsigned.")]
        [HideInInspector] public float angle;
        [Tooltip("The signed Angle. This is also the shortest distance.")]
        [HideInInspector] public float signedAngle;
        [Tooltip("The positive delta angle. This means the ClockWise travel to reach the target")]
        [HideInInspector] public float resultPositiveAngle;
        [Tooltip("The negative delta angle. This means the Counter ClockWise travel to reach the target")]
        [HideInInspector] public float resultNegativeAngle;

        #endregion

        public virtual void UpdateAnimator()
        {
            if (animator == null || !animator.enabled) return;

            AnimationBehaviour();

            animator.SetBool("CanMove", canMove);
            animator.SetBool("Strafing", isStrafing);
            animator.SetBool("IsCarrying", isCarrying);
            animator.SetInteger("ActionState", actionState);
            animator.SetBool("Grounded", grounded);
            animator.SetFloat("GroundDistance", groundDistance);
            animator.SetBool("ClimbEnter", canJumpAcross);
            animator.SetBool("IsSwimming", isSwimming);
            animator.SetBool("IsJump", isJumping);

            #region Movement

            if (canMove)
            {
                animator.SetFloat("InputMagnitude", isSprinting ? input.magnitude + 0.5f : input.magnitude, 0.2f, Time.deltaTime);
            }
            else
            {
                if (grounded)
                {
                    animator.SetFloat("InputMagnitude", 0);
                }
            }

            if (isStrafing)
            {
                animator.SetFloat("InputVertical", isDodging ? dodgeLastInput.z : input.z, 0.25f, Time.deltaTime);
                animator.SetFloat("InputHorizontal", isDodging ? dodgeLastInput.x : input.x, 0.25f, Time.deltaTime);
            }
            else
            {
                if (climbData.inPosition)
                {
                    animator.SetFloat("InputVertical", input.z, 0.25f, Time.deltaTime);
                    animator.SetFloat("InputHorizontal", input.x, 0.25f, Time.deltaTime);
                }
            }

            #endregion

            if (cc.inventoryM.inventoryHUD.fadeUI.canvasGroup.alpha == 0)
            {
                animator.SetFloat("InputAngle", inputAngle, 0.2f, Time.deltaTime);
                animator.SetFloat("RawInputAngle", inputAngle);
                animator.SetFloat("WalkStartAngle", walkStartAngle);
            }

            isDodging = cc.baseLayerInfo.IsName("Dodge.Dodge");

            LayerInfo();
            IdleAnimation();
            GetSignedAngle();
            ClimbAnimation();
            AirborneAnimation();
            CombatAnimation();
            StrafingAnimation();
            DodgeAnimation();
            JumpingAnimation();
            IntroWakeUpInput();
        }

        private void GetSignedAngle()
        {
            GameObject go = characterPositionCompass;
            if (go == null)
            {
                return;
            }

            GameObject goTarget = lookAtInputDirection;
            if (goTarget == null)
            {
                return;
            }

            angle = Quaternion.Angle(go.transform.rotation, goTarget.transform.rotation);

            // get a "forward vector" for each rotation
            Vector3 forwardA = go.transform.rotation * Vector3.forward;
            Vector3 forwardB = goTarget.transform.rotation * Vector3.forward;

            // get a numeric angle for each vector, on the X-Z plane (relative to world forward)
            float angleA = Mathf.Atan2(forwardA.x, forwardA.z) * Mathf.Rad2Deg;
            float angleB = Mathf.Atan2(forwardB.x, forwardB.z) * Mathf.Rad2Deg;

            // get the signed difference in these angles
            float _signedAngle = Mathf.DeltaAngle(angleA, angleB);

            signedAngle = _signedAngle;
            inputAngle = _signedAngle;
            walkStartAngle = _signedAngle;

            if (_signedAngle < 0)
            {
                resultNegativeAngle = _signedAngle;
                resultPositiveAngle = 360f + _signedAngle;
            }
            else
            {
                resultNegativeAngle = -360f + _signedAngle;
                resultPositiveAngle = _signedAngle;
            }
        }

        void IdleAnimation()
        {
            if (fullBodyInfo.IsTag("StandardAttack") || isAiming || isStrafing)
            {
                idleVariationTimer = 0.0f;
                return;
            }

            if (input != Vector3.zero) animator.SetInteger("IdleRandom", 0);
            if (input == Vector3.zero && grounded)
            {
                if (idleVariationTimer >= 2)
                {
                    idleVariationTimer = 0.0f;
                    animator.SetInteger("IdleRandom", Random.Range(0, 5));
                }
                idleVariationTimer += 0.5f * Time.deltaTime;
            }
            else
            {
                idleVariationTimer = 0.0f;
            }
        }

        void JumpingAnimation()
        {
            if (isJumping)
            {
                bool RunningJump = !IsAnimatorTag("RunJump");
                Vector3 jumpVelocity = targetDirection * (RunningJump ? 7.5f : 7.2f) + transform.up * (7.5f);
                rigidBody.linearVelocity = jumpVelocity;
                if (groundDistance > 0.5f)
                    isJumping = false;
            }

            if (!grounded)
            {
                jumpToFall += Time.deltaTime;
            }
            else
            {
                jumpToFall = 0;
            }
            animator.SetFloat("JumpToFall", jumpToFall);
        }

        void LayerInfo()
        {
            animator.SetLayerWeight(2, rhandLayerWeight);
            animator.SetLayerWeight(3, lhandLayerWeight);
            animator.SetLayerWeight(4, upperBodyWeight);
            animator.SetLayerWeight(5, fullBodyWeight);
            baseLayerInfo = animator.GetCurrentAnimatorStateInfo(0);
            rightArmdInfo = animator.GetCurrentAnimatorStateInfo(2);
            leftArmdInfo = animator.GetCurrentAnimatorStateInfo(3);
            upperBodyInfo = animator.GetCurrentAnimatorStateInfo(4);
            fullBodyInfo = animator.GetCurrentAnimatorStateInfo(5);
        }

        public bool IsAnimatorTag(string tag)
        {
            if (animator == null) return false;
            if (baseLayerInfo.IsTag(tag)) return true;
            if (rightArmdInfo.IsTag(tag)) return true;
            if (leftArmdInfo.IsTag(tag)) return true;
            if (upperBodyInfo.IsTag(tag)) return true;
            if (fullBodyInfo.IsTag(tag)) return true;
            return false;
        }

        protected void CombatAnimation()
        {
            animator.SetBool("Aiming", isAiming);
            animator.SetBool("IsBlocking", isBlocking);
            animator.SetBool("IsAttacking", isAttacking);
            animator.SetBool("isCharging", attackButton);
            animator.SetFloat("UpperBodyID", upperBodyID);
            animator.SetFloat("WeaponArmsID", weaponArmsID);

            isAttacking = IsAnimatorTag("Light Attack") || fullBodyInfo.IsName("Heavy Charge")
            || IsAnimatorTag("Heavy Attack") || fullBodyInfo.IsName("Aerial Attack Land") || IsAnimatorTag("Ability Attack");
            isKnockedBack = fullBodyInfo.IsName("Knock Back");
            if (isAttacking)
                rigidBody.linearVelocity = Vector3.zero;

            FinisherAnimation();
            GeneralAttackAnimation();
            SheathingWeaponAnimation();
        }

        void GeneralAttackAnimation()
        {
            if (IsAnimatorTag("Light Attack"))
            {
                if (fullBodyInfo.normalizedTime > 0.5f)
                {
                    performingLightAttack = false;
                }
            }
            if (IsAnimatorTag("Heavy Attack"))
            {
                if(fullBodyInfo.normalizedTime > 0.5f)
                {
                    performingHeavyAttack = false;
                    if (wpnHolster.alteredPrimaryE != null && wpnHolster.alteredPrimaryE.activeInHierarchy)
                        StartCoroutine(ResetAlteredWeapon(8));
                }
            }
        }

        public IEnumerator ResetAlteredWeapon(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            if (wpnHolster.alteredPrimaryE != null)
            {
                wpnHolster.primaryE.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
                wpnHolster.alteredPrimaryE.SetActive(false);
            }
        }

        void IntroWakeUpInput()
        {
            if (cc.input.magnitude > 0 &&
            systemM.loadingScreenFUI.canvasGroup.alpha == 0 && systemM.blackScreenFUI.canvasGroup.alpha == 0)
            {
                animator.SetBool("Intro", false);
            }
        }

        void SheathingWeaponAnimation()
        {
            #region Weapon arms IK weight 

            if (weaponArmsID == 1 || weaponArmsID == 2 || weaponArmsID == 5 || weaponArmsID == 6)
            {
                lhandLayerWeight = 1;
                rhandLayerWeight = 1;
            }

            if (weaponArmsID == 3 || weaponArmsID == 4 || weaponArmsID == 7)
            {
                rhandLayerWeight = 1;
                if (wpnHolster.PrimaryWeaponActive() && CanSheath())
                {
                    lhandLayerWeight += 4 * Time.deltaTime;
                    if (lhandLayerWeight > 1.0f)
                    {
                        lhandLayerWeight = 1;
                    }
                }
                if (wpnHolster.SecondaryActive() && CanSheath())
                {
                    lhandLayerWeight += 4 * Time.deltaTime;
                    if (lhandLayerWeight > 1.0f)
                    {
                        lhandLayerWeight = 1;
                    }
                }
            }

            if (animator.GetInteger("EquipLeftHandID") == 0 && animator.GetInteger("EquipRightHandID") == 0)
                weaponArmsID = 0;

            #endregion

            #region Place equipped arrow on bow string

            if (rightArmdInfo.IsTag("WeaponArms"))
            {
                if (wpnHolster.SecondaryActive())
                {
                    if (!cc.upperBodyInfo.IsName("Reload") && !isSheathing && !isReloading 
                    && wpnHolster.arrowString != null && !wpnHolster.arrowString.activeInHierarchy
                    && inventoryM.bowAndArrowInv.counter[inventoryM.bowAndArrowInv.slotArrowEquipped] > 0)
                    {
                        if (wpnHolster.arrowString)
                            wpnHolster.arrowString.SetActive(true);
                        if (wpnHolster.arrowE)
                            wpnHolster.arrowE.SetActive(false);
                    }
                }
            }

            #endregion

            #region Reload

            if (upperBodyInfo.IsName("Reload"))
            {
                // disbale muzzzle flash for gun
                if (wpnHolster.secondaryE.GetComponent<HitBox>().effects.signatureEffect != null)
                    wpnHolster.secondaryE.GetComponent<HitBox>().effects.signatureEffect.gameObject.SetActive(false);

                if (upperBodyInfo.normalizedTime > 0.45f)
                {
                    RemoveArrowFromQuiver();
                    if (wpnHolster.arrowE)
                        wpnHolster.arrowE.SetActive(true);
                }
                if (upperBodyInfo.normalizedTime > 0.85f)
                {
                    isReloading = false;
                }
            }

            if (upperBodyInfo.IsName("Shoot"))
            {
                if (upperBodyInfo.normalizedTime > 0.7f)
                {
                    // disbale muzzzle flash for gun
                    if (wpnHolster.secondaryE.GetComponent<HitBox>().effects.signatureEffect != null)
                        wpnHolster.secondaryE.GetComponent<HitBox>().effects.signatureEffect.gameObject.SetActive(false);
                }
            }

            #endregion

            if (rightArmdInfo.IsTag("Sheath R"))
            {
                if (isAiming) return;

                if (cc.rightArmdInfo.normalizedTime >= 0.45f)
                {
                    // SWITCH--------------------------------------

                    #region Switch from secondary to primary weapon

                    if (animator.GetInteger("EquipRightHandID") == 1)
                    {
                        if (wpnHolster.ArrowActive())
                        {
                            cc.lhandLayerWeight -= 2 * Time.deltaTime;
                            wpnHolster.arrowE.SetActive(false);
                        }
                    }

                    #endregion

                    // REMOVE--------------------------------------

                    #region Remove secondary weapon and equip to equip primary weapon

                    if (animator.GetInteger("EquipLeftHandID") == 1)
                    {
                        if (wpnHolster.SecondaryActive())
                        {
                            wpnHolster.secondaryE.SetActive(false);
                            wpnHolster.secondaryH.SetActive(true);
                        }
                        if (wpnHolster.ArrowOnStringActive() || wpnHolster.ArrowActive())
                        {
                            wpnHolster.arrowE.SetActive(false);
                            wpnHolster.arrowString.SetActive(false);
                        }
                    }

                    #endregion

                    // EQUIP--------------------------------------

                    #region Equip primary weapon

                    if (animator.GetInteger("EquipRightHandID") == 1)
                    {
                        if (wpnHolster.PrimaryWeaponHActive())
                        {
                            if (drawAS)
                                drawAS.PlayRandomClip();

                            wpnHolster.primaryE.SetActive(true);
                            wpnHolster.primaryH.SetActive(false);
                        }
                    }

                    #endregion

                    #region Equip arrow 

                    if (animator.GetInteger("EquipRightHandID") == 2)
                    {
                        if (wpnHolster.arrowE != null)
                        {
                            RemoveArrowFromQuiver();
                            wpnHolster.arrowE.SetActive(true);
                        }
                    }

                    #endregion
                }

                if (cc.rightArmdInfo.normalizedTime >= 0.95f)
                {
                    isSheathing = false;
                }
            }

            if (cc.rightArmdInfo.IsTag("Unsheathe R"))
            {
                if (cc.rightArmdInfo.normalizedTime >= 0.45f)
                {
                    // REMOVE

                    #region Remove all right handed weapons 

                    if (wpnHolster.PrimaryWeaponActive())
                    {
                        if (putAwayAS)
                            putAwayAS.PlayRandomClip();

                        cc.weaponArmsID = 0;
                        wpnHolster.primaryE.SetActive(false);
                        wpnHolster.primaryH.SetActive(true);
                    }
                    if (wpnHolster.SecondaryActive() && cc.weaponArmsID == 7)
                    {
                        if (putAwayAS)
                            putAwayAS.PlayRandomClip();

                       cc.weaponArmsID = 0;
                        wpnHolster.secondaryE.SetActive(false);
                        wpnHolster.secondaryH.SetActive(true);
                    }

                    if (wpnHolster.ArrowActive())
                    {
                        wpnHolster.arrowE.SetActive(false);
                    }

                    #endregion
                }

                #region Left hand weapon IK weight

                if (weaponArmsID == 3 || weaponArmsID == 4)
                {
                    cc.lhandLayerWeight -= 2 * Time.deltaTime;
                    if (cc.lhandLayerWeight < 0)
                    {
                       cc.weaponArmsID = 0;
                        cc.lhandLayerWeight = 0;
                    }
                }

                #endregion

                if (cc.rightArmdInfo.normalizedTime > 0.95f)
                {
                    cc.lhandLayerWeight = 0;
                    cc.rhandLayerWeight = 0;
                    isSheathing = false;
                }
            }

            if (leftArmdInfo.IsTag("Sheath L"))
            {
                if (cc.leftArmdInfo.normalizedTime >= 0.45f)
                {
                    // SWITCH --------------------------------------

                    #region Switch from shield to secondary

                    if (animator.GetInteger("EquipLeftHandID") == 1)
                    {
                        if (wpnHolster.ShieldActive())
                        {
                            wpnHolster.shieldE.SetActive(false);
                            wpnHolster.shieldH.SetActive(true);
                            //cc.weaponArmsID = wpnHolster.secondaryE.GetComponent<ItemData>().weaponArmsID;
                        }
                    }

                    #endregion

                    #region Switch from secondary to shield

                    if (animator.GetInteger("EquipLeftHandID") == 2)
                    {
                        if (wpnHolster.SecondaryActive())
                        {
                            wpnHolster.secondaryE.SetActive(false);
                            wpnHolster.secondaryH.SetActive(true);
                            //cc.weaponArmsID = wpnHolster.shieldE.GetComponent<ItemData>().weaponArmsID;
                        }

                        if (wpnHolster.ArrowActive() || wpnHolster.ArrowOnStringActive())
                        {
                            wpnHolster.arrowE.SetActive(false);
                            wpnHolster.arrowString.SetActive(false);
                        }
                    }

                    #endregion

                    // REMOVE--------------------------------------

                    #region Remove primary weapon to equip to secondary

                    if (animator.GetInteger("EquipLeftHandID") == 1)
                    {
                        if (wpnHolster.PrimaryWeaponActive())
                        {
                            wpnHolster.primaryE.SetActive(false);
                            wpnHolster.primaryH.SetActive(true);
                        }
                    }

                    #endregion

                    #region Remove 2 handed primary to equip shield

                    if (animator.GetInteger("EquipLeftHandID") == 2)
                    {
                        if (wpnHolster.PrimaryWeaponActive() && weaponArmsID != 1)
                        {
                            wpnHolster.primaryE.SetActive(false);
                            wpnHolster.primaryH.SetActive(true);
                        }
                    }

                    #endregion

                    // EQUIP--------------------------------------

                    #region Equip bow

                    if (animator.GetInteger("EquipLeftHandID") == 1)
                    {
                        if (wpnHolster.SecondaryHActive())
                        {
                            wpnHolster.secondaryE.SetActive(true);
                            wpnHolster.secondaryH.SetActive(false);
                        }
                    }

                    #endregion

                    #region Equip shield 

                    if (animator.GetInteger("EquipLeftHandID") == 2)
                    {
                        if (wpnHolster.ShieldHActive())
                        {
                            wpnHolster.shieldE.SetActive(true);
                            wpnHolster.shieldH.SetActive(false);
                        }
                    }

                    #endregion

                    if (cc.leftArmdInfo.normalizedTime >= 0.95f)
                    {
                        isSheathing = false;
                    }
                }
            }

            if (leftArmdInfo.IsTag("Unsheathe L"))
            {
                if (leftArmdInfo.normalizedTime >= 0.45f)
                {
                    // REMOVE--------------------------------------

                    #region Remove all equipped left handed weapons

                    if (wpnHolster.SecondaryActive())
                    {
                        if (putAwayAS)
                            putAwayAS.PlayRandomClip();

                        cc.weaponArmsID = 0;
                        wpnHolster.secondaryE.SetActive(false);
                        wpnHolster.secondaryH.SetActive(true);
                    }
                    if (wpnHolster.ShieldActive())
                    {
                        if (putAwayAS)
                            putAwayAS.PlayRandomClip();

                       cc.weaponArmsID = 0;
                        wpnHolster.shieldE.SetActive(false);
                        wpnHolster.shieldH.SetActive(true);
                    }

                    #endregion
                }

                if (leftArmdInfo.normalizedTime > 0.95f)
                {
                    cc.lhandLayerWeight = 0;
                    cc.rhandLayerWeight = 0;
                    isSheathing = false;
                }
            }
        }

        void RemoveArrowFromQuiver()
        {
            if (upperBodyInfo.normalizedTime > 0.5f)
            {
                for (int i = 0; i < wpnHolster.quiverArrows.Length; i++)
                {
                    wpnHolster.quiverArrows[i].gameObject.SetActive(true);
                    switch (inventoryM.bowAndArrowInv.counter[inventoryM.bowAndArrowInv.slotArrowEquipped])
                    {
                        case 1:
                            wpnHolster.arrow1.gameObject.SetActive(false);
                            wpnHolster.arrow2.gameObject.SetActive(false);
                            wpnHolster.arrow3.gameObject.SetActive(false);
                            wpnHolster.arrow4.gameObject.SetActive(false);
                            break;
                        case 2:
                            wpnHolster.arrow2.gameObject.SetActive(false);
                            wpnHolster.arrow3.gameObject.SetActive(false);
                            wpnHolster.arrow4.gameObject.SetActive(false);
                            break;
                        case 3:
                            wpnHolster.arrow3.gameObject.SetActive(false);
                            wpnHolster.arrow4.gameObject.SetActive(false);
                            break;
                        case 4:
                            wpnHolster.arrow4.gameObject.SetActive(false);
                            break;
                        default:
                            wpnHolster.arrow1.gameObject.SetActive(true);
                            wpnHolster.arrow2.gameObject.SetActive(true);
                            wpnHolster.arrow3.gameObject.SetActive(true);
                            wpnHolster.arrow4.gameObject.SetActive(true);
                            break;
                    }
                }
            }
        }

        void AirborneAnimation()
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("FallingLoop") && !grounded && !climbData.inPosition)
            {
                animator.applyRootMotion = false;
            }
        }

        void ClimbAnimation()
        {
            #region Enter Climb Grounded

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("EnterClimbGrounded"))
            {
                bracedTargetPosition = climbData.position - transform.rotation * handTarget.localPosition + ((transform.up * -0.20f) - (transform.forward * 0.08f));
                hangingTargetPosition = climbData.position - transform.rotation * handTarget.localPosition + ((transform.up * -0.1f) - (transform.forward * -0.35f));

                Quaternion posRotation = Quaternion.Euler(climbData.targetTransform.eulerAngles.x, climbData.targetTransform.eulerAngles.y, 0);

                transform.position = Vector3.Lerp(transform.position, ledgeStance == 0 ? bracedTargetPosition : hangingTargetPosition, 4 * Time.deltaTime);
                transform.rotation = Quaternion.Lerp(transform.rotation, posRotation, 4 * Time.deltaTime);

                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.2f)
                {
                    input = Vector3.zero;
                    inClimbEnter = false;
                    canClimbDown = false;
                    canJumpAcross = false;
                    rigidBody.useGravity = false;
                    transform.position = ledgeStance == 0 ? bracedTargetPosition : hangingTargetPosition;
                    transform.rotation = posRotation;
                    climbState = ClimbState.NA;
                }
            }

            #endregion

            #region Drop To Hang

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("DropToHang"))
            {

                bracedTargetPosition = climbData.position - transform.rotation * handTarget.localPosition + ((transform.up * -0.20f) - (transform.forward * 0.08f));
                hangingTargetPosition = climbData.position - transform.rotation * handTarget.localPosition + ((transform.up * -0.1f) - (transform.forward * -0.35f));

                Quaternion posRotation = Quaternion.Euler(climbData.targetTransform.eulerAngles.x, climbData.targetTransform.eulerAngles.y, 0);

                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.7f)
                {
                    var percentage = (((animator.GetCurrentAnimatorStateInfo(0).normalizedTime - 0.25f) / 0.8f) * 100f) * 0.01f;
                    transform.position = Vector3.Lerp(transform.position, ledgeStance == 0 ? bracedTargetPosition : hangingTargetPosition, 8 * Time.deltaTime);
                    transform.rotation = Quaternion.Lerp(transform.rotation, posRotation, percentage);
                }

                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
                {
                    input = Vector3.zero;
                    inClimbEnter = false;
                    canClimbDown = false;
                    canJumpAcross = false;
                    rigidBody.useGravity = false;
                    transform.position = ledgeStance == 0 ? bracedTargetPosition : hangingTargetPosition;
                    transform.rotation = posRotation;
                    climbState = ClimbState.NA;
                }
            }

            #endregion

            #region Climb Jump

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("ClimbJump"))
            {
                bracedTargetPosition = jumpClimbData.position - transform.rotation * handTarget.localPosition + ((transform.up * -0.20f) - (transform.forward * 0.08f));
                hangingTargetPosition = jumpClimbData.position - transform.rotation * handTarget.localPosition + ((transform.up * -0.1f) - (transform.forward * -0.35f));

                Quaternion posRotation = Quaternion.Euler(jumpClimbData.targetTransform.eulerAngles.x, jumpClimbData.targetTransform.eulerAngles.y, 0);

                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.2f)
                {
                    transform.position = Vector3.Lerp(transform.position, ledgeStance == 0 ? bracedTargetPosition : hangingTargetPosition, 8 * Time.deltaTime);
                    transform.rotation = Quaternion.Lerp(transform.rotation, posRotation, 8 * Time.deltaTime);
                }

                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.45f)
                {
                    inClimbJump = false;
                    transform.position = ledgeStance == 0 ? bracedTargetPosition : hangingTargetPosition;
                    transform.rotation = posRotation;
                }
            }

            #endregion

            #region Jump Backwards 

            animator.SetBool("LookBack", canJumpBackOnLedge);
            if (climbData.inPosition && ledgeStance == 0)
            {
                if (!canJumpBackOnLedge && input.z <= -0.8f && !animator.GetCurrentAnimatorStateInfo(0).IsName("LookBack") &&
                    !animator.GetCurrentAnimatorStateInfo(0).IsName("ClimbJump"))
                {
                    buttonTimer += 4 * Time.deltaTime;
                    if (buttonTimer > 1)
                    {
                        animator.CrossFadeInFixedTime("LookBack", 0.2f);
                        canJumpBackOnLedge = true;
                        buttonTimer = 0;
                    }
                }
                else
                    buttonTimer = 0;

                if (animator.GetCurrentAnimatorStateInfo(0).IsName("LookBack"))
                {
                    if (cc.rpgaeIM.PlayerControls.Movement.ReadValue<Vector2>().y == 0)
                    {
                        canJumpBackOnLedge = false;
                    }
                }
            }

            #endregion
        }

        void StrafingAnimation()
        {
            if (tpCam == null) return;

            if (tpCam.lockedOn)
            {
                isStrafing = tpCam.lockedOn;

                Vector3 toTarget = tpCam.lockedOnTarget.targetTransform.position - transform.position;
                Vector3 newDirection = Vector3.ProjectOnPlane(toTarget, Vector3.up);
                targetRotation = Quaternion.LookRotation(newDirection);

                Quaternion newRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                transform.rotation = newRotation;
            }
            else if(isStrafing)
            {
                if (isAiming) return;

                tpCam.angleH = tpCam.followTarget.eulerAngles.y;
            }
        }

        void DodgeAnimation()
        {
            if (isDodging)
            {
                // Extra velocity force
                var velY = transform.forward * (dodgeLastInput.z) * 2;
                var velX = transform.right * (dodgeLastInput.x) * 2.5f;
                rigidBody.linearVelocity = new Vector3(velX.x, rigidBody.linearVelocity.y, velY.z);
            }
        }

        void FinisherAnimation()
        {
            if (cc.fullBodyInfo.IsName("Finisher") && cc.fullBodyInfo.normalizedTime > 0.9f)
            {
                wpnHolster.primaryE.transform.localPosition = oldWeaponT;
                wpnHolster.primaryE.transform.localRotation = oldWeaponRot;

                foreach (Collider col in GetComponentsInChildren<Collider>())
                {
                    if (!col.GetComponent<CapsuleCollider>())
                    {
                        col.enabled = true;
                    }

                    rigidBody.useGravity = true;            
                    capCollider.enabled = true;
                    actionState = 0;                          
                }

                tpCam.transform.parent = null;
                tpCam.enabled = true;
            }
        }

        public virtual void MatchTarget(Vector3 matchPosition, Quaternion matchRotation, AvatarTarget target, MatchTargetWeightMask weightMask, float normalisedStartTime, float normalisedEndTime)
        {
            if (animator.isMatchingTarget || animator.IsInTransition(0))
                return;

            float normalizeTime = Mathf.Repeat(animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1f);

            if (normalizeTime > normalisedEndTime)
                return;

            animator.MatchTarget(matchPosition, matchRotation, target, weightMask, normalisedStartTime, normalisedEndTime);
        }

        public void LastFootStepEvent(int footStep)
        {
            if (footStep == -1)
                footStepCycle = -1;
            if (footStep == 1)
                footStepCycle = 1;

            animator.SetFloat("FootStepCycle", footStepCycle);
        }

        #region Animation Event 

        public void ActionAnimationTrigger()
        {
            if (triggerAction)
            {
                triggerAction.animator.SetBool("Activate", true);
                triggerAction.animator.SetInteger("Side", triggerAction.side);

                if (triggerAction.item)
                    triggerAction.activationSnd.PlayRandomClip();

                if (triggerAction.item != null)
                {
                    if (!itemObtained)
                    {
                        itemObtained = FindObjectOfType<ItemObtained>();
                    }

                    itemObtained.itemData = triggerAction.item.GetComponentInChildren<ItemData>();
                }
            }
        }

        public void CarryObjectEvent(int queue)
        {
            if (queue == 0)
            {
                breakbleAction.breakable.rigidBody.useGravity = false;
                breakbleAction.breakable.rigidBody.isKinematic = true;
                breakbleAction.breakable.lifted = true;
            }
            else if (queue == 1)
            {
                isCarrying = true;
                breakbleAction.breakable.lifted = false;
                breakbleAction.breakable.carried = true;
            }
            else if (queue == 2)
            {
                breakbleAction.breakable.carried = false;
                breakbleAction.breakable.dropped = true;
            }
            else if (queue == 3)
            {
                foreach (Collider col in breakbleAction.breakable.colliderGroup)
                {
                    col.enabled = true;
                }
                breakbleAction.breakable.rigidBody.useGravity = true;
                breakbleAction.breakable.rigidBody.isKinematic = false;
                breakbleAction.breakable.dropped = false;
            }
        }

        public void AttackPhaseContextEvent(string context)
        {
            if (GetComponent<ThirdPersonController>())
            {
                if (context == "Start")
                {
                }
                if (context == "End")
                {
                }
            }
        }

        public void PerfectDodgeEvent(float floatPhase)
        {
            if(floatPhase == 0)
            {
            }
        }

        public void UnarmedAttackEvent(string context)
        {
            #region Context

            if (context == "Left Punch")
            {
                hudM.ReduceEnegry(14, false);
                animator.GetBoneTransform(HumanBodyBones.LeftHand).GetComponent<HitBox>().hurtID = 1;
                animator.GetBoneTransform(HumanBodyBones.LeftHand).GetComponent<HitBox>().BeginAttack("Small Blood Hit");
            }

            if (context == "Right Punch")
            {
                hudM.ReduceEnegry(14, false);
                animator.GetBoneTransform(HumanBodyBones.RightHand).GetComponentInChildren<HitBox>().hurtID = 1;
                animator.GetBoneTransform(HumanBodyBones.RightHand).GetComponentInChildren<HitBox>().BeginAttack("Small Blood Hit");
            }

            if (context == "Right Kick")
            {
                hudM.ReduceEnegry(14, false);
                animator.GetBoneTransform(HumanBodyBones.RightFoot).GetComponent<HitBox>().hurtID = 1;
                animator.GetBoneTransform(HumanBodyBones.RightFoot).GetComponent<HitBox>().BeginAttack("Small Blood Hit");
            }

            if (context == "Heavy Attack")
            {
                hudM.ReduceEnegry(14 * 2, false);
                animator.GetBoneTransform(HumanBodyBones.RightHand).GetComponent<HitBox>().hurtID = 2;
                animator.GetBoneTransform(HumanBodyBones.RightHand).GetComponent<HitBox>().BeginAttack("Medium Blood Hit");
            }

            if (context == "End")
            {
                animator.GetBoneTransform(HumanBodyBones.LeftHand).GetComponentInChildren<HitBox>().EndAttack();
                animator.GetBoneTransform(HumanBodyBones.RightHand).GetComponentInChildren<HitBox>().EndAttack();
                animator.GetBoneTransform(HumanBodyBones.LeftFoot).GetComponentInChildren<HitBox>().EndAttack();
                animator.GetBoneTransform(HumanBodyBones.RightFoot).GetComponentInChildren<HitBox>().EndAttack();
            }

            #endregion

            rigidBody.linearVelocity = Vector3.zero;
        }

        public void ArmedAttackEvent(string context)
        {
            if(context == "Light Attack")
            {
                wpnHolster.WeaponStamina();
                wpnHolster.primaryE.GetComponent<HitBox>().hurtID = 1;
                wpnHolster.primaryE.GetComponent<HitBox>().BeginAttack("Small Blood Hit");
            }

            if (context == "Heavy Attack")
            {
                wpnHolster.WeaponStamina();
                wpnHolster.primaryE.GetComponent<HitBox>().hurtID = 2;
                wpnHolster.primaryE.GetComponent<HitBox>().BeginAttack("Medium Blood Hit");
            }

            if (context == "Ability Attack")
            {
                wpnHolster.WeaponStamina();
                wpnHolster.primaryE.GetComponent<HitBox>().hurtID = 3;
                wpnHolster.primaryE.GetComponentInChildren<HitBox>().BeginAttack("Medium Blood Hit");
            }

            if (context == "Aerial Attack")
            {
                canMove = true;
                isJumping = false;
                animator.applyRootMotion = true;
                wpnHolster.WeaponStamina();
                wpnHolster.primaryE.GetComponent<HitBox>().hurtID = 3;
                wpnHolster.primaryE.GetComponentInChildren<HitBox>().BeginAttack("Medium Blood Hit");
            }

            if (context == "Shield Attack")
            {
                wpnHolster.WeaponStamina();
                wpnHolster.shieldE.GetComponent<HitBox>().hurtID = 1;
                wpnHolster.shieldE.GetComponentInChildren<HitBox>().BeginAttack("Medium Blood Hit");
            }

            if (context == "Small Blood Hit")
            {
                wpnHolster.primaryE.GetComponentInChildren<HitBox>().BeginAttack("Small Blood Hit");
            }

            if (context == "Medium Blood Hit")
            {
                wpnHolster.primaryE.GetComponentInChildren<HitBox>().BeginAttack("Medium Blood Hit");
            }

            if (context == "Small Bleedout Hit")
            {
                wpnHolster.primaryE.GetComponentInChildren<HitBox>().BeginAttack("Small Bleedout Hit");
            }

            if (context == "Medium Bleedout Hit")
            {
                wpnHolster.primaryE.GetComponentInChildren<HitBox>().BeginAttack("Medium Bleedout Hit");
            }

            if (context == "End")
            {
                if (wpnHolster.ShieldActive())
                    wpnHolster.shieldE.GetComponentInChildren<HitBox>().EndAttack();
                if (wpnHolster.PrimaryWeaponActive())
                    wpnHolster.primaryE.GetComponentInChildren<HitBox>().EndAttack();
            }

            rigidBody.linearVelocity = Vector3.zero;
        }

        public void WeaponProjectileEvent()
        {
            if (cc.upperBodyID == 0 && isAiming)
            {
                GameObject projectile = Instantiate(wpnHolster.primaryE.GetComponentInChildren<ItemData>().throwDist.projectile, projectileSpawnPoint, aimRotationPoint) as GameObject;
                projectile.GetComponentInChildren<HitBox>().isProjectile = true;
                projectile.GetComponentInChildren<HitBox>().isAttacking = true;
                projectile.GetComponentInChildren<HitBox>().BeginAttack("Medium Blood Hit");
                projectile.SetActive(true);

                if (wpnHolster.primaryE.GetComponentInChildren<HitBox>().weaponAudio != null)
                {
                    for (int i = 0; i < wpnHolster.primaryE.GetComponentInChildren<HitBox>().weaponAudio.Length; i++)
                        if (wpnHolster.primaryE.GetComponentInChildren<HitBox>().weaponAudio[i].audioClip.GetComponent<RandomAudioPlayer>().name == "HitEnemyAS")
                            wpnHolster.primaryE.GetComponentInChildren<HitBox>().weaponAudio[i].audioClip.PlayRandomClip();
                }
            }
        }

        public void WeaponThrowEvent()
        {
            GameObject weapon = Instantiate(wpnHolster.primaryD, projectileSpawnPoint, aimRotationPoint) as GameObject;
            weapon.GetComponentInChildren<HitBox>().isProjectile = true;
            weapon.GetComponentInChildren<HitBox>().isAttacking = true;
            weapon.GetComponentInChildren<HitBox>().weaponTrail = true;
            weapon.GetComponentInChildren<ItemData>().inInventory = false;
            weapon.GetComponentInChildren<ItemData>().itemActive = true;
            weapon.GetComponentInChildren<HitBox>().wpnAtk = weapon.GetComponentInChildren<ItemData>().maxWpnAtk;
            weapon.GetComponentInChildren<HitBox>().BeginAttack("Medium Blood Hit");
            weapon.gameObject.SetActive(true);
            weapon.GetComponentInChildren<HitBox>().tag = "Player";
            weapon.GetComponentInChildren<HitBox>().targetLayers = targetLayer;

            inventoryM.RemoveNonStackableItem(ref inventoryM.weaponInv.itemData, ref inventoryM.weaponInv.image, ref inventoryM.weaponInv.highLight, ref inventoryM.weaponInv.outLineBorder, ref inventoryM.weaponInv.statValueBG, ref inventoryM.weaponInv.quantity,
            ref inventoryM.weaponInv.statValue, ref inventoryM.weaponInv.slotWeaponEquipped, ref inventoryM.weaponInv.removeNullSlots, ref inventoryM.weaponInv.counter, ref inventoryM.weaponInv.statCounter);

            inventoryM.DeactiveWeapons();
            inventoryM.DeactiveWeaponsHP();
            inventoryM.itemDescription.inventoryD = ItemDescription.InventoryDescription.Null;

           cc.weaponArmsID = 0;
            cc.upperBodyID = 0;
            cc.preWeaponArmsID = 0;
            cc.actionState = 0;

            isAiming = false;
            canShoot = false;
        }

        public void SpecialAttackEvent(string context)
        {
            #region Find Closest Enemy

            List<HumanoidBehavior> targetables = new List<HumanoidBehavior>();
            Collider[] m_colliders = Physics.OverlapSphere(transform.position, 5);
            foreach (Collider m_collder in m_colliders)
            {
                HumanoidBehavior targetable = m_collder.GetComponent<HumanoidBehavior>();
                if (targetable != null)
                {
                    targetables.Add(targetable);
                }
            }

            float hypotenuse;
            float smallesthypotenuse = Mathf.Infinity;
            HumanoidBehavior closestTargetable = null;
            foreach (HumanoidBehavior targetable in targetables)
            {
                hypotenuse = CalculateHypotenuse(targetable.transform.position);
                if (smallesthypotenuse > hypotenuse)
                {
                    closestTargetable = targetable;
                    smallesthypotenuse = hypotenuse;
                }
            }

            #endregion

            actionState = 0;

            if (closestTargetable == null || !wpnHolster.PrimaryWeaponActive() || 
            wpnHolster.primaryE.GetComponent<HitBox>().effects.signatureEffect == null 
            && attackPower > 0 && !preventAtkInteruption) return;

            if(context == "Start")
            {
                GameObject specialAttackPartilce = Instantiate(wpnHolster.primaryE.GetComponent<HitBox>().effects.signatureEffect.gameObject, closestTargetable.transform.position, closestTargetable.transform.rotation) as GameObject;
                specialAttackPartilce.GetComponentInChildren<HitBox>().isAttacking = true;
                specialAttackPartilce.GetComponentInChildren<HitBox>().PlayRandomSound("HitEnemyAS", true);
                specialAttackPartilce.GetComponentInChildren<HitBox>().BeginAttack("Medium Blood Hit");
            }
            attackPower = 0;
            preventAtkInteruption = true;
        }

        public void AttackEndEvent(string context)
        {
            #region Unarmed 

            if (!wpnHolster.PrimaryWeaponActive() && !wpnHolster.ShieldActive())
            {
                if (fullBodyInfo.IsName("Attack A"))
                {
                    animator.GetBoneTransform(HumanBodyBones.RightHand).GetComponent<HitBox>().EndAttack();
                }

                if (fullBodyInfo.IsName("Attack B"))
                {
                    animator.GetBoneTransform(HumanBodyBones.LeftHand).GetComponent<HitBox>().EndAttack();
                }

                if (fullBodyInfo.IsName("Attack C"))
                {
                    animator.GetBoneTransform(HumanBodyBones.RightFoot).GetComponent<HitBox>().EndAttack();
                }

                if (fullBodyInfo.IsName("Heavy Attack"))
                {
                    animator.GetBoneTransform(HumanBodyBones.RightHand).GetComponent<HitBox>().EndAttack();
                }
            }

            #endregion

            #region Primary

            if (fullBodyInfo.IsTag("Light Attack") || fullBodyInfo.IsTag("Heavy Attack") || fullBodyInfo.IsTag("Ability Attack") && wpnHolster.PrimaryWeaponActive())
            {
                if (!wpnHolster.PrimaryWeaponActive()) return;

                wpnHolster.primaryE.GetComponent<HitBox>().trailActive = false;

                wpnHolster.primaryE.GetComponent<HitBox>().EndAttack();
                wpnHolster.primaryE.GetComponent<HitBox>().isAttacking = false;
            }

            #endregion

            #region Shield

            if (fullBodyInfo.IsName("Shield Attack") && wpnHolster.ShieldActive())
            {
                wpnHolster.shieldE.GetComponent<HitBox>().trailActive = false;

                wpnHolster.shieldE.GetComponent<HitBox>().EndAttack();
                wpnHolster.shieldE.GetComponent<HitBox>().isAttacking = false;
            }

            #endregion
        }

        public void ThrownSpearOnFinisher(int queue)
        {
            if (queue == 1)
            {
                wpnHolster.primaryE.transform.parent = null;
                wpnHolster.primaryE.transform.position = new Vector3(21.674f, 0.9100872f, -9.091f);
                wpnHolster.primaryE.transform.rotation = Quaternion.Euler(-407.2f, -546f, -180.3f);
            }
            if (queue == 0)
            {
                wpnHolster.primaryE.transform.SetParent(GameObject.Find("WeaponER").transform);
                wpnHolster.primaryE.transform.localPosition = oldWeaponT;
                wpnHolster.primaryE.transform.localRotation = oldWeaponRot;
            }
        }

        public void DamageEvent(string context)
        {
        }

        public void ReloadEvent()
        {
            wpnHolster.secondaryE.GetComponent<HitBox>().PlayRandomSound("RifleReloadAS", false);
        }

        public void ShootEvent()
        {
        }

        #endregion
    }
}
