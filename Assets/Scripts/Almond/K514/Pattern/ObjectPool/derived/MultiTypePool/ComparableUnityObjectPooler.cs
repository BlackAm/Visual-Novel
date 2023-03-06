using System.Collections.Generic;
using UnityEngine;

namespace k514
{
    /// <summary>
    /// UnityObjectPooler<M>에 '풀링된 오브젝트를 구분할 수 있는 타입 SubType'를 추가한 구현체
    /// </summary>
    public class UnityObjectPooler<SubType, M> where M : PoolingUnityObject<SubType, M>, new() where SubType : struct
    {
        #region <Fields>

        private Dictionary<SubType, UnityObjectPooler<M>> _PoolTable;
        private SubType[] _Enumerator;

        #endregion

        #region <Constructors>

        public UnityObjectPooler()
        {
            _PoolTable = new Dictionary<SubType, UnityObjectPooler<M>>();
            _Enumerator = SystemTool.GetEnumEnumerator<SubType>(SystemTool.GetEnumeratorType.ExceptNone);
        }

        #endregion

        #region <Index>

        public UnityObjectPooler<M> this[SubType p_SubType] => _PoolTable[p_SubType];

        #endregion
        
        #region <Methods>

        public void AddPooler(SubType p_SubType, int p_PreloadCount)
        {
            if (!_PoolTable.ContainsKey(p_SubType))
            {
                var spawnedPooler = new UnityObjectPooler<M>();
                spawnedPooler.PreloadPool(p_PreloadCount, p_PreloadCount);
                _PoolTable.Add(p_SubType, spawnedPooler);
            }
        }
        
        public void AddPooler(int p_PreloadCount)
        {
            foreach (var subType in _Enumerator)
            {
                AddPooler(subType, p_PreloadCount);
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
            return GetObject(p_SubType, new TransformTool.AffineCachePreset(Vector3.zero));
        }

        /// <summary>
        /// 오브젝트 풀링으로부터 혹은 오브젝트를 신규 생성하여 리턴하는 메서드
        /// </summary>
        public M GetObject(SubType p_SubType, TransformTool.AffineCachePreset p_Affine)
        {
            var targetPooler = _PoolTable[p_SubType];
            var result = targetPooler.GenerateObject(p_Affine);
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
    public abstract class PoolingUnityObject<SubType, M> : PoolingUnityObject<M> where M : PoolingUnityObject<SubType, M>, new() where SubType : struct
    {
        #region <Fields>

        public SubType _SubType;

        #endregion
    }
}