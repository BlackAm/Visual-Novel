using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace k514
{
    public abstract partial class VFXProjectileBase : VFXUnit
    {
        /*#region <Fields>

        /// <summary>
        /// 해당 이펙트 오브젝트에 추가할 데이터 레코드
        /// </summary>
        private ProjectileVfxExtraDataRecordBridge _Record;

        /// <summary>
        /// 투사체 동작 플래그 마스크
        /// </summary>
        private ProjectileTool.ProjectileProgressFlag _ProjectileProgressFlagMask;
                
        /// <summary>
        /// 해당 투사체가 Hrunting 플래그를 가지는지 표시하는 논리 프로퍼티
        /// </summary>
        private bool IsHrunting => _ProjectileProgressFlagMask.HasAnyFlagExceptNone(ProjectileTool.ProjectileProgressFlag.Hrunting);
        
        /// <summary>
        /// 해당 투사체가 활성화된 이후 경과한 시간을 측정하는 타이머 프리셋
        /// </summary>
        protected ProgressTimer _WholeProgressTimer;

        /// <summary>
        /// 현재 스폰된 파티클 셋
        /// </summary>
        protected ParticleSystem.Particle[] _SpawnedParticleSet;

        /// <summary>
        /// 현재 파티클 시스템이 유효한 파티클 시스템을 가지는지 표시하는 플래그
        /// </summary>
        private bool _HasValidParticle;
        
        /// <summary>
        /// 파티클 충돌 이벤트 셋
        /// </summary>
        private List<ParticleCollisionEvent> _ParticleCollisionEventSet;

        /// <summary>
        /// 현재 충돌 이벤트가 유효한지 표시하는 플래그
        /// </summary>
        private bool _HasCollisionEvent;

        /// <summary>
        /// 파티클 수명 튜플
        /// </summary>
        private (float t_Value, float t_InverseValue) _ParticleLifeSpan;

        /// <summary>
        /// 좌표 추적 프리셋
        /// </summary>
        public PositionStatePreset _PositionState;
        
        #endregion
        
        #region <Callbacks>

        public override void OnSpawning()
        {
            base.OnSpawning();

            _SpawnedParticleSet = new ParticleSystem.Particle[1];
            _ParticleCollisionEventSet = new List<ParticleCollisionEvent>(1);

            var emissionModule = _MainParticleSystem.emission;
            emissionModule.SetBurst(0, new ParticleSystem.Burst(0f, 1, 1, 0f));

            _MainParticleSystemMain.duration = Mathf.Min(_MainParticleSystemMain.duration, 0.1f);
            _WholeProgressTimer.Initialize(_ParticleLifeSpan.t_Value);
                
            _PositionState = new PositionStatePreset();
            
#if UNITY_EDITOR && SERVER_DRIVE
            if (CustomDebug.VisualizeServerNode)
            {
                SetEnableRender(true);
            }
#endif
        }
        
        protected override void OnInitializeMainParticleSystem()
        {
            base.OnInitializeMainParticleSystem();

            _Record = (ProjectileVfxExtraDataRecordBridge) _PrefabKey._PrefabExtraPreset._PrefabExtraDataRecord;
            
            _ProjectileProgressFlagMask = _Record.ProjectileProgressFlag;
            _MainParticleSystemMain.maxParticles = 1;
            _ParticleLifeSpan = (_Record.LifeSpan, 1f / _Record.LifeSpan);
            _MainParticleSystemMain.startLifetime = _ParticleLifeSpan.t_Value;
            _MainParticleSystemMain.simulationSpace = ParticleSystemSimulationSpace.World;

            OnAwakeUnitCollision();
            OnAwakeUnityCollisionEvent();
        }

        public override void OnPooling()
        {
            base.OnPooling();
            
            _PositionState.SetInitialize(_Transform.position);

            OnPoolingUnitCollision();
            OnPoolingUnityCollisionEvent();
            OnPoolingTracing();

            _WholeProgressTimer.Reset();
            ClearParticleSet();
            
            SetParticleAffine(_Transform);
        }

        public override void OnRetrieved()
        {
            base.OnRetrieved();
            
            _PositionState.SetInitialize(Vector3.zero);

            OnRetrieveUnitCollision();
            OnRetrieveUnityCollisionEvent();
            OnRetrieveTracing();
        }
        
        public override void OnPositionTransition(Vector3 p_TransitionTo)
        {
            base.OnPositionTransition(p_TransitionTo);
            
            _PositionState.SetInitialize(_Transform.position);
        }
        
        #endregion

        #region <Methods>

        /// <summary>
        /// 파티클 상태를 업데이트 한다.
        /// 만약 파티클 숫자가 0보다 큰 상태에서 0으로 전이한 경우 참을 리턴한다.
        /// </summary>
        private bool UpdateParticleSet()
        {
            var prevValid = _HasValidParticle;
            var currentParticleCount = _MainParticleSystem.GetParticles(_SpawnedParticleSet);
            var result = false;
            _HasValidParticle = currentParticleCount > 0;

            if (prevValid)
            {
                if (_HasValidParticle)
                {
                    _PositionState.UpdatePosition(_SpawnedParticleSet[0].position);
                    result = false;
                }
                else
                {
                    var defaultParticle = new ParticleSystem.Particle();
                    defaultParticle.position = _Transform.position;
                    _SpawnedParticleSet[0] = defaultParticle;
                    result = true;
                }
            }
            else
            {
                if (_HasValidParticle)
                {
                    _PositionState.UpdatePosition(_SpawnedParticleSet[0].position);
                }
                result = false;
            }

            if (_HasValidParticle)
            {
                if (_UnitCollisionEventFlagMask.HasAnyFlagExceptNone(ProjectileTool.ProjectileUnitCollisionEventFlag.DriveUnitFilterPhysicsEvent))
                {
                    OnCastSphereCast();
                }  
            }

            return result;
        }

        private void ClearParticleSet()
        {
            _HasValidParticle = false;
            _SpawnedParticleSet[0] = default;
        }

        private void SetParticleLook(Vector3 p_Target)
        {
            if(_HasValidParticle)
            {
                var particle = _SpawnedParticleSet[0];
                particle.velocity = particle.totalVelocity.magnitude * particle.position.GetDirectionUnitVectorTo(p_Target);
                _SpawnedParticleSet[0] = particle;
                _MainParticleSystem.SetParticles(_SpawnedParticleSet);
            }
        }
        
        private void SetParticleDirection(Vector3 p_Direction)
        {
            if(_HasValidParticle)
            {
                var particle = _SpawnedParticleSet[0];
                particle.velocity = particle.totalVelocity.magnitude * p_Direction.normalized;
                _SpawnedParticleSet[0] = particle;
                _MainParticleSystem.SetParticles(_SpawnedParticleSet);
            }
        }

        private void SetParticleFreeze()
        {
            if (_HasValidParticle)
            {
                var particle = _SpawnedParticleSet[0];
                particle.velocity = Vector3.zero;
                _SpawnedParticleSet[0] = particle;
                _MainParticleSystem.SetParticles(_SpawnedParticleSet);
            }
        }
        
        private void SetParticleLifeOver()
        {
            if (_HasValidParticle)
            {
                var particle = _SpawnedParticleSet[0];
                particle.remainingLifetime = 0f;
                particle.velocity = Vector3.zero;
                _SpawnedParticleSet[0] = particle;
                _MainParticleSystem.SetParticles(_SpawnedParticleSet);
            }
        }

        private void SetParticleAffine(TransformTool.AffineCachePreset p_Affine)
        {
            var particle = new ParticleSystem.Particle();
            particle.position = p_Affine.Position;
            particle.velocity = p_Affine.Forward;
            _SpawnedParticleSet[0] = particle;
        }

        private (bool, TransformTool.AffineCachePreset) GetParticleAffine()
        {
            if(_HasValidParticle)
            {
                var particle = _SpawnedParticleSet[0];
                return (true, GetParticleAffine(particle));
            }
            else
            {
                return default;
            }
        }
        
        private TransformTool.AffineCachePreset GetParticleAffine(ParticleSystem.Particle p_Particle)
        {
            var pos = p_Particle.position;
            var scale = ObjectScale.CurrentValue;
            var forward = p_Particle.totalVelocity.normalized;
            var result = new TransformTool.AffineCachePreset(pos, Quaternion.identity, scale);
            result.SetBasis(forward);

            return result;
        }
        
        private TransformTool.AffineCachePreset GetCollideAffine(ParticleCollisionEvent p_Event)
        {
            var scale = ObjectScale.CurrentValue;
            var result = new TransformTool.AffineCachePreset(p_Event.intersection, Quaternion.identity, scale);
            result.SetBasis(p_Event.velocity);

            return result;
        }
        
        protected override void SetPlay()
        {
            if (!IsReservedDeadOrDead())
            {
                if (!_ProjectileProgressFlagMask.HasAnyFlagExceptNone(ProjectileTool.ProjectileProgressFlag.BlockShotWhenHasNoTarget) || FocusNode.CheckNode())
                {
                    SetSimulateSpeed(_Record.SimulateSpeedFactor);
                    
                    if (_TraceTargetEventFlagMask.HasAnyFlagExceptNone(ProjectileTool.ProjectileTraceTargetEventFlag
                        .SetVfxAffineTowardTarget))
                    {
                        if (FocusNode)
                        {
                            SetLook(FocusNode.Node.GetCenterPosition());
                        }
                    }

                    OnPlayUpdateEvent();
                    
                    foreach (var particleSystem in _ParticleSystemGroup)
                    {
                        particleSystem.Play();
                    }
                }
                else
                {
                    SetRemove(true, 0);
                }
            }
        }

        public override void SetRemove(bool p_InstantRemove, uint p_Predelay)
        {
            if (!IsReservedDeadOrDead())
            {
                if (!p_InstantRemove)
                {
                    OnSpawnSubEmitter_OnDeath();
                    SetParticleLifeOver();
                }
                
                base.SetRemove(p_InstantRemove, p_Predelay);
            }
        }

        #endregion*/
    }
}