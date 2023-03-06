using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace k514
{
    public class MapEfffectUnit : FXUnit
    {
        #region <Consts>

        private const float DefaultSpatialBlend = 1f;
        private const float DefaultAudioAttenuateLowerBound = 6f;

        #endregion
        
        #region <Fields>
#if !SERVER_DRIVE
        private AudioSource _AudioSource;
        private SafeReference<object, GameEventTimerHandlerWrapper> _EventHandler;
        public int _SfxIndex;
        public MapEffectSoundManager.MapEffectSound _MapEffectSound;
#endif
        #endregion
        
        #region <Callbacks>
#if !SERVER_DRIVE
        public override void OnSpawning()
        {
            base.OnSpawning();

            DeployableType = ObjectDeployTool.DeployableType.SFX;
            _Transform.SetParent(MapEffectSoundManager.GetInstance._MapEffectObjectWrapper);
            
            _AudioSource = GetComponent<AudioSource>();
            _AudioSource.playOnAwake = false;
            _AudioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        }

        public override void OnPooling()
        {
            base.OnPooling();

            SetVolume();
        }

        public override void OnRetrieved()
        {
            base.OnRetrieved();
            MapEffectSoundManager.GetInstance?.OnMapEffectPlayOver(_MapEffectSound, this);
            EventTimerTool.ReleaseEventHandler(ref _EventHandler);
            SetStop();
            ClearClip();
            SetLoop(false);
            SetVolume();
        }
#endif
        #endregion

        #region <Methods>
#if !SERVER_DRIVE
        public void SetVolume(float p_Value01)
        {
            _AudioSource.volume = p_Value01;
        }
        
        public void SetVolume()
        {
            SetVolume(MapEffectSoundManager.GetInstance?.Volume ?? 0f);
        }

        public void ClearClip()
        {
            _MapEffectSound = default;
            _SfxIndex = default;
            _AudioSource.clip = default;
        }
        
        public void SetClip(MapEffectSoundManager.MapEffectSound p_MapEffectSound, AudioClip p_AudioClip)
        {
            _MapEffectSound = p_MapEffectSound;
            _AudioSource.clip = p_AudioClip;
        }

        public void SetPitchRandom()
        {
            _AudioSource.pitch = Random.Range(0.95f, 1.05f);
        }

        public void SetLoop(bool p_Flag)
        {
            _AudioSource.loop = p_Flag;
        }

        /// <summary>
        /// LowerBound보다 가까운 거리에서는 소리가 보정되지 않는다.
        /// UpperBound보다 먼 거리에서는 소리가 들리지 않는다.
        /// [LowerBound, UpperBound] 구간에서는 거리에 비례하여 소리가 작아진다.
        /// </summary>
        private void SetAudioReachRange(float p_LowerBound, float p_UpperBound)
        {
            _AudioSource.minDistance = p_LowerBound;
            _AudioSource.maxDistance = p_UpperBound;
        }

        public bool SetPlay(uint p_Delay)
        {
            SetPitchRandom();
            _AudioSource.PlayDelayed(p_Delay * 0.001f);
        
            var tryClip = _AudioSource.clip;
            var duration = tryClip == null ? 0 : tryClip.length;
            var msec = (uint) (duration * 1000);

            if (msec > 0)
            {
                SetLoop(true);
                /*GameEventTimerHandlerManager.GetInstanceUnSafe.SpawnSafeEventTimerHandler(ref _EventHandler, this, SystemBoot.TimerType.GameTimer, false);
                var (_, eventHandler) = _EventHandler.GetValue();
                eventHandler
                    .AddEvent(
                        msec + p_Delay, 
                        handler =>
                        {
                            handler.Arg1.SetPlay(handler.Arg2);
                            return true;
                        }, 
                        null, this,
                        p_Delay
                    );
                eventHandler.StartEvent();*/
                MapEffectSoundManager.GetInstance.OnMapEffectPlayBegin(_MapEffectSound, this);
                return true;
            }
            else
            {
                RetrieveObject();
                return false;
            }
        }
        
        public void SetStop()
        {
            _AudioSource.Stop();
        }
#endif
        #endregion
    }
}
