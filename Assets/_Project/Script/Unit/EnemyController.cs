using System;
using System.Collections.Generic;
using _Project.Script.Controller;
using Core.Utility;
using Cysharp.Threading.Tasks;
using UnitData;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Random = UnityEngine.Random;

public class EnemyController : UnitController
{
    private float maxTurn = 0;
    float curTurn = 0;
    TileController _tileController;

    protected override void Awake()
    {
        base.Awake();
        _tileController = ApplicationManager.Inst.GetModule<TileController>();
        
    }

    /// <summary>
    /// 스킬 등록
    /// </summary>
    public async UniTask  RegisterSKill()
    {
        if (_unit.GetStatContainer().isStun) return;
        maxTurn = _unit.GetStatContainer().turnGauge._maxValue;
        var _skillStackInfoList = FindingSkill();
        foreach (var skillStackInfo in _skillStackInfoList)
        {
            var skill = skillStackInfo.skill;
            skill.InitSource(curTile);
            FindingTargetTile(skillStackInfo.skill);
            ApplicationManager.Inst.GetModule<SkillProgressController>().Stack(skillStackInfo);
            await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
        }
        
        curTurn = 0;
        await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
    }

    private  List<SkillStackInfo>  FindingSkill()
    {
        List<SkillStackInfo> resultSkillStackInfo = new List<SkillStackInfo>();
        if(SheetDataManager.Inst == null ||_unit.GetSkillList()==null )Debug.LogError("no");
        var skillList = SheetDataManager.Inst.GetSkillBaseList(_unit.GetSkillList());
        float minReqTurn = skillList[0].GetData().RequireTurn;
        //최소 요구 턴 찾기
        foreach (var skillBase in skillList)
            if(minReqTurn>skillBase.GetData().RequireTurn)minReqTurn = skillBase.GetData().RequireTurn;
        //남은 턴이 최소 요구 턴보다 많다면 실행
        while (maxTurn - curTurn >= minReqTurn)
        {
            int rand = Random.Range(0, skillList.Count);
            var targetSkill = skillList[rand];
            if (targetSkill.GetData().RequireTurn <= maxTurn - curTurn)
            {
               //curTrun은 해당 적이 maxTurn까지만 턴을 사용하게 하기 위한 요소
                curTurn += targetSkill.GetData().RequireTurn; 
                var realTurn =   InGameUnitInfo.EnemyCurTurn += targetSkill.GetData().RequireTurn;
               
                SkillStackInfo _skillStackInfo = new SkillStackInfo()
                {
                    skill = targetSkill.Clone(),
                    sourceTile = curTile,
                    stackTurn = realTurn,
                    team = Team.EnemyTeam
                };
                resultSkillStackInfo.Add(_skillStackInfo);
            }
        }
        return resultSkillStackInfo;
    }

    /// <summary>
    /// 최적의 타일 찾기
    /// </summary>
    private void FindingTargetTile(SkillData.SkillBase skill)
    {
        int rowCount = skill.GetData().RowCount;
        int columnCount = skill.GetData().ColumnCount;
        Tile resultTile = curTile;

        if (skill.GetData().TargetType == TargetType.Area)
        {
            switch (skill.GetData().SkillType)
            {
                case SkillType.Attack:
                    resultTile=TargetEnemyUnit(rowCount, columnCount);
                    break;
                case SkillType.Utility:
                    resultTile=TargetFriendlyUnit(rowCount, columnCount);
                    break;
                case SkillType.Buff:
                    resultTile=TargetFriendlyUnit(rowCount, columnCount);
                    break;
                case SkillType.Debuff:
                    resultTile=TargetEnemyUnit(rowCount, columnCount);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }      
        }
      
        skill.InitTarget(resultTile);
    }

    private Tile TargetEnemyUnit(int row, int column)
    {
        //가장 많은 적 유닛이 존재하는 곳들을 모아서 저장
        List<Tile> targetTiles = new List<Tile>();
        int maxCount = 0;
        var playerTiles = _tileController.GetAllTeamTiles(Team.PlayerTeam);
        for (int i = 0; i < playerTiles.Count; i++)
        {
            var list = _tileController.GetTiles(playerTiles[i],row, column);
            int unitCnt = 0;
            if (list.Count > 0)
            {
                foreach (var tile in list)
                    if(tile.GetUnit()?.GetTeam() == Team.PlayerTeam)unitCnt+=1;
            }

            if (unitCnt > maxCount)
            {
                targetTiles = new List<Tile>();
                targetTiles.Add(playerTiles[i]);
                maxCount = unitCnt;
            }
            else if (unitCnt == maxCount)
            {
                targetTiles.Add(playerTiles[i]);
            }
        }
        
        var random = Random.Range(0, targetTiles.Count);
        var finalTile = targetTiles[random];
        return finalTile;
    }

    private Tile TargetFriendlyUnit(int row, int column)
    {
        //가장 많은 적 유닛과 손실된 피를 저장
        List<(Tile,int)> targetTiles = new  List<(Tile,int)>() ;
        int maxScore = 0;
        var enemyTiles = _tileController.GetAllTeamTiles(Team.EnemyTeam);
        for (int i = 0; i < enemyTiles.Count; i++)
        {
            var list = _tileController.GetTiles(enemyTiles[i],row, column);
            int unitCnt = 0;
            int lostHp = 0;
            if (list.Count > 0)
            {
                foreach (var tile in list)
                {
                    if (tile.GetUnit()?.GetTeam() == Team.EnemyTeam)
                    {
                        unitCnt+=1;
                        var hpStat = tile.GetUnit().GetStatContainer().hp;
                        var lost = hpStat._maxValue - hpStat._baseValue;
                        lostHp += (int)lost;
                    }
                }
            }

            var score = lostHp + (unitCnt*3);

            if (score > maxScore)
            {
                targetTiles = new  List<(Tile,int)>() ;
                targetTiles.Add((enemyTiles[i], score));
                maxScore = score;
            }
            else if (score == maxScore)
            {
                targetTiles.Add((enemyTiles[i],score));
            }
        }
        if(targetTiles.Count == 0)Debug.LogError("No TargetTiles");
        var random = Random.Range(0, targetTiles.Count);
        var finalTile = targetTiles[random].Item1;
        return finalTile;
    }
}
