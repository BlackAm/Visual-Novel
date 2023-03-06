using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace k514
{
    public static partial class ActableTool
    {
        #region <Consts>

        public const float __DASH_SPEED_RATE = 1f;

        #endregion

        #region <Enums>

        public enum UnitActionType
        {
            None,
            Move,
            Jump,
            ActSpell,
        }
        
        public enum WalkState
        {
            Hold,
            Walk,
            Run
        }

        public enum IdleState
        {
            Relax,
            Combat,
            Groggy
        }

        [Flags]
        public enum UnitActionClusterFlag
        {
            None = 0,
            
            /// <summary>
            /// 해당 플래그를 가지는 경우, Actable 모듈이 참조하고 있는 액션 클러스터 변경시
            /// 참조 인덱스를 유지하게 된다.
            /// 
            /// 예를 들어, A커맨드 3번째 스킬을 사용하다가 B커맨드로 캔슬시 B커맨드의 3번째 스킬이 있다면
            /// 해당 스킬을 사용하게 된다.
            ///
            /// 없는 경우, 0번째 스킬부터 시전한다.
            ///
            /// 액션 클러스터가 변경되지 않는 경우에는 UnitActionClusterSequenceType를 따른다.
            /// </summary>
            KeepIndex = 1 << 0,
        }
        
        public enum UnitActionClusterSequenceType
        {
            /// <summary>
            /// 다음 인덱스의 액션을 수행한다.
            /// </summary>
            Next,

            /// <summary>
            /// 다음 인덱스의 액션을 수행한다.
            /// 인덱스가 스킬 리스트를 전부 참조한 경우, 0으로 초기화하여 루프시킨다.
            /// </summary>
            NextLoop,
            
            /// <summary>
            /// 지정한 인덱스 차례가되면 지정한 횟수만큼 구간 반복한다.
            /// </summary>
            Repeat,
            
            /// <summary>
            /// 지정한 인덱스 구간 사이에서 지정한 횟수만큼 랜덤으로 액션을 하나 수행한다.
            /// </summary>
            Random,
        }

        #endregion

        #region <Constructor>

        static ActableTool()
        {
            /*_UnitPhaseTransitionConditionTypeEnumerator =
                SystemTool.GetEnumEnumerator<UnitPhaseTransitionConditionType>(SystemTool.GetEnumeratorType.ExceptNone);*/
        }

        #endregion
        
        #region <Struct>

        /// <summary>
        /// 다수의 UnitAction을 가지는 프리셋
        /// </summary>
        public struct UnitActionCluster
        {
            #region <Fields>

            /// <summary>
            /// 유닛 액션 리스트
            /// </summary>
            public List<UnitActionTool.UnitAction> ActionList;
            
            /// <summary>
            /// 플래그 마스크
            /// </summary>
            private UnitActionClusterFlag UnitActionClusterFlagMask;

            /// <summary>
            /// 다음 액션 선정 타입
            /// </summary>
            private UnitActionClusterSequenceType UnitActionClusterSequenceType;

            /// <summary>
            /// 액션 리스트 인덱스 구간
            /// </summary>
            private (int t_Start, int t_End) IndexInterval;

            /// <summary>
            /// 액션 리스트 반복횟수
            /// </summary>
            private int RepeatCount;

            /// <summary>
            /// 현재 참조중인 액션 인덱스
            /// </summary>
            public int CurrentIndex;
            
            /// <summary>
            /// 현재 반복횟수
            /// </summary>
            private int CurrentRepeatCount;

            /// 액션 인덱스
            private int ActionIndex;

            /// <summary>
            /// 액션 갯수
            /// </summary>
            public int CurrentActionCount;
            
            #endregion

            #region <Constructors>

            public UnitActionCluster(int p_ActionIndex, UnitActionClusterSequenceType p_UnitActionClusterSequenceType = UnitActionClusterSequenceType.Next,
                int p_Start = 0, int p_End = 0, int p_RepeatCount = 0, UnitActionClusterFlag p_FlagMask = UnitActionClusterFlag.None)
            {
                ActionList = new List<UnitActionTool.UnitAction>();
                UnitActionClusterSequenceType = p_UnitActionClusterSequenceType;
                IndexInterval = (p_Start, p_End);
                RepeatCount = p_RepeatCount;
                CurrentIndex = CurrentRepeatCount = default;
                UnitActionClusterFlagMask = p_FlagMask;

                ActionIndex = p_ActionIndex;
                var tryAction = UnitActionStorage.GetInstance[p_ActionIndex];
                ActionList.Add(tryAction);
                CurrentActionCount = ActionList.Count;
                OnInitCluster();
            }
            
            public UnitActionCluster(List<int> p_ActionIndexList, UnitActionClusterSequenceType p_UnitActionClusterSequenceType = UnitActionClusterSequenceType.Next,
                int p_Start = 0, int p_End = 0, int p_RepeatCount = 0, UnitActionClusterFlag p_FlagMask = UnitActionClusterFlag.None)
            {
                ActionList = new List<UnitActionTool.UnitAction>();
                UnitActionClusterSequenceType = p_UnitActionClusterSequenceType;
                IndexInterval = (p_Start, p_End);
                RepeatCount = p_RepeatCount;
                CurrentIndex = CurrentRepeatCount = default;
                UnitActionClusterFlagMask = p_FlagMask;

                ActionIndex = default;
                foreach (var actionIndex in p_ActionIndexList)
                {
                    var tryAction = UnitActionStorage.GetInstance[actionIndex];
                    ActionList.Add(tryAction);
                }
                CurrentActionCount = ActionList.Count;
                OnInitCluster();
            }

            #endregion

            #region <Callbacks>

            public void OnInitCluster()
            {
                switch (UnitActionClusterSequenceType)
                {
                    case UnitActionClusterSequenceType.Random:
                        CurrentRepeatCount = 0;
                        CurrentIndex = Random.Range(IndexInterval.t_Start, IndexInterval.t_End + 1);
                        break;
                    default:
                    case UnitActionClusterSequenceType.Next:
                    case UnitActionClusterSequenceType.NextLoop:
                    case UnitActionClusterSequenceType.Repeat:
                        CurrentIndex = CurrentRepeatCount = 0;
                        break;
                }
            }
            
            private void OnInitCluster(int p_PrevIndex)
            {
                switch (UnitActionClusterSequenceType)
                {
                    case UnitActionClusterSequenceType.Random:
                    case UnitActionClusterSequenceType.Repeat:
                        break;
                    default:
                    case UnitActionClusterSequenceType.Next:
                    case UnitActionClusterSequenceType.NextLoop:
                        CurrentIndex = GetTransitionIndex(p_PrevIndex);
                        break;
                }
            }

            #endregion
            
            #region <Methods>

            public int GetActionIndex() => ActionIndex;
            
            public int GetTransitionIndex()
            {
                return GetTransitionIndex(CurrentIndex);
            }
            
            public int GetTransitionIndex(int p_PrevIndex)
            {
                if (p_PrevIndex > 0)
                {
                    if (UnitActionClusterFlagMask.HasAnyFlagExceptNone(UnitActionClusterFlag.KeepIndex))
                    {
                        return p_PrevIndex;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return CurrentIndex;
                }
            }

            public void AddAction(UnitActionTool.UnitAction p_UnitAction)
            {
                ActionList.Add(p_UnitAction);
                CurrentActionCount++;
            }

            public void AddAction(int p_UnitActionIndex)
            {
                AddAction(UnitActionStorage.GetInstance[p_UnitActionIndex]);
            }
            
            public void AddAction(List<int> p_UnitActionIndexList)
            {
                foreach (var index in p_UnitActionIndexList)
                {
                    AddAction(index);
                }
            }
            
            public void AddAction(List<UnitActionTool.UnitAction> p_UnitActionList)
            {
                ActionList.AddRange(p_UnitActionList);
                CurrentActionCount = ActionList.Count;
            }

            public (bool, int) UpdateActionIndex(int p_PrevIndex)
            {
                if (CurrentActionCount > 0)
                {
                    // 이전 액션 클러스터로부터 인덱스를 계승받는 경우
                    OnInitCluster(p_PrevIndex);

                    switch (UnitActionClusterSequenceType)
                    {
                        case UnitActionClusterSequenceType.Repeat:
                            if (CurrentIndex == IndexInterval.t_End && CurrentRepeatCount < RepeatCount)
                            {
                                CurrentRepeatCount++;
                                CurrentIndex = IndexInterval.t_Start;
                                return (true, CurrentIndex);
                            }
                            else
                            {
                                goto case UnitActionClusterSequenceType.Next;
                            }
                        case UnitActionClusterSequenceType.Random:
                            if (CurrentRepeatCount < RepeatCount)
                            {
                                CurrentRepeatCount++;
                                CurrentIndex = Random.Range(IndexInterval.t_Start, IndexInterval.t_End + 1);
                                return (true, CurrentIndex);
                            }
                            else
                            {
                                return default;
                            }
                        case UnitActionClusterSequenceType.NextLoop:
                            if (CurrentIndex > CurrentActionCount - 2)
                            {
                                var result = (true, CurrentIndex);
                                CurrentIndex = 0;
                                return result;
                            }
                            else
                            {
                                return (true, CurrentIndex++);
                            }
                        default:
                        case UnitActionClusterSequenceType.Next:
                            if (CurrentIndex < CurrentActionCount)
                            {
                                return (true, CurrentIndex++);
                            }
                            else
                            {
                                return default;
                            }
                    }
                }
                else
                {
                    return default;
                }
            }

            #endregion
        }
        
        /// <summary>
        /// Actable 오브젝트가 유닛 액션을 제어할 때 사용하는 프리셋
        /// </summary>
        public class UnitActionSpellPreset
        {
            #region <Fields>

            public UnitActionCluster UnitActionCluster;
            public ProgressTimerWrap ActionCooldownTimer;

            #endregion

            #region <Constructors>
            
            public UnitActionSpellPreset(int p_UnitActionIndex) : this(new UnitActionCluster(p_UnitActionIndex))
            {
            }

            public UnitActionSpellPreset(List<int> p_UnitActionIndexList) : this(new UnitActionCluster(p_UnitActionIndexList))
            {
            }

            public UnitActionSpellPreset(UnitActionCluster p_UnitActionCluster)
            {
                UnitActionCluster = p_UnitActionCluster;
                ActionCooldownTimer = new ProgressTimerWrap(0f);

                foreach (var unitAction in UnitActionCluster.ActionList)
                {
                    OnActionEntry(unitAction);
                }
            }

            #endregion

            #region <Callbacks>

            private void OnActionEntry(int p_ActionIndex)
            {
                UnitActionStorage.GetInstance.OnActionBind(p_ActionIndex);
            }
            
            private void OnActionEntry(UnitActionTool.UnitAction p_UnitAction)
            {
                UnitActionStorage.GetInstance.OnActionBind(p_UnitAction);
            }

            public void OnInitActionCluster()
            {
                UnitActionCluster.OnInitCluster();
            }

            #endregion
            
            #region <Methods>

            public void AddAction(int p_UnitActionIndex)
            {
                UnitActionCluster.AddAction(p_UnitActionIndex);
                OnActionEntry(p_UnitActionIndex);
            }

            public void AddAction(List<int> p_UnitActionIndexList)
            {
                foreach (var index in p_UnitActionIndexList)
                {
                    AddAction(index);
                }
            }
            
            public void AddAction(UnitActionTool.UnitAction p_UnitAction)
            {
                UnitActionCluster.AddAction(p_UnitAction);
                OnActionEntry(p_UnitAction);
            }

            public void AddAction(List<UnitActionTool.UnitAction> p_UnitActionList)
            {
                foreach (var action in p_UnitActionList)
                {
                    AddAction(action);
                }
            }
            
            public void OverlapActionCluster(UnitActionCluster p_UnitActionCluster)
            {
                UnitActionCluster = p_UnitActionCluster;
                foreach (var unitAction in p_UnitActionCluster.ActionList)
                {
                    OnActionEntry(unitAction);
                }
            }
                        
            public int GetActionCount() => UnitActionCluster.CurrentActionCount;
            public int GetCurrentIndex() => UnitActionCluster.CurrentIndex;
            public bool HasAction() => UnitActionCluster.CurrentActionCount > 0;
            public (bool, UnitActionTool.UnitAction) GetPrimeAction() => GetAction(0);
            public (bool, UnitActionTool.UnitAction) GetTransitionAction() => GetAction(UnitActionCluster.GetTransitionIndex());
            
            public (bool, UnitActionTool.UnitAction) GetAction(int p_ActionIndex)
            {
                if (UnitActionCluster.CurrentActionCount > p_ActionIndex)
                {
                    return (true, UnitActionCluster.ActionList[p_ActionIndex]);
                }
                else
                {
                    return default;
                }
            }

            public (bool, UnitActionTool.UnitAction) GetEntryAction()
            {
                OnInitActionCluster();
                return GetUpdateAction(0);
            }
            
            public (bool, UnitActionTool.UnitAction) GetUpdateAction(int p_PrevActionIndex)
            {
                var (valid, actionIndex) = UnitActionCluster.UpdateActionIndex(p_PrevActionIndex);
                if (valid)
                {
                    return (true, UnitActionCluster.ActionList[actionIndex]);
                }
                else
                {
                    return default;
                }
            }

            #endregion
        }
        
        #endregion
    }
}