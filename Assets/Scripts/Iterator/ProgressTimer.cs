using System.Collections.Generic;
using UnityEngine;

namespace BlackAm
{
    /// <summary>
    /// 특정한 시구간의 시간 경과율을 0~1 값으로 가지는 구조체
    /// </summary>
    public struct ProgressTimer
    {
        #region <Consts>

        public static ProgressTimer GetProgressTimer(float p_WholTime)
        {
            ProgressTimer result = default;
            result.Initialize(p_WholTime);
            return result;
        }

        public static implicit operator ProgressTimer(float p_WholTime)
        {
            return GetProgressTimer(p_WholTime);
        }

        #endregion

        #region <Fields>

        public float WholeTime { get; private set; }
        public float ElapsedTime { get; private set; }
        public float RemaindTime => WholeTime - ElapsedTime;
        public float ReversedWholTime { get; private set; }

        #endregion

        #region <Properties>

        public float ProgressRate => Mathf.Clamp01(ElapsedTime * ReversedWholTime);
        public void Progress(float p_Dt) => ElapsedTime += p_Dt;
        public void ProgressClamped(float p_Dt) => ElapsedTime = Mathf.Clamp(ElapsedTime + p_Dt, 0f, WholeTime);

        #endregion

        #region <Operator>

#if UNITY_EDITOR
        public override string ToString()
        {
            return $"[{ElapsedTime} / {WholeTime}]({ProgressRate}%)";
        }
#endif

        #endregion
        
        #region <Methods>

        public void Reset()
        {
            ElapsedTime = 0f;
        }

        public void Terminate()
        {
            ElapsedTime = WholeTime;
        }

        public void Initialize(float p_WholTime)
        {
            WholeTime = Mathf.Max(CustomMath.Epsilon, p_WholTime);
            ReversedWholTime = 1f / WholeTime;
            Reset();
        }
        
        public bool IsOver() => ProgressRate.IsReachedOne();

        public bool IsZero() => ProgressRate.IsReachedZero();

        #endregion
    }

    /// <summary>
    /// 구조체인 ProgressTimer를 감싸는 래핑 클래스
    /// </summary>
    public class ProgressTimerWrap
    {
        #region <Fields>

        private ProgressTimer _ProgressTimer;
        private bool _IsValid;
        
        #endregion

        #region <Constructors>

        public ProgressTimerWrap(float p_WholeTime)
        {
            RespawnTimer(p_WholeTime, false);
        }

        #endregion

        #region <Methods>

        public void Progress(float p_DeltaTime)
        {
            if (_IsValid)
            {
                _ProgressTimer.Progress(p_DeltaTime);
            }
        }

        public float GetProgressRate() => _ProgressTimer.ProgressRate;

        public bool IsOver()
        {
            return _ProgressTimer.IsOver();
        }

        public void RespawnTimer(float p_WholeTime, bool p_RemainElapsedTime)
        {
            _IsValid = true;
            if (p_RemainElapsedTime)
            {
                var curElapsedTime = _ProgressTimer.ElapsedTime;
                _ProgressTimer.Initialize(p_WholeTime);
                _ProgressTimer.Progress(curElapsedTime);
            }
            else
            {
                _ProgressTimer.Initialize(p_WholeTime);
            }
        }

        public bool CheckValid()
        {
            var result = _IsValid && IsOver();
            if (result)
            {
                _IsValid = false;
            }
            return result;
        }

        #endregion
    }

    /// <summary>
    /// 다수의 ProgressTimer를 순차적으로 진행시키는 클래스
    /// </summary>
    public class ProgressTimerChainObject
    {
        #region <Fields>

        private List<ProgressTimer> _ProgressTimerGroup;
        private int _CurrentIndex;
        private TaskPhase _CurrentState;
        private ProgressTimerChainTerminateType _TerminateType;
        
        #endregion

        #region <Enums>

        public enum ProgressTimerChainTerminateType
        {
            PingPong,
            Loop,
            OnceWayClear,
        }

        #endregion

        #region <Constructors>

        public ProgressTimerChainObject(ProgressTimerChainTerminateType p_TerminateType)
        {
            _ProgressTimerGroup = new List<ProgressTimer>();
            _TerminateType = p_TerminateType;
            Init();
        }

