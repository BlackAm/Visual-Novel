using Cysharp.Threading.Tasks;

namespace k514
{
    public class UnitInfoPresetData : GameTable<UnitInfoPresetData, int, UnitInfoPresetData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public int UnitNameId { get; private set; }
            public int BaseStatusId { get; private set; }
            public int AdditiveBaseStatusId { get; private set; }
            public int BattleStatusId { get; private set; }
            public int SpecialStatusId { get; private set; }
            public int UnitForceId { get; private set; }
        }

        public static async UniTask<TableRecord> GetSystemValueRecord()
        {
            return (await GetInstance()).GetTableData(0);
        }

        protected override string GetDefaultTableFileName()
        {
            return "UnitInfoPresetTable";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}