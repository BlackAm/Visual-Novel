#if !SERVER_DRIVE
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlackAm
{
    /// <summary>
    /// '일반 옵션'에 들어가는 버튼.
    /// </summary>
    public class SettingButton : AbstractUI
    {
        private Button bgButton;
        private Image bgImage;
        private Text btText;
        private Setting.OptionType optionType;
        public Setting.ButtonType btType;
        
        public void Init(Setting.OptionType optionType, Setting.ButtonType btType)
        {
            this.optionType = optionType;
            this.btType = btType;
            bgButton = GetComponent<Button>();
            bgButton.onClick.AddListener(ClickItem);
            bgImage = GetComponent<Image>();
            btText = bgButton.GetComponentInChildren<Text>();
        }
        /// 아이템 클릭.
        public void ClickItem()
        {
            switch (optionType)
            {
                case Setting.OptionType.Spec: MainGameUI.Instance.functionUI.setting.Spec = this; break;
                case Setting.OptionType.Quality: MainGameUI.Instance.functionUI.setting.Quality = this; break;
                case Setting.OptionType.Resolution: MainGameUI.Instance.functionUI.setting.Resolution = this; break;
                case Setting.OptionType.Texture: MainGameUI.Instance.functionUI.setting.Texture = this; break;
                default: break;
            }
            SetOption(optionType, btType);
        }

        /// 선택한 옵션에 따른 이벤트.
        /// 슬라이더 변경은 'Setting.cs'의 SetBackBGM, SetGameBGM, SetSoulBGM 참조.
        private void SetOption(Setting.OptionType option, Setting.ButtonType type)
        {
            if (option == Setting.OptionType.Spec)
            {
                switch (type)
                {
                    case Setting.ButtonType.Low: break;
                    case Setting.ButtonType.Middle: break;
                    case Setting.ButtonType.High: break;
                    case Setting.ButtonType.Custom: break;
                }
            } else if (option == Setting.OptionType.Quality)
            {
                switch (type)
                {
                    case Setting.ButtonType.Low:
                        QualitySettings.masterTextureLimit = 2;
#if UNITY_EDITOR
                        Debug.Log("masterTextureLimit : 2");
#endif
                        break;
                    case Setting.ButtonType.Middle:

                        QualitySettings.masterTextureLimit = 1;
#if UNITY_EDITOR
                        Debug.Log("masterTextureLimit : 1");
#endif
                    break;
                    case Setting.ButtonType.High:
                        QualitySettings.masterTextureLimit = 0;
#if UNITY_EDITOR
                        Debug.Log("masterTextureLimit : 0(기본)");
#endif
                    break;
                }
            } else if (option == Setting.OptionType.Resolution)
            {
                switch (type)
                {
                    case Setting.ButtonType.Low:
                        MainGameUI.Instance.functionUI.setting.SetSettingResolution(1920,1080);
                        UICustomRoot.GetInstanceUnSafe.SetCanvasScaler(1920,1080);
#if UNITY_EDITOR
                        Debug.Log("Resolution : 1920,1080");
#endif
                        break;
                    case Setting.ButtonType.Middle:
                        MainGameUI.Instance.functionUI.setting.SetSettingResolution(2560,1440);
                        UICustomRoot.GetInstanceUnSafe.SetCanvasScaler(2560,1440);
#if UNITY_EDITOR
                        Debug.Log("Resolution : 2560,1440");
#endif
                        break;
                    case Setting.ButtonType.High:
                        MainGameUI.Instance.functionUI.setting.SetSettingResolution(2960,1665);
                        UICustomRoot.GetInstanceUnSafe.SetCanvasScaler(2960,1665);
#if UNITY_EDITOR
                        Debug.Log("Resolution :2960,1665");
#endif
                        break;
                    case Setting.ButtonType.Custom: 
                        var x = MainGameUI.Instance.functionUI.setting._Basic_Resolution_X;
                        var y = MainGameUI.Instance.functionUI.setting._Basic_Resolution_Y;

                        if(x < 800 || y < 480){
                            x = 1280;
                            y = 720;
                        }
                        MainGameUI.Instance.functionUI.setting.SetSettingResolution(x, y);
                        UICustomRoot.GetInstanceUnSafe.SetCanvasScaler(x, y);
#if UNITY_EDITOR
                        Debug.Log($"Resolution : {x}, {y}");
#endif
                        break;
                }
            } else if (option == Setting.OptionType.Texture)
            {
                switch (type)
                {
                    case Setting.ButtonType.Low: 
                        QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
#if UNITY_EDITOR
                        if(CustomDebug.PrintGameSystemLog) Debug.Log("anisotropicF 설정 : Disable");
#endif
                        break;
                    case Setting.ButtonType.Middle: 
                        QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
#if UNITY_EDITOR
                        if(CustomDebug.PrintGameSystemLog) Debug.Log("anisotropicF 설정 : Enable(기본)");
#endif
                        break;
                    case Setting.ButtonType.High: 
                        QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
#if UNITY_EDITOR
                        if(CustomDebug.PrintGameSystemLog) Debug.Log("anisotropicF 설정 : ForceEnable");
#endif
                        break;
                }
            }
        }
        
        /// 버튼의 선택과 해제. 
        public void Selected()
        {
            //bgImage.sprite = MainGameUI.Instance.functionUI.setting.@on;
            bgImage.color = new Color(1, 1, 1, 1);
        }
        
        public void UnSelected()
        {
            //bgImage.sprite = MainGameUI.Instance.functionUI.setting.@off;
            bgImage.color = new Color(1, 1, 1, 0);
        }
    }
}
#endif
