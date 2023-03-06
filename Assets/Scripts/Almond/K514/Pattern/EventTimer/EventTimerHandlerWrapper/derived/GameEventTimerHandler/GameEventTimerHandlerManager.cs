using Cysharp.Threading.Tasks;

namespace k514
{
    /// <summary>
    /// GameEventTimerHandler의 생성 및 수명관리를 담당하는 매니저 클래스
    /// </summary>
    public class GameEventTimerHandlerManager : SceneChangeEventSingleton<GameEventTimerHandlerManager>
    {
        #region <Consts>

        private const int EventReceiverPreloadCount = 64;

        #endregion
        
        #region <Fields>

        /// <summary>
        /// 풀러
        /// </summary>
        private ObjectPooler<(SystemBoot.TimerType, ResourceLifeCycleType), GameEventTimerHandlerWrapper> _TerminateReceiverPooler;

        #endregion
        
        #region <Callbacks>

        protected override void OnCreated()
        {
            _TerminateReceiverPooler = new ObjectPooler<(SystemBoot.TimerType, ResourceLifeCycleType), GameEventTimerHandlerWrapper>();
            var enumerator = SystemTool.GetEnumEnumerator<SystemBoot.TimerType>(SystemTool.GetEnumeratorType.ExceptNone);
            foreach (var timerType in enumerator)
            {
                _TerminateReceiverPooler.Preload((timerType, ResourceLifeCycleType.WholeGame), EventReceiverPreloadCount);
                _TerminateReceiverPooler.Preload((timerType, ResourceLifeCycleType.Scene), EventReceiverPreloadCount);
            }
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

        /// <summary>
        /// 씬 종료 시에, 수명 타입이 씬인 이벤트 핸들러만 파기시킨다.
        /// </summary>
        public override void OnSceneTerminated()
        {
            var enumerator = SystemTool.GetEnumEnumerator<SystemBoot.TimerType>(SystemTool.GetEnumeratorType.ExceptNone);
            foreach (var timerType in enumerator)
            {
                _TerminateReceiverPooler.ClearPool((timerType, ResourceLifeCycleType.Scene));
            }
        }

        public override void OnSceneTransition()
        {
        }

        #endregion

        #region <Methods/SpawnTimer>

        /// <summary>
        /// 지정한 타이머 타입을 가지는 이벤트 수신자를 리턴하는 메서드
        /// 해당 수신자는 아직 이벤트 핸들러를 가지지 않기 때문에, AddEvent 등으로 이벤트를 추가하고
        /// StartEvent 등으로 이벤트 타이머에 등록시켜야 동작한다.
        /// </summary>
        public GameEventTimerHandlerWrapper SpawnEventTimerHandler(SystemBoot.TimerType p_TimerType, bool p_IsWholeLifeCycleTimer)
        {
            var lifeCycleType = p_IsWholeLifeCycleTimer ? ResourceLifeCycleType.WholeGame : ResourceLifeCycleType.Scene;
            var tryKey = (p_TimerType, lifeCycleType);
            return _TerminateReceiverPooler.GetObject(tryKey);
        }

        /// <summary>
        /// 지정한 타이머 타입을 가지는 이벤트 수신자를 리턴하는 메서드
        /// 해당 수신자는 아직 이벤트 핸들러를 가지지 않기 때문에, AddEvent 등으로 이벤트를 추가하고
        /// StartEvent 등으로 이벤트 타이머에 등록시켜야 동작한다.
        /// </summary>
        public SafeReference<object, GameEventTimerHandlerWrapper> SpawnSafeEventTimerHandler(object p_Key, SystemBoot.TimerType p_TimerType, bool p_IsWholeLifeCycleTimer)
        {
            return p_Key.GetSafeReference(SpawnEventTimerHandler(p_TimerType, p_IsWholeLifeCycleTimer));
        }
        
        #endregion

        #region <Methods/SpawnTimer/Ref>
        
        /// <summary>
        /// 지정한 타이머 타입을 가지는 이벤트 수신자를 참조 파라미터에 할당하는 메서드
        /// 참조 파라미터에 이미 할당되어 있는 오브젝트가 있는 경우, 유효성 검증도 같이 수행한다.
        ///
        /// 해당 수신자는 아직 이벤트 핸들러를 가지지 않기 때문에, AddEvent 등으로 이벤트를 추가하고
        /// StartEvent 등으로 이벤트 타이머에 등록시켜야 동작한다.
        /// </summary>
        public void SpawnEventTimerHandler(ref GameEventTimerHandlerWrapper r_SpawnedObject, SystemBoot.TimerType p_TimerType, bool p_IsWholeLifeCycleTimer)
        {
            var lifeCycleType = p_IsWholeLifeCycleTimer ? ResourceLifeCycleType.WholeGame : ResourceLifeCycleType.Scene;
            var tryKey = (p_TimerType, lifeCycleType);

            // 참조 파라미터를 검증하서 유효하지 않은 경우, 새로 풀링한다.
            if (ReferenceEquals(null, r_SpawnedObject) || r_SpawnedObject.PoolState != PoolState.Actived)
            {
                r_SpawnedObject = _TerminateReceiverPooler.GetObject(tryKey);
            }
            // 참조 파라미터를 검증하서 유효한 경우, 회수하고 새로 풀링한다.
            else
            {
                r_SpawnedObject.RetrieveObject();
                r_SpawnedObject = _TerminateReceiverPooler.GetObject(tryKey);
            }
        }
        
        /// <summary>
        /// 지정한 타이머 타입을 가지는 이벤트 수신자를 참조 파라미터에 할당하는 메서드
        /// 참조 파라미터에 이미 할당되어 있는 오브젝트가 있는 경우, 유효성 검증도 같이 수행한다.
        ///
        /// 해당 수신자는 아직 이벤트 핸들러를 가지지 않기 때문에, AddEvent 등으로 이벤트를 추가하고
        /// StartEvent 등으로 이벤트 타이머에 등록시켜야 동작한다.
        /// </summary>
        public void SpawnSafeEventTimerHandler(ref SafeReference<object, GameEventTimerHandlerWrapper> r_SpawnedObject, object p_Key, SystemBoot.TimerType p_TimerType, bool p_IsWholeLifeCycleTimer)
        {
            var (valid, value) = r_SpawnedObject.GetValue();
            if (valid)
            {
                SpawnEventTimerHandler(ref value, p_TimerType, p_IsWholeLifeCycleTimer);
                r_SpawnedObject = p_Key.GetSafeReference(value);
            }
            else
            {            
                var lifeCycleType = p_IsWholeLifeCycleTimer ? ResourceLifeCycleType.WholeGame : ResourceLifeCycleType.Scene;
                var tryKey = (p_TimerType, lifeCycleType);
                
                r_SpawnedObject = p_Key.GetSafeReference(_TerminateReceiverPooler.GetObject(tryKey));
            }
        }
        
        #endregion
    }
}