using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using Vector3 = UnityEngine.Vector3;
   
namespace k514
{
    public partial class ControlNavMesh : ControlCharacterController
    {
        #region <Consts>
   
        /// <summary>
        /// 네브메쉬 에이전트 감속도. 감속도가 충분히 크지 않다면, 정지 기능에도 곧바로 멈추지 않고
        /// 네브메쉬 표면 위를 미끄러지게 된다.
        /// </summary>
        private const float Deacceleration = 1_000_000f;
   
        /// <summary>
        /// 네브메쉬 에이전트 초기 속도
        /// </summary>
        private const float DefaultNavMeshAgentDefaultSpeed = 1f;
           
        /// <summary>
        /// 네브메쉬 에이전트의 회전속도
        /// </summary>
        private const float AngularVel = 540f;
   
        /// <summary>
        /// 경로 계산 제한시간 상한
        /// </summary>
        private const float PendingBreakInterval = 10f;
           
        /// <summary>
        /// 이동하다 유닛이 막힌 경우를 검증하는 주기
        /// </summary>
        private const float StuckCheckInterval = 2f;
                    
        /// <summary>
        /// 도달할 수 없는 경로가 리셋되는 주기
        /// </summary>
        private const float InvalidPathResetInterval = 1f;

        /// <summary>
        /// 경로 계산이 지체되는 경우, 관련 처리를 시작할 상한 시간 비율, PendingBreakInterval의 진행도 비율이다.
        /// </summary>
        private const float PendingDelayHandleUpperBoundProgressRate = 0.01f;

        /// <summary>
        /// 경로 계산 종료후 최초 이동까지 걸리는 선딜레이
        /// </summary>
        private const float PathStartFirstDelay = 0.1f;

        #endregion
           
        #region <Fields>

        /// <summary>
        /// 네브메쉬 에이전트 테이블 레코드
        /// </summary>
        private new NavMeshAgentPresetData.PhysicsTableRecord _PhysicsRecord;

        /// <summary>
        /// 네브메쉬 에이전트 고유 속도
        /// </summary>
        private float _NavMeshAgentDefaultSpeed;
   
        /// <summary>
        /// 네브 메쉬 에이전트의 활성화를 막는 키
        /// </summary>
        private bool _NavMeshAgentStateBlockKey;
           
        /// <summary>
        /// 네브메쉬 에이전트 = 길찾기 에이전트
        /// </summary>
        public NavMeshAgent _NavMeshAgent { get; private set; }
           
        /// <summary>
        /// 길찾기 장해물
        /// 해당 물리모듈의 길찾기 에이전트가 비활성화된 상태에서 활성화되어
        /// 다른 길찾기 모듈의 경로 방해를 수행하여, 겹치는 일이 없게 한다.
        /// </summary>
        public NavMeshObstacle _NavMeshObstacle { get; private set; }
   
        /// <summary>
        /// 현재 네브메쉬 에이전트의 목적지가 세트되어있는지 표시하는 플래그
        /// </summary>
        private bool _IsNavMeshAgentDestinationValid;
   
        /// <summary>
        /// 경로 계산 종료를 알리는 플래그
        /// </summary>
        private bool _IsPendingOver;
           
        /// <summary>
        /// 현재 네브메쉬 에이전트의 목적지
        /// </summary>
        private Vector3 _NavMeshDestination;
  
        /// <summary>
        /// 예약된 이동 목적지 리스트
        /// </summary>
        private List<Vector3> _ReservedNavMeshDestinationList;
           
        /// <summary>
        /// 물리 에이전트의 상태를 표시하는 플래그
        /// </summary>
        protected NavmeshAgentType _NavmeshAgentType;
   
        /// <summary>
        /// 네브메쉬 에이전트의 유효성을 표시하는 플래그
        /// </summary>
        public bool IsNavmeshAgentEnable => _NavmeshAgentType == NavmeshAgentType.Enable;
   
        /// <summary>
        /// 경로 계산 체크하는 타이머
        /// </summary>
        private ProgressTimer _PendingDeadlineCounter;
           
