using UnityEditor;
using UnityEngine;

namespace BlackAm
{
    /*public class AssetBundleBuildEditor : CustomEditorWindowBase<AssetBundleBuildEditor>
    {
        #region <Fields>

        private string _VersionDescription;
        private Vector2 _VersionDescriptionScroll;
        
        #endregion
        
        #region <Callbacks>
        
        protected override void OnCreated()
        {
            _VersionDescription = AssetBundleBuilder._DefaultVersionDescription;
        }

        protected override void OnInitiate()
        {
        }

        #endregion
        
        #region <EditorWindow>

        [MenuItem(MenuHeader + "4. AssetBundleBuilder")]
        private static async void Init()
        {
            await InitWindow(0.15f, 1.3f);
            await PatchHistoryTable.GetInstance();
        }

        protected override async void OnDrawEditor()
        {
            _DrawBlockFlag = true;

            var currentBundleVersion = SystemMaintenance.CalculateLatestBundleVersion();
            var isNewal = currentBundleVersion < 0;

            /* 번들 생성/업데이트 #1#
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(isNewal ? $" * 이전 버전 없음" : $" * 현재 버전 [{currentBundleVersion}]");
            EditorWindowTool.DrawHorizontalLine(Color.gray, 1f, -1f);

            EditorGUILayout.BeginHorizontal();
            if (isNewal)
            {
                if (EditorWindowTool.ConditionalButton("신규생성", true))
                {
                    AssetBundleBuilder.GetInstance.CurrentState =
                        AssetBundleBuilder.AssetBuilderHelperState.CreateNewBundle;
                }
            }
            else
            {
                if (AssetBundleBuilder.GetInstance.GetVersionModifiedFlag())
                {
                    if (EditorWindowTool.ConditionalButton("업데이트", true))
                    {
                        AssetBundleBuilder.GetInstance.CurrentState =
                            AssetBundleBuilder.AssetBuilderHelperState.Update;
                    }
                }
                else
                {
                    EditorGUILayout.LabelField($" >> 최신버전과 변경사항이 없으므로 수행할 수 있는 기능이 없습니다.");
                    
                    var nonModifiedStateTransition = AssetBundleBuilder.GetInstance.CurrentState !=
                                                   AssetBundleBuilder.AssetBuilderHelperState.Terminate;
                    
                    if (nonModifiedStateTransition)
                    {
                        AssetBundleBuilder.GetInstance.CurrentState =
                            AssetBundleBuilder.AssetBuilderHelperState.NonModified;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorWindowTool.DrawHorizontalLine();
                                     
            /* 패치 #1#
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField($" * 패치 적용");
            EditorWindowTool.DrawHorizontalLine();

            if (isNewal)
            {
                EditorGUILayout.LabelField("패치할 에셋번들이 존재하지 않습니다.");
            }
            else
            {
                if (EditorWindowTool.ConditionalButton("미구현", true))
                {
//                    AssetBundleBuilder.GetInstance.CurrentState =
//                        AssetBundleBuilder.AssetBuilderHelperState.Terminate;
                }
            }
            EditorWindowTool.DrawHorizontalLine();
            
            /* 번들 파기 #1#
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField($" * 에셋 번들 파기");
            EditorWindowTool.DrawHorizontalLine();

            if (isNewal)
            {
                EditorGUILayout.LabelField("파기할 에셋번들이 존재하지 않습니다.");
            }
            else
            {
                if (EditorWindowTool.ConditionalButton("파기", true))
                {
                    AssetBundleBuilder.GetInstance.CurrentState =
                        AssetBundleBuilder.AssetBuilderHelperState.Terminate;
                }
            }
            EditorWindowTool.DrawHorizontalLine();
            
            /* 상태 설명 #1#
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField($" * 현재 상태 설명");
            EditorWindowTool.DrawHorizontalLine();

            EditorWindowTool.LabelFieldWrapper(AssetBundleBuilder.GetInstance.GetCurrentDescription());
            EditorWindowTool.DrawHorizontalLine();

            /* 버전 설명문 기술 #1#
            if (AssetBundleBuilder.GetInstance.CurrentState == AssetBundleBuilder.AssetBuilderHelperState.CreateNewBundle
                    || AssetBundleBuilder.GetInstance.CurrentState == AssetBundleBuilder.AssetBuilderHelperState.Update
                )
            {
                EditorGUILayout.Separator();
                EditorGUILayout.LabelField($" * 버전 설명");
                _VersionDescriptionScroll = EditorGUILayout.BeginScrollView(_VersionDescriptionScroll);
                _VersionDescription = EditorGUILayout.TextArea(_VersionDescription, GUILayout.Height(100));
                EditorGUILayout.EndScrollView();
                EditorWindowTool.DrawHorizontalLine();
            }

            /* 기능 수행 #1#
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField($" * 기능 수행");
            EditorWindowTool.DrawHorizontalLine();
            
            var confirmButtonStagingFlag = AssetBundleBuilder.GetInstance.CurrentState !=
                                           AssetBundleBuilder.AssetBuilderHelperState.NonSelected
                                           && AssetBundleBuilder.GetInstance.CurrentState !=
                                           AssetBundleBuilder.AssetBuilderHelperState.NonModified;
            
            if (EditorWindowTool.ConditionalButton("실행", confirmButtonStagingFlag))
            {
                await AssetBundleBuilder.GetInstance.ActSelectedTask
                (
                    new AssetBundleBuilder.AssetBundleBuilderEventPreset{ VersionDescription = _VersionDescription}
                );
                goto SEG_PAINT_END;
            }
            
            if (EditorWindowTool.ConditionalButton("취소", confirmButtonStagingFlag))
            {
                _VersionDescription = AssetBundleBuilder._DefaultVersionDescription;
                AssetBundleBuilder.GetInstance.CurrentState =
                    AssetBundleBuilder.AssetBuilderHelperState.NonSelected;
            }
            EditorWindowTool.DrawHorizontalLine();
            
            SEG_PAINT_END :
            _DrawBlockFlag = false;
        }
        
        #endregion
    }*/
}