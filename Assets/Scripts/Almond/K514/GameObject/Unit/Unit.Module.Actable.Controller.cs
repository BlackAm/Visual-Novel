namespace k514
{
    public partial class Unit
    {
        #region <Consts>

        private const float __FIXED_FOCUS_ROTATION_SPEEDRATE = 1.5f;

        #endregion
        
        #region <Fields>

        /// <summary>
        /// 유닛 이동 액션 타입
        /// </summary>
        private UnitTool.UnitMoveActionType UnitMoveActionType;

        #endregion

        #region <Callbacks>

        private void OnPoolingController()
        {
            SetUnitMoveActionType(UnitTool.UnitMoveActionType.Default);
        }

#if !SERVER_DRIVE
        /// <summary>
        /// 카메라가 해당 유닛을 포커싱하기 시작한 경우 호출되는 콜백
        /// </summary>
        public override void OnCameraFocused(CameraManager.CameraMode p_ModeType)
        {
            switch (p_ModeType)
            {
                case CameraManager.CameraMode.None:
                case CameraManager.CameraMode.ObjectTracing:
                case CameraManager.CameraMode.ObjectTracingSmoothLerp:
                    SetUnitMoveActionType(UnitTool.UnitMoveActionType.Default);
                    break;
                case CameraManager.CameraMode.FirstPersonTracing:
                    SetUnitMoveActionType(UnitTool.UnitMoveActionType.FixingFocus);
                    break;
            }
        }

        /// <summary>
        /// 카메라의 촬영 모드가 변경된 경우 호출되는 콜백
        /// </summary>
        public override void OnCameraModeChanged(CameraManager.CameraMode p_PrevCameraMode, CameraManager.CameraMode p_CurrentCameraMode)
        {
            switch (p_CurrentCameraMode)
            {
                case CameraManager.CameraMode.None:
                case CameraManager.CameraMode.ObjectTracing:
                case CameraManager.CameraMode.ObjectTracingSmoothLerp:
                    SetUnitMoveActionType(UnitTool.UnitMoveActionType.Default);
                    break;
                case CameraManager.CameraMode.FirstPersonTracing:
                    SetUnitMoveActionType(UnitTool.UnitMoveActionType.FixingFocus);
                    break;
            }
        }

        /// <summary>
        /// 카메라가 해당 유닛으로부터 포커싱을 해제한 경우 호출되는 콜백
        /// </summary>
        public override void OnCameraFocusTerminated()
        {
            SetUnitMoveActionType(UnitTool.UnitMoveActionType.Default);
        }
#endif
        
        #endregion
        
        #region <Methods>

        private void SetUnitMoveActionType(UnitTool.UnitMoveActionType p_Type)
        {
            UnitMoveActionType = p_Type;
        }

        /// <summary>
        /// 입력 메시지에 의해 물리모듈이 지정한 벡터를 향하게 회전시키고
        /// 컨트롤러 속도계를 지정한 벡터값으로 덮어쓰는 메서드
        /// </summary>
        public bool ForceController(ControllerTool.ControlEventPreset p_EventPreset, float p_SpeedRate)
        {
            switch (UnitMoveActionType)
            {
                case UnitTool.UnitMoveActionType.FixingFocus:
                    var worldUV = p_EventPreset.WorldUV;
                    var uvX = worldUV.x;        
                    var xSignificant = uvX.GetSignificant();
                    var zSignificant = worldUV.z.GetSignificant();

                    switch (xSignificant)
                    {
                        case CustomMath.Significant.Minus:
                        case CustomMath.Significant.Plus:
                            RotateSelf(uvX * __FIXED_FOCUS_ROTATION_SPEEDRATE);
                            break;
                    }
                    
                    switch (zSignificant)
                    {
                        case CustomMath.Significant.Minus:
                            _PhysicsObject.OverlapVelocity(PhysicsTool.AccelerationType.SyncWithController,
                                -p_SpeedRate * GetScaledMovementSpeed() * _Transform.forward);
                            return true;
                        case CustomMath.Significant.Plus:
                            _PhysicsObject.OverlapVelocity(PhysicsTool.AccelerationType.SyncWithController,
                                p_SpeedRate * GetScaledMovementSpeed() * _Transform.forward);
                            return true;
                        default:
                        case CustomMath.Significant.Zero:
                            return false;
                    }
                case UnitTool.UnitMoveActionType.Default:
                default:
                {
                    SetLookAt(p_EventPreset.ViewPortUV);
                    _PhysicsObject.OverlapVelocity(PhysicsTool.AccelerationType.SyncWithController,
                        p_SpeedRate * GetScaledMovementSpeed() * _Transform.forward);

                    return true;
                }
            }
        }

        #endregion
    }
}