using System;
using System.Threading;
using UnityEngine;

namespace k514
{
    /// <summary>
    /// timeStamp를 측정하여 등록된 이벤트를 호출하는 기능을 가지는 클래스.
    /// </summary>
    public abstract partial class EventTimer
    {
        #region <Method/Tick>

        protected void TickProgress(float p_DeltaTime)
        {
            while (!_queue.IsEmpty() /*&& _queue.DataCount < 1000*/)
            {
                IEventTimerHandler tryEventHandler;
                lock (QUEUE_LOCK)
                {
                    tryEventHandler = _queue.Peek();
                }
         
                // 발동 타이밍이 아직인 경우 패스, 이벤트 핸들러는 시간순으로 정렬되기 때문에
                // 해당 루프문에 재진입 시킬 필요없이 바로 Break한다.
                if (_elapsedMsec < tryEventHandler.TargetEventStamp)
                {
                    break;
                }
                         
                // 발동해야 하는 경우, 해당 이벤트를 발동하는데
                // 해당 이벤트의 주기만큼 미래에 동일한 이벤트를 예약시킨다.
                lock (QUEUE_LOCK)
                {
                    _queue.Dequeue();
                    tryEventHandler. IsQueued = false;
                }
                         
                // 등록된 이벤트가 반복되어야 하는 경우
                if (tryEventHandler.EventIntervalMsec > 0)
                {
                    // 반복 이벤트가 만료된 경우 폐기한다.
                    if (tryEventHandler.CheckIntervalBreak())
                    {
                        tryEventHandler.OnTerminateEventTimerHandler();
                    }
                    // 반복 이벤트가 유효한 경우 다음 주기에 호출되도록 큐에 재삽입한다.
                    else
                    {
                        // 매 프레임 갱신되는 이벤트의 경우
                        if (tryEventHandler.IsReenterEveryTick)
                        {
                            if (tryEventHandler.InvokeEvent(p_DeltaTime, _elapsedMsec))
                            {
                                // 러프 이벤트는 매 프레임 다시 호출되어야 하므로
                                // 발동 타이밍에 1을 더해서 무한 루프를 막는다.
                                tryEventHandler.TargetEventStamp = _elapsedMsec + 1;
                                         
                                lock (QUEUE_LOCK)
                                {
                                    tryEventHandler.OnHandleReenterQueue(_queue);
                                }
                            }
                            // 이벤트 처리 결과에 따라 폐기한다.
                            else
                            {
                                tryEventHandler.OnTerminateEventTimerHandler();
                            }
                        }
                        else
                        {
                            if (tryEventHandler.InvokeEvent(p_DeltaTime, _elapsedMsec))
                            {
                                // 반복 이벤트는 일정한 주기를 가지고 다시 호출되어야 하므로
                                // 발동 타이밍에 주기 interval을 더해서 우선순위 큐에 재진입시킨다.
                                tryEventHandler.TargetStartStamp = _elapsedMsec;
                                tryEventHandler.TargetEventStamp = _elapsedMsec + tryEventHandler.EventIntervalMsec;
             
                                lock (QUEUE_LOCK)
                                {
                                    tryEventHandler.OnHandleReenterQueue(_queue);
                                }
                            }
                            // 이벤트 처리 결과에 따라 폐기한다.
                            else
                            {
                                tryEventHandler.OnTerminateEventTimerHandler();
                            }
                        }
                    }
                }
                else
                {
                    // 주기가 없는 이벤트의 경우에는, 이벤트 호출 후 파기한다.
                    tryEventHandler.InvokeEvent(p_DeltaTime, _elapsedMsec);
                    tryEventHandler.OnTerminateEventTimerHandler();
                }
            }
        }

        #endregion
        
        #region <Method/Timer/Regist>

