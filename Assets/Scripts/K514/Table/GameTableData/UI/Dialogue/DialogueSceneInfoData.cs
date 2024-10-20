using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackAm
{
    public class DialogueSceneInfoData : GameTable<DialogueSceneInfoData, int, DialogueSceneInfoData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public int SceneExtraInfo { get; protected set; }
        }

        protected override string GetDefaultTableFileName()
        {
            return "DialogueSceneInfo";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}