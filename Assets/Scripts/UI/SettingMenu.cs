#if !SERVER_DRIVE
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlackAm
{
    /// <summary>
    /// '일반 옵션'에 들어가는 메뉴 버튼.
    /// </summary>
    public class SettingMenu : AbstractUI
    {
        private Button bgButton;
        private Image bg;
        private Text content;

        public void Init()
        {
            bgButton = GetComponent<Button>();
            bgButton.onClick.AddListener(ClickItem);
            // bg = GetComponent<Image>();
            content = GetComponent<Text>("Text");
        }

        /// 아이템 클릭.
        public void ClickItem()
        {
            MainGameUI.Instance.functionUI.setting.Menu = this;
        }

        public void Selected()
        {
            content.color = new Color(255 / 255f, 255 / 255f, 255 / 255f);
            // bg.gameObject.SetActive(true);
            
            MainGameUI.Instance.functionUI.setting.soundView.gameObject.SetActive(false);
            MainGameUI.Instance.functionUI.setting.graphicView.gameObject.SetActive(false);

            switch (name)
            {
                case "Sound": MainGameUI.Instance.functionUI.setting.soundView.SetActive(true); break;
                case "Graphic": MainGameUI.Instance.functionUI.setting.graphicView.SetActive(true); break;
            }
        }
        
        public void UnSelected()
        {
            content.color = new Color(101 / 255f, 129 / 255f, 152 / 255f);
            // bg.gameObject.SetActive(false);
        }
    }
}
#endif
