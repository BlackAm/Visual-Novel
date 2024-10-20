using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BlackAm
{
    [Serializable]
    public struct AssetHolderPreset<AssetType> where AssetType : Object
    {
        public AssetType Asset;
        public AssetType[] AssetSet;
    }

    public abstract class AssetHolderBase<AssetType> : MonoBehaviour, IAssetHolderBase where AssetType : Object
    {
        #region <Fields>

        [SerializeField] private List<string> AssetPathList;
        [SerializeField] private List<AssetHolderPreset<AssetType>> AssetList;

        #endregion

        #region <Callbacks>

        public void OnCreated()
        {
            if (ReferenceEquals(null, AssetPathList))
            {
                AssetPathList = new List<string>();
            }
            
            if (ReferenceEquals(null, AssetList))
            {
                AssetList = new List<AssetHolderPreset<AssetType>>();
            }
        }

        #endregion
        
        #region <Methods>

        public Type GetAssetType() => typeof(AssetType);
        public List<string> GetAssetPathList() => AssetPathList;
        public List<AssetHolderPreset<AssetType>> GetAssetList() => AssetList;
        
        public void Reset()
        {
            AssetPathList.Clear();
            AssetList.Clear();
        }
        
        public void AddAsset(string p_AssetPath, AssetType p_Asset)
        {
            var tryIndex = AssetPathList.IndexOf(p_AssetPath);
            if (tryIndex < 0)
            {
                AssetPathList.Add(p_AssetPath);
                AssetList.Add(default);
                tryIndex = AssetPathList.Count - 1;
            }
            
            AssetList[tryIndex] = new AssetHolderPreset<AssetType>
            {
                Asset = p_Asset,
                AssetSet = null
            };
        }
        
        public void AddAsset(string p_AssetPath, AssetType p_Asset, AssetType[] p_AssetSet)
        {
            var tryIndex = AssetPathList.IndexOf(p_AssetPath);
            if (tryIndex < 0)
            {
                AssetPathList.Add(p_AssetPath);
                AssetList.Add(default);
                tryIndex = AssetPathList.Count - 1;
            }
            
            AssetList[tryIndex] = new AssetHolderPreset<AssetType>
            {
                Asset = p_Asset,
                AssetSet = p_AssetSet
            };
        }

        #endregion
    }
}