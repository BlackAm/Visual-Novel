using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace BlackAm
{
    public class SaveLoad : AbstractUI
    {
        #region <Consts>

        private const int MaxSaveFileCount = 100;

        #endregion

        private static SaveLoad _instance;

        public static SaveLoad Instance
        {
            get => _instance;
        }

        #region <Enum>

        public enum SaveLoadMode
        {
            None,
            Save,
            Load,
        }

        public enum SaveLoadType
        {
            Normal,
            Quick,
        }

        #endregion

        #region <Fields>

        private SaveLoadMode _currentSaveLoadMode;

        public SaveLoadMode CurrentSaveLoadMode
        {
            get => _currentSaveLoadMode;
            set => _currentSaveLoadMode = value;
        }

        private SaveLoadPoolingManager saveLoadPoolingManager;
        public UIScroll<SaveLoadItem> saveLoadItemScroll;

        // public List<SaveLoadItem> SaveLoadItemList;
        
        public SaveLoadItem QuickSaveItem;
        public GameObject QuickSaveButton;
        
        public SaveLoadItem AutoSaveItem;
        public GameObject AutoSaveButton;

        #endregion

        public override void OnSpawning()
        {
            base.OnSpawning();
            
            if (!ReferenceEquals(null, _UIObject)) return;

            _instance = _instance ? _instance : this;
            
            QuickSaveButton = Find("Top/QuickSave").gameObject;
            QuickSaveItem = QuickSaveButton.AddComponent<SaveLoadItem>();
            QuickSaveItem.Initialize();
            
            AutoSaveButton = Find("Top/AutoSave").gameObject;
            AutoSaveItem = AutoSaveButton.AddComponent<SaveLoadItem>();
            AutoSaveItem.Initialize();
            
            // SaveLoadItemList = new List<SaveLoadItem>();

            saveLoadPoolingManager = (SaveLoadPoolingManager) AddComponent<SaveLoadPoolingManager>("SaveLoadFiles/Viewport/Content").Initialize();

            saveLoadItemScroll = new UIScroll<SaveLoadItem>(saveLoadPoolingManager, saveLoadPoolingManager.GetComponent<RectTransform>(), 0, 0);
            for (int i = 0; i < MaxSaveFileCount; i++)
            {
                // TODO<BlackAm> - 스크롤에 보이지 않는 UI들 비활성화
                var item = saveLoadItemScroll.AddContent();
            }
        }

        #region <Methods>

        public override void Initialize()
        {
            
            // LoadUIObject("MainGameSaveLoadUI.prefab");

            /*QuickSaveButton = Find("MainGameSaveLoadUI/Top/QuickSave").gameObject;
            QuickSaveItem = QuickSaveButton.AddComponent<SaveLoadItem>();
            QuickSaveItem.Initialize();
            
            AutoSaveButton = Find("MainGameSaveLoadUI/Top/AutoSave").gameObject;
            AutoSaveItem = AutoSaveButton.AddComponent<SaveLoadItem>();
            AutoSaveItem.Initialize();
            
            // SaveLoadItemList = new List<SaveLoadItem>();

            saveLoadPoolingManager = (SaveLoadPoolingManager) AddComponent<SaveLoadPoolingManager>("MainGameSaveLoadUI/SaveLoadFiles/Viewport/Content").Initialize();

            saveLoadItemScroll = new UIScroll<SaveLoadItem>(saveLoadPoolingManager, saveLoadPoolingManager.GetComponent<RectTransform>(), 0, 0);
            for (int i = 0; i < MaxSaveFileCount; i++)
            {
                // TODO<BlackAm> - 스크롤에 보이지 않는 UI들 비활성화
                var item = saveLoadItemScroll.AddContent();
            }*/
        }
        
        // TODO<BlackAm> - 세팅 및 읽은 대사 키 확인 세이브 파일 생성

        public void SetInteractableSave(bool p_Flag)
        {
            QuickSaveItem.button.interactable = p_Flag;
            AutoSaveItem.button.interactable = p_Flag;
        }

        public void OpenUI(SaveLoadMode p_SaveLoadMode, bool p_SaveActive)
        {
            CurrentSaveLoadMode = p_SaveLoadMode;
            SetActiveUI(true);
            SetInteractableSave(p_SaveActive);
        }

        public void CloseUI()
        {
            CurrentSaveLoadMode = SaveLoadMode.None;
            SetActiveUI(false);
        }

        public void SetActiveUI(bool p_Flag)
        {
            SetActive(p_Flag);
        }

        #endregion

        
    }
}