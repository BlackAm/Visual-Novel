using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace k514
{
    /// <summary>
    /// 에디터 윈도우의 레이아웃이나 윈도우 속성 등의 기능을 구현한 클래스
    /// </summary>
    public static class EditorWindowTool
    {
        #region <Consts>

        /// <summary>
        /// 현재 활성화된 에디터 윈도우 갯수
        /// </summary>
        private static List<ICustomEditorWindow> _CurrentActiveCustomWindowGroup;

        /// <summary>
        /// 현재 에디터 윈도우 상태
        /// </summary>
        public static EditorWindowState CurrentEditorWindowState { get; private set; }

        static EditorWindowTool()
        {
            CurrentEditorWindowState = EditorWindowState.None;
            _CurrentActiveCustomWindowGroup = new List<ICustomEditorWindow>();
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
            AssemblyReloadEvents.beforeAssemblyReload += OnScriptsReloaded;
        }

        #endregion

        #region <Enums>

        public enum EditorWindowState
        {
            None,
            LoadingTable,
        }

        #endregion

        #region <Construcor>

        #endregion

        #region <Callbacks>

        /// <summary>
        /// 커스텀 윈도우가 생성되었을 때, 호출되는 콜백
        /// </summary>
        public static async UniTask OnWindowSpawn(ICustomEditorWindow p_TryWindow)
        {
            if (_CurrentActiveCustomWindowGroup.Count < 1)
            {
                _CurrentActiveCustomWindowGroup.Add(p_TryWindow);
                CurrentEditorWindowState = EditorWindowState.LoadingTable;
                await ResourceListData.GetInstance();
                
                await UniTask.SwitchToMainThread();
                SystemMaintenance.InitSystemMaintenance();
                await SystemBoot.LoadSystemInitializeBasisTable().WithCancellation(SystemMaintenance._SystemTaskCancellationToken);
                await SystemBoot.PreloadSystemTableAsync().WithCancellation(SystemMaintenance._SystemTaskCancellationToken);
                
                CurrentEditorWindowState = EditorWindowState.None;
                
                await UniTask.SwitchToMainThread();
                RepaintAllWindow();
            }
            else
            {
                switch (CurrentEditorWindowState)
                {
                    case EditorWindowState.None:
                        break;
                    case EditorWindowState.LoadingTable:
                        await UniTask.WaitUntil(() => CurrentEditorWindowState == EditorWindowState.None).WithCancellation(SystemMaintenance._SystemTaskCancellationToken);
                        break;
                }
                _CurrentActiveCustomWindowGroup.Add(p_TryWindow);
            }
        }

        /// <summary>
        /// 커스텀 윈도우가 닫혔을 때, 호출되는 콜백
        /// </summary>
        public static void OnWindowDestroy(ICustomEditorWindow p_TryWindow)
        {
            _CurrentActiveCustomWindowGroup.Remove(p_TryWindow);
            if (_CurrentActiveCustomWindowGroup.Count < 1)
            {
                SystemMaintenance.ReleaseSystem();
                SingletonTool.PrintActiveSingleton();
                CurrentEditorWindowState = EditorWindowState.None;
            }
        }
        /// <summary>
        /// 게임 플레이시, 모든 윈도우를 닫는다.
        /// </summary>
        private static void OnPlayModeChanged(PlayModeStateChange p_Type)
        {
            switch (p_Type)
            {
                case PlayModeStateChange.EnteredEditMode:
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    ClearAllWindow();
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
            }
        }

        private static void OnScriptsReloaded() 
        {
            ClearAllWindow();
        }

        #endregion
        
        #region <Methods>

        private static void RepaintAllWindow()
        {
            foreach (var editorWindow in _CurrentActiveCustomWindowGroup)
            {
                editorWindow.Repaint();
            }
        }

        private static void ClearAllWindow()
        {
            var windowCount = _CurrentActiveCustomWindowGroup.Count;
            for (int i = windowCount - 1; i > -1; i--)
            {
                _CurrentActiveCustomWindowGroup[i].Close();
            }
        }

        /// <summary>
        /// 지정한 에디터 윈도우를 화면 중앙으로 오도록 설정하는 메서드
        /// </summary>
        /// <param name="p_WindowInstance">생성된 윈도우 인스턴스</param>
        /// <param name="p_WindowScale">전체 디스플레이 대비 윈도우 스케일 비율</param>
        /// <param name="p_AspectRatioHeightScale">윈도우 가로세로비에 대해 높이값을 스케일 키우는 비율</param>
        public static void SetWindowAffine(EditorWindow p_WindowInstance, float p_WindowScale, float p_AspectRatioHeightScale)
        {
            var windowDisplayScale = 1f / EditorGUIUtility.pixelsPerPoint;
            var windowSizeRate = p_WindowScale;
            
            var displayScaledScreenResolution = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height) * windowDisplayScale;
            
            var x = (0.5f - windowSizeRate) * displayScaledScreenResolution.x;
            var y = (0.5f - windowSizeRate * p_AspectRatioHeightScale) * displayScaledScreenResolution.y;
            var w = (2.0f * windowSizeRate) * displayScaledScreenResolution.x;
            var h = (2.0f * windowSizeRate * p_AspectRatioHeightScale) * displayScaledScreenResolution.y;
            
            p_WindowInstance.position = new Rect(x, y, w, h);
        }

        /// <summary>
        /// 지정한 조건을 만족해야만 활성화되는 버튼 컴포넌트를 그리는 메서드
        /// </summary>
        public static bool ConditionalButton(string p_Label, bool p_ButtonStagingCondition)
        {
            GUI.enabled = p_ButtonStagingCondition;
            var buttonFlag = GUILayout.Button(p_Label);
            GUI.enabled = true;
            return buttonFlag;
        }

        /// <summary>
        /// 지정한 조건을 만족해야만 활성화되는 정수 슬라이더 바를 그리는 메서드
        /// </summary>
        public static void ConditionalSlider(int p_LeftValue, int p_RightValue, ref int r_TargetValue, bool p_ButtonStagingCondition)
        {
            GUI.enabled = p_ButtonStagingCondition;
            r_TargetValue = EditorGUILayout.IntSlider(r_TargetValue, p_LeftValue, p_RightValue);
            GUI.enabled = true;
        }
        
        /// <summary>
        /// 지정한 Thickness 만큼의 두께 및 Padding 만큼의 상하단 공백을 가지는 회색 라인을 그리는 메서드
        /// </summary>
        /// <param name="p_Thickness">두께</param>
        /// <param name="p_Padding">상하단 공백</param>
        public static void DrawHorizontalLine(float p_Thickness = 1f, float p_Padding = 1f)
        {
            DrawHorizontalLine(Color.gray, p_Thickness, p_Padding);
        }
        
        /// <summary>
        /// 지정한 색상과 Thickness 만큼의 두께 및 Padding 만큼의 상하단 공백을 가지는 회색 라인을 그리는 메서드
        /// </summary>
        /// <param name="p_Color">색깔</param>
        /// <param name="p_Thickness">두께</param>
        /// <param name="p_Padding">상하단 공백</param>
        public static void DrawHorizontalLine(Color p_Color, float p_Thickness = 1f, float p_Padding = 1f)
        {
            // 현재 레이아웃 컨트롤에서 p_Padding + p_Thickness 만큼의 높이를 가지는 GUI가 그려질 Rect 객체를 가져온다.
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(p_Padding + p_Thickness));
            r.height = p_Thickness;
            // padding 값의 절반만큼 y 좌표 증가, 즉 상하단에 각각 0.5 * padding 만큼의 보이지 않는 공백이 생긴다.
            r.y += p_Padding * 0.5f;
            // 기본적으로 레이아웃 컨트롤로 가져온 Rect는 좌우 padding을 가지게 되는데
            // 이는, 해당 라인이 윈도우에서 떨어져있게 보이게 만드므로 아래와 같이 Rect의 좌우 길이를 보정해준다.
            var lrPadding = 4f;
            r.x -= lrPadding * 0.5f;
            r.width += lrPadding * 1.5f;
            EditorGUI.DrawRect(r, p_Color);
        }

        /// <summary>
        /// 줄바꿈등이 적용되는 라벨필드를 그리는 메서드
        /// </summary>
        public static void LabelFieldWrapper(string p_Content)
        {
            GUIStyle textStyle = EditorStyles.label;
            textStyle.wordWrap = true;
            EditorGUILayout.LabelField(p_Content, textStyle);
        }
        
        #endregion
    }
}