using _Project.Pooling;
using Core.Utility;
using UnityEngine;

namespace _Project.Script.Relic
{
    public class Thunder : RelicEffect
    {
        protected override void OnExecute(RelicEffectContext context)
        {
            var targetTile = ApplicationManager.Inst.GetModule<TileController>().GetRandomTile();
            var thunderEffect = ApplicationManager.Inst.GetModule<PoolController>()
                .Spawn<ThunderEffect>("ThunderEffect",targetTile.GetPos(),Quaternion.identity);
            
            if (targetTile.GetUnit() != null)
            {
                targetTile.GetUnit().GetDamage(values[0],null,SkillType.Attack);
            }
            Debug.Log("천둥번개");
        }

        public override string ReturnInformation()
        {
            return $"시작 시 랜덤 타일 한개에 " +
                   $"{ColorText.GetTextColor(TxtColorType.Intelligence)}{values[0]}</color>의 데미지를 입히는" +
                   $"뇌운을 시전합니다.";
        }
    }

    public class Cheating:IntervalRelicEffect
    {
        public Cheating() : base(10f)
        {
            maxTriggerCount = 5;
            targetType = RelicTargetType.RandomTile;
        }

        protected override void OnExecute(RelicEffectContext context)
        {
            var targetTile = ApplicationManager.Inst.GetModule<TileController>().GetRandomTile();
            var thunderEffect = ApplicationManager.Inst.GetModule<PoolController>()
                .Spawn<ThunderEffect>("ThunderEffect", targetTile.GetPos(), Quaternion.identity);

            if (targetTile.GetUnit() != null)
            {
                targetTile.GetUnit().GetDamage(values[0], null, SkillType.Attack);
            }
            Debug.Log("부정행위 실행");
        }

        public override string ReturnInformation()
        {
            return $"{interval}초마다 랜덤 타일에 " +
                   $"{ColorText.GetTextColor(TxtColorType.Intelligence)}{values[0]}</color> 데미지의 번개 낙하";
        }
    }
    
    public class Roulette:RelicEffect
    {
        public Roulette()
        {
            triggerType = RelicTriggerType.BattleStart;
            maxTriggerCount = 1;
        }
        protected override void OnExecute(RelicEffectContext context)
        {
            Tile targetTile = null;
            if (Random.value < 0.5f)
            {
                targetTile = InGameUnitInfo.PlayerUnits.NonDupRandomT(1)[0].GetTile();
            }
            else
            {
                targetTile = InGameUnitInfo.EnemyUnits.NonDupRandomT(1)[0].GetTile();
            }
            
            if (targetTile.GetUnit() != null)
            {
                targetTile.GetUnit().GetDamage(999,null,SkillType.Attack);
            }
            else Debug.LogError("유닛이 찾아지지않음");
            var thunderEffect = ApplicationManager.Inst.GetModule<PoolController>()
                .Spawn<ThunderEffect>("ThunderEffect",targetTile.GetPos(),Quaternion.identity);
            
            DataManager.Inst.GetRelicSaveData().DeleteRelic(2);
        }

        public override string ReturnInformation()
        {
            return $"시작 시 50%의 확률로 아군이 즉사하거나 적군이 즉사합니다. 1번 사용 후 파괴됩니다.";
        }
    }
}