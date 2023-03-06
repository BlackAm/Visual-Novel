using UnityEngine;

namespace k514
{
    public static partial class ActableTool
    {
        #region <Methods>

        public static void Roar(this Vector3 p_RoarPivot, Unit p_FromUnit, float p_Distance, int p_HitParameterIndex, int p_VfxIndex = 0)
        {
            /*var affine = new TransformTool.AffineCachePreset(p_FromUnit, true);
            var scale = affine.ScaleFactor;
            var scaledDistance = scale * p_Distance;
            var (valid, spawned) = VfxSpawnManager.GetInstance.CastVfx<VFXUnit>(p_VfxIndex, affine);
            
#if !SERVER_DRIVE
            CameraManager.GetInstanceUnSafe.SetShake(Vector3.left, 20, 0, 300, 10);
            
            // 원격 서버로부터 타격 판정을 제어받는 유닛은 클라이언트에서 필터링을 수행하지 않는다.
            if (RemoteClientManager.GetInstanceUnSafe.IsBlockedHandleHitEvent(p_FromUnit))
            {
            }
            else
#endif
            {
                var result = p_FromUnit.FindEnemyFromPivot(p_RoarPivot, scaledDistance, Unit.UnitStateType.UnitFightableFilterMask);
                if (result)
                {
                    var hitMessage = UnitHitPresetData.GetInstanceUnSafe[p_HitParameterIndex].GetHitMessage();
                    hitMessage.AddHitMessageAttributeFlag(HitMessage.HitMessageAttributeFlag.AlwaysHit);
                    
                    var filterList = p_FromUnit._UnitFilterResultSet;
                    foreach (var targetUnit in filterList)
                    {
                        var targetCenterPos = targetUnit.GetCenterPosition();
                        var hitForcePreset = new UnitTool.UnitAddForcePreset(p_RoarPivot, targetCenterPos, p_RoarPivot.GetDirectionUnitVectorTo(targetCenterPos));
                        targetUnit.HitUnit(p_FromUnit, hitMessage, hitForcePreset, false);
                    }
                }
            }*/
        }

        #endregion
    }
}