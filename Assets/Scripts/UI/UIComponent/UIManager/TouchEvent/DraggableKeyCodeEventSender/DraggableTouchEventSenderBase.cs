#if !SERVER_DRIVE
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BlackAm
{
    /// <summary>
    /// UI 터치 이벤트 중에 드래그를 포함하는 이벤트에 관한 추가 기능을 제공하는 인터페이스
    /// </summary>
    public interface IDraggableKeyCodeTouchEventSender : IKeyCodeTouchEventSender, IDragHandler, IControllerTrackerBridge
    {
        void OnDragRangeOver(PointerEventData p_EventData);
        void OnDragFollowRange(PointerEventData p_EventData, float p_DeltaDistanceSqr);
    }

    /// <summary>
    /// TouchEventSenderBase 에 드래그 이벤트 로직을 추가한 구현체
    /// </summary>
    public abstract class DraggableKeyCodeTouchEventSenderBase : KeyCodeTouchEventSenderBase, IDraggableKeyCodeTouchEventSender
    {
        #region <Consts>

        /// <summary>
        /// 드래그 최대 거리 제곱
        /// </summary>
        protected const float __MaxDistance = 100f;

        /// <summary>
        /// 드래그 방향 변환에 필요한 기저 변화량 하한값
        /// </summary>
        private const float __DragLowerBoundPositive = 0.257f;

        /// <summary>
        /// 드래그 방향 변환에 필요한 기저 변화량 하한값 음수
        /// </summary>
        private const float __DragLowerBoundNegative = -__DragLowerBoundPositive;

        /// <summary>
        /// 대쉬 인식 상한 비율
        /// </summary>
        private const float __Dash_Accept_Rate_UpperBound = 0.9f;

        /// <summary>
        /// 대쉬 인식 거리 제곱
        /// </summary>
        private const float __DashDistance = __MaxDistance * __Dash_Accept_Rate_UpperBound;

        #endregion

        #region <Fields>

        /// <summary>
        /// 드래그 위치를 비교할 최초 오브젝트 위치
        /// </summary>
        protected Vector2 _BaseOffset;

        /// <summary>
        /// 기저 좌표를 다루는 열거형 상수
        /// </summary>
        private BaseOffsetUpdateType _BaseOffsetUpdateType;

        /// <summary>
        /// 현재 입력된 뷰포트 좌표계 유닛 벡터
        /// </summary>
        protected Vector3 CurrentControllerViewPortUV { get; private set; }

        /// <summary>
        /// 방향 기록 오브젝트
        /// </summary>
        public IControllerTracker ControllerTracker { get; set; }

        #endregion

        #region <Enums>

        /// <summary>
        /// 드래그 이벤트의 기저(BaseOffset) 선정 타입
        /// </summary>
        public enum BaseOffsetUpdateType
        {
            /// <summary>
            /// OnCreated 콜백에 의해 초기화된 값을 그대로 사용하는 경우 
            /// </summary>
            Static,

            /// <summary>
            /// 클릭시 클릭 지점을 기저로 변경하는 경우
            /// </summary>
            UpdateOnInput,

            /// <summary>
            /// 드래그 시 마지막 지점을 기저로 변경하는 경우
            /// </summary>
            UpdateOnDrag,
        }

        #endregion

        #region <Callbacks>

        public override void OnSpawning()
        {
            base.OnSpawning();
            _BaseOffset = _Transform.position.XYVector2();
            SetBaseOffUpdateType(BaseOffsetUpdateType.Static);
        }

        /// <summary>
        /// 터치가 인식된 경우의 유니티 엔진 입력 콜백
        /// </summary>
        protected override void OnKeyCodeEventPointerDown(PointerEventData p_EventData)
        {
            base.OnKeyCodeEventPointerDown(p_EventData);

            switch (_BaseOffsetUpdateType)
            {
                case BaseOffsetUpdateType.Static:
                    break;
                case BaseOffsetUpdateType.UpdateOnInput:
                case BaseOffsetUpdateType.UpdateOnDrag:
                    _BaseOffset = p_EventData.position;
                    break;
            }
        }

        /// <summary>
        /// 터치가 해제된 경우의 유니티 엔진 입력 콜백
        /// </summary>
        protected override void OnKeyCodeEventPointerUp(PointerEventData p_EventData)
        {
            ControllerTracker.SetControllerDirection(ArrowType.None, Vector3.zero);
            TouchEventManager.GetInstance.OnDragTerminated(this);

            base.OnKeyCodeEventPointerUp(p_EventData);
        }

        protected virtual void OnKeyCodeEventDrag(PointerEventData p_EventData)
        {
            var joystickDirection = p_EventData.position - _BaseOffset;
            var joystickDirectionMagnitude = joystickDirection.magnitude;

            CurrentControllerViewPortUV = joystickDirection.normalized;
            var currentControllerUV = new Vector3(CurrentControllerViewPortUV.x, 0f, CurrentControllerViewPortUV.y);
            var currentArrowType = ArrowType.None;

            // U방향에 대해서
            if (currentControllerUV.x > __DragLowerBoundPositive)
            {
                TouchEventManager.GetInstance.OnKeyReleased(ThisInputEvent, ControllerTool.LEFT_ARROW_KEYCODE_INDEX);
                TouchEventManager.GetInstance.OnKeyPressed(ThisInputEvent, ControllerTool.RIGHT_ARROW_KEYCODE_INDEX);
                currentArrowType += (int)ArrowType.SoloRight;
            }
            else if (currentControllerUV.x < __DragLowerBoundNegative)
            {
                TouchEventManager.GetInstance.OnKeyPressed(ThisInputEvent, ControllerTool.LEFT_ARROW_KEYCODE_INDEX);
                TouchEventManager.GetInstance.OnKeyReleased(ThisInputEvent, ControllerTool.RIGHT_ARROW_KEYCODE_INDEX);
                currentArrowType += (int)ArrowType.SoloLeft;
            }
            else
            {
                TouchEventManager.GetInstance.OnKeyReleased(ThisInputEvent, ControllerTool.LEFT_ARROW_KEYCODE_INDEX);
                TouchEventManager.GetInstance.OnKeyReleased(ThisInputEvent, ControllerTool.RIGHT_ARROW_KEYCODE_INDEX);
            }

            // V방향에 대해서
            if (currentControllerUV.z > __DragLowerBoundPositive)
            {
                TouchEventManager.GetInstance.OnKeyPressed(ThisInputEvent, ControllerTool.UPPER_ARROW_KEYCODE_INDEX);
                TouchEventManager.GetInstance.OnKeyReleased(ThisInputEvent, ControllerTool.LOWER_ARROW_KEYCODE_INDEX);
                currentArrowType += (int)ArrowType.SoloUp;
            }
            else if (currentControllerUV.z < __DragLowerBoundNegative)
            {
                TouchEventManager.GetInstance.OnKeyReleased(ThisInputEvent, ControllerTool.UPPER_ARROW_KEYCODE_INDEX);
                TouchEventManager.GetInstance.OnKeyPressed(ThisInputEvent, ControllerTool.LOWER_ARROW_KEYCODE_INDEX);
                currentArrowType += (int)ArrowType.SoloDown;
            }
            else
            {
                TouchEventManager.GetInstance.OnKeyReleased(ThisInputEvent, ControllerTool.UPPER_ARROW_KEYCODE_INDEX);
                TouchEventManager.GetInstance.OnKeyReleased(ThisInputEvent, ControllerTool.LOWER_ARROW_KEYCODE_INDEX);
            }

            ControllerTracker.SetControllerDirection(currentArrowType, currentControllerUV);

            // 드래그 거리에 따른 이벤트를 처리한다.
            if (joystickDirectionMagnitude > __MaxDistance)
            {
                OnDragRangeOver(p_EventData);
            }
            else
            {
                OnDragFollowRange(p_EventData, joystickDirectionMagnitude);
            }

            switch (_BaseOffsetUpdateType)
            {
                case BaseOffsetUpdateType.Static:
                case BaseOffsetUpdateType.UpdateOnInput:
                    break;
                case BaseOffsetUpdateType.UpdateOnDrag:
                    _BaseOffset = p_EventData.position;
                    break;
            }
        }

        protected virtual void OnKeyCodeEventDragRangeOver(PointerEventData p_EventData)
        {
            TouchEventManager.GetInstance.OnDragDashStateChanged(ThisInputEvent, true);
        }

        protected virtual void OnKeyCodeEventDragFollowRange(PointerEventData p_EventData, float p_DeltaDistanceSqr)
        {
            TouchEventManager.GetInstance.OnDragDashStateChanged(ThisInputEvent, p_DeltaDistanceSqr > __DashDistance);
        }

        #endregion

        #region <Callback/EventTrigger>

        /// <summary>
        /// 해당 터치 컴포넌트로 드래그 이벤트가 발생하는 경우 유니티 엔진으로부터 호출되는 입력 콜백
        /// </summary>
        public void OnDrag(PointerEventData p_EventData)
        {
            var enumerator = TouchEventRoot._TouchEventTypeEnumerator;
            foreach (var touchEventType in enumerator)
            {
                if (TouchEventFlagMask.HasAnyFlagExceptNone(touchEventType))
                {
                    switch (touchEventType)
                    {
                        case TouchEventRoot.TouchInputType.KeyCodeEvent:
                            OnKeyCodeEventDrag(p_EventData);
                            break;
                        case TouchEventRoot.TouchInputType.UnitClickEvent:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 드래그 범위를 벗어나 이동한 경우
        /// </summary>
        public void OnDragRangeOver(PointerEventData p_EventData)
        {
            var enumerator = TouchEventRoot._TouchEventTypeEnumerator;
            foreach (var touchEventType in enumerator)
            {
                if (TouchEventFlagMask.HasAnyFlagExceptNone(touchEventType))
                {
                    switch (touchEventType)
                    {
                        case TouchEventRoot.TouchInputType.KeyCodeEvent:
                            OnKeyCodeEventDragRangeOver(p_EventData);
                            break;
                        case TouchEventRoot.TouchInputType.UnitClickEvent:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 드래그 범위 안에서 이동한 경우
        /// </summary>
        public void OnDragFollowRange(PointerEventData p_EventData, float p_DeltaDistanceSqr)
        {
            var enumerator = TouchEventRoot._TouchEventTypeEnumerator;
            foreach (var touchEventType in enumerator)
            {
                if (TouchEventFlagMask.HasAnyFlagExceptNone(touchEventType))
                {
                    switch (touchEventType)
                    {
                        case TouchEventRoot.TouchInputType.KeyCodeEvent:
                            OnKeyCodeEventDragFollowRange(p_EventData, p_DeltaDistanceSqr);
                            break;
                        case TouchEventRoot.TouchInputType.UnitClickEvent:
                            break;
                    }
                }
            }
        }

        #endregion

        #region <Methods>

        public void SetBaseOffUpdateType(BaseOffsetUpdateType p_BaseOffsetUpdateType)
        {
            _BaseOffsetUpdateType = p_BaseOffsetUpdateType;
        }

        /// <summary>
        /// 해당 구현체에 터치 커맨드를 지정하는 메서드
        /// </summary>
        public override IKeyCodeTouchEventSender SetKeyCode(KeyCode p_KeyCode, ControllerTool.InputEventType p_EventType)
        {
            base.SetKeyCode(p_KeyCode, p_EventType);
            ControllerTracker = ControllerTool.GetInstanceUnSafe.GetControllerTracker(p_EventType);
            return this;
        }

        #endregion
    }

    /// <summary>
    /// DraggableTouchEventSenderBase 에 핸들(pivot) 로직이 추가된 구현체
    /// </summary>
    public abstract class DraggableKeyCodeTouchEventSenderBaseWithHandle : DraggableKeyCodeTouchEventSenderBase
    {
        #region <Fields>

        /// <summary>
        /// 핸들 이미지
        /// </summary>
        protected Image _Handle;

        /// <summary>
        /// 핸들 이미지 래퍼 오브젝트
        /// </summary>
        protected Transform _HandleTransform;

        #endregion

        #region <Callbacks>

        public override void OnSpawning()
        {
            base.OnSpawning();
            
        }

        /// <summary>
        /// 터치가 실행되는 경우, 핸들을 원점으로 되돌려준다.
        /// </summary>
        protected override void OnKeyCodeEventPointerDown(PointerEventData p_EventData)
        {
            base.OnKeyCodeEventPointerDown(p_EventData);
            _HandleTransform.localPosition = Vector3.zero;
        }

        /// <summary>
        /// 터치가 종료되는 경우, 핸들을 원점으로 되돌려준다.
        /// </summary>
        protected override void OnKeyCodeEventPointerUp(PointerEventData p_EventData)
        {
            base.OnKeyCodeEventPointerUp(p_EventData);
            _HandleTransform.localPosition = Vector3.zero;
        }

        /// <summary>
        /// 핸들은 드래그 범위를 벗어날 수 없다.
        /// </summary>
        protected override void OnKeyCodeEventDragRangeOver(PointerEventData p_EventData)
        {
            base.OnKeyCodeEventDragRangeOver(p_EventData);
            _HandleTransform.localPosition = __MaxDistance * CurrentControllerViewPortUV;
        }

        /// <summary>
        /// 핸들은 드래그 위치와 동기화 된다.
        /// </summary>
        protected override void OnKeyCodeEventDragFollowRange(PointerEventData p_EventData, float p_DeltaDistanceSqr)
        {
            base.OnKeyCodeEventDragFollowRange(p_EventData, p_DeltaDistanceSqr);
            _HandleTransform.localPosition = p_DeltaDistanceSqr * CurrentControllerViewPortUV;
        }

        #endregion

        #region <Methods>

        /// <summary>
        /// 핸들 오브젝트 이름을 리턴하는 메서드
        /// </summary>
        public abstract string GetHandlePivotName();

        #endregion
    }

    /// <summary>
    /// 드래그 이벤트에 터치 제스쳐를 처리할 수 있는 기능을 추가한 기본 클래스
    /// 터치가 하나 일 때에는 드래그로 동작하지만 둘 이상인 경우에는 제스쳐로 동작한다.
    ///
    /// 마우스 입력의 경우에는 2개이상의 터치가 존재할 수 없지만, 마우스 휠 이벤트가 존재하므로
    /// 해당 이벤트로 터치 제스쳐를 대체한다.
    /// </summary>
    public abstract class DraggableKeyCodeTouchEventSenderBaseWithGesture : DraggableKeyCodeTouchEventSenderBase
    {
        #region <Fields>

#if UNITY_ANDROID && !UNITY_EDITOR
        /// <summary>
        /// 마우스와는 다르게 터치 이벤트는 화면에 터치를 인식할 구역이 필요하다.
        /// </summary>
        private UITool.RectTransformPlane _RectTransformPlane;
#endif

        #endregion

        #region <Callbacks>

#if !UNITY_ANDROID || UNITY_EDITOR
        public override void OnUpdateUI(float p_DeltaTime)
        {
            if (TouchEventManager.GetInstance.GetKeyDown(ThisInputEvent, ButtonKeyCode))
            {
                OnPointerKeep(p_DeltaTime);
            }
        }

        /// <summary>
        /// 마우스 휠 이벤트의 경우에는 드래그 이벤트가 처리해주지 않으므로
        /// 마우스 버튼을 누르고 있는 상태에서 휠을 조작한 경우 처리하도록 구현
        /// </summary>
        protected void OnKeyCodeEventPointerKeep(float p_DeltaTime)
        {
            var currentGesture = TouchEventManager.GetInstance.GetGestureType(ThisInputEvent);
            switch (currentGesture)
            {
                case ControllerTool.TouchGestureType.None:
                    ControllerTracker.SetControllerDirection(ArrowType.None, Vector3.zero);
                    TouchEventManager.GetInstance.ResetArrowInputState(ThisInputEvent);
                    break;
                case ControllerTool.TouchGestureType.Gather:
                case ControllerTool.TouchGestureType.Scatter:
                    break;
            }

            var mouseWheelDelta = Input.mouseScrollDelta.y;
            if (mouseWheelDelta.IsReachedZero())
            {
                TouchEventManager.GetInstance.SetGestureType(ThisInputEvent, ControllerTool.TouchGestureType.Stable);
            }
            else if (mouseWheelDelta > 0f)
            {
                TouchEventManager.GetInstance.SetGestureType(ThisInputEvent, ControllerTool.TouchGestureType.Gather);
            }
            else
            {
                TouchEventManager.GetInstance.SetGestureType(ThisInputEvent, ControllerTool.TouchGestureType.Scatter);
            }
        }
#endif

        protected override void OnKeyCodeEventPointerDown(PointerEventData p_EventData)
        {
            TouchEventManager.GetInstance.SetGestureType(ThisInputEvent, ControllerTool.TouchGestureType.None);

#if UNITY_ANDROID && !UNITY_EDITOR
            _RectTransformPlane = new UITool.RectTransformPlane();
            _RectTransformPlane.InitPlane(_RectTransform);
#endif
            base.OnKeyCodeEventPointerDown(p_EventData);
        }

        /// <summary>
        /// 해당 터치 컴포넌트로 드래그 이벤트가 발생하는 경우 유니티 엔진으로부터 호출되는 입력 콜백
        /// </summary>
        protected override void OnKeyCodeEventDrag(PointerEventData p_EventData)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            // 입력된 터치 중에 해당 버튼에 대한 터치가 몇개나 있는지 체크해야 한다.
            var currentTouchCount = Input.touchCount;
            var currentTouchThisCount = 0;
            Touch touchEin = default, touchZwei = default;
            for (int i = 0; i < currentTouchCount; i++)
            {
                // 현재 화면을 클릭하고 있는 터치 핸들러 중에서
                // 해당 이벤트 송신자의 _RectTransformPlane 영역 안에 들어간 터치를
                // 선착순으로 ein, zwei에 할당한다.
                var tryTouch = Input.GetTouch(i);
                if (_RectTransformPlane.IsOverRectTransform(tryTouch.position))
                {
                    currentTouchThisCount++;
                    switch (currentTouchThisCount)
                    {
                        case 1 :
                            touchEin = tryTouch;
                            break;
                        case 2 :
                            touchZwei = tryTouch;
                            break;
                    }
                }
            }
    
            // 현재 화면을 클릭하고 있는 터치 카운트에 대해서
            switch (currentTouchThisCount)
            {
                // 제스쳐의 경우에는 최소 터치 입력이 2종류 이상 존재해야하므로, 그 미만의 경우는
                // 상위 콜백을 그대로 사용한다.
                case 0 :
                case 1 :
                    base.OnKeyCodeEventDrag(p_EventData);
                    break;
                default :
                    var currentGesture = TouchEventManager.GetInstance.GetGestureType(ThisInputEvent);
                    switch (currentGesture)
                    {
                        case ControllerTool.TouchGestureType.None:
                            ControllerTracker.SetControllerDirection(ArrowType.None, Vector3.zero);
                            TouchEventManager.GetInstance.ResetArrowInputState(ThisInputEvent);
                            break;
                        case ControllerTool.TouchGestureType.Gather:
                        case ControllerTool.TouchGestureType.Scatter:
                        case ControllerTool.TouchGestureType.Stable:
                            break;
                    }
                    
                    var touchEinDeltaPos = touchEin.deltaPosition;
                    var touchZweiDeltaPos = touchZwei.deltaPosition;
                    var touchVectorEinToZwei = touchZwei.position - touchEin.position;
                    var einSimilarity = Vector3.Dot(touchEinDeltaPos, touchVectorEinToZwei);
                    var zweiSimilarity = Vector3.Dot(touchZweiDeltaPos, touchVectorEinToZwei);

                    if (einSimilarity > 0f && zweiSimilarity < 0f)
                    {
                        TouchEventManager.GetInstance.SetGestureType(ThisInputEvent, ControllerTool.TouchGestureType.Scatter);
                    }
                    else if (einSimilarity < 0f && zweiSimilarity > 0f)
                    {
                        TouchEventManager.GetInstance.SetGestureType(ThisInputEvent, ControllerTool.TouchGestureType.Gather);
                    }
                    else
                    {
                        TouchEventManager.GetInstance.SetGestureType(ThisInputEvent, ControllerTool.TouchGestureType.Stable);
                    }
                    break;
            }
#else
            // 안드로이드 이외의 플랫폼에서는 터치 이벤트가 없으므로 마우스 휠로 대체하고 있으므로,
            // 상위 콜백을 그대로 사용한다.
            base.OnKeyCodeEventDrag(p_EventData);
#endif
        }

        #endregion

        #region <Callback/EventTrigger>

#if !UNITY_ANDROID || UNITY_EDITOR
        /// <summary>
        /// 플랫폼이 에디터이거나, pc인 경우에 마우스 휠 이벤트를 통해 제스쳐 이벤트를 처리한다.
        /// </summary>
        protected void OnPointerKeep(float p_DeltaTime)
        {
            var enumerator = TouchEventRoot._TouchEventTypeEnumerator;
            foreach (var touchEventType in enumerator)
            {
                if (TouchEventFlagMask.HasAnyFlagExceptNone(touchEventType))
                {
                    switch (touchEventType)
                    {
                        case TouchEventRoot.TouchInputType.KeyCodeEvent:
                            OnKeyCodeEventPointerKeep(p_DeltaTime);
                            break;
                        case TouchEventRoot.TouchInputType.UnitClickEvent:
                            break;
                    }
                }
            }
        }
#endif

        #endregion
    }
}
#endif