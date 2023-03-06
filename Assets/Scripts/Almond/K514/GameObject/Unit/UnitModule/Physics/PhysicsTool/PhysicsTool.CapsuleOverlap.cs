using UnityEngine;

namespace k514
{
    public partial class PhysicsTool
    {
        /// <summary>
        /// 가상 캡슐을 오버랩하여 해당 박스 내에 지정한 Transform이 있는지 검증하는 논리메서드
        /// 2번째 파라미터를 통해 반경의 배율을 지정할 수 있다.
        /// </summary>
        public static bool FindObjectDistance_CapsuleOverlap_PhysicsObjectMulRadius(IVirtualRange p_VirtualRange, float p_RadiusMulRate, Transform p_TargetTransform, int p_LayerMask, QueryTriggerInteraction p_QueryTriggerInteraction)
        {
            var basePosition = p_VirtualRange.Collider.transform.position + Vector3.up * (1f - p_RadiusMulRate);
            var radius = p_VirtualRange.Radius.CurrentValue * p_RadiusMulRate;
            var height = Mathf.Max(p_VirtualRange.Height.CurrentValue * p_RadiusMulRate, radius * 2f);
            
            return FindObjectDistance_CapsuleOverlap(basePosition, radius, height, p_TargetTransform, p_LayerMask, p_QueryTriggerInteraction);
        }
        
        /// <summary>
        /// 가상 캡슐을 오버랩하여 해당 박스 내에 지정한 Transform이 있는지 검증하는 논리메서드
        /// 2번째 파라미터를 통해 반경의 추가치를 지정할 수 있다.
        /// </summary>
        public static bool FindObjectDistance_CapsuleOverlap_PhysicsObjectAddRadius(IVirtualRange p_VirtualRange, float p_RadiusAddRate, Transform p_TargetTransform, int p_LayerMask, QueryTriggerInteraction p_QueryTriggerInteraction)
        {
            var basePosition = p_VirtualRange.Collider.transform.position + Vector3.down * p_RadiusAddRate;
            var radius = p_VirtualRange.Radius.CurrentValue + p_RadiusAddRate;
            var height = Mathf.Max(p_VirtualRange.Height.CurrentValue + p_RadiusAddRate * 2f, radius * 2f);

            return FindObjectDistance_CapsuleOverlap(basePosition, radius, height, p_TargetTransform, p_LayerMask, p_QueryTriggerInteraction);
        }
        
        /// <summary>
        /// 가상 캡슐을 오버랩하여 해당 박스 내에 지정한 Transform이 있는지 검증하는 논리메서드
        /// </summary>
        public static bool FindObjectDistance_CapsuleOverlap(Vector3 p_BasePosition, float p_Radius, float p_Height, Transform p_TargetTransform, int p_LayerMask, QueryTriggerInteraction p_QueryTriggerInteraction)
        {
#if UNITY_EDITOR
            if (Application.isPlaying && CustomDebug.DrawPivot)
            {
                CustomDebug.DrawCapsule(p_BasePosition, false, p_Radius, p_Height, Color.red, 16, 3f);
            }
#endif

            var capsulePosLow = p_BasePosition;
            var capsulePosHigh = p_BasePosition + Vector3.up * p_Height;
            int hitCount = Physics.OverlapCapsuleNonAlloc(capsulePosLow, capsulePosHigh, p_Radius, _NonAllocCollider, p_LayerMask, p_QueryTriggerInteraction);
            if (hitCount > 0)
            {
                for (int i = 0; i < hitCount; i++)
                {
                    var targetOverlap = _NonAllocCollider[i];
                    if (targetOverlap.transform == p_TargetTransform)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        
        /// <summary>
        /// 가상 캡슐을 오버랩하여 해당 박스 내에 특정 레이어를 가진 물리 오브젝트가 존재하는지 검증하는메서드
        /// </summary>
        public static int GetCount_CapsuleOverlap(FocusableInstance p_FocusableInstance, float p_YBias, int p_LayerMask, QueryTriggerInteraction p_QueryTriggerInteraction)
        {
            return GetCount_CapsuleOverlap(p_FocusableInstance._RangeObject, p_YBias, p_LayerMask, p_QueryTriggerInteraction);
        }
        
        /// <summary>
        /// 가상 캡슐을 오버랩하여 해당 박스 내에 특정 레이어를 가진 물리 오브젝트가 존재하는지 검증하는메서드
        /// </summary>
        public static int GetCount_CapsuleOverlap(IVirtualRange p_VirtualRange, float p_YBias, int p_LayerMask, QueryTriggerInteraction p_QueryTriggerInteraction)
        {
            var radiusPreset = p_VirtualRange.Radius;
            var radiusOffset = radiusPreset._DefaultOffset;
            var heightPreset = p_VirtualRange.Height;
            var heightOffset = heightPreset._DefaultOffset;

            var radius = radiusPreset.CurrentValue - radiusOffset;
            var height = Mathf.Max(heightPreset.CurrentValue - 2f * radius - heightOffset, 0f) + Mathf.Abs(p_YBias);
            var basePosition = p_VirtualRange.Collider.transform.position + (p_YBias + radius) * Vector3.up;
       
            return GetCount_CapsuleOverlap(basePosition, radius, height, p_LayerMask, p_QueryTriggerInteraction);
        }
        
        /// <summary>
        /// 가상 캡슐을 오버랩하여 해당 박스 내에 특정 레이어를 가진 물리 오브젝트가 존재하는지 검증하는메서드
        /// </summary>
        public static int GetCount_CapsuleOverlap(Vector3 p_BasePosition, float p_Radius, float p_Height, int p_LayerMask, QueryTriggerInteraction p_QueryTriggerInteraction)
        {

            var capsulePosLow = p_BasePosition;
            var capsulePosHigh = capsulePosLow + p_Height * Vector3.up;
            
            return Physics.OverlapCapsuleNonAlloc(capsulePosLow, capsulePosHigh, p_Radius, _NonAllocCollider, p_LayerMask, p_QueryTriggerInteraction);
        }
    }
}