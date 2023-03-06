using System.Collections.Generic;
using UnityEngine;

namespace k514
{
    public partial class ControlPhysicsBase
    {
        #region <Consts>

        private const uint _StackInterval = 25;

        #endregion
        
        #region <Fields>

        /// <summary>
        /// 감쇄 타이머 테이블
        /// </summary>
        private Dictionary<PhysicsTool.AccelerationType, StackTimer<PhysicsTool.AccelerationType, PhysicsTool.UnitAddForceParams>> _DampingTimer;
        
        /// <summary>
        /// 해당 물리 모듈이 힘을 가한 유닛 리스트
        /// </summary>
        private Dictionary<PhysicsTool.AccelerationType, List<Unit>> _ForcedUnit;
        
        /// <summary>
        /// 지연된 피격 이벤트 제어 테이블
        /// </summary>
        private List<SafeReference<object, GameEventTimerHandlerWrapper>> _ForceTimer;

        #endregion

        #region <Callbacks>

        /// <summary>
        /// 외력 감쇄 주기 콜백
        /// </summary>
        private bool OnDampingTick(StackTimer<PhysicsTool.AccelerationType, PhysicsTool.UnitAddForceParams> p_StackTimer, PhysicsTool.AccelerationType p_Type, PhysicsTool.UnitAddForceParams p_Params)
        {
            var record = p_Params.Record;
            var tryFlagMask = record.ForceProcessFlagMask;
            var result = true;
            var resultFilter = false;
            
            foreach (var unitAddForceProcessType in UnitTool.UnitAddForceProcessTypeEnumerator)
            {
                if (tryFlagMask.HasAnyFlagExceptNone(unitAddForceProcessType))
                {
                    switch (unitAddForceProcessType)
                    {
                        // 지정한 위치로부터 일정 거리이상 멀어지는 경우, 해당 외력을 없앤다.
                        case UnitTool.UnitAddForceProcessType.BoundDistance:
                            result &= p_Params.CheckBoundDistance(_MasterNode);
                            break;
                        // 해당 외력이 감쇄할 때마다, 주변의 적을 필터링한다.
                        case UnitTool.UnitAddForceProcessType.Filter:
                            var hitRecord = UnitHitPresetData.GetInstanceUnSafe[record.HitPresetIndex];
                            var filterParams = hitRecord.FilterParams;
                            
                            // 해당 감쇄가 2회차 이상인 경우, 필터링 간격 사이를 보간하여 필터링을 수행한다.
                            if (p_Params.IsReentered)
                            {
                                filterParams.SetInterpolatePivotAffine(p_Params.PrevAffine);
                            }

                            // 필터링을 수행한다.
                            resultFilter = _MasterNode.FindEnemyWithParams(filterParams);
                            break;
                    }
                }
            }

            // 필터링을 통해 유닛 충돌이 감지된 경우
            if (resultFilter)
            {
                var forceFilterEventMask = record.ForceFilterEventFlagMask;
                var hasDrawing = forceFilterEventMask.HasAnyFlagExceptNone(UnitTool.UnitAddForceFilterEventType.Drawing);
                var hasHit = forceFilterEventMask.HasAnyFlagExceptNone(UnitTool.UnitAddForceFilterEventType.HitUnit);
                
                var targetList = _MasterNode._UnitFilterResultSet;
                var force = hasDrawing ? _VelocityRecord[p_Type] : default;
                var forcedUnitList = hasDrawing ? _ForcedUnit[p_Type] : default;
                var hitPresetIndex = hasHit ? record.HitPresetIndex : default;
                
                foreach (var targetUnit in targetList)
                {
                    if (hasDrawing)
                    {
                        if (forcedUnitList.Contains(targetUnit))
                        {
                            targetUnit._PhysicsObject.OverlapVelocity(p_Type, force);
                        }
                        else
                        {
                            forcedUnitList.Add(targetUnit);
                            targetUnit._PhysicsObject.OverlapVelocity(p_Type, force);
                        }
                    }

                    if (hasHit)
                    {
                        var hitPreset = UnitHitPresetData.GetInstanceUnSafe[hitPresetIndex];
                        var particleCollideEventAffine = targetUnit._Transform;
                        targetUnit.HitUnit
                        (
                            _MasterNode, hitPreset.GetHitMessage(),
                            new UnitTool.UnitAddForcePreset(_Transform.position, particleCollideEventAffine.position, force), false
                        );
                    }
                }
            }

            // 차회 감쇄 이벤트에서 사용할 수 있도록, 외력 파라미터 구조체를 갱신시켜준다.
            p_Params.UpdateAffine(_MasterNode);
            p_StackTimer.SetParams(p_Params);

            return result;
        }
        
