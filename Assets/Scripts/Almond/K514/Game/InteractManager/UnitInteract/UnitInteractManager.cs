using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace k514
{
    /// <summary>
    /// 유닛이 포함된 오브젝트 간의 상호작용을 기술/제어하는 매니저 클래스
    /// </summary>
    public partial class UnitInteractManager : SceneChangeEventSingleton<UnitInteractManager>
    {
        #region <Consts>

        /// <summary>
        /// 정지마찰 상한값, 해당 값 미만으로 떨어진 힘 벡터의 원소는 0으로 수렴한다.
        /// </summary>
        public const float __VelocityElementLowerBound = 0.6f;
        
        /// <summary>
        /// 감쇄 배율, 중력 벡터를 제외한 모든 힘 벡터는 프레임 당, 감쇄 배율 만큼의 약해진다.
        /// </summary>
        public const float __DampenRate = 0.96f;
        
        /// <summary>
        /// 중력가속도
        /// </summary>
        public const float __MainGravity = 9.8f * 6.66f;
        
        /// <summary>
        /// 기본 중력 적용비
        /// </summary>
        public const float __DefaultGravityRate = 1f;

        /// <summary>
        /// 타임 블록 주기
        /// </summary>
        public const float __TimeBlock_Interval = 0.1f;
        
        /// <summary>
        /// 초기화 작업에 사용할 기저 유닛 숫자
        /// </summary>
        private const int __DefaultUnitCapacity = 16;
        
        #endregion
        
        #region <Fields>

        /// <summary>
        /// 유닛 포커스 키 셋
        /// </summary>
        private PermutationKey _KeySet;
                
        /// <summary>
        /// 이벤트 핸들러 interactId로 구분되는, UnitEventHandler
        /// </summary>
        private UnitEventHandler[] _UnitHandlerTable;

        /// <summary>
        /// 중력 벡터, GravityFactor 프로퍼티에 의해 값이 정해진다.
        /// </summary>
        public Vector3 GravityVector { get; private set; }
        
        /// <summary>
        /// GravityFactor 프로퍼티 백킹 필드
        /// </summary>
        private float __GravityFactor;
        
        /// <summary>
        /// 중력 프로퍼티
        /// </summary>
        public float GravityFactor
        {
            get => __GravityFactor;
            private set
            {
                __GravityFactor = value;
                GravityVector = __GravityFactor * __MainGravity * Vector3.down;
            }
        }

        /// <summary>
        /// 이벤트 핸들러 전체를 제어하는 핸들러 오브젝트 풀러
        /// 유닛들은 서로 다른 풀러에 의해 생성 될 수 있으므로, 하나의 풀러를 통해 다룰 수 없다.
        /// 따라서, UnitEvnetHandler를 통해 일괄 제어한다.
        /// </summary>
        private ObjectPooler<UnitEventHandler> UnitEventHandlerPool;

        /// <summary>
        /// 일정주기로 콜백을 호출하는 타이머 단위 오브젝트
        /// </summary>
        private ProgressTimerChainObject _TimeBlock;

#if !SERVER_DRIVE
        /// <summary>
        /// 플레이어 변경 이벤트 수신자
        /// </summary>
        private PlayerChangeEventReceiver _PlayerChangeEventReceiver;
#endif

        /// <summary>
        /// 업데이트 로직 처리과정에서 사망판정이 발생한 유닛들이 임시로 등록되는 버퍼
        /// </summary>
        private List<Unit> _ReservedDeadGroup;

        /// <summary>
        /// _ReservedDeadGroup에 예약 유닛이 있음을 표시하는 플래그
        /// </summary>
        private bool _DeadReservedFlag;
        
        #endregion

        #region <Enums>

        public enum EnvironmentClearType
        {
            Retrieve,
            SetDead,
            SetDeadInstant,
        }

        #endregion
        
        #region <Callbacks>

        protected override void OnCreated()
        {
            _KeySet = new PermutationKey(__MAX_INTERACT_UNIT_NUMBER);
            _UnitHandlerTable = new UnitEventHandler[__MAX_INTERACT_UNIT_NUMBER];

            UnitEventHandlerPool = new ObjectPooler<UnitEventHandler>();
            UnitEventHandlerPool.PreloadPool(__DefaultUnitCapacity, __DefaultUnitCapacity);
            
            _TimeBlock = new ProgressTimerChainObject(ProgressTimerChainObject.ProgressTimerChainTerminateType.Loop);
            _TimeBlock.AddTimer(__TimeBlock_Interval);

#if !SERVER_DRIVE
            _PlayerChangeEventReceiver = 
                PlayerManager.GetInstance.GetEventReceiver<PlayerChangeEventReceiver>(PlayerManager.PlayerChangeEventType.PlayerChanged, OnPlayerChanged);
#endif
            _ReservedDeadGroup = new List<Unit>();
            
            OnCreated_UnitInteract();
            OnCreated_UnitFilter();
            OnCreated_InstanceId();
        }

        public override void OnInitiate()
        {
            GravityFactor = __DefaultGravityRate;
        }

        public void OnUpdate(float p_DeltaTime)
        {
            var timerMessage = _TimeBlock.Progress(p_DeltaTime);
            var blockTerminated = timerMessage == TaskPhase.TaskTerminate;
            var handlerIterator = UnitEventHandlerPool.ActivedObjectPool;
            foreach (var handler in handlerIterator)
            {
                var targetUnit = handler._ThisUnit;
                if(targetUnit.HasState_Or(Unit.UnitStateType.SystemDisable)) continue;
                
                targetUnit.OnPreUpdate(p_DeltaTime);
                targetUnit.OnUpdate(p_DeltaTime);
                
                if (blockTerminated)
                {
                    targetUnit.OnUpdate_TimeBlock();
                }   
            }

            // 물리모듈을 포함한 모든 유닛 로직 종료 이후에, 유닛 상태를 갱신시켜준다.
            OnUpdateUnitState(p_DeltaTime);
        }

        public void OnUpdateUnitState(float p_DeltaTime)
        {
            var handlerIterator = UnitEventHandlerPool.ActivedObjectPool;
            foreach (var handler in handlerIterator)
            {
                var targetUnit = handler._ThisUnit;
                if(targetUnit.HasState_Or(Unit.UnitStateType.SystemDisable)) continue;
                
                targetUnit.CheckUnitPositionChanged();
            }
        }

        public void OnLateUpdate(float p_DeltaTime)
        {
            var handlerIterator = UnitEventHandlerPool.ActivedObjectPool;
            foreach (var handler in handlerIterator)
            {
                var targetUnit = handler._ThisUnit;
                if(targetUnit.HasState_Or(Unit.UnitStateType.SystemDisable)) continue;
                
                targetUnit.OnLateUpdate(p_DeltaTime);
            }

            OnHandleReservedDead();
        }

        public void OnFixedUpdate(float p_DeltaTime)
        {
            var handlerIterator = UnitEventHandlerPool.ActivedObjectPool;
            foreach (var handler in handlerIterator)
            {
                var targetUnit = handler._ThisUnit;
                if(targetUnit.HasState_Or(Unit.UnitStateType.SystemDisable)) continue;
                
                targetUnit.OnFixedUpdate(p_DeltaTime);
            }
        }

        private void OnHandleReservedDead()
        {
            if (_DeadReservedFlag)
            {
                _DeadReservedFlag = false;
                foreach (var targetUnit in _ReservedDeadGroup)
                {
                    targetUnit.SetDead(false);
                }
                _ReservedDeadGroup.Clear();
            }
        }

        public override async UniTask OnScenePreload()
        {
            await UniTask.CompletedTask;
        }

        public override void OnSceneStarted()
        {
            var handlerIterator = UnitEventHandlerPool.ActivedObjectPool;
            foreach (var handler in handlerIterator)
            {
                var targetUnit = handler._ThisUnit;
                targetUnit.SetUnitDisable(false);
            }
        }

        public override void OnSceneTerminated()
        {
        }
        
        public override void OnSceneTransition()
        {
            var handlerIterator = UnitEventHandlerPool.ActivedObjectPool;
            foreach (var handler in handlerIterator)
            {
                var targetUnit = handler._ThisUnit;
                targetUnit.SetUnitDisable(true);
            }
        }

#if !SERVER_DRIVE
        private void OnPlayerChanged(PlayerManager.PlayerChangeEventType p_EventType, FocusableInstance p_TargetTransform)
        {
            var activeHandlerList = UnitEventHandlerPool.ActivedObjectPool;
            var activeCount = activeHandlerList.Count;

            for (int i = activeCount - 1; i > -1; i--)
            {
                var targetUnit = activeHandlerList[i]._ThisUnit;
                if (!targetUnit.IsPlayer)
                {
                    // targetUnit.OnUpdateUnitLayer();
                }
            }
        }
#endif

        /// <summary>
        /// 인스턴스가 파기될 때 수행할 작업을 기술한다.
        /// </summary>
        protected override void DisposeUnManaged()
        {
            DestroyAllUnit(EnvironmentClearType.Retrieve);
            
            base.DisposeUnManaged();
        }

        #endregion

        #region <Methods>

        private UnitEventHandler PopUnitHandler(Unit p_TargetUnit)
        {
            var spawned = UnitEventHandlerPool.GenerateObject();
            var interactKey = _KeySet.GetValidKey();
            spawned.Item2.SetUnit(interactKey, p_TargetUnit);
            return UnitEventHandlerPool.InitObject(spawned);
        }

        public UnitEventHandler AddUnit(Unit p_TargetUnit)
        {
            var spawned = PopUnitHandler(p_TargetUnit);
            var interactKey = spawned._InteractId;
            _UnitHandlerTable[interactKey] = spawned;
            
            for (int i = 0; i < __MAX_INTERACT_UNIT_NUMBER; i++)
            {
                _DistanceSqrTable.SetElement_SafeComplete(interactKey, i, __Out_Of_Range_Sqr);

                var (valid, lower, upper) = _InteractStateTable.TryGetIndexSequence(interactKey, i);
                if (valid)
                {
                    var tryInteractState = _InteractStateTable.GetElement(lower, upper);
                    if(ReferenceEquals(null, tryInteractState))
                    {
                        _InteractStateTable.SetElement(lower, upper, new UnitInteractStatePreset());
                    }
                    else
                    {
                        tryInteractState.Reset();
                    }
                }
            }
            
            spawned.AddReceiver(_UnitDistanceEventReceiver);
            AddInstanceId(p_TargetUnit);
            
            return spawned;
        }

        public void RemoveUnit(UnitEventHandler p_UnitEventHandler)
        {
            var interactKey = p_UnitEventHandler._InteractId;
            var tryUnit = p_UnitEventHandler._ThisUnit;
            _UnitHandlerTable[interactKey] = null;
            _KeySet.ReturnKey(interactKey);
            RemoveInstanceId(tryUnit);
        }

        /// <summary>
        /// 현재 UnitInteractManager에 바인드된 플레이어 외의 유닛을 전부 파괴한다.
        /// </summary>
        public void DestroyAllUnitExceptPlayer(EnvironmentClearType p_Type)
        {
            var activeHandlerList = UnitEventHandlerPool.ActivedObjectPool;
            var activeCount = activeHandlerList.Count;

            for (int i = activeCount - 1; i > -1; i--)
            {
                var targetUnit = activeHandlerList[i]._ThisUnit;
                if (!targetUnit.IsPlayer)
                {
                    switch (p_Type)
                    {
                        case EnvironmentClearType.Retrieve:
                            targetUnit.RetrieveObject();
                            break;
                        /*case EnvironmentClearType.SetDead:
                            targetUnit.SetDead(false, true);
                            break;
                        case EnvironmentClearType.SetDeadInstant:
                            targetUnit.SetDead(true, true);
                            break;*/
                    }
                }
            }
        }
        
        /// <summary>
        /// 현재 UnitInteractManager에 바인드된 유닛을 전부 파괴한다.
        /// </summary>
        public void DestroyAllUnit(EnvironmentClearType p_Type)
        {
            var activeHandlerList = UnitEventHandlerPool.ActivedObjectPool;
            var activeCount = activeHandlerList.Count;

            for (int i = activeCount - 1; i > -1; i--)
            {
                var targetUnit = activeHandlerList[i]._ThisUnit;
                switch (p_Type)
                {
                    case EnvironmentClearType.Retrieve:
                        targetUnit.RetrieveObject();
                        break;
                    /*case EnvironmentClearType.SetDead:
                        targetUnit.SetDead(false, true);
                        break;
                    case EnvironmentClearType.SetDeadInstant:
                        targetUnit.SetDead(true, true);
                        break;*/
                }
            }
        }

        public void ReserveDead(Unit p_Unit)
        {
            if (!p_Unit.HasState_Or(Unit.UnitStateType.WaitForDead))
            {
                p_Unit.AddState(Unit.UnitStateType.WaitForDead);
                _ReservedDeadGroup.Add(p_Unit);
                _DeadReservedFlag = true;
            }
        }
        
        public int GetUnitNumber()
        {
            return UnitEventHandlerPool.ActivedObjectPool.Count;
        }
        
        public string GetUnitNumberInfo()
        {
            return $"{GetUnitNumber()} / {__MAX_INTERACT_UNIT_NUMBER}";
        }

        #endregion
    }
}