        /// <summary>
        /// 타이머 힙에 타이머 핸들러를 삽입하는 메서드
        /// </summary>
        public void RegistTimer(IEventTimerHandler p_EventTimerHandler)
        {
            lock (QUEUE_LOCK)
            {
                /* 이벤트 핸들러 큐 삽입시 스탬프 = 현재 스탬프 */
                p_EventTimerHandler.TargetStartStamp = _elapsedMsec;

                // 해당 이벤트 핸들러가 정지상태에서 재개된 경우
                if (p_EventTimerHandler.IsPaused)
                {
                    // 매 프레임 갱신되는 이벤트의 경우
                    if (p_EventTimerHandler.IsReenterEveryTick)
                    {
                        /* 이벤트 핸들러 이벤트 발동 스탬프 = 현재 스탬프 + 1 = 바로 다음 프레임에서 동작 */
                        p_EventTimerHandler.TargetEventStamp = _elapsedMsec + 1;      
                    }
                    else
                    {
                        /* 이벤트 핸들러 이벤트 발동 스탬프 = 정지됬던 이벤트의 남은 시간 + 현재 스탬프 + 1 */
                        p_EventTimerHandler.TargetEventStamp = p_EventTimerHandler.GetRemaindMsecInInterval() + _elapsedMsec + 1;    
                    }
                    _queue.Enqueue(p_EventTimerHandler.TimerID, p_EventTimerHandler, p_EventTimerHandler.TargetEventStamp);
                    p_EventTimerHandler.OnResumeEventTimerHandler();
                }
                // 해당 이벤트 핸들러가 최초실행되는 경우
                else
                {
                    /* 이벤트 핸들러 이벤트 발동 스탬프 = 이벤트 선딜레이 + 현재 스탬프 + 1 */
                    p_EventTimerHandler.TargetEventStamp = p_EventTimerHandler.StartMsec + _elapsedMsec + 1;
                    _queue.Enqueue(p_EventTimerHandler.TimerID, p_EventTimerHandler, p_EventTimerHandler.TargetEventStamp);
                    p_EventTimerHandler.OnActivateEventTimerHandler();
                }
            }
        }
        
        #endregion
        
        #region <Method/Timer/Delete>
        
        /// <summary>
        /// 지정한 이벤트 핸들러 타이머를 가진 이벤트 핸들러를 힙에서 제외하고 리턴하는 메서드
        /// 핸들러의 상태는 저장된다.
        /// </summary>
        public IEventTimerHandler PauseTimer(uint p_TimerId)
        {
            lock (QUEUE_LOCK)
            {
                var pausedHandler = _queue.Remove(p_TimerId);
                pausedHandler.OnPauseEventTimerHandler();
                return pausedHandler;
            }
        }

        /// <summary>
        /// 지정한 이벤트 핸들러 타이머를 가진 이벤트 핸들러를 힙으로부터 제거하는 메서드
        /// </summary>
        public void DelTimer(uint p_TimerId)
        {
            lock (QUEUE_LOCK)
            {
                var removedHandler = _queue.Remove(p_TimerId);
                removedHandler.OnTerminateEventTimerHandler();
            }
        }
        
        #endregion

        #region <Method/Timer/Spawn>

        /// <summary>
        /// 파라미터가 0개인 타이머 이벤트 핸들러를 생성하는 메서드
        /// </summary>
        public EventTimerHandler GetTimer(IEventTimerHandlerWrapper p_EventTimerHandlerWrapper, EventTimerTool.EventTimerParams p_Params, Func<EventTimerHandler, bool> p_HandleEvent, Func<EventTimerHandler, bool> p_IntervalBreakPredict = null)
        {
            var spawnedTimer = EventTimerHandler.TimerHandlerPool.GetObject();
            spawnedTimer.Event = p_HandleEvent;
            spawnedTimer.IntevalBreakPredict = p_IntervalBreakPredict;
            
            spawnedTimer.OnInitiateEventTimerHandler(this, ++_nextTimerID, p_EventTimerHandlerWrapper, p_Params);
            
            return spawnedTimer;
        }

        /// <summary>
        /// 파라미터가 0개인 타이머 이벤트 핸들러를 생성하여 타이머 힙에 예약하는 메서드
        /// </summary>
        public void RunTimer(IEventTimerHandlerWrapper p_EventTimerHandlerWrapper, EventTimerTool.EventTimerParams p_Params, Func<EventTimerHandler, bool> p_HandleEvent, Func<EventTimerHandler, bool> p_IntervalBreakPredict = null)
        {
            var spawnedTimer = GetTimer(p_EventTimerHandlerWrapper, p_Params, p_HandleEvent, p_IntervalBreakPredict);
            RegistTimer(spawnedTimer);
        }

