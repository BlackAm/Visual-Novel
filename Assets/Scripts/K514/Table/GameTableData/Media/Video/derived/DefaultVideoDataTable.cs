#if !SERVER_DRIVE
namespace BlackAm
{
    public class DefaultVideoDataTable : VideoDataTable<DefaultVideoDataTable, DefaultVideoDataTable.TableRecord>
    {
        public class TableRecord : VideoTableRecordBase
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "DefaultVideoTable";
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 0;
            EndIndex = 100;
        }

        public override VideoDataRoot.VideoType GetThisLabelType()
        {
            return VideoDataRoot.VideoType.Default;
        }
    }
}
#endif