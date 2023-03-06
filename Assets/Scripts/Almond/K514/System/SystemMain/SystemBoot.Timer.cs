using System.Collections.Generic;

namespace k514
{
    public partial class SystemBoot
    {
        #region <Consts>
        
        /// <summary>
        /// 어플리케이션의 상태와 관련 없이, cpu 타임스탬프 시간을 기준으로 이벤트를 실행하는 타이머.
        /// </summary>
        public static EventTimer SystemEventTimer { get; private set; } = new RealTimeEventTimer();
        
        /// <summary>
        /// 유니티 엔진의 FixedUpdate 프레임 시간을 기준으로 이벤트를 실행하는 타이머.
        /// </summary>
        public static EventTimer FixedGameEventTimer { get; private set; } = new UnityFrameTimeEventTimer();
        
        /// <summary>
        /// 유니티 엔진의 Update 프레임 시간을 기준으로 이벤트를 실행하는 타이머.
        /// </summary>
        public static EventTimer GameEventTimer { get; private set; } = new UnityFrameTimeEventTimer();
        
        /// <summary>
        /// 유니티 엔진의 LateUpdate 프레임 시간을 기준으로 이벤트를 실행하는 타이머.
        /// </summary>
        public static EventTimer GameLateEventTimer { get; private set; } = new UnityFrameTimeEventTimer();

        #endregion
        
        #region <Fields>

        private Dictionary<TimerType, EventTimer> _WholeTimerTable;
        private Dictionary<TimerType, EventTimer> _GameTimerTable;

        #endregion

        #region <Enums>

        public enum TimerType
        {
            SystemTimer,
            FixedGameTimer,
            GameTimer,
            LateGameTimer
        }

        #endregion

        #region <Callbacks>

        private void OnCreateSystemBootTimer()
        {
            _WholeTimerTable = new Dictionary<TimerType, EventTimer>();
            _WholeTimerTable.Add(TimerType.SystemTimer, SystemEventTimer);
            _WholeTimerTable.Add(TimerType.FixedGameTimer, FixedGameEventTimer);
            _WholeTimerTable.Add(TimerType.GameTimer, GameEventTimer);
            _WholeTimerTable.Add(TimerType.LateGameTimer, GameLateEventTimer);
            
            _GameTimerTable = new Dictionary<TimerType, EventTimer>();
            _GameTimerTable.Add(TimerType.FixedGameTimer, FixedGameEventTimer);
            _GameTimerTable.Add(TimerType.GameTimer, GameEventTimer);
            _GameTimerTable.Add(TimerType.LateGameTimer, GameLateEventTimer);
            
            SystemEventTimer.Start();
        }

        #endregion

        #region <Operator>

        public EventTimer this[TimerType p_TimerType] => _WholeTimerTable[p_TimerType];

        #endregion
        
        #region <Methods>

        public EventTimer GetTimer(TimerType p_TimerType)
        {
            return _WholeTimerTable[p_TimerType];
        }

        private void SetGameTimerStart()
        {
            foreach (var gameTimerKV in _GameTimerTable)
            {
                gameTimerKV.Value.Start();
            }
        }

        private void SetGameTimerPause()
        {
            foreach (var gameTimerKV in _GameTimerTable)
            {
                gameTimerKV.Value.Reset();
            }
        }

        private void DisposeTimer()
        {
            foreach (var timerKV in _WholeTimerTable)
            {
                timerKV.Value.Stop();
            }
        }

        #endregion
    }
}