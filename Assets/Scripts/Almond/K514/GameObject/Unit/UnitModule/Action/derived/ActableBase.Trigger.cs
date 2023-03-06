using UnityEngine;

namespace k514
{
    public partial class ActableBase
    {
        #region <Fields>
        
        /// <summary>
        /// 현재 진행중인 액션 트리거 타입
        /// </summary>
        private ActableTool.UnitActionType _CurrentActionTrigger;
        
        /// <summary>
        /// 현재 진행중인 액션을 입력한 입력 프리셋
        /// </summary>
        private InputTriggerPreset _CurrentActivatedInputTriggerPreset;
        
        /// <summary>
        /// 액션 트리거가 입력됬다면, 발동 유무와 상관없이 갱신되는 입력 정보 프리셋
        /// </summary>
        private InputTriggerPreset _ReservedInputTriggerPreset;

        /// <summary>
        /// 특정 입력을 지속하는 것으로 누적되는 값, x z의 값은 방향키로부터 누적되고
        /// y 값은 현재 트리거와 같은 타입의 입력을 지속하는 것으로 누적된다.
        /// </summary>
        private Vector3 _ExtraInputVector;

        #endregion

        #region <Methods>

        /// <summary>
        /// 입력에 의해 특정 액션의 조건을 체크하고 만족하면 해당 액션을 호출하는 메서드
        /// </summary>
        private (bool, UnitActionTool.UnitAction.UnitTryActionResultType) TryCastTrigger(ActableTool.UnitActionType p_TriggerType, ControllerTool.ControlEventPreset p_InputPreset)
        {
            var isPressed = p_InputPreset.IsInputPress;
            var result = (false, UnitActionTool.UnitAction.UnitTryActionResultType.Fail_HoldCommand);
            var isReleased = p_InputPreset.IsInputRelease;
    
            if (isPressed)
            {
                _ReservedInputTriggerPreset = new InputTriggerPreset(p_TriggerType, p_InputPreset, _CurrentActionPhase);
                result = (true, UnitActionTool.UnitAction.UnitTryActionResultType.Success_Reserved);
            }
            
            if (p_TriggerType == ActableTool.UnitActionType.Move)
            {
                var mainUV = p_InputPreset.ViewPortUV;
                _ExtraInputVector.x = mainUV.x;
                _ExtraInputVector.z = mainUV.z;
            }
            
            switch (_CurrentActionPhase)
            {
                // 현재 진행중인 스킬이 없는 경우
                case UnitActionTool.UnitActionPhase.None:
                    var commandType = p_InputPreset.CommandType;

                    if (IsUnWeaponModule())
                    {
                        commandType = _DefaultCommand;
                    }
                    var restrictActiveFlag = p_InputPreset.ControlMessageFlagMask.HasAnyFlagExceptNone(ControllerTool.ControlMessageFlag.RestrictActivateSpell);
                    // isPressed 조건을 제거하면, 꾹누르는 경우 스킬이 계속 나간다.
                    if (isPressed)
                    {
                        if (!restrictActiveFlag && IsCooldown(commandType))
                        {
                            result = (false, UnitActionTool.UnitAction.UnitTryActionResultType.Fail_Cooldown);
                        }
                        else
                        {
                            var (valid, tryAction) = GetUnitActionToActivateFirstEntry(commandType);

                            switch (tryAction._UnitActionEntryType)
                            {
                                case UnitActionTool.UnitAction.UnitActionEntryType.Invalid:
                                {
                                    result = (false, UnitActionTool.UnitAction.UnitTryActionResultType.Fail_InvalidRecord);
                                }
                                    break;
                                case UnitActionTool.UnitAction.UnitActionEntryType.InstantSpell:
                                {
                                    if (valid)
                                    {
                                        result = CheckUnitActionEntryCondition(tryAction);
                                        if (restrictActiveFlag || result.Item1)
                                        {
                                            InvokeUnitAction(new InputTriggerPreset(p_TriggerType, p_InputPreset, _CurrentActionPhase), tryAction, false);
                                        }
                                    }   
                                }
                                    break;
                                case UnitActionTool.UnitAction.UnitActionEntryType.MotionOnly:
                                case UnitActionTool.UnitAction.UnitActionEntryType.MotionSpell:
                                {
                                    if (valid)
                                    {
                                        result = CheckUnitActionEntryCondition(tryAction);
                                        if (restrictActiveFlag || result.Item1)
                                        {
                                            _MasterNode._MindObject.StopMove(ActableTool.IdleState.Relax);
                                            InvokeUnitAction(new InputTriggerPreset(p_TriggerType, p_InputPreset, _CurrentActionPhase), tryAction, false);
                                        }
                                    }
                                }
                                    break;
                            }
                        }
                    }

                    break;
                // 모션 시작 전, 차징 로직을 수행하고 있는데 입력이 발생하는 경우
                case UnitActionTool.UnitActionPhase.LogicStart:
                    // 트리거가 떼어졌거나, 예약된 다른 액션이 없는 경우
                    if (isReleased || !InvokeReservedUnitAction().Item1)
                    {
                        // 입력한 트리거가 현재 진행중인 트리거와 같은 타입이었던 경우
                        if(p_TriggerType == _CurrentActionTrigger)
                        {
                            // 현재 진행중인 액션이 차지 타입이었던 경우
                            var triggerType = _CurrentUnitAction[0].UnitActionTriggerType;
                            switch (triggerType)
                            {
                                case UnitActionTool.UnitActionTriggerType.Simple:
                                    break;
                                case UnitActionTool.UnitActionTriggerType.Charge:
                                    // 충전 정도를 처리해준다. 만약 트리거 릴리스 이벤트였던 경우, 상태를 전이시켜준다.
                                    if (p_InputPreset.IsInputRelease)
                                    {
                                        var (valid, handler) = _UnitActionDelayEventHandler.GetValue();
                                        if (valid)
                                        {
                                            handler.CancelEvent();
                                        }
                                        _ExtraInputVector.y = p_InputPreset.InputDuration;
                                        SwitchActionPhase(UnitActionTool.UnitActionPhase.MotionStart);
                                    }
                                    break;
                            }
                        }
                    }
                    break;
                case UnitActionTool.UnitActionPhase.MotionStart:
                case UnitActionTool.UnitActionPhase.MotionEnd:
                case UnitActionTool.UnitActionPhase.LogicEnd:
                    if (isPressed)
                    {
                        InvokeReservedUnitAction();
                    }
                    break;
            }
            
            return result;
        }

