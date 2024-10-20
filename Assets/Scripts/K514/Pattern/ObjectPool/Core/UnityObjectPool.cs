using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


namespace BlackAm
{
    /// <summary>
    /// 프리펩 리소스에 <c>T</c>컴포넌트를 붙이고 관리한다.
    /// </summary>
    /// <remarks>
    /// <para>풀링할 오브젝트들의 부모가 될 오브젝트에 <c>AddComponent</c>를 통해 붙이고, 수동으로 <see cref="Initialize()"/>함수를 호출해 초기화 해야한다.</para>
    /// <para>초기화시 <c>prefabName</c>을 반드시 지정해 줘야 한다.</para>
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public abstract class UnityPrefabObjectPool<T> : MonoBehaviour where T : MonoBehaviour
    {
        /// <summary>
        /// 초기화시 프리펩 이름 반드시 정해줘야함
        /// </summary>
        public string prefabName;

        private Stack<T> _pool;

        protected abstract void OnCreate(T obj);
        protected abstract void OnActive(T obj);
        protected abstract void OnPooled(T obj);

        public virtual UnityPrefabObjectPool<T> Initialize()
        {
            _pool = new Stack<T>();
            return this;
        }

        protected virtual void OnDestroy()
        {
            _pool = null;
        }

        public T GetObject()
        {
            if (_pool.Count == 0)
            {
                CreateObject();
            }

            var obj = _pool.Pop();
            OnActive(obj);
            return obj;
        }

        public void PoolObject(T obj)
        {
            if (obj == null)
            {
                Debug.LogError("Pool null reference");
                return;
            }
            OnPooled(obj);
            _pool.Push(obj);
        }

        private void CreateObject()
        {
            var prefabObj = LoadAssetManager.GetInstanceUnSafe.LoadAsset<GameObject>(ResourceType.GameObjectPrefab, ResourceLifeCycleType.WholeGame, prefabName).Item2;
            var obj = Instantiate(prefabObj, transform).AddComponent<T>();
            if(prefabObj == null)
                Debug.LogError($"Resource Load Error : {prefabName}");
            //var obj = Instantiate(Resources.Load<GameObject>(prefabName),transform).AddComponent<T>(); // 임시
            OnCreate(obj);
            OnPooled(obj);
            _pool.Push(obj);
        }
    }
}