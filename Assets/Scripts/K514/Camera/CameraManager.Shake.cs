#if !SERVER_DRIVE
using UnityEngine;

namespace BlackAm
{
    /// <summary>
    /// 카메라를 흔드는 연출을 담당하는 부분 클래스
    /// 해당 이벤트는 벡터나 부동소수점의 러프 함수를 사용하지 않기에 다른 부분 클래스 처럼 외부 클래스를 통해
    /// 이벤트를 수행하지 않는다.
    /// </summary>
    public partial class CameraManager
    {
        #region <Fields>

        /// <summary>
        /// Wrapper Transform Collection ShortHand
        /// </summary>
        private Transform ShakeWrapper;
        
        /// <summary>
        /// 카메라 흔드는 연출 이벤트 종료를 CameraEventTimer로부터 수신받는 이벤트 리시버
        /// </summary>
        private SafeReference<object, GameEventTimerHandlerWrapper> _ShakeCameraEventReceiver;
        
        #endregion

        #region <Callbacks>

        private void OnCreateShakeWrapperPartial()
        {
        }

        private void OnResetShakeWrapperPartial()
        {
            EventTimerTool.ReleaseEventHandler(ref _ShakeCameraEventReceiver);
        }

        #endregion

        #region <Methods>

        /// <summary>
        /// 속도가 일정한 카메라를 흔드는 연출을 수행하는 메서드
        /// </summary>
        public void SetShake(Vector3 p_Direction, float p_MaxDistance, uint p_PreDelayMsec, uint p_DurationMsec, int p_CycleCount)
        {
            if (!IsCameraValid) return;

            var invCycleCount = 1f / p_CycleCount;
            CameraShakePreset _CameraShakePreset = new CameraShakePreset
            {
                Direction = CustomMath.GetRandomSign() * p_Direction,
                Distance = p_MaxDistance,
                PingPongBound = 0.25f * invCycleCount,
                PingPongBoundHalves = 0.125f * invCycleCount
            };

            GameEventTimerHandlerManager.GetInstance.SpawnSafeEventTimerHandler(ref _ShakeCameraEventReceiver, this, SystemBoot.TimerType.LateGameTimer, false);
            var (_, eventHandler) = _ShakeCameraEventReceiver.GetValue();
            eventHandler
                .AddEvent(
                    (p_PreDelayMsec, p_DurationMsec, EventTimerTool.EventTimerIntervalType.Lerp),
                    (timerHandler) =>
                    {
                        var shakePreset = timerHandler.Arg2;
                        var progressRate = timerHandler.LerpTimer.ProgressRate;
                        var pingpongHalfBound = shakePreset.PingPongBoundHalves;
                        var pingpongRate =
                            Mathf.PingPong(progressRate + pingpongHalfBound, shakePreset.PingPongBound) -
                            pingpongHalfBound;
                        var currentDistance = shakePreset.Distance * pingpongRate;
                        if (timerHandler.LerpTimer.IsOver())
                        {
                            timerHandler.Arg1.localPosition = Vector3.zero;
                            return false;
                        }
                        else
                        {
                            timerHandler.Arg1.localPosition = currentDistance * shakePreset.Direction;
                            return true;
                        }
                    },
                    null,
                    ShakeWrapper,
                    _CameraShakePreset
                );
            eventHandler.StartEvent();
        }

        /// <summary>
        /// 속도가 일정한 카메라를 흔드는 연출을 수행하는 메서드
        /// </summary>
        public void SetShake(int p_TableDataIndex, uint p_PreDelayMsec)
        {
            if (!IsCameraValid || p_TableDataIndex == default) return;

            var targetRecord = CameraShakeDataNonAccel.GetInstanceUnSafe.GetTableData(p_TableDataIndex);
            CameraShakePreset _CameraShakePreset = new CameraShakePreset
            {
                Direction = CustomMath.GetRandomSign() * targetRecord.Direction,
                Distance = targetRecord.Distance,
                PingPongBound = 0.25f * targetRecord.InvCycleCount,
                PingPongBoundHalves = 0.125f * targetRecord.InvCycleCount
            };
            
            GameEventTimerHandlerManager.GetInstance.SpawnSafeEventTimerHandler(ref _ShakeCameraEventReceiver, this, SystemBoot.TimerType.LateGameTimer, false);
            var (_, eventHandler) = _ShakeCameraEventReceiver.GetValue();
            eventHandler
                .AddEvent(
                    (p_PreDelayMsec, targetRecord.DurationMsec, EventTimerTool.EventTimerIntervalType.Lerp),
                    (timerHandler) =>
                    {
                        var shakePreset = timerHandler.Arg2;
                        var progressRate = timerHandler.LerpTimer.ProgressRate;
                        var pingpongHalfBound = shakePreset.PingPongBoundHalves;
                        var pingpongRate = Mathf.PingPong(progressRate + pingpongHalfBound, shakePreset.PingPongBound) - pingpongHalfBound;
                        var currentDistance = shakePreset.Distance * pingpongRate;
                        if (timerHandler.LerpTimer.IsOver())
                        {
                            timerHandler.Arg1.localPosition = Vector3.zero;
                            return false;
                        }
                        else
                        {
                            timerHandler.Arg1.localPosition = currentDistance * shakePreset.Direction;
                            return true;
                        }
                    },
                    null,
                    ShakeWrapper, 
                    _CameraShakePreset
                );
            eventHandler.StartEvent();
        }
        
