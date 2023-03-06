#if !SERVER_DRIVE
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using k514;
using UnityEngine.UI;

namespace UI2020
{
    public class Title : AbstractUI
    {
        public override void Initialize()
        {
            LoadUIObjectAsync("MenuUITitleUI.prefab",()=>
            {
                GetComponent<Button>("MenuUITitleUI/Button").onClick.AddListener(Enter);
                GetComponent<Text>("MenuUITitleUI/Text").text = LanguageManager.GetContent(200100);
            },ResourceLifeCycleType.Scene);
           
        }
        /// <summary>
        /// 화면 터치: 로그인 화면으로...
        /// </summary>
        private void Enter()
        {
            MenuUI.Instance.ChangeScene(MenuUI.MenuUIList.Login);
        }
    }
}
#endif