using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UI2020;

namespace k514
{
    public partial class Unit
    {
        #region <Consts>

        private const uint __Combat_Info_Count_Interval = 100;
        private const int __Combat_Info_Default_Duration_Count = 30;

        #endregion
        
        #region <Fields>

        private Dictionary<UnitTool.UnitCombatInfoType, StackTimer<UnitTool.UnitCombatInfoType>> _UnitCombatInfoTable;

        /// <summary>
        /// 가장 최근에 해당 유닛이 공격한 유닛 프리셋
        /// </summary>
        public UnitCombatStatePreset LastStrikeUnitInfoPreset { get; private set; }

        /// <summary>
        /// 가장 최근에 해당 유닛을 공격한 유닛 프리셋
        /// </summary>
        public UnitCombatStatePreset LastAttackUnitInfoPreset { get; private set; }

        //공격한 케릭터 
        private Dictionary<ulong, uint> _AttackUnitList = new Dictionary<ulong, uint>();
        //마지막 공격자
        public ulong _LastAttackTrigger { get; private set; }
        //현재 누적데미지
        private uint _TotalDamage;
        
        #endregion

        #region <Callbacks>

        private void OnAwakeCombat()
        {
            _UnitCombatInfoTable = new Dictionary<UnitTool.UnitCombatInfoType, StackTimer<UnitTool.UnitCombatInfoType>>();

            var enumerator = UnitTool.UnitCombatInfoTypeEnumerator;
            foreach (var unitCombatInfoType in enumerator)
            {
                _UnitCombatInfoTable.Add
                (
                    unitCombatInfoType, 
                    new StackTimer<UnitTool.UnitCombatInfoType>(__Combat_Info_Count_Interval, unitCombatInfoType, OnTerminateCombatInfo, OnUpdateCombatInfo, false)
                );
            }
        }

        private void OnPoolingCombat()
        {
        }

        private void OnRetrieveCombat()
        {
            foreach (var stackTimer in _UnitCombatInfoTable)
            {
                stackTimer.Value.OnTerminateStackTimer();
            }
        }

        private bool OnUpdateCombatInfo(StackTimer<UnitTool.UnitCombatInfoType> p_StackTimer, UnitTool.UnitCombatInfoType p_Type)
        {
            switch (p_Type)
            {
                case UnitTool.UnitCombatInfoType.Stuck:
#if UNITY_EDITOR
                    if (CustomDebug.PrintGameSystemLog && !HasState_And(UnitStateType.DEAD))
                    {
                        Debug.Log($"[{name}] / Stucked {p_StackTimer.CountStack} ");
                    }
#endif      
                    if(HasState_Or(UnitStateType.FLOAT | UnitStateType.DEAD))
                    {
                        p_StackTimer.OverlapCount(1);
                    }
                    break;
                case UnitTool.UnitCombatInfoType.LastAttack:
                    break;
                case UnitTool.UnitCombatInfoType.LastStrike:
                    break;
            }

            return true;
        }

        private void OnTerminateCombatInfo(StackTimer<UnitTool.UnitCombatInfoType> p_StackTimer, UnitTool.UnitCombatInfoType p_Type)
        {
            switch (p_Type)
            {
                case UnitTool.UnitCombatInfoType.Stuck:
                    _AnimationObject.TryHitMotionBreak();
                    break;
                case UnitTool.UnitCombatInfoType.LastAttack:
                    LastAttackUnitInfoPreset = default;
                    break;
                case UnitTool.UnitCombatInfoType.LastStrike:
                    LastStrikeUnitInfoPreset = default;
                    break;
            }
        }
        
        /// <summary>
        /// 데미지를 가한 경우 호출되는 콜백
        /// </summary>
        private void OnStriked(Unit p_Target, HitResult p_HitResult)
        {
            // 해당 유닛이 공격한 유닛을 갱신한다.
            RequestUpdateCombatInfo(UnitTool.UnitCombatInfoType.LastStrike, p_Target);
            
            for (int i = 0; i < _ModuleCount; i++)
            {
                _ModuleList[i].CurrentSelectedModule.OnStriked(p_Target, p_HitResult);
            }
        }

