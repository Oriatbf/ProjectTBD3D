using System;
using _Project.Script.Controller;
using UnityEngine;

public class PlayerController : UnitController
{
    private bool infoShow = false;
    
    private void OnMouseDown()
    {
        if (!infoShow)
        {
            ApplicationManager.Inst.GetModule<CharacterSkillController>().Show();
            ApplicationManager.Inst.GetModule<CharacterSkillController>().Init
                (_unit.GetStatContainer().turnGauge,_unit.GetSkillList(),curTile);
            infoShow = true;
        }
        else
        {
            ApplicationManager.Inst.GetModule<CharacterSkillController>().Hide();
            infoShow = false;
        }
    }
}
