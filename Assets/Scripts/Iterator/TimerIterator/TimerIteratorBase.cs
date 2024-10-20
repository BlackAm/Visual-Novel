using System;

namespace BlackAm
{
    /// <summary>
    /// 일정 시간을 주기로, 정해진 횟수만큼
    /// 특정 자료 컬렉션을 탐색하거나 특정 변수 값을 변경하는 작업을 하는
    /// 데이터 순환자 기저 클래스
    /// </summary>
    public abstract class TimerIteratorBase : _IDisposable
    {
        ~TimerIteratorBase()
        {
            Dispose();
        }

        #region <Fields>

        /// <summary>
        /// 러프 시간 구조체
        /// </summary>
        public ProgressTimer _IteratorTimer;

        /// <summary>
        /// 반복자 작업 플래그 마스크
        /// </summary>
        protected IteratorProgressFlag ProgressFlagMask;
        
        /// <summary>
        /// 재생 스피드
        /// </summary>
        protected float _PlaySpeed;
        
        /// <summary>
        /// 애니메이션 루프 횟수
        /// </summary>
        protected (int t_Cur, int t_Max) _LoopCount;

        /// <summary>
        /// 하한 도달 루프 간격
        /// </summary>
        protected ProgressTimer _LowerBoundTimer;
        
        /// <summary>
        /// 상한 도달 루프 간격
        /// </summary>
        protected ProgressTimer _UpperBoundTimer;

        /// <summary>
        /// 하한 도달 루프 간격 랜덤구간
        /// </summary>
        protected (float, float) _LowerBoundRandomIntervalSeed;
        
        /// <summary>
        /// 상한 도달 루프 간격 랜덤구간
        /// </summary>
        protected (float, float) _UpperBoundRandomIntervalSeed;
        
        /// <summary>
        /// 현재 PingPong 상태
        /// </summary>
        protected PingPongPhase _PingPongPhase;
        
        #endregion

        #region <Enums>

        /// <summary>
        /// 반복자 작업 플래그
        /// </summary>
        [Flags]
        public enum IteratorProgressFlag
        {
            None = 0,

            /// <summary>
            /// 반복자가 pingpong 동작을 수행함
            /// </summary>
            PingPong = 1 << 0,
            
            /// <summary>
            /// 루프 간격을 가짐
            /// </summary>
            LoopInterval = 1 << 1,
            
            /// <summary>
            /// 루프 횟수와 관계 없이 무한히 루프함
            /// </summary>
            LoopInfinity = 1 << 2,
            
            /// <summary>
            /// 루프 간격 종료시, 해당 간격을 랜덤하게 변동시키는 플래그
            /// </summary>
            LoopIntervalRandomize = 1 << 3,
        }

        /// <summary>
        /// 반복자 작업 결과 타입
        /// </summary>
        public enum IterateResultType
        {
            /// <summary>
            /// 막간 진행 중
            /// </summary>
            Progressing,
            
            /// <summary>
            /// 막간 경과 및 다음 인덱스로 전환
            /// </summary>
            ProgressNext,
            
            /// <summary>
            /// 모든 인덱스 순회 완료
            /// </summary>
            ProgressOver,
            
            /// <summary>
            /// 막간이 경과해서 다음 인덱스로 전환했으나 인덱스 숫자가 적거나 해서 인덱스가 바뀌지 않음
            /// </summary>
            ProgressNextButNotChange,
            
            /// <summary>
            /// 모든 인덱스를 순회 완료했으나 인덱스 숫자가 적거나 해서 인덱스가 바뀌지 않음
            /// </summary>
            ProgressOverButNotChange,
        }

        /// <summary>
        /// pingpong 페이즈
        /// </summary>
        protected enum PingPongPhase
        {
            /// <summary>
            /// 기본 순환 페이즈, 인덱스를 올린다.
            /// </summary>
            NormalPhase,
            
            /// <summary>
            /// 인덱스 상한 도달
            /// </summary>
            UpperBound,
            
            /// <summary>
            /// 역방향 순환 페이즈, 인덱스를 낮춘다.
            /// </summary>
            ReversePhase,
            
            /// <summary>
            /// 인덱스 하한 도달
            /// </summary>
            LowerBound,
        }

        #endregion

        #region <Constructor>

        protected TimerIteratorBase()
        {
        }

