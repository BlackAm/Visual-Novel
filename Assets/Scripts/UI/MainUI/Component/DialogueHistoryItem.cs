using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlackAm
{
    public class DialogueHistoryItem : AbstractUI
    {
        public Text DialogueText;
        public Text TalkerName;

        public Button button;

        public SaveLoadManager.SaveData saveData;

        public override void Initialize()
        {
            base.Initialize();
            DialogueText = GetComponent<Text>("Text/DialogueText");
            TalkerName = GetComponent<Text>("Text/TalkerName");

            button = GetComponent<Button>("HistoryButton");
            button.onClick.AddListener(LoadGame);
        }

        public override void OnActive()
        {
            base.OnActive();
            saveData = SaveLoadManager.GetInstanceUnSafe.MakeSaveData();
        }

        public void SetDialogueText(string p_Dialogue)
        {
            DialogueText.text = p_Dialogue;
        }

        public void SetTalkerName(string p_Name)
        {
            TalkerName.text = p_Name;
        }

        public void LoadGame()
        {
            MainGameUI.Instance.functionUI.CloseUI();
            SaveLoadManager.GetInstanceUnSafe.ApplySaveDataInGame(saveData);
            TopMenu.Instance.DialogueHistory.interactable = true;
        }
    }
}