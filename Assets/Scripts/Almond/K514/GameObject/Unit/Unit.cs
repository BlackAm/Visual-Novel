using System;
using System.Collections.Generic;
using UnityEngine;

namespace k514
{
    public abstract partial class Unit : FocusableInstance
    {
        #region <Fields>

        /// <summary>
        /// 해당 유닛 프리팹의 추가 데이터
        /// </summary>
        public UnitExtraDataRecordBridge _PrefabExtraDataRecord;

        /// <summary>
        /// 해당 유닛이 프리팹 모델 테이블을 참조하여 활성화되는 경우, 해당 키를 저장하는 튜플
        /// </summary>
        public (bool, int) _PrefabModelKey;
        
        /// <summary>
        /// 해당 유닛에 포함된 모듈 그룹
        /// </summary>
        private List<IUnitModuleCluster> _ModuleList;

        /// <summary>
        /// 해당 유닛이 플레이어인지 표시하는 플래그
        /// </summary>
        public bool IsPlayer => HasAnyAuthority(UnitTool.UnitAuthorityFlag.Player);

#if !SERVER_DRIVE
        /// <summary>
        /// 해당 유닛이 카메라 포커스 대상인지 표시하는 플래그
        /// </summary>
        public bool IsFocused => CameraManager.GetInstanceUnSafe.IsTracingTarget(this);
#endif
        
        /// <summary>
        /// 해당 유닛에 포함된 모듈 숫자
        /// </summary>
        private int _ModuleCount;
        
        #endregion
        
        #region <Callbacks>

        public override void OnSpawning()
        {
            base.OnSpawning();

            InitPrefabExtraData();

            OnAwakeState();
            
            OnAwakeAttachPoint(); 
            OnAwakeAnimation();
            OnAwakeActable();
            
            OnAwakeEventHandler();
            OnAwakeStatusInfo();

            OnAwakeUnitTimerEvent();
            OnAwakeAuthority();

            OnAwakePhysics();
            OnAwakeAI();
            
            OnAwakeRole();
            
            OnAwakeTrial();
        }

        protected virtual void InitPrefabExtraData()
        {
            _ModuleList = new List<IUnitModuleCluster>();
            DeployableType = ObjectDeployTool.DeployableType.Unit;
            _PrefabExtraDataRecord = (UnitExtraDataRecordBridge) _PrefabKey._PrefabExtraPreset._PrefabExtraDataRecord;
            _PrefabModelKey = _PrefabKey._PrefabExtraPreset.PrefabModelKey;
        }
        
        public override void OnPooling()
        {
            base.OnPooling();

            OnPoolingState();
            
            OnPoolingAttachPoint();
            OnPoolingAnimation();
            OnPoolingActable();
            
            OnPoolingEventHandler();
            OnPoolingStatusInfo();
            
            OnPoolingAuthority();

            OnPoolingUnitEvent();

            OnPoolingPhysics();
            OnPoolingAI();
            
            OnPoolingRole();
            
            OnPoolingTrial();
        }

        public override void OnRetrieved()
        {
            base.OnRetrieved();

            if (EventHandlerValid)
            {
                _UnitEventHandler.OnEventTriggered(UnitEventHandlerTool.UnitEventType.UnitRetrieved, new UnitEventMessage(this));
            }
            
#if UNITY_EDITOR
            if (CustomDebug.AIStateName)
            {
                SetUnitNameWithTail($"(Retrieved)");
            }
#endif 
            
            OnRetrieveState();
            
            OnRetrieveAttachPoint();
            OnRetrieveAnimation();
            OnRetrieveActable();
            
            OnRetrieveEventHandler();
            OnRetrieveStatusInfo();       
      
            OnRetrieveUnitTimerEvent();
            OnRetrieveAuthority();
            OnRetrievePhysics();
            OnRetrieveAI();
            
            OnRetrieveRole();
            
            OnRetrieveTrial();
        }
        
        public override void OnPositionTransition(Vector3 p_TransitionTo)
        {
            base.OnPositionTransition(p_TransitionTo);

            if (EventHandlerValid)
            {
                // UnitInteractManager에 의해 해당 Unit이 거리에 의한 SystemDisable 상태가 될 수 있으므로,
                // 거리 이벤트를 갱신하기 위한 콜백을 호출해준다.
                TryUpdateSqrDistanceTable_And_ReportPositionChangeDetected();
            }
        }
        
