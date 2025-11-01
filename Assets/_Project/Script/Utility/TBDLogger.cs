using UnityEngine;

public static class TBDLogger
{
   public static void CommandLog<T>(KeyCode key, T t,string detail = null)
   {
      Debug.Log($"{key} Command In {t} Script");
      if(detail != null)Debug.Log($"{detail}");
   }
}
