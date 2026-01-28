using System.Collections.Generic;
using System.Linq;
using _Project.Script.Controller;
using Core.Utility;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VInspector;

public class FactoryManager : Singleton<FactoryManager>
{
     [SerializeField] private Unit playerUnitPrefab,enemyUnitPrefab;
     
     [Foldout("Debugging")]
     [SerializeField] List<Unit> playerUnits = new List<Unit>();
     [SerializeField] List<Unit> enemyUnits = new List<Unit>(); 
     private List<EnemyArrangeSO> enemyArrangeSOs = new List<EnemyArrangeSO>();
     [EndFoldout] 
     private EnemyArrangeSO curEnemyArrange;
     
     private readonly string SOLabel = "EnemySO";
     
     

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
               ApplicationManager.Inst.GetModule<LootController>().InitEnemyArrange(curEnemyArrange);
               ApplicationManager.Inst.GetModule<TurnController>().MapStage();
               ApplicationManager.Inst.GetModule<ActionStateStackController>().ResetAllBuffs();
               ApplicationManager.Inst.GetModule<SkillProgressController>().Reset();
               foreach (var unit in playerUnits)
               {
                    DataManager.Inst.SaveUnit(unit);
               }
          }
     }

     /// <summary>
     /// //TODO **게임 시작 함수**
     /// </summary>
     public void GameStart()
     {
          InGameUnitInfo.StoreUnits(playerUnits,enemyUnits);
          TurnInit();
          
          ApplicationManager.Inst.GetModule<TurnController>().Add(playerUnits,Team.PlayerTeam);
          ApplicationManager.Inst.GetModule<TurnController>().Add(enemyUnits,Team.EnemyTeam);
          ApplicationManager.Inst.GetModule<EnemyRegisterController>().Add(enemyUnits);
          ApplicationManager.Inst.GetModule<RelicController>().ExcuteAllRelic();
          
          
          ApplicationManager.Inst.GetModule<TurnController>().TurnStart();
          
          
          foreach (var unit in playerUnits)unit.Initalize();
          foreach (var unit in enemyUnits) unit.Initalize();
     }

     public void TurnInit()
     {
          float playerMaxTurn = 0,enemyMaxTurn = 0;
          foreach (var unit in playerUnits)
          {
               playerMaxTurn += unit.GetStatContainer().turnGauge._maxValue;
          }

          foreach (var unit in enemyUnits)
          {
               enemyMaxTurn += unit.GetStatContainer().turnGauge._maxValue;
          }
          
          InGameUnitInfo.PlayerMaxTurn = playerMaxTurn;
          InGameUnitInfo.EenemyMaxTurn = enemyMaxTurn;
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
          enemyArrangeSOs = Resources.LoadAll<EnemyArrangeSO>("SO/EnemyArrange")
               .Where(s => s.enemyArrangeType == enemyArrangeType).ToList();
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
          InGameUnitInfo.StoreUnits(playerUnits,enemyUnits);
     }

     /// <summary>
     /// id에 기반해 유닛 생성
     /// </summary>
     public Unit CreateUnit(int id,Team team)
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
}

