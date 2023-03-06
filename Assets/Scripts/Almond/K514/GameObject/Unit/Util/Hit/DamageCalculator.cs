using System.Linq;
using UI2020;
using UnityEngine;

namespace k514
{
    public static class DamageCalculator
    {
        #region <Consts>

        private const float DefaultCriticalDamageRate = 2f;
        private const float DamageLowerBound = 1;
        private const int AttributeEnhance10PercentBoundValue = 255;
        private const float _Inverse_AttributeEnhance10PercentBoundValue = 1f / AttributeEnhance10PercentBoundValue;

        #endregion

        #region <Enums>

        /// <summary>
        /// 공격 계산 타입
        /// </summary>
        public enum DamageCalcType
        {
            /// <summary>
            /// 물리 공격
            /// </summary>
            Physic,

            /// <summary>
            /// 마법 공격
            /// </summary>
            Magic
        }

        #endregion

        #region <Methods>

        private static float CalcAttributeDamageMultiplyRate(float p_AttributeEnhanceValue)
        {
            return 1f + p_AttributeEnhanceValue * _Inverse_AttributeEnhance10PercentBoundValue;
        }

        public static (UnitTool.UnitAttributeType, float) CalcAttributeDamageMultiplyRate(Unit p_Trigger, Unit p_Target, bool p_TargetValid, UnitTool.UnitAttributeType p_HitAttributeType)
        {
            // 공격 타입을 합쳐준다.
            // var strikeAttribute = p_TargetValid ? p_HitAttributeType | p_Trigger._UnitStrikeAttributeType : p_HitAttributeType;
            var strikeAttribute = p_HitAttributeType;

            // 합쳐진 공격 타입이 무속성 공격이었던 경우
            if (strikeAttribute == UnitTool.UnitAttributeType.None)
            {
                // 무속성 공격에, 공격력 보정은 100퍼센트가 된다.
                return (UnitTool.UnitAttributeType.None, 1f);
            }
            // 공격 타입이 무속성 이외였던 경우
            else
            {
                // 복합속성일지도 모르므로, 각 속성의 강화/저항값을 고려하여 가장 강력한 속성 타입 및 그 데미지 배율을 선정하여
                // 리턴해준다.
                var attributeEnumerator = UnitTool._UnitAttributeTypeEnumerator;
                var (maxGabAttribute, maxGab) = (UnitTool.UnitAttributeType.None, float.MinValue);
                /*foreach (var attributeType in attributeEnumerator)
                {
                    var gab = 0f;

                    // 각 속성 타입에 대해
                    switch (attributeType)
                    {
                        case UnitTool.UnitAttributeType.Fire:
                        {
                            var fireEnhance = p_Trigger._BattleStatusPreset.t_Current.GetFireElementalEnhance();
                            var fireResist = p_TargetValid ? p_Target._BattleStatusPreset.t_Current.GetFireElementalResist() : 0f;
                            gab = fireEnhance - fireResist;
                        }
                            break;
                        case UnitTool.UnitAttributeType.Water:
                        {
                            var waterEnhance = p_Trigger._BattleStatusPreset.t_Current.GetWaterElementalEnhance();
                            var waterResist = p_TargetValid ? p_Target._BattleStatusPreset.t_Current.GetWaterElementalResist() : 0f;
                            gab = waterEnhance - waterResist;
                        }
                            break;
                        case UnitTool.UnitAttributeType.Light:
                        {
                            var lightEnhance = p_Trigger._BattleStatusPreset.t_Current.GetLightElementalEnhance();
                            var lightResist = p_TargetValid ? p_Target._BattleStatusPreset.t_Current.GetLightElementalResist() : 0f;
                            gab = lightEnhance - lightResist;
                        }
                            break;
                        case UnitTool.UnitAttributeType.Darkness:
                        {
                            var darknessEnhance = p_Trigger._BattleStatusPreset.t_Current.GetDarkElementalEnhance();
                            var darknessResist = p_TargetValid ? p_Target._BattleStatusPreset.t_Current.GetDarkElementalResist() : 0f;
                            gab = darknessEnhance - darknessResist;
                        }
                            break;
                    }

                    // 가장 높은 값으로 보정해준다.
                    if (gab > maxGab)
                    {
                        maxGab = gab;
                        maxGabAttribute = attributeType;
                    }
                }*/

                return (maxGabAttribute, CalcAttributeDamageMultiplyRate(maxGab));
            }
        }

