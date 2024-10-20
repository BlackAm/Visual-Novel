#if !SERVER_DRIVE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlackAm
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
            TitleMenuUI.Instance.ChangeScene(TitleMenuUI.MenuUIList.ServerChange);
        }
    }
}
#endif