        /// <summary>
        /// 경로 계산 종료 이후 최초 이동까지 걸리는 선딜레이 타이머
        /// </summary>
        private ProgressTimer _PathFirstDelayCounter;
        
        /// <summary>
        /// 유닛 이동이 막혔는지 체크하는 타이머
        /// </summary>
        private ProgressTimer _StuckCheckCounter;
   
        /// <summary>
        /// 유효하지 않은 패스 이동 타이머
        /// </summary>
        private ProgressTimer _InvalidPathCounter;
           
        /// <summary>
        /// 유닛 이동이 막혔는지 체크하는 기준좌표
        /// </summary>
        private Vector3 _StuckCheckPivot;

        /// <summary>
        /// 길찾기 에이전트가 비활성화 상태에서 도달한 최신 좌표
        /// </summary>
        private Vector3 _LatestPassivePosition;

        /// <summary>
        /// 길찾기 에이전트 활성화 시, 원본 아핀객체와 위상차가 생기는데 그 값을 기록하는 필드
        /// </summary>
        private Vector3 _LatestPassiveCorrectVector;
        
        /// <summary>
        /// 길찾기 에이전트가 활성화 상태에서 도달한 최신 좌표
        /// </summary>
        private Vector3 _LatestAutonomyPosition;

        /// <summary>
        /// 길찾기 에이전트 비활성화 시, 원본 아핀객체와 위상차가 생기는데 그 값을 기록하는 필드
        /// </summary>
        private Vector3 _LatestAutonomyCorrectVector;
        
        /// <summary>
        /// 길찾기 목적지 위치 초기화를 막는 플래그
        /// </summary>
        private bool _BlockClearDestinationFlag;

        /// <summary>
        /// 길찾기 경로 계산 지연 이벤트가 처리됬음을 표시하는 플래그
        /// </summary>
        private bool _PendingDelayOnceFlag;

        private NavMeshPath _Path;
        
        #endregion
   
        #region <Enums>
   
        /// <summary>
        /// 길찾기 컴포넌트의 상태
        /// </summary>
        public enum NavmeshAgentType
        {
            /// <summary>
            /// 최초에 생성된 상태
            /// </summary>
            Spawned,
               
            /// <summary>
            /// 풀링되어 오브젝트가 활성화된 상태
            /// </summary>
            Pooled,
               
            /// <summary>
            /// 물리 에이전트가 활성화된 상태
            /// </summary>
            Enable,
               
            /// <summary>
            /// 물리 에이전트가 비활성화된 상태
            /// </summary>
            Disable
        }

        #endregion
           
        #region <Callbacks>
   
        public override IPhysics OnInitializePhysics(UnitPhysicsDataRoot.UnitPhysicsType p_PhysicsType, Unit p_TargetUnit, IPhysicsTableRecordBridge p_PhysicsPreset)
        {
            base.OnInitializePhysics(p_PhysicsType, p_TargetUnit, p_PhysicsPreset);

            _PhysicsRecord = (NavMeshAgentPresetData.PhysicsTableRecord) p_PhysicsPreset;
            _NavMeshAgent = _MasterNode.gameObject.AddNavMeshAgentSafe();
            _NavMeshAgent.speed = _NavMeshAgentDefaultSpeed = DefaultNavMeshAgentDefaultSpeed;
            _NavMeshAgent.acceleration = Deacceleration;
            _NavMeshAgent.angularSpeed = AngularVel;
            _NavMeshAgent.radius = _CharacterController.radius;
            _NavMeshAgent.height = _CharacterController.height;
            _NavMeshAgent.avoidancePriority = _PhysicsRecord.NavMeshAgentPriority;
            _NavMeshAgent.autoBraking = true;
            _NavMeshAgent.updatePosition = false;
               
            _NavMeshObstacle = p_TargetUnit.gameObject.AddComponent<NavMeshObstacle>();
            _NavMeshObstacle.shape = NavMeshObstacleShape.Capsule;
            _NavMeshObstacle.center = _CharacterController.center;
            _NavMeshObstacle.radius = _CharacterController.radius;
            _NavMeshObstacle.height = _CharacterController.height;
               
            OnAwakeOffMeshLink();
            SetNavMeshAgentState(NavmeshAgentType.Spawned);
               
            _PendingDeadlineCounter.Initialize(PendingBreakInterval);
            _PathFirstDelayCounter.Initialize(PathStartFirstDelay);
            _StuckCheckCounter.Initialize(StuckCheckInterval);
            _InvalidPathCounter.Initialize(InvalidPathResetInterval);
               
            _ReservedNavMeshDestinationList = new List<Vector3>();
            _Path = new NavMeshPath();
               
            return this;
        }
   
