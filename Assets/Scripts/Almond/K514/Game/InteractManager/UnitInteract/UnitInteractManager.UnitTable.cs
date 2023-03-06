using System;
using System.Collections.Generic;
using UnityEngine;

namespace k514
{
    public partial class UnitInteractManager
    {
        #region <Consts>

        /// <summary>
        /// 시스템 유닛 숫자 종자값
        /// </summary>
        private const int __MAX_INTERACT_UNIT_NUMBER_SEED = 13;
        
        /// <summary>
        /// 시스템에서 최대 포착 가능한 유닛 숫자, 2^__MAX_INTERACT_UNIT_NUMBER_SEED 마리
        /// </summary>
        public const int __MAX_INTERACT_UNIT_NUMBER = 1 << __MAX_INTERACT_UNIT_NUMBER_SEED;

        /// <summary>
        /// 범위 상한 거리, 해당 범위를 초과하는 값은 무한대로 취급한다.
        /// </summary>
        public const float __Out_Of_Range = 1_000f;
        
        /// <summary>
        /// 범위 상한 거리의 절반
        /// </summary>
        public const float __Out_Of_Range_Half = 500f;
        
        /// <summary>
        /// 범위 상한 거리 제곱값
        /// </summary>
        public const float __Out_Of_Range_Sqr = __Out_Of_Range * __Out_Of_Range;
        
        /// <summary>
        /// 모듈 활성화 문턱값
        /// </summary>
        private const float _Module_Awake_ThresholdSqr = 250f;
        
        /// <summary>
        /// 모듈 활성화 문턱값 음수
        /// </summary>
        private const float _Module_Awake_ThresholdSqr_Negative = -_Module_Awake_ThresholdSqr;

        #endregion
    
        #region <Fields>

#if !SERVER_DRIVE
        /// <summary>
        /// 카메라 뷰 컨트롤에 관한 이벤트를 전체 유닛에게 전파하는 이벤트 리시버
        /// </summary>
        private CameraEventReceiver _CameraViewControlEventReceiver;
#endif

        /// <summary>
        /// 유닛 이동에 관한 이벤트를 전체 유닛에게 전파하는 이벤트 리시버
        /// </summary>
        private UnitEventReceiver _UnitDistanceEventReceiver;

        /// <summary>
        /// 이벤트 핸들러 interactId로 구분되는, 각 이벤트 핸들러 유닛 간의 제곱거리 테이블
        /// </summary>
        private TriangleArray<float> _DistanceSqrTable;

        /// <summary>
        /// 이벤트 핸들러 interactId로 구분되는, 각 이벤트 핸들러 유닛 간의 전투 테이블
        /// </summary>
        private TriangleArray<UnitInteractStatePreset> _InteractStateTable;
        
        /// <summary>
        /// 플레이어를 중심으로 컴퓨터 유닛이 동작을 개시하는 거리
        /// </summary>
        public float _Module_Awake_Range { get; private set; }
        
        /// <summary>
        /// 플레이어를 중심으로 컴퓨터 유닛이 동작을 개시하는 거리 제곱
        /// </summary>
        public float _Module_Awake_SqrRange { get; private set; }

        #endregion

        #region <Callbacks>

        private void OnCreated_UnitInteract()
        {
#if SERVER_DRIVE
            _Module_Awake_Range = HeadlessServerManager.DEFAULT_MODULE_AWAKE_DISTANCE;
            _Module_Awake_SqrRange = Mathf.Pow(_Module_Awake_Range, 2);
#else
            _CameraViewControlEventReceiver = new CameraEventReceiver(CameraManager.CameraEventType.WholeAffine | CameraManager.CameraEventType.TraceTargetChanged | CameraManager.CameraEventType.FarCullingDistanceChanged, BroadCastCameraViewControlEvent);
            CameraManager.GetInstanceUnSafe.AddReceiver(_CameraViewControlEventReceiver);
#endif
            
            _UnitDistanceEventReceiver = new UnitEventReceiver(UnitEventHandlerTool.UnitEventType.PositionChanged, UpdateSqrDistanceWithUnitAuthority);
            
            _DistanceSqrTable = new TriangleArray<float>(__MAX_INTERACT_UNIT_NUMBER);
            _InteractStateTable = new TriangleArray<UnitInteractStatePreset>(__MAX_INTERACT_UNIT_NUMBER);
        }

