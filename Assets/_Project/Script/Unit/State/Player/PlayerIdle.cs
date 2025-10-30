using UnityEngine;

public class PlayerIdle : IState<PlayerStateController>
{
    private PlayerStateController playerStateController;
    
    public void OperateEnter(PlayerStateController sender)
    {
        Debug.Log("Idle상태에 진입");
        playerStateController = sender;
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