        protected override void OnModuleNotify()
        {
            base.OnModuleNotify();
               
            SetNavMeshAgentEnableKey(false);
            SetNavMeshAgentState(NavmeshAgentType.Pooled);
            OnPoolingOffMeshLink();
            ClearPhysicsAutonomySpeed();
        }
   
        protected override void OnModuleSleep()
        {
            base.OnModuleSleep();
   
            _BlockClearDestinationFlag = false;
            SetBreakingDistance(0f);
            _ClearPhysicsAutonomyMove();
        }
           
        public override void OnPreUpdate(float p_DeltaTime)
        {
            // 네브메쉬에이전트 상태를 체크해준다.
            switch (_NavmeshAgentType)
            {
                case NavmeshAgentType.Spawned:
                    break;
                case NavmeshAgentType.Pooled:
                    break;
                case NavmeshAgentType.Enable:
                    // TODO<K514> : 활성화되어 있는 중에, 유닛이 네브메쉬 서피스를 벗어난 경우 예외가 발생함.
                    // 목적지가 선정되어 있는 경우
                    if (_IsNavMeshAgentDestinationValid)
                    {
                        // pending 상태(경로 계산 중인 상태)인 경우에는 너무 오랫동안 계산을 하는 것을 방지하기 위한
                        // 타이머를 돌린다.
                        if (_NavMeshAgent.pathPending)
                        {
                            TryCheckPendingDeadline(p_DeltaTime);
                        }
                        else
                        {
                            if (_PathFirstDelayCounter.IsOver())
                            {
                                // 해당 값은 현재 위치로부터 목적지 위치까지의 거리와 같음.
                                var remainingDistance = _NavMeshAgent.remainingDistance;
                                var stoppingDistance = _NavMeshAgent.stoppingDistance;
                                
                                // 경로 계산 결과에 따라,
                                switch (_NavMeshAgent.pathStatus)
                                {
                                    // 완전한 경로
                                    case NavMeshPathStatus.PathComplete:
                                        // 경로 진행 중에는 유닛이 이동했으므로 플래그를 세운다.
                                        _MasterNode.OnUnitPositionChangeDetected();

                                        // 사고 모듈로에 해당 이동을 종료해도 되는지 여부를 요청한다.
                                        switch (_MasterNode._MindObject.OnUpdateAutonomyPhysics(remainingDistance, stoppingDistance))
                                        {
                                            case PhysicsTool.UpdateAutonomyPhysicsResult.CheckNextMoveDestination:
                                                // 현재 이동이 종료된 경우, 예약된 다음 이동에 관한 처리를 해준다.
                                                TryCastNextDestination();
                                                break;
                                            case PhysicsTool.UpdateAutonomyPhysicsResult.DoNothing:
                                                break;
                                            case PhysicsTool.UpdateAutonomyPhysicsResult.ProgressNavMeshControl:
                                                // 사고 모듈로부터 이동 허가 싸인을 받은 경우, 경로 계산 완료 콜백을 호출해준다.
                                                OnAutonomyPhysicsPendingOver(true, remainingDistance, stoppingDistance);
                                                // 경로 이동 중에 해당 물리 모듈이 어딘가에 끼었는지 체크를 해준다.
                                                TryCheckAutonomyStuck(p_DeltaTime);
                                                break;
                                        }
                                        break;
                                    // 불완전 경로
                                    case NavMeshPathStatus.PathPartial:
                                        // 경로 진행 중에는 유닛이 이동했으므로 플래그를 세운다.
                                        _MasterNode.OnUnitPositionChangeDetected();
                                        
                                        // 사고 모듈로에 해당 이동을 종료해도 되는지 여부를 요청한다.
                                        switch (_MasterNode._MindObject.OnUpdateAutonomyPhysics(remainingDistance, stoppingDistance))
                                        {
                                            // 불완전 패스에서 다음 패스로 넘어가는 경우
                                            // 현재 불완전 패스 목적지를 초기화 시키지 않고, 다음 번에 비교하는 용도로 사용한다.
                                            // 불완전 패스는 항상 목적지와 최종 도달지점(stoppingDist > remainingDist)이 다르기 때문에
                                            // 다음번 목적지도 같은 위치라면 모션 낭비만 발생하고 이동하지 않을 것이기 때문이다.
                                            // 세트된 플래그는 모듈 초기화 혹은 완전 패스 진입시 초기화된다.
                                            case PhysicsTool.UpdateAutonomyPhysicsResult.CheckNextMoveDestination:
                                                _BlockClearDestinationFlag = true;
                                                TryCastNextDestination();
                                                break;
                                            case PhysicsTool.UpdateAutonomyPhysicsResult.DoNothing:
                                                break;
                                            case PhysicsTool.UpdateAutonomyPhysicsResult.ProgressNavMeshControl:
                                                OnAutonomyPhysicsPendingOver(false, remainingDistance, stoppingDistance);
                                                TryCheckAutonomyStuck(p_DeltaTime);
                                                break;
                                        }
                                        break;
                                    // 베이킹된 맵에 문제가 있거나 하는 경우
                                    case NavMeshPathStatus.PathInvalid:
                                        OnAutonomyPhysicsPendingOver(false, remainingDistance, stoppingDistance);
                                        TryCheckInvalidPathReset(p_DeltaTime);
                                        break;
                                }
                            }
                            else
                            {
                                _PathFirstDelayCounter.Progress(p_DeltaTime);
                            }
                        }
                    }
                    break;
                case NavmeshAgentType.Disable:
                    break;
            }
               
            base.OnPreUpdate(p_DeltaTime);
        }
        
