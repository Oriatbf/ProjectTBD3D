using UnityEngine;

namespace SkillData.SkillEffects
{
    public static class ActionStateExamples
    {
        public static void DamageBuff(Unit targetUnit,int _stack)
        {
            var unit = targetUnit;
            ActionData damageBuffData = new ActionData(
                id: "DamageBuff",
                owner: unit,
                stack: _stack,
                turn: 999,
                decreaseType: DecreaseType.None,
                targetType: ActionTargetType.Self,
                buffType: SkillType.Buff
            );

            damageBuffData.action = (data, context) =>
            {
                unit.GetStatContainer().str.AddModifier(new StatModifier(EStatModifier.Add, data.stack));
                Debug.Log("데미지 증가 실행");
                data.isExist = false;
            };

            ActionState damageBuffState = new ActionState(damageBuffData);
            unit.GetActionStateContainer().AddActionState(
                ActionTrigger.None, damageBuffState);
            ApplicationManager.Inst.GetModule<ActionStateStackController>().StackAction(ActionTrigger.None,damageBuffState);
        }

        public static void FaintingBuff(Unit targetUnit,int turn)
        {
            var unit = targetUnit;
            ActionData faintingBuff = new ActionData(
                id: "FaintingBuff",
                owner: unit,
                stack: 0,
                turn: turn,
                decreaseType: DecreaseType.OnlyTurn,
                targetType: ActionTargetType.Self,
                buffType: SkillType.Debuff
            );

            faintingBuff.action = (data, context) =>
            {
                unit.GetStatContainer().isStun = true;
            };

            faintingBuff.finishAction = (data) =>
            {
                unit.GetStatContainer().isStun = false;
            };

            ActionState faintingBuffState = new ActionState(faintingBuff);
            unit.GetActionStateContainer().AddActionState(
                ActionTrigger.OnTurnEnd, faintingBuffState);
            ApplicationManager.Inst.GetModule<ActionStateStackController>().StackAction(ActionTrigger.OnTurnEnd,faintingBuffState);
        }
        
            /// <summary>
        /// 예시 1: 3턴 동안 턴 종료 시 적 전체에게 1의 데미지
        /// </summary>
        public static void Example_BurnDamage(Unit ownerUnit)
        {
            // ActionData 생성
            ActionData burnData = new ActionData(
                id: "Burn",
                owner: ownerUnit,
                stack: 1,           // 1의 데미지
                turn: 3,            // 3턴 동안
                decreaseType: DecreaseType.OnlyTurn,
                targetType: ActionTargetType.AllEnemy,
                buffType: SkillType.Debuff
            );

            // 실행할 액션 정의
            burnData.action = (data, context) =>
            {
                // 타겟에게 스택만큼 데미지
                context.TargetUnit?.GetDamage(data.stack, context, SkillType.Attack);
                Debug.Log($"화상 데미지: {data.stack}");
            };

            // 종료 시 액션
            burnData.finishAction = (data) =>
            {
                Debug.Log("화상이 끝났습니다.");
            };

            // ActionState 생성 및 추가
            ActionState burnState = new ActionState(burnData);
            ownerUnit.GetActionStateContainer().AddActionState(ActionTrigger.OnTurnEnd, burnState);
        }

        /// <summary>
        /// 예시 2: 3번 공격을 반격, 3턴 동안 유지
        /// </summary>
        public static void Example_Counter(Unit ownerUnit)
        {
            ActionData counterData = new ActionData(
                id: "Counter",
                owner: ownerUnit,
                stack: 3,           // 3번 반격
                turn: 3,            // 3턴 유지
                decreaseType: DecreaseType.TurnAndStack,  // 둘다 감소
                targetType: ActionTargetType.Attacker,     // 공격자에게
                buffType: SkillType.Buff
                
            );

            counterData.action = (data, context) =>
            {
                // 공격자에게 반격 데미지
                if (context.SourceUnit != null)
                {
                    float counterDamage = ownerUnit.GetStatContainer().str._baseValue * 0.5f;
                    context.SourceUnit.GetDamage(counterDamage, context, SkillType.Attack);
                    Debug.Log($"반격! {counterDamage} 데미지");
                }
            };

            counterData.finishAction = (data) =>
            {
                Debug.Log("반격 효과가 종료되었습니다.");
            };

            ActionState counterState = new ActionState(counterData);
            ownerUnit.GetActionStateContainer().AddActionState(ActionTrigger.OnHitted, counterState);
        }

