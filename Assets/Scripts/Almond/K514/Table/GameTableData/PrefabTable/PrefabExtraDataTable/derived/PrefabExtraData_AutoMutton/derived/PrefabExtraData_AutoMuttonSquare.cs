namespace k514
{
    public class PrefabExtraData_AutoMuttonSquare : PrefabExtraData_AutoMuttonBase<PrefabExtraData_AutoMuttonSquare, PrefabExtraData_AutoMuttonSquare.TableRecord>
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
            return "SquareAutoMuttonExtraDataTable";
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 200750;
            EndIndex = 201000;
        }

        public override PrefabExtraDataRoot.PrefabExtraDataType GetThisLabelType()
        {
            return PrefabExtraDataRoot.PrefabExtraDataType.AutoMutton_Square;
        }
    }
}