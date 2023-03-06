namespace k514
{
    /// <summary>
    /// ITimerEventTerminateReceiver 기본 구현체
    /// </summary>
    public abstract class EventTimerHandlerWrapper : IEventTimerHandlerWrapper
    {
        #region <Fields>

        /// <summary>
        /// 현재 통신 중인 EventTimer
        /// </summary>
        public EventTimer EventTimer { get; private set; }
        
        /// <summary>
        /// 이벤트 핸들러
        /// </summary>
        public IEventTimerHandler _BaseEventTimerHandler { get; private set; }

        #endregion

        #region <Constructor>

        public EventTimerHandlerWrapper()
        {
            SetEventTimer(SystemBoot.GameEventTimer);
        }
        
        public EventTimerHandlerWrapper(EventTimer p_EventTimer)
        {
            SetEventTimer(p_EventTimer);
        }

        #endregion

        #region <Callbacks>

        /// <summary>
        /// 해당 오브젝트의 이벤트가 종료되어 이벤트 타이머 핸들러가 파기되는 경우에 한 번 호출되는 메서드
        /// </summary>
        public virtual void WhenPropertyTurnToDefault()
        {
            _BaseEventTimerHandler = null;
        }

        /// <summary>
        /// 해당 오브젝트가 등록된 이벤트 타이머가 이벤트 타이머 큐에 삽입되는 경우 한 번 호출되는 메서드
        /// 그 때의 이벤트 타이머 핸들러 인스턴스 아이디를 수신받는다.
        /// </summary>
        public virtual void WhenPropertyTurnTo(IEventTimerHandler p_Value)
        {
            _BaseEventTimerHandler = p_Value;
        }

        public void WhenEventHandlerAdd(IEventTimerHandler p_BaseEventTimerHandler)
        {
            _BaseEventTimerHandler = p_BaseEventTimerHandler;
        }
        
        #endregion

        #region <Methods>

        /// <summary>
        /// 이벤트 타이머를 지정하는 메서드
        /// </summary>
        public void SetEventTimer(EventTimer p_EventTimer)
        {
            EventTimer = p_EventTimer;
        }

        /// <summary>
        /// 해당 오브젝트를 가지고 있는 이벤트 타이머 핸들러의 이벤트가 아직 유효한지 검증하는 논리메서드
        /// </summary>
        public bool IsEventValid()
        {
            return !ReferenceEquals(null, _BaseEventTimerHandler);
        }

        /// <summary>
        /// 이벤트를 실행시킨다.
        /// </summary>
        public void StartEvent()
        {
            if (IsEventValid())
            {
                _BaseEventTimerHandler.StartEvent();
            }
        }

        /// <summary>
        /// 이벤트를 일시정지시킨다.
        /// </summary>
        public void PauseEvent()
        {
            if (IsEventValid())
            {
                _BaseEventTimerHandler.PauseEvent();
            }
        }

        /// <summary>
        /// 이벤트를 종료시킨다.
        /// </summary>
        public void CancelEvent()
        {
            if (IsEventValid())
            {
                _BaseEventTimerHandler.CancelEvent();
            }
        }
        
        #endregion
    }
}