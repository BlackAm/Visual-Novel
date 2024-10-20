using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackAm
{
    public partial class DialogueGameManager
    {
        #region <Enum>

        public enum DialogueEvent
        {
            None = 0,
            CharacterImage,    // 캐릭터 이미지
            EventCG,           // 이벤트CG
            BGM,               // 배경음악
            SE,                // 효과음
            SelectDialogue,    // 선택지
            BackGroundImage,   // 배경 이미지
            Fade,              // Fade In, Out
            Liking,            // 호감도
        }

        // 엔딩 관련 함수가 있으면 분리 시켜야 함
        [Flags]
        public enum DialogueEndingFlag
        {
            None,
        }

        #endregion
        
        #region <Fields>

        // 엔딩 관련 함수가 있으면 분리 시켜야 함
        public DialogueEndingFlag CurrentDialogueEndingFlag;

        public bool isDialogueEnd = true;
        public bool isDialogueSkipable = false;

        public string CurrentDialogueText = string.Empty;

        public int CurrentSceneKey;

        public int CurrentDialogueKey;
        public int CurrentSelectDialogueKey;

        public int NextDialogueKey;
        public int NextSceneKey;
        
        public int _MaxIntervalCount;
        public int _CurrentInterval;
        public int SkipableInterval;
        public uint _PreDelay;
        public uint _Interval = 70;


        #endregion

        #region <Callbacks>

        private bool OnDialogueStartTick()
        {
            UpdateDialogue();
            return true;
        }

        private void OnDialogueOver()
        {
            isDialogueEnd = true;
            isDialogueSkipable = false;
            // TODO<BlackAm> - 대화 종료 애니메이션 제작
            UpdateDialogueState(DialogueState.None);

            if (CurrentSelectDialogueKey != 0)
            {
                ActionDialogueEvent(DialogueEvent.SelectDialogue, CurrentSelectDialogueKey);
                CurrentSelectDialogueKey = 0;
            }
            
        }

        #endregion

        #region <Method>

        public void SetSceneKey(int p_Key)
        {
            CurrentSceneKey = p_Key;
        }

        public void StartScene()
        {
            var dialogueSceneInfo = DialogueSceneInfoData.GetInstanceUnSafe[CurrentSceneKey];
            var dialogueSceneExtraInfo = DialogueSceneExtraInfo.GetInstanceUnSafe[dialogueSceneInfo.SceneExtraInfo];
            
            // UI Setting
            SetDialogue(dialogueSceneExtraInfo.DialogueStartKey);
        }

        public void LoadFromTitle(int p_Key)
        {
            CurrentDialogueKey = p_Key;
        }

        public bool SetDialogue(int p_Key)
        {
            int dialogueEventValue = 0;
            
            if (!isDialogueEventEnd || MainGameUI.Instance.mainUI.IsUIActive(MainUI.UIList.TopCenter) || SaveLoad.Instance.IsActive) return false;
            
            if (!isDialogueEnd || isDialogueSkipable)
            {
                FinishDialogueTimer();

                return true;
            }
            
            UpdateDialogueState(DialogueState.DialogueAction);

            CurrentDialogueKey = p_Key;

            var dialoguePresetData = DialoguePresetData.GetInstanceUnSafe[CurrentDialogueKey];
            if (ReferenceEquals(null, dialoguePresetData)) return false;
            
            var dialogueEventPreset = DialogueEventPresetData.GetInstanceUnSafe[dialoguePresetData.DialogueEventKey];
            if (ReferenceEquals(null, dialogueEventPreset)) return false;
            
            var dialogueEventCollection = dialogueEventPreset.DialogueEvent;
            if (!ReferenceEquals(null, dialogueEventCollection))
            {
                if (dialogueEventCollection.ContainsKey(DialogueEvent.Fade))
                {
                    dialogueEventValue = ActionDialogueEvent(DialogueEvent.Fade, dialogueEventCollection[DialogueEvent.Fade]);

                    if (dialogueEventValue < 0) return false;
                }

                foreach (var dialogueEvent in dialogueEventCollection)
                {
                    if (dialogueEvent.Key == DialogueEvent.Fade || dialogueEvent.Key == DialogueEvent.SelectDialogue) continue;

                    dialogueEventValue = ActionDialogueEvent(dialogueEvent.Key, dialogueEvent.Value);
                    if (dialogueEventValue < 0) return false;
                }

                if (dialogueEventCollection.ContainsKey(DialogueEvent.SelectDialogue))
                    CurrentSelectDialogueKey = dialogueEventCollection[DialogueEvent.SelectDialogue];
            }
            
            SetTalkerName(LanguageManager.GetContent(dialoguePresetData.Talker));
            ActionDialogue(dialoguePresetData.DialogueKey, dialoguePresetData.SkipableInterval, dialoguePresetData.PreDelay);

            NextDialogueKey = dialoguePresetData.NextDialogueKey;
            NextSceneKey = dialoguePresetData.NextSceneKey;
            
            MainGameUI.Instance.functionUI.dialogueHistory.AddDialogueHistory(CurrentDialogueKey);

            UpdateReadDialogue(dialoguePresetData.DialogueKey);
            
            return true;
        }

        public void FinishDialogueTimer()
        {
            MainGameUI.Instance.mainUI.UpdateDialogueText(CurrentDialogueText);
            
            _SpawnIntervalTimer.CancelEvent();

            OnDialogueOver();
        }

        public void SetTalkerName(string p_Name)
        {
            MainGameUI.Instance.mainUI.UpdateNameText(p_Name);
        }

        public void ActionDialogue(int p_TextKey, int p_SkipableInterval, uint p_PreDelay)
        {
            CurrentDialogueText = LanguageManager.GetContent(p_TextKey);
            ResetDialogue(CurrentDialogueText.Length, p_SkipableInterval);
            SetPreDelay(p_PreDelay);
            SetDialogueTrigger();
        }

        public void ResetDialogue(int p_IntervalCount, int p_SkipableInterval)
        {
            MainGameUI.Instance.mainUI.ResetDialogueText();
            isDialogueEnd = false;
            _CurrentInterval = 0;
            _MaxIntervalCount = p_IntervalCount;
            SkipableInterval = p_SkipableInterval;
        }

        public void UpdateReadDialogue(int p_Key)
        {
            if (!SaveLoadManager.GetInstanceUnSafe.DialogueDataFile.ReadDialogueInfo[p_Key])
            {
                SaveLoadManager.GetInstanceUnSafe.DialogueDataFile.ReadDialogueInfo[p_Key] = true;
            }
        }
        
        public bool CheckDialogueOver()
        {
            return isDialogueEnd || (_CurrentInterval >= _MaxIntervalCount);
        }

        public void UpdateDialogue()
        {
            MainGameUI.Instance.mainUI.UpdateDialogueText(CurrentDialogueText[_CurrentInterval]);
            _CurrentInterval++;
            
            isDialogueSkipable = SetSkipableDialogue();
            
            if (CheckDialogueOver())
            {
                OnDialogueOver();
            }
        }

        public bool SetSkipableDialogue()
        {
            return CheckSkipableDialogue();
        }
        
        public bool CheckSkipableDialogue()
        {
            return SkipableInterval <= _CurrentInterval;
        }

        public void SetPreDelay(uint p_PreDelay)
        {
            _PreDelay = p_PreDelay;
        }

        public void SetInterval(uint p_Interval)
        {
            _Interval = p_Interval;
        }

        public void SetDialogueTrigger()
        {
            SystemBoot
                .GameEventTimer
                .RunTimer
                (
                    null,
                    (_PreDelay, _Interval), 
                    handler => handler.Arg1.OnDialogueStartTick(), 
                    handler => handler.Arg1.CheckDialogueOver(),
                    this
                );
        }

        public void ClearDialogue()
        {
            CurrentSelectDialogueKey = 0;
            FinishDialogueTimer();
            
            if (MainGameUI.Instance.mainUI.IsSelectDialogueExist())
                MainGameUI.Instance.mainUI.ActiveMainUI(MainUI.UIList.TopCenter, true);
        }

        #endregion
    }
}
