using System;
using System.Collections.Generic;

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
        
        public static Action playerTurnValueHandle,playerCharmsValueHandle;

        public static void ResetData()
        {
            PlayerCurTurn = 0;
            PlayerMaxTurn = 0;
            EnemyCurTurn = 0;
            EnemyMaxTurn = 0;
            
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
            PlayerCurTurn = playerCurTurn;
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
              
            
            PlayersCharms = playerCharmValue;
            playerCharmsValueHandle?.Invoke();
        }
        
        
    }
}