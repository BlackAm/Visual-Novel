#if !SERVER_DRIVE

using System;
using System.Collections.Generic;
using BDG;
using k514;
using UnityEngine;
using UnityEngine.UI;

namespace UI2020
{
    public partial class MainUI : TouchEventSenderCluster
    {
        // public ExpBar expBar;
        public MenuPanel menuPanel;
        //public DungeonSelectUI dungeonSelectUI;
        // public WorldMapUI worldMapUI;
        // public ControlStick controlStick;
        // public AutoButton autoButton;

        public GameObject topLeft, middleLeft, bottomLeft, topRight, middleRight, bottomRight, background;
        // public Transform _ControlStickBase;

        public GameObject overlay,
            middleCenter,
            bottomCenter,
            other;
        
        // momo6346
        public bool serverFlag = false;

        public enum UIList
        {
            Overlay,
            BackGround,
            TopLeft,
            Buff,
            TopCenter_Money,
            TopCenter_Enemy,
            TopRight,
            MiddleLeft,
            MiddleCenter,
            MiddleRight,
            BottomLeft,
            BottomCenter,
            BottomRight,
            Other,
            Party,
            Chat,
            SkillTree,
            RaidView,
            DeadBG
        }
        
        public override void OnSpawning()
        {
            base.OnSpawning();
            
            // _ControlStickBase = transform.Find("ControlStick");
            // controlStick = RegistKeyCodeInput<ControlStick>("BackGround/ControlArea", TouchEventRoot.TouchMappingKeyCodeType.UnitControl, 0, ControllerTool.InputEventType.ControlUnit);
            // expBar = _Transform.Find("BackGround/ExpBar").gameObject.AddComponent<ExpBar>();
            menuPanel = _Transform.Find("Overlay/MenuPanel").gameObject.AddComponent<MenuPanel>();
            //dungeonSelectUI = _Transform.Find("Overlay/Dungeon").gameObject.AddComponent<DungeonSelectUI>();
            // autoButton = _Transform.Find("BottomCenter_QuickSlot/Auto/Effect").gameObject.AddComponent<AutoButton>();

            Initialize_TopRight_Menu();

            // 메뉴
            overlay = Find("Overlay").gameObject;
            background = Find("BackGround").gameObject;
            
            overlay.SetActive(true);
        }
        
        /// momo6346 - 활성화 시 1회성으로 서버에 현재 캐릭터 정보를 요청합니다.(serverFlag 초깃값을 true로...)
        /// 현재 레벨업을 위해 CHARACTER_INFO_REQUEST(600)을 보냅니다.
        private void OnEnable()
        {
            
        }

        protected override void DisposeUnManaged()
        {
            base.DisposeUnManaged();
        }

        public override void OnSceneTransition()
        {
            base.OnSceneTransition();
        }

        public void SetActiveMenu(bool active)
        {
            topLeft.SetActive(active);
            middleLeft.SetActive(active);
            bottomLeft.SetActive(active);
            topRight.SetActive(active);
            middleRight.SetActive(active);
            bottomRight.SetActive(active);
            bottomCenter.SetActive(active);
        }
        
        /// 기본 UI 출력.
        public void SetActiveUI(bool active)
        {
            overlay.SetActive(active);
            background.SetActive(active);
            topLeft.SetActive(active);
            topRight.SetActive(active);
            middleLeft.SetActive(active);
            middleCenter.SetActive(active);
            middleRight.SetActive(active);
            bottomLeft.SetActive(active);
            bottomCenter.SetActive(active);
            bottomRight.SetActive(active);
            other.SetActive(active);
        }
        
        /// 기본적으로 숨겨져있는 UI.
        public void DefaultHide()
        {
            //chat.SetActive(false);
        }

        /// 해당 UI 활성화 여부 결정.
        public void ActiveMainUI(UIList item, bool active)
        {
            switch (item)
            {
                case UIList.Overlay:
                    overlay.SetActive(active);
                    break;
                case UIList.BackGround:
                    background.SetActive(active);
                    break;
                case UIList.TopLeft:
                    topLeft.SetActive(active);
                    break;
                case UIList.TopRight:
                    topRight.SetActive(active);
                    break;
                case UIList.MiddleLeft:
                    middleLeft.SetActive(active);
                    break;
                case UIList.MiddleCenter:
                    middleCenter.SetActive(active);
                    break;
                case UIList.MiddleRight:
                    middleRight.SetActive(active);
                    break;
                case UIList.BottomLeft:
                    bottomLeft.SetActive(active);
                    break;
                case UIList.BottomCenter:
                    bottomCenter.SetActive(active);
                    break;
                case UIList.BottomRight:
                    bottomRight.SetActive(active);
                    break;
                case UIList.Other:
                    other.SetActive(active);
                    break;
            }
        }

        public override void OnUpdateUI(float p_DeltaTime)
        {
        }
        
        protected Transform Find(string path)
        {
            return transform.Find(path);
        }
        
        protected T GetComponent<T>(string path)
        {
            return transform.Find(path).GetComponent<T>();
        }
        
        protected T AddComponent<T>(string path) where T : Component
        {
            return Find(path).gameObject.AddComponent<T>();
        }
    }
}

#endif