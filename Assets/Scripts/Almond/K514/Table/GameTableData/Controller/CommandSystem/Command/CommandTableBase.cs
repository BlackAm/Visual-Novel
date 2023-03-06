namespace k514
{
    public abstract class CommandTableBase<M, K, T> : GameTable<M, K, T> where M : CommandTableBase<M, K, T>, new() where T : CommandTableBase<M, K, T>.CommandTableRecordBase, new()
    {
        public abstract class CommandTableRecordBase : GameTableRecordBase
        {
            public ControllerTool.CommandType CommandType { get; protected set; }
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        public static ControllerTool.CommandType GetCommandType(K p_Key) => GetInstanceUnSafe.GetTableData(p_Key).CommandType;
    }
}