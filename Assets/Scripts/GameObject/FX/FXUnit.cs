using UnityEngine;

namespace BlackAm
{
    public abstract class FXUnit : PrefabInstance
    {
        #region <Methods>

        public void SetAttach(Transform p_Transform)
        {
            _Transform.SetParent(p_Transform, false);   
        }
        
        #endregion
    }
}