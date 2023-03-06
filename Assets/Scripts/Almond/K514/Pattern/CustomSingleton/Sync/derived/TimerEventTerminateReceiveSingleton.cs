namespace k514
{
    /// <summary>
    /// 이벤트 타이머를 가지는 싱글톤
    /// </summary>
    public abstract class TimerEventTerminateReceiveSingleton<T> : Singleton<T>, IEventTimerHandlerWrapper where T : TimerEventTerminateReceiveSingleton<T>, new()
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

        /// <summary>
        /// 해당 오브젝트의 이벤트가 종료되어 이벤트 타이머 핸들러가 파기되는 경우에 한 번 호출되는 메서드
        /// </summary>
        public virtual void WhenPropertyTurnToDefault()
        {
            _BaseEventTimerHandler = null;
        }

        /// <summary>
        /// 해당 오브젝트가 등록된 이벤트가 이벤트 타이머 큐에 삽입되는 경우 한 번 호출되는 메서드
        /// 그 때의 이벤트 타이머 핸들러 인스턴스 아이디를 수신받는다.
        /// </summary>
        public virtual void WhenPropertyTurnTo(IEventTimerHandler p_Value)
        {
            _BaseEventTimerHandler = p_Value;
        }

        /// <summary>
        /// 해당 오브젝트가 등록된 이벤트에 AddTimer 함수에 의해 이벤트가 기술되는 경우 호출되는 콜백
        /// </summary>
        public void WhenEventHandlerAdd(IEventTimerHandler p_BaseEventTimerHandler)
        {
            _BaseEventTimerHandler = p_BaseEventTimerHandler;
        }
        
        /// <summary>
        /// 인스턴스가 파기될 때 수행할 작업을 기술한다.
        /// </summary>
        protected override void DisposeUnManaged()
        {
            CancelEvent();

            base.DisposeUnManaged();
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
                _BaseEventTimerHandler = null;
            }
        }
        
        #endregion
    }
    
    public abstract class GameTimerEventTerminateReceiveSingleton<T> : TimerEventTerminateReceiveSingleton<T> where T : GameTimerEventTerminateReceiveSingleton<T>, new()
    {
        #region <Callbacks>

        protected override void OnCreated()
        {
            SetEventTimer(SystemBoot.GameEventTimer);
        }

        #endregion
    }
}