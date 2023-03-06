using System;
using System.Collections.Generic;
using UnityEngine;

namespace k514
{
    /// <summary>
    /// 해당 유닛의 상태, 모드 등을 기술하는 유닛 부분 클래스
    /// </summary>
    public partial class Unit
    {
        #region <Fields>

        /// <summary>
        /// 현재 유닛 상태 마스크
        /// </summary>
        public UnitStateType _CurrentUnitStateMask;

        /// <summary>
        /// 현재 유닛에 쌓인 상태 스택
        /// </summary>
        private Dictionary<UnitStateType, int> _UnitStateStack;

        public List<FootStepLocation> _FootStepLocation;

        public FootStepLocation.TextureType CurrentTextureType;

#if !SERVER_DRIVE
        public Dictionary<MapEffectSoundManager.MapEffectSound, int> _EffectSoundLocation;
#endif

        public bool isWalla;
        
        #endregion
        
        #region <Enums>
        
        [Flags]
        public enum UnitStateType
        {
            /// <summary>
            /// 정상 상태
            /// </summary>
            None = 0,
            
            /// <summary>
            /// 사망
            /// </summary>
            DEAD = 1 << 0,
            
            /// <summary>
            /// 혼란 : 적에게 데미지를 주는 경우 대신 데미지를 받는다.
            /// </summary>
            CONFUSE = 1 << 1,
            
            /// <summary>
            /// 빙의 : 일정시간 캐릭터가 임의의 인공지능에 의해 제어를 받는다.
            /// </summary>
            POSSESS = 1 << 2,
            
            /// <summary>
            /// 이동불가
            /// </summary>
            IMMOBILIZE = 1 << 3,
            
            /// <summary>
            /// 침묵 : 액션이 동작하지 않는다.
            /// </summary>
            SILENCE = 1 << 4,
            
            /// <summary>
            /// 경직 : 이동 및 액션이 동작하지 않는다.
            /// </summary>
            STUCK = 1 << 5,
            
            /// <summary>
            /// 체공 : 점프 등으로 공중에 떠서 지면에 도달할 때까지 체공인 상태
            /// </summary>
            FLOAT = 1 << 6,
            
            /// <summary>
            /// 불멸 : 모든 데미지를 받지 않는다.
            /// </summary>
            IMMORTAL = 1 << 7,
            
            /// <summary>
            /// 무적 : 모든 데미지를 받지 않고, 적 유닛에게 타게팅 되지도 않는다.
            /// </summary>
            INVINCIBLE = 1 << 8,
            
            /// <summary>
            /// 저지불가 : 경직 상태가 되지 않는다.
            /// </summary>
            SUPERARMOR = 1 << 9,
            
            /// <summary>
            /// 스킬 시전 중인 상태
            /// </summary>
            DRIVESKILL = 1 << 10,
            
            /// <summary>
            /// 마을 안에 있는 상태 : 공격을 할 수 없다.
            /// </summary>
            VILLAGE = 1 << 11,

            /// <summary>
            /// 스턴
            /// </summary>
            STUN = 1 << 16,
            
            /// <summary>
            /// 출혈
            /// </summary>
            BLEED = 1 << 17,
                        
            /// <summary>
            /// 중독
            /// </summary>
            POISON = 1 << 18,
            
            /// <summary>
            /// 빙결?
            /// </summary>
            FREEZE = 1 << 19,

            /// <summary>
            /// 현 프레임 종료시 파기되는 대기 상태
            /// </summary>
            WaitForDead = 1 << 22,
            
            /// <summary>
            /// 비활성 : 시스템에 의해 일시적으로 사용불가인 상태
            /// </summary>
            SystemDisable = 1 << 23,
            
            /// <summary>
            /// 풀링된 유닛이 회수된 상태
            /// </summary>
            RETRIEVED = 1 << 24,


            /// <summary>
            /// 서버에의해서만 사망 가능
            /// </summary>
            NETWORK_DEAD = 1 << 100,
            
            /* Available */
            // 아래 상태들로만 현재 상태가 구성되어있는 경우만을 허용하는 용도
            DefaultActionAvailableMask = IMMORTAL | IMMOBILIZE | INVINCIBLE | SUPERARMOR | DRIVESKILL | BLEED | POISON,
            DefaultMoveAvailableMask = FLOAT | IMMORTAL | INVINCIBLE | SUPERARMOR | VILLAGE | SILENCE | BLEED | POISON,
            DefaultJumpAvailableMask = FLOAT | IMMORTAL | INVINCIBLE | SUPERARMOR | VILLAGE | SILENCE | BLEED | POISON,
            
