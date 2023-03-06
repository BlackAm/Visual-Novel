namespace k514
{
    public class PassiveTransitionActablePresetData : UnitActablePresetDataBase<PassiveTransitionActablePresetData, PassiveTransitionActablePresetData.ActableTableRecord>
    {
        public class ActableTableRecord : ActableTableRecordBase
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "PassiveTransitionActablePresetDataTable";
        }

        public override MultiTableIndexer<int, UnitActionDataRoot.ActableType, IActableTableRecordBridge> GetMultiGameIndex()
        {
            return UnitActionDataRoot.GetInstanceUnSafe.GameDataTableCluster;
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 0;
            EndIndex = 100000;
        }

        public override UnitActionDataRoot.ActableType GetThisLabelType()
        {
            return UnitActionDataRoot.ActableType.PassivePhaseTransition;
        }
    }
}