        /// <summary>
        /// 해당 콜백은 모션관련 콜백을 호출하기 때문에, 반드시 사고 모듈로부터 이동 진행 싸인을 받은
        /// 이후에 호출되야한다. 왜냐하면, 해당 콜백이 실행된 이후 사고 모듈이 이동을 막은 경우에
        /// 모션 낭비가 발생할 수 있기 때문이다.
        /// </summary>
        private void OnAutonomyPhysicsPendingOver(bool p_ResetClearDestinationFlag, float p_RemainingDistance, float p_StoppingDistance)
        {
            if (!_IsPendingOver)
            {
                _IsPendingOver = true;

                if (p_ResetClearDestinationFlag)
                {
                    _BlockClearDestinationFlag = false;
                }

                var targetMind = _MasterNode._MindObject;
                targetMind.OnAutonomyPhysicsPendingOver(p_RemainingDistance.IsReachedValue(p_StoppingDistance));
                SetPhysicsAutonomySpeed(targetMind.GetAISpeedRate());
            }  
        }

        /// <summary>
        /// 경로 계산 지연시 사용할 변수를 초기화시키는 메서드
        /// </summary>
        private void OnInitializePathPendingPreset(bool p_ResetFirstCount)
        {
            _PendingDelayOnceFlag = false;
            _PendingDeadlineCounter.Reset();

            if (p_ResetFirstCount)
            {
                _PathFirstDelayCounter.Reset();
            }
        }

        private void OnInitializePathStuckPreset()
        {
            _StuckCheckCounter.Reset();
            _StuckCheckPivot = _Transform.position;
        }

