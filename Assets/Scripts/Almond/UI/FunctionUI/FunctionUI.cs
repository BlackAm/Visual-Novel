#if !SERVER_DRIVE
using System;
using System.Collections.Generic;
using BDG;
using k514;
using UnityEngine;
using UnityEngine.UI;

namespace UI2020
{
    public class FunctionUI : AbstractUI
    {
        private Image _bg;
        public Setting setting;
        
        private GameObject mainBack;
        public AbstractUI _curUI;
        // momo6346 - 인벤토리 항목들이 생성됐는지?
        public bool isListAdd = false;
        // 6레벨 아이템 지급 플래그입니다.
        public bool is6LevelItemAdd = false;
        // 인벤토리 아이템이 서버통신이 되었는지 플래그입니다.
        public bool isInventoryDB = false;
        private UIIndex _curView;
        public UIIndex CurUI
        {
            get => _curView;
            set => _curView = value;
        }
        public enum UIIndex
        {
            Inventory,
            StoryDungeon,
            DungeonMap,
            Shop,
            DiaShop,
            SleepMode,
            Setting,
            CharacterSelect,
            Guild,
            Quest,
            RaidDungeon,
            BattleSetting,
            Pet,
            Friend,
            Spear,
            Exchange,
            CombatHistory,
            Mail
        }
        
        public override void OnSceneStarted()
        {
            // 씬 시작할 때마다 리스트가 한 번 생성되게 했습니다.
            // 이후 서버통신 시 이 부분은 수정되어야 합니다.
            isListAdd = false;
        }
        
        public override void Initialize()
        {
            _bg = GetComponent<Image>("BG");
            setting = AddComponent<Setting>("Setting");
        }

        public void OpenUI(UIIndex index)
        {
            _bg.color = Color.black;

            if (_curUI != null)
            {
                _curUI.SetActive(false);
                _curUI.OnDisableUI();
            }
            switch (index)
            {
                case UIIndex.SleepMode:
                    _curUI.Initialize();
                    break;
                case UIIndex.Setting:
                    _curUI = setting;
                    _curUI.Initialize();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }

            _curUI.SetActive(true);
            _curUI.OnActive();
        }
        public override void OnUpdateUI(float p_DeltaTime)
        {
            if(!ReferenceEquals(null,_curUI))
                _curUI.OnUpdateUI(p_DeltaTime);
        }
        public void CloseUI()
        {
            if(!ReferenceEquals(_curUI, null)) _curUI.SetActive(false);
            MainGameUI.Instance.mainUI.gameObject.SetActive(true);
            MainGameUI.Instance.mainUI.DefaultHide();

            if(LamiereGameManager.GetInstanceUnSafe._ClientPlayer.IsValid())
                LamiereGameManager.GetInstanceUnSafe._ClientPlayer.PlayerNamePanel.TrySyncPosition();
        }
    }
}
#endif