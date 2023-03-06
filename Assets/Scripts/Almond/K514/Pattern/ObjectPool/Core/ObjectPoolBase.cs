using System.Collections.Generic;
using UnityEngine;

namespace k514
{
    /// <summary>
    /// 오브젝트 풀러 기저 구현체
    /// </summary>
    public abstract class ObjectPoolBase<M> : IObjectPooler<M> where M : IPoolingObject<M>
    {
        #region <Finalizer>

        /// <summary>
        /// 소멸자
        /// </summary>
        ~ObjectPoolBase()
        {
            Dispose();
        }

        #endregion
        
        #region <Fields>

        /// <summary>
        /// 풀링 오브젝트의 인덱싱을 담당하는 오브젝트
        /// </summary>
        public List<M> ActivedObjectPool { get; protected set; }

        /// <summary>
        /// 재활용될(풀링된) 인스턴스 리스트
        /// </summary>
        public List<M> BreakedObjectPool { get; protected set; }

        /// <summary>
        /// 미리 풀링되어 비활성화된 오브젝트를 임시로 기록하는 리스트. 
        /// </summary>
        public List<M> PreLoadObjectGroup { get; protected set; }

        #endregion

        #region <Indexer>

        public M this[int p_Index] => ActivedObjectPool[p_Index];

        #endregion
        
        #region <Constructors>

        public ObjectPoolBase()
        {
            SpawnIndexArray();
            BreakedObjectPool = new List<M>();
            PreLoadObjectGroup = new List<M>();
        }

        #endregion

        #region <Callbacks>

        /// <summary>
        /// 오브젝트가 신규 생성되는 경우 호출되는 내부콜백
        /// </summary>
        protected virtual void OnSpawningObject(M p_TargetObject)
        {
            ActivedObjectPool.Add(p_TargetObject);
            p_TargetObject.PoolState = PoolState.Spawned;
            p_TargetObject.OnSpawning();
        }

        /// <summary>
        /// 풀링되있던 오브젝트가 활성화된 경우 호출되는 내부콜백
        /// </summary>
        private void OnPoolingPooledObject(M p_TargetObject)
        {
            ActivedObjectPool.Add(p_TargetObject);
        }

        /// <summary>
        /// 스폰된 혹은 풀링된 오브젝트가 활성화되는 경우, 어느쪽이건 공통적으로 호출되는 내부콜백
        /// </summary>
        protected virtual void OnActivatePooledObject(M p_TargetObject)
        {
            p_TargetObject.PoolState = PoolState.Actived;
            p_TargetObject.OnPooling();
        }

        /// <summary>
        /// 활성화된 오브젝트가 풀링되는 경우 호출되는 내부콜백
        /// </summary>
        protected virtual void OnRetrieveObject(M p_TargetObject)
        {
            p_TargetObject.PoolState = PoolState.Retrieving;
            ActivedObjectPool.Remove(p_TargetObject);
            p_TargetObject.OnRetrieved();
            BreakedObjectPool.Add(p_TargetObject);
            p_TargetObject.PoolState = PoolState.Pooled;
        }

        #endregion
        
        #region <Methods>

        /// <summary>
        /// 지정한 오브젝트의 인덱스를 리턴한다.
        /// </summary>
        public int GetIndex(M p_Object) => ActivedObjectPool.IndexOf(p_Object);
        
        /// <summary>
        /// 풀링 배열을 생성하는 메서드
        /// </summary>
        protected virtual void SpawnIndexArray()
        {
            ActivedObjectPool = new List<M>();
        }

        /// <summary>
        /// 오브젝트 풀링으로부터 혹은 오브젝트를 신규 생성하여 리턴하는 내부 메서드
        /// </summary>
        public virtual (bool, M) GenerateObject()
        {
            M result;
            if (BreakedObjectPool.Count > 0)
            {
#if UNITY_EDITOR
                if (CustomDebug.PrintObjectPooler)
                {
                    Debug.Log($"Pooled Object Recycled");
                }
#endif
                var lastIndex = BreakedObjectPool.Count - 1;
                result = BreakedObjectPool[lastIndex];
                BreakedObjectPool.RemoveAt(lastIndex);
#if UNITY_EDITOR
                if (result.IsDisposed)
                {
                    Debug.LogError($"파기된 Disposable의 재활성화가 감지되었습니다. 오브젝트 : [{typeof(M)}]");
                }
#endif
                return (false, result);
            }
            else
            {
#if UNITY_EDITOR
                if (CustomDebug.PrintObjectPooler)
                {
                    Debug.Log($"new Object Spawned");
                }
#endif
                result = SpawnObject();
                return (true, result);
            }
        }

