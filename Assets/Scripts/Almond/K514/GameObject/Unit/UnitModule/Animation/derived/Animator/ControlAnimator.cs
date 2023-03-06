using UnityEngine;
 
namespace k514
{
    public partial class ControlAnimator : AnimatableBase
    {
        #region <Fields>
 
        /// <summary>
        /// 애니메이터 컴포넌트 레퍼런스
        /// </summary>
        public Animator _Animator { get; private set; }
         
        /// <summary>
        /// 해당 유닛이 사용할 모션 프리셋
        /// </summary>
        private AnimatorParamStorage.AnimationParamsRecord _AnimationParamsRecord;
 
        /// <summary>
        /// 현재 컴포넌트 및 로직 상에서 진행중인 모션 프리셋
        /// </summary>
        private MotionStatePreset _CurrentStatePresetOnAnimator, _CurrentStatePresetOnLogic;
 
        /// <summary>
        /// 현재 진행중인 연속 모션이 루프인 경우, 블록 플래그
        /// </summary>
        private bool _SeqLoopMotionFlag;

        /// <summary>
        /// 애니메이터 자체의 속도
        /// </summary>
        private float _CurrentAnimatorSpeedFactor;
        
        /// <summary>
        /// 애니메이션 속도 캐싱
        /// </summary>
        private float _CachedAnimatorSpeedFactor;

        /// <summary>
        /// 모션의 속도
        /// </summary>
        private float _CurrentMotionSpeedFactor;

        public int _RelaxMotionCount;
 
        #endregion
 
        #region <Method/Motion>
         
        /// <summary>
        /// 요청했던 모션 정보(_CurrentStatePresetOnLogic)가 정상적으로 실행된 경우에,
        /// 해당 모션 정보를 현재 재생중인 정보(_CurrentStatePresetOnAnimator)로 갱신하는 메서드
        /// </summary>
        private void ApplyReservedMotionPreset()
        {
            if (IsReserveTransactionSetted())
            {
                _CurrentStatePresetOnLogic._IsReserved = false;
                _CurrentStatePresetOnAnimator = _CurrentStatePresetOnLogic;
            }
        }
 
        /// <summary>
        /// 루트 모션 사용여부를 지정하는 메서드
        /// </summary>
        public void SetRootMotionEnable(bool p_Flag)
        {
            _Animator.applyRootMotion = p_Flag;
        }
 
        /// <summary>
        /// 애니메이션 재생 속도를 세트하는 메서드
        /// </summary>
        public override void SetAnimationSpeedFactor(float p_Factor)
        {
            _CurrentAnimatorSpeedFactor = p_Factor;
            UpdateAnimationSpeed();
        }

        public void SetCachedSpeedFactor(float p_Factor)
        {
            _CachedAnimatorSpeedFactor = p_Factor;
        }
        
        /// <summary>
        /// 모션 재생 속도를 세트하는 메서드
        /// </summary>
        public override void SetMotionSpeedFactor(float p_Factor)
        {
            _CurrentMotionSpeedFactor = p_Factor;
            UpdateAnimationSpeed();
        }
        
        private void UpdateAnimationSpeed()
        {
            _Animator.speed = Mathf.Clamp(_CurrentAnimatorSpeedFactor * _CurrentMotionSpeedFactor, AnimationTool.AnimationSpeedLowerBound, AnimationTool.AnimationSpeedUpperBound);
        }

        public override void ClearAnimationSpeed()
        {
            _CurrentAnimatorSpeedFactor = 1f;
            _CurrentMotionSpeedFactor = 1f;
            UpdateAnimationSpeed();
        }

        /// <summary>
        /// 특정 애니메이션 재생 속도를 세트하는 메서드
        /// </summary>
        public override void SetAnimationFloat(string p_Name, float p_Float)
        {
            _Animator.SetFloat(p_Name, p_Float);
        }

