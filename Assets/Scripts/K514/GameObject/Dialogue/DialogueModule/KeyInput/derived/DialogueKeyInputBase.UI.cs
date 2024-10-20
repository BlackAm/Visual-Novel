using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackAm
{
    public partial class DialogueKeyInputBase
    {
        public bool OnHandleShowHistory()
        {
            return true;
        }
        
        public bool OnHandleHideUI()
        {
            // 선택지 UI가 켜져 있는 상태에서 작동하지 않도록 하는 조건문
            if (MainGameUI.Instance.functionUI.IsOpenUI(FunctionUI.UIIndex.Setting)) return false;
            
            MainGameUI.Instance.mainUI.ActiveMainUI(MainUI.UIList.BottomCenter, !MainGameUI.Instance.mainUI.IsUIActive(MainUI.UIList.BottomCenter));
            // MainGameUI.Instance.mainUI.ActiveMainUI(MainUI.UIList.MiddleCenter, !MainGameUI.Instance.mainUI.IsUIActive(MainUI.UIList.MiddleCenter));
            TopMenu.Instance.HideTopMenu(!TopMenu.Instance.IsActive);
            if (MainGameUI.Instance.mainUI.IsSelectDialogueExist())
                MainGameUI.Instance.mainUI.ActiveMainUI(MainUI.UIList.TopCenter, !MainGameUI.Instance.mainUI.IsUIActive(MainUI.UIList.TopCenter));
            return true;
        }
    }
}
