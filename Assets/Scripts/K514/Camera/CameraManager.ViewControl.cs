#if !SERVER_DRIVE
using System;
using UnityEngine;

namespace BlackAm
{
    /// <summary>
    /// 카메라 래퍼들 중에서 두번째 상위 계급 Transform인 Root Wrapper Transform을 담당하는 부분 클래스
    /// 카메라를 흔드는 연출을 담당하는 부분 클래스
    /// 해당 이벤트는 벡터나 부동소수점의 러프 함수를 사용하지 않기에 다른 부분 클래스 처럼 외부 클래스를 통해
    /// 이벤트를 수행하지 않는다.
    /// </summary>
    public partial class CameraManager
    {
        #region <Fields>
        
        /// <summary>
        /// Wrapper Transform Collection ShortHand
        /// </summary>
        public Transform ViewControlWrapper { get; private set; }

        /// <summary>
        /// CameraRootWrapperTiltDegree 에 의해 정해지는 루트 Transform의 Look 벡터
        /// </summary>
        public Vector3 _CameraBaseLookWorldUnitVector => _ViewControlRotationWrapper.forward;
        
        /// <summary>
        /// CameraRootWrapperTiltDegree 에 의해 정해지는 루트 Transform의 Look 직교벡터
        /// </summary>
        public Vector3 _CameraBaseLookWorldPerpendicularUnitVector => _ViewControlRotationWrapper.right;

        /// <summary>
        /// 카메라 뷰 컨트롤에 관여하는 아핀변환 오브젝트
        /// </summary>
        private CameraAffineTransform _ViewControlAffineTransformObject;
        
        /// <summary>
        /// 카메라 뷰 컨트롤 회전 래퍼
        /// </summary>
        private Transform _ViewControlFocusWrapper, _ViewControlRotationWrapper, _ViewControlZoomWrapper;
        
        /// <summary>
        /// 카메라 뷰 컨트롤 포커스 오프셋 이벤트 리시버
        /// </summary>
        private PositionLerpEventTimerHandler _FocusOffsetHandler;

        /// <summary>
        /// 카메라 뷰 컨트롤 회전 이벤트 핸들러
        /// </summary>
        private DirectionLerpEventTimerHandler _ViewControlRotationHandler;
        
        /// <summary>
        /// 카메라 뷰 컨트롤 줌 이벤트 핸들러
        /// </summary>
        private FloatLerpEventTimerHandler _ZoomHandler;
        
        /// <summary>
        /// 카메라 뷰 컨트롤 인풋 이벤트 리시버
        /// </summary>
        private ControllerEventReceiver _ViewControlEventReceiver;

        /// <summary>
        /// 현재 뷰컨트롤 회전 방향
        /// </summary>
        public ArrowType _ViewControlRotationDirection;

        /// <summary>
        /// 뷰컨트롤이 입력된 후에 경과된 시간
        /// </summary>
        private float _CurrentDragDuration;

        /// <summary>
        /// 현재 로직 상 줌 거리, ZoomHandler의 CurrentValue 값이 실제 줌 거리이고
        /// 이쪽은 뷰컨트롤에 의해 화면을 확대/축소 해야하는 경우 수정된 거리값을 나타낸다
        /// </summary>
        private float _CurrentZoomDistanceAtLogic;

        /// <summary>
        /// 카메라 뷰 컨트롤 플래그 마스크
        /// </summary>
        private CameraViewControlProgressType CameraViewControlFlagMask;
        
        private ( float _SaveZoom, Vector3 _SaveFocus) _SaveDirectional;

        public bool Ancher = false;

        #endregion

        #region <Enums>

        [Flags]
        public enum CameraViewControlProgressType
        {
            /// <summary>
            /// 기본 값
            /// </summary>
            None = 0,
            
            /// <summary>
            /// 입력 장치를 통한 이벤트를 발생시키는 기능을 막는다.
            /// </summary>
            BlockManualControl = 1 << 0,
            
            /// <summary>
            /// 충돌 검증 로직(CheckViewControlZoomAgainstTerrain) 종료 후, 로직상 거리를 실제 적용중인 거리와 동기화 시킨다.
            /// </summary>
            SyncLogicZoomDistanceToCurrentZoomDistance = 1 << 1,
            
