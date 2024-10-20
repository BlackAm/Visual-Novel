#if !SERVER_DRIVE
using UnityEngine;

namespace BlackAm
{
    public partial class CameraManager
    {
        #region <Callbacks>

        private void OnCreateMathPartial()
        {
        }

        #endregion
        
        #region <Methods>

        /// <summary>
        /// 카메라로부터 특정 Transform으로의 방향 벡터를 리턴하는 메서드. 단위벡터 아님.
        /// </summary>
        public Vector3 GetDirectionTo(Transform p_TargetTransform)
        {
            return ViewControlWrapper.GetDirectionVectorTo(p_TargetTransform);
        }

        /// <summary>
        /// 지정한 Transform이 위치한 가시부피 좌표계 평면과 카메라 사이의 거리를 구하는 메서드.
        /// </summary>
        public float GetFrustumDistanceTo(Transform p_TargetTransform)
        {
            var tryProjectionVector = GetDirectionTo(p_TargetTransform);
            var lookVector = _CameraBaseLookWorldUnitVector;
            
            return tryProjectionVector.GetProjectionVector(lookVector).magnitude;
        }

        /// <summary>
        /// 카메라로부터 지정한 Transform 사이의 거리를 리턴하는 메서드.
        /// </summary>
        public float GetDistanceTo(Transform p_TargetTransform)
        {
            return GetDirectionTo(p_TargetTransform).magnitude;
        }

        /// <summary>
        /// 현재 카메라 Root를 기준으로 입력받은 UV를 모델 변환시켜주는 메서드
        /// </summary>
        public Vector3 GetCameraUV(Vector3 p_WorldUV)
        {
            return _ViewControlRotationWrapper.TransformDirection(p_WorldUV).XZVector();
        }

        /// <summary>
        /// 현재 카메라의 Look의 수직인 평면 및 카메라 거리비를 기준으로 스케일된 벡터를 리턴하는 메서드
        /// </summary>
        public Vector3 GetCameraScaledDirection(float p_X, float p_Y)
        {
            return  p_Y * MainCameraTransform.up + p_X * MainCameraTransform.right;
        }

        #endregion
    }
}
#endif