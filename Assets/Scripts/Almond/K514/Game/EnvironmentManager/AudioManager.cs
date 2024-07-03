using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace k514
{
    public class AudioManager : Singleton<AudioManager>
    {
        #region <Consts>

        private const int MaxReservingSFXCount = 48;
        private const int MaxReservingSameSFXCount = 24;
        private const string MIXER_EXT = ".mixer";
        private const float _DefaultClipVolumeRate = 0.9f;
        private const float _Volume_DB_LowerBound = -80f;
        private const float _Volume_DB_UpperBound = 0f;
        
        #endregion

        #region <Fields>

        private Dictionary<SoundDataRoot.SoundFXType, AudioMixerPreset> _MixerTable;
        private Dictionary<AudioChannelType, Dictionary<AudioChannelPropertyType, string>> _ChannelPropertyNameTable;
        private Dictionary<int, int> _ReservedSFXCountCollection;
        private List<SFXUnit> _CurrentPlayingSfxGroup;
        private float _BackingField_SfxUnitVolume;
        
        public float SfxUnitVolume
        {
            get => _BackingField_SfxUnitVolume;
            set
            {
                _BackingField_SfxUnitVolume = Mathf.Clamp01(value);
#if !SERVER_DRIVE
                foreach (var sfxUnit in _CurrentPlayingSfxGroup)
                {
                    sfxUnit.SetVolume(_BackingField_SfxUnitVolume);
                }
#endif
            }
        }

        #endregion

        #region <Enums>

        public enum AudioChannelType
        {
            Master,
            Channel00,
            Channel01,
        }

        public enum AudioChannelPropertyType
        {
            Pitch,
            Volume,
        }

        #endregion
        
        #region <Callbacks>

        protected override void OnCreated()
        {
            _MixerTable = new Dictionary<SoundDataRoot.SoundFXType, AudioMixerPreset>();
            
            var mixerEnumerator = SystemTool.GetEnumEnumerator<SoundDataRoot.SoundFXType>(SystemTool.GetEnumeratorType.GetAll);
            var channelEnumerator = SystemTool.GetEnumEnumerator<AudioChannelType>(SystemTool.GetEnumeratorType.GetAll);

            foreach (var mixerType in mixerEnumerator)
            {
                var assetName = $"{mixerType}{MIXER_EXT}";
                var tryAsset = LoadAssetManager.GetInstanceUnSafe.LoadAsset<AudioMixer>(ResourceType.Dependencies,
                    ResourceLifeCycleType.Free_Condition, assetName);
                var mixer = tryAsset.Item2;
                var mixerPreset = new AudioMixerPreset(mixer);
                _MixerTable.Add(mixerType, mixerPreset);
                
                var channelTable = mixerPreset.ChannelTable;
                foreach (var channelType in channelEnumerator)
                {
                    var tryChannel = mixer.FindMatchingGroups(channelType.ToString())[0];
                    channelTable.Add(channelType, tryChannel);
                }
            }
            
            _ChannelPropertyNameTable = new Dictionary<AudioChannelType, Dictionary<AudioChannelPropertyType, string>>();
            var channelPropertyNameEnumerator = SystemTool.GetEnumEnumerator<AudioChannelPropertyType>(SystemTool.GetEnumeratorType.GetAll);

            foreach (var channelType in channelEnumerator)
            {
                var propertyNameTable = new Dictionary<AudioChannelPropertyType, string>();
                _ChannelPropertyNameTable.Add(channelType, propertyNameTable);
                foreach (var channelPropertyNameType in channelPropertyNameEnumerator)
                {
                    propertyNameTable.Add(channelPropertyNameType, $"{channelType}{channelPropertyNameType}");
                }
            }
            
            _ReservedSFXCountCollection = new Dictionary<int, int>();
            _CurrentPlayingSfxGroup = new List<SFXUnit>();

            SfxUnitVolume = _DefaultClipVolumeRate;
        }

        public override void OnInitiate()
        {
        }

#if !SERVER_DRIVE
        public void OnSfxPlayBegin(SFXUnit p_SfxUnit)
        {
            _CurrentPlayingSfxGroup.Add(p_SfxUnit);
            _ReservedSFXCountCollection[p_SfxUnit.MediaPreset.Index]++;
        }
        
        public void OnSfxPlayOver(SFXUnit p_SfxUnit)
        {
            var removeSfx = _CurrentPlayingSfxGroup.Remove(p_SfxUnit);
            if (removeSfx)
            {
                var tryKey = p_SfxUnit.MediaPreset.Index;
                if (_ReservedSFXCountCollection.ContainsKey(tryKey))
                {
                    _ReservedSFXCountCollection[tryKey]--;
                }
            }
        }
#endif

        #endregion

        #region <Methods>
        
        public bool SetVolume(SoundDataRoot.SoundFXType p_AudioType, AudioChannelType p_ChannelType, float p_Value01 = 1f)
        {
            var tryMixer = _MixerTable[p_AudioType].AudioMixer;
            return tryMixer.SetFloat(_ChannelPropertyNameTable[p_ChannelType][AudioChannelPropertyType.Volume], Mathf.Lerp(_Volume_DB_LowerBound, _Volume_DB_UpperBound, p_Value01));
        }

        public AudioMixerGroup GetAudioChannel<Me2>(MediaTool.MediaPreset<Me2> p_MediaPreset) where Me2 : Object
        {
            var tryAudioPreset = p_MediaPreset.AudioPreset;
            return GetAudioChannel(tryAudioPreset);
        }
        
        public AudioMixerGroup GetAudioChannel(AudioPreset p_AudioPreset)
        {
            return GetAudioChannel(p_AudioPreset.AudioType, p_AudioPreset.AudioChannelType);
        }
        
        public AudioMixerGroup GetAudioChannel(SoundDataRoot.SoundFXType p_AudioType, AudioChannelType p_AudioChannelType)
        {
            return _MixerTable[p_AudioType].ChannelTable[p_AudioChannelType];
        }

        public bool IsValidPlaySfx(int p_SfxIndex)
        {
            if (p_SfxIndex == default)
            {
                return true;
            }
            else
            {
                if (_CurrentPlayingSfxGroup.Count < MaxReservingSFXCount)
                {
                    if (_ReservedSFXCountCollection.ContainsKey(p_SfxIndex))
                    {
                        return _ReservedSFXCountCollection[p_SfxIndex] < MaxReservingSameSFXCount;
                    }
                    else
                    {
                        _ReservedSFXCountCollection.Add(p_SfxIndex, 0);
                        return 0 < MaxReservingSameSFXCount;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        public bool StopAllCurrentPlayingSfxUnit()
        {
            foreach (var sfxUnit in _CurrentPlayingSfxGroup)
            {
                OnSfxPlayOver(sfxUnit);
            }
            return true;
        }

        #endregion

        #region <Disposable>

        /// <summary>
        /// 인스턴스가 파기될 때 수행할 작업을 기술한다.
        /// </summary>
        protected override void DisposeUnManaged()
        {
            var mixerEnumerator = SystemTool.GetEnumEnumerator<SoundDataRoot.SoundFXType>(SystemTool.GetEnumeratorType.GetAll);
            var channelEnumerator = SystemTool.GetEnumEnumerator<AudioChannelType>(SystemTool.GetEnumeratorType.GetAll);

            foreach (var mixerType in mixerEnumerator)
            {
                foreach (var channelType in channelEnumerator)
                {
                    SetVolume(mixerType, channelType);
                }                
            }
            
            base.DisposeUnManaged();
        }

        #endregion
        
        #region <Structrs>
        
        private struct AudioMixerPreset
        {
            #region <Fields>

            public AudioMixer AudioMixer;
            public Dictionary<AudioChannelType, AudioMixerGroup> ChannelTable;

            #endregion

            #region <Constructors>

            public AudioMixerPreset(AudioMixer p_AudioMixer)
            {
                AudioMixer = p_AudioMixer;
                ChannelTable = new Dictionary<AudioChannelType, AudioMixerGroup>();
            }

            #endregion
        }
        
        public struct AudioPreset
        {
            #region <Fields>

            public SoundDataRoot.SoundFXType AudioType;
            public AudioChannelType AudioChannelType;

            #endregion

            #region <Constructors>

            public AudioPreset(SoundDataRoot.SoundFXType p_AudioType, AudioChannelType p_AudioChannelType)
            {
                AudioType = p_AudioType;
                AudioChannelType = p_AudioChannelType;
            }

            #endregion
        }

        #endregion
    }
}