#if !SERVER_DRIVE
using BDG;
using Cysharp.Threading.Tasks;
using UI2020;
using UnityEngine;

namespace k514
{
    public interface IIndexableUIDeployTableBridge : ITableBase
    {
    }

    public interface IIndexableUIDeployTableRecordBridge : ITableBaseRecord
    {
        UICustomRoot.UIManagerType ManagerTypeMask { get; }
    }

    public class UIDataRoot : MultiTableProxy<UIDataRoot, int, RenderMode, IIndexableUIDeployTableBridge, IIndexableUIDeployTableRecordBridge>
    {
        #region <Fields>

        public EventTimer UI_EventTimer { get; private set; }
        public UICustomRoot _UI_Root;
        
        #endregion
        
        /// <summary>
        /// 에셋 로드 매니저로부터 프리팹을 메모리에 올리는 기능을 수행한다.
        /// </summary>
        protected override async UniTask OnCreated()
        {
            await base.OnCreated();

            UI_EventTimer = SystemBoot.GameEventTimer;

            // UI Root 초기화에 선행되어야하는 싱글톤을 먼저 로드해준다.
            await SingletonTool.CreateAsyncSingleton(typeof(AnimationSpriteData));

            await SingletonTool.CreateAsyncSingleton(typeof(ImageNameTableData));

            // UI Root 초기화
            await UniTask.SwitchToMainThread();
            {
                var uiRootPrefabs =
                    (
                        await LoadAssetManager.GetInstanceUnSafe
                        .LoadAssetAsync<GameObject>(ResourceType.GameObjectPrefab, ResourceLifeCycleType.WholeGame, "MainUIRoot.prefab")
                        .WithCancellation(SystemMaintenance._SystemTaskCancellationToken)
                    ).Item2;

                var instantiatedPrefab = GameObject.Instantiate(uiRootPrefabs);

                _UI_Root = instantiatedPrefab.AddComponent<UICustomRoot>();

                await UICustomRoot.GetInstance().WithCancellation(SystemMaintenance._SystemTaskCancellationToken);
            }
        }

        public override async UniTask OnInitiate()
        {
            await UniTask.CompletedTask;
        }

        protected override MultiTableIndexer<int, RenderMode, IIndexableUIDeployTableRecordBridge> SpawnGameDataTableCluster()
        {
            return new IntegerMultiTableIndexer<RenderMode, IIndexableUIDeployTableRecordBridge>();
        }
    }
}
#endif