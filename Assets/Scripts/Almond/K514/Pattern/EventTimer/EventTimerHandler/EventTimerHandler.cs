using System;
using UnityEngine;

namespace k514
{
    /// <summary>
    /// 이벤트 타이머로 관리되는 이벤트 타이머 핸들러의 인터페이스
    /// </summary>
    public interface IEventTimerHandler : IPoolingObject
    {
        #region <Fields>
        
        /// <summary>
        /// 해당 이벤트 타이머 핸들러 식별자
        /// </summary>
        uint TimerID { get; }

        /// <summary>
        /// 이벤트 발동 선딜레이
        /// </summary>
        uint StartMsec { get; }
        
        /// <summary>
        /// 이벤트 발동 주기
        /// </summary>
        uint EventIntervalMsec { get; }
  
        /// <summary>
        /// 이벤트가 반복된 횟수
        /// </summary>
        int EventCompleteCount { get; }
                
        /// <summary>
        /// 해당 이벤트가 발생했을 때 deltaTime
        /// </summary>
        float LatestDeltaTime { get; }
        
        /// <summary>
        /// 해당 이벤트 타이머 핸들러가 활성화된 시점의 타임스탬프
        /// </summary>
        uint TargetStartStamp { get; set; }
        
        /// <summary>
        /// 해당 이벤트 타이머 핸들러의 다음 이벤트 발동 타임스탬프
        /// </summary>
        uint TargetEventStamp { get; set; }
        
        /// <summary>
        /// 해당 이벤트가 매 프레임 호출되어야 하는지 표시하는 플래그
        /// </summary>
        bool IsReenterEveryTick { get; }
        
        /// <summary>
        /// 해당 이벤트 타이머가 일시정지 상태인지 표시하는 플래그
        /// </summary>
        bool IsPaused { get; }
        
        /// <summary>
        /// 해당 이벤트 타이머가 이벤트 큐에 들어가있는지 표시하는 플래그
        /// </summary>
        bool IsQueued { get; set; }

        /// <summary>
        /// InvokeEvent 함수가 실행중에 캔슬이벤트가 발생한 경우를 표시하는 플래그
        /// </summary>
        bool ReserveDeadFlag { get; }

        /// <summary>
        /// 해당 이벤트가 최초로 호출되는 경우를 표시하는 플래그
        /// </summary>
        bool IsFirstInvoke { get; }

        /// <summary>
        /// 이벤트가 실행된 이후, 큐로 이벤트 타이머가 재삽입되야 할 때 호출되는 콜백
        /// </summary>
        void OnHandleReenterQueue(PriorityCollection<uint, IEventTimerHandler, ulong> p_Queue);

        #endregion
        
        #region <Callbacks>

        /// <summary>
        /// OnPooling 이후에 호출되어 해당 이벤트 타이머 핸들러의 플래그 등을 초기화시키는 메서드
        /// </summary>
        void OnInitiateEventTimerHandler(EventTimer p_EventTimer, uint p_EventTimerId, IEventTimerHandlerWrapper p_EventTimerHandlerWrapper, EventTimerTool.EventTimerParams p_Option);
        
        /// <summary>
        /// 이벤트 타이머 핸들러가 이벤트 타이머 큐에 등록될 때 호출되는 콜백
        /// </summary>
        void OnActivateEventTimerHandler();

        /// <summary>
        /// 이벤트 타이머 핸들러를 정지시킬 때 호출되는 콜백
        /// </summary>
        void OnPauseEventTimerHandler();
        
        /// <summary>
        /// 이벤트 타이머 핸들러를 정지상태에서 이벤트 타이머에 재등록됬을 때 호출되는 콜백
        /// </summary>
        void OnResumeEventTimerHandler();
        
        /// <summary>
        /// 이벤트 타이머가 초기화되어 포함되었던 이벤트들이 초기화되는 경우 호출되는 콜백
        /// </summary>
        void OnTerminateEventTimerHandler();
        
        #endregion

        #region <Methods>

        /// <summary>
        /// 이벤트 발동 메서드
        /// </summary>
        bool InvokeEvent(float p_DeltaTime, uint p_CurrentElapsedMsec);

