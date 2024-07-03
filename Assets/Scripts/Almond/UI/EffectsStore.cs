#if !SERVER_DRIVE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI2020
{
    public class EffectsStore : AbstractUI
    {
        public override void Initialize()
        {
            GetComponent<Button>("Back").onClick.AddListener(Back);
        }
        private void Back()
        {
            MenuUI.Instance._effect.SetActive(false);
        }
    }
}
#endif