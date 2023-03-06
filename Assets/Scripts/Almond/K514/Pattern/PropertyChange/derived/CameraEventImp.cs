#if !SERVER_DRIVE
using System;

namespace k514
{
    public class CameraEventSender : PropertyModifyEventSenderImp<CameraManager.CameraEventType, CameraEventMessage>
    {
        public override bool HasEvent(CameraManager.CameraEventType p_Type, CameraManager.CameraEventType p_Compare)
        {
            return p_Type.HasAnyFlagExceptNone(p_Compare);
        }
    }
    
    public class CameraEventReceiver : PropertyModifyEventReceiverImp<CameraManager.CameraEventType, CameraEventMessage>
    {
        #region <Constructor>

        public CameraEventReceiver()
        {
        }

        public CameraEventReceiver(CameraManager.CameraEventType p_EventType,
            Action<CameraManager.CameraEventType, CameraEventMessage> p_EventHandler) : base(p_EventType, p_EventHandler)
        {
        }

        #endregion
    }

    public struct CameraEventMessage
    {
    }
}
#endif