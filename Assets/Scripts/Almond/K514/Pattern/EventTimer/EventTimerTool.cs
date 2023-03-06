using System;
using System.Collections.Generic;

namespace k514
{
    public static class EventTimerTool
    {
        #region <Enums>
        
        public enum EventTimerIntervalType
        {
            /// <summary>
            /// 러프를 수행하지 않는다.
            /// </summary>
            None,
            
            /// <summary>
            /// 러프 연산은 수행하지 않고, 해당 이벤트를 매 프레임 동작하도록 한다.
            /// </summary>
            UpdateEveryFrame,
            
            /// <summary>
            /// 0부터 러프를 수행한다.
            /// </summary>
            Lerp,
            
            /// <summary>
            /// 최초 러프 이벤트 처리시 진행을 먼저하고 나서 이벤트를 처리한다.
            /// </summary>
            FlyingLerp
        }

        #endregion
        
        #region <Method/SpawnTimer>

        /// <summary>
        /// 파라미터를 0개 가지는 이벤트를 등록하는 메서드
        /// </summary>
        /// <param name="p_StartStamp">이벤트가 실행되기까지의 딜레이 msec</param>
        /// <param name="p_Interval">1 이상의 값을 넣으면 해당 이벤트는 다음 이벤트로 전이하지 않고 해당 값의 주기로 무한반복하게 된다.</param>
        /// <param name="p_IntervalBreakPredict">무한 반복이벤트의 종료조건</param>
        /// <param name="p_HandleEvent">실행할 이벤트</param>
        public static void AddEvent(this IEventTimerHandlerWrapper p_Target, EventTimerParams p_Params, Func<EventTimerHandler, bool> p_HandleEvent, Func<EventTimerHandler, bool> p_IntervalBreakPredict = null)
        {
            var eventHandler = p_Target.EventTimer.GetTimer(p_Target, p_Params, p_HandleEvent, p_IntervalBreakPredict);
            p_Target.WhenEventHandlerAdd(eventHandler);
        }
        
        /// <summary>
        /// 파라미터를 1개 가지는 이벤트를 등록하는 메서드
        /// </summary>
        /// <param name="p_StartStamp">이벤트가 실행되기까지의 딜레이 msec</param>
        /// <param name="p_Interval">1 이상의 값을 넣으면 해당 이벤트는 다음 이벤트로 전이하지 않고 해당 값의 주기로 무한반복하게 된다.</param>
        /// <param name="p_TerminatePredict">무한 반복이벤트의 종료조건</param>
        /// <param name="p_HandleEvent">실행할 이벤트</param>
        public static void AddEvent<M>(this IEventTimerHandlerWrapper p_Target, EventTimerParams p_Params, Func<EventTimerHandler<M>, bool> p_HandleEvent, Func<EventTimerHandler<M>, bool> p_IntervalBreakPredict, M p_Arg1)
        {
            var eventHandler = p_Target.EventTimer.GetTimer(p_Target, p_Params, p_HandleEvent, p_IntervalBreakPredict, p_Arg1);
            p_Target.WhenEventHandlerAdd(eventHandler);
        }
                
        /// <summary>
        /// 파라미터를 2개 가지는 이벤트를 등록하는 메서드
        /// </summary>
        /// <param name="p_StartStamp">이벤트가 실행되기까지의 딜레이 msec</param>
        /// <param name="p_Interval">1 이상의 값을 넣으면 해당 이벤트는 다음 이벤트로 전이하지 않고 해당 값의 주기로 무한반복하게 된다.</param>
        /// <param name="p_TerminatePredict">무한 반복이벤트의 종료조건</param>
        /// <param name="p_Event">실행할 이벤트</param>
        public static void AddEvent<M, K>(this IEventTimerHandlerWrapper p_Target, EventTimerParams p_Params, Func<EventTimerHandler<M, K>, bool> p_HandleEvent, Func<EventTimerHandler<M, K>, bool> p_IntervalBreakPredict, M p_Arg1, K p_Arg2)
        {
            var eventHandler = p_Target.EventTimer.GetTimer(p_Target, p_Params, p_HandleEvent, p_IntervalBreakPredict, p_Arg1, p_Arg2);
            p_Target.WhenEventHandlerAdd(eventHandler);
        }
                        
