#if !SERVER_DRIVE
using System;
using System.Collections;
using System.Collections.Generic;
using k514;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI2020
{
    public class MenuUI : AbstractUI
    {
        private static MenuUI _instance;
        public static MenuUI Instance => _instance;
        public GameObject _backGround, _bgMask1, _bgMask2, touchLock;
        public StartTitle startTitle;
        public MainTitle mainTitle;
        public NameInput nameInput;
        
        //public Top _top;
        public Notice _notice;
        public EffectsStore _effect;
        public ExteriorStore _exter;
        //public Spear _spear;

        private AbstractUI _curStateUI;
        private MenuUIList? _curState;
        
        public enum MenuUIList
        {
            StartTitle,
            MainTitle,
            NameInput,
            LoadGame,
            Rememberance,
            Setting,
            Login,
            Connect,
            ServerChange,
            Notice,
            EffectsStore,
            ExteriorStore,
            //Spear,
            TouchLock
        }

        private void Awake()
        {
            if (_instance == null) _instance = this;
            startTitle = AddComponent<StartTitle>("Menu/StartTitle");
            mainTitle = AddComponent<MainTitle>("Menu/MainTitle");
            nameInput = AddComponent<NameInput>("Menu/NameInput");

            // _login = AddComponent<Login>("Menu/Login");

            //_top = AddComponent<Top>("Menu/Top");
            _notice = AddComponent<Notice>("Menu/Notice");

            _effect = AddComponent<EffectsStore>("Menu/EffectsStore");

            _exter = AddComponent<ExteriorStore>("Menu/ExteriorStore");

            touchLock = Find("Menu/TouchLock").gameObject;
            touchLock.SetActive(false);

            // 초기 활성화.
            startTitle.SetActive(true);
            mainTitle.SetActive(true);
            nameInput.SetActive(true);
            // _login.SetActive(true);
            _notice.SetActive(true);
            //_spear.SetActive(true);

            startTitle.Initialize();
            mainTitle.Initialize();
            nameInput.Initialize();

            /*EffectsStore,
            ExteriorStore,
            Guild*/
            //_curState = MenuUIList.Title;
            
            _curState = null;
            
            // 테스트
            
            ChangeScene(MenuUIList.StartTitle);
        }
        
        public void ChangeScene(MenuUIList targetScene)
        {
            if (_curState == targetScene) return;
            if (_curStateUI != null) _curStateUI.OnDisable();
            startTitle.SetActive(targetScene == MenuUIList.StartTitle);
            mainTitle.SetActive(targetScene == MenuUIList.MainTitle);
            nameInput.SetActive(targetScene == MenuUIList.NameInput);
            // _login.SetActive(targetScene == MenuUIList.Login);
            //_top.SetActive(targetScene == MenuUIList.Top);
            _notice.SetActive(targetScene == MenuUIList.Notice);
            _effect.SetActive(targetScene == MenuUIList.EffectsStore);
            _exter.SetActive(targetScene == MenuUIList.ExteriorStore);
            //_spear.SetActive(targetScene == MenuUIList.Spear);
            switch (targetScene)
            {
                case MenuUIList.StartTitle:
                    _curStateUI = startTitle;
                    break;
                case MenuUIList.MainTitle:
                    _curStateUI = mainTitle;
                    break;
                case MenuUIList.NameInput:
                    _curStateUI = nameInput;
                    break;
                case MenuUIList.Notice:
                    _curStateUI = _notice;
                    break;
                case MenuUIList.EffectsStore:
                    _curStateUI = _effect;
                    break;
                case MenuUIList.ExteriorStore:
                    _curStateUI = _exter;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(targetScene), targetScene, null);
            }
            _curState = targetScene;
            _curStateUI.OnActive();
        }
    }
}
#endif