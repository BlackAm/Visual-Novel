#if !SERVER_DRIVE
using UnityEngine;

namespace k514
{
    public abstract class UIManagerDeployTable<M, T> : MultiTableBase<M, int, T, RenderMode, IIndexableUIDeployTableRecordBridge>, IIndexableUIDeployTableBridge
        where M : UIManagerDeployTable<M, T>, new()
        where T : UIManagerDeployTable<M, T>.TableRecord, new()
    {
        public abstract class TableRecord : GameTableRecordBase, IIndexableUIDeployTableRecordBridge
        {
            public UICustomRoot.UIManagerType ManagerTypeMask { get; protected set; }
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        public override MultiTableIndexer<int, RenderMode, IIndexableUIDeployTableRecordBridge> GetMultiGameIndex()
        {
            return UIDataRoot.GetInstanceUnSafe.GameDataTableCluster;
        }
    }
}
#endif