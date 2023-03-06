using UnityEngine;

namespace k514
{
    public static partial class CustomMath
    {
        #region <Fields>

        /// <summary>
        /// 부동소수점 하한 값
        /// </summary>
        public const float Epsilon = 0.000_1f;
        public const float HalfEpsilon = 0.000_05f;
        public const float NegativeEpsilon = -Epsilon;
        public const float NegativeHalfEpsilon = -HalfEpsilon;
        
        /// <summary>
        /// 부동소수점 상한 값
        /// </summary>
        public const float Infinity = 100_000_000f;
        public const float NegativeInfinity = -Infinity;

        /// <summary>
        /// 부동소수점 하한 벡터
        /// </summary>
        public static readonly Vector3 EpsilonVector = Epsilon * Vector3.one;
        public static readonly Vector3 HalfEpsilonVector = HalfEpsilon * Vector3.one;

        #endregion

        #region <Method/Float>

        /// <summary>
        /// 특정 실수 값이 0인지 검증하는 메서드
        /// </summary>
        public static bool IsReachedZero(this float p_Target, float p_Threshold = Epsilon) => Mathf.Abs(p_Target) <= p_Threshold;
        
        /// <summary>
        /// 특정 실수 값이 실수 하한인지 검증하는 메서드
        /// </summary>
        public static bool IsReachedEpsilon(this float p_Target, float p_Threshold = Epsilon) => Mathf.Abs(Epsilon - p_Target) <= p_Threshold;
        
        /// <summary>
        /// 특정 실수 값이 1인지 검증하는 메서드
        /// </summary>
        public static bool IsReachedOne(this float p_Target, float p_Threshold = Epsilon) => Mathf.Abs(1f - p_Target) <= p_Threshold;
        
        /// <summary>
        /// 특정 실수 값이 실수 상한인지 검증하는 메서드
        /// </summary>
        public static bool IsReachedInfinity(this float p_Target) => p_Target >= Infinity || p_Target <= NegativeInfinity;
        
        /// <summary>
        /// 지정한 값이 특정 값에 도달했는지 검증하는 메서드
        /// </summary>
        public static bool IsReachedValue(this float p_Target, float p_CompareValue, float p_Threshold = Epsilon) => Mathf.Abs(p_CompareValue - p_Target) <= p_Threshold;

        #endregion

        #region <Method/Vector2>

        public static bool IsReachedEpsilon(this Vector2 p_TargetVector, float p_Threshold = Epsilon)
        {
            return Mathf.Abs(Epsilon - p_TargetVector.x) <= p_Threshold
                   && Mathf.Abs(Epsilon - p_TargetVector.y) <= p_Threshold;
        }

        #endregion
        
        #region <Method/Vector3>

        public static bool IsReachedZero(this Vector3 p_TargetVector, float p_Threshold = Epsilon)
        {
            return Mathf.Abs(p_TargetVector.x) <= p_Threshold
                   && Mathf.Abs(p_TargetVector.y) <= p_Threshold
                   && Mathf.Abs(p_TargetVector.z) <= p_Threshold;
        }

        public static bool IsReachedEpsilon(this Vector3 p_TargetVector, float p_Threshold = Epsilon)
        {
            return Mathf.Abs(Epsilon - p_TargetVector.x) <= p_Threshold
                   && Mathf.Abs(Epsilon - p_TargetVector.y) <= p_Threshold
                   && Mathf.Abs(Epsilon - p_TargetVector.z) <= p_Threshold;
        }
        
        public static bool IsReachedOne(this Vector3 p_TargetVector, float p_Threshold = Epsilon)
        {
            return Mathf.Abs(1f - p_TargetVector.x) <= p_Threshold
                   && Mathf.Abs(1f - p_TargetVector.y) <= p_Threshold
                   && Mathf.Abs(1f - p_TargetVector.z) <= p_Threshold;
        }

        public static bool IsReachedInfinity(this Vector3 p_TargetVector)
        {
            return p_TargetVector.x >= Infinity || p_TargetVector.x <= NegativeInfinity
                   || p_TargetVector.y >= Infinity || p_TargetVector.y <= NegativeInfinity
                   || p_TargetVector.z >= Infinity || p_TargetVector.z <= NegativeInfinity;
        }

        public static bool IsReachedValue(this Vector3 p_TargetVector, float p_CompareValue, float p_Threshold = Epsilon)
        {
            return Mathf.Abs(p_CompareValue - p_TargetVector.x) <= p_Threshold
                   && Mathf.Abs(p_CompareValue - p_TargetVector.y) <= p_Threshold
                   && Mathf.Abs(p_CompareValue - p_TargetVector.z) <= p_Threshold;
        }

        #endregion
    }
}