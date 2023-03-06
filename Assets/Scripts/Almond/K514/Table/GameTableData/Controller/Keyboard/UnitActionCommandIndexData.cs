namespace k514
{
    public class UnitActionCommandIndexData : GameTable<UnitActionCommandIndexData, ControllerTool.CommandType, UnitActionCommandIndexData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public ActableTool.UnitActionType ActionType { get; private set; }
        }

        protected override string GetDefaultTableFileName()
        {
            return "UnitActionCommandIndex";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}