using UnityEngine;

namespace BlackAm
{
    public abstract class MediaUnit<M, K> : FXUnit, MediaTool.IMediaTracker<K> where M : Object where K : Object
    {
        #region <Fields>
#if !SERVER_DRIVE
        protected M _MediaPlayer;
        public MediaTool.MediaPreset<K> MediaPreset { get; private set; }
        protected SafeReference<object, GameEventTimerHandlerWrapper> _EventHandler;
#endif
        #endregion
        
        #region <Callbacks>
#if !SERVER_DRIVE
        public override void OnSpawning()
        {
            base.OnSpawning();

            _MediaPlayer = GetComponent<M>();
        }
        
        public override void OnPooling(){
            base.OnPooling();
            SetVolume();
        }
        
        public override void OnRetrieved()
        {
            base.OnRetrieved();
            
            SetMediaPreset(default);
            SetStop();
            SetClip(0, default);
            SetLoop(false);
            SetVolume();
        }

        protected abstract void OnMediaOver();
#endif
        #endregion

        #region <Methods>
#if !SERVER_DRIVE
        public virtual void SetMediaPreset(MediaTool.MediaPreset<K> p_MediaPreset)
        {
            MediaPreset.Dispose();
            MediaPreset = p_MediaPreset;
        }
        
        public abstract void SetVolume(float p_Value01);
        public abstract void SetChannel(AudioManager.AudioChannelType p_ChannelType);
        
        public void SetVolume()
        {
            SetVolume(AudioManager.GetInstance?.SfxUnitVolume ?? 0f);
        }

        public abstract MediaTool.MediaPreset<K> SetClip(int p_MediaIndex, MediaTool.MediaPreset<K> p_MediaTracker);
        public abstract void SetLoop(bool p_Flag);
        public abstract bool SetPlay(uint p_Delay);
        public abstract void SetResume();
        public abstract void SetPause();
        public abstract void SetStop();
#endif
        #endregion
    }
}