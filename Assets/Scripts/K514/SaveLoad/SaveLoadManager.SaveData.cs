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
        public SaveData MakeSaveData()
        {
            var currentDialogueEvent = new SaveData.DialogueEventData(
                DialogueGameManager.GetInstance.currentDialogueEventData.BGM,
                DialogueGameManager.GetInstance.currentDialogueEventData.FadeActivated,
                new Dictionary<Character, int>(DialogueGameManager.GetInstance.currentDialogueEventData.Liking),
                new Dictionary<Character, CharacterImageSaveData>(DialogueGameManager.GetInstance
                    .currentDialogueEventData.CharacterImageSave),
                new ImageSaveData(DialogueGameManager.GetInstance.currentDialogueEventData.BackGroundImageSave),
                new ImageSaveData(DialogueGameManager.GetInstance.currentDialogueEventData.EventCGSave));
            
            return new SaveData(DialogueGameManager.GetInstance.CurrentDialogueKey,
                DialogueGameManager.GetInstance.CurrentDialogueEndingFlag,
                currentDialogueEvent);
        }

        public bool CompareImageSave(ImageSaveData p_InGameSaveData, ImageSaveData p_FileData)
        {
            return p_InGameSaveData.ImageKey != p_FileData.ImageKey || p_InGameSaveData.Scale != p_FileData.Scale ||
                   p_InGameSaveData.Position != p_FileData.Position;
        }
    }
}
