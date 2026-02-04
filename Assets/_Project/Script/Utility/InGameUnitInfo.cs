using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Utility
{
    public static class InGameUnitInfo
    {
        public static List<Unit> PlayerUnits = new List<Unit>();
        public static List<Unit> EnemyUnits = new List<Unit>(); 
        public static List<Unit> AllUnits = new List<Unit>();
        
        public static float PlayerCurTurn = 0,PlayerMaxTurn = 0;
        public static float EnemyCurTurn = 0,EnemyMaxTurn = 0;
        public static int PlayersCharms = 0;
        private static Stat playerCharms = new Stat();
        
        
        public static Action playerTurnValueHandle,playerCharmsValueHandle;

        public static void ResetData()
        {
            PlayerCurTurn = 0;
            PlayerMaxTurn = 0;
            EnemyCurTurn = 0;
            EnemyMaxTurn = 0;
            
            playerCharms = Stat.Create(0);
            
            PlayerUnits.Clear();
            AllUnits.Clear();
            EnemyUnits.Clear();
            
            playerTurnValueHandle = null;
            playerCharmsValueHandle = null;
        }

        public static void ResetCurTurn()
        {
            PlayerCurTurn = 0;
            EnemyCurTurn = 0;
            PlayerTurnValueHandle();
        }

        
        public static void SetPlayerMaxTurn(float playerMaxTurn)
        {
            PlayerMaxTurn = playerMaxTurn;
            PlayerTurnValueHandle();
        }
        public static void SetPlayerCurTurn(float playerCurTurn)
        {
            PlayerCurTurn = Mathf.Round(playerCurTurn * 10f) / 10f;
            Debug.Log(PlayerCurTurn);
            PlayerTurnValueHandle();
        }

        private static void PlayerTurnValueHandle()
        {
            playerTurnValueHandle?.Invoke();
        }
        
        public static int GetPlayersCharms()=> PlayersCharms;

        public static void StoreUnits(List<Unit> playerUnits, List<Unit> enemyUnits)
        {
            PlayerUnits.Clear();
            EnemyUnits.Clear();
            AllUnits.Clear();
            
            PlayerUnits.AddRange(playerUnits);
            EnemyUnits.AddRange(enemyUnits);
            
            AllUnits.AddRange(playerUnits);
            AllUnits.AddRange(enemyUnits);

            PlayersCharms = 0;
            int playerCharmValue = 0;
            foreach (var unit in playerUnits)
            {
                playerCharmValue+=(int)unit.GetUnitData().charm;
            }
              
            playerCharms.SetBaseValue(playerCharmValue);
            if(playerCharms == null)Debug.LogError("No Charm");
            PlayersCharms = (int)playerCharms.FinalValue();
            playerCharmsValueHandle?.Invoke();
        }

        public static void AddCharm(int value)
        {
            playerCharms.AddModifier(new StatModifier(EStatModifier.Add,value));
            PlayersCharms = (int)playerCharms.FinalValue();
            playerCharmsValueHandle?.Invoke();
        }
        
        
    }
}