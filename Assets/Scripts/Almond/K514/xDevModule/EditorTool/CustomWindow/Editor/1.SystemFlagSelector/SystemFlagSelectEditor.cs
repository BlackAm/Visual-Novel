using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace k514
{
    public class SystemFlagSelectEditor : CustomEditorWindowBase<SystemFlagSelectEditor>
    {
        #region <Fields>

        /// <summary>
        /// 현재 선택된 리소스 타입별, 로드 타입 테이블
        /// </summary>
        private Dictionary<ResourceType, int> _CurrentSystemResourceLoadType;

        /// <summary>
        /// 에셋 로드 타입 이름 반복자
        /// </summary>
        private string[] _AssetLoadTypeNameEnumerator;
        
        #endregion

        #region <Callbacks>

        protected override void OnCreated()
        {
            _AssetLoadTypeNameEnumerator =
                SystemTool.GetEnumStringEnumerator<AssetLoadType>(SystemTool.GetEnumeratorType.ExceptNone);
            _CurrentSystemResourceLoadType = new Dictionary<ResourceType, int>();
            if (SystemTool.TryGetEnumEnumerator<ResourceType>(SystemTool.GetEnumeratorType.ExceptNone, out var o_Enumerator))
            {
                foreach (var resourceType in o_Enumerator)
                {
                    if (!resourceType.IsFixedLoadTypeResource())
                    {
                        _CurrentSystemResourceLoadType.Add(resourceType, (int) resourceType.GetResourceLoadType());
                    }
                }
            }
        }

        protected override void OnInitiate()
        {
            if (SystemTool.TryGetEnumEnumerator<ResourceType>(SystemTool.GetEnumeratorType.ExceptNone, out var o_Enumerator))
            {
                foreach (var resourceType in o_Enumerator)
                {
                    if (!resourceType.IsFixedLoadTypeResource())
                    {
                        _CurrentSystemResourceLoadType[resourceType] = (int) resourceType.GetResourceLoadType();
                    }
                }
            }
        }
        
        #endregion
        
        #region <Methods>
        
        /// <summary>
        /// 커스텀 윈도우에 의해 지정된 에셋 로드 타입을 테이블에 업데이트 시키는 메서드
        /// </summary>
        private async UniTask ApplySystemConfig()
        {
            var systemConfigData = SystemConfigData.GetInstanceUnSafe;
            systemConfigData.ClearTable();
            foreach (var alterRecord in _CurrentSystemResourceLoadType)
            {
                await systemConfigData.AddRecord(alterRecord.Key, alterRecord.Value);
            }
            await systemConfigData.UpdateTableFile(ExportDataTool.WriteType.Overlap);
        }
        
        #endregion
         
        #region <EditorWindow>
 
        [MenuItem(MenuHeader + "1. SystemFlagSelector")]
        private static async void Init()
        {
            await InitWindow(0.13f, 2.6f);
            await LoadAssetManager.GetInstance();
        }
 
        protected override async void OnDrawEditor()
        {
            _DrawBlockFlag = true;

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(" * 에셋 로드 타입 * ");
            EditorWindowTool.DrawHorizontalLine(Color.gray, 1f, -1f);
          
            var onceFlag = false;
            if (SystemTool.TryGetEnumEnumerator<ResourceType>(SystemTool.GetEnumeratorType.ExceptNone, out var o_Enumerator))
            {
                foreach (var resourceType in o_Enumerator)
                {
                    if (!resourceType.IsFixedLoadTypeResource())
                    {
                        if (onceFlag)
                        {
                            EditorWindowTool.DrawHorizontalLine(Color.black, 0.5f, -1f);
                        }
                        else
                        {
                            onceFlag = true;
                        }
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField($"{resourceType}");
                        _CurrentSystemResourceLoadType[resourceType] = EditorGUILayout.Popup(_CurrentSystemResourceLoadType[resourceType], _AssetLoadTypeNameEnumerator);
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
            EditorWindowTool.DrawHorizontalLine(Color.gray, 1f, -1f);
             
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(" * 일괄변경 * ");
            EditorWindowTool.DrawHorizontalLine();
            if (GUILayout.Button($"에셋번들 로드 타입으로 일괄 변경"))
            {
                foreach (var resourceType in o_Enumerator)
                {
                    if (!resourceType.IsFixedLoadTypeResource())
                    {
                        _CurrentSystemResourceLoadType[resourceType] = (int)AssetLoadType.FromAssetBundle;
                    }
                }
            }
             
            if (GUILayout.Button($"유니티 로드 타입으로 일괄 변경"))
            {
                foreach (var resourceType in o_Enumerator)
                {
                    if (!resourceType.IsFixedLoadTypeResource())
                    {
                        _CurrentSystemResourceLoadType[resourceType] = (int)AssetLoadType.FromUnityResource;
                    }
                }
            }
            EditorWindowTool.DrawHorizontalLine();
             
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(" * 테이블 갱신 * ");
            EditorWindowTool.DrawHorizontalLine();
            if (GUILayout.Button($"리소스 로드 타입 변경 사항을 적용"))
            {
                await ApplySystemConfig();
                AssetDatabase.Refresh();
                goto SEG_PAINT_END;
            }
            EditorWindowTool.DrawHorizontalLine();
             
            var isReleaseMode = SystemFlag.IsSystemReleaseMode();
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField($" * 현재 배포 모드 : {isReleaseMode}  * ");
            EditorWindowTool.DrawHorizontalLine();
            if (GUILayout.Button($"배포 모드를 바꾼다."))
            {
                isReleaseMode = !isReleaseMode;
                await SystemFlag.GetInstanceUnSafe.UpdateSystemFlagReleaseMode(SystemFlag.SystemFlagType.ReleaseMode, isReleaseMode);
                AssetDatabase.Refresh();
                goto SEG_PAINT_END;
            }
             
            if (EditorWindowTool.ConditionalButton("리소스 폴더 재배치", isReleaseMode))
            {
                await SystemMaintenance.ApplyBackupResourceDirectory();
                 
                // 씬 폴더가 돌아왔다면, 빌드 세팅을 갱신해준다.
                await SystemMaintenance.CreateBuildSetting();
                goto SEG_PAINT_END;
            }
            EditorWindowTool.DrawHorizontalLine();
             
            var isAutoPatchMode = SystemFlag.IsAutoPatchMode();
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField($" * 개발모드에서 자동 패치 : {isAutoPatchMode}  * ");
            EditorWindowTool.DrawHorizontalLine();
            if (GUILayout.Button($"패치 모드를 바꾼다."))
            {
                isAutoPatchMode = !isAutoPatchMode;
                await SystemFlag.GetInstanceUnSafe.UpdateSystemFlagReleaseMode(SystemFlag.SystemFlagType.AutoPatchMode, isAutoPatchMode);
                AssetDatabase.Refresh();
                goto SEG_PAINT_END;
            }
             
            EditorWindowTool.DrawHorizontalLine();
                          
            var isAutoUpdateResourceList = SystemFlag.GetAutoUpdateResourceListFlag();
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField($" * 리소스 리스트 자동 갱신 : {isAutoUpdateResourceList}  * ");
            EditorWindowTool.DrawHorizontalLine();
            if (GUILayout.Button($"갱신 설정 변경"))
            {
                isAutoUpdateResourceList = !isAutoUpdateResourceList;
                await SystemFlag.GetInstanceUnSafe.UpdateSystemFlagReleaseMode(SystemFlag.SystemFlagType.ResourceListAutoUpdate, isAutoUpdateResourceList);
                AssetDatabase.Refresh();
                goto SEG_PAINT_END;
            }
            EditorWindowTool.DrawHorizontalLine();
            
            var isTableLoadUsingByteImage = SystemFlag.IsTableByteImageMode();
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField($" * 테이블을 바이트코드를 사용해 로딩 : {isTableLoadUsingByteImage}  * ");
            EditorWindowTool.DrawHorizontalLine();
            if (GUILayout.Button($"로딩 방식 변경"))
            {
                isTableLoadUsingByteImage = !isTableLoadUsingByteImage;
                await SystemFlag.GetInstanceUnSafe.UpdateSystemFlagReleaseMode(SystemFlag.SystemFlagType.UsingByteByteCode, isTableLoadUsingByteImage);
                AssetDatabase.Refresh();
                goto SEG_PAINT_END;
            }
            EditorWindowTool.DrawHorizontalLine();
             
            SEG_PAINT_END :
            _DrawBlockFlag = false;
        }
         
        #endregion
    }
}