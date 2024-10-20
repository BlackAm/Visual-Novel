#if !SERVER_DRIVE
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlackAm
{
    public class MainTitle : AbstractUI
    {
        public override void Initialize()
        {
            LoadUIObjectAsync("MainTitle.prefab",()=>
            {
                GetComponent<Button>("MainTitle/StartNewGameButton").onClick.AddListener(StartNewGame);
                GetComponent<Text>("MainTitle/StartNewGameButton/Text").text = LanguageManager.GetContent(20020);
                
                GetComponent<Button>("MainTitle/StartLoadGameButton").onClick.AddListener(LoadGame);
                GetComponent<Text>("MainTitle/StartLoadGameButton/Text").text = LanguageManager.GetContent(20021);

                GetComponent<Button>("MainTitle/GalleryButton").onClick.AddListener(Gallery);
                GetComponent<Text>("MainTitle/GalleryButton/Text").text = LanguageManager.GetContent(20022);
                
                GetComponent<Button>("MainTitle/SettingButton").onClick.AddListener(Setting);
                GetComponent<Text>("MainTitle/SettingButton/Text").text = LanguageManager.GetContent(20023);
                
                GetComponent<Button>("MainTitle/QuitButton").onClick.AddListener(Quit);
                GetComponent<Text>("MainTitle/QuitButton/Text").text = LanguageManager.GetContent(20024);
            },ResourceLifeCycleType.Scene);
           
        }

        private void StartNewGame()
        {
            //TitleMenuUI.Instance.ChangeScene(TitleMenuUI.MenuUIList.NameInput);
            SceneControllerManager.GetInstance.TurnSceneTo(SceneControllerTool.SceneControllerShortCutType.DialogueScene, (SceneControllerTool.LoadingSceneType.Black));
            DialogueGameManager.GetInstance.SetSceneKey(1);
        }

        private void LoadGame()
        {
            SaveLoad.Instance.OpenUI(SaveLoad.SaveLoadMode.Load, true);
        }

        private void Gallery()
        {
            TitleMenuUI.Instance.ChangeScene(TitleMenuUI.MenuUIList.Gallery);
        }

        private void Setting()
        {
            
        }

        private void Quit()
        {
            
        }
    }
}
#endif