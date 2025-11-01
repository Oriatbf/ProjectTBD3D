using UnityEngine;
using VInspector;

public enum Team{PlayerTeam,EnemyTeam,neutralityTeam } //neutrality == 중립

public class Unit : MonoBehaviour
{
    [Foldout("Debug")]
    [SerializeField]private StatContainer _statContainer = new StatContainer();
    [EndFoldout]
    
    private Team team; 
    private int tileIndex;
    private Animator animator;
    private UnitData.Data unitData;
    private UnitController unitController;
    string animatorPath = "Assets/Art/Animator/";
    
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
    
    public void Init(UnitData.Data unitData, Team team)
    {
        _statContainer = new StatContainer(unitData);
        unitController.Init(unitData);
        this.unitData = unitData;
        this.team = team;
    }
    
    public Team GetTeam() => team;
    public StatContainer GetStatContainer() => _statContainer;
    public Animator GetAnimator() => animator;

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
        // PopUpUIManager.Inst.SpawnDamagePopUp(damage,transform );
    }
    private void OnHpChange(float value)
    {
        if (value <= 0)
        {
           //사망
        }
    }

    private void OnCostChange(float value)
    {
        if(value<=0)Debug.Log("cost가 0임");
    }



 
}
