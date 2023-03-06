using UnityEngine;

namespace k514
{
    public partial class VFXProjectileBase
    {
        /*#region <Callbacks>
        
        protected override void OnSpawnSubEmitter_OnCollision()
        {
            if (!_ProjectileProgressFlagMask.HasAnyFlagExceptNone(ProjectileTool.ProjectileProgressFlag.BlockCollisionSubEmit))
            {
                foreach (var index in _SubEmitterIndexListOnCollision)
                {
                    _MainParticleSystem.subEmitters.GetSubEmitterSystem(index).Play();
                    _MainParticleSystem.TriggerSubEmitter(index);
                }
            }
        }
        
        protected override void OnSpawnSubEmitter_OnDeath()
        {
            if (!_ProjectileProgressFlagMask.HasAnyFlagExceptNone(ProjectileTool.ProjectileProgressFlag.BlockTerminateSubEmit))
            {
                foreach (var index in _SubEmitterIndexListOnDeath)
                {
                    _MainParticleSystem.subEmitters.GetSubEmitterSystem(index).Play();
                    _MainParticleSystem.TriggerSubEmitter(index);
                }
            }
        }

        private void OnHandleEventResult(ProjectileTool.ProjectileEventHandleFlag p_EventFlagMask, GameObject p_Collided)
        {
            var _ValidUnit = false;
            var _CollidedUnit = default(Unit);
            var _TargetAffine = default(TransformTool.AffineCachePreset);
            
            // 해당 충돌 이벤트가 유효한지, 유효하다면 유닛 충돌이벤트인지 구분하기 위해서 일부 플래그를 먼저 처리해준다.
            if (p_EventFlagMask.HasAnyFlagExceptNone(ProjectileTool.ProjectileEventHandleFlag.FindCollidedUnit))
            {
                (_ValidUnit, _CollidedUnit) = UnitInteractManager.GetInstance.TryGetUnit(p_Collided);
            }

            if (p_EventFlagMask.HasAnyFlagExceptNone(ProjectileTool.ProjectileEventHandleFlag.UpdateCollisionEvent))
            {
                _MainParticleSystem.GetCollisionEvents(p_Collided, _ParticleCollisionEventSet);

                if (_ValidUnit)
                {
                    /* SE Cond #1#
                    // 1. 충돌한 유닛이 마스터 노드와 다른 경우
                    // 2. 충돌한 유닛과 마스터 노드가 레코드에 기술된 피아 관계인 경우
                    // 3. HitEventTrace에서 최대타격수 및 타격간격을 만족하는 경우
                    _HasCollisionEvent = !ReferenceEquals(MasterNode.Node, _CollidedUnit)
                                         && _Record.UnitGroupRelateType.HasAnyFlagExceptNone(MasterNode.Node.GetGroupRelate(_CollidedUnit))
                                         && CheckHitValidation(_CollidedUnit, Time.time);
                }
                else
                {
                    _HasCollisionEvent = true;
                }
            }
            else
            {
                _HasCollisionEvent = true;
            }
            
            // 충돌 이벤트가 유효한 경우
            if (_HasCollisionEvent)
            {
                if (_ParticleCollisionEventSet.Count > 0)
                {
                    _TargetAffine = GetCollideAffine(_ParticleCollisionEventSet[0]);
                }
                else
                {
                    var tryParticle = _SpawnedParticleSet[0];
                    _TargetAffine = GetParticleAffine(tryParticle);
                    
                    if (_TraceTargetEventFlagMask.HasAnyFlagExceptNone(ProjectileTool.ProjectileTraceTargetEventFlag.CorrectParticlePositionWhenBlocked_Unit) && _ValidUnit)
                    {
                        _TargetAffine.SetPosition(_CollidedUnit.GetCenterPosition());
                    }
                }
                
                foreach (var flagType in ProjectileTool.ProjectileEventHandleFlagEnumerator)
                {
                    if (p_EventFlagMask.HasAnyFlagExceptNone(flagType))
                    {
                        switch (flagType)
                        {
                            case ProjectileTool.ProjectileEventHandleFlag.TossDamage:
#if !SERVER_DRIVE
                                // 원격 서버로부터 타격 판정을 제어받는 유닛은 클라이언트에서 필터링을 수행하지 않는다.
                                if (RemoteClientManager.GetInstanceUnSafe.IsBlockedHandleHitEvent(MasterNode))
                                {
                                }
                                else
#endif
                                {
                                    if (_ValidUnit)
                                    {
                                        var hitPreset = UnitHitPresetData.GetInstanceUnSafe[_Record.HitPresetIndex];
                                        _CollidedUnit.HitUnit
                                        (
                                            MasterNode, hitPreset.GetHitMessage(),
                                            new UnitTool.UnitAddForcePreset(_Transform.position, _TargetAffine.Position, _TargetAffine.Forward),
                                            false
                                        );
                                    }
                                }
                                break;
                            case ProjectileTool.ProjectileEventHandleFlag.DeployEvent:
                            {
                                ObjectDeployLoader.GetInstance.CastDeployEventMap(_Record.DeployEventPresetIndex, MasterNode, _TargetAffine);
                            }
                                break;
                            case ProjectileTool.ProjectileEventHandleFlag.HitFilter:
#if !SERVER_DRIVE
                                // 원격 서버로부터 타격 판정을 제어받는 유닛은 클라이언트에서 타격 판정을 수행하지 않는다.
                                if (RemoteClientManager.GetInstanceUnSafe.IsBlockedHandleHitEvent(MasterNode))
                                {
                                }
                                else
#endif
                                {
                                    UnitHitManager.GetInstance.TossDamage
                                    (
                                        MasterNode, _Record.HitPresetIndex,
                                        new UnitTool.UnitAddForcePreset(_Transform.position, _TargetAffine.Position, _TargetAffine.Forward),
                                        _TargetAffine, false
                                    );
                                }
                                break;
                            // 해당 이벤트가 Vfx 파기 플래그를 포함하는 경우
                            case ProjectileTool.ProjectileEventHandleFlag.RemoveVfx:
                                SetRemove(false, 0);
                                break;
                        }
                    }
                    else
                    {
                        switch (flagType)
                        {
                            // 해당 이벤트가 Vfx 파기 플래그를 포함하지 않는 경우
                            case ProjectileTool.ProjectileEventHandleFlag.RemoveVfx:
                                OnSpawnSubEmitter_OnCollision();
                                break;
                        }
                    }
                }
            }
        }
        
        #endregion*/
    }
}