        public M InitObject((bool t_IsNewSpawned, M t_SpawnedObject) p_Tuple)
        {
            return InitObject(p_Tuple.t_IsNewSpawned, p_Tuple.t_SpawnedObject);
        }
        
        public M InitObject(bool p_IsNewSpawned, M p_SpawnedObject)
        {
            if (p_IsNewSpawned)
            {
                OnSpawningObject(p_SpawnedObject);
            }
            else
            {
                OnPoolingPooledObject(p_SpawnedObject);
            }
            OnActivatePooledObject(p_SpawnedObject);

            return p_SpawnedObject;
        }

        /// <summary>
        /// 오브젝트 풀링으로부터 혹은 오브젝트를 신규 생성하여 리턴하는 메서드
        /// </summary>
        public virtual M GetObject()
        {
            return InitObject(GenerateObject());
        }

        /// <summary>
        /// 오브젝트를 신규 생성시키는 메서드
        /// </summary>
        public abstract M SpawnObject();

        /// <summary>
        /// 지정한 갯수만큼의 오브젝트를 오브젝트 풀에 회수된 상태로 로드시키는 메서드
        /// 이 때, 해당 메서드로 로드되는 오브젝트 풀의 총 오브젝트 숫자가 2번째 파라미터 갯수를 넘지 않도록 로드한다.
        /// </summary>
        public List<M> PreloadPool(int p_Number, int p_CheckNumber)
        {
            PreLoadObjectGroup.Clear();
            var currentPoolCapacity = GetCurrentPoolObjectCount();
            var preloadNumber = Mathf.Min(p_Number, Mathf.Max(0, p_CheckNumber - currentPoolCapacity));
            
#if UNITY_EDITOR
            if (CustomDebug.PrintObjectPooler)
            {
                Debug.Log($"preload object : {currentPoolCapacity} (+ {p_Number}) [Limit : {p_CheckNumber}] [실제 추가된 오브젝트 수 : {preloadNumber}]");
            }
#endif
            
            for (int i = 0; i < preloadNumber; i++)
            {
                // pseudo 'Spawning' part
                M result = SpawnObject();
                OnSpawningObject(result);
                
                // pseudo 'Pooling' part
                PreLoadObjectGroup.Add(result);
            }

            foreach (var preloadedObject in PreLoadObjectGroup)
            {
                RetrieveObject(preloadedObject);
            }

            return PreLoadObjectGroup;
        }
        
        /// <summary>
        /// 지정한 오브젝트를 오브젝트 풀로 비활성화시키고 회수시키는 메서드
        /// </summary>
        public void RetrieveObject(M p_Target)
        {
            switch (p_Target.PoolState)
            {
                case PoolState.Spawned:
                case PoolState.Actived:
#if UNITY_EDITOR
                    if (CustomDebug.PrintObjectPooler)
                    {
                        Debug.Log($"object removed");
                    }
#endif
                    OnRetrieveObject(p_Target);
                    break;
                case PoolState.Pooled:
                case PoolState.Retrieving:
#if UNITY_EDITOR
                    if (CustomDebug.PrintObjectPooler)
                    {
                        Debug.Log($"object alreeady reached pooled state");
                    }
#endif
                    break;
                case PoolState.Disposed:
#if UNITY_EDITOR
                    if (CustomDebug.PrintObjectPooler)
                    {
                        Debug.Log($"object cannot pooled at {p_Target.PoolState} state");
                    }
#endif
                    break;
            }
        }
        
        /// <summary>
        /// 현재 활성화 되어 있는 모든 오브젝트를 오브젝트 풀로 비활성화시키고 회수시키는 메서드
        /// </summary>
        public void RetrieveAllObject()
        {
#if UNITY_EDITOR
            if (CustomDebug.PrintObjectPooler)
            {
                Debug.Log($"retrieve all object");
            }
#endif

            for (int i = ActivedObjectPool.Count - 1; i > -1; i--)
            {
                RetrieveObject(ActivedObjectPool[i]);
            }
        }

        /// <summary>
        /// 오브젝트 풀을 파기시키는 메서드
        /// 파기 이전에 활성화된 오브젝트들을 회수시킨다.
        /// </summary>
        public abstract void ClearPool();

        /// <summary>
        /// 현재 풀에서 관리하는 오브젝트의 갯수를 리턴하는 메서드
        /// </summary>
        public int GetCurrentPoolObjectCount()
        {
            return BreakedObjectPool.Count + ActivedObjectPool.Count;
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
            if (IsDisposed)
            {
                return;
            }
            else
            {
                IsDisposed = true;
                DisposeUnManaged();
            }
        }

        /// <summary>
        /// 인스턴스가 파기될 때 수행할 작업을 기술한다.
        /// </summary>
        private void DisposeUnManaged()
        {
            ClearPool();
        }

        #endregion
    }
}