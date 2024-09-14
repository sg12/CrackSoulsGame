using System.Collections;
using System.Collections.Generic;
using RPGAE.CharacterController;
using UnityEngine;

public class WeaponHolster : MonoBehaviour
{
    [HideInInspector] 
    public int slotWeapon;

    public QuiverArrows[] quiverArrows;

    public GameObject arrow1;

    public GameObject arrow2;

    public GameObject arrow3;

    public GameObject arrow4;

    [Header("IN GAME HOLSTER")]
    public GameObject windScarH;
    public GameObject windScarNoParticlesH;
    public GameObject clericsStaffH;
    public GameObject assasinsDaggerH;
    public GameObject theTuningForkH;
    public GameObject obsidianFuryH;
    public GameObject glaiveH;
    public GameObject quiverH;
    public GameObject commonArrowH;
    public GameObject particleArrowH;
    public GameObject warbowH;
    public GameObject luRifleH;
    public GameObject circleShieldH;

    [Header("INVENTORY WEAPON HOLSTER PREVIEW")]
    public GameObject windScarHP;
    public GameObject windScarNoParticlesHP;
    public GameObject clericsStaffHP;
    public GameObject assasinsDaggerHP;
    public GameObject theTuningForkHP;
    public GameObject obsidianFuryHP;
    public GameObject glaiveHP;
    public GameObject quiverHP;
    public GameObject commonArrowHP;
    public GameObject particleArrowHP;
    public GameObject warbowHP;
    public GameObject luRifleHP;
    public GameObject circleShieldHP;

    [Header("INVENTORY WEAPON PREVIEW")]
    public GameObject windScarEP;
    public GameObject windScarNoParticlesEP;
    public GameObject clericsStaffEP;
    public GameObject assasinsDaggerEP;
    public GameObject theTuningForkEP;
    public GameObject obsidianFuryEP;
    public GameObject glaiveEP;
    public GameObject commonArrowEP;
    public GameObject particleArrowEP;
    public GameObject warbowEP;
    public GameObject luRifleEP;
    public GameObject circleShieldEP;

    [Header("EQUIPPED WEAPON")]
    public GameObject windScarE;
    public GameObject windScarNoParticleE;
    public GameObject clericsStaffE;
    public GameObject assasinsDaggerE;
    public GameObject theTuningForkE;
    public GameObject obsidianFuryE;
    public GameObject glaiveE;
    public GameObject circleShieldE;
    public GameObject warbowE;
    public GameObject luRifleE;
    public GameObject luRiflePrefabSpot;
    public GameObject warbowString;
    public GameObject warbowPrefabSpot;
    public GameObject bowStrings;
    public GameObject commonArrowE;
    public GameObject particleArrowE;
    public GameObject commonArrowString;
    public GameObject particleArrowString;

    [Header("PREFAB WEAPON")]
    public GameObject windScarNoParticlePrefab;
    public GameObject clericsStaffPrefab;
    public GameObject assasinsDaggerPrefab;
    public GameObject theTuningForkPrefab;
    public GameObject obsidianFuryPrefab;
    public GameObject glaivePrefab;
    public GameObject commonArrowPrefab;
    public GameObject particleArrowPrefab;
    public GameObject warbowPrefab;
    public GameObject luRiflePrefab;
    public GameObject SevenSixTwoAmmoPrefab;
    public GameObject circleShieldPrefab;
    public GameObject goldBarPrefab;
    public GameObject silverBarPrefab;
    public GameObject emeraldPrefab;
    public GameObject rubyPrefab;
    public GameObject elixirofHealthPrefab;
    public GameObject elixirofEnergyPrefab;

    [HideInInspector]
    public GameObject swordEP, dSwordEP, spearEP, staffEP, hammerEP, bowEP,
    arrowEP, gunEP, gunHP,  alteredPrimaryEP, shieldP, swordHP, dSwordHP, spearHP, staffHP, hammerHP, bowHP, 
    arrowHP, shieldHP, alteredPrimaryHP, primaryH, secondaryH, shieldH, arrowH, alteredPrimaryH, headArmorE, 
    chestArmorE, legsArmorE, amuletArmorE, primaryE, primaryEP, alteredPrimaryE, 
    commonItemD, primaryD, secondaryE, secondaryD, shieldE, shieldD, bowString,
    arrowE, arrowString, arrowD, arrowPrefabSpot;

    [HideInInspector]
    public ThirdPersonController cc;
    [HideInInspector]
    private InventoryManager inventoryM;
    [HideInInspector]
    public HumanoidBehavior humanBehavior;
    [HideInInspector]
    public HUDManager hudM;

    [HideInInspector] public bool itemBeingDropped;

