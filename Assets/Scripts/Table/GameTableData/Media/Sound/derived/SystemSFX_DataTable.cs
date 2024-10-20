#if !SERVER_DRIVE
namespace BlackAm
{
    public class SystemSFX_DataTable : SoundDataTable<SystemSFX_DataTable, SystemSFX_DataTable.TableRecord>
    {
        public class TableRecord : SoundTableRecordBase
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "SystemSFX_Table";
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 100000;
            EndIndex = 150000;
        }

        public override SoundDataRoot.SoundFXType GetThisLabelType()
        {
            return SoundDataRoot.SoundFXType.SystemSFX;
        }
    }
}
#endif