#if !SERVER_DRIVE
using UnityEngine;

namespace BlackAm
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

        #endregion
    }
}
#endif