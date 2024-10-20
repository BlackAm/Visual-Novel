using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BlackAm
{
    public class SfxSpawnManager : SceneChangeEventSingleton<SfxSpawnManager>
    {
        #region <Const>

#if !SERVER_DRIVE
        private const string AudioObjectPrefabName = "AudioObject.prefab";
        private const int AudioObjectPreloadCount = 8;

        private const int KNIGHT_SOUND = 1000;
        private const int ARCHER_SOUND = 2000;
        private const int FOREST_FOOTSTEP_SOUND = 100;
        private const int STONE_FOOTSTEP_SOUND = 200;
        private const int SAND_FOOTSTEP_SOUND = 300;
        private const int SNOW_FOOTSTEP_SOUND = 400;
        private const int DEEPWATER_FOOTSTEP_SOUND = 500;
        private const int SHALLOWWATER_FOOTSTEP_SOUND = 600;
#endif
      
        #endregion

        #region <Fields>

#if !SERVER_DRIVE
        private Type _SfxObjectTypeCache;
        public Transform _SFXObjectWrapper;
#endif

        #endregion
        
        #region <Callbacks>

        protected override void OnCreated()
        {
#if !SERVER_DRIVE
            _SfxObjectTypeCache = typeof(SFXUnit);
            _SFXObjectWrapper = new GameObject("SfxSpawnManager").transform;
            _SFXObjectWrapper.SetParent(SystemBoot.GetInstance._Transform);
#endif
        }

        public override void OnInitiate()
        {
        }
        
        public override async UniTask OnScenePreload()
        {
#if SERVER_DRIVE
            await UniTask.CompletedTask;
#else
            await UniTask.SwitchToMainThread();
            PrefabPoolingManager.GetInstance.PreloadInstance(AudioObjectPrefabName, ResourceLifeCycleType.Scene,
                ResourceType.Dependencies, AudioObjectPreloadCount, _SfxObjectTypeCache);
#endif
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

        #endregion
        
        #region <Methods>

        public (bool, SFXUnit) GetSfx(int p_Index, MediaTool.MediaPreset<AudioClip> p_PrevMediaPreset, TransformTool.AffineCachePreset p_AffineCachePreset, 
            ResourceLifeCycleType p_LifeType = ResourceLifeCycleType.Scene, 
            ObjectDeployTool.ObjectDeploySurfaceDeployType p_DeployFlagMask = ObjectDeployTool.ObjectDeploySurfaceDeployType.None, 
            uint p_PreDelay = 0, bool p_AutoPlay = true)
        {
#if SERVER_DRIVE
            HeadlessServerManager.GetInstanceUnSafe.OnSfxSpawnRequest();
            return default;
#else
            var spawnedMediaPreset = SoundDataRoot.GetInstanceUnSafe.GetMediaClip(p_Index, p_PrevMediaPreset);
            if (spawnedMediaPreset)
            {
                var (isValid, spawnAffine) = ObjectDeployTool.CorrectAffinePreset(p_AffineCachePreset, p_DeployFlagMask);
                if (isValid)
                {
                    var spawned = PrefabPoolingManager.GetInstance
                        .PoolInstance<SFXUnit>
                        (
                            AudioObjectPrefabName, p_LifeType,
                            ResourceType.Dependencies, spawnAffine, _SfxObjectTypeCache
                        )
                        .Item1;
                    var isSpawnValid = !ReferenceEquals(null, spawned);
                    if (isSpawnValid)
                    {
                        spawned.SetMediaPreset(spawnedMediaPreset);
                        return p_AutoPlay 
                            ? (spawned.SetPlay(p_PreDelay), spawnedUnit : spawned)
                            : (true, spawnedUnit : spawned);
                    }
                    else
                    {
                        spawnedMediaPreset.Dispose();
                        return default;
                    }
                }
                else
                {
                    spawnedMediaPreset.Dispose();
                    return default;
                }
            }
            else
            {
                spawnedMediaPreset.Dispose();
                return default;
            }
#endif
        }

        public async UniTask<(bool, SFXUnit)> GetSfxAsync(int p_Index, MediaTool.MediaPreset<AudioClip> p_PrevMediaPreset, TransformTool.AffineCachePreset p_AffineCachePreset, 
            ResourceLifeCycleType p_LifeType = ResourceLifeCycleType.Scene, 
            ObjectDeployTool.ObjectDeploySurfaceDeployType p_DeployFlagMask = ObjectDeployTool.ObjectDeploySurfaceDeployType.None, 
            uint p_PreDelay = 0, bool p_AutoPlay = true)
        {
#if SERVER_DRIVE
            await UniTask.CompletedTask;
            HeadlessServerManager.GetInstanceUnSafe.OnSfxSpawnRequest();
            return default;
#else
            var spawnedMediaPreset = SoundDataRoot.GetInstanceUnSafe.GetMediaClip(p_Index, p_PrevMediaPreset);
            if (spawnedMediaPreset)
            {
                var (isValid, spawnAffine) = ObjectDeployTool.CorrectAffinePreset(p_AffineCachePreset, p_DeployFlagMask);
                if (isValid)
                {
                    var spawned = 
                        (
                            await PrefabPoolingManager.GetInstance
                                .PoolInstanceAsync<SFXUnit>
                                (
                                    AudioObjectPrefabName, p_LifeType,
                                    ResourceType.Dependencies, spawnAffine, _SfxObjectTypeCache
                                )
                        )
                        .Item1;
                    var isSpawnValid = !ReferenceEquals(null, spawned);
                    if (isSpawnValid)
                    {
                        spawned.SetMediaPreset(spawnedMediaPreset);
                        return p_AutoPlay
                            ? (spawned.SetPlay(p_PreDelay), spawnedUnit : spawned)
                            : (true, spawnedUnit : spawned);
                    }
                    else
                    {
                        spawnedMediaPreset.Dispose();
                        return default;
                    }
                }
                else
                {
                    spawnedMediaPreset.Dispose();
                    return default;
                }
            }
            else
            {
                spawnedMediaPreset.Dispose();
                return default;
            }
#endif
        }
        
        #endregion
    }
}