using System;
using System.Collections.Generic;
using SkillData;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class EnemyStateController : StateController<EnemyStateController>
{
    
    private List<SkillBase> skills = new List<SkillBase>();
    
    
    private void Awake()
    {
        IState<EnemyStateController> Idle = new EnemyIdle();
        IState<EnemyStateController> Attack = new EnemyAttack();
        IState<EnemyStateController> None = new EnemyNone();
        
        AddStates(State.Idle, Idle);
        AddStates(State.Attack,Attack);
        AddStates(State.None,None);
       
        m_stateMachine = new StateMachine<EnemyStateController>(this,m_states[State.None]);
    }
    
    public void SetCurSkill( List<SkillBase> skills) => this.skills = skills;
    public List<SkillBase>  GetCurSkill() => skills;
    
    private void Start()
    {
    }

    protected override void Update()
    {
        base.Update();
    }
}
