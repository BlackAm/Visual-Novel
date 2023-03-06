namespace k514
{
    public abstract class VolitionalBase : UnitModuleBase, IVolitional
    {
        ~VolitionalBase()
        {
            Dispose();
        }
        
        #region <Fields>
        
        public UnitRoleDataRoot.UnitRoleType _RoleType { get; private set; }
        public IVolitionalTableRecordBridge _RoleRecord { get; private set; }
        
        #endregion

        #region <Callbacks>
        
        public virtual IVolitional OnInitializeRole(UnitRoleDataRoot.UnitRoleType p_RoleType, Unit p_TargetUnit, IVolitionalTableRecordBridge p_RolePreset)
        {
            UnitModuleType = UnitModuleDataTool.UnitModuleType.Role;
            _RoleType = p_RoleType;
            _MasterNode = p_TargetUnit;
            _RoleRecord = p_RolePreset;
            
            return this;
        }

        public abstract string GetRoleName();
        
        public override void OnPivotChanged(PositionStatePreset p_PositionStatePreset, bool p_SyncPosition)
        {
        }
        
        #endregion
    }
}