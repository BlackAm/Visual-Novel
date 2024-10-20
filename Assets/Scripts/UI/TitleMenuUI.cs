#if !SERVER_DRIVE
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace BlackAm
{
    public class TitleMenuUI : AbstractUI
    {
        private static TitleMenuUI _instance;
        public static TitleMenuUI Instance => _instance;
        public GameObject backGround, _bgMask1, _bgMask2, touchLock;
        public StartTitle startTitle;
        public MainTitle mainTitle;
        public NameInput nameInput;
        public Gallery gallery;

        private AbstractUI _curStateUI;
        private MenuUIList? _curState;
        
        public enum MenuUIList
        {
            StartTitle,
            MainTitle,
            NameInput,
            LoadGame,
            Gallery,
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

        public override void OnSpawning()
        {
            base.OnSpawning();
            
            if (_instance == null) _instance = this;
            startTitle = AddComponent<StartTitle>("Menu/StartTitle");
            mainTitle = AddComponent<MainTitle>("Menu/MainTitle");
            nameInput = AddComponent<NameInput>("Menu/NameInput");
            gallery = AddComponent<Gallery>("Menu/Gallery");
            
            backGround = Find("Menu/BackGround").gameObject;

            touchLock = Find("Menu/TouchLock").gameObject;
            touchLock.SetActive(false);

            // 초기 활성화.
            startTitle.SetActive(true);
            mainTitle.SetActive(true);
            nameInput.SetActive(true);
            gallery.SetActive(true);

            startTitle.Initialize();
            mainTitle.Initialize();
            nameInput.Initialize();
            gallery.Initialize();
            
            _curState = null;
            
            // 테스트
            
            ChangeScene(MenuUIList.StartTitle);
        }

        private void Awake()
        {
            /*if (_instance == null) _instance = this;
            startTitle = AddComponent<StartTitle>("Menu/StartTitle");
            mainTitle = AddComponent<MainTitle>("Menu/MainTitle");
            nameInput = AddComponent<NameInput>("Menu/NameInput");
            
            backGround = Find("Menu/BackGround").gameObject;

            touchLock = Find("Menu/TouchLock").gameObject;
            touchLock.SetActive(false);

            // 초기 활성화.
            startTitle.SetActive(true);
            mainTitle.SetActive(true);
            nameInput.SetActive(true);

            startTitle.Initialize();
            mainTitle.Initialize();
            nameInput.Initialize();
            
            _curState = null;
            
            // 테스트
            
            ChangeScene(MenuUIList.StartTitle);*/
        }
        
        public void ChangeScene(MenuUIList targetScene)
        {
            if (_curState == targetScene) return;
            if (_curStateUI != null) _curStateUI.OnDisable();
            startTitle.SetActive(targetScene == MenuUIList.StartTitle);
            mainTitle.SetActive(targetScene == MenuUIList.MainTitle);
            nameInput.SetActive(targetScene == MenuUIList.NameInput);
            gallery.SetActive(targetScene == MenuUIList.Gallery);
            // _login.SetActive(targetScene == MenuUIList.Login);
            //_top.SetActive(targetScene == MenuUIList.Top);
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
                case MenuUIList.Gallery:
                    _curStateUI = gallery;
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