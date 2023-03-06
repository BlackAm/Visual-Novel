using Cysharp.Threading.Tasks;
using UnityEngine;

namespace k514
{
    public class ProjectorSpawnManager : SceneChangeEventSingleton<ProjectorSpawnManager>, IDeployableSpawner
    {
        #region <Fields>
        
        public ObjectDeployTool.DeployableType DeployableType { get; private set; }
        public Transform _ProjectorObjectWrapper;

        #endregion
        
        #region <Callbacks>

        protected override void OnCreated()
        {
            DeployableType = ObjectDeployTool.DeployableType.Projector;
            _ProjectorObjectWrapper = new GameObject("ProjectorSpawnManager").transform;
            _ProjectorObjectWrapper.SetParent(SystemBoot.GetInstance._Transform);
        }

        public override void OnInitiate()
        {
        }
        
        public override async UniTask OnScenePreload()
        {
            await UniTask.SwitchToMainThread();
            Preload(5, 8);
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

        public void Preload(int p_Index, int p_Count)
        {
#if !SERVER_DRIVE
            ProjectorSpawnData.GetInstanceUnSafe.PreloadPrefab(p_Index, ResourceLifeCycleType.Scene, p_Count);
#endif
        }
        
        public (bool, T) Project<T>(int p_Index, TransformTool.AffineCachePreset p_AffineCachePreset, 
            ResourceLifeCycleType p_LifeType = ResourceLifeCycleType.Scene, 
            ObjectDeployTool.ObjectDeploySurfaceDeployType p_DeployFlagMask = ObjectDeployTool.ObjectDeploySurfaceDeployType.None
            ) where T : SimpleProjector
        {
#if SERVER_DRIVE
            HeadlessServerManager.GetInstanceUnSafe.OnProjectorSpawnRequest();
            return default;
#else
            return ProjectorSpawnData.GetInstanceUnSafe.SpawnPrefab<T>(p_Index, p_AffineCachePreset, p_DeployFlagMask, p_LifeType);
#endif
        }

        public async UniTask<(bool, T)> ProjectAsync<T>(int p_Index, TransformTool.AffineCachePreset p_AffineCachePreset, 
            ResourceLifeCycleType p_LifeType = ResourceLifeCycleType.Scene, 
            ObjectDeployTool.ObjectDeploySurfaceDeployType p_DeployFlagMask = ObjectDeployTool.ObjectDeploySurfaceDeployType.None) where T : SimpleProjector
        {
#if SERVER_DRIVE
            await UniTask.CompletedTask;
            HeadlessServerManager.GetInstanceUnSafe.OnProjectorSpawnRequest();
            return default;
#else
            return await ProjectorSpawnData.GetInstanceUnSafe.SpawnPrefabAsync<T>(p_Index, p_AffineCachePreset, p_DeployFlagMask, p_LifeType);
#endif
        }
        
        #endregion
    }
}