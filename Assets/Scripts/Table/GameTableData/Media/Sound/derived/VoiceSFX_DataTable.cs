#if !SERVER_DRIVE
namespace BlackAm
{
    public class VoiceSFX_DataTable : SoundDataTable<VoiceSFX_DataTable, VoiceSFX_DataTable.TableRecord>
    {
        public class TableRecord : SoundTableRecordBase
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "VoiceSFX_Table";
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 150000;
            EndIndex = 200000;
        }

        public override SoundDataRoot.SoundFXType GetThisLabelType()
        {
            return SoundDataRoot.SoundFXType.VoiceSFX;
        }
    }
}
#endif