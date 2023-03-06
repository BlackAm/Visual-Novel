#if !SERVER_DRIVE
using UnityEngine;

namespace k514
{
    /// <summary>
    /// 카메라의 시야에 관련된 기능을 기술하는 부분 클래스
    /// </summary>
    public partial class CameraManager
    {
        #region <Fields>

        /// <summary>
        /// 고정 시점 타입
        /// </summary>
        private CameraFixingFocusType FixingFocusType;
        
        /// <summary>
        /// 해당 카메라의 View 벡터 기준으로 반앙각 아래 방향을 표시하는 아핀 객체
        /// </summary>
        private Transform _CameraFovHalfLowerLookVector;

        /// <summary>
        /// 카메라 랜더링 사각뿔대 근면 너비 절반 값
        /// </summary>
        private float NearPlaneBasisU;
        
        /// <summary>
        /// 카메라 랜더링 사각뿔대 근면 높이 절반 값
        /// </summary>
        private float NearPlaneBasisV;

        /// <summary>
        /// 카메라 랜더링 사각뿔대 근면에 외접하는 원의 반지름
        /// </summary>
        private float NearPlaneRadius;
        
        /// <summary>
        /// 카메라 랜더링 사각뿔대 근면에 외접하는 구의 반지름
        /// </summary>
        private float NearPlaneInnerCrossRadius;

        /// <summary>
        /// 박스 캐스트에 사용할 반경
        /// </summary>
        private Vector3 NearPlaneBoxHalfExtend;
        
        #endregion

        #region <Enums>

        public enum CameraFixingFocusType
        {
            None,
        }

        #endregion
        
        #region <Callbacks>

        private void OnCreateFixingFocusPartial()
        {
            /* 메인 카메라에 하위 오브젝트를 추가해준다. */
            _CameraFovHalfLowerLookVector = new GameObject("FovHalfLowerFowardIndicator").transform;
            _CameraFovHalfLowerLookVector.SetParent(MainCameraTransform, false);

            var fovHalf = MainCamera.fieldOfView * 0.5f;
            _CameraFovHalfLowerLookVector.Rotate(Vector3.right, fovHalf, Space.Self);
            var cameraNearPlaneDistance = MainCamera.nearClipPlane;
            NearPlaneBasisV = cameraNearPlaneDistance * Mathf.Tan(fovHalf * Mathf.Deg2Rad);
            NearPlaneBasisU = NearPlaneBasisV * MainCamera.aspect;
            NearPlaneRadius = CustomMath.GetPitagorasValue(NearPlaneBasisU, NearPlaneBasisV);
            NearPlaneInnerCrossRadius = CustomMath.GetPitagorasValue(NearPlaneBasisU, NearPlaneBasisV, cameraNearPlaneDistance);
            NearPlaneBoxHalfExtend = new Vector3(NearPlaneBasisU, NearPlaneBasisV, 0f);
        }

        #endregion

        #region <Methods>

        /// <summary>
        /// 카메라의 로컬 하단 방향 UV를 리턴하는 메서드
        /// </summary>
        private Vector3 GetFovHalfLowerLookVector()
        {
            return _CameraFovHalfLowerLookVector.forward;
        }

        /// <summary>
        /// 지정한 월드 좌표가 카메라 뷰포트 내에 있는지 검증하는 논리메서드
        /// </summary>
        public bool IsPositionAtCameraScreenSpace(Vector3 p_TargetPosition)
        {
            var viewPoint = MainCamera.WorldToViewportPoint(p_TargetPosition);
            var isEnteredViewPoint = viewPoint.x >= 0 && viewPoint.x <= 1 && viewPoint.y >= 0 && viewPoint.y <= 1 &&
                                     viewPoint.z > 0;
            return isEnteredViewPoint;
        }

        /// <summary>
        /// 지정한 오브젝트가 현재 장해물 등으로 가려져있는지 검증하는 메서드
        /// </summary>
        public bool IsBlockedToRender(FocusableInstance p_Target)
        {
            var lowerPosition = p_Target._Transform.position;
            var upperPosition = p_Target.GetTopPosition();
            var radiusOffset = p_Target.GetRadius() * MainCameraTransform.right;
            var leftPosition = p_Target.GetCenterPosition() - radiusOffset;
            var rightPosition = p_Target.GetCenterPosition() + radiusOffset;
            var startPosition = MainCameraTransform.position;
           
            return PhysicsTool.CheckAnyObject_RayCast(lowerPosition, startPosition, GameManager.Obstacle_Terrain_LayerMask, true, QueryTriggerInteraction.Collide)
                    && PhysicsTool.CheckAnyObject_RayCast(upperPosition, startPosition, GameManager.Obstacle_Terrain_LayerMask, true, QueryTriggerInteraction.Collide)
                    && PhysicsTool.CheckAnyObject_RayCast(leftPosition, startPosition, GameManager.Obstacle_Terrain_LayerMask, true, QueryTriggerInteraction.Collide)
                    && PhysicsTool.CheckAnyObject_RayCast(rightPosition, startPosition, GameManager.Obstacle_Terrain_LayerMask, true, QueryTriggerInteraction.Collide);
        }
        
        /// <summary>
        /// 지정한 오브젝트가 현재 장해물 등으로 가려져있는지 검증하는 메서드
        /// 캐스팅할 거리를 아는 경우 이쪽을 사용한다.
        /// </summary>
        public bool IsBlockedToRender(FocusableInstance p_Target, float p_Distance)
        {
            var lowerPosition = p_Target._Transform.position;
            var upperPosition = p_Target.GetTopPosition();
            var startPosition = MainCameraTransform.position;
 
            return PhysicsTool.CheckAnyObject_RayCast_WithTargetPos(lowerPosition, startPosition, p_Distance, GameManager.Obstacle_Terrain_LayerMask, true, QueryTriggerInteraction.Collide)
                   && PhysicsTool.CheckAnyObject_RayCast_WithTargetPos(upperPosition, startPosition, p_Distance, GameManager.Obstacle_Terrain_LayerMask, true, QueryTriggerInteraction.Collide);
        }

        /// <summary>
        /// 지정한 오브젝트가 카메라의 뷰푸트 안에 있는지 검증하고
        /// 필요하다면 카메라와 오브젝트 사이에 장해물이 있어서 랜더링할 수 있는지 까지 검증하는 논리메서드
        /// </summary>
        public bool IsCameraSeenObject(FocusableInstance p_Target, bool p_CheckRayCastToo)
        {
            var targetPosition = p_Target.GetCenterPosition();
            var baseUV = GetUnitVectorFromBase(targetPosition);
            var upperBound = p_Target.GetHeight(0.5f);
            var tryPosition = targetPosition - upperBound * baseUV;
            var viewPoint = MainCamera.WorldToViewportPoint(tryPosition);
            var isEnteredViewPoint = viewPoint.x >= 0 
                                     && viewPoint.x <= 1 && viewPoint.y >= 0 && viewPoint.y <= 1 
                                     && viewPoint.z > 0;
            if (p_CheckRayCastToo)
            {
                if (isEnteredViewPoint)
                {
                    return !IsBlockedToRender(p_Target);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return isEnteredViewPoint;
            }
        }

        #endregion
    }
}
#endif