namespace k514
{
    public class PrefabExtraData_SimpleProjector : PrefabExtraData_ProjectorBase<PrefabExtraData_SimpleProjector, PrefabExtraData_SimpleProjector.PrefabExtraDataSimpleProjectorRecord>
    {
        public class PrefabExtraDataSimpleProjectorRecord : PrefabExtraDataProjectorBaseRecord
        {
            public int ImageIndex { get; private set; }
        }

        protected override string GetDefaultTableFileName()
        {
            return "SimpleProjectorExtraDataTable";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 130000;
            EndIndex = 135000;
        }

        public override PrefabExtraDataRoot.PrefabExtraDataType GetThisLabelType()
        {
            return PrefabExtraDataRoot.PrefabExtraDataType.ProjectorSimple;
        }
    }
}