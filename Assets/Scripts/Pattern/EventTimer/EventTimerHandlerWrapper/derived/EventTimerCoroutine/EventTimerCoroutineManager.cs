using Cysharp.Threading.Tasks;

namespace BlackAm
{
    /// <summary>
    /// 코루틴 인스턴스를 관리하는 매니저 클래스
    /// 최초에 오브젝트 풀링을 제외하고는 힙할당이 없다.(캡쳐가 있는 대리자를 등록하는 경우에는 힙이 발생한다.)
    /// </summary>
    public class EventTimerCoroutineManager : SceneChangeEventSingleton<EventTimerCoroutineManager>
    {
        #region <Fields>

        /// <summary>
        /// 씬과 수명을 같이하는 코루틴 풀러
        /// </summary>
        private ObjectPooler<EventTimerCoroutine> _GameEventTimerCoroutinePooler;
        
        /// <summary>
        /// 게임과 수명을 같이하는 코루틴 풀러
        /// </summary>
        private ObjectPooler<EventTimerCoroutine> _SystemEventTimerCoroutinePooler;

        #endregion
        
        #region <Callbacks>

        protected override void OnCreated()
        {
            _GameEventTimerCoroutinePooler = new ObjectPooler<EventTimerCoroutine>();
            _GameEventTimerCoroutinePooler.PreloadPool(4, 4);
            
            _SystemEventTimerCoroutinePooler = new ObjectPooler<EventTimerCoroutine>();
            _SystemEventTimerCoroutinePooler.PreloadPool(4, 4);
        }

        public override void OnInitiate()
        {
        }

        public override async UniTask OnScenePreload()
        {
            await UniTask.CompletedTask;
        }

        public override void OnSceneStarted()
        {
        }

        public override void OnSceneTransition()
        {
        }

        /// <summary>
        /// 시스템 이벤트 코루틴은 회수하지 않는다.
        /// </summary>
        public override void OnSceneTerminated()
        {
            _GameEventTimerCoroutinePooler.RetrieveAllObject();
        }

        #endregion

        #region <Methods>

        /// <summary>
        /// 작업 타입에 따른 코루틴을 풀링하는 메서드
        /// </summary>
        public EventTimerCoroutine GetCoroutine(SystemBoot.TimerType p_TimerType, bool p_WholeLifeCycleTimer)
        {
            var tryTimer = SystemBoot.GetInstance[p_TimerType];
            if (p_WholeLifeCycleTimer)
            {
                var result = _SystemEventTimerCoroutinePooler.GetObject();
                result.SetEventTimer(tryTimer);
                return result;
            }
            else
            {
                var result = _GameEventTimerCoroutinePooler.GetObject();
                result.SetEventTimer(tryTimer);
                return result;          
            }
        }

        #endregion
    }
}