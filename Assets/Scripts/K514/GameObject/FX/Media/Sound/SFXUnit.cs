using UnityEngine;
using Random = UnityEngine.Random;

namespace BlackAm
{
    public class SFXUnit : MediaUnit<AudioSource, AudioClip>
    {
        #region <Consts>

        private const float DefaultSpatialBlend = 1f;
        private const float DefaultAudioAttenuateLowerBound = 6f;

        #endregion
        
        #region <Fields>
#if !SERVER_DRIVE
#endif
        #endregion
        
        #region <Callbacks>
#if !SERVER_DRIVE
        public override void OnSpawning()
        {
            base.OnSpawning();

            DeployableType = ObjectDeployTool.DeployableType.SFX;
            _Transform.SetParent(SfxSpawnManager.GetInstance._SFXObjectWrapper);
            
            _MediaPlayer.playOnAwake = false;
            _MediaPlayer.spatialBlend = DefaultSpatialBlend;
            _MediaPlayer.rolloffMode = AudioRolloffMode.Linear;

            SetAudioReachRange(DefaultAudioAttenuateLowerBound, CameraManager.GetInstanceUnSafe.UnitFarCullingDistance);
        }
#endif
        #endregion

        #region <Methods>
#if !SERVER_DRIVE

        protected override void OnMediaOver()
        {
            RetrieveObject();
        }

        public override void SetMediaPreset(MediaTool.MediaPreset<AudioClip> p_MediaPreset)
        {
            AudioManager.GetInstance?.OnSfxPlayOver(this);
            
            base.SetMediaPreset(p_MediaPreset);

            if (MediaPreset)
            {
                _MediaPlayer.clip = MediaPreset;
                _MediaPlayer.outputAudioMixerGroup = AudioManager.GetInstance?.GetAudioChannel(MediaPreset);    
            }
            else
            {
                _MediaPlayer.clip = null;
                _MediaPlayer.outputAudioMixerGroup = null;         
            }
        }

        /// <summary>
        /// LowerBound보다 가까운 거리에서는 소리가 보정되지 않는다.
        /// UpperBound보다 먼 거리에서는 소리가 들리지 않는다.
        /// [LowerBound, UpperBound] 구간에서는 거리에 비례하여 소리가 작아진다.
        /// </summary>
        private void SetAudioReachRange(float p_LowerBound, float p_UpperBound)
        {
            _MediaPlayer.minDistance = p_LowerBound;
            _MediaPlayer.maxDistance = p_UpperBound;
        }
        
        public void SetPitchRandom()
        {
            var audioType = MediaPreset.AudioPreset.AudioType;
            switch (audioType)
            {
                case SoundDataRoot.SoundFXType.GameSFX:
                    _MediaPlayer.pitch = Random.Range(0.95f, 1.05f);
                    break;
                default:
                    break;
            }
        }
        
        public override void SetVolume(float p_Value01)
        {
            _MediaPlayer.volume = p_Value01;
        }

        public override void SetChannel(AudioManager.AudioChannelType p_ChannelType)
        {
            if (MediaPreset)
            {
                _MediaPlayer.outputAudioMixerGroup = AudioManager.GetInstance.GetAudioChannel(MediaPreset.AudioPreset.AudioType, p_ChannelType);
            }
            else
            {
                _MediaPlayer.outputAudioMixerGroup = null;
            }
        }

        public override MediaTool.MediaPreset<AudioClip> SetClip(int p_MediaIndex, MediaTool.MediaPreset<AudioClip> p_PrevMediaTracker)
        {
            var mediaPreset = SoundDataRoot.GetInstanceUnSafe?.GetMediaClip(p_MediaIndex, p_PrevMediaTracker) ?? default;
            if (mediaPreset)
            {
                SetMediaPreset(mediaPreset);
                return mediaPreset;
            }
            else
            {
                return p_PrevMediaTracker;
            }
        }

        public override void SetLoop(bool p_Flag)
        {
            _MediaPlayer.loop = p_Flag;
        }

        public override bool SetPlay(uint p_Delay)
        {
            if (AudioManager.GetInstance.IsValidPlaySfx(MediaPreset.Index))
            {
                SetPitchRandom();
                _MediaPlayer.PlayDelayed(p_Delay * 0.001f);
            
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
                                handler.Arg1.OnMediaOver();
                                return true;
                            }, 
                            null, this
                        );
                    eventHandler.StartEvent();
                    AudioManager.GetInstance.OnSfxPlayBegin(this);
                    return true;
                }
                else
                {
                    RetrieveObject();
                    return false;
                }
            }
            else
            {
                RetrieveObject();
                return false;
            }
        }

        public override void SetResume()
        {
            if (!_MediaPlayer.isPlaying)
            {
                var (valid, eventHandler) = _EventHandler.GetValue();
                if (valid)
                {
                    eventHandler.StartEvent();
                    _MediaPlayer.Play();
                }
            }
        }

        public override void SetPause()
        {
            if (_MediaPlayer.isPlaying)
            {
                var (valid, eventHandler) = _EventHandler.GetValue();
                if (valid)
                {
                    eventHandler.PauseEvent();
                    _MediaPlayer.Stop();
                }
            }
        }

        public override void SetStop()
        {
            EventTimerTool.ReleaseEventHandler(ref _EventHandler);
            if (_MediaPlayer.isPlaying)
            {
                _MediaPlayer.Stop();
            }
        }
#endif
        #endregion
    }
}