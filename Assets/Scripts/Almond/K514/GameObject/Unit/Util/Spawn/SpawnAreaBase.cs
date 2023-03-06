using UnityEngine;

namespace k514
{
    public abstract class SpawnAreaBase : MonoBehaviour
    {
        #region <Fields>

        private int SpawnCountRemaind;
        private int SpawnCountAtOnce;

        #endregion
        
        #region <Methods>

        public bool Spawn(int p_PrefabIndex)
        {
            return false;
        }

        #endregion
    }
}