using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackAm
{
    public class ShowSelectDialoguePresetData : GameTable<ShowSelectDialoguePresetData, int, ShowSelectDialoguePresetData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public List<int> SelectDialogueInfo { get; protected set; }
        }

        protected override string GetDefaultTableFileName()
        {
            return "ShowSelectDialoguePresetDataTable";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}