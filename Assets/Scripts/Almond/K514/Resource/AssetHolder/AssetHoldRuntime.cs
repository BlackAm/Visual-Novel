using System.Collections.Generic;
using UnityEngine;

namespace k514
{
    public class AssetHoldRuntime<AssetType> : IAssetHoldRunTime where AssetType : Object
    {
        #region <Fields>

        private List<string> AssetPathList;
        private List<AssetHolderPreset<AssetType>> AssetList;

        #endregion

        #region <Constructors>

        public AssetHoldRuntime(List<string> p_AssetPathList, List<AssetHolderPreset<AssetType>> p_AssetList)
        {
            AssetPathList = new List<string>();
            foreach (var element in p_AssetPathList)
            {
                AssetPathList.Add(element);
            }
            
            AssetList = new List<AssetHolderPreset<AssetType>>();
            foreach (var element in p_AssetList)
            {
                AssetList.Add(element);
            }
        }

        #endregion
        
        #region <Methods>
        
        public Object GetAsset(string p_AssetPath)
        {
            var key = AssetPathList.IndexOf(p_AssetPath);
            if (key > -1)
            {
                return AssetList[key].Asset;
            }
            else
            {
                return null;
            }
        }
        
        public Object[] GetAssets(string p_AssetPath)
        {
            var key = AssetPathList.IndexOf(p_AssetPath);
            if (key > -1)
            {
                return AssetList[key].AssetSet;
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}