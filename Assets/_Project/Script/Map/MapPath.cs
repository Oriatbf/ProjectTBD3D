using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using VInspector;

namespace Map
{
    public class MapPath : MonoBehaviour
    {


        [Foldout("Points")] [Tooltip("mainPoint들의 부모")] [SerializeField]
        private Transform pathParent;

        [FormerlySerializedAs("stagePoint")] [SerializeField] private StagePoint stagePointPrefab;
        [SerializeField] private float startPosInterval; // 초기 위치 인터벌 -> mainPos[0] - interval
        [SerializeField] private int tileCount;
        [SerializeField] private Transform pointsContent;
        private List<Transform> mainPos = new List<Transform>();
        [SerializeField]private List<StagePoint> stagePoints = new List<StagePoint>();
        
        [EndFoldout] [Foldout("Move")] 
        
        [SerializeField] private Vehicle target;
        [SerializeField] private int curPoint = 0;
        [SerializeField] private float height, duration;

        [EndFoldout]
        [SerializeField]private int setIndex = 0; //stageStates의 인덱스
        StageManager stageManager;
        [SerializeField]private List<MapState> stageStates = new List<MapState>();
        
        private Camera _camera;
        private PointerEventData _pointerEventData;
        private List<RaycastResult> _raycastResults = new List<RaycastResult>();
        private StagePoint lastStagePoint;
        private int curInterval = 0;
        private bool isTargeting = false;
        
        private void Awake()
        {
            _camera = Camera.main;
            _pointerEventData = new PointerEventData(EventSystem.current);
            stageManager = GetComponent<StageManager>();
            foreach (Transform point in pathParent)
            {
                mainPos.Add(point);
                point.GetComponent<MeshRenderer>().enabled = false;
            }
            
        }

        private void Start()
        {
            var mapData = DataManager.Inst.GetMapData();
            if (!mapData.isMapGenerated)
            {
                var max = (mainPos.Count-1)*tileCount+1;
                stageStates = stageManager.GetStageState(max);
                DataManager.Inst.SaveStageStates(stageStates);
            }
            else stageStates = mapData.stageStates;
            PathPointLoading(); 
            var startPos = stagePoints[mapData.stageIndex].transform.position;
            curPoint = mapData.stageIndex;
            target.transform.position = startPos;
        }

        private void Update()
        {

            HandleSpawnTargeting();
        }
        
     

        private void HandleSpawnTargeting()
        {
            if (Input.GetMouseButtonDown(1))
            {
                CancelTargeting();
                return;
            }
        
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform.TryGetComponent(out StagePoint stagePoint))
                {
                    if(Input.GetMouseButtonDown(0))
                    {
                        Move(curInterval);
                    }
                    else
                    {
                        if (lastStagePoint == stagePoint) return;
                        curInterval = stagePoint.GetIndex()-curPoint;
                        if (curInterval > tileCount||stagePoint.GetIndex()<=curPoint) return;
                        if (lastStagePoint != null)lastStagePoint.UnTargeting();
                        stagePoint.Targeting();
                        lastStagePoint = stagePoint;
                    }

               
                }
            }
        }

        private void CancelTargeting()
        {
            lastStagePoint.UnTargeting();
            lastStagePoint = null;
            isTargeting = false;
        }


        [Button]
        private async void Move(int count)
        {
            target.SetMoving(true);
            for (int i = 1; i <= count; i++)
            {
                int index = curPoint + i;
                var targetPos = stagePoints[index].transform.position;
                targetPos.y = 0;
                await target.transform.DOMove(targetPos, duration).SetEase(Ease.Linear).AsyncWaitForCompletion();
            }
            target.SetMoving(false);
            curPoint += count;
            DataManager.Inst.SaveStageIndex(curPoint);
            await UniTask.WaitForSeconds(0.5f);
            stageManager.StageEvent(stagePoints[curPoint].GetMapState());
            
        }

      

        //MainPoint간 사이이 거리에 StagePoint소환
        private void PathPointLoading()
        {
            StagePoint _stagePoint;
            var startPos = mainPos[0].position;
            startPos.x -= startPosInterval;
            InstanceStagePoint(startPos);
            for (int i = 0; i < mainPos.Count - 1; i++)
            {
                for (int j = 0; j < tileCount; j++)
                {
                    float divide = j;
                    var targetPos = Vector3.Lerp(mainPos[i].transform.position, mainPos[i + 1].transform.position,
                        divide / tileCount);
                    if (Physics.Raycast(targetPos + new Vector3(0, 10), Vector3.down, out RaycastHit hit, 10))
                        targetPos = hit.point;
                    InstanceStagePoint(targetPos);
                }
            }
            InstanceStagePoint( mainPos[^1].transform.position);
        }

        private void InstanceStagePoint(Vector3 targetPos)
        {
            var _stagePoint=Instantiate(stagePointPrefab,targetPos, Quaternion.identity,pointsContent);
            stagePoints.Add(_stagePoint);
            _stagePoint.Init(GetMapState(setIndex),setIndex);
            setIndex++;
        }

        private MapState GetMapState(int index)
        {
            return stageStates[index];
        }
    }
}