namespace k514
{
    public class PrefabExtraData_ProjectileVfx_VectorMap : PrefabExtraData_ProjectileVfxBase<PrefabExtraData_ProjectileVfx_VectorMap, PrefabExtraData_ProjectileVfx_VectorMap.ProjectileVfxTableRecord>
    {
        public class ProjectileVfxTableRecord : ProjectileVfxTableRecordBase
        {
            /// <summary>
            /// 파티클 회전에서 참조할 배치 이벤트 인덱스
            /// </summary>
            public int VectorMapIndex { get; private set; }
        }

        protected override string GetDefaultTableFileName()
        {
            return "ProjectileVfxExtraDataTable_VectorMap";
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 155000;
            EndIndex = 165000;
        }

        public override PrefabExtraDataRoot.PrefabExtraDataType GetThisLabelType()
        {
            return PrefabExtraDataRoot.PrefabExtraDataType.ProjectileVFX_VectorMap;
        }
    }
}