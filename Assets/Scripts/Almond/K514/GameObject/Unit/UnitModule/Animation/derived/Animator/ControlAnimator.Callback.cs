using System;
using UnityEngine;

namespace k514
{
    public partial class ControlAnimator
    {
        #region <Callbacks>

        public override IAnimatable OnInitializeAnimation(UnitAnimationDataRoot.AnimatableType p_AnimatableType, Unit p_MasterNode, IAnimatableTableRecordBridge p_AnimationPreset)
        {
            base.OnInitializeAnimation(p_AnimatableType, p_MasterNode, p_AnimationPreset);

            _Animator = p_MasterNode.GetComponentInChildren<Animator>();
            if (null == _Animator)
            {
                _Animator = _MasterNode.gameObject.AddComponent<Animator>();
            }

            var animatorMotionPreset = (AnimatorMotionPresetData.AnimatableTableRecord)_AnimatonRecord;
            var targetControllerName = AnimationControllerData.GetInstanceUnSafe.GetTableData(animatorMotionPreset.AnimatorIndex).AnimationControllerName;
            _AnimationParamsRecord = AnimatorParamStorage.GetInstance.GetAnimationMotionParamsRecord(targetControllerName);
            _Animator.runtimeAnimatorController = _AnimationParamsRecord._AnimationController;
            
            return this;
        }

        public override void TryHitMotion()
        {
            _MasterNode._ActableObject.CancelUnitAction(AnimatorParamStorage.MotionType.Hit);
        }
        
        public override void TryHitMotionBreak()
        {
            // 슈퍼 메서드의 경우에는 곧바로 HitTerminate 콜백을 호출했지만
            // 애니메이션 유닛의 경우에는, 단순히 경직 카운트 종료가 아닌 '히트 모션 종료' 때에
            // 경직 판정을 없애야 하므로 애니메이션을 통해서 경직 판정 제거를 해줘야 한다.
            SetAnimationEnable(true);
        }

        protected override void OnModuleNotify()
        {
            base.OnModuleNotify();

            SetPlayDefaultMotion(false);
        }

        protected override void OnModuleSleep()
        {
        }

        public override void OnPreUpdate(float p_DeltaTime)
        {
        }

        public override void OnUpdate(float p_DeltaTime)
        {
            if (_MasterNode.HasState_Or(Unit.UnitStateType.FREEZE) && _CachedAnimatorSpeedFactor == 0f)
            {
                SetCachedSpeedFactor(_CurrentAnimatorSpeedFactor);
                SetAnimationSpeedFactor(0f);
            }
            else if (!_MasterNode.HasState_Or(Unit.UnitStateType.FREEZE) && _CachedAnimatorSpeedFactor != 0f)
            {
                SetAnimationSpeedFactor(_CachedAnimatorSpeedFactor);
                SetCachedSpeedFactor(0f);
            }
        }

        public override void OnUpdate_TimeBlock()
        {
        }
        
        public override void OnStriked(Unit p_Trigger, HitResult p_HitResult)
        {
        }

        public override void OnHitted(Unit p_Trigger, HitResult p_HitResult)
        {
        }

        public override void OnUnitHitActionStarted()
        {
            OnAnimationStart();
        }

        public override void OnUnitHitActionTerminated()
        {
            OnAnimationEnd();
        }

        public override void OnUnitActionStarted()
        {
        }

        public override void OnUnitActionTerminated()
        {
        }

        public override void OnUnitDead(bool p_Instant)
        {
        }
        
        public override void OnJumpUp()
        {
        }
        
        public override void OnReachedGround(UnitStampPreset p_UnitStampPreset)
        {
            // 스킬 시전 중에 점프 모션이 진행되면 안되므로 제한을 건다.
            if (!_MasterNode.HasState_Or(Unit.UnitStateType.DRIVESKILL) && !_MasterNode.IsAIThroughPath())
            {
                SwitchMotionState(AnimatorParamStorage.MotionType.JumpDown, AnimatorParamStorage.MotionTransitionType.Bypass_StateMachine);
            }
        }