        /// <summary>
        /// 현재 루프중인 이벤트를 종료해야 하는지 검증하는 논리메서드
        /// 참일 때, 이벤트는 실행되지 않고 이벤트 타이머로부터 파기된다.
        ///
        /// 논리메서드가 할당되어있지 않은 경우, 항상 false를 리턴하게 된다.
        /// </summary>
        bool CheckIntervalBreak();

        /// <summary>
        /// 현재 구간에서 진행된 시간을 msec단위로 리턴하는 메서드
        /// </summary>
        uint GetElapsedMsecInInterval();
        
        /// <summary>
        /// 현재 구간에서 남은 시간을 msec단위로 리턴하는 메서드
        /// </summary>
        uint GetRemaindMsecInInterval();

        /// <summary>
        /// 해당 이벤트 핸들러와 동일한 내용을 가지는 이벤트를 지정한 타이머에 생성하여 리턴하는 메서드
        /// </summary>
        IEventTimerHandler GetClone(EventTimer p_TargetTimer);

        /// <summary>
        /// 해당 이벤트를 실행시키는 메서드
        /// </summary>
        void StartEvent();
        
        /// <summary>
        /// 해당 이벤트를 일시정지시키는 메서드
        /// </summary>
        void PauseEvent();
        
        /// <summary>
        /// 해당 이벤트를 취소하고 파기시키는 메서드
        /// </summary>
        void CancelEvent();

        #endregion
    }

    public abstract class EventTimerHandlerBase<T> : PoolingObject<T>, IEventTimerHandler where T : EventTimerHandlerBase<T>, new()
    {
        #region <Consts>

        /// <summary>
        /// 이벤트 타이머 핸들러
        /// </summary>
        public static ObjectPooler<T> TimerHandlerPool;

        static EventTimerHandlerBase()
        {
            TimerHandlerPool = new ObjectPooler<T>();
            TimerHandlerPool.PreloadPool(8, 8);
        }
        
        #endregion

        #region <Fields>
        
        /// <summary>
        /// 이벤트 타이머 핸들러의 수명 관련 이벤트를 수신받을 객체
        /// </summary>
        public IEventTimerHandlerWrapper EventTimerHandlerWrapper { get; private set; }
        
        /// <summary>
        /// 해당 이벤트 타이머 핸들러 식별자
        /// </summary>
        public uint TimerID { get; set; }

        /// <summary>
        /// 해당 이벤트 타이머 핸들러를 관리하는 마스터 노드
        /// </summary>
        private EventTimer EventTimer;
        
        /// <summary>
        /// 이벤트 발동 선딜레이
        /// </summary>
        public uint StartMsec { get; private set; }
        
        /// <summary>
        /// 이벤트 발동 주기
        /// </summary>
        public uint EventIntervalMsec { get; private set; }
 
        /// <summary>
        /// 이벤트가 반복된 횟수
        /// </summary>
        public int EventCompleteCount { get; private set; }
        
        /// <summary>
        /// 해당 이벤트가 발생했을 때 deltaTime
        /// </summary>
        public float LatestDeltaTime { get; private set; }
        
        /// <summary>
        /// 해당 이벤트 타이머 핸들러가 활성화된(Regist or Repeat) 시점의 타임스탬프
        /// </summary>
        public uint TargetStartStamp { get; set; }
        
        /// <summary>
        /// 해당 이벤트 타이머 핸들러의 다음 이벤트 발동 타임스탬프
        /// </summary>
        public uint TargetEventStamp { get; set; }

        /// <summary>
        /// 해당 이벤트 타이머 핸들러가 일시정지된 시점의 타임스탬프
        /// </summary>
        private uint PausedStamp;

        /// <summary>
        /// 발동 타이밍에 호출할 이벤트의 대리자
        /// </summary>
        public Func<T, bool> Event;

        /// <summary>
        /// 주기 모드인 경우에 이벤트 루프를 해제할 키를 검증하는 논리메서드 대리자
        /// </summary>
        public Func<T, bool> IntevalBreakPredict;
        
