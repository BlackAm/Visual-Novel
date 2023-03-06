using System;
using UnityEngine;

namespace k514
{
    /// <summary>
    /// 유닛의 행동이 어떤 논리 플로우에 의해 결정되는 것이 아닌, 사용자 입력에 의해 결정되는 사고회로 모듈 기저 클래스
    /// </summary>
    public partial class PassivityAIBase : ThinkableBase
    {
        #region <Consts>

        private const uint StackTimerInterval = 1000;
        private const int StackTimerUpdateCount = 5;

        #endregion

        #region <Fields>

        /// <summary>
        /// 해당 모듈을 기술하는 테이블 레코드
        /// </summary>
        public new IThinkablePassivityTableRecordBridge _MindRecord { get; private set; }
        
        /// <summary>
        /// 시스템 입력 이벤트 수신자
        /// </summary>
        protected ControllerEventReceiver _ControllerEventReceiver;

        /// <summary>
        /// 포커스가 만기되는 이벤트를 다루는 타이머
        /// </summary>
        private StackTimer _FocusTerminateTimer;
        
        #endregion
        
        #region <Callbacks>

        public override IThinckable OnInitializeAI(UnitAIDataRoot.UnitMindType p_MindType, Unit p_TargetUnit, IThinkableTableRecordBridge p_MindPreset)
        {
            _MindType = p_MindType;
            _MasterNode = p_TargetUnit;
            base._MindRecord = p_MindPreset;
            _MindRecord = (IThinkablePassivityTableRecordBridge) p_MindPreset;
            
            _ControllerEventReceiver =
                ControllerTool.GetInstanceUnSafe
                    .GetControllerEventSender(ControllerTool.InputEventType.ControlUnit)
                    .GetEventReceiver<ControllerEventReceiver>(ControllerTool.InputEventType.ControlUnit, OnPropertyModified);
            
            _FocusTerminateTimer = new StackTimer(StackTimerInterval, OnStackTimerTerminate, null, false);
            
            return this;
        }
        
        protected override void OnModuleNotify()
        {
            base.OnModuleNotify();
            _ControllerEventReceiver.SetReceiverBlock(false);
        }

        protected override void OnModuleSleep()
        {
            base.OnModuleSleep();
            _ControllerEventReceiver.SetReceiverBlock(true);
            _MasterNode.HaltController(true);
        }

        #endregion

        #region <Methods>

        /*public override bool AttackTo(Unit p_TargetUnit, bool p_ForceAttack,
            ControllerTool.CommandType p_ReserveCommand, ThinkableTool.AIReserveCommand p_ReserveType, ThinkableTool.AIReserveHandleType p_FailHandleType)
        {
            return ActSpell(p_TargetUnit, p_ReserveCommand, p_ReserveType, p_FailHandleType);
        }

        public override bool ActSpell(Unit p_TargetUnit, ControllerTool.ControlEventPreset p_Preset, ThinkableTool.AIReserveCommand p_ReserveType, ThinkableTool.AIReserveHandleType p_FailHandleType)
        {
            var tryCommand = p_Preset.CommandType;
            var (validCommand, ActionType) = _MasterNode._ActableObject.GetCommandType(tryCommand);
            if (validCommand)
            {
                switch (ActionType)
                {
                    case ActableTool.UnitActionType.None:
                        return false;
                    case ActableTool.UnitActionType.ActSpell:
                        /*var (isActionValid, unitAction) = _MasterNode._ActableObject.GetUnitActionValid(tryCommand);
                        if (isActionValid)
                        {
                            if (p_Preset.IsInputPress)
                            {
                                if (_MasterNode._ActableObject.IsSpellEnterable(tryCommand).Item1)
                                {
                                    var (isFindValid, tryEnemy) = FindEnemy(unitAction, Unit.UnitStateType.UnitFightableFilterMask);
                                    if (isFindValid)
                                    {
                                        if (ReferenceEquals(p_TargetUnit, tryEnemy))
                                        {
                                            SetEnemy(p_TargetUnit, PrefabInstanceTool.FocusNodeRelateType.Enemy);
                                        }
                                        else
                                        {
                                            if (p_TargetUnit.IsValid() && _MasterNode._UnitFilterResultSet.Contains(p_TargetUnit))
                                            {
                                                SetEnemy(p_TargetUnit, PrefabInstanceTool.FocusNodeRelateType.Enemy);
                                            }
                                            else
                                            {
                                                SetEnemy(tryEnemy, PrefabInstanceTool.FocusNodeRelateType.Enemy);
                                            }
                                        }
                                        
                                        if (_MasterNode.OnActionTriggered(ActionType, p_Preset, _MasterNode.IsRemoteClientUnit()).Item1)
                                        {
                                            _MasterNode.SetLookToFocus(true);
                                            return true;
                                        }
                                        else
                                        {
                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        ClearEnemy();
                                        return _MasterNode.OnActionTriggered(ActionType, p_Preset, _MasterNode.IsRemoteClientUnit()).Item1;
                                    }
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                return _MasterNode.OnActionTriggered(ActionType, p_Preset, _MasterNode.IsRemoteClientUnit()).Item1;
                            }
                        }
                        else
                        {
                            return false;
                        }#1#
                    default:
                    case ActableTool.UnitActionType.Move:
                    case ActableTool.UnitActionType.Jump:
                        return _MasterNode.OnActionTriggered(ActionType, p_Preset, false).Item1;
                }
            }
            else
            {
                return false;
            }
        }
        
        public override void ReserveCommand(ThinkableTool.AIReserveHandleType p_Type)
        {
            var availableType = _MasterNode._ActableObject.GetRandomAvailableCommandType();
            ActSpell(_MasterNode.FocusNode, availableType, ThinkableTool.AIReserveCommand.None, ThinkableTool.AIReserveHandleType.TurnToNone);
        }

        public override bool ReserveCommand(ControllerTool.CommandType p_ReserveCommand, ThinkableTool.AIReserveCommand p_ReserveType, ThinkableTool.AIReserveHandleType p_FailHandleType,
            bool p_ReserveRestrictFlag)
        {
            return ActSpell(_MasterNode.FocusNode, p_ReserveCommand, p_ReserveType, p_FailHandleType);
        }*/

