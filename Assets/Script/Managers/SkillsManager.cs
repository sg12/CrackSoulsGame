using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using RPGAE.CharacterController;
using UnityEngine;

public class SkillsManager : MonoBehaviour
{
    [Header("SKILL TREE MENU")]
    public string topAbilityNameMenu;
    public Image topAbilityIconMenu;
    public Image topButtonIconMenu;
    [Space(5)]
    public string leftAbilityNameMenu;
    public Image leftAbilityIconMenu;
    public Image leftButtonIconMenu;
    [Space(5)]
    public string rightAbilityNameMenu;
    public Image rightAbilityIconMenu;
    public Image rightButtonIconMenu;
    [Space(5)]
    public string bottomAbilityNameMenu;
    public Image bottomAbilityIconMenu;
    public Image bottomButtonIconMenu;

    private Vector2 desiredTopPos;
    private Vector2 desiredBottomPos;
    private Vector2 desiredRightPos;
    private Vector2 desiredLeftPos;
    private Vector2 oldTopPos;
    private Vector2 oldBottomPos;
    private Vector2 oldRightPos;
    private Vector2 oldLeftPos;

    [Header("ABILITY HUD")]
    public FadeUI topHighlight;
    public RectTransform topAbilityHUD;
    public Image topAbilityIconHUD;
    public Image topButtonIconHUD;
    [HideInInspector] public string topAbilityNameHUD;
    [Space(5)]
    public FadeUI leftHighlight;
    public RectTransform leftAbilityHUD;
    public Image leftAbilityIconHUD;
    public Image leftButtonIconHUD;
    [HideInInspector] public string leftAbilityNameHUD;
    [Space(5)]
    public FadeUI rightHighlight;
    public RectTransform rightAbilityHUD;
    public Image rightAbilityIconHUD;
    public Image rightButtonIconHUD;
    [HideInInspector] public string rightAbilityNameHUD;
    [Space(5)]
    public FadeUI bottomHighlight;
    public RectTransform bottomAbilityHUD;
    public Image bottomAbilityIconHUD;
    public Image bottomButtonIconHUD;
    [HideInInspector] public string bottomAbilityNameHUD;

    public CanvasGroup[] buttonIconAlphaHUD;

    [Space(10)]
    public string[] listOfValidAbilityNames;
    public Image triggerButtonIconHUD;
    private int zoomState;

    [Space(10)]
    public RectTransform skillTreeRect;
    public float horizontalSpeed = 400;
    public float verticalSpeed = 400;
    public Vector2 MaxOffSetPosition = new Vector2(1000, 1000);

    [Header("ABILITY TREE ICONS")]
    public Sprite launchAbilityIcon;

    private ThirdPersonController cc;
    private InventoryManager inventoryM;
    private InteractUIManager interactUIM;

    // Start is called before the first frame update
    void Start()
    {
        inventoryM = FindObjectOfType<InventoryManager>();
        cc = FindObjectOfType<ThirdPersonController>();
        interactUIM = FindObjectOfType<InteractUIManager>();

        SetLerpPosition();
    }

    // Update is called once per frame
    void Update()
    {
        //HighLight();
        //AbilityIconHUD();
        //UpdateAbility();
        //PerformAbility();
        //SkillTreeMenuUpdate();
        //ControllerButtonType();
    }

    void SetLerpPosition()
    {
        oldTopPos = new Vector2(0, 25);
        oldBottomPos = new Vector2(0, -175f);
        oldRightPos = new Vector2(95f, -71.6f);
        oldLeftPos = new Vector2(-95f, -71.6f);

        desiredTopPos = new Vector2(0, 55);
        desiredBottomPos = new Vector2(0, -203);
        desiredRightPos = new Vector2(116.1f, -71.6f);
        desiredLeftPos = new Vector2(-116.1f, -71.6f);
    }

