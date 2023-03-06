#if !SERVER_DRIVE
using System;
using System.Collections.Generic;
using BDG;
using k514;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace UI2020
{
    /// '일반옵션' 스크립트 입니다...
    public class Setting : AbstractUI
    {
        private Slider backBGM, gameBGM;
        private Text soulTt, playerName;
        public GameObject soundView;
        private SettingMenu soundMenu;
        private float backBGMValue, gameBGMValue;
        
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
        
        #endregion

        #region <Enum>

        /// 옵션 종류.
        public enum OptionType
        {
            /// 그래픽
            Spec,
            Quality,
            Resolution,
            Texture,
            FrameLimit,

            /// 사운드
            BackBGM,
            GameBGM,
            Push,
            Guild,
            Friend,

            Soul
            /// 쿠폰
            //...
        }

        #endregion
        
        public override void Initialize()
        {
            if (!ReferenceEquals(null, _UIObject)) return;

            LoadUIObject("MainGameSettingUI.prefab");
            
            GetComponent<Button>("MainGameSettingUI/Top/Close").onClick.AddListener(Close);

            soundView = Find("MainGameSettingUI/Sound").gameObject;
            soundMenu = Find("MainGameSettingUI/Top/Image/Menu/Sound").gameObject.AddComponent<SettingMenu>();
            soundMenu.Init();
            
            backBGM = GetComponent<Slider>("MainGameSettingUI/Sound/BGM/Slider");
            backBGM.onValueChanged.AddListener(SetBackBGM);
            
            gameBGM = GetComponent<Slider>("MainGameSettingUI/Sound/Sound/Slider");
            gameBGM.onValueChanged.AddListener(SetGameBGM);

            playerName = GetComponent<Text>("MainGameSettingUI/Coupon/List/Player/Content");
                        
            /// 저장된 옵션 적용.
            GetPlayerSetting();
        }

        /// '쿠폰' 탭에서의 플레이어 이름 최신화 및 옵션 적용.
        public override void OnActive()
        {
            if (PlayerManager.GetInstance.Player.IsValid())
            {
                // TODO - 게임 시작시 유저의 닉네임 불러오기
                //playerName.text = PlayerManager.GetInstance.Player.GetUnitName();
            }
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
            MapEffectSoundManager.GetInstance.Volume = num;
        }
        
        #endregion
    }
}
#endif