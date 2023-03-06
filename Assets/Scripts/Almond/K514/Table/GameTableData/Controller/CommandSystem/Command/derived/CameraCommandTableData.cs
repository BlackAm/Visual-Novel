#if !SERVER_DRIVE
namespace k514
{
    public class CameraCommandTableData : CommandTableBase<CameraCommandTableData, CameraCommandTableData.CameraCommandType, CameraCommandTableData.TableRecord>
    {
        public enum CameraCommandType
        {
            RotateView,
            ResetView,
        }

        public class TableRecord : CommandTableRecordBase
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "CameraCommandTable";
        }
    }
}
#endif