namespace k514
{
    public interface IPhysicsTableBridge : ITableBase
    {
    }    
    
    public interface IPhysicsTableRecordBridge : ITableBaseRecord
    {
        float Mass { get; }
    }
    
    public interface ICharacterControllerTableBridge : IPhysicsTableBridge
    {
    }    
    
    public interface ICharacterControllerTableRecordBridge : IPhysicsTableRecordBridge
    {
    }

    public class UnitPhysicsDataRoot : UnitModuleDataRootBase<UnitPhysicsDataRoot, UnitPhysicsDataRoot.UnitPhysicsType, IPhysicsTableBridge, IPhysicsTableRecordBridge>
    {
        #region <Enums>

        /// <summary>
        /// 물리 모듈 연산자 타입
        /// </summary>
        public enum UnitPhysicsType
        {
            RigidBody,
            CharacterController,
            NavMesh,
        }

        #endregion

        #region <Methods>

        protected override UnitModuleDataTool.UnitModuleType GetUnitModuleType()
        {
            return UnitModuleDataTool.UnitModuleType.Physics;
        }

        public override (UnitPhysicsType, IIncarnateUnit) SpawnModule(Unit p_Target, int p_TableIndex)
        {
            var (valid, labelType) = GetLabelType(p_TableIndex);
            if (valid)
            {
                var record = this[p_TableIndex];
                switch (labelType)
                {
                    case UnitPhysicsType.RigidBody:
                        return (labelType, new ControlRigidBody().OnInitializePhysics(labelType, p_Target, record));
                    case UnitPhysicsType.CharacterController:
                        return (labelType, new ControlCharacterController().OnInitializePhysics(labelType, p_Target, record));
                    case UnitPhysicsType.NavMesh:
                        return (labelType, new ControlNavMesh().OnInitializePhysics(labelType, p_Target, record));
                }
            }

            return default;
        }
        
        #endregion
    }
}