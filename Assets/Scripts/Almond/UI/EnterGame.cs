#if !SERVER_DRIVE
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using k514;
using UnityEngine.UI;

namespace UI2020
{
    public class EnterGame : AbstractUI
    {
        public override void Initialize()
        {
            LoadUIObjectAsync("EnterGame.prefab",()=>
            {
                GetComponent<Button>("EnterGame/StartGameButton").onClick.AddListener(StartGame);
                GetComponent<Text>("EnterGame/Text").text = LanguageManager.GetContent(200100);

                GetComponent<Button>("EnterGame/RememberanceButton").onClick.AddListener(Rememberance);
                GetComponent<Text>("EnterGame/Text").text = LanguageManager.GetContent(200202);
                
                GetComponent<Button>("EnterGame/QuitButton").onClick.AddListener(Quit);
                GetComponent<Text>("EnterGame/Text").text = LanguageManager.GetContent(200203);
            },ResourceLifeCycleType.Scene);
           
        }

        private void StartGame()
        {
            
        }

        private void GetStartButtonLanguage()
        {
            //if(PlayerPrefs.GetInt() > 0)
            {}
        }

        private void Rememberance()
        {
            
        }

        private void Quit()
        {
            
        }
    }
}
#endif