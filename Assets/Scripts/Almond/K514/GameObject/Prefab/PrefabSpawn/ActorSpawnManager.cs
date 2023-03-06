using Cysharp.Threading.Tasks;
using UnityEngine;

namespace k514
{
    public class ActorSpawnManager : Singleton<ActorSpawnManager>, IDeployableSpawner
    {
        #region <Fields>
        
        public ObjectDeployTool.DeployableType DeployableType { get; private set; }

        #endregion

        #region <Callbacks>

        protected override void OnCreated()
        {
            DeployableType = ObjectDeployTool.DeployableType.Actor;
        }

        public override void OnInitiate()
        {
        }

        #endregion

        #region <Methods>

        public void Preload(int p_Index, int p_Count)
        {
            UnitSpawnData.GetInstanceUnSafe.PreloadPrefab(p_Index, ResourceLifeCycleType.Scene, p_Count);
        }

        public (bool, T) SpawnActor<T>(int p_Index, TransformTool.AffineCachePreset p_AffineCachePreset, 
            ResourceLifeCycleType p_LifeType = ResourceLifeCycleType.Scene, 
            ObjectDeployTool.ObjectDeploySurfaceDeployType p_DeployFlagMask = ObjectDeployTool.ObjectDeploySurfaceDeployType.None
            ) where T : Unit, IDeployee
        {
            var spawned = UnitSpawnData.GetInstanceUnSafe.SpawnPrefab<T>(p_Index, p_AffineCachePreset, p_DeployFlagMask, p_LifeType);
            if (spawned.Item1)
            {
                spawned.Item2.AddAuthority(UnitTool.UnitAuthorityFlag.Actor);
            }

            return spawned;
        }
        
        public async UniTask<(bool, T)> SpawnUnitAsync<T>(int p_Index, TransformTool.AffineCachePreset p_AffineCachePreset, 
            ResourceLifeCycleType p_LifeType = ResourceLifeCycleType.Scene, 
            ObjectDeployTool.ObjectDeploySurfaceDeployType p_DeployFlagMask = ObjectDeployTool.ObjectDeploySurfaceDeployType.None
            ) where T : Unit, IDeployee
        {
            var spawned = await UnitSpawnData.GetInstanceUnSafe.SpawnPrefabAsync<T>(p_Index, p_AffineCachePreset, p_DeployFlagMask, p_LifeType);
            if (spawned.Item1)
            {
                spawned.Item2.AddAuthority(UnitTool.UnitAuthorityFlag.Actor);
            }
            
            return spawned;
        }
        
        #endregion
    }
}