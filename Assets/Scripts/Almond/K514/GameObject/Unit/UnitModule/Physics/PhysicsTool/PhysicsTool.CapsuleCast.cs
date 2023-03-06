using UnityEngine;

namespace k514
{
    public partial class PhysicsTool
    {
        /// <summary>
        /// 캡슐캐스팅을 수행하여 지정한 경로 내에 가장 가까운 충돌 오브젝트와의 거리를 리턴하는 메서드
        /// 충돌이 발생하지 않았다면 파라미터의 최대 거리를 리턴한다.
        /// 2번째 파라미터를 통해 반경의 배율을 지정할 수 있다.
        /// </summary>
        public static (bool, float) GetNearestObjectDistance_CapsuleCast_PhysicsObjectMulRadius(IVirtualRange p_VirtualRange, float p_RadiusMulRate, Vector3 p_UV, float p_MaxDistance, int p_LayerMask, QueryTriggerInteraction p_QueryTriggerInteraction)
        {
            var transform = p_VirtualRange.Collider.transform;
            return GetNearestObjectDistance_CapsuleCast_MulRadius(transform, transform.position, p_VirtualRange.Radius.CurrentValue, p_VirtualRange.Height.CurrentValue, p_RadiusMulRate, p_UV, p_MaxDistance, p_LayerMask, p_QueryTriggerInteraction);
        }
        
        /// <summary>
        /// 캡슐캐스팅을 수행하여 지정한 경로 내에 가장 가까운 충돌 오브젝트와의 거리를 리턴하는 메서드
        /// 충돌이 발생하지 않았다면 파라미터의 최대 거리를 리턴한다.
        /// 2번째 파라미터를 통해 반경의 배율을 지정할 수 있다.
        /// </summary>
        public static (bool, float) GetNearestObjectDistance_CapsuleCast_MulRadius(Transform p_TransformExcept, Vector3 p_BasePosition, float p_Radius, float p_Height, float p_RadiusMulRate, Vector3 p_UV, float p_MaxDistance, int p_LayerMask, QueryTriggerInteraction p_QueryTriggerInteraction)
        {
            var basePosition = p_BasePosition + Vector3.up * (1f - p_RadiusMulRate);
            var radius = p_Radius * p_RadiusMulRate;
            var height = Mathf.Max(p_Height * p_RadiusMulRate, radius * 2f);
            
            return GetNearestObjectDistance_CapsuleCast(p_TransformExcept, basePosition, radius, height, p_UV, p_MaxDistance, p_LayerMask, p_QueryTriggerInteraction);
        }
        
        /// <summary>
        /// 캡슐캐스팅을 수행하여, 지정한 경로 내에 가장 가까운 충돌 오브젝트와의 거리를 리턴하는 메서드
        /// 충돌이 발생하지 않았다면 파라미터의 최대 거리를 리턴한다.
        /// 2번째 파라미터를 통해 반경의 추가치를 지정할 수 있다.
        /// 
        public static (bool, float) GetNearestObjectDistance_CapsuleCast_PhysicsObjectAddRadius(IVirtualRange p_VirtualRange, float p_RadiusAddRate, Vector3 p_UV, float p_MaxDistance, int p_LayerMask, QueryTriggerInteraction p_QueryTriggerInteraction)
        {
            var transform = p_VirtualRange.Collider.transform;
            return GetNearestObjectDistance_CapsuleCast_AddRadius(transform, transform.position, p_VirtualRange.Radius.CurrentValue, p_VirtualRange.Height.CurrentValue, p_RadiusAddRate, p_UV, p_MaxDistance, p_LayerMask, p_QueryTriggerInteraction);
        }
        
        /// <summary>
        /// 캡슐캐스팅을 수행하여 지정한 경로 내에 가장 가까운 충돌 오브젝트와의 거리를 리턴하는 메서드
        /// 충돌이 발생하지 않았다면 파라미터의 최대 거리를 리턴한다.
        /// 2번째 파라미터를 통해 반경의 추가치를 지정할 수 있다.
        /// </summary>
        public static (bool, float) GetNearestObjectDistance_CapsuleCast_AddRadius(Transform p_TransformExcept, Vector3 p_BasePosition, float p_Radius, float p_Height, float p_RadiusAddRate, Vector3 p_UV, float p_MaxDistance, int p_LayerMask, QueryTriggerInteraction p_QueryTriggerInteraction)
        {
            var basePosition = p_BasePosition + Vector3.down * p_RadiusAddRate;
            var radius = p_Radius + p_RadiusAddRate;
            var height = Mathf.Max(p_Height + p_RadiusAddRate * 2f, radius * 2f);
            
            return GetNearestObjectDistance_CapsuleCast(p_TransformExcept, basePosition, radius, height, p_UV, p_MaxDistance, p_LayerMask, p_QueryTriggerInteraction);
        }
        
        /// <summary>
        /// 캡슐캐스팅을 수행하여, 지정한 경로 내에 가장 가까운 충돌 오브젝트와의 거리를 리턴하는 메서드
        /// 충돌이 발생하지 않았다면 파라미터의 최대 거리를 리턴한다.
        /// </summary>
        public static (bool, float) GetNearestObjectDistance_CapsuleCast(IVirtualRange p_VirtualRange, Vector3 p_UV, float p_MaxDistance, int p_LayerMask, QueryTriggerInteraction p_QueryTriggerInteraction)
        {
            var transform = p_VirtualRange.Collider.transform;
            return GetNearestObjectDistance_CapsuleCast(transform, transform.position, p_VirtualRange.Radius.CurrentValue, p_VirtualRange.Height.CurrentValue, p_UV, p_MaxDistance, p_LayerMask, p_QueryTriggerInteraction);
        }
        
        /// <summary>
        /// 캡슐캐스팅을 수행하여, 지정한 경로 내에 가장 가까운 충돌 오브젝트와의 거리를 리턴하는 메서드
        /// 충돌이 발생하지 않았다면 파라미터의 최대 거리를 리턴한다.
        /// </summary>
        public static (bool, float) GetNearestObjectDistance_CapsuleCast(Transform p_TransformExcept, Vector3 p_BasePosition, float p_Radius, float p_Height, Vector3 p_UV, float p_MaxDistance, int p_LayerMask, QueryTriggerInteraction p_QueryTriggerInteraction)
        {
            var capsulePosLow = p_BasePosition;
            var capsulePosHigh = p_BasePosition + Vector3.up * p_Height;
            int hitCount = Physics.CapsuleCastNonAlloc(capsulePosLow, capsulePosHigh, p_Radius, p_UV, _NonAllocRayCast,　p_MaxDistance, p_LayerMask, p_QueryTriggerInteraction);
            if (hitCount > 0)
            {
                var result = false;
                var resultDistance = p_MaxDistance;
                for (int i = 0; i < hitCount; i++)
                {
                    var targetCastHit = _NonAllocRayCast[i];
                    if (!ReferenceEquals(targetCastHit.transform, p_TransformExcept))
                    {
                        result = true;
                        var targetDistance = targetCastHit.distance;
                        if (targetDistance < resultDistance)
                        {
                            resultDistance = targetDistance;
                        }
                    }
                }

#if UNITY_EDITOR
                if (result && Application.isPlaying && CustomDebug.DrawPivot)
                {
                    CustomDebug.DrawCapsule(p_BasePosition + resultDistance * p_UV, false, p_Radius, p_Height, Color.green);
                }
#endif
                
                return (result, resultDistance);
            }
            else
            {
                return (false, p_MaxDistance);
            }
        }
    }
}