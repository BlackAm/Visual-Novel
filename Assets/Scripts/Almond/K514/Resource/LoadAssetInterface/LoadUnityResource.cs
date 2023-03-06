using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;
 
namespace k514
{
    /// <summary>
    /// 에셋번들 외의 방식
    /// 1. 유니티 리소스 폴더
    /// 2. 유니티가 지정하는 솔루션 외부폴더
    /// 의 방식으로 리소스를 로드하는 클래스 
    /// </summary>
    public class LoadUnityResource : ILoadAsset
    {
        #region <Callbacks>
 
        public void OnCreated()
        {
        }
         
        #endregion
         
        #region <Methods>
 
        /// <summary>
        /// 지정한 경로의 에셋을 로드하는 메서드
        /// </summary>
        public T LoadAsset<T>(ResourceType p_ResourceType, ResourceLifeCycleType p_ResourceLifeCycleType, string p_FullPath) where T : Object
        {
#if UNITY_EDITOR
            var result = SystemTool.Load<T>(p_FullPath);
            if (CustomDebug.PrintLoadAssetManager)
            {
                var loadedComment = result == null ? "LoadFail" : "LoadSuccess";
                Debug.Log($"Unity Resource Load Manager : Load {p_FullPath} / Type {typeof(T).Name} / Result : {loadedComment}");
            }
             
            return result;
#else
            return SystemTool.Load<T>(p_FullPath);
#endif
        }

        public async UniTask<T> LoadAssetAsync<T>(ResourceType p_ResourceType, ResourceLifeCycleType p_ResourceLifeCycleType, string p_FullPath) where T : Object
        {
#if UNITY_EDITOR
            var result = await SystemTool.LoadAsync<T>(p_FullPath);

            //if (CustomDebug.PrintLoadAssetManager)
            {
                var loadedComment = result == null ? "LoadFail" : "LoadSuccess";
                Debug.Log($"Unity Resource Load Manager : Load Async {p_FullPath} / Type {typeof(T).Name} / Result : {loadedComment}");
            }
             
            return result;
#else
            return await SystemTool.LoadAsync<T>(p_FullPath);
#endif
        }

        public T[] LoadMultipleAsset<T>(ResourceType p_ResourceType, ResourceLifeCycleType p_ResourceLifeCycleType, string p_FullPath) where T : Object
        {
#if UNITY_EDITOR
            var result = SystemTool.LoadAll<T>(p_FullPath);
            if (CustomDebug.PrintLoadAssetManager)
            {
                var loadedComment = result == null ? "LoadFail" : "LoadSuccess";
                Debug.Log($"Unity Resource Load Manager : Load {p_FullPath} / Type {typeof(T).Name} / Result : {loadedComment}");
            }
             
            return result;
#else
            return SystemTool.LoadAll<T>(p_FullPath);
#endif
        }
 
        /// <summary>
        /// Resources.LoadAll 은 유니티 스레드에서만 동작.
        /// 따라서 구현할 수 없다.
        /// </summary>
        public UniTask<T[]> LoadMultipleAssetAsync<T>(ResourceType p_ResourceType, ResourceLifeCycleType p_ResourceLifeCycleType,
            string p_FullPath) where T : Object
        {
            throw new NotImplementedException("Resources.LoadAll 은 유니티 스레드에서만 동작하므로 구현할 수 없음");
        }
 
        /// <summary>
        /// 유니티 리소스로 관리되는 리소스들은 따로 리소스 수명을 관리할 필요가 없다.
        /// Resources 클래스에서 간단한 메서드를 지원하기 때문.
        /// </summary>
        public void UnloadAsset(ResourceLifeCycleType p_ResourceLifeCycleType)
        {
#if UNITY_EDITOR
            if (CustomDebug.PrintLoadAssetManager)
            {
                Debug.Log($"Unity Resource Load Manager : UnLoadUnusedResource");
            }
#endif
            // Resources.UnloadUnusedAssets();
        }
 
        /// <summary>
        /// 에셋 번들용 리소스 언로드 메서드
        /// </summary>
        public void UnloadAsset(AssetPreset p_AssetPreset)
        {
#if UNITY_EDITOR
            if (CustomDebug.PrintLoadAssetManager)
            {
                Debug.Log($"Unity Resource Load Manager : Unload AssetName {p_AssetPreset.AssetName}");
            }
#endif
             
            //TODO<K514>
            /*
                unloadasset may only be used on individual assets and can not be used 
                on gameobject's / components / assetbundles or gamemanagers
            */
            if(!(p_AssetPreset.Asset is GameObject))
            {
                // Resources.UnloadAsset(p_AssetPreset.Asset);
            }
            else
            {
                // Resources.UnloadUnusedAssets();
            }
        }
 
        /// <summary>
        /// 씬 리소스는 Resources 가 아닌 SceneManager에서 관리하므로 에러 메시지를 출력한다.
        /// </summary>
        public async UniTask<string> LoadSceneAsset(ResourceLifeCycleType p_ResourceLifeCycleType, string p_AssetName)
        {
            Debug.LogError("Use SceneManager.LoadScene method");
            await UniTask.CompletedTask;
            return null;
        }
         
        /// <summary>
        /// 씬 리소스는 Resources 가 아닌 SceneManager에서 관리하므로 에러 메시지를 출력한다.
        /// </summary>
        public async UniTask<string[]> LoadAllSceneAsset(ResourceLifeCycleType p_ResourceLifeCycleType, string p_AssetName)
        {
            Debug.LogError("Use SceneManager.LoadScene method");
            await UniTask.CompletedTask;
            return null;
        }
 
        #endregion
    }
}