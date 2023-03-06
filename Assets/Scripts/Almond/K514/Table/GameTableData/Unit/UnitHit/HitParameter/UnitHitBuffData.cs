using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace k514
{
    public class UnitHitBuffData : GameTable<UnitHitBuffData, int, UnitHitBuffData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public Unit.UnitTimerEventType EventType { get; private set; }
            public UnitHitTool.HitBuffType HitBuffType;
            
            /// <summary>
            /// 버프 발생 확률
            /// </summary>
            public float Possibility;
            
            /// <summary>
            /// 버프 발생 선 딜레이
            /// </summary>
            public uint PreDelay { get; private set; }
            
            /// <summary>
            /// 버프 지속시간
            /// </summary>
            public List<uint> Interval { get; private set; }
            
            /// <summary>
            /// 버프 발생 횟수
            /// </summary>
            public int Count { get; private set; }
            
            /// <summary>
            /// 타격시 증감시킬 단일 기저능력치
            /// </summary>
            /*public (BaseStatusPreset.BaseStatusType, List<float>) HitMonoBaseStatus;
        
            /// <summary>
            /// 타격시 증감시킬 기저능력치
            /// </summary>
            public BaseStatusPreset HitBaseStatus;
            
            /// <summary>
            /// 타격시 증감시킬 단일 전투능력치
            /// </summary>
            public (BattleStatusPreset.BattleStatusType, List<float>) HitMonoBattleStatus;
        
            /// <summary>
            /// 타격시 증감시킬 전투능력치
            /// </summary>
            public BattleStatusPreset HitBattleStatus;

            public override async UniTask OnRecordAdded()
            {
                await base.OnRecordAdded();

                HitBuffType = UnitHitTool.HitBuffType.None;
                
                if (HitMonoBaseStatus.Item1 != BaseStatusPreset.BaseStatusType.None)
                {
                    HitBuffType.AddFlag(UnitHitTool.HitBuffType.MonoBaseStatus);
                }
                if (HitBaseStatus.FlagMask != BaseStatusPreset.BaseStatusType.None)
                {
                    HitBuffType.AddFlag(UnitHitTool.HitBuffType.BaseStatus);
                }
                if (HitMonoBattleStatus.Item1 != BattleStatusPreset.BattleStatusType.None)
                {
                    HitBuffType.AddFlag(UnitHitTool.HitBuffType.MonoBattleStatus);
                }
                if (HitBattleStatus.FlagMask != BattleStatusPreset.BattleStatusType.None)
                {
                    HitBuffType.AddFlag(UnitHitTool.HitBuffType.BattleStatus);
                }
            }
            
            public bool CalcEventOccur(Unit p_CasterUnit, Unit p_TargetUnit)
            {
                var resistValue = p_TargetUnit._BattleStatusPreset.t_Current.GetDebuffResist() * p_TargetUnit._BattleStatusPreset.t_Current.GetDebuffResistMultiplyRate(true);
                return EventType != Unit.UnitTimerEventType.None && HitBuffType != UnitHitTool.HitBuffType.None && Random.value < Possibility * p_CasterUnit._BattleStatusPreset.t_Current.GetDebuffHitMultiplyRate(true) - resistValue;
            }
            
            public uint CalcInterval()
            {
                return (uint) (Random.Range(Interval.First(), Interval.Last()) * 0.001) * 1000;
            }

            public float CalcBattleStatus()
            {
                return Random.Range(HitMonoBattleStatus.Item2.First(), HitMonoBattleStatus.Item2.Last());
            }*/
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        protected override string GetDefaultTableFileName()
        {
            return "UnitHitBuffTable";
        }
    }
}