            /// <summary>
            /// 1인칭 촬영 모드에서 블록되어야할 플래그 마스크
            /// </summary>
            FirstPersonFocusModeFlagMask = BlockManualControl | SyncLogicZoomDistanceToCurrentZoomDistance,
        }

        #endregion
        
        #region <Callbacks>

        private void OnCreateViewControlPartial()
        {
            _ViewControlEventReceiver =
                ControllerTool.GetInstanceUnSafe
                    .GetControllerEventSender(ControllerTool.InputEventType.ControlView)
                    .GetEventReceiver<ControllerEventReceiver>(ControllerTool.InputEventType.ControlView, OnPropertyModified);
            
            _ViewControlAffineTransformObject = CameraAffineTransformCollection[CameraWrapperType.Root];
            _ViewControlFocusWrapper = _ViewControlAffineTransformObject._AffineBasisCollection[CameraAffineTransformType.Focus]._Transform;
            _ViewControlRotationWrapper = _ViewControlAffineTransformObject._AffineBasisCollection[CameraAffineTransformType.Rotate]._Transform;
            _ViewControlZoomWrapper = _ViewControlAffineTransformObject._AffineBasisCollection[CameraAffineTransformType.Zoom]._Transform;
            _FocusOffsetHandler = CameraAffineTransformCollection[CameraWrapperType.Root].FocusEventHandler;
            _ViewControlRotationHandler = CameraAffineTransformCollection[CameraWrapperType.Root].RotateEventHandler;
            _ZoomHandler = CameraAffineTransformCollection[CameraWrapperType.Root].ZoomEventHandler;
            _ViewControlRotationHandler.SetValueChangedCallback(OnControlViewRotated);
            _ZoomHandler.SetValueChangedCallback(OnControlViewZoomed);
        }

        public void OnPropertyModified(ControllerTool.InputEventType p_Type, ControllerTool.ControlEventPreset p_Preset)
        {
            // 1. 뷰컨트롤이 블록된 경우
            // 2. 뷰컨트롤 회전 이벤트가 진행중이었던 경우
            if (CameraViewControlFlagMask.HasAnyFlagExceptNone(CameraViewControlProgressType.BlockManualControl)
                || _ViewControlRotationHandler.IsEventValid())
            {
                return;
            }
            
            var targetCommand = p_Preset.CommandType;

            // 원상복귀
            if (targetCommand == CameraCommandTableData.GetCommandType(CameraCommandTableData.CameraCommandType.ResetView))
            {
                if (p_Preset.IsInputPress)
                {
                    // 요청으로 삭제됨
                    // ResetViewControl();
                }
            }
            // 뷰 회전
            else if (targetCommand == CameraCommandTableData.GetCommandType(CameraCommandTableData.CameraCommandType.RotateView))
            {
                var arrowType = p_Preset.ArrowType;
                if (arrowType != ArrowType.None)
                {
                    _ViewControlRotationHandler.CancelEvent();
                    _ViewControlRotationDirection = p_Preset.ArrowType;
                }
                else
                {
                    _CurrentDragDuration = _CurrentSceneCameraConfigure.RotationSpeedMinRate;
                }

                switch (p_Preset.GestureType)
                {
                    case ControllerTool.TouchGestureType.None:
                    case ControllerTool.TouchGestureType.Stable:
                        break;
                    case ControllerTool.TouchGestureType.Gather:
                        AddViewControlZoom(_CurrentSceneCameraConfigure.ZoomSpeed, Time.deltaTime, false);
                        break;
                    case ControllerTool.TouchGestureType.Scatter:
                        AddViewControlZoom(-_CurrentSceneCameraConfigure.ZoomSpeed, Time.deltaTime, false);
                        break;
                }
            }
        }

        /// <summary>
        /// 입력 이벤트에 의해, 카메라가 특정 방향(_ViewControlRotationDirection)으로 회전해야 하는 경우
        /// 호출되는 콜백
        /// </summary>
        private void OnUpdateViewControl(float p_DeltaTime)
        {
            SetViewControlRotation(_ViewControlRotationDirection, p_DeltaTime);
            
#if !KEEP_VIEW_DRAG_DIRECTION
            _ViewControlRotationDirection = ArrowType.None;
#endif
        }
        