        /// <summary>
        /// 카메라를 흔드는 연출을 수행하는 메서드, 전용 테이블에 등록된 값으로 연출을 수행하며, 일반 버전과 달리
        /// 흔드는 속도를 가속한다.
        /// </summary>
        public void SetAccelShake(int p_TableDataIndex, uint p_PreDelayMsec, uint p_DurationMsec)
        {
            if (!IsCameraValid || p_TableDataIndex == default) return;
            
            var targetRecord = CameraShakeData.GetInstanceUnSafe.GetTableData(p_TableDataIndex);
            CameraShakePreset _CameraShakePreset = new CameraShakePreset
            {
                Direction = Vector3.zero,
            };
            
            GameEventTimerHandlerManager.GetInstance.SpawnSafeEventTimerHandler(ref _ShakeCameraEventReceiver, this, SystemBoot.TimerType.LateGameTimer, false);
            var (_, eventHandler) = _ShakeCameraEventReceiver.GetValue();
            eventHandler
                .AddEvent(
                    (p_PreDelayMsec, p_DurationMsec, EventTimerTool.EventTimerIntervalType.Lerp),
                    (timerHandler) =>
                    {
                        var _CurrentShakeSpeed = timerHandler.Arg2.Direction;
                        var _TableRecord = timerHandler.Arg3;
                            
                        _CurrentShakeSpeed += timerHandler.LatestDeltaTime * _TableRecord.Acceleration;
                        var swingBound = _TableRecord.SwingBoundVector;

                        var tryXSpeed = _CurrentShakeSpeed.x;
                        var tryXBound = swingBound.x;
                        while (Mathf.Abs(tryXSpeed) > tryXBound)
                        {
                            if (tryXSpeed > 0)
                            {
                                tryXSpeed = tryXBound * 2 - tryXSpeed;
                            }
                            else
                            {
                                tryXSpeed = -tryXBound * 2 - tryXSpeed;
                            }
                            tryXSpeed = -tryXSpeed;
                        }

                        var tryYSpeed = _CurrentShakeSpeed.y;
                        var tryYBound = swingBound.y;
                        while (Mathf.Abs(tryYSpeed) > tryYBound)
                        {
                            if (tryYSpeed > 0)
                            {
                                tryYSpeed = tryYBound * 2 - tryYSpeed;
                            }
                            else
                            {
                                tryYSpeed = -tryYBound * 2 - tryYSpeed;
                            }
                            tryYSpeed = -tryYSpeed;
                        }
                
                        var tryZSpeed = _CurrentShakeSpeed.z;
                        var tryZBound = swingBound.z;
                        while (Mathf.Abs(tryZSpeed) > tryZBound)
                        {
                            if (tryZSpeed > 0)
                            {
                                tryZSpeed = tryZBound * 2 - tryZSpeed;
                            }
                            else
                            {
                                tryZSpeed = -tryZBound * 2 - tryZSpeed;
                            }
                            tryZSpeed = -tryZSpeed;
                        }
                
                        timerHandler.Arg2.Direction = new Vector3(tryXSpeed, tryYSpeed, tryZSpeed);
                        if (timerHandler.LerpTimer.IsOver())
                        {
                            timerHandler.Arg1.localPosition = Vector3.zero;
                            return false;
                        }
                        else
                        {
                            timerHandler.Arg1.localPosition = timerHandler.Arg2.Direction;
                            return true;
                        }
                    },
                    null, 
                    ShakeWrapper, 
                    _CameraShakePreset,
                    targetRecord
                );
            eventHandler.StartEvent();
        }
        
        #endregion

        #region <Structs>

        private struct CameraShakePreset
        {
            public Vector3 Direction;
            public float Distance;
            public float PingPongBound;
            public float PingPongBoundHalves;
        }

        #endregion
    }
}
#endif