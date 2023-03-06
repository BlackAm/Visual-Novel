#if !SERVER_DRIVE
using System;
using UnityEngine.UI;

namespace UI2020
{
    public class TitleUI :AbstractUI
    {
        private void Awake()
        {
            GetComponent<Button>("Button").onClick.AddListener(StartGame);
        }

        private void StartGame()
        {
            // TODO - 메인 화면 버튼 기능 추가
            // ServerConnectUI.Instance.ChangeScene(ServerConnectUI.ServerConnectUIList.Login);
        }
    }
}
#endif