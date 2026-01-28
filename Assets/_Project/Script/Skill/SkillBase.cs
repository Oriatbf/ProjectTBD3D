using System;
using System.Collections.Generic;
using GoogleSheet.Core.Type;
using UnityEngine;


[UGS(typeof(TargetType))]
public enum TargetType
{
    Area,Source,All
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
            
        public void InitTarget(Tile target)=>skillContext.InitTargetTile(target);
        public void InitSource(Tile owner) => skillContext.InitSourceTile(owner);
        public void InitStackTurn(float stackTurn) => skillContext.InitStackTurn(stackTurn);
        public SkillContext GetSkillContext() => skillContext;
        public Data GetData()=>_data;

        /// <summary>
        /// Data를 통해서 SkillBase 생성
        /// </summary>
        public SkillBase(SkillData.Data data)
        {
            _data = data;
            effects = new List<SkillEffect>();
            FindSkillEffects(_data.EffectData);
            skillContext.Init(data.TargetType, data.RowCount, data.ColumnCount);
        }

        public SkillBase(SkillBase originalSkillBase)
        {
            _data = originalSkillBase._data;
            effects = new List<SkillEffect>();
            FindSkillEffects(_data.EffectData);
            skillContext = new SkillContext(originalSkillBase.skillContext);
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
                List<int> values = new List<int>();
                if (split.Length > 1)
                {
                    string[] v = split[1].Split(',');
                    for (int i = 0; i < v.Length; i++)
                        values.Add(int.Parse(v[i]));
                }
                
                skillEffect.Init(values);
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
            Action<SkillContext> skillAction=null;
            foreach (SkillEffect effect in effects)
            {
                effect.Apply(ref skillAction);
            }
            skillAction?.Invoke(skillContext);
        }

        public float GetFinalDamage(int value) => 1;//_data.SkillType.Calculation<SkillBase>(value,skillContext.SourceTile.GetStatContainer());

        /// <summary>
        /// SkillBase 깊은 복사
        /// </summary>
        public SkillBase Clone()
        {
            SkillBase clone = new SkillBase(this);
            return clone;
        }

        
    }
    

}






