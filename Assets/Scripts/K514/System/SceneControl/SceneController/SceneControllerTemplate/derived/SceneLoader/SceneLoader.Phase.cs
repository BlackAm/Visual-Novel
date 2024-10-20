using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BlackAm
{
    public partial class SceneLoader
    {
        #region <Fields>

        #endregion

        #region <Callbacks>

        protected override void OnCreatePhase()
        {
            _PhaseWeightTable 
                = new Dictionary<SceneLoadingProgressPhase, float>
                {
                    {SceneLoadingProgressPhase.UnloadingResource, 0.5f},
                    {SceneLoadingProgressPhase.LoadingResource, 0.5f},
                    {SceneLoadingProgressPhase.LoadingScene, 0.5f},
                    {SceneLoadingProgressPhase.LoadingSceneStageOn, 0.5f},
                    {SceneLoadingProgressPhase.MergeScene, 0.5f},
                    {SceneLoadingProgressPhase.AsyncLoadTerminate, 0.5f},
                };

            base.OnCreatePhase();

            var enumerator = SystemTool.GetEnumEnumerator<SceneLoadingProgressPhase>(SystemTool.GetEnumeratorType.ExceptNone);
            foreach (var progressPhase in enumerator)
            {
                switch (progressPhase)
                {
                    case SceneLoadingProgressPhase.UnloadingResource:
                    {
                        var asyncTaskSequence = new AsyncLoadSceneSequence(progressPhase, _AsyncTaskSequenceEventHandler, _PhaseWeightTable[progressPhase]);
                        AsyncLoadSceneTaskRequestManager.GetInstance.AddAsyncTaskSequence
                        (
                            asyncTaskSequence,
                            new DefaultAsyncTaskRequestParams
                            (
                                Release_LifeCycle_Asset 
#if UNITY_EDITOR
                                , "에셋 언로드"
#endif
                            ),
                            2f, 0.3f
                        );
                        AsyncLoadSceneTaskRequestManager.GetInstance.AddAsyncTaskSequence
                        (
                            asyncTaskSequence,
                            new DefaultAsyncTaskRequestParams
                            (
                                Release_LifeCycle_Singleton
#if UNITY_EDITOR
                                , "싱글톤 언로드"
#endif
                            ),
                            2f, 0.3f
                        );

                        _AsyncTaskTable.Add(progressPhase, asyncTaskSequence);
                        break;
                    }

                    case SceneLoadingProgressPhase.LoadingResource:
                    {
                        var asyncTaskSequence = new AsyncLoadSceneSequence(progressPhase, _AsyncTaskSequenceEventHandler, _PhaseWeightTable[progressPhase]);
                        AsyncLoadSceneTaskRequestManager.GetInstance.AddAsyncTaskSequence
                        (
                            asyncTaskSequence,
                            new DefaultAsyncTaskRequestParams
                            (
                                CheckSceneBundle
#if UNITY_EDITOR
                                , "현재 씬의 번들을 로딩"
#endif
                            ),
                            2f, 0.3f
                        );
                        
                        _AsyncTaskTable.Add(progressPhase, asyncTaskSequence);
                        break;
                    }
                    case SceneLoadingProgressPhase.LoadingScene:
                    {
                        var asyncTaskSequence = new AsyncLoadSceneSequence(progressPhase, _AsyncTaskSequenceEventHandler, _PhaseWeightTable[progressPhase]);
                     
                        AsyncLoadSceneTaskRequestManager.GetInstance.AddAsyncTaskSequence
                        (
                            asyncTaskSequence,
                            new DefaultAsyncTaskRequestParams
                            (
                                AsyncLoadScene
#if UNITY_EDITOR
                                , "씬 로딩"
#endif
                            ),
                            2f, 1f
                        );
                        
                        _AsyncTaskTable.Add(progressPhase, asyncTaskSequence);
                        break;
                    }

                    case SceneLoadingProgressPhase.LoadingSceneStageOn:
                    {
                        var asyncTaskSequence = new AsyncLoadSceneSequence(progressPhase, _AsyncTaskSequenceEventHandler, _PhaseWeightTable[progressPhase]);
                        AsyncLoadSceneTaskRequestManager.GetInstance.AddAsyncTaskSequence
                        (
                            asyncTaskSequence,
                            new DefaultAsyncTaskRequestParams
                            (
                                PreloadResource
#if UNITY_EDITOR
                                , "프리로드"
#endif
                            ),
                            2f, 1f
                        );
                        
                        _AsyncTaskTable.Add(progressPhase, asyncTaskSequence);
                        break;
                    }

                    case SceneLoadingProgressPhase.MergeScene:
                    {
                        var asyncTaskSequence = new AsyncLoadSceneSequence(progressPhase, _AsyncTaskSequenceEventHandler, _PhaseWeightTable[progressPhase]);
                        AsyncLoadSceneTaskRequestManager.GetInstance.AddAsyncTaskSequence
                        (
                            asyncTaskSequence,
                            new DefaultAsyncTaskRequestParams
                            (
                                MergeScene
#if UNITY_EDITOR
                                , "씬 병합"
#endif
                            ),
                            2f, 1f
                        );

                        _AsyncTaskTable.Add(progressPhase, asyncTaskSequence);
                        break;
                    }

                    case SceneLoadingProgressPhase.AsyncLoadTerminate:
                    {
                        var asyncTaskSequence = new AsyncLoadSceneSequence(progressPhase, _AsyncTaskSequenceEventHandler, _PhaseWeightTable[progressPhase]);
                        AsyncLoadSceneTaskRequestManager.GetInstance.AddAsyncTaskSequence
                        (
                            asyncTaskSequence,
                            new DefaultAsyncTaskRequestParams
                            (
                                SceneLoadOver
#if UNITY_EDITOR
                                , "씬 로드 완료"
#endif
                            ),
                            1f, 0.5f
                        );
                        
                        _AsyncTaskTable.Add(progressPhase, asyncTaskSequence);
                        break;
                    }
                }
            }

            if (SceneControllerManager.GetInstance.IsFirstSceneTransition())
            {
                SwitchPhase(SceneLoadingProgressPhase.LoadingResource);
            }
            else
            {
                SwitchPhase(SceneLoadingProgressPhase.UnloadingResource);
            }
        }

        #endregion

        #region <Methods>

        protected override void SwitchPhase(SceneLoadingProgressPhase p_Type)
        {
            base.SwitchPhase(p_Type);

            switch (_CurrentPhase)
            {
                case SceneLoadingProgressPhase.None:
                    break;
                case SceneLoadingProgressPhase.UnloadingResource:
                case SceneLoadingProgressPhase.LoadingResource:
                case SceneLoadingProgressPhase.LoadingScene:
                case SceneLoadingProgressPhase.LoadingSceneStageOn:
                case SceneLoadingProgressPhase.MergeScene:
                case SceneLoadingProgressPhase.AsyncLoadTerminate:
                    _AsyncTaskTable[_CurrentPhase].StartAsyncTaskSequence();
                    break;
            }
        }

        #endregion
    }
}