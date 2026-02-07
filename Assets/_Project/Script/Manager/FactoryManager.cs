using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Script.Controller;
using Core.Utility;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VInspector;
using Random = UnityEngine.Random;

public class FactoryManager : Singleton<FactoryManager>
{
     [SerializeField] private Unit playerUnitPrefab,enemyUnitPrefab;
     
     [Foldout("Debugging")]
     [SerializeField] List<Unit> playerUnits = new List<Unit>();
     [SerializeField] List<Unit> enemyUnits = new List<Unit>(); 
     private List<EnemyArrangeSO> enemyArrangeSOs = new List<EnemyArrangeSO>();
     [EndFoldout] 
     private EnemyArrangeSO curEnemyArrange;
     private LoseCanvas _loseCanvas;
     private VictoryCanvas _victoryCanvas;
     private bool isGameStart = false;

     private void Awake()
     {
          InGameUnitInfo.ResetData();
     }

     private void Start()
     {
          _loseCanvas = ApplicationManager.Inst.GetModule<CanvasController>().GetCanvas<LoseCanvas>("LoseCanvas");
          _victoryCanvas = ApplicationManager.Inst.GetModule<CanvasController>().GetCanvas<VictoryCanvas>("VictoryCanvas");
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

          if (playerUnits.Count <= 0)
          {
               Lose();
               return;
          }

          if (enemyUnits.Count == 0)
          {
               ApplicationManager.Inst.GetModule<LootController>().InitEnemyArrange(curEnemyArrange);
               ApplicationManager.Inst.GetModule<TurnController>().MapStage();
               ApplicationManager.Inst.GetModule<ActionStateStackController>().ResetAllBuffs();
               ApplicationManager.Inst.GetModule<SkillProgressController>().Reset();

               if (ApplicationManager.Inst.GetModule<GameFlowController>().GetCurNodeType() == NodeType.Tutorial) return;
               //전투가 끝나고 남은 유닛들 세이브
               foreach (var unit in playerUnits)
               {
                    DataManager.Inst.SaveSurviveUnit(unit);
               }

               if (DataManager.Inst.GetMapData().curNodeCoord.type == NodeType.Boss)
               {
                    _victoryCanvas.ChangeState(true,true,true);
               }
          }

          UnitAddtionHandle();
     }

     public void Lose()
     {
          _loseCanvas.ChangeState(true,true,true);
     }

     /// <summary>
     /// //TODO **게임 시작 함수**
     /// </summary>
     public async UniTask GameStart()
     {
          UnitAddtionHandle();
          TurnInit();
          
          ApplicationManager.Inst.GetModule<TurnController>().Add(playerUnits,Team.PlayerTeam);
          ApplicationManager.Inst.GetModule<TurnController>().Add(enemyUnits,Team.EnemyTeam);
          ApplicationManager.Inst.GetModule<EnemyRegisterController>().Add(enemyUnits);
          
          ApplicationManager.Inst.GetModule<TurnController>().TurnStart();
          
          
          foreach (var unit in playerUnits)unit.Initalize();
          foreach (var unit in enemyUnits) unit.Initalize();
          
          await ApplicationManager.Inst.GetModule<RelicController>().ExcuteAllRelic();
          isGameStart = true;
          
         
     }

     public void TurnInit()
     {
          float playerMaxTurn = 0,enemyMaxTurn = 0;
          foreach (var unit in playerUnits)
          {
               if(!unit.GetStatContainer().isStun)
                    playerMaxTurn += unit.GetStatContainer().turnGauge._maxValue;
          }

          foreach (var unit in enemyUnits)
          {
               if(!unit.GetStatContainer().isStun)
                    enemyMaxTurn += unit.GetStatContainer().turnGauge._maxValue;
          }
          
          InGameUnitInfo.SetPlayerMaxTurn(playerMaxTurn); 
          InGameUnitInfo.EnemyMaxTurn = enemyMaxTurn;
          InGameUnitInfo.ResetCurTurn();
     }
     
     
     
