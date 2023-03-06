using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace k514
{
    /// <summary>
    /// 플레이어 인풋과 그에 대응하는 기능(연속 커맨드 입력, 홀딩 입력, 단순 입력 이벤트)을 제어하는 클래스
    /// </summary>
    public partial class CommandSystem : IControllerTrackerBridge, ISceneChange
    {
        #region <Consts>

        /// <summary>
        /// 유니티 엔진 KeyCode 정수값에 대응하는 인덱스에 ControllerKeyMapData.KeyValue 값이 매핑된 테이블
        /// </summary>
        public static readonly int[] KeyValueTable;

        /// <summary>
        /// ControllerKeyMapData.KeyValue 및 조합된 KeyValue 값을 키로 삼아 매핑된 커맨드 타입 컬렉션
        /// </summary>
        private static readonly Dictionary<int, ControllerTool.CommandType> KeyCommandTable;

        /// <summary>
        /// 방향키 갯수
        /// </summary>
        private const int ArrowKeyNumber = 4;
        
        /// <summary>
        /// 방향키를 담당하는 UnityEngine.KeyCode 정수 세트. 방향키 순서는 CommandType 타입에 따라 위 = 0, 왼쪽 = 1, 아래 = 2, 오른쪽 = 3 (반시계방향)
        /// </summary>
        public static readonly int[] UnityArrowKeyValueSet;
        
        /// <summary>
        /// 방향키를 제외한 나머지 KeyCode 정수 세트
        /// </summary>
        private static readonly int[] UnityNonArrowKeyValueSet;

        /// <summary>
        /// 동일한 커맨드 발동 쿨다운
        /// </summary>
        private static readonly int CommandCooldown;

        /// <summary>
        /// 커맨드 큐에 기록된 방향키 이력이 지워지는데 걸리는 제한시간
        /// </summary>
        private static readonly int CommandExpireMsec;
        
        /// <summary>
        /// 커맨드 큐에 기록할 수 있는 최대 커맨드 숫자
        /// </summary>
        private static readonly int CommandMaxCapacity;
        
        static CommandSystem()
        {
            var arrowKeyNumber = 0;
            var nonArrowKeyNumber = 0;       
            var controllerKeyMap = ControllerKeyMapData.GetInstanceUnSafe.GetTable();

            KeyValueTable = new int[ControllerTool.GetInstanceUnSafe.KeyCodeScale];
            UnityArrowKeyValueSet = new int[ArrowKeyNumber];
            
            // 커맨드 키 숫자 = 전체 키 - 방향키 4개 - None키 1개
            UnityNonArrowKeyValueSet = new int[controllerKeyMap.Count - ArrowKeyNumber - 1];
            KeyCommandTable = CommandFunctionMapData.GetInstanceUnSafe.GetTable().ToDictionary(kv => kv.Key, kv => kv.Value.Value);
            
            foreach (var keyCode in ControllerTool.GetInstanceUnSafe.KeyCodeSet)
            {
                var iKeyCode = (int)keyCode;
                switch (keyCode)
                {
                    case KeyCode.UpArrow :
                    case KeyCode.LeftArrow :
                    case KeyCode.DownArrow :
                    case KeyCode.RightArrow :
                        KeyValueTable[iKeyCode] = controllerKeyMap[keyCode].Value;
                        UnityArrowKeyValueSet[arrowKeyNumber++] = iKeyCode;
                        break;
                    case KeyCode.None :
                        break;
                    default :
                        KeyValueTable[iKeyCode] = controllerKeyMap[keyCode].Value;
                        UnityNonArrowKeyValueSet[nonArrowKeyNumber++] = iKeyCode;
                        break;
                }
            }
            
            CommandCooldown = SystemValue.GetSystemValueRecord().CommandCooldown;
            CommandExpireMsec = SystemValue.GetSystemValueRecord().CommandExpireMsec;
            CommandMaxCapacity = SystemValue.GetSystemValueRecord().CommandMaxCapacity;
        }

        #endregion
        
        #region <Fields>
        
        /// <summary>
        /// 방향 데이터 기록을 담당하는 인터페이스
        /// </summary>
        public IControllerTracker ControllerTracker { get; set; }
        
        /// <summary>
        /// 해당 커맨드 입력시스템이 다루는 입력 이벤트 타입
        /// </summary>
        public ControllerTool.InputEventType InputEventType { get; private set; }
        
        /// <summary>
        /// 해당 커맨드 입력시스템에 대응하는 입력 디바이스
        /// </summary>
        public ControllerTool.InputControllerType InputControllerType { get; private set; }
        
        /// <summary>
        /// 특정 인풋이 현재 홀드 중인지 상태를 추적하는 컬렉션
        /// </summary>
        private int CurrentKeyState;
        
        /// <summary>
        /// 방향키가 눌린 경우, 해당 순서를 일정시간 저장하는 커맨드 큐
        /// </summary>
        private List<int> CommandQueue;

        /// <summary>
        /// 마지막으로 눌린 방향키 코드, CommandQueue의 마지막 원소는 CommandExpireMsec 만큼의 수명을 가지기 때문에
        /// 방향키를 꾹누르는 경우에 대응할 수 없어서 따로 멤버를 만들어 관리하는 것.
        /// 최대 2개의 값을 저장한다.
        /// </summary>
        private (int t_Prev, int t_Current) StackedArrowKeyCodePressed;

        /// <summary>
        /// 현재 방향키 입력 상태
        /// </summary>
        private ArrowKeyInputStateType CurrentArrowInputState;

        /// <summary>
        /// 마지막에 적용되었던 방향키 이동 타입과 현재 방향키 이동 타입의 차이 성분을 가지는 방향키 이동 타입
        /// 예를 들어 (좌)방향을 누르다가 (우)방향이 눌리면 실제 상태는 (좌+우)이지만 해당 값은 그 변위 값인 (우)가 가진다.
        /// </summary>
        private ArrowType LastTransitedArrowTypeDeltaCompare;
        
        /// <summary>
        /// 특정 입력이 발생 했을 때, 어느 디바이스에 의한 것인지 표시하는 컬렉션
        /// </summary>
        private ControllerTool.InputControllerType[] InputDeviceMap;

        /// <summary>
        /// 방향키는 4키를 조합해야 하기 때문에, 각 키가 어떤 디바이스로부터 입력됬는지 평균을 내서 선정한다.
        /// </summary>
        private ControllerTool.InputControllerType ArrowDeviceType;

        /// <summary>
        /// 커맨드 입력이벤트를 입력과 동시에 발생시키면, 추가 커맨드를 입력했을 때 하나의 커맨드에서 2개 이상의 이벤트가 발생하게 되므로
        /// 입력과 이벤트 사이에 텀을 두기 위해 사용되는 플래그 TODO<4415>(미구현)
        /// </summary>
        private bool CommandActionYieldFlag;

        /// <summary>
        /// 홀딩 커맨드가 입력된 경우 쿨다운을 부여하기 위한 리스트
        /// </summary>
        private List<int> HoldingCommandBlock;
        
        #endregion

        #region <Enums>

        /// <summary>
        /// 현재 방향키가 입력 상태 타입
        /// </summary>
        public enum ArrowKeyInputStateType
        {
            Off, On
        }

        #endregion

        #region <Constructor>

        public CommandSystem(ControllerTool.InputEventType p_InputEventType)
        {
            InputEventType = p_InputEventType;
            InputControllerType = InputEventDeviceMap.GetInstanceUnSafe.GetTableData(InputEventType).DeviceType;
            // 터치-키보드 타입의 경우에는 터치 파츠가 Drag Interface에 의해 초기화 된다.
            ControllerTracker = ControllerTool.GetInstanceUnSafe.GetControllerTracker(InputEventType);
            InputDeviceMap = new ControllerTool.InputControllerType[ControllerTool.GetInstanceUnSafe.KeyCodeScale];
            CommandQueue = new List<int>();
            
            HoldingCommandBlock = new List<int>();
            UpdateCancellationToken();

            OnCreatePartialInput();
            OnInitiate();
        }

        #endregion
        
        #region <Callbacks>

        public void OnInitiate()
        {
        }

        public async UniTask OnScenePreload()
        {
            await UniTask.CompletedTask;
        }

        public void OnSceneStarted()
        {
        }

        public void OnSceneTerminated()
        {
            ClearController();
        }
        
        public void OnSceneTransition()
        {
        }
        
        /// <summary>
        /// 컨트롤러 동작 진입점
        /// </summary>
        public void OnCheckKeyEvent(float p_DeltaTime)
        {
            // 방향키 이벤트는 4방향의 입력을 한번에 처리하기 때문에, 
            // 이전 방향키의 정보를 알아야 해당 프레임에서 어떤 방향으로 변했는지 알기 위해서는
            // 마지막에 입력됬던 방향키 커맨드를 로컬 변수로 캐싱해야한다.
            StackedArrowKeyCodePressed.t_Prev = StackedArrowKeyCodePressed.t_Current;

            CheckArrowEvent();
            CheckNonArrowEvent();

            // 이벤트 처리가 끝났으면, 컨트롤러 트래커에서 현재 방향키 정보를 갱신시켜준다.
            ControllerTracker.OnUpdateController();

            void CheckArrowEvent()
            {
                // 방향키 >> 커맨드 입력이나 대쉬 체크용으로 4 방향키에 대한 상태를 체크하고
                // 이동 로직은 Input.GetAxis를 통해 처리한다.
                foreach (var arrowKeyCode in UnityArrowKeyValueSet)
                {
                    var keyValue = KeyValueTable[arrowKeyCode];
                    if (HasKeyBit(keyValue))
                    {
                        // 특정 방향 키 플래그와 실제 키 상태를 동기화 시켜준다.
                        if (InputControllerType == GetKeyUp(InputEventType, arrowKeyCode))
                        {
                            InputDeviceMap[arrowKeyCode] = ControllerTool.InputControllerType.None;
                            DeleteKeyBit(keyValue);
                        }
                    }
                    else
                    {
                        var tryDeviceType = GetKeyDown(InputEventType, arrowKeyCode);
                        if (tryDeviceType != ControllerTool.InputControllerType.None && (tryDeviceType | InputControllerType) == InputControllerType)
                        {
                            InputDeviceMap[arrowKeyCode] = tryDeviceType;
                            AddKeyBit(keyValue);
                            ArrowCommandExpireReq(keyValue);
                        }
                    }
                }

                var currentArrowState = GetCurrentArrowType();
                switch (currentArrowState)
                {
                    // 방향키 입력이 없는 경우, 컨트롤러 방향키 상태를 초기화 한다.
                    case ArrowType.None:
                        switch (CurrentArrowInputState)
                        {
                            case ArrowKeyInputStateType.Off:
                                break;
                            case ArrowKeyInputStateType.On:
                                SetControllerMoveOff();
                                break;
                        }
                        break;
                    
                    // 방향키 입력이 있는 경우, 해당 방향키로의 이벤트 콜백을 호출한다.
                    case ArrowType.SoloUp:
                    case ArrowType.SoloLeft:
                    case ArrowType.SoloDown:
                    case ArrowType.SoloRight:
                        OnArrowEventInputted(currentArrowState, true);
                        break;
                    case ArrowType.UpLeft:
                    case ArrowType.LeftDown:
                    case ArrowType.DownRight:
                    case ArrowType.RightUp:
                        OnArrowEventInputted(currentArrowState, false);
                        break;
                    // 방향키가 교착 상태인 경우, 마지막에 전이했던 값을 따른다.
                    case ArrowType.LeftRight:
                    case ArrowType.UpDown:
                    case ArrowType.UpLeftDown:
                    case ArrowType.LeftDownRight:
                    case ArrowType.DownRightUp:
                    case ArrowType.RightUpLeft:
                    case ArrowType.UpLeftDownRight:
                        switch (CurrentArrowInputState)
                        {
                            // 교착 상태이면서 Off라는 거는 최초 입력이 교착상태로 시전됬다는 것이므로, 이동 이벤트를 불발시킨다.
                            case ArrowKeyInputStateType.Off:
                                break;
                            case ArrowKeyInputStateType.On:
    #if OVERRAP_DEADLOCK_ARROWTYPE
                                // OVERRAP_DEADLOCK_ARROWTYPE 정의문이 적용되는 경우, 교착상태에서 마지막에 입력된 값을 기준으로 이벤트를 처리한다.
                                LastTransitedArrowTypeDeltaCompare = GetLastPressedArrowTypeDelta(currentArrowState);
                                OnAnyArrowKeyPressed(LastTransitedArrowTypeDeltaCompare, false);
    #else
                                // OVERRAP_DEADLOCK_ARROWTYPE 정의문이 적용되지 않는 경우, 교착상태에서는 교착상태가 되기 전에 적용되던 값을 
                                // 기준으로 이벤트를 처리한다.
                                OnArrowEventInputted(ControllerTracker.CurrentArrowType, false);
    #endif
                                break;
                        }
                        break;
                }
            }

            void CheckNonArrowEvent()
            {
                // 방향키 이외 커맨드키
                foreach (var keyCode in UnityNonArrowKeyValueSet)
                {
                    var keyValue = KeyValueTable[keyCode];
                    // 이미 해당 키가 입력중인 키인 경우
                    if (HasKeyBit(keyValue))
                    {
                        if (InputControllerType == GetKeyUp(InputEventType, keyCode))
                        {
                            InputDeviceMap[keyCode] = ControllerTool.InputControllerType.None;
                            DeleteKeyBit(keyValue);
                            OnReleaseKey(keyCode, keyValue);
                        }
                        else
                        {
                            OnKeepKey(keyCode, keyValue);
                        }
                    }
                    else
                    {
                        var tryDeviceType = GetKeyDown(InputEventType, keyCode);
                        if ((tryDeviceType & InputControllerType) != ControllerTool.InputControllerType.None)
                        {
                            InputDeviceMap[keyCode] = tryDeviceType;
                            AddKeyBit(keyValue);
                        
                            // 홀딩 또는 연속 커맨드가 가능한 타입의 키였던 경우,
                            if (IsCommandableKey(keyValue))
                            {
                                // 커맨드 기록을 갱신한다.
                                CommandExpireReq(keyValue);
                                OnPressCommandKey(keyCode, keyValue);
                            }
                            // 단일 커맨드였던 경우
                            else
                            {
                                // 커맨드 기록을 지운다.
                                ClearCommand();
                                OnPressKey(keyCode, keyValue);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 현재 입력된 키타입 및 이전 프레임에서 입력된 키타입(정수)를 가지고, 해당 프레임의 컨트롤러 대쉬 여부 및
        /// 이동 방향을 선정하는 기능을 하는 콜백
        /// </summary>
        private void OnArrowEventInputted(ArrowType p_ArrowKeyType, bool p_IsSoleArrowType)
        {
            // ArrowDeviceType은 터치와 키보드 중에 더 많이 기여한 타입으로 선정된다.
            switch (ArrowDeviceType)
            {
                // 터치 ControllerTracker는 DragTouchSenderBase클래스의 OnDrag 콜백에 의해 초기화 됨
#if !SERVER_DRIVE
                case ControllerTool.InputControllerType.TouchPanel:
                {
                    switch (CurrentArrowInputState)
                    {
                        // 방향키가 최초로 입력된 경우
                        case ArrowKeyInputStateType.Off:
                            CurrentArrowInputState = ArrowKeyInputStateType.On;
                            OnUpdateDashFlag(TouchEventManager.GetInstance.GetDashState(InputEventType));
                            OnMoveEventOccured(ControllerTool.CommandType.Move, InputEventType, ArrowDeviceType, ControllerTool.ControllerInputStateType.PressKey);
                            break;
                        // 이미 방향키 입력중인 상태에서 다른 방향으로 연속 방향키가 입력되었는지 체크한다.
                        case ArrowKeyInputStateType.On:
                            OnUpdateDashFlag(TouchEventManager.GetInstance.GetDashState(InputEventType));
                            OnMoveEventOccured(ControllerTool.CommandType.Move, InputEventType, ArrowDeviceType, ControllerTool.ControllerInputStateType.HoldingKey);
                            break;
                    }
                }
                    break;
#endif
                case ControllerTool.InputControllerType.Keyboard:
                {
                    // 키보드 ControllerTracker는 해당 섹션에 의해 초기화 됨
                    ControllerTracker.SetControllerDirection(p_ArrowKeyType);

                    switch (CurrentArrowInputState)
                    {
                        // 방향키가 최초로 입력된 경우
                        case ArrowKeyInputStateType.Off:
                            CurrentArrowInputState = ArrowKeyInputStateType.On;
                            if (!DashFlag && HasKeyBit(StackedArrowKeyCodePressed.t_Prev))
                            {
                                OnUpdateDashFlag(true);
                            }
                            OnMoveEventOccured(ControllerTool.CommandType.Move, InputEventType, ArrowDeviceType, ControllerTool.ControllerInputStateType.PressKey);
                            break;
                        // 이미 방향키 입력중인 상태에서 다른 방향으로 연속 방향키가 입력되었는지 체크한다.
                        case ArrowKeyInputStateType.On:
                            if (!DashFlag && HasPerspectiveKeyDoubleTriggered(p_ArrowKeyType, p_IsSoleArrowType).Item1)
                            {
                                OnUpdateDashFlag(true);
                            }
                            OnMoveEventOccured(ControllerTool.CommandType.Move, InputEventType, ArrowDeviceType, ControllerTool.ControllerInputStateType.HoldingKey);
                            break;
                    }
                }
                    break;
            }
        }

        /// <summary>
        /// 연속 커맨드나 홀딩 커맨드를 처리하는 인풋 콜백.
        /// 홀딩 커맨드가 우선적으로 처리된다.
        /// </summary>
        public void OnPressCommandKey(int p_TargetKey, int p_KeyCode)
        {
#if UNITY_EDITOR
            if(CustomDebug.PrintCommandState)
                Debug.Log($" {p_TargetKey} 눌림 [{p_KeyCode}](holding)");
#endif
            // 홀딩 커맨드를 우선 검증한다.
            if (!HoldingCommandBlock.Contains(CurrentKeyState) && KeyCommandTable.TryGetValue(-CurrentKeyState, out var o_HoldingCommandFunction))
            {
#if UNITY_EDITOR
                if(CustomDebug.PrintCommandState)
                    Debug.Log($"holding :: {o_HoldingCommandFunction} activated");
#endif
                // 홀딩 키 이벤트를 전파한다.
                OnSendHoldingCommandInput(p_TargetKey, o_HoldingCommandFunction, InputEventType, InputDeviceMap[p_TargetKey], ControllerTool.ControllerInputStateType.PressKey);
                
                // 홀딩 키 쿨다운을 세트한다.
                HoldingKeyEventCooldownReq(CurrentKeyState);
            }
            // 홀딩 커맨드가 없는 경우에, 연속 커맨드를 검증한다.
            else
            {
                // 현재 큐에 등록되어 있는 키 코드를 조합하여 커맨드 코드를 가져온다.
                var commandCode = GetCommandCode();
                // 현재 커맨드 코드가 한자릿수인 경우,
                if (-commandCode < ControllerKeyMapData.COMMAND_KEY_UPPERBOUND)
                {
                    OnPressKey(p_TargetKey, -commandCode);
                }
                // 두자릿수 이상인 경우
                else
                {
                    // 앞자리부터 코드를 하나씩 지워가면서
                    // 키맵에 일치하는 커맨드가 있는지 검색한다.
                    while (!KeyCommandTable.ContainsKey(commandCode))
                    {
                        // 남은 자릿수가 있는 경우 루프한다.
                        if (GetNextCommandCode(commandCode, out var o_NextCode))
                        {
#if UNITY_EDITOR
                            if(CustomDebug.PrintCommandState)
                                Debug.Log($"key code changed {commandCode} => {o_NextCode}");
#endif
                            commandCode = o_NextCode;
                        }
                        // 남은 자릿수가 없다면 입력된 코드르 단일 커맨드로서 발동시킨다.
                        else
                        {
                            OnPressKey(p_TargetKey, -commandCode);
                            return;
                        }
                    }
                    
#if UNITY_EDITOR
                    if(CustomDebug.PrintCommandState)
                        Debug.Log($"sequence :: {KeyCommandTable[commandCode]} activated");
#endif
                    /* 키맵에서 일치하는 코드를 찾아낸 경우, 루프를 탈출하여 해당 섹션에 진입한다. */
                    //
                    // 연속 키 이벤트를 전파한다.
                    ClearCommand();
                    OnSendSequenceCommandInput(p_TargetKey, KeyCommandTable[commandCode], InputEventType, InputDeviceMap[p_TargetKey], ControllerTool.ControllerInputStateType.PressKey);
                }
            }
        }

        public void OnPressKey(int p_TargetKey, int p_KeyCode)
        {
#if UNITY_EDITOR
            if(CustomDebug.PrintCommandState)
                Debug.Log($" {p_TargetKey} 눌림 [{p_KeyCode}]");
#endif

            if (KeyCommandTable.TryGetValue(p_KeyCode, out var o_FunctionType))
            {
                // 단일 기능 키 이벤트를 전파한다.
                OnSendMonoCommandInput(p_TargetKey, o_FunctionType, InputEventType, InputDeviceMap[p_TargetKey], ControllerTool.ControllerInputStateType.PressKey);
            }
        }
        
        /// <summary>
        /// 키를 계속 누르고 있는 경우 이벤트인데, 솔직히 해당 기능을 통해 구현하는 것 보다는
        /// 위 아래의 Press/Release 이벤트를 통해 구현하는 게 더 퍼포먼스가 좋다.
        /// </summary>
        public void OnKeepKey(int p_TargetKey, int p_KeyCode)
        {
#if UNITY_EDITOR
            if(CustomDebug.PrintCommandPushingState)
                Debug.Log($" {p_TargetKey} 눌리는 중 [{p_KeyCode}]");
#endif  
            OnSendHoldingCommandInput(p_TargetKey, KeyCommandTable[p_KeyCode], InputEventType, InputDeviceMap[p_TargetKey], ControllerTool.ControllerInputStateType.HoldingKey);
        }
        
        public void OnReleaseKey(int p_TargetKey, int p_KeyCode)
        {
#if UNITY_EDITOR
            if(CustomDebug.PrintCommandState)
                Debug.Log($" {p_TargetKey} 떼짐 [{p_KeyCode}]");
#endif
            // 어떤 키가 눌린 상태에 전이했다는 것은, 컬렉션에 해당 키코드가 있음을 증명한 것이므로
            // Press 이벤트와 다르게 Release 이벤트에서는 TryGet 메서드를 통해 검증하지 않아도 된다.
            // 단일 기능 키 이벤트를 전파한다.
            OnSendReleaseCommandInput(p_TargetKey, KeyCommandTable[p_KeyCode], InputEventType, InputDeviceMap[p_TargetKey], ControllerTool.ControllerInputStateType.ReleaseKey);
        }
        
        #endregion

        #region <Methods>

        /// <summary>
        /// 현재 방향키 인풋이 없을 때, 컨트롤러 방향키 상태를 정지 상태로 만들고
        /// 해당 이벤트를 각 리스너에게 전파하는 메서드
        /// </summary>
        private void SetControllerMoveOff()
        {
            CurrentArrowInputState = ArrowKeyInputStateType.Off;
            ControllerTracker.SetControllerDirection(ArrowType.None);
            OnMoveEventTerminated(ControllerTool.CommandType.Move, InputEventType, ArrowDeviceType, ControllerTool.ControllerInputStateType.ReleaseKey);
            LastTransitedArrowTypeDeltaCompare = ArrowType.None;
            OnUpdateDashFlag(false);
        }

        /// <summary>
        /// 임의의 방향키가 하나라도 입력 상태인지 체크하는 논리 메서드
        /// 루프 코스트 때문에 그냥 반복문 사용하지 않는 이쪽이 더 빠르다.
        /// </summary>
        private ArrowType GetCurrentArrowType()
        {
#if !SERVER_DRIVE
            // 해당 값이 클수록 TouchPanel 입력이 많음을 의미함
            var biasStore = 0;
#endif
            var result = ArrowType.None;

            for (int i = 0; i < ArrowKeyNumber; i++)
            {
                var unityArrowKeyCode = UnityArrowKeyValueSet[i];
                var commandValue = KeyValueTable[unityArrowKeyCode];
                if (HasKeyBit(commandValue))
                {
#if !SERVER_DRIVE
                    var arrowInputType = InputDeviceMap[unityArrowKeyCode];
                    switch (arrowInputType)
                    {
                        case ControllerTool.InputControllerType.TouchPanel:
                            biasStore++;
                            break;
                        case ControllerTool.InputControllerType.Keyboard:
                            biasStore--;
                            break;
                    }
#endif
                    result += 1 << i;
                }
            }
            
            // 방향키 입력이 없던 경우
            if (result == ArrowType.None)
            {
                ArrowDeviceType = ControllerTool.InputControllerType.None;
            }
            // 방향키 입력이 감지된 경우 입력 장치가 네 방향키에 더 기여하고 있는지 검증한다.
            else
            {
#if SERVER_DRIVE
                ArrowDeviceType = ControllerTool.InputControllerType.Keyboard;
#else
                if (biasStore < 0)
                {
                    ArrowDeviceType = ControllerTool.InputControllerType.Keyboard;
                }
                else
                {
                    ArrowDeviceType = ControllerTool.InputControllerType.TouchPanel;
                }
#endif
            }

            return result;
        }

        private bool IsSoloArrowKey(ArrowType p_TargetType)
        {
            switch (p_TargetType)
            {
                case ArrowType.SoloUp:
                case ArrowType.SoloLeft:
                case ArrowType.SoloDown:
                case ArrowType.SoloRight:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 입력 받은 키가, 홀딩 혹은 연속 키로 커맨드가 가능한 키인지 검증하는 논리 메서드
        /// </summary>
        private bool IsCommandableKey(int p_KeyCode)
        {
            return p_KeyCode < ControllerKeyMapData.COMMAND_KEY_UPPERBOUND;
        }

        /// <summary>
        /// 특정 키가 현재 눌려있는지 검증하는 논리 메서드
        /// </summary>
        public bool HasKeyBit(int p_KeyBit)
        {
            var pivot = 1 << p_KeyBit;
            return (CurrentKeyState & pivot) == pivot;
        }

        /// <summary>
        /// 특정 키를 홀딩 상태로 만드는 메서드
        /// </summary>
        public void AddKeyBit(int p_KeyBit)
        {
            CurrentKeyState |= 1 << p_KeyBit;
        }
        
        /// <summary>
        /// 특정 키를 반전시키는 메서드
        /// </summary>
        public void ReverseKeyBit(int p_KeyBit)
        {
            CurrentKeyState ^= 1 << p_KeyBit;
        }
        /// <summary>
        /// 특정 키를 릴리스 상태로 만드는 메서드
        /// </summary>
        public void DeleteKeyBit(int p_KeyBit)
        {
            CurrentKeyState &= ~(1 << p_KeyBit);
        }

        public void ClearCommand()
        {
            CommandQueue.Clear();
        }

        public void ClearYieldCommandStack()
        {
            CommandActionYieldFlag = false;
        }
        
        /// <summary>
        /// 컨트롤러 상태를 초기화 시키는 메서드
        /// </summary>
        public void ClearController()
        {
            UpdateCancellationToken();
            StackedArrowKeyCodePressed = default;
            CurrentKeyState = 0;
            HoldingCommandBlock.Clear();
            ClearYieldCommandStack();            
            ClearCommand();
            SetControllerMoveOff();
        }
        
        /// <summary>
        /// 현재 커맨드 큐에 등록된 값을 기반으로 특정 커맨드 코드를 생성하는 메서드
        /// 예를들어 Q[3, 4, 2, 1(new!)] => 3421 => -3421을 리턴한다.
        /// </summary>
        private int GetCommandCode()
        {
            var resultCode = 0;
            var currentCommandNumber = CommandQueue.Count;
            for (int i = 0; i < currentCommandNumber; i++)
            {
                resultCode += CommandQueue[i] * 10.PowByte(currentCommandNumber - i - 1);
            }

            return -resultCode;
        }

        /// <summary>
        /// GetCommandCode로 가져온 커맨드 코드와 일치하는 값이 테이블에 없는 경우, 맨 앞자리의 코드 정보를 제거한
        /// 나머지 코드를 리턴하는 메서드
        /// </summary>
        private bool GetNextCommandCode(int p_CurrentCode, out int o_NextCode)
        {
            var topCommandNumber = CommandQueue.Count - 1;
            if (topCommandNumber > 0)
            {
                var topCommand = CommandQueue[0];
                CommandQueue.RemoveAt(0);
                o_NextCode = p_CurrentCode + topCommand * 10.PowByte(topCommandNumber);
                return true;
            }
            else
            {
                o_NextCode = 0;
                return false;
            }
        }

        #if UNITY_EDITOR
        /// <summary>
        /// 현재 커맨드 큐에 등록되어 있는 값을 콘솔에 출력하는 메서드
        /// </summary>
        private void OpenCommand()
        {
            Debug.Log($"*********************************");
            var command = "[ ";
            var onceFlag = false;
            foreach (var VARIABLE in CommandQueue)
            {
                if (onceFlag)
                {
                    command += ($", {VARIABLE}");
                }
                else
                {
                    onceFlag = true;
                    command += ($"{VARIABLE}");
                }
            }

            command += (" ]");
            Debug.Log(command);
            Debug.Log($"*********************************");
        }
        #endif

        #endregion
    }
}