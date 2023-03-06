using System;

namespace k514
{
    public class PrefabEventSender : PropertyChangeEventSingleton<PrefabLoadEventSender, PrefabEventSender.UnityPrefabEventType, PrefabPoolingTool.PrefabIdentifyKey, PrefabEventSender>
    {
        #region <Enums>

        [Flags]
        public enum UnityPrefabEventType
        {
            None = 0,
            OnPrefabLoad = 1 << 0, 
            OnPrefabRelease = 1 << 1,
        }

        #endregion
        
        #region <Callbacks>

        protected override void OnCreated()
        {
            SingletonTool.CreateSingleton(typeof(PrefabLoadEventHandler));
        }

        public override void OnInitiate()
        {
        }

        public void OnPrefabKeyConfirmed(PrefabPoolingTool.PrefabIdentifyKey p_SpanwedKey)
        {
            if (p_SpanwedKey.IsUnitPrefab)
            {
                TriggerPropertyEvent(UnityPrefabEventType.OnPrefabLoad, p_SpanwedKey);
            }
        }

        public void OnPrefabKeyExpired(PrefabPoolingTool.PrefabIdentifyKey p_SpanwedKey)
        {
            if (p_SpanwedKey.IsUnitPrefab)
            {
                TriggerPropertyEvent(UnityPrefabEventType.OnPrefabRelease, p_SpanwedKey);
            }
        }

        #endregion

        #region <Methods>

        public PrefabLoadEventReceiver GetEventReceiver(Action<UnityPrefabEventType, PrefabPoolingTool.PrefabIdentifyKey> p_Event)
        {
            return GetEventReceiver<PrefabLoadEventReceiver>(UnityPrefabEventType.OnPrefabLoad | UnityPrefabEventType.OnPrefabRelease, p_Event);
        }

        #endregion
    }
}