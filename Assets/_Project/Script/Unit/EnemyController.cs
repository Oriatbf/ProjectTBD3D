using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnitData;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Random = UnityEngine.Random;

public class EnemyController : UnitController
{
    private float maxTurn = .5f;
    float curTurn = 0;
    
    /// <summary>
    /// 스킬 등록
    /// </summary>
    public async UniTask  RegisterSKill()
    {
        Debug.Log("RegisterSKill");
        var _skillStackInfoList = FindingSkill();
        foreach (var skillStackInfo in _skillStackInfoList)
        {
            var skill = skillStackInfo.skill;
            skill.InitSource(curTile);
            FindingTargetTile(skillStackInfo.skill);
            ApplicationManager.Inst.GetModule<SkillStackController>().StackSkill(skillStackInfo);
            ApplicationManager.Inst.GetModule<SkillTurnCounterController>().Enqueue(skillStackInfo);
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
                curTurn += targetSkill.GetData().RequireTurn;  
                SkillStackInfo _skillStackInfo = new SkillStackInfo()
                {
                    skill = targetSkill.Clone(),
                    sourceTile = curTile,
                    stackTurn = curTurn,
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
        var tileController = ApplicationManager.Inst.GetModule<TileController>();
        int rowCount = skill.GetData().RowCount;
        int columnCount = skill.GetData().ColumnCount;
        int mid = tileController.GetHalfCount();
        int row = tileController.GetRowCount();

        Tile maxTile = null;
        int maxCount = 0;
        for (int i = 0; i <= mid; i++)
        {
            for (int j = 0; j <row; j++)
            {
                Tile curTile = tileController.GetTile(new Vector2(i, j));
                var list = tileController.GetTiles(curTile,rowCount, columnCount);
                int playerCnt = 0;
                if (list.Count > 0)
                {
                    foreach (var tile in list)
                        if(tile.GetUnit()?.GetTeam() == Team.PlayerTeam)playerCnt+=1;
                }

                if (playerCnt > maxCount)
                {
                    maxCount = playerCnt;
                    maxTile = curTile;
                }
            }
        }
        if(maxTile == null)Debug.LogError("maxTile이 없음");
        skill.InitTarget(maxTile);
    }
}
