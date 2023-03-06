namespace k514
{
    public abstract class UnitModuleBase : IIncarnateUnit
    {
        #region <Fields>

        protected bool _ModuleValidationFlag;
        public UnitModuleDataTool.UnitModuleType UnitModuleType { get; protected set; }
        public Unit _MasterNode { get; protected set; }

        #endregion
        
        #region <Callbacks>

        public virtual void OnMasterNodePooling()
        {
            TryModuleNotify();
        }

        public virtual void OnMasterNodeRetrieved()
        {
            TryModuleSleep();
        }

        protected abstract void OnModuleNotify();
        protected abstract void OnModuleSleep();
        public abstract void OnPreUpdate(float p_DeltaTime);
        public abstract void OnUpdate(float p_DeltaTime);
        public abstract void OnUpdate_TimeBlock();
        public abstract void OnPivotChanged(PositionStatePreset p_PositionStatePreset, bool p_SyncPosition);
        public abstract void OnStriked(Unit p_Trigger, HitResult p_HitResult);
        public abstract void OnHitted(Unit p_Trigger, HitResult p_HitResult);
        public abstract void OnUnitHitActionStarted();
        public abstract void OnUnitHitActionTerminated();
        public abstract void OnUnitActionStarted();
        public abstract void OnUnitActionTransitioned(ControllerTool.CommandType p_PrevCommandType,
            ControllerTool.CommandType p_CurrentCommandType);
        public abstract void OnUnitActionTerminated();
        public abstract void OnUnitDead(bool p_Instant);
        public abstract void OnJumpUp();
        public abstract void OnReachedGround(UnitStampPreset p_UnitStampPreset);
        public abstract void OnCheckOverlapObject(UnitStampPreset p_UnitStampPreset);
        public abstract void OnFocusUnitChanged(Unit p_PrevFocusUnit, Unit p_FocusUnit);

        #endregion

        #region <Methods>
        
        public void TryModuleNotify()
        {
            if (!_ModuleValidationFlag)
            {
                _ModuleValidationFlag = true;
                OnModuleNotify();
            }
        }

        public void TryModuleSleep()
        {
            if (_ModuleValidationFlag)
            {
                _ModuleValidationFlag = false;
                OnModuleSleep();
            }
        }

        #endregion

        #region <Disposable>
        
        /// <summary>
        /// dispose 패턴 onceFlag
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// dispose 플래그를 초기화 시키는 메서드
        /// </summary>
        public void Rejunvenate()
        {
            IsDisposed = false;
        }
        
        /// <summary>
        /// 인스턴스 파기 메서드
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }
            else
            {
                IsDisposed = true;
                DisposeUnManaged();
            }
        }

        /// <summary>
        /// 인스턴스가 파기될 때 수행할 작업을 기술한다.
        /// </summary>
        protected abstract void DisposeUnManaged();

        #endregion
    }
}