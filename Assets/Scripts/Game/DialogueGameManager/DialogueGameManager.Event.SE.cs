using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlackAm
{
    public partial class DialogueGameManager
    {
        
        #region <Enum>

        public enum DialogueEventSE
        {
            None = 0,
            PlaySE,     // SE 재생
            StopSE,     // SE 멈춤
        }

        #endregion

        #region <Method>

        public int ActionSE(int p_DialogueEventKey)
        {
            var SEEvent = DialogueSEPresetData.GetInstanceUnSafe[p_DialogueEventKey].SEEvent;
            int eventValue = 0;

            foreach (var seEvent in SEEvent)
            {
                eventValue = ActionSE(seEvent.Key, seEvent.Value);

                if (eventValue < 0) return eventValue;
            }

            return eventValue;
        }

        public int ActionSE(DialogueEventSE p_SEEvent, int p_Key)
        {
            int eventValue = 0;
            switch (p_SEEvent)
            {
                case DialogueEventSE.PlaySE:
                    eventValue = PlaySE(p_Key);
                    break;
                case DialogueEventSE.StopSE:
                    eventValue = StopSE(p_Key);
                    break;
                default: eventValue = -99;
                    break;
            }

            return eventValue;
        }

        public int PlaySE(int p_Key)
        {
            SfxSpawnManager.GetInstance.GetSfx(p_Key, default, Vector3.zero);
            SetDialogueEventEnd(true);
            return p_Key;
        }

        public int StopSE(int p_Key)
        {
            AudioManager.GetInstance.StopAllCurrentPlayingSfxUnit();
            SetDialogueEventEnd(true);
            return p_Key;
        }

        #endregion

    }
}