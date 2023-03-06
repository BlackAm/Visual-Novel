#if !SERVER_DRIVE
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UI2020;

namespace k514
{
    public class DefaultUIManagerSet : AsyncSingleton<DefaultUIManagerSet>
    {
        #region <Fields>
 
        public UI_FadePanel _MainFader;
//        public TestController _MainController;
        public TestTouchPanel _TouchPanel;
        public TestScreenWrapper _TouchScreen;
        public UI_FadePanel _MainLoadingBar;
        public UITheater _UITheaterDamage;
        public UITheater _UITheaterName;
        public UITheater _UIMakerImage;
        public UIMessageBoxController _UiMessageBoxController;
        
        public MainGameUI _MainGameUi;        

       private Dictionary<(UICustomRoot.UIManagerType, int), UIBaseManagerPreset> _DefaultUIPresetCollection;
         
        #endregion
         
        #region <Callbacks>

        protected override async UniTask OnCreated()
        {
#if !SERVER_DRIVE
            _DefaultUIPresetCollection = new Dictionary<(UICustomRoot.UIManagerType, int), UIBaseManagerPreset>();
            
            _MainFader = await FindDefaultUIBase<UI_FadePanel>(UICustomRoot.UIManagerType.Fader);
//            _MainController = await FindDefaultUIBase<TestController>(UICustomRoot.UIManagerType.TestController);
            _TouchPanel = await FindDefaultUIBase<TestTouchPanel>(UICustomRoot.UIManagerType.TestTouchPanel);
            _TouchScreen = await FindDefaultUIBase<TestScreenWrapper>(UICustomRoot.UIManagerType.TestScreenWrapper);
            _MainLoadingBar = await FindDefaultUIBase<UI_FadePanel>(UICustomRoot.UIManagerType.TestLoading);
            _UITheaterDamage = await FindDefaultUIBase<UITheater>(UICustomRoot.UIManagerType.Theater, 6);
            _UITheaterName = await FindDefaultUIBase<UITheater>(UICustomRoot.UIManagerType.Theater, 7);
            _UiMessageBoxController = await FindDefaultUIBase<UIMessageBoxController>(UICustomRoot.UIManagerType.MessageBoxController);
    
            _MainGameUi = await FindDefaultUIBase<MainGameUI>(UICustomRoot.UIManagerType.MainGame);
#endif
        }
 
        public override async UniTask OnInitiate()
        {
            await UniTask.CompletedTask;
        }
 
        #endregion
 
        #region <Methods>

        private async UniTask<T> FindDefaultUIBase<T>(UICustomRoot.UIManagerType p_TargetType) where T : UIManagerBase
        {
            var result = await UICustomRoot.GetInstanceUnSafe.Get_UI_Manager_AutoSearch<T>(p_TargetType);
            _DefaultUIPresetCollection.Add((p_TargetType, result.Item1.SortingLayer), result.Item1);
            return result.Item2;
        }
        
        private async UniTask<T> FindDefaultUIBase<T>(UICustomRoot.UIManagerType p_TargetType, int p_SortingLayer) where T : UIManagerBase
        {
            var result = await UICustomRoot.GetInstanceUnSafe.Get_UI_Manager<T>(RenderMode.ScreenSpaceCamera, p_SortingLayer, p_TargetType);
            _DefaultUIPresetCollection.Add((p_TargetType, result.Item1.SortingLayer), result.Item1);
            return result.Item2;
        }

        public void SetFadeInUI(float p_PreDelay = 0f)
        {
            _MainFader.CastEntryAnimation(null, p_PreDelay);
        }
         
        public void SetFadeOutUI(float p_PreDelay = 0f)
        {
            _MainFader.CastEscapeAnimation(null, p_PreDelay);
        }
        
        public void RetrieveUIManager(UICustomRoot.UIManagerType p_TargetType, int p_SortingLayer)
        {
            var tryKey = (p_TargetType, p_SortingLayer);
            if (_DefaultUIPresetCollection.ContainsKey(tryKey))
            {
                _DefaultUIPresetCollection[tryKey].Diconnect();
            }
        }

        public void HideUI(bool p_HideFlag)
        {
            _MainGameUi.Set_UI_Hide(p_HideFlag);
            _UITheaterName.Set_UI_Hide(p_HideFlag);
            _TouchPanel.Set_UI_Hide(p_HideFlag);;
        }
        
        #endregion
    }
}
#endif