        /// <summary>
        /// 뷰 컨트롤에 의한 카메라 회전 시에 카메라가 지형 너머로 넘어가지 않도록 거리를 보정해주는 메서드
        /// 루트 회전 변화 콜백에 의해서도 호출된다.
        /// </summary>
        private void OnControlViewRotated()
        {
            if(Ancher) return;
            
            _ViewControlAffineTransformObject.OnRotateChanged();
            CheckViewControlZoomAgainstTerrain();

            if (TracingTarget.IsValid())
            {
                _TraceUp_CameraLook_DotValue = Vector3.Dot(_CameraBaseLookWorldUnitVector, -TracingTarget._Transform.up);
                _TraceUp_CameraLook_DotValue_Abs = Mathf.Abs(_TraceUp_CameraLook_DotValue);
            }
            CameraEventSender.WhenPropertyModified(CameraEventType.Rotate, new CameraEventMessage());
        }
        
        /// <summary>
        /// 뷰 컨트롤에 의한 카메라 줌 이벤트 발생시 호출되는 메서드
        /// </summary>
        private void OnControlViewZoomed()
        {
            if(Ancher) return;
            
            _ViewControlAffineTransformObject.OnZoomChanged();
            _CurrentZoomRate = 1f + (_ZoomHandler._CurrentValue - GetCameraZoomLowerBound()) * _CurrentTraceTargetPreset.InverseZoomRate;
            CameraEventSender.WhenPropertyModified(CameraEventType.Zoom, new CameraEventMessage());
        }
        
        private void OnViewControlAffinePresetChanged()
        {
            if(Ancher) return;
            
            // 현재 로직상 줌 거리 값을 로드된 테이블 값과 동기화 시켜준다.
            _CurrentZoomDistanceAtLogic = _ZoomHandler._DefaultValue;

            // 초기 카메라 충돌을 체크해준다.
            CheckViewControlZoomAgainstTerrain();
        }

        #endregion

        #region <Methods>

        public void SetViewControlRotation(ArrowType p_DirectionType, float p_DeltaTime)
        {
            var direction = Vector2.Scale(_CurrentSceneCameraConfigure.CameraRotationSpeedRateMask, CustomMath.ArrowViewPortPerpendicularVectorCollection_RightHandPivotSystem[p_DirectionType]);
            var localPivot = _ViewControlRotationWrapper.TransformVector(direction);
            _CurrentDragDuration = Mathf.Min(_CurrentSceneCameraConfigure.RotationSpeedMaxRate, _CurrentDragDuration + p_DeltaTime * _CurrentSceneCameraConfigure.RotationSpeedRate);
            _ViewControlRotationHandler.AddValue(localPivot, _CurrentSceneCameraConfigure.RotationSpeed * _CurrentDragDuration * p_DeltaTime);
        }

        /// <summary>
        /// 뷰 컨트롤 줌 거리를 지정한 값으로 하는 메서드
        /// </summary>
        public void SetViewControlZoom(float p_ZoomDistance)
        {
            if (_ZoomHandler.IsEventValid())
            {
                _ZoomHandler.CancelEvent();
            }

            _CurrentZoomDistanceAtLogic = Mathf.Clamp(p_ZoomDistance, GetCameraZoomLowerBound(), GetCameraZoomUpperBound());
            CheckViewControlZoomAgainstTerrain();
        }
        
        /// <summary>
        /// 뷰 컨트롤 줌을 지정한 값만큼 확대/축소 시키는 메서드
        /// </summary>
        public void AddViewControlZoom(float p_ZoomSpeed, float p_DeltaTime, bool p_SetLogicDistanceFlag)
        {
            var targetDistance = p_SetLogicDistanceFlag ? _CurrentZoomDistanceAtLogic : _ZoomHandler._CurrentValue;
            SetViewControlZoom(targetDistance - p_ZoomSpeed * p_DeltaTime);
        }

        public void ResetViewControl()
        {
            ResetViewControlFocus();
            ResetViewControlRotate();
            ResetViewControlZoom();
        }

        public void ResetViewControlFocus()
        {
            _FocusOffsetHandler.ResetValueLerpTo(_CurrentSceneCameraConfigure.ResetLerpPreMsec, _CurrentSceneCameraConfigure.ResetLerpMsec);
        }

