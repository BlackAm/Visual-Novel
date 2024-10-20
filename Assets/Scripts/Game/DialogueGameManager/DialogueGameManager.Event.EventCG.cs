using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlackAm
{
    public partial class DialogueGameManager
    {
        #region <Enum>

        public enum DialogueEventCG
        {
            None = 0,
            ChangeEventCG,  // EventCG 변경
            ResizeEventCG,  // EventCG 크기 변경
            MoveEventCGLerp // EventCG 이동
        }

        #endregion

        #region <Method>

        public int ActionEventCG(int p_DialogueEventKey)
        {
            var EventCGEvent = EventCGPresetData.GetInstanceUnSafe[p_DialogueEventKey].EventCGEvent;
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
                case DialogueEventCG.ResizeEventCG:
                    break;
                case DialogueEventCG.MoveEventCGLerp:
                    break;
                default: 
                    eventValue = -99;
                    break;
            }

            return eventValue;
        }

        public int ChangeEventCG(int p_Key)
        {
            var eventCGKey = ChangeEventCGPresetData.GetInstanceUnSafe[p_Key].EventCGKey;
            MainGameUI.Instance.mainUI.ChangeImage(ImageType.EventCG, eventCGKey);

            return eventCGKey;
        }

        /*public int ResizeEventCG(int p_Key)
        {
            var size = ResizeEventCGPresetData.GetInstanceUnSafe[p_Key].Size;
            MainGameUI.Instance.mainUI.ResizeImage(ImageType.EventCG, size);
            return p_Key;
        }

        public int MoveEventCGLerp(int p_Key)
        {
            if (MOveEventCGLerpPresetData.GetInstanceUnSafe.HasKey(p_Key))
            {
                
            }
        }*/

        #endregion
    }
}
    