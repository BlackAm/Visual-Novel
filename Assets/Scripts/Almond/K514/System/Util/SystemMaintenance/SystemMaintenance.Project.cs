using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

namespace k514
{
    public partial class SystemMaintenance
    {
#if UNITY_EDITOR
        /// <summary>
        /// 플레이어 세팅을 초기화 시키는 메서드
        /// </summary>
        public static void InitPlayerSetting()
        {
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            PlayerSettings.defaultScreenWidth = DEFAULT_SCREEN_WIDTH / 2;
            PlayerSettings.defaultScreenHeight = DEFAULT_SCREEN_HEIGHT / 2;
            PlayerSettings.runInBackground = true;
            PlayerSettings.captureSingleScreen = true;
            PlayerSettings.usePlayerLog = true;
            PlayerSettings.resizableWindow = false;
            PlayerSettings.visibleInBackground = true;
            PlayerSettings.allowFullscreenSwitch = true;
            PlayerSettings.forceSingleInstance = true;

            // QualitySettings.vSyncCount = 0; // Don't Sync
            QualitySettings.vSyncCount = 1; // Every V Blank
        }

        public static void TryCheckSystemSceneSetting()
        {
            var resultBuiltInList = EditorBuildSettings.scenes.ToList();
            var resultBuiltInSceneNameList = GetBuiltInScenePathSet();
            
            var systemSceneEnumerator = SystemTool.GetEnumEnumerator<SceneControllerTool.SystemSceneType>(SystemTool.GetEnumeratorType.GetAll);
            foreach (var sceneType in systemSceneEnumerator)
            {
                switch (sceneType)
                {
                    default:
                    case SceneControllerTool.SystemSceneType.DefaultScene:
                    case SceneControllerTool.SystemSceneType.BootStrapScene:
                        var trySceneName = SceneControllerTool.GetSystemSceneName(sceneType);
                        if (!resultBuiltInSceneNameList.Contains(trySceneName))
                        {
                            resultBuiltInList.Add(new EditorBuildSettingsScene(trySceneName, true));
                        }

                        break;
                    case SceneControllerTool.SystemSceneType.SceneLoader:
                        break;
                }
            } 
            
            EditorBuildSettings.scenes = resultBuiltInList.ToArray();
        }

        /// <summary>
        /// 현재 씬 테이블에 등록되어 있는 씬들을 빌드 세팅에 등록시키는 메서드
        /// </summary>
        public static async UniTask CreateBuildSetting()
        {
            var resourceListData = await ResourceListData.GetInstance();
            var sceneDataRoot = await SceneDataRoot.GetInstanceUnSafeWaiting() as SceneDataRoot;
            var resultBuiltInList = new List<EditorBuildSettingsScene>();
            
            await UniTask.SwitchToMainThread();
            var systemSceneEnumerator = SystemTool.GetEnumEnumerator<SceneControllerTool.SystemSceneType>(SystemTool.GetEnumeratorType.GetAll);
            foreach (var sceneType in systemSceneEnumerator)
            {
                switch (sceneType)
                {
                    default:
                    case SceneControllerTool.SystemSceneType.DefaultScene:
                    case SceneControllerTool.SystemSceneType.BootStrapScene:
                        var trySceneName = SceneControllerTool.GetSystemSceneName(sceneType);
                        resultBuiltInList.Add(new EditorBuildSettingsScene(trySceneName, true));
                        break;
                    case SceneControllerTool.SystemSceneType.SceneLoader:
                        break;
                }
            }

            var loadingSceneEnumerator = SystemTool.GetEnumEnumerator<SceneControllerTool.LoadingSceneType>(SystemTool.GetEnumeratorType.GetAll);
            foreach (var sceneType in loadingSceneEnumerator)
            {
                var trySceneName = SceneControllerTool.GetLoadingSceneName(sceneType);
                resultBuiltInList.Add(new EditorBuildSettingsScene(trySceneName, true));
            }
            
            // 씬 로드 타입이 에셋번들이 아니라면, 리소스 폴더내의 모든 씬을 빌트인 리스트에 등록해준다.
            if (GetResourceLoadType(ResourceType.Scene) != AssetLoadType.FromAssetBundle)
            {
                // 씬 테이블 초기화
                if (SystemTool.TryGetEnumEnumerator<SceneDataRoot.SceneDataType>(SystemTool.GetEnumeratorType.GetAll, out var o_Enumerator))
                {
                    foreach (var sceneType in o_Enumerator)
                    {
                        switch (sceneType)
                        {
                            case SceneDataRoot.SceneDataType.UnityBuiltIn:
                                break;
                            default:
                                var targetTable = sceneDataRoot[sceneType];
                                var validKeySet = targetTable.GetValidKeyEnumerator();
                                foreach (var key in validKeySet)
                                {
                                    var targetRecord = targetTable.GetTableData(key);
                                    if (!ReferenceEquals(null, targetRecord))
                                    {
                                        var targetSceneName = targetRecord.SceneName;
                                        if (!string.IsNullOrEmpty(targetSceneName))
                                        {
                                            if (resourceListData.HasKey(targetSceneName))
                                            {
                                                var fullPath = $"{UnityResourceDirectory}{resourceListData.GetTableData(targetSceneName).GetResourceFullPath().CutString(UnityResourceDirectory, false, true)}";
                                                resultBuiltInList.Add(new EditorBuildSettingsScene(fullPath, true));
                                            }
                                        }
                                    } 
                                }
                                break;
                        }
                    }
                }
            }

            EditorBuildSettings.scenes = resultBuiltInList.ToArray();
        }

