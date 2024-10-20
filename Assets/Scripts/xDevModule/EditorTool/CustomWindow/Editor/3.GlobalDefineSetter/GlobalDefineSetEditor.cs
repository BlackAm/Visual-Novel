using UnityEditor;
using UnityEngine;

namespace BlackAm
{
    public class GlobalDefineSetEditor : CustomEditorWindowBase<GlobalDefineSetEditor>
    {
        #region <Consts>

        private static readonly GlobalDefineTable.GlobalDefineType[] DefaultGlobalDefineType =
            {GlobalDefineTable.GlobalDefineType.ON_GUI};

        #endregion
        
        #region <Fields>

        private GlobalDefineHelperState _CurrentState;
        private GlobalDefineTable.GlobalDefineType _CurrentSelectedPredefineType;
        private string _NewerDefineName;
        private bool _ClearConfirm;
        private Vector2 _scrollVector;
        
        #endregion
        
        #region <Enums>
        
        private enum GlobalDefineHelperState
        {
            None,
            NameOverlapped,
            Success,
            EmptyString,
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

        private void InitGlobalDefineSet()
        {
            GlobalDefineSetter.GetInstance.InitGlobalDefineSet();
            _CurrentState = GlobalDefineHelperState.None;
            _CurrentSelectedPredefineType = GlobalDefineTable.GlobalDefineType.None;
            _NewerDefineName = string.Empty;
            _ClearConfirm = false;
            Repaint();
        }

        private void SelectPredefineProcessor(GlobalDefineTable.GlobalDefineType p_GlobalDefineType)
        {
             _CurrentSelectedPredefineType = p_GlobalDefineType;
        }

        private void TryAddDefine(string p_TargetDefineName)
        {
            if (p_TargetDefineName == string.Empty)
            {
                _CurrentState = GlobalDefineHelperState.EmptyString;
            }

            var targetDefineSet = GlobalDefineSetter.GetInstance._CurrentGlobalDefineSet;
            if (targetDefineSet.Contains(p_TargetDefineName))
            {
                _CurrentState = GlobalDefineHelperState.NameOverlapped;
                GlobalDefineSetter.GetInstance.RemoveDefine(p_TargetDefineName);
            }

            if (_CurrentState == GlobalDefineHelperState.None)
            {
                _CurrentState = GlobalDefineHelperState.Success;
                GlobalDefineSetter.GetInstance.AddDefine(p_TargetDefineName);
            }
        }

        #endregion

        #region <EditorWindow>

        [MenuItem(MenuHeader + "3. GlobalDefineSetter")]
        private static async void Init()
        {
            await InitWindow(0.15f, 1f);
        }

        protected override void OnDrawEditor()
        {
            _DrawBlockFlag = true;

            _scrollVector = EditorGUILayout.BeginScrollView (_scrollVector, false, false);
            
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(" * 현재 선언된 전처리자 * ");
            EditorWindowTool.DrawHorizontalLine(Color.gray, 1f, -1f);

            EditorGUILayout.BeginVertical();

            var targetDefineSet = GlobalDefineSetter.GetInstance._CurrentGlobalDefineSet;
            foreach (var define in targetDefineSet)
            {
                EditorGUILayout.LabelField(define);
            }
            
            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("추가할 전처리자 이름 : ");
            _NewerDefineName = EditorGUILayout.TextField(string.Empty, _NewerDefineName);
            if (GUILayout.Button("입력"))
            {
                TryAddDefine(_NewerDefineName);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();

            if (_ClearConfirm)
            {
                if (GUILayout.Button("전부 지우기 확인"))
                {
                    _ClearConfirm = false;
                    GlobalDefineSetter.GetInstance.ClearDefine();
                }
                if (GUILayout.Button("전부 지우기 취소"))
                {
                    _ClearConfirm = false;
                }
            }
            else
            {
                if (GUILayout.Button("전부 지우기"))
                {
                    _ClearConfirm = true;
                }
            }
            
            if (GUILayout.Button("초기화"))
            {
                InitGlobalDefineSet();
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(" ");
            EditorGUILayout.LabelField(" * 특수 기능 전처리기 목록 * ");
            EditorGUILayout.Separator();
            if (SystemTool.TryGetEnumEnumerator<GlobalDefineTable.GlobalDefineType>(SystemTool.GetEnumeratorType.ExceptNone, out var o_Enumerator))
            {
                foreach (var globalDefineType in o_Enumerator)
                {
                    var targetPredefineProcessorName = GlobalDefineTable.GetInstanceUnSafe.GetTable()[globalDefineType].KEY;
                    EditorGUILayout.BeginHorizontal();
                    
                    if (GUILayout.Button($"{targetPredefineProcessorName} 선택"))
                    {
                        SelectPredefineProcessor(globalDefineType);
                    }
                    
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.Separator();

            switch (_CurrentSelectedPredefineType)
            {
                case GlobalDefineTable.GlobalDefineType.None:
                    break;
                default:
                    
                    var targetPredefineProcessorNamePair = GlobalDefineTable.GetInstanceUnSafe.GetTable()[_CurrentSelectedPredefineType]; 
                    GUILayout.TextArea(targetPredefineProcessorNamePair.Description);
                    EditorGUILayout.Separator();
                    
                    EditorGUILayout.BeginHorizontal();
                    var targetPredefineProcessorName = targetPredefineProcessorNamePair.KEY.ToString();
                    if (GUILayout.Button($"{targetPredefineProcessorName} {(targetDefineSet.Contains(targetPredefineProcessorName) ? "삭제" : "추가")}"))
                    {
                        TryAddDefine(targetPredefineProcessorName);
                    }
                    EditorGUILayout.EndHorizontal();
                    
                    break;
            }
            
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(" ");
            EditorGUILayout.LabelField(" * 현재 상태 * ");
            EditorGUILayout.Separator();
            switch (_CurrentState)
            {
                case GlobalDefineHelperState.None:
                    GUILayout.TextArea("추가할 전처리 정의문을 입력해주세요. 혹은, 특수 기능 전처리기를 선택해주세요.");
                    break;
                case GlobalDefineHelperState.NameOverlapped:
                case GlobalDefineHelperState.Success:
                    InitGlobalDefineSet();
                    break;
                case GlobalDefineHelperState.EmptyString:
                    GUILayout.TextArea("텅빈 문자열입니다.");
                    break;
            }

            EditorGUILayout.EndScrollView(); 
            
            SEG_PAINT_END :
            _DrawBlockFlag = false;
        }
        
        #endregion
    }
}