#if !SERVER_DRIVE
using UnityEngine;

namespace BlackAm
{
    public partial class CameraManager
    {
        #region <Fields>
        
        /// <summary>
        /// 현재 선정된 추적 거리 프리셋
        /// </summary>
        private TraceTargetPreset _CurrentTraceTargetPreset;     
        
        /// <summary>
        /// 카메라 포커스 유닛
        /// </summary>
        private FocusableInstance TracingTarget;

        /// <summary>
        /// Smooth trace 중, 타겟 오브젝트와 최대로 거리가 벌어지는 반경
        /// </summary>
        private float _TracingSmoothRadius;
        
        /// <summary>
        /// 반경 하한
        /// </summary>
        private float _TracingSmoothRadiusNegative;

        /// <summary>
        /// Smooth Tracing Mode에서 사용할 Base Wrapper와 추적유닛 사이의 offset 벡터
        /// </summary>
        private Vector3 _SmoothTracingCameraOffset;

        #endregion

        #region <Callbacks>

        private void OnTracingTargetChanged(FocusableInstance p_PrevTraceTarget, FocusableInstance p_CurrentTraceTarget)
        {
            if (!ReferenceEquals(p_PrevTraceTarget, p_CurrentTraceTarget))
            {
                if (p_PrevTraceTarget.IsValid())
                {
                    p_PrevTraceTarget.OnCameraFocusTerminated();
                }

                if (p_CurrentTraceTarget.IsValid())
                {
                    p_CurrentTraceTarget.OnCameraFocused(CurrentCameraMode);
                }
                
                CameraEventSender.WhenPropertyModified(CameraEventType.TraceTargetChanged, new CameraEventMessage());
            }
        }

        private void OnCheckTargetMove()
        {
            if (TracingTarget.IsPositionChanged())
            {
                CheckViewControlZoomAgainstTerrain();
                CameraEventSender.WhenPropertyModified(CameraEventType.CameraPositionChanged, new CameraEventMessage());
            }
        }

        #endregion

        #region <Methods>

        private void UpdateTraceTargetSetting()
        {
            _CurrentTraceTargetPreset = _CurrentSceneCameraConfigure.TraceTargetPreset;
            InitCameraMode();
        }
        
        /// <summary>
        /// 지정한 오브젝트가 현재 카메라 매니저가 추적하는 오브젝트와 일치하는지 검증하는 논리 메서드
        /// </summary>
        public bool IsTracingTargetValid()
        {
            return !ReferenceEquals(null, TracingTarget);
        }
        
        /// <summary>
        /// 지정한 오브젝트가 현재 카메라 매니저가 추적하는 오브젝트와 일치하는지 검증하는 논리 메서드
        /// </summary>
        public bool IsTracingTarget(FocusableInstance p_TryObject)
        {
            return ReferenceEquals(TracingTarget, p_TryObject);
        }

        #endregion
        
        #region <Class>

        /// <summary>
        /// 추적 대상에 대한 거리를 기록하는 프리셋 클래스
        /// </summary>
        public class TraceTargetPreset
        {
            #region <Fields>

            /// <summary>
            /// 추적 대상 반경. Unit 계열의 클래스의 Physics Object의 Radius를 참조하거나
            /// 임의의 값을 세트한다.
            /// </summary>
            public float TraceTargetRadius;

            /// <summary>
            /// 추적 대상 반경 TraceTargetRadius에 곱해져서 카메라와 추적 대상 간의 거리 하한을 표시하는 값 배율
            /// </summary>
            private float NearBlockRate;
            
            /// <summary>
            /// 카메라와 추적 대상 간의 거리 하한 값
            /// 단, 해당 값은 뷰 컨트롤 줌의 수동 조작에만 적용되고 충돌에 의한 카메라 거리 보정에는 적용되지 않는다.
            /// </summary>
            public float NearBlockRadius;

            /// <summary>
            /// 추적 대상 반경에 TraceTargetRadius에 곱해져서 카메라와 추적 대상 간의 거리 상한을 표시하는 값 배율
            /// </summary>
            private float FarBlockRate;

            /// <summary>
            /// 카메라와 추적 대상 간의 거리 상한 값
            /// 단, 해당 값은 뷰 컨트롤 줌의 수동 조작에만 적용되고 충돌에 의한 카메라 거리 보정에는 적용되지 않는다.
            /// </summary>
            public float FarBlockRadius;

