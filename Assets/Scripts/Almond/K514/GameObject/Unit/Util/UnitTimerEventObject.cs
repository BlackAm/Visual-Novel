using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace k514
{
    /// <summary>
    /// 유닛의 버프나 비동기적인 작업을 이벤트 타이머 핸들러에 예약하고 관련 통신을 지원하는 오브젝트
    /// </summary>
    public class UnitEventTypeObject : EventTimerHandlerWrapper<Unit.UnitTimerEventType, UnitEventTypeObject>
    {
        #region <Fields>

        private uint _PreDelay;
        private uint _Interval;
        private int _TickCount;
        private int _CurrentIntervalCount, _MaxIntervalCount;
        private float _Inverse_MaxIntervalCount;
        private Unit _TargetUnit;
        private Unit _StrikerUnit;

        private bool _SendState;
        
        #endregion
        
        #region <Callbacks>

        public override void OnSpawning()
        {
        }

        public override void OnPooling()
        {
        }

        public override void OnRetrieved()
        {
            base.OnRetrieved();
            
            SetIntervalCount(0);
            SetDelay(0, 0);
            SetTargetUnit(null, null);
            SetSendState(false);
        }

        /*public override void WhenPropertyTurnToDefault()
        {
            switch (_SubType)
            {
                case Unit.UnitTimerEventType.Poison:
                    _TargetUnit.RemoveState(Unit.UnitStateType.POISON);
                    break;
                case Unit.UnitTimerEventType.Bleed:
                    _TargetUnit.RemoveState(Unit.UnitStateType.BLEED);
                    break;
                case Unit.UnitTimerEventType.Stun:
                    _TargetUnit.RemoveState(Unit.UnitStateType.STUN);
                    break;
                case Unit.UnitTimerEventType.Immobilize:
                    _TargetUnit.RemoveState(Unit.UnitStateType.IMMOBILIZE);
                    break;
                case Unit.UnitTimerEventType.Silence:
                    _TargetUnit.RemoveState(Unit.UnitStateType.SILENCE);
                    break;
                case Unit.UnitTimerEventType.Freeze:
                    _TargetUnit.RemoveState(Unit.UnitStateType.FREEZE);
                    break;
                case Unit.UnitTimerEventType.SuperArmor:
                    break;
                case Unit.UnitTimerEventType.Invincible:
                    break;
            }
            
            base.WhenPropertyTurnToDefault();
        }*/
      
        public void OnEventSpawned()
        {
#if UNITY_EDITOR
            if (CustomDebug.PrintUnitHitLog)
            {
                Debug.Log($"{_TargetUnit.name} EventSpawned : {_CurrentIntervalCount}/{_MaxIntervalCount}");
            }
#endif
            /*switch (_SubType)
            {
                case Unit.UnitTimerEventType.Poison:
                    _TargetUnit.AddState(Unit.UnitStateType.POISON);
                    break;
                case Unit.UnitTimerEventType.Bleed:
                    _TargetUnit.AddState(Unit.UnitStateType.BLEED);
                    break;
                case Unit.UnitTimerEventType.Stun:
                    _TargetUnit.AddState(Unit.UnitStateType.STUN);
                    break;
                case Unit.UnitTimerEventType.Immobilize:
                    _TargetUnit.AddState(Unit.UnitStateType.IMMOBILIZE);
                    break;
                case Unit.UnitTimerEventType.Silence:
                    _TargetUnit.AddState(Unit.UnitStateType.SILENCE);
                    break;
                case Unit.UnitTimerEventType.Freeze:
                    _TargetUnit.AddState(Unit.UnitStateType.FREEZE);
                    break;
                case Unit.UnitTimerEventType.SuperArmor:
                    break;
                case Unit.UnitTimerEventType.Invincible:
                    break;
            }*/
        }
        
        public bool OnStartTick()
        {
#if UNITY_EDITOR
            if (CustomDebug.PrintUnitHitLog)
            {
                Debug.Log($"{_TargetUnit.name} Extra Inning Tick : {_CurrentIntervalCount}/{_MaxIntervalCount}");
            }
#endif
            _CurrentIntervalCount++;
            /*var _SpawnIntervalTimer = GameEventTimerHandlerManager.GetInstance.SpawnEventTimerHandler(SystemBoot.TimerType.GameTimer, false);

            switch (_SubType)
            {
                //<TODO>514 : 이전버전으로 롤백됨
                case Unit.UnitTimerEventType.Poison:
                    _TickHitMessage = _DefaultTickHitMessage;
                    _TargetUnit.HitUnit(_StrikerUnit, _TickHitMessage, default, true);
                    break;
                case Unit.UnitTimerEventType.Bleed:
                    _TickHitMessage = _DefaultTickHitMessage;
                    _TargetUnit.HitUnit(_StrikerUnit, _TickHitMessage, default, true);
                    break;
                case Unit.UnitTimerEventType.Stun:
//                    var resistDebuff = _TargetUnit._BattleStatusPreset.t_Current.GetDebuffResistRate() + 0.5f * _CurrentIntervalCount * _Inverse_MaxIntervalCount;
//                    if (resistDebuff > Random.value)
//                    {
//                        _CurrentIntervalCount = _MaxIntervalCount + 1;
//                    }
                    break;
                case Unit.UnitTimerEventType.Immobilize:
                    break;
                case Unit.UnitTimerEventType.Silence:
                    break;
                case Unit.UnitTimerEventType.Freeze:
                    break;
                case Unit.UnitTimerEventType.SuperArmor:
                    break;
                case Unit.UnitTimerEventType.Invincible:
                    break;
                case Unit.UnitTimerEventType.TempAddValue:
                    _TargetUnit.AddValue(_SkillStatusPreset, new UnitPropertyChangePreset(_StrikerUnit));
                    _TargetUnit.AddValue(_BattleStatusPreset, new UnitPropertyChangePreset(_StrikerUnit));
                    _TargetUnit.AddValue(_BaseStatusPreset, new UnitPropertyChangePreset(_StrikerUnit));
                    _SpawnIntervalTimer
                        .AddEvent
                        (
                            _Interval,
                            handler =>
                            {
                                handler.Arg2.AddValue(handler.Arg1._SkillStatusPreset * -1f, new UnitPropertyChangePreset(handler.Arg2));
                                handler.Arg2.AddValue(handler.Arg1._BattleStatusPreset * -1f, new UnitPropertyChangePreset(handler.Arg2));
                                handler.Arg2.AddValue(handler.Arg1._BaseStatusPreset * -1f, new UnitPropertyChangePreset(handler.Arg2));
                                return true;
                            },
                            null, this,
                            _TargetUnit
                        );
                    _SpawnIntervalTimer.StartEvent();
                    break;
                case Unit.UnitTimerEventType.SmoothAddValue:
                    _TargetUnit.AddValue(_SkillStatusPreset, new UnitPropertyChangePreset(_StrikerUnit));
                    _TargetUnit.AddValue(_BattleStatusPreset, new UnitPropertyChangePreset(_StrikerUnit), _SendState);
                    _TargetUnit.AddValue(_BaseStatusPreset, new UnitPropertyChangePreset(_StrikerUnit));
                    break;
            }*/

            return true;
        }

        public bool OnCheckEventOver()
        {
            return _CurrentIntervalCount >= _MaxIntervalCount;
        }

        #endregion

        #region <Methods>

        public void SetIntervalCount(int p_IntervalCount)
        {
            _CurrentIntervalCount = 0;
            _MaxIntervalCount = p_IntervalCount;
            _Inverse_MaxIntervalCount = 1f / _MaxIntervalCount;
        }

        public void SetDelay(uint p_PreDelay, uint p_Interval)
        {
            _PreDelay = p_PreDelay;
            _Interval = p_Interval;
        }
        public void SetSendState(bool send)
        {
            _SendState = send;
        }

        public void SetTargetUnit(Unit p_TargetUnit, Unit p_StrikerUnit)
        {
            _TargetUnit = p_TargetUnit;
            _StrikerUnit = p_StrikerUnit;
        }

        public void SetTrigger()
        {
            OnEventSpawned();
            
            SystemBoot
                .GameEventTimer
                .RunTimer
                (
                    this,
                    (_PreDelay, _Interval), 
                    handler => handler.Arg1.OnStartTick(), 
                    handler => handler.Arg1.OnCheckEventOver(),
                    this
                );
        }

        #endregion
    }
}