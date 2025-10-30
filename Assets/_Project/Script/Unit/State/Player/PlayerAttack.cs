using UnityEngine;

public class PlayerAttack : IState<PlayerStateController>
{
    private PlayerStateController playerStateController;
    public void OperateEnter(PlayerStateController sender)
    {
        Debug.Log("플레이어 공격턴");
        playerStateController = sender;
    }

    public void OperateUpdate()
    {

    }

    public void OperateExit()
    {
     
    }
    
    
    private void BattleUI()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            // 자신이 클릭됐는지 확인
            if (hit.collider != null && hit.collider.gameObject == playerStateController.gameObject)
            {
               // if(_battleUI==null)Debug.LogError("BattleUI가 없음");
               // _battleUI?.SetPos(hit.transform);
                //_battleUI?.ShowHide();
            }
        }
       
        
    }


}
