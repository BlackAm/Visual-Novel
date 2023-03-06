namespace k514
{
    public class UnitRoleBossPresetData : UnitRolePresetDataBase<UnitRoleBossPresetData, UnitRoleBossPresetData.RoleTableRecord>
    {
        public class RoleTableRecord : RoleTableBaseRecord
        {
            public int AuraVfxSpawnIndex { get; private set; }
        }

        protected override string GetDefaultTableFileName()
        {
            return "UnitRoleBossPresetTable";
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 400;
            EndIndex = 500;
        }

        public override UnitRoleDataRoot.UnitRoleType GetThisLabelType()
        {
            return UnitRoleDataRoot.UnitRoleType.Boss;
        }
    }
}