        /// <summary>
        /// 현재 빌드 세팅에 등록된 씬 이름 리스트를 생성하여 반환하는 메서드
        /// </summary>
        public static string[] GetBuiltInScenePathSet()
        {
            return EditorBuildSettings.scenes.Select(scene => scene.path).ToArray();
        }
        
        /// <summary>
        /// 시스템에서 지정한 태그 네임을 프로젝트에 초기화 하는 메서드
        /// </summary>
        public static void CreateTag()
        {
            ClearTag();
            
            SerializedObject tagManager = 
                new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            var defaultTagList = InternalEditorUtility.tags;
            var targetEnumerator = SystemTool.GetEnumEnumerator<GameManager.GameTagType>(SystemTool.GetEnumeratorType.GetAll);
            foreach (var tagNamePair in targetEnumerator)
            {
                var tryName = tagNamePair.ToString();
                if (!defaultTagList.Contains(tryName))
                {
                    for (int i = 0; i < tagsProp.arraySize; i++)
                    {
                        SerializedProperty sp = tagsProp.GetArrayElementAtIndex(i);
                        // 이미 있는 태그라면 넘긴다.
                        if (sp != null && tryName.Equals(sp.stringValue))
                        {
                            goto LOOP1;
                        }
                    }
                    tagsProp.InsertArrayElementAtIndex(0);
                    tagsProp.GetArrayElementAtIndex(0).stringValue = tryName;
                }
                
                LOOP1: ;
            }
            tagManager.ApplyModifiedPropertiesWithoutUndo();
        }

        /// <summary>
        /// 시스템에서 지정한 태그 네임을 프로젝트에 초기화 하는 메서드
        /// </summary>
        public static void ClearTag()
        {
            SerializedObject tagManager = 
                new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");
            var currentTagPropCount = tagsProp.arraySize;

            for (int i = currentTagPropCount - 1; i > -1; i--)
            {
                tagsProp.DeleteArrayElementAtIndex(i);
            }
            tagManager.ApplyModifiedPropertiesWithoutUndo();
        }
                
        
        /// <summary>
        /// 시스템에서 지정한 레이어 네임을 프로젝트에 초기화 하는 메서드
        /// </summary>
        public static void CreateLayer()
        {
            ClearLayer();
            
            SerializedObject layerManager = 
                new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layersProp = layerManager.FindProperty("layers");
            
            var targetEnumerator = SystemTool.GetEnumEnumerator<GameManager.GameLayerType>(SystemTool.GetEnumeratorType.GetAll);
            foreach (var layerNamePair in targetEnumerator)
            {
                var tryName = layerNamePair.ToString();
                if(string.IsNullOrEmpty(tryName)) continue;
                for (int i = 0; i < layersProp.arraySize; i++)
                {
                    SerializedProperty sp = layersProp.GetArrayElementAtIndex(i);
                    // 이미 있는 레이어라면 넘긴다.
                    if (sp != null && tryName.Equals(sp.stringValue.RemoveSpace()))
                    {
                        goto LOOP1;
                    }
                }
 
                SerializedProperty slot = null;
                for (int i = 8; i < layersProp.arraySize; i++)
                {
                    SerializedProperty sp = layersProp.GetArrayElementAtIndex(i);
                    if (sp != null && string.IsNullOrEmpty(sp.stringValue))
                    {
                        slot = sp;
                        break;
                    }
                }

                if (slot != null)
                {
                    slot.stringValue = tryName;
                }
                
                LOOP1: ;
            }
            layerManager.ApplyModifiedPropertiesWithoutUndo();
        }
        
        public static void ClearLayer()
        {
            SerializedObject layerManager = 
                new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            
            SerializedProperty layersProp = layerManager.FindProperty("layers");
            var currentLayerPropCount = layersProp.arraySize;

            for (int i = currentLayerPropCount - 1; i > 7; i--)
            {
                layersProp.DeleteArrayElementAtIndex(i);
            }

            layerManager.ApplyModifiedPropertiesWithoutUndo();
        }

