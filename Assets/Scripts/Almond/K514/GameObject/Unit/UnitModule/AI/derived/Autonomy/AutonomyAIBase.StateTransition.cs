using UnityEngine;

namespace k514
{
    public partial class AutonomyAIBase
    {
        #region <Callbacks>

        protected override void OnAutonomyPathFindFailed()
        {
            _ForceMovePreset = default;
            SwitchStateIdle();
        }

        protected override void OnBreakAutonomyPath()
        {
            OnAutonomyPathFindFailed();
        }

        #endregion
        
        #region <Methods>
        
        /// <summary>
        /// 각 상태에 맞는 랜덤 딜레이 시드를 리턴하는 메서드
        /// </summary>
        protected int GetRandomSeed(ThinkableTool.AIState p_Type)
        {
            switch (p_Type)
            {
                case ThinkableTool.AIState.Notice:
                    return _MasterNode.IsPlayer || HasForceFocus() ? 0 : Random.Range(2, 4);
                case ThinkableTool.AIState.Trace:
                    return _MasterNode.IsPlayer || HasForceFocus() ? 0 : Random.Range(4, 8);
                case ThinkableTool.AIState.Attack:
                    return _MasterNode.IsPlayer ? 0 : 2;
                case ThinkableTool.AIState.Move:
                    return _MasterNode.IsPlayer ? 0 : Random.Range(6, 10);
                default :
                    return 0;
            }
        }

        /// <summary>
        /// 지정한 AI 상태의 진입 딜레이를 지정한 값으로 설정한다.
        /// </summary>
        protected void RandomizeAIDelay(ThinkableTool.AIState p_Type)
        {
            var tryPreset = _StatePresetRecord[p_Type];
            tryPreset.StateTransitionCount = GetRandomSeed(p_Type);
            _StatePresetRecord[p_Type] = tryPreset;
        }

        /// <summary>
        /// 현재 AI 상태의 진입 딜레이를 랜덤한 값으로 설정한다.
        /// </summary>
        protected void RandomizeCurrentAIDelay()
        {
            SetAIDelay(GetRandomSeed(_CurrentAIState));
        }

        /// <summary>
        /// 현재 AI 상태의 진입 딜레이를 지정한 값으로 설정한다.
        /// </summary>
        protected void SetAIDelay(int p_Delay)
        {
            _CurrentAIPreset.StateTransitionCount = p_Delay;
        }

        protected void ClearAIDelay()
        {
            _CurrentAIPreset.StateTransitionCount = 0;
        }

        /// <summary>
        /// 현재 선정된 사고 상태로의 전이 딜레이가 클리어 되었는지 여부를 리턴하는 논리 메서드
        /// </summary>
        protected bool IsAIStatusTransitionDelayOver(bool p_InstantTransitionFlag)
        {
            if (p_InstantTransitionFlag)
            {
                ClearAIDelay();
            }

            var result = _CurrentAIPreset.StateTransitionCount < 1;
            if (result)
            {
                RandomizeCurrentAIDelay();
            }

            return result;
        }

        /// <summary>
        /// 지정한 타입으로 사고 모듈 상태를 전이시키는 메서드
        /// </summary>
        protected void ChangeAIState(ThinkableTool.AIState p_Type)
        {
            if (p_Type != _CurrentAIState)
            {
                _CurrentAIState = p_Type;
#if UNITY_EDITOR
                if (CustomDebug.AIStateName)
                {
                    _MasterNode.SetUnitNameWithTail($"[{_CurrentAIState}]");
                }
#endif
                if (p_Type != ThinkableTool.AIState.None)
                {
                    _CurrentAIPreset = _StatePresetRecord[p_Type];
                }
            }
        }
        
        public override ThinkableTool.AIState GetCurrentAIState()
        {
            return _CurrentAIState;
        }

        /// <summary>
        /// 현재 AI 상태에 맞는 속도 배율을 리턴하는 메서드
        /// </summary>
        public override float GetAISpeedRate()
        {
            return _CurrentAIPreset.SpeedRate;
        }

