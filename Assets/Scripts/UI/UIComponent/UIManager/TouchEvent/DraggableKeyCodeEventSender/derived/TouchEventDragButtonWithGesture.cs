#if !SERVER_DRIVE
namespace BlackAm
{
    /// <summary>
    /// DraggableKeyCodeTouchEventSenderBaseWithGesture 구현체
    /// </summary>
    public class TouchEventDragButtonWithGesture : DraggableKeyCodeTouchEventSenderBaseWithGesture
    {
        #region <Callbacks>

        public override void OnSpawning()
        {
            base.OnSpawning();
            SetBaseOffUpdateType(BaseOffsetUpdateType.UpdateOnDrag);
            ControllerTracker?.SetControllerTrackerLifeSpan(ControllerTool.ControllerTrackerLifeSpanType.InitializeOnUpdate);
            TouchEventFlagMask.AddFlag(TouchEventRoot.TouchInputType.UnitClickEvent);
        }

        #endregion
    }
}
#endif