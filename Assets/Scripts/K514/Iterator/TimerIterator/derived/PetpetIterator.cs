using UnityEngine;
using Random = UnityEngine.Random;

namespace BlackAm
{
    /// <summary>
    /// 일정 주기로 특정 실수값의 구간 [하한, 상한] 사이의 값을 일정 주기로 순환하는 데이터 순환자 클래스
    /// </summary>
    public class PetpetIterator : TimerIteratorBase
    {
        #region <Fields>

        /// <summary>
        /// 실수 하한
        /// </summary>
        private float _MinScale;
        
        /// <summary>
        /// 실수 상한
        /// </summary>
        private float _MaxScale;

        /// <summary>
        /// 스케일 값
        /// </summary>
        private float _CurrentScale;
        
        #endregion
        
        #region <Constructors>

        public PetpetIterator(float p_MinScale, float p_MaxScale, float p_Interval = 1f, int p_LoopCount = 0) : base(p_Interval, p_LoopCount)
        {
            _MinScale = p_MinScale;
            _MaxScale = p_MaxScale;
            
            SetPlaySpeed(1f);
            SetPingPong(true);
        }

        #endregion

        #region <Callbacks>

        /// <summary>
        /// 지정한 시간만큼 순환자를 진행시키는 메서드
        /// </summary>
        public override IterateResultType ProgressIterating(float p_DeltaTime)
        {
            if (IsLoopOver())
            {
                return IterateResultType.ProgressOverButNotChange;
            }
            else
            {
                switch (_PingPongPhase)
                {
                    default:
                    case PingPongPhase.NormalPhase:
                    {
                        if (_IteratorTimer.IsOver())
                        {
                            _IteratorTimer.Reset();
                            _CurrentScale = _MaxScale;

                            if (ProgressFlagMask.HasAnyFlagExceptNone(IteratorProgressFlag.LoopInterval))
                            {
                                _PingPongPhase = PingPongPhase.UpperBound;
                            }
                            else if (ProgressFlagMask.HasAnyFlagExceptNone(IteratorProgressFlag.PingPong))
                            {
                                _PingPongPhase = PingPongPhase.ReversePhase;
                            }

                            return CheckLoopOver() ? IterateResultType.ProgressOver : IterateResultType.ProgressNext;
                        }
                        else
                        {
                            _IteratorTimer.Progress(p_DeltaTime * _PlaySpeed);
                            _CurrentScale = Mathf.Lerp(_MinScale, _MaxScale, _IteratorTimer.ProgressRate);
                            
                            return IterateResultType.Progressing;
                        }
                    }
                    case PingPongPhase.UpperBound:
                    {
                        if (_UpperBoundTimer.IsOver())
                        {
                            if (ProgressFlagMask.HasAnyFlagExceptNone(IteratorProgressFlag.LoopIntervalRandomize))
                            {
                                _UpperBoundTimer = _UpperBoundRandomIntervalSeed.GetRandom();
                            }
                            else
                            {
                                _UpperBoundTimer.Reset();
                            }

                            if (ProgressFlagMask.HasAnyFlagExceptNone(IteratorProgressFlag.PingPong))
                            {
                                _PingPongPhase = PingPongPhase.ReversePhase;
                            }
                            else
                            {
                                _PingPongPhase = PingPongPhase.NormalPhase;
                            }
                        }
                        else
                        {
                            _UpperBoundTimer.Progress(p_DeltaTime);
                        }
                        
                        return IterateResultType.Progressing;
                    }
                    case PingPongPhase.ReversePhase:
                    {
                        if (_IteratorTimer.IsOver())
                        {
                            _IteratorTimer.Reset();
                            _CurrentScale = _MinScale;

                            if (ProgressFlagMask.HasAnyFlagExceptNone(IteratorProgressFlag.LoopInterval))
                            {
                                _PingPongPhase = PingPongPhase.LowerBound;
                            }
                            else
                            {
                                if (ProgressFlagMask.HasAnyFlagExceptNone(IteratorProgressFlag.PingPong))
                                {
                                    _PingPongPhase = PingPongPhase.NormalPhase;
                                }
                            }

                            return CheckLoopOver() ? IterateResultType.ProgressOver : IterateResultType.ProgressNext;
                        }
                        else
                        {
                            _IteratorTimer.Progress(p_DeltaTime * _PlaySpeed);
                            _CurrentScale = Mathf.Lerp(_MaxScale, _MinScale, _IteratorTimer.ProgressRate);
                            
                            return IterateResultType.Progressing;
                        }
                    }
                    case PingPongPhase.LowerBound:
                        if (_LowerBoundTimer.IsOver())
                        {
                            if (ProgressFlagMask.HasAnyFlagExceptNone(IteratorProgressFlag.LoopIntervalRandomize))
                            {
                                _LowerBoundTimer = _LowerBoundRandomIntervalSeed.GetRandom();
                            }
                            else
                            {
                                _LowerBoundTimer.Reset();
                            }
                            
                            _PingPongPhase = PingPongPhase.NormalPhase;
                        }
                        else
                        {
                            _LowerBoundTimer.Progress(p_DeltaTime);
                        }
                        
                        return IterateResultType.Progressing;
                }
            }
        }

        #endregion
        
        #region <Methods>

        public override void Reset()
        {
            base.Reset();
            
            _CurrentScale = _MinScale;
        }

        public float GetCurrentScale()
        {
            return _CurrentScale;
        }

        #endregion

        #region <Disposable>

        /// <summary>
        /// 인스턴스가 파기될 때 수행할 작업을 기술한다.
        /// </summary>
        protected override void DisposeUnManaged()
        {
        }

        #endregion
    }
}