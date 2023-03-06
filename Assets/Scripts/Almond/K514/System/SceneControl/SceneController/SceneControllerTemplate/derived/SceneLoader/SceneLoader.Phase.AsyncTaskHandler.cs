using System;
using UnityEngine;

namespace k514
{
    public partial class SceneLoader
    {
        #region <Callbacks>

        protected override void OnSequenceBegin(AsyncLoadSceneSequence p_AsyncTaskSequence)
        {
            var (taskPhase, taskSeqIndex, taskLastIndex) = p_AsyncTaskSequence.GetSequenceKey();
            switch (taskPhase)
            {
                case SceneLoadingProgressPhase.UnloadingResource:
                    if (!SceneControllerManager.GetInstance.IsFirstSceneTransition())
                    {
                        BridgeEntryPhaseLoop();
                    }
                    break;
                case SceneLoadingProgressPhase.LoadingResource:
                    if (SceneControllerManager.GetInstance.IsFirstSceneTransition())
                    {
                        BridgeEntryPhaseLoop();
                    }
                    break;
            }
        }

        protected override void OnSequenceTerminate(AsyncLoadSceneSequence p_AsyncTaskSequence)
        {
            var (taskPhase, taskSeqIndex, taskLastIndex) = p_AsyncTaskSequence.GetSequenceKey();
            switch (taskPhase)
            {
                case SceneLoadingProgressPhase.UnloadingResource:
                    SwitchPhase(SceneLoadingProgressPhase.LoadingResource);
                    break;
                case SceneLoadingProgressPhase.LoadingResource:
                    SwitchPhase(SceneLoadingProgressPhase.LoadingScene);
                    break;
                case SceneLoadingProgressPhase.LoadingScene:
                    SwitchPhase(SceneLoadingProgressPhase.LoadingSceneStageOn);
                    break;
                case SceneLoadingProgressPhase.LoadingSceneStageOn:
                    SwitchPhase(SceneLoadingProgressPhase.MergeScene);
                    break;
                case SceneLoadingProgressPhase.MergeScene:
                    SwitchPhase(SceneLoadingProgressPhase.AsyncLoadTerminate);
                    break;
                case SceneLoadingProgressPhase.AsyncLoadTerminate:
                    BridgeTerminatePhaseLoop();
                    break;
            }
        }
        
        protected override void OnTaskBegin(AsyncLoadSceneSequence p_AsyncTaskSequence, IAsyncTaskRequest p_Handler)
        {
            var (taskPhase, taskSeqIndex, taskLastIndex) = p_AsyncTaskSequence.GetSequenceKey();
        }
        
        protected override void OnTaskSuccess(AsyncLoadSceneSequence p_AsyncTaskSequence, IAsyncTaskRequest p_Handler)
        {
            var (taskPhase, taskSeqIndex, taskLastIndex) = p_AsyncTaskSequence.GetSequenceKey();
        }
        
        protected override void OnTaskFail(AsyncLoadSceneSequence p_AsyncTaskSequence, IAsyncTaskRequest p_Handler)
        {
#if UNITY_EDITOR
            Debug.LogError("[SceneLoad Error] : 씬 로딩에 실패했습니다.");
            SwitchPhase(SceneLoadingProgressPhase.AsyncLoadTerminate);
#else
            // Application.Quit();
#endif
        }
        
        protected override void OnTaskCancel(AsyncLoadSceneSequence p_AsyncTaskSequence, IAsyncTaskRequest p_Handler)
        {
        }

        #endregion
    }
}