        private void OnStrikedNormal(Unit p_Target, HitResult p_HitResult)
        {
#if !SERVER_DRIVE
            if (IsPlayer)
            {
                PlayerManager.GetInstance.OnPlayerStriked(p_Target, p_HitResult);
            }
#endif
            UnitHitTool.PlayHitFX(UnitHitTool.HitResultType.HitNormal, p_Target, this, p_HitResult);
            OnStriked(p_Target, p_HitResult);
        }

        private void OnStrikedCritical(Unit p_Target, HitResult p_HitResult)
        {
#if !SERVER_DRIVE
            if (IsPlayer)
            {
                PlayerManager.GetInstance.OnPlayerStrikedCritical(p_Target, p_HitResult);
            }
#endif
            UnitHitTool.PlayHitFX(UnitHitTool.HitResultType.HitCritical, p_Target, this, p_HitResult);
            OnStriked(p_Target, p_HitResult);
        }

        public void OnKilledUnit(Unit p_Victim)
        {
            KillPoint++;
            
#if UNITY_EDITOR
            if (CustomDebug.PrintGameSystemLog)
            {
                Debug.Log($"[{name}]이(가) [{p_Victim.name}]을 처치했습니다. 현재 처치 수 : [{KillPoint}]");
            }
#endif

#if !SERVER_DRIVE
            if (IsPlayer)
            {
                PlayerManager.GetInstance.OnPlayerKilled(p_Victim);
            }
#endif

            TryHandle_Evolve_KillingEvent();
            // Almond.Util.EventDispatcher.TriggerEvent(PopUpEvent.POP_DIE, p_Victim);
        }

        /// <summary>
        /// 데미지를 입은 경우 호출되는 콜백.
        /// </summary>
        private void OnHitted(Unit p_Trigger, HitResult p_HitResult)
        {
            // 해당 유닛을 공격한 정보를 갱신한다.
            RequestUpdateCombatInfo(UnitTool.UnitCombatInfoType.LastAttack, p_Trigger);
            
            OnHitStuckTriggered(p_Trigger, p_HitResult);
            OnHitTickTriggered(p_Trigger, p_HitResult);
            OnHitBuffTriggered(p_Trigger, p_HitResult);

            for (int i = 0; i < _ModuleCount; i++)
            {
                _ModuleList[i].CurrentSelectedModule.OnHitted(p_Trigger, p_HitResult);
            }
        }

        private void OnHittedNormal(Unit p_Trigger, HitResult p_HitResult)
        {
#if !SERVER_DRIVE
            if (IsPlayer)
            {
                PlayerManager.GetInstance.OnPlayerHitted(p_Trigger, p_HitResult);
            }
#endif
            OnHitted(p_Trigger, p_HitResult);
        }

        private void OnHittedCritical(Unit p_Trigger, HitResult p_HitResult)
        {
#if !SERVER_DRIVE
            if (IsPlayer)
            {
                PlayerManager.GetInstance.OnPlayerHittedCritical(p_Trigger, p_HitResult);
            }
#endif
            OnHitted(p_Trigger, p_HitResult);
        }
        
