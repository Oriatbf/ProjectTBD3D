using System;
using System.Collections.Generic;
using _Project.Pooling;
using _Project.Script.Controller;
using Core.Utility;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SkillData;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;
using VInspector;

public enum Team{PlayerTeam,EnemyTeam,neutralityTeam } //neutrality == 중립

public class Unit : MonoBehaviour
{
    [Foldout("Debug")]
    [SerializeField]private StatContainer _statContainer = new StatContainer();
    [SerializeField] private int constID;
    [SerializeField] private Material baseMaterial;
    [EndFoldout]
    
    private List<int> originalBringSkills = new List<int>();
    private List<int> bringSkills = new List<int>();
    private Team team; 
    private Tile tile; 
    
    private Animator animator;
    private ActionStateContainer _actionStateContainer;
    private UnitSaveData unitData;
    private UnitController unitController;
    private SpriteRenderer spr;

    private Material originalMat, whiteMat;
    
    private HealthContent healthContent;
    private PopUpTxt ratePopUpTxt;
    string animatorPath = "Assets/_Project/Art/Animator/";
    
    private bool isDead = false;
    
    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private void Awake()
    {
        animator = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();
        
        originalMat = new Material(baseMaterial);
        spr.material = originalMat;
        whiteMat = Resources.Load<Material>("Material/White");
        
        _actionStateContainer = new ActionStateContainer(this);
        unitController = GetComponent<UnitController>();
       
        HideOutLine();
    }
    private void Start()
    {
        animator.Play("Idle");
      }
    

    #region 생성 시

    public void Init(UnitSaveData unitData,string animatorName, Team team)
    {
        var animator = Resources.Load<AnimatorOverrideController>($"Animator/{animatorName}");
        this.animator.runtimeAnimatorController = animator;
        _statContainer =  unitData.statContainer;
        originalBringSkills = unitData.bringSkills;
        bringSkills = unitData.bringSkills;
        this.unitData = unitData;
        this.team = team;
        this.constID = unitData.constId;

        if (team == Team.PlayerTeam)
        {
            if(unitData.flipDir == -1)spr.flipX = true;
            Debug.Log($"playerFlip {unitData.flipDir}");
        }
        else if (team == Team.EnemyTeam)
        {
            if(unitData.flipDir == 1)spr.flipX = true;
            ratePopUpTxt = ApplicationManager.Inst.GetModule<PoolController>().Spawn<PopUpTxt>("PopUpTxt");
        }
    }
    
    public void SetTile(Tile tile)
    {
        this.tile = tile;
        unitController.Initialize(tile);
    }
    /// <summary>
    /// 생성시 실행
    /// </summary>
    public virtual void Initalize()
    {
        SetHealthContent();
        SetAction();
        Reset();
    }

    #endregion

    /// <summary>
    /// 한 턴이 끝날때마다 리셋
    /// </summary>
    public virtual void Reset()
    {
        _statContainer.turnGauge.SetBaseValue(0);
        _actionStateContainer.ExecuteTrigger(ActionTrigger.OnTurnStart);
        ShowRate();
    }
    


    private void SetAction()
    {
        Debug.Log($"SetStatAction {unitData.animatorName}");
        _statContainer.hp.OnValueChanged += OnHpChange;
        _statContainer.turnGauge.OnValueChanged += OnCostChange;
        _statContainer.charmResist.OnValueChanged += OnCharmResistChange;
    }

    private void SetHealthContent()
    {
        healthContent = ApplicationManager.Inst.GetModule<PoolController>().Spawn<HealthContent>("HealthContent");
        healthContent.Init(_statContainer);
    }

    private void LateUpdate()
    {
        if(healthContent != null)
             healthContent.SetPos(tile.GetPos());
        if(tile == null) Debug.LogError("Unit tile is null");
        ApplicationManager.Inst.GetModule<SkillProgressController>().GetSkillStack().PositionedOnCamera(tile);
        ApplicationManager.Inst.GetModule<ActionStateStackController>().PositionedOnCamera(tile);
    }

    public List<int> GetSkillList()
    {
        if(bringSkills.Count <= 0 || bringSkills==null)Debug.LogError("NoSkills In Unit");
        return bringSkills;
    }

    public List<int> GetOriginalBringSkills()
    {
        if(originalBringSkills.Count <= 0 || originalBringSkills==null)Debug.LogError("NoSkills In Unit");
        return originalBringSkills;
    }

    public Team GetTeam() => team;
    public Tile GetTile() => tile;
    public StatContainer GetStatContainer() => _statContainer;

    public void OnTurnEnd()
    {
        _actionStateContainer.ExecuteTrigger(ActionTrigger.OnTurnEnd);
    }
    public ActionStateContainer GetActionStateContainer() => _actionStateContainer;
    public Animator GetAnimator() => animator;
    
    public UnitSaveData GetUnitData() => unitData;

    public void SetBringSkills(List<int> skills)
    {
        bringSkills = skills;
    }

