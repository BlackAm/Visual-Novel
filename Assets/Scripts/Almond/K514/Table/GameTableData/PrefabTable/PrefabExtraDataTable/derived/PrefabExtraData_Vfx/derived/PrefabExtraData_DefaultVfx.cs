namespace k514
{
    public class PrefabExtraData_DefaultVfx : PrefabExtraData_VfxBase<PrefabExtraData_DefaultVfx, PrefabExtraData_DefaultVfx.PrefabExtraDataDefaultVfxRecord>
    {
        public class PrefabExtraDataDefaultVfxRecord : VfxTableRecordBase
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "DefaultVfxExtraDataTable";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 140000;
            EndIndex = 145000;
        }

        public override PrefabExtraDataRoot.PrefabExtraDataType GetThisLabelType()
        {
            return PrefabExtraDataRoot.PrefabExtraDataType.VFX;
        }
    }
}