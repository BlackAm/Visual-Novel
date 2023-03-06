using Cysharp.Threading.Tasks;
using UnityEngine;

namespace k514
{
    public class GamePlaySceneEnvironmentBase : SceneEnvironment
    {
        #region <Callbacks>

        public override async UniTask OnScenePreload()
        {
            await base.OnScenePreload();

#if SERVER_DRIVE
            var tryMaterial = new Material(Shader.Find("Diffuse"));
            var rendererSet = FindObjectsOfType<MeshRenderer>();
            foreach (var renderer in rendererSet)
            {
                renderer.sharedMaterial = tryMaterial;
            }

/*
            // 플레이어 스폰
            var (valid, player) = 
                await UnitSpawnManager.GetInstance
                    .SpawnUnitAsync<Unit>
                    (
                        1, Vector3.zero, ResourceLifeCycleType.Scene,
                        ObjectDeployTool.ObjectDeploySurfaceDeployType.ForceSurface | ObjectDeployTool.ObjectDeploySurfaceDeployType.UsingOriginYOffset,
                        UnitTool.UnitAuthorityFlag.Player
                    );

            if (valid)
            {
                // 랜덤 몬스터 스폰
                ObjectDeployLoader.GetInstance.CastDeployEventMap(100, player);
            }
*/

    #if UNITY_EDITOR
            if (CustomDebug.VisualizeServerNode)
            {
                var light = new GameObject("Display Light").AddComponent<Light>();
                light.type = LightType.Directional;
            }
    #endif
#else
            await UniTask.SwitchToMainThread();

            var tryUISet = DefaultUIManagerSet.GetInstanceUnSafe;
            if (!ReferenceEquals(null, tryUISet))
            {
                tryUISet._UITheaterDamage.PreloadUI(UICustomRoot.UIManagerType.TestNumberPanel);
                tryUISet._UITheaterDamage.PreloadUI(UICustomRoot.UIManagerType.TestNumberSymbolPanel);
                tryUISet._UITheaterName.PreloadUI(UICustomRoot.UIManagerType.TestNamePanel);
            }
#endif
        }

        #endregion
    }
}