using System.Collections.Generic;

namespace BlackAm
{
    public class ChangeEventCGPresetData : GameTable<ChangeEventCGPresetData, int, ChangeEventCGPresetData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public int EventCGKey { get; protected set; }
        }

        protected override string GetDefaultTableFileName()
        {
            return "ChangeEventCGPresetDataTable";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}