        #endregion
        
        #region <Method/SwitchState/None>
        
        /// <summary>
        /// 현재 사고 모듈의 상태를 초기화 시키는 메서드
        /// </summary>
        protected void SwitchStateNone(bool p_HideUnit)
        {
            if (p_HideUnit)
            {
                _MasterNode.SetUnitDisable(true);
            }
            
            ChangeAIState(ThinkableTool.AIState.None);
        }

        #endregion

        #region <Method/SwitchState/Idle>
        
        /// <summary>
        /// 현재 사고 모듈을 대기 상태로 전이시키는 메서드
        /// 마지막에 사용한 Idle 타입 모션으로 전이한다.
        /// </summary>
        protected void SwitchStateIdle()
        {
            Idle(_MasterNode._ActableObject._CurrentIdleState);
        }
        
        /// <summary>
        /// 현재 사고 모듈을 대기 상태로 전이시키는 메서드
        /// 파라미터를 통해, Idle 타입을 바꿀 수 있다.
        /// </summary>
        protected void SwitchStateIdle(ActableTool.IdleState p_IdleState)
        {
            Idle(p_IdleState);
        }

        #endregion
        
        #region <Method/SwitchState/Notice>

        /// <summary>
        /// 현재 사고 모듈을 경계 상태로 전이시키는 메서드
        /// </summary>   
        protected bool SwitchStateNotice(bool p_SwitchInstance, bool p_IsFirstEntry)
        {
            /*if (p_IsFirstEntry)
            {
                ChangeAIState(ThinkableTool.AIState.Notice);
                StopMove(ActableTool.IdleState.Combat);
            }

            if (IsAIStatusTransitionDelayOver(p_SwitchInstance))
            {
                if (HasEnemy())
                {
                    if (CompareSqrInRange(ThinkableTool.AIState.Trace, _MasterNode.GetSqrDistanceFromFocus(false), true))
                    {
                        SwitchStateTrace(false, true);
                        return true;
                    }
                    else
                    {
                        // 적을 발견했지만 추적 범위 내에 없는 경우
                        return false;
                    }
                }
                else
                {
                    // 적이 없는 경우
                    return false; 
                }
            }
            else
            {
                // 상태 전이 쿨타임인 경우
                return false;
            }*/

            return false;
        }
     
        #endregion
        
        #region <Method/SwitchState/Trace>

        /// <summary>
        /// 현재 사고 모듈을 추적 상태로 전이시키는 메서드
        /// </summary> 
        protected bool SwitchStateTrace(bool p_SwitchInstance)
        {
            return SwitchStateTrace(p_SwitchInstance, _CurrentAIState != ThinkableTool.AIState.Trace);
        }
        
        /// <summary>
        /// 현재 사고 모듈을 추적 상태로 전이시키는 메서드
        /// </summary>   
        protected bool SwitchStateTrace(bool p_SwitchInstance, bool p_IsFirstEntry)
        {
            /*if (_MasterNode.HasState_Or(Unit.UnitStateType.DRIVESKILL))
            {
                return false;
            }
            else
            {
                // 공격 범위 안에 있는 경우, 즉시 공격 상태로 전이한다.
                if (IsTargetInnerAttackRange(true))
                {
                    SwitchStateAttack(false, false, false);
                    return false;
                }
                else
                {
                    if (p_IsFirstEntry)
                    {
                        ChangeAIState(ThinkableTool.AIState.Trace);
                        UpdateTraceOffsetVector(ThinkableTool.AIState.Attack);
                    }
                    
                    if (IsAIStatusTransitionDelayOver(p_SwitchInstance))
                    {
                        var (moveValid, _) = _MasterNode._PhysicsObject
                            .SetPhysicsAutonomyMove
                            (
                                _MasterNode.GetFocusPosition() + _TraceOffsetVector, 
                                ObjectDeployTool.ObjectDeploySurfaceDeployType.ForceSurfaceUsingParameterVector, 
                                _StoppingDistancePreset
                            );
                    
                        return moveValid;
                    }
                    else
                    {
                        return false;
                    }
                }
            }*/

            return false;
        }

