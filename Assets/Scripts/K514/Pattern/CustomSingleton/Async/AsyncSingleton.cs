using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BlackAm
{
    /// <summary>
    /// 초기화 로직을 비동기로 수행할 수 있는 싱글톤
    ///
    /// 기존의 싱글톤과 다음과 같은 차이점을 가진다.
    ///
    ///     1. 싱글톤을 비동기 로드한다.
    ///     2. 싱글톤이 로드 중에 싱글톤에 접근하는 경우, 로드과 완료될때까지 해당 문맥은 block된다.
    ///     3. UnSafe 접근자를 통해 로드여부를 고려하지 않고 인스턴스에 접근할 수 있다.
    ///     4. 3에 의해 기존의 [싱글톤의 호출 = 자동 로딩]이 적용되지 않아, Null을 리턴할 수 있다.
    ///     5. 4에 의해 싱글톤의 고질적인 문제인 [싱글톤 릴리스 중에, 다른 싱글톤을 참조하여 로딩해버리는 문제]를 해결 할 수 있다.
    /// 
    /// </summary>
    public abstract class AsyncSingleton<Me> : SingletonBase<Me> where Me : AsyncSingleton<Me>, new()
    {
        #region <Consts>

        /// <summary>
        /// 비동기 싱글톤에서 await 키워드를 거치지 않고 곧바로 싱글톤 인스턴스에 접근하는 프로퍼티
        /// 해당 싱글톤이 미리 초기화 되어있음이 보장되지 않는다면 null을 리턴한다.
        /// </summary>
        public static Me GetInstanceUnSafe
        {
            get
            {
                switch (TaskPhase)
                {
                    default:
                    case TaskPhase.None:
                        return null;
                    case TaskPhase.TaskProgressing:
                    case TaskPhase.TaskTerminate:
                        return _instance;
                }
            }
        }

        /// <summary>
        /// GetInstanceUnSafe가 null을 리턴하는 경우 싱글톤 로딩을 기다렸다 인스턴스를 리턴하게 하는 접근자
        /// </summary>
        public static async UniTask<object> GetInstanceUnSafeWaiting()
        {
            return GetInstanceUnSafe ?? await GetInstance();
        }

        /// <summary>
        /// 싱글톤 생성 메서드
        /// </summary>
        public static async UniTask<Me> GetInstance()
        {
            switch (TaskPhase)
            {
                case TaskPhase.None:
                {
                    if (SingletonTool.SingletonRestrictFlag)
                    {
                    }
                    else
                    {
                        TaskPhase = TaskPhase.TaskProgressing;
                        _instance = new Me();
#if UNITY_EDITOR
                        if (CustomDebug.PrintSingletonLoading)
                        {
                            Debug.Log($"[{typeof(Me).Name}] Load Start");
                        }
#endif
                        SingletonTool.OnSingletonSpawned(_instance);
                        try
                        {
                            await _instance.InitInstance().WithCancellation(SystemMaintenance._SystemTaskCancellationToken);
                            TaskPhase = TaskPhase.TaskTerminate;
#if UNITY_EDITOR
                            if (CustomDebug.PrintSingletonLoading)
                            {
                                Debug.Log($"[{typeof(Me).Name}] Load Complete");
                            }
#endif
                        }
#if UNITY_EDITOR
                        catch(Exception e)
                        {
                            if (CustomDebug.PrintSingletonLoading)
                            {
                                Debug.LogError($"[{typeof(Me).Name}] Load Failed ({e.Message})\n{e.StackTrace}");
                            }
                            _instance?.Dispose();
                        }
#else
                        catch
                        {
                            _instance?.Dispose();
                        }
#endif
                    }
                }
                    break;
                case TaskPhase.TaskProgressing:
#if UNITY_EDITOR
                    if (CustomDebug.PrintSingletonLoading)
                    {
                        Debug.LogWarning($"[{typeof(Me).Name}] GetSingleton Yield");
                    }
#endif
                    await UniTask.WaitUntil(() => TaskPhase != TaskPhase.TaskProgressing).WithCancellation(SystemMaintenance._SystemTaskCancellationToken);
                    break;
                case TaskPhase.TaskTerminate:
                    break;
            }
     
            return _instance;
        }

        #endregion

        #region <Callbacks>

        /// <summary>
        /// 싱글톤 초기화 콜백. 해당 싱글톤 생명주기 중에 단 한번만 호출되야함.
        /// </summary>
        protected abstract UniTask OnCreated();

        /// <summary>
        /// 싱글톤 초기화 콜백. 최초에 OnCreated 이후에 호출된다.
        /// 임의로 싱글톤 상태를 초기화 시켜야 할 때 임의로 호출할 수 있다.
        /// </summary>
        public abstract UniTask OnInitiate();

        #endregion

        #region <Methods>
        
        /// <summary>
        /// 싱글톤 내부 초기화 메서드
        /// </summary>
        protected virtual async UniTask InitInstance()
        {
            await OnCreated();
            await OnInitiate();
        }
        
        #endregion
    }
    
    /// <summary>
    /// 싱글톤과 MonoBehaviour를 상속하여, 유니티 컴포넌트로서 동작하는 싱글톤 추상 클래스.
    /// </summary>
    public abstract class UnityAsyncSingleton<Me> : UnitySingletonBase<Me> where Me : UnityAsyncSingleton<Me>
    {
        #region <Consts>

        /// <summary>
        /// 비동기 싱글톤에서 await 키워드를 거치지 않고 곧바로 싱글톤 인스턴스에 접근하는 프로퍼티
        /// 해당 싱글톤이 미리 초기화 되어있음이 보장되지 않는다면 null을 리턴한다.
        /// </summary>
        public static Me GetInstanceUnSafe
        {
            get
            {
                switch (TaskPhase)
                {
                    default:
                    case TaskPhase.None:
                        return null;
                    case TaskPhase.TaskProgressing:
                    case TaskPhase.TaskTerminate:
                        return _instance;
                }
            }
        }
        
        /// <summary>
        /// GetInstanceUnSafe가 null을 리턴하는 경우 싱글톤 로딩을 기다렸다 인스턴스를 리턴하게 하는 접근자
        /// </summary>
        public static async UniTask<object> GetInstanceUnSafeWaiting()
        {
            var tryInstance = GetInstanceUnSafe;
            return ReferenceEquals(null, tryInstance) ? await GetInstance() : tryInstance;
        }
        
        /// <summary>
        /// 싱글톤 생성 메서드
        /// </summary>
        public static async UniTask<Me> GetInstance()
        {
            switch (TaskPhase)
            {
                case TaskPhase.None:
                {
                    if (SingletonTool.SingletonRestrictFlag)
                    {
                    }
                    else
                    {
                        TaskPhase = TaskPhase.TaskProgressing;
                        _instance = FindObjectOfType<Me>() ?? new GameObject($"{typeof(Me).Name} (FallBackSpawned)").AddComponent<Me>();
#if UNITY_EDITOR
                        if (CustomDebug.PrintSingletonLoading)
                        {
                            Debug.Log($"[{typeof(Me).Name}] Load Start");
                        }
#endif
                        SingletonTool.OnSingletonSpawned(_instance);
                        try
                        {
                            await _instance.InitInstance().WithCancellation(SystemMaintenance._SystemTaskCancellationToken);
                            TaskPhase = TaskPhase.TaskTerminate;
#if UNITY_EDITOR
                            if (CustomDebug.PrintSingletonLoading)
                            {
                                Debug.Log($"[{typeof(Me).Name}] Load Complete");
                            }
#endif
                        }
#if UNITY_EDITOR
                        catch(Exception e)
                        {
                            if (CustomDebug.PrintSingletonLoading)
                            {
                                Debug.LogError($"[{typeof(Me).Name}] Load Failed ({e.Message})\n{e.StackTrace}");
                            }

                            if (!ReferenceEquals(null, _instance))
                            {
                                _instance.Dispose();
                            }
                        }
#else
                        catch
                        {
                            if (!ReferenceEquals(null, _instance))
                            {
                                _instance.Dispose();
                            }
                        }
#endif
                    }
                }
                    break;
                case TaskPhase.TaskProgressing:
#if UNITY_EDITOR
                    if (CustomDebug.PrintSingletonLoading)
                    {
                        Debug.LogWarning($"[{typeof(Me).Name}] GetSingleton Yield");
                    }
#endif
                    await UniTask.WaitUntil(() => TaskPhase != TaskPhase.TaskProgressing).WithCancellation(SystemMaintenance._SystemTaskCancellationToken);
                    break;
                case TaskPhase.TaskTerminate:
                    break;
            }
            
            return _instance;
        }

        #endregion

        #region <Callbacks>

        /// <summary>
        /// 유니티 초기화 콜백
        /// </summary>
        private async void Awake()
        {
            await GetInstance();
        }
        
        /// <summary>
        /// 싱글톤 초기화 콜백. 해당 싱글톤 생명주기 중에 단 한번만 호출되야함.
        /// </summary>
        protected abstract UniTask OnCreated();

        /// <summary>
        /// 싱글톤 초기화 콜백. 최초에 OnCreated 이후에 호출된다.
        /// 임의로 싱글톤 상태를 초기화 시켜야 할 때 임의로 호출할 수 있다.
        /// </summary>
        public abstract UniTask OnInitiate();

        #endregion

        #region <Methods>
        
        /// <summary>
        /// 싱글톤 내부 초기화 메서드
        /// </summary>
        protected virtual async UniTask InitInstance()
        {
            await OnCreated();
            await OnInitiate();
        }
        
        #endregion
    }
}