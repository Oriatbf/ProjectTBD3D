using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VInspector;

public class EnemyController : UnitController
{
    protected override void Start()
    {
        base.Start();
    }

    [Button]
    public async UniTask  RegisterSKill()
    {
        Debug.Log("스킬 등록");
        var _skill = sheetDataManager.GetSkillBase(0);
        var clone = _skill.Clone();
        FindingTargetTile(clone);
        clone.InitSource(curTile);
        applicationManager.GetModule<SkillStackController>().StackSkill(clone,transform.position);
        applicationManager.GetModule<SkillTurnCounterController>().Enqueue(Team.EnemyTeam,clone);
        await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
    }

    private void FindingTargetTile(SkillData.SkillBase skill)
    {
        int rowCount = skill.GetData().RowCount;
        int columnCount = skill.GetData().ColumnCount;
        int mid = TileManager.Inst.GetHalfCount();
        int row = TileManager.Inst.GetRowCount();

        Tile maxTile = null;
        int maxCount = 0;
        for (int i = 0; i <= mid; i++)
        {
            for (int j = 0; j <row; j++)
            {
                Tile curTile = TileManager.Inst.GetTile(new Vector2(i, j));
                var list = TileManager.Inst.GetTiles(curTile,rowCount, columnCount);
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
        Debug.Log(maxTile.GetPos());
        skill.InitTarget(maxTile);
    }
}