        /// <summary>
        /// 러프 모드인 경우 사용할 러프 타이머
        /// </summary>
        public ProgressTimer LerpTimer;

        /// <summary>
        /// 러프모드
        /// </summary>
        public EventTimerTool.EventTimerIntervalType IntervalType { get; private set; }

        /// <summary>
        /// 해당 이벤트가 매 프레임 호출되어야 하는지 표시하는 플래그
        /// </summary>
        public bool IsReenterEveryTick { get; private set; }

        /// <summary>
        /// 해당 이벤트 타이머가 일시정지 상태인지 표시하는 플래그
        /// </summary>
        public bool IsPaused { get; private set; }
        
        /// <summary>
        /// 해당 이벤트 타이머가 이벤트 큐에 들어가있는지 표시하는 플래그
        /// </summary>
        public bool IsQueued { get; set; }
        
        /// <summary>
        /// InvokeEvent 함수가 실행중에 캔슬이벤트가 발생한 경우를 표시하는 플래그
        /// </summary>
        public bool ReserveDeadFlag { get; private set; }

        /// <summary>
        /// 해당 이벤트가 최초로 호출되는 경우를 표시하는 플래그
        /// </summary>
        public bool IsFirstInvoke { get; private set; }

        /// <summary>
        /// 이벤트 타이머 핸들러의 수명관련 이벤트 수신 오브젝트를 가지는지 표시하는 플래그
        /// </summary>
        private bool HasTerminateEventReceiver;

        /// <summary>
        /// InvokeEvent 함수가 실행중인지 표시하는 플래그
        /// </summary>
        private bool InvokeEventSwitch;
        
        #endregion

        #region <Callbacks>

        public override void OnSpawning()
        {
        }

        public override void OnPooling()
        {
        }

        public override void OnRetrieved()
        {
            if (HasTerminateEventReceiver)
            {
                if (EventTimerHandlerWrapper._BaseEventTimerHandler == this)
                {
                    EventTimerHandlerWrapper.WhenPropertyTurnToDefault();
                }
                EventTimerHandlerWrapper = null;
            }

            TimerID = 0;
            EventTimer = null;
            StartMsec = 0;
            EventIntervalMsec = 0;
            EventCompleteCount = 0;
            LatestDeltaTime = 0f;
            TargetEventStamp = 0;
            IntervalType = default;
            IsReenterEveryTick = false;
            IsPaused = false;
            IsQueued = false;
            IsFirstInvoke = false;
            HasTerminateEventReceiver = false;
            InvokeEventSwitch = false;
            ReserveDeadFlag = false;
        }

        public void OnInitiateEventTimerHandler(EventTimer p_EventTimer, uint p_EventTimerId, IEventTimerHandlerWrapper p_EventTimerHandlerWrapper, EventTimerTool.EventTimerParams p_Option)
        {
            EventTimer = p_EventTimer;
            TimerID = p_EventTimerId;
            EventTimerHandlerWrapper = p_EventTimerHandlerWrapper;
            StartMsec = p_Option.StartStamp;
            EventIntervalMsec = p_Option.Interval;
            IntervalType = p_Option.IntervalType;

            switch (IntervalType)
            {
                case EventTimerTool.EventTimerIntervalType.None:
                    break;
                case EventTimerTool.EventTimerIntervalType.UpdateEveryFrame:
                    IsReenterEveryTick = true;
                    EventIntervalMsec = 1;
                    break;
                case EventTimerTool.EventTimerIntervalType.Lerp:
                case EventTimerTool.EventTimerIntervalType.FlyingLerp:
                    IsReenterEveryTick = true;
                    LerpTimer = ProgressTimer.GetProgressTimer(EventIntervalMsec * 0.001f);
                    break;
            }

            HasTerminateEventReceiver = !ReferenceEquals(null, EventTimerHandlerWrapper);
        }