        /// <summary>
        /// 뷰 컨트롤 줌을 초기화 시키는 메서드
        /// </summary>
        public void ResetViewControlZoom()
        {
            _CurrentZoomDistanceAtLogic = Mathf.Clamp(_ZoomHandler._DefaultValue, GetCameraZoomLowerBound(), GetCameraZoomUpperBound());
            _ZoomHandler.ResetValueLerpTo(_CurrentSceneCameraConfigure.ResetLerpPreMsec, _CurrentSceneCameraConfigure.ResetLerpMsec);
        }

        /// <summary>
        /// 뷰 컨트롤 회전을 초기화 시키는 메서드
        /// </summary>
        public void ResetViewControlRotate()
        {
            _CurrentDragDuration = _CurrentSceneCameraConfigure.RotationSpeedMinRate;
            _ViewControlRotationHandler.ResetValueLerpTo(_CurrentSceneCameraConfigure.ResetLerpPreMsec, _CurrentSceneCameraConfigure.ResetLerpMsec);
        }

        /// <summary>
        /// 초점으로부터 카메라로 충돌체크를 하여, 카메라가 Terrain과 충돌하지 않도록 거리를 앞당기는 메서드
        /// 
        ///     1. 줌을 수동으로 변경된 경우
        ///     2. 카메라가 회전하는 경우
        ///     3. 추적 타겟이 움직이거나
        ///
        /// 같은 타이밍에 호출되며, 만약 카메라가 당겨지는 과정에서 추적대상과의 최소 거리가 보다 가까운 곳에서
        /// 충돌이 발생했다면 최소 거리를 우선적으로 지켜준다.
        /// </summary>
        public void CheckViewControlZoomAgainstTerrain()
        {
            // 지형 충돌을 수행한다.
            /*var (hasCollision, collisionDistance) = GetViewControlZoomCollisionDistance();

            // 충돌이 발생한 경우, 줌 거리를 충돌 거리 값으로 세트한다.
            // 이 때, 현재 줌 거리(_CurrentZoomDistanceAtLogic)는 갱신되지 않고 캐싱되어
            // 충돌을 벗어났을 때 원래 거리로 되돌리는데 사용된다.
            //
            // 또한 충돌 검증 메서드에서는 기본적으로 현재 줌 거리(_CurrentZoomDistanceAtLogic)를 최대값으로 진행되고
            // _CurrentZoomDistanceAtLogic는 SetZoom 계열 메서드에 의해 _CurrentTraceTargetPreset.FarBlockRadius 보다 항상 낮게 클램핑된다.
            //
            // 즉, _CurrentZoomDistanceAtLogic <= _CurrentTraceTargetPreset.FarBlockRadius 이므로 충돌 거리는 항상 카메라
            // 최대 거리를 상한 값으로 가지고, 하한 값(_CurrentTraceTargetPreset.NearBlockRadius)의 경우에는 충돌검증에서 따로 처리하지 않으므로
            // 아래 조건문에서 처리를 해준다.
            if (hasCollision)
            {
                _ZoomHandler.CancelEvent();
                _ZoomHandler.SetValue(collisionDistance);
            }
            // 충돌이 검증되지 않은 경우, 현재 줌 거리를 원본 값으로 되돌려준다.
            else
            {
                if (_ZoomHandler.IsEventValid())
                {
                    // 다른 줌 이벤트가 있다면, 그 이벤트를 우선한다.
                }
                else
                {
                    var eventSpawned = _ZoomHandler.SetValueLerpTo(_CurrentZoomDistanceAtLogic, 0, _CurrentSceneCameraConfigure.ResetLerpMsec);
                    if (eventSpawned)
                    {
                        CameraEventSender.WhenPropertyModified(CameraEventType.Zoom, new CameraEventMessage());
                    }
                }
            }

            if (CameraViewControlFlagMask.HasAnyFlagExceptNone(CameraViewControlProgressType.SyncLogicZoomDistanceToCurrentZoomDistance))
            {
                _CurrentZoomDistanceAtLogic = _ZoomHandler._CurrentValue;
            }*/
        }

