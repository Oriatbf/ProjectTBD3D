using System;
using UnityEngine;

namespace Map
{
    public class StagePoint : MonoBehaviour
    {
        private MapState mapState;
        private StageContent stageContent;
        private int index = 0;
        [SerializeField] private Transform targetMark;
            
        private void Start()
        {
            stageContent=PoolManager.Inst.Spawn<StageContent>();
            stageContent.Init(mapState);
            stageContent.SetPos(transform.position);
        }

        public void Init(MapState mapState,int index)
        {
            this.index = index;
            this.mapState = mapState;
        }
        
        public int GetIndex()=>index;
        public MapState GetMapState()=>mapState;

        public void Targeting()
        {
            targetMark.gameObject.SetActive(true);
        }
        
        public void UnTargeting()
        {
            targetMark.gameObject.SetActive(false);
        }
    }
}