        /// <summary>
        /// 데미지 기본 공식 : 기본 데미지 = 공격자 물리/마법 공격 계수 * 스킬 퍼센트 계수 + 스킬 고정 계수
        /// 데미지만큼 피격 유닛의 체력을 깎고, 실제 깎인 만큼분의 수치를 리턴한다.
        /// </summary>
        public static HitResult CalcDamage(this Unit p_Target, Unit p_Trigger, HitMessage p_HitMessage, UnitTool.UnitAddForcePreset p_HitVariablePreset)
        {
            return p_Target.CalcDamage(p_Trigger, !ReferenceEquals(null, p_Trigger), p_HitMessage, p_HitVariablePreset);
        }

        /// <summary>
        /// 데미지 기본 공식 : 기본 데미지 = 공격자 물리/마법 공격 계수 * 스킬 퍼센트 계수 + 스킬 고정 계수
        /// 데미지만큼 피격 유닛의 체력을 깎고, 실제 깎인 만큼분의 수치를 리턴한다.
        /// </summary>
        public static HitResult CalcDamage(this Unit p_Target, Unit p_Trigger, bool p_IsTriggerValid, HitMessage p_HitMessage, UnitTool.UnitAddForcePreset p_HitVariablePreset)
        {
            return CalcDamage(p_Target, p_Trigger, p_IsTriggerValid, p_HitMessage, p_HitVariablePreset, HitResult.HitResultFlag.None);
        }

