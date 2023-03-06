using Cysharp.Threading.Tasks;
using UnityEngine;

namespace k514
{
    public class AutoMuttonSpawnManager : Singleton<AutoMuttonSpawnManager>, IDeployableSpawner
    {
        #region <Fields>

        public ObjectDeployTool.DeployableType DeployableType { get; protected set; }
        public Transform _AutoMuttonObjectWrapper;

        #endregion
        
        #region <Callbacks>

        protected override void OnCreated()
        {
            DeployableType = ObjectDeployTool.DeployableType.AutoMutton;
            _AutoMuttonObjectWrapper = new GameObject("AutoMuttonBox").transform;
            _AutoMuttonObjectWrapper.SetParent(SystemBoot.GetInstance._Transform);
        }

        public override void OnInitiate()
        {
        }

        #endregion

        #region <Methods>

        public void Preload(int p_Index, int p_Count)
        {
            AutoMuttonSpawnData.GetInstanceUnSafe.PreloadPrefab(p_Index, ResourceLifeCycleType.Scene, 8);
        }

        public (bool, T) RunAutoMutton<T>(int p_Index, TransformTool.AffineCachePreset p_AffineCachePreset, 
            ResourceLifeCycleType p_LifeType = ResourceLifeCycleType.Scene, 
            ObjectDeployTool.ObjectDeploySurfaceDeployType p_DeployFlagMask = ObjectDeployTool.ObjectDeploySurfaceDeployType.None,
            uint p_PreDelay = 0, bool p_AutoPlay = true
        ) where T : AutoMuttonBase
        {
            var spawned = AutoMuttonSpawnData.GetInstanceUnSafe.SpawnPrefab<T>(p_Index, p_AffineCachePreset, p_DeployFlagMask, p_LifeType);
            if (spawned.Item1)
            {
                if (p_AutoPlay)
                {
                    spawned.Item2.Run(p_PreDelay);
                }
            }

            return spawned;
        }
        
        public async UniTask<(bool, T)> RunAutoMuttonAsync<T>(int p_Index, TransformTool.AffineCachePreset p_AffineCachePreset, 
            ResourceLifeCycleType p_LifeType = ResourceLifeCycleType.Scene, 
            ObjectDeployTool.ObjectDeploySurfaceDeployType p_DeployFlagMask = ObjectDeployTool.ObjectDeploySurfaceDeployType.None,
            uint p_PreDelay = 0, bool p_AutoPlay = true
        ) where T : AutoMuttonBase
        {
            var spawned = await AutoMuttonSpawnData.GetInstanceUnSafe.SpawnPrefabAsync<T>(p_Index, p_AffineCachePreset, p_DeployFlagMask, p_LifeType);
            if (spawned.Item1)
            {
                if (p_AutoPlay)
                {
                    spawned.Item2.Run(p_PreDelay);
                }
            }

            return spawned;
        }

        #endregion
    }
}