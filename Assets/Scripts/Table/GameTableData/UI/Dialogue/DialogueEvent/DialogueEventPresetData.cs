using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackAm
{
    public class DialogueEventPresetData : GameTable<DialogueEventPresetData, int, DialogueEventPresetData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public Dictionary<DialogueGameManager.DialogueEvent, int> DialogueEvent { get; protected set; }
        }

        protected override string GetDefaultTableFileName()
        {
            return "DialogueEventPresetDataTable";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}