        /// <summary>
        /// 파라미터가 1개인 타이머 이벤트 핸들러를 생성하는 메서드
        /// </summary>
        public EventTimerHandler<M> GetTimer<M>(IEventTimerHandlerWrapper p_EventTimerHandlerWrapper, EventTimerTool.EventTimerParams p_Params, Func<EventTimerHandler<M>, bool> p_HandleEvent, Func<EventTimerHandler<M>, bool> p_IntervalBreakPredict, M arg1)
        {
            var spawnedTimer = EventTimerHandler<M>.TimerHandlerPool.GetObject();
            spawnedTimer.Event = p_HandleEvent;
            spawnedTimer.IntevalBreakPredict = p_IntervalBreakPredict;
            spawnedTimer.Arg1 = arg1;

            spawnedTimer.OnInitiateEventTimerHandler(this, ++_nextTimerID, p_EventTimerHandlerWrapper, p_Params);
            
            return spawnedTimer;
        }

        /// <summary>
        /// 파라미터가 1개인 타이머 이벤트 핸들러를 생성하여 타이머 힙에 예약하는 메서드
        /// </summary>
        public void RunTimer<M>(IEventTimerHandlerWrapper p_EventTimerHandlerWrapper, EventTimerTool.EventTimerParams p_Params, Func<EventTimerHandler<M>, bool> p_HandleEvent, Func<EventTimerHandler<M>, bool> p_IntervalBreakPredict, M arg1)
        {
            var spawnedTimer = GetTimer(p_EventTimerHandlerWrapper, p_Params, p_HandleEvent, p_IntervalBreakPredict, arg1);
            RegistTimer(spawnedTimer);
        }
        
        /// <summary>
        /// 파라미터가 2개인 타이머 이벤트 핸들러를 생성하는 메서드
        /// </summary>
        public EventTimerHandler<M, K> GetTimer<M, K>(IEventTimerHandlerWrapper p_EventTimerHandlerWrapper, EventTimerTool.EventTimerParams p_Params, Func<EventTimerHandler<M, K>, bool> p_HandleEvent, Func<EventTimerHandler<M, K>, bool> p_IntervalBreakPredict, M arg1, K arg2)
        {
            var spawnedTimer = EventTimerHandler<M, K>.TimerHandlerPool.GetObject();
            spawnedTimer.Event = p_HandleEvent;
            spawnedTimer.IntevalBreakPredict = p_IntervalBreakPredict;
            spawnedTimer.Arg1 = arg1;
            spawnedTimer.Arg2 = arg2;

            spawnedTimer.OnInitiateEventTimerHandler(this, ++_nextTimerID, p_EventTimerHandlerWrapper, p_Params);
            
            return spawnedTimer;
        }
        
        /// <summary>
        /// 파라미터가 2개인 타이머 이벤트 핸들러를 생성하여 타이머 힙에 예약하는 메서드
        /// </summary>
        public void RunTimer<M, K>(IEventTimerHandlerWrapper p_EventTimerHandlerWrapper, EventTimerTool.EventTimerParams p_Params, Func<EventTimerHandler<M, K>, bool> p_HandleEvent, Func<EventTimerHandler<M, K>, bool> p_IntervalBreakPredict, M arg1, K arg2)
        {
            var spawnedTimer = GetTimer(p_EventTimerHandlerWrapper, p_Params, p_HandleEvent, p_IntervalBreakPredict, arg1, arg2);
            RegistTimer(spawnedTimer);
        }
                
