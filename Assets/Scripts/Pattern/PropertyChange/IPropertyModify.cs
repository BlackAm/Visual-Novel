using System;
using System.Collections.Generic;

namespace BlackAm
{
    /// <summary>
    /// 데이터 타입 M의 특정한 자료형 하나의 값이 변했을 때, 해당 이벤트를 통신하기 위한 인터페이스
    /// </summary>
    /// <typeparam name="M">변경사항을 추적할 변수의 타입</typeparam>
    public interface ISinglePropertyModifyNotify<V>
    {
        /// <summary>
        /// 특정 프로퍼티의 값이 기본값이 된 경우
        /// </summary>
        void WhenPropertyTurnToDefault();
        
        /// <summary>
        /// 특정 프로퍼티의 값이 변경된 경우
        /// </summary>
        void WhenPropertyTurnTo(V p_Value);
    }
    
    /// <summary>
    /// 키 타입 K의 특정한 키 값에 대응하는 데이터 타입 M의 값이 변했을 때, 해당 이벤트를 통신하기 위한 인터페이스
    /// </summary>
    /// <typeparam name="M">변경사항을 추적할 변수의 타입</typeparam>
    public interface IMultiPropertyModifyNotify<K, V>
    {
        /// <summary>
        /// 특정 프로퍼티의 값이 기본값이 된 경우
        /// </summary>
        void WhenPropertyTurnToDefault(K p_Key);
        
        /// <summary>
        /// 특정 프로퍼티의 값이 변경된 경우
        /// </summary>
        void WhenPropertyTurnTo(K p_Key, V p_Value);
    }
    
    /// <summary>
    /// 해당 인터페이스의 구현체는 특정한 변수의 값이 변경되는 경우 호출될 콜백을 구현해야만 한다.
    /// </summary>
    /// <typeparam name="M">변경사항을 추적할 변수의 타입</typeparam>
    /// <typeparam name="K">변경사항이 발생했을 때, 콜백과 함께 사용할 데이터 타입. 파라미터</typeparam>
    public interface IPropertyModifyNotify<M, K>
    {
        /// <summary>
        /// 프로퍼티 값이 변경되는 경우 호출할 수 있도록 구현할 것
        /// </summary>
        /// <param name="p_Type">변경되는 값</param>
        /// <param name="p_Value">해당 변경 콜백에 사용할 임의 타입의 임의의 값</param>
        void WhenPropertyModified(M p_Type, K p_TryValue);
    }
    
    /// <summary>
    /// IPropertyModifyNotify[M, K] 인터페이스에 더해 추적하는 변수가 변경된 경우 특정한 그룹에게 해당 이벤트를 전달하도록 구현하는 모델
    /// </summary>
    /// <typeparam name="M">변경사항을 추적할 변수의 타입</typeparam>
    /// <typeparam name="K">변경사항이 발생했을 때, 콜백과 함께 사용할 데이터 타입. 파라미터</typeparam>
    public interface IPropertyModifyEventSender<M, K> : IPropertyModifyNotify<M, K>, _IDisposable where M : struct
    {
        /// <summary>
        /// 수신자 그룹
        /// </summary>
        Dictionary<M, HashSet<IPropertyModifyEventReceiver<M, K>>> EventReceiverGroup { get; set; }
        
        /// <summary>
        /// 수신자 그룹 미러
        /// </summary>
        Dictionary<M, HashSet<IPropertyModifyEventReceiver<M, K>>> EventReceiverMirrorGroup { get; set; }

        /// <summary>
        /// M 데이터 타입 순회자
        /// </summary>
        M[] _Enumerator { get; set; }

        /// <summary>
        /// 삭제할 원소를 적용시키도록 구현
        /// </summary>
        void ApplyRemovedReceiver();

        /// <summary>
        /// 수신자 추가 메서드
        /// </summary>
        void AddReceiver(IPropertyModifyEventReceiver<M, K> p_Receiver);
        
        /// <summary>
        /// 특정 수신자 삭제 메서드
        /// </summary>
        void RemoveReceiver(IPropertyModifyEventReceiver<M, K> p_Receiver);

        /// <summary>
        /// 수신자 그룹을 초기화 시키는 메서드
        /// </summary>
        void ClearReceiverGroup();
        
        /// <summary>
        /// 플래그 마스크를 검증하는 논리 메서드
        /// </summary>
        bool HasEvent(M p_Type, M p_Compare);
    }

    /// <summary>
    /// IPropertyModifyEventSender[M,K] 인터페이스를 통해서 IPropertyModifyEventSender[M,K] 인터페이스의 M객체가 변경되었을 때,
    /// 해당 이벤트를 수신받는 오브젝트를 기술하는 인터페이스
    /// </summary>
    /// <typeparam name="M">변경사항을 추적할 변수의 타입</typeparam>
    /// <typeparam name="K">변경사항이 발생했을 때, 콜백과 함께 사용할 데이터 타입. 파라미터</typeparam>
    public interface IPropertyModifyEventReceiver<M, K> : _IDisposable where M : struct
    {
        /// <summary>
        /// Notifier 그룹
        /// </summary>
        List<IPropertyModifyEventSender<M, K>> EventSenderGroup { get; set; }
        
        /// <summary>
        /// 수신받는 이벤트 타입
        /// </summary>
        M _ThisType { get; set; }

        /// <summary>
        /// 이벤트 핸들러
        /// </summary>
        Action<M, K> _ThisEvent { get; set; }
        
        /// <summary>
        /// 특정 타입의 프로퍼티가 변경된 경우 호출되는 콜백함수
        /// </summary>
        void OnPropertyModifyEventReceived(M p_Type, K p_TryValue);

        /// <summary>
        /// NotifierGroup 를 순회해서, IPropertyModifyNotifier.RemoveReceive 호출하도록 구현해야 한다.
        /// </summary>
        void ClearSenderGroup();
    }
}