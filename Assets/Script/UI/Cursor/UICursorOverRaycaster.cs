using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class UICursorOverRaycaster : BaseRaycaster
{
    public static PointerEventData LastPointerEventData = null;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        LastPointerEventData = null;
    }

    public override Camera eventCamera
    {
        get
        {
            return null;
        }
    }

    //Used only to obtain the PointerEventData, it was not possible otherwise due to access restrictions
    public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
    {
        UICursorOverRaycaster.LastPointerEventData = eventData;
    }
}

