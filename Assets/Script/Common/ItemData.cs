using System.Collections;
using System.Collections.Generic;
using RPGAE.CharacterController;
using UnityEngine;
using JoshH.UI;

public class ItemData : MonoBehaviour, DamageReceiver
{
    [Header("ITEM SETTINGS")]
    public ItemType itemType;
    public Sprite itemSprite;
    public string originItemName;
    [HideInInspector] public string itemName;
    [TextArea(3, 10)]
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

    #region Weapon Stats

    [System.Serializable]
    public struct WeaponAttack
    {
        public int value;
        public int rank1;
        public int rank2;
        public int rank3;
        public int GetRank1Value()
        {
            return value += rank1;
        }
        public int GetRank2Value()
        {
            return value += rank2;
        }
        public int GetRank3Value()
        {
            return value += rank3;
        }
    }

    [System.Serializable]
    public struct WeaponSpeed
    {
        public int value;
        public int rank1;
        public int rank2;
        public int rank3;
        public int GetRank1Value()
        {
            return value += rank1;
        }
        public int GetRank2Value()
        {
            return value += rank2;
        }
        public int GetRank3Value()
        {
            return value += rank3;
        }
    }

    [System.Serializable]
    public struct WeaponStun
    {
        public int value;
        public int rank1;
        public int rank2;
        public int rank3;
        public int GetRank1Value()
        {
            return value += rank1;
        }
        public int GetRank2Value()
        {
            return value += rank2;
        }
        public int GetRank3Value()
        {
            return value += rank3;
        }
    }

    [System.Serializable]
    public struct WeaponCriticalHit
    {
        public int value;
        public int rank1;
        public int rank2;
        public int rank3;
        public int GetRank1Value()
        {
            return value += rank1;
        }
        public int GetRank2Value()
        {
            return value += rank2;
        }
        public int GetRank3Value()
        {
            return value += rank3;
        }
    }

    [System.Serializable]
    public struct WeaponDurability
    {
        public int value;
        public int rank1;
        public int rank2;
        public int rank3;
        public int GetRank1Value()
        {
            return value += rank1;
        }
        public int GetRank2Value()
        {
            return value += rank2;
        }
        public int GetRank3Value()
        {
            return value += rank3;
        }
    }

    [System.Serializable]
    public struct ThrowDistance
    {
        public ThrowRange throwRange;
        public GameObject projectile;
    }

    [Header("WEAPON SETTINGS")]
    public WeaponAttack wpnAtk;
    public WeaponSpeed wpnSpd;
    public WeaponStun wpnStun;
    public WeaponCriticalHit wpnCritHit;
    public WeaponDurability wpnDura;
    public ThrowDistance throwDist;
    public int wpnWeight;
    public float weaponArmsID;

    [Space(10)]
    public WeaponSpecialStat1 wpnSpecialStat1;
    [HideInInspector] public string wpnSpecialStatName1;
    public int wpnSpecialStatValue1;
    [HideInInspector] public string wpnSpecialStatInfo1;

    public WeaponSpecialStat2 wpnSpecialStat2;
    [HideInInspector] public string wpnSpecialStatName2;
    public int wpnSpecialStatValue2;
    [HideInInspector] public string wpnSpecialStatInfo2;

    public int maxWpnAtk;
    public int maxWpnSpd;
    public int maxWpnStun;
    [HideInInspector] public int maxWpnCritHit;
    [HideInInspector] public int maxWpnDura;

    public enum ThrowRange
    {
        None,
        ShortRange,
        MedRange,
        LongRange,
        Endless
    }

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

    #endregion

    #region Bow Stats

    [System.Serializable]
    public struct BowAttack
    {
        public int value;
        public int rank1;
        public int rank2;
        public int rank3;
        public int GetRank1Value()
        {
            return value += rank1;
        }
        public int GetRank2Value()
        {
            return value += rank2;
        }
        public int GetRank3Value()
        {
            return value += rank3;
        }
    }

    [System.Serializable]
    public struct BowSpeed
    {
        public int value;
        public int rank1;
        public int rank2;
        public int rank3;
        public int GetRank1Value()
        {
            return value += rank1;
        }
        public int GetRank2Value()
        {
            return value += rank2;
        }
        public int GetRank3Value()
        {
            return value += rank3;
        }
    }

    [System.Serializable]
    public struct BowStun
    {
        public int value;
        public int rank1;
        public int rank2;
        public int rank3;
        public int GetRank1Value()
        {
            return value += rank1;
        }
        public int GetRank2Value()
        {
            return value += rank2;
        }
        public int GetRank3Value()
        {
            return value += rank3;
        }
    }

    [System.Serializable]
    public struct BowCriticalHit
    {
        public int value;
        public int rank1;
        public int rank2;
        public int rank3;
        public int GetRank1Value()
        {
            return value += rank1;
        }
        public int GetRank2Value()
        {
            return value += rank2;
        }
        public int GetRank3Value()
        {
            return value += rank3;
        }
    }

    [System.Serializable]
    public struct BowDurability
    {
        public int value;
        public int rank1;
        public int rank2;
        public int rank3;
        public int GetRank1Value()
        {
            return value += rank1;
        }
        public int GetRank2Value()
        {
            return value += rank2;
        }
        public int GetRank3Value()
        {
            return value += rank3;
        }
    }

    [System.Serializable]
    public struct BowHeadShot
    {
        public int value;
        public int rank1;
        public int rank2;
        public int rank3;
        public int GetRank1Value()
        {
            return value += rank1;
        }
        public int GetRank2Value()
        {
            return value += rank2;
        }
        public int GetRank3Value()
        {
            return value += rank3;
        }
    }

    [Header("BOW SETTINGS")]
    public BowAttack bowAtk;
    public BowSpeed bowSpd;
    public BowStun bowStun;
    public BowCriticalHit bowCritHit;
    public BowDurability bowDura;
    public BowHeadShot bowHdShot;
    public BowRange bowRange;
    public int bowWeight;

    [Space(10)]
    public BowSpecialStat1 bowSpecialStat1;
    public int bowSpecialStatValue1;
    [HideInInspector] public string bowSpecialStatName1;
    [HideInInspector] public string bowSpecialStatInfo1;

