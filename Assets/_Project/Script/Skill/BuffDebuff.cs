using System;
using SkillData;
using UnityEngine;

public class BuffDebuff
{
    public Action<BuffDebuff> buffAction;
    public Action deActiveAction;
    public Unit targetUnit;
    public int turnCount;
    public int stackCount = 1;
    public bool isStackable = false;
    public bool isExist = true;

    public BuffDebuff(Unit targetUnit,int turnCount,bool isStackable = false,int stackCount=1)
    {
        this.targetUnit = targetUnit;
        this.turnCount = turnCount;
        this.stackCount = stackCount;
        this.isStackable = isStackable;
    }

    public void AddBuffAction(Action<BuffDebuff> buffAction)
    {
        this.buffAction += buffAction;
    }

    public void Execute()
    {
        if(turnCount > 0)buffAction?.Invoke(this);
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

