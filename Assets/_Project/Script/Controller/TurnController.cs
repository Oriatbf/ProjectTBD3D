using System.Collections.Generic;
using _Project.Script.Controller;
using Core.Utility;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class TurnController : BaseController
{
    private List<StateControllerBase> players = new List<StateControllerBase>();
    private List<StateControllerBase> enemys = new List<StateControllerBase>();
    private List<Unit> allUnits = new List<Unit>();
    private Team AttackTurn = Team.EnemyTeam;
    private bool isBattling = false;
    TurnEndCanvas turnEndCanvas;

    public override void OnInitialize()
    {
        base.OnInitialize();
        SetCanvas();
    }

    private void SetCanvas()
    {
        turnEndCanvas = ApplicationManager.Inst.GetModule<CanvasController>().GetCanvas<TurnEndCanvas>("TurnEndCanvas");
        turnEndCanvas.ChangeState(true,true,true);
    }

    /// <summary>
    /// playerTurnEnd 버튼에 들어갈 액션
    /// </summary>
    public async UniTask PlayerTurnEndAction()
    {
        if (isBattling) return;
        isBattling = true;
        ApplicationManager.Inst.GetModule<CharacterSkillController>().CancelTargeting();
        ApplicationManager.Inst.GetModule<CharacterSkillController>().Hide();
        await ApplicationManager.Inst.GetModule<SkillProgressController>().ActionSkill();
        foreach (var unit in allUnits)
        {
            unit.OnTurnEnd();
        }

        Debug.Log("TurnEnd");
        ChangeStates(State.Idle, Team.PlayerTeam);
        ChangeStates(State.Attack, Team.EnemyTeam);
        isBattling = false;
        
    }

    /// <summary>
    /// 맵 선택창으로 이동
    /// </summary>
    public void MapStage()
    {
        if (turnEndCanvas == null)
        {
            Debug.LogError("PlayerTurnEnd is Null");
            return;
        }
        turnEndCanvas.NextStageActive();
    }

    public void Reset()
    {
        Debug.Log("UnitResets");
        FactoryManager.Inst.TurnInit();
        foreach (var unit in allUnits)
        {
            unit.Reset();
        }
    }
    
    public void TurnStart()
    {
        ChangeStates(State.Idle,Team.PlayerTeam);
        ChangeStates(State.Attack,Team.EnemyTeam);
    }

    /// <summary>
    /// 팀 추가
    /// </summary>
    public void Add(List<Unit> units, Team team)
    {
        switch (team)
        {
            case Team.PlayerTeam:
                players.Clear();
                break;
            case Team.EnemyTeam:
                enemys.Clear();
                break;
        }
        foreach (var unit in units)
        {
            allUnits.Add(unit);
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

        if (state == State.Attack) AttackTurn = team;
        
        foreach (var stateControl in selectedTeams)
        {
            stateControl.ChangeState(state);
        }

        if (team == Team.EnemyTeam && state == State.Attack) EnemyRegisterSkill().Forget();
    }

    private async UniTask EnemyRegisterSkill()
    {
        await ApplicationManager.Inst.GetModule<EnemyRegisterController>().Register();
        ChangeStates(State.Idle, Team.EnemyTeam);
        ChangeStates(State.Attack, Team.PlayerTeam);
       
    }
    
}
