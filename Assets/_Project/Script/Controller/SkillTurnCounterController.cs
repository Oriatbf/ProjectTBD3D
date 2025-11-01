using System.Collections.Generic;
using SkillData;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class SkillTurnCounterController : BaseController
{
    private string canvasPath = "Assets/Prefab/UI/TurnCounterCanvas";
    private string playerTurnImagePath = "Assets/Prefab/UI/PlayerTurnCounter";
    private string enemyTurnImagePath = "Assets/Prefab/UI/EnemyTurnCounter";
    private GameObject playerTurnImage,enemyTurnImage;
    
    private Queue<SkillData.SkillBase> turnQueue = new Queue<SkillData.SkillBase>();
    private GameObject canvas;
    private Transform parent;
    
    public override void OnInitialize()
    {
        SetCanvas();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (Input.GetKeyDown(KeyCode.F4))
        {
            TBDLogger.CommandLog(KeyCode.F4,this);
            ActionSkill();
        }
    }

    public void ActionSkill()
    {
        foreach (var skill in turnQueue)
        {
            skill.SkillAction();
        }
    }

    private async void SetCanvas()
    {
        canvas= await Addressables.LoadAssetAsync<GameObject>(canvasPath).Task;
        playerTurnImage = await Addressables.LoadAssetAsync<GameObject>(playerTurnImagePath).Task;
        enemyTurnImage = await Addressables.LoadAssetAsync<GameObject>(enemyTurnImagePath).Task;
        var obj = GameObject.Instantiate(canvas);
        parent = obj.transform.GetChild(0);
    }

    public void Enqueue(Team team,SkillBase skill)
    {
        var image = team == Team.PlayerTeam ? playerTurnImage : enemyTurnImage;
        var obj = GameObject.Instantiate(image, parent);
        if (obj.TryGetComponent(out TurnImage turnImage))
        {
            turnImage.SetInfo(skill.GetData().Name);
        }
        turnQueue.Enqueue(skill);
    }
    
    

    public void Dequeue()
    {
        turnQueue.Dequeue();
    }
}
