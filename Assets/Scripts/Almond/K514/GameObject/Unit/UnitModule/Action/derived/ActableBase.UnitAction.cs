using UnityEngine;
 
namespace k514
{
    public partial class ActableBase
    {
        #region <Fields>
         
        /// <summary>
        /// 현재 진행중인 유닛 액션
        /// </summary>
        private UnitActionTool.UnitAction _CurrentUnitAction;
 
        /// <summary>
        /// 현재 진행중인 유닛액션의 모션 인덱스
        /// </summary>
        private int _CurrentActionMotionSequenceIndex;
         
        /// <summary>
        /// 현재 모션의 큐 콜백 카운터
        /// </summary>
        private int _CurrentCueCount;
         
        #endregion
 
        #region <Callbacks>
         
        /// <summary>
        /// 현재 유닛 액션의 상태를 초기화시켜야하는 타이밍에 호출되는 콜백
        /// 파라미터를 통해서 일부만 초기화 시킬 수 있다.
        /// </summary>
        private void OnInitializeCurrentUnitAction(UnitActionTool.UnitActionInitializeType p_InitializeType)
        {
            switch (p_InitializeType)
            {
                case UnitActionTool.UnitActionInitializeType.ReenterOtherCommand:
                    // 모션 전이 시에 루트 모션에 의한 회전도를 초기화 시켜준다.
                    // _ControlAnimationObject.OnMotionInterrupted();
                    _CurrentCueCount = 0;
                    GetCurrentActionPreset()?.OnInitActionCluster();
                    break;
                case UnitActionTool.UnitActionInitializeType.ReenterSameCommand:
                    // 모션 전이 시에 루트 모션에 의한 회전도를 초기화 시켜준다.
                    // _ControlAnimationObject.OnMotionInterrupted();
                    _CurrentCueCount = 0;
                    break;
                case UnitActionTool.UnitActionInitializeType.ActionOver:
                    if (_MasterNode.HasState_Or(Unit.UnitStateType.DRIVESKILL))
                    {
                        ThrowUnitActionTerminated();
                    }
                     
                    OnUpdateUnitAction(null);
                    GetCurrentActionPreset()?.OnInitActionCluster();
                 
                    // 아래 두 값은 액션 전이가 일어났더라도 해당 시점에선 같아진다.                
                    _CurrentActivatedInputTriggerPreset = default;
                    ClearReserveCommandInput();
                    _ExtraInputVector = Vector3.zero;
                    break;
            }
             
            _MotionRestrictFlagMask.ClearFlag();
            var (valid, handler) = _UnitActionDelayEventHandler.GetValue();
            if (valid)
            {
                handler.CancelEvent();
            }
             
            var tryAnimator = _MasterNode._AnimationObject;
            if (!ReferenceEquals(null, tryAnimator))
            {
                tryAnimator.SetMotionSpeedFactor(1f);
                tryAnimator.SetAnimationEnable(true);
            }
        }
                 
        private void OnUpdateUnitAction(UnitActionTool.UnitAction p_UnitAction)
        {
            _CurrentUnitAction = p_UnitAction;
            UpdateMotionSequence(
                ReferenceEquals(null, _CurrentUnitAction) 
                || _CurrentUnitAction._UnitActionEntryType == UnitActionTool.UnitAction.UnitActionEntryType.InstantSpell ? -1 : 0);
        }
         
        #endregion
 
        #region <Methods>

        public (ControllerTool.CommandType, int) FindAction(int p_ActionIndex)
        {
            foreach (var commandType in _CommandList)
            {
                var tryActionPreset = _InputCommandMappedActionPresetTable[commandType];
                var tryActionCount = tryActionPreset.GetActionCount();
                for (int i = 0; i < tryActionCount; i++)
                {
                    var actionTuple = tryActionPreset.GetAction(i);
                    if (actionTuple.Item1)
                    {
                        /*var action = actionTuple.Item2;
                        if (action._UnitActionPresetRecord.KEY == p_ActionIndex)*/
                        {
                            return (commandType, i);
                        }
                    }
                    else
                    {
                        goto OuterLoop;
                    }
                }
                OuterLoop: ;
            }

            return (ControllerTool.CommandType.None, -1);
        }
        
