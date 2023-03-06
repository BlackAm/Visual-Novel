using System;
using System.Collections.Generic;
using UnityEngine;

namespace k514
{
    public static class UnitActionTool
    {
        #region <Enums>
 
        public enum UnitActionPhase
        {
            None,
            LogicStart,
            MotionStart,
            MotionEnd,
            LogicEnd,
        }

        public enum UnitActionTriggerType
        {
            Simple,
            Charge
        }
 
        public enum UnitActionInitializeType
        {
            ReenterOtherCommand,
            ReenterSameCommand,
            ActionOver
        }
         
        [Flags]
        public enum MotionRestrictFlag
        {
            None = 0,
            
            /// <summary>
            /// 다음 모션이 예약된 경우를 표시하는 플래그
            /// </summary>
            NextMotionReserved = 1 << 0,
            
            /// <summary>
            /// StartMotion 콜백이 한번이라도 호출된 경우 세워지는 플래그
            /// </summary>
            StartMotionCallbackSetted = 1 << 1,
            
            /// <summary>
            /// 액션의 선딜레이가 진행중인지 표시하는 플래그
            /// 차징 타입 액션의 경우에는 선딜레이 개념이 차징 최대 시간 개념으로 대체 되기 때문에
            /// 차징 타입 액션에서는 해당 플래그는 동작하지 않는다.
            /// </summary>
            ProgressActionFirstDelay = 1 << 2,
        }

        #endregion
 
        #region <Classes>
 
        public class UnitAction
        {
            #region <Fields>
 
            /// <summary>
            /// 해당 스킬 테이블 레코드
            /// </summary>
            public UnitActionPresetData.TableRecord _UnitActionPresetRecord;

            /// <summary>
            /// 해당 유닛 스펠 발동 타입
            /// </summary>
            public UnitActionEntryType _UnitActionEntryType;
            
            #endregion
 
            #region <Enums>

            /// <summary>
            /// 유닛 스펠 로직으로 진입할 때, 가지고 있는 모션 정보나 이벤트 정보를 표시하는 타입
            /// </summary>
            public enum UnitActionEntryType
            {
                /// <summary>
                /// 해당 스펠은 모션도, 동작시킬 이벤트도 가지질 않음
                /// </summary>
                Invalid = 0,
                
                /// <summary>
                /// 해당 스펠은 모션만 가지고 있음
                /// </summary>
                MotionOnly = 1,
                
                /// <summary>
                /// 해당 스펠은 모션을 가지지 않으나, 동작시킬 이벤트는 가짐
                /// </summary>
                InstantSpell = 2,
                
                /// <summary>
                /// 해당 스펠은 모션도, 이벤트도 가짐
                /// </summary>
                MotionSpell = 3,
            }

            public enum UnitTryActionResultType
            {
                None,
                
                Fail_UnitState,
                Fail_HoldCommand,
                Fail_InvalidRecord,
                Fail_InvalidCommandType,
                Fail_InvalidIndex,
                Fail_EntrySpell,
                Fail_EntrySeqSame,
                Fail_EntrySeqOther,
                Fail_InvalidReservedSpell,
                Fail_Cooldown,
                Fail_Mana,

                Success_Reserved,
                Success_EntrySpell,
                Success_EntrySeqSame,
                Success_EntrySeqOther,
                Success_Silence,
            }

            [Flags]
            public enum UnitActionFlag
            {
                /// <summary>
                /// 딱히 발동하는데 조건이 없는 경우
                /// </summary>
                None = 0,
                 
                /// <summary>
                /// 발동하는데 마나가 필요한 경우
                /// </summary>
                CostMana = 1 << 0,
                 
                /// <summary>
                /// 공중에서만 사용가능한 스킬인 경우
                /// </summary>
                AerialOnly = 1 << 1,
                 
                /// <summary>
                /// 피격시에만 사용가능한 스킬인 경우
                /// </summary>
                HitOnly = 1 << 2,
 
