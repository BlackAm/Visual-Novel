using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using k514;
using UnityEngine;

namespace UI2020
{
    public partial class DialogueGameManager : SceneChangeEventSingleton<DialogueGameManager>
    {
        #region <Enum>

        [Flags]
        public enum DialogueEventFlag
        {
            None = 0,
            CharacterImage = 1 << 0,    // 캐릭터 이미지
            EventCG = 1 << 1,           // 이벤트CG
            BGM = 1 << 2,               // 배경음악
            SE = 1 << 3,                // 효과음
            SelectDialogue = 1 << 4,    // 선택지
            BackGroundImage = 1 << 5,   // 배경 이미지
            Fade = 1 << 6,              // Fade In, Out
            Liking = 1 << 7,            // 호감도
        }

        #endregion
        
        #region <Fields>

        public bool isDialogueEnd = true;
        public bool isDialogueSkipable = false;

        public string CurrentDialogueText = string.Empty;

        public int CurrentDialogueKey;
        public int CurrentSelectDialogueKey;
        public int _MaxIntervalCount;
        public int _CurrentInterval;
        public int SkipableInterval;
        
        private GameEventTimerHandlerWrapper _SpawnIntervalTimer;

        #endregion
        
        #region <CallBacks>

        protected override void OnCreated()
        {
            OnCreatedLiking();
        }

        public override void OnInitiate()
        {
        }

        public override async UniTask OnScenePreload()
        {
            await UniTask.CompletedTask;
        }

        public override void OnSceneStarted()
        {
        }

        public override void OnSceneTerminated()
        {
        }

        public override void OnSceneTransition()
        {
        }

        #endregion

        #region <CallBacks>

        private void OnDialogueOver()
        {
            isDialogueEnd = true;
            // TODO<BlackAm> - 대화 종료 애니메이션 제작

            if (CurrentSelectDialogueKey != 0)
                ActionDialogueEvent(DialogueEventFlag.SelectDialogue, CurrentSelectDialogueKey);
        }

        #endregion

        #region <Methods>

        public bool SetDialogue(int p_Key)
        {
            int dialogueEventValue = 0;

            if (!isDialogueEventEnd) return false;

            if (!isDialogueEnd || isDialogueSkipable)
            {
                FinishDialogueTimer();

                return true;
            }

            isDialogueEventEnd = false;
            CurrentDialogueKey = p_Key;

            var dialoguePresetData = DialoguePresetData.GetInstanceUnSafe[CurrentDialogueKey];
            if (ReferenceEquals(null, dialoguePresetData)) return false;
            
            var dialogueEventPreset = DialogueEventPresetData.GetInstanceUnSafe[dialoguePresetData.DialogueEventKey];
            if (ReferenceEquals(null, dialogueEventPreset)) return false;

            var dialogueEventCollection = dialogueEventPreset.DialogueEvent;

            if (dialogueEventCollection.ContainsKey(DialogueEventFlag.Fade))
            {
                dialogueEventValue = ActionDialogueEvent(DialogueEventFlag.Fade, dialogueEventCollection[DialogueEventFlag.Fade]);

                if (dialogueEventValue < 0) return false;
            }

            foreach (var dialogueEvent in dialogueEventCollection)
            {
                if (dialogueEvent.Key == DialogueEventFlag.Fade || dialogueEvent.Key == DialogueEventFlag.SelectDialogue) continue;

                dialogueEventValue = ActionDialogueEvent(dialogueEvent.Key, dialogueEvent.Value);
                if (dialogueEventValue < 0) return false;
            }

            if (dialogueEventCollection.ContainsKey(DialogueEventFlag.SelectDialogue))
                CurrentSelectDialogueKey = dialogueEventCollection[DialogueEventFlag.SelectDialogue];

            isDialogueEventEnd = true;
            
            ActionDialogue(dialoguePresetData.DialogueKey, dialoguePresetData.SkipableInterval, dialoguePresetData.PreDelay);
            SetTalkerName(LanguageManager.GetContent(dialoguePresetData.Talker));
            
            return true;
        }

        public int FinishDialogueTimer()
        {
            MainGameUI.Instance.mainUI.ResetDialogueText();
            
            MainGameUI.Instance.mainUI.UpdateDialogueText(CurrentDialogueText);
            isDialogueEnd = true;
            
            _SpawnIntervalTimer.CancelEvent();

            OnDialogueOver();

            return 1;
        }

        public void SetTalkerName(string p_Name)
        {
            MainGameUI.Instance.mainUI.UpdateNameText(p_Name);
        }

        public void ActionDialogue(int p_TextKey, int p_SkipableInterval, uint p_PreDelay)
        {
            CurrentDialogueText = LanguageManager.GetContent(p_TextKey);
            ResetDialogue(CurrentDialogueText.Length);
            _SpawnIntervalTimer.AddEvent(p_PreDelay, handler =>
            {
                handler.Arg1.UpdateDialogue(p_SkipableInterval);

                if (handler.Arg1.CheckDialogueOver())
                {
                    handler.Arg1.OnDialogueOver();
                }
                return true;
            }, handler => CheckDialogueOver(), this);
            
            _SpawnIntervalTimer.StartEvent();
        }

        public void ResetDialogue(int p_IntervalCount)
        {
            MainGameUI.Instance.mainUI.ResetDialogueText();
            CurrentSelectDialogueKey = 0;
            _CurrentInterval = 0;
            _MaxIntervalCount = p_IntervalCount;
        }

        public bool CheckDialogueOver()
        {
            return isDialogueEnd || (_CurrentInterval >= _MaxIntervalCount);
        }

        public void UpdateDialogue(int p_SkipableInterval)
        {
            MainGameUI.Instance.mainUI.UpdateDialogueText(CurrentDialogueText[_CurrentInterval]);
            _CurrentInterval++;

            SetSkipableDialogue(p_SkipableInterval);
        }

        public bool SetSkipableDialogue(int p_SkipableInterval)
        {
            isDialogueSkipable = CheckSkipableDialogue(p_SkipableInterval);

            return isDialogueSkipable;
        }
        
        public bool CheckSkipableDialogue(int p_SkipableInterval)
        {
            return p_SkipableInterval <= _CurrentInterval;
        }

        #endregion

    }
}