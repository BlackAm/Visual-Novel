#if !SERVER_DRIVE
namespace BlackAm
{
    public class BGM_DataTable : SoundDataTable<BGM_DataTable, BGM_DataTable.TableRecord>
    {
        public class TableRecord : SoundTableRecordBase
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "BGM_Table";
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 0;
            EndIndex = 50000;
        }

        public override SoundDataRoot.SoundFXType GetThisLabelType()
        {
            return SoundDataRoot.SoundFXType.BGM;
        }
    }
}
#endif