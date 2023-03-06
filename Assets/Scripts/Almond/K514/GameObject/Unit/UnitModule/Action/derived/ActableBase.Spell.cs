using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace k514
{
    public partial class ActableBase
    {
        #region <Callbacks>

        public (bool, UnitActionTool.UnitAction.UnitTryActionResultType) OnHandleSpellAction(ControllerTool.ControlEventPreset p_Preset)
        {
            if (_MasterNode.HasState_Or(Unit.UnitStateType.UnitTryActionFilterMask))
            {
                return (false, UnitActionTool.UnitAction.UnitTryActionResultType.Fail_UnitState);
            }
            else
            {
                return TryCastTrigger(ActableTool.UnitActionType.ActSpell, p_Preset);
            }
        }

        public (bool, UnitActionTool.UnitAction.UnitTryActionResultType) OnActionTriggered(ActableTool.UnitActionType p_ActionType, ControllerTool.ControlEventPreset p_Preset)
        {
            return _ActionEventHandlerCollection[p_ActionType].Invoke(p_Preset);
        }

        #endregion

        #region <Methods>

        public List<ControllerTool.CommandType> GetAvailableCommandTypeList()
        {
            _AvailableCommandList.Clear();
            foreach (var commandType in _CommandList)
            {
                if (IsSpellEnterable(commandType).Item1)
                {
                    _AvailableCommandList.Add(commandType);
                }
            }
            return _AvailableCommandList;
        }

        public ControllerTool.CommandType GetRandomAvailableCommandType()
        {
            var availableCommandList = GetAvailableCommandTypeList();
            var listCount = availableCommandList.Count;

            return listCount > 0 ? availableCommandList[Random.Range(0, listCount)] : _DefaultCommand;
        }

        /// <summary>
        /// 입력받은 입력 타입과 매핑되어 있는 유닛 액션 타입을 리턴하는 메서드
        /// </summary>
        public (bool, ActableTool.UnitActionType) GetCommandType(ControllerTool.CommandType p_CommandType)
        {
            var targetTable = UnitActionCommandIndexData.GetInstanceUnSafe.GetTable();
            if (targetTable.TryGetValue(p_CommandType, out var o_TableRecord))
            {
                var actionType = o_TableRecord.ActionType;
                switch (actionType)
                {
                    case ActableTool.UnitActionType.None:
                    case ActableTool.UnitActionType.Move:
                    case ActableTool.UnitActionType.Jump:
                        return (true, actionType);
                    default:
                        return (false, ActableTool.UnitActionType.None);
                }
            }
            else
            {
                return HasActionCommand(p_CommandType) ? (true, ActableTool.UnitActionType.ActSpell) : (false, ActableTool.UnitActionType.None);
            }
        }

        public (bool, UnitActionTool.UnitAction) GetPrimeUnitAction(ControllerTool.CommandType p_Type)
        {
            if (_InputCommandMappedActionPresetTable.TryGetValue(p_Type, out var o_actionPreset))
            {
                return o_actionPreset.GetPrimeAction();
            }
            else
            {
                return default;
            }
        }

        public (bool, UnitActionTool.UnitAction) GetUnitAction(ControllerTool.CommandType p_Type)
        {
            if (_InputCommandMappedActionPresetTable.TryGetValue(p_Type, out var o_actionPreset))
            {
                return o_actionPreset.GetTransitionAction();
            }
            else
            {
                return default;
            }
        }
        
        public (bool, UnitActionTool.UnitAction) GetUnitActionValid(ControllerTool.CommandType p_Type)
        {
            return _InputCommandMappedActionPresetTable[p_Type].GetTransitionAction();
        }
        
        public (bool, float) GetUnitActionAttackRange(ControllerTool.CommandType p_Type)
        {
            if (_InputCommandMappedActionPresetTable.TryGetValue(p_Type, out var o_actionPreset))
            {
                var (valid, action) = o_actionPreset.GetTransitionAction();
                if (valid)
                {
                    return (true, action._UnitActionPresetRecord.AttackRange);
                }
                else
                {
                    return (false, o_actionPreset.GetTransitionAction().Item2._UnitActionPresetRecord.AttackRange);
                }
            }
            else
            {
                return default;
            }
        }

        public (bool, ThinkableTool.AIUnitFindType) GetUnitActionUnitFindType(ControllerTool.CommandType p_Type)
        {
            if (_InputCommandMappedActionPresetTable.TryGetValue(p_Type, out var o_actionPreset))
            {
                var (valid, action) = o_actionPreset.GetTransitionAction();
                if (valid)
                {
                    var tryFindType = action._UnitActionPresetRecord.UnitFindType;
                    return (true, tryFindType == ThinkableTool.AIUnitFindType.None ? _MasterNode._MindObject._MindRecord.DefaultUnitFindType : tryFindType);
                }
                else
                {
                    return (false, _MasterNode._MindObject._MindRecord.DefaultUnitFindType);
                }
            }
            else
            {
                return (false, _MasterNode._MindObject._MindRecord.DefaultUnitFindType);
            }
        }

        private (bool, UnitActionTool.UnitAction) GetUnitActionToActivateFirstEntry(ControllerTool.CommandType p_Type)
        {
            var actionPreset = _InputCommandMappedActionPresetTable[p_Type];
            return actionPreset.GetEntryAction();
        }
        
        private (bool, UnitActionTool.UnitAction) GetUnitActionToActivateOtherCommand(ControllerTool.CommandType p_CurrentType, ControllerTool.CommandType p_ReservedType)
        {
            if (!_InputCommandMappedActionPresetTable.ContainsKey(p_CurrentType)) return (false, default);
            var currentActionPreset = _InputCommandMappedActionPresetTable[p_CurrentType];
            var currentActionIndex = currentActionPreset.GetCurrentIndex();
            var actionPreset = _InputCommandMappedActionPresetTable[p_ReservedType];
            return actionPreset.GetUpdateAction(currentActionIndex);
        }
        
        private (bool, UnitActionTool.UnitAction) GetUnitActionToActivateSameCommand(ControllerTool.CommandType p_Type)
        {
            var actionPreset = _InputCommandMappedActionPresetTable[p_Type];
            return actionPreset.GetUpdateAction(0);
        }
        
        public (bool, UnitActionTool.UnitAction) GetCurrentUnitAction()
        {
            return (!ReferenceEquals(null, _CurrentUnitAction), _CurrentUnitAction);
        }
        
        #endregion
    }
}