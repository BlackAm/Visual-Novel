#if !SERVER_DRIVE
using UnityEngine.EventSystems;
using UnityEngine;
namespace BlackAm
{
    /// <summary>
    /// DraggableKeyCodeTouchEventSenderBaseWithGesture 구현체
    /// </summary>
    public class TouchEventDragButtonWorldMapWithGesture : DraggableKeyCodeTouchEventSenderBaseWithGesture, IDragHandler
    {
        #region <Callbacks>
        
        public override void OnSpawning()
        {
            base.OnSpawning();
            SetBaseOffUpdateType(BaseOffsetUpdateType.UpdateOnDrag);
            ControllerTracker?.SetControllerTrackerLifeSpan(ControllerTool.ControllerTrackerLifeSpanType.InitializeOnUpdate);
        }
        
        #endregion
    }
}
#endif