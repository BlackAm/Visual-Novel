using System.Collections.Generic;

namespace k514
{
    /// <summary>
    /// 모션 클립의 지정한 타임 레이트(TimeRate01)에 특정 타입의 콜백 타임스탬프를 추가하는 테이블
    /// </summary>
    public class PlayBGMPresetData : GameTable<PlayBGMPresetData, int, PlayBGMPresetData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public int BGMKey { get; protected set; }
        }

        protected override string GetDefaultTableFileName()
        {
            return "PlayBGMPresetDataTable";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}