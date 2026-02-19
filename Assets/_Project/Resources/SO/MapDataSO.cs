using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Resources.SO
{
    [CreateAssetMenu(fileName = "MapDataSO",menuName = "MapDataSO",order = 1)]
    public class MapDataSO : ScriptableObject
    {
        [Serializable]
        public struct  MapRatio
        {
            public NodeType nodeType;
            public float ratio;
        }
        [Serializable]
        public struct MapData
        {
            public List<MapRatio> mapRatios;
        }
     
        
        public int maxFloor;
        public List<MapData> mapDatas;
    }
    
}