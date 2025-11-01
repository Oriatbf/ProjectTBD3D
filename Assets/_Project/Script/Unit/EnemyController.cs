using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
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
        var _skill = sheetDataManager.GetSkillBase(1); 
        applicationManager.GetModule<SkillTurnCounterController>().Enqueue(Team.EnemyTeam,_skill.GetData().Name);
        await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
    }
}
