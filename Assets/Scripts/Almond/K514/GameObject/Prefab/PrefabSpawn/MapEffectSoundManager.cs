#if !SERVER_DRIVE
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace k514
{
    public class MapEffectSoundManager : SceneChangeEventSingleton<MapEffectSoundManager>
    {
        #region <Const>

        private const string MapEffectObjectPrefabName = "MapEffectAudioObject.prefab";
        private const int PreloadMapEffectCount = 2;
        public const int VillageAmbience = 80010;
        public const int VillageWalla = 80012;

        #endregion

        #region <Fields>

        public Dictionary<MapEffectSound, MapEfffectUnit> _ReservedMapEffectCountCollection;
        private List<MapEfffectUnit> _CurrentPlayingMapEffectGroup;
        private Type _MapEffectObjectTypeCache;
        private float _BackingField_value;
        public Transform _MapEffectObjectWrapper;

        public float Volume
        {
            get => _BackingField_value;
            set
            {
                var strayVal = Mathf.Clamp01(value);
                _BackingField_value = strayVal;
#if !SERVER_DRIVE
                foreach (var sfxUnit in _CurrentPlayingMapEffectGroup)
                {
                    sfxUnit.SetVolume(strayVal);
                }
#endif
            }
        }
        
        /// <summary>
        /// 씬 설정이 변경된 경우, 해당 이벤트를 수신받는 오브젝트
        /// </summary>
        private SceneVariableChangeEventReceiver SceneVariableChangeEventReceiver;

        #endregion

        #region <Enums>

        public MapEffectSound[] MapEffectTypeEnumerator;
        
        public enum MapEffectSound
        {
            None,
            Environment,
            Water,
        }

        #endregion

        #region <Callbacks>

        protected override void OnCreated()
        {
            _ReservedMapEffectCountCollection = new Dictionary<MapEffectSound, MapEfffectUnit>();
            _CurrentPlayingMapEffectGroup = new List<MapEfffectUnit>();
            _MapEffectObjectTypeCache = typeof(MapEfffectUnit);
            _MapEffectObjectWrapper = new GameObject("MapEffectManager").transform;
            _MapEffectObjectWrapper.SetParent(SystemBoot.GetInstance._Transform);

            TurnMapEffectVolume(1f);

            MapEffectTypeEnumerator = SystemTool.GetEnumEnumerator<MapEffectSound>(SystemTool.GetEnumeratorType.GetAll);

            foreach (var mapEffectType in MapEffectTypeEnumerator)
            {
                _ReservedMapEffectCountCollection.Add(mapEffectType, null);
            }
            
            SceneVariableChangeEventReceiver =
                SceneEnvironmentManager.GetInstance
                    .GetEventReceiver<SceneVariableChangeEventReceiver>(
                        SceneEnvironmentManager.SceneVariableEventType.OnSceneVariableChanged, OnMapVariableChanged);
        }
        
        private void OnMapVariableChanged(SceneEnvironmentManager.SceneVariableEventType p_EventType,
            SceneVariableData.TableRecord p_Record)
        {
            GetMapEffectUnit(LamiereGameManager.GetInstanceUnSafe.CurrentSceneEffectSoundIndex, _MapEffectObjectWrapper, MapEffectSound.Environment);
        }

        public override void OnInitiate()
        {
        }

        public override async UniTask OnScenePreload()
        {
            await PrefabPoolingManager.GetInstance.PreloadInstanceAsync(MapEffectObjectPrefabName, ResourceLifeCycleType.Scene,
                ResourceType.VfxPrefab, PreloadMapEffectCount, _MapEffectObjectTypeCache);
            
            foreach (var mapEffectType in MapEffectTypeEnumerator)
            {
                _ReservedMapEffectCountCollection[mapEffectType] = null;
            }
        }

        public override void OnSceneStarted()
        {
        }

        public override void OnSceneTerminated()
        {
        }

        public override void OnSceneTransition()
        {
        }

        public void OnMapEffectPlayBegin(MapEffectSound p_MapEffectSound, MapEfffectUnit p_MapEfffectUnit)
        {
            if (!_CurrentPlayingMapEffectGroup.Contains(p_MapEfffectUnit))
            {
                _CurrentPlayingMapEffectGroup.Add(p_MapEfffectUnit);
            }

            if (ReferenceEquals(null, _ReservedMapEffectCountCollection[p_MapEffectSound]))
            {
                _ReservedMapEffectCountCollection[p_MapEffectSound] = p_MapEfffectUnit;
            }
        }

        public void OnMapEffectPlayOver(MapEffectSound p_MapEffectSound, MapEfffectUnit p_MapEfffectUnit)
        {
            _CurrentPlayingMapEffectGroup.Remove(p_MapEfffectUnit);
        }

        #endregion

        #region <Methods>

        /*public bool IsValidPlayMapEffect(MapEfffectUnit p_MapEfffectUnit)
        {
            if (_CurrentPlayingMapEffectGroup.Count < MaxReservingMapEffectCount)
            {
                if (_ReservedMapEffectCountCollection.ContainsValue(p_MapEfffectUnit))
                {
                    return _ReservedMapEffectCountCollection[p_MapEfffectUnit._MapEffectSound] < MapReservingSameMapEffectCount;
                }
                else
                {
                    _ReservedMapEffectCountCollection.Add(p_MapEfffectUnit._MapEffectSound, p_MapEfffectUnit);
                    return 0 < MapReservingSameMapEffectCount;
                }
            }
            else
            {
                return false;
            }
        }*/

        public void TurnMapEffectVolume(float p_Volume01)
        {
            Volume = p_Volume01;
            foreach (var mapEffectUnit in _CurrentPlayingMapEffectGroup)
            {
                mapEffectUnit.SetVolume();
            }
        }

        public (bool, MapEfffectUnit) GetMapEffectUnit(int p_Index, TransformTool.AffineCachePreset p_AffineCachePreset, MapEffectSound p_MapEffectSound, 
            ResourceLifeCycleType p_LifeType = ResourceLifeCycleType.Scene,
            ObjectDeployTool.ObjectDeploySurfaceDeployType p_DeployFlagMask =
                ObjectDeployTool.ObjectDeploySurfaceDeployType.None,
            uint p_PreDelay = 0, bool p_AutoPlay = true)
        {
            var tryClip = SoundDataRoot.GetInstanceUnSafe.GetMediaClip(p_Index, default);
            if (tryClip)
            {
                var (isValid, spawnAffine) = ObjectDeployTool.CorrectAffinePreset(p_AffineCachePreset, p_DeployFlagMask);

                if (!ReferenceEquals(null, _ReservedMapEffectCountCollection[p_MapEffectSound]))
                {
                    _ReservedMapEffectCountCollection[p_MapEffectSound].SetClip(p_MapEffectSound, tryClip);
                    return p_AutoPlay
                        ? (_ReservedMapEffectCountCollection[p_MapEffectSound].SetPlay(p_PreDelay), spawnedUnit: _ReservedMapEffectCountCollection[p_MapEffectSound])
                        : (true, spawnedUnit: _ReservedMapEffectCountCollection[p_MapEffectSound]);
                }
                else
                {
                    if (isValid)
                    {
                        var spawned = PrefabPoolingManager.GetInstance.PoolInstance<MapEfffectUnit>(MapEffectObjectPrefabName,
                            p_LifeType,
                            ResourceType.VfxPrefab, spawnAffine, _MapEffectObjectTypeCache).Item1;
                        var isSpawnValid = !ReferenceEquals(null, spawned);
                        if (isSpawnValid)
                        {
                            spawned.SetClip(p_MapEffectSound, tryClip);
                            return p_AutoPlay
                                ? (spawned.SetPlay(p_PreDelay), spawnedUnit: spawned)
                                : (true, spawnedUnit: spawned);
                        }
                        else
                        {
                            return default;
                        }
                    }
                    else
                    {
                        return default;
                    }
                }
            }
            else
            {
                return default;
            }
        }

        public async UniTask<(bool, MapEfffectUnit)> GetMapEffectUnitAsync(int p_Index, TransformTool.AffineCachePreset p_AffineCachePreset, MapEffectSound p_MapEffectSound, 
            ResourceLifeCycleType p_LifeType = ResourceLifeCycleType.Scene,
            ObjectDeployTool.ObjectDeploySurfaceDeployType p_DeployFlagMask =
            ObjectDeployTool.ObjectDeploySurfaceDeployType.None,
            uint p_PreDelay = 0, bool p_AutoPlay = true)
        {
            var tryClip = SoundDataRoot.GetInstanceUnSafe.GetMediaClip(p_Index, default);
            if (tryClip)
            {
                var (isValid, spawnAffine) =
                    ObjectDeployTool.CorrectAffinePreset(p_AffineCachePreset, p_DeployFlagMask);

                if (isValid)
                {
                    var spawned = (await PrefabPoolingManager.GetInstance.PoolInstanceAsync<MapEfffectUnit>(
                        MapEffectObjectPrefabName, p_LifeType,
                        ResourceType.VfxPrefab, spawnAffine, _MapEffectObjectTypeCache)).Item1;
                    var isSpawnValid = !ReferenceEquals(null, spawned);
                    if (isSpawnValid)
                    {
                        spawned.SetClip(p_MapEffectSound, tryClip);
                        return p_AutoPlay
                            ? (spawned.SetPlay(p_PreDelay), spawnedUnit: spawned)
                            : (true, spawnedUnit: spawned);
                    }
                    else
                    {
                        return default;
                    }
                }
                else
                {
                    return default;
                }
            }
            else
            {
                return default;
            }
        }

        public void ReleaseSFX()
        {
            foreach (var mapEffectType in MapEffectTypeEnumerator)
            {
                if (!ReferenceEquals(null, _ReservedMapEffectCountCollection[mapEffectType]))
                {
                    _ReservedMapEffectCountCollection[mapEffectType].RetrieveObject();
                }
            }
        }
        
        #endregion
    }
    
}
#endif