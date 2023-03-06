#if !SERVER_DRIVE
using UI2020;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace k514
{
    /// <summary>
    /// ITouchEvent를 통해 처리할 이벤트 중에 UI 터치 이벤트에 관한 추가 기능을 제공하는 인터페이스
    /// </summary>
    public interface IKeyCodeTouchEventSender : ITouchEvent, IPointerUpHandler, IPointerDownHandler
    {
        /// <summary>
        /// 해당 구현체에 터치 커맨드를 지정하는 메서드
        /// </summary>
        IKeyCodeTouchEventSender SetKeyCode(KeyCode p_KeyCode, ControllerTool.InputEventType p_EventType);

        /// <summary>
        /// 해당 구현체에 지정된 터치 커맨드를 제거하는 메서드
        /// </summary>
        IKeyCodeTouchEventSender ReleaseKeyCode();
    }

    /// <summary>
    /// ITouchEventSender 인터페이스의 유니티 컴포넌트 구현체
    /// </summary>
    public abstract class KeyCodeTouchEventSenderBase : UIManagerBase, IKeyCodeTouchEventSender
    {
        #region <Fields>

        /// <summary>
        /// UI 터치 이벤트 리스너
        /// </summary>
        protected Button _Button;

        /// <summary>
        /// 해당 구현체가 보유한 키 코드
        /// </summary>
        public int ButtonKeyCode;

        /// <summary>
        /// 해당 구현체가 보유한 키 코드로만 구성된 커맨드 타입
        /// </summary>
        public ControllerTool.CommandType SoloCommandType;

        /// <summary>
        /// 해당 구현체가 입력 이벤트를 보낼 입력 타입
        /// </summary>
        public ControllerTool.InputEventType ThisInputEvent;

        /// <summary>
        /// 해당 UI에 의해 발생하는 터치 이벤트 타입
        /// </summary>
        public TouchEventRoot.TouchInputType TouchEventFlagMask;

        #endregion

        #region <Callbacks>

        public override void OnSpawning()
        {
            base.OnSpawning();
            _Button = GetComponent<Button>();

#if UNITY_EDITOR
            var targetTable = KeyCodeCommandMapData.GetInstanceUnSafe.GetTable();
            var tryKeyCode = (KeyCode) ButtonKeyCode;
            if (targetTable.ContainsKey(tryKeyCode))
            {
                SoloCommandType = targetTable[tryKeyCode].SoloCommandCode;
            }
            else
            {
                Debug.LogError($"KeyCodeCommandMap 테이블에 키코드[{tryKeyCode}]에 대응하는 커맨드코드 레코드가 없습니다.");
            }
#else
            SoloCommandType = KeyCodeCommandMapData.GetInstanceUnSafe[(KeyCode) ButtonKeyCode].SoloCommandCode;
#endif
        }
        
        public override void OnUpdateUI(float p_DeltaTime)
        {
        }

        protected virtual void OnKeyCodeEventPointerDown(PointerEventData p_EventData)
        {
            TouchEventManager.GetInstance.OnKeyPressed(ThisInputEvent, ButtonKeyCode);
        }
        
        protected virtual void OnKeyCodeEventPointerUp(PointerEventData p_EventData)
        {
            TouchEventManager.GetInstance.OnKeyReleased(ThisInputEvent, ButtonKeyCode);
        }
        
        protected void OnUnitClickEventPointerDown(PointerEventData p_EventData)
        {
        }
        
        protected void OnUnitClickEventPointerUp(PointerEventData p_EventData)
        {
            TouchEventManager.GetInstance.TrySelectObject(p_EventData);
        }
        
        /// <summary>
        /// 인스턴스가 파기될 때 수행할 작업을 기술한다.
        /// </summary>
        protected override void DisposeUnManaged()
        {
            ReleaseKeyCode();
            
            base.DisposeUnManaged();
        }

        #endregion

        #region <Callback/EventTrigger>

        /// <summary>
        /// 터치가 감지된 경우의 유니티 엔진 입력 콜백
        /// </summary>
        public void OnPointerDown(PointerEventData p_EventData)
        {
            var enumerator = TouchEventRoot._TouchEventTypeEnumerator;
            foreach (var touchEventType in enumerator)
            {
                if (TouchEventFlagMask.HasAnyFlagExceptNone(touchEventType))
                {
                    switch (touchEventType)
                    {
                        case TouchEventRoot.TouchInputType.KeyCodeEvent:
                            OnKeyCodeEventPointerDown(p_EventData);
                            break;
                        case TouchEventRoot.TouchInputType.UnitClickEvent:
                            OnUnitClickEventPointerDown(p_EventData);
                            break;
                    }
                }
            }
        }
        
        /// <summary>
        /// 터치가 해제된 경우의 유니티 엔진 입력 콜백
        /// </summary>
        public void OnPointerUp(PointerEventData p_EventData)
        {
            var enumerator = TouchEventRoot._TouchEventTypeEnumerator;
            foreach (var touchEventType in enumerator)
            {
                if (TouchEventFlagMask.HasAnyFlagExceptNone(touchEventType))
                {
                    switch (touchEventType)
                    {
                        case TouchEventRoot.TouchInputType.KeyCodeEvent:
                            OnKeyCodeEventPointerUp(p_EventData);
                            break;
                        case TouchEventRoot.TouchInputType.UnitClickEvent:
                            OnUnitClickEventPointerUp(p_EventData);
                            break;
                    }
                }
            }
        }

        #endregion
        
        #region <Methods>
        
        /// <summary>
        /// 해당 구현체에 터치 커맨드를 지정하는 메서드
        /// </summary>
        public virtual IKeyCodeTouchEventSender SetKeyCode(KeyCode p_KeyCode, ControllerTool.InputEventType p_EventType)
        {
            ButtonKeyCode = (int)p_KeyCode;
            ThisInputEvent = p_EventType;
            return this;
        }

        /// <summary>
        /// 해당 구현체에 지정된 터치 커맨드를 제거하는 메서드
        /// </summary>
        public virtual IKeyCodeTouchEventSender ReleaseKeyCode()
        {
            ButtonKeyCode = default;
            ThisInputEvent = ControllerTool.InputEventType.None;
            
            return this;
        }

        #endregion
    }
}
#endif