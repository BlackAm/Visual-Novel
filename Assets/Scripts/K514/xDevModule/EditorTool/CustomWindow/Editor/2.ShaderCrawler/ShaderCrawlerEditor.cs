using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BlackAm
{
    public class ShaderCrawlerEditor : CustomEditorWindowBase<ShaderCrawlerEditor>
    {
        #region <Fields>

        private int _DropSelected;
        private Vector2 _ScrollPosition, _ScrollPosition2, _ScrollPosition3;
        private bool _ToggleFlag;
        
        private Dictionary<string, ShaderInfoPreset> _ShaderInfoCollection, _NonIncluded_ShaderInfoCollection;
        private ShaderInfoPreset _CurrentSelectedShaderInfoPreset;
        private List<ShaderInfoPreset> _CurrentFocusedShaderInfoPresetList;

        #endregion

        #region <Enums>

        private enum ShaderCrawlerHelperButtonType
        {
            AddTo, RemoveFrom
        }

        #endregion
        
        #region <Callbacks>
        
        protected override void OnCreated()
        {
        }

        protected override void OnInitiate()
        {
            UpdateFocusedRendererGroup();
            UpdateRendererGroup();
        }

        #endregion
        
        #region <Methods>

        private void UpdateFocusedRendererGroup()
        {
            if (_CurrentFocusedShaderInfoPresetList == null)
            {
                _CurrentFocusedShaderInfoPresetList = new List<ShaderInfoPreset>();
            }
            else
            {
                _CurrentFocusedShaderInfoPresetList.Clear();
            }
        }

        private void UpdateRendererGroup()
        {
            if (_ShaderInfoCollection == null)
            {
                _ShaderInfoCollection = new Dictionary<string, ShaderInfoPreset>();
            }
            else
            {
                _ShaderInfoCollection.Clear();
            }

            if (_NonIncluded_ShaderInfoCollection == null)
            {
                _NonIncluded_ShaderInfoCollection = new Dictionary<string, ShaderInfoPreset>();
            }
            else
            {
                _NonIncluded_ShaderInfoCollection.Clear();
            }
            
            var targetShaderCrawlerTable = ShaderCrawler.GetInstanceUnSafe.GetTable();
            foreach (var shaderRecord in targetShaderCrawlerTable)
            {
                var shaderName = shaderRecord.Key;
                _ShaderInfoCollection.Add(shaderName, new ShaderInfoPreset(shaderName, true));
            }
            
            // 모든 프리팹에 대해
//            var gameObjectSet = AssetDatabase.GetAllAssetPaths();
//            foreach (var assetPath in gameObjectSet)
//            {
//                var tryGameObject = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
//                if (tryGameObject != null)
//                {
//                    var tryRenderers = tryGameObject.GetComponentsInChildren<Renderer>();
//                    foreach (var targetRenderer in tryRenderers)
//                    {
//                        var targetMaterials = targetRenderer.sharedMaterials;
//                        foreach (var targetMaterial in targetMaterials)
//                        {
//                            var targetShader = targetMaterial?.shader;
//                            if (targetShader != null)
//                            {
//                                var targetShaderName = targetShader.name;
//                                if (!_ShaderInfoCollection.ContainsKey(targetShaderName))
//                                {
//                                    var spawnedShaderInfo = new ShaderInfoPreset(targetShaderName, false);
//                                    _ShaderInfoCollection.Add(targetShaderName, spawnedShaderInfo);
//                                    _NonIncluded_ShaderInfoCollection.Add(targetShaderName, spawnedShaderInfo);
//                                }
//
//                                var targetDependencies = _ShaderInfoCollection[targetShaderName].DefendencyGroup;
//                                var tryAssetObject = targetRenderer.gameObject;
//                                if (!targetDependencies.Contains(tryAssetObject))
//                                {     
//                                    targetDependencies.Add(tryAssetObject);
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//            
            // ResourceTracker를 참조하여 셰이더 추가
            var trackerTable = ResourceTracker.GetInstanceUnSafe.GetTable();
            foreach (var tableRecordKV in trackerTable)
            {
                var tableRecord = tableRecordKV.Value;
                if (tableRecord.AssetTypeName == "Shader")
                {
                    var shaderName = $"{tableRecord.KEY}";
                    var shader = Resources.Load<Shader>(shaderName);
                    if (!ReferenceEquals(null, shader))
                    {
                        var shaderInnerName = shader.name;
                        if (!_ShaderInfoCollection.ContainsKey(shaderInnerName))
                        {
                            var spawnedShaderInfo = new ShaderInfoPreset(shaderInnerName, false);
                            
                            _ShaderInfoCollection.Add(shaderInnerName, spawnedShaderInfo);
                            _NonIncluded_ShaderInfoCollection.Add(shaderInnerName, spawnedShaderInfo);
                            _CurrentFocusedShaderInfoPresetList.Add(spawnedShaderInfo);
                        }
                    }
                }
            }
        }

        #endregion
        
        #region <EditorWindow>

        [MenuItem(MenuHeader + "2. ShaderCrawler")]
        private static async void Init()
        {
            await InitWindow(0.15f, 1.2f);
            await ResourceTracker.GetInstance();
        }

        protected override async void OnDrawEditor()
        {
            _DrawBlockFlag = true;
 
            // 외곽 스크롤뷰
            _ScrollPosition = GUILayout.BeginScrollView(_ScrollPosition, false, false,  GUILayout.Width(position.width),  GUILayout.Height(position.height));

            // 항목 타이틀
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(" * Scanned Shader List* ");
            EditorWindowTool.DrawHorizontalLine(Color.gray, 1f, -1f);
            
            var targetCrawlerTable = ShaderCrawler.GetInstanceUnSafe.GetTable();
            var IsReleaseMode = SystemFlag.IsSystemReleaseMode();
            
            // 모드 셀렉트 토글
            _ToggleFlag = EditorGUILayout.Toggle("Show non included only", _ToggleFlag);
            
            // 셰이더 목록
            var targetShaderInfoGroup = _ToggleFlag ? _NonIncluded_ShaderInfoCollection : _ShaderInfoCollection;
            var shaderNameGroup = targetShaderInfoGroup.Keys.ToArray();
            if (shaderNameGroup.Length > 0)
            {
                // 드롭다운 메뉴
                _DropSelected = EditorGUILayout.Popup("ShaderList", _DropSelected, shaderNameGroup);
                _CurrentSelectedShaderInfoPreset = targetShaderInfoGroup[shaderNameGroup[_DropSelected]];
                
                // 셰이더 타이틀
                EditorGUILayout.LabelField(" ");
                EditorGUILayout.LabelField($" * SelectedShader : [{_CurrentSelectedShaderInfoPreset.ThisShaderName}] * ");
                EditorGUILayout.Separator();
                
                // 셰이더 상태
                if (_CurrentSelectedShaderInfoPreset.IsRegisteredShaderFlag)
                {
                    EditorGUILayout.LabelField(" * 현재 Always Included Shader 리스트에 포함되어 있음.");
                }
                else
                {
                    EditorGUILayout.LabelField(" * 현재 Always Included Shader 리스트에 포함되어 있지 않음.");
                }
                
                // 포함하는 에셋 스크롤 뷰
                _ScrollPosition2 = GUILayout.BeginScrollView(_ScrollPosition2, false, false,  GUILayout.Width(position.width),  GUILayout.Height(position.height * 0.36f));
                foreach (var depencencies in _CurrentSelectedShaderInfoPreset.DefendencyGroup)
                {
                    EditorGUILayout.ObjectField(depencencies, typeof(Object), true);
                }
                GUILayout.EndScrollView();
                
                // 기능 타이틀
                EditorGUILayout.LabelField($"  ");
                EditorGUILayout.Separator();

                if (_ToggleFlag)
                {
                    if (EditorWindowTool.ConditionalButton("셰이더 목록에 일괄 추가", !_CurrentFocusedShaderInfoPresetList.Contains(_CurrentSelectedShaderInfoPreset) && !targetCrawlerTable.ContainsKey(_CurrentSelectedShaderInfoPreset.ThisShaderName)))
                    {
                        _CurrentFocusedShaderInfoPresetList.AddRange(targetShaderInfoGroup.Values);
                    }
                }

                if (SystemTool.TryGetEnumEnumerator<ShaderCrawlerHelperButtonType>(SystemTool.GetEnumeratorType.ExceptNone, out var o_Enumerator))
                {
                    foreach (var helperButtonType in o_Enumerator)
                    {
                        switch (helperButtonType)
                        {
                            case ShaderCrawlerHelperButtonType.AddTo:
                                if (EditorWindowTool.ConditionalButton("현재 선택된 셰이더 추가", !_CurrentFocusedShaderInfoPresetList.Contains(_CurrentSelectedShaderInfoPreset) && !targetCrawlerTable.ContainsKey(_CurrentSelectedShaderInfoPreset.ThisShaderName)))
                                {
                                    _CurrentFocusedShaderInfoPresetList.Add(_CurrentSelectedShaderInfoPreset);
                                }

                                break;
                            case ShaderCrawlerHelperButtonType.RemoveFrom:
                                if (EditorWindowTool.ConditionalButton("테이블에 추가될 셰이더 목록으로부터 제거", _CurrentFocusedShaderInfoPresetList.Contains(_CurrentSelectedShaderInfoPreset) && !targetCrawlerTable.ContainsKey(_CurrentSelectedShaderInfoPreset.ThisShaderName)))
                                {
                                    _CurrentFocusedShaderInfoPresetList.Remove(_CurrentSelectedShaderInfoPreset);
                                }
                                
                                break;
                        }
                    }
                }

                // 추가할 목록 타이틀
                EditorGUILayout.LabelField(" ");
                EditorGUILayout.LabelField($" * Current Focused Shader * ");
                
                // 스크롤 뷰 2
                _ScrollPosition3 = GUILayout.BeginScrollView(_ScrollPosition3, false, false,  GUILayout.Width(position.width),  GUILayout.Height(position.height * 0.36f));
                if (_CurrentFocusedShaderInfoPresetList.Count < 1)
                {
                    EditorGUILayout.LabelField("  - 목록없음.");
                }
                else
                {
                    foreach (var shaderInfoPreset in _CurrentFocusedShaderInfoPresetList)
                    {
                        EditorGUILayout.LabelField($"  - {shaderInfoPreset.ThisShaderName}");
                    }
                }
                GUILayout.EndScrollView();

                // 기능 타이틀 2
                EditorGUILayout.LabelField($"  ");
                EditorGUILayout.Separator();
                if (EditorWindowTool.ConditionalButton("테이블에 추가될 셰이더 목록 적용", _CurrentFocusedShaderInfoPresetList.Count > 0 && !IsReleaseMode))
                {
                    if (_CurrentFocusedShaderInfoPresetList.Count > 0)
                    {
                        foreach (var shaderInfoPreset in _CurrentFocusedShaderInfoPresetList)
                        {
                            var tryShaderName = shaderInfoPreset.ThisShaderName;
                            if (!targetCrawlerTable.ContainsKey(tryShaderName))
                            {
                                await ShaderCrawler.GetInstanceUnSafe.AddRecord(tryShaderName, false);
                            }
                        }
                        await ShaderCrawler.GetInstanceUnSafe.UpdateTableFile(ExportDataTool.WriteType.Overlap);
                        await SystemMaintenance.ApplyCurrentShaderCrawler();
                        UpdateFocusedRendererGroup();
                        UpdateRendererGroup();
                        goto SEG_PAINT_END;
                    }
                }

                // 배포 모드 예외문
                if (IsReleaseMode)
                {
                    EditorGUILayout.LabelField($"배포 모드에서는 셰이더 테이블을 업데이트 할 수 없습니다.");
                }
                
            }
            else
            {
                EditorGUILayout.LabelField($"포함되지 않은, 추가할 셰이더가 없습니다.");
            }

            GUILayout.EndScrollView();
            
            SEG_PAINT_END :
            _DrawBlockFlag = false;
        }

        #endregion

        #region <Classes>

        public class ShaderInfoPreset
        {
            public string ThisShaderName;
            public bool IsRegisteredShaderFlag;
            public List<Object> DefendencyGroup;

            public ShaderInfoPreset(string p_ShaderName, bool p_IsRegisteredRecord)
            {
                ThisShaderName = p_ShaderName;
                IsRegisteredShaderFlag = p_IsRegisteredRecord;
                DefendencyGroup = new List<Object>();
            }
        }

        #endregion
    }
}