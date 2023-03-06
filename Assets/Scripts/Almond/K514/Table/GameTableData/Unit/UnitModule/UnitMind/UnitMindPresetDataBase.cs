using Cysharp.Threading.Tasks;

namespace k514
{
    public abstract class UnitMindPresetDataBase<M, T> : MultiTableBase<M, int, T, UnitAIDataRoot.UnitMindType, IThinkableTableRecordBridge>, IThinkableTableBridge
        where M : UnitMindPresetDataBase<M, T>, new()
        where T : UnitMindPresetDataBase<M, T>.MindTableBaseRecord, new()
    {
        public abstract class MindTableBaseRecord : GameTableRecordBase, IThinkableTableRecordBridge
        {
            public int Priority { get; protected set; }
            public ThinkableTool.AIUnitFindType DefaultUnitFindType{ get; protected set; }

            public override async UniTask OnRecordAdded()
            {
                await base.OnRecordAdded();

                if (DefaultUnitFindType == ThinkableTool.AIUnitFindType.None)
                {
                    DefaultUnitFindType = ThinkableTool.AIUnitFindType.NearestPosition;
                }
            }
        }

        public override MultiTableIndexer<int, UnitAIDataRoot.UnitMindType, IThinkableTableRecordBridge> GetMultiGameIndex()
        {
            return UnitAIDataRoot.GetInstanceUnSafe.GameDataTableCluster;
        }
        
        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}