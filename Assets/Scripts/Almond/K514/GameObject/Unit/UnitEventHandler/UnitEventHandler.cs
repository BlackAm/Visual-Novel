using System;

namespace k514
{
    public class UnitEventHandler : PoolingObject<UnitEventHandler>
   {
        #region <Fields>

        public int _InteractId;
        public Unit _ThisUnit;
        public UnitEventSender UnitEventSender;
        
        #endregion

        #region <Callbacks>

        /// <summary>
        /// UnitInteractManager에 의해 해당 콜백시점 이전에 이미 SetUnit을 통해 유닛정보를 가지게 됨
        /// </summary>
        public override void OnSpawning()
        {
            UnitEventSender = new UnitEventSender();
        }

        public override void OnPooling()
        {
        }

        public override void OnRetrieved()
        {
            UnitEventSender.ClearReceiverGroup();
            SetUnit(default, default);
        }

        public void OnEventTriggered(UnitEventHandlerTool.UnitEventType p_Type, UnitEventMessage p_UnitEventMessage)
        {
            UnitEventSender.WhenPropertyModified(p_Type, p_UnitEventMessage);
        }

        #endregion

        #region <Methods>

        public void SetUnit(int p_InteractId, Unit p_TargetUnit)
        {
            _InteractId = p_InteractId;
            _ThisUnit = p_TargetUnit;
        }

        public void AddReceiver(UnitEventReceiver p_EventReceiver)
        {
            UnitEventSender.AddReceiver(p_EventReceiver);
        }

        public UnitEventReceiver GetReceiver(UnitEventHandlerTool.UnitEventType p_EventType,
            Action<UnitEventHandlerTool.UnitEventType, UnitEventMessage> p_Handler)
        {
            return UnitEventSender.GetEventReceiver<UnitEventReceiver>(p_EventType, p_Handler);
        }

        public void RemoveReceiver(UnitEventReceiver p_EventReceiver)
        {
            UnitEventSender.RemoveReceiver(p_EventReceiver);
        }

        public bool IsSender(UnitEventHandlerTool.UnitEventType p_Type)
        {
            return UnitEventSender.HasEvent(p_Type);
        }

        #endregion
   }
}