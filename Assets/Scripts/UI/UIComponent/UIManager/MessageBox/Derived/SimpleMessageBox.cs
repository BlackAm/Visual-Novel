#if !SERVER_DRIVE
using System;
using UnityEditor;
using UnityEngine.UI;

namespace BlackAm
{
    public class SimpleMessageBox : UI_MessageBoxBase
    {
        #region <Fields>

        private Text Title;
        private Text Contents;
        private Button button;
        private Text buttonText;
        private Action OnClick;
        public UIMessageBoxController.MessageType viewType;

        public Action BoxCallBack;

        #endregion

        #region <Callbacks>

        public override void OnSpawning()
        {
            base.OnSpawning();

            Title = _Transform.FindRecursive<Text>("Title").Item2;
            Contents = _Transform.FindRecursive<Text>("Contents").Item2;
            button = _Transform.FindRecursive<Button>("Button").Item2;
            buttonText = button.GetComponentInChildren<Text>();
            button.SetButtonHandler(OnButtonClicked);
        }

        public override void OnRetrieved()
        {
            BoxCallBack?.Invoke();
            base.OnRetrieved();
            SetAllTextContentsClear();
        }

        private void OnButtonClicked()
        {
            OnClick?.Invoke();
            // 행동서술.

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
                SetButtonContents("확인");
            }
            else
            {
                SetTitle(contentSet[0]);
                SetContents(contentSet[1]);
                SetButtonContents(contentSet[2]);
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

        public void SetButtonContents(string text)
        {
            buttonText.text = text;
        }

        public void SetHandler(Action p_OnClick, Action BoxCallBack)
        {
            OnClick = p_OnClick;
            this.BoxCallBack = BoxCallBack;
        }

        #endregion
    }
}
#endif