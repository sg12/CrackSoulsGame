using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using RPGAE.CharacterController;

public class EquippedItemSlot : MonoBehaviour
{
    [Header("REFERENCES")]
    public Image[] NumOfSEngraving;
    public Animator outLineBorderAnim;
    public Image itemImage;
    public Image statValueBG;

    private Image outlineBorder;
    public int m_numOfEngraving;

    void Start()
    {
        outlineBorder = outLineBorderAnim.GetComponent<Image>();
        foreach (Image num in NumOfSEngraving)
        {
            //num.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (m_numOfEngraving > 0)
        {
            for (int i = 0; i < m_numOfEngraving; i++)
            {
                NumOfSEngraving[i].gameObject.SetActive(true);
            }
        }
        else
        {
            foreach (Image num in NumOfSEngraving)
            {
                num.gameObject.SetActive(false);
            }
        }
    }
}
