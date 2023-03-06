using System;
using UI2020;
using UnityEngine;

namespace k514
{
    public partial class ControllerTool
    {
        #region <Fields>

        /// <summary>
        /// 이동 관련 이벤트 생성 대리자 OnMoveEventHandler
        /// OnMoveEventHandler 대리자는 각 커맨드 시스템의 입력 이벤트 타입에 따라 서로 다른 이벤트를 처리할 수 있으므로, GetMoveEventHandler 메서드를
        /// 통해 각 커맨드 시스템에 적당한 핸들러를 할당하는 방식을 사용한다.
        /// </summary>
        public delegate void OnMoveEventHandler(CommandType p_CommandType, InputEventType p_InputEventType, InputControllerType p_InputControllerType, ControllerInputStateType p_ControllerInputStateType);
        
        /// <summary>
        /// 키 입력 상태 참조 대리자 OnCheckKeyState
        /// OnCheckKeyState 대리자는 각 커맨드 시스템의 입력 디바이스 타입에 따라 서로 다른 이벤트를 처리할 수 있으므로, GetCheckKeyHandler 메서드를
        /// 통해 각 커맨드 시스템에 적당한 핸들러를 할당하는 방식을 사용한다.
        /// </summary>
        public delegate InputControllerType OnCheckKeyState(InputEventType p_InputEventType, int p_KeyCode);
        
        /// <summary>
        /// 커맨드 입력 이벤트 대리자
        /// 커맨드 입력에 대해서는 딱히 여러 버전의 핸들러가 존재하지 않으므로 각 커맨드시스템은
        /// 해당 ControllerTool의 핸들러를 공유한다.
        /// </summary>
        public delegate void OnSendCommandInput(int p_KeyCode, CommandType p_CommandType, InputEventType p_InputEventType, InputControllerType p_InputControllerType, ControllerInputStateType p_ControllerInputStateType);

        #endregion

        #region <Enums>

        public enum ArrowEventHandlerType
        {
            None,
            XZMove,
        }

        [Flags]
        public enum ControlMessageFlag
        {
            None = 0,
            Dash = 1 << 0,
            RestrictActivateSpell = 1 << 1,
        }

        #endregion
        
        #region <Delegate>

        /// <summary>
        /// 홀딩 커맨드가 입력된 경우, 이벤트를 전파하는 핸들러
        /// </summary>
        public OnSendCommandInput SendHoldingCommandInput;
        
        /// <summary>
        /// 연속 커맨드가 입력된 경우, 이벤트를 전파하는 핸들러
        /// </summary>
        public OnSendCommandInput SendSequenceCommandInput;
                
        /// <summary>
        /// 단일 커맨드가 입력된 경우, 이벤트를 전파하는 핸들러
        /// </summary>
        public OnSendCommandInput SendMonoCommandInput;
        
        /// <summary>
        /// 단일 커맨드가 입력된 경우, 이벤트를 전파하는 핸들러
        /// </summary>
        public OnSendCommandInput SendReleaseCommandInput;
        
        #endregion

        #region <Method/Delegate/OnCheckKeyState>

