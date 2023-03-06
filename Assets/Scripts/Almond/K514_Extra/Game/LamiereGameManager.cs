#if !SERVER_DRIVE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using BDG;
using Cysharp.Threading.Tasks;
using UI2020;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace k514
{
    /// <summary>
    /// 해당 클라이언트에서 플레이어로서 사용할 유닛을 다루는 싱글톤 클래스
    /// </summary>
    public partial class LamiereGameManager : SceneChangeEventAsyncSingleton<LamiereGameManager>
    {
        #region <Fields>

        // momo6346 - 스킬변경 관련 변수입니다.
        // 직업별 1,2,3,4번째 스킬이 아래 변수선언 순서입니다.
        public ControllerTool.CommandType buttonZ = ControllerTool.CommandType.Z;
        public ControllerTool.CommandType buttonV = ControllerTool.CommandType.V;
        public ControllerTool.CommandType buttonB = ControllerTool.CommandType.B;
        public ControllerTool.CommandType buttonCtrl = ControllerTool.CommandType.L_Ctrl;
        
        // 맵 이동 시 캐릭터의 스킬 정보를 저장하여 다시 배치하려고 선언.
        public Dictionary<ControllerTool.CommandType, ActableTool.UnitActionSpellPreset> saveCommandTable;
        
        /// <summary>
        /// 현재 선정된 플레이어 유닛
        /// 프로퍼티 접근자를 통해 관련 필드들을 초기화하는 기능을 가진다.
        /// </summary>
        public LamiereUnit _ClientPlayer;
        
        /// <summary>
        /// 스폰할 플레이어의 프리팹 데이터 인덱스
        /// </summary>
        public int PlayerModelDataIndex { get; private set; }

        public List<ulong> EquipItemKey = new List<ulong>();
        /// <summary>
        /// 플레이하고 있는 서버 이름
        /// </summary>
        public string ServerName;

        /// <summary>
        /// 플레이어 이름
        /// </summary>
        public string PlayerName;
        /// <summary>
        /// 서버 동기화 패킷 송신용 이벤트 오브젝트
        /// </summary>
        private GameEventTimerHandlerWrapper _SyncEventHandler;
        
        /// <summary>
        /// 플레이어 변경 이벤트 수신 오브젝트
        /// </summary>
        private PlayerChangeEventReceiver _EventReceiver;
        
        /// <summary>
        /// 씬 설정이 변경된 경우, 해당 이벤트를 수신받는 오브젝트
        /// </summary>
        public SceneVariableChangeEventReceiver SceneVariableChangeEventReceiver { get; private set; }
        
        /// <summary>
        /// 현재 맵의 바닥 타입 리스트
        /// </summary>
        public FootStepLocation.TextureType CurrentSceneTextureType { private set; get; }
        
        /// <summary>
        /// 현재 맵의 효과음 인덱스
        /// </summary>
        public int CurrentSceneEffectSoundIndex { private set; get; }

        private Vector3 _mapStartPosition;
        private Vector3 _mapStartRotation;

        public bool isTutorial = false;

        public int _CurrentScene;
        
        public bool IsQuest = false;

        public bool QuestMove = false;

        public int QuestIndex;

        private MapEfffectUnit Walla;
        
        /// <summary>
        /// 클릭 이동시 이동 경로를 표시해주는 플래그
        /// </summary>
        public bool ShowTouchedMovementWay = true;

        public bool ReSearchWayPoint = false;
        public bool isFirstDelete = false;
        //public bool isClearCommand = false;
        private int privateCurrentSceneEffectSoundIndex;
        
        #endregion

        #region <Callbacks>

        protected override async UniTask OnCreated()
        {
            // TODO<K514> : 개선 필요. 스킬 UI 등 UI 미리 로드
            await SingletonTool.CreateAsyncSingleton(typeof(PrefabModelDataRoot));
            await SingletonTool.CreateAsyncSingleton(typeof(PrefabExtraDataRoot));
            await SingletonTool.CreateAsyncSingleton(typeof(ImageNameTableData));
            await SingletonTool.CreateAsyncSingleton(typeof(ProjectorSpawnData));
            await MainGameSceneEnvironmentResourceData.GetInstance();
            await UniTask.SwitchToMainThread();
            MainGameSceneEnvironmentResourceData.GetInstanceUnSafe.ResourcePreLoad();
            
            _EventReceiver =
                PlayerManager.GetInstance.GetEventReceiver<PlayerChangeEventReceiver>(PlayerManager.PlayerChangeEventType.PlayerChanged, OnPlayerChanged);
            
            SceneVariableChangeEventReceiver =
                SceneEnvironmentManager.GetInstance
                    .GetEventReceiver<SceneVariableChangeEventReceiver>(
                        SceneEnvironmentManager.SceneVariableEventType.OnSceneVariableChanged, OnMapVariableChanged);

            // OnAwakeTargetingDecal();
            OnAwakeTouchSelectTarget();
            Priority = 200;
        }

        public override async UniTask OnInitiate()
        {
            await UniTask.CompletedTask;
        }

        ///마을 환경음
        public void OnEffectSoundVillageAmbienceOn()
        {
            MapEffectSoundManager.GetInstance.GetMapEffectUnit(MapEffectSoundManager.VillageAmbience, MapEffectSoundManager.GetInstance._MapEffectObjectWrapper, MapEffectSoundManager.MapEffectSound.Environment);
        }

        public void OnEffectSoundVillageAmbienceOff()
        {
            MapEffectSoundManager.GetInstance.GetMapEffectUnit(CurrentSceneEffectSoundIndex, MapEffectSoundManager.GetInstance._MapEffectObjectWrapper, MapEffectSoundManager.MapEffectSound.Environment);
        }
        public void OnEffectSoundVillageWallaOn()
        {
            if(Walla.IsValid())
            {
                Walla.SetPlay(0);
            }
            else
            {
                Walla = MapEffectSoundManager.GetInstance.GetMapEffectUnit(MapEffectSoundManager.VillageWalla, MapEffectSoundManager.GetInstance._MapEffectObjectWrapper, MapEffectSoundManager.MapEffectSound.None).Item2;
            }
        }

        public void OnEffectSoundVillageWallaOff()
        {
            if(Walla.IsValid()) Walla.SetStop();
        }

        private void OnMapVariableChanged(SceneEnvironmentManager.SceneVariableEventType p_EventType,
            SceneVariableData.TableRecord p_Record)
        {
            CurrentSceneTextureType = p_Record.TextureType;
            CurrentSceneEffectSoundIndex = p_Record.EffectSoundIndex;
        }

        private void OnPlayerChanged(PlayerManager.PlayerChangeEventType p_EventType, Unit p_Changed)
        {
            _ClientPlayer = p_Changed as LamiereUnit;
            
            if (_ClientPlayer.IsValid())
            {
             //   isClearCommand = false;
#if UNITY_EDITOR
                if (CustomDebug.AIStateName)
                {
                }
                else
                {
                    _ClientPlayer.AddState(Unit.UnitStateType.NETWORK_DEAD);
                }
#else
                _ClientPlayer.SetUnitName(PlayerName);
                _ClientPlayer.AddState(Unit.UnitStateType.NETWORK_DEAD);
#endif

                if (!isFirstDelete)
                {
                    //MainGameUI.Instance.mainUI.InitActionImage(true);
                    isFirstDelete = true;
                }
                
                _ClientPlayer.SwitchActable(UnitActionDataRoot.ActableType.UnWeaponPhaseTransition);

                (_ClientPlayer._PhysicsObject as ControlCharacterController).SetColliderEnable(true);
                // _MaxEffectSpeed = _ClientPlayer.GetScaledMovementSpeed();
                
                _ClientPlayer.RemoveState(Unit.UnitStateType.SILENCE);


            }

            _mapStartPosition = Vector3.zero;
        }


        /// momo6346 - state에 따른 게임매니저의 커맨드 변수를 리턴합니다.
        public ControllerTool.CommandType GetMainSkillCommand(int state)
        {
            switch (state)
            {
                case 0: return buttonZ;
                case 1: return buttonV;
                case 2: return buttonB;
                case 3: return buttonCtrl;
                default: return buttonCtrl;
            }
        }

        public void OnUpdate(float p_DeltaTime)
        {
            //OnUpdate_AutoPlay(p_DeltaTime);
        }

        public override async UniTask OnScenePreload()
        {
            await UniTask.CompletedTask;
        }

        public override void OnSceneStarted()
        {
            if (IsQuest)
            {
                var _SpawnIntervalTimer = GameEventTimerHandlerManager.GetInstance.SpawnEventTimerHandler(SystemBoot.TimerType.FixedGameTimer, false);
              
                _SpawnIntervalTimer
                    .AddEvent(
                        2000,
                        handler =>
                        {
                            handler.Arg1.QuestMove = true;
                            return true;
                        },
                        null, this);
                _SpawnIntervalTimer.StartEvent();
            }
        }

        public override void OnSceneTerminated()
        {
            // SetEnemyTargetingDecalDisable();
            Walla = default;
            //씬 전환 시 펫스킬 회수
            //MainGameUI.Instance.mainUI.Relese();
        }
        
        public override void OnSceneTransition()
        {
        }

        #endregion

        #region <Methods>

        public void SelectPlayer(Vocation p_PlayerType)
        {
            PlayerModelDataIndex = (int)p_PlayerType;
        }

        public void DeployPlayer(Vocation p_PlayerType)
        {
            RemovePlayer();
            SelectPlayer(p_PlayerType);
            DeployPlayer();
        }
        
        public void DeployPlayer()
        {
            var (t_Pos, t_Rot) = SceneEnvironmentManager.GetInstance.GetPlayerStartPreset();
            
            if (PlayerModelDataIndex != default && _ClientPlayer == null)
            {
                UnitSpawnManager.GetInstance.SpawnUnit<LamiereUnit>(PlayerModelDataIndex, t_Pos, ResourceLifeCycleType.Scene, ObjectDeployTool.ObjectDeploySurfaceDeployType.ForceSurface);
                _ClientPlayer.SetLook(t_Rot);
            }
            else
            {
                _ClientPlayer.SetPivotPosition(t_Pos, true, true);
                _ClientPlayer.SetLook(t_Rot);
            }
        }

        public void HidePlayer(bool p_HideFlag)
        {
            if (_ClientPlayer != null)
            {
                _ClientPlayer.SetUnitDisable(p_HideFlag);
            }
        }
        
        public void RemovePlayer()
        {
            if (_ClientPlayer != null)
            {
                _ClientPlayer.RetrieveObject();
                _ClientPlayer = null;
            }
        }

        #endregion

        #region <Convert>
        public static Vocation GetConvertVocation(int vocation)
        {
            return vocation > 9 ? (Vocation)(vocation - 9) : (Vocation)vocation;
        }
        #endregion

        /// <summary>
        /// 능력 설명란 표시
        /// </summary>
        string PrintStatusExplanation(string type, float _value, bool p_AddFlag, bool p_Percent){
            string print = type;
            if(p_AddFlag)
            {
                var isMinus = (_value < 0);
                float valueSetting = (isMinus ? _value * (-1) : _value);
                print += (p_Percent ? $" {valueSetting * 100}%" : $" {valueSetting}");
                print += (isMinus ? " 감소" : " 증가");
            }
            else
            {
                var isMinus = (_value < 0);

                print += isMinus ? (p_Percent ? $" {_value * 100}%" : $" {_value}") : (p_Percent ? $" +{_value * 100}%" : $" +{_value}");
            }
            
            return print;
        }
    }
}
#endif