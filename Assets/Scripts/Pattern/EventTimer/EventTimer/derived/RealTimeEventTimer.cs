using System.Diagnostics;

namespace BlackAm
{
    /// <summary>
    /// 시스템 라이브러리를 이용해 timeStamp를 측정하여 등록된 이벤트를 호출하는 기능을 가지는 클래스.
    /// </summary>
    public class RealTimeEventTimer : EventTimer
    {
        #region <Fields>

        /// <summary>
        /// 이벤트 핸들링을 위해 timeStamp를 관측하는 인스턴스
        /// </summary>
        private Stopwatch _stopWatch;

        #endregion

        #region <Constructor>

        public RealTimeEventTimer()
        {
            _stopWatch = new Stopwatch();
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
                    _elapsedMsec += (uint)_stopWatch.ElapsedMilliseconds;
                    _stopWatch.Reset();
                    _stopWatch.Start();
                    TickProgress(p_DeltaTime);
                    break;
                case EventTimerState.Blocked :
                    break;
            }
        }

        public override void Start()
        {
            _stopWatch.Start();
            _CurrentTimerState = EventTimerState.Activate;
        }

        public override void Stop()
        {
            _CurrentTimerState = EventTimerState.Blocked;
            _stopWatch.Reset();
        }

        #endregion
    }
}