        public void OnActivateEventTimerHandler()
        {
            if (!ReserveDeadFlag)
            {
                IsQueued = true;
                IsFirstInvoke = true;
                if (HasTerminateEventReceiver)
                {
                    if (EventTimerHandlerWrapper._BaseEventTimerHandler != this)
                    {
                        EventTimerHandlerWrapper.CancelEvent();
#if UNITY_EDITOR
                        if (CustomDebug.PrintEventTimerHandlerProgressFlag)
                        {
                            Debug.Log($"Timer Id : {TimerID} / Actived / Stamp : {TargetEventStamp} / {EventTimer.GetTimerElapsedMsec()}");
                        }
#endif
                        EventTimerHandlerWrapper.WhenPropertyTurnTo(this);
                    }
                    else
                    {
#if UNITY_EDITOR
                        if (CustomDebug.PrintEventTimerHandlerProgressFlag)
                        {
                            Debug.Log($"Timer Id : {TimerID} / Actived / Stamp : {TargetEventStamp} / {EventTimer.GetTimerElapsedMsec()}");
                        }
#endif
                        EventTimerHandlerWrapper.WhenPropertyTurnTo(this);
                    }
                }
            }
        }

        public void OnPauseEventTimerHandler()
        {
            if (!ReserveDeadFlag)
            {
                IsQueued = false;
                IsPaused = true;
                
                PausedStamp = EventTimer.GetTimerElapsedMsec();
            }
        }
        
        public void OnResumeEventTimerHandler()
        {
            if (!ReserveDeadFlag)
            {
                IsQueued = true;
                IsPaused = false;
            }
        }

        public void OnTerminateEventTimerHandler()
        {
            if (PoolState == PoolState.Actived)
            {
#if UNITY_EDITOR
                if (CustomDebug.PrintEventTimerHandlerProgressFlag)
                {
                    Debug.Log($"Timer Id : {TimerID} / Terminated");
                }
#endif
                RetrieveObject();
            }
        }

        public void OnHandleReenterQueue(PriorityCollection<uint, IEventTimerHandler, ulong> p_Queue)
        {
            if (ReserveDeadFlag)
            {
                RetrieveObject();
            }
            else
            {
                p_Queue.Enqueue(TimerID, this, TargetEventStamp);
                IsQueued = true;
            }
        }

        #endregion
        
        #region <Methods>

        public EventTimerTool.EventTimerParams GetParams()
        {
            return new EventTimerTool.EventTimerParams(StartMsec, EventIntervalMsec, IntervalType);
        }

        public abstract IEventTimerHandler GetClone(EventTimer p_TargetTimer);
        
        public void StartEvent()
        {
            if (PoolState == PoolState.Actived && !IsQueued)
            {
                EventTimer.RegistTimer(this);
            }
        }

        public void PauseEvent()
        {
            if (PoolState == PoolState.Actived && IsQueued && !IsPaused)
            {
                EventTimer.PauseTimer(TimerID);
            }
        }

        public void CancelEvent()
        {
            if (PoolState == PoolState.Actived && !ReserveDeadFlag)
            {
#if UNITY_EDITOR
                if (CustomDebug.PrintEventTimerHandlerProgressFlag)
                {
                    Debug.Log($"Timer Id : {TimerID} / Canceled");
                }
#endif
                if (InvokeEventSwitch)
                {
                    ReserveDeadFlag = true;
                }
                else
                {
                    if (IsQueued)
                    {
                        EventTimer.DelTimer(TimerID);
                    }
                    else
                    {
                        RetrieveObject();
                    }
                }
            }
        }
        
