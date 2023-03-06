using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace k514
{
    /// <summary>
    /// 로드된 프리팹으로부터 인스턴스를 만들고, 풀링으로 관리하는 싱글톤 매니저 클래스
    /// </summary>
    public class PrefabPoolingManager : SceneChangeEventSingleton<PrefabPoolingManager>
    {
        #region <Fields>
 
        /// <summary>
        /// 각 프리팹 인스턴스를 저장할 오브젝트 풀을 풀링하는 오브젝트 풀
        /// </summary>
        private ObjectPooler<PrefabInstancePool> _PoolPool;

        /// <summary>
        /// [{프리팹 이름, 프리펩 수명 타입, 프리팹 추가 타입}, 해당 프리팹의 인스턴스를 풀링할 오브젝트 풀] 2-1컬렉션
        /// </summary>
        public Dictionary<PrefabPoolingTool.PrefabIdentifyKey, PrefabInstancePool> _PoolCollection { get; private set; }

        /// <summary>
        /// 컬렉션에서 삭제할 키를 저장하는 리스트
        /// </summary>
        private List<PrefabPoolingTool.PrefabIdentifyKey> _RemoveReserveKeyGroup;

        private ResourceLifeCycleType[] _ResourceLifeCycleTypeEnumerator;
        
        #endregion
         
        #region <Callbacks>

        protected override void OnCreated()
        {
            _PoolPool = new ObjectPooler<PrefabInstancePool>();
            _PoolPool.PreloadPool(8, 8);
             
            _PoolCollection = new Dictionary<PrefabPoolingTool.PrefabIdentifyKey, PrefabInstancePool>(PrefabPoolingTool._PrefabIdentifyComparer);
            _RemoveReserveKeyGroup = new List<PrefabPoolingTool.PrefabIdentifyKey>();

            _ResourceLifeCycleTypeEnumerator = SystemTool.GetEnumEnumerator<ResourceLifeCycleType>(SystemTool.GetEnumeratorType.ExceptMaskNone);
        }

        public override void OnInitiate()
        {
        }

        public override async UniTask OnScenePreload()
        {
            await UniTask.CompletedTask;
        }

        public override void OnSceneStarted()
        {
             
        }
 
        public override void OnSceneTerminated()
        {
        }

        public override void OnSceneTransition()
        {
#if UNITY_EDITOR
            if (CustomDebug.PrintPrefabPolling)
            {
                Debug.Log($" PrefabPoolingManager : Retrieve Pooled Prefab Instance");
            }
#endif
            ReleasePrefab(ResourceLifeCycleType.Scene);
        }

        #endregion
          
        #region <Method/Preload>
 
        /// <summary>
        /// 프리팹 이름을 기반으로 프리팹을 로드하여 지정한 갯수만큼 오브젝트 풀에 미리 인스턴스화 시키는 메서드
        /// </summary>
        public (PrefabPoolingTool.PrefabIdentifyKey, List<PrefabInstance>) PreloadInstance(string p_PrefabName, ResourceLifeCycleType p_ResourceLifeCycleType, ResourceType p_ResourceType, int p_Count, PrefabPoolingTool.PrefabPoolingManagerPreset p_PrefabExtraDataPreset = default)
        {
            var _PoolingKey = new PrefabPoolingTool.PrefabIdentifyKey
            (
                p_PrefabName,
                p_ResourceLifeCycleType,
                p_PrefabExtraDataPreset
            );
             
            if (!_PoolCollection.TryGetValue(_PoolingKey, out var prefabTrackerPool))
            {
#if UNITY_EDITOR
                if (CustomDebug.PrintPrefabPolling)
                {
                    Debug.Log($"Name : {p_PrefabName} / Type : {p_ResourceLifeCycleType} / Number : {p_Count} prefab instant preloaded");
                }
#endif
                var resultTuple = LoadAssetManager.GetInstanceUnSafe.LoadAsset<GameObject>(p_ResourceType,
                    p_ResourceLifeCycleType, p_PrefabName);
                var targetPrefab = resultTuple.Item2;
                
#if UNITY_EDITOR
                if (CustomDebug.PrintPrefabPolling)
                {
                    if (targetPrefab == null)
                    {
                        Debug.Log($"Target Prefab name load failed : {p_PrefabName} / Preload");
                    }
                }
#endif
                prefabTrackerPool = _PoolPool.GetObject();
                _PoolCollection.Add(_PoolingKey, prefabTrackerPool);
                prefabTrackerPool.SetPrefab(_PoolingKey, new AssetPreset(targetPrefab, p_PrefabName, p_ResourceType, p_ResourceLifeCycleType));
            }
 
            return (_PoolingKey, prefabTrackerPool.Preload(p_Count, p_Count));
        }
         
        /// <summary>
        /// 프리팹 이름을 기반으로 프리팹을 로드하여 지정한 갯수만큼 오브젝트 풀에 미리 인스턴스화 시키는 메서드
        /// </summary>
        public async UniTask<(PrefabPoolingTool.PrefabIdentifyKey, List<PrefabInstance>)> PreloadInstanceAsync(string p_PrefabName, ResourceLifeCycleType p_ResourceLifeCycleType, ResourceType p_ResourceType, int p_Count, PrefabPoolingTool.PrefabPoolingManagerPreset p_PrefabExtraDataPreset = default)
        {
            var _PoolingKey = new PrefabPoolingTool.PrefabIdentifyKey
            (
                p_PrefabName,
                p_ResourceLifeCycleType,
                p_PrefabExtraDataPreset
            );
             
            if (!_PoolCollection.TryGetValue(_PoolingKey, out var prefabTrackerPool))
            {
#if UNITY_EDITOR
                if (CustomDebug.PrintPrefabPolling)
                {
                    Debug.Log($"Name : {p_PrefabName} / Type : {p_ResourceLifeCycleType} / Number : {p_Count} prefab instant preloaded");
                }
#endif
                var resultTuple = await LoadAssetManager.GetInstanceUnSafe.LoadAssetAsync<GameObject>(p_ResourceType,
                    p_ResourceLifeCycleType, p_PrefabName);
                var targetPrefab = resultTuple.Item2;
                
#if UNITY_EDITOR
                if (CustomDebug.PrintPrefabPolling)
                {
                    if (targetPrefab == null)
                    {
                        Debug.Log($"Target Prefab name load failed : {p_PrefabName} / Preload");
                    }
                }
#endif
                if (!_PoolCollection.ContainsKey(_PoolingKey))
                {
                    prefabTrackerPool = _PoolPool.GetObject();
                    _PoolCollection.Add(_PoolingKey, prefabTrackerPool);
                    prefabTrackerPool.SetPrefab(_PoolingKey, new AssetPreset(targetPrefab, p_PrefabName, p_ResourceType, p_ResourceLifeCycleType));
                }
                else
                {
                    prefabTrackerPool = _PoolCollection[_PoolingKey];
                }
            }
 
            return (_PoolingKey, prefabTrackerPool.Preload(p_Count, p_Count));
        }

        #endregion
        
        #region <Method/Pool>

        /// <summary>
        /// 프리팹 이름을 기반으로 프리팹을 로드하여 인스턴스화한 GameObject를 리턴하는 메서드
        /// </summary>
        public (PrefabInstance, PrefabPoolingTool.PrefabIdentifyKey) PoolInstance(string p_PrefabName, ResourceLifeCycleType p_ResourceLifeCycleType, ResourceType p_ResourceType, TransformTool.AffineCachePreset p_Affine, PrefabPoolingTool.PrefabPoolingManagerPreset p_PrefabExtraDataPreset = default)
        {
            var poolingKey = new PrefabPoolingTool.PrefabIdentifyKey
            (
                p_PrefabName,
                p_ResourceLifeCycleType,
                p_PrefabExtraDataPreset
            );
             
            if (!_PoolCollection.TryGetValue(poolingKey, out var prefabTrackerPool))
            {
#if UNITY_EDITOR
                if (CustomDebug.PrintPrefabPolling)
                {
                    Debug.Log($"Name : {p_PrefabName} / Type : {p_ResourceLifeCycleType} prefab pool added");
                }
#endif
                var resultTuple = LoadAssetManager.GetInstanceUnSafe.LoadAsset<GameObject>(p_ResourceType,
                    p_ResourceLifeCycleType, p_PrefabName);
                var targetPrefab = resultTuple.Item2;
                
#if UNITY_EDITOR
                if (CustomDebug.PrintPrefabPolling)
                {
                    if (targetPrefab == null)
                    {
                        Debug.LogError($"Target Prefab name load failed : {p_PrefabName} / Pooling");
                    }
                }
#endif
                prefabTrackerPool = _PoolPool.GetObject();
                if (prefabTrackerPool != null)
                {
                    _PoolCollection.Add(poolingKey, prefabTrackerPool);
                    prefabTrackerPool.SetPrefab(poolingKey, new AssetPreset(targetPrefab, p_PrefabName, p_ResourceType, p_ResourceLifeCycleType));
                    return (prefabTrackerPool.PopInstance(p_Affine), poolingKey);
                }
                else
                {
                    return default;
                }
            }
            else
            {
                return (prefabTrackerPool.PopInstance(p_Affine), poolingKey);
            }
        }
         
        /// <summary>
        /// 프리팹 이름을 기반으로 프리팹을 로드하여 인스턴스화한 GameObject에서 지정한 컴포넌트를 탐색해 리턴하는 메서드
        /// </summary>
        public (T, PrefabPoolingTool.PrefabIdentifyKey) PoolInstance<T>(string p_PrefabName, ResourceLifeCycleType p_ResourceLifeCycleType, ResourceType p_ResourceType, TransformTool.AffineCachePreset p_Affine, PrefabPoolingTool.PrefabPoolingManagerPreset p_PrefabExtraDataPreset = default) where T : Object
        {
            var result = PoolInstance(p_PrefabName, p_ResourceLifeCycleType, p_ResourceType, p_Affine, p_PrefabExtraDataPreset);
            if (ReferenceEquals(null, result.Item1))
            {
                return (default, result.Item2);
            }
            else
            {
                return (result.Item1.GetComponent<T>(), result.Item2);
            }
        }  
         
        /// <summary>
        /// 프리팹 이름을 기반으로 프리팹을 로드하여 인스턴스화한 GameObject에서 지정한 컴포넌트를 탐색해 리턴하는 메서드
        /// </summary>
        public async UniTask<(PrefabInstance, PrefabPoolingTool.PrefabIdentifyKey)> PoolInstanceAsync(string p_PrefabName, ResourceLifeCycleType p_ResourceLifeCycleType, ResourceType p_ResourceType, TransformTool.AffineCachePreset p_Affine, PrefabPoolingTool.PrefabPoolingManagerPreset p_PrefabExtraDataPreset = default)
        {
            var poolingKey = new PrefabPoolingTool.PrefabIdentifyKey
            (
                p_PrefabName,
                p_ResourceLifeCycleType,
                p_PrefabExtraDataPreset
            );
             
            if (!_PoolCollection.TryGetValue(poolingKey, out var prefabTrackerPool))
            {
#if UNITY_EDITOR
                if (CustomDebug.PrintPrefabPolling)
                {
                    Debug.Log($"Name : {p_PrefabName} / Type : {p_ResourceLifeCycleType} prefab pool added / Key Hash : {poolingKey.GetHashCode()}");
                }
#endif
                var resultTuple = await LoadAssetManager.GetInstanceUnSafe.LoadAssetAsync<GameObject>(p_ResourceType,
                    p_ResourceLifeCycleType, p_PrefabName);
                var targetPrefab = resultTuple.Item2;
                
#if UNITY_EDITOR
                if (CustomDebug.PrintPrefabPolling)
                {
                    if (targetPrefab == null)
                    {
                        Debug.LogError($"Target Prefab name load failed : {p_PrefabName} / Pooling");
                    }
                }
#endif
                if (_PoolCollection.TryGetValue(poolingKey, out prefabTrackerPool))
                {
                    return (prefabTrackerPool.PopInstance(p_Affine), poolingKey);
                }
                else
                {
                    prefabTrackerPool = _PoolPool.GetObject();
                    if (prefabTrackerPool != null)
                    {
                        _PoolCollection.Add(poolingKey, prefabTrackerPool);
                        prefabTrackerPool.SetPrefab(poolingKey, new AssetPreset(targetPrefab, p_PrefabName, p_ResourceType, p_ResourceLifeCycleType));
                        return (prefabTrackerPool.PopInstance(p_Affine), poolingKey);
                    }
                    else
                    {
                        return default;
                    }
                }
            }
            else
            {
                return (prefabTrackerPool.PopInstance(p_Affine), poolingKey);
            }
        }
         
        /// <summary>
        /// 프리팹 이름을 기반으로 프리팹을 로드하여 인스턴스화한 GameObject에서 지정한 컴포넌트를 탐색해 리턴하는 메서드
        /// </summary>
        public async UniTask<(T, PrefabPoolingTool.PrefabIdentifyKey)> PoolInstanceAsync<T>(string p_PrefabName, ResourceLifeCycleType p_ResourceLifeCycleType, ResourceType p_ResourceType, TransformTool.AffineCachePreset p_Affine, PrefabPoolingTool.PrefabPoolingManagerPreset p_PrefabExtraDataPreset = default) where T : Object
        {
            var tryUnit = await PoolInstanceAsync(p_PrefabName, p_ResourceLifeCycleType, p_ResourceType, p_Affine, p_PrefabExtraDataPreset);
            if (ReferenceEquals(null, tryUnit.Item1))
            {
                return (default, tryUnit.Item2);
            }
            else
            {
                return (tryUnit.Item1.GetComponent<T>(), tryUnit.Item2);
            }
        }  
         
        #endregion

        #region <Method/Pool/ShortCut>

        /// <summary>
        /// 프리팹 이름을 기반으로 프리팹을 로드하여 인스턴스화한 GameObject를 리턴하는 메서드
        /// </summary>
        public (PrefabInstance, PrefabPoolingTool.PrefabIdentifyKey) PoolInstance(string p_PrefabName, ResourceLifeCycleType p_ResourceLifeCycleType, ResourceType p_ResourceType, PrefabPoolingTool.PrefabPoolingManagerPreset p_PrefabExtraDataPreset = default)
        {
            return PoolInstance(p_PrefabName, p_ResourceLifeCycleType, p_ResourceType, Vector3.zero, p_PrefabExtraDataPreset);
        }
         
        /// <summary>
        /// 프리팹 이름을 기반으로 프리팹을 로드하여 인스턴스화한 GameObject에서 지정한 컴포넌트를 탐색해 리턴하는 메서드
        /// </summary>
        public (T, PrefabPoolingTool.PrefabIdentifyKey) PoolInstance<T>(string p_PrefabName, ResourceLifeCycleType p_ResourceLifeCycleType, ResourceType p_ResourceType, PrefabPoolingTool.PrefabPoolingManagerPreset p_PrefabExtraDataPreset = default) where T : Object
        {
            return PoolInstance<T>(p_PrefabName, p_ResourceLifeCycleType, p_ResourceType, Vector3.zero, p_PrefabExtraDataPreset);
        }  
         
        /// <summary>
        /// 프리팹 이름을 기반으로 프리팹을 로드하여 인스턴스화한 GameObject에서 지정한 컴포넌트를 탐색해 리턴하는 메서드
        /// </summary>
        public UniTask<(PrefabInstance, PrefabPoolingTool.PrefabIdentifyKey)> PoolInstanceAsync(string p_PrefabName, ResourceLifeCycleType p_ResourceLifeCycleType, ResourceType p_ResourceType, PrefabPoolingTool.PrefabPoolingManagerPreset p_PrefabExtraDataPreset = default)
        {
            return PoolInstanceAsync(p_PrefabName, p_ResourceLifeCycleType, p_ResourceType, Vector3.zero, p_PrefabExtraDataPreset);
        }
         
        /// <summary>
        /// 프리팹 이름을 기반으로 프리팹을 로드하여 인스턴스화한 GameObject에서 지정한 컴포넌트를 탐색해 리턴하는 메서드
        /// </summary>
        public UniTask<(T, PrefabPoolingTool.PrefabIdentifyKey)> PoolInstanceAsync<T>(string p_PrefabName, ResourceLifeCycleType p_ResourceLifeCycleType, ResourceType p_ResourceType, PrefabPoolingTool.PrefabPoolingManagerPreset p_PrefabExtraDataPreset = default) where T : Object
        {
            return PoolInstanceAsync<T>(p_PrefabName, p_ResourceLifeCycleType, p_ResourceType, Vector3.zero, p_PrefabExtraDataPreset);
        }  
    
        #endregion
        
        #region <Method/Retrieve>
 
        /// <summary>
        /// 지정하는 수명타입을 가지는 프리팹으로부터 파생된 인스턴스를 오브젝트 풀로 전부 회수시키는 메서드
        /// </summary>
        public void RetrievePrefab(ResourceLifeCycleType p_ResourceLifeCycleType)
        {
#if UNITY_EDITOR
            if (CustomDebug.PrintPrefabPolling)
            {
                Debug.Log($"Type : {p_ResourceLifeCycleType} / Try Retrieve");
            }
#endif
             
            foreach (var _tryKeyPair in _PoolCollection)
            {
                if (_tryKeyPair.Key._ResourceLifeCycleType == p_ResourceLifeCycleType)
                {
                    _tryKeyPair.Value.RetrieveInstance();
                }
            }
        }

        /// <summary>
        /// 지정하는 이름을 가지는 프리팹으로부터 파생된 인스턴스를 오브젝트 풀로 전부 회수시키는 메서드
        /// </summary>
        public void RetrievePrefab(string p_PrefabName)
        {
#if UNITY_EDITOR
            if (CustomDebug.PrintPrefabPolling)
            {
                Debug.Log($"Prefab Name : {p_PrefabName} / Try Retrieve");
            }
#endif
             
            foreach (var _tryKeyPair in _PoolCollection)
            {
                if (_tryKeyPair.Key._PrefabName == p_PrefabName)
                {
                    _tryKeyPair.Value.RetrieveInstance();
                }
            }
        }

        /// <summary>
        /// 지정하는 컴포넌트 타입을 가지는 프리팹으로부터 파생된 인스턴스를 오브젝트 풀로 전부 회수시키는 메서드
        /// </summary>
        public void RetrievePrefab(PrefabPoolingTool.PrefabPoolingManagerPreset p_PoolingManagerPreset)
        {
#if UNITY_EDITOR
            if (CustomDebug.PrintPrefabPolling)
            {
                Debug.Log($"Preset : {p_PoolingManagerPreset} / Try Retrieve");
            }
#endif
             
            foreach (var _tryKeyPair in _PoolCollection)
            {
                if (_tryKeyPair.Key._PrefabExtraPreset == p_PoolingManagerPreset)
                {
                    _tryKeyPair.Value.RetrieveInstance();
                }
            }
        }
         
        /// <summary>
        /// 지정하는 이름 및 컴포넌트를 가지는 프리팹으로부터 파생된 인스턴스를 오브젝트 풀로 전부 회수시키는 메서드
        /// </summary>
        public void RetrievePrefab(string p_PrefabName, PrefabPoolingTool.PrefabPoolingManagerPreset p_PoolingManagerPreset)
        {
#if UNITY_EDITOR
            if (CustomDebug.PrintPrefabPolling)
            {
                Debug.Log($"Name : {p_PrefabName} / Preset : {p_PoolingManagerPreset} / Try Retrieve");
            }
#endif

            foreach (var _tryType in _ResourceLifeCycleTypeEnumerator)
            {
                var assembledKey = new PrefabPoolingTool.PrefabIdentifyKey
                (
                    p_PrefabName,
                    _tryType,
                    p_PoolingManagerPreset
                );

                if (_PoolCollection.TryGetValue(assembledKey, out var o_PrefabPooler))
                {
#if UNITY_EDITOR
                    if (CustomDebug.PrintPrefabPolling)
                    {
                        Debug.Log($"  ** Name : {p_PrefabName} / Preset : {p_PoolingManagerPreset} / Type {_tryType} retrieved");
                    }
#endif
                    o_PrefabPooler.RetrieveInstance();
                }
            }
        }

        /// <summary>
        /// 지정한 타입과 이름 가진 프리팹으로부터 파생된 인스턴스를 오브젝트 풀로 전부 회수시키는 메서드
        /// </summary>
        public void RetrievePrefab(string p_PrefabName, ResourceLifeCycleType p_ResourceLifeCycleType)
        {
#if UNITY_EDITOR
            if (CustomDebug.PrintPrefabPolling)
            {
                Debug.Log($"Name : {p_PrefabName} / Type : {p_ResourceLifeCycleType} / Try Retrieve");
            }
#endif
             
            foreach (var _tryKeyPair in _PoolCollection)
            {
                var tryKey = _tryKeyPair.Key;
                if (tryKey._PrefabName == p_PrefabName 
                    && tryKey._ResourceLifeCycleType == p_ResourceLifeCycleType)
                {
                    _tryKeyPair.Value.RetrieveInstance();
                }
            }
        }
          
        /// <summary>
        /// 지정한 타입과 컴포넌트를 가진 프리팹으로부터 파생된 인스턴스를 오브젝트 풀로 전부 회수시키는 메서드
        /// </summary>
        public void RetrievePrefab(PrefabPoolingTool.PrefabPoolingManagerPreset p_PoolingManagerPreset, ResourceLifeCycleType p_ResourceLifeCycleType)
        {
#if UNITY_EDITOR
            if (CustomDebug.PrintPrefabPolling)
            {
                Debug.Log($"Preset : {p_PoolingManagerPreset} / Type : {p_ResourceLifeCycleType} / Try Retrieve");
            }
#endif
             
            foreach (var _tryKeyPair in _PoolCollection)
            {
                var tryKey = _tryKeyPair.Key;
                if (tryKey._ResourceLifeCycleType == p_ResourceLifeCycleType
                    && tryKey._PrefabExtraPreset == p_PoolingManagerPreset)
                {
                    _tryKeyPair.Value.RetrieveInstance();
                }
            }
        }
          
        /// <summary>
        /// 지정한 키를 가진 프리팹으로부터 파생된 인스턴스를 오브젝트 풀로 전부 회수시키는 메서드
        /// </summary>
        public void RetrievePrefab(PrefabPoolingTool.PrefabIdentifyKey p_PoolingKey)
        {
            RetrievePrefab(p_PoolingKey._PrefabName, p_PoolingKey._PrefabExtraPreset, p_PoolingKey._ResourceLifeCycleType);
        }
         
        /// <summary>
        /// 지정한 타입과 이름, 컴포넌트를 가진 프리팹으로부터 파생된 인스턴스를 오브젝트 풀로 전부 회수시키는 메서드
        /// </summary>
        public void RetrievePrefab(string p_PrefabName, PrefabPoolingTool.PrefabPoolingManagerPreset p_PoolingManagerPreset, ResourceLifeCycleType p_ResourceLifeCycleType)
        {
#if UNITY_EDITOR
            if (CustomDebug.PrintPrefabPolling)
            {
                Debug.Log($"Name : {p_PrefabName} / Preset : {p_PoolingManagerPreset} / Type : {p_ResourceLifeCycleType} / Try Retrieve");
            }
#endif
             
            foreach (var _tryKeyPair in _PoolCollection)
            {
                var tryKey = _tryKeyPair.Key;
                if (tryKey._PrefabName == p_PrefabName 
                    && tryKey._ResourceLifeCycleType == p_ResourceLifeCycleType
                    && tryKey._PrefabExtraPreset == p_PoolingManagerPreset)
                {
                    _tryKeyPair.Value.RetrieveInstance();
                }
            }
        }
         
        /// <summary>
        /// 모든 풀링된 인스턴스를 회수시키는 메서드
        /// </summary>
        public void RetrievePrefab()
        {
#if UNITY_EDITOR
            if (CustomDebug.PrintPrefabPolling)
            {
                Debug.Log($"Try all Retrieve");
            }
#endif
 
            foreach (var resourceLifeCycleType in _ResourceLifeCycleTypeEnumerator)
            {
                RetrievePrefab(resourceLifeCycleType);
            }
        }
 
        #endregion
 
        #region <Method/Release>
 
        /// <summary>
        /// 지정하는 수명타입을 가지는 프리팹 및 파생된 인스턴스를 전부 파기시키는 메서드
        /// </summary>
        public void ReleasePrefab(ResourceLifeCycleType p_ResourceLifeCycleType)
        {
#if UNITY_EDITOR
            if (CustomDebug.PrintPrefabPolling)
            {
                Debug.Log($"Type : {p_ResourceLifeCycleType} / Try Remove");
            }
#endif
             
            foreach (var _tryKeyPair in _PoolCollection)
            {
                if (_tryKeyPair.Key._ResourceLifeCycleType == p_ResourceLifeCycleType)
                {
                    _RemoveReserveKeyGroup.Add(_tryKeyPair.Key);
                }
            }
 
            foreach (var _tryRemoveKey in _RemoveReserveKeyGroup)
            {
#if UNITY_EDITOR
                if (CustomDebug.PrintPrefabPolling)
                {
                    Debug.Log($"  ** Name : {_tryRemoveKey._PrefabName} / Type {p_ResourceLifeCycleType} removed");
                }
#endif
                 
                var targetPool = _PoolCollection[_tryRemoveKey];
                targetPool.RetrieveObject();
                _PoolCollection.Remove(_tryRemoveKey);
            }
 
            _RemoveReserveKeyGroup.Clear();
        }
                  
        /// <summary>
        /// 지정하는 이름을 가지는 프리팹 및 파생된 인스턴스를 전부 파기시키는 메서드
        /// </summary>
        public void ReleasePrefab(string p_PrefabName)
        {
#if UNITY_EDITOR
            if (CustomDebug.PrintPrefabPolling)
            {
                Debug.Log($"PrefabName : {p_PrefabName} / Try Remove");
            }
#endif
             
            foreach (var _tryKeyPair in _PoolCollection)
            {
                if (_tryKeyPair.Key._PrefabName == p_PrefabName)
                {
                    _RemoveReserveKeyGroup.Add(_tryKeyPair.Key);
                }
            }
 
            foreach (var _tryRemoveKey in _RemoveReserveKeyGroup)
            {
#if UNITY_EDITOR
                if (CustomDebug.PrintPrefabPolling)
                {
                    Debug.Log($"  ** Name : {_tryRemoveKey._PrefabName} removed");
                }
#endif
                 
                var targetPool = _PoolCollection[_tryRemoveKey];
                targetPool.RetrieveObject();
                _PoolCollection.Remove(_tryRemoveKey);
            }
 
            _RemoveReserveKeyGroup.Clear();
        }
                           
        /// <summary>
        /// 지정하는 컴포넌트를 가지는 프리팹 및 파생된 인스턴스를 전부 파기시키는 메서드
        /// </summary>
        public void ReleasePrefab(PrefabPoolingTool.PrefabPoolingManagerPreset p_PoolingManagerPreset)
        {
#if UNITY_EDITOR
            if (CustomDebug.PrintPrefabPolling)
            {
                Debug.Log($"  ** Preset : {p_PoolingManagerPreset} / Try Remove");
            }
#endif
             
            foreach (var _tryKeyPair in _PoolCollection)
            {
                if (_tryKeyPair.Key._PrefabExtraPreset == p_PoolingManagerPreset)
                {
                    _RemoveReserveKeyGroup.Add(_tryKeyPair.Key);
                }
            }
 
            foreach (var _tryRemoveKey in _RemoveReserveKeyGroup)
            {
#if UNITY_EDITOR
                if (CustomDebug.PrintPrefabPolling)
                {
                    Debug.Log($"  ** Preset : {p_PoolingManagerPreset} removed");
                }
#endif
                 
                var targetPool = _PoolCollection[_tryRemoveKey];
                targetPool.RetrieveObject();
                _PoolCollection.Remove(_tryRemoveKey);
            }
 
            _RemoveReserveKeyGroup.Clear();
        }
         
        /// <summary>
        /// 지정하는 이름 및 컴포넌트를 가지는 프리팹 및 파생된 인스턴스를 전부 파기시키는 메서드
        /// </summary>
        public void ReleasePrefab(string p_PrefabName, PrefabPoolingTool.PrefabPoolingManagerPreset p_PoolingManagerPreset)
        {
#if UNITY_EDITOR
            if (CustomDebug.PrintPrefabPolling)
            {
                Debug.Log($"Name : {p_PrefabName} / ComponentType : {p_PoolingManagerPreset} / Try Remove");
            }
#endif

            foreach (var _tryType in _ResourceLifeCycleTypeEnumerator)
            {
                var assembledKey = new PrefabPoolingTool.PrefabIdentifyKey
                (
                    p_PrefabName,
                    _tryType,
                    p_PoolingManagerPreset
                );

                if (_PoolCollection.TryGetValue(assembledKey, out var o_PrefabPooler))
                {
#if UNITY_EDITOR
                    if (CustomDebug.PrintPrefabPolling)
                    {
                        Debug.Log($"  ** Name : {p_PrefabName} / Preset : {p_PoolingManagerPreset} / Type {_tryType} removed");
                    }
#endif
                     
                    o_PrefabPooler.RetrieveObject();
                    _PoolCollection.Remove(assembledKey);
                }
            }
        }
         
        /// <summary>
        /// 지정한 타입과 이름을 가진 프리팹 및 파생된 인스턴스를 전부 파기시키는 메서드
        /// </summary>
        public void ReleasePrefab(string p_PrefabName, ResourceLifeCycleType p_ResourceLifeCycleType)
        {
#if UNITY_EDITOR
            if (CustomDebug.PrintPrefabPolling)
            {
                Debug.Log($"Name : {p_PrefabName} / Type : {p_ResourceLifeCycleType} / Try Remove");
            }
#endif
 
            foreach (var _tryKeyPair in _PoolCollection)
            {
                if (_tryKeyPair.Key._PrefabName == p_PrefabName 
                    && _tryKeyPair.Key._ResourceLifeCycleType == p_ResourceLifeCycleType)
                {
                    _RemoveReserveKeyGroup.Add(_tryKeyPair.Key);
                }
            }
 
            foreach (var _tryRemoveKey in _RemoveReserveKeyGroup)
            {
#if UNITY_EDITOR
                if (CustomDebug.PrintPrefabPolling)
                {
                    Debug.Log($"  ** Name : {_tryRemoveKey._PrefabName} / Type {p_ResourceLifeCycleType} / removed");
                }
#endif
                var targetRecord = _PoolCollection[_tryRemoveKey];
                targetRecord.RetrieveObject();
                _PoolCollection.Remove(_tryRemoveKey);
            }
 
            _RemoveReserveKeyGroup.Clear();
        }

        /// <summary>
        /// 지정한 타입과 컴포넌트를 가진 프리팹 및 파생된 인스턴스를 전부 파기시키는 메서드
        /// </summary>
        public void ReleasePrefab(ResourceLifeCycleType p_ResourceLifeCycleType, PrefabPoolingTool.PrefabPoolingManagerPreset p_PoolingManagerPreset)
        {
#if UNITY_EDITOR
            if (CustomDebug.PrintPrefabPolling)
            {
                Debug.Log($"Type : {p_ResourceLifeCycleType} / Preset : {p_PoolingManagerPreset} / Try Remove");
            }
#endif
 
            foreach (var _tryKeyPair in _PoolCollection)
            {
                if (_tryKeyPair.Key._ResourceLifeCycleType == p_ResourceLifeCycleType
                    && _tryKeyPair.Key._PrefabExtraPreset == p_PoolingManagerPreset)
                {
                    _RemoveReserveKeyGroup.Add(_tryKeyPair.Key);
                }
            }
 
            foreach (var _tryRemoveKey in _RemoveReserveKeyGroup)
            {
#if UNITY_EDITOR
                if (CustomDebug.PrintPrefabPolling)
                {
                    Debug.Log($"  ** Type {p_ResourceLifeCycleType} / Preset : {p_PoolingManagerPreset} /  removed");
                }
#endif
                var targetRecord = _PoolCollection[_tryRemoveKey];
                targetRecord.RetrieveObject();
                _PoolCollection.Remove(_tryRemoveKey);
            }
 
            _RemoveReserveKeyGroup.Clear();
        }
         
        /// <summary>
        /// 지정한 키를 가진 프리팹 및 파생된 인스턴스를 전부 파기시키는 메서드
        /// </summary>
        public void ReleasePrefab(PrefabPoolingTool.PrefabIdentifyKey p_PoolingKey)
        {
            ReleasePrefab(p_PoolingKey._PrefabName, p_PoolingKey._PrefabExtraPreset, p_PoolingKey._ResourceLifeCycleType);
        }
         
        /// <summary>
        /// 지정한 타입과 이름 및 컴포넌트를 가진 프리팹 및 파생된 인스턴스를 전부 파기시키는 메서드
        /// </summary>
        public void ReleasePrefab(string p_PrefabName, PrefabPoolingTool.PrefabPoolingManagerPreset p_PoolingManagerPreset, ResourceLifeCycleType p_ResourceLifeCycleType)
        {
#if UNITY_EDITOR
            if (CustomDebug.PrintPrefabPolling)
            {
                Debug.Log($"Name : {p_PrefabName} / Type : {p_ResourceLifeCycleType} / Preset : {p_PoolingManagerPreset} / Try Remove");
            }
#endif
 
            foreach (var _tryKeyPair in _PoolCollection)
            {
                if (_tryKeyPair.Key._PrefabName == p_PrefabName 
                    && _tryKeyPair.Key._ResourceLifeCycleType == p_ResourceLifeCycleType
                    && _tryKeyPair.Key._PrefabExtraPreset == p_PoolingManagerPreset)
                {
                    _RemoveReserveKeyGroup.Add(_tryKeyPair.Key);
                }
            }
 
            foreach (var _tryRemoveKey in _RemoveReserveKeyGroup)
            {
#if UNITY_EDITOR
                if (CustomDebug.PrintPrefabPolling)
                {
                    Debug.Log($"  ** Name : {_tryRemoveKey._PrefabName} / Type {p_ResourceLifeCycleType} / Preset : {p_PoolingManagerPreset} /  removed");
                }
#endif
                var targetRecord = _PoolCollection[_tryRemoveKey];
                targetRecord.RetrieveObject();
                _PoolCollection.Remove(_tryRemoveKey);
            }
 
            _RemoveReserveKeyGroup.Clear();
        }
 
        /// <summary>
        /// 모든 풀링된 인스턴스를 파기시키는 메서드
        /// </summary>
        public void ReleasePrefab()
        {
#if UNITY_EDITOR
            if (CustomDebug.PrintPrefabPolling)
            {
                Debug.Log($"Try all Remove");
            }
#endif

            foreach (var resourceLifeCycleType in _ResourceLifeCycleTypeEnumerator)
            {
                ReleasePrefab(resourceLifeCycleType);
            }
        }
         
        #endregion

        #region <Methods>

#if UNITY_EDITOR
        public void PrintContainer()
        {
            var _poolCollectionKeySet = _PoolCollection.Keys;
            foreach (var keyElement in _poolCollectionKeySet)
            {
                Debug.Log(keyElement);
            }
        }
#endif

        #endregion

        #region <Disposable>

        /// <summary>
        /// 인스턴스가 파기될 때 수행할 작업을 기술한다.
        /// </summary>
        protected override void DisposeUnManaged()
        {
            _PoolPool.Dispose();

            base.DisposeUnManaged();
        }

        #endregion
    }
 
    /// <summary>
    /// 특정한 프리팹과 매핑되어 해당 프리팹의 인스턴스화 및 파기를 담당하는 오브젝트 풀.
    /// 보통 PrefabPoolingManager에 의해 생성되고 제어되는 메타적 특성을 가지며, 단독으로 사용할 수 없다.
    /// </summary>
    public class PrefabInstancePool : PoolingObject<PrefabInstancePool>
    {
        #region <Fields>
 
        /// <summary>
        /// 인스턴스화된 오브젝트를 관리하는 오브젝트 풀
        /// </summary>
        private UnityObjectPooler<PrefabInstance> _UnitPooler;

        /// <summary>
        /// 해당 오브젝트 프리팹 혹은 해당 프리팹 풀을 가리키는 키값
        /// </summary>
        private PrefabPoolingTool.PrefabIdentifyKey _ThisKey;
        
        #endregion
         
        #region <Callbacks>
 
        public override void OnSpawning()
        {
            _UnitPooler = new UnityObjectPooler<PrefabInstance>();
        }
 
        public override void OnPooling()
        {
        }
 
        public override void OnRetrieved()
        {
            if (_UnitPooler._AssetPreset.IsValid)
            {
                _ThisKey = new PrefabPoolingTool.PrefabIdentifyKey();
                _UnitPooler.ClearPool();
            }
        }

        #endregion
 
        #region <Methods>

        public UnityObjectPooler<PrefabInstance> GetObjectPooler() => _UnitPooler;
 
        /// <summary>
        /// 프리팹으로부터 지정한 갯수만큼 인스턴스화를 수행하고, 바로 풀링하는 메서드
        /// </summary>
        public List<PrefabInstance> Preload(int p_Count, int p_CheckNumber)
        {
            return _UnitPooler.PreloadPool(p_Count, p_CheckNumber);
        }
         
        /// <summary>
        /// 지정한 절대좌표에 레이캐스팅을 수행하여, 충돌표면에 프리팹으로부터 인스턴스화를 수행하고 리턴시키는 메서드
        /// </summary>
        public PrefabInstance PopInstance(TransformTool.AffineCachePreset p_AffinePreset)
        {
            return _UnitPooler.GetObject(p_AffinePreset);
        }
 
        /// <summary>
        /// 프리팹으로부터 인스턴스화된 게임 오브젝트를 회수시키는 메서드
        /// </summary>
        public void RetrieveInstance()
        {
            _UnitPooler.RetrieveAllObject();
        }

        #endregion
         
        #region <Method/Setter>
 
        /// <summary>
        /// 인스턴스화 시킬 프리팹을 지정하는 체인메서드
        /// </summary>
        public PrefabInstancePool SetPrefab(PrefabPoolingTool.PrefabIdentifyKey p_Key, AssetPreset p_AssetPreset)
        {
            _ThisKey = p_Key;
            _UnitPooler.SetPoolerPreset(p_AssetPreset, p_Key);
            return this;
        }
         
        #endregion

        #region <Disposable>
        
        /// <summary>
        /// 인스턴스가 파기될 때 수행할 작업을 기술한다.
        /// </summary>
        protected override void DisposeUnManaged()
        {
            _UnitPooler.Dispose();
            
            base.DisposeUnManaged();
        }

        #endregion
    }
}