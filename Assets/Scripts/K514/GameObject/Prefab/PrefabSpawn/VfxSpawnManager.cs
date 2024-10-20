using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BlackAm
{
    public class VfxSpawnManager : Singleton<VfxSpawnManager>, IDeployableSpawner
    {
        #region <Fields>

        public ObjectDeployTool.DeployableType DeployableType { get; protected set; }
        public Transform _VFXObjectWrapper;

        #endregion
        
        #region <Callbacks>

        protected override void OnCreated()
        {
            DeployableType = ObjectDeployTool.DeployableType.VFX;
            _VFXObjectWrapper = new GameObject("VfxSpawnManager").transform;
            _VFXObjectWrapper.SetParent(SystemBoot.GetInstance._Transform);
        }

        public override void OnInitiate()
        {
        }

        #endregion

        #region <Methods>

        public void Preload(int p_Index, int p_Count)
        {
            VfxSpawnData.GetInstanceUnSafe.PreloadPrefab(p_Index, ResourceLifeCycleType.Scene, 8);
        }

        #endregion

        #region <Method/Vfx>

        public (bool, T) CastVfx<T>(int p_Index, TransformTool.AffineCachePreset p_AffineCachePreset, 
            ResourceLifeCycleType p_LifeType = ResourceLifeCycleType.Scene, 
            ObjectDeployTool.ObjectDeploySurfaceDeployType p_DeployFlagMask = ObjectDeployTool.ObjectDeploySurfaceDeployType.None,
            uint p_PreDelay = 0, bool p_AutoPlay = true
        ) where T : VFXUnit
        {
#if SERVER_DRIVE
            HeadlessServerManager.GetInstance.OnVfxSpawnRequest();
            return default;
#else
            var spawned = VfxSpawnData.GetInstanceUnSafe.SpawnPrefab<T>(p_Index, p_AffineCachePreset, p_DeployFlagMask, p_LifeType);
            if (spawned.Item1)
            {
                if (p_AutoPlay)
                {
                    spawned.Item2.SetPlay(p_PreDelay);
                }
            }

            return spawned;
#endif
        }
        
        public async UniTask<(bool, T)> CastVfxAsync<T>(int p_Index, TransformTool.AffineCachePreset p_AffineCachePreset, 
            ResourceLifeCycleType p_LifeType = ResourceLifeCycleType.Scene, 
            ObjectDeployTool.ObjectDeploySurfaceDeployType p_DeployFlagMask = ObjectDeployTool.ObjectDeploySurfaceDeployType.None,
            uint p_PreDelay = 0, bool p_AutoPlay = true
        ) where T : VFXUnit
        {
#if SERVER_DRIVE
            await UniTask.CompletedTask;
            HeadlessServerManager.GetInstanceUnSafe.OnVfxSpawnRequest();
            return default;
#else
            var spawned = await VfxSpawnData.GetInstanceUnSafe.SpawnPrefabAsync<T>(p_Index, p_AffineCachePreset, p_DeployFlagMask, p_LifeType);
            if (spawned.Item1)
            {
                if (p_AutoPlay)
                {
                    spawned.Item2.SetPlay(p_PreDelay);
                }
            }

            return spawned;
#endif
        }

        #endregion
        
        #region <Method/VfxProjectile>

        public (bool, T) CastVfxProjectile<T>(int p_Index, TransformTool.AffineCachePreset p_AffineCachePreset, 
            ResourceLifeCycleType p_LifeType = ResourceLifeCycleType.Scene, 
            ObjectDeployTool.ObjectDeploySurfaceDeployType p_DeployFlagMask = ObjectDeployTool.ObjectDeploySurfaceDeployType.None,
            uint p_PreDelay = 0, bool p_AutoPlay = true
        ) where T : VFXProjectileBase
        {
            var spawned = VfxSpawnData.GetInstanceUnSafe.SpawnPrefab<T>(p_Index, p_AffineCachePreset, p_DeployFlagMask, p_LifeType);
            if (spawned.Item1)
            {
                if (p_AutoPlay)
                {
                    spawned.Item2.SetPlay(p_PreDelay);
                }
            }

            return spawned;
        }
        
        public async UniTask<(bool, T)> CastVfxProjectileAsync<T>(int p_Index, TransformTool.AffineCachePreset p_AffineCachePreset, 
            ResourceLifeCycleType p_LifeType = ResourceLifeCycleType.Scene, 
            ObjectDeployTool.ObjectDeploySurfaceDeployType p_DeployFlagMask = ObjectDeployTool.ObjectDeploySurfaceDeployType.None,
            uint p_PreDelay = 0, bool p_AutoPlay = true
        ) where T : VFXProjectileBase
        {
            var spawned = await VfxSpawnData.GetInstanceUnSafe.SpawnPrefabAsync<T>(p_Index, p_AffineCachePreset, p_DeployFlagMask, p_LifeType);
            if (spawned.Item1)
            {
                if (p_AutoPlay)
                {
                    spawned.Item2.SetPlay(p_PreDelay);
                }
            }

            return spawned;
        }

        #endregion
    }
}