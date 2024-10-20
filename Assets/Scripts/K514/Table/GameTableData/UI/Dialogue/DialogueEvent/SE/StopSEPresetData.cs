using System.Collections.Generic;

namespace BlackAm
{
    /// <summary>
    /// 모션 클립의 지정한 타임 레이트(TimeRate01)에 특정 타입의 콜백 타임스탬프를 추가하는 테이블
    /// </summary>
    public class StopSEPresetData : GameTable<StopSEPresetData, int, StopSEPresetData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            
        }

        protected override string GetDefaultTableFileName()
        {
            return "StopSEPresetDataTable";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}