        public override void OnCheckOverlapObject(UnitStampPreset p_UnitStampPreset)
        {
        }

        public override void OnFocusUnitChanged(Unit p_PrevFocusUnit, Unit p_FocusUnit)
        {
        }
        
        /// <summary>
        /// 전이된 애니메이션이 지정한 모션인지 체크하는 메서드 같다면 참을리턴한다.
        /// 전이되었다면 이전 애니메이션을 종료하는 이벤트를 자체적으로 호출한다.
        /// </summary>
        public override bool OnCheckAnimationTransition(UnitActionTool.UnitAction.MotionSequence p_MotionSequence)
        {
            var currentMotionTypeOnAnimator = _CurrentStatePresetOnAnimator._MotionState;
            // 진행중인 다른 모션이 없는 경우
            if (currentMotionTypeOnAnimator == AnimatorParamStorage.MotionType.None)
            {
                ApplyReservedMotionPreset();
            }
            // 다른 모션이 진행중이었던 경우
            else
            {
                if (_CurrentStatePresetOnLogic._IsReserved)
                {
                    OnAnimationEnd();
                }
                
#if UNITY_EDITOR
                if (CustomDebug.AnimationTransition)
                {
                    Debug.Log($"Motion Transition Detected [{_CurrentStatePresetOnAnimator}] => [{_CurrentStatePresetOnLogic}]");
                }
#endif
                ApplyReservedMotionPreset();
            }

            return p_MotionSequence.IsValidWith(_CurrentStatePresetOnAnimator);
        }

        /// <summary>
        /// Animator 컴포넌트의 play함수가 호출되기 직전에 호출되는 콜백
        /// </summary>
        private void OnAnimationPlayRequested()
        {
            var currentActablePhase = _MasterNode._ActableObject.GetCurrentPhase();
            switch (currentActablePhase)
            {
                case UnitActionTool.UnitActionPhase.LogicStart:
                case UnitActionTool.UnitActionPhase.MotionStart:
                {
                    // TODO - 애니메이션 속도가 필요할 경우 추가
                    /*var unitBattleStatus = _MasterNode._BattleStatusPreset.t_Current;
                    var attackSpeed = unitBattleStatus.GetMultipliedAttackSpeed(AnimationTool.AttackSpeedStatusAdaptFactor);
                    SetAnimationSpeedFactor(1f + attackSpeed);*/
                }
                    break;
                default:
                case UnitActionTool.UnitActionPhase.None:
                case UnitActionTool.UnitActionPhase.MotionEnd:
                case UnitActionTool.UnitActionPhase.LogicEnd:
                    if (_MasterNode._ActableObject.IsMovingState())
                    {
                        // TODO - 애니메이션 속도가 필요할 경우 추가
                        /*var unitBattleStatus = _MasterNode._BattleStatusPreset.t_Current;
                        var moveSpeed = unitBattleStatus.GetMultipliedMovementSpeed(AnimationTool.MoveSpeedStatusAdaptFactor);
                        SetAnimationSpeedFactor(1f/* + moveSpeed#1#);*/
                    }
                    else
                    {
                        SetAnimationSpeedFactor(1f);
                    }
                    break;
            }
        }

        /// <summary>
        /// 애니메이션이 시작되는 경우 호출되는 콜백
        /// </summary>
        public override void OnAnimationStart()
        {
#if UNITY_EDITOR
            if (CustomDebug.AnimationTransition)
            {
                Debug.Log($"Motion Started [{_CurrentStatePresetOnLogic}]");
            }
#endif
            if (_MasterNode.IsPlayer && _MasterNode._ActableObject._CurrentIdleState == ActableTool.IdleState.Combat)
            {
                switch (_CurrentStatePresetOnAnimator._MotionState)
                {
                    case AnimatorParamStorage.MotionType.JumpDown:
                    case AnimatorParamStorage.MotionType.MoveWalk:
                    case AnimatorParamStorage.MotionType.MoveRun:
                    case AnimatorParamStorage.MotionType.JumpUp:
                    case AnimatorParamStorage.MotionType.Punch:
                    case AnimatorParamStorage.MotionType.Kick:
                    case AnimatorParamStorage.MotionType.UnWeapon:
                        _MasterNode.SetIdleState();
                        break;
                }
            }
        }

