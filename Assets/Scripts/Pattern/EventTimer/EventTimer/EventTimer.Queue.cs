namespace BlackAm
{
    /// <summary>
    /// timeStamp를 측정하여 등록된 이벤트를 호출하는 기능을 가지는 클래스.
    /// </summary>
    public abstract partial class EventTimer
    {
        #region <Fields>
        
        /// <summary>
        /// 다음에 등록할 타이머 핸들러의 아이디
        /// 0 = 초기상태, NONE
        /// </summary>
        protected uint _nextTimerID;
        
        /// <summary>
        /// 현재까지 경과된 밀리세컨드 정수 값.
        /// </summary>
        protected uint _elapsedMsec;
        
        /// <summary>
        /// [타이머 핸들러 id, 타이머 핸들러, 타이머 핸들러 동작 timeStamp] 우선순위 컬렉션
        /// </summary>
        protected PriorityCollection<uint, IEventTimerHandler, ulong> _queue;

        /// <summary>
        /// 컬렉션으로 접근하는 스레드 수를 제한하기 위한 록 인스턴스
        /// </summary>
        protected readonly object QUEUE_LOCK = new object();
 
        /// <summary>
        /// timeStamp 측정을 일시정지 시키는 플래그
        /// </summary>
        protected EventTimerState _CurrentTimerState = EventTimerState.Blocked;

        #endregion

        #region <Enums>

        public enum EventTimerState
        {
            Activate,
            Blocked,
        }

        #endregion

        #region <Constructor>

        public EventTimer()
        {
            _queue = new PriorityCollection<uint, IEventTimerHandler, ulong>();
        }

        #endregion

        #region <Methods>

        /// <summary>
        /// 해당 타이머가 동작하고 지난 시간을 리턴하는 메서드
        /// </summary>
        public uint GetTimerElapsedMsec() => _elapsedMsec;
        
        /// <summary>
        /// 타이머의 시간을 경과시키고, 경과된 시간에서 예약된 이벤트를 처리하는 메서드
        /// </summary>
        public abstract void Tick(float p_DeltaTime);
      
        /// <summary>
        /// 타이머를 유효한 상태로 바꾸는 메서드
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// 타이머를 정지 상태로 바꾸는 메서드
        /// </summary>
        public abstract void Stop();
        
        /// <summary>
        /// 해당 이벤트 타이머를 리셋시키는 메서드
        /// </summary>
        public void Reset()
        {
            _elapsedMsec = 0;
            _nextTimerID = 0;
            lock (QUEUE_LOCK)
            {
                while (!_queue.IsEmpty())
                {
                    var tryHandler = _queue.Dequeue();
                    tryHandler.OnTerminateEventTimerHandler();
                }
            }
            Stop();
        }
        #endregion
    }
}