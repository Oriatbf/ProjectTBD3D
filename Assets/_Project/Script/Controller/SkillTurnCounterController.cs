using System.Collections.Generic;
using DG.Tweening;
using SkillData;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class SkillTurnCounterController : BaseController
{
    private string canvasPath = "Assets/_Project/Prefab/UI/TurnCounter/TurnCounterCanvas.prefab";
    private string playerTurnImagePath = "Assets/_Project/Prefab/UI/TurnCounter/PlayerTurnCounter Variant.prefab";
    private string enemyTurnImagePath = "Assets/_Project/Prefab/UI/TurnCounter/EnemyTurnCounter Variant.prefab";
    private GameObject playerTurnImage,enemyTurnImage;
    
    private Queue<SkillData.SkillBase> turnQueue = new Queue<SkillData.SkillBase>();
    private Queue<TurnImage> turnImageQueue = new Queue<TurnImage>();
    private GameObject canvas;
    private Transform parent;
    private float imageMoveDistance = 200;
    
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

    public async void ActionSkill()
    {
        List<GameObject> destroyObj = new List<GameObject>();
        int _count = turnImageQueue.Count;
        for (int i = 0; i < _count; i++)
        {
            RectTransform _rect = null;
            var image = turnImageQueue.Dequeue();
            destroyObj.Add(image.gameObject);
            if(image.TryGetComponent(out RectTransform rect)) _rect = rect;
            var curPos  = rect.anchoredPosition;
            if (_rect != null)
            {
                int inversion = image.GetTeam() == Team.PlayerTeam ? 1 : -1;
                Sequence sequence = DOTween.Sequence();
                sequence.Append(_rect.DOAnchorPos(curPos + new Vector2(inversion* imageMoveDistance, 0), 0.2f));
                image.ArrowAlpha();
                await sequence.Play().AsyncWaitForCompletion();
            }
            var skill = turnQueue.Dequeue();
            skill.SkillAction();
        }
        foreach (var obj in destroyObj) GameObject.Destroy(obj);
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
            turnImage.SetInfo(skill,team,skill.GetData().Name);
            turnImageQueue.Enqueue(turnImage);
        }
        turnQueue.Enqueue(skill);
    }
    
    

    public void Dequeue()
    {
        turnQueue.Dequeue();
    }
}
