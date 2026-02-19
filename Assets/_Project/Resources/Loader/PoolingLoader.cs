using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Resources.Loader
{
    public enum PoolType
    {
        UI,GameObject
    }
    
    [CreateAssetMenu(fileName = "PoolingLoader", menuName = "PoolingLoader")]
    public class PoolingLoader : ScriptableObject
    {
        [Serializable]
        public class PoolInfo
        {
            [Tooltip("ScriptName")]public string Key;
            public GameObject Prefab;
            public PoolType poolType;
            public int canvasOrder = 0;
        }
  
        public List<PoolInfo> PoolList = new List<PoolInfo>();
        
    }
}