            /// <summary>
            /// NearBlockRadius 에서 Epsilon만큼 뺀 값
            /// 카메라의 뷰 컨트롤에서 줌 상태는 수동 조작시 [NearBlockRate, FarBlockRate] 구간 사이에 있지만
            /// 충돌 검증에 의한 자동 조작시에는 구간의 제한이 없기 때문에,
            ///
            /// 카메라가 포커스로부터 너무 멀어질 수도 있고
            /// 반대로 너무 가까워질 수도 있다.
            ///
            /// 따라서, 포커스와의 거리에 따라 포커스를 컬링할 필요가 있는데
            ///
            /// 카메라가 멀어지는 것에 의한 컬링은 이미 카메라 컴포넌트 자체에서 지원하고 있기 때문에 신경쓸 필요가 없으나
            /// 카메라가 가까워지는 것에 대한 컬링 정책은 없기 때문에 근접 컬링 기준 필드가 필요하다.
            ///
            /// 해당 값은 줌 하한 값 NearBlockRate에서 다시 아주 작은 값(ε)을 뺀 더 작은 거리값으로 해당 값보다 가깝게 포커스와 카메라가
            /// 배치되는 경우 근접 컬링을 수행한다.
            ///
            /// 굳이 NearBlockRate보다 더 작은 값을 사용하는 이유는 자동 줌 조절 등을 통해
            /// 현재 포커스와 카메라의 거리가 정확히 NearBlockRate일 때에는 컬링을 해제해주기 위함이다.
            /// 
            /// </summary>
            public float NearCullingRadius;

            /// <summary>
            /// 최대 줌 거리와 최소 줌 거리 차이의 역수값
            /// [NearBlockRate, FarBlockRate] 구간에서 현재 줌 거리 값의 진행도(ProgressRate)를 계산하는 용도로 사용한다.
            /// </summary>
            public float InverseZoomRate;
            
            #endregion

            #region <Constructor>

            public TraceTargetPreset(float p_NearBlockRate, float p_FarBlockRate)
            {
                TraceTargetRadius = 0f;
                NearBlockRate = p_NearBlockRate;
                NearBlockRadius = 0f;
                FarBlockRate = p_FarBlockRate;
                FarBlockRadius = 0f;
                NearCullingRadius = NearCullingRadiusLowerBound;
                InverseZoomRate = 0f;
            }

            #endregion

            #region <Callbacks>

            /// <summary>
            /// 추적 반경이 변경된 경우
            /// 변경된 반경을 기준으로 나머지 값을 재연산하는 콜백
            /// </summary>
            private void OnRadiusChanged()
            {
                NearBlockRadius = TraceTargetRadius * NearBlockRate;
                NearCullingRadius = Mathf.Max(NearCullingRadiusLowerBound, NearBlockRadius - CustomMath.Epsilon);
                FarBlockRadius = Mathf.Max(GetInstanceUnSafe._ZoomHandler._DefaultValue, TraceTargetRadius * FarBlockRate);
                InverseZoomRate = 1f / (FarBlockRadius - NearBlockRadius);
            }

            #endregion
            
            #region <Methods>

            /// <summary>
            /// 포커스 거리 하한을 지정하는 메서드
            /// </summary>
            public void SetNearBlockRadius(float p_Radius)
            {
                NearBlockRadius = Mathf.Clamp(p_Radius, 0f, FarBlockRadius);
                NearCullingRadius = Mathf.Max(NearCullingRadiusLowerBound, NearBlockRadius - CustomMath.Epsilon);
                InverseZoomRate = 1f / (FarBlockRadius - NearBlockRadius);
            }
            
            /// <summary>
            /// 포커스 거리 상한을 지정하는 메서드
            /// </summary>
            public void SetFarBlockRadius(float p_Radius)
            {
                FarBlockRadius = Mathf.Max(p_Radius, NearBlockRadius);
                InverseZoomRate = 1f / (FarBlockRadius - NearBlockRadius);
            }
            
            /// <summary>
            /// 추적 반경을 지정하는 메서드
            /// </summary>
            public void SetTraceTargetRadius(float p_Radius)
            {
                TraceTargetRadius = Mathf.Max(0f, p_Radius);
                OnRadiusChanged();
            }

            /// <summary>
            /// 추적 반경을 지정하는 메서드
            /// </summary>
            public void SetTraceTargetRadius(FocusableInstance p_TracingTarget)
            {
                var longRadius = Mathf.Max(p_TracingTarget.GetRadius(), p_TracingTarget.GetHeight(0.5f));
                SetTraceTargetRadius(longRadius);
            }

            #endregion
        }

        #endregion
    }
}
#endif