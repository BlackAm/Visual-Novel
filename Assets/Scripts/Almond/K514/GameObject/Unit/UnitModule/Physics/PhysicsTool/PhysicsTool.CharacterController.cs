using UnityEngine;

namespace k514
{
    public partial class PhysicsTool
    {
        #region <Fields>
        
        public const float __Default_SkinWidth = 0.001f;
        public const float __Default_MinDistance = 0.001f;
        
        /// <summary>
        /// 만약 연직 아래로 0인 이동을 하면, 캐릭터 컨트롤러는 바닥을 무시하게 되므로 항상 미세한 값 만큼 연직 아래 방향으로 이동하도록 해야한다.
        /// 이 때, 미세한값을 정하는 기준은 캐릭터 컨트롤러가 이동을 하는 하한값인 minDistance 값과 캐릭터 컨트롤러가 충돌에 여유를 주는 값 skinWidth 보다 크게
        /// 움직여야 하므로, 둘 중에 큰 값을 1%만큼 보정하여 연직아래 방향으로 이동하도록 속도를 보정한다.
        /// </summary>
        public static readonly float _VelocityYLowerVectorFactor = -1.01f * Mathf.Max(__Default_MinDistance, __Default_SkinWidth);
        public static readonly float _VelocityYLowerVectorFactor_Negative = -_VelocityYLowerVectorFactor;

        #endregion

        #region <Methods>

        /// <summary>
        /// 캐릭터 컨트롤러 중심으로부터 레이캐스팅을 수행하여 충돌 정보를 리턴하는 메서드
        /// </summary>
        public static bool GetAnyObjectCenterBelow_CharacterController(CharacterController p_CharacterController, Transform p_Wrapper, int p_LayerMask, bool p_CorrectStartPosition, QueryTriggerInteraction p_QueryTriggerInteraction)
        {
            var uv = Vector3.down;
            var scaleFactor = p_Wrapper.lossyScale.y;
            var halfHeight = 0.5f * scaleFactor * p_CharacterController.height;
            var startPos = p_Wrapper.position + scaleFactor * p_CharacterController.center + halfHeight * uv;
            
            var hitCount = GetAnyObjectCount_RayCast(startPos, uv, CustomMath.Epsilon + p_CharacterController.skinWidth, p_LayerMask, p_CorrectStartPosition, p_QueryTriggerInteraction);
            return IsAnyAffine_ExistAt_RayCastResult_Except(p_Wrapper, hitCount);
        }
        
        /// <summary>
        /// 캐릭터 컨트롤러 하단부 구로부터 스피어캐스팅을 수행하여 충돌 정보를 리턴하는 메서드
        /// </summary>
        public static bool GetAnyObjectLowerSphereBelow_CharacterController(CharacterController p_CharacterController, Transform p_Wrapper, float p_Correctness, int p_LayerMask, bool p_CorrectStartPosition, QueryTriggerInteraction p_QueryTriggerInteraction)
        {
            var uv = Vector3.down;
            var scaleFactor = p_Wrapper.lossyScale.y;
            var halfHeight = 0.5f * scaleFactor * p_CharacterController.height;
            var radius = scaleFactor * p_CharacterController.radius;
            var startPos = p_Wrapper.position + scaleFactor * p_CharacterController.center + (halfHeight - radius) * uv;
            
            var hitCount = GetAnyObjectCount_SphereCast(startPos, uv, radius, CustomMath.Epsilon + p_Correctness + p_CharacterController.skinWidth, p_LayerMask, p_CorrectStartPosition, p_QueryTriggerInteraction);
            return IsAnyAffine_ExistAt_RayCastResult_Except(p_Wrapper, hitCount);
        }
        
        #endregion
    }
}