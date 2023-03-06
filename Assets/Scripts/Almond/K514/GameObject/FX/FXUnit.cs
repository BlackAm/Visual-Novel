using UnityEngine;

namespace k514
{
    public abstract class FXUnit : PrefabInstance
    {
        #region <Methods>

        public void SetAttach(Transform p_Transform)
        {
            _Transform.SetParent(p_Transform, false);   
        }

        public void SetAttach(Unit p_Pivot)
        {
            SetAttach(p_Pivot._Transform);   
        }
        
        #endregion
    }
}