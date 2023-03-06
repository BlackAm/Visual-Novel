using System;
using System.Collections.Generic;
using UnityEngine;

namespace k514
{
    /// <summary>
    /// 유닛 인공지능 모듈 기저 클래스
    /// </summary>
    public abstract partial class ActableBase : UnitModuleBase, IActable, IDeployEventRecord
    {
        ~ActableBase()
        {
            Dispose();
        }
        
        #region <Fields>

        /// <summary>
        /// 해당 모듈의 타입
        /// </summary>
        public UnitActionDataRoot.ActableType _ActableType { get; protected set; }

        /// <summary>
        /// 해당 모듈을 기술하는 테이블 레코드
        /// </summary>
        public IActableTableRecordBridge _ActableRecord { get; private set; }

        /// <summary>
        /// 해당 모듈에 등록된 유닛 액션 타입별 이벤처처리 함수(트리거 함수) 컬렉션
        /// </summary>
        private Dictionary<ActableTool.UnitActionType, Func<ControllerTool.ControlEventPreset, (bool, UnitActionTool.UnitAction.UnitTryActionResultType)>> _ActionEventHandlerCollection;
                
        /// <summary>
        /// 해당 모듈에 입력 커맨드 별로 등록된 유닛 액션 프리셋 컬렉션
        /// </summary>
        private Dictionary<ControllerTool.CommandType, ActableTool.UnitActionSpellPreset> _InputCommandMappedActionPresetTable;

        /// <summary>
        /// 해당 모듈이 보유중인 액션 커맨드 목록
        /// </summary>
        private List<ControllerTool.CommandType> _CommandList;
        
        /// <summary>
        /// 현재 시전가능한 커맨드 목록
        /// </summary>
        private List<ControllerTool.CommandType> _AvailableCommandList;
        
        /// <summary>
        /// 해당 모듈이 보유한 기본 커맨드
        /// </summary>
        public ControllerTool.CommandType _DefaultCommand { get; private set; }

        /// <summary>
        /// 해당 유닛을 기준으로 특정 스펠 액션 이벤트를 처리하는 핸들러
        /// </summary>
        public ObjectPooler<ObjectDeployEventRecord> ObjectDeployEventRecordPooler { get; private set; }
        
        #endregion
        
        #region <Callbacks>

        public IActable OnInitializeActable(UnitActionDataRoot.ActableType p_ActableType, Unit p_TargetUnit, IActableTableRecordBridge p_ActablePreset)
        {
            UnitModuleType = UnitModuleDataTool.UnitModuleType.Action;
            _ActableType = p_ActableType;
            _MasterNode = p_TargetUnit;
            _ActableRecord = p_ActablePreset;
            
            _ActionEventHandlerCollection = new Dictionary<ActableTool.UnitActionType, Func<ControllerTool.ControlEventPreset, (bool, UnitActionTool.UnitAction.UnitTryActionResultType)>>();
            _ActionEventHandlerCollection.Add(ActableTool.UnitActionType.Move, OnHandleMoveAction);
            _ActionEventHandlerCollection.Add(ActableTool.UnitActionType.Jump, OnHandleJumpAction);
            _ActionEventHandlerCollection.Add(ActableTool.UnitActionType.ActSpell, OnHandleSpellAction);
            
            _InputCommandMappedActionPresetTable = new Dictionary<ControllerTool.CommandType, ActableTool.UnitActionSpellPreset>();
            _CommandList = new List<ControllerTool.CommandType>();
            _AvailableCommandList = new List<ControllerTool.CommandType>();
            
            BindActionSpell(p_ActablePreset);
            ObjectDeployEventRecordPooler = new ObjectPooler<ObjectDeployEventRecord>();
            ObjectDeployEventRecordPooler.PreloadPool(8, 8);
            
#if UNITY_EDITOR
            SetMaxJumpCount(1);
#endif
            
            return this;
        }

        protected override void OnModuleNotify()
        {
            CurrentJumpCount = 0;
            // ResetCooldown();
            SwitchActionPhase(UnitActionTool.UnitActionPhase.None);
        }

        protected override void OnModuleSleep()
        {
            ObjectDeployEventRecordPooler.RetrieveAllObject();
            _MasterNode._MindObject.ClearReserveCommand();
        }
                
        public override void OnPreUpdate(float p_DeltaTime)
        {
        }
        
        public override void OnUpdate(float p_DeltaTime)
        {
            ProgressCooldown(p_DeltaTime);
        }

        public override void OnUpdate_TimeBlock()
        {
        }

        public override void OnPivotChanged(PositionStatePreset p_PositionStatePreset, bool p_SyncPosition)
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
        }
        
        public override void OnUnitActionTransitioned(ControllerTool.CommandType p_PrevCommandType, ControllerTool.CommandType p_CurrentCommandType)
        {
        }
        
        public override void OnUnitHitActionTerminated()
        {
        }

        public override void OnUnitActionStarted()
        {
        }

        public override void OnUnitActionTerminated()
        {
        }

        public override void OnUnitDead(bool p_Instant)
        {
            CancelUnitAction(AnimatorParamStorage.MotionType.Dead);
        }

        public override void OnJumpUp()
        {
        }
        
        public override void OnReachedGround(UnitStampPreset p_UnitStampPreset)
        {
            CurrentJumpCount = 0;
        }

        public override void OnCheckOverlapObject(UnitStampPreset p_UnitStampPreset)
        {
        }
        
        public override void OnFocusUnitChanged(Unit p_PrevFocusUnit, Unit p_FocusUnit)
        {
        }

        /// <summary>
        /// 인스턴스가 파기될 때 수행할 작업을 기술한다.
        /// </summary>
        protected override void DisposeUnManaged()
        {
        }

        /// <summary>
        /// 스킬 시전 도중에 이동 트리거가 발생한 경우 호출되는 콜백
        /// </summary>
        public void OnMoveTriggeredWhenDriveSpell(bool p_RestrictFlag)
        {
            if (p_RestrictFlag || _CurrentUnitAction.HasUnitActionFlag(UnitActionTool.UnitAction.UnitActionFlag.MoveCancelEnable))
            {
                CancelUnitAction(AnimatorParamStorage.MotionType.MoveRun);
            }
        }

        #endregion

        #region <Methods>

        private ActableTool.UnitActionSpellPreset GetCurrentActionPreset()
        {
            var currentCommandType = _CurrentActivatedInputTriggerPreset._InputPreset.CommandType;
            if (!_InputCommandMappedActionPresetTable.ContainsKey(currentCommandType)) return null;

            return currentCommandType == ControllerTool.CommandType.None ? null : _InputCommandMappedActionPresetTable[currentCommandType];
        }

        public ObjectDeployEventRecord GetObjectDeployEventRecord()
        {
            var spawned = ObjectDeployEventRecordPooler.GetObject();
            spawned.SetUnit(_MasterNode);
            return spawned;
        }

        #endregion
    }
}