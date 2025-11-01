using System;
using System.Collections.Generic;
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
     
     [Foldout("Debugging")]
     [SerializeField] List<Unit> playerUnits = new List<Unit>();
     [SerializeField] List<Unit> enemyUnits = new List<Unit>();
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
          ApplicationManager.Inst.GetModule<TurnController>().Add(playerUnits,Team.PlayerTeam);
          ApplicationManager.Inst.GetModule<TurnController>().Add(enemyUnits,Team.EnemyTeam);
          ApplicationManager.Inst.GetModule<EnemyRegisterController>().Add(enemyUnits);
     }

     /// <summary>
     /// id에 기반해 아군 유닛 소환
     /// </summary>
     public void PlayerSpawn(int id)
     {
          var _unit = CreateUnit(id);
          playerUnits.Add(_unit);
          var _tile = tileManager.GetTile(new Vector2(2, 0)); //임시 배치
          _unit.transform.position = _tile.GetPos();
     }

     /// <summary>
     /// id에 기반해 적군 유닛 소환
     /// </summary>
     public void EnemySpawn(EnemyArrangeSO so)
     {
          foreach (var enemyArrange in so.EnemyArranges)
          {
               var _unit = CreateUnit(enemyArrange.unitIndex);
               enemyUnits.Add(_unit);
               var _tile = tileManager.GetEnemyTile(enemyArrange.posIndex);
               _unit.transform.position = _tile.GetPos();
          }
     }

     /// <summary>
     /// id에 기반해 유닛 생성
     /// </summary>
     private Unit CreateUnit(int id)
     {
          var unitData = sheetDataManager.GetUnitData(id);
          if(unitData == null)Debug.LogError("ID에 해당하는 유닛이 없음");
          Unit unit = Instantiate(this.unit);
          unit.Init(unitData,Team.EnemyTeam);
          return unit;
     }
}
