using Cysharp.Threading.Tasks;

namespace BlackAm
{
    /// <summary>
    /// 게임 씬 전환을 이벤트를 기술하는 인터페이스
    /// </summary>
    public interface ISceneChange
    {
        /// <summary>
        /// 씬이 로딩되는 경우
        /// </summary>
        UniTask OnScenePreload();
        
        /// <summary>
        /// 씬이 시작되는 경우
        /// </summary>
        void OnSceneStarted();

        /// <summary>
        /// 씬이 종료되는 경우
        /// </summary>
        void OnSceneTerminated();

        /// <summary>
        /// 씬이 전이되는 경우
        /// </summary>
        void OnSceneTransition();
    }
    
    /// <summary>
    /// 게임 씬 전환을 감지하는 오브젝트의 인터페이스
    /// </summary>
    public interface ISceneChangeObserve : IPriority, ISceneChange
    {
        /// <summary>
        /// 해당 구현체에게 씬 변경 이벤트를 전달한 객체를 등록하는 메서드
        /// </summary>
        void Subscribe();

        /// <summary>
        /// 해당 구현체에게 씬 변경 이벤트를 전달한 객체를 해제하는 메서드
        /// </summary>
        void Dissubscribe();
    }
}