        /// <summary>
        /// 예시 3: 매 턴 시작 시 자신을 2 힐
        /// </summary>
        public static void Example_Regeneration(Unit ownerUnit)
        {
            ActionData regenData = new ActionData(
                id: "Regeneration",
                owner: ownerUnit,
                stack: 2,           // 2 힐
                turn: 5,            // 5턴 동안
                decreaseType: DecreaseType.OnlyTurn,
                targetType: ActionTargetType.Self,
                buffType: SkillType.Buff
            );

            regenData.action = (data, context) =>
            {
                context.TargetUnit?.GetDamage(-data.stack, context, SkillType.Utility);
                Debug.Log($"재생: {data.stack} 회복");
            };

            ActionState regenState = new ActionState(regenData);
            ownerUnit.GetActionStateContainer().AddActionState(ActionTrigger.OnTurnStart, regenState);
        }

        /// <summary>
        /// 예시 4: 공격 시 랜덤 적 2명에게 추가 데미지
        /// </summary>
        public static void Example_SplashDamage(Unit ownerUnit)
        {
            ActionData splashData = new ActionData(
                id: "Splash",
                owner: ownerUnit,
                stack: 3,           // 3의 추가 데미지
                turn: 999,          // 무한 지속
                decreaseType: DecreaseType.None,
                targetType: ActionTargetType.RandomEnemy,
                buffType: SkillType.Buff
            );
            splashData.randomCount = 2; // 랜덤 2명

            splashData.action = (data, context) =>
            {
                context.TargetUnit?.GetDamage(data.stack, context, SkillType.Attack);
                Debug.Log($"스플래시 데미지: {data.stack}");
            };

            ActionState splashState = new ActionState(splashData);
            ownerUnit.GetActionStateContainer().AddActionState(ActionTrigger.OnAttack, splashState);
        }

        /// <summary>
        /// 예시 5: 독 (스택만 감소, 맞을때마다 1씩 감소)
        /// </summary>
        public static void Example_Poison(Unit ownerUnit, int poisonStack)
        {
            ActionData poisonData = new ActionData(
                id: "Poison",
                owner: ownerUnit,
                stack: poisonStack,  // 독 스택
                turn: 0,
                decreaseType: DecreaseType.OnlyStack,
                targetType: ActionTargetType.Self,
                buffType: SkillType.Debuff
            );

            poisonData.action = (data, context) =>
            {
                // 독 데미지 (현재 스택만큼)
                context.TargetUnit?.GetDamage(data.stack, context, SkillType.Attack);
                Debug.Log($"독 데미지: {data.stack}");
            };

            poisonData.finishAction = (data) =>
            {
                Debug.Log("독이 완전히 해소되었습니다.");
            };

            ActionState poisonState = new ActionState(poisonData);
            ownerUnit.GetActionStateContainer().AddActionState(ActionTrigger.OnTurnEnd, poisonState);
        }

        public static void BloodSuck(Unit ownerUnit)
        {
            ActionData bloodSuckData = new ActionData(
                id: "bloodSuck",
                owner: ownerUnit,
                stack: 1,           
                turn: 1,
                decreaseType: DecreaseType.OnlyTurn,
                targetType: ActionTargetType.Attacker,
                buffType: SkillType.Buff
            );

            bloodSuckData.action = (data, context) =>
            {
                context.SourceUnit?.GetDamage(-data.stack, context, SkillType.Utility);
            };

            bloodSuckData.finishAction = (data) =>
            {
                Debug.Log("bloodSuck버프가 끝남");
            };

            ActionState bloodSuckState = new ActionState(bloodSuckData);
            ownerUnit.GetActionStateContainer().AddActionState(ActionTrigger.OnAttack, bloodSuckState);
        }

        /// <summary>
        /// 예시 6: 죽을 때 아군 전체 버프
        /// </summary>
        public static void Example_DeathBuff(Unit ownerUnit)
        {
            ActionData deathBuffData = new ActionData(
                id: "LastWill",
                owner: ownerUnit,
                stack: 5,           // 5 공격력 증가
                turn: 1,
                decreaseType: DecreaseType.None,
                targetType: ActionTargetType.AllAlly,
                buffType: SkillType.Buff
            );

            deathBuffData.action = (data, context) =>
            {
                // 아군에게 공격력 버프
                context.TargetUnit?.GetStatContainer().str.AddModifier(
                    new StatModifier(EStatModifier.Add, data.stack)
                );
                Debug.Log($"유언: 아군에게 {data.stack} 공격력 버프");
            };

            ActionState deathBuffState = new ActionState(deathBuffData);
            ownerUnit.GetActionStateContainer().AddActionState(ActionTrigger.OnDeath, deathBuffState);
        }
    }
}