using System;
using _Project.Script.Controller;
using UnityEngine;

public class PlayerController : UnitController
{
    private bool infoShow = false;
    
    private void OnMouseDown()
    {
    
        //ApplicationManager.Inst.GetModule<CharacterSkillController>().Show();
        ApplicationManager.Inst.GetModule<CharacterSkillController>().Init
            (_unit,_unit.GetSkillList(),curTile);
         
        

    }
}
