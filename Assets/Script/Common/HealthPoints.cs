using System.Collections;
using System.Collections.Generic;
using RPGAE.CharacterController;
using UnityEngine;

public partial class HealthPoints : MonoBehaviour
{
    [Header("HEALTH SETTINGS")]
    public float curHealthPoints;
    public float maxHealthPoints;
    [Header("STUN SETTINGS")]
    public float curStunPoints;
    public float maxStunPoints;
    [Tooltip("Time that this gameObject is invulnerable for, after receiving damage.")]
    public float invulnerabiltyTime;

    [Tooltip("The angle from the which that damageable is hitable. Always in the world XZ plane, with the forward being rotate by hitForwardRoation")]
    [Range(0.0f, 360.0f)]
    public float hitAngle = 360.0f;
    [Tooltip("Allow to rotate the world forward vector of the damageable used to define the hitAngle zone")]
    [Range(0.0f, 360.0f)]
    public float hitForwardRotation = 360.0f;
    public bool useBossHPOverlay;

    [Tooltip("When this gameObject is damaged, these other gameObjects are notified.")]
    public List<MonoBehaviour> onDamageMessageReceivers;

    #region Private 

    public DamageData tempData;
    private HUDManager hudM;
    private ThirdPersonController cc;
    private ItemData itemData;

    [HideInInspector] public bool isInvulnerable;
    protected float timeSinceLastHit = 0.0f;
    [HideInInspector] public float stickOnEffectTimer;
    [HideInInspector] public bool stickOnActive;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        ResetDamage();

        hudM = FindObjectOfType<HUDManager>();
        cc = FindObjectOfType<ThirdPersonController>();
        itemData = GetComponent<ItemData>();

        if (itemData)
        {
            if (onDamageMessageReceivers.Count == 0 || onDamageMessageReceivers == null)
                onDamageMessageReceivers.Add(itemData);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isInvulnerable)
        {
            timeSinceLastHit += Time.deltaTime;
            if (timeSinceLastHit > invulnerabiltyTime)
            {
                timeSinceLastHit = 0.0f;
                isInvulnerable = false;
            }
        }

