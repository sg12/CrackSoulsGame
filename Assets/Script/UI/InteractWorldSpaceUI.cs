using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using RPGAE.CharacterController;

public class InteractWorldSpaceUI : MonoBehaviour
{
    [Header("UI ACTION NAME")]
    public string buttonActionName;

    [Header("SETTINGS")]
    public bool rotateOnY;
    public bool rotateWithCamera;
    public float fwdOffSet = 0.5f;
    public float heightOffSet = 1;

    [Header("AUDIO")]
    public RandomAudioPlayer openSnd;

    [Header("REFERENCES")]
    public FadeUI fadeUI;
    public Image buttonImage;
    public TextMeshProUGUI buttonText;

    [HideInInspector]
    public Transform itemCenterParent;
    private ThirdPersonController cc;
    private InteractUIManager interactUIM;

    // Start is called before the first frame update
    void Start()
    {
        cc = FindObjectOfType<ThirdPersonController>();
        interactUIM = FindObjectOfType<InteractUIManager>();

        SetParentToCenterItem();
    }

    void Update()
    {
        SetUIButton();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UIPosition();
        UIRotation();
    }

    #region Set UI button 

    void SetUIButton()
    {
        if (buttonActionName == "Finisher")
        {
            switch (cc.controllerType)
            {
                case ThirdPersonInput.ControllerType.MOUSEKEYBOARD:
                    buttonImage.sprite = interactUIM.keyboardButtonIcons.mouseWheel;
                    break;
                case ThirdPersonInput.ControllerType.XBOX:
                    buttonImage.sprite = interactUIM.xboxButtonIcons.rightStickAxis;
                    break;
                case ThirdPersonInput.ControllerType.PLAYSTATION:
                    buttonImage.sprite = interactUIM.playstationButtonIcons.rightStickAxis;
                    break;
            }
            buttonText.fontSize = 25;
            buttonText.text = buttonActionName;
        }
        else if (buttonActionName == "Pick Up")
        {
            switch (cc.controllerType)
            {
                case ThirdPersonInput.ControllerType.MOUSEKEYBOARD:
                    buttonImage.sprite = interactUIM.keyboardButtonIcons.eButton;
                    break;
                case ThirdPersonInput.ControllerType.XBOX:
                    buttonImage.sprite = interactUIM.xboxButtonIcons.xButton;
                    break;
                case ThirdPersonInput.ControllerType.PLAYSTATION:
                    buttonImage.sprite = interactUIM.playstationButtonIcons.squareButton;
                    break;
            }
            buttonText.fontSize = 25;
            buttonText.text = buttonActionName;
        }
        else if (buttonActionName == "Carry")
        {
            switch (cc.controllerType)
            {
                case ThirdPersonInput.ControllerType.MOUSEKEYBOARD:
                    buttonImage.sprite = interactUIM.keyboardButtonIcons.eButton;
                    break;
                case ThirdPersonInput.ControllerType.XBOX:
                    buttonImage.sprite = interactUIM.xboxButtonIcons.xButton;
                    break;
                case ThirdPersonInput.ControllerType.PLAYSTATION:
                    buttonImage.sprite = interactUIM.playstationButtonIcons.squareButton;
                    break;
            }
            buttonText.fontSize = 25;
            buttonText.text = buttonActionName;
        }
        else if (buttonActionName == "Open Door")
        {
            switch (cc.controllerType)
            {
                case ThirdPersonInput.ControllerType.MOUSEKEYBOARD:
                    buttonImage.sprite = interactUIM.keyboardButtonIcons.eButton;
                    break;
                case ThirdPersonInput.ControllerType.XBOX:
                    buttonImage.sprite = interactUIM.xboxButtonIcons.xButton;
                    break;
                case ThirdPersonInput.ControllerType.PLAYSTATION:
                    buttonImage.sprite = interactUIM.playstationButtonIcons.squareButton;
                    break;
            }
            buttonText.fontSize = 25;
            buttonText.text = buttonActionName;
        }
        else if (buttonActionName == "Loot Chest")
        {
            switch (cc.controllerType)
            {
                case ThirdPersonInput.ControllerType.MOUSEKEYBOARD:
                    buttonImage.sprite = interactUIM.keyboardButtonIcons.eButton;
                    break;
                case ThirdPersonInput.ControllerType.XBOX:
                    buttonImage.sprite = interactUIM.xboxButtonIcons.xButton;
                    break;
                case ThirdPersonInput.ControllerType.PLAYSTATION:
                    buttonImage.sprite = interactUIM.playstationButtonIcons.squareButton;
                    break;
            }
            buttonText.fontSize = 25;
            buttonText.text = buttonActionName;
        }
        else if (buttonActionName == "Talk")
        {
            switch (cc.controllerType)
            {
                case ThirdPersonInput.ControllerType.MOUSEKEYBOARD:
                    StopAllCoroutines();
                    buttonImage.sprite = interactUIM.keyboardButtonIcons.eButton;
                    break;
                case ThirdPersonInput.ControllerType.XBOX:
                    StopAllCoroutines();
                    buttonImage.sprite = interactUIM.xboxButtonIcons.xButton;
                    break;
                case ThirdPersonInput.ControllerType.PLAYSTATION:
                    StopAllCoroutines();
                    buttonImage.sprite = interactUIM.playstationButtonIcons.squareButton;
                    break;
            }
            buttonText.fontSize = 25;
            buttonText.text = buttonActionName;
        }
        else if (buttonActionName == "Climb")
        {
            switch (cc.controllerType)
            {
                case ThirdPersonInput.ControllerType.MOUSEKEYBOARD:
                    buttonImage.sprite = interactUIM.keyboardButtonIcons.eButton;
                    break;
                case ThirdPersonInput.ControllerType.XBOX:
                    buttonImage.sprite = interactUIM.xboxButtonIcons.xButton;
                    break;
                case ThirdPersonInput.ControllerType.PLAYSTATION:
                    buttonImage.sprite = interactUIM.playstationButtonIcons.squareButton;
                    break;
            }
            buttonText.fontSize = 25;
            buttonText.text = buttonActionName;
        }
        else if (buttonActionName == "Climb Jump")
        {
            switch (cc.controllerType)
            {
                case ThirdPersonInput.ControllerType.MOUSEKEYBOARD:
                    buttonImage.sprite = interactUIM.keyboardButtonIcons.spaceButton;
                    break;
                case ThirdPersonInput.ControllerType.XBOX:
                    buttonImage.sprite = interactUIM.xboxButtonIcons.aButton;
                    break;
                case ThirdPersonInput.ControllerType.PLAYSTATION:
                    buttonImage.sprite = interactUIM.playstationButtonIcons.crossButton;
                    break;
            }
            buttonText.fontSize = 25;
            buttonText.text = buttonActionName;
        }
        else if (buttonActionName == "Climb Up")
        {
            switch (cc.controllerType)
            {
                case ThirdPersonInput.ControllerType.MOUSEKEYBOARD:
                    buttonImage.sprite = interactUIM.keyboardButtonIcons.spaceButton;
                    break;
                case ThirdPersonInput.ControllerType.XBOX:
                    buttonImage.sprite = interactUIM.xboxButtonIcons.aButton;
                    break;
                case ThirdPersonInput.ControllerType.PLAYSTATION:
                    buttonImage.sprite = interactUIM.playstationButtonIcons.crossButton;
                    break;
            }
            buttonText.fontSize = 25;
            buttonText.text = buttonActionName;
        }
        else if (buttonActionName == "Let Go")
        {
            switch (cc.controllerType)
            {
                case ThirdPersonInput.ControllerType.MOUSEKEYBOARD:
                    buttonImage.sprite = interactUIM.keyboardButtonIcons.eButton;
                    break;
                case ThirdPersonInput.ControllerType.XBOX:
                    buttonImage.sprite = interactUIM.xboxButtonIcons.bButton;
                    break;
                case ThirdPersonInput.ControllerType.PLAYSTATION:
                    buttonImage.sprite = interactUIM.playstationButtonIcons.circleButton;
                    break;
            }
            buttonText.fontSize = 25;
            buttonText.text = buttonActionName;
        }
        else if (buttonActionName == "")
        {
            if (fadeUI.canvasGroup.alpha == 1) fadeUI.FadeTransition(0, 0, 0.4f);
        }
        if (buttonActionName == null)
            fadeUI.canvasGroup.alpha = 0;
    }

