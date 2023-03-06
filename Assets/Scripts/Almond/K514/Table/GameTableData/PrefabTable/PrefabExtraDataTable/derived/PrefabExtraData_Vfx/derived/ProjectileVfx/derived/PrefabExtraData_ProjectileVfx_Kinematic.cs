namespace k514
{
    public class PrefabExtraData_ProjectileVfx_Kinematic : PrefabExtraData_ProjectileVfxBase<PrefabExtraData_ProjectileVfx_Kinematic, PrefabExtraData_ProjectileVfx_Kinematic.ProjectileVfxTableRecord>
    {
        public class ProjectileVfxTableRecord : ProjectileVfxTableRecordBase
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "ProjectileVfxExtraDataTable_Kinematic";
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 165000;
            EndIndex = 175000;
        }

        public override PrefabExtraDataRoot.PrefabExtraDataType GetThisLabelType()
        {
            return PrefabExtraDataRoot.PrefabExtraDataType.ProjectileVFX_Kinematic;
        }
    }
}