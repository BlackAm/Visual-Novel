using UnityEngine;

namespace BlackAm
{
    public static partial class CustomMath
    {
        #region <Method/ShortCut/DirectionVector>

        public static Vector3 GetDirectionVectorTo(this TransformTool.AffineCachePreset p_StartAffine, TransformTool.AffineCachePreset p_TargetAffine)
        {
            return p_TargetAffine.Position - p_StartAffine.Position;
        }
        public static Vector3 GetDirectionVectorTo(this Transform p_StartTransform, Transform p_TargetTransform)
        {
            return p_TargetTransform.position - p_StartTransform.position;
        }
        public static Vector3 GetDirectionVectorTo(this Vector3 p_StartVector, Vector3 p_TargetVector)
        {
            return p_TargetVector - p_StartVector;
        }
        public static Vector3 GetDirectionVectorTo(this TransformTool.AffineCachePreset p_StartAffine, Transform p_TargetTransform)
        {
            return p_TargetTransform.position - p_StartAffine.Position;
        }
        public static Vector3 GetDirectionVectorTo(this TransformTool.AffineCachePreset p_StartAffine, Vector3 p_TargetVector)
        {
            return p_TargetVector - p_StartAffine.Position;
        }
        public static Vector3 GetDirectionVectorTo(this Transform p_StartTransform, TransformTool.AffineCachePreset p_TargetAffine)
        {
            return p_TargetAffine.Position - p_StartTransform.position;
        }
        public static Vector3 GetDirectionVectorTo(this Transform p_StartTransform, Vector3 p_TargetPosition)
        {
            return p_TargetPosition - p_StartTransform.position;
        }
        public static Vector3 GetDirectionVectorTo(this Vector3 p_StartVector, TransformTool.AffineCachePreset p_TargetAffine)
        {
            return p_TargetAffine.Position - p_StartVector;
        }
        public static Vector3 GetDirectionVectorTo(this Vector3 p_StartVector, Transform p_TargetTransform)
        {
            return p_TargetTransform.position - p_StartVector;
        }

        #endregion

        #region <Method/ShortCut/UnitDirectionVector>
        
        public static Vector3 GetDirectionUnitVectorTo(this TransformTool.AffineCachePreset p_StartAffine, TransformTool.AffineCachePreset p_TargetAffine)
        {
            return (p_TargetAffine.Position - p_StartAffine.Position).normalized;
        }
        public static Vector3 GetDirectionUnitVectorTo(this Transform p_StartTransform, Transform p_TargetTransform)
        {
            return (p_TargetTransform.position - p_StartTransform.position).normalized;
        }
        public static Vector3 GetDirectionUnitVectorTo(this Vector3 p_StartVector, Vector3 p_TargetVector)
        {
            return (p_TargetVector - p_StartVector).normalized;
        }
        public static Vector3 GetDirectionUnitVectorTo(this TransformTool.AffineCachePreset p_StartAffine, Transform p_TargetTransform)
        {
            return (p_TargetTransform.position - p_StartAffine.Position).normalized;
        }
        public static Vector3 GetDirectionUnitVectorTo(this TransformTool.AffineCachePreset p_StartAffine, Vector3 p_TargetVector)
        {
            return (p_TargetVector - p_StartAffine.Position).normalized;
        }
        public static Vector3 GetDirectionUnitVectorTo(this Transform p_StartTransform, TransformTool.AffineCachePreset p_TargetAffine)
        {
            return (p_TargetAffine.Position - p_StartTransform.position).normalized;
        }
        public static Vector3 GetDirectionUnitVectorTo(this Transform p_StartTransform, Vector3 p_TargetPosition)
        {
            return (p_TargetPosition - p_StartTransform.position).normalized;
        }
        public static Vector3 GetDirectionUnitVectorTo(this Vector3 p_StartVector, TransformTool.AffineCachePreset p_TargetAffine)
        {
            return (p_TargetAffine.Position - p_StartVector).normalized;
        }
        public static Vector3 GetDirectionUnitVectorTo(this Vector3 p_StartVector, Transform p_TargetTransform)
        {
            return (p_TargetTransform.position - p_StartVector).normalized;
        }
        
        #endregion
        
        #region <Method/ShortCut/SqrDistance>
        
        public static float GetSqrDistanceTo(this TransformTool.AffineCachePreset p_StartAffine, TransformTool.AffineCachePreset p_TargetAffine)
        {
            return (p_TargetAffine.Position - p_StartAffine.Position).sqrMagnitude;
        }
        public static float GetSqrDistanceTo(this Transform p_StartTransform, Transform p_TargetTransform)
        {
            return (p_TargetTransform.position - p_StartTransform.position).sqrMagnitude;
        }
        public static float GetSqrDistanceTo(this Vector3 p_StartVector, Vector3 p_TargetVector)
        {
            return (p_TargetVector - p_StartVector).sqrMagnitude;
        }
        public static float GetSqrDistanceTo(this TransformTool.AffineCachePreset p_StartAffine, Transform p_TargetTransform)
        {
            return (p_TargetTransform.position - p_StartAffine.Position).sqrMagnitude;
        }
        public static float GetSqrDistanceTo(this TransformTool.AffineCachePreset p_StartAffine, Vector3 p_TargetVector)
        {
            return (p_TargetVector - p_StartAffine.Position).sqrMagnitude;
        }
        public static float GetSqrDistanceTo(this Transform p_StartTransform, TransformTool.AffineCachePreset p_TargetAffine)
        {
            return (p_TargetAffine.Position - p_StartTransform.position).sqrMagnitude;
        }
        public static float GetSqrDistanceTo(this Transform p_StartTransform, Vector3 p_TargetPosition)
        {
            return (p_TargetPosition - p_StartTransform.position).sqrMagnitude;
        }
        public static float GetSqrDistanceTo(this Vector3 p_StartVector, TransformTool.AffineCachePreset p_TargetAffine)
        {
            return (p_TargetAffine.Position - p_StartVector).sqrMagnitude;
        }
        public static float GetSqrDistanceTo(this Vector3 p_StartVector, Transform p_TargetTransform)
        {
            return (p_TargetTransform.position - p_StartVector).sqrMagnitude;
        }
        
        #endregion
    }
}