        /// <summary>
        /// 경직이 발생한 경우 호출되는 콜백
        /// </summary>
        protected void OnHitStuckTriggered(Unit p_Trigger, HitResult p_HitResult)
        {
            return;
            
            var (stuckValid, stuckIndex) = p_HitResult.GetHitParameter(UnitHitTool.HitParameterType.HitStuck);
            if (stuckValid)
            {
                // 경직 플래그를 세운다.
                AddState(UnitStateType.STUCK);

                // 경직 타입에 따라 경직 카운트를 갱신시킨다.
                var hitStuckRecord = UnitHitStuckData.GetInstanceUnSafe[stuckIndex];
                var stuckType = hitStuckRecord.StuckType;
                var stuckCount = (int)hitStuckRecord.StuckDurationCount;
                var tryTimer = _UnitCombatInfoTable[UnitTool.UnitCombatInfoType.Stuck];
                
                switch (stuckType)
                {
                    case UnitHitTool.HitStuckType.Stack:
                        tryTimer.AddCount(stuckCount);
                        break;
                    case UnitHitTool.HitStuckType.Update:
                        tryTimer.UpdateCount(stuckCount);
                        break;
                    case UnitHitTool.HitStuckType.UpdateBigger:
                        tryTimer.OverlapCount(stuckCount);
                        break;
                }

                tryTimer.SetBlockTick(true);
                _AnimationObject.TryHitMotion();
                
    #if !SERVER_DRIVE
                if (IsPlayer)
                {
                    PlayerManager.GetInstance.OnPlayerStucked(p_Trigger, p_HitResult);
                }
    #endif
            }
        }
        
        /// <summary>
        /// 타격 상태이상이 발생한 경우 호출되는 콜백
        /// </summary>
        protected void OnHitTickTriggered(Unit p_Trigger, HitResult p_HitResult)
        {
            if (p_HitResult.HitResultFlagMask.HasFlag(HitResult.HitResultFlag.Tick))
            {
                var (tickValid, tickIndex) = p_HitResult.GetHitParameter(UnitHitTool.HitParameterType.HitTick);
                if (tickValid)
                {
                    // AddUnitTickEvent(tickIndex, p_Trigger);
                }
            }
        }
                
        /// <summary>
        /// 타격 버프가 발생한 경우 호출되는 콜백
        /// </summary>
        protected void OnHitBuffTriggered(Unit p_Trigger, HitResult p_HitResult)
        {
            if (p_HitResult.HitResultFlagMask.HasFlag(HitResult.HitResultFlag.Buff))
            {
                var (buffValid, buffIndex) = p_HitResult.GetHitParameter(UnitHitTool.HitParameterType.HitBuff);
                if (buffValid)
                {
                    // AddUnitBuffEvent(buffIndex, p_Trigger);
                }
            }
        }
        
        /// <summary>
        /// 히트 모션 등이 시작된 경우 호출되는 콜백
        /// </summary>
        public void OnHitActionStart()
        {
            // 서브 유닛 모듈에 해당 이벤트를 전파해준다.
            for (int i = 0; i < _ModuleCount; i++)
            {
                _ModuleList[i].CurrentSelectedModule.OnUnitHitActionStarted();
            }
        }
        
        /// <summary>
        /// 히트 모션이 경직 처리를 위해 정지한 경우 호출되는 콜백
        /// </summary>
        public void OnHitMotionStopCued()
        {
            var tryTimer = _UnitCombatInfoTable[UnitTool.UnitCombatInfoType.Stuck];
            tryTimer.SetBlockTick(false);
        }

        /// <summary>
        /// 히트 모션 등이 종료된 경우 호출되는 콜백
        /// </summary>
        public void OnHitActionTerminate()
        {
            // 경직 상태 플래그를 지운다.
            RemoveState(UnitStateType.STUCK);
            
            // 서브 유닛 모듈에 해당 이벤트를 전파해준다.
            for (int i = 0; i < _ModuleCount; i++)
            {
                _ModuleList[i].CurrentSelectedModule.OnUnitHitActionTerminated();
            }
        }

        #endregion
        
        #region <Methods>