            /* Filter */
            // 아래 상태를 포함한 유닛을 필터링하는 용도
            UnitPartyRegisterFilterMask = DEAD | SystemDisable | RETRIEVED,
            UnitTryActionFilterMask = DEAD | SystemDisable | RETRIEVED | VILLAGE,
            UnitFightableFilterMask = DEAD | INVINCIBLE | SystemDisable | RETRIEVED,
            UnitIdlableFilterMask = DEAD | STUCK | DRIVESKILL | STUN | RETRIEVED | FREEZE,
            UnitAIStoppableFilterMask = DEAD | DRIVESKILL | RETRIEVED,
            UnitAIAwakableFilterMask = DEAD | STUCK | DRIVESKILL | STUN | RETRIEVED | FREEZE,
            UnitAIAwakeableTimerEventFilterMask = STUN | FREEZE,
            UnitViewAreaFilterMask = DEAD | INVINCIBLE | RETRIEVED,
        }

        #endregion
        
        #region <Callbacks>

        private void OnAwakeState()
        {
            _UnitStateStack = new Dictionary<UnitStateType, int>();
            _FootStepLocation = new List<FootStepLocation>();
#if !SERVER_DRIVE
            _EffectSoundLocation = new Dictionary<MapEffectSoundManager.MapEffectSound, int>();
#endif

            isWalla = false;

#if !SERVER_DRIVE
            var MapEffectTypeEnumerator = SystemTool.GetEnumEnumerator<MapEffectSoundManager.MapEffectSound>(SystemTool.GetEnumeratorType.GetAll);
            foreach (var mapEffectType in MapEffectTypeEnumerator)
            {
                _EffectSoundLocation.Add(mapEffectType, 0);
            }
#endif
        }

        private void OnPoolingState()
        {
            ClearState();

#if !SERVER_DRIVE
            CurrentTextureType = LamiereGameManager.GetInstanceUnSafe.CurrentSceneTextureType;
#endif
        }

        private void OnRetrieveState()
        {
            _FootStepLocation.Clear();
#if !SERVER_DRIVE
            _EffectSoundLocation.Clear();
#endif
        }

        private void OnUnitStateAdded(UnitStateType p_UnitStateType, bool p_IsReentered)
        {
#if UNITY_EDITOR
            if (CustomDebug.PrintUnitState)
            {
                Debug.Log($"[{name}] state transition to [{p_UnitStateType}]");
            }
#endif
        }

        private void OnUnitStateRemoved(UnitStateType p_UnitStateType)
        {
#if UNITY_EDITOR
            if (CustomDebug.PrintUnitState)
            {
                Debug.Log($"[{name}] state [{p_UnitStateType}] over");
            }
#endif
        }

#if !SERVER_DRIVE
        public void OnEnterFootStep(FootStepLocation p_FootStepLocation)
        {
            _FootStepLocation.Add(p_FootStepLocation);
            CurrentTextureType = p_FootStepLocation._TextureType;
        }

        public void OnExitFootStep(FootStepLocation p_FootStepLocation)
        {
            _FootStepLocation.Remove(p_FootStepLocation);
            
            if (_FootStepLocation.Count < 1)
            {
                CurrentTextureType = LamiereGameManager.GetInstanceUnSafe.CurrentSceneTextureType;
            }
            else
            {
                CompareFootStep();
            }
        }

        public void OnEnterEffectSound(EffectSoundLocation p_EffectSoundLocation)
        {
            MapEffectSoundManager.GetInstance.GetMapEffectUnit(p_EffectSoundLocation._EffectKey, _Transform, p_EffectSoundLocation._MapEffectSound);
            _EffectSoundLocation[p_EffectSoundLocation._MapEffectSound]++;
        }
        
        public void OnExitEffectSound(EffectSoundLocation p_EffectSoundLocation)
        {
            _EffectSoundLocation[p_EffectSoundLocation._MapEffectSound]--;

            if (_EffectSoundLocation[p_EffectSoundLocation._MapEffectSound] < 1)
            {
                if (p_EffectSoundLocation._MapEffectSound == MapEffectSoundManager.MapEffectSound.Environment)
                {
                    MapEffectSoundManager.GetInstance.GetMapEffectUnit(LamiereGameManager.GetInstanceUnSafe.CurrentSceneEffectSoundIndex, this, MapEffectSoundManager.MapEffectSound.Environment);
                }
                else
                {
                    MapEffectSoundManager.GetInstance._ReservedMapEffectCountCollection[p_EffectSoundLocation._MapEffectSound].RetrieveObject();
                    MapEffectSoundManager.GetInstance._ReservedMapEffectCountCollection[p_EffectSoundLocation._MapEffectSound] = null;
                }
            }
        }
#endif
        
