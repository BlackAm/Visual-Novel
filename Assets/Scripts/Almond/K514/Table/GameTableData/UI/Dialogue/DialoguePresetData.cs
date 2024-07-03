using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace k514
{
    public class DialoguePresetData : GameTable<DialoguePresetData, int, DialoguePresetData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public int DialogueKey { get; protected set; }
            
            public int Talker { get; protected set; }
            
            public int DialogueEventKey { get; protected set; }
            
            public int NextDialogueKey { get; protected set; }
            
            public int SkipableInterval { get; protected set; }
            
            public uint PreDelay { get; protected set; }
        }

        protected override string GetDefaultTableFileName()
        {
            return "DialoguePresetDataTable";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}