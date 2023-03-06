#if !SERVER_DRIVE
using System;
using BDG;
using k514;
using UnityEngine;
using UnityEngine.UI;
namespace UI2020
{
    public class MenuPanel : AbstractUI
    {
        private Button _closeButton;
        
        private void Awake()
        {
            AddButtonEvent("Close",OnClosed);
            AddButtonEvent("OptionButton", OpenSetting);
            AddButtonEvent("LogoutButton", Logout);
        }
        
        /// 활성/비활성 여부 판별.
        private void OnEnable()
        {
            // 좌/우측 UI 비활성.
            //MainGameUI.Instance.mainUI.SetActiveMenu(false);
        }
        private void OnDisable()
        {
            //MainGameUI.Instance.mainUI.SetActiveMenu(true);
        }
        /// <summary>
        /// 닫기
        /// </summary>
        public void OnClosed()
        {
            MainGameUI.Instance.mainUI.overlay.transform.SetAsFirstSibling();
        }
        private void OpenSetting()
        {
            MainGameUI.Instance.functionUI.OpenUI(FunctionUI.UIIndex.Setting);
        }
        
        /// 홈으로 이동...
        private void GoHome()
        {
            //SceneController.GetInstanceUnSafe.TurnSceneTo(SceneControllerTool.SceneControllerShortCutType.MainHomeScene);
            // PopUpUIManager.Instance.isDead = false;
            
            OnClosed();
        }
        
        /// 게임 종료.
        private void Quit()
        {
            DefaultUIManagerSet.GetInstanceUnSafe._UiMessageBoxController.Pop(UIMessageBoxController.MessageType.Quit,
                () =>
                {
                    // MainGameUI.Instance.popUpUI.touchLock.SetActive(false);
                    Application.Quit(); 
                }, () =>
                {
                    // MainGameUI.Instance.popUpUI.touchLock.SetActive(false);
                });
        }
        private void Logout()
        {
            SceneControllerManager.GetInstance.TurnSceneTo(SceneControllerTool.SceneControllerShortCutType.LoginScene);
            OnClosed();
        }
    }
}
#endif