        #endregion
        
        #region <Structs>

        public struct InputTriggerPreset
        {
            #region <Fields>

            /// <summary>
            /// 입력된 트리거 타입
            /// </summary>
            public ActableTool.UnitActionType _InputTrigger;
        
            /// <summary>
            /// 입력된 입력프리셋
            /// </summary>
            public ControllerTool.ControlEventPreset _InputPreset;

            /// <summary>
            /// 해당 입력이 어떤 페이즈에서 이루어졌는지 표시하는 필드
            /// </summary>
            public UnitActionTool.UnitActionPhase _InputtedPhase;

            /// <summary>
            /// 입력된 타임 스탬프
            /// </summary>
            public float _TimeStamp;
            
            #endregion

            #region <Constructor>

            public InputTriggerPreset(ActableTool.UnitActionType p_InputTrigger, ControllerTool.ControlEventPreset p_InputPreset, UnitActionTool.UnitActionPhase p_InputtedPhase)
            {
                _InputTrigger = p_InputTrigger;
                _InputPreset = p_InputPreset;
                _InputtedPhase = p_InputtedPhase;
                _TimeStamp = Time.time;
            }

            #endregion

            #region <Operator>

            public override bool Equals(object p_Right)
            {
                return Equals((InputTriggerPreset)p_Right);
            }

            public bool Equals(InputTriggerPreset r)
            {
                return 
                    _TimeStamp.IsReachedValue(r._TimeStamp) 
                    && _InputtedPhase == r._InputtedPhase 
                    && _InputTrigger == r._InputTrigger;
            }
            
            public override int GetHashCode()
            {
                return ((int)_InputtedPhase + 1) * ((int)_InputTrigger + 1);
            }

            public static bool operator ==(InputTriggerPreset p_Left, InputTriggerPreset p_Right)
            {
                return p_Left.Equals(p_Right);
            }

            public static bool operator !=(InputTriggerPreset p_Left, InputTriggerPreset p_Right)
            {
                return !(p_Left == p_Right);
            }

            #endregion
        }

        #endregion
    }
}