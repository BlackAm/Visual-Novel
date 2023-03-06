using System.Collections.Generic;

namespace k514
{
    /// <summary>
    /// 파생된 논리 플로우에 의해, 유닛이 자율적으로 행동하는 사고회로 모듈 기저 클래스
    /// </summary>
    public abstract partial class AutonomyAIBase : ThinkableBase
    {
        #region <Fields>

        /// <summary>
        /// 해당 모듈을 기술하는 테이블 레코드
        /// </summary>
        public new IThinkableAutonomyTableRecordBridge _MindRecord { get; private set; }

        /// <summary>
        /// 각 AI 상태별 변수 프리셋
        /// </summary>
        protected Dictionary<ThinkableTool.AIState, ThinkableTool.AIStatePreset> _StatePresetRecord;

        /// <summary>
        /// 현재 AI 상태
        /// </summary>
        protected ThinkableTool.AIState _CurrentAIState;

        /// <summary>
        /// 현재 AI 상태 프리셋
        /// </summary>
        protected ThinkableTool.AIStatePreset _CurrentAIPreset;

        /// <summary>
        /// 커맨드 예약 타입
        /// </summary>
        protected ControllerTool.CommandType _ReservedCommand;
                        
        /// <summary>
        /// 커맨드 예약 타입
        /// </summary>
        protected ThinkableTool.AIReserveCommand _CommandReserveType;
        
        #endregion

        #region <Callbacks>
        
        public override IThinckable OnInitializeAI(UnitAIDataRoot.UnitMindType p_MindType, Unit p_TargetUnit, IThinkableTableRecordBridge p_MindPreset)
        {
            _MindType = p_MindType;
            _MasterNode = p_TargetUnit;
            base._MindRecord = p_MindPreset;
            _MindRecord = (IThinkableAutonomyTableRecordBridge) p_MindPreset;

            _StatePresetRecord = new Dictionary<ThinkableTool.AIState, ThinkableTool.AIStatePreset>();
            foreach (var aiState in ThinkableTool._AIState_Enumerator)
            {
                _StatePresetRecord.Add(aiState, ThinkableTool.AIStatePreset.GetDefaultAIStatePreset());
            }
            
            LoadAIExtraFlagFromTableRecord();
            return this;
        }
        
        public override void OnMasterNodePooling()
        {
            ResetWander();
            LoadAIExtraFlagFromTableRecord();

            // 이후에 OnModuleNotify 콜백이 호출되므로, 위의 필드들을 먼저 초기화시켜준다.
            base.OnMasterNodePooling();
        }
        
        protected override void OnModuleNotify()
        {
            base.OnModuleNotify();
            
            OnModuleNotifyWander();
            _ForceMovePreset = default;
            if (!_MasterNode.IsQuestTargetUnit)
            {
                TryCheckAwakeModule(true);
            }
        }

        protected override void OnModuleSleep()
        {
            base.OnModuleSleep();
            
            ClearReserveCommand();
            SetSleepMind(false);
        }

        public override void OnMasterNodeRetrieved()
        {
            base.OnMasterNodeRetrieved();
            
            SetSlaveMasterUnit(null);
        }

        public override void OnUpdate(float p_DeltaTime)
        {
            if (_AIExtraFlagMask.HasAnyFlagExceptNone(ThinkableTool.AIExtraFlag.RemoteOrder))
            {
            }
            else
            {
                // 현재 점프 로직 혹은 체공 상태라면, 인공지능을 갱신하지 않는다.
                var offsetValid = _MasterNode._PhysicsObject.OnUpdatePhysicsAutonomyJump(p_DeltaTime);
                if (offsetValid && !_MasterNode.HasState_Or(Unit.UnitStateType.STUCK | Unit.UnitStateType.DEAD | Unit.UnitStateType.UnitAIAwakeableTimerEventFilterMask))
                {
                    // 스킬 시전 중이면 인공지능을 갱신하지 않는다. 단, 공격 상태에서는 갱신한다.
                    if (!_MasterNode.HasState_Or(Unit.UnitStateType.DRIVESKILL)
                        || _CurrentAIState == ThinkableTool.AIState.Attack)
                    {
                        if (_MasterNode.MasterNode)
                        {
                            OnUpdateSlaveState();
                        }
                        OnUpdateAIState();
                    }
                }
            }
        }

