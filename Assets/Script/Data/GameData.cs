using System.Diagnostics;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    #region Player Variables

    public float level;
    public float curhealth;
    public float maxhealth;
    public float curEnegry;
    public float maxEnegry;
    public float curExp;
    public float reqExp;
    public float skillPoint;
    public float[] playerPosition;
    public float[] playerRotation;

    #endregion

    #region AI Variables 

    [System.Serializable]
    public struct NPCData
    {
        public State state;
        public DialogueType dialogueType;

        public int dialogueSegmentNum;
        public bool dialogueAlteredPrimary;

        public int questPhaseReceipt;

        public bool hasReceived;

        public float positionX;
        public float positionY;
        public float positionZ;

        public float rotationX;
        public float rotationY;
        public float rotationZ;

        public enum State
        {
            Idle,
            FollowCompanion,
            RoamArea,
            PatrolPoints
        }
        public enum DialogueType
        {
            Default,
            Store,
            ItemGiver,
            MainQuestGiver,
            SideQuestGiver,
            QuestSequence,
            Follow
        }
    }
    public NPCData[] npcData;

    #endregion

    #region MiniMap Varibles

    public bool isMarkerOnMap;
    public float[] mapMarkerPosition;

    #endregion

    #region Quest Varibles

    public int mainQuestSlot;
    public int sideQuestSlot;

    [System.Serializable]
    public struct QuestData
    {
        public string questName;
        public int questLevel;
        public int questEXPReward;
        public string questDescription;
        public int curObjectivePhase;
        public int maxObjectivePhase;
        public bool questInProgress;
        public bool questIsComplete;
        public int questObjectiveNum;

        public enum QuestType
        {
            MainQuest,
            SideQuest
        }
        public QuestType questType;

        [System.Serializable]
        public struct QuestObjective
        {
            public float questLocationX;
            public float questLocationY;
            public float questLocationZ;
            public string questLocationName;
            public string questObjectiveSubject;
            public string questHint;
            public int curCollectableAmount;
            public int maxCollectableAmount;
        }
        public QuestObjective[] questObective;
    }
    public QuestData[] mainQuestData;
    public QuestData[] sideQuestData;

    #endregion

    #region Inventory Variables

    [System.Serializable]
    public struct Item
    {
        public ItemType itemType;
        public string itemName;
        public string itemSprite;
        public string itemDescription;

        public enum ItemType
        {
            General,
            Weapon,
            Bow,
            Arrow,
            Shield,
            Armor,
            Material,
            Healing,
            Key
        }

        [System.Serializable]
        public struct ItemStats
        {
            public int value;
            public int rank1;
            public int rank2;
            public int rank3;
            public ArmorType armorType;
        }

        public ItemStats wpnAtkStat;
        public ItemStats wpnSpdStat;
        public ItemStats wpnStunStat;
        public ItemStats wpnCritHitStat;
        public ItemStats wpnDuraStat;

        public ItemStats bowAtkStat;
        public ItemStats bowSpdStat;
        public ItemStats bowStunStat;
        public ItemStats bowCritHitStat;
        public ItemStats bowHdShotStat;
        public ItemStats bowDuraStat;

        public ItemStats shdAtkStat;
        public ItemStats shdSpdStat;
        public ItemStats shdStunStat;
        public ItemStats shdCritHitStat;
        public ItemStats shdBlockStat;
        public ItemStats shdDuraStat;

        public ItemStats armStat;
        public ItemStats armLResStat;
        public ItemStats armHResStat;

        #region Weapon Range 

        [System.Serializable]
        public struct ThrowDistance
        {
            public ThrowRange throwRange;
        }
        public ThrowDistance throwDistance;
        public enum ThrowRange
        {
            ShortRange,
            MedRange,
            LongRange,
            Endless
        }

        [System.Serializable]
        public struct BowRange
        {
            public int shotAmount;
            public BowRangeType bowRangeType;
        }
        public BowRange bowRange;
        public enum BowRangeType
        {
            ShortRange,
            MedRange,
            LongRange,
            Endless
        }

        #endregion

        #region Item Special

        public enum WeaponSpecialStat1
        {
            None,
            AtkBonus,
            SpdBonus,
            StunBonus,
            CritHitBonus,
            DuraBonus,
            ThrowMedBonus,
            ThrowLongBonus,
            ThrowEndlessBonus
        }
        public WeaponSpecialStat1 wpnSpecialStat1;

        public enum WeaponSpecialStat2
        {
            None,
            AtkBonus,
            SpdBonus,
            StunBonus,
            CritHitBonus,
            DuraBonus,
            ThrowMedBonus,
            ThrowLongBonus,
            ThrowEndlessBonus
        }
        public WeaponSpecialStat2 wpnSpecialStat2;

        public enum BowSpecialStat1
        {
            None,
            AtkBonus,
            SpdBonus,
            StunBonus,
            CritHitBonus,
            DuraBonus,
            HeadShotBonus,
            MedRangeBonus,
            LongRangeBonus,
            EndlessRangeBonus,
            QuickShot,
            ThreeShotBurst,
            FiveShotBurst
        }
        public BowSpecialStat1 bowSpecialStat1;

        public enum BowSpecialStat2
        {
            None,
            AtkBonus,
            SpdBonus,
            StunBonus,
            CritHitBonus,
            DuraBonus,
            HeadShotBonus,
            MedRangeBonus,
            LongRangeBonus,
            EndlessRangeBonus,
            QuickShot,
            ThreeShotBurst,
            FiveShotBurst
        }
        public BowSpecialStat2 bowSpecialStat2;

        public enum ShieldSpecialStat1
        {
            None,
            AtkBonus,
            SpdBonus,
            ShieldBlockBonus,
            StunBonus,
            CritHitBonus,
            DuraBonus
        }
        public ShieldSpecialStat1 shdSpecialStat1;

        public enum ShieldSpecialStat2
        {
            None,
            AtkBonus,
            SpdBonus,
            ShieldBlockBonus,
            StunBonus,
            CritHitBonus,
            DuraBonus
        }
        public ShieldSpecialStat2 shdSpecialStat2;

        public enum ArmorType
        {
            Head,
            Chest,
            Legs,
            Amulet
        }

        public enum ArmorSpecialStat1
        {
            None,
            ArmBonus,
            ArmLResBonus,
            ArmHResBonus
        }
        public ArmorSpecialStat1 armSpecialStat1;

        public enum ArmorSpecialStat2
        {
            None,
            ArmBonus,
            ArmLResBonus,
            ArmHResBonus
        }
        public ArmorSpecialStat2 armSpecialStat2;

        #endregion

        public int weight;
        public float weaponArmsID;

        #region Upgrade

        public int rankNum;

        [System.Serializable]
        public struct UpgradeMaterial
        {
            public string matName;
            public int matRequired;
        }
        public UpgradeMaterial[] upgradeMaterial;

        [System.Serializable]
        public struct ItemUpgrade
        {
            public int amount;
            public int durability;
        }
        public ItemUpgrade itemUpgrade;

        #endregion

        public int buyPrice;
        public int sellPrice;
        public int repairPrice;
        public int upgradePrice;
        public int specialPrice;
        public int inStockQuantity;

        public int quantity;
        public bool startEquipItem;
        public bool stackable;
        public bool inInventory;
        public bool equipped;

        public bool broken;
        public string rankTag;
        public string originItemName;
        public int numOfEngraving;

        #region Max Value

        public int maxWpnAtk;
        public int maxWpnSpd;
        public int maxWpnStun;
        public int maxWpnCritHit;
        public int maxWpnDura;

        public int maxBowAtk;
        public int maxBowSpd;
        public int maxBowStun;
        public int maxBowHdShot;
        public int maxBowCritHit;
        public int maxBowDura;

        public int maxShdAtk;
        public int maxShdSpd;
        public int maxShdStun;
        public int maxShdBlock;
        public int maxShdCritHit;
        public int maxShdDura;

        public int maxArm;
        public int maxArmLRes;
        public int maxArmHRes;

        #endregion
    }

    public Item[] wpnData;
    public Item[] bowData;
    public Item[] shdData;
    public Item[] armData;
    public Item[] matData;
    public Item[] healData;
    public Item[] keyData;

    public int unityCoins;

    public int inventorySection;

    public bool[] itemActive;

    public int wpnSlots;
    public int bowSlots;
    public int shdSlots;
    public int armSlots;
    public int matSlots;
    public int healSlots;
    public int keySlots;

    public bool preSwordEquipped;
    public bool pre2HSwordEquipped;
    public bool preSpearEquipped;
    public bool preStaffEquipped;
    public bool preBowEquipped;
    public bool preshieldEquipped;

    public int wpnSlotNum;
    public int wpnSlotEquipped;

    public int bowSlotNum;
    public int bowSlotEquipped;
    public int arrowSlotEquipped;

    public int shieldSlotNum;
    public int shieldSlotEquipped;

    public int armSlotNum;
    public int armSlotHeadEquipped;
    public int armSlotChestEquipped;
    public int armSlotLegEquipped;
    public int armSlotAmuletEquipped;

    public int matSlotNum;

    public int healSlotNum;

    public int keySlotNum;

    #endregion

    #region Skill Tree Varibles 

    public string topAbilityName;
    public string leftAbilityName;
    public string rightAbilityName;
    public string bottomAbilityName;

    public string[] purchasedSkillName;

    #endregion

    #region Miscellaneous

    // Location Check points
    public bool[] explored;

    // Trigger Switches 
    [System.Serializable]
    public struct TriggerAction
    {
        public enum InteractionType
        {
            SmallKey,
            BigKey,
            Triggered,
            permanentlyLocked,
            PressureStep,
            PressureObjectWeight,
            None
        }
        public InteractionType interactionType;
        public bool isActive;
        public bool destroyAfter;
    }
    public TriggerAction[] triggerAction;

    #endregion

    public GameData(SystemManager data)
    {
        #region Player Data

        level = data.hudM.curLevel;
        curhealth = data.hudM.curHealth;
        maxhealth = data.hudM.maxHealth;
        curEnegry = data.hudM.curEnegry;
        maxEnegry = data.hudM.maxEnegry;
        curExp = data.hudM.curExp;
        reqExp = data.hudM.expRequired;
        skillPoint = data.hudM.curSkillPoint;

        playerPosition = new float[3];
        playerPosition[0] = data.cc.transform.position.x;
        playerPosition[1] = data.cc.transform.position.y;
        playerPosition[2] = data.cc.transform.position.z;

        playerRotation = new float[3];
        playerRotation[0] = data.cc.transform.eulerAngles.x;
        playerRotation[1] = data.cc.transform.eulerAngles.y;
        playerRotation[2] = data.cc.transform.eulerAngles.z;

        int activeWpnLength = data.inventoryM.initWpns.Length;
        itemActive = new bool[activeWpnLength];
        for (int i = 0; i < activeWpnLength; i++)
        {
            itemActive[i] = data.inventoryM.initWpns[i].GetComponent<ItemData>().itemActive;
        }

        #endregion

        #region AI Data

        // Npc Data

        int npcLength = data.dialogueData.Length;
        npcData = new NPCData[npcLength];

        for (int i = 0; i < npcData.Length; i++)
        {
            npcData[i].state = (NPCData.State)data.dialogueData[i].state;
            npcData[i].dialogueType = (NPCData.DialogueType)data.dialogueData[i].dialogue.dialogueType;

            npcData[i].dialogueSegmentNum = data.dialogueData[i].dialogue.dialogueSegmentNum;
            npcData[i].dialogueAlteredPrimary = data.dialogueData[i].dialogueAltered;
            npcData[i].questPhaseReceipt = data.dialogueData[i].questPhaseReceipt;

            npcData[i].positionX = data.dialogueData[i].gameObject.transform.position.x;
            npcData[i].positionY = data.dialogueData[i].gameObject.transform.position.y;
            npcData[i].positionZ = data.dialogueData[i].gameObject.transform.position.z;

            npcData[i].rotationX = data.dialogueData[i].gameObject.transform.eulerAngles.x;
            npcData[i].rotationY = data.dialogueData[i].gameObject.transform.eulerAngles.y;
            npcData[i].rotationZ = data.dialogueData[i].gameObject.transform.eulerAngles.z;
        }

        #endregion

        #region MiniMap Data

        isMarkerOnMap = data.miniMap.isMarkerOnMap;

        mapMarkerPosition = new float[3];
        mapMarkerPosition[0] = data.miniMap.MapPointerPrefab.transform.position.x;
        mapMarkerPosition[1] = data.miniMap.MapPointerPrefab.transform.position.y;
        mapMarkerPosition[2] = data.miniMap.MapPointerPrefab.transform.position.z;

        #endregion

        #region Quest Data

        #region Main Quest Data Slots 

        mainQuestSlot = data.questM.mainQuestSlots.slotNum;

        int mainQuestDataLength = data.questM.mainQuestSlots.questData.Length;
        mainQuestData = new QuestData[mainQuestDataLength];

        for (int i = 0; i < mainQuestData.Length; i++)
        {
            if (data.questM.mainQuestSlots.questData != null)
            {
                mainQuestData[i].questType = (QuestData.QuestType)data.questM.mainQuestSlots.questData[i].questType;
                mainQuestData[i].questName = data.questM.mainQuestSlots.questData[i].questName;
                mainQuestData[i].questLevel = data.questM.mainQuestSlots.questData[i].questLevel;
                mainQuestData[i].questEXPReward = data.questM.mainQuestSlots.questData[i].EXPReward;
                mainQuestData[i].questDescription = data.questM.mainQuestSlots.questData[i].questDescription;
                mainQuestData[i].curObjectivePhase = data.questM.mainQuestSlots.questData[i].curObjectivePhase;
                mainQuestData[i].maxObjectivePhase = data.questM.mainQuestSlots.questData[i].maxObjectivePhase;

                #region Quest Objective Data

                mainQuestData[i].questObjectiveNum = data.questM.mainQuestSlots.questData[i].questObjectiveNum;

                mainQuestData[i].questObective = new QuestData.QuestObjective[data.questM.mainQuestSlots.questData[i].questObjective.Length];

                for (int mQSize = 0; mQSize < mainQuestData[i].questObective.Length; mQSize++)
                {
                    mainQuestData[i].questObective[mQSize].questLocationX = data.questM.mainQuestSlots.questData[i].questObjective[mQSize].questLocation.transform.position.x;
                    mainQuestData[i].questObective[mQSize].questLocationY = data.questM.mainQuestSlots.questData[i].questObjective[mQSize].questLocation.transform.position.y;
                    mainQuestData[i].questObective[mQSize].questLocationZ = data.questM.mainQuestSlots.questData[i].questObjective[mQSize].questLocation.transform.position.z;

                    mainQuestData[i].questObective[mQSize].questLocationName = data.questM.mainQuestSlots.questData[i].questObjective[mQSize].questLocationName;
                    mainQuestData[i].questObective[mQSize].questObjectiveSubject = data.questM.mainQuestSlots.questData[i].questObjective[mQSize].questObjectiveSubject;
                    mainQuestData[i].questObective[mQSize].questHint = data.questM.mainQuestSlots.questData[i].questObjective[mQSize].questHint;

                    mainQuestData[i].questObective[mQSize].curCollectableAmount = data.questM.mainQuestSlots.questData[i].questObjective[mQSize].curCollectableAmount;
                    mainQuestData[i].questObective[mQSize].maxCollectableAmount = data.questM.mainQuestSlots.questData[i].questObjective[mQSize].maxCollectableAmount;
                }

                #endregion

                mainQuestData[i].questInProgress = data.questM.mainQuestSlots.questData[i].inProgress;
                mainQuestData[i].questIsComplete = data.questM.mainQuestSlots.questData[i].isComplete;
            }
        }

        #endregion

        #region Side Quest Data Slots 

        sideQuestSlot = data.questM.sideQuestSlots.slotNum;

        int sideQuestDataLength = data.questM.sideQuestSlots.questData.Length;
        sideQuestData = new QuestData[sideQuestDataLength];

        for (int i = 0; i < sideQuestData.Length; i++)
        {
            if (data.questM.sideQuestSlots.questData != null)
            {
                sideQuestData[i].questType = (QuestData.QuestType)data.questM.sideQuestSlots.questData[i].questType;
                sideQuestData[i].questName = data.questM.sideQuestSlots.questData[i].questName;
                sideQuestData[i].questLevel = data.questM.sideQuestSlots.questData[i].questLevel;
                sideQuestData[i].questEXPReward = data.questM.sideQuestSlots.questData[i].EXPReward;
                sideQuestData[i].questDescription = data.questM.sideQuestSlots.questData[i].questDescription;
                sideQuestData[i].curObjectivePhase = data.questM.sideQuestSlots.questData[i].curObjectivePhase;
                sideQuestData[i].maxObjectivePhase = data.questM.sideQuestSlots.questData[i].maxObjectivePhase;

                #region Quest Objective Data

                sideQuestData[i].questObjectiveNum = data.questM.sideQuestSlots.questData[i].questObjectiveNum;

                sideQuestData[i].questObective = new QuestData.QuestObjective[data.questM.sideQuestSlots.questData[i].questObjective.Length];

                for (int sQSize = 0; sQSize < sideQuestData[i].questObective.Length; sQSize++)
                {
                    sideQuestData[i].questObective[sQSize].questLocationX = data.questM.sideQuestSlots.questData[i].questObjective[sQSize].questLocation.transform.position.x;
                    sideQuestData[i].questObective[sQSize].questLocationY = data.questM.sideQuestSlots.questData[i].questObjective[sQSize].questLocation.transform.position.y;
                    sideQuestData[i].questObective[sQSize].questLocationZ = data.questM.sideQuestSlots.questData[i].questObjective[sQSize].questLocation.transform.position.z;

                    sideQuestData[i].questObective[sQSize].questLocationName = data.questM.sideQuestSlots.questData[i].questObjective[sQSize].questLocationName;
                    sideQuestData[i].questObective[sQSize].questObjectiveSubject = data.questM.sideQuestSlots.questData[i].questObjective[sQSize].questObjectiveSubject;
                    sideQuestData[i].questObective[sQSize].questHint = data.questM.sideQuestSlots.questData[i].questObjective[sQSize].questHint;

                    sideQuestData[i].questObective[sQSize].curCollectableAmount = data.questM.sideQuestSlots.questData[i].questObjective[sQSize].curCollectableAmount;
                    sideQuestData[i].questObective[sQSize].maxCollectableAmount = data.questM.sideQuestSlots.questData[i].questObjective[sQSize].maxCollectableAmount;
                }

                #endregion

                sideQuestData[i].questInProgress = data.questM.sideQuestSlots.questData[i].inProgress;
                sideQuestData[i].questIsComplete = data.questM.sideQuestSlots.questData[i].isComplete;
            }
        }

        #endregion

        #endregion

        #region Inventory Data

        unityCoins = data.inventoryM.unityCoins;

        wpnSlots = data.inventoryM.weaponSlots;
        bowSlots = data.inventoryM.bowAndArrowSlots;
        shdSlots = data.inventoryM.shieldSlots;
        armSlots = data.inventoryM.armorSlots;
        matSlots = data.inventoryM.materialSlots;
        healSlots = data.inventoryM.healingSlots;
        keySlots = data.inventoryM.keySlots;

        inventorySection = (int)data.inventoryM.inventoryS;

        preSwordEquipped = data.inventoryM.preSwordEquipped;
        pre2HSwordEquipped = data.inventoryM.pre2HSwordEquipped;
        preshieldEquipped = data.inventoryM.preShieldEquipped;
        preSpearEquipped = data.inventoryM.preSpearEquipped;
        preStaffEquipped = data.inventoryM.preStaffEquipped;
        preBowEquipped = data.inventoryM.preBowEquipped;

        #region Inventory Weapon Data Slots

        wpnSlotNum = data.inventoryM.weaponInv.slotNum;
        wpnSlotEquipped = data.inventoryM.weaponInv.slotWeaponEquipped;

        int wpnDataLength = data.inventoryM.weaponInv.itemData.Length;
        wpnData = new Item[wpnDataLength];

        for (int i = 0; i < wpnData.Length; i++)
        {
            if (data.inventoryM.weaponInv.itemData != null)
            {
                wpnData[i].itemType = (Item.ItemType)data.inventoryM.weaponInv.itemData[i].itemType;

                wpnData[i].itemName = data.inventoryM.weaponInv.itemData[i].itemName;
                wpnData[i].itemDescription = data.inventoryM.weaponInv.itemData[i].itemDescription;

                wpnData[i].wpnAtkStat.value = data.inventoryM.weaponInv.itemData[i].wpnAtk.value;
                wpnData[i].wpnAtkStat.rank1 = data.inventoryM.weaponInv.itemData[i].wpnAtk.rank1;
                wpnData[i].wpnAtkStat.rank2 = data.inventoryM.weaponInv.itemData[i].wpnAtk.rank2;
                wpnData[i].wpnAtkStat.rank3 = data.inventoryM.weaponInv.itemData[i].wpnAtk.rank3;

                wpnData[i].wpnSpdStat.value = data.inventoryM.weaponInv.itemData[i].wpnSpd.value;
                wpnData[i].wpnSpdStat.rank1 = data.inventoryM.weaponInv.itemData[i].wpnSpd.rank1;
                wpnData[i].wpnSpdStat.rank2 = data.inventoryM.weaponInv.itemData[i].wpnSpd.rank2;
                wpnData[i].wpnSpdStat.rank3 = data.inventoryM.weaponInv.itemData[i].wpnSpd.rank3;

                wpnData[i].wpnStunStat.value = data.inventoryM.weaponInv.itemData[i].wpnStun.value;
                wpnData[i].wpnStunStat.rank1 = data.inventoryM.weaponInv.itemData[i].wpnStun.rank1;
                wpnData[i].wpnStunStat.rank2 = data.inventoryM.weaponInv.itemData[i].wpnStun.rank2;
                wpnData[i].wpnStunStat.rank3 = data.inventoryM.weaponInv.itemData[i].wpnStun.rank3;

                wpnData[i].wpnCritHitStat.value = data.inventoryM.weaponInv.itemData[i].wpnCritHit.value;
                wpnData[i].wpnCritHitStat.rank1 = data.inventoryM.weaponInv.itemData[i].wpnCritHit.rank1;
                wpnData[i].wpnCritHitStat.rank2 = data.inventoryM.weaponInv.itemData[i].wpnCritHit.rank2;
                wpnData[i].wpnCritHitStat.rank3 = data.inventoryM.weaponInv.itemData[i].wpnCritHit.rank3;

                wpnData[i].wpnDuraStat.value = data.inventoryM.weaponInv.itemData[i].wpnDura.value;
                wpnData[i].wpnDuraStat.rank1 = data.inventoryM.weaponInv.itemData[i].wpnDura.rank1;
                wpnData[i].wpnDuraStat.rank2 = data.inventoryM.weaponInv.itemData[i].wpnDura.rank2;
                wpnData[i].wpnDuraStat.rank3 = data.inventoryM.weaponInv.itemData[i].wpnDura.rank3;

                wpnData[i].throwDistance.throwRange = (Item.ThrowRange)data.inventoryM.weaponInv.itemData[i].throwDist.throwRange;

                wpnData[i].wpnSpecialStat1 = (Item.WeaponSpecialStat1)data.inventoryM.weaponInv.itemData[i].wpnSpecialStat1;
                wpnData[i].wpnSpecialStat2 = (Item.WeaponSpecialStat2)data.inventoryM.weaponInv.itemData[i].wpnSpecialStat2;

                wpnData[i].weight = data.inventoryM.weaponInv.itemData[i].wpnWeight;
                wpnData[i].weaponArmsID = data.inventoryM.weaponInv.itemData[i].weaponArmsID;

                wpnData[i].rankNum = data.inventoryM.weaponInv.itemData[i].rankNum;

                int weaponUpgradeLength = data.inventoryM.weaponInv.itemData[i].upgradeMaterial.Length;
                wpnData[i].upgradeMaterial = new Item.UpgradeMaterial[weaponUpgradeLength];

                for (int weaponUpgradeSize = 0; weaponUpgradeSize < weaponUpgradeLength; weaponUpgradeSize++)
                {
                    wpnData[i].upgradeMaterial[weaponUpgradeSize].matName = data.inventoryM.weaponInv.itemData[i].upgradeMaterial[weaponUpgradeSize].matName;
                    wpnData[i].upgradeMaterial[weaponUpgradeSize].matRequired = data.inventoryM.weaponInv.itemData[i].upgradeMaterial[weaponUpgradeSize].matRequired;
                }

                wpnData[i].buyPrice = data.inventoryM.weaponInv.itemData[i].buyPrice;
                wpnData[i].sellPrice = data.inventoryM.weaponInv.itemData[i].sellPrice;
                wpnData[i].repairPrice = data.inventoryM.weaponInv.itemData[i].repairPrice;
                wpnData[i].upgradePrice = data.inventoryM.weaponInv.itemData[i].upgradePrice;
                wpnData[i].specialPrice = data.inventoryM.weaponInv.itemData[i].specialPrice;
                wpnData[i].inStockQuantity = data.inventoryM.weaponInv.itemData[i].inStockQuantity;

                wpnData[i].quantity = data.inventoryM.weaponInv.counter[i];
                wpnData[i].stackable = data.inventoryM.weaponInv.itemData[i].stackable;
                wpnData[i].inInventory = data.inventoryM.weaponInv.itemData[i].inInventory;
                wpnData[i].equipped = data.inventoryM.weaponInv.itemData[i].equipped;

                wpnData[i].broken = data.inventoryM.weaponInv.itemData[i].broken;
                wpnData[i].rankTag = data.inventoryM.weaponInv.itemData[i].rankTag;
                wpnData[i].originItemName = data.inventoryM.weaponInv.itemData[i].originItemName;
                wpnData[i].numOfEngraving = data.inventoryM.weaponInv.itemData[i].numOfEngraving;

                wpnData[i].maxWpnAtk = data.inventoryM.weaponInv.itemData[i].maxWpnAtk;
                wpnData[i].maxWpnSpd = data.inventoryM.weaponInv.itemData[i].maxWpnSpd;
                wpnData[i].maxWpnStun = data.inventoryM.weaponInv.itemData[i].maxWpnStun;
                wpnData[i].maxWpnCritHit = data.inventoryM.weaponInv.itemData[i].maxWpnCritHit;
                wpnData[i].maxWpnDura = data.inventoryM.weaponInv.itemData[i].maxWpnDura;
            }
        }

        #endregion

        #region Inventory Bow And Arrow Data Slots

        bowSlotNum = data.inventoryM.bowAndArrowInv.slotNum;
        bowSlotEquipped = data.inventoryM.bowAndArrowInv.slotBowEquipped;
        arrowSlotEquipped = data.inventoryM.bowAndArrowInv.slotArrowEquipped;

        int bowDataLength = data.inventoryM.bowAndArrowInv.itemData.Length;
        bowData = new Item[bowDataLength];

        for (int i = 0; i < bowData.Length; i++)
        {
            if (data.inventoryM.bowAndArrowInv.itemData != null)
            {
                bowData[i].itemType = (Item.ItemType)data.inventoryM.bowAndArrowInv.itemData[i].itemType;

                bowData[i].itemName = data.inventoryM.bowAndArrowInv.itemData[i].itemName;
                bowData[i].itemDescription = data.inventoryM.bowAndArrowInv.itemData[i].itemDescription;

                bowData[i].bowAtkStat.value = data.inventoryM.bowAndArrowInv.itemData[i].bowAtk.value;
                bowData[i].bowAtkStat.rank1 = data.inventoryM.bowAndArrowInv.itemData[i].bowAtk.rank1;
                bowData[i].bowAtkStat.rank2 = data.inventoryM.bowAndArrowInv.itemData[i].bowAtk.rank2;
                bowData[i].bowAtkStat.rank3 = data.inventoryM.bowAndArrowInv.itemData[i].bowAtk.rank3;

                bowData[i].bowSpdStat.value = data.inventoryM.bowAndArrowInv.itemData[i].bowSpd.value;
                bowData[i].bowSpdStat.rank1 = data.inventoryM.bowAndArrowInv.itemData[i].bowSpd.rank1;
                bowData[i].bowSpdStat.rank2 = data.inventoryM.bowAndArrowInv.itemData[i].bowSpd.rank2;
                bowData[i].bowSpdStat.rank3 = data.inventoryM.bowAndArrowInv.itemData[i].bowSpd.rank3;

                bowData[i].bowStunStat.value = data.inventoryM.bowAndArrowInv.itemData[i].bowStun.value;
                bowData[i].bowStunStat.rank1 = data.inventoryM.bowAndArrowInv.itemData[i].bowStun.rank1;
                bowData[i].bowStunStat.rank2 = data.inventoryM.bowAndArrowInv.itemData[i].bowStun.rank2;
                bowData[i].bowStunStat.rank3 = data.inventoryM.bowAndArrowInv.itemData[i].bowStun.rank3;

                bowData[i].bowCritHitStat.value = data.inventoryM.bowAndArrowInv.itemData[i].bowCritHit.value;
                bowData[i].bowCritHitStat.rank1 = data.inventoryM.bowAndArrowInv.itemData[i].bowCritHit.rank1;
                bowData[i].bowCritHitStat.rank2 = data.inventoryM.bowAndArrowInv.itemData[i].bowCritHit.rank2;
                bowData[i].bowCritHitStat.rank3 = data.inventoryM.bowAndArrowInv.itemData[i].bowCritHit.rank3;

                bowData[i].bowHdShotStat.value = data.inventoryM.bowAndArrowInv.itemData[i].bowHdShot.value;
                bowData[i].bowHdShotStat.rank1 = data.inventoryM.bowAndArrowInv.itemData[i].bowHdShot.rank1;
                bowData[i].bowHdShotStat.rank2 = data.inventoryM.bowAndArrowInv.itemData[i].bowHdShot.rank2;
                bowData[i].bowHdShotStat.rank3 = data.inventoryM.bowAndArrowInv.itemData[i].bowHdShot.rank3;

                bowData[i].bowDuraStat.value = data.inventoryM.bowAndArrowInv.itemData[i].bowDura.value;
                bowData[i].bowDuraStat.rank1 = data.inventoryM.bowAndArrowInv.itemData[i].bowDura.rank1;
                bowData[i].bowDuraStat.rank2 = data.inventoryM.bowAndArrowInv.itemData[i].bowDura.rank2;
                bowData[i].bowDuraStat.rank3 = data.inventoryM.bowAndArrowInv.itemData[i].bowDura.rank3;

                bowData[i].bowRange.bowRangeType = (Item.BowRangeType)data.inventoryM.bowAndArrowInv.itemData[i].bowRange.bowRangeType;

                bowData[i].bowSpecialStat1 = (Item.BowSpecialStat1)data.inventoryM.bowAndArrowInv.itemData[i].bowSpecialStat1;
                bowData[i].bowSpecialStat2 = (Item.BowSpecialStat2)data.inventoryM.bowAndArrowInv.itemData[i].bowSpecialStat2;

                bowData[i].weight = data.inventoryM.bowAndArrowInv.itemData[i].bowWeight;
                bowData[i].weaponArmsID = data.inventoryM.bowAndArrowInv.itemData[i].weaponArmsID;

                bowData[i].rankNum = data.inventoryM.bowAndArrowInv.itemData[i].rankNum;

                int bowUpgradeLength = data.inventoryM.bowAndArrowInv.itemData[i].upgradeMaterial.Length;
                bowData[i].upgradeMaterial = new Item.UpgradeMaterial[bowUpgradeLength];

                for (int bowUpgradeSize = 0; bowUpgradeSize < bowUpgradeLength; bowUpgradeSize++)
                {
                    bowData[i].upgradeMaterial[bowUpgradeSize].matName = data.inventoryM.bowAndArrowInv.itemData[i].upgradeMaterial[bowUpgradeSize].matName;
                    bowData[i].upgradeMaterial[bowUpgradeSize].matRequired = data.inventoryM.bowAndArrowInv.itemData[i].upgradeMaterial[bowUpgradeSize].matRequired;
                }

                bowData[i].buyPrice = data.inventoryM.bowAndArrowInv.itemData[i].buyPrice;
                bowData[i].sellPrice = data.inventoryM.bowAndArrowInv.itemData[i].sellPrice;
                bowData[i].repairPrice = data.inventoryM.bowAndArrowInv.itemData[i].repairPrice;
                bowData[i].upgradePrice = data.inventoryM.bowAndArrowInv.itemData[i].upgradePrice;
                bowData[i].specialPrice = data.inventoryM.bowAndArrowInv.itemData[i].specialPrice;
                bowData[i].inStockQuantity = data.inventoryM.bowAndArrowInv.itemData[i].inStockQuantity;

                bowData[i].quantity = data.inventoryM.bowAndArrowInv.counter[i];
                bowData[i].stackable = data.inventoryM.bowAndArrowInv.itemData[i].stackable;
                bowData[i].inInventory = data.inventoryM.bowAndArrowInv.itemData[i].inInventory;
                bowData[i].equipped = data.inventoryM.bowAndArrowInv.itemData[i].equipped;

                bowData[i].broken = data.inventoryM.bowAndArrowInv.itemData[i].broken;
                bowData[i].rankTag = data.inventoryM.bowAndArrowInv.itemData[i].rankTag;
                bowData[i].originItemName = data.inventoryM.bowAndArrowInv.itemData[i].originItemName;
                bowData[i].numOfEngraving = data.inventoryM.bowAndArrowInv.itemData[i].numOfEngraving;

                bowData[i].maxBowAtk = data.inventoryM.bowAndArrowInv.itemData[i].maxBowAtk;
                bowData[i].maxBowSpd = data.inventoryM.bowAndArrowInv.itemData[i].maxBowSpd;
                bowData[i].maxBowStun = data.inventoryM.bowAndArrowInv.itemData[i].maxBowStun;
                bowData[i].maxBowCritHit = data.inventoryM.bowAndArrowInv.itemData[i].maxBowCritHit;
                bowData[i].maxBowHdShot = data.inventoryM.bowAndArrowInv.itemData[i].maxBowHdShot;
                bowData[i].maxBowDura = data.inventoryM.bowAndArrowInv.itemData[i].maxBowDura;
            }
        }

        #endregion

        #region Inventory Shield Data Slots

        shieldSlotNum = data.inventoryM.shieldInv.slotNum;
        shieldSlotEquipped = data.inventoryM.shieldInv.slotShieldEquipped;

        int shieldDataLength = data.inventoryM.shieldInv.itemData.Length;
        shdData = new Item[shieldDataLength];

        for (int i = 0; i < shdData.Length; i++)
        {
            if (data.inventoryM.shieldInv.itemData != null)
            {
                shdData[i].itemType = (Item.ItemType)data.inventoryM.shieldInv.itemData[i].itemType;

                shdData[i].itemName = data.inventoryM.shieldInv.itemData[i].itemName;
                shdData[i].itemDescription = data.inventoryM.shieldInv.itemData[i].itemDescription;

                shdData[i].shdAtkStat.value = data.inventoryM.shieldInv.itemData[i].shdAtk.value;
                shdData[i].shdAtkStat.rank1 = data.inventoryM.shieldInv.itemData[i].shdAtk.rank1;
                shdData[i].shdAtkStat.rank2 = data.inventoryM.shieldInv.itemData[i].shdAtk.rank2;
                shdData[i].shdAtkStat.rank3 = data.inventoryM.shieldInv.itemData[i].shdAtk.rank3;

                shdData[i].shdSpdStat.value = data.inventoryM.shieldInv.itemData[i].shdSpd.value;
                shdData[i].shdSpdStat.rank1 = data.inventoryM.shieldInv.itemData[i].shdSpd.rank1;
                shdData[i].shdSpdStat.rank2 = data.inventoryM.shieldInv.itemData[i].shdSpd.rank2;
                shdData[i].shdSpdStat.rank3 = data.inventoryM.shieldInv.itemData[i].shdSpd.rank3;

                shdData[i].shdStunStat.value = data.inventoryM.shieldInv.itemData[i].shdStun.value;
                shdData[i].shdStunStat.rank1 = data.inventoryM.shieldInv.itemData[i].shdStun.rank1;
                shdData[i].shdStunStat.rank2 = data.inventoryM.shieldInv.itemData[i].shdStun.rank2;
                shdData[i].shdStunStat.rank3 = data.inventoryM.shieldInv.itemData[i].shdStun.rank3;

                shdData[i].shdCritHitStat.value = data.inventoryM.shieldInv.itemData[i].shdCritHit.value;
                shdData[i].shdCritHitStat.rank1 = data.inventoryM.shieldInv.itemData[i].shdCritHit.rank1;
                shdData[i].shdCritHitStat.rank2 = data.inventoryM.shieldInv.itemData[i].shdCritHit.rank2;
                shdData[i].shdCritHitStat.rank3 = data.inventoryM.shieldInv.itemData[i].shdCritHit.rank3;

                shdData[i].shdBlockStat.value = data.inventoryM.shieldInv.itemData[i].shdBlock.value;
                shdData[i].shdBlockStat.rank1 = data.inventoryM.shieldInv.itemData[i].shdBlock.rank1;
                shdData[i].shdBlockStat.rank2 = data.inventoryM.shieldInv.itemData[i].shdBlock.rank2;
                shdData[i].shdBlockStat.rank3 = data.inventoryM.shieldInv.itemData[i].shdBlock.rank3;

                shdData[i].shdDuraStat.value = data.inventoryM.shieldInv.itemData[i].shdDura.value;
                shdData[i].shdDuraStat.rank1 = data.inventoryM.shieldInv.itemData[i].shdDura.rank1;
                shdData[i].shdDuraStat.rank2 = data.inventoryM.shieldInv.itemData[i].shdDura.rank2;
                shdData[i].shdDuraStat.rank3 = data.inventoryM.shieldInv.itemData[i].shdDura.rank3;

                shdData[i].shdSpecialStat1 = (Item.ShieldSpecialStat1)data.inventoryM.shieldInv.itemData[i].shdSpecialStat1;
                shdData[i].shdSpecialStat2 = (Item.ShieldSpecialStat2)data.inventoryM.shieldInv.itemData[i].shdSpecialStat2;

                shdData[i].weight = data.inventoryM.shieldInv.itemData[i].shdWeight;
                shdData[i].weaponArmsID = data.inventoryM.shieldInv.itemData[i].weaponArmsID;

                shdData[i].rankNum = data.inventoryM.shieldInv.itemData[i].rankNum;

                int shieldUpgradeLength = data.inventoryM.shieldInv.itemData[i].upgradeMaterial.Length;
                shdData[i].upgradeMaterial = new Item.UpgradeMaterial[shieldUpgradeLength];

                for (int shieldUpgradeSize = 0; shieldUpgradeSize < shieldUpgradeLength; shieldUpgradeSize++)
                {
                    shdData[i].upgradeMaterial[shieldUpgradeSize].matName = data.inventoryM.shieldInv.itemData[i].upgradeMaterial[shieldUpgradeSize].matName;
                    shdData[i].upgradeMaterial[shieldUpgradeSize].matRequired = data.inventoryM.shieldInv.itemData[i].upgradeMaterial[shieldUpgradeSize].matRequired;
                }

                shdData[i].buyPrice = data.inventoryM.shieldInv.itemData[i].buyPrice;
                shdData[i].sellPrice = data.inventoryM.shieldInv.itemData[i].sellPrice;
                shdData[i].repairPrice = data.inventoryM.shieldInv.itemData[i].repairPrice;
                shdData[i].upgradePrice = data.inventoryM.shieldInv.itemData[i].upgradePrice;
                shdData[i].specialPrice = data.inventoryM.shieldInv.itemData[i].specialPrice;
                shdData[i].inStockQuantity = data.inventoryM.shieldInv.itemData[i].inStockQuantity;

                shdData[i].quantity = data.inventoryM.shieldInv.counter[i];
                shdData[i].stackable = data.inventoryM.shieldInv.itemData[i].stackable;
                shdData[i].inInventory = data.inventoryM.shieldInv.itemData[i].inInventory;
                shdData[i].equipped = data.inventoryM.shieldInv.itemData[i].equipped;

                shdData[i].broken = data.inventoryM.shieldInv.itemData[i].broken;
                shdData[i].rankTag = data.inventoryM.shieldInv.itemData[i].rankTag;
                shdData[i].originItemName = data.inventoryM.shieldInv.itemData[i].originItemName;
                shdData[i].numOfEngraving = data.inventoryM.shieldInv.itemData[i].numOfEngraving;

                shdData[i].maxShdAtk = data.inventoryM.shieldInv.itemData[i].maxShdAtk;
                shdData[i].maxShdSpd = data.inventoryM.shieldInv.itemData[i].maxShdSpd;
                shdData[i].maxShdStun = data.inventoryM.shieldInv.itemData[i].maxShdStun;
                shdData[i].maxShdCritHit = data.inventoryM.shieldInv.itemData[i].maxShdCritHit;
                shdData[i].maxShdBlock = data.inventoryM.shieldInv.itemData[i].maxShdBlock;
                shdData[i].maxShdDura = data.inventoryM.shieldInv.itemData[i].maxShdDura;
            }
        }

        #endregion

        #region Inventory Armor Data Slots

        armSlotNum = data.inventoryM.armorInv.slotNum;
        armSlotHeadEquipped = data.inventoryM.armorInv.slotArmorHeadEquipped;
        armSlotChestEquipped = data.inventoryM.armorInv.slotArmorChestEquipped;
        armSlotLegEquipped = data.inventoryM.armorInv.slotArmorLegEquipped;
        armSlotAmuletEquipped = data.inventoryM.armorInv.slotArmorAmuletEquipped;

        int armorDataLength = data.inventoryM.armorInv.itemData.Length;
        armData = new Item[armorDataLength];

        for (int i = 0; i < armData.Length; i++)
        {
            if (data.inventoryM.armorInv.itemData != null)
            {
                armData[i].itemType = (Item.ItemType)data.inventoryM.armorInv.itemData[i].itemType;

                armData[i].itemName = data.inventoryM.armorInv.itemData[i].itemName;
                armData[i].itemDescription = data.inventoryM.armorInv.itemData[i].itemDescription;

                armData[i].armStat.value = data.inventoryM.armorInv.itemData[i].arm.value;
                armData[i].armStat.rank1 = data.inventoryM.armorInv.itemData[i].arm.rank1;
                armData[i].armStat.rank2 = data.inventoryM.armorInv.itemData[i].arm.rank2;
                armData[i].armStat.rank3 = data.inventoryM.armorInv.itemData[i].arm.rank3;

                armData[i].armLResStat.value = data.inventoryM.armorInv.itemData[i].armLRes.value;
                armData[i].armLResStat.rank1 = data.inventoryM.armorInv.itemData[i].armLRes.rank1;
                armData[i].armLResStat.rank2 = data.inventoryM.armorInv.itemData[i].armLRes.rank2;
                armData[i].armLResStat.rank3 = data.inventoryM.armorInv.itemData[i].armLRes.rank3;

                armData[i].armHResStat.value = data.inventoryM.armorInv.itemData[i].armHRes.value;
                armData[i].armHResStat.rank1 = data.inventoryM.armorInv.itemData[i].armHRes.rank1;
                armData[i].armHResStat.rank2 = data.inventoryM.armorInv.itemData[i].armHRes.rank2;
                armData[i].armHResStat.rank3 = data.inventoryM.armorInv.itemData[i].armHRes.rank3;

                armData[i].armStat.armorType = (Item.ArmorType)data.inventoryM.armorInv.itemData[i].arm.armorType;

                armData[i].armSpecialStat1 = (Item.ArmorSpecialStat1)data.inventoryM.armorInv.itemData[i].armSpecialStat1;
                armData[i].armSpecialStat2 = (Item.ArmorSpecialStat2)data.inventoryM.armorInv.itemData[i].armSpecialStat2;

                armData[i].weight = data.inventoryM.armorInv.itemData[i].armWeight;
                armData[i].weaponArmsID = data.inventoryM.armorInv.itemData[i].weaponArmsID;

                armData[i].rankNum = data.inventoryM.armorInv.itemData[i].rankNum;

                int armorUpgradeLength = data.inventoryM.armorInv.itemData[i].upgradeMaterial.Length;
                armData[i].upgradeMaterial = new Item.UpgradeMaterial[armorUpgradeLength];

                for (int armorUpgradeSize = 0; armorUpgradeSize < armorUpgradeLength; armorUpgradeSize++)
                {
                    armData[i].upgradeMaterial[armorUpgradeSize].matName = data.inventoryM.armorInv.itemData[i].upgradeMaterial[armorUpgradeSize].matName;
                    armData[i].upgradeMaterial[armorUpgradeSize].matRequired = data.inventoryM.armorInv.itemData[i].upgradeMaterial[armorUpgradeSize].matRequired;
                }

                armData[i].buyPrice = data.inventoryM.armorInv.itemData[i].buyPrice;
                armData[i].sellPrice = data.inventoryM.armorInv.itemData[i].sellPrice;
                armData[i].repairPrice = data.inventoryM.armorInv.itemData[i].repairPrice;
                armData[i].upgradePrice = data.inventoryM.armorInv.itemData[i].upgradePrice;
                armData[i].specialPrice = data.inventoryM.armorInv.itemData[i].specialPrice;
                armData[i].inStockQuantity = data.inventoryM.armorInv.itemData[i].inStockQuantity;

                armData[i].quantity = data.inventoryM.armorInv.counter[i];
                armData[i].stackable = data.inventoryM.armorInv.itemData[i].stackable;
                armData[i].inInventory = data.inventoryM.armorInv.itemData[i].inInventory;
                armData[i].equipped = data.inventoryM.armorInv.itemData[i].equipped;

                armData[i].broken = data.inventoryM.armorInv.itemData[i].broken;
                armData[i].rankTag = data.inventoryM.armorInv.itemData[i].rankTag;
                armData[i].originItemName = data.inventoryM.armorInv.itemData[i].originItemName;
                armData[i].numOfEngraving = data.inventoryM.armorInv.itemData[i].numOfEngraving;

                armData[i].maxArm = data.inventoryM.armorInv.itemData[i].maxArm;
                armData[i].maxArmLRes = data.inventoryM.armorInv.itemData[i].maxArmLRes;
                armData[i].maxArmHRes = data.inventoryM.armorInv.itemData[i].maxArmHRes;
            }
        }

        #endregion

        #region Inventory Material Data Slots

        matSlotNum = data.inventoryM.materialInv.slotNum;

        int materialDataLength = data.inventoryM.materialInv.itemData.Length;
        matData = new Item[materialDataLength];

        for (int i = 0; i < matData.Length; i++)
        {
            if (data.inventoryM.materialInv.itemData != null)
            {
                matData[i].itemType = (Item.ItemType)data.inventoryM.materialInv.itemData[i].itemType;

                matData[i].itemName = data.inventoryM.materialInv.itemData[i].itemName;
                matData[i].itemDescription = data.inventoryM.materialInv.itemData[i].itemDescription;

                matData[i].buyPrice = data.inventoryM.materialInv.itemData[i].buyPrice;
                matData[i].sellPrice = data.inventoryM.materialInv.itemData[i].sellPrice;
                matData[i].repairPrice = data.inventoryM.materialInv.itemData[i].repairPrice;
                matData[i].upgradePrice = data.inventoryM.materialInv.itemData[i].upgradePrice;
                matData[i].specialPrice = data.inventoryM.materialInv.itemData[i].specialPrice;
                matData[i].inStockQuantity = data.inventoryM.materialInv.itemData[i].inStockQuantity;

                matData[i].quantity = data.inventoryM.materialInv.counter[i];
                matData[i].stackable = data.inventoryM.materialInv.itemData[i].stackable;
                matData[i].inInventory = data.inventoryM.materialInv.itemData[i].inInventory;
                matData[i].equipped = data.inventoryM.materialInv.itemData[i].equipped;
            }
        }

        #endregion

        #region Inventory Healing Data Slots

        healSlotNum = data.inventoryM.healingInv.slotNum;

        int healingDataLength = data.inventoryM.healingInv.itemData.Length;
        healData = new Item[healingDataLength];

        for (int i = 0; i < healData.Length; i++)
        {
            if (data.inventoryM.healingInv.itemData != null)
            {
                healData[i].itemType = (Item.ItemType)data.inventoryM.healingInv.itemData[i].itemType;

                healData[i].itemName = data.inventoryM.healingInv.itemData[i].itemName;
                healData[i].itemDescription = data.inventoryM.healingInv.itemData[i].itemDescription;

                healData[i].buyPrice = data.inventoryM.healingInv.itemData[i].buyPrice;
                healData[i].sellPrice = data.inventoryM.healingInv.itemData[i].sellPrice;
                healData[i].repairPrice = data.inventoryM.healingInv.itemData[i].repairPrice;
                healData[i].upgradePrice = data.inventoryM.healingInv.itemData[i].upgradePrice;
                healData[i].specialPrice = data.inventoryM.healingInv.itemData[i].specialPrice;
                healData[i].inStockQuantity = data.inventoryM.healingInv.itemData[i].inStockQuantity;

                healData[i].quantity = data.inventoryM.healingInv.counter[i];
                healData[i].stackable = data.inventoryM.healingInv.itemData[i].stackable;
                healData[i].inInventory = data.inventoryM.healingInv.itemData[i].inInventory;
                healData[i].equipped = data.inventoryM.healingInv.itemData[i].equipped;
            }
        }

        #endregion

        #region Inventory Key Data Slots

        keySlotNum = data.inventoryM.keyInv.slotNum;

        int keyDataLength = data.inventoryM.keyInv.itemData.Length;
        keyData = new Item[keyDataLength];

        for (int i = 0; i < keyData.Length; i++)
        {
            if (data.inventoryM.keyInv.itemData != null)
            {
                keyData[i].itemType = (Item.ItemType)data.inventoryM.keyInv.itemData[i].itemType;

                keyData[i].itemName = data.inventoryM.keyInv.itemData[i].itemName;
                keyData[i].itemDescription = data.inventoryM.keyInv.itemData[i].itemDescription;

                keyData[i].buyPrice = data.inventoryM.keyInv.itemData[i].buyPrice;
                keyData[i].sellPrice = data.inventoryM.keyInv.itemData[i].sellPrice;
                keyData[i].repairPrice = data.inventoryM.keyInv.itemData[i].repairPrice;
                keyData[i].upgradePrice = data.inventoryM.keyInv.itemData[i].upgradePrice;
                keyData[i].specialPrice = data.inventoryM.keyInv.itemData[i].specialPrice;
                keyData[i].inStockQuantity = data.inventoryM.keyInv.itemData[i].inStockQuantity;

                keyData[i].quantity = data.inventoryM.keyInv.counter[i];
                keyData[i].stackable = data.inventoryM.keyInv.itemData[i].stackable;
                keyData[i].inInventory = data.inventoryM.keyInv.itemData[i].inInventory;
                keyData[i].equipped = data.inventoryM.keyInv.itemData[i].equipped;
            }
        }

        #endregion

        #endregion

        #region Skill Tree Data 

        topAbilityName = data.skillM.topAbilityNameMenu;
        leftAbilityName = data.skillM.leftAbilityNameMenu;
        rightAbilityName = data.skillM.rightAbilityNameMenu;
        bottomAbilityName = data.skillM.bottomAbilityNameMenu;

        int skillSlotLength = data.skillSlots.Length;
        purchasedSkillName = new string[skillSlotLength];
        for (int i = 0; i < purchasedSkillName.Length; i++)
        {
            if (data.skillSlots[i] != null)
            {
                if (data.skillSlots[i].purchasedSkillName != "")
                {
                    purchasedSkillName[i] = data.skillSlots[i].purchasedSkillName;
                }
            }
        }

        #endregion

        #region Miscellaneous

        int exploredLength = data.locationCPData.Length;
        explored = new bool[exploredLength];
        for (int i = 0; i < data.locationCPData.Length; i++)
        {
            explored[i] = data.locationCPData[i].explored;
        }

        int triggerActionLength = data.triggerActionData.Length;
        triggerAction = new TriggerAction[triggerActionLength];
        for (int i = 0; i < triggerAction.Length; i++)
        {
            triggerAction[i].isActive = data.triggerActionData[i].isActive;
            triggerAction[i].interactionType = (TriggerAction.InteractionType)data.triggerActionData[i].interactionType;
            triggerAction[i].destroyAfter = data.triggerActionData[i].destroyAfter;
        }

        #endregion
    }
}