using System.Collections.Generic;

namespace k514
{
    public class NodePool<C> : _IDisposable
    {
        ~NodePool()
        {
            Dispose();
        }
    
        #region <Fields>

        private ObjectPooler<Node<C>> _NodePooler;
        private List<Node<C>> _NodeArray;

        #endregion

        #region <Index>

        public Node<C> this[int p_Index] => _NodeArray[p_Index];

        #endregion
        
        #region <Constructor>

        public NodePool(int p_PreloadCount)
        {
            _NodePooler = new ObjectPooler<Node<C>>();
            _NodeArray = _NodePooler.ActivedObjectPool;
            
            if (p_PreloadCount > 0)
            {
                _NodePooler.PreloadPool(p_PreloadCount, p_PreloadCount);
            }
        }

        #endregion
        
        #region <Methods>

        public Node<C> AddNode(C p_Content)
        {
            var result = _NodePooler.GetObject();
            result.NodeContent = p_Content;
            return result;
        }
        
        public void RemoveNodeAt(int p_Index)
        {
            this[p_Index]?.RetrieveObject();
        }
        
        public void ClearNodes()
        {
            _NodePooler.RetrieveAllObject();
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
        protected void DisposeUnManaged()
        {
            _NodePooler.Dispose();
            _NodePooler = null;
            _NodeArray = null;
        }

        #endregion
    }
    
    public class Node<C> : PoolingObject<Node<C>>
    {
        #region <Consts>

        public static NodePool<C> SpawnPool(int p_PreloadCount)
        {
            return new NodePool<C>(p_PreloadCount);
        }

        #endregion
        
        #region <Fields>

        public C NodeContent;

        #endregion
        
        #region <Callbacks>

        public override void OnSpawning()
        {
        }

        public override void OnPooling()
        {
        }

        public override void OnRetrieved()
        {
            NodeContent = default;
        }

        #endregion
    }
}