        /// <summary>
        /// 파라미터가 3개인 타이머 이벤트 핸들러를 생성하는 메서드
        /// </summary>
        public EventTimerHandler<M, K, T> GetTimer<M, K, T>(IEventTimerHandlerWrapper p_EventTimerHandlerWrapper, EventTimerTool.EventTimerParams p_Params, Func<EventTimerHandler<M, K, T>, bool> p_HandleEvent, Func<EventTimerHandler<M, K, T>, bool> p_IntervalBreakPredict, M arg1, K arg2, T arg3)
        {
            var spawnedTimer = EventTimerHandler<M, K, T>.TimerHandlerPool.GetObject();
            spawnedTimer.Event = p_HandleEvent;
            spawnedTimer.IntevalBreakPredict = p_IntervalBreakPredict;
            spawnedTimer.Arg1 = arg1;
            spawnedTimer.Arg2 = arg2;
            spawnedTimer.Arg3 = arg3;

            spawnedTimer.OnInitiateEventTimerHandler(this, ++_nextTimerID, p_EventTimerHandlerWrapper, p_Params);
            
            return spawnedTimer;
        }
        
        /// <summary>
        /// 파라미터가 3개인 타이머 이벤트 핸들러를 생성하여 타이머 힙에 예약하는 메서드
        /// </summary>
        public void RunTimer<M, K, T>(IEventTimerHandlerWrapper p_EventTimerHandlerWrapper, EventTimerTool.EventTimerParams p_Params, Func<EventTimerHandler<M, K, T>, bool> p_HandleEvent, Func<EventTimerHandler<M, K, T>, bool> p_IntervalBreakPredict, M arg1, K arg2, T arg3)
        {
            var spawnedTimer = GetTimer(p_EventTimerHandlerWrapper, p_Params, p_HandleEvent, p_IntervalBreakPredict, arg1, arg2, arg3);
            RegistTimer(spawnedTimer);
        }

        /// <summary>
        /// 파라미터가 4개인 타이머 이벤트 핸들러를 생성하는 메서드
        /// </summary>
        public EventTimerHandler<M, K, T, M1> GetTimer<M, K, T, M1>(IEventTimerHandlerWrapper p_EventTimerHandlerWrapper, EventTimerTool.EventTimerParams p_Params, Func<EventTimerHandler<M, K, T, M1>, bool> p_HandleEvent, Func<EventTimerHandler<M, K, T, M1>, bool> p_IntervalBreakPredict, M arg1, K arg2, T arg3, M1 arg4)
        {
            var spawnedTimer = EventTimerHandler<M, K, T, M1>.TimerHandlerPool.GetObject();
            spawnedTimer.Event = p_HandleEvent;
            spawnedTimer.IntevalBreakPredict = p_IntervalBreakPredict;
            spawnedTimer.Arg1 = arg1;
            spawnedTimer.Arg2 = arg2;
            spawnedTimer.Arg3 = arg3;
            spawnedTimer.Arg4 = arg4;
            
            spawnedTimer.OnInitiateEventTimerHandler(this, ++_nextTimerID, p_EventTimerHandlerWrapper, p_Params);
            
            return spawnedTimer;
        }


        /// <summary>
        /// 파라미터가 4개인 타이머 이벤트 핸들러를 생성하여 타이머 힙에 예약하는 메서드
        /// </summary>
        public void RunTimer<M, K, T, M1>(IEventTimerHandlerWrapper p_EventTimerHandlerWrapper, EventTimerTool.EventTimerParams p_Params, Func<EventTimerHandler<M, K, T, M1>, bool> p_HandleEvent, Func<EventTimerHandler<M, K, T, M1>, bool> p_IntervalBreakPredict, M arg1, K arg2, T arg3, M1 arg4)
        {
            var spawnedTimer = GetTimer(p_EventTimerHandlerWrapper, p_Params, p_HandleEvent, p_IntervalBreakPredict, arg1, arg2, arg3, arg4);
            RegistTimer(spawnedTimer);
        }

