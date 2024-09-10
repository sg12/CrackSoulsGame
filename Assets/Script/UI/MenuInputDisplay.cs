using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using RPGAE.CharacterController;
using System;
using TMPro;

public class MenuInputDisplay : MonoBehaviour
{
    public string buttonActionName;

    public TextMeshProUGUI buttonText;
    public Image buttonImage1;
    public Image buttonImage2;

    [HideInInspector]
    public Transform itemCenterParent;
    private ThirdPersonController cc;
    private InteractUIManager interactUIM;

    // Start is called before the first frame update
    void Start()
    {
        cc = FindObjectOfType<ThirdPersonController>();
        interactUIM = FindObjectOfType<InteractUIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (buttonActionName == "Navigate Left")
        {
            switch (cc.controllerType)
            {
                case ThirdPersonInput.ControllerType.MOUSEKEYBOARD:
                    buttonImage1.sprite = interactUIM.keyboardButtonIcons.oneButton;
                    break;
                case ThirdPersonInput.ControllerType.XBOX:
                    buttonImage1.sprite = interactUIM.xboxButtonIcons.leftBumber;
                    break;
                case ThirdPersonInput.ControllerType.PLAYSTATION:
                    buttonImage1.sprite = interactUIM.playstationButtonIcons.L1;
                    break;
            }
        }
        else if (buttonActionName == "Navigate Right")
        {
            switch (cc.controllerType)
            {
                case ThirdPersonInput.ControllerType.MOUSEKEYBOARD:
                    buttonImage1.sprite = interactUIM.keyboardButtonIcons.twoButton;
                    break;
                case ThirdPersonInput.ControllerType.XBOX:
                    buttonImage1.sprite = interactUIM.xboxButtonIcons.rightBumber;
                    break;
                case ThirdPersonInput.ControllerType.PLAYSTATION:
                    buttonImage1.sprite = interactUIM.playstationButtonIcons.R1;
                    break;
            }
        }
        if (buttonActionName == "Back")
        {
            switch (cc.controllerType)
            {
                case ThirdPersonInput.ControllerType.MOUSEKEYBOARD:
                    buttonImage1.sprite = interactUIM.keyboardButtonIcons.escapeButton;
                    break;
                case ThirdPersonInput.ControllerType.XBOX:
                    buttonImage1.sprite = interactUIM.xboxButtonIcons.start;
                    break;
                case ThirdPersonInput.ControllerType.PLAYSTATION:
                    buttonImage1.sprite = interactUIM.playstationButtonIcons.option;
                    break;
            }
            if (buttonText != null)
            {
                buttonText.text = buttonActionName;
                buttonText.fontSize = 15;
            }
        }
        else if (buttonActionName == "Set Marker")
        {
            switch (cc.controllerType)
            {
                case ThirdPersonInput.ControllerType.MOUSEKEYBOARD:
                    buttonImage1.sprite = interactUIM.keyboardButtonIcons.attackClick;
                    break;
                case ThirdPersonInput.ControllerType.XBOX:
                    buttonImage1.sprite = interactUIM.xboxButtonIcons.rightTrigger;
                    break;
                case ThirdPersonInput.ControllerType.PLAYSTATION:
                    buttonImage1.sprite = interactUIM.playstationButtonIcons.R2;
                    break;
            }
            if (buttonText != null)
            {
                buttonText.text = buttonActionName;
                buttonText.fontSize = 15;
            }
        }
        else if (buttonActionName == "Legend")
        {
            switch (cc.controllerType)
            {
                case ThirdPersonInput.ControllerType.MOUSEKEYBOARD:
                    buttonImage1.sprite = interactUIM.keyboardButtonIcons.spaceButton;
                    break;
                case ThirdPersonInput.ControllerType.XBOX:
                    buttonImage1.sprite = interactUIM.xboxButtonIcons.aButton;
                    break;
                case ThirdPersonInput.ControllerType.PLAYSTATION:
                    buttonImage1.sprite = interactUIM.playstationButtonIcons.squareButton;
                    break;
            }
            if (buttonText != null)
            {
                buttonText.text = buttonActionName;
                buttonText.fontSize = 15;
            }
        }
        else if (buttonActionName == "Zoom")
        {
            switch (cc.controllerType)
            {
                case ThirdPersonInput.ControllerType.MOUSEKEYBOARD:
                    buttonImage1.sprite = interactUIM.keyboardButtonIcons.mouseWheel;
                    break;
                case ThirdPersonInput.ControllerType.XBOX:
                    buttonImage1.sprite = interactUIM.xboxButtonIcons.rightStickAxis;
                    break;
                case ThirdPersonInput.ControllerType.PLAYSTATION:
                    buttonImage1.sprite = interactUIM.playstationButtonIcons.rightStickAxis;
                    break;
            }
            if (buttonText != null)
            {
                buttonText.text = buttonActionName;
                buttonText.fontSize = 15;
            }
        }
        else if (buttonActionName == "Rotate")
        {
            switch (cc.controllerType)
            {
                case ThirdPersonInput.ControllerType.MOUSEKEYBOARD:
                    buttonImage1.sprite = interactUIM.keyboardButtonIcons.aimClick;
                    buttonImage2.sprite = interactUIM.keyboardButtonIcons.mouseMove;
                    break;
                case ThirdPersonInput.ControllerType.XBOX:
                    buttonImage1.sprite = interactUIM.xboxButtonIcons.leftTrigger;
                    buttonImage2.sprite = interactUIM.xboxButtonIcons.rightStickAxis;
                    break;
                case ThirdPersonInput.ControllerType.PLAYSTATION:
                    buttonImage1.sprite = interactUIM.playstationButtonIcons.L2;
                    buttonImage2.sprite = interactUIM.playstationButtonIcons.rightStickAxis;
                    break;
            }
            if (buttonText != null)
            {
                buttonText.text = buttonActionName;
                buttonText.fontSize = 15;
            }
        }
        else if (buttonActionName == "Select")
        {
            switch (cc.controllerType)
            {
                case ThirdPersonInput.ControllerType.MOUSEKEYBOARD:
                    buttonImage1.sprite = interactUIM.keyboardButtonIcons.attackClick;
                    break;
                case ThirdPersonInput.ControllerType.XBOX:
                    buttonImage1.sprite = interactUIM.xboxButtonIcons.aButton;
                    break;
                case ThirdPersonInput.ControllerType.PLAYSTATION:
                    buttonImage1.sprite = interactUIM.playstationButtonIcons.crossButton;
                    break;
            }
            if (buttonText != null)
            {
                buttonText.text = buttonActionName;
                buttonText.fontSize = 15;
            }
        }
        else if (buttonActionName == "Reset")
        {
            switch (cc.controllerType)
            {
                case ThirdPersonInput.ControllerType.MOUSEKEYBOARD:
                    buttonImage1.sprite = interactUIM.keyboardButtonIcons.mouseWheel;
                    break;
                case ThirdPersonInput.ControllerType.XBOX:
                    buttonImage1.sprite = interactUIM.xboxButtonIcons.leftStickButton;
                    break;
                case ThirdPersonInput.ControllerType.PLAYSTATION:
                    buttonImage1.sprite = interactUIM.playstationButtonIcons.leftStickButton;
                    break;
            }
            if (buttonText != null)
            {
                buttonText.text = buttonActionName;
                buttonText.fontSize = 15;
            }
        }
    }
}
