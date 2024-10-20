#if !SERVER_DRIVE
namespace BlackAm
{
    /// <summary>
    /// DraggableKeyCodeTouchEventSenderBaseWithHandle 구현체
    /// </summary>
    public class TouchEventJoystickButton : DraggableKeyCodeTouchEventSenderBaseWithHandle
    {
        #region <Callbacks>

        public override string GetHandlePivotName()
        {
            return "Pivot";
        }
     
        #endregion
    }
}
#endif