        #endregion
    
        #region <Method/UnitState>
        
        /// <summary>
        /// 파라미터 마스크의 플래그를 전부 보유했는지 검증
        /// </summary>
        public bool HasState_And(UnitStateType p_CompareMask)
        {
            return _CurrentUnitStateMask.HasAllFlag(p_CompareMask);
        }
        
        /// <summary>
        /// 파라미터 마스크의 플래그 중에 하나라도 보유했는지 검증
        /// </summary>      
        public bool HasState_Or(UnitStateType p_CompareMask)
        {
            return _CurrentUnitStateMask.HasAnyFlagExceptNone(p_CompareMask);
        }
        
        /// <summary>
        /// 파라미터 마스크에 포함된 플래그만 가지고 있는지 검증
        /// </summary>
        public bool HasState_Only(UnitStateType p_AvailableStateMask)
        {
            return _CurrentUnitStateMask.HasFlagOnly(p_AvailableStateMask);
        }
        
        /*public void TurnState(UnitStateType p_Type)
        {
            foreach (var unitStateType in UnitTool.UnitStateEnumerator)
            {
                if (HasState_Or(unitStateType))
                {
                    _CurrentUnitStateMask.RemoveFlag(unitStateType);
                    if (_UnitStateStack.ContainsKey(unitStateType))
                    {
                        _UnitStateStack[unitStateType] = 0;
                        OnUnitStateRemoved(unitStateType);
                    }
                }
            }

            AddState(p_Type);
        }*/
        
        public void AddState(UnitStateType p_Type)
        {
            if (_UnitStateStack.ContainsKey(p_Type))
            {
#if UNITY_EDITOR
                if (CustomDebug.PrintUnitState)
                {
                    Debug.Log($"[{name}] state stacked [{p_Type}] ({_UnitStateStack[p_Type]} => {_UnitStateStack[p_Type] + 1})");
                }
#endif
                _UnitStateStack[p_Type]++;
            }

            if (HasState_Or(p_Type))
            {
                OnUnitStateAdded(p_Type, true);
            }
            else
            {
                _CurrentUnitStateMask.AddFlag(p_Type);
                OnUnitStateAdded(p_Type, false);
            }
        }

        public void RemoveState(UnitStateType p_Type)
        {
            if (HasState_Or(p_Type))
            {
                if (_UnitStateStack.ContainsKey(p_Type))
                {
#if UNITY_EDITOR
                    if (CustomDebug.PrintUnitState)
                    {
                        Debug.Log($"[{name}] state stacked [{p_Type}] ({_UnitStateStack[p_Type]} => {Mathf.Max(0, _UnitStateStack[p_Type] - 1)})");
                    }
#endif
                    _UnitStateStack[p_Type] = Mathf.Max(0, _UnitStateStack[p_Type] - 1);
                    if (_UnitStateStack[p_Type] == 0)
                    {
                        _CurrentUnitStateMask.RemoveFlag(p_Type);
                        OnUnitStateRemoved(p_Type);
                    }
                }
                else
                {
                    _CurrentUnitStateMask.RemoveFlag(p_Type);
                    OnUnitStateRemoved(p_Type);
                }
            }
        }

        public void ClearState()
        {
            // TurnState(GetDefaultState());

            switch (PoolState)
            {
                case PoolState.Actived:
                    break;
                case PoolState.Pooled:
                case PoolState.Retrieving:
                    AddState(UnitStateType.RETRIEVED);
                    break;
            }
        }

        protected UnitStateType GetDefaultState()
        {
            return UnitStateType.None;
        }

        #endregion

        #region <Method/Sound>

        public void CompareFootStep()
        {
            var currentLocation = _FootStepLocation[0];
            foreach (var location in _FootStepLocation)
            {
                if (currentLocation._Priority < location._Priority)
                {
                    currentLocation = location;
                }
            }

            CurrentTextureType = currentLocation._TextureType;
        }

        #endregion
    }
}