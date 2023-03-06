using UnityEngine;
using UnityEngine.UI;

namespace k514
{
    public struct ImagePreset
    {
        #region <Fields>

        public Image TargetImage;

        #endregion

        #region <Constructor>

        public ImagePreset(Image p_Image)
        {
            TargetImage = p_Image;
        }
            
        public ImagePreset(Image p_Image, Sprite p_Sprite)
        {
            TargetImage = p_Image;
            TargetImage.sprite = p_Sprite;
        }

        #endregion

        #region <Methods>

        public void SetSprite(Sprite p_Sprite)
        {
            TargetImage.sprite = p_Sprite;
        }

        public void ClearSprite()
        {
            TargetImage.sprite = null;
        }

        #endregion
    }
}