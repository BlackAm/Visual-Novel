#if !SERVER_DRIVE
namespace BlackAm
{
    public class LoopVideoDataTable : VideoDataTable<LoopVideoDataTable, LoopVideoDataTable.TableRecord>
    {
        public class TableRecord : VideoTableRecordBase
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "LoopVideoTable";
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 1000;
            EndIndex = 1100;
        }

        public override VideoDataRoot.VideoType GetThisLabelType()
        {
            return VideoDataRoot.VideoType.LoopVideo;
        }
    }
}
#endif