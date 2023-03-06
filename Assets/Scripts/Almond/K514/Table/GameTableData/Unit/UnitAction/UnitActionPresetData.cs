using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace k514
{
    /// <summary>
    /// 유닛의 액션
    /// 기본 공격, 스킬, 연출 등을 기술하는 테이블
    /// </summary>
    public class UnitActionPresetData : GameTable<UnitActionPresetData, int, UnitActionPresetData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            /// <summary>
            /// 해당 유닛 액션의 문자열 정보 인덱스
            /// </summary>
            public int ActionLanguage { get; private set; }

            /// <summary>
            /// 해당 유닛 액션을 기술하는 플래그 마스크
            /// </summary>
            public UnitActionTool.UnitAction.UnitActionFlag UnitActionFlagMask;

            /// <summary>
            /// 해당 유닛 액션이 캔슬될 수 있는 타이밍을 기술하는 프리셋
            /// </summary>
            public UnitActionTool.ActionInterruptPreset ActionInterruptPreset { get; private set; }

            /// <summary>
            /// 해당 유닛 액션이 캔슬할 수 있는 액션 인덱스로 캔슬하려는 유닛 액션이 이미 리스트로 등록되어 있는 경우에는, 굳이 해당 리스트에 추가해줄 필요없다.
            /// </summary>
            public List<int> InterruptableActionIndexGroup { get; private set; }

            /// <summary>
            /// 해당 유닛 액션의 선딜레이
            /// </summary>
            public uint FirstDelay { get; private set; }
            
            /// <summary>
            /// 해당 유닛 액션이 포함하고 있는 순차 모션 리스트
            /// </summary>
            public List<UnitActionTool.UnitAction.MotionSequence> MotionSequences { get; private set; }
            
            /// <summary>
            /// [MotionSequences index, Cue index, DeployPresetPreset]
            /// 해당 유닛 액션이 특정 모션의, 특정 Cue에서 처리할 이벤트 프리셋 컬렉션
            /// </summary>
            public Dictionary<int, Dictionary<int, ObjectDeployEventPreset>> EventPresetMap { get; private set; }
   
            /// <summary>
            /// 해당 유닛 액션의 쿨다운
            /// </summary>
            public float ActionCoolDown { get; private set; }

            /// <summary>
            /// 공격 대상 탐색 타입
            /// </summary>
            public ThinkableTool.AIUnitFindType UnitFindType { get; private set; }

            /// <summary>
            /// '공격 대상의 반경 및 공격자의 반경을 포함한 거리'를 기준으로
            /// 인공지능이 해당 액션을 수행하는 기준 범위
            /// </summary>
            public float AttackRange { get; private set; }

            /// <summary>
            /// 해당 스킬을 발동하는데 소모하는 마나
            /// </summary>
            public float CostMana { get; private set; }
            
            public override async UniTask OnRecordAdded()
            {
                await base.OnRecordAdded();

                if (CostMana > 0f)
                {
                    UnitActionFlagMask.AddFlag(UnitActionTool.UnitAction.UnitActionFlag.CostMana);
                }
                else
                {
                    CostMana = 0f;
                }
            }

            /// <summary>
            /// 테이블에 존재하는 가장 첫번째 이벤트를 호출하는 메서드
            /// </summary>
            public bool TriggerEventPreset(ObjectDeployEventRecord p_ObjectDeployEventRecord)
            {
                if (EventPresetMap != null)
                {
                    var firstCueValue = EventPresetMap.First().Value;
                    if (firstCueValue.CheckGenericCollectionSafe())
                    {
                        var firstEventMap = firstCueValue.First().Value;
                        p_ObjectDeployEventRecord.DeployUnitActionEvent(firstEventMap, default);
                        return true;
                    }
                }

                return false;
            }
            
            /// <summary>
            /// 해당 액션이 [특정 모션/특정 큐]에서 처리할 이벤트를 가지는 경우, 해당 이벤트를 호출하는 메서드
            /// </summary>
            public bool TriggerEventPreset(ObjectDeployEventRecord p_ObjectDeployEventRecord, int p_MotionSequenceIndex, int p_CueIndex)
            {
                if (EventPresetMap != null && EventPresetMap.TryGetValue(p_MotionSequenceIndex, out var o_MotionSequenceRecord))
                {
                    if (o_MotionSequenceRecord.TryGetValue(p_CueIndex, out var o_MotionCueRecord))
                    {
                        p_ObjectDeployEventRecord.DeployUnitActionEvent(o_MotionCueRecord, default);
                        return true;
                    }
                }

                return false;
            }
        }

        protected override string GetDefaultTableFileName()
        {
            return "UnitActionPresetTable";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}