        #endregion

        #region <Methods>

        private void Init()
        {
            _CurrentIndex = -1;
            SetState(TaskPhase.None);
        }

        private TaskPhase SetState(TaskPhase p_Type)
        {
            _CurrentState = p_Type;
            return _CurrentState;
        }

        public void AddTimer(float p_Duration)
        {
            if (_CurrentIndex < 0)
            {
                _CurrentIndex = 0;
            }
            _ProgressTimerGroup.Add(p_Duration);
        }

        public void ClearTimer()
        {
            if (_CurrentIndex > -1)
            {
                _ProgressTimerGroup.Clear();
                Init();
            }
        }

        public TaskPhase Progress(float p_DeltaTime, out float o_Rate01)
        {
            if (_CurrentIndex > -1)
            {
                var targetTimer = _ProgressTimerGroup[_CurrentIndex];
                targetTimer.Progress(p_DeltaTime);
                _ProgressTimerGroup[_CurrentIndex] = targetTimer;
                
                if (targetTimer.IsOver())
                {
                    _CurrentIndex++;

                    var groupCount = _ProgressTimerGroup.Count;
                    if (_CurrentIndex == groupCount)
                    {
                        switch (_TerminateType)
                        {
                            case ProgressTimerChainTerminateType.PingPong:
                                _ProgressTimerGroup.Reverse();
                                for (int i = 0; i < groupCount; i++)
                                {
                                    var tryTimer = _ProgressTimerGroup[i];
                                    tryTimer.Reset();
                                    _ProgressTimerGroup[i] = tryTimer;
                                }
                                _CurrentIndex = groupCount == 1 ? 0 : 1;
                                break;
                            case ProgressTimerChainTerminateType.Loop:
                                for (int i = 0; i < groupCount; i++)
                                {
                                    var tryTimer = _ProgressTimerGroup[i];
                                    tryTimer.Reset();
                                    _ProgressTimerGroup[i] = tryTimer;       
                                }
                                _CurrentIndex = 0;
                                break;
                            case ProgressTimerChainTerminateType.OnceWayClear:
                                ClearTimer();
                                break;
                        }
                    }

                    o_Rate01 = 1f;
                    return SetState(TaskPhase.TaskTerminate);
                }
                else
                {
                    o_Rate01 = targetTimer.ProgressRate;
                    return SetState(TaskPhase.TaskProgressing);
                }
            }
            else
            {
                o_Rate01 = 0f;
                return SetState(TaskPhase.None);
            }
        }
        
                public TaskPhase Progress(float p_DeltaTime)
        {
            if (_CurrentIndex > -1)
            {
                var targetTimer = _ProgressTimerGroup[_CurrentIndex];
                targetTimer.Progress(p_DeltaTime);
                _ProgressTimerGroup[_CurrentIndex] = targetTimer;
                
                if (targetTimer.IsOver())
                {
                    _CurrentIndex++;

                    var groupCount = _ProgressTimerGroup.Count;
                    if (_CurrentIndex == groupCount)
                    {
                        switch (_TerminateType)
                        {
                            case ProgressTimerChainTerminateType.PingPong:
                                _ProgressTimerGroup.Reverse();
                                for (int i = 0; i < groupCount; i++)
                                {
                                    var tryTimer = _ProgressTimerGroup[i];
                                    tryTimer.Reset();
                                    _ProgressTimerGroup[i] = tryTimer;
                                }
                                _CurrentIndex = groupCount == 1 ? 0 : 1;
                                break;
                            case ProgressTimerChainTerminateType.Loop:
                                for (int i = 0; i < groupCount; i++)
                                {
                                    var tryTimer = _ProgressTimerGroup[i];
                                    tryTimer.Reset();
                                    _ProgressTimerGroup[i] = tryTimer;       
                                }
                                _CurrentIndex = 0;
                                break;
                            case ProgressTimerChainTerminateType.OnceWayClear:
                                ClearTimer();
                                break;
                        }
                    }

                    return SetState(TaskPhase.TaskTerminate);
                }
                else
                {
                    return SetState(TaskPhase.TaskProgressing);
                }
            }
            else
            {
                return SetState(TaskPhase.None);
            }
        }

        #endregion
    }
}