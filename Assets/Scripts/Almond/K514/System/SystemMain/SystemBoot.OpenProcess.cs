using Cysharp.Threading.Tasks;

namespace k514
{
    public partial class SystemBoot
    {
        #region <Callbacks>

        /// <summary>
        /// 패치 씬으로 넘어가기 이전에 처리해야할 비동기 테스크 등을 처리한다.
        /// </summary>
        private async UniTask OnSystemBootInitiated()
        {
            await UniTask.DelayFrame(1);
#if UNITY_EDITOR
            await ResourceTracker.GetInstance();
#endif
            await AssetHolderManager.GetInstance();
            await LoadSystemInitializeBasisTable();
            await UniTask.SwitchToMainThread();

            TryBootSystem();
        }

        /*private void TryPatchSystem()
        {
            // 에셋을 로드하기 이전에 시스템에 패치해야할 사항이 있는지
            // 패치 씬으로 로드하여 체크해준다.
            _CurrentPhase = SystemBootPhase.TryPatch;
            SceneControllerTool.LoadSystemScene(SceneControllerTool.SystemSceneType.PatchScene);
        }

        public void OnPatchSuccess()
        {
            TryMaintenanceSystem();
        }

        private void TryMaintenanceSystem()
        {
            _CurrentPhase = SystemBootPhase.MaintenanceSystem;
            SceneControllerTool.LoadSystemScene(SceneControllerTool.SystemSceneType.MaintenanceScene);
        }*/

        public void OnMaintenanceTerminate()
        {
            TryBootSystem();
        }

        private void TryBootSystem()
        {
            _CurrentPhase = SystemBootPhase.InitializeSystem;
            SceneControllerTool.LoadSystemScene(SceneControllerTool.SystemSceneType.BootStrapScene);
        }

        public void OnBootingSuccess()
        {
            SystemEntry.OnEntryScene();
        }

        #endregion
    }
}