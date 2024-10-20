using System.Collections.Generic;

namespace BlackAm
{
    public class ChangeBackGroundImagePresetData : GameTable<ChangeBackGroundImagePresetData, int, ChangeBackGroundImagePresetData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public int ImageKey { get; protected set; }
        }

        protected override string GetDefaultTableFileName()
        {
            return "ChangeBackGroundImagePresetDataTable";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}