        /// <summary>
        /// 타격 이벤트의 확률 변수가 확정되어있는 경우 데미지 계산 함수
        /// </summary>
        public static HitResult CalcDamage(Unit p_Target, Unit p_Trigger, bool p_IsTriggerValid, HitMessage p_HitMessage, UnitTool.UnitAddForcePreset p_HitVariablePreset, HitResult.HitResultFlag p_HitResultFlag)
        {
            var hitResultType = UnitHitTool.HitResultType.HitFail;
            var resultDamage = default(float);
            var resultForceVector = Vector3.zero;
            var damageCalcType = p_HitMessage._DamageCalcType;
            var triggerMotion = p_HitMessage.HitPreset.MotionType;

/*#if UNITY_EDITOR
            if (CustomDebug.PrintUnitHitLog)
            {
                if (p_HitMessage._HitStateFlag.HasAnyFlagExceptNone(HitMessage.HitMessageAttributeFlag.HasHitParameter))
                {
                    Debug.Log($"[CalcDamage] : {p_Trigger?.GetUnitName()} => {p_Target?.GetUnitName()} / {p_HitMessage._HitStateFlag} / {p_HitMessage.HitParameter.HitParameterType}");
                }
                else
                {
                    Debug.Log($"[CalcDamage] : {p_Trigger?.GetUnitName()} => {p_Target?.GetUnitName()} / {p_HitMessage._HitStateFlag}");
                }
            }
#endif
            
            /* SE Cond 데미지 계산 조건 #1#
            // 1. 히트 메시지가 데미지 계수를 유효하게 가지는 경우
            // 2. 히트 메시지가 데미지 0 고정 플래그를 가지지 않는 경우
            // 3. 피격 유닛이 Immortal 상태가 아닌 경우
            var hasTouchable = p_HitMessage._HitStateFlag.HasAnyFlagExceptNone(HitMessage.HitMessageAttributeFlag.HasDamageFactor)
                               && !p_HitMessage._HitStateFlag.HasAnyFlagExceptNone(HitMessage.HitMessageAttributeFlag.ZeroZeroCall)
                               && !p_Target.HasState_Or(Unit.UnitStateType.IMMORTAL);

            // 타격 판정 참가 유닛들의 전투치
            var strikerBattleStatus = p_IsTriggerValid ? p_Trigger._BattleStatusPreset.t_Current : default;
            var strikerSkillStatus = p_IsTriggerValid ? p_Trigger._SkillStatusPreset.t_Current : default;
            var victimBattleStatus = p_Target._BattleStatusPreset.t_Current;

            // 타격 이벤트 확률변수를 계산한다.
            var hitResultFlagMask = p_HitResultFlag == HitResult.HitResultFlag.None 
                ? p_HitMessage.CalcHitPossibilityFlag(strikerBattleStatus, victimBattleStatus, p_Trigger, p_Target)
                : p_HitResultFlag;

            // 이벤트 결과에 회피 플래그가 포함된 경우
            if (hitResultFlagMask.HasAnyFlagExceptNone(HitResult.HitResultFlag.Miss))
            {
                hitResultType = UnitHitTool.HitResultType.HitMissed;
                return new HitResult(hitResultType, p_Trigger, p_Target, 0f, hitResultFlagMask, p_HitMessage, resultAttribute, resultForceVector);
            }
            // 포함되지 않은 경우, 데미지 계산을 수행한다.
            else
            {
                // 이벤트 결과에 크리티컬 플래그가 포함되어있는지
                hitResultType = hitResultFlagMask.HasAnyFlagExceptNone(HitResult.HitResultFlag.Critical)
                    ? UnitHitTool.HitResultType.HitCritical
                    : UnitHitTool.HitResultType.HitNormal;

                // 공격력, 방어력 등의 전투치 계산
                if (hasTouchable)
                {
                    hitResultFlagMask.AddFlag(HitResult.HitResultFlag.HasDamage);

                    var negateDefense =
                        p_HitMessage._HitStateFlag.HasAnyFlagExceptNone(
                            HitMessage.HitMessageAttributeFlag.NegateDefense);
                    if (p_IsTriggerValid)
                    {
                        var defense = negateDefense
                            ? 0f
                            : victimBattleStatus.GetDefenseBase(damageCalcType)
                              * victimBattleStatus.GetDefenseMultiplyRate(damageCalcType);

                        if (triggerMotion == AnimatorParamStorage.MotionType.Kick)
                        {
                            // 최종 데미지 = 고정 데미지 + 공격자 공격력 * 공격자 공격력 증가율 * 퍼센트 데미지 계수 + 스킬 추가 데미지 - 피격자 방어력
                            resultDamage = p_HitMessage._FixedDamageParameter
                                           + strikerBattleStatus.GetAttackBase(damageCalcType)
                                           * strikerBattleStatus.GetAttackMultiplyRate(damageCalcType)
                                           * p_HitMessage._PercentDamageParameter + strikerSkillStatus.GetSkillDamage()
                                           - defense;
#if UNITY_EDITOR
                            if (CustomDebug.PrintGameSystemLog)
                            {
                                Debug.Log($"[{p_Trigger}] => [{p_Target}] 고정 데미지({p_HitMessage._FixedDamageParameter}) + 공격자 공격력({strikerBattleStatus.GetAttackBase(damageCalcType)}) * 공격자 공격력 증가율({strikerBattleStatus.GetAttackMultiplyRate(damageCalcType)}) * (퍼센트 데미지 계수({p_HitMessage._PercentDamageParameter}) + 스킬 데미지 증가율({strikerSkillStatus.GetSkillDamage()})) - 피격자 방어력({defense})");
                                Debug.Log($"[{p_Trigger}] => [{p_Target}] 총합 : {resultDamage}(GetDamageMultiplyRate : {strikerBattleStatus.GetDamageMultiplyRate(true)})");
                            }
#endif
                        }
                        else
                        {
                            // 최종 데미지 = 고정 데미지 + 공격자 공격력 * 공격자 공격력 증가율 * 퍼센트 데미지 계수 - 피격자 방어력
                            resultDamage = p_HitMessage._FixedDamageParameter
                                           + strikerBattleStatus.GetAttackBase(damageCalcType)
                                           * strikerBattleStatus.GetAttackMultiplyRate(damageCalcType)
                                           * p_HitMessage._PercentDamageParameter 
                                           - defense;
                            
#if UNITY_EDITOR
                            if (CustomDebug.PrintGameSystemLog)
                            {
                                Debug.Log($"[{p_Trigger}] => [{p_Target}] 고정 데미지({p_HitMessage._FixedDamageParameter}) + 공격자 공격력({strikerBattleStatus.GetAttackBase(damageCalcType)}) * 공격자 공격력 증가율({strikerBattleStatus.GetAttackMultiplyRate(damageCalcType)}) * 퍼센트 데미지 계수({p_HitMessage._PercentDamageParameter}) - 피격자 방어력({defense})");
                                Debug.Log($"[{p_Trigger}] => [{p_Target}] 총합 : {resultDamage}(GetDamageMultiplyRate : {strikerBattleStatus.GetDamageMultiplyRate(true)})");
                            }
#endif
                        }
                    }
                    else
                    {
                        var defense = negateDefense
                            ? 0f
                            : victimBattleStatus.GetDefenseBase(damageCalcType)
                              * victimBattleStatus.GetDefenseMultiplyRate(damageCalcType);

                        // 최종 데미지 = 고정 데미지 - 피격자 방어력
                        resultDamage = p_HitMessage._FixedDamageParameter - defense;
                    }

                    float levelAntiDamageRate = 1;
                    // 레벨 차이에 의한 데미지 감소(16레벨 부터)
                    if (p_Target.GetCurrentLevel() > 15)
                    {
                        for (var levelGap = p_Trigger.GetCurrentLevel() - p_Target.GetCurrentLevel(); levelGap < 0; levelGap++)
                        {
                            if (levelGap > -6)
                            {
                                levelAntiDamageRate -= 0.01f;
                            }
                            else if (-11 < levelGap &&  levelGap < -5)
                            {
                                levelAntiDamageRate -= 0.02f;
                            }
                            else
                            {
                                levelAntiDamageRate -= 0.03f;
                            }
                        }
                    }

                    // 곱연산을 하기 전에, 양수 보정을 해준다.
                    resultDamage = Mathf.Max(0, resultDamage);
                    
                    resultDamage *= levelAntiDamageRate;
                    
#if UNITY_EDITOR
                    if (CustomDebug.PrintGameSystemLog)
                    {
                        Debug.Log($"[{p_Trigger}] => [{p_Target}] 총합 : {resultDamage} (레벨 차이에 의한 데미지 감소율 : {levelAntiDamageRate * 100}%)");
                    }
#endif

                    // 피해 증가량을 적용시켜준다.
                    resultDamage *= strikerBattleStatus.GetDamageMultiplyRate(true);
                    resultDamage *= victimBattleStatus.GetAntiDamageMultiplyRate(true);


#if !SERVER_DRIVE
                    //영혼뿔 테스트
                    //if (p_Trigger._AllyMask.Equals(Unit.UnitAllyFlagType.Player))
                    //    LamiereGameManager.GetInstanceUnSafeUnSafe._ClientPlayer.UsedSoulHorn();
#endif
                }

                {
                    // HitExtra 파라미터를 적용시켜준다.
                    var hitExtraTuple = p_HitMessage.GetHitExtraRecord();

                    // 속성을 적용시켜준다.
                    if (hitExtraTuple.Item1)
                    {
                        var hitAttribute = hitExtraTuple.Item2.StrikeAttributeType;
                        var hitAttributeTuple = CalcAttributeDamageMultiplyRate(p_Trigger, p_Target, p_IsTriggerValid, hitAttribute);
                        resultAttribute = hitAttributeTuple.Item1;
                        resultDamage *= hitAttributeTuple.Item2;
                    }
                    else
                    {
                        var hitAttributeTuple = CalcAttributeDamageMultiplyRate(p_Trigger, p_Target, p_IsTriggerValid, UnitTool.UnitAttributeType.None);
                        resultAttribute = hitAttributeTuple.Item1;
                        resultDamage *= hitAttributeTuple.Item2;
                    }

                    // 타격 상태이상을 적용시켜준다.
                    var hitTickPreset = p_HitMessage.GetHitParameter(UnitHitTool.HitParameterType.HitTick);
                    if (hitTickPreset.Item1)
                    {
                        var hitTickRecord = UnitHitTickData.GetInstanceUnSafe[hitTickPreset.Item2];
                        if (hitTickRecord.CalcEventOccur(p_Trigger, p_Target))
                        {
                            hitResultFlagMask.AddFlag(HitResult.HitResultFlag.Tick);
                        }
                    }

                    // 타격 버프를 적용시켜준다.
                    var hitBuffPreset = p_HitMessage.GetHitParameter(UnitHitTool.HitParameterType.HitBuff);
                    if (hitBuffPreset.Item1)
                    {
                        var hitBuffRecord = UnitHitBuffData.GetInstanceUnSafe[hitBuffPreset.Item2];
                        if (hitBuffRecord.CalcEventOccur(p_Trigger, p_Target))
                        {
                            hitResultFlagMask.AddFlag(HitResult.HitResultFlag.Buff);
                        }
                    }

                    // 외력을 적용시켜준다.
                    var hitForceTuple = p_HitMessage.GetHitParameter(UnitHitTool.HitParameterType.HitForce);
                    if (hitForceTuple.Item1)
                    {
                        var hitForceRecord = UnitAddForceData.GetInstanceUnSafe[hitForceTuple.Item2];
                        resultForceVector = UnitTool.GetForceVector(p_Target, p_Trigger, p_IsTriggerValid, p_HitVariablePreset, hitForceRecord.ForceDirectionType, hitForceRecord.Force);
                        hitResultFlagMask.AddFlag(HitResult.HitResultFlag.Force);
                    }
                }

                if (hasTouchable)
                {
                    // 크리티컬 증가데미지를 적용시켜준다.
                    if (hitResultType == UnitHitTool.HitResultType.HitCritical)
                    {
                        if (p_IsTriggerValid)
                        {
                            resultDamage *= DefaultCriticalDamageRate *
                                            strikerBattleStatus.GetCriticalAttackMultiplyRate(damageCalcType) +
                                            strikerSkillStatus.GetCritAttackDamage(damageCalcType);
                        }
                        else
                        {
                            resultDamage *= DefaultCriticalDamageRate;
                        }
                    }
#if SERVER_DRIVE
                    if(p_Target.HasAnyAuthority(UnitTool.UnitAuthorityFlag.OtherPlayer) && p_Trigger.HasAnyAuthority(UnitTool.UnitAuthorityFlag.OtherPlayer))
                    {
                        resultDamage *= 0.25f;
                    }
#else
                    if (p_Target.HasAnyAuthority(UnitTool.UnitAuthorityFlag.OtherPlayer))
                    {
                        resultDamage *= 0.25f;
                    }
#endif

#if UNITY_EDITOR
                    if (CustomDebug.PrintGameSystemLog)
                    {
                        Debug.Log($"[{p_Trigger}] => [{p_Target}] 최종 주는 피해량 : {resultDamage} (hitResultFlagMask : {hitResultFlagMask} / hitResultType : {hitResultType})");
                    }

                    if (p_IsTriggerValid && p_Trigger.IsPlayer)
                    {
                        if (CustomDebug.FatalFury)
                        {
                            return new HitResult(hitResultType, p_Trigger, p_Target, p_Target._BattleStatusPreset.t_Total.GetHpBase(1f), hitResultFlagMask, p_HitMessage, resultAttribute, resultForceVector);
                        }
                        else if(CustomDebug.Calling20Percent)
                        {
                            return new HitResult(hitResultType, p_Trigger, p_Target, p_Target._BattleStatusPreset.t_Total.GetHpBase(0.2f), hitResultFlagMask, p_HitMessage, resultAttribute, resultForceVector);
                        }
                        else
                        {
                            return new HitResult(hitResultType, p_Trigger, p_Target, Mathf.Max(DamageLowerBound, resultDamage), hitResultFlagMask, p_HitMessage, resultAttribute, resultForceVector);
                        }
                    }
                    else
                    {
                        return new HitResult(hitResultType, p_Trigger, p_Target, Mathf.Max(DamageLowerBound, resultDamage), hitResultFlagMask, p_HitMessage, resultAttribute, resultForceVector);
                    }
#else
                    return new HitResult(hitResultType, p_Trigger, p_Target, Mathf.Max(DamageLowerBound, resultDamage), hitResultFlagMask, p_HitMessage, resultAttribute, resultForceVector); 
#endif
                }
                else
                {
                    return new HitResult(hitResultType, p_Trigger, p_Target, 0f, hitResultFlagMask, p_HitMessage, resultAttribute, resultForceVector); 
                }
            }*/

            return new HitResult(hitResultType, p_Trigger, p_Target, 0f, p_HitResultFlag, p_HitMessage, UnitTool.UnitAttributeType.None, resultForceVector);
        }

