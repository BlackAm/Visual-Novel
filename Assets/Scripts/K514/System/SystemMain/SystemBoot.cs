using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BlackAm
{
    /// <summary>
    /// 해당 스크립트를 Entry Point로 하여, 특정한 싱글톤 및 테이블 데이터 클래스들이
    /// Awake에서 초기화된다.
    /// Script Execution Order가 -50으로 설정되어 있다.
    /// </summary>
    [ExecutionOrder(-50)]
    public partial class SystemBoot : UnitySingleton<SystemBoot>, ISceneChange
    {
        #region <Fields>

        /// <summary>
        /// 아핀 객체
        /// </summary>
        [NonSerialized] public Transform _Transform;

        /// <summary>
        /// 프레임 마지막에 처리할 시스템 이벤트 플래그 마스크
        /// </summary>
        private SystemTool.SystemOnceFrameEventType _LateEventFlagMask;

        /// <summary>
        /// 현재 SystemBoot 상태를 표시하는 페이즈 상수
        /// </summary>
        private SystemBootPhase _CurrentPhase;

        #endregion
        
        #region <Enums>

        private enum SystemBootPhase
        {
            None,
            TryPatch,
            MaintenanceSystem,
            InitializeSystem,
            SystemsFunctional,
            SceneTransition,
            Terminated
        }

        #endregion
        
        #region <Callbacks>

#if UNITY_EDITOR
        private void Awake()
        {
            SingletonTool.ClearActiveSingleton();
            SingletonTool.CreateSingleton(typeof(SystemBoot));
        }
#endif

        protected override void OnCreated()
        {
            _Transform = transform;
            
#if UNITY_EDITOR
            // 에디터 모드 실행시, 종료시 및 컴파일 종료시에, 싱글톤 및 번들을 해제시키도록 이벤트 핸들러를 등록한다.
            EditorApplication.pauseStateChanged += OnEditorPauseStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
#endif
            // 시간 값을 초기화 시켜준다.
            SingletonTool.CreateSingleton(typeof(TimeManager));
            
#if UNITY_ANDROID
            // 게임 환경 설정
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Screen.orientation = ScreenOrientation.AutoRotation;
            // 디바이스 가로 허용
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;
            // 디바이스 세로 방지
            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;
#endif
            // 오브젝트 파기 방지
            DontDestroyOnLoad(gameObject);

            // 시스템 관리 폴더 초기화
            SystemMaintenance.InitSystemMaintenance();

            // 부분클래스 초기화
            OnCreateSystemBootTimer();
#if SERVER_DRIVE
            OnCreated_ServerDrive();
#endif
            // SystemBoot 오브젝트 초기화 이후 수행할 작업 예약
            OnSystemBootInitiated();
        }

        public override void OnInitiate()
        {
        }

        private void Update()
        {
            var deltaTime = Time.deltaTime;
            
            if (_CurrentPhase == SystemBootPhase.SystemsFunctional)
            {
                GameEventTimer.Tick(deltaTime);
                
                ControllerTool.GetInstanceUnSafe.OnCheckKeyEvent(deltaTime);
#if SERVER_DRIVE
                OnUpdate_ServerDrive(deltaTime);
                PopUpManager.GetInstance.OnUpdatePopUpManager(deltaTime);
#else
#endif
            }
        }

        private void LateUpdate()
        {
            var deltaTime = Time.deltaTime;
            
            if (_CurrentPhase == SystemBootPhase.SystemsFunctional)
            {
                GameLateEventTimer.Tick(deltaTime);
                
                DialogueGameManager.GetInstance.OnUpdate(deltaTime);
#if SERVER_DRIVE
#else
                // UI는 카메라 이전에 작업을 수행해야 함
                var uiRoot = UICustomRoot.GetInstanceUnSafe;
                if (!ReferenceEquals(null, uiRoot))
                {
                    uiRoot.OnUpdate(deltaTime);
                }

                // 카메라 처리
                // 카메라가 UnitInteractManager보다 먼저 수행되면, UI이름표 등의 이벤트가 처리된 이후
                // 카메라가 움직이므로 UI 위치가 이상해진다.
                CameraManager.GetInstanceUnSafe.OnUpdateCameraManager(deltaTime);
#endif
                // 각 유닛이 카메라등의 외부 매니저에 의해 발생한 이벤트를 처리함

                // 유닛과는 상관없는 시스템 기능 처리
                OnHandleOnceFrameEvent();
            }
        }

        private void FixedUpdate()
        {
            var deltaTime = Time.fixedDeltaTime;
            SystemEventTimer.Tick(deltaTime);
            
            if (_CurrentPhase == SystemBootPhase.SystemsFunctional)
            {
                FixedGameEventTimer.Tick(deltaTime);
            }
        }
        
        /// <summary>
        /// 프레임당 한번만 처리해야하는 이벤트
        /// </summary>
        private void OnHandleOnceFrameEvent()
        {
            foreach (var eventType in SystemTool.SystemLateHandleEventTypeEnumerator)
            {
                if (_LateEventFlagMask.HasAnyFlagExceptNone(eventType))
                {
                    switch (eventType)
                    {
                        case SystemTool.SystemOnceFrameEventType.FlashScreen:
                            break;
                        case SystemTool.SystemOnceFrameEventType.Beep:
                            break;
                    }
                }
            }

            _LateEventFlagMask = SystemTool.SystemOnceFrameEventType.None;
        }

#if UNITY_EDITOR
        private void OnEditorPauseStateChanged(PauseState p_Type)
        {
            switch (p_Type)
            {
                case PauseState.Paused:
                    break;
                case PauseState.Unpaused:
                    break;
            }
        }

        private void OnPlayModeChanged(PlayModeStateChange p_Type)
        {
            switch (p_Type)
            {
                case PlayModeStateChange.EnteredEditMode:
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    break;
                // '유니티에디터가 플레이 모드에서 에디터 모드로 전이했을 경우에만' => 즉, 게임의 종료
                case PlayModeStateChange.ExitingPlayMode:
                    Dispose();
                    break;
            }
        }
#endif

        #endregion

        #region <Callbacks/SceneEvent>

        /// <summary>
        /// 씬 매니저에 바인드된 함수0.
        /// 씬이 전환된 경우 씬로더에 호출되거나 최초 SystemBoot 클래스 초기화 때(BootStrapSceneEnvironment)에만 호출되며, 씬 초기상태에서 시스템 상 수행할 작업을 기술한다.
        /// </summary>
        public async UniTask OnScenePreload()
        {
            await SceneChangeEventSender.GetInstance.OnScenePreload();
        }
        
        /// <summary>
        /// 씬 매니저에 바인드된 함수1.
        /// 씬이 전환된 경우 씬로더에 호출되거나 최초 SystemBoot 클래스 초기화 때에만 호출되며, 씬 초기상태에서 시스템 상 수행할 작업을 기술한다.
        /// </summary>
        public void OnSceneStarted()
        {
            if (SceneControllerManager.GetInstance.IsSceneStable())
            {
                SceneChangeEventSender.GetInstance.OnSceneStarted();
                _CurrentPhase = SystemBootPhase.SystemsFunctional;

                // 씬 시작시 게임 이벤트 타이머를 활성화 시킨다.
                SetGameTimerStart();
                
#if !SERVER_DRIVE
                // 게임 진입을 위해, 게임 화면을 밝혀준다.
                DefaultUIManagerSet.GetInstanceUnSafe?.SetFadeInUI();

                // 브금을 같이 켜준다.
                BGMManager.GetInstance.PlayBGM(true);
#endif
            }
        }
        
        /// <summary>
        /// 현재 씬이 종료되기 SceneController.SceneJumpDelayMsec 전에 호출되어, 시스템 상 수행할 작업을 수행한다.
        /// </summary>
        public void OnSceneTerminated()
        {
            if (SceneControllerManager.GetInstance.IsSceneStable())
            {
                SceneChangeEventSender.GetInstance.OnSceneTerminated();
             
    #if !SERVER_DRIVE
                // 페이드 아웃
                DefaultUIManagerSet.GetInstanceUnSafe?.SetFadeOutUI();
    #endif
           
                // 씬 종료시 게임 이벤트 타이머를 비활성화 시킨다.
                SetGameTimerPause();
            }
        }
        
        /// <summary>
        /// 현재 씬이 종료되어 씬 전환이 일어나는 시점에 호출된다.
        /// </summary>
        public void OnSceneTransition()
        {
            if (SceneControllerManager.GetInstance.IsSceneStable())
            {
                SceneChangeEventSender.GetInstance.OnSceneTransition();
                _CurrentPhase = SystemBootPhase.SceneTransition;

#if !SERVER_DRIVE
                // 브금 종료
                BGMManager.GetInstance.ReleaseBGM();
#endif
            }
        }

        #endregion
        
        #region <Methods>

        public bool IsSystemOpen()
        {
            return _CurrentPhase == SystemBootPhase.SystemsFunctional;
        }

        public void SetSystemLateFlag(SystemTool.SystemOnceFrameEventType p_EventType)
        {
            _LateEventFlagMask.AddFlag(p_EventType);
        }

        /// <summary>
        /// 어플리케이션을 종료시키는 메서드
        /// </summary>
        public void ExitGame()
        {
#if UNITY_EDITOR
            SystemTool.PlayEditorMode(false);
#else
            Application.Quit();
#endif
        }
        
        #endregion

        #region <Disposable>

        /// <summary>
        /// 게임모드 혹은 어플리케이션 종료시, 이후 에디터 모드 등을 원할하게 하기 위해서
        /// 시스템에서 사용했던 테이블이나 에셋번들 등을 릴리스 해준다.
        /// </summary>
        protected override void DisposeUnManaged()
        {
            _CurrentPhase = SystemBootPhase.Terminated;

#if UNITY_EDITOR
            ResourceTracker.GetInstanceUnSafe?.UpdateTableFile(ExportDataTool.WriteType.Overlap);
#endif
            DisposeTimer();
            
            SystemMaintenance.ReleaseSystem();

            base.DisposeUnManaged();
            
#if UNITY_EDITOR
            SingletonTool.PrintActiveSingleton();
#endif
        }

        #endregion
    }
}