    public BowSpecialStat2 bowSpecialStat2;
    public int bowSpecialStatValue2;
    [HideInInspector] public string bowSpecialStatName2;
    [HideInInspector] public string bowSpecialStatInfo2;

    [HideInInspector] public int maxBowAtk;
    [HideInInspector] public int maxBowSpd;
    [HideInInspector] public int maxBowStun;
    [HideInInspector] public int maxBowHdShot;
    [HideInInspector] public int maxBowCritHit;
    [HideInInspector] public int maxBowDura;

    [System.Serializable]
    public struct BowRange
    {
        public int shotAmount;
        public BowRangeType bowRangeType;
    }

    public enum BowRangeType
    {
        ShortRange,
        MedRange,
        LongRange,
        Endless
    }

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

    #endregion

    #region Shield Stats

    [System.Serializable]
    public struct ShieldAttack
    {
        public int value;
        public int rank1;
        public int rank2;
        public int rank3;
        public int GetRank1Value()
        {
            return value += rank1;
        }
        public int GetRank2Value()
        {
            return value += rank2;
        }
        public int GetRank3Value()
        {
            return value += rank3;
        }
    }

    [System.Serializable]
    public struct ShieldSpeed
    {
        public int value;
        public int rank1;
        public int rank2;
        public int rank3;
        public int GetRank1Value()
        {
            return value += rank1;
        }
        public int GetRank2Value()
        {
            return value += rank2;
        }
        public int GetRank3Value()
        {
            return value += rank3;
        }
    }

    [System.Serializable]
    public struct ShieldStun
    {
        public int value;
        public int rank1;
        public int rank2;
        public int rank3;
        public int GetRank1Value()
        {
            return value += rank1;
        }
        public int GetRank2Value()
        {
            return value += rank2;
        }
        public int GetRank3Value()
        {
            return value += rank3;
        }
    }

    [System.Serializable]
    public struct ShieldCriticalHit
    {
        public int value;
        public int rank1;
        public int rank2;
        public int rank3;
        public int GetRank1Value()
        {
            return value += rank1;
        }
        public int GetRank2Value()
        {
            return value += rank2;
        }
        public int GetRank3Value()
        {
            return value += rank3;
        }
    }

    [System.Serializable]
    public struct ShieldDurability
    {
        public int value;
        public int rank1;
        public int rank2;
        public int rank3;
        public int GetRank1Value()
        {
            return value += rank1;
        }
        public int GetRank2Value()
        {
            return value += rank2;
        }
        public int GetRank3Value()
        {
            return value += rank3;
        }
    }

    [System.Serializable]
    public struct ShieldBlock
    {
        public int value;
        public int rank1;
        public int rank2;
        public int rank3;
        public int GetRank1Value()
        {
            return value += rank1;
        }
        public int GetRank2Value()
        {
            return value += rank2;
        }
        public int GetRank3Value()
        {
            return value += rank3;
        }
    }

    [Header("SHIELD SETTINGS")]
    public ShieldAttack shdAtk;
    public ShieldSpeed shdSpd;
    public ShieldBlock shdBlock;
    public ShieldStun shdStun;
    public ShieldCriticalHit shdCritHit;
    public ShieldDurability shdDura;
    public int shdWeight;

    [Space(10)]
    public ShieldSpecialStat1 shdSpecialStat1;
    [HideInInspector] public string shdSpecialStatName1;
    public int shdSpecialStatValue1;
    [HideInInspector] public string shdSpecialStatInfo1;

    public ShieldSpecialStat2 shdSpecialStat2;
    [HideInInspector] public string shdSpecialStatName2;
    public int shdSpecialStatValue2;
    [HideInInspector] public string shdSpecialStatInfo2;

    [HideInInspector] public int maxShdAtk;
    [HideInInspector] public int maxShdSpd;
    [HideInInspector] public int maxShdBlock;
    [HideInInspector] public int maxShdStun;
    [HideInInspector] public int maxShdCritHit;
    [HideInInspector] public int maxShdDura;

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

    #endregion

    #region Armor Stats

    [System.Serializable]
    public struct Armor
    {
        public int value;
        public int rank1;
        public int rank2;
        public int rank3;
        public ArmorType armorType;
        public int GetRank1Value()
        {
            return value += rank1;
        }
        public int GetRank2Value()
        {
            return value += rank2;
        }
        public int GetRank3Value()
        {
            return value += rank3;
        }
    }

    [System.Serializable]
    public struct ArmorLightHitResistance
    {
        public int value;
        public int rank1;
        public int rank2;
        public int rank3;
        public int GetRank1Value()
        {
            return value += rank1;
        }
        public int GetRank2Value()
        {
            return value += rank2;
        }
        public int GetRank3Value()
        {
            return value += rank3;
        }
    }

    [System.Serializable]
    public struct ArmorHeavyHitResistance
    {
        public int value;
        public int rank1;
        public int rank2;
        public int rank3;
        public int GetRank1Value()
        {
            return value += rank1;
        }
        public int GetRank2Value()
        {
            return value += rank2;
        }
        public int GetRank3Value()
        {
            return value += rank3;
        }
    }

    [Header("ARMOR SETTINGS")]
    public Armor arm;
    public ArmorLightHitResistance armLRes;
    public ArmorHeavyHitResistance armHRes;
    public int armWeight;

    [Space(10)]
    public ArmorSpecialStat1 armSpecialStat1;
    [HideInInspector] public string armSpecialStatName1;
    public int armSpecialStatValue1;
    [HideInInspector] public string armSpecialStatInfo1;

    public ArmorSpecialStat2 armSpecialStat2;
    [HideInInspector] public string armSpecialStatName2;
    public int armSpecialStatValue2;
    [HideInInspector] public string armSpecialStatInfo2;

    [HideInInspector] public int maxArm;
    [HideInInspector] public int maxArmLRes;
    [HideInInspector] public int maxArmHRes;

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

    public enum ArmorSpecialStat2
    {
        None,
        ArmBonus,
        ArmLResBonus,
        ArmHResBonus
    }

    #endregion

    #region Upgrade

    [Header("UPGRADE SETTINGS")]
    [Tooltip("Materials required for upgrade.")]
    public Material[] upgradeMaterial;
    [HideInInspector] public int rankNum;

