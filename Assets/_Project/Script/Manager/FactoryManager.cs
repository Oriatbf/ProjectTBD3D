using System;
using DG.Tweening;
using UnityEngine;
using VInspector;

public class FactoryManager : MonoBehaviour
{
     private SheetDataManager sheetDataManager;
     private TileManager tileManager;
     [SerializeField] private Unit unit;
     [Foldout("Testing")]
     public EnemyArrangeSO testSO;
     public int playerIndex;
     [EndFoldout]
    

     private void Awake()
     {
          DIContainer.RegisterService(this);
     }

     private void Start()
     {
          sheetDataManager=DIContainer.ResolveService<SheetDataManager>();
          tileManager=DIContainer.ResolveService<TileManager>();
          EnemySpawn(testSO);
          PlayerSpawn(0);
     }

     public void PlayerSpawn(int id)
     {
          var _unit = CreateUnit(id);
          var _tile = tileManager.GetTile(new Vector2(2, 0));
          _unit.transform.position = _tile.GetPos();
     }

     public void EnemySpawn(EnemyArrangeSO so)
     {
          foreach (var enemyArrange in so.EnemyArranges)
          {
               var _unit = CreateUnit(enemyArrange.unitIndex);
               var _tile = tileManager.GetEnemyTile(enemyArrange.posIndex);
               _unit.transform.position = _tile.GetPos();
          }
     }

     private Unit CreateUnit(int id)
     {
          var unitData = sheetDataManager.GetUnitData(id);
          if(unitData == null)Debug.LogError("ID에 해당하는 유닛이 없음");
          Unit unit = Instantiate(this.unit);
          unit.Init(unitData,Team.EnemyTeam);
          return unit;
     }
}
