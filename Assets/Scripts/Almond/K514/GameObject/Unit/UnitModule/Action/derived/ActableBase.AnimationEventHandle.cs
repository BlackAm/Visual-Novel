using UnityEngine;

namespace k514
{
    public partial class ActableBase
    {
        #region <IAnimationClipEventReceivable>

        /// <summary>
        /// 모션의 시작 타임 스탬프로부터 호출되는 콜백함수
        /// 
        /// 해당 콜백 및 이하 모션 콜백 함수들의 파라미터는 모두
        /// '현재 해당 모션이 시작된 이후 해당 모션 콜백이 호출되기 까지의 경과 시간'을 나타낸다.
        /// 
        /// 유닛 액션이 진행중일 때, 유닛 액션의 타입에 따라 해당 콜백에 진입하는 페이즈가 다르다.
        ///
        /// * 심플타입(단일 모션/연속 모션)
        ///    
        ///    Motion Start 페이즈에서 호출되며, 연속 동작의 경우 각 모션이 시작될 때마다 호출된다.
        ///
        /// * 차지타입
        ///
        ///    Logic Start 페이즈에서 호출된다.
        ///    왜냐면, 기모으는 동작은 아직 로직상으로 모션 시작으로 보지 않기 때문
        ///
        /// </summary>
        public void OnAnimationStart(float p_Duration)
        {
            var tryAnimationModule = _MasterNode._AnimationObject;
            // 애니메이션 모션 전이 체크 이벤트 콜백을 호출한다.
            if (tryAnimationModule.OnCheckAnimationTransition(_CurrentMotionSequence))
            {
                // 애니메이션 모션 시작 이벤트 콜백을 호출한다.
                tryAnimationModule.OnAnimationStart();

                // 현재 모션 페이즈 상태에 따라 이벤트를 처리한다.
                switch (_CurrentActionPhase)
                {
                    // 유닛 액션을 처리하는 경우
                    case UnitActionTool.UnitActionPhase.LogicStart:
                    case UnitActionTool.UnitActionPhase.MotionStart:
                        _CurrentCueCount = 0;
                        _MotionRestrictFlagMask.AddFlag(UnitActionTool.MotionRestrictFlag.StartMotionCallbackSetted);
                        // 시전 이펙트를 재생한다.
                        TryInvokeCueEvent(-1);
                        break;
                    case UnitActionTool.UnitActionPhase.None:
                    case UnitActionTool.UnitActionPhase.MotionEnd:
                    case UnitActionTool.UnitActionPhase.LogicEnd:
                        break;
                }
            }
            // 스킬액션에서 지정한 모션과 다른 모션으로 전이한 경우 및 유닛 액션을 수행하기 위한 선딜레이 플래그가 없는 경우
            else if(!_MotionRestrictFlagMask.HasAnyFlagExceptNone(UnitActionTool.MotionRestrictFlag.ProgressActionFirstDelay))
            {
                // 로직상 피격 상태이고, 모션 타입도 피격 상태인 경우
                if (_MasterNode.HasState_Or(Unit.UnitStateType.STUCK) && tryAnimationModule.IsCurrentMotionOnAnimator(AnimatorParamStorage.MotionType.Hit))
                {
                    // 피격 이벤트 콜백을 호출한다.
                    // _MasterNode.OnHitActionStart();
                    // _ControlAnimationObject.OnAnimationStart();
                    // 피격 이벤트 콜백에서 위 코드를 호출하므로,
                    // 아래 블록과 상동이 아니다.
                }
                else
                {
                    // 애니메이션 모션 시작 이벤트 콜백을 호출한다.
                    tryAnimationModule.OnAnimationStart();
                    
                    // 현재 모션 페이즈 상태에 따라 이벤트를 처리한다.
                    switch (_CurrentActionPhase)
                    {
                        // 유닛 액션이 진행중이었던 경우, 초기화시켜준다.
                        case UnitActionTool.UnitActionPhase.LogicStart:
                        case UnitActionTool.UnitActionPhase.MotionStart:
                            SwitchActionPhase(UnitActionTool.UnitActionPhase.None);
                            break;
                        case UnitActionTool.UnitActionPhase.None:
                        case UnitActionTool.UnitActionPhase.MotionEnd:
                        case UnitActionTool.UnitActionPhase.LogicEnd:
                            break;
                    }
                }
            }
        }
        
