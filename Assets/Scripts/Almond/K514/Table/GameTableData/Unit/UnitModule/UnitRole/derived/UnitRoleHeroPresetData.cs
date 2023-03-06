namespace k514
{
    public class UnitRoleHeroPresetData : UnitRolePresetDataBase<UnitRoleHeroPresetData, UnitRoleHeroPresetData.RoleTableRecord>
    {
        public class RoleTableRecord : RoleTableBaseRecord
        {
            public int AuraVfxSpawnIndex { get; private set; }
        }

        protected override string GetDefaultTableFileName()
        {
            return "UnitRoleHeroPresetTable";
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 300;
            EndIndex = 400;
        }

        public override UnitRoleDataRoot.UnitRoleType GetThisLabelType()
        {
            return UnitRoleDataRoot.UnitRoleType.Hero;
        }
    }
}