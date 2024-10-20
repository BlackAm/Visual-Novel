using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BlackAm
{
    /// <summary>
    /// 에셋번들을 통해 에셋을 로드하는 경우
    /// </summary>
    public class LoadAssetBundle : ILoadAsset
    {
        #region <Fields>

        /// <summary>
        /// [어셋번들 이름, 어셋번들 추적 인스턴스] 2컬렉션
        /// 각 추적 인스턴스는 참조 카운터가 0이 된 경우에, 메모리로부터 릴리스되며 해당 이벤트를 _AssetBundleCollection에 전달한다.
        /// </summary>
        private Dictionary<string, AssetBundleTracker> _AssetBundleTrackerCollection;

        /// <summary>
        /// [에셋번들로 로드된 리소스 수명 타입, 에셋번들 이름, 에셋 이름, 참조카운터] 3컬렉션
        /// </summary>
        private Dictionary<ResourceLifeCycleType, Dictionary<string, Dictionary<string, RefCounter>>> _AssetBundleRefCountTable;

        /// <summary>
        /// 각 에셋번들의 관계 정보를 가지는 객체
        /// </summary>
        public AssetBundleManifest _AssetBundleManifest;

        #endregion

        #region <Callbacks>

        public void OnCreated()
        {
            // 컬렉션 초기화
            _AssetBundleTrackerCollection = new Dictionary<string, AssetBundleTracker>();
            _AssetBundleRefCountTable = new Dictionary<ResourceLifeCycleType, Dictionary<string, Dictionary<string, RefCounter>>>();

            // 에셋번들 이름 리스트를 가져온다
            if (SystemTool.TryGetEnumEnumerator<ResourceLifeCycleType>(SystemTool.GetEnumeratorType.ExceptNone, out var o_Enumerator))
            {
#if RESOURCE_LIST_TABLE_INTO_GAMETABLE
                // 리소스 데이터가 게임 테이블로 취급되는 경우, AssetBundleNameList 보다 먼저 GameTableDataBundleName 번들을 컬렉션에 준비해야한다.
                foreach (var tryType in o_Enumerator)
                {
                    if (!_AssetBundleRefCountTable.ContainsKey(tryType))
                    {
                        _AssetBundleRefCountTable.Add(tryType, new Dictionary<string, Dictionary<string, RefCounter>>());
                    }
                    _AssetBundleRefCountTable[tryType].Add(ResourceListData.GameTableModeResourceListTableBundleName, new Dictionary<string, RefCounter>());
                }
#endif

                var assetBundleNameSet = ResourceListData.GetInstanceUnSafe.AssetBundleNameList;
                // 각 리소스 수명 타입에 대해 리스트로부터 에셋번들 이름을 추가해준다.
                foreach (var tryType in o_Enumerator)
                {
                    if (!_AssetBundleRefCountTable.ContainsKey(tryType))
                    {
                        _AssetBundleRefCountTable.Add(tryType, new Dictionary<string, Dictionary<string, RefCounter>>());
                    }

                    foreach (var tryBundleName in assetBundleNameSet)
                    {
                        if (tryBundleName != null && !_AssetBundleRefCountTable[tryType].ContainsKey(tryBundleName))
                        {
                            _AssetBundleRefCountTable[tryType].Add(tryBundleName, new Dictionary<string, RefCounter>());
                        }
                    }
                }
            }

            // 메타 에셋 번들 초기화
            TryGetDefaultAssetBundle();
        }

        #endregion

        #region <Methods>

        /// <summary>
        /// 다른 번들의 정보를 가지는 기본 에셋번들을 로드하는 메서드
        /// </summary>
        private void TryGetDefaultAssetBundle()
        {
            var bundlePath = SystemMaintenance.GetBundleFullPathOnPlayPlatform() + SystemMaintenance.DefaultBundleName;
            if (File.Exists(bundlePath))
            {
#if UNITY_EDITOR
                if (CustomDebug.PrintLoadAssetManager)
                {
                    Debug.LogWarning($"[AssetBundle] 기저번들을 로드합니다. : {bundlePath}");
                }
#endif
                var tryBundle = AssetBundle.LoadFromFile(bundlePath);
                var spawnedTracker = new AssetBundleTracker(this, SystemMaintenance.DefaultBundleName, bundlePath, tryBundle);
                _AssetBundleTrackerCollection.Add(SystemMaintenance.DefaultBundleName, spawnedTracker);
                _AssetBundleManifest = spawnedTracker.AssetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            }
        }

        /// <summary>
        /// 다른 번들의 정보를 가지는 기본 에셋번들을 릴리스하는 메서드
        /// </summary>
        public void TryUnloadDefaultAssetBundle()
        {
            if (_AssetBundleTrackerCollection.ContainsKey(SystemMaintenance.DefaultBundleName))
            {
                _AssetBundleManifest = null;
                var targetTracker = _AssetBundleTrackerCollection[SystemMaintenance.DefaultBundleName];
                targetTracker.TryDecreaseAssetRefCount(0);
            }
        }

        /// <summary>
        /// 특정 에셋 번들이 현재 해당 로드 매니저에 의해 추적중이면, 해당 추적 객체를 리턴하는 메서드.
        /// 추적 중이지 않다면, 추적하도록 추적 객체를 생성한다.
        /// </summary>
        private AssetBundleTracker TryGetAssetBundle(string p_AssetBundleName)
        {
            if (string.IsNullOrEmpty(p_AssetBundleName))
            {
#if UNITY_EDITOR
                if (CustomDebug.PrintLoadAssetManager)
                {
                    Debug.LogWarning($"Asset Bundle Load Failed");
                }
#endif
                return null;
            }
            else
            {
                // 이미 이전에 로드된 번들이라면 추적객체가 컬렉션에 등록되어 있음.
                if (_AssetBundleTrackerCollection.TryGetValue(p_AssetBundleName, out var o_AssetBundleTracker))
                {
                    return o_AssetBundleTracker;
                }
                // 추적 객체가 존재하지 않는 경우, 추적 객체를 생성 후 리턴한다.
                else
                {
                    // AssetBundle.LoadFromFile 가 제일 빠르지만, ChunckBase 및 None 기반 번들에만 적용가능
                    var bundlePath = SystemMaintenance.GetBundleFullPathOnPlayPlatform() + p_AssetBundleName;
#if UNITY_EDITOR
                    if (CustomDebug.PrintLoadAssetManager)
                    {
                        Debug.LogWarning($"Asset Bundle Load Manager : find bundle at {bundlePath}");
                    }
#endif

                    if (File.Exists(bundlePath))
                    {
                        var spawnedTracker = new AssetBundleTracker(this, p_AssetBundleName, bundlePath);
                        _AssetBundleTrackerCollection[p_AssetBundleName] = spawnedTracker;
                        spawnedTracker.TryLoadBundle();
                        return _AssetBundleTrackerCollection[p_AssetBundleName];
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// 특정 에셋 번들이 현재 해당 로드 매니저에 의해 추적중이면, 해당 추적 객체를 리턴하는 메서드.
        /// 추적 중이지 않다면, 추적하도록 추적 객체를 생성한다.
        /// </summary>
        private async UniTask<AssetBundleTracker> TryGetAssetBundleAsync(string p_AssetBundleName)
        {
            if (string.IsNullOrEmpty(p_AssetBundleName))
            {
#if UNITY_EDITOR
                if (CustomDebug.PrintLoadAssetManager)
                {
                    Debug.LogWarning($"Asset Bundle Load Failed");
                }
#endif
                return null;
            }
            else
            {
                // 이미 이전에 로드된 번들이라면 추적객체가 컬렉션에 등록되어 있음.
                if (_AssetBundleTrackerCollection.TryGetValue(p_AssetBundleName, out var o_AssetBundleTracker))
                {
                    await UniTask.WaitUntil(() => o_AssetBundleTracker._IsValid).WithCancellation(SystemMaintenance._SystemTaskCancellationToken);
                    return o_AssetBundleTracker;
                }
                // 추적 객체가 존재하지 않는 경우, 추적 객체를 생성 후 리턴한다.
                else
                {
                    // AssetBundle.LoadFromFile 가 제일 빠르지만, ChunckBase 및 None 기반 번들에만 적용가능
                    var bundlePath = SystemMaintenance.GetBundleFullPathOnPlayPlatform() + p_AssetBundleName;
#if UNITY_EDITOR
                    if (CustomDebug.PrintLoadAssetManager)
                    {
                        Debug.LogWarning($"Asset Bundle Load Manager : find bundle at {bundlePath}");
                    }
#endif

                    if (File.Exists(bundlePath))
                    {
                        var spawnedTracker = new AssetBundleTracker(this, p_AssetBundleName, bundlePath);
                        _AssetBundleTrackerCollection[p_AssetBundleName] = spawnedTracker;
                        await spawnedTracker.TryLoadBundleAsync().WithCancellation(SystemMaintenance._SystemTaskCancellationToken);
                        return _AssetBundleTrackerCollection[p_AssetBundleName];
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// 에셋 이름으로부터 해당 에셋을 포함하는 번들 이름을 찾아 리턴하는 메서드
        /// 만약 지정한 에셋이 리소스리스트 테이블이었던 경우에는, 전용 번들 이름을 리턴한다.
        /// </summary>
        public string GetAssetBundleName(string p_AssetName)
        {
            var resourceAssetName = ResourceListData.GetInstanceUnSafe.GetTableFileName(TableTool.TableNameType.Alter, true);
            
            if (resourceAssetName == p_AssetName)
            {
                return ResourceListData.GameTableModeResourceListTableBundleName;
            }
            else
            {
                return ResourceListData.GetInstanceUnSafe.GetTable()[p_AssetName].GetAssetBundleName();
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// 현재 메모리에 존재하는 에셋번들 리스트를 출력하는 메서드
        /// </summary>
        public void PrintLoadedAssetBundleList()
        {
            Debug.Log($"*** Current Loaded AssetBundle List ***");
            foreach (var assetBundlePair in _AssetBundleTrackerCollection)
            {
                Debug.Log($"Bundle Name : {assetBundlePair.Key}  /  Ref Count : {assetBundlePair.Value.LoadedAssetCount} ");
            }
        }

        /// <summary>
        /// 현재 메모리에 존재하는 에셋번들 및 각 번들 내부의 참조중인 에셋과 참조 횟수를 출력하는 메서드
        /// </summary>
        public void PrintLoadedAssetBundleResourceList()
        {
            Debug.Log($"*** Current Loaded AssetBundle Resource List ***");
            foreach (var recourceLifeCycleTypePair in _AssetBundleRefCountTable)
            {
                foreach (var assetBundleNamePair in recourceLifeCycleTypePair.Value)
                {
                    foreach (var assetNamePair in assetBundleNamePair.Value)
                    {
                        var refCounter = assetNamePair.Value;
                        var refCount = refCounter.RefCount;
                        if (refCount > 0)
                        {
                            Debug.Log($" * Asset Name : [{recourceLifeCycleTypePair.Key}][{assetBundleNamePair.Key}][{assetNamePair.Key}] / Count : {refCount}");
                        }
                    }
                }
            }
        }
#endif

        #endregion

        #region <Method/Load>

        /// <summary>
        /// 지정한 이름의 에셋을 에셋 번들 리스트에서 찾아 지정한 수명 타입으로 로드하는 메서드
        /// </summary>
        /// <param name="p_ResourceType">에셋 리소스 분류 타입</param>
        /// <param name="p_ResourceLifeCycleType">로드할 에셋의 수명 타입</param>
        /// <param name="p_AssetName">로드할 에셋 이름</param>
        /// <typeparam name="T">로드할 에셋의 타입</typeparam>
        public T LoadAsset<T>(ResourceType p_ResourceType, ResourceLifeCycleType p_ResourceLifeCycleType, string p_AssetName) where T : Object
        {
            var bundleName = GetAssetBundleName(p_AssetName);
#if UNITY_EDITOR
            if (CustomDebug.PrintLoadAssetManager)
            {
                Debug.LogWarning($"Asset Bundle Load Manager : Load Asset : {p_AssetName} / bundleName : {bundleName} / ResourceType : {p_ResourceType} / LifeCycle : {p_ResourceLifeCycleType}");
            }
#endif
            // 해당 번들 이름을 추적하는 추적객체를 생성한다.
            var assetBundleTracker = TryGetAssetBundle(bundleName);
            if (assetBundleTracker != null)
            {
                if (_AssetBundleRefCountTable[p_ResourceLifeCycleType][bundleName].ContainsKey(p_AssetName))
                {
                    _AssetBundleRefCountTable[p_ResourceLifeCycleType][bundleName][p_AssetName].RefCount++;
                }
                else
                {
                    _AssetBundleRefCountTable[p_ResourceLifeCycleType][bundleName].Add(p_AssetName, new RefCounter(p_AssetName));
                }
                _AssetBundleTrackerCollection[bundleName].LoadedAssetCount++;

                return assetBundleTracker.AssetBundle.LoadAsset<T>(p_AssetName);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 지정한 이름의 에셋을 에셋 번들 리스트에서 찾아 지정한 수명 타입으로 비동기 로드하는 메서드
        /// </summary>
        /// <param name="p_ResourceType">에셋 리소스 분류 타입</param>
        /// <param name="p_ResourceLifeCycleType">로드할 에셋의 수명 타입</param>
        /// <param name="p_AssetName">로드할 에셋 이름</param>
        /// <typeparam name="T">로드할 에셋의 타입</typeparam>
        public async UniTask<T> LoadAssetAsync<T>(ResourceType p_ResourceType, ResourceLifeCycleType p_ResourceLifeCycleType,
            string p_AssetName) where T : Object
        {
            var bundleName = GetAssetBundleName(p_AssetName);
#if UNITY_EDITOR
            if (CustomDebug.PrintLoadAssetManager)
            {
                Debug.LogWarning($"Asset Bundle Load Manager : Load Asset Async : {p_AssetName} / bundleName : {bundleName} / ResourceType : {p_ResourceType} / LifeCycle : {p_ResourceLifeCycleType}");
            }
#endif
            // 해당 번들 이름을 추적하는 추적객체를 생성한다.
            var assetBundleTracker = await TryGetAssetBundleAsync(bundleName).WithCancellation(SystemMaintenance._SystemTaskCancellationToken);
            if (assetBundleTracker != null)
            {
                if (_AssetBundleRefCountTable[p_ResourceLifeCycleType][bundleName].ContainsKey(p_AssetName))
                {
                    _AssetBundleRefCountTable[p_ResourceLifeCycleType][bundleName][p_AssetName].RefCount++;
                }
                else
                {
                    _AssetBundleRefCountTable[p_ResourceLifeCycleType][bundleName].Add(p_AssetName, new RefCounter(p_AssetName));
                }
                _AssetBundleTrackerCollection[bundleName].LoadedAssetCount++;

                var tryAsset = assetBundleTracker.AssetBundle.LoadAssetAsync<T>(p_AssetName);
                await tryAsset.WithCancellation(SystemMaintenance._SystemTaskCancellationToken)
                    .WithCancellation(SystemMaintenance._SystemTaskCancellationToken);
                return tryAsset.asset as T;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 지정한 이름의 멀티 에셋을 에셋 번들 리스트에서 찾아 지정한 수명 타입으로 로드하는 메서드
        /// </summary>
        /// <param name="p_ResourceType">에셋 리소스 분류 타입</param>
        /// <param name="p_ResourceLifeCycleType">로드할 에셋의 수명 타입</param>
        /// <param name="p_AssetName">로드할 에셋 이름</param>
        /// <typeparam name="T">로드할 에셋의 타입</typeparam>
        public T[] LoadMultipleAsset<T>(ResourceType p_ResourceType, ResourceLifeCycleType p_ResourceLifeCycleType, string p_AssetName) where T : Object
        {
            var bundleName = GetAssetBundleName(p_AssetName);
#if UNITY_EDITOR
            if (CustomDebug.PrintLoadAssetManager)
            {
                Debug.LogWarning($"Asset Bundle Load Manager : Load Asset : {p_AssetName} / bundleName : {bundleName} / ResourceType : {p_ResourceType} / LifeCycle : {p_ResourceLifeCycleType}");
            }
#endif
            // 해당 번들 이름을 추적하는 추적객체를 생성한다.
            var assetBundleTracker = TryGetAssetBundle(bundleName);
            if (assetBundleTracker != null)
            {
                if (_AssetBundleRefCountTable[p_ResourceLifeCycleType][bundleName].ContainsKey(p_AssetName))
                {
                    _AssetBundleRefCountTable[p_ResourceLifeCycleType][bundleName][p_AssetName].RefCount++;
                }
                else
                {
                    _AssetBundleRefCountTable[p_ResourceLifeCycleType][bundleName].Add(p_AssetName, new RefCounter(p_AssetName));
                }
                _AssetBundleTrackerCollection[bundleName].LoadedAssetCount++;

                return assetBundleTracker.AssetBundle.LoadAssetWithSubAssets<T>(p_AssetName);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 지정한 이름의 멀티 에셋을 에셋 번들 리스트에서 찾아 지정한 수명 타입으로 비동기 로드하는 메서드
        /// </summary>
        /// <param name="p_ResourceType">에셋 리소스 분류 타입</param>
        /// <param name="p_ResourceLifeCycleType">로드할 에셋의 수명 타입</param>
        /// <param name="p_AssetName">로드할 에셋 이름</param>
        /// <typeparam name="T">로드할 에셋의 타입</typeparam>
        public async UniTask<T[]> LoadMultipleAssetAsync<T>(ResourceType p_ResourceType, ResourceLifeCycleType p_ResourceLifeCycleType,
            string p_AssetName) where T : Object
        {
            var bundleName = GetAssetBundleName(p_AssetName);
#if UNITY_EDITOR
            if (CustomDebug.PrintLoadAssetManager)
            {
                Debug.LogWarning($"Asset Bundle Load Manager : Load Asset Async : {p_AssetName} / bundleName : {bundleName} / ResourceType : {p_ResourceType} / LifeCycle : {p_ResourceLifeCycleType}");
            }
#endif
            // 해당 번들 이름을 추적하는 추적객체를 생성한다.
            var assetBundleTracker = await TryGetAssetBundleAsync(bundleName).WithCancellation(SystemMaintenance._SystemTaskCancellationToken);
            if (assetBundleTracker != null)
            {
                if (_AssetBundleRefCountTable[p_ResourceLifeCycleType][bundleName].ContainsKey(p_AssetName))
                {
                    _AssetBundleRefCountTable[p_ResourceLifeCycleType][bundleName][p_AssetName].RefCount++;
                }
                else
                {
                    _AssetBundleRefCountTable[p_ResourceLifeCycleType][bundleName].Add(p_AssetName, new RefCounter(p_AssetName));
                }
                _AssetBundleTrackerCollection[bundleName].LoadedAssetCount++;

                var tryAsset = assetBundleTracker.AssetBundle.LoadAssetWithSubAssetsAsync<T>(p_AssetName);
                await tryAsset.WithCancellation(SystemMaintenance._SystemTaskCancellationToken).WithCancellation(SystemMaintenance._SystemTaskCancellationToken);
                return (from object asset in tryAsset.allAssets select (T)asset).ToArray();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 지정한 이름의 씬을 에셋 번들 리스트에서 찾아, 지정한 수명 타입으로 로드하는 메서드
        /// 다만, 유니티 에셋 번들에서는 SceneManager를 통해 호출할 수 있는 에셋번들의 씬 이름을 배열 형태로만 가져올 수 있기
        /// 때문에 리턴값이 string 배열이다.
        /// </summary>
        /// <param name="p_ResourceLifeCycleType">로드할 씬의 수명</param>
        /// <param name="p_AssetName">로드할 씬 이름</param>
        public async UniTask<string[]> LoadAllSceneAsset(ResourceLifeCycleType p_ResourceLifeCycleType, string p_AssetName)
        {
            var bundleName = GetAssetBundleName(p_AssetName);
#if UNITY_EDITOR
            if (CustomDebug.PrintLoadAssetManager)
            {
                Debug.LogWarning($"Asset Bundle Load Manager : Load Scene {p_AssetName} / bundleName {bundleName} / {p_ResourceLifeCycleType}");
            }
#endif
            // 해당 번들 이름을 추적하는 추적객체를 생성한다.
            var assetBundleTracker = await TryGetAssetBundleAsync(bundleName).WithCancellation(SystemMaintenance._SystemTaskCancellationToken);
            if (assetBundleTracker != null)
            {
                if (_AssetBundleRefCountTable[p_ResourceLifeCycleType][bundleName].ContainsKey(p_AssetName))
                {
                    _AssetBundleRefCountTable[p_ResourceLifeCycleType][bundleName][p_AssetName].RefCount++;
                }
                else
                {
                    _AssetBundleRefCountTable[p_ResourceLifeCycleType][bundleName].Add(p_AssetName, new RefCounter(p_AssetName));
                }
                _AssetBundleTrackerCollection[bundleName].LoadedAssetCount++;
                assetBundleTracker.SetSceneNameGroup(assetBundleTracker.AssetBundle.GetAllScenePaths());

                return assetBundleTracker.SceneNameGroup;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 지정한 이름의 씬을 에셋 번들 리스트에서 찾아, 지정한 수명 타입으로 로드하는 메서드
        /// 다만, 유니티 에셋 번들에서는 SceneManager를 통해 호출할 수 있는 에셋번들의 씬 이름을 배열 형태로만 가져올 수 있기
        /// 때문에 씬 이름을 통해 배열에서 씬 호출에 필요한 경로를 찾아 리턴한다.
        /// </summary>
        /// <param name="p_ResourceLifeCycleType">로드할 씬의 수명</param>
        /// <param name="p_AssetName">로드할 씬 이름</param>
        public async UniTask<string> LoadSceneAsset(ResourceLifeCycleType p_ResourceLifeCycleType, string p_AssetName)
        {
            var pathSet = await LoadAllSceneAsset(p_ResourceLifeCycleType, p_AssetName).WithCancellation(SystemMaintenance._SystemTaskCancellationToken);

            if (pathSet != null)
            {
                foreach (var compareScenePath in pathSet)
                {
                    if (compareScenePath.Contains($"/{p_AssetName}"))
                    {
                        return compareScenePath;
                    }
                }
            }
            return null;
        }

        #endregion

        #region <Method/Unload>

        /// <summary>
        /// 지정한 수명타입을 가지고 있는 리소스 프리셋을 제거하고, 그 숫자만큼 프리셋이 참조하던
        /// 에셋번들의 참조카운터를 갱신하는 메서드.
        ///
        /// 갱신과정에서 에셋번들의 참조카운터가 0이 되면 해당 에셋번들은 릴리스된다.
        ///
        /// 따라서, 에셋번들을 구성할 때는 가능한 한 수명타입 기준으로 구성하는게 메모리 관리에 좋을 것
        /// 
        /// </summary>
        public void UnloadAsset(ResourceLifeCycleType p_ResourceLifeCycleType)
        {
#if UNITY_EDITOR
            if (CustomDebug.PrintLoadAssetManager)
            {
                Debug.LogWarning($"Asset Bundle Load Scene : Unload Asset {p_ResourceLifeCycleType}");
            }
#endif

            var targetBundleCollection = _AssetBundleRefCountTable[p_ResourceLifeCycleType];
            foreach (var assetBundleNamePair in targetBundleCollection)
            {
                var tryBundleName = assetBundleNamePair.Key;
                if (_AssetBundleTrackerCollection.TryGetValue(tryBundleName, out var o_Tracker))
                {
                    var targetBundlePresetGroup = assetBundleNamePair.Value;
                    var wholeRefCount = 0;
                    foreach (var assetNamePair in targetBundlePresetGroup)
                    {
                        var refCounter = assetNamePair.Value;
                        wholeRefCount += refCounter.RefCount;
                        refCounter.RefCount = 0;
                    }

                    if (wholeRefCount > 0)
                    {
                        o_Tracker.TryDecreaseAssetRefCount(wholeRefCount);
                    }
                }
            }
        }

        public void UnloadAsset(AssetPreset p_AssetPreset)
        {
            var tryAssetName = p_AssetPreset.AssetName;
            var tryResourceLifeCycleType = p_AssetPreset.ResourceLifeCycleType;

#if UNITY_EDITOR
            if (CustomDebug.PrintLoadAssetManager)
            {
                Debug.Log($"Asset Bundle Load Manager : Unload AssetName {tryAssetName} / {tryResourceLifeCycleType}");
            }
#endif
            foreach (var recourceLifeCycleTypePair in _AssetBundleRefCountTable)
            {
                if (tryResourceLifeCycleType == recourceLifeCycleTypePair.Key)
                {
                    foreach (var assetBundleNamePair in recourceLifeCycleTypePair.Value)
                    {
                        if (_AssetBundleTrackerCollection.TryGetValue(assetBundleNamePair.Key, out var o_Tracker))
                        {
                            foreach (var assetNamePair in assetBundleNamePair.Value)
                            {
                                if (tryAssetName == assetNamePair.Key)
                                {
                                    var refCounter = assetNamePair.Value;
                                    o_Tracker.TryDecreaseAssetRefCount(refCounter.RefCount);
                                    refCounter.RefCount = 0;
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region <Structs>

        /// <summary>
        /// 특정 에셋번들을 이름으로 추적하면서, 참조 카운터를 세는 클래스
        /// 에셋번들 로드 매니저의 TryGetAssetBundle에 의해 생성되며, 참조 카운트가 0이 되면
        /// 에셋번들을 언로드 시키고 소멸한다.
        /// </summary>
        public class AssetBundleTracker
        {
            #region <Fields>

            /// <summary>
            /// 추적 대상 번들 이름
            /// </summary>
            public string AssetBundleName { get; private set; }

            /// <summary>
            /// 추적 대상 번들 풀패스
            /// </summary>
            public string AssetBundleFullPath { get; private set; }

            /// <summary>
            /// 추적 대상 번들
            /// </summary>
            public AssetBundle AssetBundle { get; private set; }

            /// <summary>
            /// 추적 대상 번들과 의존 관계에 있는 번들을 소유한 추적 객체 리스트
            /// </summary>
            public List<AssetBundleTracker> Dependencies;

            /// <summary>
            /// 해당 인스턴스를 포함하는 마스터노드
            /// </summary>
            private LoadAssetBundle _MasterNode;

#if UNITY_EDITOR
            /// <summary>
            /// LoadedAssetCount BackingFields
            /// </summary>
            public int __LoadedAssetCount;

            /// <summary>
            /// 추적 대상 번들로의 참조 카운터
            /// </summary>
            public int LoadedAssetCount
            {
                set
                {
                    __LoadedAssetCount = value;
                    if (CustomDebug.PrintLoadAssetManager)
                    {
                        Debug.Log($" * Ref Count Updated of {AssetBundleName} Bundle : {__LoadedAssetCount}");
                    }
                }
                get => __LoadedAssetCount;
            }
#else
            /// <summary>
            /// 추적 대상 번들로의 참조 카운터
            /// </summary>
            public int LoadedAssetCount;
#endif

            /// <summary>
            /// 유니티 에셋번들은 SceneManager 용 씬 이름을 배열로 관리하고 있기에 번들 추적 객체에 씬 이름 그룹을 캐싱해두고
            /// 사용하도록 한다.
            /// </summary>
            public string[] SceneNameGroup { get; private set; }

            /// <summary>
            /// 상호 의존하는 에셋번들끼리는 무한 루프가 발생할 가능성이 있기에 추적객체 삭제에 관한 onceFlag를 만들어 방지한다.
            /// </summary>
            private bool _OnceFlag;

            /// <summary>
            /// 동시성 문제를 피하기 위한 유효성 검증 플래그
            /// </summary>
            public bool _IsValid => AssetBundle != null;

            #endregion

            #region <Constructors>

            public AssetBundleTracker(LoadAssetBundle p_MasterNode, string p_AssetBundleName, string p_AssetBundleFullPath)
            {
#if UNITY_EDITOR
                if (CustomDebug.PrintLoadAssetManager)
                {
                    Debug.Log($"[Tracker Spawned] : {p_AssetBundleName}");
                }
#endif
                _MasterNode = p_MasterNode;
                AssetBundleName = p_AssetBundleName;
                AssetBundleFullPath = p_AssetBundleFullPath;
            }

            public AssetBundleTracker(LoadAssetBundle p_MasterNode, string p_AssetBundleName, string p_AssetBundleFullPath, AssetBundle p_AssetBundle)
            {
#if UNITY_EDITOR
                if (CustomDebug.PrintLoadAssetManager)
                {
                    Debug.Log($"[Tracker Spawned] : {p_AssetBundleName}");
                }
#endif
                _MasterNode = p_MasterNode;
                AssetBundleName = p_AssetBundleName;
                AssetBundleFullPath = p_AssetBundleFullPath;
                AssetBundle = p_AssetBundle;
            }

            #endregion

            #region <Methods>

            public void TryLoadBundle()
            {
                var tryManifest = _MasterNode._AssetBundleManifest;
#if UNITY_EDITOR
                if (CustomDebug.PrintLoadAssetManager)
                {
                    Debug.LogWarning($"[AssetBundle] : {AssetBundleName} 번들 로드를 시도합니다.");
                }
#endif
                var thisDependencies = tryManifest.GetDirectDependencies(AssetBundleName);
                if (thisDependencies.Length > 0)
                {
                    if (thisDependencies.Length > 0 && Dependencies == null)
                    {
                        Dependencies = new List<AssetBundleTracker>();
#if UNITY_EDITOR
                        if (CustomDebug.PrintLoadAssetManager)
                        {
                            Debug.Log($"** Dependencies of {AssetBundleName} **");
                            foreach (var dependency in thisDependencies)
                            {
                                Debug.Log($" - {dependency}");
                            }
                        }
#endif
                        foreach (var directDependency in thisDependencies)
                        {
                            var targetBundleTracker = _MasterNode.TryGetAssetBundle(directDependency);
                            if (targetBundleTracker != null)
                            {
                                targetBundleTracker.LoadedAssetCount++;
                                Dependencies.Add(targetBundleTracker);
                            }
                        }
                    }
                }

                var tryBundle = AssetBundle.LoadFromFile(AssetBundleFullPath);
                AssetBundle = tryBundle;
            }

            /// <summary>
            /// 메타 에셋 번들로부터 각 에셋번들의 의존성을 가지는 에셋번들 리스트를 초기화시키는 메서드
            /// </summary>
            public async UniTask TryLoadBundleAsync()
            {
                await UniTask.SwitchToMainThread();

                var tryManifest = _MasterNode._AssetBundleManifest;
                var thisDependencies = tryManifest.GetDirectDependencies(AssetBundleName);
                if (thisDependencies.Length > 0)
                {
                    Dependencies = new List<AssetBundleTracker>();
#if UNITY_EDITOR
                    if (CustomDebug.PrintLoadAssetManager)
                    {
                        Debug.Log($"** Dependencies of {AssetBundleName} **");
                        foreach (var dependency in Dependencies)
                        {
                            Debug.Log($" - {dependency.AssetBundleName}");
                        }
                    }
#endif
                    var dependencyLoadTask = new List<UniTask<AssetBundleTracker>>();
                    foreach (var directDependency in thisDependencies)
                    {
                        dependencyLoadTask.Add(_MasterNode.TryGetAssetBundleAsync(directDependency).WithCancellation(SystemMaintenance._SystemTaskCancellationToken));
                    }

                    var taskResult = await UniTask.WhenAll(dependencyLoadTask);
                    foreach (var targetBundleTracker in taskResult)
                    {
                        if (targetBundleTracker != null)
                        {
                            targetBundleTracker.LoadedAssetCount++;
                            Dependencies.Add(targetBundleTracker);
                        }
                    }
                }

                var tryBundle = AssetBundle.LoadFromFileAsync(AssetBundleFullPath);
                await tryBundle.WithCancellation(SystemMaintenance._SystemTaskCancellationToken);
                AssetBundle = tryBundle.assetBundle;
            }

            /// <summary>
            /// 번들의 씬 이름 그룹을 세트하는 메서드
            /// </summary>
            public void SetSceneNameGroup(string[] p_SceneNameGroup)
            {
                SceneNameGroup = p_SceneNameGroup;
            }

            /// <summary>
            /// 추적 대상 에셋 번들로의 참조 카운터가 0가 되는 경우, 에셋번들을 릴리스하고 해당 오브젝트를 파기한다.
            /// </summary>
            public void TryDecreaseAssetRefCount(int p_Offset)
            {
                if (p_Offset > 0)
                {
                    LoadedAssetCount -= p_Offset;
                }

                if (LoadedAssetCount < 1)
                {
                    if (!_OnceFlag)
                    {
                        _OnceFlag = true;
#if UNITY_EDITOR
                        if (CustomDebug.PrintLoadAssetManager)
                        {
                            if (LoadedAssetCount < 0)
                            {
                                Debug.LogError($"AssetBundle Reference Counter reached negative value in AssetBundle Tracker of '{AssetBundleName}' ");
                            }
                            Debug.Log($"AssetBundle '{AssetBundleName}' and AssetBundle Tracker of '{AssetBundleName}' have left memory");
                        }
#endif
                        if (AssetBundle != null)
                        {
                            AssetBundle.Unload(true);
                        }
                        SetSceneNameGroup(null);
                        AssetBundle = null;
                        _MasterNode._AssetBundleTrackerCollection.Remove(AssetBundleName);
                        if (Dependencies != null)
                        {
                            foreach (var dependency in Dependencies)
                            {
#if UNITY_EDITOR
                                if (CustomDebug.PrintLoadAssetManager)
                                {
                                    Debug.Log($"Break dependency '{AssetBundleName}' => '{dependency.AssetBundleName}'");
                                }
#endif
                                dependency.TryDecreaseAssetRefCount(1);
                            }
                        }
                    }
                }
            }

            #endregion
        }

        #endregion

        #region <Classess>

        public class RefCounter
        {
            public string AssetName;
            public int RefCount;

            public RefCounter(string p_AssetName)
            {
                AssetName = p_AssetName;
                RefCount = 1;
            }
        }

        #endregion
    }
}