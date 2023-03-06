using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace k514
{
    public static partial class PhysicsTool
    {
        #region <Fields>

        /// <summary>
        /// NonAlloc 캐스트용 배열
        /// </summary>
        public static readonly RaycastHit[] _NonAllocRayCast;

        /// <summary>
        /// NonAlloc 오버랩용 배열
        /// </summary>
        public static readonly Collider[] _NonAllocCollider;
        
        /// <summary>
        /// 가속도 타입 순환자
        /// </summary>
        public static readonly AccelerationType[] _AccelerationTypeEnumerator;
        
        /// <summary>
        /// 감쇄를 적용받지 않는 가속도 타입 순환자
        /// </summary>
        public static readonly AccelerationType[] _DampingTarget_AccelerationTypeEnumerator;
        
        /// <summary>
        /// 유닛 질량 하한값
        /// </summary>
        public const float UnitMassLowerBound = 0.01f;
        
        #endregion
        
        #region <Enums>

        /// <summary>
        /// 체공 타입
        /// </summary>
        public enum AerialState
        {
            /// <summary>
            /// 초기 상태
            /// </summary>
            None,
            
            /// <summary>
            /// 착지상태, 현재 유닛이 지면 혹은 다른 충돌체 위에 있는 경우
            /// </summary>
            OnGround,
            
            /// <summary>
            /// 체공 상태, 현재 유닛 발 밑(하단 충돌부)에 아무런 충돌이 검증되지 않는 경우
            /// </summary>
            OnAerial,
        }

        /// <summary>
        /// 가속 운동계 타입
        /// </summary>
        public enum AccelerationType
        {
            Default,
            SyncWithController,
            Gravity,
            Action,
        }

        /// <summary>
        /// 중력 적용 방식
        /// </summary>
        public enum GravityType
        {
            /// <summary>
            /// 중력을 항상 적용
            /// </summary>
            Applied,
            
            /// <summary>
            /// 타격을 받지 않는 이상 중력 무시
            /// </summary>
            Anti_HitBreak,
            
            /// <summary>
            /// 중력을 항상 무시
            /// </summary>
            Anti_Perfect
        }

        /// <summary>
        /// 네브메쉬 에이전트가 특정 위치를 pivot으로 설정했을 때, 해당 위치의 상태
        /// </summary>
        public enum NavMeshAgentDriveState
        {
            /// <summary>
            /// 네브 메쉬 에이전트를 활성화할 수 없는 경우
            /// </summary>
            NavMeshAgentInvalid,
            
            /// <summary>
            /// 지정한 위치에 도달 할 수 없는 경우
            /// </summary>
            Unreachable,
            
            /// <summary>
            /// 지정할 패스가 유효하지 않은 경우
            /// </summary>
            InvalidPath,
            
            /// <summary>
            /// 지정한 위치에 도달 할 수 있는 경우
            /// </summary>
            Reachable,
                        
            /// <summary>
            /// 지정한 위치가 현재 목표 위치와 같은 경우
            /// </summary>
            SameDestination,
                                    
            /// <summary>
            /// 지정한 위치가 현재 위치와 같은 경우
            /// </summary>
            SamePosition,
            
            /// <summary>
            /// 지정한 위치가 예약된 경우
            /// </summary>
            Reserved,
            
            /// <summary>
            /// 지정한 위치가 Off-Mesh Link 였던 경우
            /// </summary>
            JumpGateRecognition,
            
            /// <summary>
            /// 대상 유닛이 현재 움직일 수 있는 상태가 아닌 경우
            /// </summary>
            MasterNodeCantMove,
            
            /// <summary>
            /// 좌표 보정에 실패한 경우
            /// </summary>
            CorrectSurfaceFailed,
        }

        /// <summary>
        /// 길찾기 모듈에 의한 이동 적용 이후 결과 타입
        /// </summary>
        public enum UpdateAutonomyPhysicsResult
        {
            /// <summary>
            /// 다음 예약된 길찾기로 상태를 넘긴다.
            /// </summary>
            CheckNextMoveDestination,
            
            /// <summary>
            /// 길찾기를 수행하지 않는다.
            /// </summary>
            DoNothing,
            
            /// <summary>
            /// 길찾기를 속행한다.
            /// </summary>
            ProgressNavMeshControl,
        }

        #endregion

        #region <Constructor>

        static PhysicsTool()
        {
            _NonAllocRayCast = new RaycastHit[512];
            _NonAllocCollider = new Collider[512];
            _AccelerationTypeEnumerator = SystemTool.GetEnumEnumerator<AccelerationType>(SystemTool.GetEnumeratorType.GetAll);
            _DampingTarget_AccelerationTypeEnumerator = _AccelerationTypeEnumerator
                .Where(type => type != AccelerationType.Gravity).ToArray();
        }

        public static void Init()
        {
        }

        #endregion
        
        #region <Methods>

        /// <summary>
        /// 지정한 게임 오브젝트에 네브메쉬 에이전트 컴포넌트를 비활성화 상태로 추가하는 메서드
        /// </summary>
        public static NavMeshAgent AddNavMeshAgentSafe(this GameObject p_GameObject)
        {
            // 네브메쉬 에이전트 생성시에는 위치가 가장 가까운 네브메쉬 서피스로 고정되므로
            // 컴포넌트를 추가하기 전에 현재 위치정보를 저장하고 게임오브젝트를 비활성화한 상태에서 컴포넌트 추가후 좌표를 복구시키고
            // 길찾기 에이전트를 비활성화 시킨 뒤, 게임오브젝트를 재활성화 시켜준다.
            var tryAffine = p_GameObject.transform;
            var prevPos = tryAffine.position;
            p_GameObject.SetActiveSafe(false);
            var result = p_GameObject.AddComponent<NavMeshAgent>();
            result.enabled = false;
            tryAffine.position = prevPos;
            p_GameObject.SetActiveSafe(true);

            return result;
        }

        /// <summary>
        /// 지정한 좌표가 네브메쉬 서피스인지 검증하는 메서드
        /// </summary>
        public static (bool, Vector3) IsNavMeshSurface(this Vector3 p_SourcePosition, float p_SampleRadius = 1f)
        {
            return (NavMesh.SamplePosition(p_SourcePosition, out var o_navMeshHit, p_SampleRadius, NavMesh.AllAreas), o_navMeshHit.position - p_SourcePosition);
        }
        
        /// <summary>
        /// 지정한 좌표를 기준으로 일정한 반경 안의 랜덤한 한 좌표를 샘플링하여 리턴한다.
        /// 2번째 파라미터가 참이라면 네브메쉬 서피스를 찾고 거짓이라면 Terrain 레이어를 찾는다.
        /// </summary>
        public static Vector3 GetRandomNavMeshSurfacePosition(this Vector3 p_SourcePosition, float p_SampleRadius, bool p_SampleNavMeshSurface)
        {
            if (p_SampleNavMeshSurface)
            {
                NavMesh.SamplePosition(p_SourcePosition, out var o_Hit, p_SampleRadius, NavMesh.AllAreas);
                return o_Hit.position;
            }
            else
            {
                var (valid, surfaceSourcePosition) = ObjectDeployTool.CorrectAffinePreset(GameManager.Terrain_LayerMask,
                p_SourcePosition + Random.insideUnitSphere * p_SampleRadius, ObjectDeployTool.ObjectDeploySurfaceDeployType.ForceSurface);
                return surfaceSourcePosition;
            }
        }

        /// <summary>
        /// 레이캐스팅의 결과셋이 지정한 아핀객체 이외의 것을 포함하는지 검증하는 메서드
        /// </summary>
        public static bool IsAnyAffine_ExistAt_RayCastResult_Except(Transform p_Affine, int p_Count)
        {
            for (int i = 0; i < p_Count; i++)
            {
                if (!ReferenceEquals(p_Affine, _NonAllocRayCast[i].transform))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 오버랩의 결과셋이 지정한 아핀객체 이외의 것을 포함하는지 검증하는 메서드
        /// </summary>
        public static bool IsAnyAffine_ExistAt_OverlapResult_Except(Transform p_Affine, int p_Count)
        {
            for (int i = 0; i < p_Count; i++)
            {
                if (!ReferenceEquals(p_Affine, _NonAllocCollider[i].transform))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region <Structs>

        /// <summary>
        /// 길찾기 오브젝트의 정지거리 계산을 추상화한 프리셋
        /// </summary>
        public struct AutonomyPathStoppingDistancePreset
        {
            #region <Const>

#if SERVER_DRIVE
            public const float __DEFAULT_STOPPING__DISTANCE = 0f;
#else
            public const float __DEFAULT_STOPPING__DISTANCE = 0.05f;
#endif
            
            public static AutonomyPathStoppingDistancePreset GetStoppingRange(Unit p_RangeObject)
            {
                return new AutonomyPathStoppingDistancePreset
                {
                    _AutonomyPathStoppingDistanceType = AutonomyPathStoppingDistanceType.RadiusStopping,
                    _PivotRangeObject = p_RangeObject._RangeObject,
                };
            }
            
            public static AutonomyPathStoppingDistancePreset GetStoppingRange(IVirtualRange p_RangeObject)
            {
                return new AutonomyPathStoppingDistancePreset
                {
                    _AutonomyPathStoppingDistanceType = AutonomyPathStoppingDistanceType.RadiusStopping,
                    _PivotRangeObject = p_RangeObject,
                };
            }
            
            public static AutonomyPathStoppingDistancePreset GetStoppingRange(float p_MinDistance, float p_MaxDistance)
            {
                return new AutonomyPathStoppingDistancePreset
                {
                    _AutonomyPathStoppingDistanceType = AutonomyPathStoppingDistanceType.InterleaveStopping,
                    _MinDistance = p_MinDistance,
                    _MaxDistance = p_MaxDistance
                };
            }
            
            public static AutonomyPathStoppingDistancePreset GetStoppingRange(IVirtualRange p_RangeObject, float p_MinDistance, float p_MaxDistance)
            {
                return new AutonomyPathStoppingDistancePreset
                {
                    _AutonomyPathStoppingDistanceType = AutonomyPathStoppingDistanceType.RadiusInterleaveStopping,
                    _PivotRangeObject = p_RangeObject,
                    _MinDistance = p_MinDistance,
                    _MaxDistance = p_MaxDistance
                };
            }
         
            public static AutonomyPathStoppingDistancePreset GetStoppingRange(Unit p_RangeObject, Unit p_TargetObject)
            {
                return new AutonomyPathStoppingDistancePreset
                {
                    _AutonomyPathStoppingDistanceType = AutonomyPathStoppingDistanceType.HyperRadiusStopping,
                    _PivotRangeObject = p_RangeObject._RangeObject,
                    _TargetRangeObject = p_TargetObject._RangeObject,
                };
            }

            public static AutonomyPathStoppingDistancePreset GetStoppingRange(IVirtualRange p_RangeObject, IVirtualRange p_TargetObject)
            {
                return new AutonomyPathStoppingDistancePreset
                {
                    _AutonomyPathStoppingDistanceType = AutonomyPathStoppingDistanceType.HyperRadiusStopping,
                    _PivotRangeObject = p_RangeObject,
                    _TargetRangeObject = p_TargetObject,
                };
            }
                        
            public static AutonomyPathStoppingDistancePreset GetStoppingRange(Unit p_RangeObject, Unit p_TargetObject, float p_MinDistance, float p_MaxDistance)
            {
                return new AutonomyPathStoppingDistancePreset
                {
                    _AutonomyPathStoppingDistanceType = AutonomyPathStoppingDistanceType.HyperRadiusInterleaveStopping,
                    _PivotRangeObject = p_RangeObject._RangeObject,
                    _TargetRangeObject = p_TargetObject._RangeObject,
                    _MinDistance = p_MinDistance,
                    _MaxDistance = p_MaxDistance
                };
            }
            
            public static AutonomyPathStoppingDistancePreset GetStoppingRange(IVirtualRange p_RangeObject, IVirtualRange p_TargetObject, float p_MinDistance, float p_MaxDistance)
            {
                return new AutonomyPathStoppingDistancePreset
                {
                    _AutonomyPathStoppingDistanceType = AutonomyPathStoppingDistanceType.HyperRadiusInterleaveStopping,
                    _PivotRangeObject = p_RangeObject,
                    _TargetRangeObject = p_TargetObject,
                    _MinDistance = p_MinDistance,
                    _MaxDistance = p_MaxDistance
                };
            }
            
            #endregion
            
            #region <Fields>

            private AutonomyPathStoppingDistanceType _AutonomyPathStoppingDistanceType;
            private IVirtualRange _PivotRangeObject;
            private IVirtualRange _TargetRangeObject;
            private float _MinDistance;
            private float _MaxDistance;
            
            #endregion

            #region <Enums>

            private enum AutonomyPathStoppingDistanceType
            {
                None,
                RadiusStopping,
                InterleaveStopping,
                RadiusInterleaveStopping,
                HyperRadiusStopping,
                HyperRadiusInterleaveStopping
            }

            #endregion

            #region <Methods>

            public float GetStoppingDistance()
            {
                switch (_AutonomyPathStoppingDistanceType)
                {
                    case AutonomyPathStoppingDistanceType.RadiusStopping:
                        return _PivotRangeObject.Radius.CurrentValue - CustomMath.Epsilon;
                    case AutonomyPathStoppingDistanceType.InterleaveStopping:
                        return Random.Range(_MinDistance, _MaxDistance) - CustomMath.Epsilon;
                    case AutonomyPathStoppingDistanceType.RadiusInterleaveStopping:
                        return _PivotRangeObject.Radius.CurrentValue + Random.Range(_MinDistance, _MaxDistance) - CustomMath.Epsilon;
                    case AutonomyPathStoppingDistanceType.HyperRadiusStopping:
                        return _PivotRangeObject.Radius.CurrentValue + _TargetRangeObject.Radius.CurrentValue - CustomMath.Epsilon;
                    case AutonomyPathStoppingDistanceType.HyperRadiusInterleaveStopping:
                        return _PivotRangeObject.Radius.CurrentValue + Random.Range(_MinDistance, _MaxDistance) + _TargetRangeObject.Radius.CurrentValue - CustomMath.Epsilon;
                    default:
                    case AutonomyPathStoppingDistanceType.None:
                        return __DEFAULT_STOPPING__DISTANCE;
                }
            }

            #endregion
        }

        /// <summary>
        /// 유닛 외력 프리셋
        /// </summary>
        public struct UnitAddForceParams
        {
            #region <Fields>

            public UnitAddForceParamsType _UnitAddForceParamsType;
            public int UnitAddForceRecordIndex;
            public UnitAddForceData.TableRecord Record;
            public TransformTool.AffineCachePreset PrevAffine;
            public Unit PivotTrigger;
            public Vector3 PivotPosition;
            public bool IsReentered;

            #endregion

            #region <Enums>

            public enum UnitAddForceParamsType
            {
                None,
                Forced,
                TriggeredUnit,
                TriggeredPivot,
            }

            #endregion
            
            #region <Constructors>

            public UnitAddForceParams(int p_UnitAddForceRecordIndex, Unit p_ForcedUnit)
            {
                _UnitAddForceParamsType = p_UnitAddForceRecordIndex != default
                    ? UnitAddForceParamsType.Forced
                    : UnitAddForceParamsType.None;
                
                UnitAddForceRecordIndex = p_UnitAddForceRecordIndex;
                Record = UnitAddForceData.GetInstanceUnSafe[UnitAddForceRecordIndex];
                PrevAffine = new TransformTool.AffineCachePreset(p_ForcedUnit, false);
                PivotTrigger = p_ForcedUnit;
                PivotPosition = p_ForcedUnit._Transform.position;
                IsReentered = default;
            }
            
            public UnitAddForceParams(int p_UnitAddForceRecordIndex, Unit p_ForcedUnit, Unit p_PivotTrigger)
            {
                _UnitAddForceParamsType = p_UnitAddForceRecordIndex != default
                    ? UnitAddForceParamsType.TriggeredUnit
                    : UnitAddForceParamsType.None;
                
                UnitAddForceRecordIndex = p_UnitAddForceRecordIndex;
                Record = UnitAddForceData.GetInstanceUnSafe[UnitAddForceRecordIndex];
                PrevAffine = new TransformTool.AffineCachePreset(p_ForcedUnit, false);
                PivotTrigger = p_PivotTrigger;
                PivotPosition = p_PivotTrigger._Transform.position;
                IsReentered = default;
            }
            
            public UnitAddForceParams(int p_UnitAddForceRecordIndex, Unit p_ForcedUnit, Vector3 p_PivotPosition)
            {
                _UnitAddForceParamsType = p_UnitAddForceRecordIndex != default
                    ? UnitAddForceParamsType.TriggeredPivot
                    : UnitAddForceParamsType.None;
                
                UnitAddForceRecordIndex = p_UnitAddForceRecordIndex;
                Record = UnitAddForceData.GetInstanceUnSafe[UnitAddForceRecordIndex];
                PrevAffine = new TransformTool.AffineCachePreset(p_ForcedUnit, false);
                PivotTrigger = default;
                PivotPosition = p_PivotPosition;
                IsReentered = default;
            }

            #endregion

            #region <Methods>

            public bool IsValid() => _UnitAddForceParamsType != UnitAddForceParamsType.None;
            
            public void UpdateAffine(Unit p_Masetr)
            {
                IsReentered = true;
                PrevAffine = new TransformTool.AffineCachePreset(p_Masetr, false);
            }

            public bool CheckBoundDistance(Unit p_MasterNode)
            {
                switch (_UnitAddForceParamsType)
                {
                    case UnitAddForceParamsType.Forced:
                    case UnitAddForceParamsType.TriggeredPivot:
                    {
                        return p_MasterNode.GetSqrDistanceTo(PivotPosition) < Mathf.Pow(p_MasterNode.GetRadius() + Record.PivotDistanceBound, 2f);
                    }
                    case UnitAddForceParamsType.TriggeredUnit:
                    {
                        return p_MasterNode.GetSqrDistanceTo(PivotPosition) < Mathf.Pow(p_MasterNode.GetRadius() + Record.PivotDistanceBound + PivotTrigger.GetRadius(), 2f);
                    }
                    default:
                        return false;
                }
            }

            #endregion
        }

        #endregion
    }
}