        public void OnPreUpdate(float p_DeltaTime)
        {
            for (int i = 0; i < _ModuleCount; i++)
            {
                _ModuleList[i].CurrentSelectedModule.OnPreUpdate(p_DeltaTime);
            }
        }

        /// <summary>
        /// 매 프레임당 호출되는 콜백
        /// </summary>
        public void OnUpdate(float p_DeltaTime)
        {
            for (int i = 0; i < _ModuleCount; i++)
            {
                _ModuleList[i].CurrentSelectedModule.OnUpdate(p_DeltaTime);
            }
        }

        /// <summary>
        /// UnitInteractManager 로부터 일정 주기(0.1초)로 호출되는 콜백
        /// </summary>
        public void OnUpdate_TimeBlock()
        {
            for (int i = 0; i < _ModuleCount; i++)
            {
                _ModuleList[i].CurrentSelectedModule.OnUpdate_TimeBlock();
            }

            OnUpdateRegenStatus();
        }
        
        /// <summary>
        /// 인스턴스가 파기될 때 수행할 작업을 기술한다.
        /// </summary>
        protected override void DisposeUnManaged()
        {
            DisposeActable();
            DisposeAnimation();
            DisposePersona();
            DisposePhysics();
            DisposeRole();
            
            base.DisposeUnManaged();
        }

        #endregion
        
        #region <Methods>
        
        protected virtual void NotifyIncarnateModule()
        {
            if (!HasState_Or(UnitStateType.DEAD))
            {
                for (int i = 0; i < _ModuleCount; i++)
                {
                    _ModuleList[i].CurrentSelectedModule.TryModuleNotify();
                }
            }
        }

        protected virtual void SleepIncarnateModule()
        {
            if (!HasState_Or(UnitStateType.DEAD))
            {
                for (int i = 0; i < _ModuleCount; i++)
                {
                    _ModuleList[i].CurrentSelectedModule.TryModuleSleep();
                }
            }
        }

        public override void SetPivotPosition(Vector3 p_TargetPos, bool p_SyncPosition, bool p_ForceSurface)
        {
            base.SetPivotPosition(p_TargetPos, p_SyncPosition, p_ForceSurface);

            // SystemDisable 상태에서는 유닛 모듈들이 콜백을 수행할 필요가 없다.
            if (!HasState_Or(UnitStateType.SystemDisable))
            {
                for (int i = 0; i < _ModuleCount; i++)
                {
                    _ModuleList[i].CurrentSelectedModule.OnPivotChanged(_PositionState, p_SyncPosition);
                }
            }
        }
        
        /// <summary>
        /// 해당 유닛의 '시스템 사용불가' 상태를 지정하는 메서드
        /// </summary>
        public void SetUnitDisable(bool p_Flag)
        {
            if (HasState_Or(UnitStateType.SystemDisable) != p_Flag)
            {
                if (p_Flag)
                {
#if UNITY_EDITOR
                    if (CustomDebug.AIStateName)
                    {
                        SetUnitNameWithTail($"(SystemDisabled)");
                    }
#endif
                    if (EventHandlerValid)
                    {
                        _UnitEventHandler.OnEventTriggered(UnitEventHandlerTool.UnitEventType.UnitDisabled, new UnitEventMessage(this, true));
                    }
                    
                    AddState(UnitStateType.SystemDisable);
                    gameObject.SetActiveSafe(false);
                    SleepIncarnateModule();
                }
                else
                {
                    if (EventHandlerValid)
                    {
                        _UnitEventHandler.OnEventTriggered(UnitEventHandlerTool.UnitEventType.UnitDisabled, new UnitEventMessage(this, false));
                    }
                    RemoveState(UnitStateType.SystemDisable);
                    gameObject.SetActiveSafe(true);
                    NotifyIncarnateModule();

                    if (!HasState_Or(UnitStateType.SystemDisable))
                    {
                        OnCheckOverlapObject();
                    }
                }
            }
        }

        public int AddModule(IUnitModuleCluster p_Module)
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