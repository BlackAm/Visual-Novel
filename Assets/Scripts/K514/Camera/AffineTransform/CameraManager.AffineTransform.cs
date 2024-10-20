#if !SERVER_DRIVE
using System.Collections.Generic;
using UnityEngine;

namespace BlackAm
{
    public partial class CameraManager
    {
        #region <Fields>

        /// <summary>
        /// [카메라 래퍼 타입, 카메라 아핀 변환 제어 오브젝트] 컬렉션
        /// </summary>
        private Dictionary<CameraWrapperType, CameraAffineTransform> CameraAffineTransformCollection;

        #endregion

        #region <Enums>

        /// <summary>
        /// 카메라 연출을 위한 아핀 변환 타입
        /// </summary>
        public enum CameraAffineTransformType
        {
            /// <summary>
            /// 카메라 초점 평행이동
            /// </summary>
            Focus,

            /// <summary>
            /// 카메라를 중심으로 하는 회전
            /// </summary>
            Rotate,
            
            /// <summary>
            /// 줌 인, 줌 아웃
            /// </summary>
            Zoom,
        }
        
        public CameraAffineTransformType[] CameraAffineTypeEnumerator;

        #endregion
        
        #region <Callbacks>

        private void OnCreateAffineTransformPartial()
        {
        }

        private void OnResetAffineTransformPartial()
        {
            CancelAllLerp();
        }
        
        #endregion

        #region <Methods>

        public bool IsAffineTransformWrapper(CameraWrapperType p_Type)
        {
            return CameraAffineTransformCollection[p_Type] != null;
        }

        #endregion

        #region <Method/SetValue>

        /// <summary>
        /// 카메라의 회전도를 지정한 값으로 세트하는 메서드
        /// </summary>
        public void SetDegree(CameraWrapperType p_Type, Vector3 p_Degree)
        {
            if (!IsAffineTransformWrapper(p_Type)) return;
            var targetHandler = CameraAffineTransformCollection[p_Type].RotateEventHandler;
            targetHandler.SetValue(p_Degree);
        }
        
        /// <summary>
        /// 카메라의 앙각을 지정한 값으로 세트하는 메서드
        /// </summary>
        public void SetTiltDegree(CameraWrapperType p_Type, float p_TiltDegree)
        {
            SetDegree(p_Type, p_TiltDegree * Vector3.right);
        }

        /// <summary>
        /// 카메라의 시야각을 지정한 값으로 세트하는 메서드
        /// </summary>
        public void SetSightDegree(CameraWrapperType p_Type, float p_SightDegree)
        {
            SetDegree(p_Type, p_SightDegree * Vector3.up);
        }
        
        /// <summary>
        /// 카메라의 거리를 지정한 값으로 세트하는 메서드
        /// </summary>
        public void SetCameraDistanceZoom(CameraWrapperType p_Type, float p_Distance)
        {
            if (!IsAffineTransformWrapper(p_Type)) return;
            var targetHandler = CameraAffineTransformCollection[p_Type].ZoomEventHandler;
            targetHandler.SetValue(p_Distance);
        }

        /// <summary>
        /// 카메라의 초점을 지정한 값으로 세트하는 메서드
        /// </summary>
        public void SetCameraFocusOffset(CameraWrapperType p_Type, Vector3 p_Offset)
        {
            if (!IsAffineTransformWrapper(p_Type)) return;
            var targetHandler = CameraAffineTransformCollection[p_Type].FocusEventHandler;
            targetHandler.SetValue(p_Offset);
        }

        public float GetCameraDistanceZoom(CameraWrapperType p_Type)
        {
            if (!IsAffineTransformWrapper(p_Type)) return default;
            var targetHandler = CameraAffineTransformCollection[p_Type].ZoomEventHandler;
            return targetHandler._CurrentValue;
        }

        public Vector3 GetCameraFocusOffset(CameraWrapperType p_Type)
        {
            if (!IsAffineTransformWrapper(p_Type)) return default;
            var targetHandler = CameraAffineTransformCollection[p_Type].FocusEventHandler;
            return targetHandler._CurrentValue;
        }
        
        #endregion
        
        #region <Method/AddValue>
        
        /// <summary>
        /// 카메라의 회전도를 지정한 값 만큼 더하는 메서드
        /// </summary>
        public void AddDegree(CameraWrapperType p_Type, Vector3 p_Degree, float p_DeltaTime)
        {
            if (!IsAffineTransformWrapper(p_Type)) return;
            var targetHandler = CameraAffineTransformCollection[p_Type].RotateEventHandler;
            targetHandler.AddValue(p_Degree, p_DeltaTime);
        }
        
