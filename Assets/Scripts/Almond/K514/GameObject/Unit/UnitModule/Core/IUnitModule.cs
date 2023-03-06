namespace k514
{
    /// <summary>
    /// 유닛에 포함되어 어떤 법칙(물리, 사고)에 따른 행동원리를 기술하는 인터페이스
    /// </summary>
    public interface IIncarnateUnit : _IDisposable
    {
        UnitModuleDataTool.UnitModuleType UnitModuleType { get; }
        Unit _MasterNode { get; }
        void OnMasterNodePooling();
        void OnMasterNodeRetrieved();
        void OnPreUpdate(float p_DeltaTime);
        void OnUpdate(float p_DeltaTime);
        void OnUpdate_TimeBlock();
        void OnPivotChanged(PositionStatePreset p_PositionStatePreset, bool p_SyncPosition);
        void OnStriked(Unit p_Trigger, HitResult p_HitResult);
        void OnHitted(Unit p_Trigger, HitResult p_HitResult);
        void OnUnitHitActionStarted();
        void OnUnitHitActionTerminated();
        void OnUnitActionStarted();  
        void OnUnitActionTransitioned(ControllerTool.CommandType p_PrevCommandType, ControllerTool.CommandType p_CurrentCommandType);  
        void OnUnitActionTerminated();
        void OnUnitDead(bool p_Instant);
        void OnJumpUp();
        void OnReachedGround(UnitStampPreset p_UnitStampPreset);
        void OnCheckOverlapObject(UnitStampPreset p_UnitStampPreset);
        void OnFocusUnitChanged(Unit p_PrevFocusUnit, Unit p_FocusUnit);
        void TryModuleNotify();
        void TryModuleSleep();
    }
}