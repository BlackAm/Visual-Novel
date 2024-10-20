using UnityEngine;

namespace BlackAm
{
    public partial class VFXProjectileBase
    {
        /*#region <Fields>

        /// <summary>
        /// 투사체 이벤트 플래그 마스크
        /// </summary>
        private ProjectileTool.ProjectileCollisionEventFlag _CollisionEventFlagMask;

        #endregion
        
        #region <Callbacks>

        private void OnAwakeUnityCollisionEvent()
        {
        }

        private void OnPoolingUnityCollisionEvent()
        {
            _CollisionEventFlagMask = _Record.ProjectileCollisionEventFlag;
            SetCollisionLayer(_Record.CollisionLayerMask);
            
            var collisionFactorTuple = _Record.CollisionFactorTuple;
            var collisionModule = _MainParticleSystem.collision;
            collisionModule.dampen = Mathf.Clamp(collisionFactorTuple.t_Dampen01, 0f, 1f);
            collisionModule.bounce = Mathf.Clamp(collisionFactorTuple.t_Bounce02, 0f, 2f);
            collisionModule.lifetimeLoss = Mathf.Clamp(collisionFactorTuple.t_LifeTimeLose01, 0f, 1f);
            
            if (!_UnitCollisionEventFlagMask.HasAnyFlagExceptNone(ProjectileTool.ProjectileUnitCollisionEventFlag.DriveUnityPhysicsEvent))
            {
                RemoveCollisionLayer(GameManager.GameLayerMaskType.UnitLayerSet);
            }
            SetCollisionEnable(true);
        }

        private void OnRetrieveUnityCollisionEvent()
        {
            SetCollisionEnable(false);
        }
        
        /// <summary>
        /// 파티클 수명이 다해서 파기되는 경우에 호출되는 콜백
        /// </summary>
        private void OnLifeSpanOver()
        {
            // Hrunting 플래그를 가지는 경우, 포커스 유닛에게 충돌 이벤트를 그대로 전달한다.
            if (IsHrunting)
            {
                if (FocusNode)
                {
                    OnParticleCollision(FocusNode);
                }
            }

            var lifeSpanOverEventMask = ProjectileTool.ProjectileEventHandleFlag.None;
            if (_CollisionEventFlagMask.HasAnyFlagExceptNone(ProjectileTool.ProjectileCollisionEventFlag.FilterEventWhenLifeOver))
            {
                lifeSpanOverEventMask.AddFlag(ProjectileTool.ProjectileEventHandleFlag.HitFilter);
            }
            
            if (_CollisionEventFlagMask.HasAnyFlagExceptNone(ProjectileTool.ProjectileCollisionEventFlag.DeployEventWhenLifeOver))
            {
                lifeSpanOverEventMask.AddFlag(ProjectileTool.ProjectileEventHandleFlag.DeployEvent);
            }

            if (lifeSpanOverEventMask != ProjectileTool.ProjectileEventHandleFlag.None)
            {
                OnHandleEventResult(lifeSpanOverEventMask, null);
            }
        }
        
        /// <summary>
        /// 파티클 시스템이 충돌한 경우에 호출되는 콜백
        /// MainParticle이 없다면 동작하지 않는다.
        /// </summary>
        private void OnParticleCollision(GameObject p_Collided)
        {
            var tryLayer = (GameManager.GameLayerType) p_Collided.layer;
            var result = ProjectileTool.ProjectileEventHandleFlag.None;
            
            switch (tryLayer)
            {
                case GameManager.GameLayerType.Obstacle:
                    result = OnCollideWithObstacle(p_Collided);
                    break;
                case GameManager.GameLayerType.Terrain:
                    result = OnCollideWithTerrain(p_Collided);
                    break;
                case GameManager.GameLayerType.UnitA:
                case GameManager.GameLayerType.UnitB:
                case GameManager.GameLayerType.UnitC:
                case GameManager.GameLayerType.UnitD:
                case GameManager.GameLayerType.UnitE:
                case GameManager.GameLayerType.UnitF:
                case GameManager.GameLayerType.UnitG:
                case GameManager.GameLayerType.UnitH:
                    result = OnCollideWithUnit(p_Collided);
                    break;
            }

            OnHandleEventResult(result, p_Collided);
        }

        private ProjectileTool.ProjectileEventHandleFlag OnCollideWithObstacle(GameObject p_Collided)
        {
            var result = ProjectileTool.ProjectileEventHandleFlag.None;
            
            foreach (var flagType in ProjectileTool.ProjectileCollisionEventFlagEnumerator)
            {
                if (_CollisionEventFlagMask.HasAnyFlagExceptNone(flagType))
                {
                    switch (flagType)
                    {
                        case ProjectileTool.ProjectileCollisionEventFlag.BurstWhenBlocked_Terrain_Obstacle:
                            result.AddFlag(ProjectileTool.ProjectileEventHandleFlag.RemoveVfx);
                            break;
                        case ProjectileTool.ProjectileCollisionEventFlag.DeployEventWhenBlocked_Terrain_Obstacle:
                            result.AddFlag(ProjectileTool.ProjectileEventHandleFlag.UpdateCollisionEvent | ProjectileTool.ProjectileEventHandleFlag.DeployEvent);
                            break;
                        case ProjectileTool.ProjectileCollisionEventFlag.FilterEventWhenBlocked_Terrain_Obstacle:
                            result.AddFlag(ProjectileTool.ProjectileEventHandleFlag.UpdateCollisionEvent | ProjectileTool.ProjectileEventHandleFlag.HitFilter);
                            break;
                    }
                }
            }

            return result;
        }

        private ProjectileTool.ProjectileEventHandleFlag OnCollideWithTerrain(GameObject p_Collided)
        {
            var result = ProjectileTool.ProjectileEventHandleFlag.None;

            foreach (var flagType in ProjectileTool.ProjectileCollisionEventFlagEnumerator)
            {
                if (_CollisionEventFlagMask.HasAnyFlagExceptNone(flagType))
                {
                    switch (flagType)
                    {
                        case ProjectileTool.ProjectileCollisionEventFlag.BurstWhenBlocked_Terrain_Obstacle:
                            result.AddFlag(ProjectileTool.ProjectileEventHandleFlag.RemoveVfx);
                            break;
                        case ProjectileTool.ProjectileCollisionEventFlag.DeployEventWhenBlocked_Terrain_Obstacle:
                            result.AddFlag(ProjectileTool.ProjectileEventHandleFlag.UpdateCollisionEvent | ProjectileTool.ProjectileEventHandleFlag.DeployEvent);
                            break;
                        case ProjectileTool.ProjectileCollisionEventFlag.FilterEventWhenBlocked_Terrain_Obstacle:
                            result.AddFlag(ProjectileTool.ProjectileEventHandleFlag.UpdateCollisionEvent | ProjectileTool.ProjectileEventHandleFlag.HitFilter);
                            break;
                    }
                }
            }
            
            return result;
        }

        private ProjectileTool.ProjectileEventHandleFlag OnCollideWithUnit(GameObject p_Collided)
        {
            var result = ProjectileTool.ProjectileEventHandleFlag.None;

            foreach (var flagType in ProjectileTool.ProjectileCollisionEventFlagEnumerator)
            {
                if (_CollisionEventFlagMask.HasAnyFlagExceptNone(flagType))
                {
                    switch (flagType)
                    {
                        case ProjectileTool.ProjectileCollisionEventFlag.BurstWhenBlocked_Unit:
                            result.AddFlag(ProjectileTool.ProjectileEventHandleFlag.RemoveVfx);
                            break;
                        case ProjectileTool.ProjectileCollisionEventFlag.DeployEventWhenBlocked_Unit:
                            result.AddFlag(ProjectileTool.ProjectileEventHandleFlag.UpdateUnitCollisionEvent | ProjectileTool.ProjectileEventHandleFlag.DeployEvent);
                            break;
                        case ProjectileTool.ProjectileCollisionEventFlag.TossDamageWhenBlocked_Unit:
                            result.AddFlag(ProjectileTool.ProjectileEventHandleFlag.UpdateUnitCollisionEvent | ProjectileTool.ProjectileEventHandleFlag.TossDamage);
                            break;
                        case ProjectileTool.ProjectileCollisionEventFlag.FilterEventWhenBlocked_Unit:
                            result.AddFlag(ProjectileTool.ProjectileEventHandleFlag.UpdateUnitCollisionEvent | ProjectileTool.ProjectileEventHandleFlag.HitFilter);
                            break;
                    }
                }
            }
            
            return result;
        }

        #endregion

        #region <Methods>

        private void SetCollisionEnable(bool p_Flag)
        {
            var collisionModule = _MainParticleSystem.collision;
            collisionModule.enabled = p_Flag;
        }

        public void SetCollisionLayer(GameManager.GameLayerMaskType p_LayerType)
        {
            var collisionModule = _MainParticleSystem.collision;
            collisionModule.collidesWith = (int) p_LayerType;
        }

        public void AddCollisionLayer(GameManager.GameLayerMaskType p_LayerType)
        {
            var collisionModule = _MainParticleSystem.collision;
            collisionModule.collidesWith |= (int) p_LayerType;
        }

        public void AddCollisionLayer(int p_LayerMask)
        {
            var collisionModule = _MainParticleSystem.collision;
            collisionModule.collidesWith |= p_LayerMask;
        }

        public void RemoveCollisionLayer(GameManager.GameLayerMaskType p_LayerType)
        {
            var collisionModule = _MainParticleSystem.collision;
            collisionModule.collidesWith &= (int) ~p_LayerType;
        }

        public void RemoveCollisionLayer(int p_LayerMask)
        {
            var collisionModule = _MainParticleSystem.collision;
            collisionModule.collidesWith &= ~p_LayerMask;
        }

        public void ClearCollisionLayer()
        {
            var collisionModule = _MainParticleSystem.collision;
            collisionModule.collidesWith = (int) GameManager.GameLayerMaskType.Nothing;
        }
        
        #endregion*/
    }
}