        /// <summary>
        /// 애니메이터 컴포넌트를 활성화 시키는 메서드. 비활성화 시키는 경우, 애니메이션 일시 정지를 기대할 수 있다.
        /// </summary>
        public override void SetAnimationEnable(bool p_Flag)
        {
            _Animator.enabled = p_Flag;
        }
 
        /// <summary>
        /// 현재 모션 상태를 초기화하고, 기본 모션을 재생시키는 메서드
        /// </summary>
        public override void SetPlayDefaultMotion(bool p_RestrictFlag)
        {
            // 스킬 시전 중에는, 유닛 액션을 통해서 해당 메서드가 호출되어야 하므로 제한을 건다.
            if (p_RestrictFlag || !_MasterNode.HasState_Or(Unit.UnitStateType.DRIVESKILL))
            {
                var actableObject = _MasterNode._ActableObject;
                if (ReferenceEquals(null, actableObject))
                {
                    SwitchMotionState(AnimatorParamStorage.MotionType.RelaxIdle, AnimatorParamStorage.MotionTransitionType.Restrict_ErasePrevMotion);
                }
                else
                {
                    actableObject.TurnIdleState(AnimatorParamStorage.MotionTransitionType.Restrict_ErasePrevMotion);
                }
            }
        }
 
        /// <summary>
        /// 현재 모션 정보를 리셋시키는 메서드
        /// </summary>
        public void ResetCurrentMotionStateOnAnimator()
        {
            _CurrentStatePresetOnAnimator = default;
        }
 
        /// <summary>
        /// 지정한 타입의 애니메이션을 재생시킨다. 새로운 모션이 실행되거나, 혹은 현재 애니메이션과 동일한 타입의 애니메이션이 선정된 경우 true를 리턴한다.
        /// </summary>
        public override bool SwitchMotionState(AnimatorParamStorage.MotionType p_Type, AnimatorParamStorage.MotionTransitionType p_TransitionFlag)
        {
            return SwitchMotionState(p_Type, _AnimationParamsRecord.GetRandomMotionIndex(p_Type), p_TransitionFlag);
        }
         
