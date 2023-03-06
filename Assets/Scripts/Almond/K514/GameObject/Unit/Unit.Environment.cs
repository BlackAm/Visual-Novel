using System.Collections.Generic;
using UnityEngine;

namespace k514
{
    public partial class Unit
    {
        #region <Fields>

        /// <summary>
        /// 해당 유닛이 보유한 사운드 타입 마스크. 추후구현
        /// </summary>
        private UnitTool.UnitEnvironmentSoundType _UnitSoundFlagMask;

        /// <summary>
        /// 해당 유닛에 의해 재생된 음원 기록을 추적하는 테이블
        /// </summary>
        private Dictionary<UnitTool.UnitEnvironmentSoundType, MediaTool.MediaPreset<AudioClip>> _AudioTrackerTable;
        
        /// <summary>
        /// 유닛 피부 타입
        /// </summary>
        public UnitTool.UnitSkinType SkinType { get; private set; }
        
        #endregion

        #region <Callbacks>

        private void OnAwakeUnitEnvironment()
        {
            var modelTableIndex = _PrefabModelKey.Item1 ? _PrefabModelKey.Item2 : 0;
            var modelRecord = (UnitModelDataRecordBridge)PrefabModelDataRoot.GetInstanceUnSafe[modelTableIndex];
            
            SkinType = modelRecord.UnitSkinType;
            _AudioTrackerTable = new Dictionary<UnitTool.UnitEnvironmentSoundType, MediaTool.MediaPreset<AudioClip>>();

            var enumerator =
                SystemTool.GetEnumEnumerator<UnitTool.UnitEnvironmentSoundType>(SystemTool.GetEnumeratorType
                    .ExceptMask);

            foreach (var soundType in enumerator)
            {
                _AudioTrackerTable.Add(soundType, default);
            }
        }

        private void OnPoolingUnitEnvironment()
        {
        }

        private void OnRetrieveUnitEnvironment()
        {
            var enumerator =
                SystemTool.GetEnumEnumerator<UnitTool.UnitEnvironmentSoundType>(SystemTool.GetEnumeratorType
                    .ExceptMask);

            foreach (var soundType in enumerator)
            {
                SetMediaPreset(soundType, default);
            }
        }

        #endregion

        #region <Methods>

        public void SetMediaPreset(UnitTool.UnitEnvironmentSoundType p_Type, MediaTool.MediaPreset<AudioClip> p_Preset)
        {
            _AudioTrackerTable[p_Type].Dispose();
            _AudioTrackerTable[p_Type] = p_Preset;
        }
        
        public MediaTool.MediaPreset<AudioClip> GetMediaPreset(UnitTool.UnitEnvironmentSoundType p_Type)
        {
            return _AudioTrackerTable[p_Type];
        }

        #endregion
    }
}