    // Start is called before the first frame update
    void Start()
    {
        SetHolster();
        inventoryM = FindObjectOfType<InventoryManager>();
        hudM = FindObjectOfType<HUDManager>();
    }

    void SetHolster()
    {
        if (!cc)
            cc = GetComponentInParent<ThirdPersonController>();
        if (!humanBehavior)
            humanBehavior = GetComponentInParent<HumanoidBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        if (cc != null && cc.useStatSpeed)
            SetWeaponSpeed();
    }

    #region Weapon Speed

    void SetWeaponSpeed()
    {
        if (PrimaryWeaponActive())
        {
            if (cc.fullBodyInfo.IsTag("Light Attack") || cc.fullBodyInfo.IsTag("Heavy Attack") || cc.fullBodyInfo.IsTag("Aerial Attack"))
            {
                cc.animator.speed += GetPlayerSpeed() + GetWeaponSpeed() + GetShieldSpeed() / 1000f;
                cc.animator.speed = Mathf.Clamp(cc.animator.speed, 1, 1.5f);
            }
            else
                cc.animator.speed = 1;
        }

        if (SecondaryActive())
        {
            if (cc.upperBodyInfo.IsTag("Aiming"))
            {
                secondaryE.GetComponent<Animator>().speed += GetPlayerSpeed() + GetBowSpeed() / 1000f;
                secondaryE.GetComponent<Animator>().speed = Mathf.Clamp(cc.animator.speed, 1, 10);
            }
            else
                secondaryE.GetComponent<Animator>().speed = 1;
        }
    }

    float GetWeaponSpeed()
    {
        if (primaryE.GetComponent<ItemData>().maxWpnSpd > 0)
        {
            return primaryE.GetComponent<ItemData>().maxWpnSpd;
        }
        else
            return 0;
    }

    float GetBowSpeed()
    {
        if (secondaryE.GetComponent<ItemData>().maxWpnSpd > 0)
        {
            return secondaryE.GetComponent<ItemData>().maxWpnSpd;
        }
        else
            return 0;
    }

    float GetShieldSpeed()
    {
        if (ShieldActive() && shieldE.GetComponent<ItemData>().maxShdSpd > 0)
        {
            return shieldE.GetComponent<ItemData>().maxShdSpd;
        }
        else
            return 0;
    }

    float GetPlayerSpeed()
    {
        if (cc.hudM.agility > 0)
        {
            return cc.hudM.agility;
        }
        else
            return 0;
    }

    #endregion

    #region Weapon Stamina

    public void WeaponStamina()
    {
        if (hudM.curEnegry <= 0)
            return;

        if (!PrimaryWeaponActive() && !ShieldActive() && !SecondaryActive())
        {
            if (cc.fullBodyInfo.IsName("Heavy Attack"))
            {
                hudM.ReduceEnegry(10 * 2, false);
            }
            else if (cc.fullBodyInfo.IsName("Ability Attack"))
            {
                hudM.ReduceEnegry(10 * 3, false);
            }
            else
                hudM.ReduceEnegry(10, false);
            return;
        }

        if (PrimaryWeaponActive() && !ShieldActive() && hudM.curEnegry > primaryE.GetComponent<ItemData>().wpnWeight)
        {
            if (cc.fullBodyInfo.IsName("Heavy Attack"))
            {
                hudM.ReduceEnegry(primaryE.GetComponent<ItemData>().wpnWeight * 2, false);
            }
            else if (cc.fullBodyInfo.IsName("Ability Attack"))
            {
                hudM.ReduceEnegry(primaryE.GetComponent<ItemData>().wpnWeight * 3, false);
            }
            else
                hudM.ReduceEnegry(primaryE.GetComponent<ItemData>().wpnWeight, false);
            return;
        }

        if (PrimaryWeaponActive() && ShieldActive() && hudM.curEnegry > (primaryE.GetComponent<ItemData>().wpnWeight +
            shieldE.GetComponent<ItemData>().shdWeight))
        {
            if (cc.fullBodyInfo.IsName("Heavy Attack"))
            {
                int value = (primaryE.GetComponent<ItemData>().wpnWeight + shieldE.GetComponent<ItemData>().shdWeight) * 2;
                hudM.ReduceEnegry(value, false);
            }
            else if (cc.fullBodyInfo.IsName("Ability Attack"))
            {
                int value = (primaryE.GetComponent<ItemData>().wpnWeight + shieldE.GetComponent<ItemData>().shdWeight) * 3;
                hudM.ReduceEnegry(value, false);
            }
            else
            {
                int value = (primaryE.GetComponent<ItemData>().wpnWeight + shieldE.GetComponent<ItemData>().shdWeight);
                hudM.ReduceEnegry(value, false);
            }
            return;
        }

        if (ShieldActive() && !PrimaryWeaponActive() && hudM.curEnegry > shieldE.GetComponent<ItemData>().shdWeight)
        {
            if (cc.fullBodyInfo.IsName("Ability Attack"))
            {
                int value = shieldE.GetComponent<ItemData>().shdWeight * 3;
                hudM.ReduceEnegry(value, false);
            }
            else
            {
                int value = shieldE.GetComponent<ItemData>().shdWeight;
                hudM.ReduceEnegry(value, false);
            }
            return;
        }
    }

