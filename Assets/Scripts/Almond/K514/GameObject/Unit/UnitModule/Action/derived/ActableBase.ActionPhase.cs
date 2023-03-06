using UnityEngine;

namespace k514
{
    public partial class ActableBase
    {
        #region <Fields>
        
        /// <summary>
        /// 현재 진행중인 유닛 액션 페이즈
        /// </summary>
        private UnitActionTool.UnitActionPhase _CurrentActionPhase;

        /// <summary>
        /// 현재 실행중인 모션 프리셋, _CurrentActionMotionSequenceIndex 프로퍼티에 의해 갱신되는 값
        /// </summary>
        private UnitActionTool.UnitAction.MotionSequence _CurrentMotionSequence;

        /// <summary>
        /// 현재 진행중인 유닛 액션 딜레이 이벤트 핸들러
        /// </summary>
        private SafeReference<object, GameEventTimerHandlerWrapper> _UnitActionDelayEventHandler;

        #endregion

        #region <Callbacks>

        private void ThrowUnitActionStarted()
        {
            var triggerType = _CurrentMotionSequence.UnitActionTriggerType;
            switch (triggerType)
            {
                // 심플 타입의 경우에는 스킬 모션을 수행하기 까지 선딜레이가 존재하므로 이동 모션등을 캔슬해줄 필요가 있다.
                case UnitActionTool.UnitActionTriggerType.Simple:
                    TurnIdleState(ActableTool.IdleState.Combat, AnimatorParamStorage.MotionTransitionType.Bypass_StateMachine);
                    break;
                // 차지 타입의 경우에는 곧바로 차지 모션에 돌입하기 때문에, 별도의 모션전이를 필요로 하지 않는다.
                case UnitActionTool.UnitActionTriggerType.Charge:
                    TurnIdleState(ActableTool.IdleState.Combat, AnimatorParamStorage.MotionTransitionType.Bypass_StateMachine);
                    break;
            }

            _MasterNode.OnUnitActionStarted(_CurrentActivatedInputTriggerPreset._InputPreset.CommandType);
        }
        
        private void ThrowUnitActionTransitioned(ControllerTool.CommandType p_PrevCommand, ControllerTool.CommandType p_CurrentCommand)
        {
            var currentUnitActionMana = _CurrentUnitAction.GetCost();
            // _MasterNode.OnCostMana(currentUnitActionMana);
            _MasterNode.OnUnitActionTransitioned(p_PrevCommand, p_CurrentCommand);
        }
        
        private void ThrowUnitActionTerminated()
        {
            var terminatedTriggerType = _CurrentActivatedInputTriggerPreset._InputTrigger;
            switch (terminatedTriggerType)
            {
                case ActableTool.UnitActionType.None:
                case ActableTool.UnitActionType.Move:
                case ActableTool.UnitActionType.Jump:
                    break;
                case ActableTool.UnitActionType.ActSpell:
                    // 현재 트리거에 쿨다운을 부여한다.
                    // UpdateCurrentActionCooldown();
                    break;
            }
            
            _MasterNode.OnUnitActionTerminated();
        }

        #endregion
        
        #region <Methods>

