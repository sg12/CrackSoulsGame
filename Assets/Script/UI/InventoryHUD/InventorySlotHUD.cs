using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using RPGAE.CharacterController;
using TMPro;

public class InventorySlotHUD : MonoBehaviour
{
    public int slotNum = 0;

    [Header("REFERENCES")]
    public RectTransform rectT;
    public Image outLineEquipped;
    public Image highLight;
    public Image itemImage;
    public Image statValueBG;
    public TextMeshProUGUI statValue;
    public TextMeshProUGUI itemQuantity;

    public ItemData itemData;

    private GameObject contentPanel;
    private InventoryHUD inventoryHUD;

    // Start is called before the first frame update
    void Start()
    {
        contentPanel = GameObject.Find("InventoryHUDContent");
        rectT.SetParent(contentPanel.transform);
        rectT.localPosition = new Vector3(0, 0, 0);
        rectT.localScale = new Vector3(1, 1, 1);
        inventoryHUD = FindObjectOfType<InventoryHUD>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.name == "InventoryHUD Scroll View")
        {
            collision.GetComponent<InventoryHUD>().slotNum = slotNum;
            collision.GetComponent<InventoryHUD>().inventorySlotHUD = this;
            collision.GetComponent<InventoryHUD>().itemName.text = itemData.itemName + itemData.rankTag;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == "InventoryHUD Scroll View")
        {
            collision.GetComponent<InventoryHUD>().inventorySlotHUD = null;
        }
    }
}
