using System;

namespace k514
{
    public static class ThinkableTool
    {
        #region <Consts>

        public const int __SLAVE_AI_ACTION_INTERVAL = 10;
        public const int __NEGATIVE_SLAVE_AI_ACTION_INTERVAL = -__SLAVE_AI_ACTION_INTERVAL;
        public const float __SLAVE_AI_TRACE_DISTANCE_LOWERBOUND = 8f;
        public const float __SLAVE_AI_TRACE_DISTANCE_UPPERBOUND = 25f;
        public const float __SQR_SLAVE_AI_TRACE_DISTANCE_LOWERBOUND = __SLAVE_AI_TRACE_DISTANCE_LOWERBOUND * __SLAVE_AI_TRACE_DISTANCE_LOWERBOUND;
        public const float __SQR_SLAVE_AI_TRACE_DISTANCE_UPPERBOUND = __SLAVE_AI_TRACE_DISTANCE_UPPERBOUND * __SLAVE_AI_TRACE_DISTANCE_UPPERBOUND;
        public const float __SLAVE_AI_WANDERING_DISTANCE = 4f;

        #endregion
        
        #region <Enums>

        /// <summary>
        /// 유닛의 행동을 다양하게 정의하는 플래그
        /// </summary>
        [Flags]
        public enum AIExtraFlag
        {
            /// <summary>
            /// 기본 상태
            /// </summary>
            None = 0,
            
            /// <summary>
            /// 주변 적을 탐색하는 플래그,
            /// 해당 플래그를 보유하는 경우 주변적을 일정 주기로 탐색한다.
            /// </summary>
            EncounterCheck = 1 << 0,
            
            /// <summary>
            /// 방황을 하는 플래그,
            /// 해당 플래그를 보유하는 경우 일정 주기로 유닛 생성 지점(pivot point)를 주변으로
            /// 옮겨 유닛이 주변을 방황한다.
            /// </summary>
            Wandering = 1 << 1,
            
            /// <summary>
            /// 반격을 하는 플래그,
            /// 공격을 받은 경우, 최초에 공격한 유닛을 최우선 적으로 추적한다.
            /// 다만 이미, 최우선 적을 포커싱하고 있는 경우에는 동작하지 않는다.
            /// </summary>
            Counter = 1 << 2,
            
            /// <summary>
            /// 공격을 하는 플래그,
            /// 해당 플래그를 가지지 않는 인공지능은 적을 추격한다고 해도 결국 공격하는 일은 없다.
            /// 비선공/선공 인공지능을 구분하는 플래그
            /// </summary>
            Aggressive = 1 << 3,
            
            /// <summary>
            /// 해당 플래그를 가진다면, 해당 유닛은 플레이어와의 위치관계에 의한 최적화의 일환으로
            /// 비활성화되지 않는다.
            /// </summary>
            NeverSleep = 1 << 4,
            
            /// <summary>
            /// 근처에 적이 없는 경우, 세트되어 있는 PivotPosition으로 인공지능이 돌아가게 하는 플래그
            /// </summary>
            ReturnHome = 1 << 5,
                        
            /// <summary>
            /// 해당 플래그가 활성화 된 경우, 스킬 시전 중에는 스킬을 사용하지 않는다.
            /// </summary>
            RelaxedSpell = 1 << 6,
                        
            /// <summary>
            /// Return Home 플래그를 무시하고, Notive, Trace 거리를 무한으로 설정하는 플래그
            /// </summary>
            JunkYardDog = 1 << 7,
      
            /// <summary>
            /// 인공지능 상태머신을 스스로 갱신하지 않고 외부에서 Order~ 계열 함수를 통해 제어받도록 하는 플래그
            /// </summary>
            RemoteOrder = 1 << 8,
        }


        /// <summary>
        /// 길찾기 유닛이 적을 추적할 때, 추적할 위치를 선정하는 타입
        /// </summary>
        public enum AITracePivotSelectType
        {
            /// <summary>
            /// 기본 타입.
            /// </summary>
            None,
            
            /// <summary>
            /// 적의 위치를 직접 참조하여 길찾기를 수행한다.
            /// </summary>
            TargetCenter,

            /// <summary>
            /// 타겟 유닛이 바라보는 방향으로 길찾기를 수행한다.
            ///
            /// 타겟이 이동한다고 해도, 그 방향쪽으로 길찾기를 수행하므로, DirectionToTarget 같은 버벅거림 없음
            /// </summary>
            TargetForward,
            
