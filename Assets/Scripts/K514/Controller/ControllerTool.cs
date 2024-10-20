using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BlackAm
{
    /// <summary>
    /// 컨트롤 이벤트를 입력 디바이스에 따라 분배하는 기능을 담당하는 매니저 클래스
    /// </summary>
    public partial class ControllerTool : SceneChangeEventAsyncSingleton<ControllerTool>
    {
        #region <Consts>

        /// <summary>
        /// 유니티 엔진에 정의된 윗 방향키 키코드값
        /// </summary>
        public const int UPPER_ARROW_KEYCODE_INDEX = (int)KeyCode.UpArrow;
        
        /// <summary>
        /// 유니티 엔진에 정의된 왼쪽 방향키 키코드값
        /// </summary>
        public const int LEFT_ARROW_KEYCODE_INDEX = (int)KeyCode.LeftArrow;
        
        /// <summary>
        /// 유니티 엔진에 정의된 아래 방향키 키코드값
        /// </summary>
        public const int LOWER_ARROW_KEYCODE_INDEX = (int)KeyCode.DownArrow;
        
        /// <summary>
        /// 유니티 엔진에 정의된 오른쪽 방향키 키코드값
        /// </summary>
        public const int RIGHT_ARROW_KEYCODE_INDEX = (int)KeyCode.RightArrow;

        #endregion
        
        #region <Fields>

        /// <summary>
        /// 해당 컨트롤러 시스템에서 사용할 키코드 모음
        /// </summary>
        public KeyCode[] KeyCodeSet { get; private set; }

        /// <summary>
        /// 입력 이벤트별 이벤트 프리셋 참조변수 래퍼 컬렉션
        /// </summary>
        [NonSerialized] public Dictionary<InputEventType, InputEventPreset> _InputEventPresetCollection;

        /// <summary>
        /// 입력 이벤트별 커맨드 시스템 컬렉션
        /// </summary>
        [NonSerialized] public Dictionary<InputEventType, CommandSystem> _CommandSystemCollection;
        
        /// <summary>
        /// 입력 이벤트별 입력 이벤트 전파 객체 컬렉션
        /// </summary>
        [NonSerialized] public Dictionary<InputEventType, ControllerEventSender> _InputEventSenderCollection;

        /// <summary>
        /// 키코드의 최대 정수 값
        /// </summary>
        public int KeyCodeScale;
        
        #endregion
        
        #region <Enums>

        /// <summary>
        /// 입력 플랫폼 타입
        ///
        /// 앞에 온 타입일 수록, 동시 입력시 우선도가 높다.
        /// </summary>
        [Flags]
        public enum InputControllerType
        {
            /// <summary>
            /// 기본 부정 상수
            /// </summary>
            None = 0,
            
            /// <summary>
            /// UI 터치 혹은 마우스 입력
            /// </summary>
            TouchPanel = 1 << 0, 
            
            /// <summary>
            /// 키보드 입력
            /// </summary>
            Keyboard = 1 << 1, 
            
            /// <summary>
            /// 키보드 입력이랑 UI 및 마우스 입력을 모두 받는 멀티 타입
            /// </summary>
            Keyboard_TouchPanel = TouchPanel | Keyboard,
        }

        /// <summary>
        /// 입력 이벤트 타입
        /// </summary>
        [Flags]
        public enum InputEventType
        {
            /// <summary>
            /// 기본 부정 상수
            /// </summary>
            None = 0,
            
            /// <summary>
            /// 기본 입력 타입
            /// </summary>
            ControlUnit = 1 << 0,
            
            /// <summary>
            /// 카메라 조작
            /// </summary>
            ControlView = 1 << 1,
                  
            /// <summary>
            /// 시스템 컨트롤
            /// </summary>
            SystemControl = 1 << 2,
            
            /// <summary>
            /// 월드맵 컨트롤
            /// </summary>
            WorldMapControl = 1 << 3,
            
            /// <summary>
            /// 대화창 컨트롤
            /// </summary>
            DialogueControl = 1 << 4,

            /// <summary>
            /// 테스트용 입출력
            /// </summary>
            TestModule = 1 << 10,
        }

        /// <summary>
        /// 입력 이벤트 타입 반복자용 배열
        /// </summary>
        public InputEventType[] InputEventType_Enumerator { get; private set; }

        /// <summary>
        /// 입력 커맨드 타입
        /// </summary>
        public enum CommandType
        {
            None = 0,
            
            /* 상태 커맨드 */
            // 해당 타입들은 입력 이벤트의 상태 자체를 나타낸다.
            Move,
            
            /* 조합(연속) 커맨드 */
            // 해당 타입들은 어떤 순서로 커맨드가 조합되었는지를 나타낸다.
            MoveUp, MoveLeft, MoveDown, MoveRight, 
            
            Z, X, C, V, Space, Escape, Enter,
            
            A, S, D, F, G, H,
            Q, W, E, R, T,
            
            L_Ctrl, L_Shift,
            R_Shift,
            
            N1, N2, N3, N4, N5, N6,
            
            B,
            
            L_Click, R_Click,
            
            ZZ,
            
            SpaceZ, SpaceX, SpaceC, SpaceSpace,
            
            LeftDownRightZ,
            DownRightZ,
            UpUpDownX,
            
            ZX, XC, CV, ZC, XV,
            
            /* 홀딩 커맨드 */
            // 해당 타입들은 어떤 커맨드가 눌린 상태에서 추가 입력이 발생했는지를 나타낸다.
            HLeftZ, HLeftX, HLeftC, HLeftV, HLeftA,
        }

        /// <summary>
        /// 입력에 의한 컨트롤러 이벤트 타입
        /// </summary>
        public enum ControllerEventType
        {
            /// <summary>
            /// 단일 키 입력이 발생한 경우
            /// </summary>
            MonoCommand,
            
            /// <summary>
            /// 화살표에 의한 키 입력이 발생한 경우
            /// </summary>
            ArrowCommand, 

            /// <summary>
            /// 연속으로 입력된 커맨드가 발생한 경우
            /// </summary>
            SequenceCommand,
            
            /// <summary>
            /// 홀딩 커맨드가 발생한 경우
            /// </summary>
            HoldingCommand,
        }

        /// <summary>
        /// 현재 입력에 대한 상태
        /// </summary>
        public enum ControllerInputStateType
        {
            /// <summary>
            /// 커맨드가 아닌 단일키 이벤트가 발생한 경우
            /// </summary>
            PressKey,
            
            /// <summary>
            /// 특정 키를 계속 누르는 경우
            /// </summary>
            HoldingKey,
            
            /// <summary>
            /// 특정 키를 뗀 경우
            /// </summary>
            ReleaseKey,
        }

        /// <summary>
        /// 해당 클래스에 기록된 방향 데이터를 외부에서 접근한 경우
        /// 해당 값을 참조한 이후에 방향 데이터를 어떻게 할 것인지 기술하는 열거형 상수
        /// </summary>
        public enum ControllerTrackerLifeSpanType
        {
            /// <summary>
            /// 값을 그대로 유지함
            /// </summary>
            Remain,
            
            /// <summary>
            /// 갱신 콜백 호출시 해당 값들을 초기화 시킴
            /// </summary>
            InitializeOnUpdate,
        }

        public enum TouchGestureType
        {
            None, 

            /// <summary>
            /// 터치 두 점이 모이는 제스쳐
            /// </summary>
            Gather, 
            
            /// <summary>
            /// 터치 두 점이 흩어지는 제스쳐
            /// </summary>
            Scatter, 
            
            /// <summary>
            /// 터치가 지속되는 경우
            /// </summary>
            Stable
        }
        
        #endregion

        #region <Callbacks>

        protected override async UniTask OnCreated()
        {
            KeyCodeSet = ControllerKeyMapData.GetInstanceUnSafe.GetTable().Keys.ToArray();
            KeyCodeScale = (int)KeyCodeSet.Max() + 1;
            InputEventType_Enumerator = SystemTool.GetEnumEnumerator<InputEventType>(SystemTool.GetEnumeratorType.ExceptMaskNone);
            Initialize_SendCommandInputHandler();
            
            _InputEventPresetCollection = new Dictionary<InputEventType, InputEventPreset>();
            _InputEventSenderCollection = new Dictionary<InputEventType, ControllerEventSender>();
            _CommandSystemCollection = new Dictionary<InputEventType, CommandSystem>();
            
            // 입력 이벤트 별로 이벤트 전파자 및 커맨드 시스템을 생성하여 컬렉션화 함.
            foreach (var _inputEventType in InputEventType_Enumerator)
            {
                _InputEventPresetCollection.Add(_inputEventType, new InputEventPreset(_inputEventType));
                _InputEventSenderCollection.Add(_inputEventType, new ControllerEventSender());
                _CommandSystemCollection.Add(_inputEventType, new CommandSystem(_inputEventType));
            }

            await UniTask.CompletedTask;
        }

        public override async UniTask OnInitiate()
        {
            foreach (var _commandSystem in _CommandSystemCollection)
            {
                _commandSystem.Value.OnInitiate();
            }

            await UniTask.CompletedTask;
        }

        public override async UniTask OnScenePreload()
        {
            foreach (var _commandSystem in _CommandSystemCollection)
            {
                await _commandSystem.Value.OnScenePreload();
            }
        }

        public override void OnSceneStarted()
        {
            foreach (var _commandSystem in _CommandSystemCollection)
            {
                _commandSystem.Value.OnSceneStarted();
            }
        }

        public override void OnSceneTerminated()
        {
            foreach (var _commandSystem in _CommandSystemCollection)
            {
                _commandSystem.Value.OnSceneTerminated();
            }
        }

        public override void OnSceneTransition()
        {
            foreach (var _commandSystem in _CommandSystemCollection)
            {
                _commandSystem.Value.OnSceneTransition();
            }
        }
        
        public void OnCheckKeyEvent(float p_DeltaTime)
        {
            foreach (var _commandSystem in _CommandSystemCollection)
            {
                _commandSystem.Value.OnCheckKeyEvent(p_DeltaTime);
            }
        }

        #endregion
        
        #region <Methods>

        public IControllerTracker GetControllerTracker(InputEventType p_InputEventType)
        {
            return _InputEventPresetCollection[p_InputEventType].ControllerTracker;
        } 
        
        public ControllerEventSender GetControllerEventSender(InputEventType p_InputType)
        {
            switch (p_InputType)
            {
                case InputEventType.None:
                    return null;
                default:
                    return _InputEventSenderCollection[p_InputType];
            }
        }

        public CommandSystem GetCommandSystem(InputEventType p_Type)
        {
            switch (p_Type)
            {
                case InputEventType.None:
                    return _CommandSystemCollection[InputEventType.ControlUnit];
                default:
                    return _CommandSystemCollection[p_Type];
            }
        }

        #endregion

        #region <Class>

        public class InputEventPreset
        {
            #region <Fields>

            public InputEventType InputEventType;
            public IControllerTracker ControllerTracker;
            
            #endregion

            #region <Constructors>

            public InputEventPreset(InputEventType p_InputEventType)
            {
                InputEventType = p_InputEventType;
                ControllerTracker = new ControllerTrackRecorderBase();
            }

            #endregion
        }

        #endregion
    }
}