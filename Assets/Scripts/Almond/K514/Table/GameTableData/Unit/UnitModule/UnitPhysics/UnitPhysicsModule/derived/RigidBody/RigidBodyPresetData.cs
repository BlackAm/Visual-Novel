namespace k514
{
    public class RigidBodyPresetData : UnitPhysicsPresetDataBase<RigidBodyPresetData, RigidBodyPresetData.PhysicsTableRecord>
    {
        public class PhysicsTableRecord : PhysicsTableBaseRecord
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "RigidBodyPresetDataTable";
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 0;
            EndIndex = 50;
        }

        public override UnitPhysicsDataRoot.UnitPhysicsType GetThisLabelType()
        {
            return UnitPhysicsDataRoot.UnitPhysicsType.RigidBody;
        }
    }
}