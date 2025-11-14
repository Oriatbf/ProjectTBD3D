using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using VInspector;

public class FactoryManager : Singleton<FactoryManager>
{
     private SheetDataManager sheetDataManager;
     private TileManager tileManager;
     [SerializeField] private Unit playerUnitPrefab,enemyUnitPrefab;
     [Foldout("Testing")]
     public EnemyArrangeSO testSO;
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
     }

     public void RegisterDeadUnit(Unit _unit)
     {
          Team team = _unit.GetTeam();
          int targetId = _unit.GetUnitData().constId;
          if (team == Team.PlayerTeam)
          {
               foreach (var unit in playerUnits)
               {
                    if (unit.GetUnitData().constId == targetId)
                    {
                         playerUnits.Remove(unit);
                         break;
                    }
               }
          }
          else
          {
               foreach (var unit in enemyUnits)
               {
                    if (unit.GetUnitData().constId == targetId)
                    {
                         enemyUnits.Remove(unit);
                         break;
                    }
               }
               ApplicationManager.Inst.GetModule<EnemyRegisterController>().Add(enemyUnits);
          }

          if (enemyUnits.Count == 0)
          {
               ApplicationManager.Inst.GetModule<LootController>().InitEnemyArrange(testSO);
          }
     }

     /// <summary>
     /// //TODO **게임 시작 함수**
     /// </summary>
     public void GameStart()
     {
          ApplicationManager.Inst.GetModule<TurnController>().Add(playerUnits,Team.PlayerTeam);
          ApplicationManager.Inst.GetModule<TurnController>().Add(enemyUnits,Team.EnemyTeam);
          ApplicationManager.Inst.GetModule<EnemyRegisterController>().Add(enemyUnits);
          
          ApplicationManager.Inst.GetModule<TurnController>().TurnStart();
          
          
          foreach (var unit in playerUnits)unit.Initalize();
          foreach (var unit in enemyUnits) unit.Initalize();
     }

     private void Update()
     {
          if (Input.GetKeyDown(KeyCode.P))
          {
               TBDLogger.CommandLog(KeyCode.P, this);
               GameStart();
          }
     }

     /// <summary>
     /// id에 기반해 아군 유닛 소환
     /// </summary>
     public void PlayerSpawn(int id,Tile tile)
     {
          var _unit = CreateUnit(id,Team.PlayerTeam);
          playerUnits.Add(_unit);
          _unit.SetTile(tile);
          tile.SetUnit(_unit);
          _unit.transform.position = tile.GetPos();
         
     }

     /// <summary>
     /// id에 기반해 적군 유닛 소환
     /// </summary>
     private void EnemySpawn(EnemyArrangeSO so)
     {
          foreach (var enemyArrange in so.EnemyArranges)
          {
               var _unit = CreateUnit(enemyArrange.unitIndex,Team.EnemyTeam);
               enemyUnits.Add(_unit);
               var _tile = tileManager.GetEnemyTile(enemyArrange.posIndex);
               _unit.SetTile(_tile);
               _tile.SetUnit(_unit);
               _unit.transform.position = _tile.GetPos();
          }
     }

     /// <summary>
     /// id에 기반해 유닛 생성
     /// </summary>
     private Unit CreateUnit(int id,Team team)
     {
          var unitData = sheetDataManager.GetUnitData(id);
          if(unitData == null)Debug.LogError("ID에 해당하는 유닛이 없음");
          var prefab = team == Team.PlayerTeam ? playerUnitPrefab : enemyUnitPrefab;
          Unit unit = Instantiate(prefab);
          UnitSaveData saveData = new UnitSaveData()
          {
               id = unitData.Id,
               bringSkills = unitData.BringSkill,
               statContainer = new StatContainer(unitData)
          };
          unit.Init(saveData,unitData.AnimatorName,team);
          return unit;
     }
}
