using System;
using System.Collections.Generic;
using UnitData;
using UnityEngine;

public enum State
{
    Idle,Attack,None
}

public abstract class StateControllerBase : MonoBehaviour
{
    [SerializeField]protected State curState = State.None;
    public abstract void ChangeState(State newState);
    public State CurState { get { return curState; } }
}


public class StateController<T> : StateControllerBase where T : StateController<T>
{
    protected Dictionary<State,IState<T>> m_states = new Dictionary<State, IState<T>>();
    protected StateMachine<T> m_stateMachine;


    protected virtual void Update()
    {
        m_stateMachine.DoOperateUpdate();
    }
    


    /// <summary>
    ///  ChracterController의 state를 바꾸는 함수
    /// </summary>
    public override void ChangeState(State newState)
    {
        m_stateMachine.SetState(m_states[newState]);
        curState = newState;
    }

    /// <summary>
    /// 현재 State가져오는 함수
    /// </summary>
    public State GetState()
    {
        return curState;
    }

    protected void AddStates(State state, IState<T> stateInstance)
    {
        if(!m_states.ContainsKey(state))m_states.Add(state,stateInstance);
    }
}
