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
        #region <enum>

        public enum DialogueEventSelectDialogue
        {
            None,
            ShowSelectDialogue
        }

        public enum SelectDialogueCondition
        {
            None,
            Liking
        }

        public enum SelectDialogueLikingCondition
        {
            None,
            OrMoreLiking,   // 호감도 이상
            OrLessLiking,   // 호감도 이하
            OverLiking,     // 호감도 초과
            UnderLiking,    // 호감도 미만
        }

        public enum LikingExtraType
        {
            None,
            AND,
            OR,
        }

        #endregion
        
        #region <Method>

        public int ActionSelectDialogue(int p_Key)
        {
            var SelectDialogueEvent = SelectDialoguePresetData.GetInstanceUnSafe[p_Key].SelectDialogueEvent;
            
            int EventValue = 0;
            foreach (var selectDialogueEvent in SelectDialogueEvent)
            {
                EventValue = ActionSelectDialogue(selectDialogueEvent.Key, selectDialogueEvent.Value);

                if (EventValue < 0) return EventValue;
            }

            return EventValue;
        }

        public int ActionSelectDialogue(DialogueEventSelectDialogue p_SelectDialogueEvent, int p_Key)
        {
            int EventValue = 0;

            switch (p_SelectDialogueEvent)
            {
                case DialogueEventSelectDialogue.ShowSelectDialogue:
                    EventValue = ShowSelectDialogue(p_Key);
                    break;
                default:
                    EventValue = -99;
                    break;
            }

            return EventValue;
        }

        public int ShowSelectDialogue(int p_Key)
        {
            var selectDialogueInfoList = ShowSelectDialoguePresetData.GetInstanceUnSafe[p_Key].SelectDialogueInfo;

            foreach (var selectDialogueInfo in selectDialogueInfoList)
            {
                AddSelectDialogue(selectDialogueInfo);
            }

            return p_Key;
        }

        public int AddSelectDialogue(int p_Key)
        {
            var selectDialogueInfo = SelectDialogueInfoData.GetInstanceUnSafe[p_Key];
            
            MainGameUI.Instance.mainUI.AddSelectDialogue(selectDialogueInfo.NextDialogueKey, selectDialogueInfo.DialogueTextKey, selectDialogueInfo.SelectDialogueConditionKey);

            return p_Key;
        }

        #endregion
    }
}