    void UIPosition()
    {
        if (itemCenterParent != null)
        {
            transform.position = itemCenterParent.position + (transform.up * heightOffSet);
        }
    }

    void UIRotation()
    {
        Vector3 lookPos = Camera.main.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        if (rotateWithCamera)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 8 * Time.deltaTime);
            transform.eulerAngles = new Vector3(rotateOnY ? 0 : transform.eulerAngles.x, transform.eulerAngles.y, 0);
        }
    }

    void SetParentToCenterItem()
    {
        if (GetComponentInParent<ItemData>())
        {
            foreach (Transform t in GetComponentInParent<ItemData>().GetComponentsInChildren<Transform>())
            {
                if (t.name == "Center")
                {
                    itemCenterParent = t;
                }
            }
            transform.SetParent(itemCenterParent.parent.transform.parent);
        }
    }

    public void ToggleIcon(bool _On)
    {
        if (fadeUI == null) return;

        if (_On)
        {
            if (fadeUI.canvasGroup.alpha == 0)
            {
                if (openSnd != null && !openSnd.playing)
                {
                    openSnd.PlayRandomClip();
                }
                fadeUI.FadeTransition(1, 0, 0.4f);
            }
        }
        else
        {
            if (fadeUI.canvasGroup.alpha == 1)
            {
                fadeUI.FadeTransition(0, 0, 0.4f);
            }
        }
    }

    #endregion

}
