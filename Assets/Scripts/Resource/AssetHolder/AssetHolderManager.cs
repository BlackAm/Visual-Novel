using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Video;
using Object = UnityEngine.Object;

namespace BlackAm
{
    public class AssetHolderManager : AsyncSingleton<AssetHolderManager>
    {
        #region <Fields>

        private Dictionary<Type, IAssetHoldRunTime> _AssetHolderTable;
        private List<Type> _HolderTypeSet;
        private Type _ObjectTypeCache;
        
        #endregion
        
        #region <Callbacks>

        protected override async UniTask OnCreated()
        {
            _AssetHolderTable = new Dictionary<Type, IAssetHoldRunTime>();
            _HolderTypeSet = new List<Type>();
            _ObjectTypeCache = typeof(Object);
            
            await UniTask.SwitchToMainThread();
            var assetHolderSet = SystemBoot.GetInstance._Transform.GetComponentsInChildren<IAssetHolderBase>();
            
            await UniTask.SwitchToThreadPool();
            foreach (var assetHolder in assetHolderSet)
            {
                _HolderTypeSet.Add(assetHolder.GetAssetType());
            }
            
            await ImportAssetHolder<AnimationClip>();
            await ImportAssetHolder<AudioClip>();
            await ImportAssetHolder<AudioMixer>();
            await ImportAssetHolder<Object>();
            await ImportAssetHolder<GameObject>();
            await ImportAssetHolder<RenderTexture>();
            await ImportAssetHolder<Sprite>();
            await ImportAssetHolder<TextAsset>();
            await ImportAssetHolder<Texture>();
            await ImportAssetHolder<VideoClip>();
            await ImportAssetHolder<RuntimeAnimatorController>();
            await ImportAssetHolder<Shader>();
            
            await UniTask.SwitchToMainThread();
            var (valid, holderCluster) = SystemBoot.GetInstance._Transform.FindRecursive("SystemAssetHolder");
            if (valid)
            {
                Object.Destroy(holderCluster.gameObject);
            }
        }

        public override async UniTask OnInitiate()
        {
            await UniTask.CompletedTask;
        }

        #endregion

        #region <Methods>

        private async UniTask ImportAssetHolder<AssetType>() where AssetType : Object
        {
            var tryType = typeof(AssetType);
            if (_HolderTypeSet.Contains(tryType))
            {
                await UniTask.SwitchToMainThread();
                var assetHolder = SystemBoot.GetInstance._Transform.GetComponentInChildren<AssetHolderBase<AssetType>>();
                await UniTask.SwitchToThreadPool();
                _AssetHolderTable.Add(assetHolder.GetAssetType(), new AssetHoldRuntime<AssetType>(assetHolder.GetAssetPathList(), assetHolder.GetAssetList()));
            }
        }

        public T LoadAsset<T>(string p_AssetPath) where T : Object
        {
            var tryType = typeof(T);
            if (_HolderTypeSet.Contains(tryType))
            {
                if (ReferenceEquals(tryType, _ObjectTypeCache))
                {
                    return _AssetHolderTable[typeof(Object)].GetAsset(p_AssetPath) as T;
                }
                else
                {
                    return _AssetHolderTable[tryType].GetAsset(p_AssetPath) as T;
                }
            }
            else
            {
                return null;
            }
        }
        
        public T[] LoadAssets<T>(string p_AssetPath) where T : Object
        {
            var tryType = typeof(T);
            if (_HolderTypeSet.Contains(tryType))
            {
                if (ReferenceEquals(tryType, _ObjectTypeCache))
                {
                    return _AssetHolderTable[typeof(Object)].GetAssets(p_AssetPath) as T[];
                }
                else
                {
                    return _AssetHolderTable[tryType].GetAssets(p_AssetPath) as T[];
                }
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}