using System.Collections.Generic;
using UnityEngine;

namespace BlackAm
{
    public class DialogueHistory : AbstractUI
    {
        #region <Consts>

        private const int PreLoadDialogueHistoryCount = 100;
        private const float DialogueHistoryPopObjectSize = 150f;

        #endregion
        
        #region <Fields>

        private DialogueHistoryPoolingManager dialogueHistoryPoolingManager;
        public UIScroll<DialogueHistoryItem> dialogueHistoryScroll;

        public List<DialogueHistoryItem> DialogueHistoryList;

        #endregion

        #region <Methods>

        public override void Initialize()
        {
            if (!ReferenceEquals(null, _UIObject)) return;
            
            LoadUIObject("DialogueHistoryUI.prefab");
            
            dialogueHistoryPoolingManager = (DialogueHistoryPoolingManager) AddComponent<DialogueHistoryPoolingManager>("DialogueHistoryUI/DialogueHistory/Viewport/Content").Initialize();

            dialogueHistoryScroll = new UIScroll<DialogueHistoryItem>(dialogueHistoryPoolingManager, dialogueHistoryPoolingManager.GetComponent<RectTransform>(), 0, 0);

            DialogueHistoryList = new List<DialogueHistoryItem>();
            for (int i = 0; i < PreLoadDialogueHistoryCount; i++)
            {
                var item = dialogueHistoryScroll.AddContent();
                dialogueHistoryScroll.DeleteContent(item);
            }
        }

        public void AddDialogueHistory(int CurrentDialogueKey)
        {
            var content = dialogueHistoryScroll.AddContent();
            
            var dialoguePresetData = DialoguePresetData.GetInstanceUnSafe[CurrentDialogueKey];
            
            content.SetDialogueText(LanguageManager.GetContent(dialoguePresetData.DialogueKey));
            content.SetTalkerName(LanguageManager.GetContent(dialoguePresetData.Talker));
            
            DialogueHistoryList.Add(content);
        }

        #endregion

        
    }
}

