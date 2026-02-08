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
    
    private RelicTriggerType triggerType;
    
    private string description;
    
  
    public RelicData.Data GetData()=>_data;
    
    public RelicTriggerType GetTriggerType() => triggerType;

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
        description = originalRelicBase.description;

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


    #region DataString파싱
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
    
       /// <summary>
    /// EffectData 파싱 - 더 유연한 포맷 지원
    /// 형식: "EffectType:value1,value2,value3|trigger=BattleStart|target=AllAllies|cooldown=5/..."
    /// </summary>
    private void ParseRelicEffects(string data)
    {
        string[] effectParts = data.Split('/');
        
        foreach (string effectPart in effectParts)
        {
            if (string.IsNullOrEmpty(effectPart)) continue;

            var effect = ParseSingleEffect(effectPart);
            if (effect != null)
            {
                effects.Add(effect);
            }
        }
    }

    /// <summary>
    /// 단일 효과 파싱
    /// </summary>
    private RelicEffect ParseSingleEffect(string effectData)
    {
        string[] properties = effectData.Split('|');
        
        // 첫 번째는 항상 "EffectType:values" 형식
        string[] mainPart = properties[0].Split(':');
        
        if (!Enum.TryParse<RelicTypeFactory.RelicType>(mainPart[0], out var effectType))
        {
            Debug.LogError($"Invalid RelicType: {mainPart[0]}");
            return null;
        }

        var relicEffect = RelicTypeFactory.CreateInstance(effectType);
        
        // values 파싱
        List<int> values = new List<int>();
        if (mainPart.Length > 1)
        {
            string[] valueStrings = mainPart[1].Split(',');
            foreach (var valueStr in valueStrings)
            {
                if (int.TryParse(valueStr, out int value))
                {
                    values.Add(value);
                }
            }
        }

        relicEffect.Init(values);

        // 추가 속성 파싱 (trigger, target, cooldown 등)
        for (int i = 1; i < properties.Length; i++)
        {
            ParseEffectProperty(relicEffect, properties[i]);
        }

        return relicEffect;
    }

    /// <summary>
    /// 효과 속성 파싱 (확장 가능)
    /// </summary>
    private void ParseEffectProperty(RelicEffect effect, string property)
    {
        string[] keyValue = property.Split('=');
        if (keyValue.Length != 2) return;

        string key = keyValue[0].Trim().ToLower();
        string value = keyValue[1].Trim();

        // 리플렉션을 사용하여 동적으로 속성 설정 가능
        var field = effect.GetType().GetField(key, 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance);
        
        if (field != null)
        {
            try
            {
                if (field.FieldType == typeof(float))
                    field.SetValue(effect, float.Parse(value));
                else if (field.FieldType == typeof(int))
                    field.SetValue(effect, int.Parse(value));
                else if (field.FieldType.IsEnum)
                    field.SetValue(effect, Enum.Parse(field.FieldType, value));
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to set property {key}: {e.Message}");
            }
        }
    }
    

    #endregion
    
    
    public void Execute( RelicEffectContext relicEffectContext )
    {
        foreach (var relicEffect in effects)
        {
            relicEffect.Execute(new RelicEffectContext());
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
    public List<RelicEffect> GetEffects() => effects;

}
