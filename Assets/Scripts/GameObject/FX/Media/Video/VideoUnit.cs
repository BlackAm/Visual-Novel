using UnityEngine;
using UnityEngine.Video;

namespace BlackAm
{
    public class VideoUnit : MediaUnit<VideoPlayer, VideoClip>
    {
        #region <Fields>
#if !SERVER_DRIVE
#endif
        #endregion
        
        #region <Callbacks>
#if !SERVER_DRIVE
        public override void OnSpawning()
        {
            base.OnSpawning();

            DeployableType = ObjectDeployTool.DeployableType.Video;
            _Transform.SetParent(SfxSpawnManager.GetInstance._SFXObjectWrapper);
            
            _MediaPlayer.playOnAwake = false;
        }
#endif
        #endregion

        #region <Methods>
#if !SERVER_DRIVE
        protected override void OnMediaOver()
        {
            throw new System.NotImplementedException();
        }

        public override void SetMediaPreset(MediaTool.MediaPreset<VideoClip> p_MediaPreset)
        {
            base.SetMediaPreset(p_MediaPreset);
            _MediaPlayer.clip = MediaPreset;
        }

        public override void SetVolume(float p_Value01)
        {
        }

        public override void SetChannel(AudioManager.AudioChannelType p_ChannelType)
        {
            throw new System.NotImplementedException();
        }

        public override MediaTool.MediaPreset<VideoClip> SetClip(int p_MediaIndex, MediaTool.MediaPreset<VideoClip> p_MediaTracker)
        {
            throw new System.NotImplementedException();
        }

        public override void SetLoop(bool p_Flag)
        {
            _MediaPlayer.isLooping = p_Flag;
        }

        public override bool SetPlay(uint p_Delay)
        {
            var tryClip = _MediaPlayer.clip;
            var duration = tryClip == null ? 0 : tryClip.length;
            var msec = (uint) (duration * 1000);

            if (msec > 0)
            {
                GameEventTimerHandlerManager.GetInstance.SpawnSafeEventTimerHandler(ref _EventHandler, this, SystemBoot.TimerType.GameTimer, false);
                var (_, eventHandler) = _EventHandler.GetValue();
                eventHandler
                    .AddEvent(
                        msec + p_Delay, 
                        handler =>
                        {
                            handler.Arg1.RetrieveObject();
                            return true;
                        }, 
                        null, this
                    );
                eventHandler.StartEvent();
                return true;
            }
            else
            {
                RetrieveObject();
                return false;
            }
        }

        public override void SetResume()
        {
            throw new System.NotImplementedException();
        }

        public override void SetPause()
        {
            throw new System.NotImplementedException();
        }

        public override void SetStop()
        {
            _MediaPlayer.Stop();
        }
#endif
        #endregion
    }
}