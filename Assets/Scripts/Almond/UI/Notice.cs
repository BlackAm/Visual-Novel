#if !SERVER_DRIVE
using System.Collections;
using System.Collections.Generic;
using k514;
using UnityEngine;
using UnityEngine.UI;

namespace UI2020
{
    public class Notice : AbstractUI
    {
        public override void Initialize()
        {
            GetComponent<Button>("View/SelectServer/NoticeFold").onClick.AddListener(Close);

            // 언어
            GetComponent<Text>("View/SelectServer/Title").text = LanguageManager.GetContent(200500);
            GetComponent<Text>("View/SelectServer/Notice/Text").text = LanguageManager.GetContent(200501);
            GetComponent<Text>("View/SelectServer/EventNotice/Text").text = LanguageManager.GetContent(200502);
        }

        private void Close()
        {
            MenuUI.Instance.ChangeScene(MenuUI.MenuUIList.ServerChange);
        }
    }
}
#endif