        #endregion

        #region <Method/EventBind>

#if !SERVER_DRIVE
        /// <summary>
        /// Position Sender 이벤트가 발생한 경우 호출되는 메서드
        /// </summary>
        private void BroadCastCameraViewControlEvent(CameraManager.CameraEventType p_Type, CameraEventMessage p_CameraEventMessage)
        {
            switch (p_Type)
            {
                // 카메라 거리 갱신 이벤트는 취합하여, 각 유닛의 LateUpdate에서 일괄 처리하여 연산 수를 줄인다.
                case CameraManager.CameraEventType.CameraPositionChanged:
                case CameraManager.CameraEventType.Zoom:
                case CameraManager.CameraEventType.Rotate:
                case CameraManager.CameraEventType.WholeAffine:
                case CameraManager.CameraEventType.TraceTargetChanged:
                    var targetList = UnitEventHandlerPool.ActivedObjectPool;
                    foreach (var unitEventHandler in targetList)
                    {
                        var compareUnit = unitEventHandler._ThisUnit;
                        if (!compareUnit.HasState_Or(Unit.UnitStateType.SystemDisable))
                        {
                            // compareUnit.OnReserveUpdateCameraRender();
                        }
                    }
                    break;
                case CameraManager.CameraEventType.FarCullingDistanceChanged:
                    _Module_Awake_Range = CameraManager.GetInstanceUnSafe.UnitFarCullingDistance;
                    _Module_Awake_SqrRange = Mathf.Pow(_Module_Awake_Range, 2);
                    break;
            }
        }
#endif
        
        #endregion
        
        #region <Method/Distance>
        
        /// <summary>
        /// 수동으로 지정한 유닛이 관여하는 거리를 갱신하는 메서드
        /// </summary>
        public void UpdateSqrDistanceWithManual(UnitEventHandler p_UnitHandler, bool p_IgnoreSenderRequest)
        {
            if (p_IgnoreSenderRequest ||
                p_UnitHandler._ThisUnit.IsPositionEventSender())
            {
                UpdateAllSqrDistance(p_UnitHandler._ThisUnit);
            }
        }
        
        /// <summary>
        /// Position Sender 이벤트가 발생한 경우 호출되는 메서드
        /// </summary>
        private void UpdateSqrDistanceWithUnitAuthority(UnitEventHandlerTool.UnitEventType p_Type, UnitEventMessage p_UnitEventMessage)
        {
            if (p_UnitEventMessage.BoolValue)
            {
                var tryUnit = p_UnitEventMessage.TriggerUnit;
                var isEventSender = tryUnit.IsPositionEventSender();
                if (isEventSender)
                {
                    UpdateAllSqrDistance(tryUnit);
                }
            }
        }

        /// <summary>
        /// 해당 매니저에 등록된 모든 유닛 간의 제곱거리를 갱신하는 메서드
        /// </summary>
        private void UpdateAllSqrDistance(Unit p_PivotUnit)
        {
            var isEventSender = p_PivotUnit.IsPositionEventSender();
            var targetActiveUnitArray = UnitEventHandlerPool.ActivedObjectPool;
            foreach (var unitEventHandler in targetActiveUnitArray)
            {
                // 현재 UnitInteractManager에 등록되어 있는 모든 유닛과 이벤트를 발생시킨 유닛 사이의 거리를 갱신한다.
                // 즉 PositionSend 이벤트를 가지는 유닛이 움직이는 경우, 해당 유닛을 중심으로 거리가 갱신된다.
                var targetUnit = unitEventHandler._ThisUnit;
                UpdateSqrDistance(p_PivotUnit, targetUnit, isEventSender);
            }
        }

