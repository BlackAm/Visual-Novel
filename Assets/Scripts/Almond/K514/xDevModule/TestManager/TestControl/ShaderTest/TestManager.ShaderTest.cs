#if UNITY_EDITOR && ON_GUI
using UnityEngine;

namespace k514
{
    public partial class TestManager
    {
        #region <Fields>


        #endregion

        #region <Callbacks>

        void OnAwakeShaderTest()
        {
            var targetControlType = TestControlType.ShaderTest;
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.Q, EradTest, "발광테스트");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.W, DissolveTest, "분해테스트");
        }

        #endregion

        #region <Methods>

        private void EradTest(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if (p_Type.IsInputRelease)
            {
#if !SERVER_DRIVE
                if (PlayerManager.GetInstance.Player.IsValid())
                {
                    PlayerManager.GetInstance.Player._RenderObject.SetRenderEffect(RenderableTool.ShaderControlType.Intensity, 5);
                }
#endif
            }
        }
        
        private void DissolveTest(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if (p_Type.IsInputRelease)
            {
#if !SERVER_DRIVE
                if (PlayerManager.GetInstance.Player.IsValid())
                {
                    PlayerManager.GetInstance.Player._RenderObject.OnUnitDead(false);
                }
#endif
            }
        }

        #endregion
    }
}
#endif