        public override bool MoveTo(Vector3 p_TargetPosition, bool p_ForceMove, PhysicsTool.AutonomyPathStoppingDistancePreset p_AutonomyPathStoppingDistancePreset)
        {
            _MasterNode.HaltController(false);
            _MasterNode.SetPivotPosition(p_TargetPosition, false, true);
            var (moveValid, resultType) = _MasterNode._PhysicsObject.SetPhysicsAutonomyMove(_MasterNode.GetPivotPosition(), ObjectDeployTool.ObjectDeploySurfaceDeployType.None, p_AutonomyPathStoppingDistancePreset);
            
            return moveValid;
        }

        public override bool ReturnPosition(bool p_ForceMove, bool p_SwitchInstance)
        {
            MoveTo(_OriginPivot, p_ForceMove, PhysicsTool.AutonomyPathStoppingDistancePreset.GetStoppingRange(_MasterNode));
            return false;
        }

        public override void SetAIRange(ThinkableTool.AIState p_State, float p_Value)
        {
        }

        #endregion
        
        #region <Controller>

        /// <summary>
        /// 시스템 입력 이벤트가 발생하는 경우 호출되는 콜백에 바인드된 함수
        /// 입력 받은 키 타입과 매핑된 액션이 있다면, 마스터 노드의 해당 액션을 호출한다.
        /// </summary>
        public void OnPropertyModified(ControllerTool.InputEventType p_Type, ControllerTool.ControlEventPreset p_Preset)
        {
            if (_MasterNode.HasState_Or(Unit.UnitStateType.SystemDisable)) return;

            if (_MindRecord.FindTargetWhenActSpellFlag)
            {
                // ActSpell(_MasterNode.FocusNode, p_Preset, ThinkableTool.AIReserveCommand.None, ThinkableTool.AIReserveHandleType.TurnToNone);
            }
            else
            {
                _MasterNode.OnActionTriggered(p_Preset, false);
            }
        }

        #endregion

        #region <StackTimer>

        public void OnStackTimerTerminate(StackTimer p_StackTimer)
        {
            if (_MasterNode.IsValid())
            {
            }
        }

        #endregion
        
        #region <Disposable>

        /// <summary>
        /// 인스턴스가 파기될 때 수행할 작업을 기술한다.
        /// </summary>
        protected override void DisposeUnManaged()
        {
            _ControllerEventReceiver.Dispose();
            _ControllerEventReceiver = null;
        }

        #endregion
    }
}