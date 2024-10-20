using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BlackAm
{
    /// <summary>
    /// 시스템 내의 씬 변환 이벤트를 일괄적으로 제어하는 싱글톤 클래스
    /// </summary>
    public class SceneChangeEventSender : SceneChangeEventSingleton<SceneChangeEventSender>
    {
        #region <Fields>

        private SortedSet<ISceneChangeObserve> _singletonGroup;

        #endregion

        #region <Callbacks>

        protected override void OnCreated()
        {
            _singletonGroup = new SortedSet<ISceneChangeObserve>(
                Comparer<ISceneChangeObserve>.Create(
                    (left, right) =>
                    {
                        if (left.Priority == right.Priority)
                        {
                            return ReferenceEquals(left, right) ? 0 : 1;
                        }
                        else
                        {
                            return left.Priority > right.Priority ? 1 : -1;
                        }
                    }
                )
            );
        }

        public override void OnInitiate()
        {
        }

        public override async UniTask OnScenePreload()
        {
#if UNITY_EDITOR
            if (CustomDebug.PrintSceneChangeObserverPriority)
            {
                Debug.Log("ScenePreload Event Occur");
            }
                
            foreach (var trySingleton in _singletonGroup)
            {
                if (CustomDebug.PrintSceneChangeObserverPriority)
                {
                    if (trySingleton.GetType().IsSubclassOf(typeof(MonoBehaviour)))
                    {
                        Debug.Log($"** {(trySingleton as MonoBehaviour).name} Priority {trySingleton.Priority}");
                    }
                    else
                    {
                        Debug.Log($"** {trySingleton.GetType().Name} Priority {trySingleton.Priority}");
                    }
                }
                await trySingleton.OnScenePreload();
            }
#else
            foreach (var trySingleton in _singletonGroup)
            {
                await trySingleton.OnScenePreload();
            }
#endif
        }

        public override void OnSceneStarted()
        {
#if UNITY_EDITOR
            if (CustomDebug.PrintSceneChangeObserverPriority)
            {
                Debug.Log("SceneStarted Event Occur");
            }
            try
            {
                foreach (var trySingleton in _singletonGroup)
                {
                    if (CustomDebug.PrintSceneChangeObserverPriority)
                    {
                        if (trySingleton.GetType().IsSubclassOf(typeof(MonoBehaviour)))
                        {
                            Debug.Log($"** {(trySingleton as MonoBehaviour).name} Priority {trySingleton.Priority}");
                        }
                        else
                        {
                            Debug.Log($"** {trySingleton.GetType().Name} Priority {trySingleton.Priority}");
                        }
                    }
                    trySingleton.OnSceneStarted();
                }
            }
            catch(System.Exception e)
            {
                Debug.LogError(e.Message);
                Debug.LogError(e.StackTrace);
            }
#else
            foreach (var trySingleton in _singletonGroup)
            {
                trySingleton.OnSceneStarted();
            }
#endif
        }

        public override void OnSceneTerminated()
        {
#if UNITY_EDITOR
            if (CustomDebug.PrintSceneChangeObserverPriority)
            {
                Debug.Log($"SceneTerminated Event Occur");
            }

            foreach (var trySingleton in _singletonGroup)
            {
                if (CustomDebug.PrintSceneChangeObserverPriority)
                {
                    if (trySingleton.GetType().IsSubclassOf(typeof(MonoBehaviour)))
                    {
                        Debug.Log($"** {(trySingleton as MonoBehaviour).name} Priority {trySingleton.Priority}");
                    }
                    else
                    {
                        Debug.Log($"** {trySingleton.GetType().Name} Priority {trySingleton.Priority}");
                    }
                }
                trySingleton.OnSceneTerminated();
            }
#else
            foreach (var trySingleton in _singletonGroup)
            {
                trySingleton.OnSceneTerminated();
            }
#endif
        }

        public override void OnSceneTransition()
        {
#if UNITY_EDITOR
            if (CustomDebug.PrintSceneChangeObserverPriority)
            {
                Debug.Log($"Scene Transition to Loading Scene");
            }

            foreach (var trySingleton in _singletonGroup)
            {
                if (CustomDebug.PrintSceneChangeObserverPriority)
                {
                    if (trySingleton.GetType().IsSubclassOf(typeof(MonoBehaviour)))
                    {
                        Debug.Log($"** {(trySingleton as MonoBehaviour).name} Priority {trySingleton.Priority}");
                    }
                    else
                    {
                        Debug.Log($"** {trySingleton.GetType().Name} Priority {trySingleton.Priority}");
                    }
                }
                trySingleton.OnSceneTransition();
            }
#else
            foreach (var trySingleton in _singletonGroup)
            {
                trySingleton.OnSceneTransition();
            }
#endif
        }
        
        #endregion

        #region <Methods>

        public void AddSceneObserver(ISceneChangeObserve p_Singleton)
        {
            if (p_Singleton != this)
            {
#if UNITY_EDITOR
                if (CustomDebug.PrintSceneChangeObserverPriority)
                {
                    Debug.Log($"{p_Singleton.GetType().Name} added : Input Singleton Type to BootStrap preload list");
                }
#endif
                _singletonGroup.Add(p_Singleton);
            }
        }

        public void RemoveSceneObserver(ISceneChangeObserve p_Singleton)
        {
            if (p_Singleton != this && _singletonGroup.Contains(p_Singleton))
            {
#if UNITY_EDITOR
                if (CustomDebug.PrintSceneChangeObserverPriority)
                {
                    Debug.Log($"{p_Singleton.GetType().Name} removed");
                }
#endif
                _singletonGroup.Remove(p_Singleton);
            }
        }

        #endregion
    }
}