        /// <summary>
        /// 모션의 Cue 타임 스탬프에서 호출되는 콜백함수
        /// 주로 적 타격 타이밍을 표시한다.
        /// </summary>
        public void OnAnimationCue(float p_Duration)
        {
            _MotionRestrictFlagMask.RemoveFlag(UnitActionTool.MotionRestrictFlag.NextMotionReserved);
#if !SERVER_DRIVE
            if ((_MasterNode.HasAnyAuthority(UnitTool.UnitAuthorityFlag.EveryPlayer) || LamiereGameManager.GetInstanceUnSafe.isTutorial) && _MasterNode._ActableObject.IsMovingState())
            {
                var masterUnit = _MasterNode as LamiereUnit;
                
                if (!ReferenceEquals(null, masterUnit))
                {
                    SfxSpawnManager.GetInstance.GetSfx(SfxSpawnManager.GetInstance.CalculateFootStepSound(_MasterNode.CurrentTextureType, masterUnit.Vocation), default, _MasterNode);
                }
            }
#endif
            
            if (_MotionRestrictFlagMask.HasAnyFlagExceptNone(UnitActionTool.MotionRestrictFlag.StartMotionCallbackSetted))
            {
                // 해당 Cue 번호에 매핑된 이벤트가 있는지 검증하고 실행시킨다.
                TryInvokeCueEvent(_CurrentCueCount);
                _CurrentCueCount++;

                var (tryInvokeReserveUnitAction, resultMessage) = InvokeReservedUnitAction();
                if (!tryInvokeReserveUnitAction)
                {
                    // 만약 현재 모션이 Cue의 갯수에 의해 다음 모션으로 전이되어야 하는 경우
                    // 이 때, _CurrentCueCount 값의 하한은 이전 로직에서 +1이 되므로 1이 된다.
                    // if (_CurrentCueCount == _CurrentMotionSequence.CueClipCount && HasNextMotionSequence())
                    {
                        // 애니메이션 가속 등으로 현재 애니메이션의 다음 Cue 콜백이 발동하지 않도록 플래그를 제거해준다.
                        _MotionRestrictFlagMask.RemoveFlag(UnitActionTool.MotionRestrictFlag.StartMotionCallbackSetted);
                        InvokeNextAction();
                    }
                }
            }
        }

        /// <summary>
        /// 모션의 Stop 타임 스탬프에서 호출되는 콜백함수.
        /// 경직이나, 특정 모션에서 대기해야하는 경우 사용한다.
        /// </summary>
        public void OnAnimationMotionStop(float p_Duration)
        {
            var tryAnimationModule = _MasterNode._AnimationObject;
            
            /* 피격시 경직에 사용되는 섹션 */
            if (_MasterNode.HasState_Or(Unit.UnitStateType.STUCK))
            {
                tryAnimationModule.SetAnimationEnable(false);
                // _MasterNode.OnHitMotionStopCued();
            }
            /* 차징 스킬 시전시, 기모으는 연출을 위해 사용되는 섹션 */
            else
            {
                switch (_CurrentActionPhase)
                {
                    case UnitActionTool.UnitActionPhase.None:
                        break;
                    case UnitActionTool.UnitActionPhase.LogicStart:
                        var isChargingAction = _CurrentMotionSequence.UnitActionTriggerType ==
                                               UnitActionTool.UnitActionTriggerType.Charge;
                        if (isChargingAction)
                        {
                            tryAnimationModule.SetAnimationEnable(false);
                        }
                        break;
                    case UnitActionTool.UnitActionPhase.MotionStart:
                        break;
                    case UnitActionTool.UnitActionPhase.MotionEnd:
                        break;
                    case UnitActionTool.UnitActionPhase.LogicEnd:
                        break;
                }
            }   
        }
        