        /// <summary>
        /// 경로 계산이 너무 오래 걸리는 것을 방지하기 위한 타이머 함수
        /// </summary>
        private void TryCheckPendingDeadline(float p_DeltaTime)
        {
            // 이미 계산이 끝난 상태에서, 경로 계산이 다시 실행되는 경우
            if (_IsPendingOver)
            {
                // 계산 종료 플래그를 리셋시키고, 속도도 0으로 하여 대기상태로 만든다.
                _IsPendingOver = false;
                OnInitializePathPendingPreset(true);
                ClearPhysicsAutonomySpeed();
                goto SEG_PATH_FINDING_DELAY_HANDLE;
            }
            else
            {
                // 일정시간이 지나도 경로 계산이 종료되지 않는다면, 경로 실패 이벤트를 호출한다.
                if (_PendingDeadlineCounter.IsOver())
                {
                    OnInitializePathPendingPreset(true);
                    var targetMind = _MasterNode._MindObject;
                    targetMind.OnAutonomyPhysicsPendingDeadline();
                }
                else
                {
                    // 일정시간이 지나도 경로 계산이 종료되지 않는다면, 지연 이벤트를 호출한다.
                    _PendingDeadlineCounter.Progress(p_DeltaTime);
                    if (_PendingDeadlineCounter.ProgressRate > PendingDelayHandleUpperBoundProgressRate)
                    {
                        goto SEG_PATH_FINDING_DELAY_HANDLE;
                    }
                }
            }
            
            return;
            
            // 계산 지연 이벤트 세그먼트
            SEG_PATH_FINDING_DELAY_HANDLE:
            if (!_PendingDelayOnceFlag)
            {
                _PendingDelayOnceFlag = true;      
                var targetMind = _MasterNode._MindObject;
                targetMind.OnAutonomyPhysicsPendingDelay();
            }
        }
           
        /// <summary>
        /// 이전 위치와 현재 위치 간의 위상차를 비교하여, 해당 유닛이 이동중 상태인데
        /// 한곳에 머물고 있는지 검증하는 타이머 함수
        /// </summary>
        private void TryCheckAutonomyStuck(float p_DeltaTime)
        {
            if (_StuckCheckCounter.IsOver())
            {
                // 지정한 시간 동안 유닛이 자신의 반경보다 이동하지 못한 경우, 이동중 막힌걸로 판단
                if (_StuckCheckPivot.GetSqrDistanceTo(_Transform.position) < _MasterNode._RangeObject.Radius.CurrentSqrValue)
                {
                    _StuckCheckCounter.Reset();
                    
                    var targetMind = _MasterNode._MindObject;
                    targetMind.OnAutonomyPhysicsStuck();
                }
                else
                {
                    OnInitializePathStuckPreset();
                }
            }
            else
            {
                _StuckCheckCounter.Progress(p_DeltaTime);
            }
        }
           
        /// <summary>
        /// 길찾기 결과가 유효하지 않은 경로 였던 경우, 즉시 길찾기를 재개하는 것 보다는
        /// 일정 간격을 두고 하는게 과부하를 막기 때문에
        /// 경로 재계산 까지의 간격을 재는 타이머 함수
        /// </summary>
        private void TryCheckInvalidPathReset(float p_DeltaTime)
        {
            if (_InvalidPathCounter.IsOver())
            {
                _InvalidPathCounter.Reset();
                var targetMind = _MasterNode._MindObject;
                targetMind.OnAutonomyPhysicsStuck();
            }
            else
            {
                _InvalidPathCounter.Progress(p_DeltaTime);
            }
        }
  
        /// <summary>
        /// 예약된 목적지가 있는지 리턴하는 메서드
        /// </summary>
        public override bool HasReservedDestination()
        {
            return _ReservedNavMeshDestinationList.Count > 0;
        }

