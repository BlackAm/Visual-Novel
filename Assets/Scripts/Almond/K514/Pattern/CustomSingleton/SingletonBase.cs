using System;
using UnityEngine;

namespace k514
{
    /// <summary>
    /// 싱글톤 공통 인터페이스
    /// </summary>
    public interface ISingleton : _IDisposable
    {
    }
    
    /// <summary>
    /// 굳이 이렇게 번거롭게 제네릭스를 사용하는 이유는
    /// 정적 멤버 _Instance로 임의의 접근이 발생하는 경우 초기화가 보장이 되어야 하는데
    ///
    /// _Instance가 초기화되려면 게임 내에 단 하나 존재해야하는 임의의 컴포넌트화된 인스턴스를
    /// 찾아야 했고
    ///
    /// 그 과정에서 FindObjectOfType[T] 유니티 함수를 활용하는데, 만약 싱글톤의 공통적인 부분을 추상화 하지 않았다면
    /// 각각의 클래스에서 자신의 타입을 넘겨주면 되지만
    ///
    /// 싱글톤의 공통적인 부분을 추상화시켜주고 싶고, 그 추상화된 공통 부분에 각 싱글톤의 파생된 타입을 넘겨주고 싶었기
    /// 때문이다.
    ///
    /// 
    /// 제네릭스 클래스를 상속한다는 것은 단순히 placeHolder T 타입에 대한
    /// 기능을 확장한다는 용도도 있지만
    ///
    /// 그 자리에 서브 클래스를 넣는 것으로 슈퍼 클래스의 멤버가 사용될 범위를
    /// 서브 클래스로 한정시킨다는 용도도 가짐.
    /// (정확히는 각각의 서브 클래스 타입 T에 대해 Singleton'T 이라는 '신규 타입'이 생성되므로)
    /// 
    /// 전자의 경우에는 어떤 서브 클래스 역시, 슈퍼 클래스의 성질을 가져서
    /// 기능을 제공하겠다는 의도를 가지고
    ///
    /// 후자의 경우에는 어떤 서브 클래스들의 공통적인 부분을 추상화한 제네릭스 클래스에
    /// 각 서브 클래스의 정보를 넘기고 싶다 하는 의도를 가진다.
    ///
    /// SubType[T] : Singleton[T] where T ~ 와 SubType : Singleton[SubType]을
    /// 비교해보면 이해하기 쉽다.
    /// 
    /// </summary>
    /// <typeparam name="Me">싱글톤으로 관리하고 싶은 데이터 타입</typeparam>
    public abstract class SingletonBase<Me> : ISingleton where Me : SingletonBase<Me>, new()
    {
        ~SingletonBase()
        {
            Dispose();
        }

        #region <Consts>

        /// <summary>
        /// 싱글톤 인스턴스
        /// </summary>
        protected static Me _instance;

        /// <summary>
        /// 싱글톤이 초기화 진행 페이즈
        /// </summary>
        protected static TaskPhase TaskPhase;
       
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
            switch (TaskPhase)
            {
                case TaskPhase.None:
                    break;
                case TaskPhase.TaskProgressing:
                case TaskPhase.TaskTerminate:
                    if (IsDisposed)
                    {
                        return;
                    }
                    else
                    {
                        IsDisposed = true;
#if UNITY_EDITOR
                        if (CustomDebug.PrintSingletonLoading)
                        {
                            Debug.Log($"[{typeof(Me).Name}] Dispose Start");
                        }
#endif
                        try
                        {
                            DisposeUnManaged();
#if UNITY_EDITOR
                            if (CustomDebug.PrintSingletonLoading)
                            {
                                Debug.Log($"[{typeof(Me).Name}] Dispose Complete");
                            }
#endif
                        }
#if UNITY_EDITOR
                        catch (Exception e)
                        {
                            if (CustomDebug.PrintSingletonLoading)
                            {
                                Debug.LogError($"[{typeof(Me).Name}] Dispose Failed ({e.Message})\n{e.StackTrace}");
                            }

                            throw;
                        }
#else
                        catch
                        {
                        }
#endif
                    }
                    break;
            }
        }

        /// <summary>
        /// 인스턴스가 파기될 때 수행할 작업을 기술한다.
        /// </summary>
        protected virtual void DisposeUnManaged()
        {
            SingletonTool.OnSingletonDisposed(_instance);
            TaskPhase = TaskPhase.None;
            _instance = null;
        }
        
        #endregion
    }
        
    /// <summary>
    /// 싱글톤과 MonoBehaviour를 상속하여, 유니티 컴포넌트로서 동작하는 싱글톤 추상 클래스.
    /// </summary>
    public abstract class UnitySingletonBase<Me> : MonoBehaviour, ISingleton where Me : UnitySingletonBase<Me>
    {
        #region <Consts>

        /// <summary>
        /// 싱글톤 인스턴스
        /// </summary>
        protected static Me _instance;
      
        /// <summary>
        /// 싱글톤이 초기화 진행 페이즈
        /// </summary>
        protected static TaskPhase TaskPhase;

        #endregion

        #region <Callbacks>

        /// <summary>
        /// 게임 오브젝트가 파괴된 경우, 소멸 메서드를 호출해준다.
        /// </summary>
        protected virtual void OnDestroy()
        {
            Dispose();
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
            switch (TaskPhase)
            {
                case TaskPhase.None:
                    break;
                case TaskPhase.TaskProgressing:
                case TaskPhase.TaskTerminate:
                    if (IsDisposed)
                    {
                        return;
                    }
                    else
                    {
                        IsDisposed = true;
#if UNITY_EDITOR
                        if (CustomDebug.PrintSingletonLoading)
                        {
                            Debug.Log($"[{typeof(Me).Name}] Dispose Start");
                        }
#endif
                        try
                        {
                            DisposeUnManaged();
#if UNITY_EDITOR
                            if (CustomDebug.PrintSingletonLoading)
                            {
                                Debug.Log($"[{typeof(Me).Name}] Dispose Complete");
                            }
#endif
                        }
#if UNITY_EDITOR
                        catch (Exception e)
                        {
                            if (CustomDebug.PrintSingletonLoading)
                            {
                                Debug.LogError($"[{typeof(Me).Name}] Dispose Failed ({e.Message})\n{e.StackTrace}");
                            }
                            throw;
                        }
#else
                        catch
                        {
                        }
#endif
                    }
                    break;
            }
        }

        /// <summary>
        /// 인스턴스가 파기될 때 수행할 작업을 기술한다.
        /// </summary>
        protected virtual void DisposeUnManaged()
        {
            SingletonTool.OnSingletonDisposed(_instance);
            TaskPhase = TaskPhase.None;
            _instance = null;
            Destroy(this);
        }

        #endregion
    }
}