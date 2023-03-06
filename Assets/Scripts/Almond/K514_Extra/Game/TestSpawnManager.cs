#if!SERVER_DRIVE
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = System.Random;

namespace k514
{
    /// <summary>
    /// Test
    /// </summary>
    /*public class TestSpawnManager : SceneChangeEventSingleton<TestSpawnManager>
    {
        #region <Consts>

        /// <summary>
        /// 한 세력의 스폰 숫자
        /// </summary>
        private const int UnitCount = 40;

        /// <summary>
        /// 봇 유닛이 스폰된 랜덤한 위치 스케일
        /// </summary>
        private const float RandomSpawnPositionScale = 5f;
        
        #endregion
        
        #region <Fields>

        private Dictionary<UnitTool.UnitGroupType, int> _BotCount;
        private UnitEventReceiver _DeadReceiver;
        private UIBaseManagerPreset _UI_DebugPanel_Preset;
        private UI_DebugPanel _UI_DebugPanel;
        private bool _Valid;
        private GameEventTimerHandlerWrapper _SpawnIntervalTimer;
        private ControllerEventReceiver _EventReceiver;
        
        #endregion

        #region <Callbacks>

        protected override void OnCreated()
        {
            _EventReceiver =
                ControllerTool.GetInstanceUnSafe
                    .GetControllerEventSender(ControllerTool.InputEventType.SystemControl)
                    .GetEventReceiver<ControllerEventReceiver>(ControllerTool.InputEventType.SystemControl,
                        OnControllerEventTriggered);
            
            _BotCount = new Dictionary<UnitTool.UnitGroupType, int>();
            _BotCount.Add(UnitTool.UnitGroupType.Bot, 0);
            _BotCount.Add(UnitTool.UnitGroupType.Bot2, 0);
            
            /*_UI_DebugPanel_Preset = UICustomRoot.GetInstanceUnSafe.Add_UI_Manager(RenderMode.ScreenSpaceOverlay, 99,
                UICustomRoot.UIManagerType.TestDebugPanel);
                
            _UI_DebugPanel = _UI_DebugPanel_Preset.UIManagerBase as UI_DebugPanel;
            _UI_DebugPanel.Set_UI_Hide(true);
            _UI_DebugPanel.SetFirstContentTitle("BotA");
            _UI_DebugPanel.SetSecondContentTitle("BotB");
            OnUnitCountChanged(Unit.UnitAllyFlagType.Bot);
            OnUnitCountChanged(Unit.UnitAllyFlagType.Bot2);#1#
            
            _DeadReceiver = new UnitEventReceiver(UnitEventHandlerTool.UnitEventType.UnitDead, OnBotUnitEventTriggered);
        }

        public override void OnInitiate()
        {

        }

        public override async UniTask OnScenePreload()
        {
            await UniTask.CompletedTask;
            
            _SpawnIntervalTimer = GameEventTimerHandlerManager.GetInstance.SpawnEventTimerHandler(SystemBoot.TimerType.GameTimer, false);
            _SpawnIntervalTimer
                .AddEvent(
                    (0, 1000),
                    handler =>
                    {
                        var botSpawnRandSeed = 5;
                        for (int i = 0; i < botSpawnRandSeed; i++)
                        {
                            GetInstance.SpawnUnit(UnitTool.UnitGroupType.Bot, UnitTool.UnitGroupType.Bot2 | UnitTool.UnitGroupType.Monster, UnityEngine.Random.Range(11, 14));
                        }
                        var bot2SpawnRandSeed = 5;
                        for (int i = 0; i < bot2SpawnRandSeed; i++)
                        {
                            GetInstance.SpawnUnit(UnitTool.UnitGroupType.Bot2, UnitTool.UnitGroupType.Bot | UnitTool.UnitGroupType.Monster, UnityEngine.Random.Range(11, 14));
                        }
                        return true;
                    });

            _SpawnIntervalTimer.StartEvent();
        }

        public override void OnSceneStarted()
        {
        }

        public override void OnSceneTerminated()
        {
            SetEnableAutoSpawn(false);
            /*_BotCount[Unit.UnitAllyFlagType.Bot] = 0;
            _BotCount[Unit.UnitAllyFlagType.Bot2] = 0;
            OnUnitCountChanged(Unit.UnitAllyFlagType.Bot);
            OnUnitCountChanged(Unit.UnitAllyFlagType.Bot2);#1#
            LamiereUnit.UnitNameID = 1;
        }

        public override void OnSceneTransition()
        {
        }

        private void OnUnitCountChanged(UnitTool.UnitGroupType p_GroupType)
        {
            switch (p_GroupType)
            {
                case UnitTool.UnitGroupType.Bot:
                    _UI_DebugPanel.SetFirstContentValue(_BotCount[p_GroupType].ToString());
                    break;
                case UnitTool.UnitGroupType.Bot2:
                    _UI_DebugPanel.SetSecondContentValue(_BotCount[p_GroupType].ToString());
                    break;
            }
        }

        private void OnBotUnitEventTriggered(UnitEventHandlerTool.UnitEventType p_EventType, UnitEventMessage p_EventHandler)
        {
            var triggerUnit = p_EventHandler.TriggerUnit;
            if (triggerUnit.AllyMask.HasAnyFlagExceptNone(UnitTool.UnitGroupType.Bot | UnitTool.UnitGroupType.Bot2))
            {
                /*_BotCount[allyType._AllyMask]--;#1#
                var unitPooler = triggerUnit.PoolingContainer;
                var spawned = unitPooler.GetObject() as LamiereUnit;
                if (!(spawned is null))
                {
                    spawned._Transform.position = p_EventHandler.TriggerUnit._Transform.position;
                    spawned.SetUnitGroupMask(triggerUnit.AllyMask,
                        triggerUnit.AllyMask == UnitTool.UnitGroupType.Bot
                        ? UnitTool.UnitGroupType.Bot2
                        : UnitTool.UnitGroupType.Bot);
                    spawned._UnitEventHandler.AddReceiver(_DeadReceiver);
                }
            }
        }

        private void OnControllerEventTriggered(ControllerTool.InputEventType p_EventType, ControllerTool.ControlEventPreset p_EventPreset)
        {
            if (p_EventPreset.IsInputRelease)
            {
                var commandType = p_EventPreset.CommandType;
                if (commandType == ControllerTool.CommandType.Space)
                {
                    ToggleAutoSpawn();
                }
            }
        }

        #endregion

        #region <Methods>

        public void SetEnableAutoSpawn(bool p_Flag)
        {
            if (_Valid != p_Flag)
            {
                _Valid = p_Flag;
                /*_UI_DebugPanel.Set_UI_Hide(!_Valid);#1#
            }
        }

        public void ToggleAutoSpawn()
        {
            SetEnableAutoSpawn(!_Valid);
        }

        public async void SpawnUnit(UnitTool.UnitGroupType p_Ally, UnitTool.UnitGroupType p_Enemy, int p_PrefabSpawnIndex)
        {
            if (!_Valid) return;
            
            SpawnUnitManual(p_Ally, p_Enemy, p_PrefabSpawnIndex);
        }
        
        public async void SpawnUnitManual(UnitTool.UnitGroupType p_Ally, UnitTool.UnitGroupType p_Enemy, int p_PrefabSpawnIndex)
        {
            var player = PlayerManager.GetInstance.Player;
            var targetCount = _BotCount[p_Ally];
            if (player.IsValid() && targetCount < UnitCount)
            {
                var spawnPos = player._Transform.position + CustomMath.RandomSymmetricVector(CustomMath.XYZType.ZX, Mathf.Max(2f, 0.1f * RandomSpawnPositionScale), RandomSpawnPositionScale);
                var (valid, spawned) = await UnitSpawnManager.GetInstance.SpawnUnitAsync<LamiereUnit>(p_PrefabSpawnIndex, spawnPos, ResourceLifeCycleType.Scene, ObjectDeployTool.ObjectDeploySurfaceDeployType.ForceSurface);

                if (valid)
                {
                    spawned.SetUnitGroupMask(p_Ally, p_Enemy);
                    spawned._UnitEventHandler.AddReceiver(_DeadReceiver);

                    _BotCount[p_Ally]++;
                    /*OnUnitCountChanged(p_Ally);#1#
                }
            }
        }
        
        #endregion
    }*/
}
#endif