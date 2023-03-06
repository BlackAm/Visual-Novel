using UnityEngine;
using Random = UnityEngine.Random;

namespace k514
{
    /// <summary>
    /// 근접 공격을 수행하는 AI
    /// </summary>
    public class AutonomyAI_Melee : AutonomyAIBase
    {
        #region <Callbacks>

        protected override void OnUpdateAIState()
        {
            // SwitchState~~ 계열 함수는, 해당 Update 콜백 외의 호출점이 있는 기능을 정의하는 함수
            // 아래의 스위치문은 특정 상태에서 매 프레임 당 수행해야할 코드를 기술함
            switch (_CurrentAIState)
            {
                case ThinkableTool.AIState.Idle:
                    if (IsAIStatusTransitionDelayOver(false))
                    {
                        SwitchStateNotice(true, true);
                    }
                    break;
                /*case ThinkableTool.AIState.Notice:
                    if (!HasEnemy() && !FindEnemy(ThinkableTool.AIState.Notice))
                    {
                        if (_AIExtraFlagMask.HasAnyFlagExceptNone(ThinkableTool.AIExtraFlag.ReturnHome))
                        {
                            SwitchStateMove(false, true);
                        }
                        else
                        {
                            SwitchStateIdle(ActableTool.IdleState.Relax);
                        }
                    }
                    else
                    {
                        SwitchStateNotice(false, false);
                    }
                    break;
                case ThinkableTool.AIState.Trace:
                    if (!HasEnemy() && !FindEnemy(ThinkableTool.AIState.Trace))
                    {
                        SwitchStateNotice(true, true);
                    }
                    else
                    {
                        SwitchStateTrace(false, false);
                    }
                    break;
                case ThinkableTool.AIState.Attack:
                    if (!HasEnemy() && !FindEnemy(ThinkableTool.AIState.Attack))
                    {
                        if (!_MasterNode.HasState_Or(Unit.UnitStateType.DRIVESKILL))
                        {
                            if (FindEnemy(ThinkableTool.AIState.Trace))
                            {
                                SwitchStateTrace(true, true);
                            }
                            else
                            {
                                SwitchStateNotice(true, true);
                            }
                        }
                    }
                    else
                    {
                        if(IsTargetInnerAttackRange(true))
                        {
                            SwitchStateAttack(false, false, false, false);
                        }
                        else
                        {
                            SwitchStateTrace(true, true);
                        }
                    }
                    break;*/
                case ThinkableTool.AIState.Move:
                        SwitchStateMove(false, false);
                    
                    break;
            }
        }

