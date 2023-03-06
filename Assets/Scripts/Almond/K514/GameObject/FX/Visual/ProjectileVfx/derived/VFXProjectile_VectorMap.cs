using System.Collections.Generic;
using UnityEngine;

namespace k514
{
    public class VFXProjectile_VectorMap : VFXProjectileBase
    {
        #region <Fields>

        private PrefabExtraData_ProjectileVfx_VectorMap.ProjectileVfxTableRecord _Record;
        private ObjectDeployTimeline _ObjectDeployTimeline;
        
        #endregion
        
        /*#region <Callbacks>

        protected override void OnInitializeMainParticleSystem()
        {
            base.OnInitializeMainParticleSystem();

            _Record = (PrefabExtraData_ProjectileVfx_VectorMap.ProjectileVfxTableRecord) _PrefabKey._PrefabExtraPreset._PrefabExtraDataRecord;
            _ObjectDeployTimeline = new ObjectDeployTimeline();
        }

        protected override void OnPlayUpdateEvent()
        {
            base.OnPlayUpdateEvent();
                      
            _ObjectDeployTimeline.LoadTimeline(MasterNode, _Transform, _Record.VectorMapIndex);
        }

        protected override bool OnUpdateParticle(float p_DeltaTime)
        {
            if (OnUpdate(p_DeltaTime))
            {
                var (valid, affine) = _ObjectDeployTimeline.GetCurrentAffine(_WholeProgressTimer.ElapsedTime);
                if (valid)
                {
                    var tryProgressRate = Mathf.Min(_ObjectDeployTimeline.GetCurrentProgressRate() + 0.1f, 1f);
                    var particle = _SpawnedParticleSet[0];
                    var direction = affine[0].Forward;
                    var velocityUV = particle.totalVelocity.normalized;
             
                    if (_TraceDirectionTuple.Item1)
                    {
                        direction = particle.totalVelocity.magnitude * CustomMath.GetLBVector(direction, _TraceDirectionTuple.Item2, tryProgressRate).normalized;
                    }
                        
                    particle.velocity = particle.totalVelocity.magnitude * CustomMath.GetLBVector(velocityUV, direction, tryProgressRate).normalized;
                    _SpawnedParticleSet[0] = particle;
           
                    _MainParticleSystem.SetParticles(_SpawnedParticleSet);
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion*/
    }
}