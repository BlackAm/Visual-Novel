using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlackAm
{
    /// <summary>
    /// 지정한 타입의 이벤트가 발생했을 때, 해당 이벤트를 송신하는 인터페이스 구현체
    /// </summary>
    /// <typeparam name="EventType">이벤트 타입</typeparam>
    /// <typeparam name="EventPreset">이벤트 프리셋</typeparam>
    public abstract class PropertyModifyEventSenderImp<EventType, EventPreset> : IPropertyModifyEventSender<EventType, EventPreset> where EventType : struct
    {
        ~PropertyModifyEventSenderImp()
        {
            Dispose();
        }
        
        #region <Fields>

        /// <summary>
        /// 이벤트 타입별, 이벤트를 수신받을 수신자 그룹
        /// </summary>
        public Dictionary<EventType, HashSet<IPropertyModifyEventReceiver<EventType, EventPreset>>> EventReceiverGroup { get; set; }
        
        /// <summary>
        /// 이벤트 타입별, 이벤트를 수신받을 수신자 미러 그룹
        /// </summary>
        public Dictionary<EventType, HashSet<IPropertyModifyEventReceiver<EventType, EventPreset>>> EventReceiverMirrorGroup { get; set; }

        /// <summary>
        /// 이벤트 타입 열거형 상수 순환자
        /// </summary>
        public EventType[] _Enumerator { get; set; }

        #endregion

        #region <Constructor>

        public PropertyModifyEventSenderImp()
        {
            EventReceiverGroup = new Dictionary<EventType, HashSet<IPropertyModifyEventReceiver<EventType, EventPreset>>>();
            EventReceiverMirrorGroup = new Dictionary<EventType, HashSet<IPropertyModifyEventReceiver<EventType, EventPreset>>>();
            _Enumerator = SystemTool.GetEnumEnumerator<EventType>(SystemTool.GetEnumeratorType.ExceptMaskNone);
            
            foreach (var eventType in _Enumerator)
            {
                EventReceiverGroup.Add(eventType, new HashSet<IPropertyModifyEventReceiver<EventType, EventPreset>>());
                EventReceiverMirrorGroup.Add(eventType, new HashSet<IPropertyModifyEventReceiver<EventType, EventPreset>>());
            }
        }

        #endregion
        
        #region <Callbacks>

        /// <summary>
        /// 이벤트 송신 트리거 콜백
        /// </summary>
        /// <param name="p_Type">발생한 이벤트 타입</param>
        /// <param name="p_Preset">이벤트 프리셋</param>
        public void WhenPropertyModified(EventType p_Type, EventPreset p_Preset)
        {
            ApplyRemovedReceiver();
            
            foreach (var eventType in _Enumerator)
            {
                if (HasEvent(p_Type, eventType))
                {
                    var targetReceiverGroup = EventReceiverGroup[eventType];
                    foreach (var eventReceiver in targetReceiverGroup)
                    {
                        eventReceiver.OnPropertyModifyEventReceived(eventType, p_Preset);
                    }
                }
            }
        }

        #endregion

        #region <Methods>

        /// <summary>
        /// 지정한 타입의 수신자를 생성하여 추가하고 리턴하는 메서드
        /// </summary>
        public M GetEventReceiver<M>(EventType p_EventType, Action<EventType, EventPreset> p_EventHandler)
            where M : PropertyModifyEventReceiverImp<EventType, EventPreset>, new()
        {
            var result = new M();
            result.InitEventReceiver(p_EventType, p_EventHandler);
            AddReceiver(result);
            return result;
        }

        /// <summary>
        /// 제거해야할 수신자를 리스트화하여 일괄적으로 처리하는 메서드
        /// </summary>
        public void ApplyRemovedReceiver()
        {
            foreach (var eventType in _Enumerator)
            {
                var targetReceiverGroup = EventReceiverGroup[eventType];
                var targetReceiverRemoveGroup = EventReceiverMirrorGroup[eventType];
                if (targetReceiverRemoveGroup.Count > 0)
                {
                    foreach (var eventReceiver in targetReceiverRemoveGroup)
                    {
                        targetReceiverGroup.Remove(eventReceiver);
                    }
                    targetReceiverRemoveGroup.Clear();
                }
            }
        }

        /// <summary>
        /// 특정 이벤트 타입의 이벤트 수신자를 추가하는 메서드
        /// </summary>
        public void AddReceiver(IPropertyModifyEventReceiver<EventType, EventPreset> p_Receiver)
        {
            if (!ReferenceEquals(null, p_Receiver))
            {
                var added = false;
                foreach (var eventType in _Enumerator)
                {
                    if (HasEvent(p_Receiver._ThisType, eventType))
                    {
                        EventReceiverGroup[eventType].Add(p_Receiver);
                        added = true;
                    }
                }

                if (added)
                {
                    p_Receiver.EventSenderGroup.Add(this);
                }
            }
        }

        /// <summary>
        /// 이벤트 수신자를 제거하는 메서드
        /// </summary>
        public void RemoveReceiver(IPropertyModifyEventReceiver<EventType, EventPreset> p_Receiver)
        {
            if (!ReferenceEquals(null, p_Receiver))
            {
                var removed = false;
                foreach (var eventType in _Enumerator)
                {
                    if (HasEvent(p_Receiver._ThisType, eventType))
                    {
                        EventReceiverMirrorGroup[eventType].Add(p_Receiver);
                        removed = true;
                    }
                }

                if (removed)
                {
                    p_Receiver.EventSenderGroup.Remove(this);
                }
            }
        }

        /// <summary>
        /// 보유한 모든 수신자를 제거하는 메서드
        /// </summary>
        public void ClearReceiverGroup()
        {
            foreach (var @enum in _Enumerator)
            {
                var targetReceiverGroup = EventReceiverGroup[@enum];
                if (targetReceiverGroup.Count > 0)
                {
                    foreach (var eventReceiver in targetReceiverGroup)
                    {
                        eventReceiver.EventSenderGroup.Remove(this);
                    }
                    targetReceiverGroup.Clear();
                    EventReceiverMirrorGroup[@enum].Clear();
                }
            }
        }

        public abstract bool HasEvent(EventType p_Type, EventType p_Compare);

        public bool HasEvent(EventType p_Type) => EventReceiverGroup[p_Type].Count > 0;
        
        #endregion

        #region <Disposable>

        /// <summary>
        /// dispose 패턴 onceFlag
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// dispose 플래그를 초기화 시키는 메서드
        /// </summary>
        public void Rejunvenate()
        {
            IsDisposed = false;
        }
        
        /// <summary>
        /// 인스턴스 파기 메서드
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }
            else
            {
                IsDisposed = true;
                DisposeUnManaged();
            }
        }

        /// <summary>
        /// 인스턴스가 파기될 때 수행할 작업을 기술한다.
        /// </summary>
        protected void DisposeUnManaged()
        {
            ClearReceiverGroup();
        }

        #endregion
    }
}