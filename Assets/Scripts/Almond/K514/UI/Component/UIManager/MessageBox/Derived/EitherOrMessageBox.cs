#if !SERVER_DRIVE
using System;
using UnityEditor;
using UnityEngine.UI;

namespace k514
{
    public class EitherOrMessageBox : UI_MessageBoxBase
    {
        #region <Fields>

        private Text Title;
        private Text Contents;
        private Button Left;
        private Button Right;
        private Text LeftText;
        private Text RightText;
        private Action OnLeftClick;
        private Action OnRightClick;
        public UIMessageBoxController.MessageType viewType;

        public Action BoxCallBack;

        #endregion

        #region <Callbacks>

        public override void OnSpawning()
        {
            base.OnSpawning();

            Title = _Transform.FindRecursive<Text>("Title").Item2;
            Contents = _Transform.FindRecursive<Text>("Contents").Item2;
            Left = _Transform.FindRecursive<Button>("Left").Item2;
            LeftText = Left.GetComponentInChildren<Text>();
            Right = _Transform.FindRecursive<Button>("Right").Item2;
            RightText = Right.GetComponentInChildren<Text>();
            Left.SetButtonHandler(OnLeftButtonClicked);
            Right.SetButtonHandler(OnRightButtonClicked);
        }

        public override void OnRetrieved()
        {
            BoxCallBack?.Invoke();
            base.OnRetrieved();
            SetAllTextContentsClear();
        }

        private void OnLeftButtonClicked()
        {
            OnLeftClick?.Invoke();
            RetrieveObject();
        }

        private void OnRightButtonClicked()
        {
            OnRightClick?.Invoke();
            RetrieveObject();
        }

        #endregion

        #region <Methods>

        public void SetContentSet(int p_Index)
        {
            var contentSet = ((UILanguage.LanguageContent)LanguageManager.GetInstanceUnSafe[p_Index]).ContentSet;
            // 1일 경우 안내메세지만 있는 것.
            if (contentSet.Count.Equals(1))
            {
                SetTitle("안내");
                SetContents(contentSet[0]);
                SetButtonContents("확인", "취소");
            }
            else
            {
                SetTitle(contentSet[0]);
                SetContents(contentSet[1]);
                SetButtonContents(contentSet[2], contentSet[3]);
            }
        }

        public void SetTitle(string p_Text) => Title.text = p_Text;

        public void SetContents(string p_Text)
        {
            if (p_Text.Contains("\\n"))
            {
                p_Text = p_Text.Replace("\\n", "\n");
            }
            Contents.text = p_Text;
        }
        public void SetButtonContents(string p_Left, string p_Right)
        {
            LeftText.text = p_Left;
            RightText.text = p_Right;
        }

        public void SetHandler(Action p_OnLeftClick, Action p_OnRightClick, Action BoxCallBack)
        {
            OnLeftClick = p_OnLeftClick;
            OnRightClick = p_OnRightClick;
            this.BoxCallBack = BoxCallBack;
        }

        #endregion
    }
}
#endif