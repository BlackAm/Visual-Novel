namespace k514
{
    public class PrefabModelData_AutoMutton : PrefabModelDataIntTable<PrefabModelData_AutoMutton, PrefabModelData_AutoMutton.TableRecord>, PrefabModelDataTableBridge
    {
        public class TableRecord : PrefabModelDataRecord
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "AutoMuttonPrefabDataTable";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 100000;
            EndIndex = 110000;
        }

        public override PrefabModelDataRoot.PrefabModelDataType GetThisLabelType()
        {
            return PrefabModelDataRoot.PrefabModelDataType.AutoMutton;
        }
    }
}