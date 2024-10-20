using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlackAm
{
    public partial class DialogueGameManager
    {
        #region <Enum>

        public enum DialogueEventBGM
        {
            None = 0,
            ChangeBGM,  // BGM 변경
            PlayBGM,    // BGM 재생
            PauseBGM,    // BGM 멈춤
        }

        #endregion

        #region <Method>

        public int ActionBGM(int p_Key)
        {
            var BGMEvent = DialogueBGMPresetData.GetInstanceUnSafe[p_Key].BGMEvent;

            int EventValue = 0;

            foreach (var bgmEvent in BGMEvent)
            {
                EventValue = ActionBGM(bgmEvent.Key, bgmEvent.Value);

                if (EventValue < 0) return EventValue;
            }

            return EventValue;
        }

        public int ActionBGM(DialogueEventBGM p_BGMEvent, int p_Key)
        {
            int eventValue = 0;
            switch (p_BGMEvent)
            {
                case DialogueEventBGM.ChangeBGM:
                    eventValue = ChangeBGM(p_Key);
                    break;
                case DialogueEventBGM.PlayBGM:
                    eventValue = PlayBGM(p_Key);
                    break;
                case DialogueEventBGM.PauseBGM:
                    eventValue = PauseBGM(p_Key);
                    break;
                default: eventValue = -99;
                    break;
            }

            return eventValue;
        }

        public int ChangeBGM(int p_Key)
        {
            BGMManager.GetInstance.SetBGM(p_Key, false, false);
            currentDialogueEventData.BGM = p_Key;
            return p_Key;
        }

        public int PlayBGM(int p_Key)
        {
            BGMManager.GetInstance.PlayBGM(true);
            return p_Key;
        }

        public int PauseBGM(int p_Key)
        {
            BGMManager.GetInstance.PauseBGM();
            return p_Key;
        }

        #endregion

    }
}