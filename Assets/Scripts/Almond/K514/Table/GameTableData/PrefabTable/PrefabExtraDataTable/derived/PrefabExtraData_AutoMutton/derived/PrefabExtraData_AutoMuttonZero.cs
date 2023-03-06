namespace k514
{
    public class PrefabExtraData_AutoMuttonZero : PrefabExtraData_AutoMuttonBase<PrefabExtraData_AutoMuttonZero, PrefabExtraData_AutoMuttonZero.TableRecord>
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
            return "ZeroAutoMuttonExtraDataTable";
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 200000;
            EndIndex = 200250;
        }

        public override PrefabExtraDataRoot.PrefabExtraDataType GetThisLabelType()
        {
            return PrefabExtraDataRoot.PrefabExtraDataType.AutoMutton_Zero;
        }
    }
}