using UnityEngine;
#if UNITY_EDITOR
using UnityEngine.AI;

namespace k514
{
    public static class EditorTool
    {
        public static void OffMeshLinkCorrentHeight(this OffMeshLink p_OffMeshLink)
        {
            var startTransform = p_OffMeshLink.startTransform;
            var endTransform = p_OffMeshLink.endTransform;
            var rayDirection = Vector3.down;
            var correctOffset = -1000f * rayDirection;

            (_, startTransform.position) =
                PhysicsTool.GetHighestObjectPosition_RayCast(
                    startTransform.position + correctOffset, GameManager.Obstacle_Terrain_LayerMask, QueryTriggerInteraction.Ignore
                );
            
            (_, endTransform.position) =
                PhysicsTool.GetHighestObjectPosition_RayCast(
                    endTransform.position + correctOffset, GameManager.Obstacle_Terrain_LayerMask, QueryTriggerInteraction.Ignore
                );
                

        }
    }
}
#endif