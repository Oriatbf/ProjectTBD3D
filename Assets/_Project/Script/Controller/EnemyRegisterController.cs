using System.Collections.Generic;
using Core.Utility;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class EnemyRegisterController : BaseController
{
    private List<EnemyController> enemies = new List<EnemyController>();

    public void SetEnemyUnits()
    {
        var units = InGameUnitInfo.EnemyUnits;
        enemies.Clear();
        for(int i = 0;i<units.Count;i++)
        {
            if (units[i] != null&&units[i].TryGetComponent(out EnemyController enemyController))
            {
                enemies.Add(enemyController);
            }
        }
    }

    public async UniTask Register()
    {
        SetEnemyUnits();
        for (int i = 0; i < enemies.Count; i++)
        {
            await enemies[i].RegisterSKill();
        }
    }
}
