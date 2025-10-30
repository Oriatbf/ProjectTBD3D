using System;
using SkillData;
using UnityEngine;

public class BuffDebuff
{
    public Action action,deActiveAction;
    public int turnCount;
    public SkillBase skillBase;
    public int stackCount = 1;
    public bool isStackable = false;
    public bool isExist = true;

    public BuffDebuff(Action action,Action deActiveAction, SkillBase skillBase,int turnCount,bool isStackable = false,int stackCount=1)
    {
        this.deActiveAction = deActiveAction;
        this.action = action;
        this.turnCount = turnCount;
        this.skillBase = skillBase;
        this.stackCount = stackCount;
        this.isStackable = isStackable;
    }

    public void Excute()
    {
        if(turnCount > 0)action?.Invoke();
        if(isStackable && stackCount > 0)stackCount-=1;
        turnCount-=1;
        if(turnCount <=0 || stackCount <= 0)DeActivate();
    }

    private void DeActivate()
    {
        isExist = false;
        deActiveAction?.Invoke();
    }

    public void AddCount(int value)
    {
        if(isStackable)stackCount += value;
    }
    
}

