using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace k514
{
    /// <summary>
    /// 유닛의 물리 법칙을 기술하는 인터페이스
    /// </summary>
    public interface IPhysics : IIncarnateUnit, IVirtualRange
    {
        #region <Physics>

        UnitPhysicsDataRoot.UnitPhysicsType _PhysicsType { get; }
        IPhysicsTableRecordBridge _PhysicsRecord { get; }
        IPhysics OnInitializePhysics(UnitPhysicsDataRoot.UnitPhysicsType p_PhysicsType, Unit p_TargetUnit, IPhysicsTableRecordBridge p_PhysicsPreset);
        Vector3 _CurrentVelocity { get; }
        FloatProperty_Inverse Mass { get; }
        Vector3 _TargetPosition { get; }
        void OnFixedUpdate(float p_DeltaTime);
                
        /// <summary>
        /// 현재 사고 모듈에 의한 체공 물리 로직을 갱신하고, 체공 상태가 종료되었다면 참을 리턴하도록 구현
        /// </summary>
        bool OnUpdatePhysicsAutonomyJump(float p_DeltaTime);
        
        void AddAcceleration(PhysicsTool.AccelerationType p_Type, Vector3 p_Acc);
        void AddVelocity(PhysicsTool.AccelerationType p_Type, Vector3 p_Velocity, PhysicsTool.UnitAddForceParams p_UnitAddForceParams);
        void AddVelocity(PhysicsTool.UnitAddForceParams p_UnitAddForceParams);
        void OverlapVelocity(PhysicsTool.AccelerationType p_Type, Vector3 p_Velocity);
        void ClearVelocity();
        void ClearVelocity(PhysicsTool.AccelerationType p_Type, bool p_CorrectVelocity);
        void MoveTo(Vector3 p_DeltaVelocity, float p_DeltaTime);
        void TryUpdateCollision(Vector3 p_UnitVector);
        bool IsGround();
        
        /// <summary>
        /// 중력 타입을 변경하는 기능을 구현
        /// </summary>
        void SetGravityFlag(PhysicsTool.GravityType p_GravityFlag);
        
        void SetPhysicsScale(float p_ScaleRate);
        void SyncPhysicsScale();

        #endregion

        #region <PathFind>

        /// <summary>
        /// 현재 물리 모듈이 길찾기를 포함하는지 여부를 리턴하도록 구현
        /// </summary>
        bool IsAutonomyValid(bool p_CheckEnable);
        
        /// <summary>
        /// 예약된 목적지가 있는지 검증하도록 구현
        /// </summary>
        bool HasReservedDestination();
        
        /// <summary>
        /// 현재 물리 모듈이 이동중인지 검증하도록 구현
        /// </summary>
        bool IsAutonomyMoving();
        
        /// <summary>
        /// 현재 사고 모듈에 의해 물리 모듈의 속도를 제어할 수 있도록 구현
        /// </summary>
        void SetPhysicsAutonomySpeed(float p_Rate);
 
        /// <summary>
        /// 현재 사고 모듈에 의해 물리 모듈의 0속도를 제어할 수 있도록 구현
        /// </summary>
        void ClearPhysicsAutonomyMove();
        
        /// <summary>
        /// 현재 해당 물리 모듈이 가진 길찾기 경로를 리턴한다.
        /// </summary>
        NavMeshPath GetPath();

        bool CalcPath(Vector3 p_TargetPosition);

        void ShowPath(bool p_Valid, Vector3 p_FromVector);
        
        /// <summary>
        /// NavMeshPath 기반으로 지정한 경로를 해당 길찾기 에이전트에 할당하는 메서드
        /// </summary>
        (bool, PhysicsTool.NavMeshAgentDriveState) SetPath(NavMeshPath p_Path, PhysicsTool.AutonomyPathStoppingDistancePreset p_AutonomyPathStoppingDistancePreset);
         
        /// <summary>
        /// 현재 사고 모듈이 지정하는 방향으로 물리 모듈이 이동할 수 있도록 구현
        /// </summary>
        (bool, PhysicsTool.NavMeshAgentDriveState) SetPhysicsAutonomyMove(Vector3 p_Position, ObjectDeployTool.ObjectDeploySurfaceDeployType p_ObjectDeploySurfaceDeployType, PhysicsTool.AutonomyPathStoppingDistancePreset p_AutonomyPathStoppingDistancePreset);
        
        /// <summary>
        /// 현재 사고 모듈이 다음에 이동할 위치를 지정
        /// </summary>
        (bool, PhysicsTool.NavMeshAgentDriveState) AddManualMovePathEdge(Vector3 p_Edge, ObjectDeployTool.ObjectDeploySurfaceDeployType p_ObjectDeploySurfaceDeployType);

        /// <summary>
        /// 현재 사고 모듈이 지정한 경로를 따라 이동할 수 있도록 구현
        /// </summary>
        (bool, PhysicsTool.NavMeshAgentDriveState) SetManualMovePath(Vector3[] p_Path, ObjectDeployTool.ObjectDeploySurfaceDeployType p_ObjectDeploySurfaceDeployType, PhysicsTool.AutonomyPathStoppingDistancePreset p_AutonomyPathStoppingDistancePreset);
        
        #endregion
    }
}