using UnityEngine;

namespace k514
{
    public static partial class PhysicsTool
    {
        public static void SetLayerBaseCollisionDetection(this GameManager.GameLayerType p_Left, GameManager.GameLayerType p_Right, bool p_Flag)
        {
            Physics.IgnoreLayerCollision((int) p_Left, (int) p_Right, p_Flag);
        }
        
        public static void LoadLayerBaseCollisionDetection()
        {
            var tryTable = PhysicsCollisionLayerData.GetInstanceUnSafe.GetTable();
            foreach (var layerTypeKV in tryTable)
            {
                var layerType = layerTypeKV.Key;
                var layerCollisionBlockList = layerTypeKV.Value.LayerBasedCollisionDetectionBlockTable;
                if (!ReferenceEquals(null, layerCollisionBlockList))
                {
                    foreach (var blockType in layerCollisionBlockList)
                    {
                        layerType.SetLayerBaseCollisionDetection(blockType, true);
                    }
                }
            }
        }
    }
}