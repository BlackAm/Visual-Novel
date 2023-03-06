using UnityEngine;

namespace k514
{
    /// <summary>
    /// LerpValueEventTimerHandler의 Color 구현체
    /// </summary>
    public class ColorLerpEventTimerHandler : LerpValueEventTimerHandler<Color>
    {
        #region <Constructor>

        public ColorLerpEventTimerHandler(EventTimer p_EventTimer) : base(p_EventTimer)
        {
        }

        #endregion

        #region <Methods>
 
        public override bool AddValue(Color p_AdditiveValue, float p_DeltaTime)
        {
            if (!IsEventValid())
            {
                _DeltaValue = p_AdditiveValue * p_DeltaTime;
                _CurrentValue += _DeltaValue;
                OnValueChanged?.Invoke();
                return true;
            }
            else
            {
                return false;
            }
        }
        
        public override bool SetValue(Color p_TargetValue)
        {
            var prevValue = _CurrentValue;
            _CurrentValue = p_TargetValue;
            _DeltaValue = _CurrentValue - prevValue;
            OnValueChanged?.Invoke();
            return true;
        }

        public override void RevertValueChange()
        {
            _CurrentValue -= _DeltaValue;
            _DeltaValue = default;
            OnValueChanged?.Invoke();
        }

        public override bool AddValueLerpTo(Color p_AdditiveValue, uint p_PreDelayMsec, uint p_LerpDurationMsec)
        {
            return SetValueLerpTo(_CurrentValue + p_AdditiveValue, p_PreDelayMsec, p_LerpDurationMsec);
        }

        protected override bool SpawnLerpEvent(Color p_TargetValue, uint p_PreDelayMsec, uint p_LerpDurationMsec, bool p_IsReset)
        {
            if (_CurrentValue.IsColorEquals(p_TargetValue))
            {
                return false;
            }
            else
            {
                // 해당 이벤트 수신자에 이벤트를 등록하므로, 이전에 러프를 수행하던 핸들러가 있다면 자동으로 취소된다.
                EventTimer.RunTimer
                (
                    this,
                    (p_PreDelayMsec, p_LerpDurationMsec, EventTimerTool.EventTimerIntervalType.FlyingLerp),
                    (timerHandler) =>
                    {
                        var valueHandler = timerHandler.Arg3;
                        if (timerHandler.LerpTimer.IsOver())
                        {
                            valueHandler.SetValue(timerHandler.Arg2);
                            valueHandler.OnValueLerpTerminate(timerHandler.Arg4);
                            return false;
                        }
                        else
                        {
                            var progressRate = timerHandler.LerpTimer.ProgressRate;
                            var lerpValue = Color.Lerp(timerHandler.Arg1, timerHandler.Arg2, progressRate);
                            valueHandler.SetValue(lerpValue);
                            return true;
                        }  
                    }, null,
                    _CurrentValue, 
                    p_TargetValue, 
                    this,
                    p_IsReset
                );
                
                return true;
            }
        }

        #endregion
    }
}