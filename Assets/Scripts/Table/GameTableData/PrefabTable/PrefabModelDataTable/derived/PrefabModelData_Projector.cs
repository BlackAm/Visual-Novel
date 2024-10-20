namespace BlackAm
{
    public class PrefabModelData_Projector : PrefabModelDataIntTable<PrefabModelData_Projector, PrefabModelData_Projector.TableRecord>, PrefabModelDataTableBridge
    {
        public class TableRecord : PrefabModelDataRecord
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "ProjectorPrefabDataTable";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 12000;
            EndIndex = 13000;
        }

        public override PrefabModelDataRoot.PrefabModelDataType GetThisLabelType()
        {
            return PrefabModelDataRoot.PrefabModelDataType.Projector;
        }
    }
}