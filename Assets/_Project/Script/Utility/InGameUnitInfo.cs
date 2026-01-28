using System.Collections.Generic;

namespace Core.Utility
{
    public static class InGameUnitInfo
    {
        public static List<Unit> PlayerUnits = new List<Unit>();
        public static List<Unit> EnemyUnits = new List<Unit>(); 
        public static List<Unit> AllUnits = new List<Unit>();
        
        public static float PlayerCurTurn = 0,PlayerMaxTurn = 0;
        public static float EnemyCurTurn = 0,EenemyMaxTurn = 0;

        public static void ResetCurTurn()
        {
            PlayerCurTurn = 0;
            EnemyCurTurn = 0;
        }

        public static void StoreUnits(List<Unit> playerUnits, List<Unit> enemyUnits)
        {
            PlayerUnits.Clear();
            EnemyUnits.Clear();
            AllUnits.Clear();
            
            PlayerUnits.AddRange(playerUnits);
            EnemyUnits.AddRange(enemyUnits);
            
            AllUnits.AddRange(playerUnits);
            AllUnits.AddRange(enemyUnits);
        }
        
        
    }
}