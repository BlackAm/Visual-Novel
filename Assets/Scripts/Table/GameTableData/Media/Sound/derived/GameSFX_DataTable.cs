#if !SERVER_DRIVE
namespace BlackAm
{
    public class GameSFX_DataTable : SoundDataTable<GameSFX_DataTable, GameSFX_DataTable.TableRecord>
    {
        public class TableRecord : SoundTableRecordBase
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "GameSFX_Table";
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 50000;
            EndIndex = 100000;
        }

        public override SoundDataRoot.SoundFXType GetThisLabelType()
        {
            return SoundDataRoot.SoundFXType.GameSFX;
        }
    }
}
#endif