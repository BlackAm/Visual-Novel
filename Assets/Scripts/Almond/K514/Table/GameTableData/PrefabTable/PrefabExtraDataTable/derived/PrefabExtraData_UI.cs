#if !SERVER_DRIVE
namespace k514
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
            StartIndex = 130000;
            EndIndex = 140000;
        }

        public override PrefabExtraDataRoot.PrefabExtraDataType GetThisLabelType()
        {
            return PrefabExtraDataRoot.PrefabExtraDataType.UI;
        }
    }
}
#endif