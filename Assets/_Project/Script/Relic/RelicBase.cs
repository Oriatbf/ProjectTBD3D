using System;
using System.Collections.Generic;
using _Project.Script.Relic;
using SkillData;
using Unity.VisualScripting;
using UnityEngine;

public class RelicBase 
{
    private RelicData.Data _data; 
    private List<RelicEffect> effects = new List<RelicEffect>();
    private string description;
    
  
    public RelicData.Data GetData()=>_data;

    public RelicBase(RelicData.Data data)
    {
        _data = data;
        effects = new List<RelicEffect>();
        FindRelicEffects(data.RelicEffectData);
        SetDescription();
    }

    public RelicBase(RelicBase originalRelicBase)
    {
        _data = originalRelicBase.GetData();
        effects =originalRelicBase.effects;
       
    }

    private void SetDescription()
    {
        description = "";
        foreach (var relicEffect in effects)
        {
            description += relicEffect.ReturnInformation() + " ";
        }
        
    }

    public string GetRelicDescription() => description;
    
   
    

    /// <summary>
    /// EffectData split해서 리스트에 저장
    /// </summary>
    private void FindRelicEffects(string data)
    {
        string[] parts = data.Split('/');        // Split 사용
        foreach (string part in parts)
        {
            string[] split = part.Split(':');
            var effectType = Enum.Parse<RelicTypeFactory.RelicType>(split[0]);
            var relicEffect = RelicTypeFactory.CreateInstance(effectType);
            List<int> values = new List<int>();
            if (split.Length > 1)
            {
                string[] v = split[1].Split(',');
                for (int i = 0; i < v.Length; i++)
                    values.Add(int.Parse(v[i]));
            }
            
            relicEffect.Init( values);
            effects.Add(relicEffect);
        }
    }
    
    public void Excute()
    {
        foreach (var relicEffect in effects)
        {
            relicEffect.Excute();
        }
    }
    
    /// <summary>
    /// SkillBase 깊은 복사
    /// </summary>
    public RelicBase Clone()
    {
        RelicBase clone = new RelicBase(this);
        return clone;
    }

}
