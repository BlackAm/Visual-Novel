#if !SERVER_DRIVE
namespace k514
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