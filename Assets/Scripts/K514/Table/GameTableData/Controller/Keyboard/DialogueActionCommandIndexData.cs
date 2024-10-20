namespace BlackAm
{
    public class DialogueActionCommandIndexData : GameTable<DialogueActionCommandIndexData, ControllerTool.CommandType, DialogueActionCommandIndexData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public DialogueGameManager.InputAction ActionType { get; private set; }
        }

        protected override string GetDefaultTableFileName()
        {
            return "DialogueActionCommandIndex";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}