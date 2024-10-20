using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BlackAm
{
    public partial class VFXProjectileBase
    {
        /*#region <Fields>

        /// <summary>
        /// 투사체 이벤트 핸들 마스크
        /// </summary>
        private ProjectileTool.ProjectileTraceTargetEventFlag _TraceTargetEventFlagMask;
        
        /// <summary>
        /// 해당 투사체가 추적 플래그를 가지는지 표시하는 논리 프로퍼티
        /// </summary>
        private bool IsTracing => _TraceTargetEventFlagMask.HasAnyFlagExceptNone(ProjectileTool.ProjectileTraceTargetEventFlag.TraceTarget);

        /// <summary>
        /// 추적 지연시간이 경과하여 추적이 시작됨을 표시하는 플래그
        /// </summary>
        private bool _TraceStartFlag;

        /// <summary>
        /// 추적 벡터 튜플
        /// </summary>
        protected (bool, Vector3) _TraceDirectionTuple;

        /// <summary>
        /// 가장 최근에 추적한 위치
        /// </summary>
        private Vector3 _LastestTracePosition;
        
        #endregion

        #region <Callbacks>

        private void OnPoolingTracing()
        {
            _TraceTargetEventFlagMask = _Record.ProjectileTraceTargetEventFlag;
            _TraceStartFlag = false;
            _TraceDirectionTuple = default;
            _LastestTracePosition = _Transform.position;
        }
        
        private void OnRetrieveTracing()
        {
            _LastestTracePosition = default;
        }

        private void OnFirstDelayOver()
        {
            _TraceStartFlag = true;

            if (FocusNode && _TraceTargetEventFlagMask.HasAnyFlagExceptNone(ProjectileTool.ProjectileTraceTargetEventFlag.CorrectDirectionWhenFirstDelayOver))
            {
                SetParticleLook(FocusNode.Node.GetCenterPosition());
            }
        }
        
        protected void OnUpdateTraceTargetUnitVector(ParticleSystem.Particle p_Particle)
        {
            if (_TraceStartFlag)
            {
                var particlePosition = p_Particle.position;
                // 서버와 통신하는 유닛의 투사체 및 서버의 투사체는 동기화를 위해 랜덤 요소가 있는 '추적 대상 재지정' 로직을 수행하지 않는다.
                var isHrunting = IsHrunting;
                var fallBackTraceTargetFlag = !isHrunting;
 
                RE_ENTER_CALCULATE_DIRECTION :
                if (FocusNode.CheckNode(Unit.UnitStateType.UnitFightableFilterMask))
                {
                    _LastestTracePosition = FocusNode.Node.GetAttachPosition(Unit.UnitAttachPoint.BoneCenterNode);
                    
                    var direction = particlePosition.GetDirectionUnitVectorTo(_LastestTracePosition);
                    var velocityUV = p_Particle.totalVelocity.normalized;
                    var halfSightDot = _Record.TraceHalfSightDot;
                    
                    if (isHrunting || halfSightDot <= -1f || Vector3.Dot(direction, velocityUV) >= halfSightDot)
                    {
                        _TraceDirectionTuple = (true, direction);
                    }
                    else
                    {
                        if (fallBackTraceTargetFlag)
                        {
                            fallBackTraceTargetFlag = false;

                            if (UpdateFallbackTraceTarget(particlePosition))
                            {
                                goto RE_ENTER_CALCULATE_DIRECTION;
                            }
                            else
                            {
                                _TraceDirectionTuple = default;
                            }
                        }
                        else
                        {
                            _TraceDirectionTuple = default;
                        }
                    }
                }
                else
                {
                    if (isHrunting)
                    {
                        _ProjectileProgressFlagMask.RemoveFlag(ProjectileTool.ProjectileProgressFlag.Hrunting);
                        SetCollisionLayer(_Record.CollisionLayerMask);
                        _TraceDirectionTuple = default;
                    }
                    else
                    {
                        if (fallBackTraceTargetFlag)
                        {
                            fallBackTraceTargetFlag = false;

                            if (UpdateFallbackTraceTarget(particlePosition))
                            {
                                goto RE_ENTER_CALCULATE_DIRECTION;
                            }
                            else
                            {
                                _TraceDirectionTuple = default;
                            }
                        }
                        else
                        {
                            _TraceDirectionTuple = default;
                        }
                    }
                }
            }
        }
        
        #endregion

        #region <Methods>
        
        public void SetTraceTarget(Unit p_TraceTarget)
        {
            SetTraceTarget(p_TraceTarget, _Record.ParticleControlPredelay, CustomMath.MaxLineLength);
        }
        
        public void SetTraceTarget(Unit p_TraceTarget, float p_TargetSqrDistance)
        {
            SetTraceTarget(p_TraceTarget, _Record.ParticleControlPredelay, p_TargetSqrDistance);
        }

        public async void SetTraceTarget(Unit p_TraceTarget, int p_Predelay, float p_TargetSqrDistance)
        {
            FocusNode.SetNode(PrefabInstanceTool.FocusNodeRelateType.Enemy, p_TraceTarget);

#if !SERVER_DRIVE
            if (RemoteClientManager.GetInstanceUnSafe.IsNeedSynchronizeAtMultiClients(MasterNode))
#endif
            // 해당 투사체가 다수의 클라이언트에서 동기화된 동작을 해야하는 경우
            {
                // Hrunting 플래그를 세트한다.
                _ProjectileProgressFlagMask.AddFlag(ProjectileTool.ProjectileProgressFlag.Hrunting);
            }

            // Hrunting 플래그가 세트된 경우
            if (IsHrunting)
            {
                p_Predelay = 0;
                
                _ProjectileProgressFlagMask.AddFlag(ProjectileTool.ProjectileProgressFlag.DangerClose);
                _TraceTargetEventFlagMask.AddFlag(ProjectileTool.ProjectileTraceTargetEventFlag.TraceTarget 
                                                  | ProjectileTool.ProjectileTraceTargetEventFlag.CorrectDirectionWhenFirstDelayOver
                                                  | ProjectileTool.ProjectileTraceTargetEventFlag.CorrectParticlePositionWhenBlocked_Unit);
                _TraceTargetEventFlagMask.RemoveFlag(ProjectileTool.ProjectileTraceTargetEventFlag.FallbackTraceTargeting);
                
                SetCollisionEnable(false);
            }
            else
            {
                if (FocusNode)
                {
                    p_Predelay = p_TargetSqrDistance < _Record.ParticleControlPredelayApplyLowerBoundDistance
                        ? 0
                        : p_Predelay;
                }
                else
                {
                    if (UpdateFallbackTraceTarget())
                    {
                        p_TargetSqrDistance = _Transform.GetSqrDistanceTo((Vector3) FocusNode);
                        p_Predelay = p_TargetSqrDistance < _Record.ParticleControlPredelayApplyLowerBoundDistance
                            ? 0
                            : p_Predelay;
                    }
                }
            }

            if (FocusNode
                && _ProjectileProgressFlagMask.HasAnyFlagExceptNone(ProjectileTool.ProjectileProgressFlag.DangerClose)
                && p_TargetSqrDistance < Mathf.Pow(_Record.DangerCloseDistance + FocusNode.Node.GetRadius(), 2f))
            {
                OnParticleCollision(FocusNode);
                SetRemove(false, 0);
            }
            else
            {
                if (p_Predelay > 50)
                {
                    await UniTask.Delay(p_Predelay);
                    if (!this.IsValid())
                    {
                        return;
                    }
                }

                OnFirstDelayOver();
            }
        }

        /// <summary>
        /// 투사체 생성시, 선딜레이 이전에 선정된 포커스의 유효성을 검증하고
        /// 유효하지 않다면 재선정하는 메서드
        /// </summary>
        private bool UpdateFallbackTraceTarget()
        {
            if (FocusNode)
            {
                return true;
            }
            else
            {
                if (_TraceTargetEventFlagMask.HasAnyFlagExceptNone(ProjectileTool.ProjectileTraceTargetEventFlag.UsingFallbackTraceTargeting) && !IsHrunting)
                {
                    var masterNode = MasterNode.Node;
                    var(actionValid, actionResult) = masterNode._ActableObject.GetCurrentUnitAction();
                    var findDistance = actionValid ? actionResult._UnitActionPresetRecord.AttackRange : _Record.FallbackSearchTargetDistance;
                    var (findValid, findResult) = masterNode.FindEnemy(_Transform.position, findDistance, ThinkableTool.AIUnitFindType.NearestPosition, Unit.UnitStateType.UnitFightableFilterMask, true);
                    if (findValid)
                    {
                        FocusNode.SetNode(PrefabInstanceTool.FocusNodeRelateType.Enemy, findResult);
                        return true;
                    }
                    else
                    {
                        FocusNode.ClearNode();
                        return false;
                    }
                }
                else
                {
                    FocusNode.ClearNode();
                    return false;
                }
            }
        }

        /// <summary>
        /// 현재 파티클이 선딜레이 이후 포커스를 가지지 않는 경우 주변적을 탐색하여 선정하게 하는 메서드
        /// </summary>
        private bool UpdateFallbackTraceTarget(Vector3 p_CurrentParticlePosition)
        {
            if (_TraceTargetEventFlagMask.HasAnyFlagExceptNone(ProjectileTool.ProjectileTraceTargetEventFlag.UpdateFallbackTraceTargeting) && !IsHrunting)
            {
                var (valid, result) = MasterNode.Node.FindEnemy(p_CurrentParticlePosition, _Record.FallbackSearchTargetDistance, ThinkableTool.AIUnitFindType.NearestPosition, Unit.UnitStateType.UnitFightableFilterMask, false);
                if (!valid || ReferenceEquals(FocusNode.Node, result))
                {
                    return false;
                }
                else
                {
                    FocusNode.SetNode(PrefabInstanceTool.FocusNodeRelateType.Enemy, result);
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        #endregion*/
    }
}