        /// <summary>
        /// 지정한 두 유닛간의 거리 제곱값을 거리 테이블에 갱신하는 메서드
        /// </summary>
        public void UpdateSqrDistance(Unit p_FromUnit, Unit p_TargetUnit, bool p_IsFromUnitEventSender)
        {
            // 해당 섹션에 진입했다는 것은, 아직 Retrieve가 진행되지 않은 유닛 이벤트 핸들러라는 것.
            // 즉, EventKey는 아직 유효하다.
            var fromIndex = p_FromUnit.EventKey;
            var toIndex = p_TargetUnit.EventKey;
            
            // 두 유닛이 같은 유닛이라면, 자기자신과의 거리 즉 0
            // 기본값으로 이미 0이 들어가 있으므로 아무것도 하지 않는다.
            if (fromIndex == toIndex)
            {
            }
            else
            {
                // 거리 테이블은 삼각 행렬이므로 두 인덱스를 비교해야한다.
                var sqrDistance = p_FromUnit.GetSqrDistanceTo(p_TargetUnit._Transform);
                _DistanceSqrTable.SetElement_AscendantSafe(fromIndex, toIndex, sqrDistance);

                if (p_IsFromUnitEventSender)
                {
                    if (p_TargetUnit.IsInteractValid(Unit.UnitStateType.UnitAIAwakableFilterMask) && !p_TargetUnit._MindObject.HasAIExtraFlag(ThinkableTool.AIExtraFlag.NeverSleep))
                    {
                        var offset = sqrDistance - _Module_Awake_SqrRange;
                        if (offset > _Module_Awake_ThresholdSqr)
                        {
                            p_TargetUnit.UpdateModuleAwake(p_FromUnit, true);
                        }
                        else if (offset < _Module_Awake_ThresholdSqr_Negative)
                        {
                            p_TargetUnit.UpdateModuleAwake(p_FromUnit, false);
                        }
                    }
                }
            }
        }

        public void UpdateSqrDistance(Unit p_FromUnit, Unit p_TargetUnit)
        {
            UpdateSqrDistance(p_FromUnit, p_TargetUnit, p_FromUnit.IsPositionEventSender());
        }
        
        public void UpdateSqrDistanceWhenTargetMoved(Unit p_FromUnit, Unit p_TargetUnit)
        {
            if (p_TargetUnit.IsPositionChanged())
            {
                UpdateSqrDistance(p_FromUnit, p_TargetUnit, p_FromUnit.IsPositionEventSender());
            }
        }
        
        public void UpdateSqrDistanceWhenTargetHold(Unit p_FromUnit, Unit p_TargetUnit)
        {
            if (!p_TargetUnit.IsPositionChanged())
            {
                UpdateSqrDistance(p_FromUnit, p_TargetUnit, p_FromUnit.IsPositionEventSender());
            }
        }
        
        /// <summary>
        /// 거리 테이블로부터 지정한 두 유닛 사이의 제곱거리를 리턴하는 메서드
        /// </summary>
        public float GetSqrDistanceBetween(Unit p_FromUnit, Unit p_TargetUnit)
        {
            var fromIndex = p_FromUnit.EventKey;
            var toIndex = p_TargetUnit.EventKey;
            
            // 두 유닛이 같은 유닛이라면, 자기자신과의 거리 즉 0
            if (fromIndex == toIndex)
            {
                return 0f;
            }
            else
            {
                return _DistanceSqrTable.GetElement_AscendantSafe(fromIndex, toIndex);
            }
        }
        