        /// <summary>
        /// 다음 목적지 위치로의 경로 계산을 수행하는 메서드
        /// </summary>
        private void TryCastNextDestination()
        {
            while (_ReservedNavMeshDestinationList.Count > 0)
            {
                var tryDestination = _ReservedNavMeshDestinationList[0];
                _ReservedNavMeshDestinationList.RemoveAt(0);
                
                // 지정한 위치로 경로계산을 수행한다.
                var (result, resultMessage) = SetDestination(tryDestination, true, ObjectDeployTool.ObjectDeploySurfaceDeployType.None, default);
                if (result)
                {
                    switch (_MasterNode._MindObject.OnUpdateAutonomyPhysics(0f, _NavMeshAgent.stoppingDistance))
                    {
                        case PhysicsTool.UpdateAutonomyPhysicsResult.CheckNextMoveDestination:
                        case PhysicsTool.UpdateAutonomyPhysicsResult.DoNothing:
                            break;
                        case PhysicsTool.UpdateAutonomyPhysicsResult.ProgressNavMeshControl:
                            // 지정한 위치로의 경로 계산에 성공한 경우
                            _StuckCheckCounter.Reset();
                            return;
                    }
                }
            }

            // 더 이상 경로가 없는 경우, 이동 종료 콜백을 사고모듈에 던진다.
            _MasterNode._MindObject.OnAutonomyPhysicsPathOver();
        }
  
        /// <summary>
        /// 유닛 pivot 이 변경되는 경우 호출되는 콜백
        /// </summary>
        public override void OnPivotChanged(PositionStatePreset p_PositionStatePreset, bool p_SyncPosition)
        {
            base.OnPivotChanged(p_PositionStatePreset, p_SyncPosition);

            if (p_SyncPosition)
            {
                // 경로를 초기화 시켜준다.
                ResetNavigatePath();
            }
        }
   
        /// <summary>
        /// 피격 액션이 시작되는 경우 호출되는 콜백
        /// </summary>
        public override void OnUnitHitActionStarted()
        {
            base.OnUnitHitActionStarted();
               
            ClearPhysicsAutonomyMove();
        }
           
        /// <summary>
        /// 유닛 액션이 시작되는 경우 호출되는 콜백
        /// </summary>
        public override void OnUnitActionStarted()
        {
            base.OnUnitActionStarted();
               
            ClearPhysicsAutonomyMove();
        }
   
        /// <summary>
        /// 유닛 사망시 호출되는 콜백
        /// </summary>
        public override void OnUnitDead(bool p_Instant)
        {
            base.OnUnitDead(p_Instant);
            
            ClearPhysicsAutonomyMove();
            _NavMeshObstacle.enabled = false;
        }

        #endregion
           
        #region <Methods>
           
        /// <summary>
        /// 현재 물리 모듈이 길찾기를 포함하는지 여부를 리턴하도록 구현
        /// </summary>
        public override bool IsAutonomyValid(bool p_CheckEnable)
        {
            return !p_CheckEnable || IsNavmeshAgentEnable;
        }
        
        /// <summary>
        /// 네브메쉬 에이전트 활성화를 블록하는 키를 세트하는 메서드
        /// </summary>
        public void SetNavMeshAgentEnableKey(bool p_NavMeshAgentBlockKey)
        {
            _NavMeshAgentStateBlockKey = p_NavMeshAgentBlockKey;
        }
   
        /// <summary>
        /// 현재 네브메쉬 에이전트를 활성화 시킬 수 있는지 검증하는 논리메서드
        /// </summary>
        private bool IsTransitionableToEnableNavMeshAgent()
        {
            return IsGround() && !_NavMeshAgentStateBlockKey;
        }
   
        /// <summary>
        /// 네브메쉬 에이전트의 활성화 상태를 세트하는 메서드
        /// </summary>
        public void SetNavMeshAgentState(NavmeshAgentType p_Flag, bool p_NavMeshAgentBlockKey)
        {
            SetNavMeshAgentEnableKey(p_NavMeshAgentBlockKey);
            SetNavMeshAgentState(p_Flag);
        }
           
