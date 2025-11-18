using System;
using SkillData;
using UnityEngine;

public enum DecreaseType
{
    None,OnlyTurn,OnlyStack,TurnAndStack
}

public class BuffDebuff
{
    public Action<BuffDebuff> buffAction;
    public Action deActiveAction;
    public Unit targetUnit;
    public string id;
    public int turnCount;
    public int stackCount = 1;
    public DecreaseType decreaseType;
    public bool isExist = true;

    public BuffDebuff(Unit targetUnit,string id,int turnCount,DecreaseType decreaseType,int stackCount=1)
    {
        this.id = id;
        this.targetUnit = targetUnit;
        this.turnCount = turnCount;
        this.stackCount = stackCount;
        this.decreaseType = decreaseType;
    }

    public void AddBuffAction(Action<BuffDebuff> buffAction)
    {
        this.buffAction += buffAction;
    }

    public void InitExtraData(BuffDebuff buffDebuff)
    {
        turnCount = turnCount < buffDebuff.turnCount? buffDebuff.turnCount : turnCount;
        stackCount += buffDebuff.stackCount;
    }
    

    /// <summary>
    /// 디버프 실행
    /// </summary>
    public void Execute()
    {
        if(isExist)buffAction?.Invoke(this);
        if( isStackAble())stackCount-=1;
        if(isTurnAble())turnCount-=1;
        if((isTurnAble() &&turnCount <=0) || (isStackAble()&&stackCount <= 0))DeActivate();
    }

    /// <summary>
    /// 스택감소 형 디버프인가
    /// </summary>
    private bool isStackAble()
    {
        bool result = false;
        switch (decreaseType)
        {
            case DecreaseType.OnlyStack: result = true;break;
            case DecreaseType.TurnAndStack: result = true; break;
        }
        return result;
    }

    /// <summary>
    /// 턴 형 디버프인가
    /// </summary>
    private bool isTurnAble()
    {
        bool result = false;
        switch (decreaseType)
        {
            case DecreaseType.OnlyTurn: result = true;break;
            case DecreaseType.TurnAndStack: result = true; break;
        }
        return result;
    }

    public void DeActivate()
    {
        stackCount=0;
        turnCount=0;
        isExist = false;
        deActiveAction?.Invoke();
    }
    
    
}

