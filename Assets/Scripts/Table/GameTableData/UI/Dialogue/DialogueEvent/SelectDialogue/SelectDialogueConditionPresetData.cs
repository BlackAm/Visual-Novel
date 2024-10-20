using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackAm
{
    public class SelectDialogueConditionPresetData : GameTable<SelectDialogueConditionPresetData, int,
        SelectDialogueConditionPresetData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public Dictionary<DialogueGameManager.SelectDialogueCondition, int> SelectDialogueCondition { get; protected set; }
        }

        protected override string GetDefaultTableFileName()
        {
            return "SelectDialogueConditionPresetDataTable";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}