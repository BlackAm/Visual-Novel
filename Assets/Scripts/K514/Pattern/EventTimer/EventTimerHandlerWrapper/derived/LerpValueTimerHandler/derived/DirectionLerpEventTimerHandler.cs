using UnityEngine;

namespace BlackAm
{
    /// <summary>
    /// LerpValueEventTimerHandler의 방향 벡터 구현체
    /// 
    /// PositionValue가 좌표에 대한 값의 기록/변경/러프를 담당하는 이벤트 핸들러라면
    /// 해당 클래스는 벡터를 회전시키는 이벤트를 담당한다.
    ///
    /// 이때, 해당 이벤트 핸들러에 기록되는 벡터는 유닛벡터로 자동으로 보정된다.
    ///
    /// 예를 들어, 반경 1짜리의 구면의 중심으로부터 표면을 향해 나와있는 벡터 s를 지정한 방향 d로
    /// x도 만큼 회전시켜서 다른 구면의 표면을 향하도록 하거나, 러프하는 등의 이벤트를 처리한다.
    ///
    /// 주로 카메라의 회전같은 물체의 회전을 기술하는 용도로 사용된다.
    ///
    /// 해당 핸들러에서 다루는 벡터들은 전부 월드벡터를 기준으로 한다.
    /// 
    /// </summary>
    public class DirectionLerpEventTimerHandler : LerpValueEventTimerHandler<Vector3>
    {
        #region <Consts>

        private const float __DefaultRotateUpperBound = -0.95f;
        private const float __DefaultRotateLowerBound = 0.8f;

        #endregion
        
        #region <Constructor>

        public DirectionLerpEventTimerHandler(EventTimer p_EventTimer) : base(p_EventTimer)
        {
            _RotateUpperYBound = __DefaultRotateUpperBound;
            _RotateLowerYBound = __DefaultRotateLowerBound;
        }

        #endregion

        #region <Fields>

        private float _RotateUpperYBound;
        private float _RotateLowerYBound;
        
        #endregion
        
        #region <Methods>

        /// <summary>
        /// 회전을 수행하면, 회전구의 천정과 천저에서 각각 극방향으로 진행할 시, 방향이 바뀌면서 끈임없이
        /// 극방향으로 향하려고 회전방향이 바뀌는 루프에 빠지게 되는데 이를 방지하려면
        /// 극점에 도달하지 못하게 회전의 상한과 하한을 지정해야한다.
        /// </summary>
        public void SetBounds(float p_RotateUpperBound, float p_RotateLowerBound)
        {
            _RotateUpperYBound = -Mathf.Abs(p_RotateUpperBound);
            _RotateLowerYBound = Mathf.Abs(p_RotateLowerBound);
        }

        /// <summary>
        /// 벡터 파라미터는 월드 벡터 회전축, 실수 파라미터는 회전정도를 나타낸다.
        /// </summary>
        public override bool AddValue(Vector3 p_AdditiveValue, float p_DeltaTime)
        {
            if (!IsEventValid())
            {
                return SetValue(_CurrentValue.VectorRotationUsingQuaternion(p_AdditiveValue, p_DeltaTime));
            }
            else
            {
                return false;
            }
        }

        public override bool SetValue(Vector3 p_TargetValue)
        {
            var prevValue = _CurrentValue;
            _CurrentValue = p_TargetValue.normalized;
            
            if (_CurrentValue.y > _RotateLowerYBound || _CurrentValue.y < _RotateUpperYBound)
            {
                _CurrentValue = prevValue;
                return false;
            }
            else
            {
                _DeltaValue = _CurrentValue - prevValue;
                OnValueChanged?.Invoke();
                return true;
            }
        }

        public void SetValueBypassBound(Vector3 p_TargetValue)
        {
            var prevValue = _CurrentValue;
            _CurrentValue = p_TargetValue.normalized;
            _DeltaValue = _CurrentValue - prevValue;
            OnValueChanged?.Invoke();   
        }

        public override void RevertValueChange()
        {
            _CurrentValue -= _DeltaValue;
            _DeltaValue = default;
            OnValueChanged?.Invoke();
        }

        public override bool AddValueLerpTo(Vector3 p_AdditiveValue, uint p_PreDelayMsec, uint p_LerpDurationMsec)
        {
            return SetValueLerpTo(_CurrentValue + p_AdditiveValue, p_PreDelayMsec, p_LerpDurationMsec);
        }

        protected override bool SpawnLerpEvent(Vector3 p_TargetValue, uint p_PreDelayMsec, uint p_LerpDurationMsec, bool p_IsReset)
        {
            if ((_CurrentValue - p_TargetValue).IsReachedZero())
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
                        if (timerHandler.Arg4)
                        {
                            if (timerHandler.LerpTimer.IsOver())
                            {
                                valueHandler.SetValueBypassBound(timerHandler.Arg2);
                                valueHandler.OnValueLerpTerminate(true);
                                return false;
                            }
                            else
                            {
                                var progressRate = timerHandler.LerpTimer.ProgressRate;
                                var slerpValue = Vector3.Slerp(timerHandler.Arg1, timerHandler.Arg2, progressRate);
                                valueHandler.SetValueBypassBound(slerpValue);
                                return true;
                            }
                        }
                        else
                        {
                            if (timerHandler.LerpTimer.IsOver())
                            {
                                valueHandler.SetValue(timerHandler.Arg2);
                                valueHandler.OnValueLerpTerminate(false);
                                return false;
                            }
                            else
                            {
                                var progressRate = timerHandler.LerpTimer.ProgressRate;
                                var slerpValue = Vector3.Slerp(timerHandler.Arg1, timerHandler.Arg2, progressRate);
                                valueHandler.SetValue(slerpValue);
                                return true;
                            }   
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