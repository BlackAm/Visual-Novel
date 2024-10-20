using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BlackAm
{
    /*public partial class Maintenance : SceneController<Maintenance, Maintenance.MaintenanceProgressPhase, AsyncMaintenanceSequence>
    {
        #region <Enums>

        public enum MaintenanceProgressPhase
        {
            /// <summary>
            /// 동작을 하지 않는 상태
            /// </summary>
            None,
            
            /// <summary>
            /// 작업 시작
            /// </summary>
            TaskStart,
    
            /// 작업 종료 및 해당 씬 파기
            /// </summary>
            TaskTerminate,
        }

        #endregion
        
        #region <Callbacks>

        protected override void OnCreated()
        {
            _SceneType = SceneControllerTool.SystemSceneType.MaintenanceScene;

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
            AsyncMaintenanceTaskRequestManager.GetInstance?.Dispose();
            
            base.DisposeUnManaged();
            
#if UNITY_EDITOR
            Debug.LogWarning("Notice : 시스템 보수 씬이 파기되었습니다.");
#endif
        }

        #endregion
    }*/
}