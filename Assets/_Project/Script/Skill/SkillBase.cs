using System;
using System.Collections.Generic;
using GoogleSheet.Core.Type;
using UnityEngine;

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
        
        private Data _data; 
        private SkillContext skillContext = new SkillContext();
        private List<SkillEffect> effects = new List<SkillEffect>();
        string effectInfor = "";
            
        public void InitTarget(Tile target)=>skillContext.TargetTile = target;
        public void InitSource(Tile owner) => skillContext.SourceTile = owner;
        
        public Data GetData()=>_data;
        
        /// <summary>
        /// Data를 통해서 SkillBase 생성
        /// </summary>
        public SkillBase(Data data)
        {
            _data = data;
            FindSkillEffects(_data.EffectData);
            skillContext.Init(data.RowCount,data.ColumnCount);
        }

        /// <summary>
        /// EffectData split해서 리스트에 저장
        /// </summary>
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
        
        /// <summary>
        /// 스킬 정보 받기
        /// </summary>
        public string GetSkillDescription()=>_data.Infor + " " + effectInfor;
        
        /// <summary>
        /// 스킬 실행
        /// </summary>
        public void SkillAction()
        {
            foreach (SkillEffect effect in effects)
            {
                effect.Apply(skillContext);
            }
        }

        public float GetFinalDamage(int value) => 1;//_data.SkillAttribute.Calculation<SkillBase>(value,skillContext.SourceTile.GetStatContainer());

        /// <summary>
        /// SkillBase 깊은 복사
        /// </summary>
        public SkillBase Clone()
        {
            SkillBase clone = new SkillBase(_data);
            return clone;
        }

        
    }
    

}






