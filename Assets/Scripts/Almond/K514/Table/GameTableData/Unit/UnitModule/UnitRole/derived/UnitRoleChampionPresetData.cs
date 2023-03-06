namespace k514
{
    public class UnitRoleChampionPresetData : UnitRolePresetDataBase<UnitRoleChampionPresetData, UnitRoleChampionPresetData.RoleTableRecord>
    {
        public class RoleTableRecord : RoleTableBaseRecord
        {
            public int AuraVfxSpawnIndex { get; private set; }
        }

        protected override string GetDefaultTableFileName()
        {
            return "UnitRoleChampionPresetTable";
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 200;
            EndIndex = 300;
        }

        public override UnitRoleDataRoot.UnitRoleType GetThisLabelType()
        {
            return UnitRoleDataRoot.UnitRoleType.Champion;
        }
    }
}