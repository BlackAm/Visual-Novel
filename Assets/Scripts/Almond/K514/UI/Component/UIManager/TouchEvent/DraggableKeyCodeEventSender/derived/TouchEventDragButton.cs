#if !SERVER_DRIVE
namespace k514
{
    /// <summary>
    /// DraggableKeyCodeTouchEventSenderBase 구현체
    /// </summary>
    public class TouchEventDragButton : DraggableKeyCodeTouchEventSenderBase
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