        /// <summary>
        /// 외력 감쇄 종료 콜백
        /// </summary>
        private void OnDampingTerminated(StackTimer<PhysicsTool.AccelerationType, PhysicsTool.UnitAddForceParams> p_StackTimer, PhysicsTool.AccelerationType p_Type)
        {
            ClearVelocity(p_Type, true);
        }

        #endregion
        
        #region <Methods>

        /// <summary>
        /// AddForce 테이블을 참조하여 속도값을 더하는 메서드
        /// </summary>
        private void ReserveVelocity(Vector3 p_Force, PhysicsTool.UnitAddForceParams p_UnitAddForceParams)
        {
            if (p_UnitAddForceParams.IsValid())
            {
                var recordIndex = p_UnitAddForceParams.UnitAddForceRecordIndex;
                var addForceRecord = UnitAddForceData.GetInstanceUnSafe[recordIndex];
                var forceDelay = addForceRecord.ForceDelay;
                var forceDirectionType = addForceRecord.ForceDirectionType;
                
#if UNITY_EDITOR
                if (CustomDebug.PrintAddForceFlag)
                {
                    var masternodePos = _Transform.position;
                    CustomDebug.DrawArrow(masternodePos, masternodePos + p_Force, 0.5f, Color.red, 1f);
                    Debug.Log($"Add Force Occur : {p_Force}({forceDirectionType})");
                }
#endif

                if (forceDelay < 50)
                {
                    var forceType = addForceRecord.ForceType;
                    AddVelocity(forceType, p_Force, p_UnitAddForceParams);
                }
                else
                {
                    var safeReference = GameEventTimerHandlerManager.GetInstance.SpawnSafeEventTimerHandler(this, SystemBoot.TimerType.GameTimer, false);
                    var (_, eventHandler) = safeReference.GetValue();
                    
                    eventHandler
                        .AddEvent
                        (
                            forceDelay,
                            handler =>
                            {
                                var thisPhysicsObject = handler.Arg1;
                                var record = handler.Arg2;
                                var addForceParams = handler.Arg5;

                                thisPhysicsObject.AddVelocity(record.ForceType, handler.Arg3, addForceParams);
                                thisPhysicsObject.CancelPreDelayForceEvent(handler.Arg4);

                                return true;
                            }, 
                            null, this, 
                            addForceRecord,
                            p_Force,
                            safeReference,
                            p_UnitAddForceParams
                        );
                    
                    _ForceTimer.Add(safeReference);
                    eventHandler.StartEvent();
                }
            }
        }
        
        public void CancelPreDelayForceEvent(SafeReference<object, GameEventTimerHandlerWrapper> p_EventHandler)
        {
            _ForceTimer.Remove(p_EventHandler);
            EventTimerTool.ReleaseEventHandler(ref p_EventHandler);
        }
        
        public void CancelAllPreDelayForceEvent()
        {
            EventTimerTool.ReleaseEventHandler(ref _ForceTimer);
        }

        private void AddDampingStackTimer(PhysicsTool.AccelerationType p_Type, PhysicsTool.UnitAddForceParams p_UnitAddForceParams)
        {
            var targetStackTimer = _DampingTimer[p_Type];
            targetStackTimer.SetParams(p_UnitAddForceParams);
            targetStackTimer.UpdateCount(p_UnitAddForceParams.Record.DampingBound);
        }

        #endregion
    }
}