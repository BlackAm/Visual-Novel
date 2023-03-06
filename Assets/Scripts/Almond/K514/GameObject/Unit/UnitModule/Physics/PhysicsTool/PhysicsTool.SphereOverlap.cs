using UnityEngine;

namespace k514
{
    public partial class PhysicsTool
    {
        /// <summary>
        /// 가상 구를 오버랩하여 해당 구 내에 지정한 타입의 레이어 컬라이더가 있는지 검증하는 논리메서드
        /// </summary>
        public static bool GetSphereOverlap(Vector3 p_Center, float p_Radius, int p_LayerMask)
        {
            int hitCount = Physics.OverlapSphereNonAlloc(p_Center, p_Radius, _NonAllocCollider, p_LayerMask);
#if UNITY_EDITOR
            if (Application.isPlaying && CustomDebug.DrawPivot)
            {
                CustomDebug.DrawSphere(p_Center, 0f, p_Radius, Vector3.up, Color.green);
            }
#endif
            return hitCount > 0;
        }
    }
}