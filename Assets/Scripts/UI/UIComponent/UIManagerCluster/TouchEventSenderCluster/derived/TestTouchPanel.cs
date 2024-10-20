#if !SERVER_DRIVE
namespace BlackAm
{
    public class TestTouchPanel : TouchEventSenderCluster
    {
        #region <Fields>

        #endregion

        #region <Callbacks>

        public override void OnSpawning()
        {
            base.OnSpawning();
            RegistKeyCodeInput<TouchEventDragButtonWithGesture>("TouchPanel", TouchEventRoot.TouchMappingKeyCodeType.ViewControl, 0, ControllerTool.InputEventType.ControlView);
        }

        #endregion
    }
}
#endif