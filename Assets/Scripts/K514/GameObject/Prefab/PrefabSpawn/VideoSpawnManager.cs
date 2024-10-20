using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;

namespace BlackAm
{
    public class VideoSpawnManager : SceneChangeEventSingleton<VideoSpawnManager>
    {
        #region <Const>

#if !SERVER_DRIVE
        private const string VideoObjectPrefabName = "VideoObject.prefab";
        private const int VideoObjectPreloadCount = 2;
#endif
      
        #endregion

        #region <Fields>

#if !SERVER_DRIVE
        private Type _VideoPlayObjectTypeCache;
        public Transform _VideoPlayObjectWrapper;
#endif

        #endregion
        
        #region <Callbacks>

        protected override void OnCreated()
        {
#if !SERVER_DRIVE
            _VideoPlayObjectTypeCache = typeof(VideoUnit);
            _VideoPlayObjectWrapper = new GameObject("VideoSpawnManager").transform;
            _VideoPlayObjectWrapper.SetParent(SystemBoot.GetInstance._Transform);
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
            PrefabPoolingManager.GetInstance.PreloadInstance(VideoObjectPrefabName, ResourceLifeCycleType.Scene,
                ResourceType.Dependencies, VideoObjectPreloadCount, _VideoPlayObjectTypeCache);
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

        public (bool, VideoUnit) GetVideo(int p_Index, MediaTool.MediaPreset<VideoClip> p_PrevMediaPreset, TransformTool.AffineCachePreset p_AffineCachePreset, 
            ResourceLifeCycleType p_LifeType = ResourceLifeCycleType.Scene, 
            ObjectDeployTool.ObjectDeploySurfaceDeployType p_DeployFlagMask = ObjectDeployTool.ObjectDeploySurfaceDeployType.None, 
            uint p_PreDelay = 0, bool p_AutoPlay = true)
        {
#if SERVER_DRIVE
            HeadlessServerManager.GetInstanceUnSafe.OnVideoSpawnRequest();
            return default;
#else
            var spawnedMediaPreset = VideoDataRoot.GetInstanceUnSafe.GetMediaClip(p_Index, p_PrevMediaPreset);
            if (spawnedMediaPreset)
            {
                var (isValid, spawnAffine) = ObjectDeployTool.CorrectAffinePreset(p_AffineCachePreset, p_DeployFlagMask);
                if (isValid)
                {
                    var spawned = PrefabPoolingManager.GetInstance
                        .PoolInstance<VideoUnit>
                        (
                            VideoObjectPrefabName, p_LifeType,
                            ResourceType.Dependencies, spawnAffine, _VideoPlayObjectTypeCache
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

        public async UniTask<(bool, VideoUnit)> GetVideoAsync(int p_Index, MediaTool.MediaPreset<VideoClip> p_PrevMediaPreset,TransformTool.AffineCachePreset p_AffineCachePreset, 
            ResourceLifeCycleType p_LifeType = ResourceLifeCycleType.Scene, 
            ObjectDeployTool.ObjectDeploySurfaceDeployType p_DeployFlagMask = ObjectDeployTool.ObjectDeploySurfaceDeployType.None, 
            uint p_PreDelay = 0, bool p_AutoPlay = true)
        {
#if SERVER_DRIVE
            await UniTask.CompletedTask;
            HeadlessServerManager.GetInstanceUnSafe.OnVideoSpawnRequest();
            return default;
#else
            var spawnedMediaPreset = VideoDataRoot.GetInstanceUnSafe.GetMediaClip(p_Index, p_PrevMediaPreset);
            if (spawnedMediaPreset)
            {
                var (isValid, spawnAffine) = ObjectDeployTool.CorrectAffinePreset(p_AffineCachePreset, p_DeployFlagMask);
                if (isValid)
                {
                    var spawned = 
                        (
                            await PrefabPoolingManager.GetInstance
                                .PoolInstanceAsync<VideoUnit>
                                (
                                    VideoObjectPrefabName, p_LifeType,
                                    ResourceType.Dependencies, spawnAffine, _VideoPlayObjectTypeCache
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