using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BlackAm
{
    /// <summary>
    /// 우선순위 큐이면서 키로 각 데이터에 접근이 가능한 힙
    /// </summary>
    /// <typeparam name="K">컬렉션 키</typeparam>
    /// <typeparam name="V">컬렉션 값</typeparam>
    /// <typeparam name="P">컬렉션 소팅 기준</typeparam>
    public class PriorityCollection<K, V, P> where V: class
    {
        #region <Fields>

        /// <summary>
        /// 컬렉션 기조 컨테이너
        /// </summary>
        private List<HeapNode<K, V, P>> _container;

        /// <summary>
        /// pivot 노드
        /// </summary>
        private HeapNode<K, V, P> _pivot;
        
        /// <summary>
        /// 컬렉션 비교자
        /// </summary>
        private Comparer<P> _comparer;

        /// <summary>
        /// 현재 컬렉션에 등록된 데이터 셋 개수
        /// </summary>
        public int DataCount { get; private set; }

        /// <summary>
        /// 첫번째 이벤트
        /// </summary>
        public event EventHandler<PriorityQueueEventArgs<V>> FirstElementChanged;
        
        #endregion

        #region <Constructor>

        public PriorityCollection()
        {
            _container = new List<HeapNode<K, V, P>>();
            _container.Add(new HeapNode<K, V, P>()); // 인덱싱 편의를 위해 0 인덱스에 인스턴스 삽입
            _comparer = Comparer<P>.Default;
            _pivot = new HeapNode<K, V, P>();
        }

        #endregion

        #region <Callbacks>

        /// <summary>
        /// 힙 헤더에 변경사항이 발생한 경우 호출되는 이벤트
        /// </summary>
        private void OnHeapHeadChanged(V oldHead, V newHead)
        {
            if (oldHead != newHead)
            {
                FirstElementChanged?.Invoke(this, new PriorityQueueEventArgs<V>(oldHead, newHead));
            }
        }

        #endregion
        
        #region <Methods>

        /// <summary>
        /// 컨테이너를 비우는 메서드
        /// </summary>
        public void Clear()
        {
            _container.Clear();
            DataCount = 0;
        }

        /// <summary>
        /// 우선순위가 제일 높은 1번째 원소를 리턴하고, 힙을 우선순위별로 재정렬시키는 메서드.
        /// </summary>
        public V Dequeue()
        {
            V local = IsEmpty() ? default : DequeueImpl();
            V newHead = IsEmpty() ? default : _container[1].Value;
            OnHeapHeadChanged(default, newHead);
            return local;
        }

        /// <summary>
        /// 첫번째 원소를 리턴하고, 우선순위 큐를 재정렬하는 메서드
        /// </summary>
        private V DequeueImpl()
        {
            V result = _container[1].Value;
            _container[1] = _container[DataCount];
            _container[DataCount--] = _pivot;
            Heapify(1);
            return result;
        }
        
        /// <summary>
        /// 두 값을 비교해서, 왼쪽 서열이 높다면 참을 리턴하는 논리메서드
        /// </summary>
        protected bool IsHigher(P p1, P p2)
        {
            return (_comparer.Compare(p1, p2) < 1);
        }

        /// <summary>
        /// 컨테이너가 비었는지 검증하는 논리메서드
        /// </summary>
        public bool IsEmpty() => DataCount < 1;
        
        /// <summary>
        /// 힙 헤더를 리턴하는 메서드
        /// </summary>
        public V Peek()
        {
            return IsEmpty() ? default : _container[1].Value;
        }
        
        /// <summary>
        /// 지정한 우선도보다 높으면서 특정 논리 조건 Predicate[V]를 만족하는 값을 i번째 원소부터 검색하여 리턴하는 메서드
        /// 내부에서 인덱싱 방식으로 구성된 트리서칭을 함
        /// </summary>
        private V Search(P priority, int i, Predicate<V> match)
        {
            V result = default;
            if (IsHigher(_container[i].Priority, priority))
            {
                if (match(_container[i].Value))
                {
                    result = _container[i].Value;
                }
                
                // 배열로 구성된 힙
                int num = 2 * i;
                int num2 = num + 1;
                if ((result == null) && (num <= DataCount))
                {
                    result = this.Search(priority, num, match);
                }
                if ((result == null) && (num2 <= DataCount))
                {
                    result = this.Search(priority, num2, match);
                }
            }
            return result;
        }
        
        /// <summary>
        /// 지정한 우선도 보다 높으면서 match Predicate를 만족하는 값을 컨테이너에서 찾아 리턴하는 메서드
        /// </summary>
        public V FindByPriority(P priority, Predicate<V> match)
        {
            return IsEmpty() ? default : Search(priority, 1, match);
        }
        
        /// <summary>
        /// 컨테이너 내부의 i 번째, j 번째 노드를 교체하는 메서드
        /// </summary>
        private void Swap(int i, int j)
        {
            HeapNode<K, V, P> node = _container[i];
            _container[i] = _container[j];
            _container[j] = node;
        }
        
        /// <summary>
        /// 인덱스로 구성된 트리 힙을 i번째 노드부터 검색하여 정렬하는 메서드
        /// </summary>
        /// <param name="i">검색을 시작할 노드 인덱스</param>
        private void Heapify(int i)
        {
            // 왼쪽 서브 트리 노드 인덱스
            int subLeft = 2 * i;
            // 오른쪽 서브 트리 노드 인덱스
            int subRight = 2 * i + 1;

            int j = i;
            
            // 왼쪽 서브 트리 노드의 우선도가 더 높다면 교체한다.
            if ((subLeft <= DataCount) && IsHigher(_container[subLeft].Priority, _container[i].Priority))
            {
                j = subLeft;
            }
            // 오른쪽 서브 트리 노드의 우선도가 더 높다면 교체한다.
            if ((subRight <= DataCount) && IsHigher(_container[subRight].Priority, _container[j].Priority))
            {
                j = subRight;
            }
            
            // 교체 후, 교체된 서브 트리 루트로부터 힙 정렬을 단말노드까지 재귀호출해나간다.
            if (j != i)
            {
                Swap(i, j);
                Heapify(j);
            }
        }

        /// <summary>
        /// 값을 삽입하는 메서드
        /// </summary>
        /// <typeparam name="K">컬렉션 키</typeparam>
        /// <typeparam name="V">컬렉션 값</typeparam>
        /// <typeparam name="P">컬렉션 소팅 기준</typeparam>
        public void Enqueue(K key, V value, P priority)
        {
            V result = DataCount > 0 ? _container[1].Value : default;
            
            int subNode = ++DataCount;
            int subNodeRoot = subNode / 2;
            
            // 숫자를 맞춰주기 위해 pivot을 삽입한다.
            // 예를 들어, 컨테이너가 공백 상태에서 삽입하는 경우
            if (subNode == _container.Count)
            {
                _container.Add(_pivot);
            }
            
            // 신규 추가 노드와 베이스 노드의 우선도를 비교하여, 서브 노드쪽의 우선도가 낮아지도록 회전시킨다.
            while ((subNode > 1) && IsHigher(priority, _container[subNodeRoot].Priority))
            {
                // 회전이 진행됬다면, 기존의 베이스 노드에 있던 값을 서브 노드로 옮기고
                // 인덱스를 교체한다.
                _container[subNode] = _container[subNodeRoot];
                subNode = subNodeRoot;
                subNodeRoot = subNode / 2;
            }
            
            // 회전이 종료되었으면, 최종 인덱스에 값을 삽입한다.
            _container[subNode] = new HeapNode<K, V, P>(key, value, priority);
            V tryHead = _container[1].Value;
            if (!tryHead.Equals(result))
            {
                OnHeapHeadChanged(result, tryHead);
            }
        }

        /// <summary>
        /// 컨테이너로부터 지정한 키를 가진 노드를 삭제하는 메서드
        /// </summary>
        public V Remove(K key)
        {
            if (!IsEmpty())
            {
                V oldHead = _container[1].Value;
                for (int i = 1; i <= DataCount; i++)
                {
                    // 삭제할 노드를 검색한다.
                    if (_container[i].Key.Equals(key))
                    {
                        // 삭제할 노드를 마지막 노드로 옮기고, pivot으로 가린다.
                        V tryRemove = _container[i].Value;
                        Swap(i, DataCount);
                        _container[DataCount--] = _pivot;
                        Heapify(i);
                        
                        // 삭제로 인해, 힙 헤드가 변경된 경우
                        V tryHead = _container[1].Value;
                        if (!oldHead.Equals(tryHead))
                        {
                            OnHeapHeadChanged(oldHead, tryHead);
                        }
                        return tryRemove;
                    }
                }
            }
            
            return default;
        }

        /// <summary>
        /// 현재 컨테이너에 등록된 키 리스트를 리턴한다.
        /// </summary>
        public ReadOnlyCollection<K> Keys
        {
            get
            {
                List<K> list = new List<K>();
                for (int i = 1; i <= DataCount; i++)
                {
                    list.Add(_container[i].Key);
                }
                return new ReadOnlyCollection<K>(list);
            }
        }

        /// <summary>
        /// 현재 컨테이너에 등록된 값 리스트를 리턴한다.
        /// </summary>
        public ReadOnlyCollection<V> Values
        {
            get
            {
                List<V> list = new List<V>();
                for (int i = 1; i <= DataCount; i++)
                {
                    list.Add(_container[i].Value);
                }
                return new ReadOnlyCollection<V>(list);
            }
        }
        
        #endregion

        #region <Structs>
        
        /// <summary>
        /// 컬렉션에서 처리할 데이터 단위
        /// </summary>
        /// <typeparam name="KK">컬렉션 키</typeparam>
        /// <typeparam name="VV">컬렉션 값</typeparam>
        /// <typeparam name="PP">컬렉션 소팅 기준</typeparam>
        private struct HeapNode<KK, VV, PP>
        {
            public KK Key;
            public VV Value;
            public PP Priority;
            
            public HeapNode(KK p_Key, VV p_Value, PP p_Priority)
            {
                Key = p_Key;
                Value = p_Value;
                Priority = p_Priority;
            }
        }

        #endregion

        #region <Classes>
        
        /// <summary>
        /// 컬렉션 값이 변경된 경우 처리되는 이벤트에 사용되는 이벤트 프리셋
        /// </summary>
        /// <typeparam name="VV">컬렉션 값</typeparam>
        public sealed class PriorityQueueEventArgs<VV> : EventArgs
        {
            public VV NewFirstElement { get; private set; }
            public VV OldFirstElement { get; private set; }

            public PriorityQueueEventArgs(VV p_OldFirstElement, VV p_NewFirstElement)
            {
                OldFirstElement = p_OldFirstElement;
                NewFirstElement = p_NewFirstElement;
            }
        }
        
        #endregion
    }
}