#if !SERVER_DRIVE
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using k514;
using UnityEngine.UI;

namespace UI2020
{
    public class MainTitle : AbstractUI
    {
        public override void Initialize()
        {
            LoadUIObjectAsync("MainTitle.prefab",()=>
            {
                GetComponent<Button>("MainTitle/StartNewGameButton").onClick.AddListener(StartNewGame);
                GetComponent<Text>("MainTitle/StartNewGameButton/Text").text = LanguageManager.GetContent(200100);
                
                GetComponent<Button>("MainTitle/StartLoadGameButton").onClick.AddListener(LoadGame);
                GetComponent<Text>("MainTitle/StartLoadGameButton/Text").text = LanguageManager.GetContent(200100);

                GetComponent<Button>("MainTitle/RememberanceButton").onClick.AddListener(Rememberance);
                GetComponent<Text>("MainTitle/RememberanceButton/Text").text = LanguageManager.GetContent(200202);
                
                GetComponent<Button>("MainTitle/SettingButton").onClick.AddListener(Setting);
                GetComponent<Text>("MainTitle/SettingButton/Text").text = LanguageManager.GetContent(200202);
                
                GetComponent<Button>("MainTitle/QuitButton").onClick.AddListener(Quit);
                GetComponent<Text>("MainTitle/QuitButton/Text").text = LanguageManager.GetContent(200203);
            },ResourceLifeCycleType.Scene);
           
        }

        private void StartNewGame()
        {
            MenuUI.Instance.ChangeScene(MenuUI.MenuUIList.NameInput);
        }

        private void LoadGame()
        {
            
        }

        private void Rememberance()
        {
            
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