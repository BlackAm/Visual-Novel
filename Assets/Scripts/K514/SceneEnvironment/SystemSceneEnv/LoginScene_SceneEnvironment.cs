#if !SERVER_DRIVE
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BlackAm
{
    public class LoginScene_SceneEnvironment : GameSystemSceneEnvironmentBase
    {
        public override async UniTask OnScenePreload()
        {
            await LoginSceneEnvironmentResourceData.GetInstance();
        }

        public override void OnSceneStarted()
        {
            TitleMenuUI.Instance.SetActive(true);
            TitleMenuUI.Instance.startTitle.Initialize();
            TitleMenuUI.Instance.ChangeScene(TitleMenuUI.MenuUIList.StartTitle);
            
            //DefaultUIManagerSet.GetInstanceUnSafe._LoginUi.Set_UI_Hide(false);
            
            DefaultUIManagerSet.GetInstanceUnSafe._MainGameUi.Set_UI_Hide(true);
            DefaultUIManagerSet.GetInstanceUnSafe._TopMenu.Set_UI_Hide(true);
            LoginSceneEnvironmentResourceData.GetInstanceUnSafe.ResourcePreLoad();
            
            SaveLoadManager.GetInstanceUnSafe.OnReadDialogueCreated();
            SaveLoadManager.GetInstanceUnSafe.OnGallerySaveDataCreated();
        }

        public override void OnSceneTransition()
        {
            base.OnSceneTransition();
            
            TitleMenuUI.Instance.SetActive(false);
        }
    }
}
#endif