using System;
using System.Collections.Generic;
using BDG;
using k514;
using UnityEngine;
using UnityEngine.UI;

namespace UI2020
{
    public partial class DialogueGameManager
    {

        #region <Enum>

        public enum DialogueEventCG
        {
            None = 0,
            ChangeEventCG,  // EventCG 변경
            HideEventCG,    // EventCG 숨기기
        }

        #endregion

        #region <Method>

        public int ActionEventCG(int p_DialogueEventKey)
        {
            var EventCGEvent = EventCGPresetData.GetInstanceUnSafe[p_DialogueEventKey].EvebtCGEvent;
            int eventValue = 0;

            foreach (var eventCG in EventCGEvent)
            {
                eventValue = ActionEventCG(eventCG.Key, eventCG.Value);

                if (eventValue < 0) return eventValue;
            }

            return eventValue;
        }

        public int ActionEventCG(DialogueEventCG p_EventCGEvent, int p_Key)
        {
            int eventValue = 0;
            switch (p_EventCGEvent)
            {
                case DialogueEventCG.ChangeEventCG:
                    eventValue = ChangeEventCG(p_Key);
                    break;
                case DialogueEventCG.HideEventCG:
                    eventValue = ChangeEventCG(0);
                    break;
                default: 
                    eventValue = -99;
                    break;
            }

            return eventValue;
        }

        public int ChangeEventCG(int p_Key)
        {
            var EventCGKey = ChangeEventCGPresetData.GetInstanceUnSafe[p_Key].EventCGKey;
            MainGameUI.Instance.functionUI.ChangeImage(ImageType.EventCG, EventCGKey);

            return EventCGKey;
        }

        #endregion
    }
}
    