using System;
using UnityEngine;

public class PlayerController : UnitController
{
    private bool infoShow = false;
    
    private void OnMouseDown()
    {
        if (!infoShow)
        {
            ApplicationManager.Inst.GetModule<CharacterInfoController>().Show();
            ApplicationManager.Inst.GetModule<CharacterInfoController>().Init(unitData,curTile);
            infoShow = true;
        }
        else
        {
            ApplicationManager.Inst.GetModule<CharacterInfoController>().Hide();
            infoShow = false;
        }
    }
}