    [System.Serializable]
    public struct Material
    {
        public string matName;
        public int matRequired;
        public int GetRank1Value()
        {
            return matRequired += matRequired;
        }
        public int GetRank2Value()
        {
            return matRequired += matRequired;
        }
        public int GetRank3Value()
        {
            return matRequired += matRequired;
        }
    }

    #endregion

    #region Store 

    [Header("MARKET SETTINGS")]
    public int buyPrice;
    public int sellPrice;
    public int repairPrice;
    public int upgradePrice;
    public int specialPrice;
    public int inStockQuantity;

    #endregion

    #region Other Settings

    [Header("OTHER SETTINGS")]
    [Tooltip("Item Inventory quantity equivalent to one or higher becomes stackable.")]
    public int quantity;
    public bool stackable;
    public bool inInventory;
    public bool equipped;
    public GameObject destroyedParticle;

    #endregion

    #region Hide 

    public bool itemActive;
    public bool itemObtainedPickUp;
    [HideInInspector] public bool broken;
    [HideInInspector] public string rankTag;
    [HideInInspector] public int numOfEngraving;

    [HideInInspector] public MeshRenderer[] meshRenderer;
    [HideInInspector] public SkinnedMeshRenderer[] skinnedMeshRenderer;
    [HideInInspector] public ThirdPersonController cc;
    [HideInInspector] public ItemInteract itemInteract;
    [HideInInspector] public InventoryManager inventoryM;
    [HideInInspector] public SystemManager systemM;

    #endregion

    void Awake()
    {
        cc = FindObjectOfType<ThirdPersonController>();
        meshRenderer = GetComponentsInChildren<MeshRenderer>();
        skinnedMeshRenderer = GetComponentsInChildren<SkinnedMeshRenderer>();
        itemInteract = GetComponent<ItemInteract>();
        systemM = FindObjectOfType<SystemManager>();
        inventoryM = FindObjectOfType<InventoryManager>();

        if (itemInteract || equipped)
            itemActive = true;
        if (inInventory && !equipped)
            itemActive = false;

        InitializeWeapon();
        ItemActiveToggle();
    }

    void Update()
    {
        ItemActiveToggle();
    }

