using System;
using System.Collections.Generic;
using SkillData;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VInspector;

public enum Team{PlayerTeam,EnemyTeam,neutralityTeam } //neutrality == 중립

public class Unit : MonoBehaviour
{
    [Foldout("Debug")]
    [SerializeField]private StatContainer _statContainer = new StatContainer();
    [SerializeField] private List<int> bringSkills = new List<int>();
    [SerializeField] private int constID;
    [EndFoldout]
    
    private Dictionary<string,BuffDebuff> buffDebuffs = new Dictionary<string,BuffDebuff>();
    
    private Team team; 
    private Tile tile; 
    
    private Animator animator;
    private ActionContainer actionContainer;
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
        actionContainer = new ActionContainer();
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
        List<string> removeList = new List<string>();

        foreach (var buff in buffDebuffs.Values)
        {
            buff.Execute();
            if (!buff.isExist)
                removeList.Add(buff.id);
        }

        foreach (var id in removeList)
        {
            buffDebuffs.Remove(id);
        }
    }



    private void SetAction()
    {
        _statContainer.hp.OnValueChanged += OnHpChange;
        _statContainer.turnGauge.OnValueChanged += OnCostChange;
    }

    private void SetHealthContent()
    {
        healthContent = PoolManager.Inst.Spawn<HealthContent>();
        healthContent.Init(_statContainer);
       
      
    }

    private void LateUpdate()
    {
        if(healthContent != null)
             healthContent.SetPos(tile.GetPos());
        ApplicationManager.Inst.GetModule<SkillStackController>().PositionedOnCamera(tile);
        ApplicationManager.Inst.GetModule<BuffStackController>().PositionedOnCamera(tile);
    }

    public List<int> GetSkillList()
    {
        if(bringSkills.Count <= 0 || bringSkills==null)Debug.LogError("NoSkills In Unit");
        return bringSkills;
    }

    public Team GetTeam() => team;
    public Tile GetTile() => tile;
    public StatContainer GetStatContainer() => _statContainer;
    public BuffDebuff GetBuffDebuff(string id) => buffDebuffs[id];
    public ActionContainer GetActionContainer() => actionContainer;
    public Animator GetAnimator() => animator;
    
    public UnitSaveData GetUnitData() => unitData;

    public void SetBringSkills(List<int> skills)
    {
        bringSkills = skills;
       // unitController.OverWriteBringSkills(bringSkills);
    }

    public void AddBuff(string key,BuffDebuff buffDebuff)
    {
        if (buffDebuffs.ContainsKey(key))
        {
            buffDebuffs[key].InitExtraData( buffDebuff);
        }
        else
        {
            buffDebuffs[buffDebuff.id]=buffDebuff;   
            ApplicationManager.Inst.GetModule<BuffStackController>().StackBuff(buffDebuff);
        }
       
    }
    public void GetDamage(float damage,SkillContext skillContext,SkillType skillType)
    {
        if(isDead) return;
        Debug.Log($"{damage} 만큼 데미지를 받음");
        if (damage > 0)
        {
            var remainDamage = damage - _statContainer.barrier._baseValue;
            _statContainer.barrier.AddBaseValue(-damage);
            if(remainDamage > 0)
                _statContainer.hp.AddBaseValue( -remainDamage);
            skillContext.SourceUnit.GetActionContainer().attackAction?.Invoke(skillContext);
            
            actionContainer.hurtAction?.Invoke(skillContext,skillType);
        }
        else
        {
            _statContainer.hp.AddBaseValue( -damage);
            actionContainer.healAction?.Invoke(skillContext);
        }
        ApplicationManager.Inst.GetModule<PopUpUIController>().SpawnDamagePopUp(damage,transform);
    }
    
    private void OnHpChange(float value)
    {
        if (value <= 0)
        {
            isDead = true;
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
        PoolManager.Inst.Despawn(healthContent);
        ApplicationManager.Inst.GetModule<SkillStackController>().UnstackAllSkill(tile);
        ApplicationManager.Inst.GetModule<SkillTurnCounterController>().DequeueByTile(tile);
        ApplicationManager.Inst.GetModule<BuffStackController>().UnstackAllBuffs(tile);
        DataManager.Inst.DeleteUnit(unitData.constId);
        Destroy(gameObject);
    }
}