        /// <summary>
        /// AI 상태 제어 로직을 기술하는 업데이트 함수
        /// </summary>
        protected abstract void OnUpdateAIState();

        public override void OnUpdate_TimeBlock()
        {
            if (_AIExtraFlagMask.HasAnyFlagExceptNone(ThinkableTool.AIExtraFlag.RemoteOrder))
            {
            }
            else
            {
                if (_CurrentAIState != ThinkableTool.AIState.None)
                {
                    if (!_MasterNode.HasState_Or(Unit.UnitStateType.UnitIdlableFilterMask))
                    {
                        _CurrentAIPreset.StateTransitionCount--;
                        OnUpdateWanderingPivot();
                    }
                }

                if (_MasterNode.MasterNode)
                {
                    OnUpdateTimeBlockSlaveState();
                }

                OnUpdateAIState_TimeBlock();
            }
        }

        protected void OnUpdateAIState_TimeBlock()
        {
            switch (_CurrentAIState)
            {
                case ThinkableTool.AIState.Trace:
                    if (_MasterNode.FocusNode)
                    {
                        // UnitInteractManager.GetInstance.UpdateSqrDistanceWhenTargetMoved(_MasterNode, _MasterNode.FocusNode);
                    }
                    else
                    {
                        var hasReturnHomeFlag = _AIExtraFlagMask.HasAnyFlagExceptNone(ThinkableTool.AIExtraFlag.ReturnHome);
                        if (hasReturnHomeFlag)
                        {
                            ReturnPosition(false, false);
                        }
                        else
                        {
                            OnBreakAutonomyPath();
                        }
                    }

                    break;
            }
        }

        #endregion

        #region <Methods>

        public override void LoadAIPresetFromTableRecord()
        {
            var targetAIPresetDataCollection = _MindRecord.StatePresetRecord;
            foreach (var aiState in ThinkableTool._AIState_Enumerator)
            {
                if (targetAIPresetDataCollection == null || !targetAIPresetDataCollection.ContainsKey(aiState) || aiState == ThinkableTool.AIState.None)
                {
                    _StatePresetRecord[aiState] = ThinkableTool.AIStatePreset.GetDefaultAIStatePreset();
                }
                else
                {
                    _StatePresetRecord[aiState] = targetAIPresetDataCollection[aiState];
                }

                RandomizeAIDelay(aiState);
            }
        }

        /// <summary>
        /// 지정한 상태의 인공지능 프리셋을 리턴하는 메서드
        /// </summary>
        public override ThinkableTool.AIStatePreset GetAIPreset(ThinkableTool.AIState p_State)
        {
            return _StatePresetRecord[p_State];
        }
        
        protected bool CheckAIAwaitable()
        {
            return _AIExtraFlagMask.HasAnyFlagExceptNone(ThinkableTool.AIExtraFlag.NeverSleep);
        }

        protected void TryCheckAwakeModule(bool p_InitState)
        {
            if (CheckAIAwaitable())
            {
                SetAwakeMind(p_InitState);
            }
            else
            {
                SetSleepMind(p_InitState);
            }
        }

        protected void SetAwakeMind(bool p_InitState)
        {
            _MasterNode.SetUnitDisable(false);

            if (p_InitState)
            {
                LoadAIPresetFromTableRecord();
                foreach (var aiStatus in ThinkableTool._AIState_Enumerator)
                {
                    RandomizeAIDelay(aiStatus);
                }
                SwitchStateIdle(_MasterNode._ActableObject._CurrentIdleState);
            }
        }

        protected void SetSleepMind(bool p_InitState)
        {
            SwitchStateNone(p_InitState);
        }

        #endregion
        
        #region <Disposable>

        /// <summary>
        /// 인스턴스가 파기될 때 수행할 작업을 기술한다.
        /// </summary>
        protected override void DisposeUnManaged()
        {
        }

        #endregion
    }
}