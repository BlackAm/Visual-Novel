using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace k514
{
    public static class SceneControllerTool
    {
        // TODO<414K+K> 해당 리전은 추후 시스템 테이블로 이식해야한다.
        #region <Consts>

        public const string DefaultSceneName = "Assets/Resources/Scenes/BootScene.unity";
        public const string BootStrapSceneName = "Assets/Resources/Scenes/BootStrapScene.unity";
        public const string BlackLoadingSceneName = "Assets/Resources/Scenes/LoadingScenes/LoadingScene_B.unity";
        public const string SolidImageLoadingSceneName = "Assets/Resources/Scenes/LoadingScenes/LoadingScene_SL.unity";
        
        private static readonly Dictionary<SystemSceneType, string> SystemSceneNameTable 
            = new Dictionary<SystemSceneType, string>
            {
                {SystemSceneType.DefaultScene, DefaultSceneName},
                {SystemSceneType.BootStrapScene, BootStrapSceneName},
                {SystemSceneType.SceneLoader, string.Empty},
            };
        
        private static readonly Dictionary<LoadingSceneType, string> LoadingSceneNameTable 
            = new Dictionary<LoadingSceneType, string>
            {
                {LoadingSceneType.Black, BlackLoadingSceneName},
                {LoadingSceneType.SolidImage, SolidImageLoadingSceneName},
            };

        #endregion

        #region <Enums>

        public enum SystemSceneType
        {
            DefaultScene,
            BootStrapScene,
            SceneLoader,
        }

        public enum SystemHiddenSceneType
        {
            /// <summary>
            /// 신규 리소스 테스트용 씬
            /// </summary>
            Dexter,
            
            /// <summary>
            /// 패스파인딩 테스트용 씬
            /// </summary>
            HighersHigh,
            
            /// <summary>
            /// 시스템 셧다운
            /// </summary>
            NullType,
            
            /// <summary>
            /// 테스트 마을
            /// </summary>
            TestVil,
            
            /// <summary>
            /// 테스트 던전
            /// </summary>
            TestDun,
        }
        
        public enum SceneControllerShortCutType
        {
            LoginScene,
            MainHomeScene,
            CharacterSelectScene,
        }
        
        public enum LoadingSceneType
        {
            Black,
            SolidImage,
        }

        [Flags]
        public enum SceneControlFlag
        {
            None = 0,
            
            /// <summary>
            /// 시스템 부팅 이후 최초의 씬로더에 의한 씬 전이인 경우
            /// </summary>
            SystemBootInvoke = 1 << 0,
            
            /// <summary>
            /// 해당 씬 전이 이후 플레이어 제어가 필요한 경우
            /// </summary>
            HasSceneControlPlayer = 1 << 1,
            
            /// <summary>
            /// 해당 씬 전이가 EntryTable을 참조하여 발생한 경우
            /// </summary>
            HasSceneEntryIndex = 1 << 2,
        }

        #endregion
        
        #region <Methods>

        public static string GetSystemSceneName(SystemSceneType p_Type)
        {
            return SystemSceneNameTable[p_Type];
        }
        
        public static string GetLoadingSceneName(LoadingSceneType p_Type)
        {
            return LoadingSceneNameTable[p_Type];
        }
        
        public static void LoadSystemScene(SystemSceneType p_Type)
        {
            switch (p_Type)
            {
                default:
                case SystemSceneType.DefaultScene:
                case SystemSceneType.BootStrapScene:
                    SceneManager.LoadScene(SystemSceneNameTable[p_Type], LoadSceneMode.Additive);
                    break;
                case SystemSceneType.SceneLoader:
                    LoadSystemScene();
                    break;
            }
        }

        public static void LoadSystemScene()
        {
            SceneManager.LoadScene(LoadingSceneNameTable[SceneControllerManager.GetInstance.CurrentSceneControlPreset.LoadingSceneType], LoadSceneMode.Single);
        }

        /// <summary>
        /// 현재 로드할 씬의 번들을 로드하는 메서드
        /// </summary>
        public static async UniTask LoadCurrentSceneBundle()
        {
            var curSceneType = SceneControllerManager.GetInstance.CurrentSceneControlPreset.ScenePreset.SceneType;
            if (SceneControllerManager.GetInstance.IsFirstSceneTransition())
            {
                await LoadSceneBundle(curSceneType);
            }
            else
            {
                var prevSceneType = SceneControllerManager.GetInstance.PrevSceneControlPreset.ScenePreset.SceneType;
                if (prevSceneType != curSceneType)
                {
                    UnloadAllSceneBundle();
                    await LoadSceneBundle(curSceneType);
                }
            }
        }

        /// <summary>
        /// 지정한 씬 타입에 대응하는 씬 번들을 로드한다.
        /// </summary>
        private static async UniTask LoadSceneBundle(SceneDataRoot.SceneDataType p_Type)
        {
            switch (p_Type)
            {
                case SceneDataRoot.SceneDataType.UnityBuiltIn:
                case SceneDataRoot.SceneDataType.SystemHiddenScene:
                    break;
                default:
                    var targetTable = SceneDataRoot.GetInstanceUnSafe[p_Type] as ICommonSceneListData;
                    await LoadAssetManager.GetInstanceUnSafe.LoadAllScenes(ResourceLifeCycleType.Free_Condition, targetTable.FirstRecord.SceneName);
                    break;
            }
        }
        
        /// <summary>
        /// 현재 초기화된 씬 중에서 유니티빌드 및 로딩번들 씬을 제외한 모든 씬을 제거한다.
        /// </summary>
        public static void UnloadAllSceneBundle()
        {
            if (SystemTool.TryGetEnumEnumerator<SceneDataRoot.SceneDataType>(SystemTool.GetEnumeratorType.GetAll, out var o_Enumerator))
            {
                foreach (var sceneType in o_Enumerator)
                {
                    switch (sceneType)
                    {
                        case SceneDataRoot.SceneDataType.UnityBuiltIn:
                        case SceneDataRoot.SceneDataType.SystemHiddenScene:
                            break;
                        default:
                            var targetTable = SceneDataRoot.GetInstanceUnSafe[sceneType] as ICommonSceneListData;
                            LoadAssetManager.GetInstanceUnSafe.UnloadAsset(null, targetTable.FirstRecord.SceneName, ResourceType.Scene, ResourceLifeCycleType.Free_Condition);
                            break;
                    }
                }
            }
        }
        
        #endregion

        #region <Class>

        public class ScenePreset
        {
            #region <Fields>

            /// <summary>
            /// 로딩씬에서 비동기 로딩할 씬의 타입
            /// </summary>
            public SceneDataRoot.SceneDataType SceneType { get; private set; }

            /// <summary>
            /// 현재 유니티엔진에서 관리하는 로드할 씬의 인덱스
            /// </summary>
            public int SceneIndex { get; private set; }

            /// <summary>
            /// 로딩씬에서 비동기 로딩할 씬의 이름
            /// </summary>
            public string SceneName { get; private set; }

            /// <summary>
            /// 로딩씬에서 비동기 로딩할 씬의 풀패스
            /// </summary>
            public string SceneFullPath { get; private set; }

            /// <summary>
            /// 해당 씬을 기술하는 테이블의 레코드 인덱스
            /// </summary>
            public int SceneTableRecordIndex { get; private set; }

            #endregion

            #region <Constructors>

            public ScenePreset(SceneDataRoot.SceneDataType p_SceneType, int p_SceneIndex, string p_SceneName, string p_SceneFullPath, int p_TableRecord = default)
            {
                SceneIndex = p_SceneIndex;
                SceneType = p_SceneType;
                SceneName = p_SceneName;
                SceneFullPath = p_SceneFullPath;
                SceneTableRecordIndex = p_TableRecord;
            }

            #endregion

            #region <Methods>

            public void SetTableRecordIndex(int p_TableRecord)
            {
                SceneTableRecordIndex = p_TableRecord;
            }

#if UNITY_EDITOR
            public override string ToString()
            {
                return $"[SceneType : {SceneType}]\n[SceneIndex : {SceneIndex}]\n[SceneTableRecordIndex : {SceneTableRecordIndex}]\n[SceneName : {SceneName}]\n[SceneFullPath : {SceneFullPath}]\n";
            }
#endif

            #endregion
        }

        #endregion
    }
}