        public override PhysicsTool.UpdateAutonomyPhysicsResult OnUpdateAutonomyPhysics(float p_RemainingDistance, float p_StoppingDistance)
        {
            switch (_CurrentAIState)
            {
                /*case ThinkableTool.AIState.Trace:
                {
                    var tryAttackRangeSqr = GetFocusBoundSqrRange(ThinkableTool.AIState.Attack);
                    
                    /* 길찾기 목적지에 도달한 경우 #1#
                    // 길찾기 도중에 타겟이 이동할 수 있기 때문에 목적지에 도달했다고 해도
                    // 반드시 공격 범위 안에 타겟이 있는 것은 아니므로, 거리 비교를 해준다.
                    if (p_RemainingDistance < p_StoppingDistance)
                    {
                        // 그러나, 인공지능의 사고 패턴에 따라 일단 목적지에 도달했다면 주변에 적이 있던 없던 공격 패턴을 수행할 수도 있다.
                        // 해당 공격 확률은 신중함(Carefulness)으로 기술되며 그 값이 0에 가까울수록 공격을 수행한다.
                        if (_MindRecord.Carefulness > Random.value)
                        {
                            // 신중함 패턴 1 : 거리를 재고 공격을 하거나, 길찾기(Trace)를 수행한다.
                            if (_MasterNode.CompareSqrDistanceFromFocus(true, tryAttackRangeSqr))
                            {
                                SwitchStateAttack(true, true, false, false);
                                return PhysicsTool.UpdateAutonomyPhysicsResult.DoNothing;
                            }
                            else
                            {
                                if (SwitchStateTrace(true, true))
                                {
                                    return PhysicsTool.UpdateAutonomyPhysicsResult.ProgressNavMeshControl;
                                }
                                else
                                {
                                    return PhysicsTool.UpdateAutonomyPhysicsResult.DoNothing;
                                }
                            }
                        }
                        else
                        {
                            // 신중함 패턴 2 : 일단 공격한다.
                            SwitchStateAttack(true, true, false, false);
                            return PhysicsTool.UpdateAutonomyPhysicsResult.DoNothing;
                        }
                    }
                    /* 길찾기 도중 타겟이 공격 범위에 들어온 경우 #1#
                    // 2번째 절에서 매번 해당 유닛과 타겟간의 제곱거리를 갱신하기 때문에 코스트가 크다.
                    // 파라미터를 false로 바꾸면 코스트가 크게 줄어들지만, 공격 이후에 거리가 갱신되면 다시 자리를 옮기는 등의
                    // 난잡한 패턴을 보일 수 있다.
                    else if(_MasterNode.CompareSqrDistanceFromFocus(true, tryAttackRangeSqr))
                    {
                        SwitchStateAttack(true, true, false, false);
                        return PhysicsTool.UpdateAutonomyPhysicsResult.DoNothing;
                    }
                    /* 공격 대상이 범위안에 없는 경우 #1#
                    else
                    {
                        if (_AIExtraFlagMask.HasAnyFlagExceptNone(ThinkableTool.AIExtraFlag.JunkYardDog))
                        {
                            if (SwitchStateTrace(false, true))
                            {
                                return PhysicsTool.UpdateAutonomyPhysicsResult.ProgressNavMeshControl;
                            }
                            else
                            {
                                return PhysicsTool.UpdateAutonomyPhysicsResult.DoNothing;
                            }
                        }
                        else
                        {
                            /* SE Cond #1#
                            // 1. 현재 길찾기 로직이 유효한 경우
                            // 2. 현재 강제 추적 대상이 없고, 대상이 추적범위를 벗어난 경우
                            // 3. 현재 유닛이 최초 스폰 지점으로부터 귀환 거리를 벗어난 경우
                            var hasReturnHomeFlag = _AIExtraFlagMask.HasAnyFlagExceptNone(ThinkableTool.AIExtraFlag.ReturnHome);
                            if (!float.IsInfinity(p_RemainingDistance)
                                && ((!HasForceFocus() && GetSelfBoundRange() < p_RemainingDistance)
                                    || (hasReturnHomeFlag && GetSelfBoundSqrRange(ThinkableTool.AIState.Move) < _OriginPivot.GetSqrDistanceTo(_MasterNode._Transform.position)) && !float.IsInfinity(p_RemainingDistance))
                                )
                            {
                                if (hasReturnHomeFlag)
                                {
                                    ReturnPosition(false, false);
                                }
                                else
                                {
                                    OnBreakAutonomyPath();
                                }
                                
                                return PhysicsTool.UpdateAutonomyPhysicsResult.DoNothing;
                            }
                            else
                            {
                                if (SwitchStateTrace(false, true))
                                {
                                    return PhysicsTool.UpdateAutonomyPhysicsResult.ProgressNavMeshControl;
                                }
                                else
                                {
                                    return PhysicsTool.UpdateAutonomyPhysicsResult.DoNothing;
                                }
                            }
                        }
                    }
                }*/
                default:
                case ThinkableTool.AIState.Move:
                    return base.OnUpdateAutonomyPhysics(p_RemainingDistance, p_StoppingDistance);
            }
        }
        
        public override void OnAutonomyPhysicsPathOver()
        {
            _ForceMovePreset = default;
            SwitchStateIdle(ActableTool.IdleState.Relax);
        }
        
        #endregion
    }
}