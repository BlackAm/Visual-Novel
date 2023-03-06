using System;

namespace k514
{
    public interface ILerpEventTimerHandler : IEventTimerHandlerWrapper
    {
        /// <summary>
        /// 딜레이 없이 즉시, 원본 값으로 리셋하는 메서드
        /// </summary>
        bool ResetValue();
        
        /// <summary>
        /// 지정한 딜레이를 두고, 지정한 시간 동안 값을 원본으로 전이시키는 메서드
        /// </summary>
        bool ResetValueLerpTo(uint p_PreDelayMsec, uint p_LerpDurationMsec);

        /// <summary>
        /// 값이 변하는 경우 호출할 콜백을 지정하는 메서드
        /// </summary>
        void SetValueChangedCallback(Action p_OnValueChanged);

        /// <summary>
        /// Value Lerp가 종료된 경우 호출할 콜백을 지정하는 메서드
        /// </summary>
        void SetValueLerpOverCallback(Action p_OnValueLerpOver);
        
        /// <summary>
        /// Reset Lerp가 종료된 경우 호출할 콜백을 지정하는 메서드
        /// </summary>
        void SetResetLerpOverCallback(Action p_OnResetLerpOver);
    }

    /// <summary>
    /// 특정 값에 대해, EventTimer를 통해 해당 값을 원본 값으로 되돌리거나
    /// 특정 값으로 증가시키는 등의 기능을 제어하는 클래스
    /// </summary>
    public abstract class LerpValueEventTimerHandler<T> : EventTimerHandlerWrapper, ILerpEventTimerHandler
    {
        #region <Fields>

        /// <summary>
        /// 리셋 대상이되는 원본값
        /// </summary>
        public T _DefaultValue { get; protected set; }

        /// <summary>
        /// 현재 값
        /// </summary>
        public T _CurrentValue { get; protected set; }
        
        /// <summary>
        /// 현재 값 - 이전 값
        /// </summary>
        public T _DeltaValue { get; protected set; }

        /// <summary>
        /// 해당 클래스의 값이 변경된 경우 호출되는 콜백
        /// </summary>
        public Action OnValueChanged { get; private set; }
        
        /// <summary>
        /// 해당 클래스의 러프가 종료된 경우 호출되는 콜백
        /// </summary>
        public Action OnValueLerpOver { get; private set; }
        
        /// <summary>
        /// 해당 클래스의 리셋 러프가 종료된 경우 호출되는 콜백
        /// </summary>
        public Action OnResetLerpOver { get; private set; }

        #endregion

        #region <Constructor>

        protected LerpValueEventTimerHandler() : this(SystemBoot.GameEventTimer)
        {
        }

        public LerpValueEventTimerHandler(EventTimer p_EventTimer) : base(p_EventTimer)
        {
        }

        #endregion

        #region <Callbacks>

        /// <summary>
        /// 러프 함수 종료시 호출되는 메서드
        /// </summary>
        protected virtual void OnValueLerpTerminate(bool p_IsResetLerp)
        {
            if (p_IsResetLerp)
            {
                OnResetLerpOver?.Invoke();
            }
            else
            {
                OnValueLerpOver?.Invoke();
            }
        }

        #endregion
        
        #region <Methods>

        /// <summary>
        /// 기본 값을 지정하는 메서드
        /// 기본 값이 갱신되므로 나머지 값들도 초기화된다.
        /// </summary>
        public void SetDefaultValue(T p_DefaultValue)
        {
            _DefaultValue = _CurrentValue = p_DefaultValue;
            _DeltaValue = default;
        }
        
        /// <summary>
        /// 현재 값 변경시 처리할 이벤트 콜백을 지정하는 메서드
        /// </summary>
        public void SetValueChangedCallback(Action p_OnValueChanged)
        {
            OnValueChanged = p_OnValueChanged;
        }

        /// <summary>
        /// 러프 함수 종료시 처리할 이벤트 콜백을 지정하는 메서드
        /// </summary>
        public void SetValueLerpOverCallback(Action p_OnValueLerpOver)
        {
            OnValueLerpOver = p_OnValueLerpOver;
        }
                
        /// <summary>
        /// 리셋 러프 함수 종료시 처리할 이벤트 콜백을 지정하는 메서드
        /// </summary>
        public void SetResetLerpOverCallback(Action p_OnResetLerpOver)
        {
            OnResetLerpOver = p_OnResetLerpOver;
        }

        /// <summary>
        /// 현재 클래스의 값을 증가시키는 메서드
        /// </summary>
        public abstract bool AddValue(T p_AdditiveValue, float p_DeltaTime);

        /// <summary>
        /// 현재 클래스의 값을 지정한 값으로 세트하는 메서드
        /// </summary>
        public abstract bool SetValue(T p_TargetValue);

        /// <summary>
        /// 현재 클래스의 값을 deltaValue를 이용하여 한단계 이전으로 초기화 시키는 메서드
        /// </summary>
        public abstract void RevertValueChange();
        
        /// <summary>
        /// 현재 클래스의 값을 리셋시키는 메서드
        /// </summary>
        public bool ResetValue()
        {
            if (!IsEventValid())
            {
                return SetValue(_DefaultValue);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 지정한 딜레이를 두고, 지정한 시간 동안 값을 더해 전이시키는 메서드
        /// </summary>
        public abstract bool AddValueLerpTo(T p_AdditiveValue, uint p_PreDelayMsec, uint p_LerpDurationMsec);

        /// <summary>
        /// 지정한 딜레이를 두고, 지정한 시간 동안 값으로 전이시키는 메서드
        /// </summary>
        public bool SetValueLerpTo(T p_TargetValue, uint p_PreDelayMsec, uint p_LerpDurationMsec)
        {
            return SpawnLerpEvent(p_TargetValue, p_PreDelayMsec, p_LerpDurationMsec, false);
        }
        
        /// <summary>
        /// 지정한 딜레이를 두고, 지정한 시간 동안 값을 원본으로 전이시키는 메서드
        /// </summary>
        public bool ResetValueLerpTo(uint p_PreDelayMsec, uint p_LerpDurationMsec)
        {
            return SpawnLerpEvent(_DefaultValue, p_PreDelayMsec, p_LerpDurationMsec, true);
        }

        /// <summary>
        /// 러프를 진행할 이벤트 타이머를 생성하는 메서드
        /// </summary>
        protected abstract bool SpawnLerpEvent(T p_TargetValue, uint p_PreDelayMsec, uint p_LerpDurationMsec, bool p_IsReset);

        #endregion
    }
}