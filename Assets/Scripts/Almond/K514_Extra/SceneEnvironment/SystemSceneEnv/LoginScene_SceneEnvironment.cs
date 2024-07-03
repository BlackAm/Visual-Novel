#if !SERVER_DRIVE
using Cysharp.Threading.Tasks;
using UnityEngine;
using UI2020;

namespace k514
{
    public class LoginScene_SceneEnvironment : GameSystemSceneEnvironmentBase
    {
        public override async UniTask OnScenePreload()
        {
            await LoginSceneEnvironmentResourceData.GetInstance();
        }

        public override void OnSceneStarted()
        {
            MenuUI.Instance.SetActive(true);
            MenuUI.Instance.startTitle.Initialize();
            MenuUI.Instance.ChangeScene(MenuUI.MenuUIList.StartTitle);
            
            //DefaultUIManagerSet.GetInstanceUnSafe._LoginUi.Set_UI_Hide(false);
            
            DefaultUIManagerSet.GetInstanceUnSafe._MainGameUi.Set_UI_Hide(true);
            LoginSceneEnvironmentResourceData.GetInstanceUnSafe.ResourcePreLoad();
        }

        public override void OnSceneTransition()
        {
            base.OnSceneTransition();
            
            MenuUI.Instance.SetActive(false);
        }
    }
}
#endif