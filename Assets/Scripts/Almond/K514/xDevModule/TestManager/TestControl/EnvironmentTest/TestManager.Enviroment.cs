#if UNITY_EDITOR && ON_GUI

using UnityEngine;

namespace k514
{
    public partial class TestManager
    {
        #region <Fields>

        
        #endregion

        #region <Callbacks>

        void OnAwakeEnvironment()
        {
            var targetControlType = TestControlType.EnvironmentTest;
#if !SERVER_DRIVE
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.Q, PlayBGM, "신규 브금 재생");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.W, PauseBGM, "브금 일시정지");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.E, ResumeBGM, "브금 재시작");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.R, ReleaseBGM, "브금 릴리스");
            
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.A, MainLightFadeOut, "주 광원 페이드 아웃");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.S, MainLightFadeIn, "주 광원 밝기 리셋");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.D, MainLightRotate, "주 광원 회전");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.F, MainLightRotateReset, "주 광원 회전리셋");
#endif
        }

        #endregion

        #region <Methods>

#if !SERVER_DRIVE
        private void PlayBGM(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if (p_Type.IsInputRelease)
            {
                BGMManager.GetInstance.SetBGM();
            }
        }

        private void PauseBGM(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if (p_Type.IsInputRelease)
            {
                BGMManager.GetInstance.PauseBGM();
            }
        }
        
        private void ResumeBGM(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if (p_Type.IsInputRelease)
            {
                BGMManager.GetInstance.ResumeBGM();
            }
        }
        
        private void ReleaseBGM(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if (p_Type.IsInputRelease)
            {
                BGMManager.GetInstance.ReleaseBGM();
            }
        }
        
        private void MainLightFadeOut(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if (p_Type.IsInputRelease)
            {
                DirectionalLightController.GetInstance.SetMainLightColorLerp(Color.black, 1000, 1000);
            }
        }
        
        private void MainLightFadeIn(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if (p_Type.IsInputRelease)
            {
                DirectionalLightController.GetInstance.ResetMainLightColorLerp(1000, 1000);
            }
        }
        
        private void MainLightRotate(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if (p_Type.IsInputRelease)
            {
                DirectionalLightController.GetInstance.SetMainLightRotationSelfLerp(Vector2.up, 1260, 1000, 3000);
            }
        }
        
        private void MainLightRotateReset(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if (p_Type.IsInputRelease)
            {
                DirectionalLightController.GetInstance.ResetMainLightRotationSelfLerp(1000, 3000);
            }
        }
#endif

        #endregion
    }
}

#endif