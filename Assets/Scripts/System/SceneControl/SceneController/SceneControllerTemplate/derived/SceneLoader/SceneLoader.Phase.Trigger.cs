using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BlackAm
{
    public partial class SceneLoader
    {
        #region <Callbacks>

        protected override void OnEntryPhaseLoop()
        {
        }
        
        protected override void _OnTerminatePhaseLoop()
        {
            SceneControllerManager.GetInstance.OnSceneLoadOver();
        }

        #endregion

        #region <Methods>

        private async UniTask Release_LifeCycle_Asset(IAsyncTaskRequest p_AsyncTaskRequest)
        {
#if UNITY_EDITOR
            Debug.LogWarning("씬 수명단위 에셋 릴리스를 시작합니다.");
#endif
            await UniTask.CompletedTask;
            // 에셋번들의 언로드는 비동기 테스크를 지원하지 않음.
            LoadAssetManager.GetInstanceUnSafe.UnloadAsset_SceneLifeCycle();
#if UNITY_EDITOR
            Debug.LogWarning("씬 수명단위 에셋 릴리스에 성공했습니다.");
#endif
        }
        
        private async UniTask Release_LifeCycle_Singleton(IAsyncTaskRequest p_AsyncTaskRequest)
        {
#if UNITY_EDITOR
            Debug.LogWarning("씬 수명단위 싱글톤 릴리스를 시작합니다.");
#endif
            await TableManager.GetInstance.Clear_SceneLifeCycle_TableSingleton();
#if UNITY_EDITOR
            Debug.LogWarning("씬 수명단위 싱글톤 릴리스에 성공했습니다.");
#endif
        }
        
        private async UniTask CheckSceneBundle(IAsyncTaskRequest p_AsyncTaskRequest)
        {
#if UNITY_EDITOR
            Debug.LogWarning("전이할 씬의 번들을 로드합니다.");
#endif
            await SceneControllerTool.LoadCurrentSceneBundle();
#if UNITY_EDITOR
            Debug.LogWarning("번들 로드에 성공했습니다.");
#endif
        }
        
        private async UniTask<AsyncOperation> AsyncLoadScene(IAsyncTaskRequest p_AsyncTaskRequest)
        {
#if UNITY_EDITOR
            Debug.LogWarning("씬을 로드합니다.");
#endif
            await UniTask.CompletedTask;
            var currentSceneFullPath = SceneControllerManager.GetInstance.GetCurrentSceneFullPath();

            await LoadAssetManager.GetInstanceUnSafe.LoadScene
                                            (
                                                ResourceLifeCycleType.Scene,
                                                Almond.Util.Utils.GetFileName(currentSceneFullPath)
                                            );

            return SceneManager.LoadSceneAsync(currentSceneFullPath, LoadSceneMode.Additive);
        }
        
        private async UniTask PreloadResource(IAsyncTaskRequest p_AsyncTaskRequest)
        {
#if UNITY_EDITOR
            Debug.LogWarning("해당 씬에서 수행할 초기 로드 작업을 수행합니다.");
#endif
            await UniTask.DelayFrame(1);
            await SystemBoot.GetInstance.OnScenePreload();
#if UNITY_EDITOR
            Debug.LogWarning("해당 씬에서 수행할 초기 로드 작업에 성공했습니다.");
#endif
        }
        
        private async UniTask MergeScene(IAsyncTaskRequest p_AsyncTaskRequest)
        {
#if UNITY_EDITOR
            Debug.LogWarning("씬을 병합합니다.");
#endif
            await UniTask.CompletedTask;
            var currentSceneFullPath = SceneControllerManager.GetInstance.GetCurrentSceneFullPath();
            var loadScene = SceneManager.GetActiveScene();
            var loadedScene = SceneManager.GetSceneByPath(currentSceneFullPath);
            
            // 씬 매니저의 씬 병합은 비동기 테스크를 지원하지 않음.
            SceneManager.MergeScenes(loadScene, loadedScene);
#if UNITY_EDITOR
            Debug.LogWarning("씬이 병합되었습니다.");
#endif
        }
        
        private async UniTask SceneLoadOver(IAsyncTaskRequest p_AsyncTaskRequest)
        {
#if UNITY_EDITOR
            Debug.LogWarning("씬 로드 완료 작업을 시작합니다.");
#endif
            await UniTask.CompletedTask;
#if UNITY_EDITOR
            Debug.LogWarning("씬 로드 완료 작업이 성공했습니다.");
#endif
        }
        
        #endregion
    }
}