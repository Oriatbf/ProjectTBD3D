using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class TBDLogger
{
    public static bool CommandLog<T>(KeyCode key, T t, string detail = null)
    {
        if (!DataManager.Inst.debugMode) return false;
        string fileInfo = GetScriptFileInfo<T>();

        Debug.Log($"{key} Command In {typeof(T).Name} Script\n{fileInfo}");

        if (detail != null)
            Debug.Log(detail);
        return true;
    }

    private static string GetScriptFileInfo<T>()
    {
#if UNITY_EDITOR
        // T 타입의 MonoScript를 찾는다
        var monoScripts = MonoImporter.GetAllRuntimeMonoScripts();
        foreach (var ms in monoScripts)
        {
            if (ms.GetClass() == typeof(T))
            {
                string path = AssetDatabase.GetAssetPath(ms);
                // 기본적으로 1번째 줄로 이동하게 설정
                return $"{path}:1";
            }
        }
#endif
        return "(Script location not found)";
    }
}