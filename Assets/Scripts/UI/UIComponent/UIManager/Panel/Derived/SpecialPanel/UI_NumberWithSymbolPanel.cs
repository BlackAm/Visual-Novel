#if !SERVER_DRIVE
using UnityEngine;
using UnityEngine.UI;

namespace BlackAm
{
    public class UI_NumberWithSymbolPanel : UI_NumberPanel
    {
        #region <Fields>

        private Image _Symbol;
        
        #endregion

        #region <Callbacks>

        public override void OnSpawning()
        {
            base.OnSpawning();
            _Symbol = transform.FindRecursive<Image>("Image").Item2;
        }

        public override void OnPooling()
        {
            base.OnPooling();
            _Symbol.enabled = false;
        }

        #endregion
        
        #region <Methods>

        public void SetDamageSymbolImage(int p_SpriteSymbolIndex)
        {
            if (p_SpriteSymbolIndex < 10)
            {
                _Symbol.enabled = false;
                _Symbol.sprite = null;
            }
            else
            {
                _Symbol.enabled = true;
                _Symbol.sprite = SpriteSheetManager.GetInstanceUnSafe.GetNumberSprite(p_SpriteSymbolIndex);
            }
        }
        
        public override void SetColor(Color p_Color)
        {
            _IsColorChanged = true;
            
            foreach (var image in _AllImageGroup)
            {
                if (!ReferenceEquals(_Symbol, image))
                {
                    image.color = p_Color;
                }
            }
        }
        
        #endregion
    }
}
#endif