    void UpdateAbility()
    {
        if (topAbilityNameMenu == "Launch")
        {
            topAbilityNameHUD = topAbilityNameMenu;
            topAbilityIconHUD.sprite = launchAbilityIcon;
            topAbilityIconMenu.sprite = launchAbilityIcon;
            topAbilityIconHUD.enabled = true;
            topAbilityIconMenu.enabled = true;
        }
        else if (topAbilityNameMenu == "")
        {
            topAbilityNameHUD = "";
            topAbilityIconHUD.enabled = false;
            topAbilityIconMenu.enabled = false;
        }

        if (leftAbilityNameMenu == "Launch")
        {
            leftAbilityNameHUD = leftAbilityNameMenu;
            leftAbilityIconHUD.sprite = launchAbilityIcon;
            leftAbilityIconMenu.sprite = launchAbilityIcon;
            leftAbilityIconHUD.enabled = true;
            leftAbilityIconMenu.enabled = true;
        }
        else if (leftAbilityNameMenu == "")
        {
            leftAbilityNameHUD = "";
            leftAbilityIconHUD.enabled = false;
            leftAbilityIconMenu.enabled = false;
        }

        if (rightAbilityNameMenu == "Launch")
        {
            rightAbilityNameHUD = rightAbilityNameMenu;
            rightAbilityIconHUD.sprite = launchAbilityIcon;
            rightAbilityIconMenu.sprite = launchAbilityIcon;
            rightAbilityIconHUD.enabled = true;
            rightAbilityIconMenu.enabled = true;
        }
        else if (rightAbilityNameMenu == "")
        {
            rightAbilityNameHUD = "";
            rightAbilityIconHUD.enabled = false;
            rightAbilityIconMenu.enabled = false;
        }

        if (bottomAbilityNameMenu == "Launch")
        {
            bottomAbilityNameHUD = bottomAbilityNameMenu;
            bottomAbilityIconHUD.sprite = launchAbilityIcon;
            bottomAbilityIconMenu.sprite = launchAbilityIcon;
            bottomAbilityIconHUD.enabled = true;
            bottomAbilityIconMenu.enabled = true;
        }
        else if (bottomAbilityNameMenu == "")
        {
            bottomAbilityNameHUD = "";
            bottomAbilityIconHUD.enabled = false;
            bottomAbilityIconMenu.enabled = false;
        }
    }

    void PerformAbility()
    {
        // Top Ability 
        if (cc.isStrafing && cc.rpgaeIM.PlayerControls.DPadUp.triggered && cc.wpnHolster.AbilityAttackStaminaConditions())
        {
            bool conditonsToUseAbility = !cc.animator.IsInTransition(5) && !cc.isAttacking;
            if (topAbilityNameHUD == "Launch" && cc.wpnHolster.PrimaryWeaponActive() && conditonsToUseAbility)
            {
                topHighlight.canvasGroup.alpha = 1;
                cc.animator.SetFloat("AbilityAttackID", 1);
                cc.animator.SetTrigger("Ability Attack");
            }
        }
        // Left Ability 
        if (cc.isStrafing && cc.rpgaeIM.PlayerControls.DPadLeft.triggered && cc.wpnHolster.AbilityAttackStaminaConditions())
        {
            bool conditonsToUseAbility = !cc.animator.IsInTransition(5) && !cc.isAttacking;
            if (leftAbilityNameHUD == "Launch" && cc.wpnHolster.PrimaryWeaponActive() && conditonsToUseAbility)
            {
                leftHighlight.canvasGroup.alpha = 1;
                cc.animator.SetFloat("AbilityAttackID", 1);
                cc.animator.SetTrigger("Ability Attack");
            }
        }
        // Right Ability 
        if (cc.isStrafing && cc.rpgaeIM.PlayerControls.DPadRight.triggered && cc.wpnHolster.AbilityAttackStaminaConditions())
        {
            bool conditonsToUseAbility = !cc.animator.IsInTransition(5) && !cc.isAttacking;
            if (rightAbilityNameHUD == "Launch" && cc.wpnHolster.PrimaryWeaponActive() && conditonsToUseAbility)
            {
                rightHighlight.canvasGroup.alpha = 1;
                cc.animator.SetFloat("AbilityAttackID", 1);
                cc.animator.SetTrigger("Ability Attack");
            }
        }
        // Down Ability 
        if (cc.isStrafing && cc.rpgaeIM.PlayerControls.DPadDown.triggered && cc.wpnHolster.AbilityAttackStaminaConditions())
        {
            bool conditonsToUseAbility = !cc.animator.IsInTransition(5) && !cc.isAttacking;
            if (bottomAbilityNameHUD == "Launch" && cc.wpnHolster.PrimaryWeaponActive() && conditonsToUseAbility)
            {
                bottomHighlight.canvasGroup.alpha = 1;
                cc.animator.SetFloat("AbilityAttackID", 1);
                cc.animator.SetTrigger("Ability Attack");
            }
        }
    }

