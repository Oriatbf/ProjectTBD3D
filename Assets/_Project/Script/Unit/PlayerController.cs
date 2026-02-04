using System;
using _Project.Script.Controller;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : UnitController
{
    private bool infoShow = false;
    
    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        if (_unit.GetStatContainer().isStun || !FactoryManager.Inst.IsGameStart()) return;
        ApplicationManager.Inst.GetModule<CharacterSkillController>().Init
            (_unit,_unit.GetSkillList(),curTile);
         
        

    }
}
