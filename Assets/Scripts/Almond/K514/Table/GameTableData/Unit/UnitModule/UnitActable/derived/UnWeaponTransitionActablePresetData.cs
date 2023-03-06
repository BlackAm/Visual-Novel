namespace k514
{
    public class UnWeaponTransitionActablePresetData : UnitActablePresetDataBase<UnWeaponTransitionActablePresetData, UnWeaponTransitionActablePresetData.ActableTableRecord>
    {
        public class ActableTableRecord : ActableTableRecordBase
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "UnWeaponTransitionActablePresetDataTable";
        }

        public override MultiTableIndexer<int, UnitActionDataRoot.ActableType, IActableTableRecordBridge> GetMultiGameIndex()
        {
            return UnitActionDataRoot.GetInstanceUnSafe.GameDataTableCluster;
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 200000;
            EndIndex = 300000;
        }

        public override UnitActionDataRoot.ActableType GetThisLabelType()
        {
            return UnitActionDataRoot.ActableType.UnWeaponPhaseTransition;
        }
    }
}