    public bool LightAttackStaminaConditions()
    {
        if (!PrimaryWeaponActive() && !ShieldActive() && !SecondaryActive() && hudM.curEnegry > 10)
            return true;
        if (PrimaryWeaponActive() && hudM.curEnegry > primaryE.GetComponent<ItemData>().wpnWeight)
            return true;
        if (PrimaryWeaponActive() && ShieldActive() && hudM.curEnegry > (primaryE.GetComponent<ItemData>().wpnWeight + primaryE.GetComponent<ItemData>().shdWeight))
            return true;
        if (ShieldActive() && hudM.curEnegry > shieldE.GetComponent<ItemData>().shdWeight)
            return true;

        return false;
    }

    public bool HeavyAttackStaminaConditions()
    {
        if (!PrimaryWeaponActive() && !ShieldActive() && !SecondaryActive() && hudM.curEnegry > (10 * 2))
            return true;
        if (PrimaryWeaponActive() && hudM.curEnegry > (primaryE.GetComponent<ItemData>().wpnWeight * 2))
            return true;
        if (PrimaryWeaponActive() && ShieldActive() && hudM.curEnegry > (primaryE.GetComponent<ItemData>().wpnWeight + primaryE.GetComponent<ItemData>().shdWeight * 2))
            return true;
        if (ShieldActive() && hudM.curEnegry > (shieldE.GetComponent<ItemData>().shdWeight * 2))
            return true;

        return false;
    }

    public bool AbilityAttackStaminaConditions()
    {
        if (!PrimaryWeaponActive() && !ShieldActive() && !SecondaryActive() && hudM.curEnegry > (10 * 3))
            return true;
        if (PrimaryWeaponActive() && hudM.curEnegry > (primaryE.GetComponent<ItemData>().wpnWeight * 3))
            return true;
        if (PrimaryWeaponActive() && ShieldActive() && hudM.curEnegry > (primaryE.GetComponent<ItemData>().wpnWeight + primaryE.GetComponent<ItemData>().shdWeight * 3))
            return true;
        if (ShieldActive() && hudM.curEnegry > (shieldE.GetComponent<ItemData>().shdWeight * 3))
            return true;

        return false;
    }

    #endregion

    public bool PrimaryWeaponActive()
    {
        if (primaryE != null && primaryE.activeInHierarchy)
        {
            return true;
        }
        else
            return false;
    }

    public bool PrimaryWeaponHActive()
    {
        if (primaryH != null && primaryH.activeInHierarchy)
        {
            return true;
        }
        else
            return false;
    }

    public bool alteredPrimaryPrimaryActive()
    {
        if (alteredPrimaryE != null && alteredPrimaryE.activeInHierarchy)
        {
            return true;
        }
        else
            return false;
    }

    public bool SecondaryActive()
    {
        if (secondaryE != null && secondaryE.activeInHierarchy)
        {
            return true;
        }
        else
            return false;
    }

    public bool SecondaryHActive()
    {
        if (secondaryH != null && secondaryH.activeInHierarchy)
        {
            return true;
        }
        else
            return false;
    }

    public bool ShieldActive()
    {
        if (shieldE != null && shieldE.activeInHierarchy)
        {
            return true;
        }
        else
            return false;
    }

    public bool ShieldHActive()
    {
        if (shieldH != null && shieldH.activeInHierarchy)
        {
            return true;
        }
        else
            return false;
    }

    public bool ArrowActive()
    {
        if (arrowE != null && arrowE.activeInHierarchy)
        {
            return true;
        }
        else
            return false;
    }

    public bool ArrowHActive()
    {
        if (arrowH != null && arrowH.activeInHierarchy)
        {
            return true;
        }
        else
            return false;
    }

    public bool ArrowOnStringActive()
    {
        if (arrowString != null && arrowString.activeInHierarchy)
        {
            return true;
        }
        else
            return false;
    }

