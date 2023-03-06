#if !SERVER_DRIVE
using UnityEngine;
using UnityEngine.UI;

namespace UI2020
{
    public class DropItem : AbstractUI
    {
        private Image _bg, _itemImage;
        private Text _itemName;

        public float alpha;

        public void Initialize()
        {
            _bg = GetComponent<Image>("BG");
            _itemImage = GetComponent<Image>("ItemImage");
            _itemName = GetComponent<Text>("ItemName");
        }

        public void SetAlpha(float alpha)
        {
            this.alpha = alpha;
            var color = new Color(1f,1f,1f,alpha);
            _itemName.color = _itemImage.color = _bg.color = color;
        }

        public void SetName(string itemName)
        {
            _itemName.text = itemName;
        }

        public void SetSprite(Sprite sprite)
        {
            _itemImage.enabled = (sprite != null);
            _itemImage.sprite = sprite;
        }
        
    }
}
#endif