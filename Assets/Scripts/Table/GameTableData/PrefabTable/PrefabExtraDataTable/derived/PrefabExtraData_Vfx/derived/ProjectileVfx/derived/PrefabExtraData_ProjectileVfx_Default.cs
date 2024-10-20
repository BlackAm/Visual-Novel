namespace BlackAm
{
    public class PrefabExtraData_ProjectileVfx_Default : PrefabExtraData_ProjectileVfxBase<PrefabExtraData_ProjectileVfx_Default, PrefabExtraData_ProjectileVfx_Default.ProjectileVfxTableRecord>
    {
        public class ProjectileVfxTableRecord : ProjectileVfxTableRecordBase
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "ProjectileVfxExtraDataTable_Default";
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 145000;
            EndIndex = 155000;
        }

        public override PrefabExtraDataRoot.PrefabExtraDataType GetThisLabelType()
        {
            return PrefabExtraDataRoot.PrefabExtraDataType.ProjectileVFX;
        }
    }
}