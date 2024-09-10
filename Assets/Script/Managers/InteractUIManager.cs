using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;
using RPGAE.CharacterController;

public class InteractUIManager : MonoBehaviour
{
    [Serializable]
    public class KeyboardButtonIcons
    {
        public Sprite eButton, cButton, vButton, xButton,
        zButton,
        spaceButton, shiftButton, wasd,
        mouseWheel, escapeButton, mouseMove,
        aimClick, attackClick, oneButton,
        twoButton, threeButton, fourButton;
    }
    [Serializable]
    public class XboxButtonIcons
    {
        public Sprite xButton, aButton,
        bButton, yButton, leftStickButton, rightStickButton, leftStickAxis,
        rightStickAxis, start, select, leftTrigger,
        rightTrigger, leftBumber, rightBumber, DPadUp, DPadLeft, DPadRight, DPadDown;
    }
    [Serializable]
    public class PlayStationButtonIcons
    {
        public Sprite squareButton, crossButton,
        circleButton, triangleButton, leftStickButton, 
        rightStickButton, leftStickAxis, rightStickAxis, 
        share, option, L1, R1, L2, R2, DPadUp, DPadLeft, DPadRight, DPadDown;
    }

    [Header("REFERENCES")]
    public KeyboardButtonIcons keyboardButtonIcons;
    public XboxButtonIcons xboxButtonIcons;
    public PlayStationButtonIcons playstationButtonIcons;

}
