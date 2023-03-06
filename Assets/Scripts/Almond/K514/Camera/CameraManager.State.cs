#if !SERVER_DRIVE
namespace k514
{
    public partial class CameraManager
    {
        #region <Fields>

        /// <summary>
        /// 특정 카메라 모드를 일정 기간 동안 적용시키는 경우 이벤트타이머
        /// </summary>
        private SafeReference<object, GameEventTimerHandlerWrapper> _CameraStateEventHandler;
        
        /// <summary>
        /// 현재 카메라 상태
        /// </summary>
        private CameraState _CameraState;
        
        #endregion

        #region <Enums>

        private enum CameraState
        {
            /// <summary>
            /// 정상 상태
            /// </summary>
            None,
            
            /// <summary>
            /// 카메라 활동 정지 상태, root 래퍼가 동작하지 않게 된다.
            /// </summary>
            Idle,
        }

        #endregion

        #region <Callback>

        private void OnResetStateWrapperPartial()
        {
            ResetCameraState();
        }

        #endregion
        
        #region <Methods>

        /// <summary>
        /// 현재 카메라 상태를 초기화 시키는 메서드
        /// </summary>
        private void ResetCameraState()
        {
            EventTimerTool.ReleaseEventHandler(ref _CameraStateEventHandler);
            _CameraState = CameraState.None;
        }

        /// <summary>
        /// 지정한 시간동안 카메라를 idle 상태로 전이시키는 메서드
        /// 0 값을 넣으면 지속시간은 무한이 된다.
        /// </summary>
        private void SetCameraRootIdleState(uint p_Msec)
        {
            if (!IsCameraValid) return;
            
            _CameraState = CameraState.Idle;

            GameEventTimerHandlerManager.GetInstance.SpawnSafeEventTimerHandler(ref _CameraStateEventHandler, this, SystemBoot.TimerType.LateGameTimer, false);
            var (_, eventHandler) = _CameraStateEventHandler.GetValue();
            eventHandler
                .AddEvent(
                    p_Msec,
                    (handler) =>
                    {
                        GetInstanceUnSafe.ResetCameraState();
                        return true;
                    }
                );
            eventHandler.StartEvent();
        }

        /// <summary>
        /// 해당 카메라 상태가 idle인지 검증하는 메서드
        /// </summary>
        private bool IsCameraIdle => _CameraState == CameraState.Idle;

        #endregion
    }
}
#endif