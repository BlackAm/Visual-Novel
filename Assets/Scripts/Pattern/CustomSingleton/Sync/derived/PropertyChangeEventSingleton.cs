using System;

namespace BlackAm
{
    /// <summary>
    /// 특정한 이벤트 타입의 값 변화를 추적하는 싱글톤 추상 클래스
    /// </summary>
    public abstract class PropertyChangeEventSingleton<M, K, T, Me> : Singleton<Me> 
        where M : PropertyModifyEventSenderImp<K, T>, new()
        where K : struct
        where Me : PropertyChangeEventSingleton<M, K, T, Me>, new()
    {
        #region <Methods>

        /// <summary>
        /// 싱글톤 내부 초기화 메서드
        /// </summary>
        protected override void InitInstance()
        {
            _PropertyChangeEventSender = new M();
            
            base.InitInstance();
        }

        #endregion
        
        #region <PropertyChange>

        /// <summary>
        /// 프로퍼티 변경 이벤트 송신자
        /// </summary>
        protected M _PropertyChangeEventSender;

        /// <summary>
        /// 이벤트 수신자를 생성하여 추가하고 리턴하는 메서드
        /// </summary>
        public Me2 GetEventReceiver<Me2>(K p_EventType, Action<K, T> p_EventHandler)
            where Me2 : PropertyModifyEventReceiverImp<K, T>, new()
        {
            return _PropertyChangeEventSender.GetEventReceiver<Me2>(p_EventType, p_EventHandler);
        }

        /// <summary>
        /// 특정한 타입의 이벤트를 전파하는 메서드
        /// </summary>
        public void TriggerPropertyEvent(K p_EventType, T p_EventPreset)
        {
            _PropertyChangeEventSender.WhenPropertyModified(p_EventType, p_EventPreset);
        }

        /// <summary>
        /// 지정한 이벤트 수신자를 제거하는 메서드
        /// </summary>
        public void RemoveReceiver<Me2>(Me2 p_Receiver) where Me2 : PropertyModifyEventReceiverImp<K, T>, new()
        {
            _PropertyChangeEventSender.RemoveReceiver(p_Receiver);
        }

        #endregion
        
        #region <Disposable>

        /// <summary>
        /// 인스턴스가 파기될 때 수행할 작업을 기술한다.
        /// </summary>
        protected override void DisposeUnManaged()
        {
            if (_PropertyChangeEventSender != null)
            {
                _PropertyChangeEventSender.Dispose();
                _PropertyChangeEventSender = null;
            }
            
            base.DisposeUnManaged();
        }

        #endregion
    }
}