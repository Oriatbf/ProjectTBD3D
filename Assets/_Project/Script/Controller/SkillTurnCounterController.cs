using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class SkillTurnCounterController : BaseController
{
    private string canvasPath = "Assets/Prefab/UI/TurnCounterCanvas";
    private string playerTurnImagePath = "Assets/Prefab/UI/PlayerTurnCounter";
    private string enemyTurnImagePath = "Assets/Prefab/UI/EnemyTurnCounter";
    private GameObject playerTurnImage,enemyTurnImage;
    
    private Queue<Transform> turnQueue = new Queue<Transform>();
    private GameObject canvas;
    private Transform parent;
    
    public override void OnIntialize()
    {
        SetCanvas();
    }

    private async void SetCanvas()
    {
        canvas= await Addressables.LoadAssetAsync<GameObject>(canvasPath).Task;
        playerTurnImage = await Addressables.LoadAssetAsync<GameObject>(playerTurnImagePath).Task;
        enemyTurnImage = await Addressables.LoadAssetAsync<GameObject>(enemyTurnImagePath).Task;
        var obj = GameObject.Instantiate(canvas);
        parent = obj.transform.GetChild(0);
        Enqueue(Team.PlayerTeam);
    }

    public void Enqueue(Team team)
    {
        var image = team == Team.PlayerTeam ? playerTurnImage : enemyTurnImage;
        var obj = GameObject.Instantiate(image, parent);
        turnQueue.Enqueue(obj.transform);
    }

    public void Dequeue()
    {
        turnQueue.Dequeue();
    }
}