        /// <summary>
        /// 유닛 액션의 상태 전이 및 상태 전이 시에 각 페이즈에서 수행할 로직을 기술하는 메서드
        /// </summary>
        public void SwitchActionPhase(UnitActionTool.UnitActionPhase p_PhaseType)
        {
            // 상태 전이 표시
            _CurrentActionPhase = p_PhaseType;

            switch (p_PhaseType)
            {
                /* None 상태로 전이하는 경우 */
                // 해당 유닛이 보유한 관련 값을 초기상태로 되돌린다.
                // 어떤 상태에서라도 None으로 전이되는 경우, 유닛의 상태는 기본 상태가 되어야한다.
                case UnitActionTool.UnitActionPhase.None:
                    OnInitializeCurrentUnitAction(UnitActionTool.UnitActionInitializeType.ActionOver);
                    break;
                
                /* Logic Start 상태로 전이하는 경우 */
                // 해당 유닛이 어떤 액션을 실행하는 경우, 로직 준비 => 모션 실행 순서로 동작해야한다.
                case UnitActionTool.UnitActionPhase.LogicStart:
                {
                    // 유닛 상태를 배틀 상태로 세트한다.
                    ThrowUnitActionStarted();
                    
                    var actionRecord = _CurrentUnitAction._UnitActionPresetRecord;
                    var triggerType = _CurrentMotionSequence.UnitActionTriggerType;
                    switch (triggerType)
                    {
                        case UnitActionTool.UnitActionTriggerType.Simple:
                            // 액션 선딜레이를 세고, 다음 상태로 전이시킨다.
                            var firstDelay = actionRecord.FirstDelay;
                            if (firstDelay < 50)
                            {
                                SwitchActionPhase(UnitActionTool.UnitActionPhase.MotionStart);
                            }
                            else
                            {
                                _MotionRestrictFlagMask.AddFlag(UnitActionTool.MotionRestrictFlag.ProgressActionFirstDelay);

                                GameEventTimerHandlerManager.GetInstance.SpawnSafeEventTimerHandler(ref _UnitActionDelayEventHandler, this, SystemBoot.TimerType.GameTimer, false);
                                var (_, eventHandler) = _UnitActionDelayEventHandler.GetValue();
                                eventHandler
                                    .AddEvent(
                                        firstDelay,
                                        handler =>
                                        {
                                            handler.Arg1.SwitchActionPhase(UnitActionTool.UnitActionPhase.MotionStart);
                                            return true;
                                        }, 
                                        null, this
                                    );
                                eventHandler.StartEvent();
                            }

                            break;
                        case UnitActionTool.UnitActionTriggerType.Charge:
                            var tryTrigger = _CurrentActivatedInputTriggerPreset._InputPreset.KeyCode;
                            var isKeepPressing = ControllerTool.GetInstanceUnSafe.GetCommandSystem(_CurrentActivatedInputTriggerPreset._InputPreset.InputEventType).GetKey(tryTrigger);
                            // 차징 버튼이 계속 눌리는 경우
                            if (isKeepPressing)
                            {
                                // 선딜레이 동안 차징을 연출할 모션을 재생시킨다.
                                ProcessUnitActionMotion();

                                goto case UnitActionTool.UnitActionTriggerType.Simple;
                            }
                            // 차징 버튼이 해제되어 있던 경우
                            else
                            {
                                // 바로 다음 페이즈로 전이시킨다.
                                ProcessUnitActionMotion();
                                SwitchActionPhase(UnitActionTool.UnitActionPhase.MotionStart);
                            }
                            break;
                    }
                }
                    break;
                
                /* Motion Start 상태로 전이하는 경우 */
                // 해당 모션이 시작되는 경우 처리할 이벤트
                case UnitActionTool.UnitActionPhase.MotionStart:
                {
                    switch (_CurrentMotionSequence.UnitActionTriggerType)
                    {
                        case UnitActionTool.UnitActionTriggerType.Simple:
                            _MotionRestrictFlagMask.RemoveFlag(UnitActionTool.MotionRestrictFlag.ProgressActionFirstDelay);
                            ProcessUnitActionMotion();
                            break;
                        // 선딜레이만큼의 시간이 경과하면 차징입력도 자동으로 종료된다.
                        case UnitActionTool.UnitActionTriggerType.Charge:
                            _MasterNode._AnimationObject.SetAnimationEnable(true);
                            break;
                    }
                }
                    break;
                /* Motion End 상태로 전이하는 경우 */
                // 해당 모션이 종료되는 경우 처리할 이벤트
                case UnitActionTool.UnitActionPhase.MotionEnd:
                {
                    // 예약된 스킬이 없는 경우에
                    if (!InvokeReservedUnitAction().Item1)
                    {
                        // 기본 모션으로 전이시킨다.
                        _MasterNode._AnimationObject.SetPlayDefaultMotion(true);

                        // 후딜레이 이후 상태를 전이시킨다.
                        var postDelay = _CurrentMotionSequence.PostDelay;

                        if (postDelay < 50)
                        {
                            SwitchActionPhase(UnitActionTool.UnitActionPhase.LogicEnd);
                        }
                        else
                        {
                            GameEventTimerHandlerManager.GetInstance.SpawnSafeEventTimerHandler(ref _UnitActionDelayEventHandler, this, SystemBoot.TimerType.GameTimer, false);
                            var (_, eventHandler) = _UnitActionDelayEventHandler.GetValue();
                            eventHandler
                                .AddEvent(
                                    postDelay,
                                    handler =>
                                    {
                                        handler.Arg1.SwitchActionPhase(UnitActionTool.UnitActionPhase.LogicEnd);
                                        return true;
                                    }, 
                                    null, this
                                );
                            eventHandler.StartEvent();
                        }
                    }
                }
                    break;
                /* Logic End 상태로 전이하는 경우 */
                // 해당 유닛을 초기화 시킨다.
                case UnitActionTool.UnitActionPhase.LogicEnd:
                    // 예약된 스킬이 없는 경우에
                    if (!InvokeReservedUnitAction().Item1)
                    {
                        ThrowUnitActionTerminated();
                        SwitchActionPhase(UnitActionTool.UnitActionPhase.None);
                    }
                    break;
            }
        }

