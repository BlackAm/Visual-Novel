using Cysharp.Threading.Tasks;

namespace k514
{
    public class UnitSpawnManager : Singleton<UnitSpawnManager>, IDeployableSpawner
    {
        #region <Fields>
        
        public ObjectDeployTool.DeployableType DeployableType { get; private set; }

        #endregion

        #region <Callbacks>

        protected override void OnCreated()
        {
            DeployableType = ObjectDeployTool.DeployableType.Unit;
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

        public (bool, T) SpawnUnit<T>(int p_Index, TransformTool.AffineCachePreset p_AffineCachePreset, 
            ResourceLifeCycleType p_LifeType = ResourceLifeCycleType.Scene, 
            ObjectDeployTool.ObjectDeploySurfaceDeployType p_DeployFlagMask = ObjectDeployTool.ObjectDeploySurfaceDeployType.None) where T : Unit, IDeployee
        {
            var spawned = UnitSpawnData.GetInstanceUnSafe.SpawnPrefab<T>(p_Index, p_AffineCachePreset, p_DeployFlagMask, p_LifeType);
            if (spawned.Item1)
            {
                //spawned.Item2.AddAuthority(p_UnitAuthorityFlag);
            }

            return spawned;
        }
        
        public async UniTask<(bool, T)> SpawnUnitAsync<T>(int p_Index, TransformTool.AffineCachePreset p_AffineCachePreset, 
            ResourceLifeCycleType p_LifeType = ResourceLifeCycleType.Scene, 
            ObjectDeployTool.ObjectDeploySurfaceDeployType p_DeployFlagMask = ObjectDeployTool.ObjectDeploySurfaceDeployType.None) where T : Unit, IDeployee
        {
            await UniTask.SwitchToMainThread();

            var spawned = await UnitSpawnData.GetInstanceUnSafe.SpawnPrefabAsync<T>(p_Index, p_AffineCachePreset, p_DeployFlagMask, p_LifeType);
            if (spawned.Item1)
            {
                //spawned.Item2.AddAuthority(p_UnitAuthorityFlag);
            }
            
            return spawned;
        }
        
        #endregion
    }
}