        public bool InvokeEvent(float p_DeltaTime, uint p_CurrentElapsedMsec)
        {
            var result = false;
            InvokeEventSwitch = true;
            LatestDeltaTime = p_DeltaTime;
            
#if UNITY_EDITOR
            if (CustomDebug.PrintEventTimerHandlerProgressFlag)
            {
                Debug.Log($"Timer Id : {TimerID} / Invoked / Stamp : {TargetEventStamp} / {p_CurrentElapsedMsec}");
            }
#endif
            switch (IntervalType)
            {
                case EventTimerTool.EventTimerIntervalType.Lerp:
                {
                    if (LerpTimer.IsOver())
                    {
                        EventCompleteCount++;
                        result = Event(this as T);
            
                        if (IsFirstInvoke)
                        {
                            IsFirstInvoke = false;
                        }
                    
                        // 다음 러프를 위해 관련 멤버를 초기화 시킨다.
                        LerpTimer.Reset();
                        StartMsec = p_CurrentElapsedMsec;
                    }
                    else
                    {
                        result = Event(this as T);
                        LerpTimer.Progress(LatestDeltaTime);
            
                        if (IsFirstInvoke)
                        {
                            IsFirstInvoke = false;
                        }
                    }
                    break;
                }
                case EventTimerTool.EventTimerIntervalType.FlyingLerp:
                {
                    if (LerpTimer.IsOver())
                    {
                        EventCompleteCount++;
                        result = Event(this as T);
            
                        if (IsFirstInvoke)
                        {
                            IsFirstInvoke = false;
                        }
                    
                        // 다음 러프를 위해 관련 멤버를 초기화 시킨다.
                        LerpTimer.Reset();
                        StartMsec = p_CurrentElapsedMsec;
                    }
                    else
                    {
                        LerpTimer.Progress(LatestDeltaTime);
                        result = Event(this as T);
            
                        if (IsFirstInvoke)
                        {
                            IsFirstInvoke = false;
                        }
                    }
                    break;
                }
                default:
                {
                    EventCompleteCount++;
                    result = Event(this as T);
      
                    if (IsFirstInvoke)
                    {
                        IsFirstInvoke = false;
                    }
                }
                    break;
            }

            if (ReserveDeadFlag)
            {
                InvokeEventSwitch = false;
                CancelEvent();
                return result;
            }
            else
            {
                InvokeEventSwitch = false;
                return result;
            }
        }

        public bool CheckIntervalBreak()
        {
            return IntevalBreakPredict != null && IntevalBreakPredict(this as T);
        }

        public uint GetElapsedMsecInInterval()
        {
            switch (IntervalType)
            {
                default:
                case EventTimerTool.EventTimerIntervalType.None:
                case EventTimerTool.EventTimerIntervalType.UpdateEveryFrame:
                    return IsPaused ? PausedStamp - TargetStartStamp : EventTimer.GetTimerElapsedMsec() - TargetStartStamp;
                case EventTimerTool.EventTimerIntervalType.Lerp:
                case EventTimerTool.EventTimerIntervalType.FlyingLerp: 
                    return (uint)(1000 * LerpTimer.ElapsedTime);
            }
        }
        
        public uint GetRemaindMsecInInterval()
        {
            switch (IntervalType)
            {
                default:
                case EventTimerTool.EventTimerIntervalType.None:
                case EventTimerTool.EventTimerIntervalType.UpdateEveryFrame:
                    return IsPaused ? TargetEventStamp - PausedStamp : TargetEventStamp - EventTimer.GetTimerElapsedMsec();
                case EventTimerTool.EventTimerIntervalType.Lerp:
                case EventTimerTool.EventTimerIntervalType.FlyingLerp:
                    return (uint)(1000 * LerpTimer.RemaindTime);
            }
        }
        
        #endregion
    }

    /// <summary>
    /// Action 이벤트 핸들러
    /// </summary>
    public class EventTimerHandler : EventTimerHandlerBase<EventTimerHandler>
    {
        public override IEventTimerHandler GetClone(EventTimer p_TargetTimer)
        {
            return p_TargetTimer.GetTimer(EventTimerHandlerWrapper, GetParams(), Event, IntevalBreakPredict);
        }
    }

    /// <summary>
    /// Action[M] 이벤트 핸들러
    /// </summary>
    public class EventTimerHandler<M> : EventTimerHandlerBase<EventTimerHandler<M>>
    {
        public M Arg1;

        public override void OnRetrieved()
        {
            base.OnRetrieved();
            Arg1 = default;
        }
        
        public override IEventTimerHandler GetClone(EventTimer p_TargetTimer)
        {
            return p_TargetTimer.GetTimer(EventTimerHandlerWrapper, GetParams(), Event, IntevalBreakPredict, Arg1);
        }
    }

