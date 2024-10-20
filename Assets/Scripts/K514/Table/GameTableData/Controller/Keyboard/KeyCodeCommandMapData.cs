using UnityEngine;

namespace BlackAm
{
    public class KeyCodeCommandMapData : GameTable<KeyCodeCommandMapData, KeyCode, KeyCodeCommandMapData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public ControllerTool.CommandType SoloCommandCode { get; private set; }
        }

        protected override string GetDefaultTableFileName()
        {
            return "KeyCodeCommandMap";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}