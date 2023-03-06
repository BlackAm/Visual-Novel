using System.Collections.Generic;
using UnityEngine;

namespace k514
{
    /// <summary>
    /// 지정한 용량 만큼의 0을 포함한 자연수를 중복없이 가지는 리스트를 생성하고
    /// 셔플한 순열을 제공하는 클래스
    /// </summary>
    public class RandomIndexList
    {
        #region <Fields>

        private List<int> _RandomIndexList;

        #endregion

        #region <Constructor>

        public RandomIndexList(int p_IndexCapacity)
        {
            _RandomIndexList = new List<int>();
            SetCapacity(p_IndexCapacity);
        }

        #endregion

        #region <Indexer>

        public int this[int p_Index]
        {
            get { return _RandomIndexList[Mathf.Clamp(p_Index, 0, _RandomIndexList.Count)]; }
        }

        #endregion
        
        #region <Methods>

        public void SetCapacity(int p_Capacity)
        {
            _RandomIndexList.Clear();
            for (int i = 0; i < p_Capacity; i++)
            {
                _RandomIndexList.Add(i);
            }
        }

        public void ShuffleIndex()
        {
            int n = _RandomIndexList.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, n + 1);
                var value = _RandomIndexList[k];
                _RandomIndexList[k] = _RandomIndexList[n];
                _RandomIndexList[n] = value;
            }

        }

        #endregion
    }
}