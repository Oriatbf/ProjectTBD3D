using System.Collections.Generic;
using System.Linq;
using _Project.Script.Controller;
using Core.Utility;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SkillData;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class SkillTurnCounter 
{
    private GameObject playerTurnImage,enemyTurnImage,expectationTurnImage;
    
    private Queue<SkillStackInfo> turnQueue = new Queue<SkillStackInfo>();
    private Queue<TurnImage> turnImageQueue = new Queue<TurnImage>();
    private GameObject turnCounterCanvas;
    private Transform parent;
    private float imageMoveDistance = 200;
    private float imageInterval = 60;
    
    

    /// <summary>
    /// 등록된 모든 스킬 사행
    /// </summary>
    public async UniTask ActionSkill()
    {
        List<GameObject> destroyObj = new List<GameObject>();
        int _count = turnImageQueue.Count;
        int textCount = turnQueue.Count;
        if(_count != textCount)Debug.LogWarning("turnqueue개수가 서로 맞지 않음");  
        //TurnCounterUI이미지 삭제
        for (int i = 0; i < _count; i++)
        {
            RectTransform _rect = null;
            if (turnQueue.Count <= 0 || turnImageQueue.Count <= 0) break;
            //실행할(삭제될) 스킬TurnImage 받기
            var image = turnImageQueue.Dequeue();
            Debug.Log($"Dequeue");
            destroyObj.Add(image.gameObject);
            if(image.TryGetComponent(out RectTransform rect)) _rect = rect;
            var curPos  = rect.anchoredPosition;
            //실행할(삭제될) 스킬 받기
            var skillStackInfo = turnQueue.Dequeue();
            var skill = skillStackInfo.skill;
            
            var sourceUnit = skill.GetSkillContext().SourceUnit;
            
          
            
            await ApplicationManager.Inst.GetModule<CameraController>().TargetLook(skill.GetSkillContext().SourceUnit);
            await sourceUnit.AttackAnim();
            //스킬 실행
            skill.SkillAction();
            if(skill.GetSkillContext().TargetUnit!=null)
                await ApplicationManager.Inst.GetModule<CameraController>().TargetLook(skill.GetSkillContext().TargetUnit);
            // 스킬 실행 후 UI 애니메이션 시작
            if (_rect != null)
            {
                Sequence seq = DOTween.Sequence();
                int inversion = image.GetTeam() == Team.PlayerTeam ? 1 : -1;

                //UI이미지 이동 + 투명화
                seq.Append(_rect.DOAnchorPos(curPos + new Vector2(inversion * imageMoveDistance, 0), 0.2f));
                seq.JoinCallback(() => image.ArrowAlpha());
                //몬스터 위에 스킬 스택되어있던거 삭제
                seq.JoinCallback(() =>
                    ApplicationManager.Inst.GetModule<SkillProgressController>().GetSkillStack().UnstackSkill(skillStackInfo.sourceTile));
                await seq.Play().AsyncWaitForCompletion();

            }

            
            
        }
        //한 턴에 해당하는 UI gameObject 삭제
        foreach (var obj in destroyObj) GameObject.Destroy(obj);
        Debug.Log("SkillTurnCounterOriginLook");
        ApplicationManager.Inst.GetModule<CameraController>().OriginLook();
        //한 턴이 끝
        ApplicationManager.Inst.GetModule<TurnController>().Reset();
    }

    public void SetCanvas()
    {
        turnCounterCanvas= ApplicationManager.Inst.GetModule<CanvasController>().GetCanvas("TurnCounterCanvas");
        playerTurnImage = Resources.Load<GameObject>("UI/TurnCounter/PlayerTurnCounter");
        enemyTurnImage = Resources.Load<GameObject>("UI/TurnCounter/EnemyTurnCounter");
        expectationTurnImage = Resources.Load<GameObject>("UI/TurnCounter/ExpectationTurnCounter");
        parent = turnCounterCanvas.transform.GetChild(0);
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
        float curSkillReq = skillStackInfo.stackTurn;
        for (int i = 0; i < skillList.Count; i++)
        {
            var skillReq = skillList[i].stackTurn;
            //턴 순서 정리
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
        Debug.Log("턴 이미지의 개수는 "+list.Count);
        for (int i = 0; i < list.Count; i++)
        {
            list[i].GetComponent<RectTransform>().DOAnchorPos(new Vector2(0,-130-imageInterval*i), 0.2f);
        }
    }

    public void ResetAllSkillTurnCounter()
    {
        foreach (var image in turnImageQueue)
        {
            GameObject.Destroy(image.gameObject);
        }
        turnImageQueue.Clear();
        turnQueue.Clear();
    }

    public TurnImage EnqueueExpectSkill(SkillBase skillBase)
    {
        var skillContext = skillBase.GetSkillContext();
        var skillData = skillBase.GetData();
        var stackTurn = InGameUnitInfo.PlayerCurTurn + skillData.RequireTurn;
        var skillStackInfo = new SkillStackInfo
            (skillBase,skillContext.SourceTile,stackTurn,Team.PlayerTeam);
        var obj = Object.Instantiate(expectationTurnImage, parent);
        if (obj.TryGetComponent(out TurnImage turnImage))
        {
            turnImage.SetInfo(skillStackInfo);
        };
        
        var skillList = turnQueue.ToList();
        var turnImageList = turnImageQueue.ToList();
        for (int i = 0; i < skillList.Count; i++)
        {
            var skillReq = skillList[i].stackTurn;
            //턴 순서 정리
            if (stackTurn < skillReq)
            {
                turnImageList.Insert(i,turnImage);
                turnImageQueue = new Queue<TurnImage>(turnImageList);
                RefreshUI();
                return turnImage;
            }
        }
        turnImageList.Add(turnImage);
        turnImageQueue = new Queue<TurnImage>(turnImageList);
        RefreshUI();
        return turnImage;
    }

    public void DequeueExpectSkill(TurnImage turnImage)
    {
        var list = turnImageQueue.ToList();
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == turnImage)
            {
                list.RemoveAt(i);
            }
        }
        turnImageQueue = new Queue<TurnImage>(list);
        Object.Destroy(turnImage.gameObject);
        RefreshUI();
    }
    

    public void DequeueByTile(Tile sourceTile)
    {
        var skillStackInfos = turnQueue.ToList();
        var turnImages = turnImageQueue.ToList();
        for (int i = 0; i < skillStackInfos.Count; i++)
        {
            if (skillStackInfos[i].sourceTile == sourceTile)
            {
                skillStackInfos.RemoveAt(i);
                Object.Destroy(turnImages[i].gameObject);
                turnImages.RemoveAt(i);
            }
        }
        turnQueue = new Queue<SkillStackInfo>(skillStackInfos);
        turnImageQueue = new Queue<TurnImage>(turnImages);
        RefreshUI();
        
    }

    public void DequeueByTurn(float curStackTurn,float deleteStackTurn)
    {
        var skillStackInfos = turnQueue.ToList();
        var turnImages = turnImageQueue.ToList();

        for (int i = 0; i < skillStackInfos.Count; i++)
        {
            var curSkillStackTurn = skillStackInfos[i].stackTurn;
            if (curSkillStackTurn >= curStackTurn && curSkillStackTurn <= deleteStackTurn)
            {
                skillStackInfos.RemoveAt(i);
                Object.Destroy(turnImages[i].gameObject);
                turnImages.RemoveAt(i);
            }
        }
        
        turnQueue = new Queue<SkillStackInfo>(skillStackInfos);
        turnImageQueue = new Queue<TurnImage>(turnImages);
        RefreshUI();
    }
}
