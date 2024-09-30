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
    // private ThirdPersonController cc; // Отключено, так как не используется
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
        // cc = FindObjectOfType<ThirdPersonController>(); // Закомментировано, так как не используется
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
                if (receiver != null)
                {
                    receiver.OnReceiveMessage(messageType1, this, data);
                }
            }
            return;
        }

        if (isInvulnerable)
        {
            return;
        }

        curHealthPoints -= data.wpnAtk;
        Debug.Log("NPC получил урон: " + data.wpnAtk + " Очки здоровья: " + curHealthPoints);

        tempData = data;
        var messageType = curHealthPoints <= 0 ? MsgType.DEAD : MsgType.DAMAGED;

        for (var i = 0; i < onDamageMessageReceivers.Count; ++i)
        {
            var receiver = onDamageMessageReceivers[i] as DamageReceiver;
            if (receiver != null)
            {
                receiver.OnReceiveMessage(messageType, this, data);
            }
        }

        if (data.damager != null) // Проверка на null для damager
        {
            HitBox hitBox = data.damager.GetComponent<HitBox>(); // Проверка наличия HitBox
            if (hitBox != null && hitBox.isAttacking)
            {
                hitBox.isAttacking = false;
            }
        }
        else
        {
            Debug.LogWarning("Damager is null in ApplyDamage.");
        }

        if (curHealthPoints <= 0)
        {
            Debug.Log("NPC is dead.");
        }
    }


}
