#if !SERVER_DRIVE
using Cysharp.Threading.Tasks;

namespace k514
{
    public abstract class SoundDataTable<M, T> : MediaDataTableBase<M, SoundDataRoot.SoundFXType, T, IIndexableSoundTableRecordBridge>, IIndexableSoundTableBridge
        where M : SoundDataTable<M, T>, new()
        where T : SoundDataTable<M, T>.SoundTableRecordBase, new()
    {
        public abstract class SoundTableRecordBase : MediaTableRecordBase, IIndexableSoundTableRecordBridge
        {
            public override async UniTask OnRecordAdded()
            {
                await base.OnRecordAdded();
                
                AudioPreset = new AudioManager.AudioPreset(GetInstanceUnSafe.GetThisLabelType(), AudioManager.AudioChannelType.Channel00);
            }
        }

        public override MultiTableIndexer<int, SoundDataRoot.SoundFXType, IIndexableSoundTableRecordBridge> GetMultiGameIndex()
        {
            return SoundDataRoot.GetInstanceUnSafe.GameDataTableCluster;
        }
    }
}
#endif