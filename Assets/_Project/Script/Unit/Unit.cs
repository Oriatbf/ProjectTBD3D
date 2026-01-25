using System;
using System.Collections.Generic;
using _Project.Pooling;
using _Project.Script.Controller;
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
    [EndFoldout]
    
    private List<int> originalBringSkills = new List<int>();
    private List<int> bringSkills = new List<int>();
    private Team team; 
    private Tile tile; 
    
    private Animator animator;
    private ActionStateContainer _actionStateContainer;
    private UnitSaveData unitData;
    private UnitController unitController;
    
    private HealthContent healthContent;
    string animatorPath = "Assets/_Project/Art/Animator/";
    
    private bool isDead = false;
    
    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private void Awake()
    {
        animator = GetComponent<Animator>();
        _actionStateContainer = new ActionStateContainer(this);
        unitController = GetComponent<UnitController>();
    }
    private void Start()
    {
        animator.Play("Idle");
    }
    

    #region 생성 시

    public async void Init(UnitSaveData unitData,string animatorName, Team team)
    {
        var animator = await Addressables.LoadAssetAsync<AnimatorOverrideController>
            (animatorPath+animatorName+".overrideController").Task;
        this.animator.runtimeAnimatorController = animator;
        _statContainer = unitData.statContainer;
        originalBringSkills = unitData.bringSkills;
        bringSkills = unitData.bringSkills;
        this.unitData = unitData;
        this.team = team;
        this.constID = unitData.constId;
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
        List<string> removeList = new List<string>();

    }
    


    private void SetAction()
    {
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
        ApplicationManager.Inst.GetModule<SkillStackController>().PositionedOnCamera(tile);
        ApplicationManager.Inst.GetModule<BuffStackController>().PositionedOnCamera(tile);
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

    public void AttackAnim()
    {
        Debug.Log("스킬 애니메이션");
        animator.Play(Attack);
    }
    
    public void GetDamage(float damage,SkillContext skillContext,SkillType skillType)
    {
        if(isDead) return;
        Debug.Log($"{damage} 만큼 데미지를 받음");
        if (damage > 0)
        {
            ApplicationManager.Inst.GetModule<PoolController>().Spawn<HitEffect>("HitEffect",transform.position + new Vector3(0,2,-1),Quaternion.identity);
            
            var remainDamage = damage - _statContainer.barrier._baseValue;
            _statContainer.barrier.AddBaseValue(-damage);
            if(remainDamage > 0)
                _statContainer.hp.AddBaseValue( -remainDamage);
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
        
        var originPos = tile.GetPos();
        Sequence seq = DOTween.Sequence();
        var endValue = team == Team.PlayerTeam? originPos.x-1.5f : originPos.x+1.5f;
        
        seq.Append(transform.DOMoveX(endValue, 0.25f).SetEase(Ease.OutCubic));
        seq.AppendInterval(0.25f);
        seq.Append(transform.DOMoveX(originPos.x,0.5f ).SetEase(Ease.OutSine));
        seq.Play();
    }
    
    private void OnHpChange(float value)
    {
        if (value <= 0)
        {
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
            DataManager.Inst.SaveUnit(unitData);
            OnDispos();
        }
    }

    private void OnCostChange(float value)
    {
        if(value<=0)Debug.Log("cost가 0임");
    }

    private void OnDispos()
    {
        FactoryManager.Inst.RegisterDeadUnit(this);
        ApplicationManager.Inst.GetModule<PoolController>().ReturnToPool("HealthContent",healthContent.transform);
        ApplicationManager.Inst.GetModule<SkillStackController>().UnstackAllUnitSkills(tile);
        ApplicationManager.Inst.GetModule<SkillTurnCounterController>().DequeueByTile(tile);
        ApplicationManager.Inst.GetModule<BuffStackController>().UnstackAllUnitBuffs(tile);
        DataManager.Inst.DeleteUnit(unitData.constId);
        Destroy(gameObject);
    }
}
