using UnityEngine;

namespace k514
{
    public partial class PhysicsTool
    {
        /// <summary>
        /// 박스를 캐스팅하여, 특정 레이어와 충돌했는지 검증하는 메서드
        /// 충돌이 발생하지 않았다면 파라미터의 최대 거리를 리턴한다.
        /// </summary>
        public static (bool, float) GetNearestObjectDistance_BoxCast(Vector3 p_Start, Vector3 p_UV, Vector3 p_HalfExtends, Quaternion p_BoxRotation, float p_MaxDistance, int p_LayerMask)
        {
            int hitCount = Physics.BoxCastNonAlloc(p_Start, p_HalfExtends, p_UV, _NonAllocRayCast, p_BoxRotation, p_MaxDistance, p_LayerMask);
            if (hitCount > 0)
            {
                var resultDistance = p_MaxDistance;
                for (int i = 0; i < hitCount; i++)
                {
                    var targetCastHit = _NonAllocRayCast[i];
                    var targetDistance = targetCastHit.distance;
                    if (targetDistance < resultDistance)
                    {
                        resultDistance = targetDistance;
                    }
                }
#if UNITY_EDITOR
                if (Application.isPlaying && CustomDebug.DrawPivot)
                {
                    var endPos = p_Start + p_UV * resultDistance;
                    CustomDebug.DrawArrow(p_Start, endPos, 0.1f, Color.yellow, 1f);
                    CustomDebug.DrawBox(endPos, p_HalfExtends, p_BoxRotation, Color.yellow, 16, 1f);
                }
#endif
                return (true, resultDistance);
            }
            else
            {
                return (false, p_MaxDistance);
            }
        }
        
        /// <summary>
        /// 박스캐스트를 수행하여, MaxDistance 내에 지정한 Transform이 있는지 검증하는 논리메서드
        /// </summary>
        public static bool FindObjectDistance_BoxCast(Vector3 p_Start, Vector3 p_UV, Vector3 p_HalfExtends, Quaternion p_BoxRotation, float p_MaxDistance, Transform p_TargetTransform, int p_LayerMask)
        {
            int hitCount = Physics.BoxCastNonAlloc(p_Start, p_HalfExtends, p_UV, _NonAllocRayCast, p_BoxRotation, p_MaxDistance, p_LayerMask);
            if (hitCount > 0)
            {
                for (int i = 0; i < hitCount; i++)
                {
                    var targetCastHit = _NonAllocRayCast[i];
                    if (targetCastHit.transform == p_TargetTransform)
                    {
#if UNITY_EDITOR
                        if (Application.isPlaying && CustomDebug.DrawPivot)
                        {
                            var endPos = p_Start + p_UV * targetCastHit.distance;
                            CustomDebug.DrawArrow(p_Start, endPos, 0.01f, Color.blue);
                            CustomDebug.DrawBox(endPos, p_HalfExtends, p_BoxRotation, Color.green);
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