            /// <summary>
            /// 적의 위치로부터 해당 길찾기 에이전트 위치까지의 방향벡터를 구하여, 그 방향으로 적의 충돌 반경 + 공격 반큼만큼 멀어진 위치를 참조하여 길찾기를 수행한다.
            ///
            /// 타겟의 바로 뒤를 추적하기 때문에 추적자의 이동속도가 타겟보다 빠르고, 타겟이 계속 움직이는 경우 추적자가 금새 목적지에 도달하나
            /// 목적지도 금새 갱신되기 때문에 추적자의 움직임이 도달=>길찾기 를 반복하게 되어 버벅거림
            /// </summary>
            DirectionTargetToThis,
            
            /// <summary>
            /// 해당 길찾기 에이전트 위치에서 타겟으로의 방향벡터를 구하여, 그 방향으로 적의 충돌 반경 + 공격 반큼만큼 멀어진 위치를 참조하여 길찾기를 수행한다.
            /// </summary>
            DirectionThisToTarget,

            /// <summary>
            /// 적의 위치로부터 적의 충돌 반경 + 공격 반경만큼 랜덤한 방향으로 멀어진 위치를 참조하여 길찾기를 수행한다.
            /// </summary>
            RandomInRadius,
        }

        [Flags]
        public enum AIWanderingType
        {
            /// <summary>
            /// 기본 타입
            /// </summary>
            None = 0,
            
            /// <summary>
            /// 인공지능 방랑이 동작하지 않음
            /// </summary>
            Disable = 1 << 0,
            
            /// <summary>
            /// 인공지능이 방랑할 위치가 해당 유닛의 기점(Origin Pivot)이 아닌 현재 위치를 기준으로 선정됨
            /// </summary>
            WorldWander = 1 << 1,
            
            /// <summary>
            /// 해당 인공지능이 잠듦 상태에서 깨어난 경우 즉시 방랑을 수행함
            /// </summary>
            InstantWanderWhenAwakeAI = 1 << 2,
        }

        public enum AIUnitFindType
        {
            /// <summary>
            /// 적을 찾지 않음. 기본값
            /// </summary>
            None,
            
            /// <summary>
            /// 가장 가까운 위치의 유닛을 선정함
            /// </summary>
            NearestPosition,

            /// <summary>
            /// 가장 먼거리 위치의 유닛을 선정함
            /// </summary>
            FarthestPosition,
            
            /// <summary>
            /// 바라보는 방향과 가장 가까운 방향의 유닛을 선정함
            /// </summary>
            NearestAngle,
            
            /// <summary>
            /// 랜덤한 유닛을 선정함
            /// </summary>
            Random,
            
            /// <summary>
            /// 가장 우선도가 높은 유닛을 선정함
            /// </summary>
            PriorityHigh,
            
            /// <summary>
            /// 가장 우선도가 낮은 유닛을 선정함
            /// </summary>
            PriorityLow,
            
            /// <summary>
            /// 적을 찾지 않음.
            /// </summary>
            UnitNotFind,
/*
            /// <summary>
            /// 스캔번호
            /// </summary>
            ScanNumber,
            
            /// <summary>
            /// 나를 공격 중인 캐릭터
            /// </summary>
            CharUnderAttack,

            /// <summary>
            /// 나와 적대 관계인 캐릭터
            /// </summary>
            EnemyChar,

            /// <summary>
            /// 나를 공격 중인 몬스터
            /// </summary>
            MonsterUnderAttack,

            /// <summary>
            /// 퀘스트 목표
            /// </summary>
            QuestGoal,

            /// <summary>
            /// 선공 몬스터
            /// </summary>
            TargetMonster
            */
        }

        /// <summary>
        /// 유닛 사고머신 상태
        /// </summary>
        public enum AIState
        {
            /// <summary>
            /// 초기화 상태, AIInitializeType 타입을 통해 유닛 초기화 방식을 선정하는 상태
            /// </summary>
            None,
            
            /// <summary>
            /// 대기 상태
            /// </summary>
            Idle,
            
            /// <summary>
            /// 영역 내에 적이 있는 경우의 상태
            /// </summary>
            Notice,
            
            /// <summary>
            /// 영역 내의 적을 추적하는 상태
            /// </summary>
            Trace,
            
            /// <summary>
            /// 공격 중일 때의 상태
            /// </summary>
            Attack,
            
            /// <summary>
            /// 이동 중일 때의 상태
            /// </summary>
            Move,
        }
        
        /// <summary>
        /// AIState 열거형 상수 순환자
        /// </summary>
        public static AIState[] _AIState_Enumerator = SystemTool.GetEnumEnumerator<AIState>(SystemTool.GetEnumeratorType.GetAll);

        /// <summary>
        /// 인공지능 커맨드 예약 타입
        /// </summary>
        [Flags]
        public enum AIReserveCommand
        {
            /// <summary>
            /// 예약 없음. 해당 타입으로 예약 시도 시, 예약을 하지 않음
            /// </summary>
            None = 0,
  
