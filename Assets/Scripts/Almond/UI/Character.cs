#if !SERVER_DRIVE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace UI2020
{
    /// <summary>
    /// 캐릭터 씬
    /// </summary>
    public class Character : AbstractUI
    {
        public override void Initialize()
        {
            GetComponent<Button>("Top/Back").onClick.AddListener(Back);
            GetComponent<Button>("RightMenu/Costume").onClick.AddListener(GoStore);
        }

        /// <summary>
        /// 뒤로 가기(캐릭터선택으로)
        /// </summary>
        private void Back()
        {
            MenuUI.Instance.ChangeScene(MenuUI.MenuUIList.CharacterSelect);
        }

        /// <summary>
        /// 코스튬 상점으로 이동(외형,효과) - 테스트.
        /// </summary>
        private void GoStore()
        {
            MenuUI.Instance.ChangeScene(MenuUI.MenuUIList.EffectsStore);
        }
    }
   
}
#endif