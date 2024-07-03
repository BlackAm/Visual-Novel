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
            var playSEEvent = PlaySEPresetData.GetInstanceUnSafe[p_Key];
            if (!ReferenceEquals(null, playSEEvent))
                SfxSpawnManager.GetInstance.GetSfx(playSEEvent.SEKey, default,
                    LamiereGameManager.GetInstanceUnSafe._ClientPlayer);
            else return -1;
            
            // TODO<BlackAm> - 사운드 소환 위치를 없애야함, SE가 여러개 있을 가능성 있음

            return playSEEvent.SEKey;
        }

        public int StopSE(int p_Key)
        {
            var stopSEEvent = StopSEPresetData.GetInstanceUnSafe[p_Key];
            if (!ReferenceEquals(null, stopSEEvent)) AudioManager.GetInstance.StopAllCurrentPlayingSfxUnit();
            else return -1;

            return p_Key;
        }

        #endregion

    }
}