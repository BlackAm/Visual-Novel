using UnityEngine;

namespace BlackAm
{
    public static partial class CustomMath
    {
        /// <summary>
        /// 랜덤한 부호(-1f, 1f)를 리턴하는 메서드
        /// </summary>
        public static float GetRandomSign()
        {
            return GetRandomBool() ? 1f : -1f;
        }

        /// <summary>
        /// 랜덤한 논리 값을 리턴하는 메서드
        /// </summary>
        public static bool GetRandomBool()
        {
            return Random.value > 0.5f;
        }

        /// <summary>
        /// 지정한 실수와 0 사이의 랜덤한 값을 리턴하는 메서드
        /// </summary>
        public static float GetRandom(this float p_Seed)
        {
            return Random.Range(0f, p_Seed);
        }
        
        /// <summary>
        /// 지정한 튜플 구간 사이의 랜덤한 값을 리턴하는 메서드
        /// </summary>
        public static float GetRandom(this (float t_Min, float t_Max) p_RandomInterval)
        {
            return Random.Range(p_RandomInterval.t_Min, p_RandomInterval.t_Max);
        }

        /// <summary>
        /// 특정 실수구간 [min, max]에 대해, [-max, -min] U [min, max] 를 만족하는 실수를 리턴하는 메서드
        /// </summary>
        public static float RandomSymmetric(float min, float max)
        {
            return GetRandomSign() * Random.Range(min,max);
        }
        
        /// <summary>
        /// 특정 실수구간 [min, max]에 대해, [-max, -min] U [min, max] 를 만족하는 실수를 리턴하는 메서드
        /// </summary>
        public static float RandomSymmetric((float t_Min, float t_Max) range)
        {
            return RandomSymmetric(range.t_Min, range.t_Max);
        }

        /// <summary>
        /// 특정 실수구간 [min, max]에 대해, [-max, -min] U [min, max] 를 만족하는 각각의 원소를 가지는 벡터를 리턴하는 메서드
        /// </summary>
        public static Vector3 RandomSymmetricVector(XYZType p_XYZType, float min, float max)
        {
            switch (p_XYZType)
            {
                case XYZType.X:
                    return RandomSymmetric(min, max) * Vector3.right;
                case XYZType.Y:
                    return RandomSymmetric(min, max) * Vector3.up;
                case XYZType.Z:
                    return RandomSymmetric(min, max) * Vector3.forward;
                case XYZType.XY:
                    return RandomSymmetric(min, max) * Vector3.right
                        + RandomSymmetric(min, max) * Vector3.up;
                case XYZType.YZ:
                    return RandomSymmetric(min, max) * Vector3.up
                           + RandomSymmetric(min, max) * Vector3.forward;
                case XYZType.ZX:
                    return RandomSymmetric(min, max) * Vector3.forward
                           + RandomSymmetric(min, max) * Vector3.right;
                case XYZType.XYZ:
                    return RandomSymmetric(min, max) * Vector3.right
                           + RandomSymmetric(min, max) * Vector3.up
                           + RandomSymmetric(min, max) * Vector3.forward;
                default:
                case XYZType.None:
                    return Vector3.zero;
            }
        }
        
        /// <summary>
        /// 특정 실수구간 [min, max]에 대해, [-max, -min] U [min, max]를 만족하는 각각의 원소를 가지는 벡터를 리턴하는 메서드
        /// </summary>
        public static Vector3 RandomSymmetricVector((XYZType t_XYZType, float t_Min, float t_Max) range)
        {
            return RandomSymmetricVector(range.t_XYZType, range.t_Min, range.t_Max);
        }

        /// <summary>
        /// 특정 실수구간 [min, max]의 임의의 값을 각각의 원소로 가지는 벡터를 리턴하는 메서드
        /// </summary>
        public static Vector3 RandomVector(XYZType p_XYZType, float min, float max)
        {
            switch (p_XYZType)
            {
                case XYZType.X:
                    return Random.Range(min, max) * Vector3.right;
                case XYZType.Y:
                    return Random.Range(min, max) * Vector3.up;
                case XYZType.Z:
                    return Random.Range(min, max) * Vector3.forward;
                case XYZType.XY:
                    return Random.Range(min, max) * Vector3.right
                           + Random.Range(min, max) * Vector3.up;
                case XYZType.YZ:
                    return Random.Range(min, max) * Vector3.up
                           + Random.Range(min, max) * Vector3.forward;
                case XYZType.ZX:
                    return Random.Range(min, max) * Vector3.forward
                           + Random.Range(min, max) * Vector3.right;
                case XYZType.XYZ:
                    return Random.Range(min, max) * Vector3.right
                           + Random.Range(min, max) * Vector3.up
                           + Random.Range(min, max) * Vector3.forward;
                default:
                case XYZType.None:
                    return Vector3.zero;
            }
        }
        
