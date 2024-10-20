using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackAm
{
    public partial class MainUI
    {
        private const string _DialogueFaderPanelName = "DialogueFader";
        
        protected UI_FadePanel _Fader;

        public void Initialize_Fader()
        {
            var (faderValid, faderAffine) = transform.FindRecursive(_DialogueFaderPanelName);
            if (faderValid)
            {
                _Fader = faderAffine.gameObject.AddComponent<UI_FadePanel>();
                _Fader.CheckAwake();
                _Fader.Set_UI_Hide(true);
            }
        }

        public void OnFadeOver()
        {
            SetDialogueEventEnd(true);
        }

        private void UpdateFader(float p_DeltaTime)
        {
            if (!ReferenceEquals(null, _Fader))
            {
                _Fader.OnUpdateUI(p_DeltaTime);
            }
        }

        public void SetFadeInUI(float p_PreDelay, float p_FadeTime)
        {
            _Fader.SetFadeTime(p_FadeTime);
            _Fader.CastEscapeAnimation(OnFadeOver, p_PreDelay);
            SetDialogueFadeState(true);
        }

        public void SetFadeOutUI(float p_PreDelay, float p_FadeTime)
        {
            _Fader.SetFadeTime(p_FadeTime);
            _Fader.CastEntryAnimation(OnFadeOver, p_PreDelay);
            SetDialogueFadeState(false);
        }

        public void SetDialogueFadeState(bool p_Flag)
        {
            DialogueGameManager.GetInstance.currentDialogueEventData.FadeActivated = p_Flag;
        }

        public void SetFadeOn()
        {
            _Fader.OnEntryAnimationBegin();
            SetDialogueFadeState(true);
        }

        public void SetFadeOff()
        {
            _Fader.OnEscapeAnimationBegin();
            SetDialogueFadeState(false);
        }
    }
}