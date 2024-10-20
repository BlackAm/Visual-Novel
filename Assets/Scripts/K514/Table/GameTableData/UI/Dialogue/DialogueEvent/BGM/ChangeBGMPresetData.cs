using System.Collections.Generic;

namespace BlackAm
{
    public class ChangeBGMPresetData : GameTable<ChangeBGMPresetData, int, ChangeBGMPresetData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public int BGMKey { get; protected set; }
        }

        protected override string GetDefaultTableFileName()
        {
            return "ChangeBGMPresetDataTable";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}