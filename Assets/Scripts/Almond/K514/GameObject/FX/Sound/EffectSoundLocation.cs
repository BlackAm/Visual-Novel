#if !SERVER_DRIVE
using UnityEngine;

namespace k514
{
    public class EffectSoundLocation : MonoBehaviour
    {
        public int _EffectKey;
        
        public MapEffectSoundManager.MapEffectSound _MapEffectSound;
        
        public int _Priority;
        
        #region <Callbacks>

        private void OnTriggerEnter(Collider p_Collider)
        {
            var tryLayer = (GameManager.GameLayerType) p_Collider.gameObject.layer;
            
            switch (tryLayer)
            {
                case GameManager.GameLayerType.UnitA:
                case GameManager.GameLayerType.UnitB:
                case GameManager.GameLayerType.UnitC:
                case GameManager.GameLayerType.UnitD:
                case GameManager.GameLayerType.UnitE:
                case GameManager.GameLayerType.UnitF:
                case GameManager.GameLayerType.UnitG:
                case GameManager.GameLayerType.UnitH:
                    var unit = UnitInteractManager.GetInstance.TryGetUnit(p_Collider.gameObject);
                    if (unit.Item1)
                    {
                        if (unit.Item2.IsPlayer)
                        {
                            unit.Item2.OnEnterEffectSound(this);
                        }
                    }
                    break;
            }
        }

        private void OnTriggerExit(Collider p_Collider)
        {
            var tryLayer = (GameManager.GameLayerType) p_Collider.gameObject.layer;
            
            switch (tryLayer)
            {
                case GameManager.GameLayerType.UnitA:
                case GameManager.GameLayerType.UnitB:
                case GameManager.GameLayerType.UnitC:
                case GameManager.GameLayerType.UnitD:
                case GameManager.GameLayerType.UnitE:
                case GameManager.GameLayerType.UnitF:
                case GameManager.GameLayerType.UnitG:
                case GameManager.GameLayerType.UnitH:
                    var unit = UnitInteractManager.GetInstance.TryGetUnit(p_Collider.gameObject);
                    if (unit.Item1)
                    {
                        if (unit.Item2.IsPlayer)
                        {
                            unit.Item2.OnExitEffectSound(this);
                        }
                    }
                    break;
            }
        }

        #endregion
    }
}
#endif