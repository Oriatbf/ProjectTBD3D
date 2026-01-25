using System;
using System.Collections.Generic;
using System.Linq;
using SkillData;
using UnityEngine;

/// <summary>
/// ActionState 컨테이너 (Unit에 부착)
/// </summary>
public class ActionStateContainer
{
    private Dictionary<ActionTrigger, Dictionary<string, ActionState>> actionStates 
        = new Dictionary<ActionTrigger, Dictionary<string, ActionState>>();

    private Unit _unit;

    public ActionStateContainer(Unit unit)
    {
        _unit = unit;
        // 각 트리거별 딕셔너리 초기화
        foreach (ActionTrigger trigger in Enum.GetValues(typeof(ActionTrigger)))
        {
            actionStates[trigger] = new Dictionary<string, ActionState>();
        }
    }

    /// <summary>
    /// ActionState 추가
    /// </summary>
    public void AddActionState(ActionTrigger trigger, ActionState state)
    {
        string id = state.GetId();
        
        if (actionStates[trigger].ContainsKey(id))
        {
            // 이미 존재하면 데이터 병합
            actionStates[trigger][id].GetData().MergeWith(state.GetData());
        }
        else
        {
            actionStates[trigger][id] = state;
        }
        
        if(trigger == ActionTrigger.None)
            state.Execute();
    }

    /// <summary>
    /// 특정 트리거의 모든 ActionState 실행
    /// </summary>
    public void ExecuteTrigger(ActionTrigger trigger, SkillContext skillContext = null)
    {
        List<string> toRemove = new List<string>();

        foreach (var kvp in actionStates[trigger])
        {
            kvp.Value.Execute(skillContext);
            
            if (!kvp.Value.IsExist())
            {
                toRemove.Add(kvp.Key);
            }
            
        }

        // 종료된 ActionState 제거
        foreach (var id in toRemove)
        {
            actionStates[trigger].Remove(id);
            ApplicationManager.Inst.GetModule<ActionStateStackController>().UnStackBuff(_unit.GetTile(),id);
        }
    }

    /// <summary>
    /// 특정 ActionState 제거
    /// </summary>
    public void RemoveActionState(ActionTrigger trigger, string id)
    {
        if (actionStates[trigger].ContainsKey(id))
        {
            actionStates[trigger][id].Finish();
            actionStates[trigger].Remove(id);
        }
    }

    /// <summary>
    /// 모든 ActionState 제거
    /// </summary>
    public void ClearAll()
    {
        foreach (var triggerDict in actionStates.Values)
        {
            foreach (var state in triggerDict.Values)
            {
                state.Finish();
            }
            triggerDict.Clear();
        }
    }

    /// <summary>
    /// 특정 트리거의 ActionState 개수
    /// </summary>
    public int GetCount(ActionTrigger trigger)
    {
        return actionStates[trigger].Count;
    }

    /// <summary>
    /// 특정 ActionState 가져오기
    /// </summary>
    public ActionState GetActionState(ActionTrigger trigger, string id)
    {
        if (actionStates[trigger].ContainsKey(id))
            return actionStates[trigger][id];
        return null;
    }

    public bool ContainActionState(ActionState actionState)
    {
        string id = actionState.GetId();
        return actionStates.Values
            .Any(dict => dict.ContainsKey(id));
    }
}