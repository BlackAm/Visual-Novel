
using UI2020;
#if UNITY_EDITOR && ON_GUI
using UnityEngine;

namespace k514
{
    public partial class TestManager
    {
        #region <Fields>

        private int uiCount;
#if !SERVER_DRIVE
        private UIBaseManagerPreset _CutIn_AnimationManagerPreset;
#endif

        #endregion

        #region <Callbacks>

        void OnAwakeCutin()
        {
            var targetControlType = TestControlType.UITest;
            //BindKeyTestEvent(targetControlType, ControllerTool.CommandType.Q, CastUIAnimation, "컷인 UI 생성(중앙 세로)");
            //BindKeyTestEvent(targetControlType, ControllerTool.CommandType.W, CastUIAnimation, "컷인 UI 생성(중앙 가로)");
#if !SERVER_DRIVE
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.Q, SetExp100, "Exp 100%");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.W, SetExp0, "Exp 0%");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.E, SetHPMP100, "HPMP 100%");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.R, SetHPMP0, "HPMP 0%");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.T, HitPlayer, "HitPlayer");
#endif
        }

        #endregion

        #region <Methods>

        //private void CastUIAnimation(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        //{
        //    var functionType = p_Type.CommandType;
        //    if (p_Type.IsInputRelease)
        //    {
        //        switch (functionType)
        //        {
        //            case ControllerTool.CommandType.Q :
        //                _CutIn_AnimationManager.Cast_CutIn_UI
        //                (
        //                    CutIn_AnimationManager.Cut_UI_Animation_Type.TestVertical,
        //                    Cut_In_Animation.CutIn_Animation_Type.Vertical_Spread_Out,
        //                    1f, _uiAnimationPreset
        //                );
        //                break;
        //            case ControllerTool.CommandType.W :
        //                _CutIn_AnimationManager.Cast_CutIn_UI
        //                (
        //                    CutIn_AnimationManager.Cut_UI_Animation_Type.TestHorizontal_Green,
        //                    Cut_In_Animation.CutIn_Animation_Type.Vertical_Spread_Out,
        //                    1f, _uiAnimationPreset
        //                );
        //                break;
        //        }
        //    }
        //}

#if !SERVER_DRIVE
        private void SetExp100(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            var functionType = p_Type.CommandType;
            if (p_Type.IsInputRelease)
            {
                MainGameUI.Instance.mainUI.expBar.SetExp(1f);
            }
        }

        private void SetExp0(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            var functionType = p_Type.CommandType;
            if (p_Type.IsInputRelease)
            {
                MainGameUI.Instance.mainUI.expBar.SetExp(0f);
            }
        }

        private void SetHPMP100(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            var functionType = p_Type.CommandType;
            if (p_Type.IsInputRelease)
            {
                MainGameUI.Instance.mainUI.SetHp(1000, 1000);
                MainGameUI.Instance.mainUI.SetMp(1000, 1000);
            }
        }

        private void SetHPMP0(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            var functionType = p_Type.CommandType;
            if (p_Type.IsInputRelease)
            {
                MainGameUI.Instance.mainUI.SetHp(0, 1000);
                MainGameUI.Instance.mainUI.SetMp(0, 1000);
            }
        }

        private void HitPlayer(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            var functionType = p_Type.CommandType;
            if (p_Type.IsInputRelease)
            {
                PlayerManager.GetInstance.Player?.RemoveState(Unit.UnitStateType.IMMORTAL);
            }
        }
#endif

        #endregion
    }
}
#endif