namespace BlackAm
{
    /// <summary>
    /// 유니티 업데이트 콜백을 이용해 timeStamp를 측정하여 등록된 이벤트를 호출하는 기능을 가지는 클래스.
    /// </summary>
    public class UnityFrameTimeEventTimer : EventTimer
    {
        #region <Constructor>

        public UnityFrameTimeEventTimer()
        {
        }

        #endregion

        #region <Methods>
        
        /// <summary>
        /// timeStamp를 전진시키는 메서드
        /// </summary>
        public override void Tick(float p_DeltaTime)
        {
            switch (_CurrentTimerState)
            {
                case EventTimerState.Activate:
                    _elapsedMsec += (uint)(p_DeltaTime * 1000);
                    TickProgress(p_DeltaTime);
                    break;
                case EventTimerState.Blocked :
                    break;
            }
        }

        public override void Start()
        {
            _CurrentTimerState = EventTimerState.Activate;
        }

        public override void Stop()
        {
            _CurrentTimerState = EventTimerState.Blocked;
        }

        #endregion
    }
}