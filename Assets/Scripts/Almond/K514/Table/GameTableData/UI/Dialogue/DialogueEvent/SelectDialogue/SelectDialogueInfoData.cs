using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace k514
{
    public class SelectDialogueInfoData : GameTable<SelectDialogueInfoData, int, SelectDialogueInfoData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public int DialogueTextKey { get; protected set; }
            
            public int NextDialogueKey { get; protected set; }
            
            public int SelectDialogueConditionKey { get; protected set; }
        }

        protected override string GetDefaultTableFileName()
        {
            return "SelectDialogueInfoDataTable";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}