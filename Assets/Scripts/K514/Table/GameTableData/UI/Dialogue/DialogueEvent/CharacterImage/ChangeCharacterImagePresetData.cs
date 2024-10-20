using System.Collections.Generic;
using UnityEngine;

namespace BlackAm
{
    public class ChangeCharacterImagePresetData : GameTable<ChangeCharacterImagePresetData, int, ChangeCharacterImagePresetData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            /* 여러가지 슬롯 이미지 한꺼번에 변경 혹은 캐릭터당 지정 후 이미지 변경
             public List<int> Slot { get; protected set; }
            
            public List<int> ImageKey { get; protected set; }*/
            
            public Character Character { get; protected set; }
            
            public int ImageKey { get; protected set; }
            
            public Vector3 ImagePosition { get; protected set; }
            
            public float ImageScale { get; protected set; }
            
            public bool ActionFade { get; protected set; }
            
            public (float, float) FadeTuple { get; protected set; }
        }

        protected override string GetDefaultTableFileName()
        {
            return "ChangeCharacterImagePresetDataTable";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}