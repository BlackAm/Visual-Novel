using Cysharp.Threading.Tasks;
using UnityEngine;

namespace k514
{
    public partial class SystemTool
    {
        public static T Load<T>(string p_AssetName) where T : Object
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                var tryAsset = AssetHolderManager.GetInstanceUnSafe.LoadAsset<T>(p_AssetName);
                if (ReferenceEquals(null, tryAsset))
                {
                    tryAsset = Resources.Load<T>(p_AssetName);
                    if (!ReferenceEquals(null, tryAsset))
                    {
                        PrintMessage($"에셋 {p_AssetName}이 유니티 리소스 함수에 의해 로드되었습니다.");
                    }
                }
                ResourceTracker.GetInstanceUnSafe.ReplaceRecord(p_AssetName, typeof(T), false);
                return tryAsset;
            }
            else
            {
                var tryAsset = Resources.Load<T>(p_AssetName);
                return tryAsset;
            }
#else
            var tryAsset = AssetHolderManager.GetInstanceUnSafe.LoadAsset<T>(p_AssetName);
            if (ReferenceEquals(null, tryAsset))
            {
                tryAsset = Resources.Load<T>(p_AssetName);
            }
            return tryAsset;
#endif
        }
        
        public static async UniTask<T> LoadAsync<T>(string p_AssetName) where T : Object
        {
#if UNITY_EDITOR
            await UniTask.SwitchToMainThread();
            if (Application.isPlaying)
            {
                var tryAsset = AssetHolderManager.GetInstanceUnSafe.LoadAsset<T>(p_AssetName);
                if (ReferenceEquals(null, tryAsset))
                {
                    tryAsset = await Resources.LoadAsync<T>(p_AssetName) as T;
                    if (!ReferenceEquals(null, tryAsset))
                    {
                        PrintMessage($"에셋 {p_AssetName}이 유니티 리소스 함수에 의해 로드되었습니다.");
                    }
                }
                await ResourceTracker.GetInstanceUnSafe.ReplaceRecord(p_AssetName, typeof(T), false);
                return tryAsset;
            }
            else
            {
                var tryAsset = await Resources.LoadAsync<T>(p_AssetName) as T;
                return tryAsset;
            }
#else
            var tryAsset = AssetHolderManager.GetInstanceUnSafe.LoadAsset<T>(p_AssetName);
            if (ReferenceEquals(null, tryAsset))
            {
                tryAsset = await Resources.LoadAsync<T>(p_AssetName) as T;
            }
            return tryAsset;
#endif
        }

        public static T[] LoadAll<T>(string p_AssetName) where T : Object
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                var tryAsset = AssetHolderManager.GetInstanceUnSafe.LoadAssets<T>(p_AssetName);
                if (ReferenceEquals(null, tryAsset))
                {
                    tryAsset = Resources.LoadAll<T>(p_AssetName);
                    if (!ReferenceEquals(null, tryAsset))
                    {
                        PrintMessage($"에셋 {p_AssetName}이 유니티 리소스 함수에 의해 로드되었습니다.");
                    }
                }
                ResourceTracker.GetInstanceUnSafe.ReplaceRecord(p_AssetName, typeof(T), true);
                return tryAsset;
            }
            else
            {
                var tryAsset = Resources.LoadAll<T>(p_AssetName);
                return tryAsset;
            }
#else
            var tryAsset = AssetHolderManager.GetInstanceUnSafe.LoadAssets<T>(p_AssetName);
            if (ReferenceEquals(null, tryAsset))
            {
                tryAsset = Resources.LoadAll<T>(p_AssetName);
            }
            return tryAsset;
#endif
        }

#if UNITY_EDITOR
        private static void PrintMessage(string p_Message)
        {
            if (SystemFlag.IsTableByteImageMode())
            {
                Debug.LogError(p_Message);
            }
            else
            {
                Debug.LogWarning(p_Message);
            }
        }
#endif
    }
}