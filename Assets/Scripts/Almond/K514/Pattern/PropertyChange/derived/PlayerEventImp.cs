#if !SERVER_DRIVE
using System;

namespace k514
{
    public class PlayerChangeEventSender : PropertyModifyEventSenderImp<PlayerManager.PlayerChangeEventType, Unit>
    {
        public override bool HasEvent(PlayerManager.PlayerChangeEventType p_Type, PlayerManager.PlayerChangeEventType p_Compare)
        {
            return p_Type.HasAnyFlagExceptNone(p_Compare);
        }
    }
    
    public class PlayerChangeEventReceiver : PropertyModifyEventReceiverImp<PlayerManager.PlayerChangeEventType, Unit>
    {
        #region <Constructor>

        public PlayerChangeEventReceiver()
        {
        }

        public PlayerChangeEventReceiver(PlayerManager.PlayerChangeEventType p_EventType,
            Action<PlayerManager.PlayerChangeEventType, Unit> p_EventHandler) : base(p_EventType, p_EventHandler)
        {
        }

        #endregion
    }
}
#endif