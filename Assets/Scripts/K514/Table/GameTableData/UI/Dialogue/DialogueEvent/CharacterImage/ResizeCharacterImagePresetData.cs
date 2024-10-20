using System.Collections.Generic;

namespace BlackAm
{
    /// <summary>
    /// 모션 클립의 지정한 타임 레이트(TimeRate01)에 특정 타입의 콜백 타임스탬프를 추가하는 테이블
    /// </summary>
    public class ResizeCharacterImagePresetData : GameTable<ResizeCharacterImagePresetData, int, ResizeCharacterImagePresetData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public Character Character { get; protected set; }
            
            public float Scale { get; protected set; }
        }

        protected override string GetDefaultTableFileName()
        {
            return "ResizeCharacterImagePresetDataTable";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}