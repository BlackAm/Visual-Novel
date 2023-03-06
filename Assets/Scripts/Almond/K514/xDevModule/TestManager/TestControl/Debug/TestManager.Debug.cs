#if UNITY_EDITOR && !SERVER_DRIVE && ON_GUI

using UnityEngine;

namespace k514
{
    public partial class TestManager
    {
        #region <Fields>

        private UIBaseManagerPreset _DebugPanelUIPreset;
        private UI_DebugPanel _DebugPanel;
        private bool _DebugPanelDeplayFlag;
        
        #endregion

        #region <Callbacks>

        void OnAwakeDebugPanel()
        {
            var targetControlType = TestControlType.DebugPanel;
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.Q, ActDebugPanel, "디버그 UI 활성화/비활성화");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.W, DeployDebugPanel, "디버그 UI 배치변경");
        }

        #endregion

        #region <Methods>

        private async void ActDebugPanel(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if (p_Type.IsInputRelease)
            {
                if (_DebugPanelUIPreset.IsValid)
                {
                    _DebugPanel.Toggle_UI_Hide();
                }
                else
                {
                    _DebugPanelUIPreset = await (await UICustomRoot.GetInstance())
                        .Add_UI_Manager(RenderMode.ScreenSpaceOverlay, 10, UICustomRoot.UIManagerType.TestDebugPanel);
                    _DebugPanel = _DebugPanelUIPreset.UIManagerBase as UI_DebugPanel;
                }
            }
        }
        
        private void DeployDebugPanel(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if (p_Type.IsInputRelease)
            {
                if (!_DebugPanelUIPreset.IsValid)
                {
                    ActDebugPanel(p_Type, p_TryValue);
                }

                _DebugPanel.Set_UI_Hide(false);
                
                if (_DebugPanelDeplayFlag)
                {
                    _DebugPanel._RectTransform.SetPivotPreset(UITool.XAnchorType.Right, UITool.YAnchorType.Top);
                }
                else
                {
                    _DebugPanel._RectTransform.SetPivotPreset(UITool.XAnchorType.Left, UITool.YAnchorType.Top);
                }

                _DebugPanelDeplayFlag = !_DebugPanelDeplayFlag;
            }
        }

        private void PositionDebuging(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue){
            if (p_Type.IsInputRelease)
            {
                Vector3 playerPos = LamiereGameManager.GetInstanceUnSafe._ClientPlayer.transform.position;
                Vector3 worldMapPos = UI2020.MainGameUI.Instance.mainUI.worldMapUI.GetWorldMapPosition();
                Vector3 miniMapImagePos = UI2020.MainGameUI.Instance.mainUI._miniMapImage.rectTransform.localPosition;
                Debug.Log($"플레이어 위치 : {playerPos} / 월드맵 이미지 위치 : {worldMapPos} / 미니맵 이미지 위치 : {miniMapImagePos}");
            }
        }

        private void QualityDebuging(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue){
            if (p_Type.IsInputRelease)
            {
                
                Debug.Log($"[퀄리티 보고] masterTextureLimit : {QualitySettings.masterTextureLimit} / maxQueuedFrames : {QualitySettings.maxQueuedFrames} / anisotropic : {QualitySettings.anisotropicFiltering} / antiAliasing : {QualitySettings.antiAliasing}");
                Debug.Log($"프레임 : {Application.targetFrameRate}");
            }
        }
        #endregion
    }
}

#endif