    void InitializeWeapon()
    {
        if (rankNum == 0)
            itemName = originItemName;
        if (quantity > 0)
            stackable = true;
        if (bowRange.shotAmount == 0)
            bowRange.shotAmount = 1;

        #region Special Weapon Bonus Stat

        if (itemInteract == null)
        {
            if (itemType == ItemType.Weapon)
            {
                switch (wpnSpecialStat1)
                {
                    case WeaponSpecialStat1.AtkBonus:
                        wpnAtk.value += wpnSpecialStatValue1;
                        wpnSpecialStatName1 = "Attack Bonus + " + wpnSpecialStatValue1;
                        wpnSpecialStatInfo1 = "Additional attack points makes your weapon more powerful than before.";
                        numOfEngraving++;
                        break;
                    case WeaponSpecialStat1.SpdBonus:
                        wpnSpd.value += wpnSpecialStatValue1;
                        wpnSpecialStatName1 = "Speed Bonus + " + wpnSpecialStatValue1;
                        wpnSpecialStatInfo1 = "Additional speed points makes your weapon swing a lot more quicker.";
                        numOfEngraving++;
                        break;
                    case WeaponSpecialStat1.StunBonus:
                        wpnStun.value += wpnSpecialStatValue1;
                        wpnSpecialStatName1 = "Stun Bonus + " + wpnSpecialStatValue1;
                        wpnSpecialStatInfo1 = "Additional stun points staggers it's target at a faster pace.";
                        numOfEngraving++;
                        break;
                    case WeaponSpecialStat1.CritHitBonus:
                        wpnCritHit.value += wpnSpecialStatValue1;
                        wpnSpecialStatName1 = "Critical Hit Bonus + " + wpnSpecialStatValue1;
                        wpnSpecialStatInfo1 = "Additional Critical Hit points increases the chance of dealing more damage.";
                        numOfEngraving++;
                        break;
                    case WeaponSpecialStat1.DuraBonus:
                        wpnDura.value += wpnSpecialStatValue1;
                        wpnSpecialStatName1 = "Durability Bonus + " + wpnSpecialStatValue1;
                        wpnSpecialStatInfo1 = "Additional Durability points increases amount of hits before it breaks.";
                        numOfEngraving++;
                        break;
                    case WeaponSpecialStat1.ThrowMedBonus:
                        throwDist.throwRange = ThrowRange.MedRange;
                        wpnSpecialStatValue1 = 0;
                        wpnSpecialStatName1 = "Throw++";
                        wpnSpecialStatInfo1 = "Weapon can be thrown within a medium range area.";
                        numOfEngraving++;
                        break;
                    case WeaponSpecialStat1.ThrowLongBonus:
                        throwDist.throwRange = ThrowRange.LongRange;
                        wpnSpecialStatValue1 = 0;
                        wpnSpecialStatName1 = "Throw+++";
                        wpnSpecialStatInfo1 = "Weapon can be thrown within a long range area.";
                        numOfEngraving++;
                        break;
                    case WeaponSpecialStat1.ThrowEndlessBonus:
                        throwDist.throwRange = ThrowRange.Endless;
                        wpnSpecialStatValue1 = 0;
                        wpnSpecialStatName1 = "Throw∞";
                        wpnSpecialStatInfo1 = "Weapon thrown soars in the air endlessly until contact.";
                        numOfEngraving++;
                        break;
                }
            }

            if (itemType == ItemType.Weapon)
            {
                switch (wpnSpecialStat2)
                {
                    case WeaponSpecialStat2.AtkBonus:
                        wpnAtk.value += wpnSpecialStatValue2;
                        wpnSpecialStatName2 = "Attack Bonus + " + wpnSpecialStatValue2;
                        wpnSpecialStatInfo2 = "Additional attack points makes your weapon more powerful than before.";
                        numOfEngraving++;
                        break;
                    case WeaponSpecialStat2.SpdBonus:
                        wpnSpd.value += wpnSpecialStatValue2;
                        wpnSpecialStatName2 = "Speed Bonus + " + wpnSpecialStatValue2;
                        wpnSpecialStatInfo2 = "Additional speed points makes your weapon swing a lot more quicker.";
                        numOfEngraving++;
                        break;
                    case WeaponSpecialStat2.StunBonus:
                        wpnStun.value += wpnSpecialStatValue2;
                        wpnSpecialStatName2 = "Stun Bonus + " + wpnSpecialStatValue2;
                        wpnSpecialStatInfo2 = "Additional stun points staggers it's target at a faster pace.";
                        numOfEngraving++;
                        break;
                    case WeaponSpecialStat2.CritHitBonus:
                        wpnCritHit.value += wpnSpecialStatValue2;
                        wpnSpecialStatName2 = "Critical Hit Bonus + " + wpnSpecialStatValue2;
                        wpnSpecialStatInfo2 = "Additional Critical Hit points increases the chance of dealing more damage.";
                        numOfEngraving++;
                        break;
                    case WeaponSpecialStat2.DuraBonus:
                        wpnDura.value += wpnSpecialStatValue2;
                        wpnSpecialStatName2 = "Durability Bonus + " + wpnSpecialStatValue2;
                        wpnSpecialStatInfo2 = "Additional Durability points increases amount of hits before it breaks.";
                        numOfEngraving++;
                        break;
                    case WeaponSpecialStat2.ThrowMedBonus:
                        throwDist.throwRange = ThrowRange.MedRange;
                        wpnSpecialStatValue2 = 0;
                        wpnSpecialStatName2 = "Throw++";
                        wpnSpecialStatInfo2 = "Weapon can be thrown within a medium range area.";
                        numOfEngraving++;
                        break;
                    case WeaponSpecialStat2.ThrowLongBonus:
                        throwDist.throwRange = ThrowRange.LongRange;
                        wpnSpecialStatValue2 = 0;
                        wpnSpecialStatName2 = "Throw++";
                        wpnSpecialStatInfo2 = "Weapon can be thrown within a long range area.";
                        numOfEngraving++;
                        break;
                    case WeaponSpecialStat2.ThrowEndlessBonus:
                        throwDist.throwRange = ThrowRange.Endless;
                        wpnSpecialStatValue2 = 0;
                        wpnSpecialStatName2 = "Throw∞";
                        wpnSpecialStatInfo2 = "Weapon thrown soars in the air endlessly until contact.";
                        numOfEngraving++;
                        break;
                }
            }

            if (itemType == ItemType.Bow)
            {
                switch (bowSpecialStat1)
                {
                    case BowSpecialStat1.AtkBonus:
                        bowAtk.value += bowSpecialStatValue1;
                        bowSpecialStatName1 = "Attack Bonus + " + bowSpecialStatValue1;
                        bowSpecialStatInfo1 = "Additional attack points makes your bow more powerful.";
                        numOfEngraving++;
                        break;
                    case BowSpecialStat1.SpdBonus:
                        bowSpd.value += bowSpecialStatValue1;
                        bowSpecialStatName1 = "Speed Bonus + " + bowSpecialStatValue1;
                        bowSpecialStatInfo1 = "Additional speed points makes your bow draw quicker.";
                        numOfEngraving++;
                        break;
                    case BowSpecialStat1.StunBonus:
                        bowStun.value += bowSpecialStatValue1;
                        bowSpecialStatName1 = "Stun Bonus + " + bowSpecialStatValue1;
                        bowSpecialStatInfo1 = "Additional stun points deals more stun damage.";
                        numOfEngraving++;
                        break;
                    case BowSpecialStat1.CritHitBonus:
                        bowCritHit.value += bowSpecialStatValue1;
                        bowSpecialStatName1 = "Critical Hit Bonus + " + bowSpecialStatValue1;
                        bowSpecialStatInfo1 = "Additional Critical Hit points increases the chance of dealing more damage.";
                        numOfEngraving++;
                        break;
                    case BowSpecialStat1.HeadShotBonus:
                        bowHdShot.value = bowSpecialStatValue1;
                        bowSpecialStatName1 = "Head-Shot Bonus + " + bowSpecialStatValue1;
                        bowSpecialStatInfo1 = "additional head shot points deals more damage than before.";
                        numOfEngraving++;
                        break;
                    case BowSpecialStat1.DuraBonus:
                        bowDura.value += bowSpecialStatValue1;
                        bowSpecialStatName1 = "Durability Bonus + " + bowSpecialStatValue1;
                        bowSpecialStatInfo1 = "Additional Durability points increases the use of bow shots before it breaks.";
                        numOfEngraving++;
                        break;
                    case BowSpecialStat1.MedRangeBonus:
                        bowRange.bowRangeType = BowRangeType.MedRange;
                        bowSpecialStatValue1 = 0;
                        bowSpecialStatName1 = "Range++";
                        bowSpecialStatInfo1 = "Weapon can be thrown within a medium range area.";
                        numOfEngraving++;
                        break;
                    case BowSpecialStat1.LongRangeBonus:
                        bowRange.bowRangeType = BowRangeType.LongRange;
                        bowSpecialStatValue1 = 0;
                        bowSpecialStatName1 = "Range+++";
                        bowSpecialStatInfo1 = "Weapon can be thrown within a long range area.";
                        numOfEngraving++;
                        break;
                    case BowSpecialStat1.EndlessRangeBonus:
                        bowRange.bowRangeType = BowRangeType.Endless;
                        bowSpecialStatValue1 = 0;
                        bowSpecialStatName1 = "Range∞";
                        bowSpecialStatInfo1 = "arrow shot from bow soars in the air endlessly until contact.";
                        numOfEngraving++;
                        break;
                    case BowSpecialStat1.QuickShot:
                        bowSpecialStatValue1 = 0;
                        bowSpecialStatName1 = "Quick-Shot";
                        bowSpecialStatInfo1 = "Bow draws arrows instantly.";
                        numOfEngraving++;
                        break;
                    case BowSpecialStat1.ThreeShotBurst:
                        bowSpecialStatValue1 = 0;
                        bowSpecialStatName1 = "Three-Shot Burst";
                        bowSpecialStatInfo1 = "Bow draws and fires three arrows at once.";
                        bowRange.shotAmount = 3;
                        numOfEngraving++;
                        break;
                    case BowSpecialStat1.FiveShotBurst:
                        bowSpecialStatValue1 = 0;
                        bowSpecialStatName1 = "Five-Shot Burst";
                        bowSpecialStatInfo1 = "Bow draws and fires five arrows at once.";
                        bowRange.shotAmount = 5;
                        numOfEngraving++;
                        break;
                }
            }

            if (itemType == ItemType.Bow)
            {
                switch (bowSpecialStat2)
                {
                    case BowSpecialStat2.AtkBonus:
                        bowAtk.value += bowSpecialStatValue2;
                        bowSpecialStatName2 = "Attack Bonus + " + bowSpecialStatValue2;
                        bowSpecialStatInfo2 = "Additional attack points makes your bow more powerful.";
                        numOfEngraving++;
                        break;
                    case BowSpecialStat2.SpdBonus:
                        bowSpd.value += bowSpecialStatValue2;
                        bowSpecialStatName2 = "Speed Bonus + " + bowSpecialStatValue2;
                        bowSpecialStatInfo2 = "Additional speed points makes your bow draw quicker.";
                        numOfEngraving++;
                        break;
                    case BowSpecialStat2.StunBonus:
                        bowStun.value += bowSpecialStatValue2;
                        bowSpecialStatName2 = "Stun Bonus + " + bowSpecialStatValue2;
                        bowSpecialStatInfo2 = "Additional stun points deals more stun damage.";
                        numOfEngraving++;
                        break;
                    case BowSpecialStat2.CritHitBonus:
                        bowCritHit.value += bowSpecialStatValue2;
                        bowSpecialStatName2 = "Critical Hit Bonus + " + bowSpecialStatValue2;
                        bowSpecialStatInfo2 = "Additional Critical Hit points increases the chance of dealing more damage.";
                        numOfEngraving++;
                        break;
                    case BowSpecialStat2.HeadShotBonus:
                        bowHdShot.value = bowSpecialStatValue2;
                        bowSpecialStatName2 = "Head-Shot Bonus + " + bowSpecialStatValue2;
                        bowSpecialStatInfo2 = "additional head shot points deals more damage than before.";
                        numOfEngraving++;
                        break;
                    case BowSpecialStat2.DuraBonus:
                        bowDura.value += bowSpecialStatValue2;
                        bowSpecialStatName2 = "Durability Bonus + " + bowSpecialStatValue2;
                        bowSpecialStatInfo2 = "Additional Durability points increases the use of bow shots before it breaks.";
                        numOfEngraving++;
                        break;
                    case BowSpecialStat2.MedRangeBonus:
                        bowRange.bowRangeType = BowRangeType.MedRange;
                        bowSpecialStatValue2 = 0;
                        bowSpecialStatName2 = "Range++";
                        bowSpecialStatInfo2 = "Weapon can be thrown within a medium range area.";
                        numOfEngraving++;
                        break;
                    case BowSpecialStat2.LongRangeBonus:
                        bowSpecialStatValue2 = 0;
                        bowRange.bowRangeType = BowRangeType.LongRange;
                        bowSpecialStatName2 = "Range+++";
                        bowSpecialStatInfo2 = "Weapon can be thrown within a long range area.";
                        numOfEngraving++;
                        break;
                    case BowSpecialStat2.EndlessRangeBonus:
                        bowSpecialStatValue2 = 0;
                        bowRange.bowRangeType = BowRangeType.Endless;
                        bowSpecialStatName2 = "Range∞";
                        bowSpecialStatInfo2 = "arrow shot from bow soars in the air endlessly until contact.";
                        numOfEngraving++;
                        break;
                    case BowSpecialStat2.QuickShot:
                        bowSpecialStatValue2 = 0;
                        bowSpecialStatName2 = "Quick-Shot";
                        bowSpecialStatInfo2 = "Bow draws arrows instantly.";
                        numOfEngraving++;
                        break;
                    case BowSpecialStat2.ThreeShotBurst:
                        bowSpecialStatValue2 = 0;
                        bowSpecialStatName2 = "Three-Shot Burst";
                        bowSpecialStatInfo2 = "Bow draws and fires three arrows at once.";
                        bowRange.shotAmount = 3;
                        numOfEngraving++;
                        break;
                    case BowSpecialStat2.FiveShotBurst:
                        bowSpecialStatValue2 = 0;
                        bowSpecialStatName2 = "Five-Shot Burst";
                        bowSpecialStatInfo2 = "Bow draws and fires five arrows at once.";
                        bowRange.shotAmount = 5;
                        numOfEngraving++;
                        break;
                }
            }

            if (itemType == ItemType.Shield)
            {
                switch (shdSpecialStat1)
                {
                    case ShieldSpecialStat1.AtkBonus:
                        shdAtk.value += shdSpecialStatValue1;
                        shdSpecialStatName1 = "Attack Bonus + " + shdSpecialStatValue1;
                        shdSpecialStatInfo1 = "Additional attack points makes your shield bash more powerful than before.";
                        numOfEngraving++;
                        break;
                    case ShieldSpecialStat1.SpdBonus:
                        shdSpd.value += shdSpecialStatValue1;
                        shdSpecialStatName1 = "Speed Bonus + " + shdSpecialStatValue1;
                        shdSpecialStatInfo1 = "Additional speed points makes your shield a lot more quicker.";
                        numOfEngraving++;
                        break;
                    case ShieldSpecialStat1.StunBonus:
                        shdStun.value += shdSpecialStatValue1;
                        shdSpecialStatName1 = "Stun Bonus + " + shdSpecialStatValue1;
                        shdSpecialStatInfo1 = "Additional stun points staggers it's target at a faster pace.";
                        numOfEngraving++;
                        break;
                    case ShieldSpecialStat1.CritHitBonus:
                        shdCritHit.value += shdSpecialStatValue1;
                        shdSpecialStatName1 = "Critical Hit Bonus + " + shdSpecialStatValue1;
                        shdSpecialStatInfo1 = "Additional Critical Hit points increases the chance of dealing more damage.";
                        numOfEngraving++;
                        break;
                    case ShieldSpecialStat1.ShieldBlockBonus:
                        shdBlock.value += shdSpecialStatValue1;
                        shdSpecialStatName1 = "Shield Block Bonus + " + shdSpecialStatValue1;
                        shdSpecialStatInfo1 = "Additional shield block points increases the chance of dealing more damage.";
                        numOfEngraving++;
                        break;
                    case ShieldSpecialStat1.DuraBonus:
                        bowDura.value += shdSpecialStatValue1;
                        shdSpecialStatName1 = "Durability Bonus + " + shdSpecialStatValue1;
                        shdSpecialStatInfo1 = "Additional Durability points increases the use of shield blocks before it breaks.";
                        numOfEngraving++;
                        break;
                }
            }

            if (itemType == ItemType.Shield)
            {
                switch (shdSpecialStat2)
                {
                    case ShieldSpecialStat2.AtkBonus:
                        shdAtk.value += shdSpecialStatValue2;
                        shdSpecialStatName2 = "Attack Bonus + " + shdSpecialStatValue2;
                        shdSpecialStatInfo2 = "Additional attack points makes your shield bash more powerful than before.";
                        numOfEngraving++;
                        break;
                    case ShieldSpecialStat2.SpdBonus:
                        shdSpd.value += shdSpecialStatValue2;
                        shdSpecialStatName2 = "Speed Bonus + " + shdSpecialStatValue2;
                        shdSpecialStatInfo2 = "Additional speed points makes your shield a lot more quicker.";
                        numOfEngraving++;
                        break;
                    case ShieldSpecialStat2.StunBonus:
                        shdStun.value += shdSpecialStatValue2;
                        shdSpecialStatName2 = "Stun Bonus + " + shdSpecialStatValue2;
                        shdSpecialStatInfo2 = "Additional stun points staggers it's target at a faster pace.";
                        numOfEngraving++;
                        break;
                    case ShieldSpecialStat2.CritHitBonus:
                        shdCritHit.value += shdSpecialStatValue2;
                        shdSpecialStatName2 = "Critical Hit Bonus + " + shdSpecialStatValue2;
                        shdSpecialStatInfo2 = "Additional Critical Hit points increases the chance of dealing more damage.";
                        numOfEngraving++;
                        break;
                    case ShieldSpecialStat2.ShieldBlockBonus:
                        shdBlock.value += shdSpecialStatValue2;
                        shdSpecialStatName2 = "Shield Block Bonus + " + shdSpecialStatValue2;
                        shdSpecialStatInfo2 = "Additional shield block points increases the chance of dealing more damage.";
                        numOfEngraving++;
                        break;
                    case ShieldSpecialStat2.DuraBonus:
                        shdDura.value += shdSpecialStatValue2;
                        shdSpecialStatName2 = "Durability Bonus + " + shdSpecialStatValue2;
                        shdSpecialStatInfo2 = "Additional Durability points increases the use of shield blocks before it breaks.";
                        numOfEngraving++;
                        break;
                }
            }

            if (itemType == ItemType.Armor)
            {
                switch (armSpecialStat1)
                {
                    case ArmorSpecialStat1.ArmBonus:
                        arm.value += armSpecialStatValue1;
                        armSpecialStatName1 = "Armor Bonus + " + armSpecialStatValue1;
                        armSpecialStatInfo1 = "additional armor points adds resistance to phyiscal damage.";
                        numOfEngraving++;
                        break;
                    case ArmorSpecialStat1.ArmLResBonus:
                        armLRes.value += armSpecialStatValue1;
                        armSpecialStatName1 = "Armor Light Resistance Bonus + " + armSpecialStatValue1;
                        armSpecialStatInfo1 = "additional armor light attack points adds resistance to light phyiscal damage.";
                        numOfEngraving++;
                        break;
                    case ArmorSpecialStat1.ArmHResBonus:
                        armHRes.value += armSpecialStatValue1;
                        armSpecialStatName1 = "Armor Heavy Resistance Bonus + " + armSpecialStatValue1;
                        armSpecialStatInfo1 = "additional armor heavy attack points adds resistance to heavy phyiscal damage.";
                        numOfEngraving++;
                        break;
                }
            }

            if (itemType == ItemType.Armor)
            {
                switch (armSpecialStat2)
                {
                    case ArmorSpecialStat2.ArmBonus:
                        arm.value += armSpecialStatValue2;
                        armSpecialStatName2 = "Armor Bonus + " + armSpecialStatValue2;
                        armSpecialStatInfo2 = "additional armor points adds resistance to phyiscal damage.";
                        numOfEngraving++;
                        break;
                    case ArmorSpecialStat2.ArmLResBonus:
                        armLRes.value += armSpecialStatValue2;
                        armSpecialStatName2 = "Armor Light Resistance Bonus + " + armSpecialStatValue2;
                        armSpecialStatInfo2 = "additional armor light attack points adds resistance to light phyiscal damage.";
                        numOfEngraving++;
                        break;
                    case ArmorSpecialStat2.ArmHResBonus:
                        armHRes.value += armSpecialStatValue2;
                        armSpecialStatName2 = "Armor Heavy Resistance Bonus + " + armSpecialStatValue2;
                        armSpecialStatInfo2 = "additional armor heavy attack points adds resistance to heavy phyiscal damage.";
                        numOfEngraving++;
                        break;
                }
            }
        }

        #endregion

        maxWpnAtk = wpnAtk.value;
        maxWpnSpd = wpnSpd.value;
        maxWpnStun = wpnStun.value;
        maxWpnCritHit = wpnCritHit.value;
        maxWpnDura = wpnDura.value;

        maxBowAtk = bowAtk.value;
        maxBowSpd = bowSpd.value;
        maxBowStun = bowStun.value;
        maxBowHdShot = bowHdShot.value;
        maxBowCritHit = bowCritHit.value;
        maxBowDura = bowDura.value;

        maxShdAtk = shdAtk.value;
        maxShdSpd = shdSpd.value;
        maxShdStun = shdStun.value;
        maxShdBlock = bowHdShot.value;
        maxShdCritHit = shdCritHit.value;
        maxShdDura = shdDura.value;

        maxArm = arm.value;
        maxArmLRes = armLRes.value;
        maxArmHRes = armHRes.value;
    }

