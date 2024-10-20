using System.Collections.Generic;
using UnityEngine;

namespace BlackAm
{
    public partial class MainUI
    {
        private const int MaxSelectDialogueCount = 5;
        private const float SelectDialoguePopObjectSize = 100f;

        private SelectDialoguePoolingManager selectDialoguePoolingManager;
        private List<SelectDialogueButton> _selectDialogue;

        private void Initialize_TopMiddle_SelectDialogue()
        {
            var basePath = "TopCenter_SelectDialogue";
            
            _selectDialogue = new List<SelectDialogueButton>();

            selectDialoguePoolingManager = (SelectDialoguePoolingManager) AddComponent<SelectDialoguePoolingManager>($"{basePath}/SelectDialoguePoolingManager").Initialize();

            for (var i = 0; i < MaxSelectDialogueCount; i++)
            {
                var selectDialogue = selectDialoguePoolingManager.GetObject();
                selectDialogue.transform.localPosition = Vector3.zero;
                selectDialoguePoolingManager.PoolObject(selectDialogue);
            }
        }
        
        public void OnSelectDialogueClicked()
        {
            ClearSelectDialogue();
            SetDialogueEventEnd(true);
        }

        public void AddSelectDialogue(int p_NextDialogueKey, int p_DialogueTextKey, int p_ConditionKey)
        {
            var selectDialogue = selectDialoguePoolingManager.GetObject();
            selectDialogue.transform.localPosition = Vector3.zero;
            selectDialogue.SetNextDialogueKey(p_NextDialogueKey);
            selectDialogue.SetDialogueText(LanguageManager.GetContent(p_DialogueTextKey));
            selectDialogue.SetButtonDisable(selectDialogue.CheckInteractableButton(p_ConditionKey));

            for (var i = 0; i < _selectDialogue.Count; i++)
            {
                _selectDialogue[i].transform.localPosition += Vector3.down * SelectDialoguePopObjectSize;
            }
            
            _selectDialogue.Add(selectDialogue);
        }

        public void ClearSelectDialogue()
        {
            while (_selectDialogue.Count > 0)
            {
                selectDialoguePoolingManager.PoolObject(_selectDialogue[_selectDialogue.Count - 1]);
                _selectDialogue.Remove(_selectDialogue[_selectDialogue.Count - 1]);
            }
            
            MainGameUI.Instance.mainUI.ActiveMainUI(UIList.TopCenter, false);
        }

        public bool IsSelectDialogueExist()
        {
            return _selectDialogue.Count > 0;
        }
    }
}