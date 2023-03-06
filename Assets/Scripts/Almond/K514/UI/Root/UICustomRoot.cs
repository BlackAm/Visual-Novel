
using UnityEngine.UI;
#if !SERVER_DRIVE
using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace k514
{
    public class UICustomRoot : SceneChangeEventUnityAsyncSingleton<UICustomRoot>
    {
        #region <Fields>

        [NonSerialized] public RectTransform _Transform;
        private Dictionary<RenderMode, (RectTransform, Dictionary<int, UIManagerPreset>)> _CanvasPresetGroup;
        private RenderMode[] _RenderMode_Enumerator;
        private UIManagerType[] _ManagerType_Enumerator;
        public Camera UICamera { get; private set; }
        private bool _HideFlag;
        
        #endregion

        #region <Enum>

        [Flags]
        public enum UIManagerType
        {
            GameLogin = 1 << 2,
            MainGame = 1 << 3,
            
            Theater = 1 << 5,
            Fader = 1 << 9,

            TestController = 1 << 10,
            TestLoading = 1 << 11,
            TestTouchPanel = 1 << 12,
            TestScreenWrapper = 1 << 13,
            MessageBoxController = 1 << 14,
            
            TestImagePanel = 1 << 16,
            TestTextPanel = 1 << 17,
            TestNumberPanel = 1 << 18,
            TestNumberSymbolPanel = 1 << 19,
            TestNamePanel = 1 << 20,
        }

        #endregion
        
        #region <Callbacks>

        protected override async UniTask OnCreated()
        {
            DontDestroyOnLoad(this);
            gameObject.SetActiveSafe(false);
            await UIManagerPrefabTable.GetInstance();
            await TouchEventRoot.GetInstance();
            await UniTask.SwitchToMainThread();
            _Transform = GetComponent<RectTransform>();
            _CanvasPresetGroup = new Dictionary<RenderMode, (RectTransform, Dictionary<int, UIManagerPreset>)>();
            _ManagerType_Enumerator = SystemTool.GetEnumEnumerator<UIManagerType>(SystemTool.GetEnumeratorType.ExceptMaskNone);
            _RenderMode_Enumerator = SystemTool.GetEnumEnumerator<RenderMode>(SystemTool.GetEnumeratorType.GetAll);
            
            UICamera = _Transform.GetDefaultCamera(CameraClearFlags.Depth, Color.black);
            UICamera.name = "UI_ScreenSpaceCanvasCamera";
            UICamera.cullingMask = GameManager.UI_LayerMask;
            UICamera.orthographic = true;
            UICamera.depth = 1;

            foreach (var renderMode in _RenderMode_Enumerator)
            {
                var renderModeWrapper = new GameObject($"{renderMode}");
                var renderModeWrapperRectTransform = renderModeWrapper.AddComponent<RectTransform>();
                renderModeWrapperRectTransform.SetPivotPreset
                (
                    UITool.XAnchorType.Stretch, UITool.YAnchorType.Stretch,
                    Vector2.zero, Vector2.zero
                );
                renderModeWrapper.transform.SetParent(_Transform, false);
                _CanvasPresetGroup.Add(renderMode, (renderModeWrapperRectTransform, new Dictionary<int, UIManagerPreset>()));

                var indexableTable = UIDataRoot.GetInstanceUnSafe[renderMode];
                var targetCollection = indexableTable.GetValidKeyEnumerator();
                if (!ReferenceEquals(null, targetCollection))
                {
                    foreach (var key in targetCollection)
                    {
                        var ordinalKey = indexableTable.Convert_To_OrdinalKey(key);
                        var managerMaskRecord = indexableTable.GetTableData(key);
                        var managerMask = managerMaskRecord.ManagerTypeMask;
                        
                        foreach (var uiManagerType in _ManagerType_Enumerator)
                        {
                            if ((managerMask & uiManagerType) == uiManagerType)
                            {
                                await Add_UI_Manager(renderMode, ordinalKey, uiManagerType);
                            }
                        }
                    }
                }
            }
            
            Broadcast_HideUI(true);
            gameObject.SetActiveSafe(true);
        }

        public override async UniTask OnInitiate()
        {
            await UniTask.CompletedTask;
        }
        
        public override async UniTask OnScenePreload()
        {
            foreach (var renderMode in _RenderMode_Enumerator)
            {
                var canvasCollection = _CanvasPresetGroup[renderMode].Item2;
                foreach (var canvasKV in canvasCollection)
                {
                    var canvasManagerCollection = canvasKV.Value.UIManagerBaseCollection;
                    foreach (var uiManagerBaseKV in canvasManagerCollection)
                    {
                        var uiManagerBase = uiManagerBaseKV.Value;
                        await uiManagerBase.OnScenePreload();
                    }
                }
            }
        }

        public override void OnSceneStarted()
        {
            foreach (var renderMode in _RenderMode_Enumerator)
            {
                var canvasCollection = _CanvasPresetGroup[renderMode].Item2;
                foreach (var canvasKV in canvasCollection)
                {
                    var canvasManagerCollection = canvasKV.Value.UIManagerBaseCollection;
                    foreach (var uiManagerBaseKV in canvasManagerCollection)
                    {
                        var uiManagerBase = uiManagerBaseKV.Value;
                        uiManagerBase.OnSceneStarted();
                    }
                }
            }
        }

        public override void OnSceneTerminated()
        {
            foreach (var renderMode in _RenderMode_Enumerator)
            {
                var canvasCollection = _CanvasPresetGroup[renderMode].Item2;
                foreach (var canvasKV in canvasCollection)
                {
                    var canvasManagerCollection = canvasKV.Value.UIManagerBaseCollection;
                    foreach (var uiManagerBaseKV in canvasManagerCollection)
                    {
                        var uiManagerBase = uiManagerBaseKV.Value;
                        uiManagerBase.OnSceneTerminated();
                    }
                }
            } 
        }

        public override void OnSceneTransition()
        {
            foreach (var renderMode in _RenderMode_Enumerator)
            {
                var canvasCollection = _CanvasPresetGroup[renderMode].Item2;
                foreach (var canvasKV in canvasCollection)
                {
                    var canvasManagerCollection = canvasKV.Value.UIManagerBaseCollection;
                    foreach (var uiManagerBaseKV in canvasManagerCollection)
                    {
                        var uiManagerBase = uiManagerBaseKV.Value;
                        uiManagerBase.OnSceneTransition();
                    }
                }
            }
        }

        public void OnUpdate(float p_DeltaTime)
        {
            foreach (var renderMode in _RenderMode_Enumerator)
            {
                var canvasCollection = _CanvasPresetGroup[renderMode].Item2;
                foreach (var canvasKV in canvasCollection)
                {
                    var canvasManagerCollection = canvasKV.Value.UIManagerBaseCollection;
                    foreach (var uiManagerBaseKV in canvasManagerCollection)
                    {
                        var uiManagerBase = uiManagerBaseKV.Value;
                        if (!uiManagerBase._HideFlag)
                        {
                            uiManagerBase.OnUpdateUI(p_DeltaTime);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 인스턴스가 파기될 때 수행할 작업을 기술한다.
        /// </summary>
        protected override void DisposeUnManaged()
        {
            if (_CanvasPresetGroup != null)
            {
                foreach (var canvasPresetArrayKV in _CanvasPresetGroup)
                {
                    var canvasCollection = canvasPresetArrayKV.Value.Item2;
                    foreach (var canvasKV in canvasCollection)
                    {
                        var targetCollection = canvasKV.Value.UIManagerBaseCollection;
                        foreach (var uiManagerBase in targetCollection)
                        {
                            uiManagerBase.Value.Dispose();
                        }
                    }
                }
                _CanvasPresetGroup = null;
            }

            base.DisposeUnManaged();
        }

        #endregion

        #region <Methods>

        /// <summary>
        /// 지정한 랜더모드/레이어 번호를 가지는 캔버스에 특정 UIManager를 등록시키는 메서드
        /// 랜더모드의 경우에는 3종류 밖에 없어서 각 모드에 대응하는 래퍼 오브젝트가 미리 생성되어 있지만
        /// 레이어 번호에 대응하는 캔버스는 그렇지 않기에, 파라미터로 요구하는 레이어 캔버스가 없다면
        /// 해당 메서드에 의해 생성된다.
        ///
        /// 만약, 지정한 캔버스에 이미 동일한 타입의 UIManager가 있다면 새로 생성하지 않고
        /// 기존에 있던 UIManager를 UIManagerPreset으로 감싸 리턴한다.
        /// 
        /// </summary>
        public async UniTask<UIBaseManagerPreset> Add_UI_Manager(RenderMode p_RenderMode, int p_SortingLayer, UIManagerType p_ManagerType)
        {
            var renderModeTuple = _CanvasPresetGroup[p_RenderMode];
            var renderModeWrapper = renderModeTuple.Item1;
            var renderModeCollection = renderModeTuple.Item2;
            if (!renderModeCollection.TryGetValue(p_SortingLayer, out var targetCanvasPreset))
            {
                var spawnedCanvas = renderModeWrapper.CreateDefaultCanvas();
                spawnedCanvas.name = $"Canvas [Layer.{p_SortingLayer}]";
                spawnedCanvas.renderMode = p_RenderMode;

                switch (p_RenderMode)
                {
                    case RenderMode.ScreenSpaceCamera:
                        spawnedCanvas.worldCamera = UICamera;
                        spawnedCanvas.sortingOrder = p_SortingLayer;
                        spawnedCanvas.sortingLayerName = ((GameManager.GameSortingLayerType)p_SortingLayer).ToString();
                        break;
                    case RenderMode.ScreenSpaceOverlay:
                        spawnedCanvas.sortingOrder = p_SortingLayer;
                        break;
                    case RenderMode.WorldSpace:
                        spawnedCanvas.worldCamera = CameraManager.GetInstanceUnSafe.MainCamera;
                        spawnedCanvas.sortingOrder = p_SortingLayer;
                        spawnedCanvas.gameObject.TurnLayerTo(GameManager.GameLayerType.WorldUI, false);
                        break;
                }

                targetCanvasPreset = new UIManagerPreset(spawnedCanvas, p_RenderMode, p_SortingLayer);
                renderModeCollection.Add(p_SortingLayer, targetCanvasPreset);
            }

            var targetCanvas = targetCanvasPreset._Canvas;
            var targetCollection = targetCanvasPreset.UIManagerBaseCollection;

            // 이미 해당 캔버스에 동일한 UIManager가 등록된 경우, 동일한 오브젝트를 리턴한다.
            if (targetCollection.TryGetValue(p_ManagerType, out var o_UIManagerBase))
            {
                return new UIBaseManagerPreset(o_UIManagerBase, p_ManagerType, targetCanvasPreset);
            }
            else
            {
                var uiManagerPreset = UIManagerPrefabTable.GetInstanceUnSafe.GetTableData(p_ManagerType);
                var prefabName = uiManagerPreset.ManagerPrefabName;
                var component = uiManagerPreset.ManagerComponent;
                var canvasManager = 
                (
                    await PrefabPoolingManager.GetInstance
                    .PoolInstanceAsync<UIManagerBase>
                    (
                        prefabName,
                        ResourceLifeCycleType.Free_Condition, 
                        ResourceType.GameObjectPrefab, (p_RenderMode, targetCanvas, component)
                    )
                )
                .Item1;
                if (!ReferenceEquals(null, canvasManager))
                {
                    targetCollection.Add(p_ManagerType, canvasManager);
                    return new UIBaseManagerPreset(canvasManager, p_ManagerType, targetCanvasPreset);
                }
                else
                {
                    return default;
                }
            }
        }

        #endregion

        #region <Method/FindUI>

        /// <summary>
        /// 지정한 랜더모드와 레이어를 가지는 캔버스로부터 특정 UIManager 타입에 대응하는 UIManager를 리턴하는 메서드
        /// </summary>
        public async UniTask<(UIBaseManagerPreset, T)> Get_UI_Manager<T>(RenderMode p_RenderMode, int p_SortingLayer, UIManagerType p_ManagerType) where T : UIManagerBase
        {
            await UniTask.SwitchToMainThread();
            
            var targetCanvasPresetArray = _CanvasPresetGroup[p_RenderMode].Item2;
            var targetCanvasPresetArrayLength = targetCanvasPresetArray.Count;
            if (targetCanvasPresetArrayLength > 0)
            {
                if (targetCanvasPresetArray.TryGetValue(p_SortingLayer, out var targetPreset))
                {
                    var targetCollection = targetPreset.UIManagerBaseCollection;
                    var _ResultUIBaseManager = targetCollection[p_ManagerType];
                    return (new UIBaseManagerPreset(_ResultUIBaseManager, p_ManagerType, targetPreset), _ResultUIBaseManager as T);
                }
            }
            
            return default;
        }

        /// <summary>
        /// 지정한 랜더모드와 레이어를 가지는 캔버스로부터 특정 UIManager 타입에 대응하는 UIManager를 리턴하는 메서드
        /// 대응하는 UIManager가 없다면 추가한다.
        /// </summary>
        public async UniTask<(UIBaseManagerPreset, T)> Get_UI_Manager_Fallback<T>(RenderMode p_RenderMode, int p_SortingLayer, UIManagerType p_ManagerType) where T : UIManagerBase
        {
            var tryFind = await Get_UI_Manager<T>(p_RenderMode, p_SortingLayer, p_ManagerType);
            if (tryFind.Item1.IsValid)
            {
                return tryFind;
            }
            else
            {
                var tryAdd = await Add_UI_Manager(p_RenderMode, p_SortingLayer, p_ManagerType);
                return (tryAdd, tryAdd.UIManagerBase as T);
            }
        }
        
        /// <summary>
        /// 캔버스 정보가 없는 상태에서 특정 UIManager 타입을 가지고 UIManager를 찾아야하는 경우
        /// 모든 캔버스를 검색하여 조건에 맞는 최초의 UIManager를 리턴하는 메서드
        /// </summary>
        public async UniTask<(UIBaseManagerPreset, T)> Get_UI_Manager_AutoSearch<T>(UIManagerType p_ManagerType) where T : UIManagerBase
        {
            await UniTask.SwitchToMainThread();
            
            foreach (var renderMode in _RenderMode_Enumerator)
            {
                var targetCanvasPresetArray = _CanvasPresetGroup[renderMode].Item2;
                foreach (var canvasPresetKV in targetCanvasPresetArray)
                {
                    var targetCanvasPreset = canvasPresetKV.Value;
                    var canvasManagerCollection = targetCanvasPreset.UIManagerBaseCollection;
                    if (canvasManagerCollection.ContainsKey(p_ManagerType))
                    {
                        var _ResultUIBaseManager = canvasManagerCollection[p_ManagerType];
                        return (new UIBaseManagerPreset(_ResultUIBaseManager, p_ManagerType, targetCanvasPreset), _ResultUIBaseManager as T);
                    }
                }
            }

            return default;
        }
        
        /// <summary>
        /// Get_UI_Manager 숏컷
        /// </summary>
        public async UniTask<UIBaseManagerPreset> Get_UI_Manager(RenderMode p_RenderMode, int p_SortingLayer, UIManagerType p_ManagerType)
        {
            return (await Get_UI_Manager<UIManagerBase>(p_RenderMode, p_SortingLayer, p_ManagerType)).Item1;
        }
        
        /// <summary>
        /// Get_UI_Manager_AutoSearch 숏컷
        /// </summary>
        public async UniTask<UIBaseManagerPreset> Get_UI_Manager_AutoSearch(UIManagerType p_ManagerType)
        {
            return (await Get_UI_Manager_AutoSearch<UIManagerBase>(p_ManagerType)).Item1;
        }

        #endregion

        #region <Method/BroadCast>

        public void Set_UI_Hide(bool p_HideFlag)
        {
            if (_HideFlag != p_HideFlag)
            {
                gameObject.SetActiveSafe(_HideFlag);
                _HideFlag = p_HideFlag;
            }
        }

        public void Broadcast_HideUI(bool p_HideFlag)
        {
            foreach (var renderModeKV in _CanvasPresetGroup)
            {
                var canvasPreset = renderModeKV.Value.Item2;
                foreach (var canvasPresetKV in canvasPreset)
                {
                    var _UIManagerCollection = canvasPresetKV.Value.UIManagerBaseCollection;
                    foreach (var uiManagerBaseKV in _UIManagerCollection)
                    {
                        var targetUIManagerBase = uiManagerBaseKV.Value;
                        targetUIManagerBase.Set_UI_Hide(p_HideFlag);
                    }
                }
            }
        }

        #endregion
    }

    #region <Classes>

    /// <summary>
    /// 어떤 Canvas 컴포넌트에 대한 참조 정보 및 랜더 모드, 정렬을 위한 캔버스 레이어 정보를 가지며
    /// 해당 Canvas에 포함된 UIManager를 컬렉션으로 기술하는 클래스
    /// </summary>
    public class UIManagerPreset
    {
        #region <Fields>

        public Canvas _Canvas { get; private set; }
        public RectTransform _CanvasTransform;
        public RenderMode _RenderMode { get; private set; }
        public int _LayerIndex { get; private set; }
        public Dictionary<UICustomRoot.UIManagerType, UIManagerBase> UIManagerBaseCollection { get; private set; }

        #endregion

        #region <Indexer>

        public UIManagerBase this[UICustomRoot.UIManagerType p_UIManagerType]
        {
            get => UIManagerBaseCollection[p_UIManagerType]; 
            set => UIManagerBaseCollection[p_UIManagerType] = value; 
        }
        
        #endregion        
        
        #region <Constructors>

        public UIManagerPreset(Canvas p_Canvas, RenderMode p_RenderMode, int p_LayerIndex)
        {
            _Canvas = p_Canvas;
            _CanvasTransform = _Canvas.GetComponent<RectTransform>();
            _RenderMode = p_RenderMode;
            _LayerIndex = p_LayerIndex;
            UIManagerBaseCollection = new Dictionary<UICustomRoot.UIManagerType, UIManagerBase>();
        }

        #endregion
    }

    #endregion

    #region <Structs>

    /// <summary>
    /// 어떤 캔버스에 포함된 UIManager를 구분하는 용도로 사용하는 프리셋
    /// 내부에는 캔버스와 UIManager타입 및 UIManager 오브젝트 참조 정보가 기술되어 있으며
    /// 캔버스로부터 해당 UIManager를 릴리스시키는 기능을 제공한다.
    /// </summary>
    public struct UIBaseManagerPreset
    {
        #region <Fields>

        private UIManagerPreset _MasterNode;
        
        /// <summary>
        /// 같은 종류의 UIManager 컴포넌트라도 용도에 따라 그 타입을 달라질 수 있다.
        /// FadePanel 같은 경우 스크립트는 완전히 같지만 페이드 연출, 크리티컬 연출 등
        /// 타입은 다르다.
        /// </summary>
        public UICustomRoot.UIManagerType UIManagerType;
        public UIManagerBase UIManagerBase;
        public bool IsValid;
        public int SortingLayer;
        
        #endregion

        #region <Constructors>

        public UIBaseManagerPreset(UIManagerBase p_UIManagerBase, UICustomRoot.UIManagerType p_UIManagerType,
            UIManagerPreset uiManagerPreset)
        {
            UIManagerBase = p_UIManagerBase;
            UIManagerType = p_UIManagerType;
            _MasterNode = uiManagerPreset;
            SortingLayer = uiManagerPreset._Canvas.sortingOrder;
            IsValid = true;
        }

        #endregion

        #region <Methods>

        /// <summary>
        /// 해당 UIManagerBase를 관리하는 캔버스 프리셋으로부터 해당 UIManagerBase 연결을 끊는다.
        /// </summary>
        public void Diconnect()
        {
            _MasterNode[UIManagerType] = null;
            _MasterNode = null;
            IsValid = false;
            UIManagerBase.RetrieveObject();
        }

        #endregion
    }

    #endregion
}
#endif