
using UnityEngine;
#if !SERVER_DRIVE
using UnityEngine.EventSystems;

namespace k514
{
    public partial class TouchEventManager
    {
        #region <Consts>

        private const float __CastUpperBound = 100f;

        #endregion
        
        #region <Callbacks>

        private void OnAwakeTouchObject()
        {
        }
        
        public void TrySelectObject(PointerEventData p_EventData)
        {
            // 드래그 종료시 호출되는 경우에는, 유닛 선택을 수행하지 않는다.
            if (p_EventData.dragging) return;
            
            var screenPos = p_EventData.position;
            var ray = CameraManager.GetInstanceUnSafe.MainCamera.ScreenPointToRay(screenPos);
            var resultTuple = PhysicsTool.GetNearestObject_RayCast(ray, __CastUpperBound, GameManager.Terrain_UnitEC_LayerMask, QueryTriggerInteraction.Collide);

            if (resultTuple.Item1)
            {
                var rayCastHit = resultTuple.Item2;
                var hitObject = rayCastHit.transform.gameObject;
                var targetLayer = hitObject.layer;
 
                if (hitObject.HasUnitLayer())
                {
                    var tryUnit = hitObject.GetComponent<Unit>();
                    var eventPreset = new TouchEventPreset(tryUnit, p_EventData);
                    if (tryUnit.IsPlayer)
                    {
                        TriggerPropertyEvent(TouchEventRoot.TouchEventType.PlayerSelected | TouchEventRoot.TouchEventType.UnitSelected, eventPreset);
                    }
                    else
                    {
                        TriggerPropertyEvent(TouchEventRoot.TouchEventType.UnitSelected, eventPreset);
                    }
                }
                else
                {
                    var eventPreset = new TouchEventPreset(rayCastHit.point, p_EventData);
                    TriggerPropertyEvent(TouchEventRoot.TouchEventType.PositionSelected, eventPreset);
                }
            }
        }

        #endregion
    }
}
#endif