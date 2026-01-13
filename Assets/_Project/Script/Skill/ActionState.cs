using System;
using System.Collections.Generic;
using SkillData;
using UnityEngine;

/// <summary>
/// 감소 타입 (스택, 턴, 둘다, 무한)
/// </summary>
public enum DecreaseType
{
    None,           // 무한 (감소 없음)
    OnlyTurn,       // 턴만 감소
    OnlyStack,      // 스택만 감소
    TurnAndStack    // 턴과 스택 둘다 감소
}

/// <summary>
/// 타겟 타입 (단일, 랜덤, 적 전체, 아군 전체, 전체)
/// </summary>
public enum ActionTargetType
{
    Self,           // 자신
    Single,         // 단일 대상
    RandomEnemy,    // 랜덤 적 n명
    AllEnemy,       // 적 전체
    AllAlly,        // 아군 전체
    All,            // 전체
    Attacker        // 나를 공격한 대상
}

/// <summary>
/// 액션 실행 시점
/// </summary>
public enum ActionTrigger
{
    OnGet,
    OnTurnStart,    // 턴 시작 시
    OnTurnEnd,      // 턴 종료 시
    OnAttack,       // 공격 시
    OnHit,          // 피격 시
    OnHeal,         // 힐 받을 시
    OnDeath         // 사망 시
}


/// <summary>
/// ActionState의 핵심 데이터
/// </summary>
[Serializable]
public class ActionData
{
    public string id;                           // 고유 ID
    public int stack;                           // 스택 값
    public int turn;                            // 턴 수
    public bool isExist = true;                 // 작동 여부
    public DecreaseType decreaseType;           // 감소 타입
    public ActionTargetType targetType;         // 타겟 타입
    public int randomCount = 1;                 // 랜덤 타겟 개수 (RandomEnemy용)
    
    // 조건 필터
    public SkillType? triggerSkillType = null;  // 특정 스킬 타입만 트리거 (null이면 모두)
    public bool requireDamage = false;          // 데미지가 있어야 트리거
    
    public Unit ownerUnit;                      // 소유자 유닛
    public Tile ownerTile;                      // 소유자 타일
    public Unit sourceUnit;                     // 시전자 유닛 (이 액션을 건 사람)
    
    public Action<ActionData, SkillContext> action;         // 실행할 액션
    public Action<ActionData> finishAction;                 // 종료 시 액션

    public ActionData(string id, Unit owner, int stack = 1, int turn = 1, 
        DecreaseType decreaseType = DecreaseType.OnlyTurn, 
        ActionTargetType targetType = ActionTargetType.Single)
    {
        this.id = id;
        this.ownerUnit = owner;
        this.ownerTile = owner.GetTile();
        this.stack = stack;
        this.turn = turn;
        this.decreaseType = decreaseType;
        this.targetType = targetType;
    }

    /// <summary>
    /// 액션 데이터 복사 (중첩 시 사용)
    /// </summary>
    public void MergeWith(ActionData other)
    {
        // 턴은 더 긴 것으로
        turn = Mathf.Max(turn, other.turn);
        // 스택은 추가
        stack += other.stack;
    }
}

/// <summary>
/// 액션 상태 관리 클래스 (기존 BuffDebuff 대체)
/// </summary>
public class ActionState
{
    private ActionData data;

    public ActionState(ActionData data)
    {
        this.data = data;
    }

    public ActionData GetData() => data;
    public string GetId() => data.id;
    public bool IsExist() => data.isExist;

    /// <summary>
    /// 액션 실행 (트리거 발동 시)
    /// </summary>
    public void Execute(SkillContext skillContext = null)
    {
        if (!data.isExist) return;

        /*
        // 조건 체크
        if (!CheckConditions(skillContext))
            return;
            */

        // 타겟 결정 및 액션 실행
        ExecuteOnTargets(skillContext);

        // 감소 처리
        DecreaseValues();

        // 종료 조건 체크
        if (ShouldFinish())
        {
            Finish();
        }
    }

    /// <summary>
    /// 실행 조건 체크
    /// </summary>
    private bool CheckConditions(SkillContext skillContext)
    {
        /*
        // 스킬 타입 조건 체크
        if (data.triggerSkillType.HasValue && skillContext != null)
        {
            if (skillContext.skillType != data.triggerSkillType.Value)
                return false;
        }

        // 데미지 조건 체크
        if (data.requireDamage && skillContext != null)
        {
            if (skillContext.lastDamage <= 0)
                return false;
        }
*/
        return true;
    }

