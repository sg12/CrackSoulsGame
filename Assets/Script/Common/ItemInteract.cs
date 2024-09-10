using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using RPGAE.CharacterController;

public class ItemInteract : MonoBehaviour
{
    [Header("ITEM SETTINGS")]
    public float pickUpDistance = 2.5f;
    public RaycastHit currentGroundInfo;

    [Header("SCENE TRANSITION SETTINGS")]
    public string sceneName;
    public int SpawnPointIDRef;

    [Header("QUEST SETTINGS")]
    [Tooltip("The name of the current quest you're progressing through.")]
    public string questSequenceNameReceipt;
    [Tooltip("The required phase number in order to progress to the next.")]
    public int questPhaseReceipt;
    [Tooltip("Once the required action is meet you will increment a phase.")]
    public int addQuestPhaseBy = 1;
    [Tooltip("Once the required action is meet you will increment a quantity amount.")]
    public int addQuestQuantityBy;

    [Header("REFERENCES")]
    public ParticleSystem itemOnGroundParticle;
    public InteractWorldSpaceUI interactUI;

    #region Private 

    [HideInInspector] public bool grounded;
    [HideInInspector] public Rigidbody rigidBody;
    [HideInInspector] public Collider[] colliders;
    [HideInInspector] public ItemData itemData;
    [HideInInspector] public ThirdPersonController cc;
    [HideInInspector] public InventoryManager inventoryM;
    [HideInInspector] public QuestManager questM;
    [HideInInspector] public ItemObtained itemObtained;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        cc = FindObjectOfType<ThirdPersonController>();
        inventoryM = FindObjectOfType<InventoryManager>();
        questM = FindObjectOfType<QuestManager>();
        itemData = GetComponent<ItemData>();
        rigidBody = GetComponent<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();

        if (interactUI == null)
        {
            interactUI = GetComponent<ItemData>().transform.parent.GetComponentInChildren<InteractWorldSpaceUI>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        float dist = Vector3.Distance(transform.position, cc.transform.position);
        if(itemData.itemActive && dist < pickUpDistance)
        {
            if(itemData.GetComponent<HitBox>() == null)
            {
                GetItemBehaviour();
                if (interactUI != null)
                {
                    interactUI.ToggleIcon(true);
                }
            }
            else
            {
                if (!itemData.GetComponent<HitBox>().isProjectile)
                {
                    GetItemBehaviour();
                    if (interactUI != null)
                    {
                        interactUI.ToggleIcon(true);
                    }
                }
            }
        }
        else
        {
            if (interactUI != null)
            {
                interactUI.fadeUI.canvasGroup.alpha = 0; 
            }
        }
        //CheckGround(); // To Do: Add drop sound effects?
        ItemOnGroundParticle();
    }

    void ItemOnGroundParticle()
    {
        if (!inventoryM.isPauseMenuOn)
        {
            if (itemData.itemActive && !itemOnGroundParticle.isPlaying)
                itemOnGroundParticle.Play();
            else
                itemOnGroundParticle.Stop();
        }
        else
        {
            if (itemOnGroundParticle.isPlaying)
                itemOnGroundParticle.Stop();
        }
    }

    void GetItemBehaviour()
    {
        if (cc.cc.rpgaeIM.PlayerControls.Interact.triggered)
        {
            if (itemData.itemType == ItemData.ItemType.Weapon)
            {
                CheckInventoryItems(ref inventoryM.weaponInv.itemData, ref inventoryM.weaponInv.image);
            }
            else if (itemData.itemType == ItemData.ItemType.Bow || itemData.itemType == ItemData.ItemType.Arrow)
            {
                CheckInventoryItems(ref inventoryM.bowAndArrowInv.itemData, ref inventoryM.bowAndArrowInv.image);
            }
            else if (itemData.itemType == ItemData.ItemType.Shield)
            {
                CheckInventoryItems(ref inventoryM.shieldInv.itemData, ref inventoryM.shieldInv.image);
            }
            else if (itemData.itemType == ItemData.ItemType.Armor)
            {
                CheckInventoryItems(ref inventoryM.armorInv.itemData, ref inventoryM.armorInv.image);
            }
            else if (itemData.itemType == ItemData.ItemType.Material)
            {
                CheckInventoryItems(ref inventoryM.materialInv.itemData, ref inventoryM.materialInv.image);
            }
            else if (itemData.itemType == ItemData.ItemType.Healing)
            {
                CheckInventoryItems(ref inventoryM.healingInv.itemData, ref inventoryM.healingInv.image);
            }
            else if (itemData.itemType == ItemData.ItemType.Key)
            {
                CheckInventoryItems(ref inventoryM.keyInv.itemData, ref inventoryM.keyInv.image);
            }
            else if (itemData.itemType == ItemData.ItemType.General)
            {
                if (itemData.itemName == "Coin" || itemData.itemName == "Coin Bag")
                {
                    inventoryM.unityCoins += gameObject.GetComponent<ItemData>().quantity;
                    itemData.itemActive = false;
                }
            }

            #region Quest Event

            for (int i = 0; i < questM.mainQuestSlots.questData.Length; i++)
            {
                if (questM.mainQuestSlots.questData[i].questName == questSequenceNameReceipt
                && questM.mainQuestSlots.questData[i].questObjective[questM.mainQuestSlots.questData[i].questObjectiveNum].curCollectableAmount < questM.mainQuestSlots.questData[i].questObjective[questM.mainQuestSlots.questData[i].questObjectiveNum].maxCollectableAmount)
                {
                    questM.AddQuestObjective(ref questSequenceNameReceipt, ref questPhaseReceipt, ref addQuestPhaseBy, ref addQuestQuantityBy);
                }
            }

            for (int i = 0; i < questM.sideQuestSlots.questData.Length; i++)
            {
                if (questM.sideQuestSlots.questData[i].questName == questSequenceNameReceipt
                && questM.sideQuestSlots.questData[i].questObjective[questM.sideQuestSlots.questData[i].questObjectiveNum].curCollectableAmount < questM.sideQuestSlots.questData[i].questObjective[questM.sideQuestSlots.questData[i].questObjectiveNum].maxCollectableAmount)
                {
                    questM.AddQuestObjective(ref questSequenceNameReceipt, ref questPhaseReceipt, ref addQuestPhaseBy, ref addQuestQuantityBy);
                }
            }

            #endregion
        }
    }

    public void CheckInventoryItems(ref ItemData[] currentInventory, ref Image[] itemImage)
    {
        if (cc.infoMessage.info.text == "You don't have enough inventory space.")
            return;

        if (!itemObtained)
            itemObtained = FindObjectOfType<ItemObtained>();

        GameObject itemDataConvert = itemData.gameObject;
        if (itemDataConvert.GetComponentInChildren<ItemData>().itemObtainedPickUp)
        {
            itemObtained.itemData = itemData;
            itemObtained.itemInteract = this;

            itemObtained.sceneName = sceneName;
            itemObtained.SpawnPointIDRef = SpawnPointIDRef;
        }
        else
        {
            inventoryM.AddItemDataSlot(ref currentInventory,
            ref itemImage, ref itemDataConvert);
        }
        itemData.itemActive = false;
    }

    void CheckGround()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.SphereCast(ray, 3, out currentGroundInfo, transform.position.magnitude, ~0, QueryTriggerInteraction.Ignore))
        {
            grounded = false;
        }
        else
        {
            grounded = true;
        }
    }
}