        /// <summary>
        /// OnCheckKeyState 핸들러를 초기화 하는 메서드
        /// </summary>
        public void GetCheckKeyHandler(InputControllerType p_InputControllerType, ref OnCheckKeyState r_GetKeyUp, ref OnCheckKeyState r_GetKeyDown)
        {
            switch (p_InputControllerType)
            {
                case InputControllerType.None:
                    r_GetKeyUp = (inputEventType, keyCode) => InputControllerType.None;
                    r_GetKeyDown = (inputEventType, keyCode) => InputControllerType.None;
                    return;
                case InputControllerType.Keyboard:
                    r_GetKeyUp = (inputEventType, keyCode) => Input.GetKeyUp((KeyCode)keyCode) ? InputControllerType.Keyboard : InputControllerType.None;
                    r_GetKeyDown = (inputEventType, keyCode) => Input.GetKeyDown((KeyCode)keyCode) ? InputControllerType.Keyboard : InputControllerType.None;
                    break;
#if !SERVER_DRIVE
                case InputControllerType.TouchPanel:
                    r_GetKeyUp = (inputEventType, keyCode) => TouchEventManager.GetInstance.GetKeyUp(inputEventType, keyCode) ? InputControllerType.TouchPanel : InputControllerType.None;
                    r_GetKeyDown = (inputEventType, keyCode) => TouchEventManager.GetInstance.GetKeyDown(inputEventType, keyCode) ? InputControllerType.TouchPanel : InputControllerType.None;
                    break;
                case InputControllerType.Keyboard_TouchPanel:
                    /* SE condition */
                    // 1. 키보드 입력이 없을 때에는, Input 클래스의 GetKey, GetKeyUp는 false를 리턴한다.
                    // 따라서, 키를 누르고 있지 않을 때는 두 함수가 모두 false를 만족할 때 뿐인데
                    // 두 함수가 모두 true일 경우는 존재할 수 없는 케이스라서, 아래와 같이 두 함수의 결과물이 같다면
                    // 키를 뗀것으로 간주한다.
                    //
                    // 주의할 점으로 GetKeyDown의 경우 최초에 눌렀을 때만 true를 리턴하기에
                    // 지속적으로 키가 눌렸는지 == true인지를 관측하기 위해서는 GetKeyDown 대신에 GetKey를 사용한다.
                    //
                    // 기본적으로 해당 입력시스템은 커맨드 시스템을 상정하고 있어서, GetKeyDown 이벤트를 검증할 때에는
                    // 최초에 한번만 입력을 인식받기 위해서 GetKeyDown을 사용하고 있다.
                    //
                    r_GetKeyUp = (inputEventType, keyCode) =>
                    {
                        // 어떤 입력을 릴리스하는 경우에는 두 입력 디바이스의 조건을 만족해야함.
                        // 따라서 리턴도 항상 Keyboard_TouchPanel 혹은 None이 된다.
                        var keyboardCheck = Input.GetKey((KeyCode)keyCode) == Input.GetKeyUp((KeyCode)keyCode);
                        var touchCheck = TouchEventManager.GetInstance.GetKeyUp(inputEventType, keyCode);

                        return keyboardCheck && touchCheck ? InputControllerType.Keyboard_TouchPanel : InputControllerType.None;
                    };
                    
                    r_GetKeyDown = (inputEventType, keyCode) =>
                    {
                        // 만약 터치/키보드 양쪽에서 입력 검증이 된다면, 터치를 우선적으로 리턴한다.
                        // 어떤 입력을 엔트리하는 경우에는 두 입력 디바이스의 조건 중 하나 이상을 만족해야함.
                        var keyboardCheck = Input.GetKeyDown((KeyCode)keyCode) ;
                        var touchCheck = TouchEventManager.GetInstance.GetKeyDown(inputEventType, keyCode);

                        if (touchCheck)
                        {
                            return InputControllerType.TouchPanel;
                        }
                        else
                        {
                            if (keyboardCheck)
                            {
                                return InputControllerType.Keyboard;
                            }
                            else
                            {
                                return InputControllerType.None;
                            }
                        }
                    };
      
                    break;
#endif
            }
        }
        
        #endregion

        #region <Method/Delegate/SendCommandInput>

        public void Initialize_SendCommandInputHandler()
        {
            SendHoldingCommandInput = HoldingCommandInputSendHandler;
            SendSequenceCommandInput = SequenceCommandInputSendHandler;
            SendMonoCommandInput = MonoCommandInputSendHandler;
            SendReleaseCommandInput = ReleaseCommandInputSendHandler;
        }

        private void HoldingCommandInputSendHandler
            (int p_KeyCode, CommandType p_CommandType, InputEventType p_InputEventType, InputControllerType p_InputControllerType, ControllerInputStateType p_ControllerInputStateType)
        {
            _InputEventSenderCollection[p_InputEventType].WhenPropertyModified
            (
                p_InputEventType,
                new ControlEventPreset(p_KeyCode, ControllerEventType.HoldingCommand, p_ControllerInputStateType,
                    p_CommandType, p_InputEventType, _InputEventPresetCollection[p_InputEventType].ControllerTracker)
            );
        }
        
        private void SequenceCommandInputSendHandler
            (int p_KeyCode, CommandType p_CommandType, InputEventType p_InputEventType, InputControllerType p_InputControllerType, ControllerInputStateType p_ControllerInputStateType)
        {
            _InputEventSenderCollection[p_InputEventType].WhenPropertyModified
            (
                p_InputEventType,
                new ControlEventPreset(p_KeyCode, ControllerEventType.SequenceCommand, p_ControllerInputStateType,
                    p_CommandType, p_InputEventType, _InputEventPresetCollection[p_InputEventType].ControllerTracker)
            );
        }
        
