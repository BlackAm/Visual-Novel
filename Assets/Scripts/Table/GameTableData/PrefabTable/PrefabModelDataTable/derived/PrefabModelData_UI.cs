#if !SERVER_DRIVE
namespace BlackAm
{
    public class PrefabModelData_UI : PrefabModelDataIntTable<PrefabModelData_UI, PrefabModelData_UI.TableRecord>, PrefabModelDataTableBridge
    {
        public class TableRecord : PrefabModelDataRecord
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "UIPrefabDataTable";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 20000;
            EndIndex = 30000;
        }

        public override PrefabModelDataRoot.PrefabModelDataType GetThisLabelType()
        {
            return PrefabModelDataRoot.PrefabModelDataType.UI;
        }
    }
}
#endif