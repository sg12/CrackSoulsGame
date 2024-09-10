using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using RPGAE.CharacterController;
using UnityEngine;

public class MiniMapDrag : MonoBehaviour
{
    private MiniMap MiniMap;
    private RectTransform Area;
    private Vector2 origin;
    private Vector2 direction;

    Vector2 localPoint;

    Vector2 relativePoint;

    ThirdPersonController cc;

    // Start is called before the first frame update
    void Start()
    {
        MiniMap = transform.root.GetComponentInChildren<MiniMap>();
        cc = FindObjectOfType<ThirdPersonController>();
        direction = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        MiniMap.MMCamera.transform.position = relativePoint;
    }


    private Vector2 PointerDataToRelativePos(PointerEventData eventData)
    {
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(Area, eventData.pressPosition, eventData.pressEventCamera, out localPoint))
        {
            MiniMap.MMCamera.transform.position = relativePoint;
        }
        return relativePoint;
    }
}
