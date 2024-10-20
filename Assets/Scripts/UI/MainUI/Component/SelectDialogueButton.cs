using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace BlackAm
{
    public class SelectDialogueButton : AbstractUI
    {
        public int NextDialogueKey;
        public Text DialogueText;
        public Button button;
        public DialogueGameManager dialogueGameManager;

        public override void Initialize()
        {
            DialogueText = GetComponent<Text>("DialogueText");
            button = GetComponent<Button>();
            button.onClick.AddListener(ActionDialogue);
            dialogueGameManager = DialogueGameManager.GetInstance;
        }

        public void ActionDialogue()
        {
            MainGameUI.Instance.mainUI.OnSelectDialogueClicked();
            dialogueGameManager.SetDialogue(NextDialogueKey);
        }

        public void SetNextDialogueKey(int p_DialogueKey)
        {
            NextDialogueKey = p_DialogueKey;
        }

        public void SetDialogueText(string p_Dialogue)
        {
            DialogueText.text = p_Dialogue;
        }

        public void SetButtonDisable(bool p_Interactable)
        {
            button.interactable = p_Interactable;
        }

        public bool CheckInteractableButton(int p_ConditionKey)
        {
            bool isInteractable = true;

            if (p_ConditionKey == 0) return isInteractable;
            var selectDialogueConditionCollection = SelectDialogueConditionPresetData.GetInstanceUnSafe[p_ConditionKey].SelectDialogueCondition;
            foreach (var selectDialogueCondition in selectDialogueConditionCollection)
            {
                isInteractable = CheckInteractableButton(selectDialogueCondition.Key, selectDialogueCondition.Value);

                if (!isInteractable) return isInteractable;
            }

            return isInteractable;
        }

        public bool CheckInteractableButton(DialogueGameManager.SelectDialogueCondition p_SelectDialogueCondition, int p_Key)
        {
            bool isInteractable = true;
            switch (p_SelectDialogueCondition)
            {
                case DialogueGameManager.SelectDialogueCondition.None:
                    return isInteractable;
                case DialogueGameManager.SelectDialogueCondition.Liking:
                    isInteractable = CheckLiking(p_Key);
                    break;
            }

            return isInteractable;
        }

        #region <Method/Liking>

        private bool CheckLiking(int p_Key)
        {
            var likingData = SelectDialogueConditionLikingPresetData.GetInstanceUnSafe[p_Key];
            bool isInteractable = true;

            switch (likingData.SelectDialogueLikingCondition)
            {
                case DialogueGameManager.SelectDialogueLikingCondition.OrMoreLiking:
                    isInteractable = CheckMoreLiking(likingData.LikingCollection, likingData.LikingExtraType);
                    break;
                case DialogueGameManager.SelectDialogueLikingCondition.OrLessLiking:
                    isInteractable = CheckLessLiking(likingData.LikingCollection, likingData.LikingExtraType);
                    break;
                case DialogueGameManager.SelectDialogueLikingCondition.OverLiking:
                    isInteractable = CheckOverLiking(likingData.LikingCollection, likingData.LikingExtraType);
                    break;
                case DialogueGameManager.SelectDialogueLikingCondition.UnderLiking:
                    isInteractable = CheckUnderLiking(likingData.LikingCollection, likingData.LikingExtraType);
                    break;
            }

            return isInteractable;
        }

        #endregion

        #region <Method/Liking/More>

        private bool CheckMoreLiking(Dictionary<Character, int> p_LikingCollection, DialogueGameManager.LikingExtraType p_LikingExtraType = DialogueGameManager.LikingExtraType.AND)
        {
            bool isInteractable = true;

            switch (p_LikingExtraType)
            {
                case DialogueGameManager.LikingExtraType.AND:
                    isInteractable = CheckMoreLikingAnd(p_LikingCollection, p_LikingExtraType);
                    break;
                case DialogueGameManager.LikingExtraType.OR:
                    isInteractable = CheckMoreLikingOr(p_LikingCollection, p_LikingExtraType);
                    break;
            }

            return isInteractable;
        }

        private bool CheckMoreLikingAnd(Dictionary<Character, int> p_LikingCollection, DialogueGameManager.LikingExtraType p_LikingExtraType = DialogueGameManager.LikingExtraType.AND)
        {
            bool isInteractable = true;

            foreach (var character in p_LikingCollection)
            {
                if (dialogueGameManager.currentDialogueEventData.Liking[character.Key] < character.Value)
                {
                    isInteractable = false;
                    break;
                }
            }
            
            return isInteractable;
        }

        private bool CheckMoreLikingOr(Dictionary<Character, int> p_LikingCollection, DialogueGameManager.LikingExtraType p_LikingExtraType = DialogueGameManager.LikingExtraType.AND)
        {
            bool isInteractable = false;

            foreach (var character in p_LikingCollection)
            {
                if (dialogueGameManager.currentDialogueEventData.Liking[character.Key] >= character.Value)
                {
                    isInteractable = true;
                    break;
                }
            }

            return isInteractable;
        }

        #endregion

        #region <Method/Liking/Less>

        private bool CheckLessLiking(Dictionary<Character, int> p_LikingCollection, DialogueGameManager.LikingExtraType p_LikingExtraType = DialogueGameManager.LikingExtraType.AND)
        {
            bool isInteractable = true;

            switch (p_LikingExtraType)
            {
                case DialogueGameManager.LikingExtraType.AND:
                    isInteractable = CheckLessLikingAnd(p_LikingCollection, p_LikingExtraType);
                    break;
                case DialogueGameManager.LikingExtraType.OR:
                    isInteractable = CheckLessLikingOr(p_LikingCollection, p_LikingExtraType);
                    break;
            }

            return isInteractable;
        }
        
        private bool CheckLessLikingAnd(Dictionary<Character, int> p_LikingCollection, DialogueGameManager.LikingExtraType p_LikingExtraType = DialogueGameManager.LikingExtraType.AND)
        {
            bool isInteractable = true;

            foreach (var character in p_LikingCollection)
            {
                if (dialogueGameManager.currentDialogueEventData.Liking[character.Key] > character.Value)
                {
                    isInteractable = false;
                    break;
                }
            }

            return isInteractable;
        }

        private bool CheckLessLikingOr(Dictionary<Character, int> p_LikingCollection, DialogueGameManager.LikingExtraType p_LikingExtraType = DialogueGameManager.LikingExtraType.AND)
        {
            bool isInteractable = false;

            foreach (var character in p_LikingCollection)
            {
                if (dialogueGameManager.currentDialogueEventData.Liking[character.Key] <= character.Value)
                {
                    isInteractable = true;
                    break;
                }
            }

            return isInteractable;
        }

        #endregion

        #region <Method/Liking/Over>

        private bool CheckOverLiking(Dictionary<Character, int> p_LikingCollection, DialogueGameManager.LikingExtraType p_LikingExtraType = DialogueGameManager.LikingExtraType.AND)
        {
            bool isInteractable = true;

            switch (p_LikingExtraType)
            {
                case DialogueGameManager.LikingExtraType.AND:
                    isInteractable = CheckOverLikingAnd(p_LikingCollection, p_LikingExtraType);
                    break;
                case DialogueGameManager.LikingExtraType.OR:
                    isInteractable = CheckOverLikingOr(p_LikingCollection, p_LikingExtraType);
                    break;
            }

            return isInteractable;
        }
        
        private bool CheckOverLikingAnd(Dictionary<Character, int> p_LikingCollection, DialogueGameManager.LikingExtraType p_LikingExtraType = DialogueGameManager.LikingExtraType.AND)
        {
            bool isInteractable = true;

            foreach (var character in p_LikingCollection)
            {
                if (dialogueGameManager.currentDialogueEventData.Liking[character.Key] <= character.Value)
                {
                    isInteractable = false;
                    break;
                }
            }

            return isInteractable;
        }

        private bool CheckOverLikingOr(Dictionary<Character, int> p_LikingCollection, DialogueGameManager.LikingExtraType p_LikingExtraType = DialogueGameManager.LikingExtraType.AND)
        {
            bool isInteractable = false;

            foreach (var character in p_LikingCollection)
            {
                if (dialogueGameManager.currentDialogueEventData.Liking[character.Key] > character.Value)
                {
                    isInteractable = true;
                    break;
                }
            }

            return isInteractable;
        }

        #endregion

        #region <Method/Liking/Under>

        private bool CheckUnderLiking(Dictionary<Character, int> p_LikingCollection, DialogueGameManager.LikingExtraType p_LikingExtraType = DialogueGameManager.LikingExtraType.AND)
        {
            bool isInteractable = true;

            switch (p_LikingExtraType)
            {
                case DialogueGameManager.LikingExtraType.AND:
                    isInteractable = CheckUnderLikingAnd(p_LikingCollection, p_LikingExtraType);
                    break;
                case DialogueGameManager.LikingExtraType.OR:
                    isInteractable = CheckUnderLikingOr(p_LikingCollection, p_LikingExtraType);
                    break;
            }

            return isInteractable;
        }
        
        private bool CheckUnderLikingAnd(Dictionary<Character, int> p_LikingCollection, DialogueGameManager.LikingExtraType p_LikingExtraType = DialogueGameManager.LikingExtraType.AND)
        {
            bool isInteractable = true;

            foreach (var character in p_LikingCollection)
            {
                if (dialogueGameManager.currentDialogueEventData.Liking[character.Key] >= character.Value)
                {
                    isInteractable = false;
                    break;
                }
            }

            return isInteractable;
        }

        private bool CheckUnderLikingOr(Dictionary<Character, int> p_LikingCollection, DialogueGameManager.LikingExtraType p_LikingExtraType = DialogueGameManager.LikingExtraType.AND)
        {
            bool isInteractable = false;

            foreach (var character in p_LikingCollection)
            {
                if (dialogueGameManager.currentDialogueEventData.Liking[character.Key] < character.Value)
                {
                    isInteractable = true;
                    break;
                }
            }

            return isInteractable;
        }

        #endregion
    }
}