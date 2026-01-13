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
            ApplicationManager.Inst.GetModule<CharacterInfoController>().Show();
            ApplicationManager.Inst.GetModule<CharacterInfoController>().Init
                (_unit.GetStatContainer().turnGauge,_unit.GetSkillList(),curTile);
            infoShow = true;
        }
        else
        {
            ApplicationManager.Inst.GetModule<CharacterInfoController>().Hide();
            infoShow = false;
        }
    }
}
