using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace k514
{
    public interface ICommonSceneListData : ITableBase
    {
        IIndexableSceneDataRecordBridge FirstRecord { get; }

        int GetSceneIndex(string p_SceneName);
    }

    public interface IIndexableSceneDataRecordBridge : ITableBaseRecord
    {
        int KEY { get; }
        string SceneName { get; }
    }

    public static class SceneDataTool
    {
        #region <Enums>

        /// <summary>
        /// 해당 씬에서 수행할 수 있는 시스템 기능들을 마스크로 지정하는 열거형 상수
        /// </summary>
        [Flags]
        public enum SceneVariablePropertyType
        {
            None = 0,
            
            /// <summary>
            /// 안전구역(주로 마을)이 있는지
            /// </summary>
            HasSafeArea = 1 << 0,
            
            /// <summary>
            /// 해당 씬에 npc가 있는지
            /// </summary>
            HasNPC = 1 << 1,
            
            /// <summary>
            /// 해당 씬에 사냥터가 있는지
            /// </summary>
            HasBattleArea = 1 << 2,
            
            /// <summary>
            /// UI를 기준으로만 기능을 수행하고 다른 씬으로 전이하는 케이스의 씬인지
            /// </summary>
            UIOnly = 1 << 3,
            
            /// <summary>
            /// 멀티 플레이를 지원함
            /// </summary>
            SupportMultiPlay = 1 << 4,
            
            /// <summary>
            /// 스폰과 리젠 및 클리어 조건이 무제한인 맵임
            /// </summary>
            OpenField = 1 << 5,
            
            /// <summary>
            /// 스폰과 리젠 및 클리어 조건이 지정된 맵임
            /// </summary>
            Dungeon = 1 << 6,
        }

        #endregion
    }

    public class SceneDataRoot : MultiTableProxy<SceneDataRoot, int, SceneDataRoot.SceneDataType, ICommonSceneListData, IIndexableSceneDataRecordBridge>
    {
        #region <Fields>
        
        /// <summary>
        /// [씬 인덱스, 씬 프리셋] 컬렉션
        /// </summary>
        public Dictionary<int, SceneControllerTool.ScenePreset> IScenePresetTable { get; private set; }

        /// <summary>
        /// [씬 이름, 씬 프리셋] 컬렉션
        /// </summary>
        public Dictionary<string, SceneControllerTool.ScenePreset> SScenePresetTable { get; private set; }
        
        /// <summary>
        /// [시스템 씬 타입, 씬 프리셋] 컬렉션
        /// </summary>
        public static Dictionary<SceneControllerTool.SystemHiddenSceneType, SceneControllerTool.ScenePreset> SystemHiddenSceneTable { get; private set; }

        #endregion
        
        #region <Enums>

        public enum SceneDataType
        {
            /* Reserved Scene */
            UnityBuiltIn,
            SystemHiddenScene,

            /* Custom */
            DungeonScene,
            SystemScene,
            MainScene,
            TestScene,
        }
        
        #endregion

        #region <Indexer>

        public new SceneControllerTool.ScenePreset this[int p_Key] => IScenePresetTable[p_Key];
        public SceneControllerTool.ScenePreset this[string p_Key] => SScenePresetTable[p_Key];
        public SceneControllerTool.ScenePreset this[SceneControllerTool.SystemHiddenSceneType p_Key] => SystemHiddenSceneTable[p_Key];
        public Dictionary<SceneDataType, List<string>> scenePathCollection = new Dictionary<SceneDataType, List<string>>();
        #endregion

        #region <Callbacks>
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        void DisplayLog(string str)
        {
            if(sw.IsRunning)
            {
                sw.Stop();
            }
            Debug.LogWarning($"{str} : Time({sw.ElapsedMilliseconds}).mesc");
            sw.Reset();
            sw.Start();
        }

        protected override async UniTask OnCreated()
        {
            await base.OnCreated();
            IScenePresetTable = new Dictionary<int, SceneControllerTool.ScenePreset>();
            SScenePresetTable = new Dictionary<string, SceneControllerTool.ScenePreset>();
            SystemHiddenSceneTable = new Dictionary<SceneControllerTool.SystemHiddenSceneType, SceneControllerTool.ScenePreset>();

#if UNITY_EDITOR
            await SystemMaintenance.CreateBuildSetting();
#endif
            
            var multiTableCluster = GameDataTableCluster;
            var labelIterator = multiTableCluster._LabelEnumerator;
            var sceneLoadType = ResourceType.Scene.GetResourceLoadType();

            

            foreach (var sceneDataType in labelIterator)
            {
                scenePathCollection.Add(sceneDataType, new List<string>());
            }
            foreach (var sceneList in ResourceListData.GetInstanceUnSafe.GetTable())
            {
                if (sceneList.Value.GetResourceFullPath().EndsWith(".unity"))
                {
                    SceneDataType sceneDataType = SceneDataType.UnityBuiltIn;
                    if (sceneList.Value.GetResourceFullPath().Contains("SceneList200"))
                    {
                        sceneDataType = SceneDataType.DungeonScene;
                    }
                    else if (sceneList.Value.GetResourceFullPath().Contains("SceneList300"))
                    {
                        sceneDataType = SceneDataType.SystemScene;
                    }
                    else if (sceneList.Value.GetResourceFullPath().Contains("SceneList400"))
                    {
                        sceneDataType = SceneDataType.MainScene;
                    }
                    else if (sceneList.Value.GetResourceFullPath().Contains("SceneListTest"))
                    {
                        sceneDataType = SceneDataType.TestScene;
                    }
                    
                    if(sceneDataType != SceneDataType.UnityBuiltIn)
                    {
                        var path = sceneList.Value.GetResourceFullPath();
                        string fullPath = path.Substring(path.IndexOf("Assets"));
                        scenePathCollection[sceneDataType].Add(fullPath);
                    }
                }
            }

            foreach (var sceneDataType in labelIterator)
            {
                switch (sceneDataType)
                {
                    case SceneDataType.UnityBuiltIn:
                    {
                        await UniTask.SwitchToMainThread();
                        
                        var builtInSceneCount = SceneManager.sceneCountInBuildSettings;
                        var loadingSceneTypeEnumerator = SystemTool.GetEnumEnumerator<SceneControllerTool.SystemHiddenSceneType>(SystemTool.GetEnumeratorType.GetAll);
                        
                        for (int i = 0; i < builtInSceneCount; i++)
                        {
                            var trySceneFullPath = SceneUtility.GetScenePathByBuildIndex(i);
                            var trySceneName = trySceneFullPath.GetFileNameFromPath(true);
                            var targetScenePreset = new SceneControllerTool.ScenePreset(sceneDataType, i, trySceneName, trySceneFullPath);
                            
                            IScenePresetTable.Add(i, targetScenePreset);
                            SScenePresetTable.Add(trySceneName, targetScenePreset);

                            // 시스템 히든 씬은 열거형 상수를 통해 접근할 필요가 있으므로, 전용 컬렉션에 정리해준다.
                            var exceptExtSceneName = trySceneName.CutString(".", true, false);
                            foreach (var sceneType in loadingSceneTypeEnumerator)
                            {
                                if (exceptExtSceneName == sceneType.ToString())
                                {
                                    SystemHiddenSceneTable.Add(sceneType, targetScenePreset);
                                    break;
                                }
                            }
                        }
                        await UniTask.SwitchToThreadPool();
                        break;
                    }
                    // 빌드 씬 외의 테이블에 등록된 씬의 경우, 테이블을 참조하여 인덱서를 초기화 시켜준다.
                    // 이 때, 씬 로드 타입이 에셋번들인 경우 씬의 풀네임을 알기 위해서는 에셋번들에 접근하여야 하므로
                    // 해당 경우에 한해 에셋번들을 로드하는 작업을 수행한다.
                    default:
                    {
                        var targetTable = this[sceneDataType];
                            switch (sceneLoadType)
                        {
                            // 만약 테이블 씬이 번들로부터 로드된다면, 먼저 한번 해당 값들을 인덱싱하기 위해
                            // 모든 씬을 번들로부터 한번 로드한 뒤, 해당 씬의 전체 경로를 알아낸다.
                            case AssetLoadType.FromAssetBundle:
                            {
                                        
                                // 에셋번들은 지정한 씬의 씬 이름을 포함한 번들을 한번에 로드하는 구조로 되어 있기 때문에
                                // 각 테이블의 첫번째 씬 이름을 기준으로 해당 테이블의 씬들의 풀패스를 배열로 가져올 수 있다.
                                var targetRecordFirstSceneName = targetTable as ICommonSceneListData;

                                        var tryFirstSceneName = targetRecordFirstSceneName?.FirstRecord?.SceneName;
                                        if (!string.IsNullOrEmpty(tryFirstSceneName))
                                        {
                                            List<string> bundleScenePathGroup = new List<string>();
                                            if (sceneDataType != SceneDataType.SystemHiddenScene)
                                            {
                                                bundleScenePathGroup = new List<string>(scenePathCollection[sceneDataType]);
                                            }
                                            else
                                            {
                                                bundleScenePathGroup = new List<string>(
                                                await LoadAssetManager.GetInstanceUnSafe.LoadAllScenes
                                                (
                                                    ResourceLifeCycleType.Scene,
                                                    targetRecordFirstSceneName.FirstRecord.SceneName
                                                ));
                                            }
                                            // 번들로부터 로드한 씬 풀 패스에 대해서
                                            foreach (var bundleScenePath in bundleScenePathGroup)
                                            {
                                                // 풀패스가 번들로부터 가져올 수 있다.
                                                var trySceneFullPath = bundleScenePath;
                                                var trySceneName = trySceneFullPath.GetFileNameFromPath(true);
                                                // 원본 테이블로부터 동명의 이름을 가지는 씬 인덱스를 검색한다.
                                                var trySceneIndex = targetRecordFirstSceneName.GetSceneIndex(trySceneName);
                                                // 이미 빌드 씬 타입에 의해 초기화된 씬 이름은 버린다. 
                                                // 즉, 에셋번들보다는 빌드 씬에 먼저등록된 동명의 씬이 우선도가 높다.
                                                if (!SScenePresetTable.ContainsKey(trySceneName))
                                                {
                                                    var scenePreset = new SceneControllerTool.ScenePreset(sceneDataType, trySceneIndex, trySceneName, trySceneFullPath, trySceneIndex);
                                                    SScenePresetTable[trySceneName] = scenePreset;
                                                    // 시스템 히든 씬은 열거형 상수를 통해 접근할 필요가 있으므로, 전용 컬렉션에 정리해준다.
                                                    if (sceneDataType == SceneDataType.SystemHiddenScene)
                                                    {
                                                        if (SystemTool.TryGetEnumEnumerator<SceneControllerTool.SystemHiddenSceneType>(SystemTool.GetEnumeratorType.GetAll, out var o_Enumerator))
                                                        {
                                                            foreach (var sceneType in o_Enumerator)
                                                            {
                                                                if (!SystemHiddenSceneTable.ContainsKey(sceneType) && trySceneName == $"{sceneType.ToString()}.unity")
                                                                {
                                                                    SystemHiddenSceneTable.Add(sceneType, scenePreset);
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    IScenePresetTable.Add(trySceneIndex, scenePreset);
                                                }
                                            }
                                        }
                                        break;
                                    }
                                // 만약 테이블 씬이 유니티에 의해서 로드된다면, 테이블 씬의 인덱스는
                                // 실제 유니티 빌트인 인덱스와 달라질 수 있으므로 여기에서 보정해준다.
                                case AssetLoadType.FromUnityResource:
                            {
                                var validKeySet = targetTable.GetValidKeyEnumerator();
                                foreach (var key in validKeySet)
                                {
                                    var targetRecord = targetTable.GetTableData(key);
                                    if (!ReferenceEquals(null, targetRecord))
                                    {
                                        var targetSceneName = targetRecord.SceneName;
                                        if (!string.IsNullOrEmpty(targetSceneName))
                                        {
                                            // 이미 유니티 Built-In 설정에 의해 초기화 된 씬인 경우, 인덱스 테이블이
                                            // 유니티 Built-In 설정을 기준으로 참조되도록 각 테이블을 수정해준다.
                                            if ( SScenePresetTable.ContainsKey(targetSceneName))
                                            {
                                                SScenePresetTable[targetSceneName].SetTableRecordIndex(key);
                                                IScenePresetTable[key] = SScenePresetTable[targetSceneName];
                                            }
                                            else
                                            {
                                                // 해당 씬이름이 포함되어 있지 않다는 것은
                                                // 씬 테이블 자동 갱신 기능을 비활성화하여 더미값이 테이블안에 들어있다는 것.
                                            }
                                        }
                                    }
                                }
                                break;
                            }
                        }
                        break;
                    }
                }
            }

            if (sceneLoadType == AssetLoadType.FromAssetBundle)
            {
                SceneControllerTool.UnloadAllSceneBundle();
            }
        }

        #endregion
        
        #region <Methods>

        public string[] GetLoadAllScenesName(string p_AssetName)
        {
            string[] assetName = null;
            //bundleScenePathGroup = new List<string>(scenePathCollection[sceneDataType]);
            
            foreach (var scene in scenePathCollection)
            {
                for(int i = 0; i < scene.Value.Count; i++)
                {
                    if (scene.Value[i].Contains($"/{p_AssetName}"))
                    {
                        return scene.Value.ToArray();
                    }
                    
                }
            }

            return assetName;
        }

        protected override MultiTableIndexer<int, SceneDataType, IIndexableSceneDataRecordBridge> SpawnGameDataTableCluster()
        {
            return new IntegerMultiTableIndexer<SceneDataType, IIndexableSceneDataRecordBridge>();
        }
        
        #endregion
    }
}