        /// <summary>
        /// 네브메쉬 에이전트의 활성화 상태를 세트하는 메서드
        /// </summary>
        public void SetNavMeshAgentState(NavmeshAgentType p_Flag)
        {
            if (_NavmeshAgentType != p_Flag)
            {
                switch (_NavmeshAgentType)
                {
                    case NavmeshAgentType.Spawned:
                        switch (p_Flag)
                        {
                            // Pooled 외의 전이는 허용하지 않는다.
                            case NavmeshAgentType.Pooled:
                                break;
                            default:
                                return;
                        }
                        break;
                    case NavmeshAgentType.Pooled:
                        switch (p_Flag)
                        {
                            // Spawned 로의 전이는 허용하지 않는다.
                            case NavmeshAgentType.Spawned:
                                return;
                            case NavmeshAgentType.Enable:
                                if (!IsTransitionableToEnableNavMeshAgent())
                                {
                                    p_Flag = NavmeshAgentType.Disable;
                                }
                                break;
                        }
                        break;
                    case NavmeshAgentType.Enable:
                        switch (p_Flag)
                        {
                            // Spawned 로의 전이는 허용하지 않는다.
                            case NavmeshAgentType.Spawned:
                                return;
                        }
                        break;
                    case NavmeshAgentType.Disable:
                        switch (p_Flag)
                        {
                            // Spawned 로의 전이는 허용하지 않는다.
                            case NavmeshAgentType.Spawned:
                                return;
                            case NavmeshAgentType.Enable:
                                if (!IsTransitionableToEnableNavMeshAgent())
                                {
                                    return;
                                }
                                break;
                        }
                        break;
                }

                _NavmeshAgentType = p_Flag;
                   
                switch (_NavmeshAgentType)
                {
                    // 에이전트가 활성화 된 경우,
                    case NavmeshAgentType.Enable:
                        _NavMeshObstacle.enabled = false;
                        _NavMeshAgent.enabled = true;
                        break;
                    // 에이전트가 비활성화된 경우, baseOffset만큼 현재 위치를 보정시켜준다.
                    case NavmeshAgentType.Spawned:
                    case NavmeshAgentType.Pooled:
                    case NavmeshAgentType.Disable:
                        _NavMeshAgent.enabled = false;
                        _NavMeshObstacle.enabled = true;
                        ClearNavMeshBaseLocalYOffset();
                        break;
                }
            }
        }
   
        /// <summary>
        /// 네브메쉬 에이전트의 baseOffset을 제외한 현재 네브메쉬 에이전트의 좌표(표면 위의)를 리턴하는 메서드
        /// </summary>
        public Vector3 GetNavMeshAgentPosition()
        {
            return _Transform.position + Vector3.down * _NavMeshAgent.baseOffset;
        }
           
        /// <summary>
        /// 네브메쉬 에이전트의 baseOffset을 0으로하고, 그값만큼 높이를 보정하는 메서드
        /// </summary>
        public void ClearNavMeshBaseLocalYOffset()
        {
            var baseOffset = _NavMeshAgent.baseOffset;
            if (baseOffset > 0f)
            {
                _Transform.position += Vector3.up * baseOffset;
                _NavMeshAgent.baseOffset = 0f;
            }
        }
           
        /// <summary>
        /// 네브메쉬 에이전트의 baseOffset으로 세트하는 메서드
        /// 지정할 수 있는 실수구간은 ( 절대값 _VelocityYLowerVectorFactor, +무한 )
        /// </summary>
        public void SetNavMeshBaseLocalYOffset(float p_Offset)
        {
            _NavMeshAgent.baseOffset = p_Offset > PhysicsTool._VelocityYLowerVectorFactor_Negative ? p_Offset : 0f;
        }
   
