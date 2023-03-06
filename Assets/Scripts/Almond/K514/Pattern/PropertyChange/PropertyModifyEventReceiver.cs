using System;
using System.Collections.Generic;
using UnityEngine;

namespace k514
{
    /// <summary>
    /// 지정한 타입의 이벤트가 발생했을 때, 해당 이벤트를 수신하는 인터페이스 구현체
    /// </summary>
    /// <typeparam name="EventType">이벤트 타입</typeparam>
    /// <typeparam name="EventPreset">이벤트 프리셋</typeparam>
    public abstract class PropertyModifyEventReceiverImp<EventType, EventPreset> : IPropertyModifyEventReceiver<EventType, EventPreset> where EventType : struct
    {
        ~PropertyModifyEventReceiverImp()
        {
            Dispose();
        }
        
        #region <Fields>

        /// <summary>
        /// 해당 이벤트 수신자에게 이벤트를 송신하는 오브젝트 그룹
        /// </summary>
        public List<IPropertyModifyEventSender<EventType, EventPreset>> EventSenderGroup { get; set; }

        /// <summary>
        /// 해당 수신자가 수신받는 이벤트 타입 플래그 마스크
        /// </summary>
        public EventType _ThisType { get; set; }

        /// <summary>
        /// 해당 수신자의 이벤트 핸들러
        /// </summary>
        public Action<EventType, EventPreset> _ThisEvent { get; set; }

        /// <summary>
        /// 해당 이벤트 리시버를 일시적으로 비활성시키고자 할 때 사용하는 플래그 필드
        /// </summary>
        private bool ReceiverBlockFlag;
        
        #endregion

        #region <Constructor>

        public PropertyModifyEventReceiverImp()
        {
            EventSenderGroup = new List<IPropertyModifyEventSender<EventType, EventPreset>>();
        }
        
        public PropertyModifyEventReceiverImp(EventType p_Type, Action<EventType, EventPreset> p_Event) : this()
        {
            InitEventReceiver(p_Type, p_Event);
        }

        public void InitEventReceiver(EventType p_Type, Action<EventType, EventPreset> p_Event)
        {
            _ThisType = p_Type;
            _ThisEvent = p_Event;
        }

        #endregion

        #region <Callbacks>

        /// <summary>
        /// 해당 이벤트 타입에 맞는 이벤트가 송신됨, 만약 여러 이벤트를 플래그로 가진다면 그 플래그 중에
        /// 해당하는 이벤트 타입을 파라미터로 받음
        /// </summary>
        public virtual void OnPropertyModifyEventReceived(EventType p_Type, EventPreset p_Preset)
        {
            if (!ReceiverBlockFlag)
            {
                _ThisEvent.Invoke(p_Type, p_Preset);
            }
        }

        #endregion

        #region <Methods>

        /// <summary>
        /// 이벤트 핸들러에 블록 플래그를 설정하는 메서드
        /// </summary>
        public void SetReceiverBlock(bool p_BlockFlag)
        {
            ReceiverBlockFlag = p_BlockFlag;
        }

        /// <summary>
        /// 이벤트 수신자를 그룹을 비우는 메서드
        /// </summary>
        public void ClearSenderGroup()
        {
            var count = EventSenderGroup.Count;
            for (int i = count - 1; i > -1; i--)
            {
                var eventSender = EventSenderGroup[i];
                eventSender.RemoveReceiver(this);
            }
        }

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
            ClearSenderGroup();
        }

        #endregion
    }
}