using Core.Utility;
using SkillData.SkillEffects;
using UnityEngine;

namespace _Project.Script.Relic
{
    public class DamageBuff : RelicEffect
    {
        public override void Excute()
        {
            var players = InGameUnitInfo.PlayerUnits;
            foreach (var player in players)
            {
                ActionStateExamples.DamageBuff(player, values[0]);
            }
            
            Debug.Log("데미지 버프 유물");
        }
    }
}