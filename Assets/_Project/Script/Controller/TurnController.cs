using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class TurnController : BaseController
{
    private readonly string turnEndCanvasPath = "Assets/_Project/Prefab/UI/PlayerTurnEndCanvas.prefab";
    private List<StateControllerBase> players = new List<StateControllerBase>();
    private List<StateControllerBase> enemys = new List<StateControllerBase>();
    private List<Unit> allUnits = new List<Unit>();
    private Team AttackTurn = Team.EnemyTeam;
    PlayerTurnEnd playerTurnEnd;

    public override void OnInitialize()
    {
        base.OnInitialize();
        SetCanvas();
    }

    private async void SetCanvas()
    {
        var canvas = await Addressables.LoadAssetAsync<GameObject>(turnEndCanvasPath).Task;
        var obj = GameObject.Instantiate(canvas);
        if (obj.TryGetComponent(out PlayerTurnEnd playerTurnEnd)) this.playerTurnEnd = playerTurnEnd;
        playerTurnEnd.SetTurnEndAction(PlayerTurnEndAction);
        playerTurnEnd.SetNextStageAction(() => SceneManager.LoadScene("MapScene"));
    }

    /// <summary>
    /// playerTurnEnd 버튼에 들어갈 액션
    /// </summary>
    private async void PlayerTurnEndAction()
    {
        if (AttackTurn == Team.EnemyTeam) return;
        ApplicationManager.Inst.GetModule<CharacterInfoController>().Hide();
        await ApplicationManager.Inst.GetModule<SkillTurnCounterController>().ActionSkill().AsAsyncUnitUniTask();
        
        ChangeStates(State.Idle, Team.PlayerTeam);
        ChangeStates(State.Attack, Team.EnemyTeam);
        
    }

    /// <summary>
    /// 맵 선택창으로 이동
    /// </summary>
    public void MapStage() => playerTurnEnd.NextStageActive();

    public override void OnUpdate()
    {
        base.OnUpdate();
    }
    
    public void Reset()
    {
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

        if (team == Team.EnemyTeam && state == State.Attack) EnemyRegisterSkill();
    }

    private async void EnemyRegisterSkill()
    {
        await ApplicationManager.Inst.GetModule<EnemyRegisterController>().Register();
        ChangeStates(State.Idle, Team.EnemyTeam);
        ChangeStates(State.Attack, Team.PlayerTeam);
       
    }
    
}
