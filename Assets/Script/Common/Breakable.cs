using System.Collections;
using System.Collections.Generic;
using RPGAE.CharacterController;
using UnityEngine;

public class Breakable : MonoBehaviour, DamageReceiver
{
    [Header("BREAK OBJECT SETTINGS")]
    public float OffSetToHandX = -0.03f;
    public float OffSetToHandY = 0.47f;
    public float OffSetToHandZ = 0.3f;
    public bool canBeCarried;
    [Tooltip("Break object OnCollision with other object")]
    public bool breakOnCollision = true;
    public Transform brokenObject;
    public Vector3 throwStrength;
    public GameObject[] itemDrops;
    private TriggerAction[] triggerActions;

    [Header("AUDIO")]
    public RandomAudioPlayer landAudio;
    public RandomAudioPlayer breakAudio;

    public bool grounded;
    public float groundedRayDistance = 0.25f;
    public RaycastHit currentGroundInfo;
    public LayerMask groundedLayer;

    #region Private 

    [HideInInspector] public Transform carryPosition;

    [HideInInspector] public bool canBreak;
    public bool lifted;
    public bool carried;
    public bool dropped;

    [HideInInspector] public Collider _collider;
    [HideInInspector] public Rigidbody rigidBody;
    [HideInInspector] public HealthPoints hp;

    [HideInInspector] public Collider[] colliderGroup;

    private ThirdPersonController cc;
    private bool landSound;

    #endregion

    void Start()
    {
        this.gameObject.layer = LayerMask.NameToLayer("Breakable");

        _collider = GetComponent<Collider>();
        rigidBody = GetComponent<Rigidbody>();
        colliderGroup = GetComponentsInChildren<Collider>();
        cc = FindObjectOfType<ThirdPersonController>();
        triggerActions = GetComponentsInChildren<TriggerAction>();
        hp = GetComponent<HealthPoints>();

        carryPosition = cc.animator.GetBoneTransform(HumanBodyBones.LeftHand).transform;
    }

    void Update()
    {
        CarryBehavior();
        CheckGround();
        //CheckGrounded();
    }

    void OnCollisionEnter(Collision other)
    {
        if (breakOnCollision && rigidBody && canBreak && !other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(BreakObject());
        }
    }

    IEnumerator BreakObject()
    {
        if (rigidBody) Destroy(rigidBody);
        if (_collider) Destroy(_collider);
        yield return new WaitForEndOfFrame();
        brokenObject.transform.parent = null;
        brokenObject.gameObject.SetActive(true);
        for (int i = 0; i < itemDrops.Length; i++)
        {
            if (itemDrops[i] != null)
            {
                GameObject items = Instantiate(itemDrops[i], transform.position + new Vector3(Random.Range(-0.8f, 0.8f),
                1f, Random.Range(-0.8f, 0.8f)), Quaternion.Euler(0, 0, 90)) as GameObject;
                items.GetComponentInChildren<ItemData>().itemActive = true;
                items.SetActive(true);
            }
        }
        Destroy(gameObject);
    }

    void CarryBehavior()
    {
        if (!canBeCarried)
            return;

        if (lifted)
        {
            rigidBody.useGravity = false;
            foreach (Collider col in colliderGroup)
            {
                col.enabled = false;
            }
            transform.SetParent(cc.transform);

            transform.position = Vector3.Lerp(transform.position, carryPosition.position + ((carryPosition.up * OffSetToHandX) -
            (carryPosition.forward * OffSetToHandY) - (carryPosition.right * OffSetToHandZ)), 4 * Time.deltaTime);
        }
        if (carried)
        {
            transform.position = carryPosition.position + ((carryPosition.up * OffSetToHandX) - (carryPosition.forward * OffSetToHandY) - (carryPosition.right * OffSetToHandZ));
        }
        if (dropped)
        {
            transform.position = Vector3.Lerp(transform.position, carryPosition.position + ((carryPosition.up * OffSetToHandX) -
            (carryPosition.forward * OffSetToHandY) - (carryPosition.right * (OffSetToHandZ + 0.35f))), 20 * Time.deltaTime);
        }
    }

    public void TurnOffTag(bool isOn)
    {
        if (!isOn)
        {
            foreach (TriggerAction action in triggerActions)
            {
                action.gameObject.tag = "Action";
            }
        }
        else
        {
            foreach (TriggerAction action in triggerActions)
            {
                action.gameObject.tag = "Untagged";
            }
        }
    }

    public void OnReceiveMessage(MsgType type, object sender, object data)
    {
        switch (type)
        {
            case MsgType.DAMAGED:
                {
                    HealthPoints.DamageData damageData = (HealthPoints.DamageData)data;
                    Damaged(damageData);
                }
                break;
            case MsgType.DEAD:
                {
                    HealthPoints.DamageData damageData = (HealthPoints.DamageData)data;
                    Die(damageData);
                }
                break;
            default:
                break;
        }
    }

    void Damaged(HealthPoints.DamageData data)
    {
        for (int i = 0; i < data.damager.GetComponent<HitBox>().attackPoints.Length; i++)
        {
            data.damager.GetComponent<HitBox>().PlayRandomSound("HitObstacleAS", false);
            data.damager.GetComponent<HitBox>().CreateParticle(data.damager.GetComponent<HitBox>().effects.obstacleEffectHit,
            data.damager.GetComponent<HitBox>().attackPoints[i].attackRoot.transform.position);
        }
    }

    public void Die(HealthPoints.DamageData data)
    {
        breakAudio.transform.SetParent(null, true);
        if (breakAudio)
            breakAudio.PlayRandomClip();

        Destroy(breakAudio, breakAudio.clip == null ? 0.0f : breakAudio.clip.length + 0.5f);
        if (rigidBody)
            StartCoroutine(BreakObject());
    }

    public void CheckGrounded()
    {
        if (hp != null && hp.curHealthPoints < 1) return;

        RaycastHit hit;
        Ray ray = new Ray(transform.position + Vector3.up, -Vector3.up);
        grounded = Physics.Raycast(ray, out hit, groundedRayDistance, Physics.AllLayers,
        QueryTriggerInteraction.Ignore);

        if (grounded)
        {
            if (hit.collider.tag != "Player")
            {
                if (landAudio != null && !landSound)
                {

                    if (landAudio)
                        landAudio.PlayRandomClip();

                    landSound = true;
                }
                TurnOffTag(false);
                rigidBody.isKinematic = true;
                if (!canBreak)
                    rigidBody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
            }
        }
        else
        {
            landSound = false;
            rigidBody.isKinematic = false;
            rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    void CheckGround()
    {
        if (hp != null && hp.curHealthPoints < 1) return;

        Ray ray = new Ray(transform.position, -Vector3.up);
        Debug.DrawRay(transform.position, Vector3.down * groundedRayDistance, Color.red);

        if (Physics.Raycast(ray, out currentGroundInfo, groundedLayer))
        {
            if(currentGroundInfo.distance < 0.5f)
            {
                grounded = true;
            }
            else
            {
                grounded = false;
            }
        }

        if (grounded)
        {

            if (landAudio != null && !landSound)
            {

                if (landAudio)
                    landAudio.PlayRandomClip();

                landSound = true;
            }
            TurnOffTag(false);

            if (!canBreak)
            {
                rigidBody.constraints = RigidbodyConstraints.FreezePositionX | 
                RigidbodyConstraints.FreezePositionZ | 
                RigidbodyConstraints.FreezeRotation;
            }
        }
        else
        {
            landSound = false;
            rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }
}
