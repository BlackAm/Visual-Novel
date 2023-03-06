using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace k514
{
    public partial class Unit
    {
        #region <Fields>

        /// <summary>
        /// 다른 오브젝트와 상호작용하기 위한 이벤트 처리 객체
        /// </summary>
        public UnitEventHandler _UnitEventHandler;

        /// <summary>
        /// 해당 유닛의 Environment Manager 관리 id
        /// </summary>
        public int EventKey => EventHandlerValid ? _UnitEventHandler._InteractId : -1;

        /// <summary>
        /// 해당 유닛이 다른 유닛과 상호작용하기 위해 사용하는 필터 결과 리스트
        /// </summary>
        [NonSerialized] public List<Unit> _UnitFilterResultSet;

        /// <summary>
        /// 유닛 이벤트 핸들러가 유효한지 검증하는 플래그
        /// </summary>
        public bool EventHandlerValid;

        public GameEventTimerHandlerWrapper idleEventHandler;

        public bool IsCombatIdleChecking = false;

#if !SERVER_DRIVE
        public UI_NamePanel PlayerNamePanel;
#endif

        /// <summary>
        /// 유닛을 활성화 시키고 있는 객체에 대한 정보
        /// </summary>
        private int _UnitViewer;
        
        #endregion

        #region <Callbacks>

        private void OnAwakeEventHandler()
        {
            _UnitFilterResultSet = new List<Unit>();
        }

        private void OnPoolingEventHandler()
        {
            _UnitEventHandler = UnitInteractManager.GetInstance.AddUnit(this);
            EventHandlerValid = true;

#if !SERVER_DRIVE
            // PlayerNamePanel = DefaultUIManagerSet.GetInstanceUnSafe._UITheaterName.PoolNameLabel(this, _Default_UnitInfo.GetGroupMarkSpriteIndex());
#endif
            OnCompareAuthorityFlagChange(UnitTool.UnitAuthorityFlag.None, UnitAuthorityFlagMask);
            OnAuthorityFlagChange();
        }

        private void OnRetrieveEventHandler()
        {
            if (!ReferenceEquals(null, _UnitEventHandler))
            {
                UnitInteractManager.GetInstance.RemoveUnit(_UnitEventHandler);
                _UnitEventHandler.RetrieveObject();
                _UnitEventHandler = null;
                EventHandlerValid = false;
            }
        }

        /// <summary>
        /// 유닛의 위치가 변경되었다고 판단되는 경우에 호출되는 콜백
        /// p_EnoughThresholdMoveFlag 값은 PositionTracker에서 정의하고 있는 하한거리보다 더 크게 위치가 변경되었는지 표시하는 플래그
        ///
        /// 기본적으로 모든 모듈의 활동이 종료된 Update 이후, LateUpdate 타이밍에 호출된다.
        /// </summary>
        protected override void OnSendUnitPositionUpdate(bool p_EnoughThresholdMoveFlag)
        {
            _UnitEventHandler.OnEventTriggered(UnitEventHandlerTool.UnitEventType.PositionChanged, new UnitEventMessage(this, p_EnoughThresholdMoveFlag));
        }

        /// <summary>
        /// 파라미터로 받은 p_FromUnit와 해당 유닛간의 거리가 변경되는 경우 해당 유닛의 인공지능 모듈의
        /// 상태 활성화 체크 메서드를 호출하는 콜백
        /// </summary>
        public void UpdateModuleAwake(Unit p_SenderUnit, bool p_SleepFlag)
        {
            ulong senderUnitKey = 0;
#if SERVER_DRIVE
            // 임시작업
            // 맵서버에서 이종의 거리 이벤트 송신자가 있는 경우, 인공지능 Awake 로직이 진동하는 현상
            if(p_SleepFlag)
            {
                //현재 유닛이 포함되어 있는지
                bool hasUnitViwer = (_UnitViewer & (1 << p_SenderUnit.EventKey)) != 0;
                if (hasUnitViwer)
                {
                    //포함되어 있다면 삭제해준다
                    _UnitViewer &= ~(1 << p_SenderUnit.EventKey);
                }
                //현재 유닛을 보고 있는 주유닛이 없을경우에 유닛을 Disable 해준다.
                p_SleepFlag = _UnitViewer == 0 ? p_SleepFlag : !p_SleepFlag;

            }
            else
            {
                //현재 유닛을 활성화 하는데 이미 주유닛을 포함하고 있지 않다면
                if ((_UnitViewer & (1 << p_SenderUnit.EventKey)) == 0)
                {
                    //주유닛을 포함
                    _UnitViewer |= (1 << p_SenderUnit.EventKey);

                    //활성화할경우 유닛 활성 메세지 보냄
                    if (p_SenderUnit._UnitNetworkPreset.UnitUniqueKey != 0)
                    {
                        Almond.Util.EventDispatcher.TriggerEvent(PopUpEvent.POP_VIEW, p_SenderUnit._UnitNetworkPreset.UnitUniqueKey, this);
                    }

                }
            }
#endif
            if (!IsQuestTargetUnit && !IsPlayer)
            {
                SetUnitDisable(p_SleepFlag);
            }
        }

        #endregion

        #region <Methods>

        //활성화 객체에 대한 정보 리셋
        public void ResetUnitView()
        {
            _UnitViewer = 0;
        }

        /// <summary>
        /// 호출되는 즉시, 이벤트 수신자 권한과 상관없이 해당 유닛을 기준으로 제곱 거리 테이블을 갱신시키고
        /// 해당 유닛의 위치가 변경됬음을 알리는 플래그를 세트하는 메서드
        /// </summary>
        public void TryUpdateSqrDistanceTable_And_ReportPositionChangeDetected()
        {
            if(EventHandlerValid)
            {
                // UnitInteractManager.GetInstance.UpdateSqrDistanceWithManual(_UnitEventHandler, true);
            }
            OnUnitPositionChangeDetected();
        }
        
        public void AddEventReceiver(UnitEventReceiver p_UnitEventReceiver)
        {
            _UnitEventHandler.AddReceiver(p_UnitEventReceiver);
        }

        public UnitEventReceiver GetEventReceiver(UnitEventHandlerTool.UnitEventType p_EventType, Action<UnitEventHandlerTool.UnitEventType, UnitEventMessage> p_Handler)
        {
            return _UnitEventHandler.UnitEventSender.GetEventReceiver<UnitEventReceiver>(p_EventType, p_Handler);
        }

        /// <summary>
        /// 현재 필터된 유닛 필터 리스트의 유닛들을 거리 오름차순으로 정렬하여 리턴하는 메서드
        /// </summary>
        /*public List<Unit> GetUnitFilterResultSetByDistance_OrderBy_AscendingOrder()
        {
            UnitInteractManager.GetInstance.UpdateSqrDistanceWithManual(_UnitEventHandler, true);
            Dictionary<float, Unit> NearMonster = new Dictionary<float, Unit>();

            
            for(int i = 0; i < _UnitFilterResultSet.Count; i++)
            {
                if(NearMonster.ContainsKey(UnitInteractManager.GetInstance.GetSqrDistanceBetween(this, _UnitFilterResultSet[i])))
                {
                    continue;
                }
                NearMonster.Add(UnitInteractManager.GetInstance.GetSqrDistanceBetween(this, _UnitFilterResultSet[i]), _UnitFilterResultSet[i]);
            }

            //var count = _UnitFilterResultSet.Count;
            //QuickSort(0, count - 1);
            var Monster = NearMonster.OrderBy(x => x.Key);
            var UnitList = new List<Unit>();
            foreach(var unit in Monster)
            {
                UnitList.Add(unit.Value);
            }
            return UnitList;// _UnitFilterResultSet;
        }*/

        /*private void QuickSort(int p_SortStartIndex, int p_SortEndIndex)
        {
            if (p_SortStartIndex < p_SortEndIndex)
            {
                int pivot = Partition(p_SortStartIndex, p_SortEndIndex);
                QuickSort(p_SortStartIndex, pivot - 1);
                QuickSort(pivot + 1, p_SortEndIndex);
            }
        }

        private int Partition(int p_SortStartIndex, int p_SortEndIndex)
        {
            int pivot = p_SortStartIndex;
            for (int i = p_SortStartIndex; i < p_SortEndIndex; i++)
            {
                if (UnitInteractManager.GetInstance.GetSqrDistanceBetween(this, _UnitFilterResultSet[i])
                    <= UnitInteractManager.GetInstance.GetSqrDistanceBetween(this, _UnitFilterResultSet[p_SortEndIndex]))
                {
                    SystemTool.Swap(ref pivot, ref i);
                    pivot++;
                }
            }
            SystemTool.Swap(ref pivot, ref p_SortEndIndex);
            return pivot;
        }*/

        public void SetHideNameUI(bool p_FixVisibleFlag, bool p_HideFlag)
        {
            if (EventHandlerValid)
            {
                _UnitEventHandler.OnEventTriggered(UnitEventHandlerTool.UnitEventType.SwitchHideUINameLabel, new UnitEventMessage(this, p_FixVisibleFlag, p_HideFlag));
            }
        }

        public void SetIdleState()
        {
            if (IsCombatIdleChecking)
            {
                idleEventHandler.CancelEvent();
            }

            IsCombatIdleChecking = true;

            idleEventHandler = GameEventTimerHandlerManager.GetInstance.SpawnEventTimerHandler(SystemBoot.TimerType.GameTimer, false);
            idleEventHandler
                    .AddEvent
                    (
                        20000,
                        handler =>
                        {
                            var thisRef = handler.Arg1;
                            thisRef._ActableObject.TurnIdleState(ActableTool.IdleState.Relax, AnimatorParamStorage.MotionTransitionType.Bypass_StateMachine);
                            thisRef.IsCombatIdleChecking = false;
                            return true;
                        }, 
                        null, this
                    );
            idleEventHandler.StartEvent();
        }

        #endregion
    }
}