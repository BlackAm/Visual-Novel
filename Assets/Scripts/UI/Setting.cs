#if !SERVER_DRIVE
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace BlackAm
{
    /// '일반옵션' 스크립트 입니다...
    public class Setting : AbstractUI
    {
        private Slider backBGM, gameBGM;
        public GameObject graphicView, soundView;
        private SettingMenu graphicMenu, soundMenu;
        public int _Basic_Resolution_X;
        public int _Basic_Resolution_Y;
        public int _Setting_Resolution_X;
        public int _Setting_Resolution_Y;
        
        /// 옵션명 => 버튼타입 => 오브젝트.
        /// 버튼을 제어할 때 사용됩니다...
        public Dictionary<OptionType, Dictionary<ButtonType, SettingButton>> settingButton;
        private float backBGMValue, gameBGMValue, soulValue;
        
        #region <Property>
        
        /// 카테고리.
        private SettingMenu _menu;
        public SettingMenu Menu
        {
            get => _menu;
            set
            {
                if (_menu != null && _menu == value) return;
                if(_menu != null) _menu.UnSelected();
                _menu = value;
                _menu.Selected();
            }
        }
        
        /// 옵션 별 버튼.
        private SettingButton _spec;
        public SettingButton Spec
        {
            get => _spec;
            set
            {
                if (_spec != null && _spec == value) return;
                if(_spec != null) _spec.UnSelected();
                _spec = value;
                _spec.Selected();

                if (_spec.btType != ButtonType.Custom)
                {
                    /// '사용자 지정'을 제외한 버튼을 누를 시, 나머지 옵션들을 자동 조정한다.
                    for (int i = 1; i <= 3; i++)
                    {
                        if(i.Equals(2)) continue; //해상도는 조절불가
                        settingButton[(OptionType) i][_spec.btType].ClickItem();
                    }
                }
            }
        }
        
        private SettingButton _quality;
        public SettingButton Quality
        {
            get => _quality;
            set
            {
                if (_quality != null && _quality == value) return;
                if(_quality != null) _quality.UnSelected();
                _quality = value;
                _quality.Selected();

                if (Spec.btType != ButtonType.Custom)
                {
                    if (Spec.btType != _quality.btType)
                    {
                       settingButton[OptionType.Spec][ButtonType.Custom].ClickItem(); 
                    }
                }
            }
        }
        
        private SettingButton _resolution;
        public SettingButton Resolution
        {
            get => _resolution;
            set
            {
                if (_resolution != null && _resolution == value) return;
                if(_resolution != null) _resolution.UnSelected();
                _resolution = value;
                _resolution.Selected();
                /*
                if (Spec.btType != ButtonType.Custom)
                {
                    if (Spec.btType != _resolution.btType)
                    {
                        settingButton[OptionType.Spec][ButtonType.Custom].ClickItem(); 
                    }
                    
                }
                */
                settingButton[OptionType.Resolution][_resolution.btType].ClickItem();
            }
        }
        
        private SettingButton _texture;
        public SettingButton Texture
        {
            get => _texture;
            set
            {
                if (_texture != null && _texture == value) return;
                if(_texture != null) _texture.UnSelected();
                _texture = value;
                _texture.Selected();
                
                if (Spec.btType != ButtonType.Custom)
                {
                    if (Spec.btType != _texture.btType)
                    {
                        settingButton[OptionType.Spec][ButtonType.Custom].ClickItem(); 
                    }
                }
            }
        }
        
        private SettingButton _frameLimit;
        public SettingButton FrameLimit
        {
            get => _frameLimit;
            set
            {
                if (_frameLimit != null && _frameLimit == value) return;
                if(_frameLimit != null) _frameLimit.UnSelected();
                _frameLimit = value;
                _frameLimit.Selected();
                if (Spec.btType != ButtonType.Custom)
                {
                    if (Spec.btType != _frameLimit.btType && !(Spec.btType.Equals(ButtonType.High) && _frameLimit.btType.Equals(ButtonType.Middle)))
                    {
                        settingButton[OptionType.Spec][ButtonType.Custom].ClickItem(); 
                    }
                }
            }
        }
        
        private SettingButton _push;
        public SettingButton Push
        {
            get => _push;
            set
            {
                if (_push != null && _push == value) return;
                if(_push != null) _push.UnSelected();
                _push = value;
                _push.Selected();
            }
        }
        
        private SettingButton _guild;
        public SettingButton Guild
        {
            get => _guild;
            set
            {
                if (_guild != null && _guild == value) return;
                if(_guild != null) _guild.UnSelected();
                _guild = value;
                _guild.Selected();
            }
        }
        
        private SettingButton _friend;
        public SettingButton Friend
        {
            get => _friend;
            set
            {
                if (_friend != null && _friend == value) return;
                if(_friend != null) _friend.UnSelected();
                _friend = value;
                _friend.Selected();
            }
        }
        
        #endregion

        // TODO<BlackAm> - 해상도 설정시 UI 사이즈 변경이 가능하도록 변경
        #region <Enum>

        /// 옵션 종류.
        public enum OptionType
        {
            /// 그래픽
            Spec,
            Quality,
            Resolution,
            Texture,

            /// 사운드
            BackBGM,
            GameBGM,
        }

        /// 옵션 제어 버튼 타입.
        public enum ButtonType
        {
            Low,
            Middle,
            High,
            Custom,
            On,
            Off
        }

        #endregion
        
        public override void Initialize()
        {
            if (!ReferenceEquals(null, _UIObject)) return;

            LoadUIObject("MainGameSettingUI.prefab");
            
            settingButton = new Dictionary<OptionType, Dictionary<ButtonType, SettingButton>>();
            
            /// 옵션 별 버튼 초기화.
            for (int i = 0; i < Enum.GetValues(typeof(OptionType)).Length; i++)
            {
                settingButton.Add((OptionType)i, new Dictionary<ButtonType, SettingButton>());
                for (int j = 0; j < Enum.GetValues(typeof(ButtonType)).Length; j++)
                {
                    settingButton[(OptionType)i].Add((ButtonType)j, null);
                }
            }
            
            GetComponent<Button>("MainGameSettingUI/Top/Close").onClick.AddListener(Close);

            graphicView = Find("MainGameSettingUI/Graphic").gameObject;
            soundView = Find("MainGameSettingUI/Sound").gameObject;
            graphicMenu = Find("MainGameSettingUI/Top/Image/Menu/Graphic").gameObject.AddComponent<SettingMenu>();
            soundMenu = Find("MainGameSettingUI/Top/Image/Menu/Sound").gameObject.AddComponent<SettingMenu>();
            graphicMenu.Init();
            soundMenu.Init();
            
            // 간편설정
            // var specLow = Find("MainGameSettingUI/Graphic/Easy/Button/Low").gameObject.AddComponent<SettingButton>();
            // var specMiddle = Find("MainGameSettingUI/Graphic/Easy/Button/Middle").gameObject.AddComponent<SettingButton>();
            // var specHigh = Find("MainGameSettingUI/Graphic/Easy/Button/High").gameObject.AddComponent<SettingButton>();
            // var specCustom = Find("MainGameSettingUI/Graphic/Easy/Button/Custom").gameObject.AddComponent<SettingButton>();
            // 품질효과
            // var qualityLow = Find("MainGameSettingUI/Graphic/Quality/Button/Low").gameObject.AddComponent<SettingButton>();
            // var qualityMiddle = Find("MainGameSettingUI/Graphic/Quality/Button/Middle").gameObject.AddComponent<SettingButton>();
            // var qualityHigh = Find("MainGameSettingUI/Graphic/Quality/Button/High").gameObject.AddComponent<SettingButton>();
            // 해상도
            var resolutionLow = Find("MainGameSettingUI/Graphic/Resolution/Button/Low").gameObject.AddComponent<SettingButton>();
            var resolutionMiddle = Find("MainGameSettingUI/Graphic/Resolution/Button/Middle").gameObject.AddComponent<SettingButton>();
            var resolutionHigh = Find("MainGameSettingUI/Graphic/Resolution/Button/High").gameObject.AddComponent<SettingButton>();
            var resolutionBasic = Find("MainGameSettingUI/Graphic/Resolution/Button/Basic").gameObject.AddComponent<SettingButton>();
            // 텍스쳐
            // var textureLow = Find("MainGameSettingUI/Graphic/Texture/Button/Low").gameObject.AddComponent<SettingButton>();
            // var textureMiddle = Find("MainGameSettingUI/Graphic/Texture/Button/Middle").gameObject.AddComponent<SettingButton>();
            // var textureHigh = Find("MainGameSettingUI/Graphic/Texture/Button/High").gameObject.AddComponent<SettingButton>();
            
            // specLow.Init(OptionType.Spec, ButtonType.Low);
            // specMiddle.Init(OptionType.Spec, ButtonType.Middle);
            // specHigh.Init(OptionType.Spec, ButtonType.High);
            // specCustom.Init(OptionType.Spec, ButtonType.Custom);
            // qualityLow.Init(OptionType.Quality, ButtonType.Low);
            // qualityMiddle.Init(OptionType.Quality, ButtonType.Middle);
            // qualityHigh.Init(OptionType.Quality, ButtonType.High);
            resolutionLow.Init(OptionType.Resolution, ButtonType.Low);
            resolutionMiddle.Init(OptionType.Resolution, ButtonType.Middle);
            resolutionHigh.Init(OptionType.Resolution, ButtonType.High);
            resolutionBasic.Init(OptionType.Resolution, ButtonType.Custom);
            // textureLow.Init(OptionType.Texture, ButtonType.Low);
            // textureMiddle.Init(OptionType.Texture, ButtonType.Middle);
            // textureHigh.Init(OptionType.Texture, ButtonType.High);
            
            // settingButton[OptionType.Spec][ButtonType.Low] = specLow;
            // settingButton[OptionType.Spec][ButtonType.Middle] = specMiddle;
            // settingButton[OptionType.Spec][ButtonType.High] = specHigh;
            // settingButton[OptionType.Spec][ButtonType.Custom] = specCustom;
            // settingButton[OptionType.Quality][ButtonType.Low] = qualityLow;
            // settingButton[OptionType.Quality][ButtonType.Middle] = qualityMiddle;
            // settingButton[OptionType.Quality][ButtonType.High] = qualityHigh;
            settingButton[OptionType.Resolution][ButtonType.Low] = resolutionLow;
            settingButton[OptionType.Resolution][ButtonType.Middle] = resolutionMiddle;
            settingButton[OptionType.Resolution][ButtonType.High] = resolutionHigh;
            settingButton[OptionType.Resolution][ButtonType.Custom] = resolutionBasic;
            // settingButton[OptionType.Texture][ButtonType.Low] = textureLow;
            // settingButton[OptionType.Texture][ButtonType.Middle] = textureMiddle;
            // settingButton[OptionType.Texture][ButtonType.High] = textureHigh;
            
            _Basic_Resolution_X = Screen.width;
            _Basic_Resolution_Y = Screen.height;
            _Setting_Resolution_X = _Basic_Resolution_X;
            _Setting_Resolution_Y = _Basic_Resolution_Y;
            
            // 초깃값
            settingButton[OptionType.Resolution][ButtonType.Custom].ClickItem();
            // Spec = specHigh;
            
            backBGM = GetComponent<Slider>("MainGameSettingUI/Sound/BGM/Slider");
            backBGM.onValueChanged.AddListener(SetBackBGM);
            
            gameBGM = GetComponent<Slider>("MainGameSettingUI/Sound/Sound/Slider");
            gameBGM.onValueChanged.AddListener(SetGameBGM);

            Menu = graphicMenu;
                        
            /// 저장된 옵션 적용.
            GetPlayerSetting();
        }

        /// '쿠폰' 탭에서의 플레이어 이름 최신화 및 옵션 적용.
        public override void OnActive()
        {
            
        }

        public override void OnDisable()
        {
        }
        
        #region <Setting>

        /// 기본값으로 버튼 설정.
        /// 버튼의 상태가 enum 값으로 저장됩니다...
        private void SetPlayerSetting()
        {
            // 배경 음악.
            PlayerPrefs.SetFloat("BackBGM", backBGMValue);

            // 게임 사운드.
            PlayerPrefs.SetFloat("GameBGM", gameBGMValue);
        }

        /// 기본값으로 버튼 설정.
        /// 버튼의 상태가 enum 값으로 저장됩니다...
        private void GetPlayerSetting()
        {
            // 배경 음악.
            if (PlayerPrefs.HasKey("BackBGM"))
            {
                backBGM.value = backBGMValue = PlayerPrefs.GetFloat("BackBGM");
                SetBackBGM(backBGMValue);
            }

            // 게임 사운드.
            if (PlayerPrefs.HasKey("GameBGM"))
            {
                gameBGM.value = gameBGMValue = PlayerPrefs.GetFloat("GameBGM");
                SetGameBGM(gameBGMValue);
            }
        }

        #endregion

        #region <Menu>
        
        /// 뒤로 가기(캐릭터선택으로)
        private void Close()
        {
            SetPlayerSetting();
            MainGameUI.Instance.functionUI.CloseUI();
        }

        private void SaveSetting(string answer)
        {
            switch (answer)
            {
                case "Yes":
#if UNITY_EDITOR
                    Debug.LogWarning("저장되었습니다.");
#endif
                    SetPlayerSetting();
                    break;
                case "No":
#if UNITY_EDITOR
                    Debug.LogWarning("저장을 취소했습니다.");
#endif
                    break;
            }

            MainGameUI.Instance.functionUI.CloseUI();
            // MainGameUI.Instance.popUpUI.touchLock.SetActive(false);
        }
        #endregion

        #region <Resolution>

        public void SetSettingResolution(int p_X, int p_Y)
        {
            _Setting_Resolution_X = p_X;
            _Setting_Resolution_Y = p_Y;
        }

        #endregion

        #region <Sound>
        ///  배경음악 값 변경.
        private void SetBackBGM(float num)
        {
            backBGMValue = num;
            BGMManager.GetInstance.Volume = num;
           
        }

        ///  게임소리 값 변경.
        private void SetGameBGM(float num)
        {
            gameBGMValue = num;
            AudioManager.GetInstance.SfxUnitVolume = num;
        }
        
        #endregion
    }
}
#endif