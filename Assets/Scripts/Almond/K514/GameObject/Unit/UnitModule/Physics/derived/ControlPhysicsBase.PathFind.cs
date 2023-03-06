using UnityEngine;
using UnityEngine.AI;

namespace k514
{
    public abstract partial class ControlPhysicsBase
    {
        #region <Callbacks>

        /// <summary>
        /// 현재 길찾기 모듈에 의해 점프 관련 체공 로직이 수행중인지 검증하는 논리메서드
        /// </summary>
        public virtual bool OnUpdatePhysicsAutonomyJump(float p_DeltaTime)
        {
            return IsGround();
        }

        #endregion

        #region <Methods>

        /// <summary>
        /// 현재 물리 모듈이 길찾기를 포함하는지 여부를 리턴하도록 구현
        /// </summary>
        public virtual bool IsAutonomyValid(bool p_CheckEnable)
        {
            return false;
        }
        
        /// <summary>
        /// 길찾기 모듈의 속도배율을 지정하는 메서드
        /// </summary>
        public virtual void SetPhysicsAutonomySpeed(float p_Rate)
        {
        }

        /// <summary>
        /// 길찾기를 종료시키는 메서드
        /// </summary>
        public virtual void ClearPhysicsAutonomyMove()
        {
        }

        /// <summary>
        /// 현재 길찾기 모듈의 패스 정보를 가져오는 메서드
        /// </summary>
        public virtual NavMeshPath GetPath()
        {
            return null;
        }

        public virtual bool CalcPath(Vector3 p_TargetPosition)
        {
            return false;
        }

        public virtual void ShowPath(bool p_Valid, Vector3 p_FromVector)
        {
        }

        /// <summary>
        /// 지정한 패스로 길찾기 모듈의 동작시키는 메서드
        /// </summary>
        public virtual (bool, PhysicsTool.NavMeshAgentDriveState) SetPath(NavMeshPath p_Path, PhysicsTool.AutonomyPathStoppingDistancePreset p_AutonomyPathStoppingDistancePreset)
        {
            return (false, PhysicsTool.NavMeshAgentDriveState.Unreachable);
        }

        /// <summary>
        /// 지정한 위치로의 길찾기를 수행시키고 이동하게 하는 메서드
        /// </summary>
        public virtual (bool, PhysicsTool.NavMeshAgentDriveState) SetPhysicsAutonomyMove(Vector3 p_Position, ObjectDeployTool.ObjectDeploySurfaceDeployType p_ObjectDeploySurfaceDeployType, PhysicsTool.AutonomyPathStoppingDistancePreset p_AutonomyPathStoppingDistancePreset)
        {
            return (false, PhysicsTool.NavMeshAgentDriveState.Unreachable);
        }

        /// <summary>
        /// 현재 길찾기 이후 다음으로 이동할 지점을 추가하는 메서드
        /// </summary>
        public virtual (bool, PhysicsTool.NavMeshAgentDriveState) AddManualMovePathEdge(Vector3 p_Edge, ObjectDeployTool.ObjectDeploySurfaceDeployType p_ObjectDeploySurfaceDeployType)
        {
            return (false, PhysicsTool.NavMeshAgentDriveState.Unreachable);
        }

        /// <summary>
        /// 현재 길찾기 이후 다음으로 이동할 지점을 다수 추가하는 메서드
        /// </summary>
        public virtual (bool, PhysicsTool.NavMeshAgentDriveState) SetManualMovePath(Vector3[] p_Path, ObjectDeployTool.ObjectDeploySurfaceDeployType p_ObjectDeploySurfaceDeployType, PhysicsTool.AutonomyPathStoppingDistancePreset p_AutonomyPathStoppingDistancePreset)
        {
            return (false, PhysicsTool.NavMeshAgentDriveState.Unreachable);
        }

        /// <summary>
        /// 다음 길찾기 지점이 예약되었는지 여부를 리턴하는 논리메서드
        /// </summary>
        public virtual bool HasReservedDestination()
        {
            return false;
        }

        /// <summary>
        /// 현재 길찾기 모듈이 동작중인지 여부를 리턴하는 논리메서드
        /// </summary>
        public virtual bool IsAutonomyMoving()
        {
            return false;
        }

        #endregion
    }
}