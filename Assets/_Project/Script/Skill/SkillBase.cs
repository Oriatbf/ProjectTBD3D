using System;
using System.Collections.Generic;
using GoogleSheet.Core.Type;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[UGS(typeof(SkillType))]
public enum SkillType{Skill,Item}

[UGS(typeof(SkillAttribute))]
public enum SkillAttribute
{
    Physical,Mage,Charm,All
}

[UGS(typeof(TargetType))]
public enum TargetType
{
    Target,NoneTarget
}

namespace SkillData
{
    [Serializable]
    public class SkillBase 
    {
        
        public Data _data;
        private SkillContext skillContext = new SkillContext();
        private List<SkillEffect> effects = new List<SkillEffect>();
        string effectInfor = "";
            
        public void InitTarget(Unit target)=>skillContext.Target = target;
        public void InitOwner(Unit owner) => skillContext.Source = owner;
        
        public SkillBase(Data data)
        {
            _data = data; 
            FindSkillEffects(_data.EffectData);
        }

        private void FindSkillEffects(string data)
        {
            string[] parts = data.Split('/');        // Split 사용
            foreach (string part in parts)
            {
                string[] split = part.Split(':');
                var effectType = Enum.Parse<EffectTypeFactory.EffectType>(split[0]);
                var skillEffect = EffectTypeFactory.CreateInstance(effectType);
                
                string[] v = split[1].Split(',');
                List<int> values = new List<int>();
                for (int i = 0; i < v.Length; i++)
                    values.Add(int.Parse(v[i]));
                
                skillEffect.Init(this, values);
                effects.Add(skillEffect);
            }

            effectInfor = "";
            foreach (var effect in effects)
                effectInfor += effect.ReturnInformation() + " ";
        }
        
        public string GetSkillDescription()=>_data.Infor + " " + effectInfor;

        public void SkillAction()
        {
            foreach (SkillEffect effect in effects)
            {
                effect.Apply(skillContext);
            }
        }

        public float GetFinalDamage(int value) => 1;//_data.SkillAttribute.Calculation<SkillBase>(value,skillContext.Source.GetStatContainer());

        public SkillBase Clone()
        {
            SkillBase clone = new SkillBase(_data);
            return clone;
        }

        
    }
    

}






