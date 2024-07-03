using System.Collections;
using System.Collections.Generic;
using k514;
using UI2020.UIComponent;
using UnityEngine;

namespace UI2020
{
    public partial class MainUI
    {
        private const int MaxSelectDialogueCount = 5;
        private const float SelectDialoguePopObjectSize = 50f;

        private SelectDialoguePoolingManager selectDialoguePoolingManager;
        private List<SelectDialogueButton> _selectDialogue;
        
        private void Initialize_MiddleCenter_SelectDialogue()
        {
            var basePath = "MiddleCenter_SelectDialogue";

            _selectDialogue = new List<SelectDialogueButton>();

            selectDialoguePoolingManager = (SelectDialoguePoolingManager) AddComponent<SelectDialoguePoolingManager>($"{basePath}/SelectDialogue").Initialize();

            for (var i = 0; i < MaxSelectDialogueCount; i++)
            {
                var selectDialogue = selectDialoguePoolingManager.GetObject();
                selectDialogue.transform.localPosition = Vector3.zero;
                _selectDialogue.Add(selectDialogue);
            }
        }

        public void AddSelectDialogue(int p_NextDialogueKey, int p_DialogueTextKey, int p_ConditionKey)
        {
            var selectDialogue = selectDialoguePoolingManager.GetObject();
            selectDialogue.SetNextDialogueKey(p_NextDialogueKey);
            selectDialogue.SetDialogueText(LanguageManager.GetContent(p_DialogueTextKey));
            selectDialogue.SetButtonDisable(selectDialogue.CheckInteractableButton(p_ConditionKey));

            for (var i = 0; i < _selectDialogue.Count; i++)
            {
                _selectDialogue[i].transform.localPosition += Vector3.up * SelectDialoguePopObjectSize;
            }
            
            _selectDialogue.Add(selectDialogue);
        }
    }
}
