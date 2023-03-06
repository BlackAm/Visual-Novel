using System;
using System.Collections.Generic;
using UnityEngine;

namespace k514
{
    // <TODO> : 시간 되감기 이벤트에 대응하도록, 큐 기반에서 리스트 기반으로 바꾸고 이전에 거쳐간 이벤트에 재진입할 수 있게 바꿀지 고려
    /// <summary>
    /// 내부에 타이머 이벤트를 큐로 관리하여 비동기 작업을 모사하는 코루틴 클래스
    /// </summary>
    public class EventTimerCoroutine : EventTimerHandlerWrapper<EventTimerCoroutine>
    {
        #region <Consts>

        /// <summary>
        /// 해당 이벤트 코루틴 오브젝트에 등록 가능한 이벤트 핸들러 갯수
        /// </summary>
        private const int _EVENT_QUEUE_CAPACITY = 64;

        #endregion
        
        #region <Fields>

        /// <summary>
        /// 해당 이벤트 코루틴에서 실행될 이벤트 핸들러 큐
        /// </summary>
        private Queue<IEventTimerHandler> _EventQueue;

        /// <summary>
        /// 해당 이벤트 코루틴에 등록된 이벤트 핸들러의 원본 데이터 큐
        /// </summary>
        private List<IEventTimerHandler> _EventRecordList;
        
        /// <summary>
        /// 현재 수행중인 코루틴 상태
        /// </summary>
        private CoroutineState _CurrentState;
        
        /// <summary>
        /// 코루틴 플래그 마스크
        /// </summary>
        public CoroutinProcessFlag EventTimerCoroutineFlagMask;
        
        #endregion

        #region <Enums>

        private enum CoroutineState
        {
            Block,
            Running,
        }

        [Flags]
        public enum CoroutinProcessFlag
        {
            None = 0,

            /// <summary>
            /// 해당 플래그를 포함하는 코루틴은, 등록된 이벤트를 모두 소진하는 경우
            /// 다시 처음부터 이벤트를 실행한다.
            /// </summary>
            LoopCoroutine = 1 << 0,
            
            /// <summary>
            /// 해당 플래그를 포함하는 코루틴은, 코루틴 일시정지 시에 코루틴이 초기화되지 않고
            /// 재개할 때 해당 코루틴이 멈췄던 타임스탬프부터 작업을 개시한다.
            /// </summary>
            KeepStatusWhenResume = 1 << 1,
        }

        #endregion
        
        #region <Callbacks>

        public override void OnSpawning()
        {
            _EventQueue = new Queue<IEventTimerHandler>(_EVENT_QUEUE_CAPACITY);
            _EventRecordList = new List<IEventTimerHandler>(_EVENT_QUEUE_CAPACITY);
        }

        public override void OnPooling()
        {
        }

        public override void OnRetrieved()
        {
            _CurrentState = CoroutineState.Block;
            
            // 아직 이벤트 타이머에 등록되지 않은 이벤트 핸들러 큐를 비운다.
            ClearQueue();
            
            base.OnRetrieved();
        }

        /// <summary>
        /// 이벤트 핸들러가 활성화 상태에서 파기된 경우 다음 이벤트를 호출해야한다.
        /// </summary>
        public override void WhenPropertyTurnToDefault()
        {
            if (IsEventValid())
            {
                switch (_CurrentState)
                {
                    // Toggle 메서드를 통해, 이벤트 외부의 요청으로 이벤트가 종료된 경우
                    case CoroutineState.Block:
                        break;
                    // 이벤트가 조건을 달성하여 만료된 경우
                    case CoroutineState.Running:
                        // 예약된 이벤트가 존재하는 경우
                        if (_EventQueue.Count > 0)
                        {
                            // 예약된 이벤트 핸들러를 이벤트 타이머에 등록시킨다.
                            StartNextEvent();
                        }
                        // 예약된 이벤트가 더 이상 없는 경우
                        else
                        {
                            // 현재 코루틴 상태를 갱신시켜준다.
                            _CurrentState = CoroutineState.Block;
                            
                            // 루프 플래그가 세워진 경우
                            if (EventTimerCoroutineFlagMask.HasAnyFlagExceptNone(CoroutinProcessFlag.LoopCoroutine))
                            {
#if UNITY_EDITOR
                                if (CustomDebug.PrintEventTimerBasedCoroutine)
                                {
                                    Debug.Log($" - LoopCoroutine - ");
                                }
#endif
                                // 자동으로 코루틴을 재실행해준다.
                                ToggleActiveCoroutine();
                            }
                        }
                        break;
                }
            }
        }