    void ItemActiveToggle()
    {
        if (itemActive)
        {
            foreach (MeshRenderer mesh in meshRenderer)
            {
                if(mesh.tag != "Item") mesh.tag = "Item";

                if (!mesh.enabled && !inventoryM.isPauseMenuOn)
                {
                    if (itemInteract)
                    {
                        itemInteract.rigidBody.useGravity = true;
                        itemInteract.rigidBody.isKinematic = false;
                    }
                    if (itemInteract && itemInteract.interactUI)
                    {
                        itemInteract.interactUI.GetComponent<InteractWorldSpaceUI>().fadeUI.canvasGroup.alpha = 0;
                    }
                }

                bool conditions = systemM.blackScreenFUI.canvasGroup.alpha == 1;
                if (!inventoryM.isPauseMenuOn && conditions)
                    mesh.enabled = true;
                else if (inventoryM.isPauseMenuOn)
                    mesh.enabled = false;
            }

            foreach (SkinnedMeshRenderer smesh in skinnedMeshRenderer)
            {
                if (smesh.tag != "Item") smesh.tag = "Item";

                if (!smesh.enabled && !inventoryM.isPauseMenuOn)
                {
                    if (itemInteract)
                    {
                        itemInteract.rigidBody.useGravity = true;
                        itemInteract.rigidBody.isKinematic = false;
                    }
                    if (itemInteract && itemInteract.interactUI)
                    {
                        itemInteract.interactUI.GetComponent<InteractWorldSpaceUI>().fadeUI.canvasGroup.alpha = 0;
                    }
                }

                bool conditions = systemM.blackScreenFUI.canvasGroup.alpha == 1;
                if (!inventoryM.isPauseMenuOn && conditions)
                    smesh.enabled = true;
                else if (inventoryM.isPauseMenuOn)
                    smesh.enabled = false;
            }

            if (itemInteract)
            {
                foreach (Collider col in itemInteract.colliders)
                {
                    if (!col.enabled)
                    {
                        col.enabled = true;
                    }
                }
            }
        }
        else
        {
            if (cc.wpnHolster.PrimaryWeaponActive() || cc.wpnHolster.alteredPrimaryPrimaryActive() ||
                cc.wpnHolster.SecondaryActive() || cc.wpnHolster.ShieldActive())
                return;

            foreach (MeshRenderer mesh in meshRenderer)
            {
                if (mesh.tag != "Item") mesh.tag = "Item";
                if (mesh.enabled)
                {
                    if (itemInteract)
                    {
                        itemInteract.itemOnGroundParticle.Stop();
                        itemInteract.rigidBody.useGravity = false;
                        itemInteract.rigidBody.isKinematic = true;
                    }
                    if (itemInteract && itemInteract.interactUI)
                        itemInteract.interactUI.GetComponent<InteractWorldSpaceUI>().fadeUI.canvasGroup.alpha = 0;
                }

                mesh.enabled = false;
            }
            foreach (SkinnedMeshRenderer smesh in skinnedMeshRenderer)
            {
                if (smesh.tag != "Item") smesh.tag = "Item";
                if (smesh.enabled)
                {
                    if (itemInteract)
                    {
                        itemInteract.itemOnGroundParticle.Stop();
                        itemInteract.rigidBody.useGravity = false;
                        itemInteract.rigidBody.isKinematic = true;
                    }

                    if (itemInteract && itemInteract.interactUI)
                        itemInteract.interactUI.GetComponent<InteractWorldSpaceUI>().fadeUI.canvasGroup.alpha = 0;

                    smesh.enabled = false;
                }

                smesh.enabled = false;
            }
            if (itemInteract)
            {
                foreach (Collider col in itemInteract.colliders)
                {
                    if (col.enabled)
                    {
                        col.enabled = false;
                    }
                }
            }
        }
    }

