using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BlackAm
{
    public partial class SaveLoadManager
    {
        public enum LoadType
        {
            None,
            TitleLoad,
            StoryLoad,
        }

        public LoadType loadType;
        
        public async void ApplySaveDataInGame(SaveData p_SaveData)
        {
            DialogueGameManager.GetInstance.ClearDialogue();

            DialogueGameManager.GetInstance.currentDialogueEventData.Liking = p_SaveData.DialogueEvent.Liking;
            DialogueGameManager.GetInstance.CurrentDialogueEndingFlag = p_SaveData.DialogueEndingFlag;
            if(CompareImageSave(DialogueGameManager.GetInstance.currentDialogueEventData.BackGroundImageSave, p_SaveData.DialogueEvent.BackGroundImageSave))
            {
                MainGameUI.Instance.mainUI.ChangeImage(ImageType.BG, p_SaveData.DialogueEvent.BackGroundImageSave.ImageKey);
                MainGameUI.Instance.mainUI.ResizeImage(ImageType.BG, p_SaveData.DialogueEvent.BackGroundImageSave.Scale);
                MainGameUI.Instance.mainUI.SetImagePosition(ImageType.BG, p_SaveData.DialogueEvent.BackGroundImageSave.Position);
            }

            if (CompareImageSave(DialogueGameManager.GetInstance.currentDialogueEventData.EventCGSave, p_SaveData.DialogueEvent.EventCGSave))
            {
                MainGameUI.Instance.mainUI.ChangeImage(ImageType.EventCG, p_SaveData.DialogueEvent.EventCGSave.ImageKey);
                MainGameUI.Instance.mainUI.ResizeImage(ImageType.EventCG, p_SaveData.DialogueEvent.EventCGSave.Scale);
                MainGameUI.Instance.mainUI.SetImagePosition(ImageType.EventCG, p_SaveData.DialogueEvent.EventCGSave.Position);
                
                // MainGameUI.Instance.mainUI.ActiveMainUI(MainUI.UIList.EventCG, p_SaveData.DialogueEvent.EventCGSave.ImageKey != 0);
            }

            await MainGameUI.Instance.mainUI.ClearCharacterImage();
            
            var enumerator = SystemTool.GetEnumEnumerator<Character>(SystemTool.GetEnumeratorType.GetAll);
            foreach (var character in enumerator)
            {
                if(ReferenceEquals(null, p_SaveData.DialogueEvent.CharacterImageSave[character])) continue;

                MainGameUI.Instance.mainUI.ChangeCharacterImage(character,
                    p_SaveData.DialogueEvent.CharacterImageSave[character]);
            }

            if (MainGameUI.Instance.mainUI.IsSelectDialogueExist())
            {
                MainGameUI.Instance.mainUI.ActiveMainUI(MainUI.UIList.TopCenter, false);
                MainGameUI.Instance.mainUI.ClearSelectDialogue();
            }

            if (p_SaveData.DialogueEvent.FadeActivated)
            {
                MainGameUI.Instance.mainUI.SetFadeOn();
            }
            else
            {
                MainGameUI.Instance.mainUI.SetFadeOff();
            }

            switch (loadType)
            {
                case LoadType.None:
                case LoadType.StoryLoad:
                    DialogueGameManager.GetInstance.SetDialogue(p_SaveData.DialogueKey);
                    break;
                case LoadType.TitleLoad:
                    DialogueGameManager.GetInstance.LoadFromTitle(p_SaveData.DialogueKey);
                    break;
            }
        }
    }
}
