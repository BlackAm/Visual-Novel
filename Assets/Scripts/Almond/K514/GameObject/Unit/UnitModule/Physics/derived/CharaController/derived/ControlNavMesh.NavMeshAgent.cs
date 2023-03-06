using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace k514
{
    public partial class ControlNavMesh
    {
        #region <Callbacks>

        private void OnDestinationUpdated(Vector3 p_Destination, float p_StoppingDistance)
        {
            SetNavMeshDestinationPivotPosition(p_Destination);
            SetBreakingDistance(p_StoppingDistance);
            _MasterNode.OnSelectDestination();
            _NavMeshAgent.updatePosition = true;
            
#if SERVER_DRIVE
            HeadlessServerManager.GetInstance.OnUnitAutonomyPathRequested(_MasterNode, _NavMeshDestination);
#endif
            OnUpdateLatestPassivePosition();
        }

        /// <summary>
        /// 길찾기 에이전트가 활성화된 경우에는
        /// 다음 프레임에 물리엔진이 보정되는 과정에서 아핀 좌표의 위상차가 생긴다.
        /// 해당 값을 캐싱한다. 
        /// </summary>
        private async void OnUpdateLatestPassivePosition()
        {
            // 길찾기 에이전트가 활성화된 경우에는
            // 다음 프레임에 물리엔진이 보정되는 과정에서 아핀 좌표의 위상차가 생긴다.
            // 해당 값을 캐싱한다.
            _LatestPassivePosition = _Transform.position;
            
            await UniTask.NextFrame();

            if (_MasterNode.IsValid())
            {
                _LatestPassiveCorrectVector = _NavMeshAgent.pathEndPosition - _NavMeshDestination;
            }
        }
        
        /// <summary>
        /// 길찾기 에이전트가 비활성화된 경우에는
        /// 다음 프레임에 물리엔진이 보정되는 과정에서 아핀 좌표의 위상차가 생긴다.
        /// 해당 값을 캐싱한다. 
        /// </summary>
        private async void OnUpdateLatestAutonomyPosition()
        {
            _LatestAutonomyPosition = _Transform.position;
                
            await UniTask.NextFrame();
                
            if (_MasterNode.IsValid())
            {
                _LatestAutonomyCorrectVector = _LatestAutonomyPosition - _Transform.position;
                base.MoveTo(_LatestAutonomyCorrectVector, 1f);
            }
        }

        #endregion
        
        #region <Method/AutoPath>

        /// <summary>
        /// 길찾기 에이전트에 등록되어 있는 경로 오브젝트를 리턴하는 메서드
        /// 경로 오브젝트는 길찾기 함수(SetDestination)로 NavMeshAgent.path 필드에 초기화되며
        /// 해당 값을 다른 길찾기 에이전트에 SetPath해주면 해당 경로를 그대로 이동한다.
        /// </summary>
        public override NavMeshPath GetPath()
        {
            return _NavMeshAgent.path;
        }

        /// <summary>
        /// 길찾기 경로 오브젝트를 해당 물리 모듈의 길찾기 에이전트에 세트해주는 메서드
        /// </summary>
        public override (bool, PhysicsTool.NavMeshAgentDriveState) SetPath(NavMeshPath p_Path, PhysicsTool.AutonomyPathStoppingDistancePreset p_AutonomyPathStoppingDistancePreset)
        {
            // 마스터 노드 유닛이 움직일 수 있는 경우에,
            if (_MasterNode.HasState_Only(Unit.UnitStateType.DefaultMoveAvailableMask))
            {
                var corners = p_Path.corners;
                var cornerCount = corners.Length;
                if (cornerCount < 1)
                {
                    // 경로 객체 내부의 정점 갯수가 부족한 경우
                    return (false, PhysicsTool.NavMeshAgentDriveState.InvalidPath);
                }
                else
                {
                    // 경로 세트를 위해 길찾기 에이전트를 활성화시킨다.
                    SetNavMeshAgentState(NavmeshAgentType.Enable);
                    var (result, resultMessage) = (false, PhysicsTool.NavMeshAgentDriveState.Unreachable);

                    // 에이전트가 활성화 되어 있는 경우
                    if (IsNavmeshAgentEnable)
                    {
                        // 이미 목적지가 세트되어 있는 경우
                        if (_IsNavMeshAgentDestinationValid)
                        {
                            // 경로에 정점이 단 하나이고, 해당 위치가 기존의 목적지와 같은 경우 경로를 세트하지 않는다.
                            if (cornerCount == 1 && (corners[0] - _NavMeshDestination).sqrMagnitude.IsReachedZero())
                            {
                                (result, resultMessage) = (true, PhysicsTool.NavMeshAgentDriveState.SameDestination);
                            }
                            else
                            {
                                if (_NavMeshAgent.SetPath(p_Path))
                                {
                                    OnDestinationUpdated(corners.Last(), p_AutonomyPathStoppingDistancePreset.GetStoppingDistance());
                                    (result, resultMessage) = (true, PhysicsTool.NavMeshAgentDriveState.Reachable);
                                }
                                else
                                {
                                    (result, resultMessage) = (false, PhysicsTool.NavMeshAgentDriveState.Unreachable);
                                }
                            }
                        }
                        else
                        {
                            // 정지거리를 가져온다.
                            var tryStoppingDistance = p_AutonomyPathStoppingDistancePreset.GetStoppingDistance();
                            
                            // 경로에 정점이 단 하나이고, 해당 위치가 현재 위치와 같은 경우 경로를 세트하지 않는다.
                            if (cornerCount == 1 && (corners[0] - _Transform.position).sqrMagnitude <= Mathf.Pow(tryStoppingDistance, 2))
                            {
                                (result, resultMessage) = (false, PhysicsTool.NavMeshAgentDriveState.SamePosition);
                            }
                            else
                            {
                                if (_NavMeshAgent.SetPath(p_Path))
                                {
                                    OnDestinationUpdated(corners.Last(), tryStoppingDistance);
                                    (result, resultMessage) = (true, PhysicsTool.NavMeshAgentDriveState.Reachable);
                                }
                                else
                                {
                                    (result, resultMessage) = (false, PhysicsTool.NavMeshAgentDriveState.Unreachable);
                                }
                            }
                        }
                    }
                    else
                    {
                        (result, resultMessage) = (false, PhysicsTool.NavMeshAgentDriveState.NavMeshAgentInvalid);
                    }
                    
                    switch (resultMessage)
                    {
                        case PhysicsTool.NavMeshAgentDriveState.CorrectSurfaceFailed:
                        case PhysicsTool.NavMeshAgentDriveState.NavMeshAgentInvalid:
                        case PhysicsTool.NavMeshAgentDriveState.Unreachable:
                        case PhysicsTool.NavMeshAgentDriveState.SamePosition:
                            break;
                    }

                    return (result, resultMessage);
                }
            }
            else
            {
                SetNavMeshAgentState(NavmeshAgentType.Disable);
                return (false, PhysicsTool.NavMeshAgentDriveState.MasterNodeCantMove);
            }
        }

        /// <summary>
        /// 지정한 좌표로의 경로를 계산하고, 길찾기 에이전트가 해당 경로를 따라 이동하도록 하는 메서드
        /// </summary>
        private (bool, PhysicsTool.NavMeshAgentDriveState) SetDestination(Vector3 p_Destination, bool p_OverlapCurrentDestination, ObjectDeployTool.ObjectDeploySurfaceDeployType p_ObjectDeploySurfaceDeployType, PhysicsTool.AutonomyPathStoppingDistancePreset p_AutonomyPathStoppingDistancePreset)
        {
            // 좌표를 터레인 표면에 세트한다.
            var (valid, formedPosition) = ObjectDeployTool.CorrectAffinePreset(GameManager.Obstacle_Terrain_LayerMask, p_Destination, p_ObjectDeploySurfaceDeployType);

            if (valid)
            {
                // 추가하려는 좌표가 기존 좌표를 덮어써야하는 경우
                if (p_OverlapCurrentDestination)
                {
                    // 기존 좌표가 있던 경우
                    if (_IsNavMeshAgentDestinationValid)
                    {
                        // 새로 경로를 갱신하려는 지점과 기존의 목적지가 같은 경우
                        if ((formedPosition - _NavMeshDestination).sqrMagnitude.IsReachedZero())
                        {
                            return (true, PhysicsTool.NavMeshAgentDriveState.SameDestination);
                        }
                        else
                        {
                            if (_NavMeshAgent.SetDestination(formedPosition))
                            {
                                OnDestinationUpdated(formedPosition, p_AutonomyPathStoppingDistancePreset.GetStoppingDistance());
#if SERVER_DRIVE
                                if (_MasterNode.IsPlayer)
#else
                                if (_MasterNode.IsPlayer && !LamiereGameManager.GetInstanceUnSafe.IsQuest)
#endif
                                {
                                    _MasterNode.SetTargetPosition(formedPosition);
                                }
                                return (true, PhysicsTool.NavMeshAgentDriveState.Reachable);
                            }
                            else
                            {
                                return (false, PhysicsTool.NavMeshAgentDriveState.Unreachable);
                            }
                        }
                    }
                    else
                    {
                        goto SEG_CALC_NEW_PATH;
                    }
                }
                // 이동 큐에 좌표를 추가하는 경우
                else
                {
                    // 기존 좌표가 있던 경우
                    if (_IsNavMeshAgentDestinationValid)
                    {
                        // 이동 큐에 다른 좌표가 있던 경우
                        var reservedCount = _ReservedNavMeshDestinationList.Count;
                        if (reservedCount > 0)
                        {
                            var lastAddedDestination = _ReservedNavMeshDestinationList[reservedCount - 1];
                            // 새로 경로를 갱신하려는 지점과 마지막에 예약된 지점이 같은 경우
                            if ((formedPosition - lastAddedDestination).sqrMagnitude.IsReachedZero())
                            {
                                return (true, PhysicsTool.NavMeshAgentDriveState.SameDestination);
                            }
                            else
                            {
                                _ReservedNavMeshDestinationList.Add(formedPosition);
                                return (true, PhysicsTool.NavMeshAgentDriveState.Reserved);
                            }
                        }
                        else
                        {
                            // 새로 경로를 갱신하려는 지점과 기존의 목적지가 같은 경우
                            if ((formedPosition - _NavMeshDestination).IsReachedZero())
                            {
                                return (true, PhysicsTool.NavMeshAgentDriveState.SameDestination);
                            }
                            else
                            {
                                _ReservedNavMeshDestinationList.Add(formedPosition);
                                return (true, PhysicsTool.NavMeshAgentDriveState.Reserved);
                            }
                        }
                    }
                    else
                    {
                        goto SEG_CALC_NEW_PATH;
                    }
                }
            }
            else
            {
                return (false, PhysicsTool.NavMeshAgentDriveState.CorrectSurfaceFailed);
            }

            // 현재 길찾기 에이전트가 이동중이지 않고, 지정한 위치로의 경로 계산 및 이동이 필요한 경우 도달하는 구역
            SEG_CALC_NEW_PATH:
            {
                // 정지거리를 가져온다.
                var tryStoppingDistance = p_AutonomyPathStoppingDistancePreset.GetStoppingDistance();
                if ((formedPosition - _Transform.position).sqrMagnitude <= Mathf.Pow(tryStoppingDistance, 2))
                {
                    return (false, PhysicsTool.NavMeshAgentDriveState.SamePosition);
                }
                else
                {
                    if (_NavMeshAgent.SetDestination(formedPosition))
                    {
                        OnDestinationUpdated(formedPosition, tryStoppingDistance);
#if !SERVER_DRIVE
                        if (LamiereGameManager.GetInstanceUnSafe.ShowTouchedMovementWay && _MasterNode.IsPlayer && !LamiereGameManager.GetInstanceUnSafe.IsQuest)
                        {
                            _MasterNode.SetTargetPosition(formedPosition);
                        }
#endif
                        return (true, PhysicsTool.NavMeshAgentDriveState.Reachable);
                    }
                    else
                    {
                        return (false, PhysicsTool.NavMeshAgentDriveState.Unreachable);
                    }
                }
            }
        }

        /// <summary>
        /// 지정한 좌표로의 경로를 계산하고, 길찾기 에이전트가 해당 경로를 따라 이동하도록 하는 메서드
        /// 해당 메서드는 현재 이동중인 경로가 있는 경우 그 경로를 덮어쓰는 지정한 목적지에 대한 경로를 새로 계산한다.
        /// </summary>
        public override (bool, PhysicsTool.NavMeshAgentDriveState) SetPhysicsAutonomyMove(Vector3 p_Position, ObjectDeployTool.ObjectDeploySurfaceDeployType p_ObjectDeploySurfaceDeployType, PhysicsTool.AutonomyPathStoppingDistancePreset p_AutonomyPathStoppingDistancePreset)
        {
            // 마스터 노드 유닛이 움직일 수 있는 경우에,
            if (_MasterNode.GetLatestStampPreset().IsStampedTerrainOrObstacle() && _MasterNode.HasState_Only(Unit.UnitStateType.DefaultMoveAvailableMask))
            {
                SetNavMeshAgentState(NavmeshAgentType.Enable);
                var (result, resultMessage) = (false, PhysicsTool.NavMeshAgentDriveState.Unreachable);
                
                // 에이전트가 활성화 되어 있는 경우
                if (IsNavmeshAgentEnable)
                {
                    // 에이전트가 현재 offMeshLink를 지나는 경우
                    if (false && _NavMeshAgent.currentOffMeshLinkData.valid)
                    {
                        // 오프 메쉬 관련 로직은 추후 재구현
                        /*
                        // 해당 위치에 정확히 도달하기 위해, 정지거리를 0으로 한다.
                        SetBreakingDistance(0f);
                        On_Off_Mesh_LinkBegin();
                        resultState = PhysicsTool.NavMeshAgentDriveState.JumpGateRecognition;
                        */
                    }
                    // 그 외의 경우
                    else
                    {
                        _OfflinkProcessPhase = OfflinkProcessPhase.ReadyToOfflink;
                        if (_NavMeshAgent.isOnNavMesh)
                        {
                            (result, resultMessage) = SetDestination(p_Position, true, p_ObjectDeploySurfaceDeployType, p_AutonomyPathStoppingDistancePreset);
                        }
                        else
                        {
                            (result, resultMessage) = (false, PhysicsTool.NavMeshAgentDriveState.Unreachable);
                        }
                    }
                }
                else
                {
                    (result, resultMessage) = (false, PhysicsTool.NavMeshAgentDriveState.NavMeshAgentInvalid);
                }

                switch (resultMessage)
                {
                    case PhysicsTool.NavMeshAgentDriveState.JumpGateRecognition:
                        // 오프 메쉬 관련 로직은 추후 재구현
                        /*
                        switch (_OfflinkProcessPhase)
                        {
                            case OfflinkProcessPhase.ReadyToOfflink:
                            {
                                _MasterNode.OnJumpUpAreal();
                                
                                var offLinkData = _NavMeshAgent.currentOffMeshLinkData;
                                _OffMeshLink_StartPosition = offLinkData.startPos;
                                _OffMeshLink_EndPosition = _NavMeshAgent.pathEndPosition;
                                
                                var offMeshLinkOffset =
                                    _OffMeshLink_StartPosition.GetDirectionVectorTo(_OffMeshLink_EndPosition);
                                var distanceFactor = offMeshLinkOffset.magnitude * 0.2f;
                                var offMeshLinkOffsetY = offMeshLinkOffset.y;
                                
                                _OffMeshLink_SpeedRate = Mathf.Clamp(distanceFactor, 0.6f, 6f);
                                if (offMeshLinkOffsetY < 0f)
                                {
                                    _JumpHeight = (-offMeshLinkOffsetY + _MasterNode._BattleStatusPreset.t_Current.GetJumpForce()) * 2f;
                                    _OffMeshLink_JumpOrbitEquation = 0.75f;
                                }
                                else
                                {
                                    _JumpHeight = (offMeshLinkOffsetY + _MasterNode._BattleStatusPreset.t_Current.GetJumpForce()) * 2f;
                                    if (offMeshLinkOffsetY > Height.CurrentValue)
                                    {
                                        _OffMeshLink_JumpOrbitEquation = 0.75f;
                                    }
                                    else
                                    {
                                        _OffMeshLink_JumpOrbitEquation = 0.5f;
                                    }
                                }
    
                                _OfflinkProcessPhase = OfflinkProcessPhase.OfflinkProgressing;
                            }
                                break;
                            case OfflinkProcessPhase.OfflinkProgressing:
                                break;
                            case OfflinkProcessPhase.OfflinkTerminate:
                                break;
                        }
                        */
                        break;
                    case PhysicsTool.NavMeshAgentDriveState.CorrectSurfaceFailed:
                    case PhysicsTool.NavMeshAgentDriveState.NavMeshAgentInvalid:
                    case PhysicsTool.NavMeshAgentDriveState.Unreachable:
                    case PhysicsTool.NavMeshAgentDriveState.SamePosition:
                        break;
                }

                return (result, resultMessage);
            }
            else
            {
                SetNavMeshAgentState(NavmeshAgentType.Disable);
                return (false, PhysicsTool.NavMeshAgentDriveState.MasterNodeCantMove);
            }

        }

        #endregion

        #region <Method/ManualPath>

        /// <summary>
        /// 길찾기 이동 큐에 다음 이동 목적지를 예약하는 메서드
        /// </summary>
        public override (bool, PhysicsTool.NavMeshAgentDriveState) AddManualMovePathEdge(Vector3 p_Edge, ObjectDeployTool.ObjectDeploySurfaceDeployType p_ObjectDeploySurfaceDeployType)
        {
            if (IsNavmeshAgentEnable)
            {
                return SetDestination(p_Edge, false, p_ObjectDeploySurfaceDeployType, default);
            }
            else
            {
                return SetPhysicsAutonomyMove(p_Edge, p_ObjectDeploySurfaceDeployType, default);
            }
        }

        /// <summary>
        /// 길찾기 이동 큐에 다음 이동 목적지를 예약하는 메서드
        /// </summary>
        public override (bool, PhysicsTool.NavMeshAgentDriveState) SetManualMovePath(Vector3[] p_Path, ObjectDeployTool.ObjectDeploySurfaceDeployType p_ObjectDeploySurfaceDeployType, PhysicsTool.AutonomyPathStoppingDistancePreset p_AutonomyPathStoppingDistancePreset)
        {
            var isReserved = false;
            foreach (var edge in p_Path)
            {
                var (result, _) = AddManualMovePathEdge(edge, p_ObjectDeploySurfaceDeployType);
                if (!isReserved && result)
                {
                    isReserved = true;
                }
            }

            return (isReserved, PhysicsTool.NavMeshAgentDriveState.Reserved);
        }

        #endregion

        #region <Methods>

        /// <summary>
        /// 네브메쉬 에이전트의 속도를
        /// 0으로 하는 메서드
        /// </summary>
        public override void ClearPhysicsAutonomyMove()
        {
            // 길찾기가 활성화중이었던 경우 초기화 해야할 목록
            if (_IsNavMeshAgentDestinationValid)
            {
                _ClearPhysicsAutonomyMove();
            }
            else
            {
                _InvalidPathCounter.Reset();
                _StuckCheckCounter.Reset();

                SetNavMeshAgentState(NavmeshAgentType.Disable);
            }
        }

        private void _ClearPhysicsAutonomyMove()
        {
            var pendingOver = _IsPendingOver;
            _ReservedNavMeshDestinationList.Clear();
            _NavMeshAgent.updatePosition = false;

            // 길찾기 목적지를 초기화해서는 안되는 경우
            if (_BlockClearDestinationFlag)
            {
                var cachedPosition = _NavMeshDestination;
                SetNavMeshDestinationPivotPosition(_Transform.position);
                _NavMeshDestination = cachedPosition;
            }
            else
            {
                SetNavMeshDestinationPivotPosition(_Transform.position);
            }
                
            _IsNavMeshAgentDestinationValid = false; 
            _MasterNode.OnReachedDestination();
            ClearPhysicsAutonomySpeed();
                
            _InvalidPathCounter.Reset();
            _StuckCheckCounter.Reset();

            SetNavMeshAgentState(NavmeshAgentType.Disable);
                
            if (pendingOver)
            {
                OnUpdateLatestAutonomyPosition();
            }
        }

        /// <summary>
        /// 이동 목적지를 지정하는 메서드
        /// </summary>
        private void SetNavMeshDestinationPivotPosition(Vector3 p_Pivot)
        {
            _IsNavMeshAgentDestinationValid = true;
            _IsPendingOver = false;
            OnInitializePathPendingPreset(false);
            _NavMeshDestination = p_Pivot;
            OnInitializePathStuckPreset();
        }

        /// <summary>
        /// 네브메쉬 에이전트의 이동속도를
        /// 네브메쉬 에이전트의 고유속도에 파라미터로 받은 속도 비율 값
        /// 및 유닛 이동속도를 곱한 값으로 업데이트하는 메서드
        /// </summary>
        public override void SetPhysicsAutonomySpeed(float p_Rate)
        {
            _NavMeshAgent.speed = _NavMeshAgentDefaultSpeed * p_Rate * _MasterNode.GetScaledMovementSpeed() * _MasterNode._ActableObject.GetMoveStateSpeedRate();
        }

        private void ClearPhysicsAutonomySpeed()
        {
            _NavMeshAgent.velocity = Vector3.zero;
            _NavMeshAgent.speed = 0f;
        }

        /// <summary>
        /// 네브메쉬 에이전트의 정지거리를 세트하는 메서드
        /// </summary>
        public void SetBreakingDistance(float p_Distance)
        {
            _NavMeshAgent.stoppingDistance = p_Distance;
        }
        
        /// <summary>
        /// 해당 네브메쉬 에이전트의 경로정보를 초기화시키는 메서드
        /// </summary>
        public void ResetNavigatePath()
        {
            if (_NavMeshAgent.isOnNavMesh)
            {
                _NavMeshAgent.ResetPath();
                ClearPhysicsAutonomyMove();
            }
        }

        /// <summary>
        /// 해당 네브메쉬 에이전트 활성화 여부를 리턴하는 논리 메서드
        /// </summary>
        public override bool IsAutonomyMoving()
        {
            return IsNavmeshAgentEnable;
        }

        #endregion
    }
}