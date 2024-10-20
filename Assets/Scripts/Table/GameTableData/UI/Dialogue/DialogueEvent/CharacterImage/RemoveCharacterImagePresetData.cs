using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackAm
{
    public class RemoveCharacterImagePresetData : GameTable<RemoveCharacterImagePresetData, int ,RemoveCharacterImagePresetData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public Character Character { get; protected set; }
        }

        protected override string GetDefaultTableFileName()
        {
            return "RemoveCharacterImagePresetData";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}
