using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : UICursorBehaviour
{
    public Animator m_animator;

    public override void OnClickableElementChanged()
    {
        if (m_animator != null && m_animator.isActiveAndEnabled)
        {
            m_animator.SetBool("Hover", base.IsOverClickeableElement);
        }
    }

    public override void OnClick()
    {
        if (m_animator != null && m_animator.isActiveAndEnabled)
        {
            m_animator.SetTrigger("Click");
        }
    }

    public void InventorySectionClick()
    {
        inventoryM.pauseMenuNavigation = 0;
        inventoryM.PauseMenuSectionIsActive(false);
        inventoryM.pauseMenuS = InventoryManager.PauseMenuSection.Inventory;
    }

    public void QuestSectionClick()
    {
        inventoryM.pauseMenuNavigation = 1;
        inventoryM.PauseMenuSectionIsActive(false);
        inventoryM.pauseMenuS = InventoryManager.PauseMenuSection.Quest;
    }

    public void MapSectionClick()
    {
        inventoryM.pauseMenuNavigation = 2;
        inventoryM.PauseMenuSectionIsActive(false);
        inventoryM.pauseMenuS = InventoryManager.PauseMenuSection.Map;
    }

    public void SkillsSectionClick()
    {
        inventoryM.pauseMenuNavigation = 3;
        inventoryM.PauseMenuSectionIsActive(false);
        inventoryM.pauseMenuS = InventoryManager.PauseMenuSection.Skills;
    }

    public void SystemSectionClick()
    {
        inventoryM.pauseMenuNavigation = 4;
        inventoryM.PauseMenuSectionIsActive(false);
        inventoryM.pauseMenuS = InventoryManager.PauseMenuSection.System;
    }
}