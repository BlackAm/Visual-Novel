using System;
using UnityEngine;

namespace k514
{
    public static class UnitHitTool
    {
        #region <Consts>

        /// <summary>
        /// 타겟 유닛이 무적등의 이유로 타격 이벤트가 처리되지 않은 경우의 결과 구조체 1
        /// </summary>
        public static HitResult HIT_FAIL((bool, UnitHitParameterData.TableRecord) p_HitParams) 
            => new HitResult
        {
            HitParameterPreset = p_HitParams,
            HitResultType = HitResultType.HitNoOne
        };
        /// <summary>
        /// 타겟 유닛이 무적등의 이유로 타격 이벤트가 처리되지 않은 경우의 결과 구조체 2
        /// </summary>
        public static HitResult HIT_NO_ONE((bool, UnitHitParameterData.TableRecord) p_HitParams) 
            => new HitResult
                {
                    HitParameterPreset = p_HitParams,
                    HitResultType = HitResultType.HitNoOne
                };

        #endregion
        
        #region <Enums>

        [Flags]
        public enum HitParameterType
        {
            None = 0,
            HitForce = 1 << 0,
            HitBuff = 1 << 1,
            HitInteract = 1 << 2,
            HitStuck = 1 << 3,
            HitTick = 1 << 4,
            HitUISkin = 1 << 5,
            HitExtra = 1 << 6,
        }
        
        public static HitParameterType[] _HitParameterTypeEnumerator;

        [Flags]
        public enum HitBuffType
        {
            None = 0,
            
            MonoBaseStatus = 1 << 0,
            BaseStatus = 1 << 1,
            MonoBattleStatus = 1 << 2,
            BattleStatus = 1 << 3,
        }

        [Flags]
        public enum HitExtraAttributeType
        {
            None = 0,
            ExtraHit = 1 << 0,
            HitStorm = 1 << 1,
        }
        
        /// <summary>
        /// 경직 타입
        /// </summary>
        public enum HitStuckType
        {
            /// <summary>
            /// 경직이 누적되는 타입
            /// </summary>
            Stack,
             
            /// <summary>
            /// 신규 피격 경직 값으로 갱신되는 타입
            /// </summary>
            Update,
             
            /// <summary>
            /// 신규 피격시, 더 경직이 긴 쪽으로 갱신하는 타입
            /// </summary>
            UpdateBigger,
        }

        public enum HitResultType
        {
            HitNoOne,
            HitFail,
            HitNormal,
            HitCritical,
            HitMissed,
        }

        public static HitResultType[] _HitResultTypeEnumerator;

        #endregion

        #region <Constructor>

        static UnitHitTool()
        {
            _HitResultTypeEnumerator = SystemTool.GetEnumEnumerator<HitResultType>(SystemTool.GetEnumeratorType.GetAll);
            _HitParameterTypeEnumerator = SystemTool.GetEnumEnumerator<HitParameterType>(SystemTool.GetEnumeratorType.GetAll);
        }

        #endregion

        #region <Methods>

        public static void ApplyHitResult(HitResult p_HitResult, bool p_CalcActualDamage)
        {
            if (p_CalcActualDamage)
            {
                p_HitResult.ApplyHitDamage();
            }
            p_HitResult.ApplyHitResult();
        }

        public static Unit GetHitSFXPivotUnit(HitResultType p_Type, Unit p_Trigger, Unit p_Target)
        {
            switch (p_Type)
            {
                default:
                case HitResultType.HitNoOne:
                case HitResultType.HitFail:
                    return p_Trigger;
                case HitResultType.HitNormal:
                case HitResultType.HitCritical:
                case HitResultType.HitMissed:
                    return p_Target;
            }
        }

        public static void PlayHitFX(HitResultType p_Type, Unit p_Target, Unit p_Trigger, HitResult p_HitResult)
        {
#if SERVER_DRIVE
            HeadlessServerManager.GetInstanceUnSafe.OnHitFXSpawnRequest();     
#else
            var hitInteractTuple = p_HitResult.GetHitParameter(HitParameterType.HitInteract);
            if (hitInteractTuple.Item1)
            {
                var interactRecord = UnitHitInteractData.GetInstanceUnSafe[hitInteractTuple.Item2];
                var fxRecord = interactRecord[p_Target.SkinType];
                var affine = new TransformTool.AffineCachePreset(p_Target);
                var tryPair = fxRecord.HitFxMap[p_Type];
                var hitResultFlag = p_HitResult.HitResultFlagMask;
                var hasTrigger = hitResultFlag.HasAnyFlagExceptNone(HitResult.HitResultFlag.HasTrigger);
                
                var forward = hitResultFlag.HasAnyFlagExceptNone(HitResult.HitResultFlag.Force)
                    ? p_HitResult.ForceVector.normalized
                    : UnitTool.GetForceVector(p_Target, p_Trigger, hasTrigger, default, UnitTool.UnitAddForceType.TargetDirection, interactRecord.FallbackSeedUV);
                
                var attachPoint = interactRecord.FxAttachPoint;
                affine.SetBasis(forward);
                affine.SetPosition(p_Target[attachPoint].position);
                affine.SetScaleFactor(p_Trigger.ObjectScale.CurrentValue);
                
                VfxSpawnManager.GetInstance.CastVfx<VFXUnit>(tryPair.t_VfxIndex, affine);

                var sfxPivotUnit = GetHitSFXPivotUnit(p_Type, p_Trigger, p_Target);
                var combatMediaPreset = sfxPivotUnit.GetMediaPreset(UnitTool.UnitEnvironmentSoundType.Combat);
                var (valid, sfxUnit) = SfxSpawnManager.GetInstance.GetSfx(tryPair.t_SfxIndex, combatMediaPreset, affine);
                if (valid)
                {
                    sfxPivotUnit.SetMediaPreset(UnitTool.UnitEnvironmentSoundType.Combat, sfxUnit.MediaPreset);
                }

                if (CameraManager.GetInstanceUnSafe.IsTracingTarget(p_Target))
                {
                    switch (p_Type)
                    {
                        case HitResultType.HitNormal:
                        case HitResultType.HitCritical:
                            CameraManager.GetInstanceUnSafe.SetShake(interactRecord.CameraShakeIndex, interactRecord.CameraShakePreDelay);
                            break;
                    }
                }
            }
#endif
        }
        
        #endregion
    }
}