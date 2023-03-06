using System;
using UnityEngine;

namespace k514
{
    /// <summary>
    /// 일정한 시간마다 카운터를 줄여가며, 카운터가 0이 되면 지정받은 이벤트를 처리하는 클래스
    /// </summary>
    public class StackTimer
    {
        #region <Fields>

        public bool IsValid;
        public int CountStack { get; private set; }
        private uint IntervalMsec;
        private SafeReference<object, GameEventTimerHandlerWrapper> UpdateHandler;
        private Action<StackTimer> DisposeEventReceiver;
        private Func<StackTimer, bool> UpdateEventReceiver;
        private bool _ManualInvokeTickFlag;
        private bool _BlockTickFlag;
    
        #endregion

        #region <Constructor>

        public StackTimer(uint p_IntervalMsec, Action<StackTimer> p_DisposeEvent, Func<StackTimer, bool> p_UpdateEvent, bool p_ManualInvokeTickFlag)
        {
            IsValid = false;
            IntervalMsec = p_IntervalMsec;
            DisposeEventReceiver = p_DisposeEvent;
            UpdateEventReceiver = p_UpdateEvent;
            _ManualInvokeTickFlag = p_ManualInvokeTickFlag;
        }

        #endregion

        #region <Callbacks>

        public bool OnTick()
        {
            if (IsValid)
            {
                if (_BlockTickFlag)
                {
                    return true;
                }
                else
                {
                    if (CountStack > 0)
                    {
                        CountStack--;
                        return UpdateEventReceiver?.Invoke(this) ?? true;
                    }
                    else
                    {
                        DisposeEventReceiver.Invoke(this);
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        public void OnTerminateStackTimer()
        {
            IsValid = false;
            SetBlockTick(false);
     
            EventTimerTool.ReleaseEventHandler(ref UpdateHandler);
        }

        #endregion
        
        #region <Methods>
        
        public void SetBlockTick(bool p_Flag)
        {
            _BlockTickFlag = p_Flag;
        }
        
        public void AddCount(int p_Count)
        {
            UpdateCount(CountStack + p_Count);
        }
                
        public void OverlapCount(int p_Count)
        {
            if (p_Count > CountStack)
            {
                UpdateCount(p_Count);
            }
        }
        
        public void UpdateCount(int p_InitialStack)
        {
            IsValid = true;

            if (_ManualInvokeTickFlag)
            {
                CountStack = p_InitialStack;
            }
            else
            {
                var (valid, handler) = UpdateHandler.GetValue();
                if (valid && handler.IsEventValid())
                {
                    CountStack = p_InitialStack;
                }
                else
                {
                    CountStack = p_InitialStack;
                    GameEventTimerHandlerManager.GetInstance.SpawnSafeEventTimerHandler(ref UpdateHandler, this, SystemBoot.TimerType.GameTimer, false);
                    var (_, eventHandler) = UpdateHandler.GetValue();
                    eventHandler
                        .AddEvent
                        (
                            (0, IntervalMsec),
                            _handler =>
                            {
                                var thisRef = _handler.Arg1;
                                return thisRef.OnTick();
                            }, 
                            null, this
                        );
                    eventHandler.StartEvent();
                }
            }
        }

        #endregion
    }
        
    /// <summary>
    /// 일정한 시간마다 카운터를 줄여가며, 카운터가 0이 되면 지정받은 이벤트를 처리하는 클래스
    /// </summary>
    public class StackTimer<T>
    {
        #region <Fields>

        public bool IsValid;
        public int CountStack { get; private set; }
        private uint IntervalMsec;
        private SafeReference<object, GameEventTimerHandlerWrapper> UpdateHandler;
        private T Symbol;
        private Action<StackTimer<T>,T> DisposeEventReceiver;
        private Func<StackTimer<T>, T, bool> UpdateEventReceiver;
        private bool _ManualInvokeTickFlag;
        private bool _BlockTickFlag;

        #endregion

        #region <Constructor>

        public StackTimer(uint p_IntervalMsec, T p_Symbol, Action<StackTimer<T>, T> p_DisposeEvent, Func<StackTimer<T>, T, bool> p_UpdateEvent, bool p_ManualInvokeTickFlag)
        {
            IsValid = false;
            IntervalMsec = p_IntervalMsec;
            Symbol = p_Symbol;
            DisposeEventReceiver = p_DisposeEvent;
            UpdateEventReceiver = p_UpdateEvent;
            _ManualInvokeTickFlag = p_ManualInvokeTickFlag;
        }

        #endregion

        #region <Callbacks>

        public bool OnTick()
        {
            if (IsValid)
            {
                if (_BlockTickFlag)
                {
                    return true;
                }
                else
                {
                    if (CountStack > 0)
                    {
                        CountStack--;
                        return UpdateEventReceiver?.Invoke(this, Symbol) ?? true;
                    }
                    else
                    {
                        DisposeEventReceiver.Invoke(this, Symbol);
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        public void OnTerminateStackTimer()
        {
            IsValid = false;
            SetBlockTick(false);
            
            EventTimerTool.ReleaseEventHandler(ref UpdateHandler);
        }

        #endregion
        
        #region <Methods>

        public void SetBlockTick(bool p_Flag)
        {
            _BlockTickFlag = p_Flag;
        }

        public void AddCount(int p_Count)
        {
            UpdateCount(CountStack + p_Count);
        }
        
        public void OverlapCount(int p_Count)
        {
            if (p_Count > CountStack)
            {
                UpdateCount(p_Count);
            }
        }
        
        public void UpdateCount(int p_InitialStack)
        {
            IsValid = true;
            
            if (_ManualInvokeTickFlag)
            {
                CountStack = p_InitialStack;
            }
            else
            {
                var (valid, handler) = UpdateHandler.GetValue();
                if (valid && handler.IsEventValid())
                {
                    CountStack = p_InitialStack;
                }
                else
                {
                    CountStack = p_InitialStack;
                    GameEventTimerHandlerManager.GetInstance.SpawnSafeEventTimerHandler(ref UpdateHandler, this, SystemBoot.TimerType.GameTimer, false);
                    var (_, eventHandler) = UpdateHandler.GetValue();
                    eventHandler
                        .AddEvent
                        (
                            (0, IntervalMsec),
                            _handler =>
                            {
                                var thisRef = _handler.Arg1;
                                return thisRef.OnTick();
                            }, 
                            null, this
                        );
                    eventHandler.StartEvent();
                }
            }
        }

        #endregion
    }
    
    /// <summary>
    /// 일정한 시간마다 카운터를 줄여가며, 카운터가 0이 되면 지정받은 이벤트를 처리하는 클래스
    /// </summary>
    public class StackTimer<T, K>
    {
        #region <Fields>

        public bool IsValid;
        public int CountStack { get; private set; }
        private uint IntervalMsec;
        private SafeReference<object, GameEventTimerHandlerWrapper> UpdateHandler;
        private T Symbol;
        public K Params;
        private Action<StackTimer<T, K>, T> DisposeEventReceiver;
        private Func<StackTimer<T, K>, T, K, bool> UpdateEventReceiver;
        private bool _ManualInvokeTickFlag;
        private bool _BlockTickFlag;

        #endregion

        #region <Constructor>

        public StackTimer(uint p_IntervalMsec, T p_Symbol, Action<StackTimer<T, K>, T> p_DisposeEvent, Func<StackTimer<T, K>, T, K, bool> p_UpdateEvent, bool p_ManualInvokeTickFlag)
        {
            IsValid = false;
            IntervalMsec = p_IntervalMsec;
            Symbol = p_Symbol;
            Params = default;
            DisposeEventReceiver = p_DisposeEvent;
            UpdateEventReceiver = p_UpdateEvent;
            _ManualInvokeTickFlag = p_ManualInvokeTickFlag;
        }

        #endregion

        #region <Callbacks>

        public bool OnTick()
        {
            if (IsValid)
            {
                if (_BlockTickFlag)
                {
                    return true;
                }
                else
                {
                    if (CountStack > 0)
                    {
                        CountStack--;
                        return UpdateEventReceiver?.Invoke(this, Symbol, Params) ?? true;
                    }
                    else
                    {
                        DisposeEventReceiver.Invoke(this, Symbol);
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        public void OnTerminateStackTimer()
        {
            IsValid = false;
            Params = default;
            SetBlockTick(false);
   
            EventTimerTool.ReleaseEventHandler(ref UpdateHandler);
        }

        #endregion
        
        #region <Methods>

        public void SetBlockTick(bool p_Flag)
        {
            _BlockTickFlag = p_Flag;
        }

        public void SetParams(K p_Params)
        {
            Params = p_Params;
        }
        
        public void AddCount(int p_Count)
        {
            UpdateCount(CountStack + p_Count);
        }
                
        public void OverlapCount(int p_Count)
        {
            if (p_Count > CountStack)
            {
                UpdateCount(p_Count);
            }
        }
        
        public void UpdateCount(int p_InitialStack)
        {
            IsValid = true;
            
            if (_ManualInvokeTickFlag)
            {
                CountStack = p_InitialStack;
            }
            else
            {
                var (valid, handler) = UpdateHandler.GetValue();
                if (valid && handler.IsEventValid())
                {
                    CountStack = p_InitialStack;
                }
                else
                {
                    CountStack = p_InitialStack;
                    GameEventTimerHandlerManager.GetInstance.SpawnSafeEventTimerHandler(ref UpdateHandler, this, SystemBoot.TimerType.GameTimer, false);
                    var (_, eventHandler) = UpdateHandler.GetValue();
                    eventHandler
                        .AddEvent
                        (
                            (0, IntervalMsec),
                            _handler =>
                            {
                                var thisRef = _handler.Arg1;
                                return thisRef.OnTick();
                            }, 
                            null, this
                        );
                    eventHandler.StartEvent();
                }
            }
        }

        #endregion
    }
}