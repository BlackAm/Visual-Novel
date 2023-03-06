#if !SERVER_DRIVE
using UnityEngine;
using UnityEngine.UI;
#endif

namespace k514
{
    public static partial class UITool
    {
        #region <Enums>

#if !SERVER_DRIVE
        public enum XAnchorType
        {
            Left,
            Center,
            Right,
            Stretch
        }

        public enum YAnchorType
        {
            Top,
            Middle,
            Bottom,
            Stretch
        }
#endif

        #endregion

        #region <Methods>

#if !SERVER_DRIVE

        public static void SetImageAlpha(this Image p_TargetImage, float p_TargetAlpha)
        {
            p_TargetImage.SetImageAlpha(p_TargetImage.color, p_TargetAlpha);
        }
        
        public static void SetImageAlpha(this Image p_TargetImage, Color p_Origin, float p_TargetAlpha)
        {
            p_TargetImage.color = p_TargetImage.color.SetAlpha(p_TargetAlpha);
        }
             
        public static void SetTextAlpha(this Text p_TargetText, float p_TargetAlpha)
        {
            p_TargetText.SetTextAlpha(p_TargetText.color, p_TargetAlpha);
        }
        
        public static void SetTextAlpha(this Text p_TargetText, Color p_Origin, float p_TargetAlpha)
        {
            p_TargetText.color = p_TargetText.color.SetAlpha(p_TargetAlpha);
        }
        
#endif
        #endregion
    }
}