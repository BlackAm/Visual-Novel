using System;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace BlackAm
{
    public class SaveLoadItem : AbstractUI
    {
        public Button button;
        
        public Text SaveFileNumber;
        public Text SavedDateTime;
        
        public Image ThumbnailImage;

        public SaveLoadManager.SaveData saveData;

        public override void Initialize()
        {
            ThumbnailImage = GetComponent<Image>("Thumbnail/ThumbnailImage");
            SaveFileNumber = GetComponent<Text>("Text/SaveFileNumber");
            SaveFileNumber.text = gameObject.name;
            SavedDateTime = GetComponent<Text>("Text/SavedDateTime");

            button = GetComponent<Button>();
            button.onClick.AddListener(() => { ActionSaveOrLoad(SaveLoad.Instance.CurrentSaveLoadMode).Forget(); });

            var SaveFileData = SaveLoadManager.GetInstanceUnSafe.GetDataFileInDirectory<SaveLoadManager.SaveData>($"{SystemMaintenance.SaveDataFileDirectory}{SaveFileNumber.text}");
            if (SaveFileData.Item1)
            {
                saveData = SaveFileData.Item2;
                SetThumbnailImage(SaveFileNumber.text);
                SetSavedDateTime(saveData.SaveDataTime);
            }
        }

        public async UniTask ActionSaveOrLoad(SaveLoad.SaveLoadMode p_SaveLoadMode, SaveLoad.SaveLoadType p_SaveLoadType = SaveLoad.SaveLoadType.Normal)
        {
            switch (p_SaveLoadMode)
            {
                case SaveLoad.SaveLoadMode.Load:
                    LoadGame(p_SaveLoadType);
                    break;
                case SaveLoad.SaveLoadMode.Save:
                    await SaveGame(p_SaveLoadType);
                    break;
                default:
                    throw new ArgumentException("Wrong Save Load Type");
            }
        }
        
        #region <Method/Save>

        public async UniTask SaveGame(SaveLoad.SaveLoadType p_SaveLoadType)
        {
            saveData = SaveLoadManager.GetInstanceUnSafe.MakeSaveData();

            await SaveLoadManager.GetInstanceUnSafe.SaveGame(p_SaveLoadType, saveData, SaveFileNumber.text);
            SetThumbnailImage(SaveFileNumber.text);
            SetSavedDateTime(saveData.SaveDataTime);
        }

        #endregion

        #region <Method/Load>

        public void LoadGame(SaveLoad.SaveLoadType p_SaveLoadType)
        {
            if (ReferenceEquals(null, saveData)) return;

            switch (DialogueGameManager.GetInstance._KeyInputObject._KeyInputType)
            {
                case DialogueGameManager.KeyInputType.Title:
                    SceneControllerManager.GetInstance.TurnSceneTo(SceneControllerTool.SceneControllerShortCutType.DialogueScene, (SceneControllerTool.LoadingSceneType.Black));
                    SaveLoadManager.GetInstanceUnSafe.loadType = SaveLoadManager.LoadType.TitleLoad;
                    LoadStoryGame(p_SaveLoadType);
                    break;
                case DialogueGameManager.KeyInputType.Story:
                    LoadStoryGame(p_SaveLoadType);
                    break;
            }
        }

        public void LoadStoryGame(SaveLoad.SaveLoadType p_SaveLoadType)
        {
            UpdateTopMenuButton();

            SaveLoad.Instance.CloseUI();
            SaveLoadManager.GetInstanceUnSafe.ApplySaveDataInGame(saveData);
        }

        public void UpdateTopMenuButton()
        {
            TopMenu.Instance.Load.interactable = true;
            TopMenu.Instance.QuickSave.interactable = true;
            TopMenu.Instance.QuickLoad.interactable = true;
            TopMenu.Instance.SkipDialogue.interactable = true;
            TopMenu.Instance.AutoDialogue.interactable = true;
            TopMenu.Instance.Back.interactable = false;
        }

        #endregion

        #region <Method/SetSaveData>

        public void SetThumbnailImage(string p_ResourceName)
        {
            ThumbnailImage.sprite = SaveLoadManager.GetInstanceUnSafe.LoadImageAsSprite(p_ResourceName);
        }

        public void SetDialogueKey(int p_DialogueKey)
        {
            saveData.DialogueKey = p_DialogueKey;
        }

        public void SetSavedDateTime(string p_DateTime)
        {
            SavedDateTime.text = p_DateTime;
        }

        public void SetDialogueEndingFlag(DialogueGameManager.DialogueEndingFlag p_Flag)
        {
            saveData.DialogueEndingFlag = p_Flag;
        }

        public void AddDialogueEndingFlag(DialogueGameManager.DialogueEndingFlag p_Flag)
        {
            saveData.DialogueEndingFlag.AddFlag(p_Flag);
        }

        public void RemoveDialogueEndingFlag(DialogueGameManager.DialogueEndingFlag p_Flag)
        {
            saveData.DialogueEndingFlag.RemoveFlag(p_Flag);
        }

        public void SetDialogueEvent(SaveLoadManager.SaveData.DialogueEventData p_DialogueEvent)
        {
            saveData.DialogueEvent = p_DialogueEvent;
        }

        #endregion
        
        
    }
}