        /// <summary>
        /// 지정한 타입의 애니메이션을 재생시킨다. 새로운 모션이 실행되거나, 혹은 현재 애니메이션과 동일한 타입의 애니메이션이 선정된 경우 true를 리턴한다.
        /// 2번째 파라미터에 의해 여러 애니메이션 중 지정한 인덱스의 애니메이션을 재생할 수 있다.
        /// 3번째 파라미터로 전이 타입을 정할 수 있다.
        /// </summary>
        public override bool SwitchMotionState(AnimatorParamStorage.MotionType p_Type, int p_Index, AnimatorParamStorage.MotionTransitionType p_TransitionFlag)
        {
            if (_MasterNode.HasState_Or(Unit.UnitStateType.SystemDisable)) return false;

#if UNITY_EDITOR
            if (CustomDebug.AnimationTransition)
            {
                Debug.Log($"[{_MasterNode.GetUnitName()}] : [{p_Type} / {p_Index} / {p_TransitionFlag}] Req (current : {_CurrentStatePresetOnAnimator._MotionState})");
            }
#endif
            if (_MasterNode.IsPlayer && _RelaxMotionCount != 0 && p_Type != AnimatorParamStorage.MotionType.RelaxIdle) _RelaxMotionCount = 0;
            
            var targetMotionCollection = _AnimationParamsRecord._MotionParams;

            // 현재 진행중인 모션 정보를 지워준다.
            if (p_TransitionFlag.HasAnyFlagExceptNone(AnimatorParamStorage.MotionTransitionType.Restrict_ErasePrevMotion))
            {
                ResetCurrentMotionStateOnAnimator();
            }

            // 지정받은 모션이 다른 모션으로 대체되어야 하는지 체크
            MotionTransitionMultiplyStateMachine(_CurrentStatePresetOnAnimator._MotionState, ref p_Type, ref p_Index);
             
            var isEnterable = false;
            // 1. 현재 재생중인 모션이 없음
            // 2. 현재 모션이 루프모션이 아님
            if (IsPlayedNoneMotion() || !IsCurrentMotionLooping())
            {
                // 5. 전이 타입 p_TransitionFlag에 따라 가능한 전이인지 체크한다.
                isEnterable = GetTransitionKeyFromFlag(p_Type, p_Index, p_TransitionFlag);
            }
            else
            {
                // 3-1. 아직 실행되지 않은 예약된 모션이 존재하는 경우
                if (IsReserveTransactionSetted())
                {
                    // 예약된 모션을 임시로 현재 모션으로 가정하고, 진입하고자 하는 모션이
                    // 진입할 수 있는지 검증한다.
                    var cachedCurrentPreset = _CurrentStatePresetOnAnimator;
                    _CurrentStatePresetOnAnimator = _CurrentStatePresetOnLogic;
                    
                    // 4. 현재 재생중인 모션과 다른 타입 혹은 같은 타입이어도 다른 인덱스의 모션으로 전이하려는 경우
                    // 5. 전이 타입 p_TransitionFlag에 따라 가능한 전이인지 체크한다.
                    isEnterable = !IsCurrentMotionOnAnimator(p_Type, p_Index) &&
                                  GetTransitionKeyFromFlag(p_Type, p_Index, p_TransitionFlag);
                    
                    // 진입할 수 없었던 경우, 현재 모션을 원래대로 돌려놓는다.
                    if (!isEnterable)
                    {
                        _CurrentStatePresetOnAnimator = cachedCurrentPreset;
                    }
                }
                // 3-2. 예약된 모션이 없는 경우
                else
                {
                    // 4. 현재 재생중인 모션과 다른 타입 혹은 같은 타입이어도 다른 인덱스의 모션으로 전이하려는 경우
                    var isCurrentMotionOnAnimator = IsCurrentMotionOnAnimator(p_Type, p_Index);
                    // 5. 전이 타입 p_TransitionFlag에 따라 가능한 전이인지 체크한다.
                    isEnterable = !isCurrentMotionOnAnimator && GetTransitionKeyFromFlag(p_Type, p_Index, p_TransitionFlag);

                    // 같은 모션이라서 전이에 실패했다 하더라도, 모션 아핀값은 갱신시켜준다.
                    if (isCurrentMotionOnAnimator)
                    {
                        CacheMasterAffine();
                    }
                }
            }

            if(isEnterable)
            {
                // 타겟 모션이 존재하는 경우
                if (targetMotionCollection.ContainsKey(p_Type))
                {
                    var targetClipList = targetMotionCollection[p_Type].Item2;
                    var targetMotionCount = targetClipList.Count;
         
                    if (targetMotionCount > 0)
                    {
                        OnMotionInterruptedAtLogic();
                        _CurrentStatePresetOnLogic = p_Type != AnimatorParamStorage.MotionType.RelaxIdle || !_AnimationParamsRecord.HasMotion(p_Type, 1)
                            ? new MotionStatePreset(_MasterNode, p_Type, Mathf.Min(targetMotionCount - 1, p_Index),
                                _AnimationParamsRecord)
                            : new MotionStatePreset(_MasterNode, p_Type,
                                _AnimationParamsRecord.GetRelaxMotionIndex(this, _RelaxMotionCount),
                                _AnimationParamsRecord);
                        
                        
                        OnMotionTriggered();
                        var targetAnimationClip = _CurrentStatePresetOnLogic._AnimationClip;
#if UNITY_EDITOR
                        if (_Animator.isActiveAndEnabled)
                        {
                            OnAnimationPlayRequested();
                            _Animator.Play(targetAnimationClip.name, -1, 0f);
                        }
#else
                        OnAnimationPlayRequested();
                        _Animator.Play(targetAnimationClip.name, -1, 0f);
#endif
                        // TODO<K514>
                        // CrossFade 사용시에, Fade되는 특이점의 위치에 따라서
                        // OnAnimationStart 및 OnAnimationTerminate가 호출되지 않는 경우가 있어서
                        // 모션의 시작과 끝 이벤트를 전달받지 못하고 로직에서 애니메이션을 제어할 수 없는 경우가 생긴다.
                        // _Animator.CrossFade(targetAnimationClip.name, 0.05f, 0);

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                // 타겟 모션이 존재하지 않는 경우
                else
                {
                    return false;
                }
            }
            else
            {
                // 예를 들어, Run을 실행시켜서, 모션이 실행되던 안되던 결과적으로 현재 모션 상태가 Run인 경우 true
                return _CurrentStatePresetOnAnimator._MotionState == p_Type;
            }
        }
 
        /// <summary>
        /// 지정한 전이타입에 따라 현재 애니메이션이 지정한 모션의 지정한 인덱스로 전이할 수 있는지 체크하는 논리 메서드
        /// </summary>
        public bool GetTransitionKeyFromFlag(AnimatorParamStorage.MotionType p_Type, int p_Index, AnimatorParamStorage.MotionTransitionType p_TransitionFlag)
        {
            var hasAndMask = p_TransitionFlag.HasAnyFlagExceptNone(AnimatorParamStorage.MotionTransitionType.AndMask);
            foreach (var motionTransitionType in AnimationTool._MotionTransitionType_Enumerator)
            {
                if (p_TransitionFlag.HasAnyFlagExceptNone(motionTransitionType))
                {
                    var result = hasAndMask;
                    switch (motionTransitionType)
                    {
                        case AnimatorParamStorage.MotionTransitionType.Bypass_StateMachine:
                            result = MotionTransitionValidCheckStateMachine(_CurrentStatePresetOnAnimator._MotionState, p_Type, p_Index);
                            break;
                        case AnimatorParamStorage.MotionTransitionType.Bypass_InverseStateMachine:
                            result = !MotionTransitionValidCheckStateMachine(_CurrentStatePresetOnAnimator._MotionState, p_Type, p_Index);
                            break;
                        case AnimatorParamStorage.MotionTransitionType.Restrict:
                        case AnimatorParamStorage.MotionTransitionType.Restrict_ErasePrevMotion:
                            result = true;
                            break;
                        case AnimatorParamStorage.MotionTransitionType.Restrict_ToSameMotion:
                            result = _CurrentStatePresetOnAnimator._MotionState == p_Type;
                            break;
                        case AnimatorParamStorage.MotionTransitionType.Restrict_ToDiffentMotion:
                            result = _CurrentStatePresetOnAnimator._MotionState != p_Type;
                            break;
                    }

                    if (hasAndMask != result)
                    {
                        return result;
                    }
                }
            }
            
            return hasAndMask;
        }
         
        /// <summary>
        /// 현재 실행중인 모션이 지정한 타입인지 검증하는 메서드
        /// </summary>
        public override bool IsCurrentMotionOnAnimator(AnimatorParamStorage.MotionType p_Type)
        {
            return _CurrentStatePresetOnAnimator._MotionState == p_Type;
        }
 
        /// <summary>
        /// 현재 실행중인 모션이 지정한 타입의 지정한 인덱스 모션인지 검증하는 메서드
        /// </summary>
        private bool IsCurrentMotionOnAnimator(AnimatorParamStorage.MotionType p_Type, int p_Index)
        {
            return _CurrentStatePresetOnAnimator._MotionState == p_Type && _CurrentStatePresetOnAnimator._MotionIndex == p_Index;
        }
 
        /// <summary>
        /// 지정한 애니메이션 컨트롤러가 현재 재생중인 모션이 없는지 검증하는 메서드
        /// </summary>
        public bool IsPlayedNoneMotion()
        {
            return IsCurrentMotionOnAnimator(AnimatorParamStorage.MotionType.None);
        }
 
        /// <summary>
        /// 현재 실행중인 모션이 루프 모션인지 검증하는 메서드
        /// </summary>
        private bool IsCurrentMotionLooping()
        {
            return _CurrentStatePresetOnAnimator._AnimationClip.isLooping;
        }

        /// <summary>
        /// Animator.Play 으로 실행 요청한 모션이 아직 실행되지 않은 경우 참을 리턴하는 논리 메서드
        /// </summary>
        private bool IsReserveTransactionSetted()
        {
            return _CurrentStatePresetOnLogic._IsReserved;
        }
 
        /// <summary>
        /// 유닛의 상태에 따라 전이하려는 모션이 다른 모션으로 대체되어야 한다면, 해당 모션 타입을 리턴하는 메서드
        /// </summary>
        public void MotionTransitionMultiplyStateMachine(AnimatorParamStorage.MotionType p_FromMotion, ref AnimatorParamStorage.MotionType r_ToMotion, ref int r_Index)
        {
            switch (r_ToMotion)
            {
                case AnimatorParamStorage.MotionType.RelaxIdle:
                case AnimatorParamStorage.MotionType.CombatIdle:
                case AnimatorParamStorage.MotionType.GroggyIdle:
                case AnimatorParamStorage.MotionType.MoveWalk:
                case AnimatorParamStorage.MotionType.MoveRun:
                    // 부유 상태에서는 위의 모션들이 점프 모션이 된다.
                    if (_MasterNode.HasState_Or(Unit.UnitStateType.FLOAT))
                    {
                        r_Index = IsCurrentMotionOnAnimator(AnimatorParamStorage.MotionType.JumpUp, 0) ? 0 : 1;
                        r_ToMotion = AnimatorParamStorage.MotionType.JumpUp;
                    }
                    break;
                 
                case AnimatorParamStorage.MotionType.JumpUp :
                    // 히트 상태에서는 점프모션이 히트모션이 된다.
                    if (_MasterNode.HasState_Or(Unit.UnitStateType.STUCK))
                    {
                        r_ToMotion = p_FromMotion;
                    }
                    break;
            }
 
            // 지정한 모션 타입이 존재하지 않는 경우, FallBack을 적용한다.
            if (!_AnimationParamsRecord.HasMotion(r_ToMotion))
            {
                var tryMotion = _AnimationParamsRecord.GetFallBackMotionType(r_ToMotion, _MasterNode);
                // fallback이 None이라면 현재 모션을 그대로 이어준다.
                if (tryMotion == AnimatorParamStorage.MotionType.None)
                {
                    r_ToMotion = p_FromMotion;
                    r_Index = _CurrentStatePresetOnAnimator._MotionIndex;
                }
                // 그 외의 경우, 선정된 fallBack 모션 타입의 적합한 인덱스의 모션을 세트한다.
                else
                {
                    switch (tryMotion)
                    {
                        case AnimatorParamStorage.MotionType.RelaxIdle:
                            if (_MasterNode._ActableObject._CurrentIdleState == ActableTool.IdleState.Combat &&
                                _AnimationParamsRecord.HasMotion(AnimatorParamStorage.MotionType.CombatIdle))
                            {
                                r_ToMotion = AnimatorParamStorage.MotionType.CombatIdle;
                            }
                            else
                            {
                                r_ToMotion = tryMotion;
                            }
                            break;
                        case AnimatorParamStorage.MotionType.CombatIdle:
                            if (_MasterNode._ActableObject._CurrentIdleState == ActableTool.IdleState.Relax &&
                                _AnimationParamsRecord.HasMotion(AnimatorParamStorage.MotionType.RelaxIdle))
                            {
                                r_ToMotion = AnimatorParamStorage.MotionType.RelaxIdle;
                            }
                            else
                            {
                                r_ToMotion = tryMotion;
                            }
                            break;
                        default:
                            r_ToMotion = tryMotion;
                            break;
                    }
                    r_Index = _AnimationParamsRecord.GetRandomMotionIndex(r_ToMotion);
                }
            }
        }
 
        /// <summary>
        /// 어떤 모션에서 다른 모션으로 전이가 가능한지에 대한 여부를 리턴하는 메서드
        /// </summary>
        public bool MotionTransitionValidCheckStateMachine(AnimatorParamStorage.MotionType p_FromMotion, AnimatorParamStorage.MotionType p_ToMotion, int p_Index)
        {
            // from motion에 대해
            switch (p_FromMotion)
            {
                // None 상태는 어떤 모션으로도 전이할 수 있다.
                case AnimatorParamStorage.MotionType.None:
                    return true;
                // 착지 상태에서 걷기모션으로 넘어갈 수 없다.
                case AnimatorParamStorage.MotionType.JumpDown:
                    return p_ToMotion != AnimatorParamStorage.MotionType.MoveWalk;
                // 그로기 상태는 현재 로직 상에서 Groggy가 해제된 경우 혹은 사망 모션을 출력해야할 때에만 다른 모션으로 전이할 수 있다.
                case AnimatorParamStorage.MotionType.GroggyIdle:
                    switch (p_ToMotion)
                    {
                        case AnimatorParamStorage.MotionType.Dead:
                            return true;
                        default :
                            return _MasterNode._ActableObject._CurrentIdleState != ActableTool.IdleState.Groggy;
                    }
                // 달리기 모션 및 걷는 모션은 루프 모션이므로, 같은 모션으로의 재진입은 불가능하다.
                case AnimatorParamStorage.MotionType.RelaxIdle:
                case AnimatorParamStorage.MotionType.CombatIdle:
                case AnimatorParamStorage.MotionType.MoveWalk:
                case AnimatorParamStorage.MotionType.MoveRun:
                    return p_FromMotion != p_ToMotion;
                // 점프 상태에서
                case AnimatorParamStorage.MotionType.JumpUp:
                    switch (p_ToMotion)
                    {
                        case AnimatorParamStorage.MotionType.SpecialMove:
                        case AnimatorParamStorage.MotionType.Punch:
                        case AnimatorParamStorage.MotionType.Kick:
                        case AnimatorParamStorage.MotionType.Hit:
                        case AnimatorParamStorage.MotionType.JumpDown:
                        case AnimatorParamStorage.MotionType.UnWeapon:
                        case AnimatorParamStorage.MotionType.Dead:
                            return true;
                        // 점프 모션은 연속 모션
                        case AnimatorParamStorage.MotionType.JumpUp:
                            return _CurrentStatePresetOnAnimator._MotionIndex != p_Index;
                        default:
                            return false;
                    }
                // 공격이나 특수 동작에서는 다른 모션으로 전이할 수 없다.
                case AnimatorParamStorage.MotionType.SpecialMove:
                case AnimatorParamStorage.MotionType.Punch:
                case AnimatorParamStorage.MotionType.Kick:
                case AnimatorParamStorage.MotionType.UnWeapon:
                case AnimatorParamStorage.MotionType.Hit:
                    switch (p_ToMotion)
                    {
                        case AnimatorParamStorage.MotionType.CombatIdle :
                        case AnimatorParamStorage.MotionType.GroggyIdle :
                        case AnimatorParamStorage.MotionType.RelaxIdle :
                        case AnimatorParamStorage.MotionType.SpecialMove:
                        case AnimatorParamStorage.MotionType.Punch:
                        case AnimatorParamStorage.MotionType.Kick:
                        case AnimatorParamStorage.MotionType.UnWeapon:
                        case AnimatorParamStorage.MotionType.Hit:
                            return true;
                        default:
                            return false;
                    }
                case AnimatorParamStorage.MotionType.Dead:
                    return false;
                // 그 외 예외 케이스에서는 다른 모션으로 전이할 수 없다.
                default:
                    return false;
            }
        }
 
        #endregion
 
        #region <Methods>
 
        public override void CacheMasterAffine()
        {
            var masterAffine = _MasterNode._Transform;
            CachedMasterNodeUV = masterAffine;
        }
        
        public void SetModelTransformLocalLinearYOffset(float p_StartLocalYPos, float p_EndLocalYPos, float p_ProgressRate01)
        {
            var offsetVector = Mathf.Lerp(p_StartLocalYPos, p_EndLocalYPos, p_ProgressRate01);
            SetModelTransformLocalYOffset(offsetVector);
        }
         
        public void SetModelTransformLocalYOffset(float p_Offset)
        {
            var _ModelTransform = _MasterNode.GetAttachPoint(Unit.UnitAttachPoint.AnimationRootNode);
            _ModelTransform.localPosition = Vector3.up * p_Offset;
        }
         
        public void SetModelTransformLocalLinearOffset(Vector3 p_StartLocalPos, Vector3 p_EndLocalPos, float p_ProgressRate01)
        {
            var offsetVector = Vector3.Lerp(p_StartLocalPos, p_EndLocalPos, p_ProgressRate01);
            SetModelTransformLocalOffset(offsetVector);
        }
         
        public void SetModelTransformLocalOffset(Vector3 p_Offset)
        {
            var _ModelTransform = _MasterNode.GetAttachPoint(Unit.UnitAttachPoint.AnimationRootNode);
            _ModelTransform.localPosition = p_Offset;
        }
 
#if UNITY_EDITOR
        public void PrintAnimationClip()
        {
            Debug.Log($" * Unit [{_MasterNode.GetUnitName()} Animation Controller State *");
            foreach (var motionType in AnimationTool._MotionTypeEnumerator)
            {
                var targetAnimationClipList = _AnimationParamsRecord._MotionParams[motionType].Item2;
                foreach (var animationClip in targetAnimationClipList)
                {
                    Debug.Log($"  ** MotionType [{motionType}] \n{animationClip}");
                }
            }
        }
#endif
 
        #endregion
 
        #region <Structs>
 
        public struct MotionStatePreset
        {
            public Unit _MasterNode;
            public AnimatorParamStorage.MotionType _MotionState;
            public int _MotionIndex;
            public AnimationClip _AnimationClip;
            public MotionPresetData.MotionPlaceType _AnimationPlaceType;
            public bool _IsReserved;
 
            #region <Constructors>
 
            public MotionStatePreset(Unit p_MasterNode, AnimatorParamStorage.MotionType p_MotionState, int p_MotionIndex, AnimatorParamStorage.AnimationParamsRecord p_AnimationParamsRecord)
            {
                _MasterNode = p_MasterNode;
                _MotionState = p_MotionState;
                _MotionIndex = p_MotionIndex;
                _AnimationClip = p_AnimationParamsRecord._MotionParams[_MotionState].Item2[_MotionIndex].AnimationClip;
                var motionPreset = p_AnimationParamsRecord._MotionParams[_MotionState].Item2[_MotionIndex]
                    .MotionPresetRecord;
                _AnimationPlaceType = motionPreset?.MotionPlaceType ?? default;
                _IsReserved = true;
            }
 
            #endregion
 
            #region <Operator>
 
            public override bool Equals(object p_Right)
            {
                return Equals((MotionStatePreset)p_Right);
            }

            public bool Equals(MotionStatePreset r)
            {
                return _MotionState == r._MotionState && _MotionIndex == r._MotionIndex;
            }

            public override int GetHashCode()
            {
                return ((int)_MotionState + 1) * (_MotionIndex + 1);
            }

            public static bool operator ==(MotionStatePreset p_Left, MotionStatePreset p_Right)
            {
                return p_Left.Equals(p_Right);
            }
 
            public static bool operator !=(MotionStatePreset p_Left, MotionStatePreset p_Right)
            {
                return !(p_Left == p_Right);
            }
 
#if UNITY_EDITOR
            public override string ToString()
            {
                return $"{_MotionState}({_MotionIndex})";
            }
#endif
 
            #endregion
        }
 
        #endregion
         
        #region <Disposable>

        /// <summary>
        /// 인스턴스가 파기될 때 수행할 작업을 기술한다.
        /// </summary>
        protected override void DisposeUnManaged()
        {
            _Animator.runtimeAnimatorController = null;
            AnimatorParamStorage.GetInstance?.DisposeAnimationMotionParamsRecord(_AnimationParamsRecord);
            _AnimationParamsRecord = new AnimatorParamStorage.AnimationParamsRecord();
        }
 
        #endregion
    }
}