    public void SetActiveQuiverArrows()
    {
        for (int i = 0; i < quiverArrows.Length; i++)
        {
            arrow1 = quiverArrows[0].gameObject;
            arrow2 = quiverArrows[1].gameObject;
            arrow3 = quiverArrows[2].gameObject;
            arrow4 = quiverArrows[3].gameObject;
            quiverArrows[i].gameObject.SetActive(false);

            switch (inventoryM.bowAndArrowInv.counter[inventoryM.bowAndArrowInv.slotArrowEquipped])
            {
                case 1:
                    arrow1.gameObject.SetActive(true);
                    break;
                case 2:
                    arrow1.gameObject.SetActive(true);
                    arrow2.gameObject.SetActive(true);
                    break;
                case 3:
                    arrow1.gameObject.SetActive(true);
                    arrow2.gameObject.SetActive(true);
                    arrow3.gameObject.SetActive(true);
                    break;
                case 4:
                    arrow1.gameObject.SetActive(true);
                    arrow2.gameObject.SetActive(true);
                    arrow3.gameObject.SetActive(true);
                    arrow4.gameObject.SetActive(true);
                    break;
                default:
                    arrow1.gameObject.SetActive(true);
                    arrow2.gameObject.SetActive(true);
                    arrow3.gameObject.SetActive(true);
                    arrow4.gameObject.SetActive(true);
                    break;
            }
        }
    }

    public void SetArrowDamage(ref GameObject itemObj, ref ItemData itemData)
    {
        itemObj.GetComponentInChildren<HitBox>().arrowAtk = itemData.maxBowAtk;
        itemObj.GetComponentInChildren<HitBox>().arrowStun = itemData.maxBowStun;
        itemObj.GetComponentInChildren<HitBox>().arrowHdShot = itemData.maxBowHdShot;

        itemObj.GetComponentInChildren<ItemData>().bowRange.shotAmount = itemData.bowRange.shotAmount;
        itemObj.GetComponentInChildren<ItemData>().bowRange.bowRangeType = itemData.bowRange.bowRangeType;
    }