    void HighLight()
    {
        topHighlight.GetComponent<RectTransform>().anchoredPosition = topAbilityHUD.anchoredPosition;
        leftHighlight.GetComponent<RectTransform>().anchoredPosition = leftAbilityHUD.anchoredPosition;
        rightHighlight.GetComponent<RectTransform>().anchoredPosition = rightAbilityHUD.anchoredPosition;
        bottomHighlight.GetComponent<RectTransform>().anchoredPosition = bottomAbilityHUD.anchoredPosition;

        if (topHighlight.canvasGroup.alpha == 1)
            topHighlight.FadeTransition(0, 0, 1);
        if (leftHighlight.canvasGroup.alpha == 1)
            leftHighlight.FadeTransition(0, 0, 1);
        if (rightHighlight.canvasGroup.alpha == 1)
            rightHighlight.FadeTransition(0, 0, 1);
        if (bottomHighlight.canvasGroup.alpha == 1)
            bottomHighlight.FadeTransition(0, 0, 1);
    }

    #region ControllerButtonType

    void ControllerButtonType()
    {
        switch (cc.controllerType)
        {
            case ThirdPersonInput.ControllerType.MOUSEKEYBOARD:
                // Trigger
                triggerButtonIconHUD.sprite = interactUIM.keyboardButtonIcons.aimClick;
                // Menu
                topButtonIconMenu.sprite = interactUIM.keyboardButtonIcons.zButton;
                leftButtonIconMenu.sprite = interactUIM.keyboardButtonIcons.xButton;
                rightButtonIconMenu.sprite = interactUIM.keyboardButtonIcons.cButton;
                bottomButtonIconMenu.sprite = interactUIM.keyboardButtonIcons.vButton;
                // HUD
                topButtonIconHUD.sprite = interactUIM.keyboardButtonIcons.zButton;
                leftButtonIconHUD.sprite = interactUIM.keyboardButtonIcons.xButton;
                rightButtonIconHUD.sprite = interactUIM.keyboardButtonIcons.cButton;
                bottomButtonIconHUD.sprite = interactUIM.keyboardButtonIcons.vButton;
                break;
            case ThirdPersonInput.ControllerType.XBOX:
                // Trigger
                triggerButtonIconHUD.sprite = interactUIM.xboxButtonIcons.leftTrigger;
                // Menu
                topButtonIconMenu.sprite = interactUIM.xboxButtonIcons.DPadUp;
                leftButtonIconMenu.sprite = interactUIM.xboxButtonIcons.DPadLeft;
                rightButtonIconMenu.sprite = interactUIM.xboxButtonIcons.DPadRight;
                bottomButtonIconMenu.sprite = interactUIM.xboxButtonIcons.DPadDown;
                // HUD
                topButtonIconHUD.sprite = interactUIM.xboxButtonIcons.DPadUp;
                leftButtonIconHUD.sprite = interactUIM.xboxButtonIcons.DPadLeft;
                rightButtonIconHUD.sprite = interactUIM.xboxButtonIcons.DPadRight;
                bottomButtonIconHUD.sprite = interactUIM.xboxButtonIcons.DPadDown;
                break;
            case ThirdPersonInput.ControllerType.PLAYSTATION:
                // Trigger
                triggerButtonIconHUD.sprite = interactUIM.playstationButtonIcons.L2;
                // Menu
                topButtonIconMenu.sprite = interactUIM.playstationButtonIcons.DPadUp;
                leftButtonIconMenu.sprite = interactUIM.playstationButtonIcons.DPadLeft;
                rightButtonIconMenu.sprite = interactUIM.playstationButtonIcons.DPadRight;
                bottomButtonIconMenu.sprite = interactUIM.playstationButtonIcons.DPadDown;
                // HUD
                topButtonIconHUD.sprite = interactUIM.playstationButtonIcons.DPadUp;
                leftButtonIconHUD.sprite = interactUIM.playstationButtonIcons.DPadLeft;
                rightButtonIconHUD.sprite = interactUIM.playstationButtonIcons.DPadRight;
                bottomButtonIconHUD.sprite = interactUIM.playstationButtonIcons.DPadDown;
                break;
        }
    }

    #endregion

    #region Skill Tree Update 

