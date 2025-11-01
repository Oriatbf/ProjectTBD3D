using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TurnController : BaseController
{
    private List<StateControllerBase> players = new List<StateControllerBase>();
    private List<StateControllerBase> enemys = new List<StateControllerBase>();


    public override void OnUpdate()
    {
        base.OnUpdate();
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Debugging on TurnController");
            ChangeStates(State.Idle,Team.PlayerTeam);
            ChangeStates(State.Attack,Team.EnemyTeam);
        }
    }

    public void Add(List<Unit> units, Team team)
    {
        foreach (var unit in units)
        {
            if (unit.TryGetComponent(out StateControllerBase controllerBase))
            {
                switch (team)
                {
                    case Team.PlayerTeam:
                        players.Add(controllerBase);
                        break;
                    case Team.EnemyTeam:
                        enemys.Add(controllerBase);
                        break;
                }
                Debug.Log($"{team}이 추가되었습니다");
            }
        }
      
    }

    public void ChangeStates(State state,Team team)
    {
        List<StateControllerBase> selectedTeams = new List<StateControllerBase>();
        switch (team)
        {
            case Team.PlayerTeam:
                selectedTeams = players;
                break;
            case Team.EnemyTeam:
                selectedTeams = enemys;
                break;
        }

        foreach (var stateControll in selectedTeams)
        {
            stateControll.ChangeState(state);
        }

        if (team == Team.EnemyTeam && state == State.Attack) EnemyRegisterSkill();
    }

    private async void EnemyRegisterSkill()
    {
        await ApplicationManager.Inst.GetModule<EnemyRegisterController>().Register();
        ChangeStates(State.Idle, Team.EnemyTeam);
        ChangeStates(State.Attack, Team.PlayerTeam);
       
    }
    
}