        /// <summary>
        /// 지정한 값 만큼 네브메쉬 에이전트의 baseOffset을 더하는 메서드
        /// 이 때, baseoffset은 0이하가 되지 않으며 0이하가 되도록 값을 입력했다면
        /// 적용되지 않은 나머지 음수 값을 리턴한다.
        /// </summary>
        public float AddNavMeshBaseLocalYOffset(float p_Offset)
        {
            var targetOffset = _NavMeshAgent.baseOffset + p_Offset;
            SetNavMeshBaseLocalYOffset(targetOffset);
            return Mathf.Min(PhysicsTool._VelocityYLowerVectorFactor_Negative, targetOffset);
        }

        /// <summary>
        /// 길찾기 에이전트가 활성화되어있는 동안에는 현재 상태를 착지상태로 고정시켜준다.
        /// </summary>
        protected override bool UpdateAerialState()
        {
            if (IsNavmeshAgentEnable)
            {
                var prevAerialState = _AerialState;
                _AerialState = PhysicsTool.AerialState.OnGround;
                return prevAerialState != _AerialState;
            }
            else
            {
                return base.UpdateAerialState();
            }
        }

        /// <summary>
        /// 기존의 캐릭터 컨트롤러의 이동과는 다르게, 네브메쉬 에이전트는 네브메쉬 표면으로부터
        /// y축으로 멀어지는 경우, 네브메쉬 엔진에 의해 곧바로 표면으로 돌아오도록 좌표를 수정당하기 때문에
        /// 같은 방법으로는 체공 연출을 구현할 수 없다.
        ///
        /// 네브메쉬 에이전트는 y축 위상에 대응하는 baseOffset이라는 멤버를 제공하는데, 해당 값을
        /// 제어하는 것으로 에이전트 자체는 네브메쉬 표면에 고정된 상태로
        /// 나머지 컬라이더를 포함한 게임오브젝트는 y축 이동이 가능하다.
        ///
        /// 아래 메서드는 기존의 캐릭터 컨트롤러 이동 메서드를 오버라이드하여
        /// y축에 대해서만 baseOffset을 변형시키도록 구현되어 있다.
        /// </summary>       
        public override void MoveTo(Vector3 p_DeltaVelocity, float p_DeltaTime)
        {
            // 네브메쉬 에이전트가 활성화되어 있는 경우
            if (IsNavmeshAgentEnable)
            {
                return;
                // offlink load 중에는 피격 당한 경우를 제외하고 외력의 영향을 받지 않는다.
//                if (_InOnNavMesh_Off_MeshLink && !_MasterNode.HasState_Or(Unit.UnitStateType.STUCK))
//                {
//                    p_DeltaVelocity = Vector3.zero;
//                }
//   
//                // 속도벡터에서 Y값은 baseOffset으로 처리하므로 제거해준다.
//                var deltaVelocityY = p_DeltaVelocity.y;
//                p_DeltaVelocity = p_DeltaVelocity.XZVector();
//                       
//                /* 네브메쉬 에이전트 길찾기 방향을 기준으로 */
//                // +y축으로 속도가 작용중인 경우
//                if (deltaVelocityY > 0f)
//                {
//                    p_DeltaVelocity.y += PhysicsTool._VelocityYLowerVectorFactor_Negative;
//                    deltaVelocityY += PhysicsTool._VelocityYLowerVectorFactor;
//                }
//                // -y축으로 속도가 작용중인 경우
//                else if (deltaVelocityY < 0f)
//                {
//                    p_DeltaVelocity.y += PhysicsTool._VelocityYLowerVectorFactor;
//                    deltaVelocityY += PhysicsTool._VelocityYLowerVectorFactor_Negative;
//                }
//                   
//                // base offset 값을 보정하고 속도를 적용시킨다.
//                p_DeltaVelocity.y += AddNavMeshBaseLocalYOffset(deltaVelocityY);
//                _CharacterController.Move(p_DeltaVelocity);
//                _MasterNode.OnUnitPositionChangeDetected();
            }
            // 네브메쉬 에이전트가 비활성화되어 있다면, 캐릭터 컨트롤러 이동 로직을 따른다.
            else
            {
                base.MoveTo(p_DeltaVelocity, p_DeltaTime);
            }
        }
           
        #endregion    
    }
}