#if !SERVER_DRIVE
namespace BlackAm
{
    public class TestController : TouchEventSenderCluster
    {
        #region <Fields>

        #endregion

        #region <Callbacks>

        public override void OnSpawning()
        {
            base.OnSpawning();
            
            RegistKeyCodeInput<TouchEventButton>("Z", TouchEventRoot.TouchMappingKeyCodeType.UnitSpell, 0, ControllerTool.InputEventType.ControlUnit);
            RegistKeyCodeInput<TouchEventButton>("X", TouchEventRoot.TouchMappingKeyCodeType.UnitSpell, 1, ControllerTool.InputEventType.ControlUnit);
            RegistKeyCodeInput<TouchEventButton>("V", TouchEventRoot.TouchMappingKeyCodeType.UnitSpell, 2, ControllerTool.InputEventType.ControlUnit);
            RegistKeyCodeInput<TouchEventButton>("B", TouchEventRoot.TouchMappingKeyCodeType.UnitSpell, 3, ControllerTool.InputEventType.ControlUnit);
            RegistKeyCodeInput<TouchEventButton>("LC", TouchEventRoot.TouchMappingKeyCodeType.UnitSpell, 4, ControllerTool.InputEventType.ControlUnit);
            RegistKeyCodeInput<TouchEventButton>("LS", TouchEventRoot.TouchMappingKeyCodeType.UnitSpell, 5, ControllerTool.InputEventType.ControlUnit);
            RegistKeyCodeInput<TouchEventButton>("SPACE", TouchEventRoot.TouchMappingKeyCodeType.UnitSpell, 6, ControllerTool.InputEventType.ControlUnit);
            RegistKeyCodeInput<TouchEventButton>("RS", TouchEventRoot.TouchMappingKeyCodeType.UnitSpell, 7, ControllerTool.InputEventType.ControlUnit);
            
            RegistKeyCodeInput<TouchEventJoystickButton>("Joy", TouchEventRoot.TouchMappingKeyCodeType.UnitControl, 0, ControllerTool.InputEventType.ControlUnit);
            RegistKeyCodeInput<TouchEventButton>("C", TouchEventRoot.TouchMappingKeyCodeType.UnitControl, 1, ControllerTool.InputEventType.ControlUnit);
        }

        #endregion
    }
}
#endif