using System;
using UnityEngine;

namespace BlackAm
{
    /// <summary>
    /// 싱글톤 기본 구현체
    ///
    /// 싱글톤에 접근자 프로퍼티 호출시, 자동으로 전역 변수를 생성하여
    /// 이후 해당 싱글톤에 대한 참조를 전역 변수 참조로 한다.
    /// </summary>
    public abstract class Singleton<Me> : SingletonBase<Me> where Me : Singleton<Me>, new()
    {
        #region <Consts>

        /// <summary>
        /// 싱글톤 프로퍼티
        /// </summary>
        public static Me GetInstance => GetInstanceObject();

        /// <summary>
        /// 싱글톤 생성 메서드
        /// </summary>
        private static Me GetInstanceObject()
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
                            _instance.InitInstance();
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
        protected abstract void OnCreated();

        /// <summary>
        /// 싱글톤 초기화 콜백. 최초에 OnCreated 이후에 호출된다.
        /// 임의로 싱글톤 상태를 초기화 시켜야 할 때 임의로 호출할 수 있다.
        /// </summary>
        public abstract void OnInitiate();

        #endregion

        #region <Methods>
        
        /// <summary>
        /// 싱글톤 내부 초기화 메서드
        /// </summary>
        protected virtual void InitInstance()
        {
            OnCreated();
            OnInitiate();
        }
        
        #endregion
    }
    
    /// <summary>
    /// 싱글톤과 MonoBehaviour를 상속하여, 유니티 컴포넌트로서 동작하는 싱글톤 추상 클래스.
    /// </summary>
    public abstract class UnitySingleton<Me> : UnitySingletonBase<Me> where Me : UnitySingleton<Me>
    {
        #region <Consts>

        /// <summary>
        /// 싱글톤 프로퍼티
        /// </summary>
        public static Me GetInstance => GetInstanceObject();

        /// <summary>
        /// 싱글톤 생성 메서드
        /// </summary>
        private static Me GetInstanceObject()
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
                            _instance.InitInstance();
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
        private void Awake()
        {
            GetInstanceObject();
        }
        
        /// <summary>
        /// 싱글톤 초기화 콜백. 해당 싱글톤 생명주기 중에 단 한번만 호출되야함.
        /// </summary>
        protected abstract void OnCreated();

        /// <summary>
        /// 싱글톤 초기화 콜백. 최초에 OnCreated 이후에 호출된다.
        /// 임의로 싱글톤 상태를 초기화 시켜야 할 때 임의로 호출할 수 있다.
        /// </summary>
        public abstract void OnInitiate();

        #endregion

        #region <Methods>
        
        /// <summary>
        /// 싱글톤 내부 초기화 메서드
        /// </summary>
        protected virtual void InitInstance()
        {
            OnCreated();
            OnInitiate();
        }
        
        #endregion
    }
}