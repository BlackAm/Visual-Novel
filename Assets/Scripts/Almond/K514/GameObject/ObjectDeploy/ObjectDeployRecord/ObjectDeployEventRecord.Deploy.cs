using Cysharp.Threading.Tasks;
using UnityEngine;

namespace k514
{
    public partial class ObjectDeployEventRecord
    {
#if !SERVER_DRIVE
        private async void DeployVfx(Unit p_MasterNode, int p_VfxIndex, TransformTool.AffineCachePreset p_AffinePreset, ObjectDeployEventExtraPreset p_DeployExtraPreset)
        {
            var (valid, spawnedVfx) =
                await VfxSpawnManager.GetInstance.CastVfxAsync<VFXUnit>(p_VfxIndex, p_AffinePreset, p_PreDelay: 0, p_AutoPlay: false);
            if (valid)
            {
                if (p_DeployExtraPreset._IsFoucsOnMainWeapon)
                {
                    spawnedVfx.SetAttach(p_MasterNode.GetAttachPoint(Unit.UnitAttachPoint.MainWeapon));
                }
                spawnedVfx.MasterNode.SetNode(PrefabInstanceTool.MasterNodeRelateType.Creation, p_MasterNode);
                spawnedVfx.BreakLoop();
                spawnedVfx.SetPlay(0);
            }
        }
        
        private async void DeploySfx(Unit p_MasterNode, int p_SfxIndex, TransformTool.AffineCachePreset p_AffinePreset, ObjectDeployEventExtraPreset p_DeployExtraPreset)
        {
            var (valid, spawnedSfx) =
                await SfxSpawnManager.GetInstance.GetSfxAsync(p_SfxIndex, _AudioClipMediaTracker, p_AffinePreset, p_PreDelay: 0, p_AutoPlay: false);

            if (valid)
            {
                SetAudioClipPreset(spawnedSfx.MediaPreset);
                spawnedSfx.MasterNode.SetNode(PrefabInstanceTool.MasterNodeRelateType.Creation, p_MasterNode);
                spawnedSfx.SetPlay(0);
            }
        }

        private async void DeployProjector(Unit p_MasterNode, int p_ProjectorIndex, TransformTool.AffineCachePreset p_AffinePreset, ObjectDeployEventExtraPreset p_DeployExtraPreset)
        {
            var (valid, spawnedProjector) =
                await ProjectorSpawnManager.GetInstance.ProjectAsync<SimpleProjector>(p_ProjectorIndex, p_AffinePreset);
        }
#endif
        
        private async void DeployVfxProjectile(Unit p_MasterNode, int p_VfxIndex, float p_SqrDistanceTarget, TransformTool.AffineCachePreset p_AffinePreset, ObjectDeployEventExtraPreset p_DeployExtraPreset)
        {
            /*var (valid, spawnedVfxProjectile) =
                await VfxSpawnManager.GetInstance.CastVfxProjectileAsync<VFXProjectileBase>(p_VfxIndex, p_AffinePreset, p_PreDelay: 0, p_AutoPlay: false);
            
            if (valid)
            {
                spawnedVfxProjectile.MasterNode.SetNode(PrefabInstanceTool.MasterNodeRelateType.Creation, p_MasterNode);
                // 배치이벤트에 의해 선정된 범위 내 타겟이 있는 경우에는 해당 타겟을 향해 투사체가 동작한다.
                // 선정된 타겟이 없는 경우에는 MasterNode 유닛을 기준으로
                spawnedVfxProjectile.SetTraceTarget(p_DeployExtraPreset.RangeTarget.t_Filtered, p_SqrDistanceTarget);
                spawnedVfxProjectile.BreakLoop();
                spawnedVfxProjectile.SetPlay(0);
            }*/
        }
        
        private async void DeployUnit(Unit p_MasterNode, int p_UnitIndex, TransformTool.AffineCachePreset p_AffinePreset, ObjectDeployEventExtraPreset p_DeployExtraPreset)
        {
            var (valid, spawnedUnit) =
                await UnitSpawnManager.GetInstance.SpawnUnitAsync<Unit>(p_UnitIndex, p_AffinePreset);
        }
        
        private void DeployAddForce(Unit p_MasterNode, int p_AddForceIndex, TransformTool.AffineCachePreset p_AffinePreset, ObjectDeployEventExtraPreset p_DeployExtraPreset)
        {
            p_MasterNode._PhysicsObject.AddVelocity(new PhysicsTool.UnitAddForceParams(p_AddForceIndex, _MasterNode));
        }
        
        /*private void DeployHitFilter(Unit p_MasterNode, int p_EventSequence, int p_HitFilterIndex,
            TransformTool.AffineCachePreset p_PrevAffinePreset, TransformTool.AffineCachePreset p_CurrentAffinePreset, ObjectDeployEventExtraPreset p_DeployExtraPreset)
        {
            var prevPosition = p_PrevAffinePreset.Position;
            var curPosition = p_CurrentAffinePreset.Position;
            var deltaDirection = prevPosition.GetDirectionUnitVectorTo(curPosition);
            var hitVariablePreset = new UnitTool.UnitAddForcePreset(p_DeployExtraPreset.OriginPivot.GetAffinePivotPosition(), curPosition, deltaDirection);
            var (filterResultType, _) = UnitHitManager.GetInstance.TossDamageWithNullableObserver(p_MasterNode, p_HitFilterIndex, hitVariablePreset, p_CurrentAffinePreset, _BlockBusterFlag, _HitProcessObserver);

            if (_BlockBusterFlag && filterResultType != UnitFilterTool.FilterResultType.None)
            {
                DeployStateStorage[p_EventSequence].DeploySequenceState = DeploySequenceState.ReserveLastDeployEvent;
            }
        }*/

        private async void DeployAutoMutton(Unit p_MasterNode, int p_AutoMuttonIndex, TransformTool.AffineCachePreset p_AffinePreset, ObjectDeployEventExtraPreset p_DeployExtraPreset)
        {
            var (valid, spawnedMutton) =
                await AutoMuttonSpawnManager.GetInstance.RunAutoMuttonAsync<AutoMuttonBase>(p_AutoMuttonIndex, p_AffinePreset, p_PreDelay: 0, p_AutoPlay: false);
            
            if (valid)
            {
                spawnedMutton.MasterNode.SetNode(PrefabInstanceTool.MasterNodeRelateType.Creation, p_MasterNode);
                spawnedMutton.FocusNode.SetNode(PrefabInstanceTool.FocusNodeRelateType.Enemy, p_DeployExtraPreset.RangeTarget.t_Filtered);
                spawnedMutton.Run(0);
            }
        }
        
        private void DeployEventMap(Unit p_MasterNode, int p_DeployMapIndex, TransformTool.AffineCachePreset p_AffinePreset, ObjectDeployEventExtraPreset p_DeployExtraPreset)
        {
            ObjectDeployLoader.GetInstance.CastDeployEventMap(p_DeployMapIndex, p_MasterNode, p_AffinePreset);
        }

        /*private void DeployBuff(Unit p_MasterNode, int p_BuffIndex)
        {
#if !SERVER_DRIVE
            LamiereGameManager.GetInstanceUnSafe.ApplyQuickItem(p_BuffIndex, p_MasterNode as LamiereUnit);
#endif
        }*/
    }
}