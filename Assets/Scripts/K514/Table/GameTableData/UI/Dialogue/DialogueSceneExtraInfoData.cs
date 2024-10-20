using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackAm
{
    public class DialogueSceneExtraInfo : GameTable<DialogueSceneExtraInfo, int, DialogueSceneExtraInfo.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public int SceneNameKey { get; protected set; }
            
            public int ChapterNum { get; protected set; }
            
            public int SceneNum { get; protected set; }
            
            public int ThumbnailKey { get; protected set; }
            
            public int DialogueStartKey { get; protected set; }
        }

        protected override string GetDefaultTableFileName()
        {
            return "DialogueSceneExtraInfo";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}