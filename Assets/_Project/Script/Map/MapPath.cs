using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using VInspector;

namespace Map
{
    

    public class MapPath : MonoBehaviour
    {


        [Foldout("Points")] [Tooltip("mainPoint들의 부모")] [SerializeField]
        private Transform pathParent;

        [SerializeField] private StagePoint stagePoint;
        [SerializeField] private float startPosInterval; // 초기 위치 인터벌 -> mainPos[0] - interval
        [SerializeField] private int tileCount;
        private List<Transform> mainPos = new List<Transform>();
        [SerializeField]private List<StagePoint> stagePoints = new List<StagePoint>();
    
        [EndFoldout] [Foldout("Move")] 
        
        [SerializeField] private Transform player;
        [SerializeField] private int curPoint = 0;
        [SerializeField] private float height, duration;

        [EndFoldout]
        [SerializeField]private int setIndex = 0;
        StageManager stageManager;
        [SerializeField]private List<MapState> stageStates = new List<MapState>();
        private void Awake()
        {
            stageManager = GetComponent<StageManager>();
            foreach (Transform point in pathParent)
            {
                mainPos.Add(point);
                point.GetComponent<MeshRenderer>().enabled = false;
            }
            var startPos = mainPos[0].position;
            startPos.x -= startPosInterval;
            player.position = startPos;
            
        }

        private void Start()
        {
            var max = (mainPos.Count-1)*tileCount+1;
            Debug.Log(max);
            stageStates = stageManager.GetStageState(max);
            PathPointLoading();
        }


        [Button]
        private async void Move()
        {
            for (int i = 0; i < tileCount; i++)
            {
                int index = curPoint + i;
                var targetPos = stagePoints[index].transform.position;
                targetPos.y = 0;
                await player.DOMove(targetPos, duration).SetEase(Ease.Linear).AsyncWaitForCompletion();
            }
            curPoint += tileCount;
        }

      

        /// <summary>
        /// MainPoint에 SmallPoint Add
        /// </summary>
        private void PathPointLoading()
        {
            StagePoint _stagePoint;
            for (int i = 0; i < mainPos.Count - 1; i++)
            {
                for (int j = 0; j < tileCount; j++)
                {
                    float divide = j;
                    var targetPos = Vector3.Lerp(mainPos[i].transform.position, mainPos[i + 1].transform.position,
                        divide / tileCount);
                    if (Physics.Raycast(targetPos + new Vector3(0, 10), Vector3.down, out RaycastHit hit, 10))
                        targetPos = hit.point;
                    _stagePoint = Instantiate(stagePoint, targetPos, Quaternion.identity);
                    stagePoints.Add(_stagePoint);
                    _stagePoint.Init(GetMapState(setIndex++));
                }
            }
            _stagePoint=Instantiate(stagePoint, mainPos[^1].transform.position, Quaternion.identity);
            stagePoints.Add(_stagePoint);
            _stagePoint.Init(GetMapState(setIndex++));
        }

        private MapState GetMapState(int index)
        {
            return stageStates[index];
        }
    }
}