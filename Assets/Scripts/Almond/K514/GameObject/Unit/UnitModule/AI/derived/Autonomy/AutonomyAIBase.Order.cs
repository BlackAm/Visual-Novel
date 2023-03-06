using UnityEngine;

namespace k514
{
    public partial class AutonomyAIBase
    {
        #region <Field>

        /// <summary>
        /// 해당 사고모듈이 이동중일때, 다른 상태로의 전이를 블록하는 플래그
        /// </summary>
        protected (bool t_ForceFlag, PhysicsTool.AutonomyPathStoppingDistancePreset t_StoppingDistancePreset) _ForceMovePreset;

        #endregion

        #region <Mehtods>
        
        /// <summary>
        /// 지정한 위치로 이동 명령을 내리는 메서드
        /// </summary>
        public override bool MoveTo(Vector3 p_TargetPosition, bool p_ForceMove, PhysicsTool.AutonomyPathStoppingDistancePreset p_AutonomyPathStoppingDistancePreset)
        { 
            return SwitchStateMove(p_TargetPosition, p_ForceMove, true, p_AutonomyPathStoppingDistancePreset);
        }

        /// <summary>
        /// 인공지능을 대기상태로 전이시키는 메서드
        /// </summary>
        public override void Idle(ActableTool.IdleState p_IdleType)
        {
            base.Idle(p_IdleType);
            ChangeAIState(ThinkableTool.AIState.Idle);
        }

        /// <summary>
        /// 해당 사고 모듈이 활성화되었을 때의 최초 위치로 이동시키는 메서드
        /// </summary>
        public override bool ReturnPosition(bool p_ForceMove, bool p_SwitchInstance)
        {
            return SwitchStateMove(_OriginPivot, p_ForceMove, p_SwitchInstance, PhysicsTool.AutonomyPathStoppingDistancePreset.GetStoppingRange(_MasterNode));
        }

        /// <summary>
        /// 지정한 유닛 반대 방향으로 일정 거리를 이동시키는 메서드
        /// </summary>
        public void SetRunAway(Unit p_FromUnit)
        {
            var runDV = p_FromUnit._Transform.GetDirectionUnitVectorTo(_MasterNode._Transform);
            runDV = runDV.VectorRotationUsingQuaternion(Vector3.up, Random.Range(-30f, 30f));
            var runDist = _MasterNode.GetRadius(2f) + Random.Range(0.5f * _WanderingRadius, _WanderingRadius);
            var runPos = _MasterNode._Transform.position + runDist * runDV;
            MoveTo(runPos, false, PhysicsTool.AutonomyPathStoppingDistancePreset.GetStoppingRange(_MasterNode));
        }
        
        #endregion
    }
}