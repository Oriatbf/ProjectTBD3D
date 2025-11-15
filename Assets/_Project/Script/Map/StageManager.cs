using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using VInspector;
using Random = UnityEngine.Random;

namespace Map
{
    public enum MapState
    {
        Enemy,
        StrongEnemy,
        Shop,
        Event,
        Boss,
        None
    }
    [Serializable]
    public class StageData
    {
        public MapState mapState; // 스테이지 종류
        public int percentage; //이 스테이지가 나올 퍼센트
        public int priority; //확률 재 분배 시 우선순위(내림차순 정렬)
        [Tooltip("디버깅용")]public int range; //랜덤 정수의 범위 설정
        public UnityEvent triggerEvent;
    }
    public class StageManager : MonoBehaviour
    {
        


        [Foldout("Data Setting")] [SerializeField]
        private int stageCount;

        [Tooltip("연속 동일한 스테이지가 나올 최대 개수")] [SerializeField]
        private int maxConsecutiveSameCount;

        [SerializeField] private List<StageData> stageDatas = new List<StageData>();

        [EndFoldout] 
        [Foldout("Debugging")] [SerializeField]
        private MapState curMapState;

        [SerializeField] private int curStageIndex;
        [EndFoldout] [SerializeField] private List<MapState> finalStageTypes;

        private void Awake()
        {
            stageDatas = stageDatas.OrderByDescending(s => s.priority).ToList(); //우선순위로 정렬
            CheckPercent(stageDatas); //지정한 확률 체크
        }
        
        public  List<MapState> GetStageState(int stageCount)
        {
            this.stageCount = stageCount;
            SetStage();
            return finalStageTypes;
        }


        //스테이지의 이벤트
        public void StageEvent(MapState mapState)
        {
            Action action = null;
            switch (mapState)
            {
                case MapState.Enemy:
                    action += ()=>SceneManager.LoadScene("GameScene");
                    break;
                case MapState.StrongEnemy:
                    action += ()=>SceneManager.LoadScene("GameScene");
                    break;
                case MapState.Boss:
                    action += ()=>SceneManager.LoadScene("GameScene");
                    break;
            }
            action?.Invoke();
        }
        


        #region 스테이지 설정 알고리즘

        /// <summary>
        /// 스테이지의 확률 체크 및 재분배
        /// </summary>
        private void CheckPercent(List<StageData> _stageDatas)
        {
            int totalPercent = _stageDatas.Sum(s => s.percentage);

            if (totalPercent != 100)
            {
                Debug.Log("확률이 맞지 않습니다 확률 재분배를 합니다.");
                int remaining = totalPercent < 100 ? 100 - totalPercent : totalPercent - 100;
                int count = _stageDatas.Count;

                // 기본값 분배
                int perStage = remaining / count;
                int extra = remaining % count;
                if (totalPercent < 100)
                {
                    //우선 순위에 따라 extra추가
                    for (int i = 0; i < count; i++)
                        _stageDatas[i].percentage += perStage + (i < extra ? 1 : 0);
                }
                else
                {
                    //우선 순위 역순에 따라 extrar감소
                    for (int i = 0; i < count; i++)
                        _stageDatas[count - 1 - i].percentage -= perStage + (i < extra ? 1 : 0);
                }
            }

            int sum = 0;
            foreach (var stage in _stageDatas)
                stage.range = sum += stage.percentage; //퍼센트에 따라 범위 변경
        }

        /// <summary>
        /// 스테이지 설정
        /// </summary>
        [Button]
        private void SetStage()
        {
            finalStageTypes = new List<MapState>();
            finalStageTypes.Add(MapState.None);
            finalStageTypes.Add(MapState.Enemy);
            for (int i = 1; i < stageCount - 1; i++)
            {
                var stage = GetStage(stageDatas);
                if (CountConsecutiveSame(stage) >= maxConsecutiveSameCount)
                {
                    stage = GetExclusionStage(stage);
                }

                if (stage != null) finalStageTypes.Add(stage.mapState);
            }

            finalStageTypes.Add(MapState.Boss);
        }

        /// <summary>
        /// 리스트에서 확률에 기반해 하나의 스테이지 반환
        /// </summary>
        private StageData GetStage(List<StageData> _stageDatas)
        {
            int random = Random.Range(1, 100 + 1);
            StageData stage = _stageDatas.FirstOrDefault(s => s.range >= random);
            if(stage == null)Debug.LogError("stage is null");
            return stage;
        }


        /// <summary>
        /// 특정 스테이지를 제외한 스테이지 반환
        /// </summary>
        private StageData GetExclusionStage(StageData exclusionStage)
        {
            List<StageData> newStageDatas = stageDatas
                .Where(s => s.mapState != exclusionStage.mapState)
                .Select(s => new StageData
                {
                    mapState = s.mapState,
                    percentage = s.percentage,
                    priority = s.priority,
                    range = s.range
                }).ToList();
            CheckPercent(newStageDatas);
            return GetStage(newStageDatas);
        }

        /// <summary>
        /// 스테이지 연속 개수 반환
        /// </summary>
        private int CountConsecutiveSame(StageData targetStageData)
        {
            if(targetStageData ==null)Debug.LogError("targetStageData is null");
            int count = 0;
            for (int i = finalStageTypes.Count - 1; i >= 0 && finalStageTypes[i] == targetStageData.mapState; i--)
                count += 1;
            return count;
        }

        #endregion

    }
}