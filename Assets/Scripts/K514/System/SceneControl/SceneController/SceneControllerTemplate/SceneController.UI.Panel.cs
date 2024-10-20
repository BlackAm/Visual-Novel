#if !SERVER_DRIVE
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

namespace BlackAm
{
    public partial class SceneController<M, K, T>
    {
        #region <Consts>

        private const float _DefaultSceneLoadingFadeDuration = 1f;
        private const string _FaderPanelName = "LoadingPanel";
        private const string _MainLabelName = "PhaseText";

        #endregion

        #region <Fields>

        private SliderPreset _ProgressBar;
        protected UI_FadePanel _Fader;
        private Text _Label;
        private bool _LockProgress;

        #endregion

        #region <Callbacks>

        private void OnCreatePanel()
        {
            var canvasScaler = GetComponent<CanvasScaler>();
            if (!ReferenceEquals(null, canvasScaler))
            {
                canvasScaler.InitCanvasScaler();
            }

            _ProgressBar = transform.SetSliderPreset();
            var (faderValid, faderAffine) = transform.FindRecursive(_FaderPanelName);
            if (faderValid)
            {
                _Fader = faderAffine.gameObject.AddComponent<UI_FadePanel>();
                _Fader.CheckAwake();
                _Fader.SetFadeTime(_DefaultSceneLoadingFadeDuration);
                _Fader.OnEntryAnimationBegin();
            }
            var (labelValid, labelAffine) = transform.FindRecursive<Text>(_MainLabelName);
            if (labelValid)
            {
                _Label = labelAffine;
            }
        }

        private void OnUpdatePanel(float p_DeltaTime)
        {
            if (!ReferenceEquals(null, _Fader))
            {
                _Fader.OnUpdateUI(p_DeltaTime);
            }
        }

        #endregion

        #region <Methods>

        protected void SetProgress(float p_Rate, bool p_LockProgress = false)
        {
            if(_LockProgress && !p_LockProgress)
            {
                return;
            }

            if (_ProgressBar.IsValid)
            {
                var iRate = (int) (p_Rate * 100);
                _ProgressBar.SetProgress(p_Rate);
                _ProgressBar.SetText($"{iRate}%");
            }
        }
        protected void SetLockProgress(bool p_Value)
        {
            _LockProgress = p_Value;
        }
        protected async void SetLabelText(SystemLanguage.SystemLanguageType p_Type)
        {
            if (!ReferenceEquals(null, _Label))
            {
                var content = SystemLanguage.GetInstanceUnSafe[p_Type].content;
                await UniTask.SwitchToMainThread();
                _Label.text = content;
            }
        }
        
        protected async void SetLabelText(string p_Text)
        {
            if (!ReferenceEquals(null, _Label))
            {
                await UniTask.SwitchToMainThread();
                _Label.text = p_Text;
            }
        }

        #endregion
    }
}
#endif
