using System.Collections.Generic;
using UnityEngine;

namespace k514
{
    public interface IVolitional : IIncarnateUnit
    {
        UnitRoleDataRoot.UnitRoleType _RoleType { get; }
        IVolitionalTableRecordBridge _RoleRecord { get; }
        IVolitional OnInitializeRole(UnitRoleDataRoot.UnitRoleType p_RoleType, Unit p_TargetUnit, IVolitionalTableRecordBridge p_RolePreset);
        string GetRoleName();
    }
}