        /// <summary>
        /// 카메라의 앙각을 지정한 값 만큼 더하는 메서드
        /// </summary>
        public void AddTiltDegree(CameraWrapperType p_Type, float p_TiltDegree, float p_DeltaTime)
        {
            AddDegree(p_Type, p_TiltDegree * Vector3.right, p_DeltaTime);
        }

        /// <summary>
        /// 카메라의 시야각을 지정한 값 만큼 더하는 메서드
        /// </summary>
        public void AddSightDegree(CameraWrapperType p_Type, float p_SightDegree, float p_DeltaTime)
        {
            AddDegree(p_Type, p_SightDegree * Vector3.up, p_DeltaTime);
        }
        
        /// <summary>
        /// 카메라의 거리를 지정한 값 만큼 더하는 메서드
        /// </summary>
        public void AddCameraDistanceZoom(CameraWrapperType p_Type, float p_Distance, float p_DeltaTime)
        {
            if (!IsAffineTransformWrapper(p_Type)) return;
            var targetHandler = CameraAffineTransformCollection[p_Type].ZoomEventHandler;
            targetHandler.AddValue(p_Distance, p_DeltaTime);
        }
        
        /// <summary>
        /// 카메라의 초점을 지정한 값 만큼 더하는 메서드
        /// </summary>
        public void AddCameraFocusOffset(CameraWrapperType p_Type, Vector3 p_Offset, float p_DeltaTime)
        {
            if (!IsAffineTransformWrapper(p_Type)) return;
            var targetHandler = CameraAffineTransformCollection[p_Type].FocusEventHandler;
            targetHandler.AddValue(p_Offset, p_DeltaTime);
        }
        
        #endregion
        
        #region <Method/ResetValue>

        /// <summary>
        /// 카메라의 회전도를 원본 값으로 초기화 시키는 메서드
        /// </summary>
        public void ResetDegree(CameraWrapperType p_Type)
        {
            if (!IsAffineTransformWrapper(p_Type)) return;
            CameraAffineTransformCollection[p_Type].RotateEventHandler.ResetValue();
        }
  
        /// <summary>
        /// 카메라의 거리를 원본 값으로 초기화 시키는 메서드
        /// </summary>
        public void ResetCameraDistanceZoom(CameraWrapperType p_Type)
        {
            if (!IsAffineTransformWrapper(p_Type)) return;
            var targetHandler = CameraAffineTransformCollection[p_Type].ZoomEventHandler;
            targetHandler.ResetValue();
        }
        
        /// <summary>
        /// 카메라의 초점을 원본 값으로 초기화 시키는 메서드
        /// </summary>
        public void ResetCameraFocusOffset(CameraWrapperType p_Type)
        {
            if (!IsAffineTransformWrapper(p_Type)) return;
            var targetHandler = CameraAffineTransformCollection[p_Type].FocusEventHandler;
            targetHandler.ResetValue();
        }
        
        #endregion

        #region <Method/SetLerp>

        /// <summary>
        /// 카메라의 회전도를 지정한 값으로 러프하는 메서드
        /// </summary>
        public void SetDegreeLerp(CameraWrapperType p_Type, Vector3 p_Degree, uint p_PreDelayMsec, uint p_LerpDurationMsec)
        {
            if (!IsAffineTransformWrapper(p_Type)) return;
            var targetHandler = CameraAffineTransformCollection[p_Type].RotateEventHandler;
            targetHandler.SetValueLerpTo(p_Degree, p_PreDelayMsec, p_LerpDurationMsec);
        }
        
        /// <summary>
        /// 카메라의 앙각을 지정한 값으로 러프하는 메서드
        /// </summary>
        public void SetTiltDegreeLerp(CameraWrapperType p_Type, float p_TiltDegree, uint p_PreDelayMsec, uint p_LerpDurationMsec)
        {
            SetDegreeLerp(p_Type, p_TiltDegree * Vector3.right, p_PreDelayMsec, p_LerpDurationMsec);
        }

        /// <summary>
        /// 카메라의 시야각을 지정한 값으로 러프하는 메서드
        /// </summary>
        public void SetSightDegreeLerp(CameraWrapperType p_Type, float p_SightDegree, uint p_PreDelayMsec, uint p_LerpDurationMsec)
        {
            SetDegreeLerp(p_Type, p_SightDegree * Vector3.up, p_PreDelayMsec, p_LerpDurationMsec);
        }
        
        /// <summary>
        /// 카메라의 거리를 지정한 값으로 러프하는 메서드
        /// </summary>
        public void SetCameraDistanceZoomLerp(CameraWrapperType p_Type, float p_Distance, uint p_PreDelayMsec, uint p_LerpDurationMsec)
        {
            if (!IsAffineTransformWrapper(p_Type)) return;
            var targetHandler = CameraAffineTransformCollection[p_Type].ZoomEventHandler;
            targetHandler.SetValueLerpTo(p_Distance, p_PreDelayMsec, p_LerpDurationMsec);
        }
        