        public bool HasActionCommand(ControllerTool.CommandType p_CommandType)
        {
            return _InputCommandMappedActionPresetTable.ContainsKey(p_CommandType);
        }
        
        public (bool, UnitActionTool.UnitAction.UnitTryActionResultType) IsSpellEnterable(ControllerTool.CommandType p_CommandType)
        {
            if (HasActionCommand(p_CommandType))
            {
                if (IsCooldown(p_CommandType))
                {
                    return (false, UnitActionTool.UnitAction.UnitTryActionResultType.Fail_Cooldown);
                }
                else
                {
                    return CheckUnitActionEntryCondition(p_CommandType);
                }
            }
            else
            {
                return (false, UnitActionTool.UnitAction.UnitTryActionResultType.Fail_InvalidCommandType);
      
            }
        }

        /// <summary>
        /// 지정한 커맨드 타입에 현재 진입가능한지 검증하는 메서드
        /// </summary>      
        private (bool, UnitActionTool.UnitAction.UnitTryActionResultType) CheckUnitActionEntryCondition(ControllerTool.CommandType p_CommandType)
        {
            var actionPreset = _InputCommandMappedActionPresetTable[p_CommandType];
            var actionTuple = actionPreset.GetTransitionAction();
            
            if (actionTuple.Item1)
            {
                return CheckUnitActionEntryCondition(actionTuple.Item2);
            }
            else
            {
                return (false, UnitActionTool.UnitAction.UnitTryActionResultType.Fail_InvalidIndex);
            }
        }
        
        /// <summary>
        /// 지정한 유닛 액션이 현재 진입가능한지 검증하는 메서드
        /// </summary>      
        private (bool, UnitActionTool.UnitAction.UnitTryActionResultType) CheckUnitActionEntryCondition(UnitActionTool.UnitAction p_UnitAction)
        {
            var targetEnumerator = UnitActionStorage.GetInstance._ActionEntryConditionEnumerator;
            var availableState = Unit.UnitStateType.DefaultActionAvailableMask;
            
            foreach (var actionEntryCondition in targetEnumerator)
            {
                if (p_UnitAction.HasUnitActionFlag(actionEntryCondition))
                {
                    switch (actionEntryCondition)
                    {
                        /*case UnitActionTool.UnitAction.UnitActionFlag.CostMana:
                            if (_MasterNode._BattleStatusPreset.t_Current.GetMpBase() < p_UnitAction.GetCost())
                            {
                                return (false, UnitActionTool.UnitAction.UnitTryActionResultType.Fail_Mana);
                            }
                            break;*/
                        case UnitActionTool.UnitAction.UnitActionFlag.AerialOnly:
                            availableState |= Unit.UnitStateType.FLOAT;
                            break;
                        case UnitActionTool.UnitAction.UnitActionFlag.HitOnly:
                            availableState |= Unit.UnitStateType.STUCK;
                            break;
                    }
                }
            }

            var result = _MasterNode.HasState_Only(availableState);
            if (_MasterNode.IsPlayer && _MasterNode.HasState_Or(Unit.UnitStateType.SILENCE))
            {
                return (true, UnitActionTool.UnitAction.UnitTryActionResultType.Success_Silence);
            }
            else
            {
                return (result, result ? UnitActionTool.UnitAction.UnitTryActionResultType.Success_EntrySpell : UnitActionTool.UnitAction.UnitTryActionResultType.Fail_EntrySpell);
            }
        }
         
        /// <summary>
        /// 현재 액션의 다음 모션이 있는지 검증하는 논리메서드
        /// </summary>
        /*private bool HasNextMotionSequence()
        {
            return _CurrentUnitAction._UnitActionPresetRecord.MotionSequences.Count > _CurrentActionMotionSequenceIndex + 1;
        }*/

        /// <summary>
        /// 현재 액션의 다음 모션으로 모션 프리셋을 갱신시키는 메서드
        /// </summary>
        private void UpdateMotionSequence(int p_Index)
        {
            _CurrentActionMotionSequenceIndex = p_Index;

            if (_CurrentActionMotionSequenceIndex > -1)
            {
                _CurrentMotionSequence = _CurrentUnitAction[_CurrentActionMotionSequenceIndex];
            }
            else
            {
                _CurrentMotionSequence = default;
            }
        }
 