        /// <summary>
        /// 애니메이션이 종료되는 경우 호출되는 콜백
        /// </summary>
        public override void OnAnimationEnd()
        {
            var currentMotionTypeOnAnimator = _CurrentStatePresetOnAnimator._MotionState;
            if (currentMotionTypeOnAnimator == AnimatorParamStorage.MotionType.None)
            {
                // 최초 모션이 초기화 되지 않은 경우
                return;
            }

            var clipTypeOnAnimator = _AnimationParamsRecord._MotionParams[currentMotionTypeOnAnimator].Item1;
            var HasLoopOnAnimator = _CurrentStatePresetOnAnimator._AnimationClip.isLooping;
            
            // 예약된 모션이 있는 경우,
            if (_CurrentStatePresetOnLogic._IsReserved)
            {
#if UNITY_EDITOR
                if (CustomDebug.AnimationTransition)
                {
                    Debug.Log($"Motion Ended [{_CurrentStatePresetOnAnimator}] (reserve to [{_CurrentStatePresetOnLogic}])");
                }
#endif
                OnMotionTerminated();
            }
            // 예약된 모션이 없는 경우,
            else
            {
                switch (clipTypeOnAnimator)
                {
                    // 랜덤 모션에서
                    case AnimatorParamStorage.AnimationParamsRecord.ClipListType.Random:
                        
                        // 루프 모션이었던 경우, 해당 블록에서 한 루프가 끝났을 때의 작업을 기술한다.
                        if (HasLoopOnAnimator)
                        {
#if UNITY_EDITOR
                            if (CustomDebug.AnimationTransition)
                            {
                                Debug.Log($"Motion Loop Detected [{_CurrentStatePresetOnAnimator}]");
                            }
#endif
                            OnMotionTerminated();
                            OnMotionTriggered();
                        }
                        // 루프 모션이 아니었던 경우, 기본 모션을 재생시켜준다.
                        else
                        {
                            if (currentMotionTypeOnAnimator != AnimatorParamStorage.MotionType.Dead)
                            {
#if UNITY_EDITOR
                                if (CustomDebug.AnimationTransition)
                                {
                                    Debug.Log($"Motion Ended [{_CurrentStatePresetOnAnimator}]");
                                }
#endif
                                OnMotionTerminated();
                                SetPlayDefaultMotion(false);
                            }
                        }

                        break;
                    // 연속 모션에서
                    case AnimatorParamStorage.AnimationParamsRecord.ClipListType.Sequence:
                        
                        // 루프 모션이었던 경우, 해당 블록에서 한 루프가 끝났을 때의 작업을 기술한다.
                        if (HasLoopOnAnimator)
                        {
#if UNITY_EDITOR
                            if (CustomDebug.AnimationTransition)
                            {
                                Debug.Log($"Motion looped [{_CurrentStatePresetOnAnimator}]");
                            }
#endif
                            OnMotionTerminated();
                            OnMotionTriggered();
                        }
                        // 루프 모션이 아니었던 경우,
                        else
                        {
                            var targetClipList = _AnimationParamsRecord._MotionParams[currentMotionTypeOnAnimator].Item2;
                            var targetMotionCount = targetClipList.Count;
                            var currentMotionIndex = _CurrentStatePresetOnAnimator._MotionIndex;
                            
                            // 연속 모션에 남은 모션이 있는 경우, 재생시켜준다.
                            if (currentMotionIndex < targetMotionCount - 1)
                            {
#if UNITY_EDITOR
                                if (CustomDebug.AnimationTransition)
                                {
                                    Debug.Log($"Sequence Motion Trasition [{currentMotionIndex}] => [{currentMotionIndex + 1}] of [{_CurrentStatePresetOnLogic._MotionState}]");
                                }
#endif
                                currentMotionIndex++;
                                OnMotionTerminated();
                                SwitchMotionState(_CurrentStatePresetOnLogic._MotionState, currentMotionIndex, AnimatorParamStorage.MotionTransitionType.Bypass_StateMachine);
                            }
                            // 연속 모션이 남은 모션이 없는 경우, 기본 모션을 재생시켜준다.
                            else
                            {
                                if (currentMotionTypeOnAnimator != AnimatorParamStorage.MotionType.Dead)
                                {
#if UNITY_EDITOR
                                    if (CustomDebug.AnimationTransition)
                                    {
                                        Debug.Log($"Sequence Motion Ended [{_CurrentStatePresetOnLogic}]");
                                    }
#endif
                                    OnMotionTerminated();
                                    SetPlayDefaultMotion(false);
                                }
                            }
                        }

                        break;
                }
            }
        }
        
