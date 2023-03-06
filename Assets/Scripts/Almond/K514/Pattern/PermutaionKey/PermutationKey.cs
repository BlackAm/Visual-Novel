using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace k514
{
    public class PermutationKey
    {
        #region <Fields>

        /// <summary>
        /// FIFO
        /// </summary>
        private Queue<int> _BreakedKeyStack;

        #endregion

        #region <Constructors>

        public PermutationKey(int p_Capacity)
        {
            _BreakedKeyStack = new Queue<int>(Enumerable.Range(0, p_Capacity));
        }

        #endregion

        #region <Methods>

        public int GetValidKey()
        {
            return  _BreakedKeyStack.Dequeue();
        }

        public void ReturnKey(int p_Key)
        {
            _BreakedKeyStack.Enqueue(p_Key);
        }

        #endregion
    }
}