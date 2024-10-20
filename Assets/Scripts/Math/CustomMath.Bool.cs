using System.Collections.Generic;
using UnityEngine;

namespace BlackAm
{
    public static partial class CustomMath
    {
        /// <summary>
        /// 직선 / 반직선 최대 길이
        /// </summary>
        public const float MaxLineLength = 1_000f;

        /// <summary>
        /// 리앙 바스키 알고리즘의 경로 비교 시에 사용하는 비교 상수
        /// </summary>
        public const float LiangBarskyCompareThreshold = 0.1f;
        
        /// <summary>
        /// 벡터 성분 등을 표현하는 열거형 상수
        /// </summary>
        public enum XYZType
        {
            None = 0,
            X = 1, Y = 2, Z = 4,
            XY = 3, YZ = 6, ZX = 5,
            XYZ = 7,
        }

        /// <summary>
        /// 전 실수 구간을 0과 1을 기준으로 나누는 열거형 상수 타입
        /// </summary>
        public enum Progress01Type
        {
            /// <summary>
            /// (-∞,0)
            /// </summary>
            ZeroToNegative,
            /// <summary>
            /// 0
            /// </summary>
            Zero,
            /// <summary>
            /// (0, 1)
            /// </summary>
            ZeroToOne,
            /// <summary>
            /// 1
            /// </summary>
            One,
            /// <summary>
            /// (1, ∞)
            /// </summary>
            OneToPositive,
            /// <summary>
            /// ±∞
            /// </summary>
            Infinity
        }
        
        /// <summary>
        /// 부호 타입
        /// </summary>
        public enum Significant
        {
            None,
            Minus,
            Zero,
            Plus,
        }
        
        /// <summary>
        /// 지정한 실수 값이 구간 [-p_Threshold, p_Threshold] 에 진입한 경우 해당 값을 0값으로 교체하는 메서드
        /// </summary>
        public static float FloorFloatValue(this float p_Target, float p_Threshold)
        {
            if (p_Target.IsReachedZero(p_Threshold))
            {
                p_Target = 0f;
            }
            return p_Target;
        }
        
        /// <summary>
        /// 지정한 벡터 값의 임의의 원소가 구간 [-p_Threshold, p_Threshold] 에 진입한 경우 해당 값을 0값으로 교체하는 메서드
        /// </summary>
        public static Vector3 FloorVectorValue(this Vector3 p_Target, float p_Threshold)
        {
            return new Vector3(p_Target.x.FloorFloatValue(p_Threshold), p_Target.y.FloorFloatValue(p_Threshold), p_Target.z.FloorFloatValue(p_Threshold));
        }

        /// <summary>
        /// 지정한 벡터의 원소쌍에 대해, 특정 논리조건을 만족하는 원소 타입을 리턴하는 메서드
        /// </summary>
        public static XYZType GetXYZType_ReachedInfinity(this Vector3 p_TargetVector)
        {
            var xPred = IsReachedInfinity(p_TargetVector.x) ? 1 << 0 : 0;
            var yPred = IsReachedInfinity(p_TargetVector.y) ? 1 << 1 : 0;
            var zPred = IsReachedInfinity(p_TargetVector.z) ? 1 << 2 : 0;

            return (XYZType) (xPred + yPred + zPred);
        }

        /// <summary>
        /// 지정한 값이 현재 위치한 구간을 리턴하는 메서드
        /// </summary>
        public static Progress01Type GetProgressType01(this float p_Target)
        {
            if (p_Target < 0f)
            {
                return Progress01Type.ZeroToNegative;
            }
            else if (p_Target < Epsilon)
            {
                return Progress01Type.Zero;
            }
            else if (p_Target < 1f)
            {
                return Progress01Type.ZeroToOne;
            }
            else if (p_Target < 1f + Epsilon)
            {
                return Progress01Type.One;
            }
            else if(p_Target < Infinity)
            {
                return Progress01Type.OneToPositive;
            }
            else
            {
                return Progress01Type.Infinity;
            }
        }
        
        /// <summary>
        /// 지정한 실수의 부호를 리턴하는 메서드
        /// </summary>
        public static Significant GetSignificant(this float p_Target)
        {
            if (p_Target.IsReachedZero())
            {
                return Significant.Zero;
            }
            else
            {
                return p_Target > 0f ? Significant.Plus : Significant.Minus;
            }
        }

    }
}