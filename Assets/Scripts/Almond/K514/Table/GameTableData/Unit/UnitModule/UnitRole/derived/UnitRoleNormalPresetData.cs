namespace k514
{
    public class UnitRoleNormalPresetData : UnitRolePresetDataBase<UnitRoleNormalPresetData, UnitRoleNormalPresetData.RoleTableRecord>
    {
        public class RoleTableRecord : RoleTableBaseRecord
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "UnitRoleNormalPresetTable";
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 0;
            EndIndex = 100;
        }

        public override UnitRoleDataRoot.UnitRoleType GetThisLabelType()
        {
            return UnitRoleDataRoot.UnitRoleType.None;
        }
    }
}