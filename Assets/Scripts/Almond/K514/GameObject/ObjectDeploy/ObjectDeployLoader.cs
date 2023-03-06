using Cysharp.Threading.Tasks;

namespace k514
{
    public class ObjectDeployLoader : SceneChangeEventSingleton<ObjectDeployLoader>, IDeployEventRecord
    {
        #region <Consts>

        private const int PreloadCount = 16;

        #endregion
       
        #region <Fields>

        public ObjectPooler<ObjectDeployEventRecord> ObjectDeployEventRecordPooler { get; private set; }

        #endregion

        #region <Callbacks>

        protected override void OnCreated()
        {
            ObjectDeployEventRecordPooler = new ObjectPooler<ObjectDeployEventRecord>();
        }

        public override void OnInitiate()
        {
        }

        public override async UniTask OnScenePreload()
        {
            await UniTask.SwitchToMainThread();
            ObjectDeployEventRecordPooler.PreloadPool(PreloadCount, PreloadCount);
        }

        public override void OnSceneStarted()
        {
        }

        public override void OnSceneTerminated()
        {
            ObjectDeployEventRecordPooler.RetrieveAllObject();
        }

        public override void OnSceneTransition()
        {
        }

        #endregion

        #region <Methods>

        public ObjectDeployEventRecord GetObjectDeployEventRecord(Unit p_Pivot)
        {
            var spawned = ObjectDeployEventRecordPooler.GetObject();
            spawned.SetUnit(p_Pivot);
            return spawned;
        }
        
        public ObjectDeployEventRecord GetObjectDeployEventRecord(Unit p_Pivot, TransformTool.AffineCachePreset p_Affine)
        {
            var spawned = ObjectDeployEventRecordPooler.GetObject();
            spawned.SetUnit(p_Pivot);
            spawned.SetZeroAffine(p_Affine);
            
            return spawned;
        }

        public void CastDeployEventMap(int p_DeployIndex, Unit p_Pivot)
        {
            CastDeployEventMap(p_DeployIndex, p_Pivot, p_Pivot._Transform);
        }

        public async void CastDeployEventMap(int p_DeployIndex, Unit p_Pivot, TransformTool.AffineCachePreset p_Affine)
        {
            if (p_DeployIndex == default) return;
            
            var spawned = ObjectDeployEventRecordPooler.GetObject();
            spawned.SetUnit(p_Pivot);
            spawned.SetZeroAffine(p_Affine);
            
            var targetPreset = ObjectDeployPresetMapData.GetInstanceUnSafe[p_DeployIndex];
            spawned.CalculateAllFrameDeployAffine(targetPreset.DeployEventPresetMap, default);
            
            await spawned.CastDeployEvent();
        }

        #endregion
    }
}