    void SkillTreeMenuUpdate()
    {
        if (!inventoryM.referencesObj.skillsSection.activeInHierarchy)
            return;

        // Zoom
        if (cc.zoomValue > 0 && zoomState < 3)
            zoomState++;
        if (cc.zoomValue < 0 && zoomState > 1)
            zoomState--;

        switch (zoomState)
        {
            case 3:
                Vector2 newSize3 = new Vector2(3, 3);
                skillTreeRect.localScale = Vector2.Lerp(skillTreeRect.localScale, newSize3, 8 * Time.deltaTime);
                break;
            case 2:
                Vector2 newSize2 = new Vector2(2, 2);
                skillTreeRect.localScale = Vector2.Lerp(skillTreeRect.localScale, newSize2, 8 * Time.deltaTime);
                break;
            case 1:
                Vector2 newSize1 = new Vector2(1, 1);
                skillTreeRect.localScale = Vector2.Lerp(skillTreeRect.localScale, newSize1, 8 * Time.deltaTime);
                break;
        }

        // Movement
        Vector2 inputMovement = new Vector2(cc.rpgaeIM.PlayerControls.Movement.ReadValue<Vector2>().x, cc.rpgaeIM.PlayerControls.Movement.ReadValue<Vector2>().y);
        inputMovement.Normalize();
        skillTreeRect.anchoredPosition += new Vector2(-inputMovement.x * horizontalSpeed * Time.deltaTime, -inputMovement.y * verticalSpeed * Time.deltaTime);

        // Clamp
        skillTreeRect.anchoredPosition = new Vector3(Mathf.Clamp(skillTreeRect.anchoredPosition.x, -MaxOffSetPosition.x, MaxOffSetPosition.x),
        Mathf.Clamp(skillTreeRect.anchoredPosition.y, -MaxOffSetPosition.y, MaxOffSetPosition.y), 0);
    }

    #endregion

    #region Ability Icon HUD

    void AbilityIconHUD()
    {
        if (cc.isStrafing)
        {
            Vector2 newSize = new Vector2(1.15f, 1.15f);

            topAbilityHUD.localScale = Vector2.Lerp(topAbilityHUD.localScale, newSize, 8 * Time.deltaTime);
            topAbilityHUD.anchoredPosition = Vector2.Lerp(topAbilityHUD.anchoredPosition, desiredTopPos, 8 * Time.deltaTime);

            bottomAbilityHUD.localScale = Vector2.Lerp(bottomAbilityHUD.localScale, newSize, 8 * Time.deltaTime);
            bottomAbilityHUD.anchoredPosition = Vector2.Lerp(bottomAbilityHUD.anchoredPosition, desiredBottomPos, 8 * Time.deltaTime);

            rightAbilityHUD.localScale = Vector2.Lerp(rightAbilityHUD.localScale, newSize, 8 * Time.deltaTime);
            rightAbilityHUD.anchoredPosition = Vector2.Lerp(rightAbilityHUD.anchoredPosition, desiredRightPos, 8 * Time.deltaTime);

            leftAbilityHUD.localScale = Vector2.Lerp(leftAbilityHUD.localScale, newSize, 8 * Time.deltaTime);
            leftAbilityHUD.anchoredPosition = Vector2.Lerp(leftAbilityHUD.anchoredPosition, desiredLeftPos, 8 * Time.deltaTime);

            foreach (CanvasGroup button in buttonIconAlphaHUD)
            {
                if (button.alpha != 0.9f)
                    button.alpha = Mathf.Lerp(button.alpha, 0.9f, 12 * Time.deltaTime);
            }
        }
        else
        {
            Vector2 oldize = new Vector2(1, 1);

            topAbilityHUD.localScale = Vector2.Lerp(topAbilityHUD.localScale, oldize, 8 * Time.deltaTime);
            topAbilityHUD.anchoredPosition = Vector2.Lerp(topAbilityHUD.anchoredPosition, oldTopPos, 8 * Time.deltaTime);

            bottomAbilityHUD.localScale = Vector2.Lerp(bottomAbilityHUD.localScale, oldize, 8 * Time.deltaTime);
            bottomAbilityHUD.anchoredPosition = Vector2.Lerp(bottomAbilityHUD.anchoredPosition, oldBottomPos, 8 * Time.deltaTime);

            rightAbilityHUD.localScale = Vector2.Lerp(rightAbilityHUD.localScale, oldize, 8 * Time.deltaTime);
            rightAbilityHUD.anchoredPosition = Vector2.Lerp(rightAbilityHUD.anchoredPosition, oldRightPos, 8 * Time.deltaTime);

            leftAbilityHUD.localScale = Vector2.Lerp(leftAbilityHUD.localScale, oldize, 8 * Time.deltaTime);
            leftAbilityHUD.anchoredPosition = Vector2.Lerp(leftAbilityHUD.anchoredPosition, oldLeftPos, 8 * Time.deltaTime);

            foreach (CanvasGroup button in buttonIconAlphaHUD)
            {
                if (button.alpha != 0)
                    button.alpha = Mathf.Lerp(button.alpha, 0, 12 * Time.deltaTime);
            }
        }
    }

    #endregion

}
