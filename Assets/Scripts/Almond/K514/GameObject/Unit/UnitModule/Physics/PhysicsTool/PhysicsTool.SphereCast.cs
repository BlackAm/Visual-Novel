using UnityEngine;

namespace k514
{
    public partial class PhysicsTool
    {
        /// <summary>
        /// 스피어캐스팅을 수행하여 충돌 정보를 리턴하는 메서드, 캐스팅할 거리/방향을 아는 경우
        /// </summary>
        public static bool CheckAnyObject_SphereCast(Vector3 p_StartPos, Vector3 p_UV, float p_Radius, float p_Distance, int p_LayerMask, bool p_CorrectStartPosition, QueryTriggerInteraction p_QueryTriggerInteraction)
        {
            return GetAnyObjectCount_SphereCast(p_StartPos, p_UV, p_Radius, p_Distance, p_LayerMask, p_CorrectStartPosition, p_QueryTriggerInteraction) > 0;
        }
        
        /// <summary>
        /// 스피어캐스팅을 수행하여 충돌 정보를 리턴하는 메서드, 캐스팅할 거리/방향을 아는 경우, 리턴값이 정수인 경우
        /// </summary>
        public static int GetAnyObjectCount_SphereCast(Vector3 p_StartPos, Vector3 p_UV, float p_Radius, float p_Distance, int p_LayerMask, bool p_CorrectStartPosition, QueryTriggerInteraction p_QueryTriggerInteraction)
        {
            int hitCount = Physics.SphereCastNonAlloc(
                p_CorrectStartPosition ? p_StartPos - CustomMath.Epsilon * p_UV : p_StartPos, 
                p_Radius, p_UV, _NonAllocRayCast, 
                p_CorrectStartPosition ? p_Distance + CustomMath.Epsilon : p_Distance, 
                p_LayerMask, p_QueryTriggerInteraction);

#if UNITY_EDITOR
            if (Application.isPlaying && CustomDebug.DrawSpherePhysicsCheck)
            {
                var targetPos = p_StartPos + p_Distance * p_UV;
                CustomDebug.DrawArrow(p_StartPos, targetPos, 0.1f, Color.blue, 1f);
                CustomDebug.DrawSphere(targetPos, 0f, p_Radius, p_StartPos.GetDirectionUnitVectorTo(targetPos), Color.red, 1, 1f);
            }
#endif
            return hitCount;
        }
    }
}