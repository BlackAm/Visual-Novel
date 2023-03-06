using System;
using Object = UnityEngine.Object;

namespace k514
{
    public interface IAssetHolderBase
    {
        Type GetAssetType();
        void OnCreated();
        void Reset();
    }
    
    public interface IAssetHoldRunTime
    {
        Object GetAsset(string p_AssetPath);
        Object[] GetAssets(string p_AssetPath);
    }
    
    public static class AssetHolderTool
    {
        public static readonly Type[] HolderTypeSet = typeof(AssetHolderBase<>).GetSubClassTypeSet();
    }
}