        /// <summary>
        /// 카메라의 초점을 지정한 값으로 러프하는 메서드
        /// </summary>
        public void SetCameraFocusOffsetLerp(CameraWrapperType p_Type, Vector3 p_Offset, uint p_PreDelayMsec, uint p_LerpDurationMsec)
        {
            if (!IsAffineTransformWrapper(p_Type)) return;
            var targetHandler = CameraAffineTransformCollection[p_Type].FocusEventHandler;
            targetHandler.SetValueLerpTo(p_Offset, p_PreDelayMsec, p_LerpDurationMsec);
        }

        #endregion
        
        #region <Method/AddLerp>

        /// <summary>
        /// 카메라의 회전도를 지정한 값 만큼 더하는 메서드
        /// </summary>
        public void AddDegreeLerp(CameraWrapperType p_Type, Vector3 p_Degree, uint p_PreDelayMsec, uint p_LerpDurationMsec)
        {
            if (!IsAffineTransformWrapper(p_Type)) return;
            var targetHandler = CameraAffineTransformCollection[p_Type].RotateEventHandler;
            targetHandler.AddValueLerpTo(p_Degree, p_PreDelayMsec, p_LerpDurationMsec);
        }
        
        /// <summary>
        /// 카메라의 앙각을 지정한 값 만큼 더하는 메서드
        /// </summary>
        public void AddTiltDegreeLerp(CameraWrapperType p_Type, float p_TiltDegree, uint p_PreDelayMsec, uint p_LerpDurationMsec)
        {
            AddDegreeLerp(p_Type, p_TiltDegree * Vector3.right, p_PreDelayMsec, p_LerpDurationMsec);
        }

        /// <summary>
        /// 카메라의 시야각을 지정한 값 만큼 더하는 메서드
        /// </summary>
        public void AddSightDegreeLerp(CameraWrapperType p_Type, float p_SightDegree, uint p_PreDelayMsec, uint p_LerpDurationMsec)
        {
            AddDegreeLerp(p_Type, p_SightDegree * Vector3.up, p_PreDelayMsec, p_LerpDurationMsec);
        }
        
        /// <summary>
        /// 카메라의 거리를 지정한 값 만큼 더하는 메서드
        /// </summary>
        public void AddCameraDistanceZoomLerp(CameraWrapperType p_Type, float p_Distance, uint p_PreDelayMsec, uint p_LerpDurationMsec)
        {
            if (!IsAffineTransformWrapper(p_Type)) return;
            var targetHandler = CameraAffineTransformCollection[p_Type].ZoomEventHandler;
            targetHandler.AddValueLerpTo(p_Distance, p_PreDelayMsec, p_LerpDurationMsec);
        }
                
        /// <summary>
        /// 카메라의 초점을 지정한 값 만큼 더하는 메서드
        /// </summary>
        public void AddCameraFocusOffsetLerp(CameraWrapperType p_Type, Vector3 p_Offset, uint p_PreDelayMsec, uint p_LerpDurationMsec)
        {
            if (!IsAffineTransformWrapper(p_Type)) return;
            var targetHandler = CameraAffineTransformCollection[p_Type].FocusEventHandler;
            targetHandler.AddValueLerpTo(p_Offset, p_PreDelayMsec, p_LerpDurationMsec);
        }
        
        #endregion
        
        #region <Method/ResetLerp>

        /// <summary>
        /// 카메라의 회전을 원본 값으로 초기화 시키는 메서드
        /// </summary>
        public void ResetDegreeLerp(CameraWrapperType p_Type, uint p_PreDelayMsec, uint p_LerpDurationMsec)
        {
            if (!IsAffineTransformWrapper(p_Type)) return;
            CameraAffineTransformCollection[p_Type].RotateEventHandler.ResetValueLerpTo(p_PreDelayMsec, p_LerpDurationMsec);
        }
        
        /// <summary>
        /// 카메라의 거리를 원본 값으로 초기화 시키는 메서드
        /// </summary>
        public void ResetCameraDistanceZoomLerp(CameraWrapperType p_Type, uint p_PreDelayMsec, uint p_LerpDurationMsec)
        {
            if (!IsAffineTransformWrapper(p_Type)) return;
            var targetHandler = CameraAffineTransformCollection[p_Type].ZoomEventHandler;
            targetHandler.ResetValueLerpTo(p_PreDelayMsec, p_LerpDurationMsec);
        }
                                
        /// <summary>
        /// 카메라의 초점을 원본 값으로 초기화 시키는 메서드
        /// </summary>
        public void ResetCameraFocusOffsetLerp(CameraWrapperType p_Type, uint p_PreDelayMsec, uint p_LerpDurationMsec)
        {
            if (!IsAffineTransformWrapper(p_Type)) return;
            var targetHandler = CameraAffineTransformCollection[p_Type].FocusEventHandler;
            targetHandler.ResetValueLerpTo(p_PreDelayMsec, p_LerpDurationMsec);
        }
        
