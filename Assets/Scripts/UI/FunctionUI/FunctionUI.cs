#if !SERVER_DRIVE
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlackAm
{
    public class FunctionUI : AbstractUI
    {
        public Setting setting;
        // public SaveLoad saveLoad;
        public DialogueHistory dialogueHistory;
        
        private GameObject mainBack;
        public AbstractUI _curUI;
        private UIIndex _curView;
        public UIIndex CurUI
        {
            get => _curView;
            set => _curView = value;
        }
        public enum UIIndex
        {
            None,
            Setting,
            DialogueHistory,
        }
        
        /// 해당 창이 열려있는지 리턴합니다.
        public bool IsOpenUI(UIIndex view)
        {
            return CurUI == view;
        }

        public bool IsUIActive()
        {
            return _curUI.IsActive;
        }
        
        public override void OnSceneStarted()
        {
            
            // 씬 시작할 때마다 리스트가 한 번 생성되게 했습니다.
            // 이후 서버통신 시 이 부분은 수정되어야 합니다.
        }
        
        public override void Initialize()
        {
            setting = AddComponent<Setting>("Setting");
            dialogueHistory = AddComponent<DialogueHistory>("DialogueHistory");
            
            setting.Initialize();
            dialogueHistory.Initialize();
            
            setting.SetActive(false);
            dialogueHistory.SetActive(false);
        }

        public void OpenUI(UIIndex index)
        {
            if (_curUI != null)
            {
                _curUI.SetActive(false);
                _curUI.OnDisableUI();
            }
            switch (index)
            {
                case UIIndex.Setting:
                    _curUI = setting;
                    break;
                case UIIndex.DialogueHistory:
                    _curUI = dialogueHistory;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }

            _curUI.SetActive(true);
            _curUI.OnActive();
            CurUI = index;
        }
        public override void OnUpdateUI(float p_DeltaTime)
        {
            if(!ReferenceEquals(null,_curUI))
                _curUI.OnUpdateUI(p_DeltaTime);
        }
        public void CloseUI()
        {
            if(!ReferenceEquals(_curUI, null)) _curUI.SetActive(false);
            CurUI = UIIndex.None;
        }

        public void HideUI(bool p_Flag)
        {
            if(!ReferenceEquals(null, _curUI)) _curUI.SetActive(p_Flag);
        }
        
        public void GetSprite(int p_Index, Image p_Image)
        {
            var loadResult = LoadAssetManager.GetInstanceUnSafe.LoadAsset<Sprite>(
                ResourceType.Image, ResourceLifeCycleType.Scene,
                ImageNameTableData.GetInstanceUnSafe.GetTableData(p_Index).ResourceName);
            
            p_Image.sprite = loadResult.Item2;
        }
    }
}
#endif