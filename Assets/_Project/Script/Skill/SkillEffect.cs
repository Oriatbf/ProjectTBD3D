using System.Collections.Generic;
using SkillData;
using UnityEngine;


/// <summary>
/// SkillEffect는 부모인 SkillBase에서 하나의 스킬을 담당하는 요소
/// </summary>
public abstract class SkillEffect
{
    protected List<int> values = new List<int>(); //스킬마다 사용되는 정수 값

    /// <summary>
    /// SkillBase, values 데이터 받는 코드 values는 스킬별 사용되는 값들이 저장되어 있음
    /// </summary>
    public void Init(SkillBase skillBase,List<int> values)
    {
        this.values = values;
    }
    
    /// <summary>
    /// 스킬 구현 코드
    /// </summary>
    public abstract void Apply(SkillContext skillContext); 
    protected abstract void SkillAction(SkillContext skillContext);
    
    /// <summary>
    /// 각 스킬별 설명을 적어서 리턴하는 코드
    /// </summary>
    public abstract string ReturnInformation();
    
}

/// <summary>
/// 텍스트 컬러를 지정할 때 사용
/// </summary>
public enum TxtColorType
{
    Str,Intelligence,Health,Charm
}

/// <summary>
/// 리치 텍스트 색 받는 코드
/// </summary>
public static class IColorText
{
    public static string GetTextColor(TxtColorType type)
    {
        string color = "";
        switch (type)
        {
            case TxtColorType.Str:
                color = "<color=#EF4040>";
                break;
            case TxtColorType.Intelligence:
                color = "<color=#40B8EF>";
                break;
            case TxtColorType.Health:
                color = "<color=#40EF5A>";
                break;
            case TxtColorType.Charm:
                color = "<color=#fa97c9>";
                break;
        }
        return color;
    }
}