    /// <summary>
    /// Action[M,K] 이벤트 핸들러
    /// </summary>
    public class EventTimerHandler<M, K> : EventTimerHandlerBase<EventTimerHandler<M, K>>
    {
        public M Arg1;
        public K Arg2;

        public override void OnRetrieved()
        {
            base.OnRetrieved();
            Arg1 = default;
            Arg2 = default;
        }
                
        public override IEventTimerHandler GetClone(EventTimer p_TargetTimer)
        {
            return p_TargetTimer.GetTimer(EventTimerHandlerWrapper, GetParams(), Event, IntevalBreakPredict, Arg1, Arg2);
        }
    }

    /// <summary>
    /// Action[M,K,T] 이벤트 핸들러
    /// </summary>
    public class EventTimerHandler<M, K, T> : EventTimerHandlerBase<EventTimerHandler<M, K, T>>
    {
        public M Arg1;
        public K Arg2;
        public T Arg3;

        public override void OnRetrieved()
        {
            base.OnRetrieved();
            Arg1 = default;
            Arg2 = default;
            Arg3 = default;
        }
        
        public override IEventTimerHandler GetClone(EventTimer p_TargetTimer)
        {
            return p_TargetTimer.GetTimer(EventTimerHandlerWrapper, GetParams(), Event, IntevalBreakPredict, Arg1, Arg2, Arg3);
        }
    }
    
    /// <summary>
    /// Action[M,K,T,M1] 이벤트 핸들러
    /// </summary>
    public class EventTimerHandler<M, K, T, M1> : EventTimerHandlerBase<EventTimerHandler<M, K, T, M1>>
    {
        public M Arg1;
        public K Arg2;
        public T Arg3;
        public M1 Arg4;

        public override void OnRetrieved()
        {
            base.OnRetrieved();
            Arg1 = default;
            Arg2 = default;
            Arg3 = default;
            Arg4 = default;
        }
        
        public override IEventTimerHandler GetClone(EventTimer p_TargetTimer)
        {
            return p_TargetTimer.GetTimer(EventTimerHandlerWrapper, GetParams(), Event, IntevalBreakPredict, Arg1, Arg2, Arg3, Arg4);
        }
    }
        
    /// <summary>
    /// Action[M,K,T,M1,M2] 이벤트 핸들러
    /// </summary>
    public class EventTimerHandler<M, K, T, M1, M2> : EventTimerHandlerBase<EventTimerHandler<M, K, T, M1, M2>>
    {
        public M Arg1;
        public K Arg2;
        public T Arg3;
        public M1 Arg4;
        public M2 Arg5;

        public override void OnRetrieved()
        {
            base.OnRetrieved();
            Arg1 = default;
            Arg2 = default;
            Arg3 = default;
            Arg4 = default;
            Arg5 = default;
        }
        
        public override IEventTimerHandler GetClone(EventTimer p_TargetTimer)
        {
            return p_TargetTimer.GetTimer(EventTimerHandlerWrapper, GetParams(), Event, IntevalBreakPredict, Arg1, Arg2, Arg3, Arg4, Arg5);
        }
    }
        
    /// <summary>
    /// Action[M,K,T,M1,M2,M3] 이벤트 핸들러
    /// </summary>
    public class EventTimerHandler<M, K, T, M1, M2, M3> : EventTimerHandlerBase<EventTimerHandler<M, K, T, M1, M2, M3>>
    {
        public M Arg1;
        public K Arg2;
        public T Arg3;
        public M1 Arg4;
        public M2 Arg5;
        public M3 Arg6;

        public override void OnRetrieved()
        {
            base.OnRetrieved();
            Arg1 = default;
            Arg2 = default;
            Arg3 = default;
            Arg4 = default;
            Arg5 = default;
            Arg6 = default;
        }
        
        public override IEventTimerHandler GetClone(EventTimer p_TargetTimer)
        {
            return p_TargetTimer.GetTimer(EventTimerHandlerWrapper, GetParams(), Event, IntevalBreakPredict, Arg1, Arg2, Arg3, Arg4, Arg5, Arg6);
        }
    }
}