namespace k514
{
    public class PrefabExtraData_AutoMuttonNonZero : PrefabExtraData_AutoMuttonBase<PrefabExtraData_AutoMuttonNonZero, PrefabExtraData_AutoMuttonNonZero.TableRecord>
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
            return "NonZeroAutoMuttonExtraDataTable";
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 200250;
            EndIndex = 200500;
        }

        public override PrefabExtraDataRoot.PrefabExtraDataType GetThisLabelType()
        {
            return PrefabExtraDataRoot.PrefabExtraDataType.AutoMutton_NonZero;
        }
    }
}