using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BlackAm
{
    public partial class SceneLoader : SceneController<SceneLoader, SceneLoader.SceneLoadingProgressPhase, AsyncLoadSceneSequence>
    {
        #region <Enums>

        public enum SceneLoadingProgressPhase
        {
            /// <summary>
            /// 동작을 하지 않는 상태
            /// </summary>
            None,

            /// <summary>
            /// 리소스 언로딩 중
            /// </summary>
            UnloadingResource,

            /// <summary>
            /// 리소스 로딩 중
            /// </summary>
            LoadingResource,

            /// <summary>
            /// 비동기 씬 로딩 중
            /// </summary>
            LoadingScene,
                                  
            /// <summary>
            /// 로딩 완료 시점, 로딩 씬과 로드된 씬이 동시에 존재함.
            /// </summary>
            LoadingSceneStageOn,

            /// <summary>
            /// MergeScene 메서드 참조, 로딩 씬과 로드된 씬이 병합됨
            /// </summary>
            MergeScene,
            
            /// <summary>
            /// 씬 병합 이후, 로딩 씬에 사용되었던 해당 오브젝트 및 관련 리소스를 제거하고
            /// 씬 종료 애니메이션을 수행하는 페이즈
            /// </summary>
            AsyncLoadTerminate,
        }

        #endregion
        
        #region <Callbacks>

        protected override void OnCreated()
        {
            _SceneType = SceneControllerTool.SystemSceneType.SceneLoader;

            base.OnCreated();
        }

        #endregion

        #region <Disposable>
        
        /// <summary>
        /// 해당 오브젝트를 제거한다.
        /// 오브젝트 제거는 매 로딩 종료시에 일어나므로, 해당 파기메서드의 호출은 반드시 게임 시스템의 종료를 의미하는 것은 아니다.
        /// </summary>
        protected override void DisposeUnManaged()
        {
            AsyncLoadSceneTaskRequestManager.GetInstance?.Dispose();
            
            base.DisposeUnManaged();
            
#if UNITY_EDITOR
            Debug.LogWarning("Notice : 씬 로더가 파기되었습니다.");
#endif
        }

        #endregion
    }
}