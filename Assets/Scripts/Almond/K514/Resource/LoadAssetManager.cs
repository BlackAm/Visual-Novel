using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace k514
{
    /// <summary>
    /// 에셋을 로드하는 기능을 제어하는 매니저 클래스
    /// </summary>
    public class LoadAssetManager : AsyncSingleton<LoadAssetManager>
    {
        #region <Consts>

        /// <summary>
        /// 로드 방식에 따라 각 로드 방식을 제어하는 매니저를 컬렉션으로 관리한다.
        /// </summary>
        private Dictionary<LoadManagerType, ILoadAsset> AssetLoaderGroup;

        #endregion

        #region <Fields>

        private bool _AssetBundleLoadOccurFlag;
        private bool _UnityResourceLoadOccurFlag;

        #endregion
        
        #region <Enums>

        /// <summary>
        /// 로드 매니저 타입
        /// </summary>
        public enum LoadManagerType
        {
            /// <summary>
            /// 에셋 번들 매니저
            /// </summary>
            AssetBundleManager, 
            
            /// <summary>
            /// 유니티 리소스 폴더 매니저
            /// </summary>
            UnityResourceManager
        }

        #endregion
        
        #region <Callbacks>

        protected override async UniTask OnCreated()
        {
            // 각 타입의 로드 매니저를 초기화하고 컬렉팅 및 초기화
            AssetLoaderGroup = new Dictionary<LoadManagerType, ILoadAsset>();
            AssetLoaderGroup.Add(LoadManagerType.AssetBundleManager, new LoadAssetBundle());
            AssetLoaderGroup.Add(LoadManagerType.UnityResourceManager, new LoadUnityResource());

            _AssetBundleLoadOccurFlag = false;
            _UnityResourceLoadOccurFlag = false;

            await UniTask.SwitchToMainThread();
            foreach (var loadAssetManager in AssetLoaderGroup)
            {
                loadAssetManager.Value.OnCreated();
            }

            await UniTask.CompletedTask;
        }

        public override async UniTask OnInitiate()
        {
            await UniTask.CompletedTask;
        }

        /// <summary>
        /// 인스턴스가 파기될 때 수행할 작업을 기술한다.
        /// </summary>
        protected override void DisposeUnManaged()
        {
            UnloadAsset();

            base.DisposeUnManaged();
        }

        #endregion

        #region <Methods>

        /// <summary>
        /// 각 리소스 타입의 로드 타입에 맞는 로드 매니저를 리턴하는 메서드.
        /// </summary>
        public ILoadAsset GetLoadAssetManager(ResourceType p_ResourceType)
        {
            var tryAssetLoadType = p_ResourceType.GetResourceLoadType();
            switch (tryAssetLoadType)
            {
                case AssetLoadType.FromAssetBundle:
                    return AssetLoaderGroup[LoadManagerType.AssetBundleManager];
                default :
                    return AssetLoaderGroup[LoadManagerType.UnityResourceManager];
            }
        }
        
        /// <summary>
        /// 에셋 번들 로드 매니저를 리턴하는 메서드
        /// </summary>
        public LoadAssetBundle GetAssetBundleLoadManager() => AssetLoaderGroup[LoadManagerType.AssetBundleManager] as LoadAssetBundle;
       
        /// <summary>
        /// 유니티 리소스 로드 매니저를 리턴하는 메서드
        /// </summary>
        public LoadUnityResource GetUnityResourceLoadManager() => AssetLoaderGroup[LoadManagerType.UnityResourceManager] as LoadUnityResource;
        
        #endregion

        #region <Method/Load>

        /// <summary>
        /// 지정한 타입에 적절한 에셋을 로드하는 메서드
        /// </summary>
        public (AssetPreset, T) LoadAsset<T>(AssetLoadType p_AssetLoadType, ResourceType p_ResourceType, ResourceLifeCycleType p_ResourceLifeCycleType, string p_AssetName) where T : Object
        {
            switch (p_AssetLoadType)
            {
                case AssetLoadType.FromAssetBundle :
                {
                    _AssetBundleLoadOccurFlag = true;
                    // 에셋 이름을 그대로 넘기는 것으로, 에셋 번들 로드 매니저에서 에셋 번들을 탐색하는 방식
                    var resultObject = AssetLoaderGroup[LoadManagerType.AssetBundleManager].LoadAsset<T>(p_ResourceType, p_ResourceLifeCycleType, p_AssetName);
                    var resultPreset = new AssetPreset(resultObject, p_AssetName, p_ResourceType, p_ResourceLifeCycleType);
                    return (resultPreset, resultObject);
                }
                case AssetLoadType.FromUnityResource :
                    _UnityResourceLoadOccurFlag = true;
                    // 정상적으로 ResourceListData 테이블이 초기화 된 경우
                    if (ResourceListData.GetInstanceUnSafe.TryGetUnityResourceLoadPath(p_AssetName, out var resourcePath))
                    {
                        p_ResourceLifeCycleType = ResourceLifeCycleType.None;
                        var resultObject = AssetLoaderGroup[LoadManagerType.UnityResourceManager].LoadAsset<T>(p_ResourceType, p_ResourceLifeCycleType, resourcePath);
                        var resultPreset = new AssetPreset(resultObject, p_AssetName, p_ResourceType, p_ResourceLifeCycleType);
                        return (resultPreset, resultObject);
                    }
                    else
                    {
                        return default;
                    }
            }
            return default;
        }
     
        /// <summary>
        /// 지정한 타입에 적절한 에셋을 로드하는 메서드, 시스템 테이블을 참조하여 각 리소스 타입에 맞는 로드 수단을 선택하도록 오버로드되어 있음.
        /// </summary>
        public (AssetPreset, T) LoadAsset<T>(ResourceType p_ResourceType, ResourceLifeCycleType p_ResourceLifeCycleType, string p_AssetName) where T : Object
        {
            var loadType = p_ResourceType.GetResourceLoadType();
            if (p_ResourceLifeCycleType == ResourceLifeCycleType.None) p_ResourceLifeCycleType = ResourceLifeCycleType.WholeGame;
            return LoadAsset<T>(loadType, p_ResourceType, p_ResourceLifeCycleType, p_AssetName);
        }

        #endregion

        #region <Method/LoadAsync>
        
        /// <summary>
        /// 지정한 타입에 적절한 에셋을 로드하는 메서드
        /// </summary>
        public async UniTask<(AssetPreset, T)> LoadAssetAsync<T>(AssetLoadType p_AssetLoadType, ResourceType p_ResourceType, ResourceLifeCycleType p_ResourceLifeCycleType, string p_AssetName) where T : Object
        {
            switch (p_AssetLoadType)
            {
                case AssetLoadType.FromAssetBundle :
                {
                    _AssetBundleLoadOccurFlag = true;

                        // 에셋 이름을 그대로 넘기는 것으로, 에셋 번들 로드 매니저에서 에셋 번들을 탐색하는 방식
                        var resultObject = await AssetLoaderGroup[LoadManagerType.AssetBundleManager].LoadAssetAsync<T>(p_ResourceType, p_ResourceLifeCycleType, p_AssetName);

                        var resultPreset = new AssetPreset(resultObject, p_AssetName, p_ResourceType, p_ResourceLifeCycleType);
                        return (resultPreset, resultObject);

                    }
                case AssetLoadType.FromUnityResource :
                    _UnityResourceLoadOccurFlag = true;
                    // 정상적으로 ResourceListData 테이블이 초기화 된 경우
                    if (ResourceListData.GetInstanceUnSafe.TryGetUnityResourceLoadPath(p_AssetName, out var resourcePath))
                    {
                        p_ResourceLifeCycleType = ResourceLifeCycleType.None;
                        var resultObject = await AssetLoaderGroup[LoadManagerType.UnityResourceManager].LoadAssetAsync<T>(p_ResourceType, p_ResourceLifeCycleType, resourcePath);
                        var resultPreset = new AssetPreset(resultObject, p_AssetName, p_ResourceType, p_ResourceLifeCycleType);
                        return (resultPreset, resultObject);
                    }
                    else
                    {
                        return default;
                    }
            }
            return default;
        }

        /// <summary>
        /// 지정한 타입에 적절한 에셋을 로드하는 메서드, 시스템 테이블을 참조하여 각 리소스 타입에 맞는 로드 수단을 선택하도록 오버로드되어 있음.
        /// </summary>
        public async UniTask<(AssetPreset, T)> LoadAssetAsync<T>(ResourceType p_ResourceType, ResourceLifeCycleType p_ResourceLifeCycleType, string p_AssetName) where T : Object
        {
            var loadType = p_ResourceType.GetResourceLoadType();
            return await LoadAssetAsync<T>(loadType, p_ResourceType, p_ResourceLifeCycleType, p_AssetName);
        }
        
        #endregion

        #region <Method/MultiLoad>

        /// <summary>
        /// 지정한 타입에 적절한 에셋을 로드하는 메서드
        /// </summary>
        public (AssetPreset, T[]) LoadMultipleAsset<T>(AssetLoadType p_AssetLoadType, ResourceType p_ResourceType, ResourceLifeCycleType p_ResourceLifeCycleType, string p_AssetName) where T : Object
        {
            switch (p_AssetLoadType)
            {
                case AssetLoadType.FromAssetBundle :
                {
                    _AssetBundleLoadOccurFlag = true;
                    // 에셋 이름을 그대로 넘기는 것으로, 에셋 번들 로드 매니저에서 에셋 번들을 탐색하는 방식
                    var resultObject = AssetLoaderGroup[LoadManagerType.AssetBundleManager].LoadMultipleAsset<T>(p_ResourceType, p_ResourceLifeCycleType, p_AssetName);
                    var resultPreset = new AssetPreset(resultObject, p_AssetName, p_ResourceType, p_ResourceLifeCycleType);
                    return (resultPreset, resultObject);
                }
                case AssetLoadType.FromUnityResource :
                    _UnityResourceLoadOccurFlag = true;
                    // 정상적으로 ResourceListData 테이블이 초기화 된 경우
                    if (ResourceListData.GetInstanceUnSafe.TryGetUnityResourceLoadPath(p_AssetName, out var resourcePath))
                    {
                        p_ResourceLifeCycleType = ResourceLifeCycleType.None;
                        var resultObject = AssetLoaderGroup[LoadManagerType.UnityResourceManager].LoadMultipleAsset<T>(p_ResourceType, p_ResourceLifeCycleType, resourcePath);                    
                        var resultPreset = new AssetPreset(resultObject, p_AssetName, p_ResourceType, p_ResourceLifeCycleType);
                        return (resultPreset, resultObject);
                    }
                    else
                    {
                        return default;
                    }
            }
            return default;
        }
   
        /// <summary>
        /// 지정한 타입에 적절한 에셋을 로드하는 메서드, 시스템 테이블을 참조하여 각 리소스 타입에 맞는 로드 수단을 선택하도록 오버로드되어 있음.
        /// </summary>
        public (AssetPreset, T[]) LoadMultipleAsset<T>(ResourceType p_ResourceType, ResourceLifeCycleType p_ResourceLifeCycleType, string p_AssetName) where T : Object
        {
            var loadType = p_ResourceType.GetResourceLoadType();
            if (p_ResourceLifeCycleType == ResourceLifeCycleType.None) p_ResourceLifeCycleType = ResourceLifeCycleType.WholeGame;
            return LoadMultipleAsset<T>(loadType, p_ResourceType, p_ResourceLifeCycleType, p_AssetName);
        }

        /// <summary>
        /// 지정한 타입에 적절한 에셋을 로드하는 메서드
        /// </summary>
        public async UniTask<(AssetPreset, T[])> LoadMultipleAssetAsync<T>(AssetLoadType p_AssetLoadType, ResourceType p_ResourceType, ResourceLifeCycleType p_ResourceLifeCycleType, string p_AssetName) where T : Object
        {
            switch (p_AssetLoadType)
            {
                case AssetLoadType.FromAssetBundle :
                {
                    _AssetBundleLoadOccurFlag = true;
                    // 에셋 이름을 그대로 넘기는 것으로, 에셋 번들 로드 매니저에서 에셋 번들을 탐색하는 방식
                    var resultObject = await AssetLoaderGroup[LoadManagerType.AssetBundleManager].LoadMultipleAssetAsync<T>(p_ResourceType, p_ResourceLifeCycleType, p_AssetName);
                    var resultPreset = new AssetPreset(resultObject, p_AssetName, p_ResourceType, p_ResourceLifeCycleType);
                    return (resultPreset, resultObject);
                }
                case AssetLoadType.FromUnityResource :
                    _UnityResourceLoadOccurFlag = true;
                    // 정상적으로 ResourceListData 테이블이 초기화 된 경우
                    if (ResourceListData.GetInstanceUnSafe.TryGetUnityResourceLoadPath(p_AssetName, out var resourcePath))
                    {
                        p_ResourceLifeCycleType = ResourceLifeCycleType.None;
                        var resultObject = await AssetLoaderGroup[LoadManagerType.UnityResourceManager].LoadMultipleAssetAsync<T>(p_ResourceType, p_ResourceLifeCycleType, resourcePath);
                        var resultPreset = new AssetPreset(resultObject, p_AssetName, p_ResourceType, p_ResourceLifeCycleType);
                        return (resultPreset, resultObject);
                    }
                    else
                    {
                        return default;
                    }
            }
            return default;
        }

        /// <summary>
        /// 지정한 타입에 적절한 에셋을 로드하는 메서드, 시스템 테이블을 참조하여 각 리소스 타입에 맞는 로드 수단을 선택하도록 오버로드되어 있음.
        /// </summary>
        public async UniTask<(AssetPreset, T[])> LoadMultipleAssetAsync<T>(ResourceType p_ResourceType, ResourceLifeCycleType p_ResourceLifeCycleType, string p_AssetName) where T : Object
        {
            var loadType = p_ResourceType.GetResourceLoadType();
            if (p_ResourceLifeCycleType == ResourceLifeCycleType.None) p_ResourceLifeCycleType = ResourceLifeCycleType.WholeGame;
            return await LoadMultipleAssetAsync<T>(loadType, p_ResourceType, p_ResourceLifeCycleType, p_AssetName);
        }
        
        #endregion
        
        #region <Method/LoadScene>
        
        /// <summary>
        /// 로드된 에셋번들 내부에 존재하는 씬의 이름을 리턴하는 메서드
        /// </summary>
        public async UniTask<string> LoadScene(ResourceLifeCycleType p_ResourceLifeCycleType, string p_AssetName)
        {
            return await AssetLoaderGroup[LoadManagerType.AssetBundleManager].LoadSceneAsset(p_ResourceLifeCycleType, p_AssetName);
        }

        /// <summary>
        /// 특정 에셋 번들에 등록된 씬들의 이름을 리스트로 리턴하는 메서드
        /// </summary>
        public async UniTask<string[]> LoadAllScenes(ResourceLifeCycleType p_ResourceLifeCycleType, string p_AssetName)
        {
            //return SceneDataRoot.GetInstanceUnSafe.GetLoadAllScenesName(p_AssetName);
            return await AssetLoaderGroup[LoadManagerType.AssetBundleManager].LoadAllSceneAsset(p_ResourceLifeCycleType, p_AssetName);
        }
        
        #endregion
        
        #region <Method/Unload>

        /// <summary>
        /// 지정한 수명 타입을 가지는 에셋들을 릴리스하는 메서드
        /// </summary>
        public void UnloadAsset(ResourceLifeCycleType p_ResourceLifeCycleType)
        {
            if (_AssetBundleLoadOccurFlag)
            {
                GetAssetBundleLoadManager().UnloadAsset(p_ResourceLifeCycleType);
            }

            if (_UnityResourceLoadOccurFlag)
            {
                GetUnityResourceLoadManager().UnloadAsset(p_ResourceLifeCycleType);
            }
        }

        /// <summary>
        /// 씬 단위 수명 타입을 가지는 에셋들을 릴리스하는 메서드
        /// </summary>
        public void UnloadAsset_SceneLifeCycle()
        {
            UnloadAsset(ResourceLifeCycleType.Scene);
        }
        
        /// <summary>
        /// 특정 리소스 타입의 지정한 이름, 혹은 오브젝트 그 자체를 입력하여 에셋 매니저를 통해
        /// 메모리에서 에셋을 릴리스하는 메서드
        /// </summary>
        public void UnloadAsset(Object p_Asset, string p_AssetName, ResourceType p_ResourceType, ResourceLifeCycleType p_ResourceLifeCycleType)
        {
            UnloadAsset(new AssetPreset(p_Asset, p_AssetName, p_ResourceType, p_ResourceLifeCycleType));
        }

        /// <summary>
        /// 특정 리소스 타입의 지정한 이름, 혹은 오브젝트 그 자체를 입력하여 에셋 매니저를 통해
        /// 메모리에서 에셋을 릴리스하는 메서드.
        /// 주로 에디터모드의 윈도우 종료 및 게임 모드 종료 시 호출된다.
        /// </summary>
        public void UnloadAsset(AssetPreset p_AssetPreset)
        {
            if (p_AssetPreset.IsValid)
            {
                var resourceLoadType = p_AssetPreset.ResourceType.GetResourceLoadType();
                switch (resourceLoadType)
                {
                    case AssetLoadType.FromAssetBundle:
                        GetAssetBundleLoadManager().UnloadAsset(p_AssetPreset);
                        break;
                    case AssetLoadType.FromUnityResource:
                        GetUnityResourceLoadManager().UnloadAsset(p_AssetPreset);
                        break;
                }
            }
        }
        
        /// <summary>
        /// 모든 로드 타입의 에셋을 언로드하는 메서드
        /// </summary>
        public void UnloadAsset()
        {
            if (SystemTool.TryGetEnumEnumerator<ResourceLifeCycleType>(SystemTool.GetEnumeratorType.ExceptNone, out var o_Enumerator))
            {
                foreach (var resourceLifeCycleType in o_Enumerator)
                {
                    UnloadAsset(resourceLifeCycleType);
                }
            }

            GetAssetBundleLoadManager().TryUnloadDefaultAssetBundle();
            
            AssetBundle.UnloadAllAssetBundles(true);
            // Resources.UnloadUnusedAssets();
            
#if UNITY_EDITOR
            if (CustomDebug.PrintLoadAssetManager)
            {
                Debug.Log($"******************** Unload All Asset Result **********************\n");
                GetAssetBundleLoadManager().PrintLoadedAssetBundleList();
                GetAssetBundleLoadManager().PrintLoadedAssetBundleResourceList();
                Debug.Log($"\n*****************************************************************");
            }
#endif
        }

        #endregion
    }
}