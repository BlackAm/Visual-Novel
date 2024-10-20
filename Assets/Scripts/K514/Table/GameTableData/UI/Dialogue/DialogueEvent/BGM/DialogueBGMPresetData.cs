using System.Collections.Generic;

namespace BlackAm
{
    public class DialogueBGMPresetData : GameTable<DialogueBGMPresetData, int, DialogueBGMPresetData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public Dictionary<DialogueGameManager.DialogueEventBGM, int> BGMEvent { get; protected set; }
        }

        protected override string GetDefaultTableFileName()
        {
            return "DialogueBGMPresetDataTable";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}