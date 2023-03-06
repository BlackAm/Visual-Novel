using UnityEngine;

namespace k514
{
    public partial class BootStrap : SceneController<BootStrap, BootStrap.BootingProgressPhase, AsyncBootingSequence>
    {
        #region <Enums>

        public enum BootingProgressPhase
        {
            /// <summary>
            /// 동작을 하지 않는 상태
            /// </summary>
            None,
            
            /// <summary>
            /// 부팅 시작
            /// </summary>
            BootStart,
            
            /// <summary>
            /// 부팅에 필요한 시스템 초기화 작업
            /// </summary>
            SystemOpenProcess,
            SystemOpenProcess2,
            SystemOpenProcess3,
            SystemOpenProcess4,
            
            /// <summary>
            /// 부팅 종료 및 해당 씬 파기
            /// </summary>
            BootTerminate,
        }

        #endregion
        
        #region <Callbacks>

        protected override void OnCreated()
        {
            _SceneType = SceneControllerTool.SystemSceneType.BootStrapScene;

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
            AsyncBootingTaskRequestManager.GetInstance?.Dispose();
            
            base.DisposeUnManaged();
            
#if UNITY_EDITOR
            Debug.LogWarning("Notice : 부팅 씬이 파기되었습니다.");
#endif
        }

        #endregion
    }
}