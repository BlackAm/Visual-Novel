using UnityEngine;

namespace k514
{
    public class TimeManager : Singleton<TimeManager>
    {
        #region <Consts>
        
        /// <summary>
        /// 기본 FPS
        /// </summary>
        public const int __Default_GAME_FRAME_PER_SECOND = 60;

        /// <summary>
        /// 고정 갱신 함수 주기
        /// </summary>
        public const float __Default_FIXED_TIMESTEP = 0.02f;

        /// <summary>
        /// 고정 갱신 함수 주기 밀리세컨드 값
        /// </summary>
        public static uint __FIXED_TIMESTEP_MSEC { get; private set; }
        
        #endregion
        
        #region <Fields>

        #endregion

        #region <Callbacks>

        protected override void OnCreated()
        {
        }

        public override void OnInitiate()
        {
            SetFPS();
            SetFixedTimeStep();
            SetTimeScale();
        }
        
        #endregion

        #region <Methods>

        public void SetFPS(int p_FPS = __Default_GAME_FRAME_PER_SECOND)
        {
            Application.targetFrameRate = p_FPS;
        }

        public void SetFixedTimeStep(float p_TimeStep = __Default_FIXED_TIMESTEP)
        {
            Time.fixedDeltaTime = p_TimeStep;
            __FIXED_TIMESTEP_MSEC = (uint) (1000 * p_TimeStep);
        }
        
        /// <summary>
        /// 시간 가속율을 지정하는 메서드
        ///
        /// todo<414k> : 타이머에 예약된 이벤트들은 이미 큐에 타임스탬프가 예약된 상황이므로
        /// 타이머 큐 자체가 갱신되는 속도를 해당 메서드와 동기시켜 조정하는 것으로 [전체 이벤트를 가속]시킬 수 있음.
        /// </summary>
        public void SetTimeScale(float p_TimeScale = 1f)
        {
            Time.timeScale = p_TimeScale;
        }
        
        #endregion
    }
}