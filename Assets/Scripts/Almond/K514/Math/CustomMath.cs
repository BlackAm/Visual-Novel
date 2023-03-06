using UnityEngine;

namespace k514
{
    public static partial class CustomMath
    {
        /// <summary>
        /// CustomMath에서 정의한 구간 내의 역수 값을 리턴하는 메서드
        /// </summary>     
        public static float GetBoundedInverseValue(this float p_TargetValue)
        {
            var sign = Mathf.Sign(p_TargetValue);
            var absValue = Mathf.Abs(p_TargetValue);
            switch (absValue.GetProgressType01())
            {
                case Progress01Type.Zero:
                    return sign * Infinity;
                case Progress01Type.ZeroToOne:
                case Progress01Type.One:
                case Progress01Type.ZeroToNegative:
                case Progress01Type.OneToPositive:
                    return sign * 1f / Mathf.Clamp(absValue, Epsilon, Infinity);
                case Progress01Type.Infinity:
                    return 0f;
            }
            return 1f / p_TargetValue;
        }

        /// <summary>
        /// CustomMath에서 정의한 구간 내의 값을 리턴하는 메서드
        /// </summary>     
        public static float GetBoundedValue(this float p_TargetValue)
        {
            var sign = Mathf.Sign(p_TargetValue);
            var absValue = Mathf.Abs(p_TargetValue);
            switch (absValue.GetProgressType01())
            {
                case Progress01Type.Zero:
                    return 0f;
                case Progress01Type.ZeroToOne:
                case Progress01Type.One:
                case Progress01Type.ZeroToNegative:
                case Progress01Type.OneToPositive:
                    return sign * Mathf.Clamp(absValue, Epsilon, Infinity);
                case Progress01Type.Infinity:
                    return sign * Infinity;
            }
            return p_TargetValue;
        }

        
        /// <summary>
        /// 1바이트 정수 거듭제곱 메서드
        /// </summary>
        public static int PowByte(this int pBase, int pPow)
        {
            var lResult = 1;
            while (pPow > 0)
            {
                if ( (pPow & 1) == 1 ) lResult *= pBase;
                pBase *= pBase;
                pPow >>= 1;
            }
            return lResult;
        }
        
        /// <summary>
        /// 4바이트 정수 거듭제곱 메서드
        /// </summary>
        public static int PowInt(this int pBase, int pPow)
        {
            var lResult = 1;
            while (pPow > 0)
            {
                if ( (pPow & 1) == 1 ) lResult *= pBase;
                pBase *= pBase;
                pPow >>= 1;
            }
            return lResult;
        }

        public static bool IsPowerOfTwo(this int p_Value)
        {
            return (p_Value & (p_Value - 1)) == 0;
        }
        
        public static bool IsPowerOfTwoExceptZero(this int p_Value)
        {
            return (p_Value != 0) && p_Value.IsPowerOfTwo();
        }
        
        public static float SafeDivide(this float p_Divedend, float p_Divisor)
        {
            if (p_Divedend.IsReachedZero())
            {
                return 0f;
            }
            else
            {
                if (p_Divisor.IsReachedZero())
                {
                    return Infinity;
                }
                else
                {
                    return p_Divedend / p_Divisor;
                }
            }
        }

        public static float GetPitagorasValue(float p_A, float p_B)
        {
            return Mathf.Sqrt(p_A * p_A + p_B * p_B);
        }
        
        public static float GetPitagorasValue(float p_A, float p_B, float p_C)
        {
            return Mathf.Sqrt(p_A * p_A + p_B * p_B + p_C * p_C);
        }
    }
}