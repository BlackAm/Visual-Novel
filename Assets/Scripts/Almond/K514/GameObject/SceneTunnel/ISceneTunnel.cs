using Cysharp.Threading.Tasks;

namespace k514
{
    /// <summary>
    /// 씬 전환의 시작/끝 시점에 특정한 이벤트를 수행할 인터페이스
    /// </summary>
    public interface ISceneChangeObserveObject : ISceneChangeObserve
    {
    }

#if UNITY_EDITOR
    /// <summary>
    /// ISceneChangeObserveObjectImp 구현 예시
    /// </summary>
    public abstract class ISceneChangeObserveObjectImp : ISceneChangeObserveObject
    {
        /// <summary>
        /// 우선도 변수가 클수록 늦게 초기화 된다.
        /// </summary>
        public int Priority { get; set; }
        
        /// <summary>
        /// SceneChangeEventSender에 해당 싱글톤을 등록하여 씬 전환 시작/끝의 이벤트를 호출받게 하는 메서드
        /// </summary>
        public void Subscribe()
        {
            SceneChangeEventSender.GetInstance.AddSceneObserver(this);
        }

        /// <summary>
        /// SceneChangeEventSender와의 연결을 해제하는 메서드
        /// </summary>
        public void Dissubscribe()
        {
            SceneChangeEventSender.GetInstance.RemoveSceneObserver(this);
        }

        /// <summary>
        /// 현재 씬이 최초에 로드된 경우 수행할 이벤트를 기술한다.
        /// </summary>
        public async UniTask OnScenePreload()
        {
            await UniTask.CompletedTask;
        }

        /// <summary>
        /// 현재 씬이 로직상 시작되는 경우 수행할 이벤트를 기술한다.
        /// </summary>
        public void OnSceneStarted()
        {
        }

        /// <summary>
        /// 현재 씬이 파기되는 경우 수행할 이벤트를 기술한다.
        /// </summary>
        public void OnSceneTerminated()
        {
        }

        /// <summary>
        /// 현재 씬이 로딩씬으로 넘어가는 경우 수행할 이벤트를 기술한다.
        /// </summary>
        public void OnSceneTransition()
        {
        }
    }
#endif
}