using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageDataSO",menuName = "Scriptable Map/StageData",order = 1)]

public class StageDataSO : ScriptableObject
{

    [Tooltip("연속 동일한 스테이지가 나올 최대 개수")]
    public int maxConsecutiveSameCount;
    
    public List<StageData> stageDatas;
}

[Serializable]
public class StageData
{
    public RoomType mapState; // 스테이지 종류
    public int percentage; //이 스테이지가 나올 퍼센트
    public int priority; //확률 재 분배 시 우선순위(내림차순 정렬)
    [Tooltip("디버깅용")]public int range; //랜덤 정수의 범위 설정
}

