#if !SERVER_DRIVE
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI2020
{
    /*public class MenuButton : AbstractUI
    {
        public Button button;
        public Image image;
        public Text text;
        public GameObject updateMark;

        private void Awake()
        {
            button = GetComponent<Button>();
            image = GetComponent<Image>("Image");
            text = GetComponent<Text>("Text");
            updateMark = Find("UpdateMark").gameObject;
            
            updateMark.SetActive(false);
        }

        protected override void DisposeUnManaged()
        {
            base.DisposeUnManaged();
        }

        public MenuButton SetText(string p_Text)
        {
            if (text == null)
            {
                text = GetComponent<Text>("Text");
            }

            text.text = p_Text;
            return this;
        }
    }*/
}
#endif