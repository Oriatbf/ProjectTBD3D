using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Project.Resources.Loader
{
    [CreateAssetMenu(fileName = "CanvasLoader", menuName = "TBD/Loader/CanvasLoader")]
    public class CanvasLoader : ScriptableObject
    {
        [Serializable]
        public class CanvasInfo
        {
            public string Key;
            public GameObject Prefab;
            public string[] SceneName;
        }
  
        public List<CanvasInfo> CanvasDictionary = new List<CanvasInfo>();

        public GameObject GetCanvasPrefab(string key)
        {
            return CanvasDictionary.FirstOrDefault(x=>x.Key == key)?.Prefab;
        }

        public List<CanvasInfo> GetCanvasListForScene(string sceneName)
        {
            return CanvasDictionary.Where(x=>x.SceneName.Contains(sceneName)||x.SceneName.Contains("All")).ToList();
        }
    }
}
