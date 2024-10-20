using System.Collections.Generic;

namespace BlackAm
{
    public class HighlightCharacterImagePresetData : GameTable<HighlightCharacterImagePresetData, string, HighlightCharacterImagePresetData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            
        }

        protected override string GetDefaultTableFileName()
        {
            return "HighlightCharacterImagePresetData";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}