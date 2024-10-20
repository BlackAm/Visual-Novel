#if !SERVER_DRIVE

namespace BlackAm
{
    public partial class FocusableInstance
    {
        #region <Callbacks>

        /// <summary>
        /// 카메라가 해당 유닛을 포커싱하기 시작한 경우 호출되는 콜백
        /// </summary>
        public virtual void OnCameraFocused(CameraManager.CameraMode p_ModeType)
        {
        }

        /// <summary>
        /// 카메라의 촬영 모드가 변경된 경우 호출되는 콜백
        /// </summary>
        public virtual void OnCameraModeChanged(CameraManager.CameraMode p_PrevCameraMode, CameraManager.CameraMode p_CurrentCameraMode)
        {
        }

        /// <summary>
        /// 카메라가 해당 유닛으로부터 포커싱을 해제한 경우 호출되는 콜백
        /// </summary>
        public virtual void OnCameraFocusTerminated()
        {
        }

        #endregion
    }
}
#endif