        /// <summary>
        /// 파라미터를 3개 가지는 이벤트를 등록하는 메서드
        /// </summary>
        /// <param name="p_StartStamp">이벤트가 실행되기까지의 딜레이 msec</param>
        /// <param name="p_Interval">1 이상의 값을 넣으면 해당 이벤트는 다음 이벤트로 전이하지 않고 해당 값의 주기로 무한반복하게 된다.</param>
        /// <param name="p_TerminatePredict">무한 반복이벤트의 종료조건</param>
        /// <param name="p_Event">실행할 이벤트</param>
        public static void AddEvent<M, K, T>(this IEventTimerHandlerWrapper p_Target, EventTimerParams p_Params, Func<EventTimerHandler<M, K, T>, bool> p_HandleEvent, Func<EventTimerHandler<M, K, T>, bool> p_IntervalBreakPredict, M p_Arg1, K p_Arg2, T p_Args3)
        {
            var eventHandler = p_Target.EventTimer.GetTimer(p_Target, p_Params, p_HandleEvent, p_IntervalBreakPredict, p_Arg1, p_Arg2, p_Args3);
            p_Target.WhenEventHandlerAdd(eventHandler);
        }
        
        /// <summary>
        /// 파라미터를 4개 가지는 이벤트를 등록하는 메서드
        /// </summary>
        /// <param name="p_StartStamp">이벤트가 실행되기까지의 딜레이 msec</param>
        /// <param name="p_Interval">1 이상의 값을 넣으면 해당 이벤트는 다음 이벤트로 전이하지 않고 해당 값의 주기로 무한반복하게 된다.</param>
        /// <param name="p_TerminatePredict">무한 반복이벤트의 종료조건</param>
        /// <param name="p_Event">실행할 이벤트</param>
        public static void AddEvent<M, K, T, M1>(this IEventTimerHandlerWrapper p_Target, EventTimerParams p_Params, Func<EventTimerHandler<M, K, T, M1>, bool> p_HandleEvent, Func<EventTimerHandler<M, K, T, M1>, bool> p_IntervalBreakPredict, M p_Arg1, K p_Arg2, T p_Args3, M1 p_Args4)
        {
            var eventHandler = p_Target.EventTimer.GetTimer(p_Target, p_Params, p_HandleEvent, p_IntervalBreakPredict, p_Arg1, p_Arg2, p_Args3, p_Args4);
            p_Target.WhenEventHandlerAdd(eventHandler);
        }
                
        /// <summary>
        /// 파라미터를 5개 가지는 이벤트를 등록하는 메서드
        /// </summary>
        /// <param name="p_StartStamp">이벤트가 실행되기까지의 딜레이 msec</param>
        /// <param name="p_Interval">1 이상의 값을 넣으면 해당 이벤트는 다음 이벤트로 전이하지 않고 해당 값의 주기로 무한반복하게 된다.</param>
        /// <param name="p_TerminatePredict">무한 반복이벤트의 종료조건</param>
        /// <param name="p_Event">실행할 이벤트</param>
        public static void AddEvent<M, K, T, M1, M2>(this IEventTimerHandlerWrapper p_Target, EventTimerParams p_Params, Func<EventTimerHandler<M, K, T, M1, M2>, bool> p_HandleEvent, Func<EventTimerHandler<M, K, T, M1, M2>, bool> p_IntervalBreakPredict, M p_Arg1, K p_Arg2, T p_Args3, M1 p_Args4, M2 p_args5)
        {
            var eventHandler = p_Target.EventTimer.GetTimer(p_Target, p_Params, p_HandleEvent, p_IntervalBreakPredict, p_Arg1, p_Arg2, p_Args3, p_Args4, p_args5);
            p_Target.WhenEventHandlerAdd(eventHandler);
        }
                
