using Cysharp.Threading.Tasks;
using UI2020;
using UnityEngine;

namespace k514
{
    /// <summary>
    /// 라미에르 기본 씬
    /// </summary>
    public class LamierePlayerDeploySceneEnvironment : GamePlaySceneEnvironmentBase
    {
        #region <Callbacks>

        /// <summary>
        /// 로딩 과정에서, 전이할 씬에서 해당 객체를 찾았을 때 호출되는 전처리 콜백
        /// </summary>
        public override async UniTask OnScenePreload()
        {
            await base.OnScenePreload();
            
#if SERVER_DRIVE
            await (await ServerPopPortalData.GetInstance()).ReloadTable($"PortalPlacementData_{HeadlessServerManager.GetInstance.CurrentSceneEntryIndex}");
            Debug.Log($"k514, [Log] Server Node : ZoneToDemonEvent Requested.({HeadlessServerManager.GetInstance.CurrentSceneEntryIndex}/{HeadlessServerManager.GetInstance.CurrentChannelIndex})");
            NetworkPacketManager.ZoneToDemonConnectInfo(ServerPopPortalData.GetInstanceUnSafe.GetPortalList(HeadlessServerManager.GetInstance.CurrentSceneEntryIndex));
#else
            // LamiereGameManager.GetInstanceUnSafe.DeployPlayer();
#endif
        }

        /// <summary>
        /// 로딩 과정이 끝난 경우 호출되는 콜백
        /// </summary>
        public override async void OnSceneStarted()
        {
            base.OnSceneStarted();

            var (valid, currentSceneEntryIndex) = SceneControllerManager.GetInstance.CurrentSceneControlPreset.TryGetSceneEntryIndex();
            if (valid)
            {
#if SERVER_DRIVE
                await UniTask.WaitUntil(() => HeadlessServerManager.GetInstance.DamonConnectedFlag);
                await PopUpManager.GetInstance.CreatePopObject(currentSceneEntryIndex);
#else
#endif
            }

#if SERVER_DRIVE
            
#else
            LamiereGameManager.GetInstanceUnSafe.HidePlayer(false);

            DefaultUIManagerSet.GetInstanceUnSafe._MainLoadingBar.Set_UI_Hide(true);
            DefaultUIManagerSet.GetInstanceUnSafe._TouchPanel.Set_UI_Hide(false);
            DefaultUIManagerSet.GetInstanceUnSafe._MainGameUi.Set_UI_Hide(false);
#endif
        }

        /// <summary>
        /// 씬이 종료되어 로딩씬으로 전이될 때 호출되는 콜백
        /// </summary>
        public override void OnSceneTransition()
        {
            base.OnSceneTransition();

#if !SERVER_DRIVE
            LamiereGameManager.GetInstanceUnSafe.HidePlayer(true);
#endif
        }

        #endregion
    }
}