        /// <summary>
        /// 시스템에서 지정한 레이어 네임을 프로젝트에 초기화 하는 메서드
        /// </summary>
        public static void CreateSortingLayer()
        {
            ClearSortingLayer();
            SerializedObject sortingLayerManager = 
                new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty sortingLayersProp = sortingLayerManager.FindProperty("m_SortingLayers");

            var targetEnumerator = SystemTool.GetEnumEnumerator<GameManager.GameSortingLayerType>(SystemTool.GetEnumeratorType.GetAll);
            foreach (var sortingLayerNamePair in targetEnumerator)
            {
                var tryName = sortingLayerNamePair.ToString();
                for (int i = 0; i < sortingLayersProp.arraySize; i++)
                {
                    SerializedProperty sp = sortingLayersProp.GetArrayElementAtIndex(i);
                    SerializedProperty spName = sp.FindPropertyRelative("name");
                    // 이미 있는 레이어라면 넘긴다.
                    if (tryName.Equals(spName.stringValue))
                    {
                        goto LOOP1;
                    }
                }
 
                // 신규 sortingLayer를 등록해준다. 등록하기 전 후에 tagManager.asset을 갱신시켜준다.
                // Layer는 32개의 빈공간이 이미 존재하지만, sortingLayer는 따로 공간을 할당시키고 이름을 짓는 방식으로
                // 구현되어 있기 때문
                sortingLayerManager.ApplyModifiedProperties();
                {
                    // sortingLayer를 추가하는 AddSortingLayer 메서드를 Editor 어셈블리에서 가져온다.
                    var editorAsm = Assembly.GetAssembly(typeof(Editor));
                    Type t = editorAsm.GetType("UnityEditorInternal.InternalEditorUtility");
                    var AddSortingLayer_Method = t.GetMethod("AddSortingLayer", (BindingFlags.Static | BindingFlags.NonPublic), null, new Type[0], null);
                    AddSortingLayer_Method?.Invoke(null, null);
                }
                sortingLayerManager.Update();
 
                // 신규 추가된 sortingLayer의 이름을 변경해준다.
                int idx = sortingLayersProp.arraySize - 1;
                SerializedProperty entry = sortingLayersProp.GetArrayElementAtIndex(idx);
                entry.FindPropertyRelative("name").stringValue = tryName;
                
                LOOP1: ;
            }
            sortingLayerManager.ApplyModifiedPropertiesWithoutUndo();
        }

        public static void ClearSortingLayer()
        {
            SerializedObject sortingLayerManager =
                new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty sortingLayersProp = sortingLayerManager.FindProperty("m_SortingLayers");

            var currentSortingLayerPropCount = sortingLayersProp.arraySize;
            for (int i = currentSortingLayerPropCount - 1; i > -1; i--)
            {
                SerializedProperty sp = sortingLayersProp.GetArrayElementAtIndex(i);
                SerializedProperty spName = sp.FindPropertyRelative("name");
                if (!spName.stringValue.Equals("Default"))
                {
                    sortingLayersProp.DeleteArrayElementAtIndex(i);
                }
            }
            sortingLayerManager.ApplyModifiedProperties();
            sortingLayerManager.Update();
        }
        
        /// <summary>
        /// 프로젝트의 그래픽 세팅 > Always Included Shader 직렬화 오브젝트를 리턴하는 메서드
        /// </summary>
        private static SerializedObject Get_AlwaysIncludedShader_SerializedObject()
        {
            var graphicsSettingsObj = AssetDatabase.LoadAssetAtPath<GraphicsSettings>("ProjectSettings/GraphicsSettings.asset");
            return new SerializedObject(graphicsSettingsObj);
        }

        /// <summary>
        /// 현재 ShaderCrawler의 테이블 레코드를 기반으로 Always Included Shader를 갱신하는 메서드
        /// </summary>
        public static async UniTask ApplyCurrentShaderCrawler()
        {
            var serializedObject = Get_AlwaysIncludedShader_SerializedObject();
            var arrayProp = serializedObject.FindProperty("m_AlwaysIncludedShaders");
            var shaderTable = (await ShaderCrawler.GetInstance()).GetTable();
            arrayProp.ClearArray();

            var cnt = 0;
            foreach (var shaderRecord in shaderTable)
            {
                arrayProp.InsertArrayElementAtIndex(cnt);
                var arrayElem = arrayProp.GetArrayElementAtIndex(cnt++);
                arrayElem.objectReferenceValue = Shader.Find(shaderRecord.Value.KEY);
            }
            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}