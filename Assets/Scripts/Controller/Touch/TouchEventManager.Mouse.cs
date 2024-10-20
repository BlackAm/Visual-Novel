using UnityEngine.EventSystems;

namespace BlackAm
{
    public partial class TouchEventManager
    {
        public void OnDialogueActioned(PointerEventData p_EventData)
        {
            var eventPreset = new TouchEventPreset(p_EventData);
            TriggerPropertyEvent(TouchEventRoot.TouchEventType.DialogueActioned, eventPreset);
        }
    }
}