        public TimerIteratorBase(float p_Interval, int p_LoopCount)
        {
            _IteratorTimer.Initialize(p_Interval);

            if (p_LoopCount < 1)
            {
                ProgressFlagMask.AddFlag(IteratorProgressFlag.LoopInfinity);
            }
            else
            {
                _LoopCount = (0, p_LoopCount);
            }

            SetPlaySpeed(1f);
        }

        #endregion

        #region <Callbacks>

        /// <summary>
        /// 지정한 시간만큼 순환자를 진행시키는 기저 메서드
        /// </summary>
        public abstract IterateResultType ProgressIterating(float p_DeltaTime);

        #endregion

        #region <Methods>

        public virtual void Reset()
        {
            _IteratorTimer.Reset();
            _LoopCount.t_Cur = 0;
            _PingPongPhase = PingPongPhase.NormalPhase;
        }

        public void SetPlaySpeed(float p_PlaySpeed)
        {
            _PlaySpeed = p_PlaySpeed;
        }

        public void SetPingPong(bool p_Flag)
        {
            if (p_Flag)
            {
                ProgressFlagMask.AddFlag(IteratorProgressFlag.PingPong);
            }
            else
            {
                ProgressFlagMask.RemoveFlag(IteratorProgressFlag.PingPong);
            }
        }

        public void SetLoopInterval(bool p_Flag, float p_UpperBoundInterval, float p_LowerBoundInterval)
        {
            if (p_Flag)
            {
                ProgressFlagMask.AddFlag(IteratorProgressFlag.LoopInterval);
                
                _UpperBoundTimer = p_UpperBoundInterval;
                _LowerBoundTimer = p_LowerBoundInterval;
            }
            else
            {
                ProgressFlagMask.RemoveFlag(IteratorProgressFlag.LoopInterval);
            }
        }
        
        public void SetLoopIntervalRandomize(bool p_Flag, (float t_RandLower, float t_RandUpper) p_UpperBoundInterval, (float t_RandLower, float t_RandUpper) p_LowerBoundInterval)
        {
            if (p_Flag)
            {
                ProgressFlagMask.AddFlag(IteratorProgressFlag.LoopInterval | IteratorProgressFlag.LoopIntervalRandomize);
                
                _UpperBoundRandomIntervalSeed = p_UpperBoundInterval;
                _UpperBoundTimer = _UpperBoundRandomIntervalSeed.GetRandom();
                _LowerBoundRandomIntervalSeed = p_LowerBoundInterval;
                _LowerBoundTimer = _LowerBoundRandomIntervalSeed.GetRandom();
            }
            else
            {
                ProgressFlagMask.RemoveFlag(IteratorProgressFlag.LoopIntervalRandomize);
            }
        }

        public void SetLoopCount(int p_LoopCount)
        {
            if (p_LoopCount < 1)
            {
                ProgressFlagMask.AddFlag(IteratorProgressFlag.LoopInfinity);
            }
            else
            {
                ProgressFlagMask.RemoveFlag(IteratorProgressFlag.LoopInfinity);
                _LoopCount = (_LoopCount.t_Cur, p_LoopCount);
            }
        }

        public bool CheckLoopOver()
        {
            if (ProgressFlagMask.HasAnyFlagExceptNone(IteratorProgressFlag.LoopInfinity))
            {
                return false;
            }
            else
            {
                _LoopCount.t_Cur++;
                return _LoopCount.t_Max <= _LoopCount.t_Cur;
            }
        }

        public bool IsLoopOver()
        {
            return !ProgressFlagMask.HasAnyFlagExceptNone(IteratorProgressFlag.LoopInfinity)
                && _LoopCount.t_Max <= _LoopCount.t_Cur;
        }

        #endregion
        
        #region <Disposable>
        
        /// <summary>
        /// dispose 패턴 onceFlag
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// dispose 플래그를 초기화 시키는 메서드
        /// </summary>
        public void Rejunvenate()
        {
            IsDisposed = false;
        }
        
        /// <summary>
        /// 인스턴스 파기 메서드
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }
            else
            {
                IsDisposed = true;
                DisposeUnManaged();
            }
        }

        /// <summary>
        /// 인스턴스가 파기될 때 수행할 작업을 기술한다.
        /// </summary>
        protected abstract void DisposeUnManaged();

        #endregion
    }
}