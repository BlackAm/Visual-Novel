using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlackAm
{
    public partial class DialogueGameManager
    {
        #region <Enum>

        public enum DialogueEventFade
        {
            None = 0,
            FadeIn,     // 페이드 인
            FadeOut,    // 페이드 아웃
        }

        #endregion

        #region <Method>

        public int ActionFade(int p_Key)
        {
            var FadeEvent = DialogueFadePresetData.GetInstanceUnSafe[p_Key].FadeEvent;
            
            int eventValue = 0;
            foreach (var fadeEvent in FadeEvent)
            {
                eventValue = ActionFade(fadeEvent.Key, fadeEvent.Value);
                
                if (eventValue < 0) return eventValue;
            }
            
            
            return eventValue;
        }

        public int ActionFade(DialogueEventFade p_FadeEvent, int p_Key)
        {
            int eventValue = 0;
            switch (p_FadeEvent)
            {
                case DialogueEventFade.FadeIn:
                    eventValue = FadeIn(p_Key);
                    break;
                case DialogueEventFade.FadeOut:
                    eventValue = FadeOut(p_Key);
                    break;
                default:
                    eventValue = -99;
                    break;
            }

            return eventValue;
        }

        public int FadeIn(int p_Key)
        {
            if (currentDialogueEventData.FadeActivated)
            {
                MainGameUI.Instance.mainUI.OnFadeOver();
                return 0;
            }
            
            var fadeEvent = DialogueFadeInPresetData.GetInstanceUnSafe[p_Key];
            MainGameUI.Instance.mainUI.SetFadeInUI(fadeEvent.PreDelay, fadeEvent.FadeTime);
            return p_Key;
        }

        public int FadeOut(int p_Key)
        {
            if (!currentDialogueEventData.FadeActivated)
            {
                MainGameUI.Instance.mainUI.OnFadeOver();
                return 0;
            }
            
            var fadeEvent = DialogueFadeOutPresetData.GetInstanceUnSafe[p_Key];
            MainGameUI.Instance.mainUI.SetFadeOutUI(fadeEvent.PreDelay, fadeEvent.FadeTime);
            return p_Key;
        }

        #endregion

    }
}