            /// <summary>
            /// 캔슬 가능한 타이밍에 예약되있는 커맨드를 발동시키는 플래그
            /// 해당 플래그가 없다면 예약한 커맨드는 현재 진행중인 스킬이 완전히 종료된 이후에 발동된다.
            /// </summary>
            SpellEntry_Instant = 1 << 0,
              
            /// <summary>
            /// 캔슬 가능한 타이밍에 예약되있는 커맨드를 발동시키는 플래그
            /// 해당 플래그가 없다면 예약한 커맨드는 현재 진행중인 스킬이 완전히 종료된 이후에 발동된다.
            /// 해당 플래그는 1회 사용후 제거된다.
            /// </summary>
            SpellEntry_Instant_OnceFlag = 1 << 1,
            HasInstantEntry = SpellEntry_Instant | SpellEntry_Instant_OnceFlag,
            
            /// <summary>
            /// 시전한 스킬이 재사용가능한 경우, 재사용이 불가능할 때 까지 계속 시전한다.
            /// </summary>
            Success_KeepHoldingCommandIfAvailable = 1 << 2,

            /// <summary>
            /// 시전한 스킬 종료 후 기본 스킬 커맨드를 예약한다.
            /// 해당 플래그는 1회 사용후 제거된다.
            /// </summary>
            Success_TurnToDefaultCommand_OnceFlag = 1 << 3,
            Fail_TurnToDefaultCommand_OnceFlag = 1 << 4,
            TurnToDefaultCommand_OnceFlag = Success_TurnToDefaultCommand_OnceFlag | Fail_TurnToDefaultCommand_OnceFlag,
            
            /// <summary>
            /// 시전한 스킬 종료 후 랜덤 스킬 커맨드를 예약한다.
            /// </summary>
            Success_TurnToRandomAvailable_FallbackDefault = 1 << 5,
            Fail_TurnToRandomAvailable_FallbackDefault = 1 << 6,
            TurnToRandomCommand = Success_TurnToRandomAvailable_FallbackDefault | Fail_TurnToRandomAvailable_FallbackDefault,

            /// <summary>
            /// 시전한 스킬 종료 후 랜덤 스킬 커맨드를 예약한다.
            /// 해당 플래그는 1회 사용후 제거된다.
            /// </summary>
            Success_TurnToRandomAvailable_FallbackDefault_OnceFlag = 1 << 7,
            Fail_TurnToRandomAvailable_FallbackDefault_OnceFlag = 1 << 8,
            TurnToRandomCommand_OnceFlag = Success_TurnToRandomAvailable_FallbackDefault_OnceFlag | Fail_TurnToRandomAvailable_FallbackDefault_OnceFlag,
            
            /// <summary>
            /// 시전한 스킬 종료 후 예약 커맨드를 파기한다.
            /// 해당 플래그는 1회 사용후 제거된다.
            /// </summary>
            Success_TurnToNoneCommand_OnceFlag = 1 << 9,
            Fail_TurnToNoneCommand_OnceFlag = 1 << 10,
            TurnToNoneCommand_OnceFlag = Success_TurnToNoneCommand_OnceFlag | Fail_TurnToNoneCommand_OnceFlag,
        }

        /// <summary>
        /// 커맨드 예약 이벤트 타입
        /// </summary>
        public enum AIReserveHandleType
        {
            TurnToNone,
            TurnToRandomAvailable_FallbackDefault,
            TurnToDefault,
        }

        public static AIReserveCommand[] _AIReserveCommand_Enumerator = SystemTool.GetEnumEnumerator<AIReserveCommand>(SystemTool.GetEnumeratorType.ExceptMaskNone);
        
        #endregion
        
        #region <Struct>

        public struct AIStatePreset
        {
            #region <Consts>

            public static AIStatePreset GetDefaultAIStatePreset()
            {
                return new AIStatePreset(0f, 1f);
            }

            #endregion
            
            #region <Fields>

            public int StateTransitionCount;
            public float Range;
            public float SqrRange;
            public float SpeedRate;
            
            #endregion

            #region <Constructor>

            public AIStatePreset(float p_SpeedRate)
            {
                StateTransitionCount = 0;
                Range = 0f;
                SqrRange = 0f;
                SpeedRate = p_SpeedRate;
            }
            
            public AIStatePreset(float p_Range, float p_SpeedRate)
            {
                StateTransitionCount = 0;
                Range = p_Range;
                SqrRange = Range * Range;
                SpeedRate = p_SpeedRate;
            }

            #endregion

            #region <Methods>

            public void SetRange(float p_Range)
            {
                Range = p_Range;
                SqrRange = Range * Range;
            }

            #endregion
        }

        #endregion
    }
}