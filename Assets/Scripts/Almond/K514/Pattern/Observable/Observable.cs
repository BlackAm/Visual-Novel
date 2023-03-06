
using System;
using System.Collections.Generic;

namespace k514
{
    /// <summary>
    /// msdn "push-based notification"
    /// 특정 자료형 T를 추적하는 Tracker[T] 객체들을 생성하고 관리하는 객체
    ///
    /// 해당 객체에 등록된 옵저버들은 특정한 객체 T를 파라미터로 가지는 콜백 OnNext에 의해
    /// 특정 객체 T를 추적할 수 있으며,
    ///
    /// 해당 객체 등록시에 주어진 Unsubscriber 를 통해 해당 객체로부터 빠져나가는 일이 가능하다.
    ///
    /// PropertyChanged는 프로퍼티 기반 + 임의로 사용할 수 있는 파라미터 추적이라 field 단위로 추적이 가능한 반면,
    /// 해당 옵저버 인터페이스 구현을 통한 추적은 접근 방법이 void 함수라 따로 추적 객체에서 수행할 일을 제어하기엔
    /// 넘길 수 있는 정보가 너무 한정되어 있어서 쓰기 불편하다.
    ///
    /// 둘 다 mutiple 객체에 이벤트를 전달하면서, multiple 객체가 비동기적으로 정해진다는 공통점을 지닌다.
    /// 
    /// </summary>
    /// <typeparam name="M"></typeparam>
    public class ObservableImp<M> : IObservable<M>
    {
        #region <Fields>

        /// <summary>
        /// 현재 T 타입에 대한 활성화된 관측자 그룹
        /// </summary>
        /// 
        private List<Observer<M>> _ObserverGroup;
        private List<Observer<M>> _RemoveObserverGroup;
        
        
        #endregion

        #region <Constructors>

        public ObservableImp() 
        {
            _ObserverGroup = new List<Observer<M>>();
            _RemoveObserverGroup = new List<Observer<M>>();
        }

        #endregion
        
        #region <Methods>
        
        /// <summary>
        /// 특정 옵저버를 등록한다.
        /// </summary>
        public IDisposable Subscribe(IObserver<M> p_Observer)
        {
            var observer = p_Observer as Observer<M>;
            if (!_ObserverGroup.Contains(observer))
            {
                _ObserverGroup.Add(observer);
                observer.SetObserverGroup(_ObserverGroup, _RemoveObserverGroup);
            }
            return observer;
        }

        /// <summary>
        /// 등록된 Observer들을 통해 특정 데이터에 대한 기능을 호출하는 메서드
        /// </summary>
        public void DoAct(M p_ObserveTarget)
        {
            ApplyRemoveObserverGroup();
            foreach (var observer in _ObserverGroup)
            {
                observer.OnNext(p_ObserveTarget);
            }
        }

        /// <summary>
        /// 등록된 Observer들을 클리어하는 메서드
        /// </summary>
        public void Clear()
        {
            foreach (var observer in _ObserverGroup)
            {
                observer.Dispose();
            }
            ApplyRemoveObserverGroup();
        }

        /// <summary>
        /// 삭제 예정인 Observer들을 일괄 삭제하는 메서드
        /// </summary>
        private void ApplyRemoveObserverGroup()
        {
            foreach (var observer in _RemoveObserverGroup)
            {
                _ObserverGroup.Remove(observer);
            }
            _RemoveObserverGroup.Clear();
        }

        #endregion
    }

    /// <summary>
    /// 특정 자료형 T를 관측하고 그 상태에 따라 간접적으로 데이터를 사용하게 하는 프록시같은 객체
    /// </summary>
    public class Observer<M> : IObserver<M>, _IDisposable
    {
        ~Observer()
        {
            Dispose();
        }
        
        #region <Fields>

        private List<Observer<M>> _ObserverGroup;
        private List<Observer<M>> _RemoveObserverGroup;
        
        #endregion

        #region <Methods>

        public void SetObserverGroup(List<Observer<M>> p_ObserverGroup, List<Observer<M>> p_RemoveObserverGroup)
        {
            _ObserverGroup = p_ObserverGroup;
            _RemoveObserverGroup = p_RemoveObserverGroup;
        }

        #endregion

        #region <IObserver>

        /// <summary>
        /// 추적 완료 시 콜백, Dispose로부터 호출된다.
        /// </summary>
        public virtual void OnCompleted()
        {
        }

        /// <summary>
        /// 추적 중 에러가 발생한 경우 콜백, 호출시마다 인스턴스를 할당하므로 구현하지 않음.
        /// </summary>
        public void OnError(Exception error)
        {
        }

        /// <summary>
        /// 추적 요청 시, 관측하던 객체의 특정 기능을 제어하는 콜백
        /// </summary>
        public virtual void OnNext(M value)
        {
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
            OnCompleted();
            _RemoveObserverGroup?.Add(this);
        }

        #endregion
    }
}