        public void RequestUpdateCombatInfo(UnitTool.UnitCombatInfoType p_Type, Unit p_Unit)
        {
            switch (p_Type)
            {
                case UnitTool.UnitCombatInfoType.Stuck:
                    break;
                case UnitTool.UnitCombatInfoType.LastAttack:
                    _UnitCombatInfoTable[UnitTool.UnitCombatInfoType.LastAttack].UpdateCount(__Combat_Info_Default_Duration_Count);
                    LastAttackUnitInfoPreset = new UnitCombatStatePreset(p_Type, p_Unit, this, Time.time);
                    UnitInteractManager.GetInstance.UpdateCombatInfoBetween(this, LastAttackUnitInfoPreset);
                    break;
                case UnitTool.UnitCombatInfoType.LastStrike:
                    _UnitCombatInfoTable[UnitTool.UnitCombatInfoType.LastStrike].UpdateCount(__Combat_Info_Default_Duration_Count);
                    LastStrikeUnitInfoPreset = new UnitCombatStatePreset(p_Type, this, p_Unit, Time.time);
                    break;
            }
        }

        /// <summary>
        /// 현재 유닛의 경직을 제거하는 메서드
        /// </summary>
        public void ClearCombatInfo(UnitTool.UnitCombatInfoType p_Type)
        {
            _UnitCombatInfoTable[p_Type].OnTerminateStackTimer();
        }

        /// <summary>
        /// 해당 유닛에 피격 판정을 더하는 메서드
        /// </summary>
        public HitResult HitUnit(Unit p_Trigger, HitMessage p_HitMessage, UnitTool.UnitAddForcePreset p_HitVariablePreset, bool IsTickDamage)
        {
            // 타격 유닛은 존재하지 않을 수도 있다.
            var isStrikerValid = !ReferenceEquals(null, p_Trigger);

#if !SERVER_DRIVE
            if(p_Trigger.IsPlayer) 
            {
                /*if(!LamiereGameManager.GetInstanceUnSafeUnSafeUnSafe.isFirstHitted && !LamiereGameManager.GetInstanceUnSafeUnSafe._AutoPlayMode.Equals(LamiereGameManager.PlayerAutoMode.AutoMode)) {
                    var mainUI = UI2020.MainGameUI.Instance.mainUI;
                    mainUI.SetScanner();
                    LamiereGameManager.GetInstanceUnSafeUnSafe.isFirstHitted = true; 
                }*/
                
                // momo6346 - 변경된 몬스터 스캔.
                // 일반사냥은 스캔 전까진 따로 출력되지 않습니다.
                // 자동사냥은 활성화 즉시 몬스터 스캔을 합니다.
                /*if(LamiereGameManager.GetInstanceUnSafeUnSafe._AutoPlayMode.Equals(LamiereGameManager.PlayerAutoMode.AutoMode) ||
                   MainGameUI.Instance.mainUI._searchedMonsterList.Count > 0) {
                    var mainUI = UI2020.MainGameUI.Instance.mainUI;
                    mainUI.SetScanner();
                }*/
            }
#endif
            
            /* SE Cond */
            // 1. 타격 유닛이 플레이어이거나 피격유닛이 원격 유닛이 아니거나 원격 유닛이었어도 서버노드에 있는 유닛이었던 경우
            // 2. 피격 유닛이 사망했거나 무적상태가 아닌 경우
            if (isStrikerValid && p_Trigger.IsPlayer && !HasState_Or(UnitStateType.DEAD | UnitStateType.INVINCIBLE))
            {
                if (!IsTickDamage)
                {
                    // HitMessage를 기준으로 HitResult를 연산한다.
                    var hitResult = this.CalcDamage(p_Trigger, isStrikerValid, p_HitMessage, p_HitVariablePreset);
                    // 연산된 데미지를 적용한다.
                    hitResult.ApplyHitDamage();
                
                    // 데미지 적용 이후 이벤트를 처리해준다.
                    UnitHitTool.ApplyHitResult(hitResult, false);
                    return hitResult;
                }
                else
                {
                    // HitMessage를 기준으로 HitResult를 연산한다.
                    var hitResult = this.CalcTickDamage(p_Trigger, isStrikerValid, p_HitMessage);
                    // 연산된 데미지를 적용한다.
                    hitResult.ApplyHitDamage();
                    
                    // 데미지 적용 이후 이벤트를 처리해준다.
                    UnitHitTool.ApplyHitResult(hitResult, false);
                    return hitResult;
                }
                
            }
            else
            {
                var hitResult = UnitHitTool.HIT_FAIL(p_HitMessage.GetHitParameter());
                UnitHitTool.PlayHitFX(UnitHitTool.HitResultType.HitFail, p_Trigger, this, hitResult);
                return hitResult;
            }
        }

