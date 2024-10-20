namespace BlackAm
{
    public class ResizeBackGroundImagePresetData : GameTable<ResizeBackGroundImagePresetData, int,
        ResizeBackGroundImagePresetData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public float Size { get; protected set; }
        }

        protected override string GetDefaultTableFileName()
        {
            return "ResizeBackGroundImagePresetDataTable";
        }
        
        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}