    public void SetItemData(ref GameObject itemObj, ref ItemData ID)
    {
        itemObj.GetComponentInChildren<ItemData>().itemType = ID.itemType;
        itemObj.GetComponentInChildren<ItemData>().itemSprite = ID.itemSprite;
        itemObj.GetComponentInChildren<ItemData>().originItemName = ID.originItemName;
        itemObj.GetComponentInChildren<ItemData>().itemName = ID.itemName;

        if (cc != null)
        {
            if (itemBeingDropped)
            {
                ID.weaponArmsID = 0;
                cc.preWeaponArmsID = 0;
            }
            else
            {
                itemObj.GetComponentInChildren<ItemData>().weaponArmsID = ID.weaponArmsID;
                cc.preWeaponArmsID = ID.weaponArmsID; 
            }
        }

        if (humanBehavior != null)
        {
            itemObj.GetComponentInChildren<ItemData>().weaponArmsID = ID.weaponArmsID;
        }

        itemObj.GetComponentInChildren<ItemData>().itemDescription = ID.itemDescription;

        itemObj.GetComponentInChildren<ItemData>().wpnAtk.value = ID.wpnAtk.value;
        itemObj.GetComponentInChildren<ItemData>().wpnAtk.rank1 = ID.wpnAtk.rank1;
        itemObj.GetComponentInChildren<ItemData>().wpnAtk.rank2 = ID.wpnAtk.rank2;
        itemObj.GetComponentInChildren<ItemData>().wpnAtk.rank3 = ID.wpnAtk.rank3;
        itemObj.GetComponentInChildren<ItemData>().wpnSpd.value = ID.wpnSpd.value;
        itemObj.GetComponentInChildren<ItemData>().wpnSpd.rank1 = ID.wpnSpd.rank1;
        itemObj.GetComponentInChildren<ItemData>().wpnSpd.rank2 = ID.wpnSpd.rank2;
        itemObj.GetComponentInChildren<ItemData>().wpnSpd.rank3 = ID.wpnSpd.rank3;
        itemObj.GetComponentInChildren<ItemData>().wpnStun.value = ID.wpnStun.value;
        itemObj.GetComponentInChildren<ItemData>().wpnStun.rank1 = ID.wpnStun.rank1;
        itemObj.GetComponentInChildren<ItemData>().wpnStun.rank2 = ID.wpnStun.rank2;
        itemObj.GetComponentInChildren<ItemData>().wpnStun.rank3 = ID.wpnStun.rank3;
        itemObj.GetComponentInChildren<ItemData>().wpnCritHit.value = ID.wpnCritHit.value;
        itemObj.GetComponentInChildren<ItemData>().wpnCritHit.rank1 = ID.wpnCritHit.rank1;
        itemObj.GetComponentInChildren<ItemData>().wpnCritHit.rank2 = ID.wpnCritHit.rank2;
        itemObj.GetComponentInChildren<ItemData>().wpnCritHit.rank3 = ID.wpnCritHit.rank3;
        itemObj.GetComponentInChildren<ItemData>().wpnDura.value = ID.wpnDura.value;
        itemObj.GetComponentInChildren<ItemData>().wpnDura.rank1 = ID.wpnDura.rank1;
        itemObj.GetComponentInChildren<ItemData>().wpnDura.rank2 = ID.wpnDura.rank2;
        itemObj.GetComponentInChildren<ItemData>().wpnDura.rank3 = ID.wpnDura.rank3;
        itemObj.GetComponentInChildren<ItemData>().throwDist.throwRange = ID.throwDist.throwRange;
        itemObj.GetComponentInChildren<ItemData>().wpnWeight = ID.wpnWeight;

        itemObj.GetComponentInChildren<ItemData>().wpnSpecialStat1 = ID.wpnSpecialStat1;
        itemObj.GetComponentInChildren<ItemData>().wpnSpecialStatValue1 = ID.wpnSpecialStatValue1;
        itemObj.GetComponentInChildren<ItemData>().wpnSpecialStat2 = ID.wpnSpecialStat2;
        itemObj.GetComponentInChildren<ItemData>().wpnSpecialStatValue2 = ID.wpnSpecialStatValue2;

        itemObj.GetComponentInChildren<ItemData>().bowAtk.value = ID.bowAtk.value;
        itemObj.GetComponentInChildren<ItemData>().bowAtk.rank1 = ID.bowAtk.rank1;
        itemObj.GetComponentInChildren<ItemData>().bowAtk.rank2 = ID.bowAtk.rank2;
        itemObj.GetComponentInChildren<ItemData>().bowAtk.rank3 = ID.bowAtk.rank3;
        itemObj.GetComponentInChildren<ItemData>().bowSpd.value = ID.bowSpd.value;
        itemObj.GetComponentInChildren<ItemData>().bowSpd.rank1 = ID.bowSpd.rank1;
        itemObj.GetComponentInChildren<ItemData>().bowSpd.rank2 = ID.bowSpd.rank2;
        itemObj.GetComponentInChildren<ItemData>().bowSpd.rank3 = ID.bowSpd.rank3;
        itemObj.GetComponentInChildren<ItemData>().bowStun.value = ID.bowStun.value;
        itemObj.GetComponentInChildren<ItemData>().bowStun.rank1 = ID.bowStun.rank1;
        itemObj.GetComponentInChildren<ItemData>().bowStun.rank2 = ID.bowStun.rank2;
        itemObj.GetComponentInChildren<ItemData>().bowStun.rank3 = ID.bowStun.rank3;
        itemObj.GetComponentInChildren<ItemData>().bowCritHit.value = ID.bowCritHit.value;
        itemObj.GetComponentInChildren<ItemData>().bowCritHit.rank1 = ID.bowCritHit.rank1;
        itemObj.GetComponentInChildren<ItemData>().bowCritHit.rank2 = ID.bowCritHit.rank2;
        itemObj.GetComponentInChildren<ItemData>().bowCritHit.rank3 = ID.bowCritHit.rank3;
        itemObj.GetComponentInChildren<ItemData>().bowHdShot.value = ID.bowHdShot.value;
        itemObj.GetComponentInChildren<ItemData>().bowHdShot.rank1 = ID.bowHdShot.rank1;
        itemObj.GetComponentInChildren<ItemData>().bowHdShot.rank2 = ID.bowHdShot.rank2;
        itemObj.GetComponentInChildren<ItemData>().bowHdShot.rank3 = ID.bowHdShot.rank3;
        itemObj.GetComponentInChildren<ItemData>().bowDura.value = ID.bowDura.value;
        itemObj.GetComponentInChildren<ItemData>().bowDura.rank1 = ID.bowDura.rank1;
        itemObj.GetComponentInChildren<ItemData>().bowDura.rank2 = ID.bowDura.rank2;
        itemObj.GetComponentInChildren<ItemData>().bowDura.rank3 = ID.bowDura.rank3;
        itemObj.GetComponentInChildren<ItemData>().bowRange.shotAmount = ID.bowRange.shotAmount;
        itemObj.GetComponentInChildren<ItemData>().bowRange.bowRangeType = ID.bowRange.bowRangeType;
        itemObj.GetComponentInChildren<ItemData>().bowWeight = ID.bowWeight;

        itemObj.GetComponentInChildren<ItemData>().bowSpecialStat1 = ID.bowSpecialStat1;
        itemObj.GetComponentInChildren<ItemData>().bowSpecialStatValue1 = ID.bowSpecialStatValue1;
        itemObj.GetComponentInChildren<ItemData>().bowSpecialStat2 = ID.bowSpecialStat2;
        itemObj.GetComponentInChildren<ItemData>().bowSpecialStatValue2 = ID.bowSpecialStatValue2;

        itemObj.GetComponentInChildren<ItemData>().shdAtk.value = ID.shdAtk.value;
        itemObj.GetComponentInChildren<ItemData>().shdAtk.rank1 = ID.shdAtk.rank1;
        itemObj.GetComponentInChildren<ItemData>().shdAtk.rank2 = ID.shdAtk.rank2;
        itemObj.GetComponentInChildren<ItemData>().shdAtk.rank3 = ID.shdAtk.rank3;
        itemObj.GetComponentInChildren<ItemData>().shdSpd.value = ID.shdSpd.value;
        itemObj.GetComponentInChildren<ItemData>().shdSpd.rank1 = ID.shdSpd.rank1;
        itemObj.GetComponentInChildren<ItemData>().shdSpd.rank2 = ID.shdSpd.rank2;
        itemObj.GetComponentInChildren<ItemData>().shdSpd.rank3 = ID.shdSpd.rank3;
        itemObj.GetComponentInChildren<ItemData>().shdBlock.value = ID.shdBlock.value;
        itemObj.GetComponentInChildren<ItemData>().shdBlock.rank1 = ID.shdBlock.rank1;
        itemObj.GetComponentInChildren<ItemData>().shdBlock.rank2 = ID.shdBlock.rank2;
        itemObj.GetComponentInChildren<ItemData>().shdBlock.rank3 = ID.shdBlock.rank3;
        itemObj.GetComponentInChildren<ItemData>().shdStun.value = ID.shdStun.value;
        itemObj.GetComponentInChildren<ItemData>().shdStun.rank1 = ID.shdStun.rank1;
        itemObj.GetComponentInChildren<ItemData>().shdStun.rank2 = ID.shdStun.rank2;
        itemObj.GetComponentInChildren<ItemData>().shdStun.rank3 = ID.shdStun.rank3;
        itemObj.GetComponentInChildren<ItemData>().shdCritHit.value = ID.shdCritHit.value;
        itemObj.GetComponentInChildren<ItemData>().shdCritHit.rank1 = ID.shdCritHit.rank1;
        itemObj.GetComponentInChildren<ItemData>().shdCritHit.rank2 = ID.shdCritHit.rank2;
        itemObj.GetComponentInChildren<ItemData>().shdCritHit.rank3 = ID.shdCritHit.rank3;
        itemObj.GetComponentInChildren<ItemData>().shdDura.value = ID.shdDura.value;
        itemObj.GetComponentInChildren<ItemData>().shdDura.rank1 = ID.shdDura.rank1;
        itemObj.GetComponentInChildren<ItemData>().shdDura.rank2 = ID.shdDura.rank2;
        itemObj.GetComponentInChildren<ItemData>().shdDura.rank3 = ID.shdDura.rank3;
        itemObj.GetComponentInChildren<ItemData>().shdWeight = ID.shdWeight;

        itemObj.GetComponentInChildren<ItemData>().shdSpecialStat1 = ID.shdSpecialStat1;
        itemObj.GetComponentInChildren<ItemData>().shdSpecialStatValue1 = ID.shdSpecialStatValue1;
        itemObj.GetComponentInChildren<ItemData>().shdSpecialStat2 = ID.shdSpecialStat2;
        itemObj.GetComponentInChildren<ItemData>().shdSpecialStatValue2 = ID.shdSpecialStatValue2;

        itemObj.GetComponentInChildren<ItemData>().arm.value = ID.arm.value;
        itemObj.GetComponentInChildren<ItemData>().arm.rank1 = ID.arm.rank1;
        itemObj.GetComponentInChildren<ItemData>().arm.rank2 = ID.arm.rank2;
        itemObj.GetComponentInChildren<ItemData>().arm.rank3 = ID.arm.rank3;
        itemObj.GetComponentInChildren<ItemData>().armLRes.value = ID.armLRes.value;
        itemObj.GetComponentInChildren<ItemData>().armLRes.rank1 = ID.armLRes.rank1;
        itemObj.GetComponentInChildren<ItemData>().armLRes.rank2 = ID.armLRes.rank2;
        itemObj.GetComponentInChildren<ItemData>().armLRes.rank3 = ID.armLRes.rank3;
        itemObj.GetComponentInChildren<ItemData>().armHRes.value = ID.armHRes.value;
        itemObj.GetComponentInChildren<ItemData>().armHRes.rank1 = ID.armHRes.rank1;
        itemObj.GetComponentInChildren<ItemData>().armHRes.rank2 = ID.armHRes.rank2;
        itemObj.GetComponentInChildren<ItemData>().armHRes.rank3 = ID.armHRes.rank3;
        itemObj.GetComponentInChildren<ItemData>().arm.armorType = ID.arm.armorType;
        itemObj.GetComponentInChildren<ItemData>().armWeight = ID.armWeight;

        itemObj.GetComponentInChildren<ItemData>().armSpecialStat1 = ID.armSpecialStat1;
        itemObj.GetComponentInChildren<ItemData>().armSpecialStatValue1 = ID.armSpecialStatValue1;
        itemObj.GetComponentInChildren<ItemData>().armSpecialStat2 = ID.armSpecialStat2;
        itemObj.GetComponentInChildren<ItemData>().armSpecialStatValue2 = ID.armSpecialStatValue2;

        itemObj.GetComponentInChildren<ItemData>().rankNum = ID.rankNum;
        itemObj.GetComponentInChildren<ItemData>().upgradeMaterial = ID.upgradeMaterial;

        itemObj.GetComponentInChildren<ItemData>().buyPrice = ID.buyPrice;
        itemObj.GetComponentInChildren<ItemData>().sellPrice = ID.sellPrice;
        itemObj.GetComponentInChildren<ItemData>().repairPrice = ID.repairPrice;
        itemObj.GetComponentInChildren<ItemData>().upgradePrice = ID.upgradePrice;
        itemObj.GetComponentInChildren<ItemData>().inStockQuantity = ID.inStockQuantity;
        itemObj.GetComponentInChildren<ItemData>().quantity = ID.quantity;
        itemObj.GetComponentInChildren<ItemData>().stackable = ID.stackable;

        itemObj.GetComponentInChildren<ItemData>().equipped = true;
        if (itemObj.GetComponentInChildren<HitBox>() != null)
            itemObj.GetComponentInChildren<HitBox>().equipped = true;

        if (itemObj.GetComponent<HealthPoints>()?.curHealthPoints == itemObj.GetComponent<HealthPoints>()?.maxHealthPoints)
        {

        }

        if (cc)
        {
            switch (itemObj.GetComponentInChildren<ItemData>().bowSpecialStat1)
            {
                case ItemData.BowSpecialStat1.QuickShot:
                    if (cc.wpnHolster.SecondaryActive()) cc.bowAnim.speed = 20;
                    break;
                case ItemData.BowSpecialStat1.ThreeShotBurst:
                    itemObj.GetComponentInChildren<ItemData>().bowRange.shotAmount = 3;
                    if (cc.wpnHolster.SecondaryActive()) cc.bowAnim.speed = 1;
                    break;
                case ItemData.BowSpecialStat1.FiveShotBurst:
                    itemObj.GetComponentInChildren<ItemData>().bowRange.shotAmount = 5;
                    if (cc.wpnHolster.SecondaryActive()) cc.bowAnim.speed = 1;
                    break;
            }
            switch (itemObj.GetComponentInChildren<ItemData>().bowSpecialStat2)
            {
                case ItemData.BowSpecialStat2.QuickShot:
                    if (cc.wpnHolster.SecondaryActive()) cc.bowAnim.speed = 20;
                    break;
                case ItemData.BowSpecialStat2.ThreeShotBurst:
                    itemObj.GetComponentInChildren<ItemData>().bowRange.shotAmount = 3;
                    if (cc.wpnHolster.SecondaryActive()) cc.bowAnim.speed = 1;
                    break;
                case ItemData.BowSpecialStat2.FiveShotBurst:
                    itemObj.GetComponentInChildren<ItemData>().bowRange.shotAmount = 5;
                    if (cc.wpnHolster.SecondaryActive()) cc.bowAnim.speed = 1;
                    break;
            }
        }
    }

