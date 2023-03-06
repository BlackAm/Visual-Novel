using UnityEngine;

namespace k514
{
    public class FootStepLocation : MonoBehaviour
    {
        public TextureType _TextureType;

        public int _Priority;
        
        #region <Callbacks>

#if !SERVER_DRIVE
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
                        unit.Item2.OnEnterFootStep(this);
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
                        unit.Item2.OnExitFootStep(this);
                    }
                    break;
            }
        }
#endif

        #endregion

        #region <Enums>

        public enum TextureType
        {
            None,
            Forest,
            Stone,
            Sand,
            Snow,
            DeepWater,
            ShallowWater,
        }

        public enum TextureCompareType
        {
            None,
            TimeStamp,
            Priority,
        }

        #endregion
    }
}