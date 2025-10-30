using System.Collections.Generic;
using DG.Tweening;
using SkillData;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyIdle : IState<EnemyStateController>
{
    private EnemyStateController enemyStateController;
    public void OperateEnter(EnemyStateController sender)
    {
        enemyStateController = sender;
        StateAction();
    }

    public void OperateUpdate()
    {
        
    }

    public void OperateExit()
    {
        
    }

    public void StateAction()
    {
      
    }


    
}
