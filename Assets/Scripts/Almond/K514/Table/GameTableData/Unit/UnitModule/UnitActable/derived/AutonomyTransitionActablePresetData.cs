namespace k514
{
    public class AutonomyTransitionActablePresetData : UnitActablePresetDataBase<AutonomyTransitionActablePresetData, AutonomyTransitionActablePresetData.ActableTableRecord>
    {
        public class ActableTableRecord : ActableTableRecordBase
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "AutonomyTransitionActablePresetDataTable";
        }

        public override MultiTableIndexer<int, UnitActionDataRoot.ActableType, IActableTableRecordBridge> GetMultiGameIndex()
        {
            return UnitActionDataRoot.GetInstanceUnSafe.GameDataTableCluster;
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 100000;
            EndIndex = 200000;
        }

        public override UnitActionDataRoot.ActableType GetThisLabelType()
        {
            return UnitActionDataRoot.ActableType.AutonomyPhaseTransition;
        }
    }
}