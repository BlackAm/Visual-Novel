#if !SERVER_DRIVE
using System;

namespace BlackAm
{
    public class TouchEventSender : PropertyModifyEventSenderImp<TouchEventRoot.TouchEventType, TouchEventManager.TouchEventPreset>
    {
        public override bool HasEvent(TouchEventRoot.TouchEventType p_Type, TouchEventRoot.TouchEventType p_Compare)
        {
            return p_Type.HasAnyFlagExceptNone(p_Compare);
        }
    }
    
    public class TouchEventReceiver : PropertyModifyEventReceiverImp<TouchEventRoot.TouchEventType, TouchEventManager.TouchEventPreset>
    {
        #region <Constructor>

        public TouchEventReceiver() : base()
        {
        }

        public TouchEventReceiver(TouchEventRoot.TouchEventType p_EventType,
            Action<TouchEventRoot.TouchEventType, TouchEventManager.TouchEventPreset> p_EventHandler) : base(p_EventType, p_EventHandler)
        {
        }

        #endregion
    }
}
#endif