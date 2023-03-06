using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace k514
{
    public class UnitHitTickData : GameTable<UnitHitTickData, int, UnitHitTickData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public Unit.UnitTimerEventType EventType { get; private set; }
            public float Possibility;
            public uint PreDelay { get; private set; }
            public uint Interval { get; private set; }
            public List<int> Count { get; private set; }
            public int HitMessageIndex { get; private set; }
            
            /*public bool CalcEventOccur(Unit p_CasterUnit, Unit p_TargetUnit)
            {
                var resistValue = p_TargetUnit._BattleStatusPreset.t_Current.GetDebuffResist() * p_TargetUnit._BattleStatusPreset.t_Current.GetDebuffResistMultiplyRate(true);
                return EventType != Unit.UnitTimerEventType.None && Random.value < Possibility * p_CasterUnit._BattleStatusPreset.t_Current.GetDebuffHitMultiplyRate(true) - resistValue;
            }*/

            public int CalcCount()
            {
                return Random.Range(Count.First(), Count.Last());
            }
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        protected override string GetDefaultTableFileName()
        {
            return "UnitHitTickTable";
        }
    }
}
