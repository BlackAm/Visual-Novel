using System.Collections.Generic;

namespace k514
{
    /// <summary>
    /// 모션 클립의 지정한 타임 레이트(TimeRate01)에 특정 타입의 콜백 타임스탬프를 추가하는 테이블
    /// </summary>
    public class ExpandCharacterImagePresetData : GameTable<ExpandCharacterImagePresetData, string, ExpandCharacterImagePresetData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public List<int> Slot { get; protected set; }
            
            public float Size { get; protected set; }
        }

        protected override string GetDefaultTableFileName()
        {
            return "ReduceCharacterImagePresetDataTable";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}