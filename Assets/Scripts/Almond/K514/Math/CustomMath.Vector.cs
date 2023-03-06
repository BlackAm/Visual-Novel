using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace k514
{
    /// <summary>
    /// 방향키 방향 조합 타입
    /// </summary>
    public enum ArrowType
    {
        None = 0,
            
        SoloUp = 1,
        SoloLeft = 2,
        SoloDown = 4,
        SoloRight = 8,
            
        UpLeft = 3, 
        LeftDown = 6, 
        DownRight = 12, 
        RightUp = 9,
            
        // 교착 상태들
        LeftRight = 10,
        UpDown = 5,
        UpLeftDown = 7,
        LeftDownRight = 14,
        DownRightUp = 13,
        RightUpLeft = 11,
        UpLeftDownRight = 15,
    }

    /// <summary>
    /// 벡터 데이터를 보정하는 타입
    /// </summary>
    public enum VectorCorrectType 
    {
        /// <summary>
        /// 보정 없음, 그대로 가져온다.
        /// </summary>
        None,
            
        /// <summary>
        /// X, Z 요소만 남긴다.
        /// </summary>
        TurnToXZ,
    }
    
    public static partial class CustomMath
    {
        public const int AFFINE_CORRECT_FACTOR = 1_000_000;
        public const float AFFINE_CORRECT_FACTOR_INV = 1f / AFFINE_CORRECT_FACTOR;
        
        public static Dictionary<ArrowType, Vector3> ArrowWorldVectorCollection 
            = new Dictionary<ArrowType, Vector3>
            {
                {ArrowType.None, Vector3.zero},
                {ArrowType.UpDown, Vector3.zero},
                {ArrowType.LeftRight, Vector3.zero},
                {ArrowType.UpLeftDown, Vector3.zero},
                {ArrowType.LeftDownRight, Vector3.zero},
                {ArrowType.DownRightUp, Vector3.zero},
                {ArrowType.RightUpLeft, Vector3.zero},
                {ArrowType.UpLeftDownRight, Vector3.zero},
            
                {ArrowType.SoloLeft, Vector3.left},
                {ArrowType.LeftDown, (Vector3.left + Vector3.back).normalized},
                {ArrowType.SoloDown, Vector3.back},
                {ArrowType.DownRight, (Vector3.back + Vector3.right).normalized},
                {ArrowType.SoloRight, Vector3.right},
                {ArrowType.RightUp, (Vector3.right + Vector3.forward).normalized},
                {ArrowType.SoloUp, Vector3.forward},
                {ArrowType.UpLeft, (Vector3.forward + Vector3.left).normalized},
            };
        
        public static Dictionary<ArrowType, Vector2> ArrowViewPortVectorCollection_RightHandPivotSystem 
            = new Dictionary<ArrowType, Vector2>
            {
                {ArrowType.None, Vector2.zero},
                {ArrowType.UpDown, Vector2.zero},
                {ArrowType.LeftRight, Vector2.zero},
                {ArrowType.UpLeftDown, Vector2.zero},
                {ArrowType.LeftDownRight, Vector2.zero},
                {ArrowType.DownRightUp, Vector2.zero},
                {ArrowType.RightUpLeft, Vector2.zero},
                {ArrowType.UpLeftDownRight, Vector2.zero},
            
                {ArrowType.SoloLeft, Vector2.left},
                {ArrowType.LeftDown, (Vector2.left + Vector2.down).normalized},
                {ArrowType.SoloDown, Vector2.down},
                {ArrowType.DownRight, (Vector2.down + Vector2.right).normalized},
                {ArrowType.SoloRight, Vector2.right},
                {ArrowType.RightUp, (Vector2.right + Vector2.up).normalized},
                {ArrowType.SoloUp, Vector2.up},
                {ArrowType.UpLeft, (Vector2.up + Vector2.left).normalized},
            };

        public static Dictionary<ArrowType, Vector2> ArrowViewPortVectorCollection_RightNegativeHandPivotSystem
            => ArrowViewPortVectorCollection_RightHandPivotSystem.ToDictionary(kv => kv.Key, kv => -kv.Value);
        
        public static Dictionary<ArrowType, Vector2> ArrowViewPortPerpendicularVectorCollection_RightHandPivotSystem
            => ArrowViewPortVectorCollection_RightHandPivotSystem.ToDictionary(kv => kv.Key, kv =>
            {
                var perpVector3 = Vector3.Cross(kv.Value, Vector3.forward).normalized;
                return new Vector2(perpVector3.x, perpVector3.y);
            });
        
        public static Vector3 XZVector(this Vector3 p_TargetVector) => new Vector3(p_TargetVector.x, 0f, p_TargetVector.z);
        public static Vector3 XZUVector(this Vector3 p_TargetVector) => new Vector3(p_TargetVector.x, 0f, p_TargetVector.z).normalized;
        public static Vector3 XZVector(this Vector2 p_TargetVector) => new Vector3(p_TargetVector.x, 0f, p_TargetVector.y);
        public static Vector3 XYVector(this Vector3 p_TargetVector) => new Vector3(p_TargetVector.x, p_TargetVector.y, 0f);
        public static Vector2 XYVector2(this Vector3 p_TargetVector) => new Vector2(p_TargetVector.x, p_TargetVector.y);
        public static Vector3 YZVector(this Vector3 p_TargetVector) => new Vector3(0f, p_TargetVector.y, p_TargetVector.z);

        public static Vector3 CorrectVector(this Vector3 p_TargetVector, VectorCorrectType p_Type)
        {
            switch (p_Type)
            {
                case VectorCorrectType.TurnToXZ:
                    return new Vector3(p_TargetVector.x, 0f, p_TargetVector.z);
                default:
                case VectorCorrectType.None:
                    return p_TargetVector;
            }
        }

        public static float Dot(this Transform p_Transform, Vector3 p_UV)
        {
            return Vector3.Dot(p_Transform.forward, p_UV);
        }

        /// <summary>
        /// 두 벡터의 2차원 행렬값 = 외적크기를 리턴하는 메서드
        /// 혹은 두 벡터의 XY 성분 벡터가 얼마나 직각을 이루는지 리턴하는 메서드
        /// </summary>
        public static float DeterminantXY(this Vector3 p_LeftVector, Vector3 p_RightVector)
        {
            return p_LeftVector.x * p_RightVector.y - p_LeftVector.y * p_RightVector.x;
        }
        
        /// <summary>
        /// 두 벡터의 2차원 행렬값 = 외적크기를 리턴하는 메서드
        /// 혹은 두 벡터의 YZ 성분 벡터가 얼마나 직각을 이루는지 리턴하는 메서드
        /// </summary>
        public static float DeterminantYZ(this Vector3 p_LeftVector, Vector3 p_RightVector)
        {
            return p_LeftVector.y * p_RightVector.z - p_LeftVector.z * p_RightVector.y;
        }

        /// <summary>
        /// 두 벡터의 2차원 행렬값 = 외적크기를 리턴하는 메서드
        /// 혹은 두 벡터의 XZ 성분 벡터가 얼마나 직각을 이루는지 리턴하는 메서드
        /// </summary>
        public static float DeterminantXZ(this Vector3 p_LeftVector, Vector3 p_RightVector)
        {
            return p_LeftVector.x * p_RightVector.z - p_LeftVector.z * p_RightVector.x;
        }
        
        /// <summary>
        /// 두 벡터의 3차원 행렬값 = 외적크기를 리턴하는 메서드
        /// 혹은 두 벡터가 얼마나 직각을 이루는지 리턴하는 메서드
        /// </summary>
        public static float DeterminantXYZ(this Vector3 p_LeftVector, Vector3 p_RightVector)
        {
            return (p_LeftVector.y * p_RightVector.z - p_LeftVector.z * p_RightVector.y) 
                    - (p_LeftVector.x * p_RightVector.z - p_LeftVector.z * p_RightVector.x)
                    + (p_LeftVector.x * p_RightVector.y - p_LeftVector.y * p_RightVector.x);
        }
        
        /// <summary>
        /// 지정한 두 기저단위벡터로 구성되는 평면에서 degree 만큼의 각도를 지니는 단위 벡터를 리턴하는 메서드
        /// </summary>
        /// <param name="p_XBasis">모x축 기저 단위 벡터</param>
        /// <param name="p_YBasis">모y축 기저 단위 벡터</param>
        /// <param name="p_Degree">모x축을 시작으로 하는 반시계 방향 각도</param>
        public static Vector3 GetPlaneDegreeVector(Vector3 p_XBasis, Vector3 p_YBasis, float p_Degree)
        {
            var radian = p_Degree * Mathf.Deg2Rad;
            return p_XBasis * Mathf.Cos(radian) + p_YBasis * Mathf.Sin(radian);
        }

        /// <summary>
        /// 특정한 벡터로의 사영벡터를 리턴하는 메서드
        /// </summary>
        /// <param name="p_Main">사영될 벡터</param>
        /// <param name="p_ProjectionTargetNormal">사영할 벡터</param>
        public static Vector3 GetProjectionVector(this Vector3 p_TargetVector, Vector3 p_ProjectionToUnitVector)
        {
            return GetProjectionVectorMagnitude(p_TargetVector, p_ProjectionToUnitVector) * p_ProjectionToUnitVector;
        }
    
        /// <summary>
        /// 특정한 벡터로의 사영벡터의 magnitude를 리턴하는 메서드
        /// </summary>
        /// <param name="p_Main">사영될 벡터</param>
        /// <param name="p_ProjectionTargetNormal">사영할 벡터</param>
        public static float GetProjectionVectorMagnitude(this Vector3 p_TargetVector, Vector3 p_ProjectionToUnitVector)
        {
            return Vector3.Dot(p_TargetVector, p_ProjectionToUnitVector);
        }
        
        /// <summary>
        /// 특정한 평면으로의 사영벡터를 리턴하는 메서드
        /// </summary>
        /// <param name="p_TargetVector">사영될 벡터</param>
        /// <param name="p_PlaneNormalUnitVector">사영할 평면의 법선단위벡터</param>
        public static Vector3 GetPlaneProjectionVector(this Vector3 p_TargetVector, Vector3 p_PlaneNormalUnitVector)
        {
            return Vector3.Cross(Vector3.Cross(p_PlaneNormalUnitVector, p_TargetVector), p_PlaneNormalUnitVector);
        }
        
        /// <summary>
        /// 특정한 평면으로의 사영벡터의 magnitude를 리턴하는 메서드
        /// </summary>
        /// <param name="p_TargetVector">사영될 벡터</param>
        /// <param name="p_PlaneNormalUnitVector">사영할 평면의 법선단위벡터</param>
        public static float GetPlaneProjectionVectorMagnitude(this Vector3 p_TargetVector, Vector3 p_PlaneNormalUnitVector)
        {
            return Vector3.Cross(p_PlaneNormalUnitVector, p_TargetVector).DeterminantXYZ(p_PlaneNormalUnitVector);
        }
        
        /// <summary>
        /// 특정한 평면으로의 사영벡터의 magnitude를 리턴하는 메서드
        /// </summary>
        /// <param name="p_TargetVector">사영될 벡터</param>
        /// <param name="p_PlaneNormalUnitVector">사영할 평면의 법선단위벡터</param>
        public static float GetPlaneProjectionVectorMagnitude2(this Vector3 p_TargetVector, Vector3 p_PlaneNormalUnitVector)
        {
            return (p_PlaneNormalUnitVector.DeterminantXYZ(p_TargetVector) * Vector3.one).DeterminantXYZ(p_PlaneNormalUnitVector);
        }
        
        /// <summary>
        /// 지정한 벡터의 스칼라값을 임의의 상/하한값을 가지도록 재구성하여 리턴하는 메서드
        /// </summary>
        public static Vector3 GetBoundedVector(this Vector3 p_TargetVector)
        {
            return new Vector3(
                    p_TargetVector.x.GetBoundedValue(), 
                    p_TargetVector.y.GetBoundedValue(),
                    p_TargetVector.z.GetBoundedValue()
                );
        }

        /// <summary>
        /// 지정한 벡터의 스칼라값을 역수로가지는 벡터를 리턴하는 메서드
        /// </summary>
        public static Vector3 GetBoundedInverseVector(this Vector3 p_TargetVector)
        {
            return new Vector3(
                    p_TargetVector.x.GetBoundedInverseValue(), 
                    p_TargetVector.y.GetBoundedInverseValue(), 
                    p_TargetVector.z.GetBoundedInverseValue()
                );
        }

        /// <summary>
        /// 특정 점을 중심으로 반대편에 있는 좌표를 리턴하는 메서드
        /// </summary>
        public static Vector3 GetSymmetricPosition(this Vector3 p_TargetVector, Vector3 p_PivotVector)
        {
            return 2f * p_PivotVector - p_TargetVector;
        }

        /// <summary>
        /// 두 벡터가 평행한지 검증하는 메서드
        /// </summary>
        public static bool IsParallel(this Vector3 p_LeftVector, Vector3 p_RightVector, bool p_CheckReverseVector)
        {
            var leftUV = p_LeftVector.normalized;
            var rightUV = p_RightVector.normalized;
            return leftUV == rightUV || (p_CheckReverseVector && leftUV == -rightUV);
        }
        
        /// <summary>
        /// 특정 위치 basePoint에서 baseDirection 방향으로 구성된 선분에 대해, targetPoint의 선분 너머 대칭점을 찾아 리턴하는 메서드
        /// </summary>
        public static Vector3 GetSymmetryPointThroughLine(Vector3 p_BasePoint, Vector3 p_BaseDirection, Vector3 p_TargetPoint)
        {
            var targetVector = p_TargetPoint - p_BasePoint;
            var frotierNormVector = p_BaseDirection.normalized;
        
            return p_TargetPoint + 2f * (Vector3.Dot(targetVector, frotierNormVector) * frotierNormVector - targetVector);
        }
        
        /// <summary>
        /// 특정 벡터의 XZ 평면 상의 각도를 리턴하는 메소드
        /// </summary>
        public static float GetXZDegreeVector(Vector3 p_Direction)
        {
            return Mathf.Atan2(p_Direction.z, p_Direction.x) * Mathf.Rad2Deg;
        }
        
        /// <summary>
        /// 지정한 각도를 가지는 XZ 평면 상의 단위 벡터를 리턴하는 메소드
        /// </summary>
        public static Vector3 GetXZDirectionUnitVector(float p_Degree)
        {
            var rad = p_Degree * Mathf.Deg2Rad;
            return new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad));
        }
        
        /// <summary>
        /// 지정한 벡터의 XZ 상의 수직 단위 벡터를 리턴하는 메서드
        /// </summary>
        public static Vector3 GetXZPerpendicularUnitVector(this Vector3 p_TargetVector)
        {
            var normVector = p_TargetVector.normalized;
            if (normVector == Vector3.up || normVector == Vector3.down)
            {
                return Vector3.right;
            }
            else
            {
                return Vector3.Cross(normVector, Vector3.up).normalized;
            }
        }
        
        public static (Vector3 t_Right, Vector3 t_Up) GetPerpendicularUnitVector(this Vector3 p_TargetVector)
        {
            var right = p_TargetVector.GetXZPerpendicularUnitVector();
            var up = Vector3.Cross(p_TargetVector, right).normalized;
            return (right, up);
        }
        
        /// <summary>
        /// XZ 평면 상의 특정 벡터와 그 수직 벡터를 보간한 사이 단위 벡터를 리턴하는 메서드
        /// 보간 변수가 1에 가까울 수록 원본 벡터가, 0에 가까울수록 수직 벡터가 리턴된다.
        /// </summary>
        public static Vector3 GetWeightedXZPerpendicularVector(this Vector3 p_MainDirection, float p_MainWeight)
        {
            var perpVec = GetXZPerpendicularUnitVector(p_MainDirection);
            return p_MainDirection.normalized * p_MainWeight + perpVec * (1f - p_MainWeight);
        }

        /// <summary>
        /// 특정 벡터에 포함되어 있는 + 혹은 0 원소를 리턴하는 메서드
        /// </summary>
        public static XYZType GetVectorPositiveType(this Vector3 p_TargetVector)
        {
            var tryX = p_TargetVector.x < 0f ? 0 : 1 << 0;
            var tryY = p_TargetVector.y < 0f ? 0 : 1 << 1;
            var tryZ = p_TargetVector.z < 0f ? 0 : 1 << 2;
            
            return XYZType.None + tryX + tryY + tryZ;
        }

        /// <summary>
        /// 오차가 생겼는지 벡터를 검증하여 리턴하는 메서드
        /// </summary>
        public static Vector3 GetSafeVector(this Vector3 p_TargetVector)
        {
            return new Vector3
            (
                p_TargetVector.x.IsReachedZero() ? 0f : p_TargetVector.x,
                p_TargetVector.y.IsReachedZero() ? 0f : p_TargetVector.y,
                p_TargetVector.z.IsReachedZero() ? 0f : p_TargetVector.z
            );
        }
        
        /// <summary>
        /// 특정 벡터를 영벡터로 러프시키는 메서드. x, y, z 값은 각각 0값을 향해 부호를 가리지 않고 p_DeltaValue 상한값을 가지고
        /// 이동한다. 만약 p_DeltaValue 값을 음수로 넣었다면 0으로부터 멀어진다.
        /// </summary>
        public static Vector3 ZeroLerp(this Vector3 p_TargetVector, float p_DeltaValue)
        {
            var tryX = p_TargetVector.x;
            p_TargetVector.x = Mathf.MoveTowards(tryX, 0f, p_DeltaValue);
                
            var tryY = p_TargetVector.y;
            p_TargetVector.y = Mathf.MoveTowards(tryY, 0f, p_DeltaValue);
                
            var tryZ = p_TargetVector.z;
            p_TargetVector.z = Mathf.MoveTowards(tryZ, 0f, p_DeltaValue);

            return p_TargetVector;
        }

        /// <summary>
        /// 특정 벡터를 영벡터로 러프시키는 메서드. x, y, z 값은 각각 0값을 향해 부호를 가리지 않고 p_DeltaValue 상한값을 가지고
        /// 이동한다. 만약 p_DeltaValue 값을 음수로 넣었다면 0으로부터 멀어진다.
        ///
        /// ZeroLerp와 다르게 x, y, z 원소의 대소에 상관없이 동일한 속력으로 0과 가까워지거나 멀어진다.
        /// 
        /// </summary>
        public static Vector3 ZeroLerpSameRate(this Vector3 p_TargetVector, float p_DeltaValue)
        {
            var normVector = p_TargetVector.normalized.AbsoluteVector();
            p_TargetVector.x = Mathf.MoveTowards(p_TargetVector.x, 0f, p_DeltaValue * normVector.x);
            p_TargetVector.y = Mathf.MoveTowards(p_TargetVector.y, 0f, p_DeltaValue * normVector.y);
            p_TargetVector.z = Mathf.MoveTowards(p_TargetVector.z, 0f, p_DeltaValue * normVector.z);

            return p_TargetVector;  
        }
        
        /// <summary>
        /// ZeroLerpSameRate 메서드는 각 원소별로 러프를 진행하는 메서드인데
        /// 해당 메서드는 벡터에 대해서 러프를 진행한다. 다만 이 과정에서
        /// 거리계산이 사용될 수도 있기 때문에 코스트는 ZeroLerpSameRate보다 클 것.
        /// </summary>
        public static Vector3 ZeroLerpSameRate2(this Vector3 p_TargetVector, float p_DeltaValue)
        {
            return Vector3.MoveTowards(p_TargetVector, Vector3.zero, p_DeltaValue);
        }
        
        /// <summary>
        /// 특정 벡터의 각 성분이 지정한 상/하한 값 구간 안에 들어가도록 Clamp한 벡터를 리턴하는 메서드
        /// </summary>
        public static Vector3 ClampVector(this Vector3 p_TargetVector, float p_MinValue, float p_MaxValue)
        {
            p_TargetVector.x = Mathf.Clamp(p_TargetVector.x, p_MinValue, p_MaxValue);
            p_TargetVector.y = Mathf.Clamp(p_TargetVector.y, p_MinValue, p_MaxValue);
            p_TargetVector.z = Mathf.Clamp(p_TargetVector.z, p_MinValue, p_MaxValue);

            return p_TargetVector;
        }

        /// <summary>
        /// 두 벡터 사이를 보간하여 궤도 벡터 값을 리턴한다.
        /// </summary>
        public static Vector3 SlerpFromTo(this Vector3 p_StartVector, Vector3 p_EndVector, float p_Weight01)
        {
            return Vector3.Slerp(p_StartVector, p_EndVector, p_Weight01);
        }

        /// <summary>
        /// 지정한 벡터의 원소를 전부 양수화하여 리턴한다.
        /// </summary>
        public static Vector3 AbsoluteVector(this Vector3 p_TargetVector)
        {
            return new Vector3(Mathf.Abs(p_TargetVector.x), Mathf.Abs(p_TargetVector.y), Mathf.Abs(p_TargetVector.z));
        }

        /// <summary>
        /// 지정한 두 벡터 사이를 보간하여 리턴한다.
        /// </summary>
        public static Vector3 GetLBVector(this Vector3 p_StartPos, Vector3 p_EndPos, float p_LBFactor)
        {
            return (1f - p_LBFactor) * p_StartPos + p_LBFactor * p_EndPos;
        }
    }
}