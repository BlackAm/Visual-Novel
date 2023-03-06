using System.IO;
using UnityEditor;
using UnityEngine;

namespace k514
{
    /*public class PatchPackageBuildEditor : CustomEditorWindowBase<PatchPackageBuildEditor>
    {
        #region <Fields>

        private int _SliderValue0, _SliderValue1;
        private bool _CheckBox0;
            
        #endregion

        #region <Enums>
        #endregion
        
        #region <Callbacks>
        
        protected override void OnCreated()
        {
            PatchTool.CheckClientVersionValid();
            _SliderValue0 = PatchTool.__Version_Null;
            _SliderValue1 = SystemMaintenance.CalculateLatestBundleVersion();
        }

        protected override void OnInitiate()
        {
        }

        #endregion
        
        #region <EditorWindow>

        [MenuItem(MenuHeader + "5. PatchPackageBuilder")]
        private static async void Init()
        {
            await InitWindow(0.17f, 1f);
            await PatchHistoryTable.GetInstance();
            await NetworkNodeTableData.GetInstance();
        }

        protected override async void OnDrawEditor()
        {
            _DrawBlockFlag = true;

            var currentBundleVersion = SystemMaintenance.CalculateLatestBundleVersion();
            var currentClientVersion = PatchTool.GetClientVersion();
            var clientVersionIsNull = currentClientVersion < PatchTool.__Version_Lower_Bound;
            var hasBundle = currentBundleVersion >= PatchTool.__Version_Lower_Bound;
            
            // 1번 항목 : 현재 버전 표시
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(
                hasBundle ? 
                $" - Bundle Version : [{currentBundleVersion}]" 
                : " - Bundle Version : [--]  [생성된 번들이 없습니다.]"
            );
            EditorGUILayout.LabelField($" - Client Version : [{(clientVersionIsNull ? "적용된 패치 없음" : currentClientVersion.ToString())}]");
            EditorWindowTool.DrawHorizontalLine(Color.gray, 1f, -1f);

            if (hasBundle)
            {
                // 2번 항목 : 패치 파일 생성
                EditorGUILayout.Separator();
                EditorGUILayout.LabelField($" * 패치 파일 작성");
                EditorWindowTool.DrawHorizontalLine();
                if (currentBundleVersion >= PatchTool.__Version_Lower_Bound)
                {
                    // 패치 선택 범위는 [최소버전, 최신버전]이 되어야 한다.
                    _SliderValue0 = EditorGUILayout.IntSlider(_SliderValue0, PatchTool.__Version_Lower_Bound, currentBundleVersion);
                    if (GUILayout.Button($"[{currentBundleVersion}] 버전 까지의 링크 패치파일 만들기"))
                    {
                        PatchPackageBuilder.GetInstance.BuildPartialPatchPackage(currentBundleVersion);
                    }
                    EditorWindowTool.DrawHorizontalLine();
                    if (GUILayout.Button($"[{_SliderValue0}] 풀버전 패치파일 만들기"))
                    {
                        PatchPackageBuilder.GetInstance.BuildFullPatchPackage(_SliderValue0);
                    }
                    EditorWindowTool.DrawHorizontalLine();
                }
                
                // 3번 항목 : 클라이언트 버전 변경
                EditorWindowTool.DrawHorizontalLine();
                EditorGUILayout.Separator();
                EditorGUILayout.LabelField($" * 클라이언트 퀵 패치");
                _SliderValue1 = EditorGUILayout.IntSlider(_SliderValue1, PatchTool.__Version_Lower_Bound - 1, currentBundleVersion);

                var sliderValueIsNull = _SliderValue1 < PatchTool.__Version_Lower_Bound;
                _SliderValue1 = sliderValueIsNull ? PatchTool.__Version_Null : _SliderValue1;
                _CheckBox0 = EditorGUILayout.Toggle("네트워크 사용", _CheckBox0);
                
                if (GUILayout.Button(sliderValueIsNull ? $"[초기 버전으로 클라이언트 버전 변경" : $"[{_SliderValue1}] 버전으로 클라이언트 버전 변경"))
                {
                    await PatchPackageBuilder.GetInstance.BuildPatchPackageAndPatch(currentClientVersion, _SliderValue1, _CheckBox0);
                    goto SEG_PAINT_END;
                }
                EditorWindowTool.DrawHorizontalLine();
                
                // 4번 항목 : 패치 파일 파기
                EditorGUILayout.Separator();
                EditorGUILayout.LabelField($" * 패키지 파일 스토리지 파기");
                EditorWindowTool.DrawHorizontalLine();
                if (GUILayout.Button("파기"))
                {
                    if (Directory.Exists(SystemMaintenance.PatchPackageBranch))
                    {
                        Directory.Delete(SystemMaintenance.PatchPackageBranch, true);
                    }

                    if (Directory.Exists(SystemMaintenance.BundleDirectoryBranch))
                    {
                        Directory.Delete(SystemMaintenance.BundleDirectoryBranch, true);
                    }
                    PatchTool.InitClientVersion();
                }
                EditorWindowTool.DrawHorizontalLine();
            }
            
            SEG_PAINT_END :
            _DrawBlockFlag = false;
        }

        
        #endregion
    }*/
}