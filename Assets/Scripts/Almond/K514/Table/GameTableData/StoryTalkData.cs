using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace k514
{
    public class StoryTalkData : GameTable<StoryTalkData, int, StoryTalkData.Data>
    {
        public class Data : GameTableRecordBase
        {
            public string CharacterName { get; private set; }
            public string CharacterImageIndex { get; private set; }
            public string Quest { get; private set; }
            public int CGIndex { get; private set; }
            public uint Delay { get; private set; }
            public bool CharacterRight { get; private set; }
        }
        
        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        protected override string GetDefaultTableFileName()
        {
            return "StoryTalkTable";
        }
    }
}