        //데미지 검증
        public static bool VerifyDamage(HitResult p_HitResult, ulong hitDamage, ulong hitPresetID)
        {
            var hitMessage = UnitHitPresetData.GetInstanceUnSafe.GetTableData((int)hitPresetID).GetHitMessage();
            var result = CalcDamage(p_HitResult.Target, p_HitResult.Trigger, !ReferenceEquals(null, p_HitResult.Trigger), hitMessage, new UnitTool.UnitAddForcePreset(), p_HitResult.HitResultFlagMask);
            return hitDamage <= result.ResultDamage;
        }
        
        // 상태이상 데미지
        public static HitResult CalcTickDamage(this Unit p_Target, Unit p_Trigger, bool p_IsTriggerValid, HitMessage p_HitMessage)
        {
            // var resultDamage = p_Target._BattleStatusPreset.t_Total.GetHpBase() * p_HitMessage._PercentDamageParameter;
            var resultDamage = 1;
            var hitResultType = UnitHitTool.HitResultType.HitNormal;
            var hitAttributeTuple = CalcAttributeDamageMultiplyRate(p_Trigger, p_Target, p_IsTriggerValid, UnitTool.UnitAttributeType.None);
            var resultAttribute = hitAttributeTuple.Item1;
            
            return new HitResult(hitResultType, p_Trigger, p_Target, Mathf.Max(DamageLowerBound, resultDamage), HitResult.HitResultFlag.HasDamage, p_HitMessage, resultAttribute, Vector3.zero);
        }

        #endregion
    }
}