using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class EnemyRegisterController : BaseController
{
    private List<EnemyController> enemies = new List<EnemyController>();

    public void Add(List<Unit> units)
    {
        enemies.Clear();
        foreach (var unit in units)
        {
            if (unit.TryGetComponent(out EnemyController enemyController))
            {
                enemies.Add(enemyController);
            }
        }
    }

    public async UniTask Register()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            await enemies[i].RegisterSKill();
        }
    }
}
