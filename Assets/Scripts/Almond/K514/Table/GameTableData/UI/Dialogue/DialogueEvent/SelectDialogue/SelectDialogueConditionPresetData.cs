using System.Collections;
using System.Collections.Generic;
using UI2020;
using UnityEngine;

namespace k514
{
    public class SelectDialogueConditionPresetData : GameTable<SelectDialogueConditionPresetData, int,
        SelectDialogueConditionPresetData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public (DialogueGameManager.SelectDialogueCondition, int) SelectDialogueCondition { get; protected set; }
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