using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace k514
{
    public partial class AutonomyAIBase
    {
        #region <Callbacks>
                
        /// <summary>
        /// 예약 커맨드 타입이 None 이외의, 발동 가능한 타입으로 전이된 경우 호출된 콜백
        /// </summary>
        protected void OnReserveCommandChanged(ControllerTool.CommandType p_PrevCommandType, ControllerTool.CommandType p_CurrentCommandType)
        {
            var (_, attackRange) = _MasterNode._ActableObject.GetUnitActionAttackRange(_ReservedCommand);
            _StatePresetRecord[ThinkableTool.AIState.Attack] = new ThinkableTool.AIStatePreset(attackRange, _StatePresetRecord[ThinkableTool.AIState.Attack].SpeedRate);
        }
        
        /// <summary>
        /// 예약 커맨드 타입이 None으로 전이된 경우 호출되는 콜백
        /// </summary>
        protected void OnReserveCommandChangedNone(ControllerTool.CommandType p_PrevCommandType)
        {
            _StatePresetRecord[ThinkableTool.AIState.Attack] = new ThinkableTool.AIStatePreset(0f, _StatePresetRecord[ThinkableTool.AIState.Attack].SpeedRate);
        }

        /*private void OnSuccessActiveSpell()
        {
            var enumerator = ThinkableTool._AIReserveCommand_Enumerator;
            foreach (var aiReserveCommand in enumerator)
            {
                if (_CommandReserveType.HasAnyFlagExceptNone(aiReserveCommand))
                {
                    switch (aiReserveCommand)
                    {
                        case ThinkableTool.AIReserveCommand.Success_KeepHoldingCommandIfAvailable:
                            if (_MasterNode._ActableObject.IsSpellEnterable(_ReservedCommand).Item1)
                            {
                                // 재사용 가능하다면 현재 커맨드를 유지한다.
                            }
                            else
                            {
                                // 재사용 할 수 없다면, 해당 플래그를 제거한다.
                                _CommandReserveType.RemoveFlag(aiReserveCommand);
                            }
                            break;
                        case ThinkableTool.AIReserveCommand.Success_TurnToDefaultCommand_OnceFlag:
                            ReserveCommand(ThinkableTool.AIReserveHandleType.TurnToDefault);
                            _CommandReserveType.RemoveFlag(ThinkableTool.AIReserveCommand.TurnToDefaultCommand_OnceFlag);
                            break;
                        case ThinkableTool.AIReserveCommand.Success_TurnToRandomAvailable_FallbackDefault:
                            ReserveCommand(ThinkableTool.AIReserveHandleType.TurnToRandomAvailable_FallbackDefault);
                            break;
                        case ThinkableTool.AIReserveCommand.Success_TurnToRandomAvailable_FallbackDefault_OnceFlag:
                            ReserveCommand(ThinkableTool.AIReserveHandleType.TurnToRandomAvailable_FallbackDefault);
                            _CommandReserveType.RemoveFlag(ThinkableTool.AIReserveCommand.TurnToRandomCommand_OnceFlag);
                            break;
                        case ThinkableTool.AIReserveCommand.Success_TurnToNoneCommand_OnceFlag:
                            ReserveCommand(ThinkableTool.AIReserveHandleType.TurnToNone);
                            _CommandReserveType.RemoveFlag(ThinkableTool.AIReserveCommand.TurnToNoneCommand_OnceFlag);
                            break;
                    }
                }
            }
        }
        
        private void OnFailActiveSpell(UnitActionTool.UnitAction.UnitTryActionResultType p_FailMessageType)
        {
            var enumerator = ThinkableTool._AIReserveCommand_Enumerator;
            foreach (var aiReserveCommand in enumerator)
            {
                if (_CommandReserveType.HasAnyFlagExceptNone(aiReserveCommand))
                {
                    switch (aiReserveCommand)
                    {
                        case ThinkableTool.AIReserveCommand.Fail_TurnToDefaultCommand_OnceFlag:
                            ReserveCommand(ThinkableTool.AIReserveHandleType.TurnToDefault);
                            _CommandReserveType.RemoveFlag(ThinkableTool.AIReserveCommand.TurnToDefaultCommand_OnceFlag);
                            break;
                        case ThinkableTool.AIReserveCommand.Fail_TurnToRandomAvailable_FallbackDefault:
                            ReserveCommand(ThinkableTool.AIReserveHandleType.TurnToRandomAvailable_FallbackDefault);
                            break;
                        case ThinkableTool.AIReserveCommand.Fail_TurnToRandomAvailable_FallbackDefault_OnceFlag:
                            ReserveCommand(ThinkableTool.AIReserveHandleType.TurnToRandomAvailable_FallbackDefault);
                            _CommandReserveType.RemoveFlag(ThinkableTool.AIReserveCommand.TurnToRandomCommand_OnceFlag);
                            break;
                        case ThinkableTool.AIReserveCommand.Fail_TurnToNoneCommand_OnceFlag:
                            _CommandReserveType.RemoveFlag(ThinkableTool.AIReserveCommand.TurnToNoneCommand_OnceFlag);
                            ReserveCommand(ThinkableTool.AIReserveHandleType.TurnToNone);
                            break;
                    }
                }
            }
        }*/
        
        #endregion

        #region <Methods>

        /// <summary>
        /// 예약 커맨드를 새로 선정해야 하는 경우 호출되는 콜백
        /// </summary>
        /*public override void ReserveCommand(ThinkableTool.AIReserveHandleType p_Type)
        {
#if SERVER_DRIVE
            if (_MasterNode.IsPlayer && _MasterNode.HasState_Or(Unit.UnitStateType.SILENCE))
#else
            if (_MasterNode.IsPlayer && (_MasterNode._ActableObject.IsUnWeaponModule() || _MasterNode.HasState_Or(Unit.UnitStateType.SILENCE)))
#endif
            {
                p_Type = ThinkableTool.AIReserveHandleType.TurnToDefault;
            }
            
            var prevCommandType = _ReservedCommand;
            switch (p_Type)
            {
                case ThinkableTool.AIReserveHandleType.TurnToNone:
                    _ReservedCommand = ControllerTool.CommandType.None;
                    OnReserveCommandChangedNone(prevCommandType);
                    break;
                case ThinkableTool.AIReserveHandleType.TurnToRandomAvailable_FallbackDefault:
                    _ReservedCommand = _MasterNode._ActableObject.GetRandomAvailableCommandType();
                    OnReserveCommandChanged(prevCommandType, _ReservedCommand);
                    break;
                case ThinkableTool.AIReserveHandleType.TurnToDefault:
                    _ReservedCommand = _MasterNode._ActableObject._DefaultCommand;
                    OnReserveCommandChanged(prevCommandType, _ReservedCommand);
                    break;
            }
        }
        
        /// <summary>
        /// 지정한 커맨드로의 전이가 성공했다면 참을 리턴한다.
        /// </summary>
        public override bool ReserveCommand(ControllerTool.CommandType p_ReserveCommand, ThinkableTool.AIReserveCommand p_ReserveType, ThinkableTool.AIReserveHandleType p_FailHandleType, bool p_ReserveRestrictFlag)
        {
            _CommandReserveType = p_ReserveType;

            // None 커맨드를 예약한 경우, 현재 발동가능한 스킬 중에 하나를 랜덤으로 선정한다.
            // 발동가능한 스킬이 없다면 기본 스킬 커맨드를 선정해준다.
            if (p_ReserveCommand == ControllerTool.CommandType.None)
            {
                var prevCommandType = _ReservedCommand;

#if !SERVER_DRIVE
                if (_MasterNode.IsPlayer && (_MasterNode._ActableObject.IsUnWeaponModule() || _MasterNode.HasState_Or(Unit.UnitStateType.SILENCE)))
                {
                    _ReservedCommand = _MasterNode._ActableObject._DefaultCommand;
                }
                else
#endif
                {
                    _ReservedCommand = _MasterNode._ActableObject.GetRandomAvailableCommandType();
                }
                
                OnReserveCommandChanged(prevCommandType, _ReservedCommand);
                return false;
            }
            else
            {
                if (p_ReserveRestrictFlag)
                {
                    var prevCommandType = _ReservedCommand;
                    _ReservedCommand = p_ReserveCommand;
                    OnReserveCommandChanged(prevCommandType, _ReservedCommand);
                    return true;
                }
                else
                {
                    // 그 외의 경우에는 전이하려는 타입이 현재 발동가능한지 검증한다.
                    // 발동 불가능한 경우, p_FailHandleType에 따라 예약 이벤트를 처리한다.
                    var (valid, _) = _MasterNode._ActableObject.IsSpellEnterable(p_ReserveCommand);
                    if (valid)
                    {
                        var prevCommandType = _ReservedCommand;
                        _ReservedCommand = p_ReserveCommand;
                        OnReserveCommandChanged(prevCommandType, _ReservedCommand);
                        return true;
                    }
                    else
                    {
                        ReserveCommand(p_FailHandleType);
                        return false;
                    }
                }
            }
        }*/

        public override void ClearReserveCommand()
        {
            _CommandReserveType = ThinkableTool.AIReserveCommand.None;
            _ReservedCommand = ControllerTool.CommandType.None;
            OnReserveCommandChangedNone(_ReservedCommand);
        }

        #endregion
    }
}