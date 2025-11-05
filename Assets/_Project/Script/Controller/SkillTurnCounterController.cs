using System.Collections.Generic;
using System.Linq;
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
    
    private Queue<SkillStackInfo> turnQueue = new Queue<SkillStackInfo>();
    private Queue<TurnImage> turnImageQueue = new Queue<TurnImage>();
    private GameObject canvas;
    private Transform parent;
    private float imageMoveDistance = 200;
    private float imageInterval = 60;
    
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

    /// <summary>
    /// 등록된 모든 스킬 사행
    /// </summary>
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
            var skillStackInfo = turnQueue.Dequeue();
            var skill = skillStackInfo.skill;
            skill.SkillAction();
            ApplicationManager.Inst.GetModule<SkillStackController>().UnstackSkill(skillStackInfo.sourceTile);
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

    /// <summary>
    /// 스킬 등록
    /// </summary>
    public void Enqueue(SkillStackInfo skillStackInfo)
    {
        var image = skillStackInfo.team == Team.PlayerTeam ? playerTurnImage : enemyTurnImage;
        var obj = GameObject.Instantiate(image, parent);
        if (obj.TryGetComponent(out TurnImage turnImage))
        {
            turnImage.SetInfo(skillStackInfo);
            EnqueueSkill(skillStackInfo,turnImage);
        }
        RefreshUI();
    }

    /// <summary>
    /// RequireTurn에 따라 데이터 재배치
    /// </summary>
    private void EnqueueSkill(SkillStackInfo skillStackInfo,TurnImage turnImage)
    {
  
        var skillList = turnQueue.ToList();
        var turnImageList = turnImageQueue.ToList();
        var skill = skillStackInfo.skill;
        float curSkillReq = skillStackInfo.stackTurn;
        for (int i = 0; i < skillList.Count; i++)
        {
            var skillReq = skillList[i].stackTurn;
            if (curSkillReq < skillReq)
            {
                skillList.Insert(i,skillStackInfo);
                turnImageList.Insert(i,turnImage);
                turnQueue = new Queue<SkillStackInfo>(skillList);
                turnImageQueue = new Queue<TurnImage>(turnImageList);
                return;
            }
        }
        skillList.Add(skillStackInfo);
        turnImageList.Add(turnImage);
        turnQueue = new Queue<SkillStackInfo>(skillList);
        turnImageQueue = new Queue<TurnImage>(turnImageList);
    }

    /// <summary>
    /// UI재배치
    /// </summary>
    private void RefreshUI()
    {
        var list = turnImageQueue.ToList();
        for (int i = 0; i < list.Count; i++)
        {
            list[i].GetComponent<RectTransform>().DOAnchorPos(new Vector2(0,-imageInterval*i), 0.2f);
        }
    }
    

    public void Dequeue()
    {
        turnQueue.Dequeue();
    }
}
