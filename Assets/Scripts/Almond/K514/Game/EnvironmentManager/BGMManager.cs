#if !SERVER_DRIVE
using UnityEngine;

namespace k514
{
    public class BGMManager : GameTimerEventTerminateReceiveSingleton<BGMManager>, MediaTool.IMediaTracker<AudioClip>
    {
        #region <Consts>

        private const uint TermNextBGMPlayLoopMsec = 1000U;
        
        #endregion
        
        #region <Fields>

        private IEventTimerHandler _BGM_Timer;
        private Transform _Transform;
        private AudioSource _BGMPlayer;
        public MediaTool.MediaPreset<AudioClip> MediaPreset { get; private set; }
        
        /// <summary>
        /// 씬 설정이 변경된 경우, 해당 이벤트를 수신받는 오브젝트
        /// </summary>
        private SceneVariableChangeEventReceiver SceneVariableChangeEventReceiver;
        
        public float Volume
        {
            get => _BGMPlayer.volume;
            set => _BGMPlayer.volume = Mathf.Clamp01(value);
        }
        
        #endregion

        #region <Callbacks>

        protected override void OnCreated()
        {
            base.OnCreated();
            
            var bgmObject = new GameObject("BGMManager");
            _Transform = bgmObject.transform;
            _Transform.SetParent(SystemBoot.GetInstance._Transform);
            _BGMPlayer = bgmObject.AddComponent<AudioSource>();
            _BGMPlayer.playOnAwake = false;
            _BGMPlayer.loop = false;
            _BGMPlayer.outputAudioMixerGroup = AudioManager.GetInstance.GetAudioChannel(SoundDataRoot.SoundFXType.BGM, AudioManager.AudioChannelType.Channel00);
            Volume = AudioManager.GetInstance.SfxUnitVolume;
            
            // 이벤트 수신 오브젝트 초기화
            SceneVariableChangeEventReceiver =
                SceneEnvironmentManager.GetInstance
                    .GetEventReceiver<SceneVariableChangeEventReceiver>(
                        SceneEnvironmentManager.SceneVariableEventType.OnSceneVariableChanged, OnMapVariableChanged);
        }

        public override void OnInitiate()
        {
            SetMediaPreset(default);
        }

        private void OnMapVariableChanged(SceneEnvironmentManager.SceneVariableEventType p_EventType, SceneVariableData.TableRecord p_Record)
        {
            SetBGM(p_Record.BackGroundSound, false, false);
        }

        private void On_BGM_End()
        {
            SetBGM();
        }

        #endregion
        
        #region <Methods>

        public void SetMediaPreset(MediaTool.MediaPreset<AudioClip> p_MediaPreset)
        {
            MediaPreset.Dispose();
            MediaPreset = p_MediaPreset;
        }

        public void SetBGM()
        {
            SetBGM(MediaPreset.Index, true, true);
        }

        public void SetBGM(int p_BgmIndex, bool p_AutoPlay, bool p_Reenter)
        {
            var mediaPreset = SoundDataRoot.GetInstanceUnSafe.GetMediaClip(p_BgmIndex, MediaPreset);
            if (mediaPreset)
            {
                _BGMPlayer.clip = mediaPreset;
                SetMediaPreset(mediaPreset);
    
                if (p_AutoPlay && _BGMPlayer.clip != null)
                {
                    PlayBGM(p_Reenter);
                }
            }
            else
            {
#if UNITY_EDITOR
                if (p_BgmIndex == default)
                {
                    Debug.LogWarning("[BGM] BGM 로딩에 실패했습니다.(지정인덱스가 기본값입니다.)");
                }
                else
                {
                    Debug.LogError("[BGM] BGM 로딩에 실패했습니다.(에셋 로드 실패)");
                }

#endif
                SetMediaPreset(default);
            }
        }

        public void PlayBGM(bool p_Reenter)
        {
            if (_BGMPlayer.clip != null)
            {
                if (p_Reenter || !_BGMPlayer.isPlaying)
                {
                    _BGMPlayer.Play();

                    var bgmLength = _BGMPlayer.clip.length;
                    var bgmLengthMsec = (uint) (bgmLength * 1000) + TermNextBGMPlayLoopMsec;
                    CancelEvent();
                    this.AddEvent(
                        bgmLengthMsec,
                        handler =>
                        {
                            GetInstance.On_BGM_End();
                            return true;
                        }
                    );
                    StartEvent();
                }
            }
        }

        public void ResumeBGM()
        {
            if (_BGMPlayer.clip != null && !_BGMPlayer.isPlaying)
            {
                _BGMPlayer.UnPause();
                StartEvent();
            }    
        }

        public void PauseBGM()
        {
            if (_BGMPlayer.clip != null && _BGMPlayer.isPlaying)
            {
                _BGMPlayer.Pause();
                PauseEvent();
            }
        }
        
        public void ReleaseBGM()
        {
            if (_BGMPlayer.clip != null)
            {
                _BGMPlayer.Stop();
                CancelEvent();
            }
        }

        #endregion
    }
}
#endif