        /// <summary>
        /// 지정한 래퍼의 변환을 전부 초기화시키는 메서드
        /// </summary>
        public void ResetAllAffine(CameraWrapperType p_Type)
        {
            if (!IsAffineTransformWrapper(p_Type)) return;
            CameraAffineTransformCollection[p_Type].ResetAllAffineTransform();
        }
        
        /// <summary>
        /// 모든 래퍼의 변환을 모두 초기화시키는 메서드
        /// </summary>
        public void ResetAllLerp()
        {
            foreach (var cameraAffineTransformKV in CameraAffineTransformCollection)
            {
                var cameraAffineTransform = cameraAffineTransformKV.Value;
                cameraAffineTransform?.ResetAllAffineTransform();
            }
        }

        #endregion
        
        #region <Method/CheckEventValid>

        /// <summary>
        /// 카메라의 회전 변환 러프 이벤트가 진행중인지 여부를 검증하는 메서드
        /// </summary>
        public bool IsCameraDegreeLerpValid(CameraWrapperType p_Type)
        {
            if (!IsAffineTransformWrapper(p_Type)) return false;
            return CameraAffineTransformCollection[p_Type].RotateEventHandler.IsEventValid();
        }
        
        /// <summary>
        /// 카메라의 거리 변환 러프 이벤트가 진행중인지 여부를 검증하는 메서드
        /// </summary>
        public bool IsCameraDistanceZoomLerpValid(CameraWrapperType p_Type)
        {
            if (!IsAffineTransformWrapper(p_Type)) return false;
            var targetHandler = CameraAffineTransformCollection[p_Type].ZoomEventHandler;
            return targetHandler.IsEventValid();
        }
                                
        /// <summary>
        /// 카메라의 초점 변환 러프 이벤트가 진행중인지 여부를 검증하는 메서드
        /// </summary>
        public bool IsCameraFocusOffsetLerpValid(CameraWrapperType p_Type)
        {
            if (!IsAffineTransformWrapper(p_Type)) return false;
            var targetHandler = CameraAffineTransformCollection[p_Type].FocusEventHandler;
            return targetHandler.IsEventValid();
        }
        
        #endregion
        
        #region <Method/CancelLerp>

        /// <summary>
        /// 카메라의 회전 변환 러프 이벤트를 캔슬시키는 메서드
        /// </summary>
        public void CancelDegreeLerp(CameraWrapperType p_Type)
        {
            if (!IsAffineTransformWrapper(p_Type) || IsCameraDegreeLerpValid(p_Type)) return;
            CameraAffineTransformCollection[p_Type].RotateEventHandler.CancelEvent();
        }
        
        /// <summary>
        /// 카메라의 거리 변환 러프 이벤트를 캔슬시키는 메서드
        /// </summary>
        public void CancelCameraDistanceZoomLerp(CameraWrapperType p_Type)
        {
            if (!IsAffineTransformWrapper(p_Type) || IsCameraDistanceZoomLerpValid(p_Type)) return;
            var targetHandler = CameraAffineTransformCollection[p_Type].ZoomEventHandler;
            targetHandler.CancelEvent();
        }
                                       
        /// <summary>
        /// 카메라의 초점 변환 러프 이벤트를 캔슬시키는 메서드
        /// </summary>
        public void CancelCameraFocusOffsetLerp(CameraWrapperType p_Type)
        {
            if (!IsAffineTransformWrapper(p_Type) || IsCameraFocusOffsetLerpValid(p_Type)) return;
            var targetHandler = CameraAffineTransformCollection[p_Type].FocusEventHandler;
            targetHandler.CancelEvent();
        }
        
        /// <summary>
        /// 지정한 래퍼의 러프 이벤트를 전부 캔슬시키는 메서드
        /// </summary>
        public void CancelAllLerp(CameraWrapperType p_Type)
        {
            if (!IsAffineTransformWrapper(p_Type)) return;
            CancelDegreeLerp(p_Type);
            CancelCameraDistanceZoomLerp(p_Type);
            CancelCameraFocusOffsetLerp(p_Type);
        }
                
        /// <summary>
        /// 모든 래퍼의 러프 이벤트를 전부 캔슬시키는 메서드
        /// </summary>
        public void CancelAllLerp()
        {
            foreach (var cameraAffineTransformKV in CameraAffineTransformCollection)
            {
                var cameraAffineTransform = cameraAffineTransformKV.Value;
                cameraAffineTransform?.CancelAllCameraEvent();
            }
        }
        
        #endregion
    }
}
#endif