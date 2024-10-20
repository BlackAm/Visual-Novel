#if !SERVER_DRIVE
using Cysharp.Threading.Tasks;

namespace BlackAm
{
    public class UISpawnManager : Singleton<UISpawnManager>, IDeployableSpawner
    {
        #region <Fields>

        public ObjectDeployTool.DeployableType DeployableType { get; protected set; }

        #endregion
        
        #region <Callbacks>

        protected override void OnCreated()
        {
            DeployableType = ObjectDeployTool.DeployableType.UI;
        }

        public override void OnInitiate()
        {
        }

        #endregion

        #region <Methods>

        public void Preload(int p_Index, int p_Count)
        {
            UIPrefabSpawnData.GetInstanceUnSafe.PreloadPrefab(p_Index, ResourceLifeCycleType.Scene, 8);
        }

        public (bool, T) PopUI<T>(int p_Index, TransformTool.AffineCachePreset p_AffineCachePreset, 
            ResourceLifeCycleType p_LifeType = ResourceLifeCycleType.Scene, 
            ObjectDeployTool.ObjectDeploySurfaceDeployType p_DeployFlagMask = ObjectDeployTool.ObjectDeploySurfaceDeployType.None
        ) where T : UIManagerBase
        {
            var spawned = UIPrefabSpawnData.GetInstanceUnSafe.SpawnPrefab<T>(p_Index, p_AffineCachePreset, p_DeployFlagMask, p_LifeType);
            if (spawned.Item1)
            {
            }

            return spawned;
        }
        
        public async UniTask<(bool, T)> PopUIAsync<T>(int p_Index, TransformTool.AffineCachePreset p_AffineCachePreset, 
            ResourceLifeCycleType p_LifeType = ResourceLifeCycleType.Scene, 
            ObjectDeployTool.ObjectDeploySurfaceDeployType p_DeployFlagMask = ObjectDeployTool.ObjectDeploySurfaceDeployType.None
        ) where T : UIManagerBase
        {
            var spawned = await UIPrefabSpawnData.GetInstanceUnSafe.SpawnPrefabAsync<T>(p_Index, p_AffineCachePreset, p_DeployFlagMask, p_LifeType);
            if (spawned.Item1)
            {
            }

            return spawned;
        }

        #endregion
    }
}
#endif