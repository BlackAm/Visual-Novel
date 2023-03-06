using System.Collections.Generic;
using System.Linq;
using UI2020;
using UnityEngine;

namespace k514
{
    public partial class AutonomyAIBase
    {
        #region <Consts>

        private const float PathFindRequestRandomLowerRate = 0.66f;

        #endregion
        
        #region <Callbacks>
        
        public override void OnUnitHitActionStarted()
        {
        }
        
        public override void OnUnitHitActionTerminated()
        {
            RandomizeCurrentAIDelay();
        }

        public override void OnUnitActionStarted()
        {
        }
      
        /// <summary>
        /// 예약했던 스펠이 발동하는 경우
        /// </summary>
        public override void OnUnitActionTransitioned(ControllerTool.CommandType p_PrevCommandType, ControllerTool.CommandType p_CurrentCommandType)
        {
            if (_ReservedCommand == p_CurrentCommandType)
            {
                // OnSuccessActiveSpell();
            }
        }
        
        public override void OnUnitActionTerminated()
        {
        }

        public override void OnJumpUp()
        {
        }
        
        /// <summary>
        /// 적이 도망치는데 성공한 경우 호출되는 콜백
        /// </summary>
        protected void OnOverlookLoser()
        {
            // 적을 잊고 귀환한다.
            SwitchStateMove(false, false);
        }

        #endregion
        
        #region <Methods>

        /*protected void UpdateTraceOffsetVector(ThinkableTool.AIState p_Type)
        {
            UpdateTraceOffsetVector(_StatePresetRecord[p_Type].Range);
        }
        
        protected void UpdateTraceOffsetVector(float p_Offset)
        {
            if (HasForceFocus())
            {
                var targetUnit = _MasterNode.FocusNode.Node;
                if (targetUnit.IsPositionChanged())
                {
                    switch (_AITracePivotSelectType)
                    {
                        case ThinkableTool.AITracePivotSelectType.TargetCenter:
                        {
                            _TraceOffsetVector = Vector3.zero;
                            _StoppingDistancePreset = PhysicsTool.AutonomyPathStoppingDistancePreset.GetStoppingRange(_MasterNode, targetUnit, PathFindRequestRandomLowerRate * p_Offset, p_Offset);
                        }
                            break;
                        case ThinkableTool.AITracePivotSelectType.None:
                        case ThinkableTool.AITracePivotSelectType.TargetForward:
                        {
                            var targetRange = _MasterNode.GetDistanceLowerBoundFromFocus();
                            var direction = targetUnit._Transform.forward;
                            _TraceOffsetVector = Random.Range(targetRange + PathFindRequestRandomLowerRate * p_Offset, targetRange + p_Offset) * direction;
                            _StoppingDistancePreset = default;
                        }
                            break;
                        case ThinkableTool.AITracePivotSelectType.DirectionTargetToThis:
                        {
                            var targetRange = _MasterNode.GetDistanceLowerBoundFromFocus();
                            var direction = _MasterNode.GetFocusUV();
                            _TraceOffsetVector = -Random.Range(targetRange + PathFindRequestRandomLowerRate * p_Offset, targetRange + p_Offset) * direction;
                            _StoppingDistancePreset = default;
                        }
                            break;
                        case ThinkableTool.AITracePivotSelectType.DirectionThisToTarget:
                        {
                            var targetRange = _MasterNode.GetDistanceLowerBoundFromFocus();
                            var direction = targetUnit.GetDirectionUnitVectorTo(_MasterNode);
                            _TraceOffsetVector = -Random.Range(targetRange + PathFindRequestRandomLowerRate * p_Offset, targetRange + p_Offset) * direction;
                            _StoppingDistancePreset = default;
                        }
                            break;
                        case ThinkableTool.AITracePivotSelectType.RandomInRadius:
                        {
                            var targetRange = _MasterNode.GetDistanceLowerBoundFromFocus();
                            _TraceOffsetVector = CustomMath.RandomSymmetricVector(CustomMath.XYZType.ZX, targetRange, targetRange + p_Offset);
                            _StoppingDistancePreset = default;
                        }
                            break;
                    }
                }
                else
                {
                    switch (_AITracePivotSelectTypeWhenTargetMoving)
                    {
                        case ThinkableTool.AITracePivotSelectType.TargetCenter:
                        {
                            _TraceOffsetVector = Vector3.zero;
                            _StoppingDistancePreset = PhysicsTool.AutonomyPathStoppingDistancePreset.GetStoppingRange(_MasterNode, targetUnit, PathFindRequestRandomLowerRate * p_Offset, p_Offset);
                        }
                            break;
                        case ThinkableTool.AITracePivotSelectType.None:
                        case ThinkableTool.AITracePivotSelectType.TargetForward:
                        {
                            var targetRange = _MasterNode.GetDistanceLowerBoundFromFocus();
                            var direction = targetUnit._Transform.forward;
                            _TraceOffsetVector = Random.Range(targetRange + PathFindRequestRandomLowerRate * p_Offset, targetRange + p_Offset) * direction;
                            _StoppingDistancePreset = default;
                        }
                            break;
                        case ThinkableTool.AITracePivotSelectType.DirectionTargetToThis:
                        {
                            var targetRange = _MasterNode.GetDistanceLowerBoundFromFocus();
                            var direction = _MasterNode.GetFocusUV();
                            _TraceOffsetVector = -Random.Range(targetRange + PathFindRequestRandomLowerRate * p_Offset, targetRange + p_Offset) * direction;
                            _StoppingDistancePreset = default;
                        }
                            break;
                        case ThinkableTool.AITracePivotSelectType.DirectionThisToTarget:
                        {
                            var targetRange = _MasterNode.GetDistanceLowerBoundFromFocus();
                            var direction = targetUnit.GetDirectionUnitVectorTo(_MasterNode);
                            _TraceOffsetVector = -Random.Range(targetRange + PathFindRequestRandomLowerRate * p_Offset, targetRange + p_Offset) * direction;
                            _StoppingDistancePreset = default;
                        }
                            break;
                        case ThinkableTool.AITracePivotSelectType.RandomInRadius:
                        {
                            var targetRange = _MasterNode.GetDistanceLowerBoundFromFocus();
                            _TraceOffsetVector = CustomMath.RandomSymmetricVector(CustomMath.XYZType.ZX, targetRange, targetRange + p_Offset);
                            _StoppingDistancePreset = default;
                        }
                            break;
                    }
                }
            }
            else
            {
                _TraceOffsetVector = Vector3.zero;
            }
        }*/

        protected void ClearTraceOffsetVector()
        {
            _TraceOffsetVector = Vector3.zero;
            _StoppingDistancePreset = default;
        }
        
        protected bool HasForceFocus()
        {
            return _MasterNode.FocusNode.CheckNode(PrefabInstanceTool.FocusNodeRelateType.ForceEnemy);
        }

        protected ThinkableTool.AIUnitFindType GetUnitFindType()
        {
            if (_CurrentAIState == ThinkableTool.AIState.Attack)
            {
                var (_, tryFindType) = _MasterNode._ActableObject.GetUnitActionUnitFindType(_ReservedCommand);
                return tryFindType;
            }
            else
            {
                return _MindRecord.DefaultUnitFindType;
            }
        }

        #endregion
    }
}