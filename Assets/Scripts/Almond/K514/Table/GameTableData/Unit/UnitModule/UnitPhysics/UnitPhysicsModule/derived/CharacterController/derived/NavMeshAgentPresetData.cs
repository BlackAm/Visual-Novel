namespace k514
{
    public class NavMeshAgentPresetData : CharacterControllerPresetDataBase<NavMeshAgentPresetData, NavMeshAgentPresetData.PhysicsTableRecord>
    {
        public class PhysicsTableRecord : CharacterControllerTableBaseRecord
        {
            public int NavMeshAgentPriority { get; private set; }
        }

        protected override string GetDefaultTableFileName()
        {
            return "NavMeshAgentPresetDataTable";
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 100;
            EndIndex = 1000;
        }

        public override UnitPhysicsDataRoot.UnitPhysicsType GetThisLabelType()
        {
            return UnitPhysicsDataRoot.UnitPhysicsType.NavMesh;
        }
    }
}