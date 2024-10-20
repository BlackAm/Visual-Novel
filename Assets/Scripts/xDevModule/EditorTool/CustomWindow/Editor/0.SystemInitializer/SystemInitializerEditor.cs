using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace BlackAm
{
    public class SystemInitializerEditor : CustomEditorWindowBase<SystemInitializerEditor>
    {
        #region <Consts>

        private static Dictionary<SystemInitializeType, (string, string)> _initializeCollection
            = new Dictionary<SystemInitializeType, (string, string)>
            {
                {
                    SystemInitializeType.All,
                    ("모든 시스템 값 초기화", "모든 초기화 사항을 적용시킵니다.")
                },
                {
                    SystemInitializeType.GlobalDefine, 
                    ("전처리기 초기화", "전처리기를 초기화합니다.")
                },
                {
                    SystemInitializeType.SystemFlagData, 
                    ("시스템 플래그 초기화", "시스템 플래그 테이블 xml을 새로 생성합니다.")
                },
                {
                    SystemInitializeType.SystemConfigData, 
                    ("시스템 설정 테이블 초기화", "시스템 설정 테이블 xml을 새로 생성합니다.")
                },
                {
                    SystemInitializeType.ResourceListData, 
                    ("리소스 리스트 테이블 초기화", "리소스 리스트 테이블 xml을 새로 생성합니다.")
                },
                {
                    SystemInitializeType.ProjectSetting, 
                    ("프로젝트 세팅 초기화", "레이어, 태그 등 프로젝트 세팅을 초기화합니다.")
                },
                {
                    SystemInitializeType.EditBuildSetting, 
                    ("빌드 씬 등록 초기화", "빌드 세팅에 씬 테이블에서 관리하는 씬을 적용시킵니다.")
                },
                {
                    SystemInitializeType.InitPlayerSetting, 
                    ("플레이어 세팅 초기화", "런타임 프로그램 관련 설정을 초기화시킵니다.")
                },
                {
                    SystemInitializeType.AlwaysIncludedShader, 
                    ("빌드 셰이더 리스트 초기화", "해당 프로젝트에 항상 포함될 셰이더의 리스트를 초기화 시킵니다.")
                },
                {
                    SystemInitializeType.PlayerPrefInitialize, 
                    ("PlayerPref 초기화", "유니티 PlayerPref을 통해 클라이언트에 저장되어 있는 값들을 초기화시킵니다.")
                },
            };

        #endregion

        #region <Fields>

        private SystemInitializeType _SystemInitializeType;

        #endregion
        
        #region <Enums>

        private enum SystemInitializeType
        {
            None,
            All,
            GlobalDefine,
            SystemFlagData,
            SystemConfigData,
            ResourceListData,
            ProjectSetting,
            EditBuildSetting,
            InitPlayerSetting,
            AlwaysIncludedShader,
            PlayerPrefInitialize,
        }

        #endregion

        #region <Callbacks>
        
        protected override void OnCreated()
        {
        }

        protected override void OnInitiate()
        {
        }

        #endregion
        
        #region <Methods>

        private async UniTask ApplyInitialize(SystemInitializeType p_Type)
        {
            switch (p_Type)
            {
                case SystemInitializeType.All:
                    if (SystemTool.TryGetEnumEnumerator<SystemInitializeType>(SystemTool.GetEnumeratorType.ExceptNone, out var o_Enumerator))
                    {
                        foreach (var systemInitializeType in o_Enumerator)
                        {
                            if (systemInitializeType != SystemInitializeType.All)
                            {
                                await ApplyInitialize(systemInitializeType);
                            }
                        }
                    }
                    break;
                case SystemInitializeType.GlobalDefine:
                    GlobalDefineSetter.GetInstance.ClearDefine();
                    break;
                case SystemInitializeType.SystemFlagData :
                    await SystemFlag.GetInstanceUnSafe.CreateDefaultTable(true);
                    break;
                case SystemInitializeType.SystemConfigData:
                    await SystemConfigData.GetInstanceUnSafe.CreateDefaultTable(true);
                    await SystemMaintenance.ApplyBackupResourceDirectory();
                    break;
                case SystemInitializeType.ResourceListData:
                    await ResourceListData.GetInstanceUnSafe.CreateDefaultTable(true);
                    break;
                case SystemInitializeType.ProjectSetting:
                    SystemMaintenance.CreateTag();
                    SystemMaintenance.CreateLayer();
                    SystemMaintenance.CreateSortingLayer();
                    break;
                case SystemInitializeType.EditBuildSetting :
                    await SystemMaintenance.CreateBuildSetting();
                    break;
                case SystemInitializeType.InitPlayerSetting :
                    SystemMaintenance.InitPlayerSetting();
                    break;
                case SystemInitializeType.AlwaysIncludedShader :
                    await SystemFlag.GetInstanceUnSafe.CreateDefaultTable(true);
                    break;
                case SystemInitializeType.PlayerPrefInitialize :
                    PlayerPrefs.DeleteAll();
                    break;
            }
            
            // 프로젝트 파일시스템 최신화
            await UniTask.SwitchToMainThread();
            AssetDatabase.Refresh();
        }

        #endregion
            
        #region <EditorWindow>

        [MenuItem(MenuHeader + "0. SystemInitializer")]
        private static async void Init()
        {
            await InitWindow(0.15f, 1.2f);
        }

        protected override async void OnDrawEditor()
        {
            _DrawBlockFlag = true;
            
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(" * 초기화 기능 목록 * ");
            EditorWindowTool.DrawHorizontalLine(Color.gray, 1f, -1f);

            if (SystemTool.TryGetEnumEnumerator<SystemInitializeType>(SystemTool.GetEnumeratorType.ExceptNone,out var o_Enumerator))
            {
                foreach (var initializeType in o_Enumerator)
                {
                    var targetInitializeName = _initializeCollection[initializeType].Item1;
                    if (GUILayout.Button($"{targetInitializeName} 선택"))
                    {
                        _SystemInitializeType = initializeType;
                    }
                }
            }
            EditorGUILayout.Separator();

            switch (_SystemInitializeType)
            {
                case SystemInitializeType.None:
                    break;
                default:
                    var targetPredefineProcessorNamePair = _initializeCollection[_SystemInitializeType]; 
                    GUILayout.TextArea(targetPredefineProcessorNamePair.Item2);
                    EditorGUILayout.Separator();
                    
                    var targetPredefineProcessorName = targetPredefineProcessorNamePair.Item1;
                    if (GUILayout.Button($"{targetPredefineProcessorName} 적용"))
                    {
                        await ApplyInitialize(_SystemInitializeType);
                        goto SEG_PAINT_END;
                    }
                    break;
            }
            
            SEG_PAINT_END :
            _DrawBlockFlag = false;
        }

        #endregion
    }
}