        /// <summary>
        /// 파라미터를 6개 가지는 이벤트를 등록하는 메서드
        /// </summary>
        /// <param name="p_StartStamp">이벤트가 실행되기까지의 딜레이 msec</param>
        /// <param name="p_Interval">1 이상의 값을 넣으면 해당 이벤트는 다음 이벤트로 전이하지 않고 해당 값의 주기로 무한반복하게 된다.</param>
        /// <param name="p_TerminatePredict">무한 반복이벤트의 종료조건</param>
        /// <param name="p_Event">실행할 이벤트</param>
        public static void AddEvent<M, K, T, M1, M2, M3>(this IEventTimerHandlerWrapper p_Target, EventTimerParams p_Params, Func<EventTimerHandler<M, K, T, M1, M2, M3>, bool> p_HandleEvent, Func<EventTimerHandler<M, K, T, M1, M2, M3>, bool> p_IntervalBreakPredict, M p_Arg1, K p_Arg2, T p_Args3, M1 p_Args4, M2 p_Args5, M3 p_Args6)
        {
            var eventHandler = p_Target.EventTimer.GetTimer(p_Target, p_Params, p_HandleEvent, p_IntervalBreakPredict, p_Arg1, p_Arg2, p_Args3, p_Args4, p_Args5, p_Args6);
            p_Target.WhenEventHandlerAdd(eventHandler);
        }

        #endregion

        #region <Method/Release>

        public static void ReleaseEventHandler(ref GameEventTimerHandlerWrapper r_TargetObject)
        {
            if (r_TargetObject.IsEventValid())
            {
                r_TargetObject.RetrieveObject();
                r_TargetObject = null;
            }
        }
        
        public static void ReleaseEventHandler(ref List<GameEventTimerHandlerWrapper> r_TargetObjectList)
        {
            foreach (var eventHandler in r_TargetObjectList)
            {
                if (eventHandler.IsEventValid())
                {
                    eventHandler.RetrieveObject();
                }
            }

            r_TargetObjectList.Clear();
        }

        #endregion
        
        #region <Method/Release/SafeRef>

        public static void ReleaseEventHandler(ref SafeReference<object, GameEventTimerHandlerWrapper> r_TargetObject)
        {
            var (valid, tryValue) = r_TargetObject.GetValue();
            if (valid)
            {
                tryValue.CancelEvent();
            }

            r_TargetObject = default;
        }
        
        public static void ReleaseEventHandler(ref List<SafeReference<object, GameEventTimerHandlerWrapper>> r_TargetObjectList)
        {
            foreach (var targetObject in r_TargetObjectList)
            {
                var (valid, tryValue) = targetObject.GetValue();
                if (valid)
                {
                    tryValue.CancelEvent();
                }
            }
            r_TargetObjectList.Clear();
        }

        #endregion

        #region <Structs>

        public struct EventTimerParams
        {
            #region <Fields>

            public uint StartStamp;
            public uint Interval;
            public EventTimerIntervalType IntervalType;
            
            #endregion

            #region <Constructors>

            public EventTimerParams(uint p_StartStamp, uint p_Interval, EventTimerIntervalType p_IntervalType)
            {
                StartStamp = p_StartStamp;
                Interval = p_Interval;
                IntervalType = p_IntervalType;
            }

            #endregion

            #region <Operator>

            public static implicit operator EventTimerParams(uint p_StartStamp)
            {
                return new EventTimerParams(p_StartStamp, 0, EventTimerIntervalType.None);
            }
            
            public static implicit operator EventTimerParams(EventTimerIntervalType p_IntervalType)
            {
                return new EventTimerParams(0, 0, p_IntervalType);
            }

            public static implicit operator EventTimerParams((uint t_StartStamp, uint t_Interval) p_Tuple)
            {
                return new EventTimerParams(p_Tuple.t_StartStamp, p_Tuple.t_Interval, EventTimerIntervalType.None);
            }

            public static implicit operator EventTimerParams((uint t_StartStamp, uint t_Interval, EventTimerIntervalType t_IntervalType) p_Tuple)
            {
                return new EventTimerParams(p_Tuple.t_StartStamp, p_Tuple.t_Interval, p_Tuple.t_IntervalType);
            }

#if UNITY_EDITOR
            public override string ToString()
            {
                return $"[StartStamp : {StartStamp}]\n[Interval : {Interval}]\n[IntervalType : {IntervalType}]";
            }
#endif
            
            #endregion
        }

        #endregion
    }
}