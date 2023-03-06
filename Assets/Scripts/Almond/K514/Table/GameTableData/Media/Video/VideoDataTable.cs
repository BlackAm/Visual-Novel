#if !SERVER_DRIVE
namespace k514
{
    public abstract class VideoDataTable<M, T> : MediaDataTableBase<M, VideoDataRoot.VideoType, T, IIndexableVideoTableRecordBridge>, IIndexableVideoTableBridge
        where M : VideoDataTable<M, T>, new()
        where T : VideoDataTable<M, T>.VideoTableRecordBase, new()
    {
        public abstract class VideoTableRecordBase : MediaTableRecordBase, IIndexableVideoTableRecordBridge
        {
        }

        public override MultiTableIndexer<int, VideoDataRoot.VideoType, IIndexableVideoTableRecordBridge> GetMultiGameIndex()
        {
            return VideoDataRoot.GetInstanceUnSafe.GameDataTableCluster;
        }
    }
}
#endif