        /// <summary>
        /// 모션의 종료 타임 스탬프에서 호출되는 콜백함수.
        /// </summary>
        public void OnAnimationEnd(float p_Duration)
        {
            var tryAnimationModule = _MasterNode._AnimationObject;
            _MotionRestrictFlagMask.RemoveFlag(UnitActionTool.MotionRestrictFlag.StartMotionCallbackSetted);
         
            if (_MasterNode.HasState_Or(Unit.UnitStateType.STUCK))
            {
                // _MasterNode.OnHitActionTerminate();
            }
            else
            {
                switch (_CurrentActionPhase)
                {
                    // 스킬 로직에 의존하지 않는 대기, 이동, 점프 모션 등의 모션 종료 콜백 
                    case UnitActionTool.UnitActionPhase.None:
                        tryAnimationModule.OnAnimationEnd();
                        break;
                    // 해당 페이즈에 진입하는 경우
                    // 1. 차징 모션인데, StopMotion 콜백이 없어서 모션만이 먼저 끝나버린 경우
                    case UnitActionTool.UnitActionPhase.LogicStart:
                        if (!_MotionRestrictFlagMask.HasAnyFlagExceptNone(UnitActionTool.MotionRestrictFlag.NextMotionReserved) 
                            && _CurrentMotionSequence.UnitActionTriggerType == UnitActionTool.UnitActionTriggerType.Charge)
                        {
                            TryInvokeCueEvent(-2);
                            var (valid, handler) = _UnitActionDelayEventHandler.GetValue();
                            if (valid)
                            {
                                handler.CancelEvent();
                            }
                            tryAnimationModule.OnAnimationEnd();
                            SwitchActionPhase(UnitActionTool.UnitActionPhase.MotionEnd);
                        }
                        break;
                    // 스킬 액션 모션이 정상 종료된 경우
                    case UnitActionTool.UnitActionPhase.MotionStart:
                    {
                        if (!_MotionRestrictFlagMask.HasAnyFlagExceptNone(UnitActionTool.MotionRestrictFlag.NextMotionReserved))
                        {
                            TryInvokeCueEvent(-2);
                            /*if (HasNextMotionSequence())
                            {
                                InvokeNextAction();
                            }
                            else*/
                            {
                                tryAnimationModule.OnAnimationEnd();
                                SwitchActionPhase(UnitActionTool.UnitActionPhase.MotionEnd);
                            }
                        }
                    }
                        break;
                    // 허수
                    case UnitActionTool.UnitActionPhase.MotionEnd:
                        break;
                    case UnitActionTool.UnitActionPhase.LogicEnd:
                        break;
                }
            }
        }
        
        /// <summary>
        /// 애니메이터 컴포넌트로부터 루트모션에 의한 아핀변환이 발생한 경우 호출되는 콜백,
        /// Update 이후 LateUpdate 이전에 호출된다.
        /// </summary>
        public void OnAnimatorMove()
        {
            _MasterNode._AnimationObject.OnAnimatorMove();
        }
        
        #endregion

        #region <Methods>
        
        /// <summary>
        /// 현재 액션의 가장 첫번째 이벤트를 호출하는 메서드
        /// </summary>
        protected void TryInvokePrimeEvent()
        {
            var targetRecord = _CurrentUnitAction._UnitActionPresetRecord;
            targetRecord.TriggerEventPreset(GetObjectDeployEventRecord());
        }
        
        /// <summary>
        /// 현재 액션의 지정한 큐 혹은 특정한 예약 상수(-1, -2)에서 처리할 이벤트가 있는지 찾고, 호출하는 메서드
        /// -1은 모션 시작, -2는 모션 종료를 나타낸다.
        /// </summary>
        protected void TryInvokeCueEvent(int p_CueCount)
        {
            var targetRecord = _CurrentUnitAction._UnitActionPresetRecord;
            targetRecord.TriggerEventPreset(GetObjectDeployEventRecord(), _CurrentActionMotionSequenceIndex, p_CueCount);
        }

        #endregion
    }
}