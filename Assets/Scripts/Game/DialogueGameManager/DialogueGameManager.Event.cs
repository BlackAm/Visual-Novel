using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackAm
{
    public partial class DialogueGameManager
    {
        #region <Fields>

        public SaveLoadManager.SaveData.DialogueEventData currentDialogueEventData;

        public bool isDialogueEventEnd;

        #endregion

        #region <Callbacks>

        public void OnInitiateDialogueEvent()
        {
            OnInitiateBackGroundImage();
        }

        #endregion
        
        #region <Method>

        public int ActionDialogueEvent(DialogueEvent p_DialogueEvent, int p_Key)
        {
            int dialogueEventValue = 0;
            SetDialogueEventEnd(false);
            switch (p_DialogueEvent)
            {
                case DialogueEvent.CharacterImage:
                    dialogueEventValue = ActionCharacterImage(p_Key);
                    break;
                case DialogueEvent.EventCG:
                    dialogueEventValue = ActionEventCG(p_Key);
                    break;
                case DialogueEvent.BGM:
                    dialogueEventValue = ActionBGM(p_Key);
                    break;
                case DialogueEvent.SE:
                    dialogueEventValue = ActionSE(p_Key);
                    break;
                case DialogueEvent.SelectDialogue:
                    dialogueEventValue = ActionSelectDialogue(p_Key);
                    break;
                case DialogueEvent.BackGroundImage:
                    dialogueEventValue = ActionBackGroundImage(p_Key);
                    break;
                case DialogueEvent.Fade:
                    dialogueEventValue = ActionFade(p_Key);
                    break;
                case DialogueEvent.Liking:
                    break;
                default:
                    dialogueEventValue = -1;
                    break;
            }

            return dialogueEventValue;
        }

        public void SetDialogueEventEnd(bool p_Flag)
        {
            isDialogueEventEnd = p_Flag;
        }

        #endregion
    }
}