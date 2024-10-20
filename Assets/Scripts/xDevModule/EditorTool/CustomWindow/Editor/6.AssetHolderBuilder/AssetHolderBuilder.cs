using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Video;
using Object = UnityEngine.Object;

namespace BlackAm
{
    /// <summary>
    /// 각 버전의 변경 사항 테이블(PatchTool)을 참조하여 특정 버전으로의 패치파일을 생성하는 클래스
    /// </summary>
    public class AssetHolderBuilder : Singleton<AssetHolderBuilder>
    {
        #region <Consts>

        private static readonly string _AssetHolderPath = $"{SystemMaintenance.GetDependencyResourcePathBranch(DependencyResourceSubType.GameObject)}SystemAssetHolder";
        private static readonly string _TextAssetDependencyPath = $"{SystemMaintenance.UnityResourceAbsolutePath}{SystemMaintenance.GetDependencyResourcePathBranch(DependencyResourceSubType.TextAsset)}";

        #endregion
        
        #region <Fields>

        private Dictionary<Type, IAssetHolderBase> _assetHolderTable;
        private GameObject _targetAssetHolderPrefab;
            
        #endregion
        
        #region <Callbacks>

        protected override void OnCreated()
        {
            _assetHolderTable = new Dictionary<Type, IAssetHolderBase>();
            _targetAssetHolderPrefab = Resources.Load<GameObject>(_AssetHolderPath);
            if (ReferenceEquals(null, _targetAssetHolderPrefab))
            {
                var assetHolder = new GameObject();
                var targetPath = $"{SystemMaintenance.UnityResourceDirectory}{_AssetHolderPath}.prefab";
                _targetAssetHolderPrefab = PrefabUtility.SaveAsPrefabAsset(assetHolder, targetPath);
                Object.DestroyImmediate(assetHolder);
            }
            
            var tryHolderTypeSet = AssetHolderTool.HolderTypeSet;
            foreach (var type in tryHolderTypeSet)
            {
                var tryComponent = _targetAssetHolderPrefab.GetComponent(type);
                if (tryComponent == null)
                {
                    tryComponent = _targetAssetHolderPrefab.AddComponent(type);
                }

                var holder = tryComponent as IAssetHolderBase;
                if (!ReferenceEquals(null, holder))
                {
                    _assetHolderTable.Add(holder.GetAssetType(), holder);
                    holder.OnCreated();
                }
            }
        }

        public override void OnInitiate()
        {
        }

        #endregion

        #region <Methods>

        private void _InitAssetHolder()
        {
            foreach (var assetHolderKV in _assetHolderTable)
            {
                assetHolderKV.Value.Reset();
            }
        }

        public void InitAssetHolder()
        {
            _InitAssetHolder();
            PrefabUtility.SavePrefabAsset(_targetAssetHolderPrefab);
        }

        public void UpdateAssetHolder()
        {
            _InitAssetHolder();
            
            var resourceListTable = ResourceListData.GetInstanceUnSafe.GetTable();
            var resourceTrackerTable = ResourceTracker.GetInstanceUnSafe.GetTable();
            
            foreach (var tableRecordKV in resourceListTable)
            {
                var tableRecord = tableRecordKV.Value;
                var tryPath = tableRecord.GetResourceFullPath();
                var tryDependencyName = SystemMaintenance.GetDependencyResourceSubTypeString(tryPath);
                switch (tryDependencyName)
                {
                    case var _ when ReferenceEquals(null, tryDependencyName):
                        break;
                    case var _ when string.IsNullOrEmpty(tryDependencyName):
                    {
                        var relativePath = tryPath.CutString(SystemMaintenance.UnityResourceDirectory, false, true);
                        var offExtPath = relativePath.GetPathWithoutExtension();
                        if (!resourceTrackerTable.ContainsKey(offExtPath))
                        {
                            AddAsset(null, offExtPath, false);
                        }
                        break;
                    }
                    default:
                    {
                        var relativePath = tryPath.CutString(SystemMaintenance.UnityResourceDirectory, false, true);
                        var offExtPath = relativePath.GetPathWithoutExtension();
                        if (!resourceTrackerTable.ContainsKey(offExtPath))
                        {
                            AddAsset(tryDependencyName, offExtPath, false);
                        }
                        break;
                    }
                }
            }

            foreach (var tableRecordKV in resourceTrackerTable)
            {
                var tableRecord = tableRecordKV.Value;
                AddAsset(tableRecord.AssetTypeName, tableRecordKV.Key, tableRecord.IsMultiAsset);
            }
            
            PrefabUtility.SavePrefabAsset(_targetAssetHolderPrefab);
        }

        private void AddAsset(string p_AssetNameType, string p_AssetPath, bool p_IsMultiAsset)
        {
            switch (p_AssetNameType)
            {
                case "AnimationClip":
                {
                    AddAsset<AnimationClip>(p_AssetPath, p_IsMultiAsset);
                    break;
                }
                case "AudioClip" :
                {
                    AddAsset<AudioClip>(p_AssetPath, p_IsMultiAsset);
                    break;
                }
                case "AudioMixer" :
                {
                    AddAsset<AudioMixer>(p_AssetPath, p_IsMultiAsset);
                    break;
                }
                case "GameObject" : // same as prefab
                {
                    AddAsset<GameObject>(p_AssetPath, p_IsMultiAsset);
                    break;
                }
                case "RenderTexture":
                {
                    AddAsset<RenderTexture>(p_AssetPath, p_IsMultiAsset);
                    break;
                }
                case "Sprite" :
                {
                    AddAsset<Sprite>(p_AssetPath, p_IsMultiAsset);
                    break;
                }
                case "TextAsset" :
                {
                    AddAsset<TextAsset>(p_AssetPath, p_IsMultiAsset);
                    break;
                }
                case "Texture" :
                {
                    AddAsset<Texture>(p_AssetPath, p_IsMultiAsset);
                    break;
                }
                case "VideoClip" :
                {
                    AddAsset<VideoClip>(p_AssetPath, p_IsMultiAsset);
                    break;
                }
                case "RuntimeAnimatorController" :
                {
                    AddAsset<RuntimeAnimatorController>(p_AssetPath, p_IsMultiAsset);
                    break;
                }
                case "Shader" :
                {
                    AddAsset<Shader>(p_AssetPath, p_IsMultiAsset);
                    break;
                }
                default :
                {
                    AddAsset<Object>(p_AssetPath, p_IsMultiAsset);
                    break;
                }
            }
        }

        private void AddAsset<AssetType>(string p_AssetPath, bool p_IsMultiAsset) where AssetType : Object
        {
            var tryComponent = _assetHolderTable[typeof(AssetType)] as AssetHolderBase<AssetType>;
            if (p_IsMultiAsset)
            {
                var tryAsset = Resources.Load<AssetType>(p_AssetPath);
                var tryAssets = Resources.LoadAll<AssetType>(p_AssetPath);
                if (!ReferenceEquals(null, tryAssets))
                {
                    tryComponent.AddAsset(p_AssetPath, tryAsset, tryAssets);
                }
            }
            else
            {
                var tryAsset = Resources.Load<AssetType>(p_AssetPath);
                if (!ReferenceEquals(null, tryAsset))
                {
                    tryComponent.AddAsset(p_AssetPath, tryAsset);
                }
            }
        }
        
        #endregion
    }
}