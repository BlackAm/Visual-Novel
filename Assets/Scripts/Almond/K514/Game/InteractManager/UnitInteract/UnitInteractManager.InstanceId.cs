using System.Collections.Generic;
using UnityEngine;

namespace k514
{
    public partial class UnitInteractManager
    {
        #region <Fields>

        /// <summary>
        /// 현재 활성화된 유닛의 인스턴스 아이디를 캐싱하는 테이블
        /// </summary>
        private Dictionary<int, Unit> _InstanceIdTable;
        
        #endregion

        #region <Callbacks>

        private void OnCreated_InstanceId()
        {
            _InstanceIdTable = new Dictionary<int, Unit>();
        }

        #endregion

        #region <Methods>

        private void AddInstanceId(Unit p_Spawned)
        {
            var tryId = p_Spawned.gameObject.GetInstanceID();
            _InstanceIdTable[tryId] = p_Spawned;
        }

        private void RemoveInstanceId(Unit p_Spawned)
        {
            var tryId = p_Spawned.gameObject.GetInstanceID();
            _InstanceIdTable[tryId] = null;
        }
        
        public (bool, Unit) TryGetUnit(GameObject p_GameObject)
        {
            var tryId = p_GameObject.GetInstanceID();
            var tryUnit = _InstanceIdTable[tryId];

            return (tryUnit.IsValid(), tryUnit);
        }

        #endregion
    }
}