using System.Collections.Generic;

namespace BlackAm
{
    public abstract class DialogueKeyInputDataBase<M, T> : MultiTableBase<M, int, T, DialogueGameManager.KeyInputType, IKeyInputTableRecordBridge>, IKeyInputTableBridge
        where M : DialogueKeyInputDataBase<M, T>, new()
        where T : DialogueKeyInputDataBase<M, T>.KeyInputTableBaseRecord, new()
    {
        public abstract class KeyInputTableBaseRecord : GameTableRecordBase, IKeyInputTableRecordBridge
        {
            public Dictionary<ControllerTool.CommandType, DialogueGameManager.InputAction> InputAction { get; protected set; }
        }

        public override MultiTableIndexer<int, DialogueGameManager.KeyInputType, IKeyInputTableRecordBridge> GetMultiGameIndex()
        {
            return DialogueKeyInputDataRoot.GetInstanceUnSafe.GameDataTableCluster;
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}