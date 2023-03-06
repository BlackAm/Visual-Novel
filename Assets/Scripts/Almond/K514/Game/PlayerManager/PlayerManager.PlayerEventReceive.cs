#if !SERVER_DRIVE
namespace k514
{
    public partial class PlayerManager
    {
        #region <Fields>

        /// <summary>
        /// 플레이어의 직접적인 컨트롤은 플레이어가 보유한 인공지능 모듈(Passivity AI) 계열에서 처리하고,
        /// 해당 플레이어 매니저에 등록된 컨트롤러는 플레이어 상태와 무관하게 입력 상태를 추적하는 용도
        /// </summary>
        private ControllerEventReceiver PlayerControllerEventChecker;

        /// <summary>
        /// 플레이어 컨트롤러의 이동을 담당하는 트리거가 입력된 상태인지 표시하는 플래그
        /// </summary>
        private bool IsJoyStickKeepTrigger;

        /// <summary>
        /// 터치 입력 이벤트를 수신받는 객체
        /// </summary>
        private TouchEventReceiver TouchEventReceiver;
        
        #endregion

        #region <Callbacks>

        private void OnCreated_PlayerEventReceivePartial()
        {
            PlayerControllerEventChecker =             
                ControllerTool.GetInstanceUnSafe
                    .GetControllerEventSender(ControllerTool.InputEventType.ControlUnit)
                    .GetEventReceiver<ControllerEventReceiver>(ControllerTool.InputEventType.ControlUnit, OnControllerTriggered);
            
            TouchEventReceiver =
                TouchEventManager.GetInstance.GetEventReceiver<TouchEventReceiver>(
                    TouchEventRoot.TouchEventType.UnitSelected | TouchEventRoot.TouchEventType.PositionSelected, OnTouchEventTriggered);
        }

        #endregion
        
        #region <EventReceive/Controller>

        public void OnControllerTriggered(ControllerTool.InputEventType p_Type, ControllerTool.ControlEventPreset p_Preset)
        {
            var command = p_Preset.CommandType;
            switch (command)
            {
                case ControllerTool.CommandType.Move:
                    if (p_Preset.IsInputRelease)
                    {
                        OnPlayerControlJoyStickReleased(p_Preset);
                    }
                    else
                    {
                        OnPlayerControlJoyStickTriggered(p_Preset);
                    }
                    break;
            }
        }

        public void OnPlayerControlJoyStickTriggered(ControllerTool.ControlEventPreset p_Preset)
        {
            IsJoyStickKeepTrigger = true;
            // LamiereGameManager.GetInstanceUnSafe.OnJoystickTriggered();
        }

        public void OnPlayerControlJoyStickReleased(ControllerTool.ControlEventPreset p_Preset)
        {
            IsJoyStickKeepTrigger = false;
            // LamiereGameManager.GetInstanceUnSafe.OnJoystickReleased();
        }
        
        #endregion

        #region <EventReceive/TouchInput>

        private void OnTouchEventTriggered(TouchEventRoot.TouchEventType p_Type, TouchEventManager.TouchEventPreset p_Preset)
        {
            switch (p_Type)
            {
                case TouchEventRoot.TouchEventType.None:
                    break;
                case TouchEventRoot.TouchEventType.PlayerSelected:
                    break;
                case TouchEventRoot.TouchEventType.UnitSelected:
                    break;
                case TouchEventRoot.TouchEventType.PositionSelected:
                    /*
                        if (_BackingField_Player.IsValid() && p_Preset.PointerEventData.clickCount % 2 == 0)
                        {
                            var targetPosition = p_Preset.WorldVector;
                            var uv = Player._Transform.position.GetDirectionUnitVectorTo(targetPosition);
                            if (_BackingField_Player.OrderAIMoveTo(targetPosition, false))
                            {
                                var affine = new TransformTool.AffineCachePreset(targetPosition);
                                affine.SetBasis(uv);
                                var (valid, projector) = ProjectorSpawnManager.GetInstanceUnSafe.Project<SimpleProjector>(1, affine);
                                if (valid)
                                {
                                }
                            }
                        }
                    */
                    break;
            }
        }

        #endregion
    }
}
#endif