﻿using System.Collections.Generic;
using UI2020;

namespace k514
{
    /// <summary>
    /// 모션 클립의 지정한 타임 레이트(TimeRate01)에 특정 타입의 콜백 타임스탬프를 추가하는 테이블
    /// </summary>
    public class EventCGPresetData : GameTable<EventCGPresetData, int, EventCGPresetData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public Dictionary<DialogueGameManager.DialogueEventCG, int> EvebtCGEvent { get; protected set; }
        }

        protected override string GetDefaultTableFileName()
        {
            return "EventCGPresetDataTable";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}