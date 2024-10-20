using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackAm
{
    public class DialoguePlayModePresetData : GameTable<DialoguePlayModePresetData, int , DialoguePlayModePresetData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public List<int> PlayModeList { get; protected set; }
        }

        protected override string GetDefaultTableFileName()
        {
            return "DialoguePlayModePresetDataTable";
        }
        
        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}