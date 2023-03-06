#if UNITY_EDITOR && ON_GUI
using UnityEngine;

namespace k514
{
    public partial class TestManager
    {
        #region <Callbacks>

        private void OnAwakeCameraTest()
        {
            var targetControlType = TestControlType.CameraTest;
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.Q, CameraControl, "카메라 줌 아웃");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.W, CameraControl, "카메라 흔들기");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.A, CameraControl, "1인칭 카메라");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.S, CameraControl, "탑뷰 카메라");
        }

        #endregion

        #region <Methods>

        private void CameraControl(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
#if !SERVER_DRIVE
            var functionType = p_Type.CommandType;

            switch (functionType)
            {
                case ControllerTool.CommandType.Q:
                    if (p_Type.IsInputRelease)
                    {
                        CameraManager.GetInstanceUnSafe.AddCameraDistanceZoom(CameraManager.CameraWrapperType.Directional_0, 1f, 1f);
                    }
                    break;
                case ControllerTool.CommandType.W:
                    if (p_Type.IsInputRelease)
                    {
                        CameraManager.GetInstanceUnSafe.SetShake(Vector3.left, 15f, 100, 300, 25);
                    }
                    break;
                case ControllerTool.CommandType.A:
                    if (p_Type.IsInputRelease)
                    {
                        CameraManager.GetInstanceUnSafe.SetCameraModeFirstPersonTracing(PlayerManager.GetInstance);
                    }
                    break;
                case ControllerTool.CommandType.S:
                    if (p_Type.IsInputRelease)
                    {
                        CameraManager.GetInstanceUnSafe.SetCameraModeTracingObject(PlayerManager.GetInstance);
                    }
                    break;
            }
#endif
        }

        #endregion
    }
}
#endif