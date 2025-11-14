using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VInspector;

public enum Team{PlayerTeam,EnemyTeam,neutralityTeam } //neutrality == 중립

public class Unit : MonoBehaviour
{
    [Foldout("Debug")]
    [SerializeField]private StatContainer _statContainer = new StatContainer();
    [SerializeField] private List<int> bringSkills = new List<int>();
    [EndFoldout]
    
    private Team team; 
    private Tile tile; 
    private Animator animator;
    private UnitSaveData unitData;
    private UnitController unitController;
    private HealthContent healthContent;
    string animatorPath = "Assets/_Project/Art/Animator/";
    
    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private void Awake()
    {
        animator = GetComponent<Animator>();
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
        unitController.Init(unitData);
        this.unitData = unitData;
        this.team = team;
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
        healthContent.SetPos(tile.GetPos());
      
    }
    
    public List<int> GetSkillList()=>bringSkills;
    public Team GetTeam() => team;
    public StatContainer GetStatContainer() => _statContainer;
    public Animator GetAnimator() => animator;
    
    public UnitSaveData GetUnitData() => unitData;

    public void GetDamage(float damage)
    {
        Debug.Log($"{damage} 만큼 데미지를 받음");
        if (damage > 0)
        {
            var remainDamage = damage - _statContainer.barrier._baseValue;
            _statContainer.barrier.AddBaseValue(-damage);
            if(remainDamage > 0)
                _statContainer.hp.AddBaseValue( -remainDamage);
        }
        else
        {
            _statContainer.hp.AddBaseValue( -damage);
        }
        ApplicationManager.Inst.GetModule<PopUpUIController>().SpawnDamagePopUp(damage,transform);
    }
    private void OnHpChange(float value)
    {
        if (value <= 0)
        {
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
        Destroy(gameObject);
    }



 
}