                /// <summary>
                /// 해당 플래그가 활성화되어 있는 스킬 동작 중에는, 이동기능으로 스킬을 즉시 캔슬할 수 있다.
                /// </summary>
                MoveCancelEnable = 1 << 4,
            }
 
            #endregion
 
            #region <Constructors>
 
            public UnitAction(int p_RecordKey)
            {
                _UnitActionPresetRecord = UnitActionPresetData.GetInstanceUnSafe.GetTableData(p_RecordKey);
                var motionSeq = _UnitActionPresetRecord.MotionSequences;
                var hasMotion = motionSeq.CheckGenericCollectionSafe() ? 1 : 0;
                var eventMap = _UnitActionPresetRecord.EventPresetMap;
                var hasEvent = eventMap.CheckGenericCollectionSafe() ? 2 : 0;

                _UnitActionEntryType = (UnitActionEntryType) (hasMotion + hasEvent);
            }
 
            #endregion
 
            #region <Methods>
 
            /// <summary>
            /// 파라미터로 받은 유닛 액션에서 해당 유닛 액션으로 전이가 가능한지 검증하는 메서드
            /// </summary>
            public bool IsInterruptable(UnitAction p_TargetAction)
            {
                var targetIndex = p_TargetAction._UnitActionPresetRecord.KEY;
                return _UnitActionPresetRecord.InterruptableActionIndexGroup?.Contains(targetIndex) ?? false;
            }
 
            public bool CheckInterruptTiming(UnitActionPhase p_Phase, int p_MotionIndex, int p_TryCue)
            {
                return _UnitActionPresetRecord.ActionInterruptPreset.IsInterruptable(p_Phase, p_MotionIndex, p_TryCue);
            }
 
            public bool HasUnitActionFlag(UnitActionFlag p_CompareMask)
            {
                return _UnitActionPresetRecord.UnitActionFlagMask.HasAnyFlagExceptNone(p_CompareMask);
            }

            public float GetCost()
            {
                return _UnitActionPresetRecord.CostMana;
            }

            #endregion

            #region <Indexer>

            public MotionSequence this[int p_Index] => _UnitActionPresetRecord.MotionSequences[p_Index];

            #endregion
             
            #region <Structs>
 
            /// <summary>
            /// 어떤 스킬에서 진행할 모션에 관한 프리셋
            /// 모션 타입, 모션의 인덱스, 모션 전이를 위한 Cue의 개수, 모션 전이 조건, 재생 속도 등을 포함한다.
            /// </summary>
            public struct MotionSequence
            {
                #region <Fields>
 
                /// <summary>
                /// 진행할 모션 타입
                /// </summary>
                public AnimatorParamStorage.MotionType _MotionType;
                 
                /// <summary>
                /// 진행할 모션 타입의 모션 인덱스, -1라면 랜덤으로 한 모션을 실행
                /// </summary>
                public int _MotionIndex;
                 
                /// <summary>
                /// 모션 전이를 위해 필요한 Cue 클립 갯수
                /// </summary>
                public int CueClipCount;
 
                /// <summary>
                /// 모션 후 딜레이
                /// </summary>
                public uint PostDelay;
 
                /// <summary>
                /// 모션 재생 속도 비율
                /// </summary>
                public float MotionSpeedRate;

                /// <summary>
                /// 해당 유닛 액션을 실행하는 입력 타입
                /// </summary>
                public UnitActionTriggerType UnitActionTriggerType;

                /// <summary>
                /// 프리셋 유효성
                /// </summary>
                public bool IsValid;
                 
                #endregion
                 
                #region <Constructors>
                 
                public MotionSequence(AnimatorParamStorage.MotionType p_MotionType, int p_MotionIndex, int p_CueClipCount, uint p_PostDelay, float p_MotionSpeedRate, UnitActionTriggerType p_UnitActionTriggerType)
                {
                    _MotionType = p_MotionType;
                    _MotionIndex = p_MotionIndex;
                    CueClipCount = p_CueClipCount;
                    PostDelay = p_PostDelay;
                    MotionSpeedRate = p_MotionSpeedRate;
                    UnitActionTriggerType = p_UnitActionTriggerType;
                    IsValid = true;
                }
                 
