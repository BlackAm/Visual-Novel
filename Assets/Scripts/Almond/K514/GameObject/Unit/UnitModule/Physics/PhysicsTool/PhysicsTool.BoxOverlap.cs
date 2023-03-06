using UnityEngine;

namespace k514
{
    public partial class PhysicsTool
    {
        /// <summary>
        /// 가상 박스를 오버랩하여 해당 박스 내에 지정한 레이어 컬라이더가 있는지 검증하는 논리메서드
        /// </summary>
        public static bool GetBoxOverlap(Vector3 p_Start, int p_LayerMask)
        {
            return GetBoxOverlap(p_Start, CustomMath.HalfEpsilonVector, Quaternion.identity, p_LayerMask);
        }
        
        /// <summary>
        /// 가상 박스를 오버랩하여 해당 박스 내에 지정한 레이어 컬라이더가 있는지 검증하는 논리메서드
        /// </summary>
        public static bool GetBoxOverlap(Vector3 p_Start, Vector3 p_HalfExtends, Quaternion p_BoxRotation, int p_LayerMask)
        {
            int hitCount = Physics.OverlapBoxNonAlloc(p_Start, p_HalfExtends, _NonAllocCollider, p_BoxRotation, p_LayerMask);
#if UNITY_EDITOR
            if (Application.isPlaying && CustomDebug.DrawPivot)
            {
                CustomDebug.DrawBox(p_Start, p_HalfExtends, p_BoxRotation, Color.green);
            }
#endif
            return hitCount > 0;
        }
        
        /// <summary>
        /// 가상 박스를 오버랩하여 해당 박스 내에 지정한 Transform이 있는지 검증하는 논리메서드
        /// </summary>
        public static bool FindTransform_BoxOverlap(Vector3 p_Start, Vector3 p_HalfExtends, Quaternion p_BoxRotation, Transform p_TargetTransform, int p_LayerMask)
        {
            int hitCount = Physics.OverlapBoxNonAlloc(p_Start, p_HalfExtends, _NonAllocCollider, p_BoxRotation, p_LayerMask);
            if (hitCount > 0)
            {
                for (int i = 0; i < hitCount; i++)
                {
                    var targetOverlap = _NonAllocCollider[i];
                    if (targetOverlap.transform == p_TargetTransform)
                    {
#if UNITY_EDITOR
                        if (Application.isPlaying && CustomDebug.DrawPivot)
                        {
                            CustomDebug.DrawBox(p_Start, p_HalfExtends, p_BoxRotation, Color.green);
                        }
#endif
                        return true;
                    }
                }
            }
            return false;
        }
    }
}