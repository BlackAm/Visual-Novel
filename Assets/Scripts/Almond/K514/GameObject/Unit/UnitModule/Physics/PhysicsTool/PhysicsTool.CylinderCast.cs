using UnityEngine;

namespace k514
{
    public partial class PhysicsTool
    {
        /// <summary>
        /// 지정한 물리 모듈의 발 밑 부분의 충돌을 검증하는 메서드
        /// </summary>
        public static bool CastCylinderCollisionBelow(this IPhysics p_PhysicsModule, float p_SearchDistance, int p_LayerMask, QueryTriggerInteraction p_QueryTriggerInteraction)
        {
            var targetTransform = p_PhysicsModule._MasterNode._Transform;
            var radius = p_PhysicsModule.Radius.CurrentValue;
            var collisionStartPos = targetTransform.position + Vector3.up * radius;
            var hitCount = Physics.SphereCastNonAlloc(
                collisionStartPos, radius, 
                Vector3.down, _NonAllocRayCast, 
                p_SearchDistance, p_LayerMask, 
                p_QueryTriggerInteraction);
            
#if UNITY_EDITOR
            if (Application.isPlaying && CustomDebug.DrawPivot)
            {
                CustomDebug.DrawSphere(collisionStartPos, 0f, radius, Vector3.up, Color.red, 27);
                CustomDebug.DrawCircle(targetTransform.position, 0f, radius, Vector3.up, Color.blue, 27);
            }
#endif

            for (var i = 0; i < hitCount; i++)
            {
                var rayCastHit = _NonAllocRayCast[i];
                if (rayCastHit.transform != targetTransform)
                {
                    return true;
                }
            }
            
            return false;
        }
        
        /// <summary>
        /// 지정한 물리 모듈로부터 아래방향으로 일정 거리 안에 특정 레이어 컬라이더가 존재하는지 검증하는 메서드    
        /// 레이어 마스크가, 유닛 및 터레인으로 한정된 오버로드 메서드
        /// </summary>
        public static bool CastCylinderCollisionBelow(this IPhysics p_PhysicsModule, float p_SearchDistance, QueryTriggerInteraction p_QueryTriggerInteraction)
        {
            return p_PhysicsModule.CastCylinderCollisionBelow(p_SearchDistance, GameManager.Obstacle_Terrain_UnitEC_LayerMask, p_QueryTriggerInteraction);
        }
    }
}