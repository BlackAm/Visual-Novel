using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BlackAm
{
    public interface IIndexableSoundTableBridge : IIndexableMediaTableBridge
    {
    }

    public interface IIndexableSoundTableRecordBridge : IIndexableMediaTableRecordBridge
    {
    }

    public class SoundDataRoot : MediaDataRootBase<SoundDataRoot, SoundDataRoot.SoundFXType, IIndexableSoundTableBridge, IIndexableSoundTableRecordBridge, AudioClip>
    {
        #region <Enum>

        public enum SoundFXType
        {
            GameSFX,
            VoiceSFX,
            SystemSFX,
            BGM,
        }

        #endregion

        #region <Callbacks>

        protected override async UniTask OnCreated()
        {
            _ResourceType = ResourceType.AudioClip;
            
            await base.OnCreated();
        }

        #endregion
    }
}