        /// <summary>
        /// [초점 래퍼로부터 줌 래퍼 쪽으로] 지정한 거리만큼
        /// 박스 캐스팅을 수행하여 그 사이에 장해물이 존재하는지 검증하고
        /// 있다면 초점 래퍼로부터 가장 가까운 지형지물의 거리를 리턴하는 메서드
        /// </summary>
        /*private (bool, float) GetViewControlZoomCollisionDistance()
        {
            var rayDistance = _CurrentZoomDistanceAtLogic;
           
            // 포커스 대상이 있는 경우, 포커스 대상과 카메라 사이에 장해물이 있는지 검증한다.
            if (IsTracingTargetValid())
            {
                // 로직상 카메라 줌 거리와 현재 적용중인 줌 거리 간의 차이를 고려해서, 장해물 검증을 할 좌표를 구한다.
                var backDistanceOffset = (_ZoomHandler._CurrentValue - _CurrentZoomDistanceAtLogic) * _CameraBaseLookWorldUnitVector;
                var cameraPosition = MainCameraTransform.position + backDistanceOffset;
                
                // 충돌검증은 유닛 가상범위의 특정 4지점으로의 레이캐스팅으로 수행한다.
                var blockedRender = PhysicsTool.CheckAnyObject_RayCast_WithTargetPos(TracingTarget.GetTopPosition(), cameraPosition, rayDistance,
                                        GameManager.Obstacle_Terrain_LayerMask, true, QueryTriggerInteraction.Collide)
                                    && PhysicsTool.CheckAnyObject_RayCast_WithTargetPos(TracingTarget._Transform.position, cameraPosition, rayDistance,
                                        GameManager.Obstacle_Terrain_LayerMask, true, QueryTriggerInteraction.Collide)
                                    && PhysicsTool.CheckAnyObject_RayCast_WithTargetPos(TracingTarget.GetCenterPosition() + TracingTarget.GetRadius() * _CameraBaseLookWorldPerpendicularUnitVector, cameraPosition, rayDistance,
                                        GameManager.Obstacle_Terrain_LayerMask, true, QueryTriggerInteraction.Collide)
                                    && PhysicsTool.CheckAnyObject_RayCast_WithTargetPos(TracingTarget.GetCenterPosition() - TracingTarget.GetRadius() * _CameraBaseLookWorldPerpendicularUnitVector, cameraPosition, rayDistance,
                                        GameManager.Obstacle_Terrain_LayerMask, true, QueryTriggerInteraction.Collide);
                
                // 포커스 대상과 카메라 사이에 장해물이 검증된 경우, 카메라의 NearPlane만큼의 가상 직육면체 박스를 캐스팅하여
                // 카메라의 View가 위치할 거리를 구해준다.
                //
                // 굳이 레이캐스팅 후, 박스 캐스팅을 하는 이유는 박스캐스팅으로는 현재 유닛이 다른 장해물에 가려져있는지 파악하기가 어렵고
                // 레이캐스팅으로는 카메라의 NearPlane이 '잘리지 않고' 배치될 정확한 위치를 파악하기 어렵기 때문이다.
                if (blockedRender)
                {
                    return PhysicsTool.GetNearestObjectDistance_BoxCast
                    (
                        _ViewControlAffineTransformObject.Head.position,
                        -_ViewControlRotationHandler._CurrentValue, 
                        NearPlaneBoxHalfExtend,
                        _ViewControlRotationWrapper.rotation,
                        rayDistance, 
                        GameManager.Obstacle_Terrain_LayerMask
                    ); 
                }
                else
                {
                    return default;
                }
            }
            else
            {
                return PhysicsTool.GetNearestObjectDistance_BoxCast
                (
                    _ViewControlAffineTransformObject.Head.position,
                    -_ViewControlRotationHandler._CurrentValue, 
                    NearPlaneBoxHalfExtend,
                    _ViewControlRotationWrapper.rotation,
                    rayDistance, 
                    GameManager.Obstacle_Terrain_LayerMask
                ); 
            }
        }*/

        private void UpdateAffineSetting()
        {
            // 카메라 아핀 변환 제어 오브젝트에 현재 씬 테이블 레코드 바인딩
            _ViewControlAffineTransformObject.SetAffineTransform(_CurrentSceneVariableRecord);
            OnViewControlAffinePresetChanged();
        }

        private void AddViewControlBlockFlag(CameraViewControlProgressType p_Flag)
        {
            CameraViewControlFlagMask.AddFlag(p_Flag);
        }

        private void RemoveViewControlBlockFlag(CameraViewControlProgressType p_Flag)
        {
            CameraViewControlFlagMask.RemoveFlag(p_Flag);
        }
        #endregion
    }
}
#endif