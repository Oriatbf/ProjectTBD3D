using System.Collections.Generic;
using _Project.Script.Relic;
using Core.Utility;
using Cysharp.Threading.Tasks;
using SkillData;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Project.Script.Controller
{
   public class RelicController : BaseController
    {
        private List<RelicBase> relics = new List<RelicBase>();
        private List<IntervalRelicEffect> intervalEffects = new List<IntervalRelicEffect>();
        private List<PassiveRelicEffect> passiveEffects = new List<PassiveRelicEffect>();

        public override ControllerInfo ControllerInfo { get; } = new()
        {
            ContainSceneNames = new string[] {"GamePlay" },
            Priority = 0,
            UpdateInterval = 1,  
            LateUpdateInterval = 0,
            FixedUpdateInterval = 0,
        };

        public override void OnInitialize()
        {
            base.OnInitialize();
            var relicIds = DataManager.Inst.GetRelicSaveData().relicIDList;
            relics = SheetDataManager.Inst.GetRelicDataByIds(relicIds);

            // 특수 효과들 분류
            CategorizeEffects();
            
            // 패시브 효과 적용
            ApplyPassiveEffects();

            // 이벤트 리스너 등록
            RegisterEventListeners();
        }

        /// <summary>
        /// 효과 타입별로 분류
        /// </summary>
        private void CategorizeEffects()
        {
            foreach (var relic in relics)
            {
                foreach (var effect in relic.GetEffects())
                {
                    if (effect is IntervalRelicEffect intervalEffect)
                        intervalEffects.Add(intervalEffect);
                    
                    if (effect is PassiveRelicEffect passiveEffect)
                        passiveEffects.Add(passiveEffect);
                }
            }
        }

        /// <summary>
        /// 패시브 효과 적용
        /// </summary>
        private void ApplyPassiveEffects()
        {
            foreach (var passiveEffect in passiveEffects)
            {
                passiveEffect.ApplyPassive();
            }
        }

        /// <summary>
        /// 이벤트 리스너 등록
        /// </summary>
        private void RegisterEventListeners()
        {
            /*
            // 유닛 사망 이벤트
            EventManager.Inst.AddListener<UnitBase>("OnUnitDeath", OnUnitDeath);
            
            // 유닛 피격 이벤트
            EventManager.Inst.AddListener<UnitBase, float>("OnUnitDamaged", OnUnitDamaged);
            
            // 유닛 공격 이벤트
            EventManager.Inst.AddListener<UnitBase, UnitBase>("OnUnitAttack", OnUnitAttack);
            
            // 스킬 사용 이벤트
            EventManager.Inst.AddListener<UnitBase, SkillBase>("OnSkillUse", OnSkillUse);
            
            // 유닛 이동 이벤트
            EventManager.Inst.AddListener<UnitBase, TileBase>("OnUnitMove", OnUnitMove);
            */        
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            
            // 인터벌 효과 업데이트
            foreach (var intervalEffect in intervalEffects)
            {
                intervalEffect.Update(Time.deltaTime);
            }
        }

        /// <summary>
        /// 전투 시작 시 유물 효과 실행
        /// </summary>
        public async UniTask ExecuteAllRelicOnBattleStart()
        {
            await UniTask.WaitForSeconds(1f);
            
            var context = new RelicEffectContext();
            
            foreach (var relic in relics)
            {
                if(relic.GetTriggerType() == RelicTriggerType.BattleStart)
                    relic.Execute(context);
            }
            
            await UniTask.WaitForSeconds(0.5f);
        }

        /// <summary>
        /// 턴 시작 시 실행
        /// </summary>
        public void ExecuteOnTurnStart()
        {
            var context = new RelicEffectContext();
            foreach (var relic in relics)
            {
                 if(relic.GetTriggerType() == RelicTriggerType.TurnStart)
                    relic.Execute(context);
            }
        }

        /// <summary>
        /// 유닛 사망 이벤트 핸들러
        /// </summary>
        private void OnUnitDeath(Unit unit)
        {
            var context = new RelicEffectContext
            {
                TargetUnit = unit
            };

            foreach (var relic in relics)
            {
                if(relic.GetTriggerType() == RelicTriggerType.OnUnitDeath)
                    relic.Execute(context);
            }

            // 적 처치 시
            if (unit.GetTeam() == Team.EnemyTeam)
            {
                foreach (var relic in relics)
                {
                    if(relic.GetTriggerType() == RelicTriggerType.OnEnemyKill)
                        relic.Execute(context);
                }
            }
        }

        /// <summary>
        /// 유닛 피격 이벤트 핸들러
        /// </summary>
        private void OnUnitDamaged(Unit unit, float damage)
        {
            var context = new RelicEffectContext
            {
                TargetUnit = unit,
                DamageAmount = damage
            };

            foreach (var relic in relics)
            {
                if(relic.GetTriggerType() == RelicTriggerType.OnUnitDamaged)
                    relic.Execute(context);
            }
        }

        /// <summary>
        /// 유닛 공격 이벤트 핸들러
        /// </summary>
        private void OnUnitAttack(Unit attacker, Unit target)
        {
            var context = new RelicEffectContext
            {
                SourceUnit = attacker,
                TargetUnit = target
            };

            foreach (var relic in relics)
            {
                if(relic.GetTriggerType() == RelicTriggerType.OnUnitAttack)
                    relic.Execute(context);
            }
        }

        
        

        /// <summary>
        /// 전투 종료 시 정리
        /// </summary>
        public  void OnFinalize()
        {
            /*
            // 패시브 효과 제거
            foreach (var passiveEffect in passiveEffects)
            {
                passiveEffect.RemovePassive();
            }

            // 모든 유물 리셋
            foreach (var relic in relics)
            {
                relic.Reset();
            }

            // 이벤트 리스너 해제
            EventManager.Inst.RemoveListener<UnitBase>("OnUnitDeath", OnUnitDeath);
            EventManager.Inst.RemoveListener<UnitBase, float>("OnUnitDamaged", OnUnitDamaged);
            EventManager.Inst.RemoveListener<UnitBase, UnitBase>("OnUnitAttack", OnUnitAttack);
            EventManager.Inst.RemoveListener<UnitBase, SkillBase>("OnSkillUse", OnSkillUse);
            EventManager.Inst.RemoveListener<UnitBase, TileBase>("OnUnitMove", OnUnitMove);

            base.OnFinalize();
            */
        }

        // 기존 호환성을 위한 메서드
        public async UniTask ExcuteAllRelic()
        {
            await ExecuteAllRelicOnBattleStart();
        }
    }
}