        /// <summary>
        /// 파라미터가 5개인 타이머 이벤트 핸들러를 생성하는 메서드
        /// </summary>
        public EventTimerHandler<M, K, T, M1, M2> GetTimer<M, K, T, M1, M2>(IEventTimerHandlerWrapper p_EventTimerHandlerWrapper, EventTimerTool.EventTimerParams p_Params, Func<EventTimerHandler<M, K, T, M1, M2>, bool> p_HandleEvent, Func<EventTimerHandler<M, K, T, M1, M2>, bool> p_IntervalBreakPredict, M arg1, K arg2, T arg3, M1 arg4, M2 arg5)
        {
            var spawnedTimer = EventTimerHandler<M, K, T, M1, M2>.TimerHandlerPool.GetObject();
            spawnedTimer.Event = p_HandleEvent;
            spawnedTimer.IntevalBreakPredict = p_IntervalBreakPredict;
            spawnedTimer.Arg1 = arg1;
            spawnedTimer.Arg2 = arg2;
            spawnedTimer.Arg3 = arg3;
            spawnedTimer.Arg4 = arg4;
            spawnedTimer.Arg5 = arg5;
            
            spawnedTimer.OnInitiateEventTimerHandler(this, ++_nextTimerID, p_EventTimerHandlerWrapper, p_Params);

            return spawnedTimer;
        }

        /// <summary>
        /// 파라미터가 5개인 타이머 이벤트 핸들러를 생성하여 타이머 힙에 예약하는 메서드
        /// </summary>
        public void RunTimer<M, K, T, M1, M2>(IEventTimerHandlerWrapper p_EventTimerHandlerWrapper, EventTimerTool.EventTimerParams p_Params, Func<EventTimerHandler<M, K, T, M1, M2>, bool> p_HandleEvent, Func<EventTimerHandler<M, K, T, M1, M2>, bool> p_IntervalBreakPredict, M arg1, K arg2, T arg3, M1 arg4, M2 arg5)
        {
            var spawnedTimer = GetTimer(p_EventTimerHandlerWrapper, p_Params, p_HandleEvent, p_IntervalBreakPredict, arg1, arg2, arg3, arg4, arg5);
            RegistTimer(spawnedTimer);
        }
        
        /// <summary>
        /// 파라미터가 6개인 타이머 이벤트 핸들러를 생성하는 메서드
        /// </summary>
        public EventTimerHandler<M, K, T, M1, M2, M3> GetTimer<M, K, T, M1, M2, M3>(IEventTimerHandlerWrapper p_EventTimerHandlerWrapper, EventTimerTool.EventTimerParams p_Params, Func<EventTimerHandler<M, K, T, M1, M2, M3>, bool> p_HandleEvent, Func<EventTimerHandler<M, K, T, M1, M2, M3>, bool> p_IntervalBreakPredict, M arg1, K arg2, T arg3, M1 arg4, M2 arg5, M3 arg6)
        {
            var spawnedTimer = EventTimerHandler<M, K, T, M1, M2, M3>.TimerHandlerPool.GetObject();
            spawnedTimer.Event = p_HandleEvent;
            spawnedTimer.IntevalBreakPredict = p_IntervalBreakPredict;
            spawnedTimer.Arg1 = arg1;
            spawnedTimer.Arg2 = arg2;
            spawnedTimer.Arg3 = arg3;
            spawnedTimer.Arg4 = arg4;
            spawnedTimer.Arg5 = arg5;
            spawnedTimer.Arg6 = arg6;

            spawnedTimer.OnInitiateEventTimerHandler(this, ++_nextTimerID, p_EventTimerHandlerWrapper, p_Params);
            
            return spawnedTimer;
        }

        /// <summary>
        /// 파라미터가 5개인 타이머 이벤트 핸들러를 생성하여 타이머 힙에 예약하는 메서드
        /// </summary>
        public void RunTimer<M, K, T, M1, M2, M3>(IEventTimerHandlerWrapper p_EventTimerHandlerWrapper, EventTimerTool.EventTimerParams p_Params, Func<EventTimerHandler<M, K, T, M1, M2, M3>, bool> p_HandleEvent, Func<EventTimerHandler<M, K, T, M1, M2, M3>, bool> p_IntervalBreakPredict, M arg1, K arg2, T arg3, M1 arg4, M2 arg5, M3 arg6)
        {
            var spawnedTimer = GetTimer(p_EventTimerHandlerWrapper, p_Params, p_HandleEvent, p_IntervalBreakPredict, arg1, arg2, arg3, arg4, arg5, arg6);
            RegistTimer(spawnedTimer);
        }
        
        #endregion
    }
}