        public override void WhenEventHandlerAdd(IEventTimerHandler p_BaseEventTimerHandler)
        {
            _EventRecordList.Add(p_BaseEventTimerHandler);
        }
        
        #endregion

        #region <Methods>

        /// <summary>
        /// 큐에 예약된 다음 이벤트를 실행시킨다.
        /// </summary>
        public void StartNextEvent()
        {
            base.WhenEventHandlerAdd(_EventQueue.Dequeue());
          
            StartEvent();
        }

        /// <summary>
        /// 코루틴 작업을 활성화시키는 메서드
        /// 정지 상태에선 코루틴 작업을 시작하게 하고
        /// 동작 상태에선 코루틴 작업을 정지시켜, 다음에 활성화시킬 때에는
        /// 처음 작업부터 하도록 초기화시킨다.
        /// </summary>
        public void ToggleActiveCoroutine()
        {
            switch (_CurrentState)
            {
                // 코루틴을 실행해야하는 경우
                case CoroutineState.Block :
                    if (EventTimerCoroutineFlagMask.HasAnyFlagExceptNone(CoroutinProcessFlag.KeepStatusWhenResume) && IsEventValid())
                    {
                        _CurrentState = CoroutineState.Running;
                        StartEvent();
                        
#if UNITY_EDITOR
                        if (CustomDebug.PrintEventTimerBasedCoroutine)
                        {
                            Debug.Log($"ResumeCoroutine id : {_BaseEventTimerHandler.TimerID}");
                        }
#endif
                    }
                    else
                    {
                        ClearQueue();
                
                        if (_EventRecordList.Count > 0)
                        {
                            foreach (var eventHandler in _EventRecordList)
                            {
                                _EventQueue.Enqueue(eventHandler.GetClone(EventTimer));
                            } 
                            
                            _CurrentState = CoroutineState.Running;
                            StartNextEvent();
                            
#if UNITY_EDITOR
                            if (CustomDebug.PrintEventTimerBasedCoroutine)
                            {
                                Debug.Log($"StartCoroutine id : {_BaseEventTimerHandler.TimerID}");
                            }
#endif
                        }
                    }
            
                    break;
                // 코루틴을 정지시켜야하는 경우
                case CoroutineState.Running :
                    _CurrentState = CoroutineState.Block;
                    
                    if (EventTimerCoroutineFlagMask.HasAnyFlagExceptNone(CoroutinProcessFlag.KeepStatusWhenResume))
                    {
#if UNITY_EDITOR
                        if (CustomDebug.PrintEventTimerBasedCoroutine)
                        {
                            Debug.Log($"PauseCoroutine id : {_BaseEventTimerHandler.TimerID}");
                        }
#endif
                        PauseEvent();
                    }
                    else
                    {
#if UNITY_EDITOR
                        if (CustomDebug.PrintEventTimerBasedCoroutine)
                        {
                            Debug.Log($"CancelCoroutine id : {_BaseEventTimerHandler.TimerID}");
                        }
#endif
                        CancelEvent();
                    }

                    break;
            }
        }

        /// <summary>
        /// 이벤트 큐를 제거해준다.
        /// </summary>
        private void ClearQueue()
        {
            while (_EventQueue.Count > 0)
            {
                var removeEventHandler = _EventQueue.Dequeue();
                removeEventHandler.OnTerminateEventTimerHandler();
            }
        }

        #endregion
    }
}