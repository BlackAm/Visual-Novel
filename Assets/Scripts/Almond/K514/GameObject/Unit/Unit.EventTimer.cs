namespace k514
{
    public partial class Unit
    {
        #region <Consts>

        private const int UnitTimerEventHandlerPreloadCount = 4;

        #endregion
        
        #region <Fields>

        private ObjectPooler<UnitTimerEventType, UnitEventTypeObject> _UnitTimerEventPooler;

        #endregion

        #region <Enums>
        
        /// <summary>
        /// 유닛 타이머 이벤트를 통해 수행할 작업 타입
        /// </summary>
        public enum UnitTimerEventType
        {
            None,
            
            /// <summary>
            /// 중독
            /// </summary>
            Poison,
            
            /// <summary>
            /// 출혈
            /// </summary>
            Bleed,
            
            /// <summary>
            /// 기절
            /// </summary>
            Stun,
            
            /// <summary>
            /// 속박
            /// </summary>
            Immobilize,
            
            /// <summary>
            /// 침묵
            /// </summary>
            Silence,
            
            /// <summary>
            /// 빙결
            /// </summary>
            Freeze,
            
            /// <summary>
            /// 저지불가
            /// </summary>
            SuperArmor,
            
            /// <summary>
            /// 무적
            /// </summary>
            Invincible,
            
            /// <summary>
            /// 능력치 일시 증가
            /// </summary>
            TempAddValue,
            
            /// <summary>
            /// 능력치 러프 증가
            /// </summary>
            SmoothAddValue,
        }

        #endregion
        
        #region <Callbacks>

        private void OnAwakeUnitTimerEvent()
        {
            _UnitTimerEventPooler = new ObjectPooler<UnitTimerEventType, UnitEventTypeObject>();
            _UnitTimerEventPooler.Preload(UnitTimerEventHandlerPreloadCount);
        }

        private void OnPoolingUnitEvent()
        {
            _UnitEventHandler.OnEventTriggered(UnitEventHandlerTool.UnitEventType.PositionChanged, new UnitEventMessage(this, false));
        }

        private void OnRetrieveUnitTimerEvent()
        {
            ClearTimerEvent();
        }

        #endregion

        #region <Methods>

        public int GetTimerEventCount(UnitTimerEventType p_Type)
        {
            return _UnitTimerEventPooler[p_Type].ActivedObjectPool.Count;
        }

        public bool HasTimerEvent(UnitTimerEventType p_Type)
        {
            return GetTimerEventCount(p_Type) > 0;
        }
        
        public void AddUnitTimerEvent(UnitTimerEventType p_EventType, Unit p_Striker, uint p_PreDelay, uint p_Interval, int p_IntervalCount, int p_HitPresetIndex, bool SendState = false)
        {
            if (HasTimerEvent(p_EventType))
            {
                ClearTimerEvent();
            }
            var spawnedUnitTimerEvent = _UnitTimerEventPooler.GetObject(p_EventType);
            spawnedUnitTimerEvent.SetDelay(p_PreDelay, p_Interval);
            spawnedUnitTimerEvent.SetIntervalCount(p_IntervalCount);
            spawnedUnitTimerEvent.SetTargetUnit(this, p_Striker);
            spawnedUnitTimerEvent.SetTrigger();
        }

        public void ClearTimerEvent()
        {
            _UnitTimerEventPooler.ClearPool();
        }

        #endregion
    }
}