        private void MonoCommandInputSendHandler
            (int p_KeyCode, CommandType p_CommandType, InputEventType p_InputEventType, InputControllerType p_InputControllerType, ControllerInputStateType p_ControllerInputStateType)
        {
            switch (p_CommandType)
            {
                default: break;
                /*case CommandType.E: 
                    if(!MainGameUI.Instance.mainUI._quickSlot[2].isSkillMode) MainGameUI.Instance.mainUI._quickSlot[2].OnClickQuickSlot();
                    break;

                case CommandType.R: 
                    if(!MainGameUI.Instance.mainUI._quickSlot[3].isSkillMode) MainGameUI.Instance.mainUI._quickSlot[3].OnClickQuickSlot();
                    break;
                case CommandType.T: 
                    if(!MainGameUI.Instance.mainUI._quickSlot[4].isSkillMode) MainGameUI.Instance.mainUI._quickSlot[4].OnClickQuickSlot();
                    break;*/
            }

            // 퀵슬롯 - 내용이 스킬인 경우에만 아래가 실행됩니다..
            _InputEventSenderCollection[p_InputEventType].WhenPropertyModified
            (
                p_InputEventType,
                new ControlEventPreset(p_KeyCode, ControllerEventType.MonoCommand, p_ControllerInputStateType,
                    p_CommandType, p_InputEventType, _InputEventPresetCollection[p_InputEventType].ControllerTracker)
            );
        }

        private void ReleaseCommandInputSendHandler
            (int p_KeyCode, CommandType p_CommandType, InputEventType p_InputEventType, InputControllerType p_InputControllerType, ControllerInputStateType p_ControllerInputStateType)
        {
            _InputEventSenderCollection[p_InputEventType].WhenPropertyModified
            (
                p_InputEventType,
                new ControlEventPreset(p_KeyCode, ControllerEventType.MonoCommand, p_ControllerInputStateType,
                    p_CommandType, p_InputEventType, _InputEventPresetCollection[p_InputEventType].ControllerTracker)
            );
        }
        
        #endregion

        #region <Method/Delegate/OnMoveEventHandler>

        /// <summary>
        /// 지정한 이동 타입의 이벤트 핸들러를 리턴하는 메서드
        /// </summary>
        public void GetMoveEventHandler(ArrowEventHandlerType p_DirectionHandleType, ref OnMoveEventHandler r_OnMoveEventHandler, ref OnMoveEventHandler r_OnMoveStopEventHandler)
        {
            switch (p_DirectionHandleType)
            {
                case ArrowEventHandlerType.XZMove:
                    r_OnMoveEventHandler = XZMoveEventHandler;
                    r_OnMoveStopEventHandler = XZMoveStopEventHandler;
                    break;
                default:
                case ArrowEventHandlerType.None:
                    r_OnMoveEventHandler = null;
                    r_OnMoveStopEventHandler = null;
                    break;
            }
        }

        /// <summary>
        /// XZMove 이동 이벤트 핸들러
        /// </summary>
        private void XZMoveEventHandler(CommandType p_Type, InputEventType p_InputEventType, InputControllerType p_InputControllerType, ControllerInputStateType p_ControllerInputStateType)
        {
            _InputEventSenderCollection[p_InputEventType].WhenPropertyModified
            (
                p_InputEventType,
                new ControlEventPreset
                (
                    default, ControllerEventType.ArrowCommand, p_ControllerInputStateType,
                    p_Type, p_InputEventType, _InputEventPresetCollection[p_InputEventType].ControllerTracker, 
                    GetCommandSystem(p_InputEventType).DashFlag ? ControlMessageFlag.Dash : ControlMessageFlag.None
                )
            );
        }
        
        /// <summary>
        /// XZMove 이동정지 이벤트 핸들러
        /// </summary>
        private void XZMoveStopEventHandler(CommandType p_Type, InputEventType p_InputEventType, InputControllerType p_InputControllerType, ControllerInputStateType p_ControllerInputStateType)
        {
            _InputEventSenderCollection[p_InputEventType].WhenPropertyModified
            (
                p_InputEventType,
                new ControlEventPreset
                (
                    default, ControllerEventType.ArrowCommand, p_ControllerInputStateType,
                    p_Type, p_InputEventType, null, 
                    GetCommandSystem(p_InputEventType).DashFlag ? ControlMessageFlag.Dash : ControlMessageFlag.None
                )
            );
        }
      
        #endregion
        
        #region <Structs>

        public struct ControlEventPreset
        {
            #region <Fields>

            /// <summary>
            /// 해당 컨트롤러 이벤트의 키 타입
            /// </summary>
            public int KeyCode;

