namespace BlackAm
{
    public class PrefabExtraData_ProjectileVfx_TimeInterpolate : PrefabExtraData_ProjectileVfxBase<PrefabExtraData_ProjectileVfx_TimeInterpolate, PrefabExtraData_ProjectileVfx_TimeInterpolate.ProjectileVfxTableRecord>
    {
        public class ProjectileVfxTableRecord : ProjectileVfxTableRecordBase
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "ProjectileVfxExtraDataTable_TimeInterpolate";
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 175000;
            EndIndex = 185000;
        }

        public override PrefabExtraDataRoot.PrefabExtraDataType GetThisLabelType()
        {
            return PrefabExtraDataRoot.PrefabExtraDataType.ProjectileVFX_TimeInterpolate;
        }
    }
}