                public MotionSequence(AnimatorParamStorage.MotionType p_MotionType, int p_MotionIndex, uint p_PostDelay, float p_MotionSpeedRate)
                    : this(p_MotionType, p_MotionIndex, 0, p_PostDelay, p_MotionSpeedRate, UnitActionTriggerType.Simple)
                {
                }
                 
                public MotionSequence(AnimatorParamStorage.MotionType p_MotionType, int p_MotionIndex, int p_CueClipCount, uint p_PostDelay, float p_MotionSpeedRate)
                    : this(p_MotionType, p_MotionIndex, p_CueClipCount, p_PostDelay, p_MotionSpeedRate, UnitActionTriggerType.Simple)
                {
                }
                 
                #endregion
                 
                #region <Methods>

                public bool IsValidWith(AnimatorParamStorage.MotionType p_MotionType, int p_MotionIndex)
                {
                    return IsValid && _MotionType == p_MotionType && _MotionIndex == p_MotionIndex;
                }
                 
                public bool IsValidWith(ControlAnimator.MotionStatePreset p_MotionStatePreset)
                {
                    return IsValid && _MotionType == p_MotionStatePreset._MotionState && _MotionIndex == p_MotionStatePreset._MotionIndex;
                }

                #endregion
            }
 
            #endregion
             
            #region <Operator>
 
            public static implicit operator UnitAction(int p_Index)
            {
                return UnitActionStorage.GetInstance[p_Index];
            }
 
            #endregion
        }
 
        #endregion
         
        #region <Structs>

        public struct MotionClipEventPreset
        {
            #region <Fields>
 
            public AnimatorParamStorage.MotionClipEventType _MotionClipEventType;
            public float AnimationClipEventTimeRate01;
              
            #endregion
 
            #region <Constructors>
 
            /// <summary>
            /// 테이블 로더에 의해서만 사용됨
            /// </summary>
            public MotionClipEventPreset(AnimatorParamStorage.MotionClipEventType p_MotionClipEventType, float p_AnimationClipEventTimeRate01)
            {
                _MotionClipEventType = p_MotionClipEventType;
                AnimationClipEventTimeRate01 = p_AnimationClipEventTimeRate01;
            }
 
            #endregion
        }
 
        public struct ActionInterruptPreset
        {
            public InterruptType _InterruptType;
            public int _MotionIndex;
            public int _CueIndex;
            private bool _Valid;
             
            public enum InterruptType
            {
                /// <summary>
                /// 지정한 큐에서만 액션 인터럽트를 허용하는 경우
                /// </summary>
                JustCueCount,
                 
                /// <summary>
                /// 지정한 큐 및 이후 큐에서 액션 인터럽트를 허용하는 경우
                /// </summary>
                LaterCueCount,
            }
 
            public ActionInterruptPreset(InterruptType p_Type, int p_MotionIndex, int p_CueIndex)
            {
                _InterruptType = p_Type;
                _MotionIndex = p_MotionIndex;
                _CueIndex = p_CueIndex;
                _Valid = true;
            }
 
            public bool IsInterruptable(UnitActionPhase p_TryPhase, int p_TryMotionIndex, int p_TryCueIndex)
            {
                if (_Valid && p_TryMotionIndex == _MotionIndex)
                {
                    switch (_InterruptType)
                    {
                        // 지정한 페이즈에서만 액션이 전이됨
                        case InterruptType.JustCueCount:
                            return p_TryCueIndex == _CueIndex;
                        // 지정한 페이즈 이후로 액션이 전이됨
                        case InterruptType.LaterCueCount:
                            return p_TryCueIndex >= _CueIndex;
                    }
                    return false;
                }
                else
                {
                    return p_TryPhase == UnitActionPhase.MotionEnd || p_TryPhase == UnitActionPhase.LogicEnd;
                }
            }
        }
 
        #endregion
    }
}