    public void SetDropItemData(ref ItemData item)
    {
        if (item.itemName == "Windscar" + item.rankTag)
        {
            primaryD = windScarNoParticlePrefab;
            SetItemData(ref primaryD, ref item);
        }

        if (item.itemName == "The Tuning fork" + item.rankTag)
        {
            primaryD = theTuningForkPrefab;
            SetItemData(ref primaryD, ref item);
        }

        if (item.itemName == "Assassin Dagger" + item.rankTag)
        {
            primaryD = assasinsDaggerPrefab;
            SetItemData(ref primaryD, ref item);
        }

        if (item.itemName == "Cleric's Staff" + item.rankTag)
        {
            primaryD = clericsStaffPrefab;
            SetItemData(ref primaryD, ref item);
        }

        if (item.itemName == "Glaive" + item.rankTag)
        {
            primaryD = glaivePrefab;
            SetItemData(ref primaryD, ref item);
        }

        if (item.itemName == "Obsidian Fury" + item.rankTag)
        {
            primaryD = obsidianFuryPrefab;
            SetItemData(ref primaryD, ref item);
        }

        if (item.itemName == "Warbow" + item.rankTag)
        {
            secondaryD = warbowPrefab;
            SetItemData(ref secondaryD, ref item);
        }

        if (item.itemName == "LuRifle" + item.rankTag)
        {
            secondaryD = luRiflePrefab;
            SetItemData(ref secondaryD, ref item);
        }

        if (item.itemName == "7.62mm")
        {
            arrowD = SevenSixTwoAmmoPrefab;
            SetItemData(ref arrowD, ref item);
        }

        if (item.itemName == "Common Arrow")
        {
            arrowD = commonArrowPrefab;
            SetItemData(ref arrowD, ref item);
        }

        if (item.itemName == "Particle Arrow")
        {
            arrowD = particleArrowPrefab;
            SetItemData(ref arrowD, ref item);
        }

        if (item.itemName == "Circle Shield" + item.rankTag)
        {
            shieldD = circleShieldPrefab;
            SetItemData(ref shieldD, ref item);
        }

        if (item.itemName == "Elixir of Health")
        {
            commonItemD = elixirofHealthPrefab;
            SetItemData(ref commonItemD, ref item);
        }

        if (item.itemName == "Elixir of Energy")
        {
            commonItemD = elixirofEnergyPrefab;
            SetItemData(ref commonItemD, ref item);
        }

        if (item.itemName == "Silver Bar")
        {
            commonItemD = silverBarPrefab;
            SetItemData(ref commonItemD, ref item);
        }

        if (item.itemName == "Gold Bar")
        {
            commonItemD = goldBarPrefab;
            SetItemData(ref commonItemD, ref item);
        }

        if (item.itemName == "Ruby")
        {
            commonItemD = rubyPrefab;
            SetItemData(ref commonItemD, ref item);
        }

        if (item.itemName == "Emerald")
        {
            commonItemD = emeraldPrefab;
            SetItemData(ref commonItemD, ref item);
        }
    }

