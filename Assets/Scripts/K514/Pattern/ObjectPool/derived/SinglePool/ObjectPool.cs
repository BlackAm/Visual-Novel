using UnityEngine;

namespace BlackAm
{
    /// <summary>
    /// 일반 클래스 인스턴스를 풀링하기 위한 IObjectPooler 구현체
    /// </summary>
    public class ObjectPooler<M> : ObjectPoolBase<M> where M : PoolingObject<M>, new()
    {
        #region <Methods>
        
        /// <summary>
        /// 오브젝트를 신규 생성시키는 메서드
        /// </summary>
        public override M SpawnObject()
        {
            M result = new M();
            result.PoolingContainer = this;
            
            return result;
        }

        /// <summary>
        /// 오브젝트 풀을 파기시키는 메서드
        /// 파기 이전에 활성화된 오브젝트들을 회수시킨다.
        /// </summary>
        public override void ClearPool()
        {
#if UNITY_EDITOR
            if (CustomDebug.PrintObjectPooler)
            {
                Debug.Log($"clear pool");
            }
#endif
            RetrieveAllObject();
            ActivedObjectPool.Clear();
            PreLoadObjectGroup.Clear();

            var breakedPoolScale = BreakedObjectPool.Count;
            for (int i = breakedPoolScale - 1; i > -1; i--)
            {
                var breakedObject = BreakedObjectPool[i];
                breakedObject.Dispose();
            }
            BreakedObjectPool.Clear();
        }

        #endregion
    }

    /// <summary>
    /// 일반 클래스 인스턴스를 풀링하기 위한 IPoolingObject 구현체
    /// </summary>
    public abstract class PoolingObject<M> : IPoolingObject<M> where M : PoolingObject<M>
    {
        #region <Finalizer>

        ~PoolingObject()
        {
            Dispose();
        }

        #endregion

        #region <IPoolingObject>

        public IObjectPooler<M> PoolingContainer { get; set; }
        public PoolState PoolState { get; set; }

        public abstract void OnSpawning();

        public abstract void OnPooling();

        public abstract void OnRetrieved();

        public void RetrieveObject()
        {
            if (PoolState != PoolState.None)
            {
                SetCompareKey(null);
                PoolingContainer.RetrieveObject(this as M);
            }
        }

        #endregion

        #region <SafeReference>

        public object CompareKey { get; private set; }

        public void SetCompareKey(object p_CompareKey)
        {
            CompareKey = p_CompareKey;
        }

        #endregion
                
        #region <Objects>

        /// <summary>
        /// 해당 오브젝트가 오브젝트 풀러 이외의 방법으로 생성된 경우, 생성 콜백을 수동 호출해주는 메서드
        /// </summary>
        public void CheckAwake()
        {
            if (PoolState == PoolState.None)
            {
                OnSpawning();
                OnPooling();
            }
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
        protected virtual void DisposeUnManaged()
        {
            switch (PoolState)
            {
                case PoolState.Actived :
                    RetrieveObject();
                    DisposeContainer();
                    break;
                case PoolState.Pooled :
                case PoolState.Retrieving :
                    DisposeContainer();
                    break;
                case PoolState.Disposed :
                    break;
            }
        }

        public virtual void DisposeContainer()
        {
            PoolingContainer.BreakedObjectPool.Remove(this as M);
            PoolingContainer = null;
            PoolState = PoolState.Disposed;
        }

        #endregion
    }
}