using System.Collections;
using System.Collections.Generic;
using UI2020;
using UnityEngine;

namespace k514
{
    public class SelectDialogueConditionLikingPresetData : GameTable<SelectDialogueConditionLikingPresetData, int, SelectDialogueConditionLikingPresetData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public DialogueGameManager.SelectDialogueLikingCondition SelectDialogueLikingCondition { get; protected set; }
            
            public Dictionary<Character, int> LikingCollection { get; protected set; }
            
            public DialogueGameManager.LikingExtraType LikingExtraType { get; protected set; }
        }

        protected override string GetDefaultTableFileName()
        {
            return "SelectDialogueConditionLikingPresetDataTable";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}