        public override void OnAnimatorMove()
        {
            var currentPlaceType = _CurrentStatePresetOnAnimator._AnimationPlaceType;

            switch (currentPlaceType)
            {
                case MotionPresetData.MotionPlaceType.None:
                    break;
                case MotionPresetData.MotionPlaceType.InPlaceRootMotion:
                {
                    var deltaPos = _Animator.deltaPosition;
                    var deltaRot = _Animator.deltaRotation;
                    var targetTransform = _MasterNode.GetAttachPoint(Unit.UnitAttachPoint.MainTransform);
                    var deltaTime = Time.deltaTime;

                    targetTransform.forward = deltaRot * targetTransform.forward;
                    _MasterNode.MoveTo(deltaPos, deltaTime);
                }
                    break;
                case MotionPresetData.MotionPlaceType.InPlaceRootMotionExceptY:
                {
                    var deltaPos = _Animator.deltaPosition.XZVector();
                    var deltaRot = _Animator.deltaRotation;
                    var targetTransform = _MasterNode.GetAttachPoint(Unit.UnitAttachPoint.MainTransform);
                    var deltaTime = Time.deltaTime;

                    targetTransform.forward = deltaRot * targetTransform.forward;
                    _MasterNode.MoveTo(deltaPos, deltaTime);
                }
                    break;
                case MotionPresetData.MotionPlaceType.InPlaceRootMotionPositionOnly:
                {
                    var deltaPos = _Animator.deltaPosition;
                    var deltaTime = Time.deltaTime;
                    _MasterNode.MoveTo(deltaPos, deltaTime);
                }
                    break;
                case MotionPresetData.MotionPlaceType.InPlaceRootMotionRotationOnly:
                {
                    var deltaRot = _Animator.deltaRotation;
                    var targetTransform = _MasterNode.GetAttachPoint(Unit.UnitAttachPoint.MainTransform);
                    targetTransform.forward = deltaRot * targetTransform.forward;
                }
                    break;
            }
        }

        /// <summary>
        /// 실제 모션이 실행되기 바로 이전에 호출되는 콜백
        /// 루프 모션에서도 각 루프 시작 전에 호출된다.
        /// </summary>
        public void OnMotionTriggered()
        {
            CacheMasterAffine();
        }

        /// <summary>
        /// 진행중인 모션이 있는데, 로직상에서 애니메이션 모션 전이가 일어나는 경우
        /// 로직상 전이 직전에 호출되는 콜백
        /// </summary>
        public void OnMotionInterruptedAtLogic()
        {
            // 현재 진행중인 모션이 공격모션인 경우에만 방향을 이어준다.
            var currentMotionType = _CurrentStatePresetOnAnimator._MotionState;
            switch (currentMotionType)
            {
                case AnimatorParamStorage.MotionType.Punch:
                case AnimatorParamStorage.MotionType.Kick:
                case AnimatorParamStorage.MotionType.UnWeapon:
                    _MasterNode._Transform.forward = CachedMasterNodeUV.Forward;
                    break;
            }
        }

        /// <summary>
        /// 실제 모션이 끝난 이후에 호출되는 콜백
        /// 루프 모션이나 다른 예약된 모션이 있는 경우
        /// 해당 모션 호출이전, OnMotionTriggered 보다도 이전에 먼저 호출된다.
        /// </summary>
        public void OnMotionTerminated()
        {
        }

        #endregion
    }
}