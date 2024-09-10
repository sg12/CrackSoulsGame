using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public abstract class UICursorBehaviour : MonoBehaviour
{
    /// <summary>
    /// Cursor speed for movement
    /// </summary>
    [Tooltip("Cursor speed for movement")]
    public float m_Speed = 450f;
    private RectTransform m_RectTransform;
    public GameObject ClickableElement { get; set; }
    [HideInInspector] public FadeUI fadeUI;
    [HideInInspector] public InventoryManager inventoryM;

    public RectTransform RectTransform
    {
        get
        {
            return m_RectTransform;
        }
    }
    public bool IsOverClickeableElement
    {
        get
        {
            return ClickableElement != null ? true : false;
        }
    }

    protected virtual void Awake()
    {
        m_RectTransform = this.GetComponent<RectTransform>();
        inventoryM = FindObjectOfType<InventoryManager>();
        fadeUI = GetComponent<FadeUI>();
    }
    public abstract void OnClickableElementChanged();
    public abstract void OnClick();

}
