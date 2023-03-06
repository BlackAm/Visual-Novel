#if UNITY_EDITOR && ON_GUI
using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace k514
{
    public partial class TestManager : SceneChangeEventUnityAsyncSingleton<TestManager>
    {
        #region <Consts>

        /// <summary>
        /// GUI 스케일
        /// </summary>
        private static float _GUI_SCALE;

        /// <summary>
        /// GUI 스케일 프로퍼티
        /// </summary>
        private static float GUI_SCALE
        {
            get => _GUI_SCALE;
            set
            {
                _GUI_SCALE = value * SystemMaintenance.INV_DEFAULT_SCREEN_WIDTH;
            }
        }

        /// <summary>
        /// gui 폰트사이즈
        /// </summary>
        private static int FontSize => (int)(36 * GUI_SCALE);

        /// <summary>
        /// gui 행에 포함될 버튼 갯수
        /// </summary>
        private const int RowSize = 6;

        /// <summary>
        /// gui 행에 포함될 버튼 너비
        /// </summary>
        private static int BoxSizeWidth => (int)(360 * GUI_SCALE);
        
        /// <summary>
        /// gui 행에 포함될 버튼 높이
        /// </summary>
        private static int BoxSizeHeight => (int)(88 * GUI_SCALE);

        /// <summary>
        /// 박스간 간격
        /// </summary>
        private static int BoxIntervalSize => (int)(6 * GUI_SCALE);
        
        /// <summary>
        /// 페이지 제어 버튼 상자 크기
        /// </summary>
        private static Rect ToggleValidationRect
        {
            get
            {
                var boxIntervalSize = BoxIntervalSize;
                var boxSizeHeight = 1.5f * BoxSizeHeight;
                return new Rect(boxIntervalSize, boxIntervalSize, boxSizeHeight, boxSizeHeight);
            }
        }

        /// <summary>
        /// 버튼 설명 상자 크기
        /// </summary>
        private static Rect ToolTipRect
        {
            get
            {
                var boxIntervalSize = BoxIntervalSize;
                return new Rect(boxIntervalSize + ToggleValidationRect.xMax, boxIntervalSize, BoxSizeWidth * 2f, BoxSizeHeight * 0.75f - 2 * GUI_SCALE);
            }
        }

        /// <summary>
        /// 현재 선택된 유닛 상자 크기
        /// </summary>
        private static Rect FocusUnitRect
        {
            get
            {
                var boxIntervalSize = BoxIntervalSize;
                var boxSizeHeight = 0.75f * BoxSizeHeight;
                return new Rect(boxIntervalSize + ToggleValidationRect.xMax, boxIntervalSize + boxSizeHeight + 1 * GUI_SCALE, BoxSizeWidth * 2f, boxSizeHeight - 2 * GUI_SCALE);
            }
        }

        /// <summary>
        /// 페이지 제어 버튼 상자 크기
        /// </summary>
        private static Rect LeftBracketRect
        {
            get
            {
                var boxIntervalSize = BoxIntervalSize;
                var boxSizeHeight = 1.5f * BoxSizeHeight;
                return new Rect(boxIntervalSize + ToolTipRect.xMax, boxIntervalSize, boxSizeHeight, boxSizeHeight);
            }
        }

        /// <summary>
        /// 페이지 제어 버튼 상자 크기2
        /// </summary>
        private static Rect RightBracketRect
        {
            get
            {
                var boxIntervalSize = BoxIntervalSize;
                var boxSizeHeight = 1.5f * BoxSizeHeight;
                return new Rect(boxIntervalSize + LeftBracketRect.xMax, boxIntervalSize, boxSizeHeight, boxSizeHeight);
            }
        }

        #endregion
        
        #region <Fields>

        /// <summary>
        /// 해당 매니저 아핀 오브젝트
        /// </summary>
        private Transform _Transform;
        
        /// <summary>
        /// [테스트 타입, 커맨드 타입, 커맨드 설명 프리셋]
        /// </summary>
        private Dictionary<TestControlType, Dictionary<ControllerTool.CommandType, TestGUIPreset>> CommandProxyCollection;

        /// <summary>
        /// 현재 테스트 매니저 테스트 타입
        /// </summary>
        private TestControlType _CurrentTestControlType;

        /// <summary>
        /// 입력 이벤트 수신자
        /// </summary>
        private ControllerEventReceiver _ControllerEventReceiver;

        /// <summary>
        /// 테스트 GUI 유효성 키
        /// </summary>
        private bool _IsUIValid;

        /// <summary>
        /// 터치 이벤트, 클릭 이벤트 리시버
        /// </summary>
        private TouchEventReceiver _TouchEventReceiver;

        /// <summary>
        /// 현재 선택된 유닛
        /// </summary>
        private Unit _SelectedUnit;

        /// <summary>
        /// 현재 지정된 위치
        /// </summary>
        private Vector3 _FocusedPosition;
        
        /// <summary>
        /// 유닛 이벤트 수신자
        /// </summary>
        private UnitEventReceiver _UnitEventReceiver;

        #endregion

        #region <Enums>

        public enum TestControlType
        {
            ObjectControlTest,

            ProjectorTest,
            AssetLoadTest,
            TableTest,
            CameraTest,
            CoroutineTest,
            DebugPanel,
            PoolingPrefabTest,
            UITest,
            EnvironmentTest,
            ItemTest,
            SceneControlTest,
            ShaderTest,
            QuickTest,
            NetworkTest,
            Count,
        }

        #endregion
        
        #region <Callbacks>

        protected override async UniTask OnCreated()
        {
            _Transform = transform;
            _Transform.parent = SystemBoot.GetInstance._Transform;
            
            CommandProxyCollection = new Dictionary<TestControlType, Dictionary<ControllerTool.CommandType, TestGUIPreset>>();
            _CurrentTestControlExtraGUICollection = new Dictionary<TestControlType, TestGUIExtraInputPreset>();
            if (SystemTool.TryGetEnumEnumerator<TestControlType>(SystemTool.GetEnumeratorType.GetAll, out var o_Enumerator))
            {
                foreach (var testControlType in o_Enumerator)
                {
                    CommandProxyCollection.Add(testControlType, new Dictionary<ControllerTool.CommandType, TestGUIPreset>());
                    _CurrentTestControlExtraGUICollection.Add(testControlType, new TestGUIExtraInputPreset());
                }
            }
            
            _ControllerEventReceiver =
                ControllerTool.GetInstanceUnSafe
                    .GetControllerEventSender(ControllerTool.InputEventType.TestModule)
                    ?.GetEventReceiver<ControllerEventReceiver>(ControllerTool.InputEventType.TestModule, OnInputEventTriggered);

            _TouchEventReceiver =
                TouchEventManager.GetInstance.GetEventReceiver<TouchEventReceiver>(
                    TouchEventRoot.TouchEventType.UnitSelected | TouchEventRoot.TouchEventType.PositionSelected, OnTouchEventTriggered);
            
            _UnitEventReceiver = new UnitEventReceiver(UnitEventHandlerTool.UnitEventType.UnitDead, OnUnitDeadEventTriggered);
            
#if !SERVER_DRIVE
            OnAwakeObjectControl();
#endif         
            OnAwakeProjector();
            OnAwakeAssetLoad();
            OnAwakeTable();
            OnAwakeCameraTest();
            OnAwakeCoroutine();
#if !SERVER_DRIVE
            OnAwakeDebugPanel();
#endif         
            OnAwakePrefabPooling();
            OnAwakeSceneTest();
            OnAwakeShaderTest();
            OnAwakeCutin();
            OnAwakeEnvironment();
#if !SERVER_DRIVE
            OnAwakeItem();
#endif         
            OnAwakeQuick();
            OnAwakeNetwork();
            
            ToggleTestManagerEnable();
            OnPageSwitched();
            
            await UniTask.CompletedTask;
        }

        public override async UniTask OnInitiate()
        {
            await UniTask.CompletedTask;
        }

        private void OnGUI()
        {
            if (SystemBoot.GetInstance.IsSystemOpen())
            {
                GUI_SCALE = Screen.width;
                if (_IsUIValid)
                {
                    DrawToggleValidationGUI();
                    DrawTestGUI();
                }
                else
                {
                    DrawToggleValidationGUI();
                }
            }
        }

        private void DrawTestGUI()
        {
            var currentCount = 0;
            GUI.skin.label.fontSize = GUI.skin.box.fontSize = GUI.skin.textArea.fontSize =
                GUI.skin.button.fontSize = GUI.skin.textField.fontSize = FontSize;
            GUI.skin.label.alignment = GUI.skin.box.alignment = GUI.skin.textArea.alignment 
                = GUI.skin.button.alignment = GUI.skin.textField.alignment = TextAnchor.MiddleCenter;
            
            GUI.Box(ToolTipRect, "");
            GUI.Box(FocusUnitRect, _SelectedUnit.IsValid() ? _SelectedUnit.GetUnitName() : "Non Seleted");
            if (GUI.Button(LeftBracketRect, new GUIContent("<<", "이전 명령 셋으로 페이즈를 넘김.")))
            {
                _CurrentTestControlType--;
                if (_CurrentTestControlType < 0)
                {
                    _CurrentTestControlType = TestControlType.Count - 1;
                    OnPageSwitched();
                }
            }

            if (GUI.Button(RightBracketRect, new GUIContent(">>", "다음 명령 셋으로 페이즈를 넘김.")))
            {
                _CurrentTestControlType++;
                if (_CurrentTestControlType >= TestControlType.Count)
                {
                    _CurrentTestControlType = 0;
                    OnPageSwitched();
                }
            }

            DrawExtraInputGUI();

            var currentPresetCollection = CommandProxyCollection[_CurrentTestControlType];
            foreach (var testGuiPreset in currentPresetCollection)
            {
                var targetPreset = testGuiPreset.Value;
                var row = currentCount / RowSize;
                var col = currentCount % RowSize;
                var currentPos = 
                    new Rect(
                        (BoxSizeWidth + BoxIntervalSize) * row + BoxIntervalSize, 
                        (BoxSizeHeight + BoxIntervalSize) * col + BoxIntervalSize + FocusUnitRect.yMax,
                        BoxSizeWidth, 
                        BoxSizeHeight
                    );
                    
                // 지정한 위치에 버튼 설명 및 툴팁 문자열을 등록한다.
                GUI.Box(currentPos, new GUIContent($"{targetPreset.TargetCommandType} \n {targetPreset.Title}", targetPreset.Description));
                // 마우스가 지정한 박스 위에 있는 경우, 지정했던 툴팁을 출력해준다.
                GUI.Label(ToolTipRect, GUI.tooltip);
                // 툴팁 문자열은 스택 형식이기 때문에, 초기화 시켜준다.
                GUI.tooltip = null;
                    
                currentCount++;
            }
        }

        private void DrawToggleValidationGUI()
        {
            if (GUI.Button(ToggleValidationRect, new GUIContent("Toggle", "테스트 기능 On/Off")))
            {
                ToggleTestManagerEnable();
            }
        }

        #endregion

        #region <PropertyModifying>

        private void OnInputEventTriggered(ControllerTool.InputEventType p_Type, ControllerTool.ControlEventPreset p_Preset)
        {
            if (_IsUIValid)
            {
                var functionType = p_Preset.CommandType;
                var currentPresetCollection = CommandProxyCollection[_CurrentTestControlType];
                if (currentPresetCollection.TryGetValue(functionType, out var bindedTestEventPreset))
                {
                    bindedTestEventPreset.TestEvent(p_Preset, p_Preset.WorldUV);
                }
            }
        }

        private void OnTouchEventTriggered(TouchEventRoot.TouchEventType p_Type, TouchEventManager.TouchEventPreset p_Preset)
        {
            switch (p_Type)
            {
                case TouchEventRoot.TouchEventType.None:
                    break;
                case TouchEventRoot.TouchEventType.PlayerSelected:
                    break;
                case TouchEventRoot.TouchEventType.UnitSelected:
                    var targetUnit = p_Preset.SelectedUnit;
                    if (targetUnit.IsValid())
                    {
                        // 유닛 재선택시 이벤트 해제
                        if (ReferenceEquals(_SelectedUnit, targetUnit))
                        {
                            _SelectedUnit = null;
                            _UnitEventReceiver.ClearSenderGroup();
                        }
                        else
                        {
                            _SelectedUnit = targetUnit;
                            _SelectedUnit.AddEventReceiver(_UnitEventReceiver);
                        }
                    }
                    break;
                case TouchEventRoot.TouchEventType.PositionSelected:
                    _FocusedPosition = p_Preset.WorldVector;
                    break;
            }
        }

        private void OnUnitDeadEventTriggered(UnitEventHandlerTool.UnitEventType p_Type, UnitEventMessage p_Preset)
        {
            p_Preset.TriggerUnitHandler.RemoveReceiver(_UnitEventReceiver);
            _SelectedUnit = null;
        }

        #endregion

        #region <Methods>

        public bool IsSelected(Unit p_Target)
        {
            return ReferenceEquals(_SelectedUnit, p_Target);
        }

        private void BindKeyTestEvent(TestControlType p_ControlType, ControllerTool.CommandType p_KeyType, Action<ControllerTool.ControlEventPreset, Vector3> p_TestEvent, string p_Title)
        {
            BindKeyTestEvent(p_ControlType, p_KeyType, p_TestEvent, p_Title, p_Title);
        }
        
        private void BindKeyTestEvent(TestControlType p_ControlType, ControllerTool.CommandType p_KeyType, Action<ControllerTool.ControlEventPreset, Vector3> p_TestEvent, string p_Title, string p_Description)
        {
            CommandProxyCollection[p_ControlType].Add(p_KeyType, new TestGUIPreset{ TargetCommandType = p_KeyType, TestEvent = p_TestEvent, Title = p_Title, Description = p_Description});
        }

        private void ToggleTestManagerEnable()
        {
            _IsUIValid = !_IsUIValid;
        }

        #endregion

        #region <Structs>

        public struct TestGUIPreset
        {
            public Action<ControllerTool.ControlEventPreset, Vector3> TestEvent;
            public ControllerTool.CommandType TargetCommandType;
            public string Title;
            public string Description;
        }

        #endregion

        #region <SceneChangeObserving>

        public override async UniTask OnScenePreload()
        {
            await UniTask.CompletedTask;
        }

        public override void OnSceneStarted()
        {
            gameObject.SetActiveSafe(true);
        }

        public override void OnSceneTerminated()
        {
            gameObject.SetActiveSafe(false);
        }

        public override void OnSceneTransition()
        {
        }
        
        #endregion
    }
}
#endif