#if !SERVER_DRIVE
using System.Collections.Generic;

namespace k514
{
    public abstract class MediaDataTableBase<M, K, T, Me> : MultiTableBase<M, int, T, K, Me>, IIndexableMediaTableBridge
        where M : MediaDataTableBase<M, K, T, Me>, new()
        where K : struct
        where T : MediaDataTableBase<M, K, T, Me>.MediaTableRecordBase, Me, new()
        where Me : IIndexableMediaTableRecordBridge
    {
        public abstract class MediaTableRecordBase : GameTableRecordBase, IIndexableMediaTableRecordBridge
        {
            public List<string> NameList { get; protected set; }
            public SystemTool.DataAccessType Type { get; protected set; }
            public AudioManager.AudioPreset AudioPreset { get; protected set; }
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}
#endif