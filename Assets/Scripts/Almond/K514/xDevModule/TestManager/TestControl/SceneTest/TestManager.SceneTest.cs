#if UNITY_EDITOR && ON_GUI
using UnityEngine;

namespace k514
{
    public partial class TestManager
    {
        #region <Fields>


        #endregion

        #region <Callbacks>

        void OnAwakeSceneTest()
        {
            var targetControlType = TestControlType.SceneControlTest;
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.Q, SceneControl, "씬 전이α", "마을 씬으로 전이");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.W, SceneControl, "씬 전이β", "던전 씬으로 전이");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.E, SceneControl, "씬 전이β2", "던전 씬으로 전이");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.R, SceneControl, "씬 전이 던전1", "던전 씬으로 전이");
            
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.A, SceneControl, "씬 전이 던전2", "던전 씬으로 전이");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.S, SceneControl, "씬 전이 던전3", "던전 씬으로 전이");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.D, SceneControl, "씬 전이 던전4", "던전 씬으로 전이");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.F, SceneControl, "프로모션 던전", "시연용 씬으로 전이");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.G, SceneControl, "마을", "마을 씬으로 전이");
        }

        #endregion

        #region <Methods>

        private void SceneControl(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            var functionType = p_Type.CommandType;
            if (p_Type.IsInputRelease)
            {
                switch (functionType)
                {
                    case ControllerTool.CommandType.Q:
                        break;
                    case ControllerTool.CommandType.W:
                        SceneControllerManager.GetInstance.TurnDungeonTo(DungeonDataRoot.DungeonDataType.Test0, 0);
                        break;
                    case ControllerTool.CommandType.E:
                        SceneControllerManager.GetInstance.TurnDungeonTo(DungeonDataRoot.DungeonDataType.Test1, 0);
                        break;
                    case ControllerTool.CommandType.F:
                        SceneControllerManager.GetInstance.TurnSceneTo(514);
                        break;
                    case ControllerTool.CommandType.G:
                        SceneControllerManager.GetInstance.TurnSceneTo(400);
                        break;
                }
            }
        }

        #endregion
    }
}
#endif