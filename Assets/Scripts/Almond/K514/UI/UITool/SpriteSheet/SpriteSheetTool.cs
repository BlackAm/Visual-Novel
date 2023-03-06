using UnityEngine;
using UnityEngine.UI;

namespace k514
{
    public partial class UITool
    {
        #region <Enums>

        public enum AnimationSpriteType
        {
            None,
            Gary,
            
            PromotionCount, PromotionEffect,
        }

        #endregion

        #region <Methods>

#if !SERVER_DRIVE
        public static SpriteAnimationPreset GetSpriteAnimationPreset(this SpriteSheetCarrier p_Carrier,
            Transform p_TargetWrapper, string p_ImageObjectName, int p_StartIndex, int p_EndIndex, 
            float p_AnimationDuration, int p_AnimationLoopCount)
        {
            var result 
                = new SpriteAnimationPreset
                (
                    p_TargetWrapper.FindRecursive<Image>(p_ImageObjectName).Item2,
                    p_Carrier.SelectList(p_StartIndex, p_EndIndex), p_AnimationDuration, p_AnimationLoopCount
                );

            return result;
        }
#endif

        #endregion
    }
}