        /// <summary>
        /// 현재 로직상에 선택된 유닛 액션 레코드에 등록되어 있는 모션을 재생시키는 메서드.
        /// 다음 모션 자체로 전이하는 로직은 해당 메소드가 아니라, 애니메이션 Cue 콜백에서 수행된다.
        /// </summary>
        private void ProcessUnitActionMotion()
        {
            var tryAnimator = _MasterNode._AnimationObject;
            if (!ReferenceEquals(null, tryAnimator))
            {
                tryAnimator.SetMotionSpeedFactor(_CurrentMotionSequence.MotionSpeedRate);
                tryAnimator.SwitchMotionState(_CurrentMotionSequence._MotionType, _CurrentMotionSequence._MotionIndex,
                    AnimatorParamStorage.MotionTransitionType.Bypass_StateMachine);
            }
        }
         
        /// <summary>
        /// 액션 진행 중에 다른 액션으로의 입력이 예약이 있는지 검증하는 메서드
        /// </summary>
        private bool HasInterruptingUnitActionReserved()
        {
            return _ReservedInputTriggerPreset != _CurrentActivatedInputTriggerPreset &&
                   // 최초 입력시, 즉 None 시점에는 Current와 Reserved 프리셋이 완전 같은 내용이기 때문에, None시점은 제외한다.
                   _ReservedInputTriggerPreset._InputtedPhase != UnitActionTool.UnitActionPhase.None;
        }

        /// <summary>
        /// 지정한 페이즈에서 현재 액션이 인터럽트를 허용하고, 예약된 액션도 가능한 경우를 검증하는 메서드
        /// 인터럽트가 가능하다면 곧바로 인터럽트를 실행시킨다.
        /// </summary>
        private (bool, UnitActionTool.UnitAction.UnitTryActionResultType) InvokeReservedUnitAction()
        {
            /* SE Cond */
            // 1. 기준이 될 현재 액션이 선정되어 있는 경우
            // 2. 현재 액션의 상태가 다른 액션으로 전이가 가능한 상태/타이밍인 경우
            // 3. 사고 모듈이 다음 액션 발동을 허용하는 경우
            if (_CurrentUnitAction != null 
                && _CurrentUnitAction.CheckInterruptTiming(_CurrentActionPhase, _CurrentActionMotionSequenceIndex, _CurrentCueCount)
                /*&& _MasterNode._MindObject.CheckEnterableNextAction()*/)
            {
                // 트리거가 입력된 경우
                var reservedCommandType = _ReservedInputTriggerPreset._InputPreset.CommandType;
                if (IsUnWeaponModule())
                {
                    reservedCommandType = _DefaultCommand;
                }
                
                if (_InputCommandMappedActionPresetTable.ContainsKey(reservedCommandType) && !IsCooldown(reservedCommandType) && HasInterruptingUnitActionReserved())
                {
                    var currentCommandType = _CurrentActivatedInputTriggerPreset._InputPreset.CommandType;
                    // 다른 타입의 트리거 액션으로 전이하는 경우
                    if (reservedCommandType != currentCommandType)
                    {
                        var resultTuple = GetUnitActionToActivateOtherCommand(currentCommandType, reservedCommandType);
                        if (resultTuple.Item1)
                        {
                            var tryUnitAction = resultTuple.Item2;
                            // tryUnitAction에서 _CurrentUnitAction로 전이가 가능한지 검증한다.
                            if (CheckUnitActionEntryCondition(_CurrentUnitAction).Item1 && tryUnitAction.IsInterruptable(_CurrentUnitAction))
                            {
                                // 현재 트리거에 쿨다운을 부여한다.
                                // UpdateCurrentActionCooldown();
                                OnInitializeCurrentUnitAction(UnitActionTool.UnitActionInitializeType.ReenterOtherCommand);
                                var triggerType = tryUnitAction[0].UnitActionTriggerType;
                                switch (triggerType)
                                {
                                    // 심플 모션은 LogicStart페이즈를 스킵하여 커맨드 연계를 자연스럽게 한다.
                                    case UnitActionTool.UnitActionTriggerType.Simple:
                                        InvokeUnitAction(_ReservedInputTriggerPreset, tryUnitAction, true);
                                        break;
                                    // 차지 모션은 LogicStart에서 일어나므로 스킵해선 안된다.
                                    case UnitActionTool.UnitActionTriggerType.Charge:
                                        InvokeUnitAction(_ReservedInputTriggerPreset, tryUnitAction, false);
                                        break;
                                }
                                return (true, UnitActionTool.UnitAction.UnitTryActionResultType.Success_EntrySeqOther);
                            }
                        }
                        
                        // 전이에 실패한 경우, 예약 입력 프리셋을 초기화 시켜준다.
                        ClearReserveCommandInput();
                        return (false, UnitActionTool.UnitAction.UnitTryActionResultType.Fail_EntrySeqOther);
                    }
                    // 동일한 타입의 다음 액션으로 전이하는 경우
                    else
                    {
                        // 다음 액션이 존재하는 경우
                        var resultTuple = GetUnitActionToActivateSameCommand(currentCommandType);
                        var tryUnitAction = resultTuple.Item2;
                        if (resultTuple.Item1 && CheckUnitActionEntryCondition(tryUnitAction).Item1)
                        {
                            OnInitializeCurrentUnitAction(UnitActionTool.UnitActionInitializeType.ReenterSameCommand);
                            var triggerType = tryUnitAction[0].UnitActionTriggerType;
                            switch (triggerType)
                            {
                                // 심플 모션은 LogicStart페이즈를 스킵하여 커맨드 연계를 자연스럽게 한다.
                                case UnitActionTool.UnitActionTriggerType.Simple:
                                    InvokeUnitAction(_ReservedInputTriggerPreset, tryUnitAction, true);
                                    break;
                                // 차지 모션은 LogicStart에서 일어나므로 스킵해선 안된다.
                                case UnitActionTool.UnitActionTriggerType.Charge:
                                    InvokeUnitAction(_ReservedInputTriggerPreset, tryUnitAction, false);
                                    break;
                            }
                            return (true, UnitActionTool.UnitAction.UnitTryActionResultType.Success_EntrySeqSame);
                        }
                        else
                        {
                            // 전이에 실패한 경우, 예약 입력 프리셋을 초기화 시켜준다.
                            ClearReserveCommandInput();
                            return (false, UnitActionTool.UnitAction.UnitTryActionResultType.Fail_EntrySeqSame);
                        }
                    }
                }
            }

            return (false, UnitActionTool.UnitAction.UnitTryActionResultType.Fail_InvalidReservedSpell);
        }
                 
