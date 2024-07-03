#if !SERVER_DRIVE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI2020
{
    /// <summary>
    /// 외형상점 씬
    /// </summary>
    public class ExteriorStore : AbstractUI
    {
        public override void Initialize()
        {
            GetComponent<Button>("LeftMenu/Back").onClick.AddListener(Back);
        }

        private void Back()
        {
            MenuUI.Instance._exter.SetActive(false);
        }
    }
}
#endif