        /// <summary>
        /// 거리 테이블로부터 지정한 두 유닛 사이의 제곱거리를 리턴하는 메서드
        /// TargetUnit이 정지 상태인 경우 거리테이블을 갱신하고 거리 값을 리턴한다.
        /// </summary>
        public float GetSqrDistanceBetween_UpdateSqrDistanceWhenTargetUnitHold(Unit p_FromUnit, Unit p_TargetUnit)
        {
            var fromIndex = p_FromUnit.EventKey;
            var toIndex = p_TargetUnit.EventKey;
            
            // 두 유닛이 같은 유닛이라면, 자기자신과의 거리 즉 0
            if (fromIndex == toIndex)
            {
                return 0f;
            }
            else
            {
                UpdateSqrDistanceWhenTargetHold(p_FromUnit, p_TargetUnit);
                return _DistanceSqrTable.GetElement_AscendantSafe(fromIndex, toIndex);
            }
        }

        #endregion
        
        #region <Method/FindUnit>

        /// <summary>
        /// 근방의 지정한 범위내에 플레이어 타입의 유닛이 있는지 검색하는 논리메서드
        /// </summary>
        public bool HasPlayerInRange(Unit p_FromUnit, float p_Distance, bool p_CalculateBoundFlag)
        {
            UpdateSqrDistanceWithManual(p_FromUnit._UnitEventHandler, true);
            return FilterUnit_And
                (
                    p_FromUnit, 
                    new FilterParams(p_Distance, p_CalculateBoundFlag), 
                    UnitFilterTool.UnitFilterFlagType.Player | UnitFilterTool.UnitFilterFlagType.SqrDistanceTable, 
                    p_FromUnit._UnitFilterResultSet
                )
                .HasAnyFlagExceptNone(UnitFilterTool.FilterResultType.Unit);
        }
        
        /// <summary>
        /// 모듈 활성화 범위내에 인공지능 활성화 유닛이 있는지 검색하는 논리메서드
        /// </summary>
        public bool HasDistanceEventSenderInRange(Unit p_FromUnit, bool p_CalculateBoundFlag)
        {
            return HasDistanceEventSenderInRange(p_FromUnit, _Module_Awake_Range, p_CalculateBoundFlag);
        }
        
        /// <summary>
        /// 근방의 지정한 범위내에 인공지능 활성화 유닛이 있는지 검색하는 논리메서드
        /// </summary>
        public bool HasDistanceEventSenderInRange(Unit p_FromUnit, float p_Distance, bool p_CalculateBoundFlag)
        {
            if (p_FromUnit.EventHandlerValid)
            {
                UpdateSqrDistanceWithManual(p_FromUnit._UnitEventHandler, true);
                return FilterUnit_And
                    (
                        p_FromUnit, 
                        new FilterParams(p_Distance, p_CalculateBoundFlag), 
                        UnitFilterTool.UnitFilterFlagType.DistanceEventSender | UnitFilterTool.UnitFilterFlagType.SqrDistanceTable,
                        p_FromUnit._UnitFilterResultSet
                    )
                    .HasAnyFlagExceptNone(UnitFilterTool.FilterResultType.Unit);
            }
            else
            {
                return false;
            }
        }
        
