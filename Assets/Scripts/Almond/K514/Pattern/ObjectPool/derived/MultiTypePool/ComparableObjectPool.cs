using System.Collections.Generic;
using UnityEngine;

namespace k514
{
    /// <summary>
    /// ObjectPooler<M>에 '풀링된 오브젝트를 구분할 수 있는 타입 SubType'를 추가한 구현체
    /// </summary>
    public class ObjectPooler<SubType, M> where M : PoolingObject<SubType, M>, new() where SubType : struct
    {
        #region <Fields>

        private Dictionary<SubType, ObjectPooler<M>> _PoolTable;
        private SubType[] _Enumerator;
        
        #endregion

        #region <Constructors>

        public ObjectPooler()
        {
            _PoolTable = new Dictionary<SubType, ObjectPooler<M>>();
            _Enumerator = SystemTool.GetEnumEnumerator<SubType>(SystemTool.GetEnumeratorType.ExceptNone);
        }

        #endregion

        #region <Index>

        public ObjectPooler<M> this[SubType p_SubType] => _PoolTable[p_SubType];

        #endregion

        #region <Methods>

        public void Preload(SubType p_SubType, int p_PreloadCount)
        {
            if (!_PoolTable.ContainsKey(p_SubType))
            {
                var spawnedPooler = new ObjectPooler<M>();
                spawnedPooler.PreloadPool(p_PreloadCount, p_PreloadCount);
                _PoolTable.Add(p_SubType, spawnedPooler);
            }
        }
        
        public void Preload(int p_PreloadCount)
        {
            foreach (var subType in _Enumerator)
            {
                Preload(subType, p_PreloadCount);
            }
        }

        /// <summary>
        /// 오브젝트 풀링으로부터 혹은 오브젝트를 신규 생성하여 리턴하는 메서드
        /// </summary>
        public M GetObject() => GetObject(default);

        /// <summary>
        /// 오브젝트 풀링으로부터 혹은 오브젝트를 신규 생성하여 리턴하는 메서드
        /// </summary>
        public M GetObject(SubType p_SubType)
        {
            var targetPooler = _PoolTable[p_SubType];
            var result = targetPooler.GenerateObject();
            result.Item2._SubType = p_SubType;
            return targetPooler.InitObject(result);
        }

        /// <summary>
        /// 지정한 타입을 가지는 풀러의 전회수 함수를 호출하는 메서드
        /// </summary>
        public void ClearPool(SubType p_SubType)
        {
            if (_PoolTable.ContainsKey(p_SubType))
            {
                var targetPooler = _PoolTable[p_SubType];
                targetPooler.RetrieveAllObject();
            }
        }
        
        /// <summary>
        /// 보유한 모든 풀러의 전회수 함수를 호출하는 메서드
        /// </summary>
        public void ClearPool()
        {
            foreach (var subType in _Enumerator)
            {
                ClearPool(subType);
            }
        }

        #endregion
    }

    /// <summary>
    /// PoolingObject<M>에 '풀링된 오브젝트를 구분할 수 있는 타입 K'를 추가한 구현체
    /// </summary>
    public abstract class PoolingObject<SubType, M> : PoolingObject<M> where M : PoolingObject<SubType, M>, new() where SubType : struct
    {
        #region <Fields>

        public SubType _SubType;

        #endregion
    }
}