using System;

namespace BlackAm
{
    public class PrefabLoadEventSender : PropertyModifyEventSenderImp<PrefabEventSender.UnityPrefabEventType, PrefabPoolingTool.PrefabIdentifyKey>
    {
        public override bool HasEvent(PrefabEventSender.UnityPrefabEventType p_Type, PrefabEventSender.UnityPrefabEventType p_Compare)
        {
            return p_Type.HasAnyFlagExceptNone(p_Compare);
        }
    }
    
    public class PrefabLoadEventReceiver : PropertyModifyEventReceiverImp<PrefabEventSender.UnityPrefabEventType, PrefabPoolingTool.PrefabIdentifyKey>
    {
        #region <Constructor>

        public PrefabLoadEventReceiver() : base()
        {
        }

        public PrefabLoadEventReceiver(PrefabEventSender.UnityPrefabEventType p_EventType,
            Action<PrefabEventSender.UnityPrefabEventType, PrefabPoolingTool.PrefabIdentifyKey> p_EventHandler) : base(p_EventType, p_EventHandler)
        {
        }

        #endregion
    }
}