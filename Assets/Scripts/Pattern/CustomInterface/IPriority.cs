namespace BlackAm
{
    /// <summary>
    /// 우선도 인터페이스
    /// </summary>
    public interface IPriority
    {
        /// <summary>
        /// 초기화 순서를 결정하는 우선도 변수
        /// </summary>
        int Priority { get; }
    }
}