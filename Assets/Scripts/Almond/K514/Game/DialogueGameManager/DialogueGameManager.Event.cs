using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI2020
{
    public partial class DialogueGameManager
    {
        #region <Fields>

        public bool isDialogueEventEnd = true;

        #endregion
        
        #region <Method>

        // TODO<BlackAm> - return 방식을 (int) 에서 (Enum, int)로 바꾸기
        // TODO<BlackAm> - 에러가 발생했을 때 디버그 하기 편하기 위함 
        public int ActionDialogueEvent(DialogueEventFlag p_DialogueEvent, int p_Key)
        {
            int dialogueEventValue = 0;
            isDialogueEventEnd = false;
            switch (p_DialogueEvent)
            {
                case DialogueEventFlag.CharacterImage:
                    dialogueEventValue = ActionCharacterImage(p_Key);
                    break;
                case DialogueEventFlag.EventCG:
                    dialogueEventValue = ActionEventCG(p_Key);
                    break;
                case DialogueEventFlag.BGM:
                    dialogueEventValue = ActionBGM(p_Key);
                    break;
                case DialogueEventFlag.SE:
                    dialogueEventValue = ActionSE(p_Key);
                    break;
                case DialogueEventFlag.SelectDialogue:
                    dialogueEventValue = ActionSelectDialogue(p_Key);
                    break;
                case DialogueEventFlag.BackGroundImage:
                    dialogueEventValue = ActionBackGroundImage(p_Key);
                    break;
                case DialogueEventFlag.Fade:
                    break;
                case DialogueEventFlag.Liking:
                    break;
                default:
                    dialogueEventValue = -1;
                    break;
            }

            return dialogueEventValue;
        }

        #endregion
    }
}