        private void InvokeNextAction()
        {
            var record = _CurrentUnitAction._UnitActionPresetRecord;
            var motionSequenceList = record.MotionSequences;

            // 모션을 넘겨준다.
            UpdateMotionSequence(_CurrentActionMotionSequenceIndex + 1);
            if (_CurrentActionMotionSequenceIndex < motionSequenceList.Count)
            {
                _MotionRestrictFlagMask.AddFlag(UnitActionTool.MotionRestrictFlag.NextMotionReserved);

                switch (_CurrentMotionSequence.UnitActionTriggerType)
                {
                    case UnitActionTool.UnitActionTriggerType.Simple:
                    {
                        var postDelay = _CurrentMotionSequence.PostDelay;
                        if (postDelay < 50)
                        {
                            SwitchActionPhase(UnitActionTool.UnitActionPhase.MotionStart);
                        }
                        else
                        {
                            GameEventTimerHandlerManager.GetInstance.SpawnSafeEventTimerHandler(ref _UnitActionDelayEventHandler, this, SystemBoot.TimerType.GameTimer, false);
                            var (_, eventHandler) = _UnitActionDelayEventHandler.GetValue();
                            eventHandler
                                .AddEvent(
                                    postDelay,
                                    handler =>
                                    {
                                        handler.Arg1.SwitchActionPhase(UnitActionTool.UnitActionPhase.MotionStart);
                                        return true;
                                    }, 
                                    null, this
                                );
                            eventHandler.StartEvent();
                        }
                    }
                        break;
                    case UnitActionTool.UnitActionTriggerType.Charge:
                    {
                        var postDelay = _CurrentMotionSequence.PostDelay;
                        if (postDelay < 50)
                        {
                            ProcessUnitActionMotion();
                            SwitchActionPhase(UnitActionTool.UnitActionPhase.LogicStart);
                        }
                        else
                        {
                            GameEventTimerHandlerManager.GetInstance.SpawnSafeEventTimerHandler(ref _UnitActionDelayEventHandler, this, SystemBoot.TimerType.GameTimer, false);
                            var (_, eventHandler) = _UnitActionDelayEventHandler.GetValue();
                            eventHandler
                                .AddEvent(
                                    postDelay,
                                    handler =>
                                    {
                                        handler.Arg1.ProcessUnitActionMotion();
                                        handler.Arg1.SwitchActionPhase(UnitActionTool.UnitActionPhase.LogicStart);
                                        return true;
                                    }, 
                                    null, this
                                );
                            eventHandler.StartEvent();
                        }
                    }
                        break;
                }
            }
        }
        
        /// <summary>
        /// 현재 액션을 캔슬한다.
        /// </summary>
        public void CancelUnitAction(AnimatorParamStorage.MotionType p_CancelMotion)
        {
            SwitchActionPhase(UnitActionTool.UnitActionPhase.None);
            
            // None 타입 모션은 없으므로 예외가 발생한다.
            if (p_CancelMotion != AnimatorParamStorage.MotionType.None)
            {
                _MasterNode._AnimationObject.SwitchMotionState(p_CancelMotion, AnimatorParamStorage.MotionTransitionType.Restrict);
            }
        }

        /// <summary>
        /// 현재 액션 페이즈를 리턴한다.
        /// </summary>
        public UnitActionTool.UnitActionPhase GetCurrentPhase()
        {
            return _CurrentActionPhase;
        }

        #endregion
    }
}