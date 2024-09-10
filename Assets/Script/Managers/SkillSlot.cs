using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using RPGAE.CharacterController;

public class SkillSlot : MonoBehaviour
{
    [Header("SKILL SETTINGS")]
    public string skillName;
    public VideoClip videoClip;
    [TextArea(3, 10)]
    public string skillDescription;

    [Header("AUDIO")]
    public RandomAudioPlayer skillPurchasedAS;
    public RandomAudioPlayer skillCancelAS;
    public RandomAudioPlayer menuMoveAS;

    [Header("REFERENCES")]
    public Slider line;
    public SkillSlot requirdPurchase;
    public Image m_fillImage;
    public float m_waitTimeToHold = 0;
    public FadeUI slotInfoFadeUI;
    public RawImage previewVideoUI;
    public TextMeshProUGUI previewSkillName;
    public TextMeshProUGUI previewDescription;
    public FadeUI abilityAssignMenu;

    private float m_waitTime;
    private float m_progressWait = 1f;
    private bool m_onCollision;
    private bool m_thisSkillSlot;
    public bool m_isPurchased;
    private Color m_skillColor;
    private GameObject m_previewVideoUI;

    [HideInInspector]
    public string purchasedSkillName;

    private SkillsManager skillM;
    private UICursorVirtualMouseInput cursorVirtual;
    private ThirdPersonController cc;
    private HUDManager hudM;


    // Start is called before the first frame update
    void Start()
    {
        skillM = FindObjectOfType<SkillsManager>();
        hudM = FindObjectOfType<HUDManager>();
        cc = FindObjectOfType<ThirdPersonController>();
        cursorVirtual = FindObjectOfType<UICursorVirtualMouseInput>();

        m_skillColor = m_fillImage.color;
        m_previewVideoUI = previewVideoUI.GetComponentInParent<VideoPlayer>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        MarkAsPurchased();
        AssignAbility(m_onCollision);
        SkillPreviewToggle(m_onCollision);
        line.value = m_fillImage.fillAmount;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == "SkillCursor")
        {
            if (menuMoveAS)
                menuMoveAS.PlayRandomClip();

            m_onCollision = true;
            other.GetComponent<Animator>().SetBool("Hover", true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.name == "SkillCursor")
        {
            m_onCollision = false;
            other.GetComponent<Animator>().SetBool("Hover", false);
        }
    }

    void SkillPreviewToggle(bool active)
    {
        if (active)
        {
            m_thisSkillSlot = true;

            if (videoClip == null)
                m_previewVideoUI.gameObject.SetActive(false);
            else
                m_previewVideoUI.gameObject.SetActive(true);

            previewSkillName.text = skillName;
            previewDescription.text = skillDescription;

            if (slotInfoFadeUI.canvasGroup.alpha != 1)
                slotInfoFadeUI.FadeTransition(1, 0, 0.3f);
            if (slotInfoFadeUI.canvasGroup.alpha == 1 && videoClip)
            {
                m_previewVideoUI.GetComponent<VideoPlayer>().clip = videoClip;
                m_previewVideoUI.GetComponent<VideoPlayer>().Play();
            }
            PurchaseSkill();
        }
        else
        {
            if (m_thisSkillSlot)
            {
                m_previewVideoUI.GetComponent<VideoPlayer>().clip = null;
                slotInfoFadeUI.FadeTransition(0, 0, 0.3f);
                m_thisSkillSlot = false;
            }
            if (m_fillImage.fillAmount > 0f && !m_isPurchased)
                m_fillImage.fillAmount = 0f;
        }
    }

    void PurchaseSkill()
    {
        if(hudM.curSkillPoint < 1 && cc.cursorButton && !m_isPurchased)
        {
            cc.infoMessage.info.text = "Skill points are required to purchase this.";
            return;
        }

        if (cc.cursorButton && m_thisSkillSlot)
        {
            if (Time.time > m_waitTime && !m_isPurchased)
            {
                if (requirdPurchase)
                {
                    if (requirdPurchase.m_isPurchased)
                    {
                        m_fillImage.fillAmount += 1.0f / m_progressWait * Time.deltaTime;
                        if (m_fillImage.fillAmount >= 1f)
                        {
                            hudM.curSkillPoint--;
                            if (skillPurchasedAS)
                                skillPurchasedAS.PlayRandomClip();

                            m_isPurchased = true;
                        }
                    }
                    else
                        cc.infoMessage.info.text = "You need to purchase a skill linked to this.";
                }
                else
                {
                    m_fillImage.fillAmount += 1.0f / m_progressWait * Time.deltaTime;
                    if (m_fillImage.fillAmount >= 1f)
                    {
                        hudM.curSkillPoint--;
                        if (skillPurchasedAS)
                            skillPurchasedAS.PlayRandomClip();

                        m_isPurchased = true;
                    }
                }
            }
        }
        else
        {
            if (m_fillImage.fillAmount > 0f && !m_isPurchased)
                m_fillImage.fillAmount = 0f;
        }
    }

    void MarkAsPurchased()
    {
        if (m_isPurchased)
        {
            m_fillImage.fillAmount = 1;
            purchasedSkillName = skillName;
        }
    }

    void AssignAbility(bool active)
    {
        foreach (string _name in skillM.listOfValidAbilityNames)
        {
            if (skillName != _name)
                return;
        }

        if (active)
        {
            if (abilityAssignMenu.canvasGroup.alpha != 1)
                abilityAssignMenu.FadeTransition(1, 0, 0.3f);
        }
        else
        {
            if (abilityAssignMenu.canvasGroup.alpha == 1)
                abilityAssignMenu.FadeTransition(0, 0, 0.3f);

            return;
        }

        if (!m_isPurchased) return;

        if (cc.rpgaeIM.PlayerControls.DPadUp.triggered)
        {
            skillM.topAbilityNameMenu = skillName;
        }

        if (cc.rpgaeIM.PlayerControls.DPadLeft.triggered)
        {
            skillM.leftAbilityNameMenu = skillName;
        }

        if (cc.rpgaeIM.PlayerControls.DPadRight.triggered)
        {
            skillM.rightAbilityNameMenu = skillName;
        }

        if (cc.rpgaeIM.PlayerControls.DPadDown.triggered)
        {
            skillM.bottomAbilityNameMenu = skillName;
        }
    }
}