    /// <summary>
    /// 타겟에게 액션 실행
    /// </summary>
    private void ExecuteOnTargets(SkillContext skillContext)
    {
        if (data.action == null) return;

        List<Unit> targets = GetTargets(skillContext);
        
        foreach (var target in targets)
        {
            // 각 타겟에 대해 임시 컨텍스트 생성
            SkillContext tempContext = skillContext ?? new SkillContext();
            if (tempContext.SourceUnit == null)
                tempContext.SourceUnit = data.sourceUnit ?? data.ownerUnit;
            
            tempContext.TargetUnit = target;
            tempContext.TargetTile = target?.GetTile();

            data.action.Invoke(data, tempContext);
        }
    }

    /// <summary>
    /// 타겟 리스트 가져오기
    /// </summary>
    private List<Unit> GetTargets(SkillContext skillContext)
    {
        List<Unit> targets = new List<Unit>();
        Team ownerTeam = data.ownerUnit.GetTeam();

        switch (data.targetType)
        {
            case ActionTargetType.Self:
                targets.Add(data.ownerUnit);
                break;

            case ActionTargetType.Single:
                if (skillContext?.TargetUnit != null)
                    targets.Add(skillContext.TargetUnit);
                else
                    targets.Add(data.ownerUnit);
                break;

            case ActionTargetType.Attacker:
                // 공격자 (skillContext에서 SourceUnit)
                if (skillContext?.SourceUnit != null)
                    targets.Add(skillContext.SourceUnit);
                break;

            case ActionTargetType.AllEnemy:
                targets.AddRange(GetAllUnitsOfTeam(GetEnemyTeam(ownerTeam)));
                break;

            case ActionTargetType.AllAlly:
                targets.AddRange(GetAllUnitsOfTeam(ownerTeam));
                break;

            case ActionTargetType.All:
                targets.AddRange(GetAllUnits());
                break;

            case ActionTargetType.RandomEnemy:
                targets.AddRange(GetRandomEnemies(ownerTeam, data.randomCount));
                break;
        }

        return targets;
    }

    /// <summary>
    /// 스택/턴 감소
    /// </summary>
    private void DecreaseValues()
    {
        switch (data.decreaseType)
        {
            case DecreaseType.OnlyStack:
                data.stack--;
                break;
            case DecreaseType.OnlyTurn:
                data.turn--;
                break;
            case DecreaseType.TurnAndStack:
                data.stack--;
                data.turn--;
                break;
        }
    }

    /// <summary>
    /// 종료 조건 체크
    /// </summary>
    private bool ShouldFinish()
    {
        switch (data.decreaseType)
        {
            case DecreaseType.OnlyStack:
                return data.stack <= 0;
            case DecreaseType.OnlyTurn:
                return data.turn <= 0;
            case DecreaseType.TurnAndStack:
                return data.stack <= 0 || data.turn <= 0;
            case DecreaseType.None:
                return false;
        }
        return false;
    }

    /// <summary>
    /// 액션 상태 종료
    /// </summary>
    public void Finish()
    {
        data.isExist = false;
        data.stack = 0;
        data.turn = 0;
        data.finishAction?.Invoke(data);
    }

    // ===== 헬퍼 메서드들 =====

    private Team GetEnemyTeam(Team team)
    {
        return team == Team.PlayerTeam ? Team.EnemyTeam : Team.PlayerTeam;
    }

    private List<Unit> GetAllUnitsOfTeam(Team team)
    {
        // 실제 구현은 게임의 유닛 관리 시스템에 따라 다름
        // 예시: TileManager를 통해 모든 타일의 유닛 검색
        List<Unit> units = new List<Unit>();
        // TODO: 실제 유닛 검색 로직
        return units;
    }

    private List<Unit> GetAllUnits()
    {
        List<Unit> units = new List<Unit>();
        // TODO: 모든 유닛 검색 로직
        return units;
    }

    private List<Unit> GetRandomEnemies(Team ownerTeam, int count)
    {
        List<Unit> enemies = GetAllUnitsOfTeam(GetEnemyTeam(ownerTeam));
        List<Unit> randomEnemies = new List<Unit>();
        
        int actualCount = Mathf.Min(count, enemies.Count);
        for (int i = 0; i < actualCount; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, enemies.Count);
            randomEnemies.Add(enemies[randomIndex]);
            enemies.RemoveAt(randomIndex);
        }
        
        return randomEnemies;
    }
}