    public async UniTask AttackAnim()
    {
        animator.Play(Attack);

        // 한 프레임 대기 (상태 전환용)
        await UniTask.Yield();

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        // 애니메이션 길이만큼 대기
        await UniTask.Delay(TimeSpan.FromSeconds(state.length));
        await UniTask.WaitForSeconds(0.15f);

        Debug.Log("공격 애니메이션 끝");
    }
    
    public void GetDamage(float damage,SkillContext skillContext,SkillType skillType)
    {
        if(isDead) return;
        Debug.Log($"{damage} 만큼 데미지를 받음");
        if (damage > 0)
        {
            switch (skillType)
            {
                case SkillType.Penetration:
                    _statContainer.hp.AddBaseValue( -damage);
                    break;
                default:
                    var remainDamage = damage - _statContainer.barrier._baseValue;
                    _statContainer.barrier.AddBaseValue(-damage);
                    if(remainDamage > 0)
                        _statContainer.hp.AddBaseValue( -remainDamage);
                    break;
            }
            
            ApplicationManager.Inst.GetModule<PoolController>()
                .Spawn<HitEffect>("HitEffect",transform.position + new Vector3(0,2,-1),Quaternion.identity);
           
            if (skillContext != null)
            {
                skillContext.SourceUnit.GetActionStateContainer().ExecuteTrigger(ActionTrigger.OnAttack,skillContext); 
                _actionStateContainer.ExecuteTrigger(ActionTrigger.OnHitted,skillContext);      
            }
           
            DamagedEffect();
        }
        else
        {
            _statContainer.hp.AddBaseValue( -damage);
            if (skillContext != null)
            {
                _actionStateContainer.ExecuteTrigger(ActionTrigger.OnHeal, skillContext);
            }
        }
        ApplicationManager.Inst.GetModule<PopUpUIController>().SpawnDamagePopUp(damage,transform);
    }

    /// <summary>
    /// 데미지를 받았을 때 실행할 애니메이션
    /// </summary>
    private void DamagedEffect()
    {
        transform.DOKill();
        
        ApplicationManager.Inst.GetModule<AudioController>().PlayAudio("Hit");
        
        WhiteFlash().Forget();
        
        var originPos = tile.GetPos();
        Sequence seq = DOTween.Sequence();
        var endValue = team == Team.PlayerTeam? originPos.x-1.5f : originPos.x+1.5f;
        seq.Append(transform.DOMoveX(endValue, 0.25f).SetEase(Ease.OutCubic));
        seq.AppendInterval(0.25f);
        seq.Append(transform.DOMoveX(originPos.x,0.5f ).SetEase(Ease.OutSine));
        seq.Play();
    }

    private async UniTask WhiteFlash()
    {
        if(spr != null) spr.material = whiteMat;
        await UniTask.WaitForSeconds(0.15f);
        if(spr != null)spr.material = originalMat;
    }
    
    private void OnHpChange(float value)
    {
        Debug.Log($"{unitData.animatorName}의 남은피는 {value}");
        if (value <= 0)
        {
            Debug.Log($"{unitData.animatorName} 사망");
            isDead = true;
            OnDispos();
        }
    }

    private void OnCharmResistChange(float value)
    {
        if (value >= _statContainer.charmResist._maxValue)
        {
            ApplicationManager.Inst.GetModule<PoolController>()
                .Spawn<CharmEffect>("CharmEffect",transform.position + new Vector3(0,0.3f));
            DataManager.Inst.SaveSurviveUnit(this);
            OnDispos(true);
        }
    }

    private void OnCostChange(float value)
    {
        if(value<=0)Debug.Log("cost가 0임");
    }

    private void OnDispos(bool isCharm = false)
    {
        FactoryManager.Inst.RegisterDeadUnit(this);
        if(healthContent != null)
            ApplicationManager.Inst.GetModule<PoolController>().ReturnToPool("HealthContent",healthContent.transform);
        if(ratePopUpTxt != null)
            ApplicationManager.Inst.GetModule<PoolController>().ReturnToPool("PopUpTxt",ratePopUpTxt.transform);
        ApplicationManager.Inst.GetModule<SkillProgressController>().UnStackAll(tile);
        ApplicationManager.Inst.GetModule<ActionStateStackController>().UnstackAllUnitBuffs(tile);
        tile.DestroyUnit();
        if(!isCharm)
            DataManager.Inst.DeleteUnit(unitData.constId);
        Destroy(gameObject);
    }

    public void ShowOutLine(Color color)
    {
        originalMat.SetFloat("_OutlineSize", 0.5f);
        originalMat.SetColor("_OutlineColor", color);
    }

    public void HideOutLine()
    {
        originalMat.SetFloat("_OutlineSize", 0f);
    }

    public void ShowRate()
    {
        if (team == Team.PlayerTeam) return;
        Debug.Log("ShowRate");
        var rate = TamingHelper.TaimgCalculator(this);
        ratePopUpTxt.SetTxt($"약 {Mathf.Floor((rate*100))}%",Color.red,true).Forget();
        var pos = Camera.main.WorldToScreenPoint(tile.GetPos()) + new Vector3(100, 100);
        ratePopUpTxt.transform.position = pos;
    }

    public void HideRate()
    {
        if (team == Team.PlayerTeam) return;
        ratePopUpTxt.Hide();
    }
    
}
