using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BlackAm
{
    public partial class DialogueGameManager : SceneChangeEventSingleton<DialogueGameManager>
    {
        #region <Fields>

        /// <summary>
        /// 해당 유닛에 포함된 모듈 그룹
        /// </summary>
        private List<IDialogueModuleCluster> _ModuleList;
        
        /// <summary>
        /// 해당 유닛에 포함된 모듈 숫자
        /// </summary>
        private int _ModuleCount;

        private GameEventTimerHandlerWrapper _SpawnIntervalTimer;
        
        private SceneVariableChangeEventReceiver SceneVariableChangeEventReceiver;

        #endregion
        
        #region <CallBacks>

        protected override void OnCreated()
        {
            currentDialogueEventData = new SaveLoadManager.SaveData.DialogueEventData(0, false, new Dictionary<Character, int>());

            OnCreatedLiking();
            OnCreatedKeyInput();
            OnCreatedCharacterImage();
            
            _SpawnIntervalTimer = GameEventTimerHandlerManager.GetInstance.SpawnEventTimerHandler(SystemBoot.TimerType.FixedGameTimer, false);

            SceneVariableChangeEventReceiver =
                SceneEnvironmentManager.GetInstance
                    .GetEventReceiver<SceneVariableChangeEventReceiver>(
                        SceneEnvironmentManager.SceneVariableEventType.OnSceneVariableChanged, OnMapVariableChanged);
        }

        public override void OnInitiate()
        {
            _ModuleList = new List<IDialogueModuleCluster>();

            OnInitiateDialogueEvent();
            OnInitiatePlayMode();

            _PlayMode = DialoguePlayMode.BasicPlay;
        }

        public override async UniTask OnScenePreload()
        {
            await UniTask.CompletedTask;
            
            SetDialogueEventEnd(true);
            isDialogueEnd = true;
            isDialogueSkipable = false;
            currentDialogueEventData.FadeActivated = false;
        }

        public override void OnSceneStarted()
        {
            NotifyDialogueModule();
        }

        public override void OnSceneTerminated()
        {
            SleepDialogueModule();
        }

        public override void OnSceneTransition()
        {
        }

        public void OnUpdate(float p_DeltaTime)
        {
            for (int i = 0; i < _ModuleCount; i++)
            {
                _ModuleList[i].CurrentSelectedModule.OnUpdate(p_DeltaTime);
            }
        }
        
        private void OnMapVariableChanged(SceneEnvironmentManager.SceneVariableEventType p_EventType, SceneVariableData.TableRecord p_Record)
        {
            if (ReferenceEquals(null, _KeyInputModule))
            {
                LoadKeyInput(p_Record.SceneKeyInputIndex);
            }
            else
            {
                SwitchInputAction(p_Record.SceneKeyInputIndex.First());
            }
        }

        #endregion

        #region <Methods>

        protected virtual void NotifyDialogueModule()
        {
            for (int i = 0; i < _ModuleCount; i++)
            {
                _ModuleList[i].CurrentSelectedModule.TryModuleNotify();
            }
        }

        protected virtual void SleepDialogueModule()
        {
            for (int i = 0; i < _ModuleCount; i++)
            {
                _ModuleList[i].CurrentSelectedModule.TryModuleSleep();
            }
        }

        public int AddModule(IDialogueModuleCluster p_Module)
        {
            _ModuleList.Add(p_Module);
            _ModuleCount = _ModuleList.Count;
            return _ModuleCount - 1;
        }

        public void RemoveModule(int p_TargetIndex)
        {
            var lastIndex = _ModuleCount - 1;
            if (p_TargetIndex > -1)
            {
                _ModuleList.Swap(p_TargetIndex, lastIndex);
                _ModuleList[p_TargetIndex].SetModuleHandleIndex(p_TargetIndex);
                _ModuleList[lastIndex].SetModuleHandleIndex(-1);
                _ModuleList.RemoveAt(lastIndex);
                _ModuleCount--;
            }
        }

        #endregion

    }
}