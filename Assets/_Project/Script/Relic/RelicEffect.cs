using System.Collections.Generic;
using UnityEngine;

namespace _Project.Script.Relic
{
  /// <summary>
    /// 기본 유물 효과 추상 클래스 - 확장성 강화
    /// </summary>
    public abstract class RelicEffect
    {
        protected List<int> values = new List<int>();
        protected RelicTriggerType triggerType;
        protected RelicTargetType targetType;
        
        // 효과 발동 조건
        protected float cooldown = 0f;           // 쿨타임
        protected float lastTriggerTime = 0f;    // 마지막 발동 시간
        protected int maxTriggerCount = -1;      // 최대 발동 횟수 (-1은 무제한)
        protected int currentTriggerCount = 0;   // 현재 발동 횟수
        protected float probability = 1f;        // 발동 확률 (0~1)

        public RelicTriggerType TriggerType => triggerType;
        public RelicTargetType TargetType => targetType;

        public virtual void Init(List<int> _values)
        {
            values = _values;
            OnInit();
        }

        /// <summary>
        /// 초기화 시 추가 설정이 필요한 경우 override
        /// </summary>
        protected virtual void OnInit() { }

        /// <summary>
        /// 효과 발동 가능 여부 체크
        /// </summary>
        public virtual bool CanExecute(RelicEffectContext context)
        {
            // 쿨타임 체크
            if (cooldown > 0 && Time.time - lastTriggerTime < cooldown)
                return false;

            // 최대 발동 횟수 체크
            if (maxTriggerCount > 0 && currentTriggerCount >= maxTriggerCount)
                return false;

            // 확률 체크
            if (probability < 1f && Random.value > probability)
                return false;

            return CheckCustomCondition(context);
        }

        /// <summary>
        /// 커스텀 조건 체크 - 각 유물별로 override
        /// </summary>
        protected virtual bool CheckCustomCondition(RelicEffectContext context)
        {
            return true;
        }

        /// <summary>
        /// 효과 실행
        /// </summary>
        public void Execute(RelicEffectContext context)
        {
            if (!CanExecute(context))
                return;

            lastTriggerTime = Time.time;
            currentTriggerCount++;

            OnExecute(context);
        }

        /// <summary>
        /// 실제 효과 로직 - 각 유물별로 override
        /// </summary>
        protected abstract void OnExecute(RelicEffectContext context);

        /// <summary>
        /// 유물 설명 반환
        /// </summary>
        public abstract string ReturnInformation();

        /// <summary>
        /// 효과 리셋 (전투 종료 시 등)
        /// </summary>
        public virtual void Reset()
        {
            currentTriggerCount = 0;
            lastTriggerTime = 0f;
        }
    }
  
    /// <summary>
    /// 조건부 효과를 위한 베이스 클래스
    /// </summary>
    public abstract class ConditionalRelicEffect : RelicEffect
    {
        protected ConditionalRelicEffect()
        {
            triggerType = RelicTriggerType.Conditional;
        }
    }

    /// <summary>
    /// 인터벌 효과를 위한 베이스 클래스
    /// </summary>
    public abstract class IntervalRelicEffect : RelicEffect
    {
        protected float interval;
        private float timer;

        protected IntervalRelicEffect(float _interval)
        {
            triggerType = RelicTriggerType.Interval;
            interval = _interval;
        }

        public void Update(float deltaTime)
        {
            timer += deltaTime;
            if (timer >= interval)
            {
                timer = 0f;
                Execute(new RelicEffectContext());
            }
        }
    }

    /// <summary>
    /// 패시브 효과를 위한 베이스 클래스
    /// </summary>
    public abstract class PassiveRelicEffect : RelicEffect
    {
        protected PassiveRelicEffect()
        {
            triggerType = RelicTriggerType.Passive;
        }

        public abstract void ApplyPassive();
        public abstract void RemovePassive();
    }
}