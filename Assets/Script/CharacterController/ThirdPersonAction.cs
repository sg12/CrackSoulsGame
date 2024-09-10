using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGAE.CharacterController
{
    public class ThirdPersonAction : ThirdPersonMotor
    {
        [Header("ACTION")]
        [Tooltip("Tag of the object you want to access")]
        public string actionTag = "Action";

        [Header("--- Debug Only ---")]
        public TriggerAction triggerAction;
        public List<TriggerAction> inQueActions;
        public TriggerAction breakbleAction;
        [Tooltip("Check this to enter the debug mode")]
        [HideInInspector] public bool actionPerformed;
        [HideInInspector] public bool canTriggerAction;
        [HideInInspector] public bool isPlayingAnimation;

        [HideInInspector] public bool isUsingLadder;
        [HideInInspector] public bool isExitingLadder;
        private bool triggerExitOnce;
        public bool collidedWithTrigger;
        private bool performingAction;

        public void ActionUpdate()
        {
            UseLadder();
            TriggerActionInput();
            CarryObjectActionInput();

            if (triggerAction != null && cc.baseLayerInfo.IsTag("Action") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= triggerAction.endExitTimeAnimation)
            {
                if (!canSwim && !isSwimming)
                {
                    EnableGravityAndCollision(0f);         // enable again the gravity and collision
                    actionState = 0;                       // set actionState 1 to avoid falling transitions
                    performingAction = false;
                    collidedWithTrigger = false;
                }
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.name == "WaterTrigger")
            {
                if (isSwimming || climbData.inPosition) return;

                distY = other.bounds.max.y;
                canSwim = true;
                canMove = true;
            }

            // Save and load local data transition
            if (other.GetComponentInParent<SceneTransitionDestination>() != null)
            {
                if (other.gameObject.name == "TransitionCollider" && PlayerPrefs.GetInt("SceneInTransitionDataRef") == 0)
                {
                    systemM.canLoadScene = true;

                    cc.canMove = false;
                    cc.input = Vector3.zero;
                    cc.targetDirection = Vector3.zero;

                    sceneTD = other.GetComponentInParent<SceneTransitionDestination>();

                    // Save current local room data and load the destionation room and his spawn position
                    systemM.SaveAndLoadTransitionScene(sceneTD.sceneName, sceneTD.SpawnPointIDRef);
                }
            }
            
            // Since we saved already save position when the player previously transitioned to the next room, 
            // now repositon the player when he comes back from that collider to its desired position to avoid 
            // transition back...
            if (other.gameObject.name == "RepositionCollider" && PlayerPrefs.GetInt("SceneInTransitionDataRef") == 1)
            {
                transform.position = other.GetComponentInParent<SceneTransitionDestination>().
                spawnReposition[PlayerPrefs.GetInt("SpawnPointIDRef")].position;
                transform.rotation = other.GetComponentInParent<SceneTransitionDestination>().
                spawnReposition[PlayerPrefs.GetInt("SpawnPointIDRef")].rotation;
            }
            

            if (other.gameObject.CompareTag(actionTag) && !isPlayingAnimation)
            {
                CheckForTriggerAction(other);
            }

            if (other.name == "DeathCollider")
            {
                GetComponent<HealthPoints>().curHealthPoints = 0;

                HealthPoints.DamageData data;

                data.damager = this;
                data.damageSource = other.transform.position;

                data.wpnAtk = 0;
                data.wpnStun = 0;
                data.arrowAtk = 0;
                data.arrowStun = 0;
                data.shdAtk = 0;
                data.shdStun = 0;

                cc.GetComponent<HealthPoints>().ApplyDamage(data);
            }

            #region Ladder

            if (other.gameObject.CompareTag(actionTag) && !isPlayingAnimation)
            {
                if (!isUsingLadder) return;
                if (cc.baseLayerInfo.IsName("EnterLadderTop") || cc.baseLayerInfo.IsName("EnterLadderBottom")) return;

                if (triggerAction)
                {
                    if (other.gameObject.name == "TriggerLadderActionBottom")
                    {
                        // exit ladder when reach the bottom by pressing the cancelInput or pressing down at
                        if (cc.rpgaeIM.PlayerControls.Interact.triggered || (input.z <= -0.05f && !triggerExitOnce))
                        {
                            triggerExitOnce = true;
                            cc.animator.CrossFadeInFixedTime("ExitLadderBottom", 0.1f);             // trigger the animation clip        
                        }
                    }
                    else if (other.gameObject.name == "TriggerLadderActionTop")   // exit the ladder from the top
                    {
                        if ((cc.input.z >= 0.05f) && !triggerExitOnce)         // trigger the exit animation by pressing up
                        {
                            triggerExitOnce = true;
                            cc.animator.CrossFadeInFixedTime("ExitLadderTop", 0.1f);             // trigger the animation clip
                        }
                    }
                }
            }

            #endregion
        }

        void OnTriggerExit(Collider other)
        {
            if (other.name == "WaterTrigger")
            {
                if (waterHeightLevel < offsetToSurf && isSwimming)
                {
                    dived = false;
                    isSwimming = false;
                    rigidBody.useGravity = true;

                    distY = 0.0f;
                    actionState = 0;
                    waterHeightLevel = 0;

                    GameObject effect = Instantiate(waterDripsPFX, transform.position, transform.rotation) as GameObject;
                    effect.SetActive(true);
                    waterDripPos = effect.transform;
                    effect.transform.parent = transform;
                }
                canSwim = false;
            }

            if (other.gameObject.CompareTag(actionTag))
            {
                ResetPlayerSettings();
                if(triggerAction.buttonType == TriggerAction.ButtonType.InteractButton)
                {
                    collidedWithTrigger = false;
                }
                if (!isUsingLadder && !cc.IsAnimatorTag("Action") && !isPlayingAnimation)
                {
                    isUsingLadder = false;
                }
            }
        }

        void TriggerActionInput()
        {
            if (triggerAction == null) return;

            if (triggerAction.autoAction && actionConditions() && collidedWithTrigger ||
            triggerAction.buttonType == TriggerAction.ButtonType.ActionButton && cc.rpgaeIM.PlayerControls.Action.triggered && 
            actionConditions() && !performingAction && collidedWithTrigger)
            {
                performingAction = true;
                PutAwayWeapon();
                TriggerAnimation();
                collidedWithTrigger = false;
            }

            if (!triggerAction.isActive && triggerAction.buttonType == TriggerAction.ButtonType.InteractButton && 
            cc.rpgaeIM.PlayerControls.Interact.triggered && actionConditions() && LockedConditions() && !performingAction && 
            collidedWithTrigger)
            {
                performingAction = true;
                PutAwayWeapon();
                TriggerAnimation();
                TriggerEnterLadder();
                TriggerActionReference();
                collidedWithTrigger = false;
            }
        }

        void PutAwayWeapon()
        {
            if (triggerAction.name == "MovingPlatform" && triggerAction.GetComponentInChildren<QuestWayPoint>() != null) return;

            if (wpnHolster.PrimaryWeaponActive())
            {
                animator.SetInteger("EquipRightHandID", 0);
                animator.SetTrigger("EquipRightHand");
                isSheathing = true;
            }

            if (wpnHolster.PrimaryWeaponActive() && wpnHolster.ShieldActive())
            {
                animator.SetInteger("EquipRightHandID", 0);
                animator.SetTrigger("EquipRightHand");
                animator.SetInteger("EquipLeftHandID", 0);
                animator.SetTrigger("EquipLeftHand");
                isSheathing = true;
            }

            if (wpnHolster.SecondaryActive())
            {
                animator.SetInteger("EquipLeftHandID", 0);
                animator.SetTrigger("EquipLeftHand");
                isSheathing = true;
            }

            if (wpnHolster.SecondaryActive() && wpnHolster.ArrowOnStringActive())
            {
                animator.SetInteger("EquipLeftHandID", 0);
                animator.SetTrigger("EquipLeftHand");

                animator.SetInteger("EquipRightHandID", 0);
                animator.SetTrigger("EquipRightHand");
                isSheathing = true;
            }
        }

        public bool actionConditions()
        {
            if (!isJumping || cc.baseLayerInfo.IsTag("Action") || isPlayingAnimation)
            {
                return true;
            }
            return false;
        }

        public bool LockedConditions()
        {
            if (!collidedWithTrigger) return false;

            foreach (ItemData item in inventoryM.keyInv.itemData)
            {
                if (item.itemName == "Key" && triggerAction.interactionType == TriggerAction.InteractionType.Key && triggerAction.isLocked)
                {
                    triggerAction.interactionType = TriggerAction.InteractionType.Normal;
                    triggerAction.isLocked = false;
                    inventoryM.RemoveStackableItem(ref inventoryM.keyInv.itemData, ref inventoryM.keyInv.image,
                    ref inventoryM.keyInv.highLight, ref inventoryM.keyInv.outLineBorder, ref inventoryM.keyInv.statValueBG, ref inventoryM.keyInv.quantity,
                    ref inventoryM.keyInv.statValue, ref inventoryM.keyInv.slotNum, ref inventoryM.keyInv.counter, ref inventoryM.keyInv.statCounter,
                    ref inventoryM.keyInv.removeNullSlots, 1);
                    return true;
                }
            }

            if (!triggerAction.isLocked)
                return true;

            switch (triggerAction.interactionType)
            {
                case TriggerAction.InteractionType.Key:
                    infoMessage.info.text = "A key is required to open this.";
                    break;
                case TriggerAction.InteractionType.Triggered:
                    infoMessage.info.text = "Find a trigger to unlock this.";
                    break;
                case TriggerAction.InteractionType.permanentlyLocked:
                    infoMessage.info.text = "It will not budge...";
                    break;
            }
            return false;
        }

        void TriggerAnimation()
        {
            if (triggerAction.isActive) return;
            if (triggerAction.isActive && triggerAction.animator != null && triggerAction.item != null)
            {
                // Prevent opening treasure chest that has already been used
                return;
            }

            // trigger the animation behaviour & match target
            if (!string.IsNullOrEmpty(triggerAction.playAnimation))
            {
                isPlayingAnimation = true;
                animator.CrossFadeInFixedTime(triggerAction.playAnimation, 0.1f);    // trigger the action animation clip
            }

            if (breakbleAction != null)
                breakbleAction.onTriggerEnterUI = false;

            // destroy the triggerAction if checked with destroyAfter
            if (triggerAction.destroyAfter)
                StartCoroutine(DestroyDelay(triggerAction));
        }

        void TriggerActionReference()
        {
            if (triggerAction.isActive) return;
            if (triggerAction.isActive && triggerAction.animator != null && triggerAction.item != null)
            {
                // Prevent opening treasure chest that has already been used
                return;
            }

            if (triggerAction.interactionType == TriggerAction.InteractionType.PressureStep ||
                triggerAction.interactionType == TriggerAction.InteractionType.PressureObjectWeight)
                return;

            if (!triggerAction.breakable)
                triggerAction.breakable = triggerAction.GetComponentInParent<Breakable>();

            if (breakbleAction != null)
                breakbleAction.onTriggerEnterUI = false;

            triggerAction.onTriggerEnterUI = false;

            if (triggerAction.animator)
            {
                inQueActions.Add(triggerAction);

                if (triggerAction.playAnimation != "OpenChest")
                {
                    triggerAction.animator.SetBool("Activate", true);
                    triggerAction.animator.SetInteger("Side", triggerAction.side);
                }
            }

            for (int i = 0; i < inQueActions.Count; i++)
            {
                if (inQueActions[i].isActiveTimer > 0)
                    StartCoroutine(ActiveTimer(inQueActions[i].isActiveTimer));
            }

            if (triggerAction.transformer)
            {
                for (int i = 0; i < triggerAction.thisGroupOfTriggerActions.Length; i++)
                {
                    triggerAction.thisGroupOfTriggerActions[i].isActive = true;
                }
                if (triggerAction.transformer.transformType == Transformer.TransformType.ToEndPointAndBack)
                {
                    if (triggerAction.transformer.invert == true)
                        triggerAction.transformer.invert = false;
                }
                triggerAction.transformer.pause = false;
            }

            // Play particle system
            if (triggerAction.particleS)
            {
                triggerAction.particleS.gameObject.SetActive(false);
                triggerAction.particleS.gameObject.SetActive(true);
            }

            // Play activation sound
            if (triggerAction.activationSnd && !triggerAction.item)
                triggerAction.activationSnd.PlayRandomClip();

            triggerAction.isActive = true;
            performingAction = false;
        }

        public virtual IEnumerator ActiveTimer(float timer)
        {
            for (int i = 0; i < inQueActions.Count; i++)
            {
                for (int ii = 0; ii < inQueActions[i].thisGroupOfTriggerActions.Length; ii++)
                {
                    inQueActions[i].thisGroupOfTriggerActions[ii].isActive = true;
                }
            }

            yield return new WaitForSeconds(timer);

            for (int i = 0; i < inQueActions.Count; i++)
            {
                for (int ii = 0; ii < inQueActions[i].thisGroupOfTriggerActions.Length; ii++)
                {
                    inQueActions[i].thisGroupOfTriggerActions[ii].isActive = false;
                }
                if (inQueActions[i].animator)
                    inQueActions[i].animator.SetBool("Activate", false);
            }
        }

        #region Ladder 

        void TriggerEnterLadder()
        {
            if (triggerAction.playAnimation == "EnterLadderTop" || triggerAction.playAnimation == "EnterLadderBottom")
            {
                isUsingLadder = true;
                cc.actionState = 1;     // set actionState 1 to avoid falling transitions               
            }
        }

        void UseLadder()
        {
            cc.animator.SetFloat("InputVertical", cc.input.z, 0.25f, Time.deltaTime);

            // enter ladder behaviour           
            if (cc.animator.GetCurrentAnimatorStateInfo(0).IsName("EnterLadderTop") || cc.animator.GetCurrentAnimatorStateInfo(0).IsName("EnterLadderBottom"))
            {
                if (triggerAction.useTriggerRotation)
                {
                    // smoothly rotate the character to the target
                    transform.rotation = Quaternion.Lerp(transform.rotation, triggerAction.matchTarget.transform.rotation, cc.animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
                }

                if (triggerAction.matchTarget != null)
                {
                    // use match target to match the Y and Z target 
                    cc.MatchTarget(triggerAction.matchTarget.transform.position, triggerAction.matchTarget.transform.rotation, AvatarTarget.Root, new MatchTargetWeightMask(new Vector3(1, 1, 1), 0), triggerAction.startMatchTarget, triggerAction.endMatchTarget);
                }
            }

            // exit ladder behaviour
            isExitingLadder = cc.baseLayerInfo.IsName("ExitLadderTop") || cc.baseLayerInfo.IsName("ExitLadderBottom");

            if (isExitingLadder && cc.baseLayerInfo.normalizedTime >= 0.8f)
            {
                isUsingLadder = false;
                triggerExitOnce = false;
                if (triggerAction)
                    triggerAction.isActive = false;
                // after playing the animation we reset some values
                ResetPlayerSettings();
            }
        }

        #endregion

        public virtual IEnumerator DestroyDelay(TriggerAction triggerAction)
        {
            var _triggerAction = triggerAction;
            yield return new WaitForSeconds(_triggerAction.destroyDelay);
            ResetPlayerSettings();
            Destroy(_triggerAction.gameObject);
        }

        public virtual void AnimationBehaviour()
        {
            if (playingAnimation)
            {
                if (triggerAction.matchTarget != null)
                {
                    // use match target to match the Y and Z target 
                    cc.MatchTarget(triggerAction.matchTarget.transform.position, triggerAction.matchTarget.transform.rotation, triggerAction.avatarTarget,
                        new MatchTargetWeightMask(triggerAction.matchTargetMask, 0), triggerAction.startMatchTarget, triggerAction.endMatchTarget);
                }

                if (triggerAction.useTriggerRotation)
                {
                    // smoothly rotate the character to the target
                    transform.rotation = Quaternion.Lerp(transform.rotation, triggerAction.transform.rotation, animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
                }

                if (triggerAction.resetPlayerSettings && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= triggerAction.endExitTimeAnimation)
                {
                    // after playing the animation we reset some values
                    ResetPlayerSettings();
                }

                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= triggerAction.endExitTimeAnimation)
                {
                    // after playing the animation we reset some values
                    ResetPlayerSettings();
                }

                if (triggerAction.interactiveUI)
                    triggerAction.interactiveUI.GetComponent<InteractWorldSpaceUI>().fadeUI.FadeTransition(0, 0, 0.5f);
            }
        }

        protected virtual bool playingAnimation
        {
            get
            {
                if (triggerAction == null)
                {
                    isPlayingAnimation = false;
                    return false;
                }

                if (!isPlayingAnimation && !string.IsNullOrEmpty(triggerAction.playAnimation) && animator.GetCurrentAnimatorStateInfo(0).IsName(triggerAction.playAnimation))
                {
                    isPlayingAnimation = true;
                    ApplyPlayerSettings();
                }
                else if (isPlayingAnimation && !string.IsNullOrEmpty(triggerAction.playAnimation) && !animator.GetCurrentAnimatorStateInfo(0).IsName(triggerAction.playAnimation))
                    isPlayingAnimation = false;

                return isPlayingAnimation;
            }
        }

        protected virtual void CheckForTriggerAction(Collider other)
        {
            var _triggerAction = other.GetComponent<TriggerAction>();
            if (!_triggerAction) return;

            var dist = Vector3.Distance(transform.forward, _triggerAction.transform.forward);

            if (!_triggerAction.activeFromForward || dist <= 0.8f)
            {
                triggerAction = _triggerAction;
                if (_triggerAction.breakable != null)
                    breakbleAction = _triggerAction;

                if (!triggerAction.breakable)
                    triggerAction.breakable = triggerAction.GetComponentInParent<Breakable>();

                if (!cc.baseLayerInfo.IsTag("Action") && !isPlayingAnimation)
                    collidedWithTrigger = true;
                else
                    collidedWithTrigger = false;

                canTriggerAction = true;
            }
            else
            {
                canTriggerAction = false;
                collidedWithTrigger = false;
            }
        }

        protected virtual void ApplyPlayerSettings()
        {
            if (triggerAction.disableGravity)
            {
                rigidBody.useGravity = false;              // disable gravity of the player
                rigidBody.linearVelocity = Vector3.zero;
                grounded = true;                           // ground the character so that we can run the root motion without any issues
                animator.SetBool("Grounded", true);        // also ground the character on the animator so that he won't float after finishes the climb animation
                actionState = 1;                           // set actionState 1 to avoid falling transitions
            }
            if (triggerAction.disableCollision)
                capCollider.isTrigger = true;              // disable the collision of the player if necessary
        }

        public virtual void ResetPlayerSettings()
        {
            if (!playingAnimation || animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= triggerAction.endExitTimeAnimation)
            {
                if (!canSwim && !isSwimming)
                {
                    EnableGravityAndCollision(0f);         // enable again the gravity and collision
                    actionState = 0;                       // set actionState 1 to avoid falling transitions
                }
            }
            actionPerformed = false;
            canTriggerAction = false;
        }

        void CarryObjectActionInput()
        {
            if (!breakbleAction || !breakbleAction.breakable || cc.IsAnimatorTag("Action")) return;

            if (isSwimming)
            {
                DetachCarryObject();

                cc.actionState = 2;
                isCarrying = false;
            }

            if (cc.rpgaeIM.PlayerControls.Interact.triggered && isCarrying && !breakbleAction.breakable.lifted &&
                breakbleAction.breakable.carried)
            {
                DetachCarryObject();

                cc.actionState = 1;
                isCarrying = false;
            }

            if (cc.rpgaeIM.PlayerControls.Attack.triggered && isCarrying)
            {
                DetachCarryObject();

                cc.actionState = 2;
                isCarrying = false;
                breakbleAction.breakable.canBreak = true;

                breakbleAction.breakable.rigidBody.AddForce((transform.forward * breakbleAction.breakable.throwStrength.z) +
                (transform.up * breakbleAction.breakable.throwStrength.y), ForceMode.Impulse);
            }
        }

        void DetachCarryObject()
        {
            foreach (Collider col in breakbleAction.breakable.colliderGroup)
            {
                col.enabled = true;
            }

            breakbleAction.breakable.TurnOffTag(true);
            breakbleAction.breakable.transform.parent = null;
            breakbleAction.breakable.rigidBody.useGravity = true;
            breakbleAction.breakable.rigidBody.isKinematic = false;
            breakbleAction.breakable.carried = false;
            breakbleAction.breakable.lifted = false;
            breakbleAction.isActive = false;
        }
    }
}
