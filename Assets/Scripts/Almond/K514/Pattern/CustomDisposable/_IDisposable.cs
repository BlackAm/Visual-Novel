using System;

namespace k514
{
    /// <summary>
    /// msdn 예제에 맞춰 IDisposable에 기능을 추가한 인터페이스
    /// </summary>
    public interface _IDisposable : IDisposable
    {
        /// <summary>
        /// dispose 패턴 onceFlag
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// dispose 플래그를 초기화 시키는 메서드
        /// </summary>
        void Rejunvenate();
    }

#if UNITY_EDITOR
    /// <summary>
    /// msdn 예제에 맞춰 IDisposable에 기능을 추가한 인터페이스 _IDisposable을 구현한 클래스
    /// </summary>
    public abstract class _IDisposableImp : _IDisposable
    {
        // 유니티 컴포넌트의 경우에는 소멸자를 쓰지 말고, OnDestory를 통해 소멸자 메서드를 호출할 것
        ~_IDisposableImp()
        {
            Dispose();
        }
        
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
                
                // GC가 오브젝트를 파기하면서 소멸자~Finalize를 호출하지 않도록 함
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// 인스턴스가 파기될 때 수행할 작업을 기술한다.
        /// </summary>
        protected abstract void DisposeUnManaged();
    }
#endif
}