#if !SERVER_DRIVE

namespace k514
{
    public abstract class UIPoolerBase : UIManagerClusterBase
    {
        #region <Const>

        private const int PreloadPanel = 64;

        #endregion
        
        #region <Fields>

        #endregion
        
        #region <Methods>

        public void PreloadUI(UICustomRoot.UIManagerType p_ManagerType)
        {
            return;
            
            var uiManagerPreset = UIManagerPrefabTable.GetInstanceUnSafe.GetTableData(p_ManagerType);
            var prefabName = uiManagerPreset.ManagerPrefabName;
            var component = uiManagerPreset.ManagerComponent;
            
            var (_, preloadList) = PrefabPoolingManager.GetInstance.PreloadInstance(prefabName, ResourceLifeCycleType.Scene,
                ResourceType.GameObjectPrefab, PreloadPanel, component);

            foreach (var prefabInstance in preloadList)
            {
                prefabInstance._Transform.SetParent(_Transform, false);
            }
        }
        
        public UIManagerBase PoolUI(UICustomRoot.UIManagerType p_ManagerType)
        {
            var uiManagerPreset = UIManagerPrefabTable.GetInstanceUnSafe.GetTableData(p_ManagerType);
            var prefabName = uiManagerPreset.ManagerPrefabName;
            var component = uiManagerPreset.ManagerComponent;
            var spawned = PrefabPoolingManager.GetInstance
                .PoolInstance<UIManagerBase>(
                    prefabName,
                    ResourceLifeCycleType.Scene,
                    ResourceType.GameObjectPrefab, component).Item1;
            
            spawned._Transform.SetParent(_Transform, false);
            AddSlaveNode(spawned);
            return spawned;
        }
        
        public T PoolUI<T>(UICustomRoot.UIManagerType p_ManagerType) where T : UIManagerBase
        {
            var uiManagerPreset = UIManagerPrefabTable.GetInstanceUnSafe.GetTableData(p_ManagerType);
            var prefabName = uiManagerPreset.ManagerPrefabName;
            
            var spanwed = PrefabPoolingManager.GetInstance
                .PoolInstance<T>(
                    prefabName,
                    ResourceLifeCycleType.Scene,
                    ResourceType.GameObjectPrefab, typeof(T)).Item1;
            
            spanwed._Transform.SetParent(_Transform, false);
            spanwed.gameObject.SetActive(false);
            AddSlaveNode(spanwed);
            return spanwed;
        }
        
        #endregion
    }
}
#endif