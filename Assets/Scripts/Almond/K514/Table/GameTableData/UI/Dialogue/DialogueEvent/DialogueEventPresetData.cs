using System.Collections;
using System.Collections.Generic;
using k514;
using UI2020;
using UnityEngine;

namespace UI2020
{
    public class DialogueEventPresetData : GameTable<DialogueEventPresetData, int, DialogueEventPresetData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public Dictionary<DialogueGameManager.DialogueEventFlag, int> DialogueEvent { get; protected set; }
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