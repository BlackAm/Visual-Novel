using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System;

namespace BlackAm
{
    public class NameInput : AbstractUI
    {
        #region <Fields>

        public Text name;

        #endregion
        
        #region <Callbacks>

        public override void Initialize()
        {
            LoadUIObjectAsync("NameInput.prefab", () =>
            {
                GetComponent<Text>("NameInput/Text").text = LanguageManager.GetContent(20000);

                var path = "NameInput/InputField/";
                GetComponent<Text>("NameInput/InputField/Placeholder").text = LanguageManager.GetContent(20030);
                name = GetComponent<Text>("NameInput/InputField/Text");
                
                GetComponent<Button>("NameInput/SubmitButton").onClick.AddListener(SubmitName);
                GetComponent<Text>("NameInput/SubmitButton/Text").text = LanguageManager.GetContent(20002);
            }, ResourceLifeCycleType.Scene);    
        }

        #endregion

        #region <Methods>

        public void SubmitName()
        {
            TitleMenuUI.Instance.touchLock.SetActive(true);
            DefaultUIManagerSet.GetInstanceUnSafe._UiMessageBoxController.Pop(UIMessageBoxController.MessageType.SetPlayerName,
                () =>
                {
                    // PlayerPrefs.SetString("Name", name.text);
                    SceneControllerManager.GetInstance.TurnSceneTo(SceneControllerTool.SceneControllerShortCutType.DialogueScene, (SceneControllerTool.LoadingSceneType.Black));
                    MainGameUI.Instance.popUpUI.touchLock.SetActive(false);
                    TitleMenuUI.Instance.touchLock.SetActive(false);
                    TitleMenuUI.Instance.nameInput.SetActive(false);
                    DialogueGameManager.GetInstance.SetSceneKey(1);
                }, () =>
                {
                    MainGameUI.Instance.popUpUI.touchLock.SetActive(false);
                    TitleMenuUI.Instance.touchLock.SetActive(false);
                });
        }

        #endregion
    }
}