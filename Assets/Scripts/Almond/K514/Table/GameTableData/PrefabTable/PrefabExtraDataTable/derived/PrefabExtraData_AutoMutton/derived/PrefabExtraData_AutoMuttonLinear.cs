namespace k514
{
    public class PrefabExtraData_AutoMuttonLinear : PrefabExtraData_AutoMuttonBase<PrefabExtraData_AutoMuttonLinear, PrefabExtraData_AutoMuttonLinear.TableRecord>
    {
        public class TableRecord : AutoMuttonBaseTableRecord
        {
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        protected override string GetDefaultTableFileName()
        {
            return "LinearAutoMuttonExtraDataTable";
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 200500;
            EndIndex = 200750;
        }

        public override PrefabExtraDataRoot.PrefabExtraDataType GetThisLabelType()
        {
            return PrefabExtraDataRoot.PrefabExtraDataType.AutoMutton_Linear;
        }
    }
}