     /// <summary>
     /// id에 기반해 아군 유닛 소환
     /// </summary>
     public void PlayerSpawn(UnitSaveData unitData,Tile tile)
     {
          var _unit = CreateDataBasedUnit(unitData,Team.PlayerTeam);
          playerUnits.Add(_unit);
          _unit.SetTile(tile);
          tile.SetUnit(_unit);
          _unit.transform.position = tile.GetPos();
         
     }

     /// <summary>
     /// id에 기반해 적군 유닛 소환
     /// </summary>
     public void EnemySpawn(EnemyArrangeType enemyArrangeType)
     {
          var curFloor = DataManager.Inst.GetMapData().curNodeCoord.floor;
          int diffculty = 0;
          switch (curFloor)
          {
               case <=4:
                    diffculty = 0;
                    break;
               case <=9:
                    diffculty = 1;
                    break;
               case >= 10:
                    diffculty = 2;
                    break;
          }

          Debug.Log(enemyArrangeType);
          if (enemyArrangeType == EnemyArrangeType.enemy)
          {
               enemyArrangeSOs = Resources.LoadAll<EnemyArrangeSO>("SO/EnemyArrange")
                    .Where(s => s.enemyArrangeType == enemyArrangeType && s.appearAct == 0 && s.difficulty ==diffculty).ToList();
          }
          else
          {
               enemyArrangeSOs = Resources.LoadAll<EnemyArrangeSO>("SO/EnemyArrange")
                    .Where(s => s.enemyArrangeType == enemyArrangeType).ToList();
          }
         
          var random = Random.Range(0,enemyArrangeSOs.Count);
          curEnemyArrange = enemyArrangeSOs[random];
          var so = curEnemyArrange;
          foreach (var enemyArrange in so.EnemyArranges)
          {
               var _unit = CreateUnit(enemyArrange.unitIndex,Team.EnemyTeam);
               enemyUnits.Add(_unit);
               var _tile = ApplicationManager.Inst.GetModule<TileController>().GetEnemyTile(enemyArrange.posIndex);
               _unit.SetTile(_tile);
               _tile.SetUnit(_unit);
               _unit.transform.position = _tile.GetPos();
          }
     }

     public void UnitSpawnRandom(int id, Team team)
     {
          var _unit = CreateUnit(id,team);
          var randomTile = ApplicationManager.Inst.GetModule<TileController>().GetRandomTile(team);
          while (randomTile.GetUnit())
          {
               randomTile = ApplicationManager.Inst.GetModule<TileController>().GetRandomTile(team);
          }
          _unit.SetTile(randomTile);
          randomTile.SetUnit(_unit);
          _unit.transform.position = randomTile.GetPos();
          _unit.Initalize();
          if(team == Team.PlayerTeam)playerUnits.Add(_unit);
          else enemyUnits.Add(_unit);
          UnitAddtionHandle();
     }

     /// <summary>
     /// id에 기반해 유닛 생성
     /// </summary>
     private Unit CreateUnit(int id,Team team)
     {
          var unitData = SheetDataManager.Inst.GetUnitData(id);
          if(unitData == null)Debug.LogError("ID에 해당하는 유닛이 없음");
          var prefab = team == Team.PlayerTeam ? playerUnitPrefab : enemyUnitPrefab;
          Unit unit = Instantiate(prefab);
          UnitSaveData saveData = new UnitSaveData(unitData);
          unit.Init(saveData,unitData.AnimatorName,team);
          return unit;
     }
     
     /// <summary>
     /// 이미 데이터가 존재하는 유닛 생성
     /// </summary>
     private Unit CreateDataBasedUnit(UnitSaveData unitData,Team team)
     {
          var prefab = team == Team.PlayerTeam ? playerUnitPrefab : enemyUnitPrefab;
          Unit unit = Instantiate(prefab);
          var animatorName = SheetDataManager.Inst.GetUnitData(unitData.id).AnimatorName;
          unit.Init(unitData,animatorName,team);
          return unit;
     }
     
     public List<Unit> GetPlayerUnits()=>playerUnits;
     public bool IsGameStart() => isGameStart;

     private void UnitAddtionHandle()
     {
          InGameUnitInfo.StoreUnits(playerUnits,enemyUnits);
     }
}

