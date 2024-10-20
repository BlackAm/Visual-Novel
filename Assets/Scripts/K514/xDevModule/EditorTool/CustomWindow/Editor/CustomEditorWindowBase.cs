using System;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace BlackAm
{
    public interface ICustomEditorWindow
    {
        void Repaint();
        void Close();
    }

    public abstract class CustomEditorWindowBase<T> : EditorWindow, ICustomEditorWindow where T : CustomEditorWindowBase<T>
    {
        #region <Consts>

        /// <summary>
        /// 현재 윈도우 인스턴스
        ///
        /// 동시에 하나 밖에 존재할 수 없으므로, 일종의 싱글톤이다.
        /// </summary>
        private static T CurrentWindow;
        
        /// <summary>
        /// 유니티 에디터 메뉴 헤더
        /// </summary>
        protected const string MenuHeader = "CustomEditor/";

        /// <summary>
        /// 해당 커스텀 윈도우 클래스를 상속받는 타입의 윈도우를 생성하고 초기화시키는 정적 메서드
        /// </summary>
        protected static async UniTask<bool> InitWindow(float p_WindowScale = 0.15f, float p_AspectRatio = 1f)
        {
            // 플레이 모드에서는 에디터가 동작하지 않음
            if (Application.isPlaying)
            {
                return false;
            }
            else
            {
                if (ReferenceEquals(null, CurrentWindow))
                {
                    CurrentWindow = GetWindow<T>(true, typeof(T).Name, true);
                    EditorWindowTool.SetWindowAffine(CurrentWindow, p_WindowScale, p_AspectRatio);
                    await EditorWindowTool.OnWindowSpawn(CurrentWindow);
                    CurrentWindow.OnCreated();
                    CurrentWindow.OnInitiate();
                }

                return true;
            }
        }

        #endregion

        #region <Fields>

        /// <summary>
        /// GUI 그리기 핸들
        /// 에디터 작업 중에 비동기 작업이 포함되어 있는 경우, 그 작업을 기다리는 용도로 사용한다.
        /// </summary>
        protected bool _DrawBlockFlag;

        #endregion

        #region <Callbacks>

        /// <summary>
        /// 인스턴스가 생성됬던 경우, 처리할 작업을 기술하는 콜백
        /// </summary>
        protected abstract void OnCreated();

        /// <summary>
        /// 인스턴스가 임의의 타이밍에 상태를 초기화해야하는 경우, 처리할 작업을 기술하는 콜백
        /// </summary>
        protected abstract void OnInitiate();

        public void OnGUI()
        {
            switch (EditorWindowTool.CurrentEditorWindowState)
            {
                case var state when state == EditorWindowTool.EditorWindowState.None && Application.isPlaying:
                    EditorWindowTool.LabelFieldWrapper("플레이 모드에서는 동작하지 않습니다.");
                    break;
                case var state when state == EditorWindowTool.EditorWindowState.None && _DrawBlockFlag:
                    EditorWindowTool.LabelFieldWrapper("지정한 작업을 처리중입니다.");
                    break;
                case EditorWindowTool.EditorWindowState.None:
                    OnDrawEditor();
                    break;
                case EditorWindowTool.EditorWindowState.LoadingTable:
                    EditorWindowTool.LabelFieldWrapper("에디터 기능 초기화 중입니다.");
                    break;
            }
        }

        protected abstract void OnDrawEditor();

        /// <summary>
        /// 에디터로부터 싱글톤을 제거한다.
        /// </summary>
        private void OnDestroy()
        {
            EditorWindowTool.OnWindowDestroy(CurrentWindow);
            CurrentWindow = null;
        }
        
        #endregion
    }
}