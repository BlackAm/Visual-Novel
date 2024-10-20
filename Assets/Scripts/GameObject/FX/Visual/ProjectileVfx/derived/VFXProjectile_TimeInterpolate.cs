using UnityEngine;

namespace BlackAm
{
    public class VFXProjectile_TimeInterpolate : VFXProjectileBase
    {
        /*#region <Callbacks>

        protected override bool OnUpdateParticle(float p_DeltaTime)
        {
            if (OnUpdate(p_DeltaTime))
            {
                if (_TraceDirectionTuple.Item1)
                {
                    var tryProgressRate = Mathf.Min(_WholeProgressTimer.ProgressRate + 0.1f, 1f);
                    var particle = _SpawnedParticleSet[0];
                    var velocityUV = particle.totalVelocity.normalized;
                    particle.velocity = particle.totalVelocity.magnitude * CustomMath.GetLBVector(velocityUV, _TraceDirectionTuple.Item2, tryProgressRate).normalized;
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