        WeaponDamage();
        StickOnDamage();
    }

    void StickOnDamage()
    {
        if (!stickOnActive) return;

        if (stickOnEffectTimer > 0)
        {
            stickOnEffectTimer -= Time.deltaTime;
            curHealthPoints -= 10 * Time.deltaTime;
        }

        if(stickOnEffectTimer <= 0)
        {
            var messageType = curHealthPoints <= 0 ? MsgType.DEAD : MsgType.DAMAGED;
            for (var i = 0; i < onDamageMessageReceivers.Count; ++i)
            {
                var receiver = onDamageMessageReceivers[i] as DamageReceiver;
                receiver.OnReceiveMessage(messageType, this, tempData);
            }
            stickOnActive = false;
        }
    }

    void WeaponDamage()
    {
        if (itemData == null) return;

        if (itemData.itemType == ItemData.ItemType.Weapon)
        {
            if (GetComponent<HitBox>() != null && GetComponent<HitBox>().isAttacking && GetComponent<HitBox>().equipped)
            {
                var messageType = curHealthPoints <= 0 ? MsgType.DEAD : MsgType.DAMAGED;

                for (var i = 0; i < onDamageMessageReceivers.Count; ++i)
                {
                    var receiver = onDamageMessageReceivers[i] as DamageReceiver;
                    receiver.OnReceiveMessage(messageType, this, tempData);
                }
            }
        }

        if (itemData.itemType == ItemData.ItemType.Bow)
        {
            if (GetComponent<HitBox>() != null && GetComponent<HitBox>().isAttacking && GetComponent<HitBox>().equipped)
            {
                var messageType = curHealthPoints <= 0 ? MsgType.DEAD : MsgType.DAMAGED;

                for (var i = 0; i < onDamageMessageReceivers.Count; ++i)
                {
                    var receiver = onDamageMessageReceivers[i] as DamageReceiver;
                    receiver.OnReceiveMessage(messageType, this, tempData);
                }
            }
        }
    }

    public void ResetDamage()
    {
        curHealthPoints = maxHealthPoints;
        curStunPoints = maxStunPoints;
        isInvulnerable = false;
        timeSinceLastHit = 0.0f;
    }

    public void ApplyDamage(DamageData data)
    {
        if (curHealthPoints <= 0)
        {
            curHealthPoints = 0;
            var messageType1 = MsgType.DEAD;
            for (var i = 0; i < onDamageMessageReceivers.Count; ++i)
            {
                var receiver = onDamageMessageReceivers[i] as DamageReceiver;
                receiver.OnReceiveMessage(messageType1, this, data);
            }
            return;
        }

        if(GetComponent<AIController>() != null)
        {
            if (curStunPoints <= 0 && GetComponent<AIController>().finisherInProcess)
            {
                #region Hit SFX

                if (GetComponent<ThirdPersonController>())
                {
                    if (data.damager.GetComponent<ItemData>() != null)
                    {
                        if (data.damager.GetComponent<ItemData>().itemType == ItemData.ItemType.Weapon)
                            data.damager.GetComponent<HitBox>().PlayRandomSound("HitEnemyAS", false);
                        if (data.damager.GetComponent<ItemData>().itemType == ItemData.ItemType.Arrow)
                            data.damager.GetComponent<HitBox>().PlayRandomSound("HitEnemyAS", true);
                    }
                    else
                        data.damager.GetComponent<HitBox>().PlayRandomSound("HitEnemyAS", false);
                }
                else if (GetComponent<AIController>())
                {
                    if (data.damager.GetComponent<ItemData>() != null)
                    {
                        if (data.damager.GetComponent<ItemData>().itemType == ItemData.ItemType.Weapon)
                            data.damager.GetComponent<HitBox>().PlayRandomSound("HitEnemyAS", false);
                        if (data.damager.GetComponent<ItemData>().itemType == ItemData.ItemType.Arrow)
                            data.damager.GetComponent<HitBox>().PlayRandomSound("HitEnemyAS", true);
                    }
                    else
                        data.damager.GetComponent<HitBox>().PlayRandomSound("HitEnemyAS", false);
                }

                #endregion

                var messageType2 = MsgType.DAMAGED;
                for (var i = 0; i < onDamageMessageReceivers.Count; ++i)
                {
                    var receiver = onDamageMessageReceivers[i] as DamageReceiver;
                    receiver.OnReceiveMessage(messageType2, this, data);
                }
                return;
            }
        }

        if (isInvulnerable)
        {
            return;
        }

        Vector3 forward = transform.forward;
        forward = Quaternion.AngleAxis(hitForwardRotation, transform.up) * forward;

        #region Shield Defence

        bool vulnerability = cc.IsAnimatorTag("Light Attack") && cc.fullBodyInfo.IsName("Heavy Charge") &&
        cc.IsAnimatorTag("Heavy Attack") && cc.IsAnimatorTag("Shield Attack");

        bool conditionsToBeHurt = !cc.isBlocking;
        // Prevents enemy damage on a 280 frontal angle while blocking with a shield equipped.
        if (gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (conditionsToBeHurt && !vulnerability)
            {
                hitAngle = 360f;
                forward = -transform.forward;
            }
            else
            {
                hitAngle = 280f;
                forward = -transform.forward;
            }
        }

        #endregion

        // Project the direction to damager to the plane formed by the direction of damage.
        Vector3 positionToDamager = data.damageSource - transform.position;
        positionToDamager -= transform.up * Vector3.Dot(transform.up, positionToDamager);

        if (Vector3.Angle(forward, positionToDamager) > hitAngle * 0.5f)
            return;

        if (invulnerabiltyTime > 0)
            isInvulnerable = true;

        #region Hit SFX

        if (GetComponent<ThirdPersonController>())
        {
            if (data.damager.GetComponent<ItemData>() != null)
            {
                if (data.damager.GetComponent<ItemData>().itemType == ItemData.ItemType.Weapon)
                    data.damager.GetComponent<HitBox>().PlayRandomSound("HitEnemyAS", false);
                if (data.damager.GetComponent<ItemData>().itemType == ItemData.ItemType.Arrow)
                    data.damager.GetComponent<HitBox>().PlayRandomSound("HitEnemyAS", true);
            }
            else
                data.damager.GetComponent<HitBox>().PlayRandomSound("HitEnemyAS", false);
        }
        else if (GetComponent<AIController>())
        {
            if (data.damager.GetComponent<ItemData>() != null)
            {
                if (data.damager.GetComponent<ItemData>().itemType == ItemData.ItemType.Weapon)
                    data.damager.GetComponent<HitBox>().PlayRandomSound("HitEnemyAS", false);
                if (data.damager.GetComponent<ItemData>().itemType == ItemData.ItemType.Arrow)
                    data.damager.GetComponent<HitBox>().PlayRandomSound("HitEnemyAS", true);
            }
            else
                data.damager.GetComponent<HitBox>().PlayRandomSound("HitEnemyAS", false);
        }

        #endregion

        #region Damage Resistance

        // Player damage resistance to enemy attacks
        if (gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            data.wpnAtk -= (int)hudM.defence;
            data.arrowAtk -= (int)hudM.defence;
            data.shdAtk -= (int)hudM.defence;

            /*
            if (cc.wpnHolster.headArmorE != null)
                maxArmorDefence += cc.wpnHolster.headArmorE.GetComponent<ItemData>().maxArm;
            if (cc.wpnHolster.chestArmorE != null)
                maxArmorDefence += cc.wpnHolster.chestArmorE.GetComponent<ItemData>().maxArm;
            if (cc.wpnHolster.legsArmorE != null)
                maxArmorDefence += cc.wpnHolster.legsArmorE.GetComponent<ItemData>().maxArm;
            if (cc.wpnHolster.amuletArmorE != null)
                maxArmorDefence += cc.wpnHolster.amuletArmorE.GetComponent<ItemData>().maxArm;

            int maxArmorDefence = 0;

            data.wpnAtk -= maxArmorDefence;
            data.wpnCritHit -= maxArmorDefence;
            data.arrowAtk -= maxArmorDefence;
            data.arrowHdShot -= maxArmorDefence;
            data.arrowCritHit -= maxArmorDefence;
            data.shdAtk -= maxArmorDefence;
            data.shdCritHit -= maxArmorDefence;
            */
        }

        #endregion

        curStunPoints -= data.wpnStun;
        curStunPoints -= data.arrowStun;
        curStunPoints -= data.shdStun;

        curHealthPoints -= data.wpnAtk;
        curHealthPoints -= data.arrowAtk;
        curHealthPoints -= data.shdAtk;

        for (int i = 0; i < data.damager.GetComponent<HitBox>().attackPoints.Length; i++)
        {
            if (data.damager.GetComponent<HitBox>().effects.meleeEffectsHit != null)
            {
                data.damager.GetComponent<HitBox>().CreateParticle(data.damager.GetComponent<HitBox>().effects.meleeEffectsHit,
                data.damager.GetComponent<HitBox>().attackPoints[i].attackRoot.transform.position);
            }
        }

        tempData = data;
        var messageType = curHealthPoints <= 0 ? MsgType.DEAD : MsgType.DAMAGED;

        for (var i = 0; i < onDamageMessageReceivers.Count; ++i)
        {
            var receiver = onDamageMessageReceivers[i] as DamageReceiver;
            receiver.OnReceiveMessage(messageType, this, data);
        }

        if (data.damager.GetComponent<HitBox>().isAttacking)
            data.damager.GetComponent<HitBox>().isAttacking = false;
    }

#if UNITY_EDITOR
    /*
   private void OnDrawGizmosSelected()
   {
        if(GetComponent<ThirdPersonController>())
        {
            Vector3 forward = transform.forward;
            forward = Quaternion.AngleAxis(hitForwardRotation, transform.up) * forward;

            if (Event.current.type == EventType.Repaint)
            {
                UnityEditor.Handles.color = Color.blue;
                UnityEditor.Handles.ArrowHandleCap(0, transform.position, Quaternion.LookRotation(forward), 1.0f,
                    EventType.Repaint);
            }

            UnityEditor.Handles.color = new Color(1.0f, 0.0f, 0.0f, 0.5f);
            forward = Quaternion.AngleAxis(-hitAngle * 0.5f, transform.up) * forward;
            UnityEditor.Handles.DrawSolidArc(transform.position, transform.up, forward, hitAngle, 1.0f);
        }
   }
   */
#endif
}
