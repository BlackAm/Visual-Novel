namespace k514
{
    /// <summary>
    /// 퀵 스펠에서 사용할 이펙트나 시구간, 범위 등의 공통 파라미터를 기술하는 테이블 클래스
    /// </summary>
    public class UnitQuickSpellAction : GameTable<UnitQuickSpellAction, int, UnitQuickSpellAction.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        protected override string GetDefaultTableFileName()
        {
            return "UnitQuickSpellActionPresetTable";
        }
    }
}