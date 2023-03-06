using UnityEngine;

namespace k514
{
    /// <summary>
    /// 정해진 이벤트를 수행하고 회수되는 오브젝트
    /// </summary>
    public abstract class AutoMuttonBase : PrefabInstance
    {
        #region <Fields>

        /// <summary>
        /// 오토머튼 레코드
        /// </summary>
        protected AutoMuttonExtraDataRecordBridge _Record;
        
        /// <summary>
        /// 수명을 측정하는데 사용할 시간 이벤트 객체
        /// </summary>
        protected SafeReference<object, GameEventTimerHandlerWrapper> _TimerEventHandler;
         
        /// <summary>
        /// 최초 스폰되었을 때의 부모 아핀 객체
        /// </summary>
        private Transform _OriginParent;
        
        /// <summary>
        /// 파기가 예약됬는지 표시하는 플래그
        /// </summary>
        private bool _DeadReserveFalg;

        /// <summary>
        /// 오토머튼 수명 타이머
        /// </summary>
        protected ProgressTimer _WholeProgressTimer;
        
        #endregion

        #region <Callbacks>
 
        public override void OnSpawning()
        {
            base.OnSpawning();

            _Record = (AutoMuttonExtraDataRecordBridge) _PrefabKey._PrefabExtraPreset._PrefabExtraDataRecord;
            DeployableType = ObjectDeployTool.DeployableType.AutoMutton;
            _Transform.SetParent(AutoMuttonSpawnManager.GetInstance._AutoMuttonObjectWrapper);
            _OriginParent = _Transform.parent;

            var tryTimeLinePreset = _Record.ObjectDeployTimelinePreset;
            var lifeSpan = tryTimeLinePreset.Interval * tryTimeLinePreset.Count 
                           + ObjectVectorMapData.GetInstanceUnSafe[_Record.VectorMapIndex].EventDuration 
                           + 0.5f;
            _WholeProgressTimer.Initialize(lifeSpan);
        }

        public override void OnPooling()
        {
            base.OnPooling();
            
            _WholeProgressTimer.Reset();
        }
 
        public override void OnRetrieved()
        {
            base.OnRetrieved();

            EventTimerTool.ReleaseEventHandler(ref _TimerEventHandler);
            _Transform.SetParent(_OriginParent, false);
        
            _DeadReserveFalg = false;
        }

        protected bool OnUpdateTimer(float p_DeltaTime)
        {
            _WholeProgressTimer.Progress(p_DeltaTime);
            
            if (_WholeProgressTimer.IsOver())
            {
                SetRemove(false, 0);
                return false;
            }
            else
            {
                return true;
            }
        }
        
        protected abstract bool OnUpdateAutoMutton(float p_DeltaTime);

        #endregion
 
        #region <Methods>

        public bool IsReservedDeadOrDead()
        {
            return _DeadReserveFalg || !this.IsValid();
        }
        
        public void SetRemove(bool p_InstantRemove, uint p_Predelay)
        {
            if (!IsReservedDeadOrDead())
            {
                GameEventTimerHandlerManager.GetInstance.SpawnSafeEventTimerHandler(ref _TimerEventHandler, this, SystemBoot.TimerType.GameTimer, false);
                
                if (p_InstantRemove || p_Predelay < 50)
                {
                    RetrieveObject();
                }
                else
                {
                    _DeadReserveFalg = true;

                    var (_, eventHandler) = _TimerEventHandler.GetValue();
                    eventHandler
                        .AddEvent(
                            p_Predelay,
                            handler =>
                            {
                                handler.Arg1.RetrieveObject();
                                return true;
                            }, 
                            null, this
                        );
                    eventHandler.StartEvent();
                }
            }
        }

        public virtual void Run(uint p_PreDelay)
        {
            GameEventTimerHandlerManager.GetInstance.SpawnSafeEventTimerHandler(ref _TimerEventHandler, this, SystemBoot.TimerType.GameTimer, false);
            var (_, eventHandler) = _TimerEventHandler.GetValue();
            eventHandler.AddEvent(
                (p_PreDelay, 0, EventTimerTool.EventTimerIntervalType.UpdateEveryFrame),
                handler => handler.Arg1.OnUpdateAutoMutton(handler.LatestDeltaTime), null, this);
            eventHandler.StartEvent();
        }

        public void SetAttach(Transform p_Transform)
        {
            _Transform.SetParent(p_Transform, false);   
        }

        public void SetAttach(Unit p_Pivot)
        {
            SetAttach(p_Pivot._Transform);   
        }

        #endregion
    }
}