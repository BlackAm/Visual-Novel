using Cysharp.Threading.Tasks;

namespace k514
{
    /// <summary>
    /// 유니티 Scene 변경과 상호작용하는 싱글톤 추상 클래스
    /// </summary>
    public abstract class SceneChangeEventAsyncSingleton<Me> : AsyncSingleton<Me>, ISceneChangeObserve where Me : SceneChangeEventAsyncSingleton<Me>, new()
    {
        #region <Methods>

        /// <summary>
        /// 싱글톤 내부 초기화 메서드
        /// </summary>
        protected override async UniTask InitInstance()
        {
            Subscribe();
            
            await base.InitInstance();
        }

        #endregion
        
        #region <SceneChangedObserving>

        /// <summary>
        /// 초기화 순서를 결정하는 우선도 변수
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// 씬 전환 이벤트 감지 싱글톤을 싱글톤 매니저에 등록하는 메서드.
        /// 싱글톤들은 자동으로 등록된다.
        /// </summary>
        public void Subscribe()
        {
            SceneChangeEventSender.GetInstance?.AddSceneObserver(_instance);
        }

        /// <summary>
        /// 더 이상 특정 싱글톤이 씬 전이 이벤트를 호출받지 않아야하는 경우 사용한다.
        /// </summary>
        public void Dissubscribe()
        {
            SceneChangeEventSender.GetInstance?.RemoveSceneObserver(_instance);
        }

        /// <summary>
        /// 씬이 로딩된 경우
        /// </summary>
        public abstract UniTask OnScenePreload();
        
        /// <summary>
        /// 씬이 시작되는 경우
        /// </summary>
        public abstract void OnSceneStarted();

        /// <summary>
        /// 씬이 종료되는 경우
        /// </summary>
        public abstract void OnSceneTerminated();
        
        /// <summary>
        /// 씬 종료 이후 연출 다음, 씬이 로딩씬으로 전이되는 경우
        /// </summary>
        public abstract void OnSceneTransition();

        #endregion
        
        #region <Disposable>

        /// <summary>
        /// 인스턴스가 파기될 때 수행할 작업을 기술한다.
        /// </summary>
        protected override void DisposeUnManaged()
        {
            Dissubscribe();
            
            base.DisposeUnManaged();
        }

        #endregion
    }
    
    /// <summary>
    /// 싱글톤 중에 MonoBehaviour를 상속하여, 유니티 컴포넌트로서 동작하는 싱글톤 추상 클래스에
    /// 씬의 시작과 종료 이벤트에 대응하는 콜백을 추가시킨 추상 클래스
    /// 초기화 작업 순서는 OnCreated -> Init -> Subscribe 순서가 된다.
    /// </summary>
    public abstract class SceneChangeEventUnityAsyncSingleton<Me> : UnityAsyncSingleton<Me>, ISceneChangeObserve where Me : SceneChangeEventUnityAsyncSingleton<Me>
    {
        #region <Methods>

        /// <summary>
        /// 싱글톤 내부 초기화 메서드
        /// </summary>
        protected override async UniTask InitInstance()
        {
            Subscribe();
            
            await base.InitInstance();
        }

        #endregion

        #region <SceneChangedObserving>

        /// <summary>
        /// 초기화 순서를 결정하는 우선도 변수
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// 씬 전환 이벤트 감지 싱글톤을 싱글톤 매니저에 등록하는 메서드
        /// 싱글톤들은 자동으로 등록된다.
        /// </summary>
        public void Subscribe()
        {
            SceneChangeEventSender.GetInstance?.AddSceneObserver(_instance);
        }

        /// <summary>
        /// 더 이상 특정 싱글톤이 씬 전이 이벤트를 호출받지 않아야하는 경우 사용한다.
        /// </summary>
        public void Dissubscribe()
        {
            SceneChangeEventSender.GetInstance?.RemoveSceneObserver(_instance);
        }

        /// <summary>
        /// 씬이 로딩된 경우
        /// </summary>
        public abstract UniTask OnScenePreload();
        
        /// <summary>
        /// 씬이 시작되는 경우
        /// </summary>
        public abstract void OnSceneStarted();

        /// <summary>
        /// 씬이 종료되는 경우
        /// </summary>
        public abstract void OnSceneTerminated();

        /// <summary>
        /// 로딩씬으로 전이하는 경우
        /// </summary>
        public abstract void OnSceneTransition();
        
        #endregion
        
        #region <Disposable>

        /// <summary>
        /// 인스턴스가 파기될 때 수행할 작업을 기술한다.
        /// </summary>
        protected override void DisposeUnManaged()
        {
            Dissubscribe();
            
            base.DisposeUnManaged();
        }

        #endregion
    }
}