        private void InvokeUnitAction(InputTriggerPreset p_InputPreset, UnitActionTool.UnitAction p_UnitAction, bool p_SkipLogicStartPhase)
        {
            var prevCommand = _CurrentActivatedInputTriggerPreset._InputPreset.CommandType;
            _CurrentActivatedInputTriggerPreset = p_InputPreset;
            _CurrentActionTrigger = p_InputPreset._InputTrigger;
            OnUpdateUnitAction(p_UnitAction);

            switch (p_UnitAction._UnitActionEntryType)
            {
                case UnitActionTool.UnitAction.UnitActionEntryType.InstantSpell:
                    var currentCommand = _CurrentActivatedInputTriggerPreset._InputPreset.CommandType;
                    _MasterNode.OnUnitActionStarted(currentCommand);
                    ThrowUnitActionTransitioned(prevCommand, currentCommand);
                    TryInvokePrimeEvent();
                    SwitchActionPhase(UnitActionTool.UnitActionPhase.LogicEnd);
                    break;
                case UnitActionTool.UnitAction.UnitActionEntryType.MotionOnly:
                case UnitActionTool.UnitAction.UnitActionEntryType.MotionSpell:
                    SwitchActionPhase(p_SkipLogicStartPhase ? UnitActionTool.UnitActionPhase.MotionStart : UnitActionTool.UnitActionPhase.LogicStart);
                    ThrowUnitActionTransitioned(prevCommand, _CurrentActivatedInputTriggerPreset._InputPreset.CommandType);
                    break;
            }
        }

        public void ClearReserveCommandInput()
        {
            _ReservedInputTriggerPreset = _CurrentActivatedInputTriggerPreset;
        }

        #endregion
    }
}