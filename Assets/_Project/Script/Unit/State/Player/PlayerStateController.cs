using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerStateController : StateController<PlayerStateController>
{
    private void Awake()
    {
        IState<PlayerStateController> Idle = new PlayerIdle();
        IState<PlayerStateController> Attack = new PlayerAttack();
        AddStates(State.Idle,Idle);
        AddStates(State.Attack,Attack);
        
        m_stateMachine = new StateMachine<PlayerStateController>(this,m_states[State.Idle]);
    }

    protected override void Update()
    {
        base.Update();
    }



    
}