        #endregion

        #region <Method/SwitchState/Attack>
        
        /// <summary>
        /// 현재 사고 모듈을 공격 상태로 전이시키는 메서드
        /// </summary>   
        protected bool SwitchStateAttack(bool p_SwitchInstance, bool p_AllowAggressiveFlag, bool p_RestrictActFlag)
        {
            return SwitchStateAttack(p_SwitchInstance, _CurrentAIState != ThinkableTool.AIState.Attack, p_AllowAggressiveFlag, p_RestrictActFlag);
        }
        
        /// <summary>
        /// 현재 사고 모듈을 공격 상태로 전이시키는 메서드
        /// </summary>   
        protected bool SwitchStateAttack(bool p_SwitchInstance, bool p_IsFirstEntry, bool p_AllowAggressiveFlag, bool p_RestrictActFlag)
        {
            if (p_IsFirstEntry)
            {
                ChangeAIState(ThinkableTool.AIState.Attack);
                RandomizeCurrentAIDelay();
                StopMove(ActableTool.IdleState.Combat);
            }

            if (IsAIStatusTransitionDelayOver(p_SwitchInstance))
            {
                // SE Cond
                // 1. RelaxedSpell이 없어서 언제라도 스킬을 캔슬하고 다른 스킬을 발동할 수 있는 경우
                // 2. 현재 Instant 타입으로 스킬이 예약된 경우
                // 3. 현재 스킬 시전중이 아닌 경우
                if (!_AIExtraFlagMask.HasAnyFlagExceptNone(ThinkableTool.AIExtraFlag.RelaxedSpell) 
                    || _CommandReserveType.HasAnyFlagExceptNone(ThinkableTool.AIReserveCommand.HasInstantEntry)
                    || !_MasterNode.HasState_Or(Unit.UnitStateType.DRIVESKILL))
                {
                    _CommandReserveType.RemoveFlag(ThinkableTool.AIReserveCommand.SpellEntry_Instant_OnceFlag);

                    if (p_AllowAggressiveFlag || _AIExtraFlagMask.HasAnyFlagExceptNone(ThinkableTool.AIExtraFlag.Aggressive))
                    {
                        // 유닛 액션을 요청하고, 실행됬는지 여부를 리턴받음.
                        var (hasActionTransition, resultMessage) = _MasterNode.OnActionTriggered(_ReservedCommand, p_RestrictActFlag);
                        // 성공한 경우의 이벤트 처리 호출은, 해당 함수가 아니라 유닛 액션 모듈로부터 호출된다.
                        // 다만 연속스킬을 처리해야하므로 스킬 예약/발동에 성공한 경우, 스킬 딜레이를 지워준다.
                        if (hasActionTransition)
                        {
                            ClearAIDelay();
                            return true;
                        }
                        // 유닛 액션 시전에 실패한 경우 이벤트를 처리한다.
                        else
                        {
                            RandomizeCurrentAIDelay();
                        }
                    }
                }
            }

            return false;
        }
        
        #endregion
        
        #region <Method/SwitchState/Move>