        public void ApplyHitResult(HitResult p_HitResult)
        {
            var trigger = p_HitResult.Trigger;
            var isTriggerValid = p_HitResult.HitResultFlagMask.HasAnyFlagExceptNone(HitResult.HitResultFlag.HasTrigger);
            if (IsPlayer && _ActableObject._CurrentIdleState != ActableTool.IdleState.Combat)
            {
                _ActableObject.TurnIdleState(ActableTool.IdleState.Combat, AnimatorParamStorage.MotionTransitionType.Restrict_ErasePrevMotion);
                SetIdleState();
            }
            else if (IsPlayer && _ActableObject._CurrentIdleState == ActableTool.IdleState.Combat)
            {
                SetIdleState();
            }
#if SERVER_DRIVE
            AddHitUnit(p_HitResult.Trigger._UnitNetworkPreset.UnitUniqueKey, (uint)p_HitResult.ResultDamage);
            NetworkPacketManager.CharacterStateInfo(_UnitNetworkPreset.UnitUniqueKey, StateAction.HP_SYNC, (uint)_BattleStatusPreset.t_Current.GetProperty(BattleStatusPreset.BattleStatusType.HP_Base), this);
#endif
            // 데미지 계산 결과에 따라
            switch (p_HitResult.HitResultType)
            {
                case UnitHitTool.HitResultType.HitNormal:
                    if (HasState_Or(UnitStateType.DEAD))
                    {
                        if (isTriggerValid)
                        {
                            trigger.OnStrikedNormal(this, p_HitResult);
                            trigger.OnKilledUnit(this);
                        }
                    }
                    else
                    {
                        if (isTriggerValid)
                        {
                            trigger.OnStrikedNormal(this, p_HitResult);
                            if (p_HitResult.Target.HasState_Or(UnitStateType.FREEZE))
                            {
                                p_HitResult.Target.RemoveState(UnitStateType.FREEZE);
                            }
                        }
                        OnHittedNormal(trigger, p_HitResult);
                    }
                    break;
                case UnitHitTool.HitResultType.HitCritical:
                    if (HasState_Or(UnitStateType.DEAD))
                    {
                        if (isTriggerValid)
                        {
                            trigger.OnStrikedCritical(this, p_HitResult);
                            trigger.OnKilledUnit(this);
                        }
                    }
                    else
                    {
                        if (isTriggerValid)
                        {
                            trigger.OnStrikedCritical(this, p_HitResult);
                            if (p_HitResult.Target.HasState_Or(UnitStateType.FREEZE))
                            {
                                p_HitResult.Target.RemoveState(UnitStateType.FREEZE);
                            }
                        }
                        OnHittedCritical(trigger, p_HitResult);
                    }
                    break;
                case UnitHitTool.HitResultType.HitMissed:
                    OnHitted(trigger, p_HitResult);
                    break;
            }
        }

        

        void DamageCut(ref Dictionary<ulong, uint> damageList, float damageCut)
        {
            //데미지 정렬
            damageList.OrderByDescending(ranking => ranking.Value);
            var list = damageList.Keys.ToList();

            for (int i = 0; i < list.Count; i++)
            {
                if (damageList[list[i]] < damageCut)
                {
                    //일정데미지 이하일경우 삭제
                    damageList.Remove(list[i]);
                }
            }
        }

        #endregion
    }
}