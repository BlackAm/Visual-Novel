using System.Collections.Generic;
using UnityEngine;
 
namespace k514
{
    public partial class Unit
    {
        #region <Fields>
 
        /// <summary>
        /// 현재 선택된 해당 유닛의 역할(Role) 기술 모듈
        /// </summary>
        public IVolitional _RoleObject;

        /// <summary>
        /// Volational 모듈
        /// </summary>
        private UnitModuleCluster<UnitRoleDataRoot.UnitRoleType, IVolitional> _RoleModule;

        #endregion
         
        #region <Callbacks>
 
        private void OnAwakeRole()
        {
            _RoleModule 
                = new UnitModuleCluster<UnitRoleDataRoot.UnitRoleType, IVolitional>(
                    this, UnitModuleDataTool.UnitModuleType.Role, _PrefabExtraDataRecord.RolePresetIdList);
            _RoleObject = (IVolitional) _RoleModule.CurrentSelectedModule;
        }
 
        private void OnPoolingRole()
        {
            _RoleObject = _RoleModule.SwitchModule();
        }
 
        private void OnRetrieveRole()
        {
            _RoleObject?.OnMasterNodeRetrieved();
        }
 
        #endregion
         
        #region <Methods>
 
        public void SwitchRole()
        {
            _RoleObject = _RoleModule.SwitchModule();
        }
        
        public void SwitchRole(UnitRoleDataRoot.UnitRoleType p_ModuleType)
        {
            _RoleObject = _RoleModule.SwitchModule(p_ModuleType);
        }
        
        public void SwitchRole(int p_Index)
        {
            _RoleObject = _RoleModule.SwitchModule(p_Index);
        }
        
        private void DisposeRole()
        {
            if (_RoleModule != null)
            {
                _RoleModule.Dispose();
                _RoleModule = null;
            }

            _RoleObject = null;
        }
         
        #endregion
    }
}