        /// <summary>
        /// 현재 사고 모듈을 pivotPosition 위치로 이동시키는 SwitchStateMove 기본 메서드
        /// </summary>
        protected bool SwitchStateMove(bool p_SwitchInstance, bool p_IsFirstEntry, PhysicsTool.AutonomyPathStoppingDistancePreset p_AutonomyPathStoppingDistancePreset)
        {
            if (p_IsFirstEntry)
            {
                ChangeAIState(ThinkableTool.AIState.Move);
            }
            
            if (IsAIStatusTransitionDelayOver(p_SwitchInstance))
            {
                if (_ForceMovePreset.t_ForceFlag && _MasterNode.HasState_Or(Unit.UnitStateType.DRIVESKILL))
                {
                    _MasterNode._ActableObject.CancelUnitAction(AnimatorParamStorage.MotionType.RelaxIdle);
                }

                var (moveValid, _) = _MasterNode._PhysicsObject.SetPhysicsAutonomyMove(
                    _MasterNode.GetPivotPosition(), ObjectDeployTool.ObjectDeploySurfaceDeployType.None, 
                    _ForceMovePreset.t_ForceFlag ? _ForceMovePreset.t_StoppingDistancePreset : p_AutonomyPathStoppingDistancePreset);
                
                if (moveValid)
                {
                }
                else
                {
                    OnAutonomyPathFindFailed();
                }
                
                return moveValid;
            }
            else
            {
                if(p_IsFirstEntry)
                {
                    StopMove(ActableTool.IdleState.Relax);
                }

                return false;
            }
        }
        
        /// <summary>
        /// 현재 사고 모듈을 pivotPosition 위치로 이동시키는 메서드
        /// </summary>
        protected bool SwitchStateMove(bool p_SwitchInstance, bool p_IsFirstEntry)
        {
            return SwitchStateMove(p_SwitchInstance, p_IsFirstEntry, PhysicsTool.AutonomyPathStoppingDistancePreset.GetStoppingRange(_MasterNode));
        }

        /// <summary>
        /// 현재 사고 모듈을 pivotPosition 위치로 이동시키는 메서드
        /// </summary>
        protected bool SwitchStateMove(Vector3 p_TargetPosition, bool p_ForceMove, bool p_SwitchInstance)
        {
            return SwitchStateMove(p_TargetPosition, p_ForceMove, p_SwitchInstance, PhysicsTool.AutonomyPathStoppingDistancePreset.GetStoppingRange(_MasterNode));
        }
        
        /// <summary>
        /// 현재 사고 모듈을 pivotPosition 위치로 이동시키는 메서드
        /// </summary>
        protected bool SwitchStateMove(Vector3 p_TargetPosition, bool p_ForceMove, bool p_SwitchInstance, bool p_IsFirstEntry)
        {
            return SwitchStateMove(p_TargetPosition, p_ForceMove, p_SwitchInstance, p_IsFirstEntry, PhysicsTool.AutonomyPathStoppingDistancePreset.GetStoppingRange(_MasterNode));
        }
        
        /// <summary>
        /// 지정한 위치를 pivotPosition으로 세트하고 해당 위치로 이동시키는 메서드
        /// </summary>
        protected bool SwitchStateMove(Vector3 p_TargetPosition, bool p_ForceMove, bool p_SwitchInstance, PhysicsTool.AutonomyPathStoppingDistancePreset p_AutonomyPathStoppingDistancePreset)
        {
            _ForceMovePreset = (p_ForceMove, p_AutonomyPathStoppingDistancePreset);
            _MasterNode.SetPivotPosition(p_TargetPosition, false, true);
            return SwitchStateMove(p_SwitchInstance, _CurrentAIState != ThinkableTool.AIState.Move, p_AutonomyPathStoppingDistancePreset);
        }
        
        /// <summary>
        /// 지정한 위치를 pivotPosition으로 세트하고 해당 위치로 이동시키는 메서드
        /// </summary>
        protected bool SwitchStateMove(Vector3 p_TargetPosition, bool p_ForceMove, bool p_SwitchInstance, bool p_IsFirstEntry, PhysicsTool.AutonomyPathStoppingDistancePreset p_AutonomyPathStoppingDistancePreset)
        {
            _ForceMovePreset = (p_ForceMove, p_AutonomyPathStoppingDistancePreset);
            _MasterNode.SetPivotPosition(p_TargetPosition, false, true);
            return SwitchStateMove(p_SwitchInstance, p_IsFirstEntry, p_AutonomyPathStoppingDistancePreset);
        }

        #endregion
    }
}