using UnityEngine;

namespace BlackAm
{
    public class MoveBackGroundImageLerpPresetData : GameTable<MoveBackGroundImageLerpPresetData, int,
        MoveBackGroundImageLerpPresetData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            public Vector2 StartPosition { get; protected set; } 
            
            public Vector2 EndPosition { get; protected set; }
            
            public float Speed { get; protected set; }
        }
        
        protected override string GetDefaultTableFileName()
        {
            return "MoveBackGroundImageLerpPresetDataTable";
        }
        
        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}