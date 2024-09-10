using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using RPGAE.CharacterController;

public class ItemOptionsPreview : MonoBehaviour 
{

    readonly List<string> names = new List<string>() { "Choose:", "Cancel"};

    [Header("AUDIO")]
    public RandomAudioPlayer cancelAS;

    #region Private 

    private bool onButton;
    private TMP_Dropdown dropDown;
    private TextMeshProUGUI selectedName;
    private GameObject dropDownObj;
    private InventoryManager inventoryM;

    #endregion

    // Use this for initialization
    void Awake()
    {
        dropDown = GetComponent<TMP_Dropdown>();
        inventoryM = FindObjectOfType<InventoryManager>();
        selectedName = GameObject.Find("PreviewLabel").GetComponent<TextMeshProUGUI>();
        PopulateList();
    }

    // Update is called once per frame
    void Update () 
    {
        dropDownObj = GameObject.Find("Dropdown List");

        if (onButton)
        {
            if (inventoryM.cc.rpgaeIM.PlayerControls.Action.triggered)
            {
                dropDown.Show();
                onButton = false;
            }
        }
    }

    public void DropDownSelect(int index)
    {
        selectedName.text = names[index];

        if (index == 1)
        {
            // Cancel Item
            if (inventoryM.inventoryS == InventoryManager.InventorySection.Key)
            {
                Image outLine = inventoryM.materialInv.outLineBorder[inventoryM.materialInv.slotNum];

                outLine.enabled = false;
                outLine.color = inventoryM.outLineHover;
            }
            if (cancelAS)
                cancelAS.PlayRandomClip();

            dropDown.value = 0;
            DestroyImmediate(dropDownObj);
            gameObject.SetActive(false);
        }
    }

    void PopulateList()
    {
        dropDown.AddOptions(names);
    }
}
