
using System.Collections;
using System.Collections.Generic;
using RPGAE.CharacterController;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    [Header("WEAPON SETTINGS")]
    public LayerMask targetLayers;
    public bool weaponTrail;
    public bool stickOnContact;
    public bool destroyOnContact;
    [Tooltip("Weapon projectile velocity while unequipped.")]
    public Vector3 projectileVel;

    public AttackPoint[] attackPoints = new AttackPoint[0];
    public Effects effects;
    public Audio[] weaponAudio;

    [Header("SET AI DAMAGE TO PLAYER/AUTO SETS FOR PLAYER")]
    public int wpnAtk;
    public int wpnSpd;
    public int wpnStun;
    public int wpnCritHit;
    public int wpnDura;
    public int arrowAtk;
    public int arrowSpd;
    public int arrowStun;
    public int arrowCritHit;
    public int arrowHdShot;
    public int bowDura;
    public int shdAtk;
    public int shdSpd;
    public int shdStun;
    public int shdCritHit;
    public int shdBlock;
    public int shdDura;

    #region Melee Weapon

    [System.Serializable]
    public class AttackPoint
    {
        public float radius;
        public Vector3 offset;
        public Transform attackRoot;
    }

    [System.Serializable]
    public class Effects
    {
        public ParticleSystem meleeEffectsHit;
        public ParticleSystem obstacleEffectHit;
        public ParticleSystem stickOnContactEffectHit;
        public ParticleSystem signatureEffect;
    }

    [System.Serializable]
    public class Audio
    {
        public RandomAudioPlayer audioClip;
    }

    protected Vector3[] m_PreviousPos = null;
    const int PARTICLE_COUNT = 1;
    protected ParticleSystem[] m_ParticlesPool = new ParticleSystem[PARTICLE_COUNT];
    protected int m_CurrentParticle = 0;
    protected static RaycastHit[] s_RaycastHitCache = new RaycastHit[32];
    protected static Collider[] s_ColliderCache = new Collider[32];

    #endregion

    #region Private 

    private bool oneShot;
    [HideInInspector] public bool oneShotHP;
    [HideInInspector] public bool equipped;
    [HideInInspector] public bool isAttacking = false;
    [HideInInspector] public bool isAbleToParry = false;
    [HideInInspector] public bool trailActive;
    [HideInInspector] public bool isProjectile;
    [HideInInspector] public string bloodEffectName;
    public float hurtID;

    [HideInInspector] public HitBox enemyHitBox;
    [HideInInspector] public WeaponHolster wpnHolster;
    [HideInInspector] public AIController aiController;
    [HideInInspector] public TrailRenderer trailEquipped;     
    [HideInInspector] public TrailRenderer trailUnequipped;
    [HideInInspector] public ThirdPersonController cc;
    [HideInInspector] public HUDManager hudM;

    private Vector3 weaponVel;
    private Vector3 lastPos;
    private Rigidbody rigidBody;

    #endregion 

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        trailUnequipped = GetComponent<TrailRenderer>();
        trailEquipped = GetComponentInChildren<TrailRenderer>();
        hudM = FindObjectOfType<HUDManager>();

        SetWeaponStats();
        SetWeaponHolster();
        SetWeaponRangeType();
    }

    void SetWeaponHolster()
    {
        if (!cc)
            cc = GetComponentInParent<ThirdPersonController>();
        if (!aiController)
            aiController = GetComponentInParent<AIController>();
        if (aiController && !wpnHolster)
            wpnHolster = aiController.wpnHolster;
        else if (cc && !wpnHolster)
            wpnHolster = cc.wpnHolster;
    }

    void Update()
    {
        WeaponTrail();
        SetWeaponStats();
    }

    void FixedUpdate()
    {
        ProjectileCollision();
        HitBoxType(isAttacking);
    }

    public void BeginAttack(string _bloodEffectName)
    {
        trailActive = true;
        isAttacking = true;

        PlayRandomSound("SwingAS", false);

        bloodEffectName = _bloodEffectName;
        isAbleToParry = true;
        oneShot = false;

        m_PreviousPos = new Vector3[attackPoints.Length];

        for (int i = 0; i < attackPoints.Length; ++i)
        {
            Vector3 worldPos = attackPoints[i].attackRoot.position +
                               attackPoints[i].attackRoot.TransformVector(attackPoints[i].offset);
            m_PreviousPos[i] = worldPos;
        }
    }

    public void EndAttack()
    {
        trailActive = false;
        isAttacking = false;
    }

    public void DeactivateProjectile()
    {
        isAttacking = false;
        isProjectile = false;
        GetComponent<ItemData>().enabled = false;
        GetComponent<HitBox>().enabled = false;
        GetComponent<ItemInteract>().enabled = false;
        Destroy(GetComponent<Rigidbody>());

        if (GetComponentInChildren<MeshRenderer>())
            GetComponentInChildren<MeshRenderer>().enabled = false;
        if (GetComponentInChildren<SkinnedMeshRenderer>())
            GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        if (GetComponentInChildren<ParticleSystem>())
            GetComponentInChildren<ParticleSystem>().Stop();
        if (GetComponentInChildren<Light>())
            GetComponentInChildren<Light>().enabled = false;
    }

    #region Set Weapon 

    void SetWeaponStats()
    {
        if (wpnHolster)
        {
            if (aiController)
            {
                if (GetComponentInParent<HumanoidBehavior>())
                    targetLayers = GetComponentInParent<HumanoidBehavior>().targetScanner.targetLayer;
                if (GetComponentInParent<LuBehavior>())
                    targetLayers = GetComponentInParent<LuBehavior>().targetScanner.targetLayer;
            }

            ItemData weaponSettings = GetComponent<ItemData>();
            HealthPoints weaponDurability = GetComponent<HealthPoints>();
            if (weaponSettings != null)
            {
                switch (weaponSettings.itemType)
                {
                    case ItemData.ItemType.Weapon:
                        if (wpnHolster.PrimaryWeaponActive())
                        {
                            wpnAtk = wpnHolster.primaryE.GetComponent<ItemData>().maxWpnAtk;
                            wpnSpd = wpnHolster.primaryE.GetComponent<ItemData>().maxWpnSpd;
                            wpnStun = wpnHolster.primaryE.GetComponent<ItemData>().maxWpnStun;
                            wpnCritHit = wpnHolster.primaryE.GetComponent<ItemData>().maxWpnCritHit;
                            wpnDura = wpnHolster.primaryE.GetComponent<ItemData>().maxWpnDura;
                            weaponDurability.maxHealthPoints = wpnDura;
                            if (!oneShotHP)
                            {
                                weaponDurability.ResetDamage();
                                oneShotHP = true;
                            }
                        }
                        break;
                    case ItemData.ItemType.Bow:
                        if (wpnHolster.SecondaryActive())
                        {
                            arrowAtk = wpnHolster.secondaryE.GetComponent<ItemData>().maxBowAtk;
                            arrowSpd = wpnHolster.secondaryE.GetComponent<ItemData>().maxBowSpd;
                            arrowStun = wpnHolster.secondaryE.GetComponent<ItemData>().maxBowStun;
                            arrowCritHit = wpnHolster.secondaryE.GetComponent<ItemData>().maxBowCritHit;
                            arrowHdShot = wpnHolster.secondaryE.GetComponent<ItemData>().maxBowHdShot;
                            bowDura = wpnHolster.secondaryE.GetComponent<ItemData>().maxBowDura;
                            weaponDurability.maxHealthPoints = bowDura;
                            if (!oneShotHP)
                            {
                                weaponDurability.ResetDamage();
                                oneShotHP = true;
                            }
                        }
                        break;
                    case ItemData.ItemType.Shield:
                        if (wpnHolster.ShieldActive())
                        {
                            shdAtk = wpnHolster.shieldE.GetComponent<ItemData>().maxShdAtk;
                            shdSpd = wpnHolster.shieldE.GetComponent<ItemData>().maxShdSpd;
                            shdStun = wpnHolster.shieldE.GetComponent<ItemData>().maxShdStun;
                            shdCritHit = wpnHolster.shieldE.GetComponent<ItemData>().maxShdCritHit;
                            shdBlock = wpnHolster.shieldE.GetComponent<ItemData>().maxShdBlock;
                            shdDura = wpnHolster.shieldE.GetComponent<ItemData>().maxShdDura;
                            weaponDurability.maxHealthPoints = shdDura;
                            if (!oneShotHP)
                            {
                                weaponDurability.ResetDamage();
                                oneShotHP = true;
                            }
                        }
                        break;
                }
            }
        }
        else
        {
            // Set Weapon stat for thrown melee weapons
            ItemData weaponSettings = GetComponent<ItemData>();
            if (weaponSettings == null) return;

            switch (weaponSettings.itemType)
            {
                case ItemData.ItemType.Weapon:
                    wpnAtk = weaponSettings.maxWpnAtk;
                    wpnSpd = weaponSettings.maxWpnSpd;
                    wpnStun = weaponSettings.maxWpnStun;
                    wpnCritHit = weaponSettings.maxWpnCritHit;
                    break;
            }
        }
    }

    void WeaponTrail()
    {
        if (trailEquipped == null) return;

        if (lastPos != transform.position)
        {
            weaponVel = transform.position - lastPos;
            weaponVel /= Time.deltaTime;
            lastPos = transform.position;
        }

        if (!weaponTrail) return;

        // Trigger the Weapon trail.
        if (trailActive)
        {
            trailEquipped.time = 0.2f;
            trailEquipped.minVertexDistance = 0.1f;
            trailEquipped.enabled = true;
        }
        else
        {
            trailEquipped.enabled = false;
        }

        // Weapon trail during projectile state.
        if (weaponVel.sqrMagnitude > 5)
        {
            if (isProjectile)
            {
                trailUnequipped.time = 1;
                trailUnequipped.minVertexDistance = 0.1f;

                // Look at the directional its moving in.
                transform.LookAt(transform.position + rigidBody.linearVelocity);
            }
        }
    }

    void ProjectileCollision()
    {
        RaycastHit hit;
        if (isProjectile)
        {
            int bitMask = ~((1 << 8) | (1 << 9));
            if (Physics.Raycast(transform.position, transform.forward, out hit, GetBoxColliderLength(), bitMask))
            {
                if (!stickOnContact)
                    return;

                rigidBody.isKinematic = true;
                rigidBody.constraints = RigidbodyConstraints.FreezeAll;
                isProjectile = false;

                if ((targetLayers.value & (1 << hit.transform.gameObject.layer)) == 0)
                    return;

                if (destroyOnContact) Destroy(gameObject);
            }
        }
    }

    float GetBoxColliderLength()
    {
        if(GetComponent<ItemData>() != null && GetComponent<ItemData>().itemType == ItemData.ItemType.Weapon)
        {
            BoxCollider boxC = GetComponentInChildren<BoxCollider>();

            Vector3 size = boxC.size;
            float rayLength = size.z + 0.01f;

            return rayLength;
        }

        if (GetComponent<ItemData>() != null && GetComponent<ItemData>().itemType == ItemData.ItemType.Arrow)
        {
            return 1;
        }

        return 1;
    }

    void SetWeaponRangeType()
    {
        if (isProjectile)
        {
            if(cc != null)
            {
                if (GetComponent<ItemData>() != null && GetComponent<ItemData>().itemType == ItemData.ItemType.Weapon)
                {
                    switch (GetComponent<ItemData>().throwDist.throwRange)
                    {
                        case ItemData.ThrowRange.ShortRange:
                            GetComponent<Rigidbody>().mass = 10;
                            break;
                        case ItemData.ThrowRange.MedRange:
                            GetComponent<Rigidbody>().mass = 6;
                            break;
                        case ItemData.ThrowRange.LongRange:
                            GetComponent<Rigidbody>().mass = 4;
                            break;
                        case ItemData.ThrowRange.Endless:
                            GetComponent<Rigidbody>().useGravity = false;
                            break;
                    }
                }

                if (GetComponent<ItemData>() != null && GetComponent<ItemData>().itemType == ItemData.ItemType.Arrow)
                {
                    switch (FindObjectOfType<ThirdPersonController>().wpnHolster.secondaryE.GetComponentInChildren<ItemData>().bowRange.bowRangeType)
                    {
                        case ItemData.BowRangeType.ShortRange:
                            GetComponent<Rigidbody>().mass = 6.5f;
                            break;
                        case ItemData.BowRangeType.MedRange:
                            GetComponent<Rigidbody>().mass = 5;
                            break;
                        case ItemData.BowRangeType.LongRange:
                            GetComponent<Rigidbody>().mass = 3.5f;
                            break;
                        case ItemData.BowRangeType.Endless:
                            GetComponent<Rigidbody>().useGravity = false;
                            break;
                    }
                }
            }
            if (GetComponentInChildren<ItemInteract>() != null)
                GetComponentInChildren<ItemInteract>().interactUI.fadeUI.canvasGroup.alpha = 0;

            rigidBody.AddForce((transform.forward * projectileVel.z) + (transform.up * projectileVel.y), ForceMode.Impulse);
        }
    }

    public void PlayRandomSound(string weaponAS, bool detachFromObject)
    {
        if (weaponAudio != null)
        {
            for (int i = 0; i < weaponAudio.Length; i++)
            {
                if (weaponAudio[i]?.audioClip?.GetComponent<RandomAudioPlayer>()?.gameObject?.name == weaponAS)
                {
                    weaponAudio[i].audioClip.PlayRandomClip();

                    if (detachFromObject)
                    {
                        if (GetComponent<ItemData>()?.equipped == true) return;

                        weaponAudio[i].audioClip.transform.SetParent(null, true);
                        Destroy(weaponAudio[i].audioClip, weaponAudio[i].audioClip.clip == null ? 0.0f : weaponAudio[i].audioClip.clip.length + 0.5f);
                    }
                }
            }
        }
    }


    public void AerialAttackReaction()
    {
        PlayRandomSound("AerialLandAS", false);
        CreateParticle(effects.obstacleEffectHit, attackPoints[0].attackRoot.transform.position);
        oneShot = true;
    }

    public void CreateParticle(ParticleSystem effect, Vector3 attPoint)
    {
        if (effect != null)
        {
            m_ParticlesPool[m_CurrentParticle] = Instantiate(effect);
            m_ParticlesPool[m_CurrentParticle].transform.position = attPoint;
            m_ParticlesPool[m_CurrentParticle].time = 0;
            m_ParticlesPool[m_CurrentParticle].Play();
            m_CurrentParticle = (m_CurrentParticle + 1) % PARTICLE_COUNT;
        }
    }

    #endregion

    #region Damage 

    void HitBoxType(bool isActive)
    {
        if (isActive)
        {
            for (int i = 0; i < attackPoints.Length; ++i)
            {
                if (attackPoints[i]?.attackRoot == null)
                    continue;

                AttackPoint pts = attackPoints[i];
                Vector3 worldPos = pts.attackRoot.position + pts.attackRoot.TransformVector(pts.offset);
                Vector3 attackVector = worldPos - m_PreviousPos[i];

                if (attackVector.magnitude < 0.001f)
                {
                    attackVector = Vector3.forward * 0.0001f;
                }

                Ray r = new Ray(worldPos, attackVector.normalized);
                int contacts = Physics.SphereCastNonAlloc(r, pts.radius, s_RaycastHitCache, attackVector.magnitude, ~0, QueryTriggerInteraction.Ignore);

                for (int c = 0; c < contacts; ++c)
                {
                    Collider col = s_RaycastHitCache[c].collider;
                    if (col != null)
                    {
                        if (isAttacking)
                            CheckParry(col, pts);
                        if (isAttacking)
                            CheckDamage(col, pts);
                    }
                }
                m_PreviousPos[i] = worldPos;
            }
        }
    }


    private bool CheckParry(Collider other, AttackPoint pts)
    {
        if (gameObject.layer != LayerMask.NameToLayer("Player"))
        {
            // Prevent anything but the player from triggering this action
            return false;
        }

        if ((targetLayers.value & (1 << other.gameObject.layer)) == 0)
        {
            // Hit an object that is not in our layer, this end the attack. we "bounce" off it
            return false;
        }

        AIController WeaponToParryOff = other.GetComponentInChildren<AIController>();
        if (WeaponToParryOff == null)
        {
            return false;
        }

        if (wpnHolster.ShieldActive() && cc.wpnHolster.shieldE.GetComponent<HitBox>().isAttacking)
        {
            if (!cc.slowDown && WeaponToParryOff.wpnHolster.primaryE.GetComponent<HitBox>().isAttacking)
            {
                WeaponToParryOff.stunned = true;
                WeaponToParryOff.wpnHolster.primaryE.GetComponent<HitBox>().trailActive = false;
                //WeaponToParryOff.animator.CrossFadeInFixedTime("Hit_Deflect_Start", 0.3f);
                Time.timeScale = 0.01f;
                cc.slowMotionTimer = 0.9f;
                cc.slowDown = true;
                cc.slowDownVolume.weight = 1;
                if (cc.slowMotionStartAS)
                    cc.slowMotionStartAS.PlayRandomClip();

                PlayRandomSound("BlockAS", false);
                CreateParticle(effects.signatureEffect, pts.attackRoot.transform.position);
            }
        }
        return true;
    }

    private bool CheckDamage(Collider other, AttackPoint pts)
    {
        // Hitting an obstacle will cancel the attack 
        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle") && isAttacking)
        {
            PlayRandomSound("HitObstacleAS", false);
            CreateParticle(effects.obstacleEffectHit, pts.attackRoot.transform.position);
            isAttacking = false;
        }

        // Aerial Attack Effect 
        if (cc && cc.IsAnimatorTag("AerialAttack") && cc.grounded && !oneShot)
        {
            PlayRandomSound("AerialLandAS", false);
            CreateParticle(effects.obstacleEffectHit, pts.attackRoot.transform.position);
            oneShot = true;
        }

        HealthPoints d = other.GetComponentInParent<HealthPoints>();

        if (d == null)
        {
            return false;
        }

        if ((targetLayers.value & (1 << other.gameObject.layer)) == 0)
        {
            //hit an object that is not in our layer, this end the attack. we "bounce" off it
            return false;
        }

        HealthPoints.DamageData data;

        data.damager = this;
        data.damageSource = transform.position;

        if (cc)
        {
            if(GetComponent<ItemData>() != null)
            {
                data.wpnAtk = CalculateDamage();
                data.wpnStun = CalculateStunDamage();
                data.arrowAtk = CalculateDamage() + CalculateHeadShotDamage(other);
                data.arrowStun = CalculateStunDamage();
                data.shdAtk = CalculateDamage();
                data.shdStun = CalculateStunDamage();

                data.wpnAtk += (int)hudM.strength;
                data.arrowAtk += (int)hudM.strength;
                data.shdAtk += (int)hudM.strength;
                data.isCritical = 0;
                data.playerDefense = 0;
                d.ApplyDamage(data);
            }
            else
            {
                // Unarmed Melee Attack
                data.wpnAtk = 25;
                data.wpnStun = 25;
                data.arrowAtk = 0;
                data.arrowStun = 0;
                data.shdAtk = 0;
                data.shdStun = 0;

                data.wpnAtk += (int)hudM.strength;
                data.arrowAtk += (int)hudM.strength;
                data.shdAtk += (int)hudM.strength;
                data.isCritical = 0;
                data.playerDefense = 0;
                d.ApplyDamage(data);
            }
        }
        else
        {
            data.wpnAtk = wpnAtk;
            data.wpnStun = 0;
            data.arrowAtk = arrowAtk;
            data.arrowStun = 0;
            data.shdAtk = shdAtk;
            data.shdStun = 0;

            data.isCritical = 0;
            data.playerDefense = 0;
            d.ApplyDamage(data);
        }
        return true;
    }

    #region CalculateDamage

    int CalculateDamage()
    {
        if (wpnAtk > 0 && GetComponent<ItemData>().itemType == ItemData.ItemType.Weapon)
        {
            int potentialDamage = Random.Range((wpnAtk / 2) + 15, wpnAtk);
            potentialDamage += CalculateCrit(potentialDamage);
            return potentialDamage;
        }
        else if (arrowAtk > 0 && GetComponent<ItemData>().itemType == ItemData.ItemType.Arrow)
        {
            int potentialDamage = Random.Range((arrowAtk / 2) + 15, arrowAtk);
            potentialDamage += CalculateCrit(potentialDamage);
            return potentialDamage;
        }
        else if (shdAtk > 0 && GetComponent<ItemData>().itemType == ItemData.ItemType.Shield)
        {
            int potentialDamage = Random.Range((shdAtk / 2) + 15, shdAtk);
            potentialDamage += CalculateCrit(potentialDamage);
            return potentialDamage;
        }
        return 0;
    }

    int CalculateStunDamage()
    {
        if (wpnStun > 0 && GetComponent<ItemData>().itemType == ItemData.ItemType.Weapon)
        {
            int potentialDamage = Random.Range((wpnStun / 2) + 15, wpnStun);
            potentialDamage += CalculateCrit(potentialDamage);
            return potentialDamage;
        }
        else if (arrowStun > 0 && GetComponent<ItemData>().itemType == ItemData.ItemType.Arrow)
        {
            int potentialDamage = Random.Range((arrowStun / 2) + 15, arrowStun);
            potentialDamage += CalculateCrit(potentialDamage);
            return potentialDamage;
        }
        else if (shdStun > 0 && GetComponent<ItemData>().itemType == ItemData.ItemType.Shield)
        {
            int potentialDamage = Random.Range((shdStun / 2) + 15, shdStun);
            potentialDamage += CalculateCrit(potentialDamage);
            return potentialDamage;
        }
        return 0;
    }

    int CalculateCrit(int damage)
    {
        if (wpnCritHit > 0 && GetComponent<ItemData>().itemType == ItemData.ItemType.Weapon)
        {
            if (Random.value <= wpnCritHit / 400f)
            {
                int critDamage = (int)(damage * Random.Range(0.25f, 0.50f));
                return critDamage;
            }
        }
        else if (arrowCritHit > 0 && GetComponent<ItemData>().itemType == ItemData.ItemType.Arrow)
        {
            if (Random.value <= arrowCritHit / 400f)
            {
                int critDamage = (int)(damage * Random.Range(0.25f, 0.50f));
                return critDamage;
            }
        }
        else if (shdCritHit > 0 && GetComponent<ItemData>().itemType == ItemData.ItemType.Shield)
        {
            if (Random.value <= shdCritHit / 400f)
            {
                int critDamage = (int)(damage * Random.Range(0.25f, 0.50f));
                return critDamage;
            }
        }
        return 0;
    }

    int CalculateHeadShotDamage(Collider other)
    {
        if (other.tag == "HeadShot")
        {
            if (arrowHdShot > 0 && GetComponent<ItemData>().itemType == ItemData.ItemType.Arrow)
            {
                int potentialDamage = Random.Range((arrowHdShot / 2) + 15, arrowHdShot);
                return potentialDamage;
            }
        }
        return 0;
    }

    #endregion

    #endregion

    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < attackPoints.Length; ++i)
        {
            AttackPoint pts = attackPoints[i];

            if (pts.attackRoot != null)
            {
                Vector3 worldPos = pts.attackRoot.TransformVector(pts.offset);
                Gizmos.color = new Color(1.0f, 1.0f, 1.0f, 0.4f);
                Gizmos.DrawSphere(pts.attackRoot.position + worldPos, pts.radius);
            }
        }
    }
}
