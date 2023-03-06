#if !SERVER_DRIVE
using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI2020
{
    public class UITop : AbstractUI
    {
        private Text _titleText;
        private Text _gold, _ancientGold, _diamond;
        public GameObject title, property;
        public Button closeBT;
        public GameObject normal;

        public override void Initialize()
        {
            if (!ReferenceEquals(null, _UIObject))
                return;

            LoadUIObjectAsync("MainGameTopUI.prefab",()=>
            {
                closeBT = GetComponent<Button>("MainGameTopUI/Close");
                closeBT.onClick.AddListener(OnClickCloseButton);
                title = Find("MainGameTopUI/PanelName").gameObject;
                property = Find("MainGameTopUI/Money").gameObject;
                normal = Find("MainGameTopUI/PanelName/Image/Normal").gameObject;

                _titleText = GetComponent<Text>("MainGameTopUI/PanelName/Text");

                _diamond = GetComponent<Text>("MainGameTopUI/Money/Dia/Text");
                _gold = GetComponent<Text>("MainGameTopUI/Money/Gold/Text");
                _ancientGold = GetComponent<Text>("MainGameTopUI/Money/Silver/Text");
                // temporary setting
                SetDiamond(0);
                SetGold(0);
                SetAncientGold(0);
            });
            
            SetActive(false);
        }
        
        public void SetTitle(string title)
        {
            _titleText.text = title;
        }

        public void SetGold(int value)
        {
            _gold.text = value.ToString();
        }

        public void SetAncientGold(int value)
        {
            _ancientGold.text = value.ToString();
        }
        
        public void SetDiamond(int value)
        {
            _diamond.text = value.ToString();
        }

        public void OnClickCloseButton()
        {
            MainGameUI.Instance.functionUI.CloseUI();
        }

        public void ActiveTop(bool active)
        {
            title.SetActive(active);
            property.SetActive(active);
            closeBT.gameObject.SetActive(active);
        }
    }
}
#endif