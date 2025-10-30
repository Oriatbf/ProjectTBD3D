using UnityEngine;
using VInspector;

public class EnemyController : CharacterController
{
    protected override void Start()
    {
        base.Start();
    }

    [Button]
    public void RegisterSKill(int id)
    {
        var _skill = sheetDataManager.GetSkillBase(id); 
        applicationManager.GetModule<SkillTurnCounterController>().Enqueue(Team.EnemyTeam);
        
    }
}
