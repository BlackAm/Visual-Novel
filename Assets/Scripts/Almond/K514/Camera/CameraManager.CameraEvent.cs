#if !SERVER_DRIVE
using System;

namespace k514
{
    public partial class CameraManager
    {
        #region <Fields>

        private CameraEventSender CameraEventSender;
        
        /// <summary>
        /// [줌 최소거리, 줌 최대거리] 구간에서 현재 줌 거리 배율, 가장 가까울 때 1 가장 멀리 있을 때 2를 가진다.
        /// </summary>
        public float _CurrentZoomRate;

        /// <summary>
        /// 현재 추적 대상의 up벡터와 카메라 Look벡터 내적값
        /// </summary>
        public float _TraceUp_CameraLook_DotValue;
        
        /// <summary>
        /// 현재 추적 대상의 up벡터와 카메라 Look벡터 내적 절대값
        /// </summary>
        public float _TraceUp_CameraLook_DotValue_Abs;
        
        #endregion

        #region <Enums>

        [Flags]
        public enum CameraEventType
        {
            None = 0,
            CameraPositionChanged = 1 << 0,
            Zoom = 1 << 1,
            Rotate = 1 << 2,
            FarCullingDistanceChanged = 1 << 3,
            TraceTargetChanged = 1 << 4,
            
            WholeAffine = CameraPositionChanged | Zoom | Rotate,
        }

        #endregion

        #region <Callbakcs>

        private void OnCreateEventPartial()
        {
            CameraEventSender = new CameraEventSender();
        }

        #endregion

        #region <Methods>

        public void AddReceiver(CameraEventReceiver p_Receiver)
        {
            CameraEventSender.AddReceiver(p_Receiver);
        }

        #endregion
    }
}
#endif