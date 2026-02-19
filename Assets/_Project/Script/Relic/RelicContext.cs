using System.Collections.Generic;
using GoogleSheet.Core.Type;

namespace _Project.Script.Relic
{
    /// <summary>
    /// 유물 효과가 발동되는 시점 정의
    /// </summary>
    [UGS(typeof(RelicTriggerType))]
    public enum RelicTriggerType
    {
        BattleStart,        // 전투 시작
        TurnStart,          // 턴 시작
        TurnEnd,            // 턴 종료
        OnUnitDeath,        // 유닛 사망 시
        OnUnitDamaged,      // 유닛 피격 시
        OnUnitAttack,       // 유닛 공격 시
        OnUnitMove,         // 유닛 이동 시
        OnSkillUse,         // 스킬 사용 시
        OnEnemyKill,        // 적 처치 시
        Interval,           // 일정 시간마다
        Conditional,        // 특정 조건 충족 시
        BattleEnd,          // 전투 종료
        Passive,            // 패시브 (항상 적용)
    }
    
    /// <summary>
    /// 유물 효과의 대상 정의
    /// </summary>
    public enum RelicTargetType
    {
        AllAllies,          // 모든 아군
        RandomAlly,         // 랜덤 아군
        AllEnemies,         // 모든 적
        RandomEnemy,        // 랜덤 적
        RandomTile,         // 랜덤 타일
        AllTiles,           // 모든 타일
        Self,               // 자신
        NearestEnemy,       // 가장 가까운 적
        LowestHpAlly,       // HP가 가장 낮은 아군
        HighestHpEnemy,     // HP가 가장 높은 적
        Area,               // 범위
        Custom,             // 커스텀 (직접 지정)
    }
    /// <summary>
    /// 유물 효과 컨텍스트 - 효과 실행에 필요한 모든 정보를 담음
    /// </summary>
    public class RelicEffectContext
    {
        public Unit SourceUnit;
        public Unit TargetUnit { get; set; }       // 대상 유닛
        public Tile TargetTile { get; set; }      // 타겟 타일
        public float DamageAmount { get; set; }         // 데미지 양
        public int ComboCount { get; set; }             // 콤보 카운트
        public float ElapsedTime { get; set; }          // 경과 시간
        public Dictionary<string, object> CustomData { get; set; } = new();  // 커스텀 데이터
    }
}