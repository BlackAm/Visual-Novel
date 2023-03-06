using System;
using UnityEngine;

namespace k514
{
    public class UnitEventSender : PropertyModifyEventSenderImp<UnitEventHandlerTool.UnitEventType, UnitEventMessage>
    {
        public override bool HasEvent(UnitEventHandlerTool.UnitEventType p_Type, UnitEventHandlerTool.UnitEventType p_Compare)
        {
            return p_Type.HasAnyFlagExceptNone(p_Compare);
        }
    }
    
    public class UnitEventReceiver : PropertyModifyEventReceiverImp<UnitEventHandlerTool.UnitEventType, UnitEventMessage>
    {
        #region <Constructor>

        public UnitEventReceiver() : base()
        {
        }

        public UnitEventReceiver(UnitEventHandlerTool.UnitEventType p_EventType,
            Action<UnitEventHandlerTool.UnitEventType, UnitEventMessage> p_EventHandler) : base(p_EventType, p_EventHandler)
        {
        }

        #endregion
    }

    public struct UnitEventMessage
    {
        #region <Fields>

        public Unit TriggerUnit;
        public UnitEventHandler TriggerUnitHandler;
        public bool BoolValue;
        public bool BoolValue2;
        public Color ColorValue;
        
        #endregion

        #region <Constructors>

        public UnitEventMessage(Unit p_Unit)
        {
            TriggerUnit = p_Unit;
            TriggerUnitHandler = p_Unit._UnitEventHandler;
            BoolValue = default;
            BoolValue2 = default;
            ColorValue = default;
        }
        
        public UnitEventMessage(Unit p_Unit, Color p_Color)
        {
            TriggerUnit = p_Unit;
            TriggerUnitHandler = p_Unit._UnitEventHandler;
            BoolValue = default;
            BoolValue2 = default;
            ColorValue = p_Color;
        }
        
        public UnitEventMessage(Unit p_Unit, bool p_Value)
        {
            TriggerUnit = p_Unit;
            TriggerUnitHandler = p_Unit._UnitEventHandler;
            BoolValue = p_Value;
            BoolValue2 = default;
            ColorValue = default;
        }
        
        public UnitEventMessage(Unit p_Unit, bool p_Value, bool p_Value2)
        {
            TriggerUnit = p_Unit;
            TriggerUnitHandler = p_Unit._UnitEventHandler;
            BoolValue = p_Value;
            BoolValue2 = p_Value2;
            ColorValue = default;
        }

        #endregion
    }
}