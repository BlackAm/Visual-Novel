namespace BlackAm
{
    /// <summary>
    /// 주로 이벤트타이머 AddTimer의 파라미터로 활용되어 특정 타이머 이벤트가 종료되었는지에 대한
    /// 이벤트를 수신받는 오브젝트를 기술하는 인터페이스
    /// </summary>
    public interface IEventTimerHandlerWrapper : ISinglePropertyModifyNotify<IEventTimerHandler>
    {
        /// <summary>
        /// 현재 통신 중인 EventTimer
        /// </summary>
        EventTimer EventTimer { get; }
        
        /// <summary>
        /// 이벤트 핸들러
        /// </summary>
        IEventTimerHandler _BaseEventTimerHandler { get; }
        
        /// <summary>
        /// 이벤트 핸들러가 할당되야하는 경우 호출되는 콜백
        /// </summary>
        void WhenEventHandlerAdd(IEventTimerHandler p_BaseEventTimerHandler);
        
        /// <summary>
        /// 이벤트 타이머를 지정하는 메서드
        /// </summary>
        void SetEventTimer(EventTimer p_EventTimer);
        
        /// <summary>
        /// 해당 오브젝트를 가지고 있는 이벤트 타이머 핸들러의 이벤트가 아직 유효한지 검증하는 논리메서드
        /// </summary>
        bool IsEventValid();

        /// <summary>
        /// 이벤트를 실행시킨다.
        /// </summary>
        void StartEvent();
        
        /// <summary>
        /// 이벤트를 정지시킨다.
        /// </summary>
        void PauseEvent();
        
        /// <summary>
        /// 이벤트를 종료시킨다.
        /// </summary>
        void CancelEvent();
    }
}