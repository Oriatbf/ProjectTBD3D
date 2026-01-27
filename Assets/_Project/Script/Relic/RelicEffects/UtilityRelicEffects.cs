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

        public override string ReturnInformation()
        {
            return $"시작 시 모든 아군유닛에게 데미지버프 " +
                   $"{ColorText.GetTextColor(TxtColorType.Str)}{values[0]}</color>을 부여합니다";
        }
    }
}