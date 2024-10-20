using UnityEngine;

namespace BlackAm
{
    public static partial class CustomMath
    {
        public static Color SetAlpha(this Color p_TargetVector, float p_AlphaValue01)
        {
            return new Color(p_TargetVector.r, p_TargetVector.b, p_TargetVector.g, p_AlphaValue01);
        }
        
        public static void SetAlphaRef(this ref Color p_TargetVector, float p_AlphaValue01)
        {
            p_TargetVector = new Color(p_TargetVector.r, p_TargetVector.b, p_TargetVector.g, p_AlphaValue01);
        }
     
        public static bool IsColorEquals(this Color p_TargetVector, Color p_CompareColor)
        {
            return p_TargetVector.r.IsReachedValue(p_CompareColor.r)
                   && p_TargetVector.b.IsReachedValue(p_CompareColor.b)
                   && p_TargetVector.g.IsReachedValue(p_CompareColor.g)
                   && p_TargetVector.a.IsReachedValue(p_CompareColor.r);
        }
        
        public static bool IsColorBlack(this Color p_TargetVector)
        {
            return p_TargetVector.r.IsReachedZero()
                   && p_TargetVector.g.IsReachedZero()
                   && p_TargetVector.b.IsReachedZero();
        }
        
        public static bool IsColorWhite(this Color p_TargetVector)
        {
            return p_TargetVector.r.IsReachedOne()
                   && p_TargetVector.g.IsReachedOne()
                   && p_TargetVector.b.IsReachedOne();
        }
        
        public static bool IsColorTransparent(this Color p_TargetVector)
        {
            return p_TargetVector.a.IsReachedZero();
        }
        
        public static bool IsColorOpaque(this Color p_TargetVector)
        {
            return p_TargetVector.a.IsReachedOne();
        }
    }
}