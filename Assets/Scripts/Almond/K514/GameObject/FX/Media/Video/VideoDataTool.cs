using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;

namespace k514
{
    public interface IIndexableVideoTableBridge : IIndexableMediaTableBridge
    {
    }

    public interface IIndexableVideoTableRecordBridge : IIndexableMediaTableRecordBridge
    {
    }

    public class VideoDataRoot : MediaDataRootBase<VideoDataRoot, VideoDataRoot.VideoType, IIndexableVideoTableBridge, IIndexableVideoTableRecordBridge, VideoClip>
    {
        #region <Enum>

        public enum VideoType
        {
            Default,
            LoopVideo,
        }

        #endregion

        #region <Callbacks>

        protected override async UniTask OnCreated()
        {
            _ResourceType = ResourceType.VideoClip;
            
            await base.OnCreated();
        }

        #endregion
    }
}