            /// <summary>
            /// 해당 컨트롤러 이벤트의 방향 타입, 만약 방향키가 아니었던 경우
            /// None 타입이 입력된다
            /// </summary>
            public ArrowType ArrowType;

            /// <summary>
            /// 컨트롤러 이벤트 타입
            /// </summary>
            public ControllerEventType ControllerEventType;

            /// <summary>
            /// 해당 컨트롤러 이벤트에 관여하는 트리거의 상태
            /// </summary>
            public ControllerInputStateType ControllerInputStateType;
            
            /// <summary>
            /// 해당 컨트롤러 이벤트의 명령 타입
            /// </summary>
            public CommandType CommandType;
            
            /// <summary>
            /// 해당 컨트롤러 이벤트의 입력 타입
            /// </summary>
            public InputEventType InputEventType;

            /// <summary>
            /// 추가 플래그 마스크
            /// </summary>
            public ControlMessageFlag ControlMessageFlagMask;
            
            /// <summary>
            /// 해당 컨트롤러 이벤트의 방향 월드 벡터
            /// </summary>
            public Vector3 WorldUV;
            
            /// <summary>
            /// 해당 컨트롤러 이벤트의 카메라 매니저의 메인 카메라 기준 로컬 벡터
            /// </summary>
            public Vector3 ViewPortUV;

            /// <summary>
            /// 해당 컨트롤러 이벤트가 시작된 타임스탬프
            /// </summary>
            public float InputStartTimeStamp;
            
            /// <summary>
            /// 해당 컨트롤러 이벤트가 입력된 이후 경과한 시간
            /// </summary>
            public float InputDuration;

#if !SERVER_DRIVE
            /// <summary>
            /// 해당 입력 이벤트의 제스쳐 타입
            /// </summary>
            public TouchGestureType GestureType;
#endif
            
            #endregion

            #region <Properties>

            public bool IsInputPress => ControllerInputStateType == ControllerInputStateType.PressKey;
            public bool IsInputHold => ControllerInputStateType == ControllerInputStateType.HoldingKey;
            public bool IsInputRelease => ControllerInputStateType == ControllerInputStateType.ReleaseKey;

            #endregion

            #region <Constructors>

            public ControlEventPreset(
                int p_KeyCode, ControllerEventType p_ControllerEventType, ControllerInputStateType p_ControllerInputStateType,
                CommandType p_CommandType, InputEventType p_InputEventType,
                IControllerTracker p_IControllerTracker,  ControlMessageFlag p_ControlMessageFlagMask = ControlMessageFlag.None)
            {
                UnityEngine.Debug.LogError($"1 ControlEventPreset {p_CommandType}");
                KeyCode = p_KeyCode;
                ControllerEventType = p_ControllerEventType;
                ControllerInputStateType = p_ControllerInputStateType;
                CommandType = p_CommandType;
                InputEventType = p_InputEventType;
                ControlMessageFlagMask = p_ControlMessageFlagMask;
                
                if (p_IControllerTracker != null)
                {
                    ArrowType = p_IControllerTracker.CurrentArrowType;
                    WorldUV = p_IControllerTracker.CurrentControllerUV;
#if SERVER_DRIVE
                    ViewPortUV = WorldUV;
#else
                    ViewPortUV = CameraManager.GetInstanceUnSafe.GetCameraUV(WorldUV);
#endif
                }
                else
                {
                    ArrowType = ArrowType.None;
                    WorldUV = Vector3.zero;
                    ViewPortUV = default;
                }
                
                InputStartTimeStamp = 0f;
                InputDuration = 0f;
#if !SERVER_DRIVE
                GestureType = TouchEventManager.GetInstance.GetGestureType(InputEventType);
#endif
            }

            #endregion

            #region <Operator>

            public static implicit operator ControlEventPreset(CommandType p_CommandType)
            {
                UnityEngine.Debug.LogError($"2 ControlEventPreset {p_CommandType}");

                return new ControlEventPreset
                (
                    default(int), default(ControllerEventType), 
                    default(ControllerInputStateType), p_CommandType, 
                    InputEventType.ControlUnit, default(IControllerTracker)
                );
            }

            #endregion

            #region <Methods>

            public void SetTimeStamp(float p_StartTimeStamp, float p_InputDuration)
            {
                InputStartTimeStamp = p_StartTimeStamp;
                InputDuration = p_InputDuration;
            }

            public void AddMessageFlag(ControlMessageFlag p_Flag)
            {
                ControlMessageFlagMask.AddFlag(p_Flag);
            }

            #endregion
        }

        #endregion
    }
}