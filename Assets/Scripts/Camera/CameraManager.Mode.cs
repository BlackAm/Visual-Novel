
using System;
#if !SERVER_DRIVE
using UnityEngine;

namespace BlackAm
{
    public partial class CameraManager
    {
        #region <Consts>

        /// <summary>
        /// ObjectTracingSmoothLerp 모드 추적 반경 기본값
        /// </summary>
        private const float Default_Smooth_Trace_Radius = 1.8f;

        #endregion

        #region <Fields>

        /// <summary>
        /// Wrapper Transform Collection ShortHand
        /// </summary>
        public Transform BaseWrapper;

        /// <summary>
        /// 현재 카메라 촬영 모드
        /// </summary>
        private CameraMode CurrentCameraMode;

        #endregion
                
        #region <Enums>

        public enum CameraMode
        {
            /// <summary>
            /// 카메라가 특별히 움직이지 않는 경우
            /// </summary>
            None,
            
            /// <summary>
            /// 카메라가 특정 오브젝트를 추적하는 경우
            /// </summary>
            ObjectTracing,
            
            /// <summary>
            /// 카메라가 특정 오브젝트를 추적하는데, 보간을 이용하여 부드럽게 이동하는 경우
            /// </summary>
            ObjectTracingSmoothLerp,
            
            /// <summary>
            /// 카메라가 특정 오브젝트를 기준으로 1인칭 시점 연출을 한다.
            /// </summary>
            FirstPersonTracing,
        }

        #endregion

        #region <Callbacks>

        private void OnCreateBaseWrapperPartial()
        {
            
        }

        private void OnResetBaseWrapperPartial()
        {
            SetCameraModeNone();
            SetCameraSmoothTraceRadius(Default_Smooth_Trace_Radius);
        }

        private void OnCameraModeChanged(CameraMode p_PrevCameraMode, CameraMode p_CurrentCameraMode)
        {
            switch (p_PrevCameraMode)
            {
                case CameraMode.None:
                    break;
                case CameraMode.ObjectTracing:
                    break;
                case CameraMode.ObjectTracingSmoothLerp:
                    break;
                case CameraMode.FirstPersonTracing:
                    // 1인칭 모드가 해제된 경우, 뷰 컨트롤 기능을 활성화 시킨다.
                    RemoveViewControlBlockFlag(CameraViewControlProgressType.FirstPersonFocusModeFlagMask);
                    break;
            }
            
            switch (p_CurrentCameraMode)
            {
                case CameraMode.None:
                    break;
                case CameraMode.ObjectTracing:
                case CameraMode.ObjectTracingSmoothLerp:
                    ResetViewControl();
                    break;
                case CameraMode.FirstPersonTracing:
                    // 1인칭 모드에서는 뷰 컨트롤 기능이 동작하지 않음
                    AddViewControlBlockFlag(CameraViewControlProgressType.FirstPersonFocusModeFlagMask);
                    
                    // 카메라가 포커스에게 초근접할 수 있도록, 근접 컬링 거리 및
                    // 현재 포커싱 거리를 0으로 세팅해준다.
                    _CurrentTraceTargetPreset.SetNearBlockRadius(0f);
          
                    // * 여기서 Root 타입은 뷰 컨트롤에 사용되는 아핀 래퍼임.
                    // 1인칭 시점으로 보이도록 카메라 아핀값을 200ms에 걸쳐 조작한다.
                    SetCameraFocusOffsetLerp(CameraWrapperType.Root, TracingTarget.GetHeightOffsetVector(1f), 0, 200);
                    break;
            }

            if (TracingTarget.IsValid())
            {
                TracingTarget.OnCameraModeChanged(p_PrevCameraMode, p_CurrentCameraMode);
            }
        }

        public void OnCameraFocusDead(FocusableInstance p_Focus)
        {
            switch (CurrentCameraMode)
            {
                case CameraMode.None:
                    break;
                case CameraMode.ObjectTracing:
                case CameraMode.ObjectTracingSmoothLerp:
                    SetCameraModeNone();
                    break;
                case CameraMode.FirstPersonTracing:
                    SetCameraModeNone();
                    ResetViewControl();
                    break;
            }
        }

        #endregion

        #region <Methods>
        
        /// <summary>
        /// 카메라를 초기화 시키는 메서드
        /// </summary>
        private void InitCameraMode()
        {
            InitCameraMode(CurrentCameraMode, TracingTarget);
        }

        /// <summary>
        /// 카메라를 초기화 시키는 메서드
        /// </summary>
        private void InitCameraMode(CameraMode p_Type, FocusableInstance p_TracingTarget)
        {
            var prevCameraMode = CurrentCameraMode;
            var prevTarget = TracingTarget;
            
            if (p_TracingTarget.IsValid())
            {
                CurrentCameraMode = p_Type;
                TracingTarget = p_TracingTarget;
                _CurrentTraceTargetPreset.SetTraceTargetRadius(p_TracingTarget);
            }
            else
            {
                CurrentCameraMode = CameraMode.None;
                TracingTarget = null;
                _CurrentTraceTargetPreset.SetTraceTargetRadius(1f);
            }

            OnCameraModeChanged(prevCameraMode, CurrentCameraMode);
            OnTracingTargetChanged(prevTarget, TracingTarget);
        }

        public Vector3 GetUnitVectorFromBase(Vector3 p_TargetPos)
        {
            return BaseWrapper.position.GetDirectionUnitVectorTo(p_TargetPos);
        }

        #endregion
        
        #region <Method/None>
        
        /// <summary>
        /// 현재 카메라 모드를 None으로 전이시키는 메서드
        /// </summary>
        public void SetCameraModeNone()
        {
            InitCameraMode(CameraMode.None, null);
        }
        
        /// <summary>
        /// 현재 카메라 모드를 오브젝트 추적 모드로 전이시키는 메서드
        /// </summary>
        public void SetCameraModeTracingObject(FocusableInstance p_TargetTransform)
        {
            InitCameraMode(CameraMode.ObjectTracing, p_TargetTransform);
        }

        /// <summary>
        /// ObjectTracingSmoothLerp 모드에서 사용할 추적 시작 반경 
        /// </summary>
        public void SetCameraSmoothTraceRadius(float p_Radius)
        {
            _TracingSmoothRadius = p_Radius;
            _TracingSmoothRadiusNegative = -_TracingSmoothRadius;
        }

        /// <summary>
        /// 현재 카메라 모드를 오브젝트 Smooth 추적 모드로 전이시키는 메서드
        /// 2번째 파라미터 p_SmoothRadius는 지연 추적을 위해 유닛 초점과 최대로 멀어지는 반경을 의미한다.
        /// </summary>
        public void SetCameraModeSmoothTracingObject(FocusableInstance p_TargetTransform)
        {
            InitCameraMode(CameraMode.ObjectTracingSmoothLerp, p_TargetTransform);
            _SmoothTracingCameraOffset = Vector3.zero;
        }

        /// <summary>
        /// 현재 카메라 모드를 1인칭 시점으로 전이시키는 메서드
        /// </summary>
        public void SetCameraModeFirstPersonTracing(FocusableInstance p_TargetTransform)
        {
            InitCameraMode(CameraMode.FirstPersonTracing, p_TargetTransform);
        }

        #endregion
    }
}
#endif