        /// <summary>
        /// 지정한 유닛과 가장 가까이에 있는 유닛을 리턴하는 메서드
        /// </summary>
        public Unit GetNearestUnitFrom(Unit p_FromUnit, Unit.UnitStateType p_UnitStateFilterMask, float p_SearchDistance, bool p_CalculateBoundFlag)
        {
            UpdateSqrDistanceWithManual(p_FromUnit._UnitEventHandler, true);
            
            var targetList = UnitEventHandlerPool.ActivedObjectPool;
            Unit result = null;

            if (p_CalculateBoundFlag)
            {
                var trySqrDistance = float.MaxValue;
                
                foreach (var unitEventHandler in targetList)
                {
                    var compareUnit = unitEventHandler._ThisUnit;
                    if (!ReferenceEquals(p_FromUnit, compareUnit) && compareUnit.IsInteractValid(p_UnitStateFilterMask))
                    {
                        trySqrDistance = Mathf.Min(trySqrDistance, Mathf.Pow(p_SearchDistance + p_FromUnit.GetRadius() + compareUnit.GetRadius(), 2f));
                        var compareSqrDistance = GetSqrDistanceBetween(p_FromUnit, compareUnit);
                        if (compareSqrDistance < trySqrDistance)
                        {
                            trySqrDistance = compareSqrDistance;
                            result = compareUnit;
                        }
                    }
                }
            }
            else
            {
                var trySqrDistance = Mathf.Pow(p_SearchDistance, 2f);
                
                foreach (var unitEventHandler in targetList)
                {
                    var compareUnit = unitEventHandler._ThisUnit;
                    if (!ReferenceEquals(p_FromUnit, compareUnit) && compareUnit.IsInteractValid(p_UnitStateFilterMask))
                    {
                        var compareSqrDistance = GetSqrDistanceBetween(p_FromUnit, compareUnit);
                        if (compareSqrDistance < trySqrDistance)
                        {
                            trySqrDistance = compareSqrDistance;
                            result = compareUnit;
                        }
                    }
                }
            }

            return result;
        }
        
        /// <summary>
        /// 지정한 유닛과 가장 가까이에 있는 적 유닛을 리턴하는 메서드
        /// </summary>
        /*public Unit GetNearestEnemyUnitFrom(Unit p_FromUnit, Unit.UnitStateType p_UnitStateFilterMask, float p_SearchDistance, bool p_CalculateBoundFlag)
        {
            UpdateSqrDistanceWithManual(p_FromUnit._UnitEventHandler, true);
            
            var targetList = UnitEventHandlerPool.ActivedObjectPool;
            Unit result = null;
            
            if (p_CalculateBoundFlag)
            {
                var trySqrDistance = float.MaxValue;
                foreach (var unitEventHandler in targetList)
                {
                    var compareUnit = unitEventHandler._ThisUnit;
                    if (!ReferenceEquals(p_FromUnit, compareUnit) && compareUnit.IsInteractValid(p_UnitStateFilterMask)
                                                                  && p_FromUnit.GetGroupRelate(compareUnit) == UnitTool.UnitGroupRelateType.Enemy)
                    {
                        trySqrDistance = Mathf.Min(trySqrDistance, Mathf.Pow(p_SearchDistance + p_FromUnit.GetRadius() + compareUnit.GetRadius(), 2f));
                        var compareSqrDistance = GetSqrDistanceBetween(p_FromUnit, compareUnit);
                        if (compareSqrDistance < trySqrDistance)
                        {
                            trySqrDistance = compareSqrDistance;
                            result = compareUnit;
                        }
                    }
                }
            }
            else
            {
                var trySqrDistance = Mathf.Pow(p_SearchDistance, 2f);
                
                foreach (var unitEventHandler in targetList)
                {
                    var compareUnit = unitEventHandler._ThisUnit;
                    if (!ReferenceEquals(p_FromUnit, compareUnit) && compareUnit.IsInteractValid(p_UnitStateFilterMask)
                                                                  && p_FromUnit.GetGroupRelate(compareUnit) == UnitTool.UnitGroupRelateType.Enemy)
                    {
                        var compareSqrDistance = GetSqrDistanceBetween(p_FromUnit, compareUnit);
                        if (compareSqrDistance < trySqrDistance)
                        {
                            trySqrDistance = compareSqrDistance;
                            result = compareUnit;
                        }
                    }
                }
            }
            
            return result;
        }

        public (bool, Unit) GetEnemyUnit(Unit p_FromUnit, List<int> p_QuestMonsterKey, Unit.UnitStateType p_UnitStateFilterMask, float p_SearchDistance, bool p_CalculateBoundFlag)
        {
            UpdateSqrDistanceWithManual(p_FromUnit._UnitEventHandler, true);
            
            var targetList = UnitEventHandlerPool.ActivedObjectPool;
            Unit result = null;
            
            if (p_CalculateBoundFlag)
            {
                var trySqrDistance = float.MaxValue;
                foreach (var unitEventHandler in targetList)
                {
                    var compareUnit = unitEventHandler._ThisUnit;
                    if (!ReferenceEquals(p_FromUnit, compareUnit) && compareUnit.IsInteractValid(p_UnitStateFilterMask)
                                                                  && p_FromUnit.GetGroupRelate(compareUnit) == UnitTool.UnitGroupRelateType.Enemy)
                    {
                        trySqrDistance = Mathf.Min(trySqrDistance, Mathf.Pow(p_SearchDistance + p_FromUnit.GetRadius() + compareUnit.GetRadius(), 2f));
                        var compareSqrDistance = GetSqrDistanceBetween(p_FromUnit, compareUnit);
                        if (compareSqrDistance < trySqrDistance)
                        {
                            for (var i = 0; i < p_QuestMonsterKey.Count; i++)
                            {
                                if (compareUnit._PrefabModelKey.Item2 == p_QuestMonsterKey[i])
                                {
                                    trySqrDistance = compareSqrDistance;
                                    result = compareUnit;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                var trySqrDistance = Mathf.Pow(p_SearchDistance, 2f);
                
                foreach (var unitEventHandler in targetList)
                {
                    var compareUnit = unitEventHandler._ThisUnit;
                    if (!ReferenceEquals(p_FromUnit, compareUnit) && compareUnit.IsInteractValid(p_UnitStateFilterMask)
                                                                  && p_FromUnit.GetGroupRelate(compareUnit) == UnitTool.UnitGroupRelateType.Enemy)
                    {
                        var compareSqrDistance = GetSqrDistanceBetween(p_FromUnit, compareUnit);
                        if (compareSqrDistance < trySqrDistance)
                        {
                            trySqrDistance = compareSqrDistance;
                            result = compareUnit;
                        }
                    }
                }
            }
            
            return (!ReferenceEquals(null, result), result);
        }*/

