﻿using System.Collections.Generic;

namespace BlackAm
{
    /// <summary>
    /// 모션 클립의 지정한 타임 레이트(TimeRate01)에 특정 타입의 콜백 타임스탬프를 추가하는 테이블
    /// </summary>
    public class DialogueFadeOutPresetData : GameTable<DialogueFadeOutPresetData, int, DialogueFadeOutPresetData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public float PreDelay { get; protected set; }
            
            public float FadeTime { get; protected set; }
        }

        protected override string GetDefaultTableFileName()
        {
            return "DialogueFadeOutPresetDataTable";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}