    #region AI 

    public void SetAIWeapon(ItemData weapon)
    {
        if (weapon.itemName == "Windscar" + weapon.rankTag)
        {
            primaryH = windScarNoParticlesH;
            primaryE = windScarNoParticleE;
            SetItemData(ref primaryE, ref weapon);
        }

        if (weapon.itemName == "The Tuning fork" + weapon.rankTag)
        {
            primaryH = theTuningForkH;
            primaryE = theTuningForkE;
            SetItemData(ref primaryE, ref weapon);
        }

        if (weapon.itemName == "Assassin Dagger" + weapon.rankTag)
        {
            primaryH = assasinsDaggerH;
            primaryE = assasinsDaggerE;
            SetItemData(ref primaryE, ref weapon);
        }

        if (weapon.itemName == "Cleric's Staff" + weapon.rankTag)
        {
            primaryH = clericsStaffH;
            primaryE = clericsStaffE;
            SetItemData(ref primaryE, ref weapon);
        }

        if (weapon.itemName == "Glaive" + weapon.rankTag)
        {
            primaryH = glaiveH;
            primaryE = glaiveE;
            SetItemData(ref primaryE, ref weapon);
        }

        if (weapon.itemName == "Obsidian Fury" + weapon.rankTag)
        {
            primaryH = obsidianFuryH;
            primaryE = obsidianFuryE;
            SetItemData(ref primaryE, ref weapon);
        }

        if (weapon.itemName == "Warbow" + weapon.rankTag)
        {
            bowString = warbowString;
            arrowPrefabSpot = warbowPrefabSpot;
            secondaryH = warbowH;
            secondaryE = warbowE;
            SetItemData(ref secondaryE, ref weapon);
        }

        if (weapon.itemName == "Lu Rifle" + weapon.rankTag)
        {
            arrowPrefabSpot = luRiflePrefabSpot;
            secondaryH = luRifleH;
            secondaryE = luRifleE;
            SetItemData(ref secondaryE, ref weapon);
        }

        if (weapon.itemName == "Common Arrow")
        {
            arrowE = commonArrowE;
            arrowH = commonArrowH;
            arrowString = commonArrowString;

            if (bowString)
                bowStrings.transform.SetParent(bowString.transform);

            SetItemData(ref arrowE, ref weapon);
        }

        if (weapon.itemName == "Particle Arrow")
        {
            arrowE = particleArrowE;
            arrowH = particleArrowH;
            arrowString = particleArrowString;

            if (bowString)
                bowStrings.transform.SetParent(bowString.transform);

            SetItemData(ref arrowE, ref weapon);
        }
    }

    #endregion
}
