namespace BlackAm
{
    /// <summary>
    /// 주로 이벤트타이머 AddTimer의 파라미터로 활용되어 특정 타이머 이벤트가 종료되었는지에 대한
    /// 이벤트를 수신받는 풀링 오브젝트
    /// </summary>
    public abstract class EventTimerHandlerWrapper<M> : PoolingObject<M>, IEventTimerHandlerWrapper where M : EventTimerHandlerWrapper<M>
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

        #region <Callbacks>

        public override void OnRetrieved()
        {
            CancelEvent();
            SetEventTimer(null);
        }

        /// <summary>
        /// 해당 오브젝트의 이벤트가 종료되어 이벤트 타이머 핸들러가 파기되는 경우에 한 번 호출되는 메서드
        /// </summary>
        public virtual void WhenPropertyTurnToDefault()
        {
            RetrieveObject();
        }

        /// <summary>
        /// 해당 오브젝트가 등록된 이벤트 타이머가 이벤트 타이머 큐에 삽입되는 경우 한 번 호출되는 메서드
        /// 그 때의 이벤트 타이머 핸들러 인스턴스 아이디를 수신받는다.
        /// </summary>
        public void WhenPropertyTurnTo(IEventTimerHandler p_Value)
        {
            _BaseEventTimerHandler = p_Value;
        }

        public virtual void WhenEventHandlerAdd(IEventTimerHandler p_BaseEventTimerHandler)
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
        /// 이벤트 타이머를 지정하는 메서드
        /// </summary>
        public void SetEventTimer(SystemBoot.TimerType p_TimerType)
        {
            var tryTimer = SystemBoot.GetInstance[p_TimerType];
            SetEventTimer(tryTimer);
        }

        /// <summary>
        /// 해당 오브젝트를 가지고 있는 이벤트 타이머 핸들러의 이벤트가 아직 유효한지 검증하는 논리메서드
        /// </summary>
        public bool IsEventValid()
        {
            return !ReferenceEquals(null, _BaseEventTimerHandler) && _BaseEventTimerHandler.PoolState == PoolState.Actived;
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
                _BaseEventTimerHandler = null;
            }
        }
        
        #endregion
    }
}