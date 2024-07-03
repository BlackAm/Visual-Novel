using System.Collections.Generic;

namespace k514
{
    /// <summary>
    /// 모션 클립의 지정한 타임 레이트(TimeRate01)에 특정 타입의 콜백 타임스탬프를 추가하는 테이블
    /// </summary>
    public class ChangeCharacterImagePresetData : GameTable<ChangeCharacterImagePresetData, string, ChangeCharacterImagePresetData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            /* 여러가지 슬롯 이미지 한꺼번에 변경 혹은 캐릭터당 지정 후 이미지 변경
             public List<int> Slot { get; protected set; }
            
            public List<int> ImageKey { get; protected set; }*/
            
            public Character Character { get; protected set; }
            
            public int ImageKey { get; protected set; }
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