using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using RPGAE.CharacterController;

public class InteractOverlayUI : MonoBehaviour
{
    public string buttonActionName;

    [Header("AUDIO")]
    public RandomAudioPlayer iconSound;

    [Header("REFERENCES")]
    public FadeUI fadeUI;
    public Image buttonImage;
    public TextMeshProUGUI buttonText;

    ThirdPersonController cc;
    InteractUIManager interactUIM;

    // Start is called before the first frame update
    void Start()
    {
        fadeUI = GetComponent<FadeUI>();
        cc = FindObjectOfType<ThirdPersonController>();
        interactUIM = FindObjectOfType<InteractUIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        SetUIButton();
    }

    #region Set UI button 

    void SetUIButton()
    {
        if (buttonActionName == "Move")
        {
            switch (cc.controllerType)
            {
                case ThirdPersonInput.ControllerType.MOUSEKEYBOARD:
                    buttonImage.sprite = interactUIM.keyboardButtonIcons.wasd;
                    buttonText.text = buttonActionName;
                    break;
                case ThirdPersonInput.ControllerType.XBOX:
                    buttonImage.sprite = interactUIM.xboxButtonIcons.leftStickAxis;
                    buttonText.text = buttonActionName;
                    break;
                case ThirdPersonInput.ControllerType.PLAYSTATION:
                    buttonImage.sprite = interactUIM.playstationButtonIcons.leftStickAxis;
                    buttonText.text = buttonActionName;
                    break;
            }
            FadeOn();
            buttonText.fontSize = 12;
        }
        else if (buttonActionName == "Dive")
        {
            switch (cc.controllerType)
            {
                case ThirdPersonInput.ControllerType.MOUSEKEYBOARD:
                    buttonImage.sprite = interactUIM.keyboardButtonIcons.eButton;
                    buttonText.text = buttonActionName;
                    break;
                case ThirdPersonInput.ControllerType.XBOX:
                    buttonImage.sprite = interactUIM.xboxButtonIcons.xButton;
                    buttonText.text = buttonActionName;
                    break;
                case ThirdPersonInput.ControllerType.PLAYSTATION:
                    buttonImage.sprite = interactUIM.playstationButtonIcons.squareButton;
                    buttonText.text = buttonActionName;
                    break;
            }
            FadeOn();
            buttonText.fontSize = 12;
        }
        else if (buttonActionName == "Rise")
        {
            switch (cc.controllerType)
            {
                case ThirdPersonInput.ControllerType.MOUSEKEYBOARD:
                    buttonImage.sprite = interactUIM.keyboardButtonIcons.eButton;
                    buttonText.text = buttonActionName;
                    break;
                case ThirdPersonInput.ControllerType.XBOX:
                    buttonImage.sprite = interactUIM.xboxButtonIcons.xButton;
                    buttonText.text = buttonActionName;
                    break;
                case ThirdPersonInput.ControllerType.PLAYSTATION:
                    buttonImage.sprite = interactUIM.playstationButtonIcons.squareButton;
                    buttonText.text = buttonActionName;
                    break;
            }
            FadeOn();
            buttonText.fontSize = 12;
        }
        else if (buttonActionName == "Climb")
        {
            switch (cc.controllerType)
            {
                case ThirdPersonInput.ControllerType.MOUSEKEYBOARD:
                    buttonImage.sprite = interactUIM.keyboardButtonIcons.eButton;
                    buttonText.text = buttonActionName;
                    break;
                case ThirdPersonInput.ControllerType.XBOX:
                    buttonImage.sprite = interactUIM.xboxButtonIcons.aButton;
                    buttonText.text = buttonActionName;
                    break;
                case ThirdPersonInput.ControllerType.PLAYSTATION:
                    buttonImage.sprite = interactUIM.playstationButtonIcons.squareButton;
                    buttonText.text = buttonActionName;
                    break;
            }
            FadeOn();
            buttonText.fontSize = 12;
        }
        else if (buttonActionName == "Climb Jump")
        {
            switch (cc.controllerType)
            {
                case ThirdPersonInput.ControllerType.MOUSEKEYBOARD:
                    buttonImage.sprite = interactUIM.keyboardButtonIcons.spaceButton;
                    buttonText.text = buttonActionName;
                    break;
                case ThirdPersonInput.ControllerType.XBOX:
                    buttonImage.sprite = interactUIM.xboxButtonIcons.aButton;
                    buttonText.text = buttonActionName;
                    break;
                case ThirdPersonInput.ControllerType.PLAYSTATION:
                    buttonImage.sprite = interactUIM.playstationButtonIcons.crossButton;
                    buttonText.text = buttonActionName;
                    break;
            }
            FadeOn();
            buttonText.fontSize = 12;
        }
        else if (buttonActionName == "Climb Down")
        {
            switch (cc.controllerType)
            {
                case ThirdPersonInput.ControllerType.MOUSEKEYBOARD:
                    buttonImage.sprite = interactUIM.keyboardButtonIcons.eButton;
                    buttonText.text = buttonActionName;
                    break;
                case ThirdPersonInput.ControllerType.XBOX:
                    buttonImage.sprite = interactUIM.xboxButtonIcons.bButton;
                    buttonText.text = buttonActionName;
                    break;
                case ThirdPersonInput.ControllerType.PLAYSTATION:
                    buttonImage.sprite = interactUIM.playstationButtonIcons.circleButton;
                    buttonText.text = buttonActionName;
                    break;
            }
            FadeOn();
            buttonText.fontSize = 12;
        }
        else if (buttonActionName == "Let Go")
        {
            switch (cc.controllerType)
            {
                case ThirdPersonInput.ControllerType.MOUSEKEYBOARD:
                    buttonImage.sprite = interactUIM.keyboardButtonIcons.eButton;
                    buttonText.text = buttonActionName;
                    break;
                case ThirdPersonInput.ControllerType.XBOX:
                    buttonImage.sprite = interactUIM.xboxButtonIcons.xButton;
                    buttonText.text = buttonActionName;
                    break;
                case ThirdPersonInput.ControllerType.PLAYSTATION:
                    buttonImage.sprite = interactUIM.playstationButtonIcons.circleButton;
                    buttonText.text = buttonActionName;
                    break;
            }
            FadeOn();
            buttonText.fontSize = 12;
        }
        else if (buttonActionName == "Throw")
        {
            switch (cc.controllerType)
            {
                case ThirdPersonInput.ControllerType.MOUSEKEYBOARD:
                    buttonImage.sprite = interactUIM.keyboardButtonIcons.twoButton;
                    buttonText.text = buttonActionName;
                    break;
                case ThirdPersonInput.ControllerType.XBOX:
                    buttonImage.sprite = interactUIM.xboxButtonIcons.DPadRight;
                    buttonText.text = buttonActionName;
                    break;
                case ThirdPersonInput.ControllerType.PLAYSTATION:
                    buttonImage.sprite = interactUIM.playstationButtonIcons.DPadRight;
                    buttonText.text = buttonActionName;
                    break;
            }
            FadeOn();
            buttonText.fontSize = 12;
        }
        else if (buttonActionName == "Projectile")
        {
            switch (cc.controllerType)
            {
                case ThirdPersonInput.ControllerType.MOUSEKEYBOARD:
                    buttonImage.sprite = interactUIM.keyboardButtonIcons.threeButton;
                    buttonText.text = buttonActionName;
                    break;
                case ThirdPersonInput.ControllerType.XBOX:
                    buttonImage.sprite = interactUIM.xboxButtonIcons.DPadDown;
                    buttonText.text = buttonActionName;
                    break;
                case ThirdPersonInput.ControllerType.PLAYSTATION:
                    buttonImage.sprite = interactUIM.playstationButtonIcons.DPadDown;
                    buttonText.text = buttonActionName;
                    break;
            }
            FadeOn();
            buttonText.fontSize = 12;
        }
        else if (buttonActionName == "Back")
        {
            switch (cc.controllerType)
            {
                case ThirdPersonInput.ControllerType.MOUSEKEYBOARD:
                    buttonImage.sprite = interactUIM.keyboardButtonIcons.escapeButton;
                    buttonText.text = buttonActionName;
                    break;
                case ThirdPersonInput.ControllerType.XBOX:
                    buttonImage.sprite = interactUIM.xboxButtonIcons.start;
                    buttonText.text = buttonActionName;
                    break;
                case ThirdPersonInput.ControllerType.PLAYSTATION:
                    buttonImage.sprite = interactUIM.playstationButtonIcons.option;
                    buttonText.text = buttonActionName;
                    break;
            }
            FadeOn();
            buttonText.fontSize = 15;
        }
        else if (buttonActionName == "Set Marker")
        {
            switch (cc.controllerType)
            {
                case ThirdPersonInput.ControllerType.MOUSEKEYBOARD:
                    buttonImage.sprite = interactUIM.keyboardButtonIcons.attackClick;
                    buttonText.text = buttonActionName;
                    break;
                case ThirdPersonInput.ControllerType.XBOX:
                    buttonImage.sprite = interactUIM.xboxButtonIcons.rightTrigger;
                    buttonText.text = buttonActionName;
                    break;
                case ThirdPersonInput.ControllerType.PLAYSTATION:
                    buttonImage.sprite = interactUIM.playstationButtonIcons.R2;
                    buttonText.text = buttonActionName;
                    break;
            }
            FadeOn();
            buttonText.fontSize = 15;
        }
        else if (buttonActionName == "Legend")
        {
            switch (cc.controllerType)
            {
                case ThirdPersonInput.ControllerType.MOUSEKEYBOARD:
                    buttonImage.sprite = interactUIM.keyboardButtonIcons.spaceButton;
                    buttonText.text = buttonActionName;
                    break;
                case ThirdPersonInput.ControllerType.XBOX:
                    buttonImage.sprite = interactUIM.xboxButtonIcons.xButton;
                    buttonText.text = buttonActionName;
                    break;
                case ThirdPersonInput.ControllerType.PLAYSTATION:
                    buttonImage.sprite = interactUIM.playstationButtonIcons.squareButton;
                    buttonText.text = buttonActionName;
                    break;
            }
            FadeOn();
            buttonText.fontSize = 15;
        }
        else if (buttonActionName == "Zoom")
        {
            switch (cc.controllerType)
            {
                case ThirdPersonInput.ControllerType.MOUSEKEYBOARD:
                    buttonImage.sprite = interactUIM.keyboardButtonIcons.mouseWheel;
                    buttonText.text = buttonActionName;
                    break;
                case ThirdPersonInput.ControllerType.XBOX:
                    buttonImage.sprite = interactUIM.xboxButtonIcons.rightStickAxis;
                    buttonText.text = buttonActionName;
                    break;
                case ThirdPersonInput.ControllerType.PLAYSTATION:
                    buttonImage.sprite = interactUIM.playstationButtonIcons.rightStickAxis;
                    buttonText.text = buttonActionName;
                    break;
            }
            FadeOn();
            buttonText.fontSize = 15;
        }
        else if (buttonActionName == "Rotate")
        {
            switch (cc.controllerType)
            {
                case ThirdPersonInput.ControllerType.MOUSEKEYBOARD:
                    buttonImage.sprite = interactUIM.keyboardButtonIcons.aimClick;
                    buttonText.text = buttonActionName;
                    break;
                case ThirdPersonInput.ControllerType.XBOX:
                    buttonImage.sprite = interactUIM.xboxButtonIcons.leftTrigger;
                    buttonText.text = buttonActionName;
                    break;
                case ThirdPersonInput.ControllerType.PLAYSTATION:
                    buttonImage.sprite = interactUIM.playstationButtonIcons.L2;
                    buttonText.text = buttonActionName;
                    break;
            }
            FadeOn();
            buttonText.fontSize = 15;
        }
        else if (buttonActionName == "Select")
        {
            switch (cc.controllerType)
            {
                case ThirdPersonInput.ControllerType.MOUSEKEYBOARD:
                    buttonImage.sprite = interactUIM.keyboardButtonIcons.attackClick;
                    buttonText.text = buttonActionName;
                    break;
                case ThirdPersonInput.ControllerType.XBOX:
                    buttonImage.sprite = interactUIM.xboxButtonIcons.aButton;
                    buttonText.text = buttonActionName;
                    break;
                case ThirdPersonInput.ControllerType.PLAYSTATION:
                    buttonImage.sprite = interactUIM.playstationButtonIcons.crossButton;
                    buttonText.text = buttonActionName;
                    break;
            }
            FadeOn();
            buttonText.fontSize = 15;
        }
        else if (buttonActionName == "Reset")
        {
            switch (cc.controllerType)
            {
                case ThirdPersonInput.ControllerType.MOUSEKEYBOARD:
                    buttonImage.sprite = interactUIM.keyboardButtonIcons.mouseWheel;
                    buttonText.text = buttonActionName;
                    break;
                case ThirdPersonInput.ControllerType.XBOX:
                    buttonImage.sprite = interactUIM.xboxButtonIcons.leftStickButton;
                    buttonText.text = buttonActionName;
                    break;
                case ThirdPersonInput.ControllerType.PLAYSTATION:
                    buttonImage.sprite = interactUIM.playstationButtonIcons.leftStickButton;
                    buttonText.text = buttonActionName;
                    break;
            }
            FadeOn();
            buttonText.fontSize = 15;
        }
        else if (buttonActionName == "Adrenaline Rush")
        {
            switch (cc.controllerType)
            {
                case ThirdPersonInput.ControllerType.MOUSEKEYBOARD:
                    buttonImage.sprite = interactUIM.keyboardButtonIcons.attackClick;
                    buttonText.text = buttonActionName;
                    break;
                case ThirdPersonInput.ControllerType.XBOX:
                    buttonImage.sprite = interactUIM.xboxButtonIcons.rightTrigger;
                    buttonText.text = buttonActionName;
                    break;
                case ThirdPersonInput.ControllerType.PLAYSTATION:
                    buttonImage.sprite = interactUIM.playstationButtonIcons.R2;
                    buttonText.text = buttonActionName;
                    break;
            }
            fadeUI.canvasGroup.alpha += 0.025f;
            buttonText.fontSize = 15;
        }
        else if (buttonActionName == "Navigate Left")
        {
            switch (cc.controllerType)
            {
                case ThirdPersonInput.ControllerType.MOUSEKEYBOARD:
                    buttonImage.sprite = interactUIM.keyboardButtonIcons.oneButton;
                    buttonText.text = buttonActionName;
                    break;
                case ThirdPersonInput.ControllerType.XBOX:
                    buttonImage.sprite = interactUIM.xboxButtonIcons.leftBumber;
                    buttonText.text = buttonActionName;
                    break;
                case ThirdPersonInput.ControllerType.PLAYSTATION:
                    buttonImage.sprite = interactUIM.playstationButtonIcons.L1;
                    buttonText.text = buttonActionName;
                    break;
            }
            FadeOn();
        }
        else if (buttonActionName == "Navigate Right")
        {
            switch (cc.controllerType)
            {
                case ThirdPersonInput.ControllerType.MOUSEKEYBOARD:
                    buttonImage.sprite = interactUIM.keyboardButtonIcons.twoButton;
                    buttonText.text = buttonActionName;
                    break;
                case ThirdPersonInput.ControllerType.XBOX:
                    buttonImage.sprite = interactUIM.xboxButtonIcons.rightBumber;
                    buttonText.text = buttonActionName;
                    break;
                case ThirdPersonInput.ControllerType.PLAYSTATION:
                    buttonImage.sprite = interactUIM.playstationButtonIcons.R1;
                    buttonText.text = buttonActionName;
                    break;
            }
            FadeOn();
        }
        else if(buttonActionName == "")
        {
            if (fadeUI.canvasGroup.alpha == 1)
                fadeUI.FadeTransition(0, 0, 0.5f);
        }

        if (buttonText.text != buttonActionName)
        {
            if (fadeUI.canvasGroup.alpha == 1)
                fadeUI.FadeTransition(0, 0, 0.5f);
        }
    }


    void FadeOn()
    {
        if (fadeUI.canvasGroup.alpha == 0)
        {
            if (iconSound != null && !iconSound.playing)
            {
                iconSound.PlayRandomClip();
            }
            fadeUI.FadeTransition(1, 0, 0.5f);
        }
    }
}

#endregion
