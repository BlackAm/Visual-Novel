#if !SERVER_DRIVE
namespace BlackAm
{
    public class PrefabExtraData_UI : PrefabExtraDataIntTable<PrefabExtraData_UI, PrefabExtraData_UI.TableRecord>, PrefabExtraDataTableBridge
    {
        public class TableRecord : PrefabExtraDataRecord
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "UIExtraDataTable";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 120000;
            EndIndex = 120250;
        }

        public override PrefabExtraDataRoot.PrefabExtraDataType GetThisLabelType()
        {
            return PrefabExtraDataRoot.PrefabExtraDataType.UI;
        }
    }
}
#endif