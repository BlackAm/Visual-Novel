using Cysharp.Threading.Tasks;

namespace BlackAm
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

        #endregion
    }
}