        #endregion

        #region <Method/Combat>

        /// <summary>
        /// 지정한 두 유닛간의 전투 정보를 갱신하는 메서드
        /// </summary>
        public void UpdateCombatInfoBetween(Unit p_Pivot, UnitCombatStatePreset p_PivotAttackInfo)
        {
            // 타격 유닛은 null일 수 있으므로 체크해준다.
            var strikerUnit = p_PivotAttackInfo.Striker;
            if (strikerUnit.IsValid())
            {
                var fromIndex = strikerUnit.EventKey;
                var toIndex = p_Pivot.EventKey;
                
                // 두 유닛이 같은 유닛이라면 아무것도 하지 않는다.
                if (fromIndex == toIndex)
                {
                }
                else
                {
                    var tryInteractState = _InteractStateTable.GetElement_AscendantSafe(fromIndex, toIndex);
                    tryInteractState.UpdateCombatState(p_PivotAttackInfo);
                }
            }
        }
        
        /// <summary>
        /// 지정한 두 유닛간의 전투 정보를 리턴하는 메서드
        /// </summary>
        public UnitCombatStatePreset GetCombatInfoBetween(Unit p_FromUnit, Unit p_TargetUnit)
        {
            var fromIndex = p_FromUnit.EventKey;
            var toIndex = p_TargetUnit.EventKey;
            
            // 두 유닛이 같은 유닛이라면, 자기자신과의 전투 정보이므로 의미가 없다.
            if (fromIndex == toIndex)
            {
                return default;
            }
            else
            {
                var tryInteractState = _InteractStateTable.GetElement_AscendantSafe(fromIndex, toIndex);
                var combatInfo = tryInteractState._CombatInfo;
                return combatInfo.IsTimeStampValid(Time.time) ? combatInfo : default;
            }
        }
        
        #endregion
    }
}