    #region Damage

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
                    Destroyed(damageData);
                }
                break;
        }
    }

    void Damaged(HealthPoints.DamageData Data)
    {
        if (itemType == ItemType.Arrow) return;

        HitBox hitB = GetComponent<HitBox>();

        if (itemType == ItemType.Shield)
        {
            hitB.PlayRandomSound("BlockAS", false);
            if (!cc.slowDown)
            {
                Instantiate(hitB.effects.obstacleEffectHit, transform.position, transform.rotation);
            }
        }
    }

    public void Destroyed(HealthPoints.DamageData damageMessage)
    {
        if (GetComponentInParent<AIController>() != null || itemType == ItemType.Arrow) return;

        cc.preventAtkInteruption = true;

        HitBox hitB = GetComponent<HitBox>();

        hitB.PlayRandomSound("BreakAS", true);
        cc.infoMessage.info.text = "(" + itemName + ")" + " is broken";
        if (destroyedParticle)
            Instantiate(destroyedParticle, transform.position, transform.rotation);

        if (itemType == ItemType.Weapon)
        {
            if (inventoryM.itemRepair)
            {
                for (int i = 0; i < inventoryM.weaponInv.itemData.Length; i++)
                {
                    if (inventoryM.weaponInv.itemData[i].equipped)
                    {
                        inventoryM.weaponInv.highLight[i].GetComponent<UIGradient>().enabled = true;
                        inventoryM.weaponInv.highLight[i].GetComponent<UIGradient>().LinearColor1 = inventoryM.brokenColor1;
                        inventoryM.weaponInv.highLight[i].GetComponent<UIGradient>().LinearColor2 = inventoryM.brokenColor2;
                        inventoryM.weaponInv.itemData[i].equipped = false;
                        inventoryM.weaponInv.itemData[i].broken = true;
                    }
                }
            }
            else
            {
                for (int i = 0; i < inventoryM.weaponInv.itemData.Length; i++)
                {
                    if (!inventoryM.weaponInv.itemData[i].stackable && inventoryM.weaponInv.itemData[i].equipped)
                    {
                        inventoryM.RemoveNonStackableItem(ref inventoryM.weaponInv.itemData, ref inventoryM.weaponInv.image, ref inventoryM.weaponInv.highLight, ref inventoryM.weaponInv.outLineBorder, ref inventoryM.weaponInv.statValueBG, ref inventoryM.weaponInv.quantity,
                        ref inventoryM.weaponInv.statValue, ref inventoryM.weaponInv.slotWeaponEquipped, ref inventoryM.weaponInv.removeNullSlots, ref inventoryM.weaponInv.counter, ref inventoryM.weaponInv.statCounter);
                    }
                    else if (inventoryM.weaponInv.itemData[i].stackable && inventoryM.weaponInv.itemData[i].equipped)
                    {
                        inventoryM.RemoveStackableItem(ref inventoryM.weaponInv.itemData, ref inventoryM.weaponInv.image, ref inventoryM.weaponInv.highLight, ref inventoryM.weaponInv.outLineBorder, ref inventoryM.weaponInv.statValueBG,
                        ref inventoryM.weaponInv.quantity, ref inventoryM.weaponInv.statValue, ref inventoryM.weaponInv.slotWeaponEquipped, ref inventoryM.weaponInv.counter,
                        ref inventoryM.weaponInv.statCounter, ref inventoryM.weaponInv.removeNullSlots, 1);
                    }
                }
            }

            hitB.oneShotHP = false;
            inventoryM.DeactiveWeapons();
            inventoryM.DeactiveWeaponsHP();
        }

        if (itemType == ItemType.Bow)
        {
            if (inventoryM.itemRepair)
            {
                for (int i = 0; i < inventoryM.bowAndArrowInv.itemData.Length; i++)
                {
                    if (inventoryM.bowAndArrowInv.itemData[i].equipped)
                    {
                        inventoryM.bowAndArrowInv.highLight[i].GetComponent<UIGradient>().enabled = true;
                        inventoryM.bowAndArrowInv.highLight[i].GetComponent<UIGradient>().LinearColor1 = inventoryM.brokenColor1; 
                        inventoryM.bowAndArrowInv.highLight[i].GetComponent<UIGradient>().LinearColor2 = inventoryM.brokenColor2;
                        inventoryM.bowAndArrowInv.itemData[i].equipped = false;
                        inventoryM.bowAndArrowInv.itemData[i].broken = true;
                    }
                }
            }
            else
            {
                for (int i = 0; i < inventoryM.bowAndArrowInv.itemData.Length; i++)
                {
                    if (!inventoryM.bowAndArrowInv.itemData[i].stackable && inventoryM.bowAndArrowInv.itemData[i].equipped)
                    {
                        inventoryM.RemoveNonStackableItem(ref inventoryM.bowAndArrowInv.itemData, ref inventoryM.weaponInv.image, ref inventoryM.bowAndArrowInv.highLight, ref inventoryM.bowAndArrowInv.outLineBorder, ref inventoryM.bowAndArrowInv.statValueBG, ref inventoryM.bowAndArrowInv.quantity,
                        ref inventoryM.bowAndArrowInv.statValue, ref inventoryM.bowAndArrowInv.slotNum, ref inventoryM.weaponInv.removeNullSlots, ref inventoryM.bowAndArrowInv.counter, ref inventoryM.bowAndArrowInv.statCounter);
                    }
                    else if (inventoryM.bowAndArrowInv.itemData[i].stackable && inventoryM.bowAndArrowInv.itemData[i].equipped)
                    {
                        inventoryM.RemoveStackableItem(ref inventoryM.bowAndArrowInv.itemData, ref inventoryM.bowAndArrowInv.image, ref inventoryM.bowAndArrowInv.highLight, ref inventoryM.bowAndArrowInv.outLineBorder, ref inventoryM.bowAndArrowInv.statValueBG,
                        ref inventoryM.bowAndArrowInv.quantity, ref inventoryM.bowAndArrowInv.statValue, ref inventoryM.bowAndArrowInv.slotBowEquipped, ref inventoryM.bowAndArrowInv.counter,
                        ref inventoryM.bowAndArrowInv.statCounter, ref inventoryM.bowAndArrowInv.removeNullSlots, 1);
                    }
                }
            }

            hitB.oneShotHP = false;
            inventoryM.DeactiveBows();
            inventoryM.DeactiveBowsHP();
            inventoryM.DeactiveArrows();
            inventoryM.DeactiveArrowsHP();
        }

        if (itemType == ItemType.Shield)
        {
            cc.isBlocking = false;
            cc.animator.CrossFadeInFixedTime("Shield Break", 0.2f);

            if (inventoryM.itemRepair)
            {
                for (int i = 0; i < inventoryM.shieldInv.itemData.Length; i++)
                {
                    if (inventoryM.shieldInv.itemData[i].equipped)
                    {
                        inventoryM.shieldInv.highLight[i].GetComponent<UIGradient>().enabled = true;
                        inventoryM.shieldInv.highLight[i].GetComponent<UIGradient>().LinearColor1 = inventoryM.brokenColor1;
                        inventoryM.shieldInv.highLight[i].GetComponent<UIGradient>().LinearColor2 = inventoryM.brokenColor2;
                        inventoryM.shieldInv.itemData[i].equipped = false;
                        inventoryM.shieldInv.itemData[i].broken = true;
                    }
                }
            }
            else
            {
                for (int i = 0; i < inventoryM.shieldInv.itemData.Length; i++)
                {
                    if (!inventoryM.shieldInv.itemData[i].stackable && inventoryM.shieldInv.itemData[i].equipped)
                    {
                        inventoryM.RemoveNonStackableItem(ref inventoryM.shieldInv.itemData, ref inventoryM.shieldInv.image, ref inventoryM.shieldInv.highLight, ref inventoryM.shieldInv.outLineBorder, ref inventoryM.shieldInv.statValueBG, ref inventoryM.shieldInv.quantity,
                        ref inventoryM.shieldInv.statValue, ref inventoryM.shieldInv.slotNum, ref inventoryM.shieldInv.removeNullSlots, ref inventoryM.shieldInv.counter, ref inventoryM.shieldInv.statCounter);
                    }
                    else if (inventoryM.shieldInv.itemData[i].stackable && inventoryM.shieldInv.itemData[i].equipped)
                    {
                        inventoryM.RemoveStackableItem(ref inventoryM.shieldInv.itemData, ref inventoryM.shieldInv.image, ref inventoryM.shieldInv.highLight, ref inventoryM.shieldInv.outLineBorder, ref inventoryM.shieldInv.statValueBG,
                        ref inventoryM.shieldInv.quantity, ref inventoryM.shieldInv.statValue, ref inventoryM.shieldInv.slotShieldEquipped, ref inventoryM.shieldInv.counter,
                        ref inventoryM.shieldInv.statCounter, ref inventoryM.shieldInv.removeNullSlots, 1);
                    }
                }
            }

            hitB.oneShotHP = false;
            inventoryM.DeactiveShield();
            inventoryM.DeactiveShieldHP();
        }
        gameObject.SetActive(false);
    }

    #endregion
}