        /// <summary>
        /// 특정 실수구간 [min, max]의 임의의 값을 각각의 원소로 가지는 벡터를 리턴하는 메서드
        /// </summary>
        public static Vector3 RandomVector((XYZType t_XYZType, float t_Min, float t_Max) range)
        {
            return RandomVector(range.t_XYZType, range.t_Min, range.t_Max);
        }
        
        /// <summary>
        /// 특정 실수구간 [min, max]에 대해, [-max, -min] U [min, max] 를 만족하는 각각의 원소를 가지는 벡터만큼 멀어진 좌표를 리턴하는 메서드
        /// </summary>
        public static Vector3 RandomSymmetricPosition(this Transform p_Pivot, XYZType p_XYZType, float min, float max)
        {
            return p_Pivot.position + RandomSymmetricVector(p_XYZType, min, max);
        }

        /// <summary>
        /// 특정 실수구간 [min, max]의 임의의 값을 각각의 원소로 가지는 벡터만큼 멀어진 좌표를 리턴하는 메서드
        /// </summary>
        public static Vector3 RandomPosition(this Transform p_Pivot, XYZType p_XYZType, float min, float max)
        {
            return p_Pivot.position + RandomVector(p_XYZType, min, max);
        }

        /// <summary>
        /// 특정 실수구간 [min, max]에 대해, [-max, -min] U [min, max] 를 만족하는 각각의 원소를 가지는 벡터만큼 멀어진 좌표를 리턴하는 메서드
        /// </summary>
        public static Vector3 RandomSymmetricPosition(this Vector3 p_Pivot, XYZType p_XYZType, float min, float max)
        {
            return p_Pivot + RandomSymmetricVector(p_XYZType, min, max);
        }

        /// <summary>
        /// 특정 실수구간 [min, max]의 임의의 값을 각각의 원소로 가지는 벡터만큼 멀어진 좌표를 리턴하는 메서드
        /// </summary>
        public static Vector3 RandomPosition(this Vector3 p_Pivot, XYZType p_XYZType, float min, float max)
        {
            return p_Pivot + RandomVector(p_XYZType, min, max);
        }
        
        /// <summary>
        /// 지정한 정수값을 통해 호출하는 경우, 해당 값의 크기에 따라 일정 값을 빼고, 뺀값 만큼 리턴하는 메서드
        /// 예를 들어, 어떤 정수 a, b에 대해 b += a.MoveValue(10, 0, out a); 코드는 a에서 일정값을 빼고 b에 더하는 기능을 한다.
        /// 이 때, a가 0이되는 경우에는 뺄 수 있는 값도 0이기 때문에 리턴값도 0이된다.
        ///
        /// 또한, p_UpperBoundNumberingCount 파라미터는 숫자 크기를 가늠할 자릿수를 나타내는 파라미터로 3을 넣었다면
        /// 한번에 감소할 수 있는 최대치는 10^3인 1000이 된다.
        ///
        /// p_UpperBoundNumberingCountOffset 파라미터는 감소최대치에 관여하는 인수로,
        /// p_UpperBoundNumberingCount가 a이었을 때, p_UpperBoundNumberingCountOffset가 b라면 최대 감소수치는 10^(a-b)가 된다.
        /// 
        /// </summary>
        public static int MoveValue(this int p_TargetValue, int p_UpperBoundNumberingCount, int p_UpperBoundNumberingCountOffset, out int p_PrevValue)
        {
            if (p_TargetValue < 1)
            {
                p_PrevValue = p_TargetValue;
                return 0;
            }
            else
            {
                int subtractVal = 0;
                int numberingIndex = p_UpperBoundNumberingCount - p_UpperBoundNumberingCountOffset;
                int currentUpperBound = 10.PowInt(numberingIndex);
                while (subtractVal == 0)
                {
                    var currentUpperBoundFlat = 10.PowInt(numberingIndex - 1);
                    if (p_TargetValue > currentUpperBound - 1)
                    {
                        subtractVal = Random.Range(currentUpperBoundFlat, currentUpperBound + 1);
                    }
                    else
                    {
                        currentUpperBound = currentUpperBoundFlat;
                        numberingIndex--;
                    }
                }
                subtractVal = Mathf.Min(subtractVal, p_TargetValue);
                p_PrevValue = p_TargetValue - subtractVal;
                return subtractVal;
            }
        }
    }
}