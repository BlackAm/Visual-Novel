#if !SERVER_DRIVE
using System.Collections.Generic;

namespace BlackAm
{
    /// <summary>
    /// 유니티 엔진에서는 터치에 관련된 입력 이벤트를 처리하는 기능을 제공하고 있지만, 해당 터치 기능들은
    /// 마우스와 독립적으로 동작하므로 테스트 용이성 및 플랫폼 이식성을 위해 해당 터치 라이브러리를 사용하지 않고
    /// 유니티에서 제공하는 포인터, 드래그 핸들러 콜백을 사용하여 마우스 및 터치 디스플레이 등의 다양한 입력 디바이스에 대응하는
    /// 입력 이벤트 핸들러 매니저
    /// </summary>
    public partial class TouchEventManager
    {
        #region <Fields>

        /// <summary>
        /// 지정한 이벤트 타입의 특정 키가 입력중인지 표시하는 컬렉션
        /// </summary>
        private Dictionary<ControllerTool.InputEventType, bool[]> _InputStateCollection;
        
        /// <summary>
        /// 터치 입력의 제스처 상태를 표시하는 컬렉션
        /// </summary>
        private Dictionary<ControllerTool.InputEventType, ControllerTool.TouchGestureType> _MobileStateCollection;
        
        /// <summary>
        /// 지정한 이벤트 타입의 입력이 대쉬인지 표시하는 컬렉션
        /// </summary>
        private Dictionary<ControllerTool.InputEventType, bool> _DashStateCollection;

        /// <summary>
        /// 현재 입력되는 커맨드 타입과 매핑되어 있는 액션 버튼 인스턴스 테이블
        /// </summary>
        public Dictionary<ControllerTool.CommandType, KeyCodeTouchEventSenderBase> ActionButtonMap;

        #endregion

        #region <Callbacks>

        private void OnAwakeKeyCode()
        {
            ActionButtonMap = new Dictionary<ControllerTool.CommandType, KeyCodeTouchEventSenderBase>();
        }

        public void OnRegistActionButton(ControllerTool.CommandType p_CommandType, KeyCodeTouchEventSenderBase p_Trigger)
        {
            ActionButtonMap.Add(p_CommandType, p_Trigger);
        }

        public void OnKeyPressed(ControllerTool.InputEventType p_Input, int p_KeyCode)
        {
            _InputStateCollection[p_Input][p_KeyCode] = true;
        }
        
        public void OnKeyReleased(ControllerTool.InputEventType p_Input, int p_KeyCode)
        {
            _InputStateCollection[p_Input][p_KeyCode] = false;
        }

        /// <summary>
        /// 드래그가 종료된 경우
        /// </summary>
        public void OnDragTerminated(DraggableKeyCodeTouchEventSenderBase inputTouchEventSender)
        {
            var eventType = inputTouchEventSender.ThisInputEvent;
            ResetArrowInputState(eventType);
            OnDragDashStateChanged(eventType, false);
        }

        /// <summary>
        /// 드래그 대쉬 플래그가 변경된 경우
        /// </summary>
        public void OnDragDashStateChanged(ControllerTool.InputEventType p_Input, bool p_OccurFlag)
        {
            _DashStateCollection[p_Input] = p_OccurFlag;
        }

        #endregion

        #region <Methods>

        public T GetCommandKeyMappedEventSender<T>(ControllerTool.CommandType p_CommandType)
            where T : KeyCodeTouchEventSenderBase
        {
            return ActionButtonMap[p_CommandType] as T;
        }

        public void ResetArrowInputState(ControllerTool.InputEventType p_InputEventType)
        {
            var arrowSet = CommandSystem.UnityArrowKeyValueSet;
            foreach (var arrowKeyCode in arrowSet)
            {
                OnKeyReleased(p_InputEventType, arrowKeyCode);
            }
        }

        public ControllerTool.TouchGestureType GetGestureType(ControllerTool.InputEventType p_InputEventType)
        {
            return _MobileStateCollection.ContainsKey(p_InputEventType) ? _MobileStateCollection[p_InputEventType] : ControllerTool.TouchGestureType.None;
        }
        
        public void SetGestureType(ControllerTool.InputEventType p_InputEventType,
            ControllerTool.TouchGestureType p_GestureType)
        {
            _MobileStateCollection[p_InputEventType] = p_GestureType;
        }


        public bool GetKeyUp(ControllerTool.InputEventType p_InputEventType, int p_KeyCode)
        {
            return !_InputStateCollection[p_InputEventType][p_KeyCode];
        }

        public bool GetKeyDown(ControllerTool.InputEventType p_InputEventType, int p_KeyCode)
        {
            return _InputStateCollection[p_InputEventType][p_KeyCode];
        }
        
        public bool GetDashState(ControllerTool.InputEventType p_InputEventType)
        {
            return _DashStateCollection[p_InputEventType];
        }

        private void ResetInputState()
        {
            var arrayScale = ControllerTool.GetInstanceUnSafe.KeyCodeScale;
            foreach (var inputEventTypeStatePair in _InputStateCollection)
            {
                var targetStateArray = inputEventTypeStatePair.Value;
                for (var i = 0; i < arrayScale; i++)
                {
                    targetStateArray[i] = false;
                }
            }
        }

        #endregion
    }
}
#endif