using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace k514
{
    public struct HitMessage
    {
        #region <Consts>
        
        /// <summary>
        /// 크리티컬 확률 상한
        /// </summary>
        private const float CriticalRateUpperBound = 0.5f;
        
        /// <summary>
        /// 적중률 하한
        /// </summary>
        private const float HitRateLowerBound = 0.05f;

        #endregion
        
        #region <Fields>
        
        /// <summary>
        /// 고정 데미지 계수
        /// </summary>
        public float _FixedDamageParameter;
        
        /// <summary>
        /// 퍼센트 데미지 계수
        /// </summary>
        public float _PercentDamageParameter;

        /// <summary>
        /// 데미지 계산 타입
        /// </summary>
        public DamageCalculator.DamageCalcType _DamageCalcType;
        
        /// <summary>
        /// 히트 상태 플래그
        /// </summary>
        public HitMessageAttributeFlag _HitStateFlag;

        /// <summary>
        /// 타격 랜덤 정보에 관한 파라미터 프리셋
        /// </summary>
        public HitRandomParams HitRandomParams;

        /// <summary>
        /// 타격 프리셋
        /// </summary>
        public UnitHitPresetData.TableRecord HitPreset;
        
        /// <summary>
        /// 타격 파타미터
        /// </summary>
        public UnitHitParameterData.TableRecord HitParameter;
        
        #endregion
 
        #region <Enums>

        /// <summary>
        /// 해당 피격정보가 포함할 플래그 타입
        /// </summary>
        [Flags]
        public enum HitMessageAttributeFlag
        {
            /// <summary>
            /// 기본 타격 상태
            /// </summary>
            None = 0, 
             
            /// <summary>
            /// 해당 타격이 언제나 크리티컬 히트하는 경우
            /// </summary>
            AlwaysCritical = 1 << 0,
            
            /// <summary>
            /// 해당 타격이 언제나 빗맞는 경우
            /// </summary>
            AlwaysMiss = 1 << 1,
            
            /// <summary>
            /// 방어 무시 공격인 경우
            /// </summary>
            NegateDefense = 1 << 2,
            
            /// <summary>
            /// 데미지가 무조건 0이 되는 경우
            /// </summary>
            ZeroZeroCall = 1 << 3,
            
            /// <summary>
            /// 데미지 계수를 가지는 경우
            /// </summary>
            HasDamageFactor = 1 << 4,
            
            /// <summary>
            /// HitParameter를 가지는 경우
            /// </summary>
            HasHitParameter = 1 << 5,
            
            /// <summary>
            /// 해당 메시지가 레코드로부터 활성화된 경우
            /// </summary>
            LoadFromRecord = 1 << 6,
            
            /// <summary>
            /// 해당 타격이 언제나 적중하는 경우, AlwaysMiss보다 우선 적용된다.
            /// </summary>
            AlwaysHit = 1 << 7,
        }

        #endregion
 
        #region <Constructors>

        public HitMessage(float p_FixedDamageParameter, float p_PercentDamageParameter, 
            DamageCalculator.DamageCalcType p_DamageCalcType = DamageCalculator.DamageCalcType.Physic)
        {
            _HitStateFlag = HitMessageAttributeFlag.None;
            _FixedDamageParameter = p_FixedDamageParameter;
            _PercentDamageParameter = p_PercentDamageParameter;
            _DamageCalcType = p_DamageCalcType;
            HitRandomParams = HitRandomParams.GetDefault();
            HitParameter = default;
            HitPreset = default;
            CheckDamageFactorValidation();
        }

        public HitMessage(float p_FixedDamageParameter, float p_PercentDamageParameter, 
            DamageCalculator.DamageCalcType p_DamageCalcType, HitRandomParams p_HitRandomParams)
        : this(p_FixedDamageParameter, p_PercentDamageParameter, p_DamageCalcType)
        {
            HitRandomParams = p_HitRandomParams;
        }
        
        public HitMessage(float p_FixedDamageParameter, float p_PercentDamageParameter, 
            DamageCalculator.DamageCalcType p_DamageCalcType, int p_HitParameterIndex)
            : this(p_FixedDamageParameter, p_PercentDamageParameter, p_DamageCalcType)
        {
            SetHitParameter(p_HitParameterIndex);
        }

        #endregion
 
        #region <Methods>

        private void CheckDamageFactorValidation()
        {
            if (_FixedDamageParameter.IsReachedZero() && _PercentDamageParameter.IsReachedZero())
            {
                _HitStateFlag.RemoveFlag(HitMessageAttributeFlag.HasDamageFactor);
            }
            else
            {
                AddHitMessageAttributeFlag(HitMessageAttributeFlag.HasDamageFactor);
            }
        }

        public void SetHitPresetInfo(int p_HitPresetIndex)
        {
            AddHitMessageAttributeFlag(HitMessageAttributeFlag.LoadFromRecord);
            HitPreset = UnitHitPresetData.GetInstanceUnSafe[p_HitPresetIndex];
            SetHitParameter(HitPreset.HitParameterIndex);
        }
        
        public void SetHitParameter(int p_HitParameterIndex)
        {
            if (p_HitParameterIndex != default)
            {
                AddHitMessageAttributeFlag(HitMessageAttributeFlag.HasHitParameter);
                HitParameter = UnitHitParameterData.GetInstanceUnSafe[p_HitParameterIndex];
            }
        }

        public (bool, UnitHitParameterData.TableRecord) GetHitParameter()
        {
            return (_HitStateFlag.HasAnyFlagExceptNone(HitMessageAttributeFlag.HasHitParameter), HitParameter);
        }
        
        public (bool, int) GetHitParameter(UnitHitTool.HitParameterType p_Type)
        {
            if (_HitStateFlag.HasAnyFlagExceptNone(HitMessageAttributeFlag.HasHitParameter))
            {
                return HitParameter.GetHitParameter(p_Type);
            }
            return default;
        }
        
        public (bool, UnitHitExtraData.TableRecord) GetHitExtraRecord()
        {
            if (_HitStateFlag.HasAnyFlagExceptNone(HitMessageAttributeFlag.HasHitParameter))
            {
                return HitParameter.GetHitExtraRecord();
            }
            return default;
        }

        public void AddDamageUnit(float p_Fixed, float p_Percent)
        {
            _FixedDamageParameter += p_Fixed;
            _PercentDamageParameter += p_Percent;
            CheckDamageFactorValidation();
        }

        public void MultiplyDamageUnit(float p_Unit)
        {
            _FixedDamageParameter *= p_Unit;
            _PercentDamageParameter *= p_Unit;
            CheckDamageFactorValidation();
        }

        public void AddHitMessageAttributeFlag(HitMessageAttributeFlag p_Flag)
        {
            _HitStateFlag.AddFlag(p_Flag);
        }
        
        /*public HitResult.HitResultFlag CalcHitPossibilityFlag(BattleStatusPreset p_StrikerBattleStatusPreset, BattleStatusPreset p_HitterBattleStatusPreset, Unit p_Trigger, Unit p_Target)
        {
            var result = HitResult.HitResultFlag.None;

            /* 회피 계산 #1#
            if (!_HitStateFlag.HasAnyFlagExceptNone(HitMessageAttributeFlag.AlwaysHit))
            {
                if (_HitStateFlag.HasAnyFlagExceptNone(HitMessageAttributeFlag.AlwaysMiss))
                {
                    result.AddFlag(HitResult.HitResultFlag.Miss);
                    return result;
                }
                else
                {
                    var hitRate = HitRandomParams._HitRate;
                    var stuckRate = HitRandomParams._StuckOccurRate;
                    if (p_Trigger.IsPlayer && p_Target.GetCurrentLevel() > 15)
                    {
                        // 적중률 계산 = 기본 적중률 - 레벨 차이에 의한 적중률 감소 + 공격자 적중률 * 공격자 적중률 보정치 - 기본 회피율 - 피격자 회피율 * 피격자 회피율 보정치
                        for (var levelGap = p_Trigger.GetCurrentLevel() - p_Target.GetCurrentLevel(); levelGap < 0; levelGap++)
                        {
                            if (levelGap < -10)
                            {
                                hitRate -= 0.05f;
                            }
                            else
                            {
                                hitRate += 0.01f * levelGap;
                            }
                        }
                        hitRate = hitRate + 
                                  p_StrikerBattleStatusPreset.GetHitRate() *
                                  p_StrikerBattleStatusPreset.GetHitRateMultiplyRate(true)
                                  - p_HitterBattleStatusPreset.GetDodgeRate() *
                                  p_HitterBattleStatusPreset.GetDodgeRateMultiplyRate(true);
                    }
                    else
                    {
                        // 적중률 계산 = 기본 적중률 + 공격자 적중률 * 공격자 적중률 보정치 - 기본 회피율 - 피격자 회피율 * 피격자 회피율 보정치
                        hitRate = hitRate
                                  + p_StrikerBattleStatusPreset.GetHitRate() *
                                  p_StrikerBattleStatusPreset.GetHitRateMultiplyRate(true)
                                  - stuckRate *
                                  p_HitterBattleStatusPreset.GetDodgeRate() *
                                  p_HitterBattleStatusPreset.GetDodgeRateMultiplyRate(true);
                    }

                    // 적중률 하한을 보정해준다.
                    hitRate = Mathf.Max(hitRate, HitRateLowerBound);

                    if (p_Trigger.IsPlayer && p_Trigger._ActableObject.IsUnWeaponModule())
                    {
                        hitRate = 1.1f;
                    }

                    if (hitRate < Random.value)
                    {
                        result.AddFlag(HitResult.HitResultFlag.Miss);
                        return result;
                    }
                }
            }

            /* 크리티컬 계산 #1#
            if (_HitStateFlag.HasAnyFlagExceptNone(HitMessageAttributeFlag.AlwaysCritical))
            {
                result.AddFlag(HitResult.HitResultFlag.Critical);
            }
            else
            {
                // 크리티컬 확률 계산 = 기본 크리티컬 확률 + 공격자 크리티컬 확률 * 공격자 크리티컬 확률 보정치 + 타격 크리티컬 값 - 피격자 안티 크리티컬 확률
                var criticalRate = HitRandomParams._CriticalRate
                                   + p_StrikerBattleStatusPreset.GetCriticalRate(_DamageCalcType) * p_StrikerBattleStatusPreset.GetCriticalMultiplyRate(_DamageCalcType) 
                                   - p_HitterBattleStatusPreset.GetAntiCriticalRate(_DamageCalcType);
                
                // 크리티컬 확률 상한을 보정해준다.
                criticalRate = Mathf.Min(criticalRate, CriticalRateUpperBound);
                
                if (criticalRate > Random.value)
                {
                    result.AddFlag(HitResult.HitResultFlag.Critical);
                }
            }

            /* 경직 계산 #1#
            var hitStuckPreset = GetHitParameter(UnitHitTool.HitParameterType.HitStuck);
            if (hitStuckPreset.Item1)
            {
                if (HitRandomParams._StuckOccurRate > Random.value)
                {
                    result.AddFlag(HitResult.HitResultFlag.Stuck);
                }
            }
            
            return result;
        }*/

        #endregion
    }

    public struct HitRandomParams
    {
        #region <Consts>

        /// <summary>
        /// 기본 경직 발생 확률
        /// </summary>
        private const float DefaultStuckOccurRate = 0.05f;
        
        /// <summary>
        /// 기본 크리티컬 확률
        /// </summary>
        private const float DefaultCriticalRate = 0.03f;
        
        /// <summary>
        /// 기본 적중률
        /// </summary>
        private const float DefaultHitRate = 0.97f;
        
        /// <summary>
        /// 기본 팩토리
        /// </summary>
        public static HitRandomParams GetDefault() => new HitRandomParams(DefaultStuckOccurRate, DefaultCriticalRate, DefaultHitRate);
        
        #endregion
        
        #region <Fields>
         
        /// <summary>
        /// 경직 발생 확률
        /// </summary>
        public float _StuckOccurRate;

        /// <summary>
        /// 크리티컬 확률
        /// </summary>
        public float _CriticalRate;

        /// <summary>
        /// 적중률
        /// </summary>
        public float _HitRate;

        #endregion

        #region <Constructor>

        public HitRandomParams(float p_StuckOccurRate, float p_CriticalRate, float p_HitRate)
        {
            _StuckOccurRate = p_StuckOccurRate;
            _CriticalRate = p_CriticalRate;
            _HitRate = p_HitRate;
        }

        #endregion
    }

    public struct HitResult
    {
        #region <Fields>

        /// <summary>
        /// 히트 이벤트 결과 타입
        /// </summary>
        public UnitHitTool.HitResultType HitResultType;
        
        /// <summary>
        /// 타격 유닛
        /// </summary>
        public Unit Trigger;
        
        /// <summary>
        /// 피격 유닛
        /// </summary>
        public Unit Target;

        /// <summary>
        /// 타격 힘 방향, UV 아님
        /// </summary>
        public Vector3 ForceVector;

        /// <summary>
        /// 타격 속성
        /// </summary>
        public UnitTool.UnitAttributeType HitAttribute;
        
        /// <summary>
        /// 연산 데미지
        /// </summary>
        public float ResultDamage;
        
        /// <summary>
        /// 실제 적용된 데미지
        /// </summary>
        public float ActualDamage;
        
        /// <summary>
        /// 히트 상태 플래그
        /// </summary>
        public HitResultFlag HitResultFlagMask;

        /// <summary>
        /// 해당 히트 이벤트의 시작이 되는 히트 레코드 인덱스
        /// </summary>
        public int HitParameterIndex;
        
        /// <summary>
        /// 적용할 HitParameter
        /// </summary>
        public (bool, UnitHitParameterData.TableRecord) HitParameterPreset;
 
        #endregion

        #region <Enums>

        [Flags]
        public enum HitResultFlag
        {
            None = 0,
            Critical = 1 << 0,
            Miss = 1 << 1,
            Force = 1 << 2,
            Stuck = 1 << 3,
            Tick = 1 << 4,
            Dead = 1 << 5,
            Buff = 1 << 6,
            HasTrigger = 1 << 7,
            HasDamage = 1 << 8,
        }

        #endregion

        #region <Constructors>
        
        public HitResult(UnitHitTool.HitResultType p_HitResultType, Unit p_Trigger, Unit p_Target, float p_ResultDamage, 
            HitResultFlag p_HitResultFlagMask, int p_HitParameterIndex, UnitTool.UnitAttributeType p_AttributeType, Vector3 p_ForceVector)
        {
            HitResultType = p_HitResultType;
            Trigger = p_Trigger;
            Target = p_Target;
            HitResultFlagMask = p_HitResultFlagMask;

#if UNITY_EDITOR
            if (CustomDebug.PrintUnitHitLog)
            {
                Debug.Log($"[HitResult] : {p_Trigger?.GetUnitName()} => {p_Target?.GetUnitName()} / {HitResultType} / {HitResultFlagMask}");
            }
#endif
            
            if (HitResultFlagMask.HasAnyFlagExceptNone(HitResultFlag.HasDamage))
            {
#if UNITY_EDITOR
                if (CustomDebug.TinyTinySwing)
                {
                    ResultDamage = Mathf.Min(p_ResultDamage , 1f);
                }
                else if (CustomDebug.PlayerHittingOneDamage && p_Target.IsPlayer)
                {
                    ResultDamage = Mathf.Min(p_ResultDamage , 1f);
                }
                else
                {
                    ResultDamage = p_ResultDamage;
                }
#else
                ResultDamage = p_ResultDamage;
#endif
            }
            else
            {
                ResultDamage = 0f;
            }
            
            ActualDamage = 0f;

            if (!ReferenceEquals(null, Trigger))
            {
                HitResultFlagMask.AddFlag(HitResultFlag.HasTrigger);
            }
            
            HitParameterIndex = p_HitParameterIndex;
            HitParameterPreset = (true, UnitHitParameterData.GetInstanceUnSafe[HitParameterIndex]);
            HitAttribute = p_AttributeType;
            ForceVector = p_ForceVector;
        }
        
        public HitResult(UnitHitTool.HitResultType p_HitResultType, Unit p_Trigger, Unit p_Target, float p_ResultDamage, 
            HitResultFlag p_HitResultFlagMask, HitMessage p_HitMessage, UnitTool.UnitAttributeType p_AttributeType, Vector3 p_ForceVector)
        {
            HitResultType = p_HitResultType;
            Trigger = p_Trigger;
            Target = p_Target;
            HitResultFlagMask = p_HitResultFlagMask;

#if UNITY_EDITOR
            if (CustomDebug.PrintUnitHitLog)
            {
                Debug.Log($"[HitResult] : {p_Trigger?.GetUnitName()} => {p_Target?.GetUnitName()} / {HitResultType} / {HitResultFlagMask}");
            }
#endif
            
            if (HitResultFlagMask.HasAnyFlagExceptNone(HitResultFlag.HasDamage))
            {
#if UNITY_EDITOR
                if (CustomDebug.TinyTinySwing)
                {
                    ResultDamage = Mathf.Min(p_ResultDamage , 1f);
                }
                else if (CustomDebug.PlayerHittingOneDamage && p_Target.IsPlayer)
                {
                    ResultDamage = Mathf.Min(p_ResultDamage , 1f);
                }
                else
                {
                    ResultDamage = p_ResultDamage;
                }
#else
                ResultDamage = p_ResultDamage;
#endif
            }
            else
            {
                ResultDamage = 0f;
       
            }
            
            ActualDamage = 0f;

            if (!ReferenceEquals(null, Trigger))
            {
                HitResultFlagMask.AddFlag(HitResultFlag.HasTrigger);
            }
            
            if (p_HitMessage._HitStateFlag.HasAnyFlagExceptNone(HitMessage.HitMessageAttributeFlag.HasHitParameter))
            {
                var hitParameter = p_HitMessage.HitParameter;
                HitParameterIndex = hitParameter.KEY;
                HitParameterPreset = (true, hitParameter);
            }
            else
            {
                HitParameterIndex = 0;
                HitParameterPreset = default;
            }

            HitAttribute = p_AttributeType;
            ForceVector = p_ForceVector;
        }

        #endregion

        #region <Methods>
            
        public void ApplyHitDamage(bool isDisplay = true)
        {
#if SERVER_DRIVE
            ActualDamage = Target.AddValue(BattleStatusPreset.BattleStatusType.HP_Base, new UnitPropertyChangePreset(Trigger), -ResultDamage);
#else
            var hitSkinIndexTuple = GetHitParameter(UnitHitTool.HitParameterType.HitUISkin);
            if (hitSkinIndexTuple.Item1)
            {
                var symbolIndex = UnitHitUISkinData.GetInstanceUnSafe.GetTableData(hitSkinIndexTuple.Item2).HitSkinMap[HitResultType];
                // ActualDamage = Target.AddValue(BattleStatusPreset.BattleStatusType.HP_Base, new UnitPropertyChangePreset(Trigger, UnitTool.UnitPropertyChangeDisplayType.NumberWithSymbol, symbolIndex), -ResultDamage, isDisplay);
            }
            else
            {
                // ActualDamage = Target.AddValue(BattleStatusPreset.BattleStatusType.HP_Base, new UnitPropertyChangePreset(Trigger), -ResultDamage);
            }
#endif
            if (Target.HasState_Or(Unit.UnitStateType.DEAD))
            {
                HitResultFlagMask.AddFlag(HitResultFlag.Dead);
            }
        }

        public void ApplyHitResult()
        {
            Target.ApplyHitResult(this);
        }

        public (bool, int) GetHitParameter(UnitHitTool.HitParameterType p_Type)
        {
            if (HitParameterPreset.Item1)
            {
                return HitParameterPreset.Item2.GetHitParameter(p_Type);
            }
            return default;
        }
        
        public (bool, UnitHitExtraData.TableRecord) GetHitExtraRecord()
        {
            if (HitParameterPreset.Item1)
            {
                return HitParameterPreset.Item2.GetHitExtraRecord();
            }
            return default;
        }
        
        #endregion
    }
}